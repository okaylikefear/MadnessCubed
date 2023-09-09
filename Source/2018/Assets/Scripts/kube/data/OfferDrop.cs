using System;
using System.Collections.Generic;

namespace kube.data
{
	public class OfferDrop : Offer
	{
		public override void parse(string par1)
		{
			string[] array = par1.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				OfferBox.special["drop"] = 2;
			}
		}

		public List<int> list = new List<int>();
	}
}
