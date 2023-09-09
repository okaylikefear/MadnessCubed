using System;
using kube;
using UnityEngine;

public class SpecMenu : ShopMenu
{
	public new void Start()
	{
		base.Start();
		this.rentDialog = Cub2Menu.Find<RentItemDialog>("dialog_rent_item");
	}

	protected override void InitFilter(UIToggle filter, int index)
	{
		filter.GetComponentInChildren<UILabel>().text = Localize.ItemsTypes[index + 1];
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
			array = Kube.IS.getSpecListNums(InventoryScript.ItemPage.Spec);
		}
		else if (this.inventoryPageType == 1)
		{
			array = base.getListNums(InventoryScript.ItemPage.Battle);
		}
		else if (this.inventoryPageType == 2)
		{
			array = Kube.IS.getSpecListNums(InventoryScript.ItemPage.Moves);
		}
		KGUITools.removeAllChildren(this.container.gameObject, true);
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject;
			if (this.inventoryPageType == 1)
			{
				gameObject = this.container.gameObject.AddChild(this.itemPrefab);
			}
			else
			{
				gameObject = this.container.gameObject.AddChild(this.specItemPrefab);
			}
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
			int t = 7;
			if (this.inventoryPageType == 1)
			{
				t = 3;
			}
			this.fi.SelectSlot(new FastInventar(t, kubeId));
			return;
		}
		this.fi.stop();
		this.onBuyKube(kubeId);
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
		for (int i = 0; i < this.filters.Length; i++)
		{
			if (this.filters[i].value)
			{
				this.inventoryPageType = i;
				break;
			}
		}
		if (this.inventoryPageType == 1)
		{
			this.dialog.itemId = num;
			this.dialog.gameObject.SetActive(true);
		}
		else if (Kube.GPS.inventarSpecItems[num] <= 0)
		{
			this.rentDialog.itemId = num;
			this.rentDialog.gameObject.SetActive(true);
		}
	}

	public GameObject specItemPrefab;

	public RentItemDialog rentDialog;
}
