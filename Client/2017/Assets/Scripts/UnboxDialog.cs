using System;
using System.Collections.Generic;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class UnboxDialog : MonoBehaviour
{
	private void Awake()
	{
		if (this._scroll)
		{
			return;
		}
		this._scroll = base.GetComponentInChildren<UIScrollView>(true);
	}

	private void Update()
	{
		if (this._spin)
		{
			this.offsetX = this.container.clipOffset.x;
			this._scroll.MoveRelative(new Vector3(-Time.deltaTime * this.speed, 0f));
			foreach (object obj in this.spinner.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.localPosition.x + 4f * this.w < this.offsetX)
				{
					transform.localPosition = new Vector3(transform.localPosition.x + (float)this.spinner.transform.childCount * this.w, 0f, 0f);
				}
			}
			if (this.placeToStop != null)
			{
				if (this.speed > this.initialSpeed * 0.5f)
				{
					this.speed -= Time.deltaTime * this.slowDown;
					return;
				}
				Vector3[] worldCorners = this.container.worldCorners;
				Vector3 position = (worldCorners[2] + worldCorners[0]) * 0.5f;
				Transform cachedTransform = this.container.cachedTransform;
				Vector3 a = cachedTransform.InverseTransformPoint(this.placeToStop.position);
				Vector3 b = cachedTransform.InverseTransformPoint(position);
				float x = (a - b).x;
				if (this.speed <= this.initialSpeed * 0.5f && Mathf.Abs(x) < Time.deltaTime * this.speed)
				{
					this.speed = 0f;
					if (this._gift.Type == 5)
					{
						this.whatlabel.text = Localize.weaponNames[WeaponSkins.weaponId(this._gift.Num)];
					}
					if (this._gift.Type == 4)
					{
						this.whatlabel.text = Localize.weaponNames[this._gift.Num];
					}
					if (this._gift.Type == 3)
					{
						this.whatlabel.text = Localize.gameItemsNames[this._gift.Num];
					}
					this._spin = false;
					base.Invoke("ShowGift", 0.5f);
				}
			}
		}
		else if (this._scroll.isDragging)
		{
			float x2 = this._scroll.panel.clipOffset.x;
			float num = this._dragOffsetX - x2;
			this._dragOffsetX = x2;
			foreach (object obj2 in this.spinner.transform)
			{
				Transform transform2 = (Transform)obj2;
				if (num < 0f && transform2.localPosition.x + 4f * this.w < x2)
				{
					transform2.localPosition = new Vector3(transform2.localPosition.x + (float)this.spinner.transform.childCount * this.w, 0f, 0f);
				}
				if (num > 0f && transform2.localPosition.x - 8f * this.w > x2)
				{
					transform2.localPosition = new Vector3(transform2.localPosition.x - (float)this.spinner.transform.childCount * this.w, 0f, 0f);
				}
			}
		}
	}

	private void ShowGift()
	{
		CaseGiftDialog caseGiftDialog = Cub2UI.FindAndOpenDialog<CaseGiftDialog>("dialog_casegift");
		if (this._box.Type == 3)
		{
			caseGiftDialog.fi = this._box;
		}
		else
		{
			caseGiftDialog.fi = this._gift;
		}
		caseGiftDialog.Open(this._box.Type == 3);
		base.gameObject.SetActive(false);
	}

	public void OnEnable()
	{
		this._spin = false;
		KGUITools.removeAllChildren(this.container.gameObject, true);
		this.takeBtn.isEnabled = true;
		this.Invalidate();
	}

	private FastInventar[] selectGameItems()
	{
		if (this.caseType == CaseType.video)
		{
			return UnboxBox.selectVideoGameItems();
		}
		return UnboxBox.selectCaseGameItems(this.caseType);
	}

	protected void SelectItemsForMenu()
	{
		this.listNums = this.selectGameItems();
		KGUITools.removeAllChildren(this.container.gameObject, true);
		this.spinner = new GameObject("spin");
		for (int i = 0; i < this.listNums.Length; i++)
		{
			GameObject gameObject = this.spinner.AddChild(this.itemPrefab);
			gameObject.name = this.itemPrefab.name + i;
			if (this.w == 0f)
			{
				this.w = NGUIMath.CalculateRelativeWidgetBounds(gameObject.transform, false).size.x;
			}
			ItemDescIcon component = gameObject.GetComponent<ItemDescIcon>();
			component.fi = this.listNums[i];
			component.countLabel.enabled = false;
			gameObject.transform.localPosition = new Vector3((float)i * this.w, 0f, 0f);
		}
		this.spinner.transform.parent = this.container.gameObject.transform;
		this.spinner.transform.localScale = Vector3.one;
		this.offsetX = this.w * 3.5f;
		this.spinner.transform.localPosition = new Vector3(-this.offsetX, 0f, 0f);
	}

	public void Invalidate()
	{
		if (Kube.SS != null)
		{
			Kube.RM.require("Assets2_MenuItems", null);
		}
		this.SelectItemsForMenu();
	}

	private int legacyType(int p)
	{
		switch (p)
		{
		case 3:
			return 0;
		case 4:
			return 1;
		case 5:
			return 2;
		default:
			return 0;
		}
	}

	public void onClick()
	{
		this._scroll.enabled = false;
		this.takeBtn.isEnabled = false;
		this._spin = true;
		this.speed = this.initialSpeed;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["box_id"] = this._box_id.ToString();
		if (this.caseType == CaseType.video)
		{
			dictionary["si"] = this.FIJoin(this.listNums);
		}
		Kube.SS.Request(905, dictionary, new ServerCallback(this.onUnbox));
	}

	private string FIJoin(FastInventar[] listNums)
	{
		string text = string.Empty;
		for (int i = 0; i < listNums.Length; i++)
		{
			if (i > 0)
			{
				text += ";";
			}
			text += listNums[i].ToString();
		}
		return text;
	}

	protected void onUnbox(string response)
	{
		UnityEngine.Debug.Log(response);
		if (response.StartsWith("error"))
		{
			return;
		}
		string b = response.Substring(response.Length - 32);
		string text = response.Substring(0, response.Length - 32);
		if (AuxFunc.GetMD5(text + Kube.SS.phpSecret) != b)
		{
			Kube.Ban();
			return;
		}
		GameParamsScript.InventarItems inventarItems2;
		GameParamsScript.InventarItems inventarItems = inventarItems2 = Kube.GPS.inventarItems;
		int num;
		int index = num = this._box_id;
		num = inventarItems2[num];
		inventarItems[index] = num - 1;
		JsonData jsonData = JsonMapper.ToObject(text);
		this.bonus = MissionHelper.parseBonusFI(jsonData["arr"].ToString());
		FastInventar b2 = default(FastInventar);
		using (Dictionary<FastInventar, int>.Enumerator enumerator = this.bonus.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				KeyValuePair<FastInventar, int> keyValuePair = enumerator.Current;
				this._gift = keyValuePair.Key;
				b2 = keyValuePair.Key;
			}
		}
		int index2 = 0;
		for (int i = 0; i < this.listNums.Length; i++)
		{
			if (this.listNums[i] == b2)
			{
				index2 = i;
				break;
			}
		}
		if (b2.Type == 3)
		{
			GameParamsScript.InventarItems inventarItems4;
			GameParamsScript.InventarItems inventarItems3 = inventarItems4 = Kube.GPS.inventarItems;
			int index3 = num = b2.Num;
			num = inventarItems4[num];
			inventarItems3[index3] = num + 1;
		}
		else if (b2.Type == 7)
		{
			Kube.GPS.inventarSpecItems[b2.Num] = DataUtils.TimeAdd(Kube.GPS.inventarSpecItems[b2.Num], 86400);
		}
		else if (b2.Type == 4)
		{
			Kube.GPS.inventarWeapons[b2.Num] = 1;
		}
		else if (b2.Type == 5)
		{
			Kube.GPS.weaponsSkin[b2.Num] = 1;
		}
		else if (b2.Type == 9)
		{
			Kube.GPS.playerIsClothes[b2.Num] = 1;
		}
		else if (b2.Type == 8)
		{
			Kube.GPS.playerSkins[b2.Num] = 1;
		}
		this.placeToStop = this.spinner.transform.GetChild(index2);
		if (jsonData.Keys.Contains("box"))
		{
			this._box = FastInventar.Parse(jsonData["box"].ToString());
			GameParamsScript.InventarItems inventarItems6;
			GameParamsScript.InventarItems inventarItems5 = inventarItems6 = Kube.GPS.inventarItems;
			int index4 = num = this._box.Num;
			num = inventarItems6[num];
			inventarItems5[index4] = num + 1;
		}
	}

	private void ItemsCubesUpdate()
	{
		BoxDescObj boxDescObj = Kube.IS.itemDesc[this._box_id] as BoxDescObj;
		int num;
		if (Kube.GPS.inventarItems[this._box_id] > 0)
		{
			num = 0;
		}
		else if (boxDescObj && boxDescObj.video && DateTime.UtcNow > Kube.GPS.nextUnbox)
		{
			num = 1;
		}
		else
		{
			num = 2;
			PriceButton component = this.buyBtn.GetComponent<PriceButton>();
			component.value = Kube.GPS.inventarItemPrice2[this._box_id];
			component.isGold = true;
		}
		this.buyBtn.isEnabled = true;
		GameObject[] array = new GameObject[]
		{
			this.takeBtn.gameObject,
			this.videoBtn.gameObject,
			this.buyBtn.gameObject
		};
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(i == num);
		}
	}

	public void onBuy()
	{
		if (!Kube.IS.canPay(new FastInventar(InventarType.items, this._box_id)))
		{
			MainMenu.ShowBank();
			return;
		}
		this.buyBtn.isEnabled = false;
		Kube.SS.BuyItem(this._box_id, 1, Kube.IS.gameObject, "BuyItemDone");
	}

	private void VideoAdCb(string ans)
	{
		string[] array = ans.Split(new char[]
		{
			'^'
		});
		if (array[0] == "1")
		{
			GameParamsScript.InventarItems inventarItems2;
			GameParamsScript.InventarItems inventarItems = inventarItems2 = Kube.GPS.inventarItems;
			int num;
			int index = num = this._box_id;
			num = inventarItems2[num];
			inventarItems[index] = num + 1;
			this.takeBtn.gameObject.SetActive(true);
			int num2 = 4;
			if (array.Length > 1)
			{
				Kube.GPS.nextUnbox = DataUtils.UnitToDatetime(TryConvert.ToInt32(array[1], 0));
			}
			else
			{
				Kube.GPS.nextUnbox = DateTime.UtcNow + TimeSpan.FromHours((double)num2);
			}
		}
	}

	public void onVideo()
	{
		InviteDialog inviteDialog = Cub2UI.FindAndOpenDialog<InviteDialog>("dialog_invite");
		inviteDialog.handler = delegate()
		{
			Kube.SN.ShowGift(InviteDialog.current.uid);
		};
	}

	public void Start()
	{
		Kube.RM.require("Assets2", null);
		Kube.RM.requireByTag("Menu");
	}

	public void Open(int p)
	{
		this.Awake();
		this._scroll.enabled = true;
		this._scroll.ResetPosition();
		this._box = new FastInventar(-1, 0);
		this._box_id = p;
		this.title.text = Localize.gameItemsNames[this._box_id];
		this.desc.text = Localize.gameItemsDesc[this._box_id];
		this.caseType = Kube.IS.itemDesc[this._box_id].caseType;
		base.gameObject.SetActive(true);
		this.whatlabel.text = string.Empty;
		this.boxico.mainTexture = Kube.ASS2.gameItemsTex[this._box_id];
		this.ItemsCubesUpdate();
		this._scroll.ResetPosition();
	}

	public GameObject itemPrefab;

	public UILabel title;

	public UILabel desc;

	public UITexture boxico;

	public UIButton takeBtn;

	public UIButton buyBtn;

	public UIButton videoBtn;

	public UILabel whatlabel;

	public UIPanel container;

	protected UIScrollView _scroll;

	protected float offsetX;

	protected bool _spin;

	protected float w;

	private float speed = 1f;

	public float initialSpeed = 600f;

	private float slowDown = 100f;

	protected GameObject spinner;

	protected float _dragOffsetX;

	private Transform placeToStop;

	private FastInventar _gift;

	private FastInventar _box = new FastInventar(-1, 0);

	protected Dictionary<FastInventar, int> bonus;

	[NonSerialized]
	public CaseType caseType;

	private FastInventar[] listNums;

	protected int _box_id = -1;
}
