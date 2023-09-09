using System;
using kube;
using UnityEngine;

public class FastItemPanel : FastInventarPanel
{
	protected override FastInventar[] fastInventar
	{
		get
		{
			if (!Kube.BCS || !Kube.BCS.ps || !this.ingame)
			{
				return Kube.GPS.fastInventarWeapon;
			}
			return Kube.BCS.ps.fastInventar;
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
			component.cntvalue = ((!ps) ? 0 : ps.itemCnt(num, Kube.GPS.inventarItems[num]));
			if (ps && ps.nextItemUse(num) > Time.time)
			{
				component.tx.alpha = 0.5f;
			}
			else
			{
				component.tx.alpha = 1f;
			}
		}
	}
}
