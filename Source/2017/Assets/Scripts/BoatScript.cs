using System;
using kube;
using UnityEngine;

public class BoatScript : TransportScript
{
	public override void TransportInit()
	{
		base.gameObject.GetComponent<Rigidbody>().centerOfMass = this.centerOfMass;
	}

	private void DriveBoat()
	{
		if (Kube.IS.ps.paused)
		{
			return;
		}
		CubePhys cubePhysType = Kube.WHS.GetCubePhysType(base.transform.position);
		if (cubePhysType != CubePhys.water)
		{
			return;
		}
		float axis = UnityEngine.Input.GetAxis("Vertical");
		float axis2 = UnityEngine.Input.GetAxis("Horizontal");
		if (Mathf.Abs(axis) > 0.01f)
		{
			base.GetComponent<Rigidbody>().AddForce(Mathf.Sign(axis) * base.transform.forward * this.enginePower);
		}
		if (Mathf.Abs(axis2) > 0.01f)
		{
			base.GetComponent<Rigidbody>().AddTorque(Vector3.up * axis2 * this.rotSpeed * Time.deltaTime);
		}
	}

	public override void TransportDrive(int numDriver)
	{
		if (numDriver == 0)
		{
			this.DriveBoat();
		}
	}

	public override void TransportUpdate(int numPlace)
	{
		CubePhys cubePhysType = Kube.WHS.GetCubePhysType(base.transform.position + base.transform.up * this.offsetY);
		if (cubePhysType == CubePhys.water)
		{
			int num = Mathf.CeilToInt(base.transform.position.y);
			int i;
			for (i = num; i < Kube.WHS.sizeY; i++)
			{
				Vector3 position = base.transform.position;
				position.y = this.offsetY + (float)i;
				cubePhysType = Kube.WHS.GetCubePhysType(position);
				if (cubePhysType != CubePhys.water)
				{
					break;
				}
			}
			float num2 = (float)i - base.transform.position.y;
			if (num2 >= 0f)
			{
				Vector3 velocity = base.GetComponent<Rigidbody>().velocity;
				if (base.GetComponent<Rigidbody>().velocity.y < 0f)
				{
					velocity.y = 0f;
				}
				if (Vector3.Angle(base.transform.forward, base.GetComponent<Rigidbody>().velocity) > 5f)
				{
					velocity.x = Mathf.Lerp(velocity.x, 0f, Time.deltaTime);
					velocity.z = Mathf.Lerp(velocity.z, 0f, Time.deltaTime);
				}
				base.GetComponent<Rigidbody>().velocity = velocity;
				float num3 = Mathf.Max(1f, num2);
				if (num2 > 1f)
				{
					num3 *= 50f;
				}
				else if ((double)num2 > 0.5)
				{
					num3 *= 10f;
				}
				base.GetComponent<Rigidbody>().AddForce(Vector3.up * this.waterForce * num3 * num3 * Time.fixedDeltaTime, ForceMode.Impulse);
			}
		}
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f), Time.deltaTime * 16f);
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

	public AudioSource engineSound;

	public float damage = 20f;

	public float distance = 1000f;

	public float shootDeltaTime = 0.5f;

	private float lastShootTime;

	public GameObject bulletPrefab;

	public Vector3 centerOfMass;

	public float enginePower = 50f;

	public float rotSpeed = 1000f;

	public float pushPower = 5000f;

	public float waterForce = 300f;

	public float offsetY = -0.5f;

	private bool flagNewData;

	private Vector3 newPosition;

	private Quaternion newRotation;

	private Vector3 newVelocity;

	private Vector3 newAngularVelocity;

	private bool flagNewVelocities;
}
