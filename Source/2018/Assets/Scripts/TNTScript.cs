using System;
using kube;
using Photon;
using UnityEngine;

public class TNTScript : Photon.MonoBehaviour
{
	private void SetPlayerId(int _id)
	{
		this.playerId = _id;
	}

	private void Start()
	{
		this.timeToExplosion = Time.time + this.explosionTime;
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
	}

	private void Update()
	{
		if (base.photonView.isMine && Time.time > this.timeToExplosion)
		{
			this.DoExplosion();
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
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.explosionRadius);
		for (int i = 0; i < array.Length; i++)
		{
			if (this.dm.damage != 0)
			{
				float num = 1f - Vector3.Distance(base.transform.position, array[i].ClosestPointOnBounds(base.transform.position)) / this.explosionRadius;
				DamageMessage damageMessage = new DamageMessage();
				damageMessage.damage = (short)((float)this.dm.damage * num);
				damageMessage.id_killer = this.dm.id_killer;
				damageMessage.weaponType = -3;
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

	public float explosionTime = 5f;

	public float explosionRadius = 4f;

	public float damage = 60f;

	private float timeToExplosion;

	private DamageMessage dm;

	private int playerId;

	public GameObject explosion;

	private NetworkObjectScript NO;

	private bool cubesHearted;
}
