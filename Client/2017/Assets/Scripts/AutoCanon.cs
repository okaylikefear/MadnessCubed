using System;
using System.Collections;
using kube;
using UnityEngine;

public class AutoCanon : Pawn
{
	private void Start()
	{
		if (base.photonView.isMine)
		{
			base.Invoke("Aim", 1f);
		}
		if (this.shot == null)
		{
			for (int i = 0; i < Kube.ASS6.charWeaponsGO.Length; i++)
			{
				if (Kube.ASS6.weaponsBulletPrefab[i].name == this.bulletGO.name)
				{
					this.shot = Kube.ASS6.charWeaponsGO[i].GetComponent<AudioSource>().clip;
					break;
				}
			}
		}
		if (!this.bulletGO.GetComponent<BulletScript>())
		{
			this.aimAtFloor = true;
		}
		if (base.GetComponent<AudioSource>() == null)
		{
			base.gameObject.AddComponent<AudioSource>();
			base.GetComponent<AudioSource>().playOnAwake = false;
		}
		this._ammo = this.maxAmmo;
		this._health = this.maxHealth;
	}

	private void Renew()
	{
		this._ammo = this.maxAmmo;
		this._health = this.maxHealth;
	}

	private void OwnerIsDead()
	{
		base.Invoke("Remove", 1f);
	}

	private void SetOwner(PlayerScript ps)
	{
		this._owner = ps;
	}

	public void TryToDrive(int playerId)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_TryToDrive", PhotonTargets.MasterClient, new object[]
			{
				playerId
			});
		}
		else
		{
			this._TryToDrive(playerId, null);
		}
	}

	[PunRPC]
	public void _TryToDrive(int playerId, PhotonMessageInfo info)
	{
		if (PhotonNetwork.isMasterClient)
		{
			this._owner = PlayerScript.FromId(playerId);
			this.Renew();
		}
	}

	private void FireGun(Vector3 aimPoint)
	{
		this._ammo--;
		this.nextShoot = Time.time + this.shootDelay;
		this._FireGun(aimPoint, null);
		base.photonView.RPC("_FireGun", PhotonTargets.Others, new object[]
		{
			aimPoint
		});
	}

	[PunRPC]
	private void _FireGun(Vector3 aimPoint, PhotonMessageInfo info)
	{
		DamageMessage damageMessage = new DamageMessage();
		damageMessage.damage = (short)this.shootDamage;
		damageMessage.id_killer = 0;
		damageMessage.team = 99;
		this.WeaponShot(this.bulletGO, aimPoint, damageMessage);
	}

	private Vector3 calcAimPoint()
	{
		if (this.aimAtFloor)
		{
			return this._ps.transform.position;
		}
		return this._ps.transform.position + new Vector3(0f, 0.6f, 0f);
	}

	private void Update()
	{
		if (this.dead)
		{
			return;
		}
		if (this.muzzleFlash)
		{
			if (this._muzzleFlashTime <= 0.0)
			{
				this.muzzleFlash.enableEmission = false;
			}
			else
			{
				this._muzzleFlashTime -= (double)Time.deltaTime;
			}
		}
		if (this.lightObj)
		{
			if (this._lightTime <= 0f)
			{
				this.lightObj.enabled = false;
			}
			else
			{
				this._lightTime -= Time.deltaTime;
			}
		}
		if (!base.photonView.isMine)
		{
			return;
		}
		bool flag = false;
		Vector3 vector = Vector3.zero;
		if (this._ps)
		{
			vector = this.calcAimPoint();
			Vector3 forward = vector - base.transform.position;
			if (forward.magnitude < 10f)
			{
				forward.y = 0f;
			}
			else if (Mathf.Abs(forward.y) > 2f)
			{
				forward.y = Mathf.Sign(forward.y) * 2f;
			}
			Quaternion quaternion = Quaternion.LookRotation(forward);
			this.head.transform.rotation = Quaternion.Lerp(this.head.transform.rotation, quaternion, 0.5f);
			flag = quaternion.AlmostEquals(this.head.transform.rotation, 0.1f);
			if (this._ps.dead)
			{
				this._ps = null;
			}
		}
		if (this._ammo > 0 && this.nextShoot < Time.time && flag && this._ps)
		{
			this.FireGun(vector);
		}
	}

	private void Remove()
	{
		PhotonNetwork.Destroy(base.gameObject);
	}

	private IEnumerator CreateBullet(GameObject bulletGO, Vector3 aimPoint, DamageMessage dm)
	{
		int index = UnityEngine.Random.Range(0, this.shotPoint.Length - 1);
		Vector3 pos = this.shotPoint[index].position;
		GameObject bullet = UnityEngine.Object.Instantiate(bulletGO, Vector3.zero, Quaternion.identity) as GameObject;
		bullet.transform.position = pos;
		bullet.transform.LookAt(aimPoint);
		BulletScript bs = bullet.GetComponent<BulletScript>();
		if (bs != null)
		{
			bs.accuarcy = this.accuarcy;
			bs.fatalDistance = this.fatalDistance;
		}
		if (this._owner != null)
		{
			dm.id_killer = this._owner.onlineId;
			dm.team = this._owner.team;
		}
		else
		{
			dm.id_killer = -1;
			dm.team = 99;
		}
		dm.attacker = this;
		bullet.SendMessage("SetDamageParam", dm);
		if (bullet.GetComponent<Collider>())
		{
			bullet.GetComponent<Collider>().enabled = false;
			yield return new WaitForSeconds(0.3f);
			bullet.GetComponent<Collider>().enabled = true;
		}
		yield break;
	}

	public void WeaponShot(GameObject bulletGO, Vector3 shotPoint, DamageMessage dm)
	{
		if (this.shot != null)
		{
			base.GetComponent<AudioSource>().clip = this.shot;
			base.GetComponent<AudioSource>().Play();
		}
		base.StartCoroutine(this.CreateBullet(bulletGO, shotPoint, dm));
		if (this.muzzleFlash)
		{
			this.muzzleFlash.enableEmission = true;
			this._muzzleFlashTime = 0.2;
			this.muzzleFlash.Emit(1);
		}
		if (this.lightObj)
		{
			this.lightObj.enabled = true;
			this._lightTime = 0.05f;
		}
	}

	public new int getTeam()
	{
		if (this._owner)
		{
			return this._owner.team;
		}
		return -1;
	}

	private void Aim()
	{
		Pawn[] array = UnityEngine.Object.FindObjectsOfType<Pawn>();
		float magnitude = this.minShotDist;
		Pawn pawn = null;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 direction = array[i].transform.position - base.transform.position;
			Pawn pawn2 = array[i];
			if (!(array[i] == this))
			{
				if (!(array[i] is AutoCanon))
				{
					if (Kube.BCS.gameType == GameType.mission || Kube.BCS.gameType == GameType.survival || Kube.BCS.gameType == GameType.test)
					{
						if (array[i] is PlayerScript)
						{
							goto IL_1B7;
						}
						if (array[i].getTeam() == this.getTeam())
						{
							goto IL_1B7;
						}
					}
					if (Kube.BCS.gameType == GameType.teams)
					{
						if (array[i].getTeam() == this.getTeam())
						{
							goto IL_1B7;
						}
						if (array[i] is PlayerScript && ((PlayerScript)array[i]).team == this._owner.team)
						{
							goto IL_1B7;
						}
					}
					if (!(array[i].tag == "ThisPlayerItem"))
					{
						if (direction.magnitude < magnitude)
						{
							pawn2 = array[i];
							if (!(pawn2 == null) && !pawn2.dead)
							{
								if (Kube.BCS.gameType != GameType.mission || pawn2.getTeam() != this.getTeam())
								{
									if (!Physics.Raycast(this.shotPoint[0].position, direction, direction.magnitude, 46337))
									{
										pawn = pawn2;
										magnitude = direction.magnitude;
									}
								}
							}
						}
					}
				}
			}
			IL_1B7:;
		}
		if (pawn)
		{
			this._ps = pawn;
		}
		base.Invoke("Aim", 1f);
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (PhotonNetwork.connected)
		{
			if (stream.isWriting)
			{
				stream.SendNext(this._ammo);
				stream.SendNext(this.head.transform.rotation);
			}
			else
			{
				this._ammo = (int)stream.ReceiveNext();
				this.head.transform.rotation = (Quaternion)stream.ReceiveNext();
			}
		}
	}

	[PunRPC]
	private void _Die()
	{
		if (this.dead)
		{
			return;
		}
		base.CancelInvoke();
		this.dead = true;
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		base.gameObject.layer = 2;
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.ragdoll, base.transform.position, base.transform.rotation);
		Pawn.CopyTransformsRecurse(base.transform, gameObject.transform);
		base.Invoke("DestroyPhotonView", 2f);
	}

	private void ApplyDamage(DamageMessage dm)
	{
		if (PlayerScript.FromId(dm.id_killer) == this._owner)
		{
			return;
		}
		if (Kube.BCS.gameType == GameType.mission)
		{
			if (PlayerScript.FromId(dm.id_killer))
			{
				return;
			}
		}
		else if (Kube.BCS.gameType == GameType.teams && dm.team == this._owner.team)
		{
			return;
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ApplyDamage", PhotonTargets.All, new object[]
			{
				dm.damage
			});
		}
		else
		{
			this._ApplyDamage(dm.damage, null);
		}
	}

	[PunRPC]
	private void _ApplyDamage(short _damage, PhotonMessageInfo info)
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		this._health -= (int)_damage;
		if (this._health <= 0)
		{
			this._Die();
			base.photonView.RPC("_Die", PhotonTargets.Others, new object[0]);
		}
	}

	public GameObject head;

	public ParticleSystem muzzleFlash;

	protected double _muzzleFlashTime;

	public Light lightObj;

	protected float _lightTime;

	public Transform[] shotPoint;

	private Pawn _ps;

	public int shootDamage = 10;

	public int maxAmmo = 100;

	protected int _ammo;

	protected int _health;

	public AudioClip shot;

	public bool aimAtFloor;

	protected PlayerScript _owner;

	public float shootDelay = 1f;

	public GameObject bulletGO;

	private float nextShoot;

	public float accuarcy = 1f;

	public float fatalDistance = 10f;

	public float minShotDist = 30f;

	public int maxHealth = 30;

	public GameObject ragdoll;
}
