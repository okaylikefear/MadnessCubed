using System;
using UnityEngine;

public class DevicesMenu : DecorMenu
{
	protected override void InitFilter(UIToggle filter, int index)
	{
		if (index >= Localize.DeviceTypes.Length)
		{
			filter.gameObject.SetActive(false);
			return;
		}
		filter.GetComponentInChildren<UILabel>().text = Localize.DeviceTypes[index];
	}

	protected override void SelectItemsForMenu()
	{
		if (!UIToggle.current.value)
		{
			return;
		}
		int num = Array.IndexOf<UIToggle>(this.filters, UIToggle.current);
		if (num == -1)
		{
			return;
		}
		int[] array = null;
		if (num == 0)
		{
			array = base.getListNums(InventoryScript.ItemPage.Switch);
		}
		else if (num == 1)
		{
			array = base.getListNums(InventoryScript.ItemPage.AA);
		}
		else if (num == 2)
		{
			array = base.getListNums(InventoryScript.ItemPage.Transport);
		}
		else if (num == 3)
		{
			array = base.getListNums(InventoryScript.ItemPage.Other);
		}
		else if (num == 4)
		{
			array = base.getListNums(InventoryScript.ItemPage.Guns);
		}
		KGUITools.removeAllChildren(this.container.gameObject, true);
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = this.container.gameObject.AddChild(this.itemPrefab);
			DecorItem component = gameObject.GetComponent<DecorItem>();
			int itemId = array[i];
			component.itemId = itemId;
		}
		this.container.GetComponent<PagePanel>().Reposition();
	}
}
