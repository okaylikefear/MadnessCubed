using System;
using kube;
using UnityEngine;

public class BoxesMenu : ShopMenu
{
	public new void Start()
	{
		base.Start();
		this.rentDialog = Cub2Menu.Find<RentItemDialog>("dialog_rent_item");
		this.SelectItemsForMenu();
	}

	protected override void InitFilter(UIToggle filter, int index)
	{
	}

	protected override void SelectItemsForMenu()
	{
		this.inventoryPageType = 0;
		int[] array = null;
		if (this.inventoryPageType == 0)
		{
			array = Kube.IS.getListNums(InventoryScript.ItemPage.Boxes);
		}
		KGUITools.removeAllChildren(this.container.gameObject, true);
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = this.container.gameObject.AddChild(this.itemPrefab);
			BoxItem component = gameObject.GetComponent<BoxItem>();
			int n = array[i];
			int t = 3;
			component.fi = new FastInventar(t, n);
		}
		this.container.GetComponent<PagePanel>().Reposition();
	}

	public override void onSelectKube(int kubeId)
	{
		this.onBuyKube(kubeId);
	}

	public override void onBuyKube(FastInventar fi)
	{
		UnboxDialog unboxDialog = Cub2UI.FindDialog<UnboxDialog>("dialog unbox");
		unboxDialog.Open(fi.Num);
	}

	public RentItemDialog rentDialog;
}
