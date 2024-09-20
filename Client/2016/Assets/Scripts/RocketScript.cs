using System;
using kube;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
	private void SetDamageParam(DamageMessage _dm)
	{
		this.dm = new DamageMessage();
		this.dm.damage = _dm.damage;
		this.dm.id_killer = _dm.id_killer;
		this.dm.weaponType = _dm.weaponType;
		this.dm.team = _dm.team;
		this.dm.attacker = _dm.attacker;
	}

	protected void Start()
	{
		base.transform.parent = null;
		if (!base.GetComponent<Rigidbody>().useGravity)
		{
			this.flyDirection = base.transform.TransformDirection(Vector3.forward);
		}
		else
		{
			base.GetComponent<Rigidbody>().AddForce(base.transform.TransformDirection(Vector3.forward) * this.flySpeed * base.GetComponent<Rigidbody>().mass, ForceMode.Impulse);
		}
	}

	private void Update()
	{
		if (!base.GetComponent<Rigidbody>().useGravity)
		{
			base.transform.position += this.flyDirection * this.flySpeed * Time.deltaTime;
			if (this.rocketView != null)
			{
				this.rocketView.transform.Rotate(0f, 0f, 500f * Time.deltaTime);
			}
		}
		else if (this.rocketView != null)
		{
			this.rocketView.transform.rotation = Quaternion.LookRotation(base.GetComponent<Rigidbody>().velocity);
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		if (this.dead)
		{
			return;
		}
		Collider[] array;
		if (!this.isElectro)
		{
			array = Physics.OverlapSphere(base.transform.position, this.explosionRadius);
		}
		else
		{
			array = new Collider[]
			{
				col.collider
			};
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (this.dm.damage != 0)
			{
				float num = 1f - Vector3.Distance(base.transform.position, array[i].ClosestPointOnBounds(base.transform.position)) / this.explosionRadius;
				DamageMessage damageMessage = new DamageMessage();
				damageMessage.damage = (short)((float)this.dm.damage * num);
				damageMessage.id_killer = this.dm.id_killer;
				damageMessage.weaponType = this.dm.weaponType;
				damageMessage.team = this.dm.team;
				if (array[i].gameObject.layer == 8 && !this.cubesHearted && !this.isElectro)
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
					array[i].gameObject.SendMessageUpwards("ApplyDamage", damageMessage, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		array = Physics.OverlapSphere(base.transform.position, this.explosionRadius);
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
		this.dead = true;
	}

	public DamageMessage dm;

	public float flySpeed;

	public GameObject explosion;

	public bool isElectro;

	public float explosionRadius = 10f;

	public GameObject rocketView;

	private NetworkObjectScript NO;

	public Vector3 flyDirection;

	private bool cubesHearted;

	private bool dead;
}
