using System;
using System.Collections.Generic;
using kube.data;

public class OfferDialogDiscount : OfferDialog
{
	protected override void OfferInit()
	{
		List<OfferItem> list = ((OfferDiscount)this.offer).list;
		for (int i = 0; i < this.items.Length; i++)
		{
			if (i < list.Count)
			{
				this.items[i].fi = new FastInventar((int)list[i].type, list[i].id);
				if (list[i].type == InventarType.weapons)
				{
					this.isWeapon = true;
				}
			}
			else
			{
				this.items[i].gameObject.SetActive(false);
			}
		}
	}

	public void OnButton()
	{
		base.gameObject.SetActive(false);
		if (this.isWeapon)
		{
			Cub2Menu.instance.OpenTab("Arsenal_menu");
		}
		else
		{
			Cub2Menu.instance.OpenTab("Decor_menu");
		}
	}

	public ItemDescIcon[] items;

	protected bool isWeapon;
}
