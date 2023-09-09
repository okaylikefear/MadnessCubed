using System;
using System.Collections.Generic;
using LitJson;

namespace kube.data
{
	public class OfferBox
	{
		public static void init(JsonData par1)
		{
			List<Offer> list = new List<Offer>();
			for (int i = 0; i < par1.Count; i++)
			{
				JsonData jsonData = par1[i];
				int num = int.Parse(jsonData["type"].ToString());
				Offer offer = (Offer)Activator.CreateInstance(OfferBox._tt[num]);
				offer.type = num;
				offer.expire = DateTime.Parse(jsonData["expire"].ToString());
				offer.expireSeconds = (int)Math.Floor((offer.expire - DateTime.UtcNow).TotalSeconds);
				if (jsonData["params"] != null)
				{
					offer.parse(jsonData["params"].ToString());
				}
				list.Add(offer);
			}
			OfferBox._list = list.ToArray();
		}

		public static Offer[] list()
		{
			return OfferBox._list;
		}

		private static Offer[] _list = new Offer[0];

		public static Dictionary<int, float> cubes = new Dictionary<int, float>();

		public static Dictionary<int, float> weapons = new Dictionary<int, float>();

		public static Dictionary<int, float> items = new Dictionary<int, float>();

		public static Dictionary<int, float> bank = new Dictionary<int, float>();

		public static Dictionary<string, object> special = new Dictionary<string, object>();

		private static Type[] _tt = new Type[]
		{
			typeof(Offer),
			typeof(OfferDiscount),
			typeof(OfferBank),
			typeof(OfferDrop)
		};
	}
}
