using System;
using UnityEngine;

public class ItemDesc : ScriptableObject
{
	public int id
	{
		get
		{
			int num = 0;
			for (int i = 0; i < base.name.Length; i++)
			{
				char c = base.name[i];
				if (c >= '0' && c <= ':')
				{
					num = 10 * num + (int)(c - '0');
				}
			}
			return num;
		}
	}

	public string itemName;

	public InventarType itemType;

	public InventarMenu menuType;

	public string page;
}
