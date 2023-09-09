using System;
using UnityEngine;

public class ItemDescObj : ScriptableObject
{
	public string desc;

	public int needLevel;

	public bool hidden;

	public InventoryScript.ItemPage page;

	public CaseType caseType;

	[NonSerialized]
	public int id;
}
