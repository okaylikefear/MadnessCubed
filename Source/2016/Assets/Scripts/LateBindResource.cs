using System;
using UnityEngine;

public class LateBindResource : ScriptableObject
{
	public int id;

	public LateBindResource.ResourceType t;

	public Texture icon;

	public GameObject go;

	public Material mat;

	public new string name;

	public string desc;

	public enum ResourceType
	{
		Item,
		SpecialItem,
		Weapon,
		Clothes,
		Skin,
		Bullet,
		WeaponSkin,
		Photon
	}
}
