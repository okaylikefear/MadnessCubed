using System;
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
			array = base.getListNums(InventoryScript.ItemPage.Weapons);
		}
		if (num == 1)
		{
			array = base.getListNums(InventoryScript.ItemPage.Monsters);
		}
		if (num == 2)
		{
			array = base.getListNums(InventoryScript.ItemPage.Location);
		}
		if (num == 3)
		{
			array = base.getListNums(InventoryScript.ItemPage.Abilis);
		}
		KGUITools.removeAllChildren(this.container.gameObject, true);
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
