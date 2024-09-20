using System;
using kube;
using UnityEngine;

public class FreezeBullet : MonoBehaviour
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
				gameObject.transform.LookAt(this.hitPos);
				if (this.traceFollowWeapon)
				{
					gameObject.transform.parent = base.transform;
				}
				gameObject.SendMessage("SetBulletTrace", raycastHit.point);
			}
			if (this.sparkles != null)
			{
				UnityEngine.Object.Instantiate(this.sparkles, raycastHit.point, Quaternion.FromToRotation(Vector3.forward, raycastHit.normal));
			}
			if (this.dm.damage != 0)
			{
				if (this.fatalDistance != this.distance && raycastHit.distance > this.fatalDistance)
				{
					this.dm.damage = (short)Mathf.RoundToInt((float)this.dm.damage / (raycastHit.distance - this.fatalDistance));
				}
				if (raycastHit.collider.gameObject.layer == 8)
				{
					if (this.dm.weaponType == 0 || this.dm.weaponType == 8 || this.dm.weaponType == 9)
					{
						DamageMessage damageMessage = this.dm;
						damageMessage.damage *= 3;
					}
					Vector3 vector = new Vector3(Mathf.Round(raycastHit.point.x - raycastHit.normal.x * 0.02f), Mathf.Round(raycastHit.point.y - raycastHit.normal.y * 0.02f), Mathf.Round(raycastHit.point.z - raycastHit.normal.z * 0.02f));
					int num2 = (int)Mathf.Max(0f, (float)Kube.WHS.cubesDamage[(int)vector.x, (int)vector.y, (int)vector.z] - (float)this.dm.damage / 5f);
					string cubesToChange = string.Concat(new string[]
					{
						Kube.OH.GetServerCode(1, 2),
						string.Empty,
						Kube.OH.GetServerCode((int)vector.x, 2),
						string.Empty,
						Kube.OH.GetServerCode((int)vector.y, 2),
						string.Empty,
						Kube.OH.GetServerCode((int)vector.z, 2),
						string.Empty,
						Kube.OH.GetServerCode(num2, 2)
					});
					this.NO = Kube.BCS.NO;
					this.NO.ChangeCubesHealth(cubesToChange);
				}
				else
				{
					this.dm.damagePos = raycastHit.point;
					raycastHit.collider.gameObject.SendMessageUpwards("ApplyDamage", this.dm, SendMessageOptions.DontRequireReceiver);
					Pawn component;
					if ((component = raycastHit.collider.gameObject.GetComponent<Pawn>()) != null)
					{
						Kube.OH.PlayerBlood(raycastHit.point, raycastHit.normal, component);
					}
					FreezeStruct freezeStruct;
					freezeStruct.freezeTime = this.freezeTime;
					freezeStruct.team = this.dm.team;
					raycastHit.collider.transform.root.SendMessageUpwards("Freeze", freezeStruct, SendMessageOptions.DontRequireReceiver);
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
			GameObject gameObject2 = UnityEngine.Object.Instantiate(this.bulletTrace, base.transform.position, base.transform.rotation) as GameObject;
			gameObject2.SendMessage("SetBulletTrace", ray.origin + ray.direction * this.distance);
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

	public bool traceFollowWeapon;

	public float accuarcy;

	public float fellBack = 3f;

	public float fatalDistance = 1000f;

	protected NetworkObjectScript NO;

	private Vector3 hitPos;

	private Vector3 hitDir;

	public float freezeTime = 1f;
}
