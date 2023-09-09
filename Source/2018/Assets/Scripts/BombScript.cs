using System;
using kube;
using kube.ui;
using Photon;
using UnityEngine;

public class BombScript : Photon.MonoBehaviour
{
	private void SetPlayerId(int _id)
	{
		this.playerId = _id;
	}

	private void Start()
	{
		this.timeToExplosion = Time.time + this.bombCountdownTime;
		this.dm = new DamageMessage();
		if (base.photonView.isMine)
		{
			this.dm.damage = (short)this.damage;
		}
		else
		{
			this.dm.damage = 0;
		}
		this.dm.id_killer = this.playerId;
		base.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
		this.activeStartTime = Time.time + this.activationTime;
		if (base.photonView.instantiationData != null && !(bool)base.photonView.instantiationData[0])
		{
			this.activated = false;
		}
		if (this.activated)
		{
			Kube.BCS.gameTypeController.SendMessage("BombPlanted", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			SphereCollider sphereCollider = new GameObject("dropTrigger")
			{
				transform = 
				{
					parent = base.transform,
					localPosition = Vector3.zero
				}
			}.AddComponent<SphereCollider>();
			sphereCollider.isTrigger = true;
			sphereCollider.radius = 1.5f;
		}
	}

	private void Update()
	{
		if (!this.activated)
		{
			return;
		}
		this.AlwaysUpdate();
		if (!base.photonView.isMine)
		{
			return;
		}
		if (Time.time > this.timeToExplosion)
		{
			this.DoExplosion();
		}
	}

	private void BombDisarm()
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_BombDisarm", PhotonTargets.All, new object[0]);
		}
		else
		{
			this._BombDisarm(null);
		}
	}

	[PunRPC]
	private void _BombDisarm(PhotonMessageInfo info)
	{
		this.armed = false;
		Kube.BCS.gameTypeController.SendMessage("BombDisarm", this, SendMessageOptions.DontRequireReceiver);
		if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	private void DoExplosion()
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_DoExplosion", PhotonTargets.All, new object[0]);
		}
		else
		{
			this._DoExplosion(null);
		}
	}

	[PunRPC]
	private void _DoExplosion(PhotonMessageInfo info)
	{
		if (PhotonNetwork.isMasterClient)
		{
			BombController bombController = Kube.BCS.gameTypeController as BombController;
			if (bombController)
			{
				bombController.BombExplode(this);
			}
		}
		this.startExp = true;
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.explosionRadius);
		for (int i = 0; i < array.Length; i++)
		{
			if (this.dm.damage != 0)
			{
				float num = 1f - Vector3.Distance(base.transform.position, array[i].ClosestPointOnBounds(base.transform.position)) / this.explosionRadius;
				DamageMessage damageMessage = new DamageMessage();
				damageMessage.damage = (short)((float)this.dm.damage * num);
				damageMessage.id_killer = this.dm.id_killer;
				damageMessage.weaponType = this.dm.weaponType;
				if (array[i].gameObject.layer == 8 && !this.cubesHearted)
				{
					if (this.NO == null)
					{
						this.NO = Kube.BCS.NO;
					}
					Vector3 a = new Vector3(Mathf.Round(base.transform.position.x), Mathf.Round(base.transform.position.y), Mathf.Round(base.transform.position.z));
					string text = string.Empty;
					string text2 = string.Empty;
					int num2 = 0;
					for (int j = (int)a.x - 7; j <= (int)a.x + 7; j++)
					{
						for (int k = (int)a.y - 7; k <= (int)a.y + 7; k++)
						{
							for (int l = (int)a.z - 7; l <= (int)a.z + 7; l++)
							{
								if (j >= 0 && k >= 0 && l >= 0 && j < Kube.WHS.sizeX && k < Kube.WHS.sizeY && l < Kube.WHS.sizeZ)
								{
									if (Kube.WHS.cubeTypes[j, k, l] != 0)
									{
										float num3 = Vector3.Distance(a, new Vector3((float)j, (float)k, (float)l));
										if (this.explosionRadius > num3)
										{
											int num4 = (int)Mathf.Max(0f, (float)Kube.WHS.cubesDamage[j, k, l] - (float)this.dm.damage * Mathf.Max(0f, 1f - num3 / (this.explosionRadius / 2f)));
											string text3 = text2;
											text2 = string.Concat(new string[]
											{
												text3,
												Kube.OH.GetServerCode(j, 2),
												Kube.OH.GetServerCode(k, 2),
												Kube.OH.GetServerCode(l, 2),
												Kube.OH.GetServerCode(num4, 2)
											});
											num2++;
										}
									}
								}
							}
						}
					}
					text = text + Kube.OH.GetServerCode(num2, 2) + text2;
					this.NO.ChangeCubesHealth(text);
					this.cubesHearted = true;
				}
				else
				{
					array[i].gameObject.transform.root.SendMessage("ApplyDamage", damageMessage, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		array = Physics.OverlapSphere(base.transform.position, this.explosionRadius);
		for (int m = 0; m < array.Length; m++)
		{
			if (array[m].gameObject.GetComponent<Rigidbody>() != null)
			{
				array[m].gameObject.GetComponent<Rigidbody>().AddForceAtPosition(0.01f * (float)this.dm.damage * (array[m].transform.position - base.transform.position).normalized, base.transform.position, ForceMode.Impulse);
			}
		}
		if (this.explosion != null)
		{
			UnityEngine.Object.Instantiate(this.explosion, base.transform.position, base.transform.rotation);
		}
		UnityEngine.Object.Destroy(base.gameObject);
		if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	public void ActivateByPlayer(PlayerScript playerScript)
	{
		this.rechargingWeaponTick = Time.time;
		if (this._init)
		{
			return;
		}
		this._init = true;
		this.rechargingWeaponStart = Time.time;
	}

	public bool CanActivate(PlayerScript playerScript)
	{
		return this.armed && playerScript.team == 1;
	}

	private void OnGUI()
	{
		if (!this._init)
		{
			return;
		}
		KUI.DownScale();
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

	private void AlwaysUpdate()
	{
		if (!this._init)
		{
			return;
		}
		if (Time.time - this.rechargingWeaponTick > 0.5f)
		{
			this._init = false;
			return;
		}
		float num = (Time.time - this.rechargingWeaponStart) / this.reloadTime;
		if (num >= 1f)
		{
			this._init = false;
			this.BombDisarm();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		PlayerScript componentInChildren = other.GetComponentInChildren<PlayerScript>();
		if (componentInChildren == null || componentInChildren.dead)
		{
			return;
		}
		if (componentInChildren.team == 1)
		{
			return;
		}
		componentInChildren.GiveLotOfDrop(new FastInventar[]
		{
			new FastInventar(InventarType.weapons, 73)
		});
		PhotonNetwork.Destroy(base.gameObject);
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
		}
		else
		{
			base.transform.position = (Vector3)stream.ReceiveNext();
		}
	}

	public void Remove()
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_Remove", PhotonTargets.All, new object[0]);
		}
		else
		{
			this._Remove(null);
		}
	}

	[PunRPC]
	private void _Remove(PhotonMessageInfo info)
	{
		this.armed = false;
		this.activated = false;
		if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	public float activationTime = 5f;

	private float activeStartTime;

	public float pickpickTime = 1.5f;

	public float pickpickDist = 6f;

	public float explosionRadius = 4f;

	public float damage = 60f;

	private float timeToExplosion;

	private DamageMessage dm;

	private int playerId;

	public GameObject explosion;

	public float checkDeltaTime = 1f;

	private float lastCheckTime;

	private bool armed = true;

	private bool activated = true;

	private bool startExp;

	private NetworkObjectScript NO;

	private bool cubesHearted;

	private float rechargingWeaponStart;

	private float rechargingWeaponTick;

	private float reloadTime = 10f;

	protected bool _init;

	public float bombCountdownTime = 60f;
}
