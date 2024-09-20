using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class HUDCreatingModeBase : MonoBehaviour
{
	public void SelectGeom()
	{
		List<int> list = new List<int>();
		list.Add(0);
		for (int i = 0; i < Kube.IS.specItemDesc.Length; i++)
		{
			if (Kube.IS.specItemDesc[i].page == InventoryScript.ItemPage.Forms)
			{
				if (Kube.GPS.inventarSpecItems[i] > 0 || Kube.GPS.isVIP)
				{
					list.Add(i - 10);
				}
			}
		}
		this.geomIds = list.ToArray();
	}

	public virtual void SetCube(int index)
	{
	}

	[NonSerialized]
	public int[] geomIds;
}
