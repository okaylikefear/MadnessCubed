using System;
using kube;
using UnityEngine;

public class InventarWeaponScript : MonoBehaviour
{
	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
	}

	public void onAssetsLoaded(int id)
	{
		if (Kube.ASS6 == null)
		{
			return;
		}
		if (this._numWeapon != -1)
		{
			this.SetNewWeapon(this._numWeapon);
		}
	}

	private void SetNewWeapon(int numWeapon)
	{
		if (Kube.ASS6 == null)
		{
			this._numWeapon = numWeapon;
			Kube.RM.require("Assets6", null);
			Kube.RM.requireByTag("Weapons");
			return;
		}
		if (this.weapon != null)
		{
			UnityEngine.Object.Destroy(this.weapon);
		}
		if (numWeapon < 0 || numWeapon >= Kube.IS.weaponParams.Length)
		{
			return;
		}
		this.weapon = (UnityEngine.Object.Instantiate(Kube.OH.charWeaponsGO[numWeapon], base.transform.position, base.transform.rotation) as GameObject);
		this.weapon.transform.parent = this.weaponHolder.transform;
		this.weapon.transform.localPosition = new Vector3(0f, 0f, 0f);
		this.weapon.transform.localRotation = Quaternion.identity;
	}

	private void Start()
	{
		this.Init();
	}

	private void Update()
	{
		this.weaponHolder.transform.RotateAround(Vector3.up, 1f * Time.deltaTime);
	}

	public GameObject weaponHolder;

	private GameObject weapon;

	private bool initialized;

	private int _numWeapon = -1;
}
