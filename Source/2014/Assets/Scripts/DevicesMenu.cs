using System;
using kube;
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
			array = Kube.IS.itemsSwitchNums;
		}
		else if (num == 1)
		{
			array = Kube.IS.itemsAANums;
		}
		else if (num == 2)
		{
			array = Kube.IS.devicesOtherNums;
		}
		KGUITools.removeAllChildren(this.container.gameObject, true);
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = NGUITools.AddChild(this.container.gameObject, this.itemPrefab);
			DecorItem component = gameObject.GetComponent<DecorItem>();
			int itemId = array[i];
			component.itemId = itemId;
		}
		this.container.GetComponent<PagePanel>().Reposition();
	}
}
