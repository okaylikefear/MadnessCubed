using System;
using System.Collections.Generic;
using LitJson;

namespace kube.data
{
	public class PackBox
	{
		public static PackInfo[] list()
		{
			List<PackInfo> list = new List<PackInfo>();
			if (PackBox.packs == null)
			{
				return list.ToArray();
			}
			for (int i = 0; i < PackBox.packs.Length; i++)
			{
				if (PackBox.packs[i].items.Length > 0)
				{
					if (PackBox.packs[i].Validate())
					{
						list.Add(PackBox.packs[i]);
					}
				}
			}
			return list.ToArray();
		}

		public static void parse(JsonData data)
		{
			char[] separator = new char[]
			{
				';'
			};
			int count = data.Count;
			PackBox.packs = new PackInfo[count];
			for (int i = 0; i < count; i++)
			{
				JsonData jsonData = data[i];
				string[] array = jsonData["value"].ToString().Split(separator);
				PackInfo packInfo = new PackInfo();
				packInfo.price = int.Parse(jsonData["price"].ToString());
				int num = array.Length / 2;
				packInfo.id = int.Parse(jsonData["id"].ToString());
				if (jsonData["icon"] != null)
				{
					packInfo.icon = jsonData["icon"].ToString();
				}
				packInfo.cnt = new int[num];
				packInfo.items = new FastInventar[num];
				for (int j = 0; j < num; j++)
				{
					string text = array[j * 2];
					packInfo.cnt[j] = int.Parse(array[j * 2 + 1]);
					int n = int.Parse(text.Substring(1));
					InventarType t;
					if (text[0] == 'w')
					{
						t = InventarType.weapons;
					}
					else if (text[0] == 'q')
					{
						t = InventarType.weaponSkin;
					}
					else
					{
						t = InventarType.items;
					}
					packInfo.items[j] = new FastInventar((int)t, n);
				}
				PackBox.packs[i] = packInfo;
			}
		}

		private static PackInfo[] packs;
	}
}
