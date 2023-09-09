using System;
using kube;
using Photon;
using UnityEngine;

public class NetGrenadeScript : Photon.MonoBehaviour
{
	[RPC]
	public void _Throw(Vector3 vec, PhotonMessageInfo info)
	{
		base.gameObject.rigidbody.AddForce(vec, ForceMode.Impulse);
	}

	public void Throw(Vector3 vec)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_Throw", PhotonTargets.All, new object[]
			{
				vec
			});
		}
		else
		{
			this._Throw(vec, null);
		}
	}

	private void SetDamageParam(DamageMessage _dm)
	{
		this.dm = new DamageMessage();
		this.dm.damage = _dm.damage;
		this.dm.id_killer = _dm.id_killer;
		this.dm.weaponType = _dm.weaponType;
		this.dm.team = _dm.team;
	}

	private void Start()
	{
		this.correctPlayerPos = base.transform.position;
		this.correctPlayerRot = base.transform.rotation;
		base.Invoke("Detonate", 3f);
	}

	private void Update()
	{
		if (!base.photonView.isMine)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.correctPlayerPos, Time.deltaTime * 10f);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.correctPlayerRot, Time.deltaTime * 10f);
		}
	}

	[RPC]
	private void _Explode(Vector3 pos, PhotonMessageInfo info)
	{
		if (this.explosion != null)
		{
			UnityEngine.Object.Instantiate(this.explosion, pos, base.transform.rotation);
		}
		PhotonNetwork.Destroy(base.gameObject);
	}

	private void Detonate()
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_Explode", PhotonTargets.All, new object[]
			{
				base.transform.position
			});
		}
		else
		{
			this._Explode(base.transform.position, null);
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.explosionRadius);
		for (int i = 0; i < array.Length; i++)
		{
			float num = 1f - Vector3.Distance(base.transform.position, array[i].ClosestPointOnBounds(base.transform.position)) / this.explosionRadius;
			DamageMessage damageMessage = new DamageMessage();
			damageMessage.damage = (short)((float)this.dm.damage * num);
			damageMessage.id_killer = this.dm.id_killer;
			damageMessage.weaponType = this.dm.weaponType;
			damageMessage.team = this.dm.team;
			if (array[i].gameObject.layer == 8 && !this.cubesHearted)
			{
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
								if (Kube.WHS.cubes[j, k, l].type != 0)
								{
									float num3 = Vector3.Distance(a, new Vector3((float)j, (float)k, (float)l));
									if (this.explosionRadius > num3)
									{
										int num4 = (int)Mathf.Max(0f, (float)Kube.WHS.cubes[j, k, l].health - (float)this.dm.damage * Mathf.Max(0f, 1f - num3 / (this.explosionRadius / 2f)));
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
				Kube.BCS.NO.ChangeCubesHealth(text);
				this.cubesHearted = true;
			}
			else
			{
				array[i].gameObject.transform.root.SendMessage("ApplyDamage", damageMessage, SendMessageOptions.DontRequireReceiver);
			}
		}
		array = Physics.OverlapSphere(base.transform.position, this.explosionRadius);
		for (int m = 0; m < array.Length; m++)
		{
			if (array[m].gameObject.rigidbody != null)
			{
				array[m].gameObject.rigidbody.AddForceAtPosition(0.01f * (float)this.dm.damage * (array[m].transform.position - base.transform.position).normalized, base.transform.position, ForceMode.Impulse);
			}
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (PhotonNetwork.connected)
		{
			if (stream.isWriting)
			{
				stream.SendNext(base.transform.position);
				stream.SendNext(base.transform.rotation);
			}
			else
			{
				this.correctPlayerPos = (Vector3)stream.ReceiveNext();
				this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
			}
		}
	}

	[HideInInspector]
	public DamageMessage dm;

	public float flySpeed;

	public GameObject explosion;

	public bool isElectro;

	public float explosionRadius = 10f;

	public GameObject rocketView;

	private bool cubesHearted;

	protected Vector3 correctPlayerPos = new Vector3(-10000f, -10000f, 0f);

	protected Quaternion correctPlayerRot = Quaternion.identity;
}
