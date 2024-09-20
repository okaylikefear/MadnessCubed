using System;
using UnityEngine;

public class RobotBossScript : MonsterScript
{
	protected override void CreateShot(Vector3 shotPoint)
	{
		int num = UnityEngine.Random.Range(0, this.weapons.Length);
		if (this.dead)
		{
			return;
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_CreateShot2", PhotonTargets.All, new object[]
			{
				shotPoint,
				num
			});
		}
		else
		{
			this._CreateShot2(shotPoint, num, null);
		}
	}

	[PunRPC]
	protected void _CreateShot2(Vector3 shotPoint, int point, PhotonMessageInfo info)
	{
		if (this.dead)
		{
			return;
		}
		DamageMessage damageMessage = new DamageMessage();
		if (PhotonNetwork.offlineMode || base.photonView.isMine)
		{
			damageMessage.damage = (short)UnityEngine.Random.Range(this.shootDamage.x, this.shootDamage.y);
		}
		else
		{
			damageMessage.damage = 0;
		}
		damageMessage.id_killer = 0;
		damageMessage.team = 99;
		this.weapons[point].fatalDistance = this.maxShootDist;
		this.weapons[point].WeaponShot(this.bullets[point], shotPoint, damageMessage);
	}

	public WeaponScript[] weapons;

	public GameObject[] bullets;
}
