using System;
using UnityEngine;

public class ShopMenu : MonoBehaviour
{
	private void Awake()
	{
		if (this.fi == null)
		{
			this.fi = base.transform.parent.GetComponentInChildren<FastInventarPanel>();
		}
		MonoBehaviour.print("Awake");
	}

	public void Start()
	{
		if (!this._init)
		{
			this.Init();
		}
	}

	private void Init()
	{
		if (this._init)
		{
			return;
		}
		if (this.dialog == null)
		{
			this.dialog = Cub2Menu.Find<BuyItemDialog>("dialog_buy_item");
		}
		for (int i = 0; i < this.filters.Length; i++)
		{
			this.InitFilter(this.filters[i], i);
			EventDelegate.Add(this.filters[i].onChange, new EventDelegate(new EventDelegate.Callback(this.onChangeFilter)));
		}
		this._init = true;
	}

	public void onChangeFilter()
	{
		if (!UIToggle.current.value)
		{
			return;
		}
		this.fi.stop();
		this.SelectItemsForMenu();
	}

	protected virtual void SelectItemsForMenu()
	{
	}

	private void Update()
	{
	}

	public void OnEnable()
	{
		if (!this._init)
		{
			this.Init();
		}
		this.fi.stop();
		this.SelectItemsForMenu();
	}

	protected virtual void InitFilter(UIToggle filter, int index)
	{
	}

	public virtual void onSelectKube(int kubeId)
	{
	}

	public virtual void onSelectItem(DecorItem item)
	{
		if (this._selected)
		{
			this._selected.value = false;
		}
		this._selected = item;
		if (this._selected)
		{
			this._selected.value = true;
		}
	}

	public virtual void onBuyKube(int itemId)
	{
	}

	public FastInventarPanel fi;

	public UIPanel container;

	public UIToggle[] filters;

	public GameObject itemPrefab;

	public BuyItemDialog dialog;

	public GameObject fastInventory;

	private bool _init;

	protected int inventoryPageType = -1;

	protected DecorItem _selected;
}
