using System;
using UnityEngine;

public class WeaponParamsObj : ScriptableObject
{
	public string desc;

	[NonSerialized]
	public int id;

	public int order;

	public int weaponType;

	public int UsingBullets;

	public int[] clipSize;

	[NonSerialized]
	public int currentClipSizeIndex;

	public float[] reloadTime;

	[NonSerialized]
	public int currentReloadTimeIndex;

	public int BulletsType;

	public float[] DeltaShotArray;

	[NonSerialized]
	public int currentDeltaShotIndex;

	public float DeltaShot;

	public float[] Damage;

	[NonSerialized]
	public int currentDamageIndex;

	public float[] Accuracy;

	[NonSerialized]
	public int currentAccuracyIndex;

	public float Distance;

	public int Type;

	public Texture[] aimTex;

	public float fatalDistance;

	public float accuarcy;

	public InventoryScript.WeaponGroup weaponGroup;

	public bool hidden;

	public int needHealthLevel;

	public int needArmorLevel;

	public int needJumpLevel;

	public int needSpeedLevel;

	public int needResistLevel;

	public int needLevel;

	public bool sniper;

	public CaseType caseType;
}
