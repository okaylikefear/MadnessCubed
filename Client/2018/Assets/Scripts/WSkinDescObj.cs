using System;
using UnityEngine;

public class WSkinDescObj : ScriptableObject
{
	public string title;

	public int weaponId;

	public WeaponParamsObj weapon;

	public int price;

	[NonSerialized]
	public int id;

	public bool hidden;

	public CaseType caseType;
}
