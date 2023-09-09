using System;
using kube;

public class FastWeaponPanel : FastInventarPanel
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
		for (int i = 0; i < this.slots.Length; i++)
		{
			SlotItem component = this.slots[i].GetComponent<SlotItem>();
			component.invItem = this.fastInventar[this.slotOffset + i];
		}
	}
}
