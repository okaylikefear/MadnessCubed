using System;
using kube;
using UnityEngine;

public class DanceMenu : ShopMenu
{
	public new void Start()
	{
		base.Start();
		this.rentDialog = Cub2Menu.Find<RentItemDialog>("dialog_rent_item");
	}

	protected override void InitFilter(UIToggle filter, int index)
	{
	}

	protected override void SelectItemsForMenu()
	{
		if (!UIToggle.current.value)
		{
			return;
		}
		this.inventoryPageType = Array.IndexOf<UIToggle>(this.filters, UIToggle.current);
		if (this.inventoryPageType == -1)
		{
			return;
		}
		int[] array = null;
		if (this.inventoryPageType == 0)
		{
			array = Kube.IS.charMovesNums;
		}
		KGUITools.removeAllChildren(this.container.gameObject, true);
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = NGUITools.AddChild(this.container.gameObject, this.itemPrefab);
			DecorItem component = gameObject.GetComponent<DecorItem>();
			int n = array[i];
			int t = 7;
			if (this.inventoryPageType == 1)
			{
				t = 3;
			}
			component.fi = new FastInventar(t, n);
		}
		this.container.GetComponent<PagePanel>().Reposition();
	}

	public override void onSelectKube(int kubeId)
	{
		if (this.inventoryPageType == 1 && Kube.GPS.inventarItems[kubeId] > 0)
		{
			this.fi.SelectSlot(new FastInventar(1, kubeId));
			return;
		}
		this.fi.stop();
		this.onBuyKube(kubeId);
	}

	public override void onBuyKube(int itemId)
	{
		if (this.inventoryPageType == 1)
		{
			this.dialog.itemId = itemId;
			this.dialog.gameObject.SetActive(true);
		}
		else if (Kube.GPS.inventarSpecItems[itemId] <= 0)
		{
			this.rentDialog.itemId = itemId;
			this.rentDialog.gameObject.SetActive(true);
		}
	}

	public RentItemDialog rentDialog;
}
