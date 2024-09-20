using System;
using System.Collections.Generic;

namespace kube.data
{
	public class UnboxBox
	{
		public static FastInventar[] selectGameItems(CaseType caseType)
		{
			if (caseType == CaseType.video)
			{
				return UnboxBox.selectVideoGameItems();
			}
			return UnboxBox.selectCaseGameItems(caseType);
		}

		public static FastInventar[] selectVideoGameItems()
		{
			List<FastInventar> list = new List<FastInventar>();
			list.AddRange(UnboxBox.selectCaseGameItems(CaseType.construction));
			list.AddRange(UnboxBox.selectCaseGameItems(CaseType.weapon));
			list.AddRange(UnboxBox.selectCaseGameItems(CaseType.auto));
			FastInventar[] result = list.ToArray().Shuffle<FastInventar>();
			Array.Resize<FastInventar>(ref result, 20);
			return result;
		}

		public static FastInventar[] selectCaseGameItems(CaseType caseType)
		{
			List<FastInventar> list = new List<FastInventar>();
			for (int i = 0; i < Kube.IS.weaponSkins.Length; i++)
			{
				if (Kube.IS.weaponSkins[i].caseType == caseType)
				{
					list.Add(new FastInventar(InventarType.weaponSkin, i));
				}
			}
			for (int j = 0; j < Kube.IS.weaponParams.Length; j++)
			{
				if (Kube.IS.weaponParams[j].caseType == caseType)
				{
					list.Add(new FastInventar(InventarType.weapons, j));
				}
			}
			for (int k = 0; k < Kube.IS.itemDesc.Length; k++)
			{
				if (Kube.IS.itemDesc[k].page != InventoryScript.ItemPage.Boxes)
				{
					if (Kube.IS.itemDesc[k].caseType == caseType)
					{
						list.Add(new FastInventar(InventarType.items, k));
					}
				}
			}
			for (int l = 0; l < Kube.IS.specItemDesc.Length; l++)
			{
				if (Kube.IS.specItemDesc[l].caseType == caseType)
				{
					list.Add(new FastInventar(InventarType.spec, l));
				}
			}
			for (int m = 0; m < Kube.IS.skins.Length; m++)
			{
				if (Kube.IS.skins[m].caseType == caseType)
				{
					list.Add(new FastInventar(InventarType.skins, m));
				}
			}
			for (int n = 0; n < Kube.IS.dressItems.Length; n++)
			{
				if (Kube.IS.dressItems[n].caseType == caseType)
				{
					list.Add(new FastInventar(InventarType.dressItems, n));
				}
			}
			return list.ToArray().Shuffle<FastInventar>();
		}
	}
}
