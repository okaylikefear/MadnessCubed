using System;
using System.Collections.Generic;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class MaptopMyTab : MonoBehaviour
{
	public void ResetMap()
	{
		this.newMap.SetActive(true);
	}

	private void Awake()
	{
		this.loading.SetActive(false);
		this.LoadAndShow();
	}

	public void LoadAndShow()
	{
		this.loading.SetActive(true);
		Kube.SS.Request(800, null, new ServerCallback(this.onLoaded));
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void onLoaded(string response)
	{
		this.loading.SetActive(false);
		JsonData jsonData = JsonMapper.ToObject(response);
		this.items = MapTop.parse(jsonData["items"]);
		this._addprice = (int)jsonData["price"];
		this.Invalidate();
	}

	private void OnEnable()
	{
		KGUITools.removeAllChildren(this.container.gameObject, false);
		this.container.ResetPosition();
		this.LoadAndShow();
	}

	private void BuyNewMapDone()
	{
		this.Invalidate();
	}

	private void Invalidate()
	{
		if (Kube.GPS == null)
		{
			return;
		}
		KGUITools.removeAllChildren(this.container.gameObject, false);
		this.addslot.SetActive(true);
		this.addslot.GetComponentInChildren<UILabel>().text = string.Format(Localize.createForX, this._addprice);
		int height = this.itemPrefab.GetComponent<UIWidget>().height;
		int num = 0;
		Vector3 localPosition = Vector3.zero;
		int num2 = this.items.Length;
		for (int i = 0; i < num2; i++)
		{
			GameObject gameObject;
			if (this._itemcache.Count > i)
			{
				gameObject = this._itemcache[i];
				gameObject.SetActive(true);
			}
			else
			{
				gameObject = this.container.gameObject.AddChild(this.itemPrefab);
				this._itemcache.Add(gameObject);
			}
			localPosition = gameObject.transform.localPosition;
			localPosition.y = (float)num;
			num -= height + 5;
			gameObject.transform.localPosition = localPosition;
			MyMaptopItem component = gameObject.GetComponent<MyMaptopItem>();
			component.mapId = i;
			component.oid = int.Parse(this.items[i].id.ToString());
			component.title.text = this.items[i].name.ToString();
			component.id.text = Localize.c_map_id + this.items[i].roomMapNumber.ToString();
			component.info = this.items[i];
			int num3 = int.Parse(this.items[i].roomType.ToString());
			if (num3 < MaptopOnlineTab.modeSprites.Length)
			{
				component.mode.spriteName = MaptopOnlineTab.modeSprites[num3];
			}
		}
		if (this.items.Length < 20)
		{
			localPosition = this.addslot.transform.localPosition;
			localPosition.y = (float)num;
			this.addslot.transform.localPosition = localPosition;
		}
		else
		{
			this.addslot.SetActive(false);
		}
		this.container.UpdatePosition();
	}

	private void onReset(string data)
	{
		this.LoadAndShow();
	}

	public void onResetSlot(MyMaptopItem mapItem)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["oid"] = mapItem.oid.ToString();
		Kube.SS.Request(803, dictionary, new ServerCallback(this.onReset));
	}

	public TopInfo hasRecord(long roomMapNumber, int roomType)
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			if (this.items[i].roomMapNumber == roomMapNumber && this.items[i].roomType == roomType)
			{
				return this.items[i];
			}
		}
		return null;
	}

	public void onSelectSlot(MyMaptopItem mapItem)
	{
		AddTopDialog addTopDialog = Cub2UI.FindAndOpenDialog<AddTopDialog>("dialog_addtop");
		addTopDialog.owner = this;
		addTopDialog.info = mapItem.info;
	}

	public void onBuySlot()
	{
		AddTopDialog addTopDialog = Cub2UI.FindAndOpenDialog<AddTopDialog>("dialog_addtop");
		addTopDialog.owner = this;
		addTopDialog.info = null;
	}

	private const int MAX_MAPS = 20;

	public GameObject addslot;

	public UIScrollView container;

	public GameObject hint;

	protected int _addprice = 10;

	public GameObject newMap;

	public GameObject loading;

	protected int _NewMapType;

	public GameObject itemPrefab;

	private TopInfo[] items;

	private List<GameObject> _itemcache = new List<GameObject>();
}
