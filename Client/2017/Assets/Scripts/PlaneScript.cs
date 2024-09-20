using System;
using kube;
using UnityEngine;

public class PlaneScript : TransportScript
{
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
		if (this.ruleGO != null)
		{
			this.ruleGOInitRotate = this.ruleGO.transform.localRotation.eulerAngles;
		}
	}

	private bool IsWheelOnGround()
	{
		bool flag = false;
		for (int i = 0; i < this.wheelsPhys.Length; i++)
		{
			WheelHit wheelHit;
			flag = (flag || this.wheelsPhys[i].GetGroundHit(out wheelHit));
		}
		return flag;
	}

	private void DriveCar0()
	{
		if (Kube.IS.ps.paused)
		{
			return;
		}
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
		for (int i = 0; i < this.wheelsPhys.Length; i++)
		{
			this.wheelsPhys[i].brakeTorque = 0f;
		}
		if (Input.GetButton("Jump"))
		{
			for (int j = 0; j < this.wheelsPhys.Length; j++)
			{
				this.wheelsPhys[j].motorTorque = 1f;
				this.wheelsPhys[j].brakeTorque = 0f;
			}
			this.meanRPM += 5f;
			this.meanRPM = Mathf.Min(this.meanRPM, this.maxRPM);
		}
		else
		{
			for (int k = 0; k < this.wheelsPhys.Length; k++)
			{
				this.wheelsPhys[k].motorTorque = 0f;
				this.wheelsPhys[k].brakeTorque = this.wheelBrakeTorque[k];
			}
			this.meanRPM -= 1f;
			this.meanRPM = Mathf.Max(20f, this.meanRPM);
		}
		if (UnityEngine.Input.GetAxis("Vertical") > 0.1f)
		{
			this.ruleRotateX = 1f;
		}
		else if (UnityEngine.Input.GetAxis("Vertical") < -0.1f)
		{
			this.ruleRotateX = -1f;
		}
		else
		{
			this.ruleRotateX = 0f;
		}
		if (UnityEngine.Input.GetKey(KeyCode.A) || num == 2)
		{
			this.ruleRotateY += Time.fixedDeltaTime * 15f;
			this.ruleRotateY = Mathf.Min(this.ruleRotateY, 1f);
		}
		else if (UnityEngine.Input.GetKey(KeyCode.D) || num == 1)
		{
			this.ruleRotateY -= Time.fixedDeltaTime * 15f;
			this.ruleRotateY = Mathf.Max(this.ruleRotateY, -1f);
		}
		else if (Mathf.Abs(this.ruleRotateY) > 0.05f)
		{
			this.ruleRotateY -= Mathf.Sign(this.ruleRotateY) * Time.fixedDeltaTime * 15f;
		}
		else
		{
			this.ruleRotateY = 0f;
		}
		for (int l = 0; l < this.wheelsPhys.Length; l++)
		{
			this.wheelsPhys[l].steerAngle = this.wheelMaxRotateAngle[l] * this.ruleRotateY;
		}
		if (UnityEngine.Input.GetAxis("Fire1") > 0f && !Kube.IS.ps.paused && Time.time - this.canonLastShootTime > this.canonShootDeltaTime)
		{
			this.canonLastShootTime = Time.time;
			Ray ray = new Ray(this.canonShootPoint.transform.position, this.canonShootPoint.transform.TransformDirection(Vector3.forward) * 1000f);
			this.CreateCanonShot(ray.origin, ray.direction);
		}
		float y = this.driverCameraToMove[0].transform.rotation.eulerAngles.y + UnityEngine.Input.GetAxis("Mouse X") * 4f;
		float num2 = this.driverCameraToMove[0].transform.rotation.eulerAngles.z - UnityEngine.Input.GetAxis("Mouse Y") * 4f;
		if (num2 > 180f)
		{
			num2 -= 360f;
		}
		num2 = Mathf.Max(-15f, num2);
		num2 = Mathf.Min(4.5f, num2);
		this.driverCameraToMove[0].transform.rotation = Quaternion.Euler(0f, y, num2);
	}

	public override void TransportLateUpdate(int numPlace)
	{
		bool flag = this.IsWheelOnGround();
		Vector3 velocity = base.GetComponent<Rigidbody>().velocity;
		velocity.y = 0f;
		float num = Vector3.Angle(base.transform.forward, velocity);
		base.GetComponent<Rigidbody>().AddForce(base.transform.forward * this.motorTorque * (this.meanRPM / this.maxRPM));
		if (num < 30f && velocity.magnitude > 5f)
		{
			base.GetComponent<Rigidbody>().AddForce(Vector3.up * this.FlyForce * velocity.magnitude * Time.deltaTime);
		}
		if (!flag)
		{
			base.GetComponent<Rigidbody>().AddForce(num * this.FlyForceStop * -base.GetComponent<Rigidbody>().velocity.normalized * Time.deltaTime);
		}
		if (!flag && base.GetComponent<Rigidbody>().velocity.magnitude > 5f)
		{
			Quaternion quaternion = base.GetComponent<Rigidbody>().rotation * Quaternion.Euler(Vector3.up * -this.ruleRotateY * this.ruleRotateTorque * Time.deltaTime);
			quaternion *= Quaternion.Euler(Vector3.right * this.ruleRotateX * this.ruleRotateTorque * Time.deltaTime);
			base.GetComponent<Rigidbody>().MoveRotation(quaternion);
			base.GetComponent<Rigidbody>().velocity = Vector3.Lerp(base.GetComponent<Rigidbody>().velocity, base.transform.forward * base.GetComponent<Rigidbody>().velocity.magnitude, 5f * Time.deltaTime);
		}
		if (numPlace >= 0)
		{
			Kube.IS.ps.cameraComp.transform.position = this.driverCameraTransform[numPlace].position;
			Kube.IS.ps.cameraComp.transform.rotation = this.driverCameraTransform[numPlace].rotation;
		}
		if (numPlace == 0)
		{
			Ray ray = new Ray(Kube.IS.ps.cameraComp.transform.position, Kube.IS.ps.cameraComp.transform.TransformDirection(Vector3.forward));
			int num2 = 38657;
			if (PhotonNetwork.offlineMode || base.photonView.isMine)
			{
				num2 -= 512;
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, this.distance, num2))
			{
				Vector3 vector = raycastHit.point;
			}
			else
			{
				Vector3 vector = ray.origin + ray.direction * this.distance;
			}
		}
		else if (numPlace == 1)
		{
		}
	}

	private void CreateCanonShot(Vector3 rayOrigin, Vector3 rayDirection)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_CreateCanonShot", PhotonTargets.All, new object[]
			{
				rayOrigin,
				rayDirection
			});
		}
		else
		{
			this._CreateCanonShot(rayOrigin, rayDirection, null);
		}
	}

	[PunRPC]
	private void _CreateCanonShot(Vector3 rayOrigin, Vector3 rayDirection, PhotonMessageInfo info)
	{
		int num = 38657;
		this.canonLastShootTime = Time.time;
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
			damageMessage.damage = (short)this.canonDamage;
		}
		else
		{
			damageMessage.damage = 0;
		}
		damageMessage.id_killer = Kube.BCS.onlineId;
		damageMessage.team = Kube.IS.ps.team;
		damageMessage.weaponType = 6;
		GameObject gameObject = UnityEngine.Object.Instantiate(this.canonShotPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.position = this.canonShootPoint.transform.position;
		gameObject.transform.LookAt(worldPosition);
		gameObject.SendMessage("SetDamageParam", damageMessage);
	}

	public override void TransportDrive(int numDriver)
	{
		if (numDriver == 0)
		{
			this.DriveCar0();
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
			this.ruleRotateY = Mathf.Lerp(this.ruleRotateY, this.newRuleRotate, Time.deltaTime * (float)PhotonNetwork.sendRateOnSerialize);
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
		for (int k = 0; k < this.wheelsPhys.Length; k++)
		{
			this.wheelsRotateAngle[k] += this.wheelsPhys[k].rpm * Time.deltaTime * 6f;
			while (this.wheelsRotateAngle[k] > 180f)
			{
				this.wheelsRotateAngle[k] -= 360f;
			}
			while (this.wheelsRotateAngle[k] < -180f)
			{
				this.wheelsRotateAngle[k] += 360f;
			}
		}
		if (this.ruleGO != null)
		{
			this.ruleGO.transform.localRotation = Quaternion.Euler(this.ruleGOInitRotate + Vector3.up * this.ruleMaxAngle * this.ruleRotateY);
		}
		float num = this.meanRPM;
		Vector3 vector = base.transform.InverseTransformDirection(this.rigidbody.velocity);
		num = Mathf.Max(this.meanRPM, 150f * vector.z);
		num = Mathf.Min(num, this.maxRPM);
		base.GetComponent<AudioSource>().pitch = 1f + this.audioPitchKoeff * Mathf.Abs(num) / this.maxRPM;
		this.rotor.transform.Rotate(Vector3.forward * Time.deltaTime * 1000f * Mathf.Abs(num) / this.maxRPM);
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

	public override void NetSender(int numPlace)
	{
		if (numPlace == 0)
		{
			if (PhotonNetwork.room != null)
			{
				base.photonView.RPC("_NS0", PhotonTargets.Others, new object[]
				{
					base.transform.position,
					base.transform.rotation,
					base.GetComponent<Rigidbody>().velocity,
					base.GetComponent<Rigidbody>().angularVelocity,
					this.wheelsPhys[0].motorTorque,
					this.wheelsPhys[0].brakeTorque,
					this.ruleRotateY
				});
			}
		}
		else if (numPlace == 1 && PhotonNetwork.room != null)
		{
			base.photonView.RPC("_NS1", PhotonTargets.Others, new object[0]);
		}
	}

	[PunRPC]
	public void _NS0(Vector3 _newPosition, Quaternion _newRotation, Vector3 _newVelocity, Vector3 _newAngularVelocity, float _newMotorTorque, float _newBrakeTorque, float _newRuleRotate)
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

	[PunRPC]
	public void _NS1(float _gunGunRotationX, float _gunGunRotationY)
	{
	}

	public GameObject[] wheelsPhysGO;

	private WheelCollider[] wheelsPhys;

	public float[] wheelMaxRotateAngle;

	public float[] wheelBrakeTorque;

	public float motorTorque = 10f;

	public float maxRPM = 1000f;

	public float frictionTorque = 10f;

	private float[] wheelsRotateAngle;

	private Vector3[] wheelsPhysNullPos;

	public Vector3 centerOfMass;

	private float ruleRotateY;

	private float ruleRotateX;

	public GameObject ruleGO;

	public float ruleMaxAngle = 30f;

	private Vector3 ruleGOInitRotate;

	public float audioPitchKoeff = 1.5f;

	public GameObject[] driverCameraToMove;

	public float canonShootDeltaTime = 2f;

	public GameObject canonShootPoint;

	private float canonLastShootTime;

	public float canonDamage = 300f;

	public GameObject canonShotPrefab;

	public float distance = 1000f;

	public float canonRotationSpeed = 1f;

	private float meanRPM;

	public GameObject rotor;

	public float ruleRotateTorque = 100f;

	public float FlyForce = 100f;

	public float FlyForceStop = 100f;

	private Vector3 newPosition;

	private Quaternion newRotation;

	private Vector3 newVelocity;

	private Vector3 newAngularVelocity;

	private bool flagNewVelocities;

	private float newMotorTorque;

	private float newBrakeTorque;

	private float newRuleRotate;
}
