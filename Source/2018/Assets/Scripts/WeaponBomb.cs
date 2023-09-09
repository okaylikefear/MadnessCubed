using System;
using kube;
using kube.ui;
using UnityEngine;

public class WeaponBomb : WeaponScript
{
	public override void WeaponShot(GameObject bulletGO, Vector3 shotPoint, DamageMessage dm)
	{
	}

	public override void PrimaryAttack(int numWeapon, Vector3 shotPoint, DamageMessage dm, Animation animation)
	{
		if (this._init)
		{
			return;
		}
		if (!(Kube.BCS.gameTypeController as BombController).canPlant(this.owner.transform.position))
		{
			return;
		}
		PlayerScript owner = this.owner;
		animation.Rewind(owner.weaponAnim1face[numWeapon]);
		animation.Play(owner.weaponAnim1face[numWeapon]);
		this._init = true;
		this.rechargingWeaponStart = Time.time;
	}

	private void Update()
	{
		if (this.owner.type != 0)
		{
			return;
		}
		if (!this._init)
		{
			return;
		}
		if (UnityEngine.Input.GetAxis("Fire1") == 0f)
		{
			this._init = false;
			return;
		}
		float num = (Time.time - this.rechargingWeaponStart) / this.reloadTime;
		if (num >= 1f)
		{
			this._init = false;
			PlayerScript owner = this.owner;
			owner.RemoveWeapon(73);
			UnityEngine.Debug.Log("drop bomb");
			PhotonNetwork.Instantiate(this.prefabName, base.transform.Find("ShootPoint").position, Quaternion.identity, 0);
		}
		if (!(Kube.BCS.gameTypeController as BombController).canPlant(this.owner.transform.position))
		{
			this._init = false;
		}
	}

	private void OnGUI()
	{
		if (this.owner.type != 0)
		{
			return;
		}
		if (!this._init)
		{
			return;
		}
		float num = (float)KUI.width;
		float num2 = (float)KUI.height;
		float num3 = (Time.time - this.rechargingWeaponStart) / this.reloadTime;
		if (num3 >= 1f)
		{
			return;
		}
		GUI.DrawTexture(new Rect(0.5f * num - 50f, 0.5f * num2 + 20f, 100f, 16f), Kube.ASS1.levelLine);
		num3 = Mathf.Min(1f, num3);
		GUI.DrawTexture(new Rect(0.5f * num - 48f, 0.5f * num2 + 22f, 96f * num3, 12f), Kube.ASS1.levelProgress);
	}

	protected bool _init;

	protected float rechargingWeaponStart;

	public float reloadTime = 5f;

	public string prefabName = "Assets26/BombDrop";

	private Vector3 initialPos;
}
