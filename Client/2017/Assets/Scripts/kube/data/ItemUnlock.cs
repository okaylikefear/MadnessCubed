using System;
using LitJson;

namespace kube.data
{
	public class ItemUnlock
	{
		public static void Parse(JsonData unl)
		{
			if (unl == null)
			{
				return;
			}
			for (int i = 0; i < unl.Count; i++)
			{
				string text = unl[i].ToString();
				string a = text.Substring(0, 1);
				int key = int.Parse(text.Substring(1));
				if (a == "w")
				{
					Kube.GPS.weaponUnlock[key] = true;
				}
				else if (a == "s")
				{
					Kube.GPS.specUnlock[key] = true;
				}
				else if (a == "i")
				{
					Kube.GPS.itemUnlock[key] = true;
				}
				else if (a == "m")
				{
					Kube.GPS.missionUnlock[key] = true;
				}
				else if (a == "c")
				{
					Kube.GPS.charUnlock[key] = true;
				}
			}
		}
	}
}
