using System;
using kube;
using UnityEngine;

public class SimpleCarScript : CarBase
{
	[ContextMenu("Editor Setup")]
	private new void EditorSetup()
	{
		this.GearRatio = new float[]
		{
			0.93f,
			1.13f,
			1.4f,
			1.8f,
			2.7f,
			4.3f
		};
	}

	public override void TransportInit()
	{
		base.gameObject.GetComponent<Rigidbody>().centerOfMass = this.centerOfMass;
		this.wheelsPhys = new WheelCollider[this.wheelsPhysGO.Length];
		this.wheelsRotateAngle = new float[this.wheelsPhysGO.Length];
		for (int i = 0; i < this.wheelsPhys.Length; i++)
		{
			this.wheelsPhys[i] = this.wheelsPhysGO[i].GetComponent<WheelCollider>();
		}
		this.wheelsPhysNullPos = new Vector3[this.wheelsPhysGO.Length];
		for (int j = 0; j < this.wheelsPhysNullPos.Length; j++)
		{
			this.wheelsPhysNullPos[j] = this.wheelsPhys[j].transform.localPosition;
		}
		if (this.ruleGO)
		{
			this.ruleGOInitRotate = this.ruleGO.transform.localRotation.eulerAngles;
		}
	}

	private void DriveCar()
	{
		int num = 0;
		float axis = UnityEngine.Input.GetAxis("Horizontal");
		if (axis < -0.2f)
		{
			num = 2;
		}
		else if (axis > 0.2f)
		{
			num = 1;
		}
		base.ShiftGears();
		for (int i = 0; i < this.wheelsPhys.Length; i++)
		{
			this.wheelsPhys[i].brakeTorque = 0f;
		}
		float axis2 = UnityEngine.Input.GetAxis("Vertical");
		if (Mathf.Abs(axis2) > 0.1f)
		{
			for (int j = 0; j < this.wheelsPhys.Length; j++)
			{
				if (this.wheelsPhys[j].motorTorque != 0f && Mathf.Sign(axis2) != Mathf.Sign(this.wheelsPhys[j].motorTorque))
				{
					this.wheelsPhys[j].brakeTorque = this.wheelBrakeTorque[j];
					this.wheelsPhys[j].motorTorque = 0f;
				}
				else if (this.isWheelDriven[j])
				{
					this.wheelsPhys[j].motorTorque = Mathf.Sign(axis2) * this.motorTorque / this.GearRatio[this.CurrentGear];
					this.wheelsPhys[j].brakeTorque = 0f;
				}
			}
		}
		else
		{
			for (int k = 0; k < this.wheelsPhys.Length; k++)
			{
				this.wheelsPhys[k].motorTorque = 0f;
				this.wheelsPhys[k].brakeTorque = this.frictionTorque;
			}
		}
		if (UnityEngine.Input.GetKey(KeyCode.D) || num == 1)
		{
			this.ruleRotate += Time.fixedDeltaTime * this.ruleMinIncrement;
			this.ruleRotate = Mathf.Min(this.ruleRotate, 1f);
		}
		else if (UnityEngine.Input.GetKey(KeyCode.A) || num == 2)
		{
			this.ruleRotate -= Time.fixedDeltaTime * this.ruleMinIncrement;
			this.ruleRotate = Mathf.Max(this.ruleRotate, -1f);
		}
		else if (Mathf.Abs(this.ruleRotate) > 0.05f)
		{
			this.ruleRotate -= Mathf.Sign(this.ruleRotate) * Time.fixedDeltaTime * this.ruleMinIncrement;
		}
		else
		{
			this.ruleRotate = 0f;
		}
		for (int l = 0; l < this.wheelsPhys.Length; l++)
		{
			this.wheelsPhys[l].steerAngle = this.wheelMaxRotateAngle[l] * this.ruleRotate;
		}
		Kube.IS.ps.rotationX = Mathf.Clamp(Kube.IS.ps.rotationX, -60f, 60f);
	}

	private void DriveCar1(int numDriver)
	{
		Kube.IS.ps.rotationX = Mathf.Clamp(Kube.IS.ps.rotationX, -60f, 60f);
		Transform parent = this.driverCameraTransform[numDriver].parent;
		float y = parent.rotation.eulerAngles.y + UnityEngine.Input.GetAxis("Mouse X") * 4f;
		float num = parent.rotation.eulerAngles.z - UnityEngine.Input.GetAxis("Mouse Y") * 4f;
		if (num > 180f)
		{
			num -= 360f;
		}
		num = Mathf.Max(-40f, num);
		num = Mathf.Min(12f, num);
		parent.rotation = Quaternion.Euler(0f, y, num);
	}

	public override void TransportDrive(int numDriver)
	{
		if (numDriver == 0)
		{
			this.DriveCar();
		}
		else
		{
			this.DriveCar1(numDriver);
		}
	}

	private void Update()
	{
		for (int i = 0; i < this.wheelsPhys.Length; i++)
		{
			this.wheelsRotateAngle[i] += this.wheelsPhys[i].rpm * Time.deltaTime * 6f;
			while (this.wheelsRotateAngle[i] > 180f)
			{
				this.wheelsRotateAngle[i] -= 360f;
			}
			while (this.wheelsRotateAngle[i] < -180f)
			{
				this.wheelsRotateAngle[i] += 360f;
			}
			this.wheelsModels[i].transform.localPosition = new Vector3(this.wheelsModels[i].transform.localPosition.x, this.wheelsModels[i].transform.localPosition.y + (this.wheelsPhys[i].transform.localPosition.y - this.wheelsPhysNullPos[i].y), this.wheelsModels[i].transform.localPosition.z);
			this.wheelsModels[i].transform.localRotation = Quaternion.Euler(0f, this.wheelsPhys[i].steerAngle, this.wheelsRotateAngle[i]);
		}
	}

	public override void TransportUpdate(int numPlace)
	{
		if (this.driversId[0] == 0)
		{
			for (int i = 0; i < this.wheelsPhys.Length; i++)
			{
				this.wheelsPhys[i].brakeTorque = this.wheelBrakeTorque[i];
			}
		}
		if ((this.driversId[0] != 0 || !base.photonView.isMine) && numPlace != 0)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.newPosition, Time.deltaTime * (float)PhotonNetwork.sendRateOnSerialize);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.newRotation, Time.deltaTime * (float)PhotonNetwork.sendRateOnSerialize);
			for (int j = 0; j < this.wheelsPhys.Length; j++)
			{
				this.wheelsPhys[j].motorTorque = Mathf.Lerp(this.wheelsPhys[j].motorTorque, this.newMotorTorque, Time.deltaTime * (float)PhotonNetwork.sendRateOnSerialize);
				this.wheelsPhys[j].brakeTorque = Mathf.Lerp(this.wheelsPhys[j].brakeTorque, this.newBrakeTorque, Time.deltaTime * (float)PhotonNetwork.sendRateOnSerialize);
			}
			this.ruleRotate = Mathf.Lerp(this.ruleRotate, this.newRuleRotate, Time.deltaTime * (float)PhotonNetwork.sendRateOnSerialize);
			if (this.flagNewVelocities)
			{
				this.rigidbody.velocity = this.newVelocity;
				this.rigidbody.angularVelocity = this.newAngularVelocity;
				this.flagNewVelocities = false;
			}
		}
		else
		{
			this.newPosition = base.transform.position;
			this.newRotation = base.transform.rotation;
		}
		this.meanRPM = 0f;
		this.wRpm = 0f;
		int num = 0;
		for (int k = 0; k < this.wheelsPhys.Length; k++)
		{
			if (this.isWheelDriven[k])
			{
				this.meanRPM += this.wheelsPhys[k].rpm;
				num++;
			}
			this.wRpm += this.wheelsPhys[k].rpm;
		}
		this.wRpm /= (float)this.wheelsPhys.Length;
		this.wdRpm = this.meanRPM / (float)num;
		this.meanRPM /= (float)num * this.GearRatio[this.CurrentGear];
		if (this.ruleGO)
		{
			this.ruleGO.transform.localRotation = Quaternion.Euler(this.ruleGOInitRotate + Vector3.up * this.ruleMaxAngle * this.ruleRotate);
		}
		this.audioSource.pitch = Mathf.Clamp(Mathf.Abs(this.meanRPM / this.maxRPM) + 1f, 0f, 2f);
	}

	public override void AnimateDriver(int numDriver, PlayerScript driver)
	{
		driver.GetComponent<Animation>().CrossFade(driver.animQuadroSit);
	}

	public override void LateAnimateDriver(int numDriver, PlayerScript driver)
	{
		Vector3 axis = driver.transform.TransformDirection(Vector3.right);
		driver.newRotationY = Mathf.Lerp(driver.newRotationY, driver.rotationY, Time.deltaTime * 5f);
		driver.headTransform.RotateAround(axis, Mathf.Min(Mathf.Max(-driver.newRotationY * 0.0174532924f - 0.3f, -1.5f), 1.5f));
		driver.rightHandTransform.RotateAround(axis, -driver.newRotationY * 0.0174532924f);
	}

	public override void SerializeWrite(PhotonStream stream)
	{
		stream.SendNext(base.transform.position);
		stream.SendNext(base.transform.rotation);
		stream.SendNext(this.rigidbody.velocity);
		stream.SendNext(this.rigidbody.angularVelocity);
		stream.SendNext(this.wheelsPhys[0].motorTorque);
		stream.SendNext(this.wheelsPhys[0].brakeTorque);
		stream.SendNext(this.ruleRotate);
	}

	public override void SerializeRead(PhotonStream stream)
	{
		this.newPosition = (Vector3)stream.ReceiveNext();
		this.newRotation = (Quaternion)stream.ReceiveNext();
		this.newVelocity = (Vector3)stream.ReceiveNext();
		this.newAngularVelocity = (Vector3)stream.ReceiveNext();
		this.newMotorTorque = (float)stream.ReceiveNext();
		this.newBrakeTorque = (float)stream.ReceiveNext();
		this.newRuleRotate = (float)stream.ReceiveNext();
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
				this.rigidbody.velocity,
				this.rigidbody.angularVelocity,
				this.wheelsPhys[0].motorTorque,
				this.wheelsPhys[0].brakeTorque,
				this.ruleRotate
			});
		}
	}

	[PunRPC]
	public void _NS(Vector3 _newPosition, Quaternion _newRotation, Vector3 _newVelocity, Vector3 _newAngularVelocity, float _newMotorTorque, float _newBrakeTorque, float _newRuleRotate)
	{
		this.newPosition = _newPosition;
		this.newRotation = _newRotation;
		this.newVelocity = _newVelocity;
		this.newAngularVelocity = _newAngularVelocity;
		this.newMotorTorque = _newMotorTorque;
		this.newBrakeTorque = _newBrakeTorque;
		this.newRuleRotate = _newRuleRotate;
		this.flagNewVelocities = true;
	}

	public GameObject[] wheelsPhysGO;

	public GameObject[] wheelsModels;

	public float[] wheelMaxRotateAngle;

	public float[] wheelBrakeTorque;

	public float motorTorque = 10f;

	public float frictionTorque = 10f;

	private float[] wheelsRotateAngle;

	private Vector3[] wheelsPhysNullPos;

	public Vector3 centerOfMass;

	private float ruleRotate;

	public GameObject ruleGO;

	public float ruleMaxAngle = 30f;

	private Vector3 ruleGOInitRotate;

	public float audioPitchKoeff = 1.5f;

	public float ruleMinIncrement = 10f;

	private Vector3 newPosition;

	private Quaternion newRotation;

	private Vector3 newVelocity;

	private Vector3 newAngularVelocity;

	private bool flagNewVelocities;

	private float newMotorTorque;

	private float newBrakeTorque;

	private float newRuleRotate;

	private float rotationY;

	private float rotationZ;

	public float wdRpm;

	public float wRpm;
}
