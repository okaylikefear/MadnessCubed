using System;
using System.Collections.Generic;

namespace kube.data
{
	public class OfferBank : Offer
	{
		public override void parse(string par1)
		{
			string[] array = par1.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				int num = int.Parse(array[i].Substring(1));
				this.list.Add(num);
				OfferBox.bank[num] = 1f;
			}
		}

		public List<int> list = new List<int>();
	}
}
