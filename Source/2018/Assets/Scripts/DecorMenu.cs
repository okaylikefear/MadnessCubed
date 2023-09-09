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
			array = base.getListNums(InventoryScript.ItemPage.Lights);
		}
		if (this.inventoryPageType == 1)
		{
			array = base.getListNums(InventoryScript.ItemPage.Furniture);
		}
		if (this.inventoryPageType == 2)
		{
			array = base.getListNums(InventoryScript.ItemPage.Doors);
		}
		if (this.inventoryPageType == 3)
		{
			array = base.getListNums(InventoryScript.ItemPage.Ladders);
		}
		if (this.inventoryPageType == 4)
		{
			array = base.getListNums(InventoryScript.ItemPage.Green);
		}
		if (this.inventoryPageType == 5)
		{
			array = base.getListNums(InventoryScript.ItemPage.Decor);
		}
		if (this.inventoryPageType == 6)
		{
			array = base.getListNums(InventoryScript.ItemPage.Road);
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

	public override void onBuyKube(FastInventar fi)
	{
		int num = fi.Num;
		if (Kube.IS.needUnlock(fi))
		{
			Cub2UI.MessageBox(Localize.need_prew_upgrade, null);
			return;
		}
		if (!Kube.IS.canBuy(fi))
		{
			UnlockDialog unlockDialog = Cub2UI.FindAndOpenDialog<UnlockDialog>("dialog_unlock");
			unlockDialog.fi = fi;
			if (fi.Type == 3)
			{
				unlockDialog.itemCode = "i" + num;
			}
			else
			{
				unlockDialog.itemCode = "s" + num;
			}
			return;
		}
		this.dialog.itemId = num;
		this.dialog.gameObject.SetActive(true);
	}
}
