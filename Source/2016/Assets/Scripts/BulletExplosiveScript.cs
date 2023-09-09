using System;
using kube;
using UnityEngine;

public class BulletExplosiveScript : MonoBehaviour
{
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
		this.cubesHearted = false;
		if (this.sound != null)
		{
			UnityEngine.Object.Instantiate(this.sound, base.transform.position, base.transform.rotation);
		}
		Ray ray = new Ray(base.transform.position, Quaternion.Euler(UnityEngine.Random.insideUnitSphere * this.accuarcy) * base.transform.TransformDirection(Vector3.forward));
		this.hitDir = ray.direction;
		int num = 38657;
		if (Kube.BCS != null && Kube.BCS.onlineId == this.dm.id_killer)
		{
			num -= 512;
		}
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, this.distance, num))
		{
			this.hitPos = raycastHit.point;
			if (this.bulletTrace != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(this.bulletTrace, base.transform.position, base.transform.rotation) as GameObject;
				gameObject.SendMessage("SetBulletTrace", raycastHit.point);
			}
			if (this.sparkles != null)
			{
				UnityEngine.Object.Instantiate(this.sparkles, raycastHit.point, Quaternion.FromToRotation(Vector3.forward, raycastHit.normal));
			}
			Collider[] array = Physics.OverlapSphere(raycastHit.point, this.explosionRadius);
			for (int i = 0; i < array.Length; i++)
			{
				if (this.dm.damage != 0)
				{
					float num2 = 1f - Vector3.Distance(raycastHit.point, array[i].ClosestPointOnBounds(raycastHit.point)) / this.explosionRadius;
					DamageMessage damageMessage = new DamageMessage();
					damageMessage.damage = (short)((float)this.dm.damage * num2);
					damageMessage.id_killer = this.dm.id_killer;
					damageMessage.weaponType = this.dm.weaponType;
					damageMessage.team = this.dm.team;
					if (array[i].gameObject.layer == 8 && !this.cubesHearted)
					{
						if (this.NO == null)
						{
							this.NO = Kube.BCS.NO;
						}
						Vector3 a = new Vector3(Mathf.Round(raycastHit.point.x), Mathf.Round(raycastHit.point.y), Mathf.Round(raycastHit.point.z));
						string text = string.Empty;
						string text2 = string.Empty;
						int num3 = 0;
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
											float num4 = Vector3.Distance(a, new Vector3((float)j, (float)k, (float)l));
											if (this.explosionRadius > num4)
											{
												int num5 = (int)Mathf.Max(0f, (float)Kube.WHS.cubesHealth[Kube.WHS.cubeTypes[j, k, l]] - (float)this.dm.damage * Mathf.Max(0f, 1f - num4 / (this.explosionRadius / 2f)));
												string text3 = text2;
												text2 = string.Concat(new string[]
												{
													text3,
													Kube.OH.GetServerCode(j, 2),
													Kube.OH.GetServerCode(k, 2),
													Kube.OH.GetServerCode(l, 2),
													Kube.OH.GetServerCode(num5, 2)
												});
												num3++;
											}
										}
									}
								}
							}
						}
						text = text + Kube.OH.GetServerCode(num3, 2) + text2;
						this.NO.ChangeCubesHealth(text);
						this.cubesHearted = true;
					}
					else
					{
						array[i].gameObject.SendMessageUpwards("ApplyDamage", damageMessage, SendMessageOptions.DontRequireReceiver);
					}
				}
				if (array[i].gameObject.GetComponent<Rigidbody>() != null)
				{
					if (this.dm.damage != 0)
					{
						array[i].gameObject.GetComponent<Rigidbody>().AddForceAtPosition(0.01f * (float)this.dm.damage * (array[i].transform.position - raycastHit.point).normalized, raycastHit.point, ForceMode.Impulse);
					}
					else
					{
						array[i].gameObject.GetComponent<Rigidbody>().AddForceAtPosition(0.5f * (array[i].transform.position - raycastHit.point).normalized, raycastHit.point, ForceMode.Impulse);
					}
				}
			}
		}
		else if (this.bulletTrace != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(this.bulletTrace, base.transform.position, Quaternion.identity) as GameObject;
			gameObject2.SendMessage("SetBulletTrace", ray.origin + ray.direction * this.distance);
		}
		base.Invoke("ForceBackRigidbodies", 0.05f);
	}

	private void ForceBackRigidbodies()
	{
		if (this.hitPos == Vector3.zero)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(this.hitPos, 0.25f);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject.GetComponent<Rigidbody>() != null)
			{
				array[i].gameObject.GetComponent<Rigidbody>().AddForceAtPosition(this.hitDir * 10f, this.hitPos, ForceMode.Impulse);
			}
		}
	}

	private void Update()
	{
	}

	public DamageMessage dm;

	public GameObject sparkles;

	public float distance = 1000f;

	public GameObject sound;

	public GameObject bulletTrace;

	public float accuarcy;

	public float fellBack = 3f;

	public float explosionRadius = 2f;

	private bool cubesHearted;

	protected NetworkObjectScript NO;

	private Vector3 hitPos;

	private Vector3 hitDir;
}
