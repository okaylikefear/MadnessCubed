using System;
using System.Collections.Generic;

namespace kube.data
{
	public class OfferDiscount : Offer
	{
		public override void parse(string par1)
		{
			char[] separator = new char[]
			{
				'='
			};
			string[] array = par1.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(separator);
				int num = int.Parse(array2[0].Substring(1));
				float value = 0.5f;
				if (array2.Length > 1)
				{
					value = float.Parse(array2[1]);
				}
				if (array2[0][0] == 'c')
				{
					OfferBox.cubes[num] = value;
				}
				else if (array2[0][0] == 'w')
				{
					OfferBox.weapons[num] = value;
					this.list.Add(new OfferItem
					{
						type = InventarType.weapons,
						id = num
					});
				}
				else if (array2[0][0] == 'i')
				{
					OfferBox.items[num] = value;
					this.list.Add(new OfferItem
					{
						type = InventarType.items,
						id = num
					});
				}
			}
		}

		public List<OfferItem> list = new List<OfferItem>();
	}
}
