using System;
using System.Collections.Generic;
using kube.data;
using UnityEngine;

public class OfferDialogDiscount : OfferDialog
{
	protected override void OfferInit()
	{
		List<OfferItem> list = ((OfferDiscount)this.offer).list;
		KGUITools.removeAllChildren(this.container.gameObject, true);
		int num = 0;
		while (num < list.Count && num < 10)
		{
			GameObject gameObject = this.container.gameObject.AddChild(this.itemPrefab);
			ItemDescIcon component = gameObject.GetComponent<ItemDescIcon>();
			if (num < list.Count)
			{
				component.fi = new FastInventar((int)list[num].type, list[num].id);
				if (list[num].type == InventarType.weapons)
				{
					this.isWeapon = true;
				}
			}
			else
			{
				component.gameObject.SetActive(false);
			}
			num++;
		}
		this.container.Reposition();
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

	public GameObject itemPrefab;

	protected bool isWeapon;

	public UITable container;
}
