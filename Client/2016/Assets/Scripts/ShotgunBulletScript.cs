using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class ShotgunBulletScript : BulletScript
{
	private void Start()
	{
		if (this.sound != null)
		{
			UnityEngine.Object.Instantiate(this.sound, base.transform.position, base.transform.rotation);
		}
		string text = null;
		int num = 0;
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		this.realDamage = (float)((int)this.dm.damage / this.rounds);
		this.roundsDirections = new Vector3[this.rounds];
		this.roundsHitPoints = new Vector3[this.rounds];
		DamageMessage damageMessage = new DamageMessage();
		damageMessage.damage = 0;
		damageMessage.id_killer = this.dm.id_killer;
		damageMessage.weaponType = this.dm.weaponType;
		damageMessage.team = this.dm.team;
		for (int i = 0; i < this.rounds; i++)
		{
			this.roundsDirections[i] = Quaternion.Euler(UnityEngine.Random.insideUnitSphere * this.accuarcy) * base.transform.TransformDirection(Vector3.forward);
			Ray ray = new Ray(base.transform.position, this.roundsDirections[i]);
			int num2 = 38657;
			if (Kube.GPS != null && Kube.BCS.onlineId == this.dm.id_killer)
			{
				num2 -= 512;
			}
			this.roundsHitPoints[i] = Vector3.zero;
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, this.distance, num2))
			{
				this.roundsHitPoints[i] = raycastHit.point;
				if (this.bulletTrace != null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(this.bulletTrace, base.transform.position, Quaternion.identity) as GameObject;
					gameObject.SendMessage("SetBulletTrace", raycastHit.point);
				}
				short num3 = (short)Mathf.RoundToInt(this.realDamage);
				if (raycastHit.distance > this.fatalDistance)
				{
					num3 = (short)Mathf.RoundToInt(this.realDamage / raycastHit.distance);
				}
				if (num3 != 0)
				{
					if (raycastHit.collider.gameObject.layer == 8)
					{
						if (this.dm.weaponType == 0 || this.dm.weaponType == 8 || this.dm.weaponType == 9)
						{
							num3 *= 3;
						}
						Vector3 vector = new Vector3(Mathf.Round(raycastHit.point.x - raycastHit.normal.x * 0.02f), Mathf.Round(raycastHit.point.y - raycastHit.normal.y * 0.02f), Mathf.Round(raycastHit.point.z - raycastHit.normal.z * 0.02f));
						int num4 = Kube.WHS.cubesDamage[(int)vector.x, (int)vector.y, (int)vector.z];
						text = string.Concat(new string[]
						{
							Kube.OH.GetServerCode((int)vector.x, 2),
							string.Empty,
							Kube.OH.GetServerCode((int)vector.y, 2),
							string.Empty,
							Kube.OH.GetServerCode((int)vector.z, 2)
						});
						if (dictionary.ContainsKey(text))
						{
							num4 = dictionary[text];
						}
						num++;
						num4 = (int)Mathf.Max(0f, (float)num4 - (float)num3 / 5f);
						dictionary[text] = num4;
					}
					else
					{
						damageMessage.damage = num3;
						raycastHit.collider.gameObject.SendMessageUpwards("ApplyDamage", damageMessage, SendMessageOptions.DontRequireReceiver);
						Pawn component;
						if ((component = raycastHit.collider.gameObject.GetComponent<Pawn>()) != null)
						{
							Kube.OH.PlayerBlood(raycastHit.point, raycastHit.normal, component);
						}
					}
				}
				if (raycastHit.collider.gameObject.layer == 8)
				{
					Vector3 vector2 = new Vector3(Mathf.Round(raycastHit.point.x - raycastHit.normal.x * 0.02f), Mathf.Round(raycastHit.point.y - raycastHit.normal.y * 0.02f), Mathf.Round(raycastHit.point.z - raycastHit.normal.z * 0.02f));
					Kube.WHS.PlayCubeHit(vector2, SoundHitType.bullet);
					Kube.WHS.PlayCubeSparks(vector2, raycastHit.point, raycastHit.normal, SoundHitType.bullet);
				}
			}
			else if (this.bulletTrace != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(this.bulletTrace, base.transform.position, Quaternion.identity) as GameObject;
				gameObject2.SendMessage("SetBulletTrace", ray.origin + ray.direction * this.distance);
			}
		}
		base.Invoke("ForceBackRigidbodies", 0.05f);
		if (num > 0)
		{
			text = string.Empty;
			num = 0;
			foreach (KeyValuePair<string, int> keyValuePair in dictionary)
			{
				text = text + keyValuePair.Key + string.Empty + Kube.OH.GetServerCode(keyValuePair.Value, 2);
				num++;
			}
			text = Kube.OH.GetServerCode(num, 2) + string.Empty + text;
			Kube.BCS.NO.ChangeCubesHealth(text);
		}
	}

	private void ForceBackRigidbodies()
	{
		for (int i = 0; i < this.rounds; i++)
		{
			if (!(this.roundsHitPoints[i] == Vector3.zero))
			{
				Collider[] array = Physics.OverlapSphere(this.roundsHitPoints[i], 0.25f);
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].gameObject.GetComponent<Rigidbody>() != null)
					{
						array[j].gameObject.GetComponent<Rigidbody>().AddForceAtPosition(this.roundsDirections[i].normalized * 10f, this.roundsHitPoints[i], ForceMode.Impulse);
					}
				}
			}
		}
	}

	public int rounds = 8;

	private Vector3[] roundsDirections;

	private Vector3[] roundsHitPoints;

	private float realDamage;
}
