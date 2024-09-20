using System;
using kube;
using UnityEngine;

public class RespawnMenu : DecorMenu
{
	protected override void InitFilter(UIToggle filter, int index)
	{
		filter.GetComponentInChildren<UILabel>().text = Localize.SpawnerTypes[index];
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
			array = Kube.IS.decorWeaponsNums;
		}
		if (num == 1)
		{
			array = Kube.IS.decorMonstersNums;
		}
		if (num == 2)
		{
			array = Kube.IS.decorLocationNums;
		}
		if (num == 3)
		{
			array = Kube.IS.itemsAbilsNums;
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
}
