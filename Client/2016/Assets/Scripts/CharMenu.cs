using System;
using kube;
using kube.game;
using UnityEngine;

public class CharMenu : MonoBehaviour
{
	protected string tempClothesStr
	{
		get
		{
			string text = string.Empty;
			for (int i = 0; i < this.tempClothes.Length; i++)
			{
				if (text.Length != 0)
				{
					text += ";";
				}
				text = text + string.Empty + this.tempClothes[i];
			}
			return text;
		}
		set
		{
		}
	}

	private void Start()
	{
		if (!this.isInit)
		{
			this.Init();
		}
		if (Kube.ASS5 == null)
		{
			Kube.RM.require("Assets5", null);
		}
		else
		{
			this.ApplyDress();
		}
	}

	private void Init()
	{
		this.clothesTypeNum = 0;
		this.tempClothes = new int[Kube.GPS.playerClothes.Length];
		for (int i = 0; i < this.filters.Length; i++)
		{
			EventDelegate.Add(this.filters[i].onChange, new EventDelegate.Callback(this.onFilter));
		}
		this.isInit = true;
	}

	private void onFilter()
	{
		this.itemInfo.SetActive(false);
		if (!UIToggle.current.value)
		{
			return;
		}
		this.clothesTypeNum = Array.IndexOf<UIToggle>(this.filters, UIToggle.current);
		this.playerSkin = Kube.GPS.playerSkin;
		this.tempClothes = (int[])Kube.GPS.playerClothes.Clone();
		this.ApplyDress();
		this.Invalidate();
	}

	public void onAssetsLoaded(int id)
	{
		if (Kube.ASS5 != null)
		{
			this.ApplyDress();
		}
	}

	public void ApplyDress()
	{
		if (Kube.ASS5 != null)
		{
			this.roomCharacter.SetActive(true);
			this.roomCharacter.SendMessage("DressSkin", string.Concat(new object[]
			{
				string.Empty,
				this.playerSkin,
				";",
				this.tempClothesStr
			}));
			GameUtils.ChangeLayersRecursively(this.roomCharacter.transform, "MenuRoom");
		}
	}

	private void Update()
	{
		if (this.roomCharacter != null)
		{
			this.roomCharacter.transform.rotation = Quaternion.Lerp(this.roomCharacter.transform.rotation, Quaternion.Euler(0f, this.newCharRotation, 0f), 5f * Time.deltaTime);
		}
	}

	private void OnEnable()
	{
		if (!this.isInit)
		{
			this.Init();
		}
		for (int i = 0; i < Kube.GPS.playerClothes.Length; i++)
		{
			this.tempClothes[i] = Kube.GPS.playerClothes[i];
		}
		this.playerSkin = Kube.GPS.playerSkin;
		this.camera.gameObject.SetActive(true);
		Kube.RM.require("Assets2", null);
		this.itemInfo.SetActive(false);
		this.newCharRotation = 33f;
		this.Invalidate();
		this.ApplyDress();
	}

	private void OnDisable()
	{
		if (this.camera)
		{
			this.camera.gameObject.SetActive(false);
		}
		this.newCharRotation = 33f;
		this.roomCharacter.transform.rotation = Quaternion.Euler(0f, this.newCharRotation, 0f);
		this.roomCharacter.SendMessage("DressSkin", string.Concat(new object[]
		{
			string.Empty,
			Kube.GPS.playerSkin,
			";",
			Kube.GPS.playerClothesStr
		}));
		GameUtils.ChangeLayersRecursively(this.roomCharacter.transform, "MenuRoom");
	}

	private void Invalidate()
	{
		KGUITools.removeAllChildren(this.container.gameObject, true);
		int[] array = null;
		if (this.clothesTypeNum == 0)
		{
			array = new int[Localize.skinName.Length];
			for (int i = 0; i < Localize.skinName.Length; i++)
			{
				array[i] = i;
			}
		}
		if (this.clothesTypeNum == 1)
		{
			array = Kube.IS.shopHats;
		}
		else if (this.clothesTypeNum == 2)
		{
			array = Kube.IS.shopTors;
		}
		else if (this.clothesTypeNum == 3)
		{
			array = Kube.IS.shopBack;
		}
		else if (this.clothesTypeNum == 4)
		{
			array = Kube.IS.shopArms;
		}
		else if (this.clothesTypeNum == 5)
		{
			array = Kube.IS.shopFoots;
		}
		else if (this.clothesTypeNum == 6)
		{
			array = Kube.IS.shopShoulders;
		}
		if (array == null)
		{
			return;
		}
		if (this.clothesTypeNum != 0)
		{
			GameObject gameObject = this.container.gameObject.AddChild(this.itemPrefab);
			CharItem component = gameObject.GetComponent<CharItem>();
			component.itemId = -1;
			component.GetComponentInChildren<UILabel>().text = Localize.is_no_item;
			if (Kube.GPS.playerClothes[this.clothesTypeNum - 1] == -1)
			{
				this.selectedItem = component;
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			GameObject gameObject2 = this.container.gameObject.AddChild(this.itemPrefab);
			CharItem component2 = gameObject2.GetComponent<CharItem>();
			int num = array[j];
			string text;
			if (this.clothesTypeNum == 0)
			{
				text = Localize.skinName[num];
				if (Kube.ASS2 != null)
				{
					component2.GetComponentInChildren<UITexture>().mainTexture = Kube.OH.inventarSkinsTex[num];
				}
				component2.selected = (Kube.GPS.playerSkin == num);
			}
			else
			{
				text = Localize.clothesName[num];
				if (Kube.OH.inventarClothesTex.ContainsKey(num))
				{
					component2.GetComponentInChildren<UITexture>().mainTexture = Kube.OH.inventarClothesTex[num];
				}
				else if (Kube.ASS2 != null)
				{
					component2.GetComponentInChildren<UITexture>().mainTexture = Kube.ASS2.inventarClothesTex[num];
				}
				component2.selected = (Kube.GPS.playerClothes[this.clothesTypeNum - 1] == num);
			}
			if (component2.selected)
			{
				this.selectedItem = component2;
			}
			component2.GetComponentInChildren<UILabel>().text = text;
			component2.itemId = num;
			component2.type = this.clothesTypeNum;
		}
		this.container.Reposition();
	}

	protected CharItem selectedItem
	{
		get
		{
			return this._selectedItem;
		}
		set
		{
			if (this._selectedItem != null)
			{
				this._selectedItem.selected = false;
			}
			this._selectedItem = value;
			if (this._selectedItem != null)
			{
				this._selectedItem.selected = true;
			}
		}
	}

	public void onItemSelect(CharItem charItem)
	{
		this.selectedItem = charItem;
		this.itemInfo.SetActive(true);
		float[] array = new float[16];
		float[] array2 = new float[]
		{
			(float)Kube.GPS.playerHealth,
			(float)Kube.GPS.playerArmor,
			(float)Kube.GPS.playerSpeed,
			(float)Kube.GPS.playerJump,
			(float)Kube.GPS.playerDefend
		};
		bool flag;
		bool flag2;
		if (this.selectedItem.itemId != -1)
		{
			if (this.clothesTypeNum == 0)
			{
				flag = (Kube.GPS.playerSkins[this.selectedItem.itemId] > 0);
				flag2 = (this.selectedItem.itemId == Kube.GPS.playerSkin);
			}
			else
			{
				flag = (Kube.GPS.playerIsClothes[this.selectedItem.itemId] > 0);
				flag2 = (Kube.GPS.playerClothes[this.clothesTypeNum - 1] == this.selectedItem.itemId);
			}
		}
		else
		{
			flag = true;
			flag2 = (Kube.GPS.playerClothes[this.clothesTypeNum - 1] == -1);
		}
		this.price.gameObject.SetActive(!flag);
		this.apply.gameObject.SetActive(!flag2 && flag);
		if (charItem.itemId != -1)
		{
			if (this.clothesTypeNum == 0)
			{
				this.playerSkin = charItem.itemId;
				int num = Kube.GPS.skinPrice[this.playerSkin, 1];
				int num2 = Kube.GPS.skinPrice[this.playerSkin, 2];
				if (num2 > 0)
				{
					this.price.text.text = num2.ToString();
					this.price.isGold = true;
				}
				else
				{
					this.price.text.text = num.ToString();
					this.price.isGold = false;
				}
				for (int i = 0; i < this.sliders.Length; i++)
				{
					array[i] = Kube.GPS.skinBonus[this.playerSkin, i];
				}
			}
			else
			{
				int itemId = charItem.itemId;
				int num3 = Kube.GPS.clothesPrice[itemId, 1];
				int num4 = Kube.GPS.clothesPrice[itemId, 2];
				if (num4 > 0)
				{
					this.price.text.text = num4.ToString();
					this.price.isGold = true;
				}
				else
				{
					this.price.text.text = num3.ToString();
					this.price.isGold = false;
				}
				for (int j = 0; j < this.sliders.Length; j++)
				{
					array[j] = Kube.GPS.clothesBonus[itemId, j];
				}
				this.tempClothes[this.clothesTypeNum - 1] = charItem.itemId;
			}
		}
		else if (this.clothesTypeNum != 0)
		{
			this.tempClothes[this.clothesTypeNum - 1] = -1;
		}
		for (int k = 0; k < this.sliders.Length; k++)
		{
			if (Localize.BonusTypeStr.Length >= k)
			{
				this.sliders[k].title.text = Localize.BonusTypeStr[k] + ": ";
				float num5 = (float)Mathf.CeilToInt(array2[k] + array[k]);
				this.sliders[k].slider.value = num5 / 200f;
				this.sliders[k].sliderMain.value = array2[k] / 200f;
				this.sliders[k].value.text = num5.ToString();
				this.sliders[k].increment.text = "+" + array[k].ToString();
			}
		}
		this.ApplyDress();
	}

	public void onBuyClick()
	{
		CharItem selectedItem = this.selectedItem;
		int itemId = selectedItem.itemId;
		int num;
		int num2;
		if (this.clothesTypeNum == 0)
		{
			num = Kube.GPS.skinPrice[itemId, 1];
			num2 = Kube.GPS.skinPrice[itemId, 2];
		}
		else
		{
			num = Kube.GPS.clothesPrice[itemId, 1];
			num2 = Kube.GPS.clothesPrice[itemId, 2];
		}
		if (Kube.GPS.playerMoney1 >= num && Kube.GPS.playerMoney2 >= num2)
		{
			if (this.clothesTypeNum == 0)
			{
				Kube.SS.BuySkin(selectedItem.itemId, Kube.IS.gameObject, "BuySkinDone");
			}
			else
			{
				Kube.SS.BuyClothes(selectedItem.itemId, Kube.IS.gameObject, "BuyClothesDone");
			}
		}
		else
		{
			MainMenu.ShowBank();
		}
	}

	public void onApplyClick()
	{
		CharItem selectedItem = this.selectedItem;
		if (this.clothesTypeNum == 0)
		{
			Kube.SS.SetSkin(selectedItem.itemId);
		}
		else
		{
			Kube.SS.SetClothes(this.tempClothesStr);
		}
		selectedItem.isSet = true;
		this.homeMenu.UpgradeParamRecountBonuces();
	}

	public void UpdateChar()
	{
		if (this.roomCharacter.activeSelf)
		{
			this.roomCharacter.SendMessage("DressSkin", string.Concat(new object[]
			{
				string.Empty,
				this.playerSkin,
				";",
				this.tempClothesStr
			}));
		}
		this.onItemSelect(this.selectedItem);
	}

	public void CharRight()
	{
		this.newCharRotation -= 45f;
	}

	public void CharLeft()
	{
		this.newCharRotation += 45f;
	}

	public new Camera camera;

	[HideInInspector]
	public Camera oldCamera;

	public GameObject itemInfo;

	public UIToggle[] filters;

	public GameObject itemPrefab;

	public PagePanel container;

	private float newCharRotation;

	public GameObject roomCharacter;

	public PriceButton price;

	public UIButton apply;

	public CharItemParam[] sliders;

	public HomeMenu homeMenu;

	protected int[] tempClothes = new int[32];

	private bool isInit;

	protected int playerSkin;

	private int clothesTypeNum;

	protected CharItem _selectedItem;
}
