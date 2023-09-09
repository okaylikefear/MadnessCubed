using System;
using kube;
using UnityEngine;

public class FastItemPanel : FastInventarPanel
{
	protected override FastInventar[] fastInventar
	{
		get
		{
			return Kube.GPS.fastInventarWeapon;
		}
	}

	protected override void Invalidate()
	{
		PlayerScript ps = Kube.BCS.ps;
		for (int i = 0; i < this.slots.Length; i++)
		{
			SlotItem component = this.slots[i].GetComponent<SlotItem>();
			component.invItem = this.fastInventar[this.slotOffset + i];
			int num = this.fastInventar[this.slotOffset + i].Num;
			component.cntvalue = ps.itemCnt(num, Kube.GPS.inventarItems[num]);
			if (ps.nextItemUse(num) > Time.time)
			{
				component.tx.alpha = 0.5f;
			}
			else
			{
				component.tx.alpha = 1f;
			}
		}
	}

	[NonSerialized]
	public int[] slotscnt = new int[10];
}
