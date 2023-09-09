using System;

[Serializable]
public class WeaponSkinDesc
{
	public string name;

	public int weaponId;

	public int price;

	[NonSerialized]
	public int id;

	public bool hidden;
}
