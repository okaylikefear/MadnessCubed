using System;
using kube;
using UnityEngine;

public class TitanScript : TransportScript
{
	public override void SetView(int numPlace, bool view3face)
	{
		if (this.driversGO[numPlace] != Kube.BCS.ps.gameObject)
		{
			view3face = true;
		}
		this._smr.enabled = view3face;
		if (!view3face)
		{
			this.canonGun.transform.parent = this.fpsWeaponTransform;
			this.canonGun.transform.localPosition = Vector3.zero;
			this.canonGun.transform.localRotation = Quaternion.identity;
		}
		else
		{
			this.canonGun.transform.parent = this._handGO;
			this.canonGun.transform.localPosition = Vector3.zero;
			this.canonGun.transform.localRotation = Quaternion.identity;
		}
	}

	public override bool HasFPS(int transportToDrivePlace)
	{
		return true;
	}

	public override void TransportInit()
	{
		this.correctPlayerPos = base.transform.localPosition;
		this.correctPlayerRot = base.transform.localRotation;
	}

	protected override void TransportAwake()
	{
		this._ch = base.GetComponent<CharacterController>();
		this._anim = base.GetComponent<Animator>();
		this._smr = base.GetComponentInChildren<SkinnedMeshRenderer>();
		this._handGO = this.canonGun.transform.parent;
	}

	public override void TransportLateUpdate(int numPlace)
	{
		base.TransportLateUpdate(numPlace);
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
		this._anim.SetBool("fire", true);
		int num = 38657;
		this.canonNextShootTime = Time.time + this.canonDeltaShootTime;
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
		this.canonGun.GetComponent<AudioSource>().Play();
	}

	protected override void TransportSlave()
	{
		Vector3 vector = this.correctPlayerPos - base.transform.position;
		Vector3 vector2 = vector;
		vector2.y = 0f;
		this._anim.SetBool("run", vector2.magnitude > 0.1f);
		CubePhys cubePhysType = Kube.WHS.GetCubePhysType(base.transform.position - Vector3.up * 0.5f);
		this.velocity = vector;
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.correctPlayerRot, Time.deltaTime * (float)PhotonNetwork.sendRateOnSerialize);
	}

	public override void TransportDrive(int numDriver)
	{
		if (Kube.IS.ps.paused)
		{
			return;
		}
		this.velocity.x = 0f;
		this.velocity.z = 0f;
		float axis = UnityEngine.Input.GetAxis("Horizontal");
		bool flag;
		bool flag2;
		if (axis < -0.2f)
		{
			flag = true;
			flag2 = false;
		}
		else if (axis > 0.2f)
		{
			flag = false;
			flag2 = true;
		}
		else
		{
			flag = false;
			flag2 = false;
		}
		float axis2 = UnityEngine.Input.GetAxis("Vertical");
		bool flag3;
		bool flag4;
		if (axis2 < -0.2f)
		{
			flag3 = true;
			flag4 = false;
		}
		else if (axis2 > 0.2f)
		{
			flag3 = false;
			flag4 = true;
		}
		else
		{
			flag3 = false;
			flag4 = false;
		}
		PlayerScript ps = Kube.BCS.ps;
		float y = base.transform.localEulerAngles.y + UnityEngine.Input.GetAxis("Mouse X") * ps.sensitivityX;
		this.rotationY += UnityEngine.Input.GetAxis("Mouse Y") * ps.sensitivityY;
		this.rotationY = Mathf.Clamp(this.rotationY, ps.minimumY, ps.maximumY);
		this.driverCameraTransform[numDriver].localEulerAngles = new Vector3(-this.rotationY, 0f, 0f);
		if (!Kube.IS.ps.view3face)
		{
			this.driverFPSCameraTransform[numDriver].localEulerAngles = new Vector3(-this.rotationY, 0f, 0f);
		}
		if (flag4)
		{
			this.forwardRun = Mathf.Lerp(this.forwardRun, this.runSpeed, Time.fixedDeltaTime * 20f);
		}
		else if (flag3)
		{
			this.forwardRun = Mathf.Lerp(this.forwardRun, -this.runSpeed, Time.fixedDeltaTime * 20f);
		}
		else
		{
			this.forwardRun = Mathf.Lerp(this.forwardRun, 0f, Time.fixedDeltaTime * 20f);
		}
		if (flag)
		{
			this.sideRun = Mathf.Lerp(this.sideRun, -this.runSpeed, Time.fixedDeltaTime * 20f);
		}
		else if (flag2)
		{
			this.sideRun = Mathf.Lerp(this.sideRun, this.runSpeed, Time.fixedDeltaTime * 20f);
		}
		else
		{
			this.sideRun = Mathf.Lerp(this.sideRun, 0f, Time.fixedDeltaTime * 20f);
		}
		base.transform.localEulerAngles = new Vector3(0f, y, 0f);
		this.velocity = base.transform.TransformDirection(new Vector3(this.sideRun, this.velocity.y, this.forwardRun));
		Vector3 vector = this.velocity;
		vector.y = 0f;
		this._anim.SetBool("run", vector.magnitude > 0.1f);
		bool flag5 = UnityEngine.Input.GetAxis("Fire1") != 0f;
		Ray camRay = Kube.BCS.ps.getCamRay();
		if (flag5)
		{
			RaycastHit raycastHit;
			if (!Physics.Raycast(camRay, out raycastHit, this.distance))
			{
				raycastHit.point = camRay.origin + camRay.direction * this.distance;
			}
			Vector3 position = this.canonShootPoint.transform.position;
			Ray ray = new Ray(position, raycastHit.point - position);
			if (this.canonNextShootTime < Time.time)
			{
				this.CreateCanonShot(ray.origin, ray.direction);
			}
		}
		bool button = Input.GetButton("Jump");
		if (this.grounded && this.nextJump < Time.time && button)
		{
			this.velocity.y = this.jumpSpeed * 1.2f;
			this.nextJump = Time.time + 0.5f;
			this._anim.SetTrigger("jump");
			this.NetJump();
		}
		this.correctPlayerPos = base.transform.position;
		this.correctPlayerRot = base.transform.rotation;
	}

	public override void TransportUpdate(int numPlace)
	{
		if (this.canonNextShootTime + this.canonDeltaShootTime < Time.time)
		{
			this._anim.SetBool("fire", false);
		}
		if (!this.grounded)
		{
			this.velocity.y = this.velocity.y + Kube.OH.gravity * Time.fixedDeltaTime;
		}
		CollisionFlags collisionFlags = this._ch.Move(this.velocity * Time.deltaTime * 10f);
		this.grounded = ((collisionFlags & CollisionFlags.Below) != CollisionFlags.None);
	}

	public override void AnimateDriver(int numDriver, PlayerScript driver)
	{
	}

	public override void LateAnimateDriver(int numDriver, PlayerScript driver)
	{
	}

	private void NetJump()
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_NS0", PhotonTargets.Others, new object[]
			{
				base.transform.position,
				base.transform.rotation,
				true
			});
		}
	}

	public override void NetSender(int numPlace)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_NS0", PhotonTargets.Others, new object[]
			{
				base.transform.position,
				base.transform.rotation,
				false
			});
		}
	}

	[PunRPC]
	public void _NS0(Vector3 _newPosition, Quaternion _newRotation, bool jump)
	{
		this.correctPlayerPos = _newPosition;
		this.correctPlayerRot = _newRotation;
		this.velocity = _newPosition - base.transform.position;
		if (jump)
		{
			this._anim.SetTrigger("jump");
		}
	}

	public Vector3 centerOfMass;

	public GameObject canonGun;

	public GameObject canonShootPoint;

	private float canonNextShootTime;

	public float canonDeltaShootTime = 1f;

	public float canonDamage = 300f;

	public GameObject canonShotPrefab;

	public float distance = 1000f;

	protected CharacterController _ch;

	protected Animator _anim;

	private float meanRPM;

	protected SkinnedMeshRenderer _smr;

	protected Transform _handGO;

	private float rotationY;

	private float forwardRun;

	private float sideRun;

	public float runSpeed = 5f;

	[HideInInspector]
	public Vector3 velocity;

	public float jumpSpeed = 5f;

	private bool grounded;

	private float nextJump;

	private Vector3 correctPlayerPos;

	private Quaternion correctPlayerRot;

	public Transform fpsWeaponTransform;
}
