using System;
using kube;
using UnityEngine;

public class DecorMenu : ShopMenu
{
	protected override void InitFilter(UIToggle filter, int index)
	{
		if (index >= Localize.DecorTypes.Length)
		{
			filter.gameObject.SetActive(false);
			return;
		}
		filter.GetComponentInChildren<UILabel>().text = Localize.DecorTypesNew[index];
	}

	public override void onSelectKube(int kubeId)
	{
		if (Kube.GPS.inventarItems[kubeId] > 0)
		{
			this.fi.SelectSlot(new FastInventar(1, kubeId));
		}
		else
		{
			this.fi.stop();
			this.onBuyKube(kubeId);
		}
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
			array = Kube.IS.decorLightNums;
		}
		if (this.inventoryPageType == 1)
		{
			array = Kube.IS.decorFurnitureNums;
		}
		if (this.inventoryPageType == 2)
		{
			array = Kube.IS.decorDoorsNums;
		}
		if (this.inventoryPageType == 3)
		{
			array = Kube.IS.decorLeddersNums;
		}
		if (this.inventoryPageType == 4)
		{
			array = Kube.IS.decorGreenNums;
		}
		if (this.inventoryPageType == 5)
		{
			array = Kube.IS.decorDecorNums;
		}
		if (this.inventoryPageType == 6)
		{
			array = Kube.IS.decorRoadNums;
		}
		KGUITools.removeAllChildren(this.container.gameObject, true);
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = NGUITools.AddChild(this.container.gameObject, this.itemPrefab);
			DecorItem component = gameObject.GetComponent<DecorItem>();
			int itemId = array[i];
			component.itemId = itemId;
		}
		this.container.GetComponent<PagePanel>().Reposition();
	}

	public override void onBuyKube(int itemId)
	{
		this.dialog.itemId = itemId;
		this.dialog.gameObject.SetActive(true);
	}
}
