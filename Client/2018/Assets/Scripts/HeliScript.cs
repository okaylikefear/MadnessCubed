using System;
using kube;
using UnityEngine;

public class HeliScript : TransportScript
{
	public override void TransportInit()
	{
		base.gameObject.GetComponent<Rigidbody>().centerOfMass = this.centerOfMass;
	}

	private void ShootHeli()
	{
		if (UnityEngine.Input.GetAxis("Fire1") > 0f && !Kube.IS.ps.paused && Time.time - this.lastShootTime > this.shootDeltaTime)
		{
			this.lastShootTime = Time.time;
			int num = UnityEngine.Random.Range(0, this.turrelGun.Length);
			this.CreateShot(this.turrelGun[num].transform.position, this.turrelGun[num].transform.forward);
		}
	}

	private void DriveHeli()
	{
		if (Kube.IS.ps.paused)
		{
			return;
		}
		if (this.turrelGun.Length > 0)
		{
			this.ShootHeli();
		}
		if (UnityEngine.Input.GetAxis("Jump") > 0f && base.transform.position.y < (float)(Kube.WHS.sizeY - 10) && base.GetComponent<Rigidbody>().velocity.y < 10f)
		{
			base.GetComponent<Rigidbody>().AddRelativeForce(base.transform.up * this.enginePower);
		}
		float num = 0f;
		float axis = UnityEngine.Input.GetAxis("Vertical");
		if (UnityEngine.Input.GetKey(KeyCode.Q))
		{
			num = -1f;
		}
		else if (UnityEngine.Input.GetKey(KeyCode.E))
		{
			num = 1f;
		}
		float axis2 = UnityEngine.Input.GetAxis("Horizontal");
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f), Time.deltaTime * 2f);
		if (Mathf.Abs(num) > 0.01f)
		{
			base.GetComponent<Rigidbody>().AddTorque(base.transform.forward * 0.5f * num * this.pushPower * Time.deltaTime);
		}
		if (Mathf.Abs(axis) > 0.01f)
		{
			base.GetComponent<Rigidbody>().AddTorque(base.transform.right * axis * this.pushPower * Time.deltaTime);
		}
		if (Mathf.Abs(axis2) > 0.01f)
		{
			base.GetComponent<Rigidbody>().AddTorque(Vector3.up * axis2 * this.rotSpeed * Time.deltaTime);
		}
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

	[PunRPC]
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
		Vector3 worldPosition;
		if (Physics.Raycast(ray, out raycastHit, this.distance, num))
		{
			worldPosition = raycastHit.point;
		}
		else
		{
			worldPosition = ray.origin + ray.direction * this.distance;
		}
		DamageMessage damageMessage = new DamageMessage();
		if (Kube.BCS.onlineId == this.driversId[0])
		{
			damageMessage.damage = (short)this.damage;
		}
		else
		{
			damageMessage.damage = 0;
		}
		damageMessage.id_killer = Kube.BCS.onlineId;
		damageMessage.team = Kube.IS.ps.team;
		damageMessage.weaponType = 0;
		GameObject gameObject = UnityEngine.Object.Instantiate(this.bulletPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.position = ray.origin;
		gameObject.transform.LookAt(worldPosition);
		gameObject.SendMessage("SetDamageParam", damageMessage);
		this.turrelShoot.Play();
	}

	public override void TransportDrive(int numDriver)
	{
		if (numDriver == 0)
		{
			this.DriveHeli();
		}
	}

	public override void TransportUpdate(int numPlace)
	{
		this.spinner1.transform.RotateAroundLocal(Vector3.up, Time.deltaTime * 10f);
		this.spinner2.transform.RotateAroundLocal(Vector3.forward, Time.deltaTime * 10f);
		if ((this.driversId[0] != 0 || !base.photonView.isMine) && numPlace != 0)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.newPosition, Time.deltaTime * (float)PhotonNetwork.sendRateOnSerialize);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.newRotation, Time.deltaTime * (float)PhotonNetwork.sendRateOnSerialize);
			if (this.flagNewVelocities)
			{
				base.GetComponent<Rigidbody>().velocity = this.newVelocity;
				base.GetComponent<Rigidbody>().angularVelocity = this.newAngularVelocity;
				this.flagNewVelocities = false;
			}
		}
		else
		{
			this.newPosition = base.transform.position;
			this.newRotation = base.transform.rotation;
		}
	}

	public override void AnimateDriver(int numDriver, PlayerScript driver)
	{
		driver.GetComponent<Animation>().CrossFade(driver.animQuadroSit);
	}

	public override void LateAnimateDriver(int numDriver, PlayerScript driver)
	{
	}

	public override void SerializeWrite(PhotonStream stream)
	{
		stream.SendNext(base.transform.position);
		stream.SendNext(base.transform.rotation);
		stream.SendNext(base.GetComponent<Rigidbody>().velocity);
		stream.SendNext(base.GetComponent<Rigidbody>().angularVelocity);
	}

	public override void SerializeRead(PhotonStream stream)
	{
		this.newPosition = (Vector3)stream.ReceiveNext();
		this.newRotation = (Quaternion)stream.ReceiveNext();
		this.newVelocity = (Vector3)stream.ReceiveNext();
		this.newAngularVelocity = (Vector3)stream.ReceiveNext();
		this.flagNewVelocities = true;
	}

	public override void NetSender(int numPlace)
	{
		if (numPlace == 0 && PhotonNetwork.room != null)
		{
			base.photonView.RPC("_NS", PhotonTargets.Others, new object[]
			{
				base.transform.position,
				base.transform.rotation,
				base.GetComponent<Rigidbody>().velocity,
				base.GetComponent<Rigidbody>().angularVelocity
			});
		}
	}

	[PunRPC]
	public void _NS(Vector3 _newPosition, Quaternion _newRotation, Vector3 _newVelocity, Vector3 _newAngularVelocity)
	{
		this.newPosition = _newPosition;
		this.newRotation = _newRotation;
		this.newVelocity = _newVelocity;
		this.newAngularVelocity = _newAngularVelocity;
		this.flagNewVelocities = true;
	}

	public override void TransportGUI(int numPlace)
	{
	}

	public GameObject platform;

	public GameObject[] turrelGun;

	public AudioSource turrelShoot;

	public AudioSource engineSound;

	public float damage = 20f;

	public float distance = 1000f;

	public float shootDeltaTime = 0.5f;

	private float lastShootTime;

	public GameObject bulletPrefab;

	public GameObject spinner1;

	public GameObject spinner2;

	public Vector3 centerOfMass;

	public float enginePower = 50f;

	public float rotSpeed = 1000f;

	public float pushPower = 5000f;

	private bool flagNewData;

	private Vector3 newPosition;

	private Quaternion newRotation;

	private Vector3 newVelocity;

	private Vector3 newAngularVelocity;

	private bool flagNewVelocities;
}
