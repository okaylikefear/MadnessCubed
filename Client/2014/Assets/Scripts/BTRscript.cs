using System;
using kube;
using UnityEngine;

public class BTRscript : TransportScript
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
		this.canonGunRotation = new Vector2(-90f, 0f);
		this.gunGunRotation = new Vector2(-90f, 0f);
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
		if (UnityEngine.Input.GetAxis("Vertical") > 0.1f)
		{
			if (this.meanRPM < -10f)
			{
				for (int j = 0; j < this.wheelsPhys.Length; j++)
				{
					this.wheelsPhys[j].motorTorque = 0f;
					this.wheelsPhys[j].brakeTorque = this.wheelBrakeTorque[j];
				}
			}
			else
			{
				for (int k = 0; k < this.wheelsPhys.Length; k++)
				{
					if (this.isWheelDriven[k])
					{
						this.wheelsPhys[k].motorTorque = this.motorTorque * Mathf.Max(0f, 1f - Mathf.Abs(this.meanRPM) / this.maxRPM);
						this.wheelsPhys[k].brakeTorque = 0f;
					}
				}
			}
		}
		else if (UnityEngine.Input.GetAxis("Vertical") < -0.1f)
		{
			if (this.meanRPM > 10f)
			{
				for (int l = 0; l < this.wheelsPhys.Length; l++)
				{
					this.wheelsPhys[l].motorTorque = 0f;
					this.wheelsPhys[l].brakeTorque = this.wheelBrakeTorque[l];
				}
			}
			else
			{
				for (int m = 0; m < this.wheelsPhys.Length; m++)
				{
					if (this.isWheelDriven[m])
					{
						this.wheelsPhys[m].motorTorque = -this.motorTorque * Mathf.Max(0f, 1f - Mathf.Abs(this.meanRPM) / this.maxRPM);
						this.wheelsPhys[m].brakeTorque = 0f;
					}
				}
			}
		}
		else
		{
			for (int n = 0; n < this.wheelsPhys.Length; n++)
			{
				this.wheelsPhys[n].motorTorque = 0f;
				this.wheelsPhys[n].brakeTorque = this.frictionTorque;
			}
		}
		if (UnityEngine.Input.GetKey(KeyCode.D) || num == 1)
		{
			this.ruleRotate += Time.fixedDeltaTime * 15f;
			this.ruleRotate = Mathf.Min(this.ruleRotate, 1f);
		}
		else if (UnityEngine.Input.GetKey(KeyCode.A) || num == 2)
		{
			this.ruleRotate -= Time.fixedDeltaTime * 15f;
			this.ruleRotate = Mathf.Max(this.ruleRotate, -1f);
		}
		else if (Mathf.Abs(this.ruleRotate) > 0.05f)
		{
			this.ruleRotate -= Mathf.Sign(this.ruleRotate) * Time.fixedDeltaTime * 15f;
		}
		else
		{
			this.ruleRotate = 0f;
		}
		for (int num2 = 0; num2 < this.wheelsPhys.Length; num2++)
		{
			this.wheelsPhys[num2].transform.localRotation = Quaternion.Euler(0f, this.wheelMaxRotateAngle[num2] * this.ruleRotate, 0f);
		}
		if (UnityEngine.Input.GetAxis("Fire1") > 0f && !Kube.IS.ps.paused && Time.time - this.canonLastShootTime > this.canonShootDeltaTime)
		{
			this.canonLastShootTime = Time.time;
			Ray ray = new Ray(this.canonShootPoint.transform.position, this.canonShootPoint.transform.TransformDirection(Vector3.forward) * 1000f);
			this.CreateCanonShot(ray.origin, ray.direction);
		}
		float y = this.driverCameraToMove[0].transform.rotation.eulerAngles.y + UnityEngine.Input.GetAxis("Mouse X") * 4f;
		float num3 = this.driverCameraToMove[0].transform.rotation.eulerAngles.z - UnityEngine.Input.GetAxis("Mouse Y") * 4f;
		if (num3 > 180f)
		{
			num3 -= 360f;
		}
		num3 = Mathf.Max(-15f, num3);
		num3 = Mathf.Min(4.5f, num3);
		this.driverCameraToMove[0].transform.rotation = Quaternion.Euler(0f, y, num3);
	}

	private void DriveCar1()
	{
		if (Kube.IS.ps.paused)
		{
			return;
		}
		if (UnityEngine.Input.GetAxis("Fire1") > 0f && !Kube.IS.ps.paused && Time.time - this.gunLastShootTime > this.gunShootDeltaTime)
		{
			this.gunLastShootTime = Time.time;
			Ray ray = new Ray(this.gunShootPoint.transform.position, this.gunShootPoint.transform.TransformDirection(Vector3.forward) * 1000f);
			this.CreateGunShot(ray.origin, ray.direction);
		}
		float y = this.driverCameraToMove[1].transform.rotation.eulerAngles.y + UnityEngine.Input.GetAxis("Mouse X") * 4f;
		float num = this.driverCameraToMove[1].transform.rotation.eulerAngles.z - UnityEngine.Input.GetAxis("Mouse Y") * 4f;
		if (num > 180f)
		{
			num -= 360f;
		}
		num = Mathf.Max(-40f, num);
		num = Mathf.Min(12f, num);
		this.driverCameraToMove[1].transform.rotation = Quaternion.Euler(0f, y, num);
	}

	public override void TransportLateUpdate(int numPlace)
	{
		if (numPlace >= 0)
		{
			Kube.IS.ps.cameraComp.transform.position = this.driverCameraTransform[numPlace].position;
			Kube.IS.ps.cameraComp.transform.rotation = this.driverCameraTransform[numPlace].rotation;
		}
		if (numPlace == 0)
		{
			Ray ray = new Ray(Kube.IS.ps.cameraComp.transform.position, Kube.IS.ps.cameraComp.transform.TransformDirection(Vector3.forward));
			int num = 38657;
			if (PhotonNetwork.offlineMode || base.photonView.isMine)
			{
				num -= 512;
			}
			RaycastHit raycastHit;
			Vector3 a;
			if (Physics.Raycast(ray, out raycastHit, this.distance, num))
			{
				a = raycastHit.point;
			}
			else
			{
				a = ray.origin + ray.direction * this.distance;
			}
			Vector3 vector = base.transform.InverseTransformDirection(a - this.canonTower.transform.position);
			this.canonGunRotation.x = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
			this.canonGunRotation.y = Mathf.Atan2(vector.y, Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z)) * 57.29578f;
		}
		else if (numPlace == 1)
		{
			Ray ray2 = new Ray(Kube.IS.ps.cameraComp.transform.position, Kube.IS.ps.cameraComp.transform.TransformDirection(Vector3.forward));
			int num2 = 38657;
			if (PhotonNetwork.offlineMode || base.photonView.isMine)
			{
				num2 -= 512;
			}
			RaycastHit raycastHit2;
			Vector3 a2;
			if (Physics.Raycast(ray2, out raycastHit2, this.distance, num2))
			{
				a2 = raycastHit2.point;
			}
			else
			{
				a2 = ray2.origin + ray2.direction * this.distance;
			}
			Vector3 vector2 = base.transform.InverseTransformDirection(a2 - this.gunTower.transform.position);
			this.gunGunRotation.x = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
			this.gunGunRotation.y = Mathf.Atan2(vector2.y, Mathf.Sqrt(vector2.x * vector2.x + vector2.z * vector2.z)) * 57.29578f;
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

	[RPC]
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
		if (Kube.GPS.playerId == this.driversId[0])
		{
			damageMessage.damage = (short)this.canonDamage;
		}
		else
		{
			damageMessage.damage = 0;
		}
		damageMessage.id_killer = Kube.GPS.playerId;
		damageMessage.team = Kube.IS.ps.team;
		damageMessage.weaponType = 6;
		GameObject gameObject = UnityEngine.Object.Instantiate(this.canonShotPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.position = this.canonShootPoint.transform.position;
		gameObject.transform.LookAt(worldPosition);
		gameObject.SendMessage("SetDamageParam", damageMessage);
		this.canonGun.audio.Play();
	}

	private void CreateGunShot(Vector3 rayOrigin, Vector3 rayDirection)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_CreateGunShot", PhotonTargets.All, new object[]
			{
				rayOrigin,
				rayDirection
			});
		}
		else
		{
			this._CreateGunShot(rayOrigin, rayDirection, null);
		}
	}

	[RPC]
	private void _CreateGunShot(Vector3 rayOrigin, Vector3 rayDirection, PhotonMessageInfo info)
	{
		int num = 38657;
		this.gunLastShootTime = Time.time;
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
		if (Kube.GPS.playerId == this.driversId[1])
		{
			damageMessage.damage = (short)this.gunDamage;
		}
		else
		{
			damageMessage.damage = 0;
		}
		damageMessage.id_killer = Kube.GPS.playerId;
		damageMessage.team = Kube.IS.ps.team;
		damageMessage.weaponType = 12;
		GameObject gameObject = UnityEngine.Object.Instantiate(this.gunShotPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.position = this.gunShootPoint.transform.position;
		gameObject.transform.LookAt(worldPosition);
		gameObject.SendMessage("SetDamageParam", damageMessage);
		this.gunGun.audio.Play();
	}

	public override void TransportDrive(int numDriver)
	{
		if (numDriver == 0)
		{
			this.DriveCar0();
		}
		else if (numDriver == 1)
		{
			this.DriveCar1();
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
				base.rigidbody.velocity = this.newVelocity;
				base.rigidbody.angularVelocity = this.newAngularVelocity;
				this.flagNewVelocities = false;
			}
		}
		else
		{
			this.newPosition = base.transform.position;
			this.newRotation = base.transform.rotation;
		}
		this.meanRPM = 0f;
		for (int k = 0; k < this.wheelsPhys.Length; k++)
		{
			this.meanRPM += this.wheelsPhys[k].rpm / (float)this.wheelsPhys.Length;
		}
		for (int l = 0; l < this.wheelsPhys.Length; l++)
		{
			this.wheelsRotateAngle[l] += this.wheelsPhys[l].rpm * Time.deltaTime * 6f;
			while (this.wheelsRotateAngle[l] > 180f)
			{
				this.wheelsRotateAngle[l] -= 360f;
			}
			while (this.wheelsRotateAngle[l] < -180f)
			{
				this.wheelsRotateAngle[l] += 360f;
			}
			this.wheelsModels[l].transform.localPosition = new Vector3(this.wheelsModels[l].transform.localPosition.x, this.wheelsModels[l].transform.localPosition.y + (this.wheelsPhys[l].transform.localPosition.y - this.wheelsPhysNullPos[l].y), this.wheelsModels[l].transform.localPosition.z);
			this.wheelsModels[l].transform.localRotation = Quaternion.Euler(0f, this.wheelsPhys[l].transform.localRotation.eulerAngles.y, this.wheelsRotateAngle[l]);
		}
		if (this.ruleGO != null)
		{
			this.ruleGO.transform.localRotation = Quaternion.Euler(this.ruleGOInitRotate + Vector3.up * this.ruleMaxAngle * this.ruleRotate);
		}
		base.audio.pitch = 1f + this.audioPitchKoeff * Mathf.Abs(this.meanRPM) / this.maxRPM;
		float b = this.canonGunRotation.x + 90f;
		this.canonTower.transform.localRotation = Quaternion.Euler(0f, Mathf.LerpAngle(this.canonTower.transform.localRotation.eulerAngles.y, b, Mathf.Sqrt(Time.deltaTime) * this.canonRotationSpeed), 0f);
		float y = this.canonGunRotation.y;
		this.canonGun.transform.localRotation = Quaternion.Euler(0f, Mathf.LerpAngle(this.canonGun.transform.localRotation.eulerAngles.y, y, Mathf.Sqrt(Time.deltaTime) * this.canonRotationSpeed), 0f);
		float x = this.gunGunRotation.x;
		this.gunTower.transform.localRotation = Quaternion.Euler(0f, Mathf.LerpAngle(this.gunTower.transform.localRotation.eulerAngles.y, x, Mathf.Sqrt(Time.deltaTime) * this.gunRotationSpeed), 0f);
		float y2 = this.gunGunRotation.y;
		this.gunGun.transform.localRotation = Quaternion.Euler(0f, Mathf.LerpAngle(this.gunGun.transform.localRotation.eulerAngles.y, y2, Mathf.Sqrt(Time.deltaTime) * this.gunRotationSpeed), 0f);
	}

	public override void AnimateDriver(int numDriver, PlayerScript driver)
	{
		driver.animation.CrossFade(driver.animQuadroSit);
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
					base.rigidbody.velocity,
					base.rigidbody.angularVelocity,
					this.wheelsPhys[0].motorTorque,
					this.wheelsPhys[0].brakeTorque,
					this.ruleRotate,
					this.canonGunRotation.x,
					this.canonGunRotation.y
				});
			}
		}
		else if (numPlace == 1 && PhotonNetwork.room != null)
		{
			base.photonView.RPC("_NS1", PhotonTargets.Others, new object[]
			{
				this.canonGunRotation.x,
				this.canonGunRotation.y
			});
		}
	}

	[RPC]
	public void _NS0(Vector3 _newPosition, Quaternion _newRotation, Vector3 _newVelocity, Vector3 _newAngularVelocity, float _newMotorTorque, float _newBrakeTorque, float _newRuleRotate, float _canonGunRotationX, float _canonGunRotationY)
	{
		this.newPosition = _newPosition;
		this.newRotation = _newRotation;
		this.newVelocity = _newVelocity;
		this.newAngularVelocity = _newAngularVelocity;
		this.newMotorTorque = _newMotorTorque;
		this.newBrakeTorque = _newBrakeTorque;
		this.newRuleRotate = _newRuleRotate;
		this.canonGunRotation.x = _canonGunRotationX;
		this.canonGunRotation.y = _canonGunRotationY;
		this.flagNewVelocities = true;
	}

	[RPC]
	public void _NS1(float _gunGunRotationX, float _gunGunRotationY)
	{
		this.gunGunRotation.x = _gunGunRotationX;
		this.gunGunRotation.y = _gunGunRotationY;
	}

	public GameObject[] wheelsPhysGO;

	private WheelCollider[] wheelsPhys;

	public GameObject[] wheelsModels;

	public bool[] isWheelDriven;

	public float[] wheelMaxRotateAngle;

	public float[] wheelBrakeTorque;

	public float motorTorque = 10f;

	public float maxRPM = 1000f;

	public float frictionTorque = 10f;

	private float[] wheelsRotateAngle;

	private Vector3[] wheelsPhysNullPos;

	public Vector3 centerOfMass;

	private float ruleRotate;

	public GameObject ruleGO;

	public float ruleMaxAngle = 30f;

	private Vector3 ruleGOInitRotate;

	public float audioPitchKoeff = 1.5f;

	private Vector2 canonGunRotation;

	private Vector2 gunGunRotation;

	public GameObject[] driverCameraToMove;

	public float canonShootDeltaTime = 2f;

	public GameObject canonTower;

	public GameObject canonGun;

	public GameObject canonShootPoint;

	private float canonLastShootTime;

	public float canonDamage = 300f;

	public GameObject canonShotPrefab;

	public float distance = 1000f;

	public float canonRotationSpeed = 1f;

	public float gunShootDeltaTime = 2f;

	public GameObject gunTower;

	public GameObject gunGun;

	public GameObject gunShootPoint;

	private float gunLastShootTime;

	public float gunDamage = 300f;

	public GameObject gunShotPrefab;

	public float gunRotationSpeed = 3f;

	private float meanRPM;

	private Vector3 newPosition;

	private Quaternion newRotation;

	private Vector3 newVelocity;

	private Vector3 newAngularVelocity;

	private bool flagNewVelocities;

	private float newMotorTorque;

	private float newBrakeTorque;

	private float newRuleRotate;
}
