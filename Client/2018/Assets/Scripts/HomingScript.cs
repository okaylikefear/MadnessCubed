using System;
using kube;
using UnityEngine;

public class HomingScript : TransportScript
{
	protected override void TransportStart()
	{
		this.velocity = base.transform.forward;
		this._damageDrivers = false;
	}

	public override void AppplyPosition(Transform transform, int transportToDrivePlace)
	{
	}

	public override bool isDisableController(int transportToDrivePlace)
	{
		return false;
	}

	public override bool isHideWeapon(int transportToDrivePlace)
	{
		return false;
	}

	public override void SetView(int numPlace, bool view3face)
	{
		if (this.driversGO[numPlace] != Kube.BCS.ps.gameObject)
		{
			view3face = true;
		}
		this._smr.enabled = view3face;
	}

	public override bool HasFPS(int transportToDrivePlace)
	{
		return false;
	}

	public override void TransportInit()
	{
		this.correctPlayerPos = base.transform.localPosition;
		this.correctPlayerRot = base.transform.localRotation;
	}

	protected override void TransportAwake()
	{
		this._smr = base.GetComponentInChildren<MeshRenderer>();
	}

	public override void TransportLateUpdate(int numPlace)
	{
		if (numPlace == -1)
		{
			return;
		}
		Kube.IS.ps.cameraComp.transform.position = this.driverFPSCameraTransform[numPlace].position;
		Kube.IS.ps.cameraComp.transform.rotation = this.driverFPSCameraTransform[numPlace].rotation;
	}

	protected override void TransportSlave()
	{
		base.transform.rotation = Quaternion.LookRotation(this.velocity);
	}

	public override void TransportDrive(int numDriver)
	{
		if (Kube.IS.ps.paused)
		{
			return;
		}
		PlayerScript ps = Kube.BCS.ps;
		this.correctPlayerPos = base.transform.position;
		this.correctPlayerRot = base.transform.rotation;
		Quaternion quaternion = Quaternion.LookRotation(this.velocity);
		float num = UnityEngine.Input.GetAxis("Mouse X") * ps.sensitivityX;
		float num2 = UnityEngine.Input.GetAxis("Mouse Y") * ps.sensitivityY;
		Vector3 eulerAngles = quaternion.eulerAngles;
		eulerAngles.x -= num2;
		eulerAngles.y += num;
		Vector3 forward = Quaternion.Euler(eulerAngles) * Vector3.forward;
		this.velocity = forward.normalized;
		base.transform.rotation = Quaternion.LookRotation(forward);
	}

	public override void TransportUpdate(int numPlace)
	{
		base.transform.position += this.velocity * this.flySpeed * Time.deltaTime;
	}

	public override void AnimateDriver(int numDriver, PlayerScript driver)
	{
	}

	public override void LateAnimateDriver(int numDriver, PlayerScript driver)
	{
	}

	public override void NetSender(int numPlace)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_NS0", PhotonTargets.Others, new object[]
			{
				base.transform.position,
				base.transform.rotation,
				false
			});
		}
	}

	[PunRPC]
	public void _NS0(Vector3 _newPosition, Quaternion _newRotation, bool jump)
	{
		this.correctPlayerPos = _newPosition;
		this.correctPlayerRot = _newRotation;
		this.velocity = _newPosition - base.transform.position;
	}

	private void SetDamageParam(DamageMessage _dm)
	{
		this.dm = new DamageMessage();
		this.dm.damage = _dm.damage;
		this.dm.id_killer = _dm.id_killer;
		this.dm.weaponType = _dm.weaponType;
		this.dm.team = _dm.team;
		this.dm.attacker = _dm.attacker;
	}

	private void OnCollisionEnter(Collision col)
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		if (this.isDead)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, (float)this.explosionRadius);
		bool flag = false;
		NetworkObjectScript no = Kube.BCS.NO;
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i].transform.root == base.transform.root))
			{
				if (this.dm.damage != 0)
				{
					float num = 1f - Vector3.Distance(base.transform.position, array[i].ClosestPointOnBounds(base.transform.position)) / (float)this.explosionRadius;
					DamageMessage damageMessage = new DamageMessage();
					damageMessage.damage = (short)((float)this.dm.damage * num);
					damageMessage.id_killer = this.dm.id_killer;
					damageMessage.weaponType = this.dm.weaponType;
					damageMessage.team = this.dm.team;
					if (array[i].gameObject.layer == 8 && !flag)
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
										if (Kube.WHS.cubeTypes[j, k, l] != 0)
										{
											float num3 = Vector3.Distance(a, new Vector3((float)j, (float)k, (float)l));
											if ((float)this.explosionRadius > num3)
											{
												int num4 = (int)Mathf.Max(0f, (float)Kube.WHS.cubesDamage[j, k, l] - (float)this.dm.damage * Mathf.Max(0f, 1f - num3 / ((float)this.explosionRadius / 2f)));
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
						no.ChangeCubesHealth(text);
						flag = true;
					}
					else
					{
						array[i].gameObject.SendMessageUpwards("ApplyDamage", damageMessage, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}
		array = Physics.OverlapSphere(base.transform.position, (float)this.explosionRadius);
		for (int m = 0; m < array.Length; m++)
		{
			if (array[m].gameObject.GetComponent<Rigidbody>() != null)
			{
				if (this.dm.damage != 0)
				{
					array[m].gameObject.GetComponent<Rigidbody>().AddForceAtPosition(0.01f * (float)this.dm.damage * (array[m].transform.position - base.transform.position).normalized, base.transform.position, ForceMode.Impulse);
				}
				else
				{
					array[m].gameObject.GetComponent<Rigidbody>().AddForceAtPosition(1f * (array[m].transform.position - base.transform.position).normalized, base.transform.position, ForceMode.Impulse);
				}
			}
		}
		if (this.explosion != null)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.explosion, base.transform.position, base.transform.rotation);
			gameObject.SendMessage("SetDamageParam", this.dm, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			col.collider.gameObject.SendMessage("PushChar", base.transform.TransformDirection(Vector3.forward).normalized * (float)this.dm.damage * 0.5f, SendMessageOptions.DontRequireReceiver);
		}
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
		for (int n = 0; n < componentsInChildren.Length; n++)
		{
			componentsInChildren[n].transform.parent = null;
			componentsInChildren[n].enableEmission = false;
			MeshRenderer[] componentsInChildren2 = componentsInChildren[n].GetComponentsInChildren<MeshRenderer>();
			for (int num5 = 0; num5 < componentsInChildren2.Length; num5++)
			{
				componentsInChildren2[num5].enabled = false;
			}
			UnityEngine.Object.Destroy(componentsInChildren[n].gameObject, 5f);
		}
		UnityEngine.Object.Destroy(base.gameObject, 0.1f);
		this.isDead = true;
	}

	public float distance = 1000f;

	private float meanRPM;

	protected MeshRenderer _smr;

	public float flySpeed = 5f;

	[HideInInspector]
	public Vector3 velocity;

	private Vector3 correctPlayerPos;

	private Quaternion correctPlayerRot;

	public DamageMessage dm;

	public int explosionRadius = 10;

	public GameObject explosion;
}
