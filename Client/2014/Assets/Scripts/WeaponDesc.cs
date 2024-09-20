using System;
using UnityEngine;

public class WeaponDesc : ItemDesc
{
	public void OnEnable()
	{
		int num = int.Parse(base.name.Substring(6));
	}

	public string goname;

	public Texture tex2;
}
