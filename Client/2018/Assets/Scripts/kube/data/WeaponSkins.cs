using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace kube.data
{
	internal class WeaponSkins
	{
		public static WSkinDescObj[] select(int weaponId)
		{
			List<WSkinDescObj> list = new List<WSkinDescObj>();
			WSkinDescObj defskin = WeaponSkins.DEFSKIN;
			defskin.id = -1;
			defskin.name = string.Empty;
			list.Add(defskin);
			for (int i = 0; i < Kube.IS.weaponSkins.Length; i++)
			{
				Kube.IS.weaponSkins[i].id = i;
				if (!Kube.IS.weaponSkins[i].hidden || Kube.GPS.weaponsSkin[i] != 0)
				{
					if (Kube.IS.weaponSkins[i].weaponId == weaponId)
					{
						list.Add(Kube.IS.weaponSkins[i]);
					}
				}
			}
			return list.ToArray();
		}

		public static int weaponId(int skinId)
		{
			return Kube.IS.weaponSkins[skinId].weaponId;
		}

		public static void Parse(JsonData wpu)
		{
			if (wpu == null || !wpu.IsObject)
			{
				return;
			}
			for (int i = 0; i < Kube.IS.weaponParams.Length; i++)
			{
				string text = "w" + i.ToString();
				if (wpu.Keys.Contains(text))
				{
					int value = int.Parse((string)wpu[text]);
					Kube.GPS.weaponsCurrentSkin[i] = value;
				}
			}
			for (int j = 0; j < Kube.IS.weaponSkins.Length; j++)
			{
				string item = "s" + j.ToString();
				string empty = string.Empty;
				if (wpu.Keys.Contains(item))
				{
					Kube.GPS.weaponsSkin[j] = 1;
				}
			}
		}

		public static WSkinDescObj DEFSKIN = ScriptableObject.CreateInstance<WSkinDescObj>();
	}
}
