using System;

namespace kube.data
{
	public class PackInfo
	{
		public bool Validate()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			this.kind = PackKind.def;
			for (int i = 0; i < this.items.Length; i++)
			{
				if (this.items[i].Type == 3)
				{
					num2++;
					if (Array.IndexOf<int>(PackInfo.boxes, this.items[i].Num) != -1)
					{
						this.kind = PackKind.box;
					}
				}
				else if (this.items[i].Type == 4)
				{
					num2++;
					if (this.kind == PackKind.def)
					{
						this.kind = PackKind.wpn;
					}
				}
				else if (this.items[i].Type == 5)
				{
					num2++;
					if (this.kind == PackKind.def)
					{
						this.kind = PackKind.skn;
					}
					int weaponId = Kube.IS.weaponSkins[this.items[i].Num].weaponId;
					if (Kube.GPS.inventarWeapons[weaponId] < 0)
					{
						num++;
					}
				}
			}
			if (this.kind == PackKind.box)
			{
				for (int j = 0; j < this.items.Length; j++)
				{
					if (Array.IndexOf<int>(PackInfo.boxes, this.items[j].Num) != -1)
					{
						if (Kube.GPS.inventarItems[this.items[j].Num] > 0)
						{
							return false;
						}
					}
				}
			}
			if (this.kind == PackKind.wpn)
			{
				for (int k = 0; k < this.items.Length; k++)
				{
					if (this.items[k].Type == 4 && Kube.GPS.inventarWeapons[this.items[k].Num] > 0)
					{
						num3++;
					}
				}
				if (num3 > 0 && num3 == num2)
				{
					return false;
				}
			}
			if (this.kind == PackKind.skn)
			{
				if (num > 0)
				{
					return false;
				}
				for (int l = 0; l < this.items.Length; l++)
				{
					if (this.items[l].Type == 5 && Kube.GPS.weaponsSkin[this.items[l].Num] > 0)
					{
						num3++;
					}
				}
				if (num3 > 0 && num3 == num2)
				{
					return false;
				}
			}
			return true;
		}

		public int id;

		public int price;

		public string icon;

		public int[] cnt;

		public FastInventar[] items;

		protected static int[] boxes = new int[]
		{
			189,
			190,
			191
		};

		public PackKind kind;
	}
}
