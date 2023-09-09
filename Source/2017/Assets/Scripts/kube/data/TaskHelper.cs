using System;
using System.Collections.Generic;

namespace kube.data
{
	public class TaskHelper
	{
		public static Dictionary<BonusDesc, int> parseBonus(string par1)
		{
			Dictionary<BonusDesc, int> dictionary = new Dictionary<BonusDesc, int>();
			string[] array = par1.Split(new char[]
			{
				';'
			});
			int num = array.Length / 2;
			for (int i = 0; i < num; i++)
			{
				string text = array[i * 2];
				BonusDesc key = default(BonusDesc);
				if (text[0] == 'w')
				{
					text = text.Substring(1);
					key.type = 1;
				}
				else if (text[0] == 'i')
				{
					text = text.Substring(1);
					key.type = 0;
				}
				else if (text[0] == '$')
				{
					text = i.ToString();
					key.type = -1;
				}
				else if (text[0] == '#')
				{
					text = i.ToString();
					key.type = -2;
				}
				key.id = int.Parse(text);
				int value = int.Parse(array[i * 2 + 1]);
				dictionary[key] = value;
			}
			return dictionary;
		}
	}
}
