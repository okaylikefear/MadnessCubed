using System;
using System.Collections;
using kube;
using UnityEngine;

public class SimpleTurelScript : TransportScript
{
	public override void TransportInit()
	{
	}

	private void DriveCar()
	{
		if (Kube.IS.ps.paused)
		{
			return;
		}
		if (UnityEngine.Input.GetAxis("Fire1") > 0f && !Kube.IS.ps.paused && this.overHeat < 1f && Time.time - this.lastShootTime > this.shootDeltaTime)
		{
			this.lastShootTime = Time.time;
			Ray ray = Kube.IS.ps.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			this.CreateShot(ray.origin, ray.direction);
			this.overHeat += 0.1f;
			if (this.overHeat > 1f)
			{
				this.overHeat += 0.3f;
			}
		}
		float y = this.platform.transform.rotation.eulerAngles.y + UnityEngine.Input.GetAxis("Mouse X") * 4f;
		float num = this.platform.transform.rotation.eulerAngles.z - UnityEngine.Input.GetAxis("Mouse Y") * 4f;
		if (num > 180f)
		{
			num -= 360f;
		}
		num = Mathf.Max(-35f, num);
		num = Mathf.Min(35f, num);
		this.platform.transform.rotation = Quaternion.Euler(0f, y, num);
	}

	private void CreateShot(Vector3 rayOrigin, Vector3 rayDirection)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_CreateShot", PhotonTargets.All, new object[]
			{
				rayOrigin,
				rayDirection
			});
		}
		else
		{
			this._CreateShot(rayOrigin, rayDirection, null);
		}
	}

	[RPC]
	private void _CreateShot(Vector3 rayOrigin, Vector3 rayDirection, PhotonMessageInfo info)
	{
		int num = 38657;
		this.lastShootTime = Time.time;
		if (PhotonNetwork.offlineMode || base.photonView.isMine)
		{
			num -= 512;
		}
		Ray ray = new Ray(rayOrigin, rayDirection);
		RaycastHit raycastHit;
		Vector3 shotPoint;
		if (Physics.Raycast(ray, out raycastHit, this.distance, num))
		{
			shotPoint = raycastHit.point;
		}
		else
		{
			shotPoint = ray.origin + ray.direction * this.distance;
		}
		DamageMessage damageMessage = new DamageMessage();
		if (Kube.GPS.playerId == this.driversId[0])
		{
			damageMessage.damage = (short)this.damage;
		}
		else
		{
			damageMessage.damage = 0;
		}
		damageMessage.id_killer = Kube.GPS.playerId;
		damageMessage.team = Kube.IS.ps.team;
		damageMessage.weaponType = 0;
		base.StartCoroutine(this.CreateBullet(shotPoint, damageMessage));
	}

	private IEnumerator CreateBullet(Vector3 shotPoint, DamageMessage dm)
	{
		for (int i = 0; i < this.turrelShoot.Length; i++)
		{
			if (i != 0)
			{
				yield return new WaitForSeconds(this.shootDeltaTime / (float)this.turrelShoot.Length);
			}
			GameObject bullet = UnityEngine.Object.Instantiate(this.bulletPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			bullet.transform.position = this.turrelShoot[i].transform.position;
			bullet.transform.LookAt(shotPoint);
			bullet.SendMessage("SetDamageParam", dm);
			this.turrelShoot[i].audio.Play();
		}
		yield break;
	}

	public override void TransportDrive(int numDriver)
	{
		if (numDriver == 0)
		{
			this.DriveCar();
		}
	}

	public override void TransportUpdate(int numPlace)
	{
		if ((this.driversId[0] != 0 || !base.photonView.isMine) && numPlace != 0)
		{
			this.platform.transform.rotation = Quaternion.Slerp(this.platform.transform.rotation, this.newRotation, Time.deltaTime * (float)PhotonNetwork.sendRateOnSerialize);
			if (this.flagNewData)
			{
				this.overHeat = this.newOverHeat;
			}
		}
		else
		{
			this.newRotation = this.platform.transform.rotation;
			this.newOverHeat = this.overHeat;
		}
		if (Time.time - this.lastShootTime < this.shootDeltaTime)
		{
			for (int i = 0; i < this.turrelGun.Length; i++)
			{
				this.turrelGun[i].transform.RotateAroundLocal(Vector3.right, Time.deltaTime * 10f);
			}
		}
		if (this.overHeat > 0f)
		{
			this.overHeat -= Time.deltaTime * 0.15f;
		}
	}

	public override void AnimateDriver(int numDriver, PlayerScript driver)
	{
		driver.animation.CrossFade(driver.animQuadroSit);
	}

	public override void LateAnimateDriver(int numDriver, PlayerScript driver)
	{
	}

	public override void SerializeWrite(PhotonStream stream)
	{
	}

	public override void SerializeRead(PhotonStream stream)
	{
	}

	public override void NetSender(int numPlace)
	{
		if (numPlace == 0 && PhotonNetwork.room != null)
		{
			base.photonView.RPC("_NS", PhotonTargets.Others, new object[]
			{
				this.platform.transform.rotation,
				this.overHeat
			});
		}
	}

	[RPC]
	public void _NS(Quaternion _newRotation, float _newOverHeat)
	{
		this.newOverHeat = _newOverHeat;
		this.newRotation = _newRotation;
		this.flagNewData = true;
	}

	public override void TransportGUI(int numPlace)
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		if (numPlace == 0)
		{
			GUI.DrawTexture(new Rect(0.5f * num - 175f, 0.8f * num2, 350f, 32f), Kube.ASS3.progressBar_gray);
			if (this.overHeat < 0.8f)
			{
				GUI.DrawTexture(new Rect(0.5f * num - 173f, 0.8f * num2 + 2f, 346f * this.overHeat, 28f), Kube.ASS3.progressBar_green);
			}
			else
			{
				Color color = GUI.color;
				GUI.color = new Color(Mathf.Sin(Time.time * 10f) * 0.5f + 0.5f, 1f, 1f, 1f);
				GUI.DrawTexture(new Rect(0.5f * num - 173f, 0.8f * num2 + 2f, 346f * Mathf.Min(1f, this.overHeat), 28f), Kube.ASS3.progressBar_red);
				GUI.color = color;
			}
		}
	}

	public GameObject platform;

	public GameObject[] turrelGun;

	public GameObject[] turrelShoot;

	public float damage = 20f;

	public float distance = 1000f;

	public float shootDeltaTime = 0.5f;

	private float lastShootTime;

	public GameObject bulletPrefab;

	private float overHeat;

	private Quaternion newRotation;

	private float newOverHeat;

	private bool flagNewData;
}
