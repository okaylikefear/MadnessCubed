using System;
using kube;
using UnityEngine;

public class TransportScript : SyncObjectScript
{
	public int health
	{
		get
		{
			this.Init();
			return -this._health + Kube.GPS.codeI;
		}
		set
		{
			this.Init();
			this._health = Kube.GPS.codeI - value;
		}
	}

	public int maxHealth
	{
		get
		{
			this.Init();
			return -this._maxHealth + Kube.GPS.codeI;
		}
		set
		{
			this.Init();
			this._maxHealth = Kube.GPS.codeI - value;
		}
	}

	public virtual void TransportDrive(int driverNum)
	{
	}

	public virtual void TransportInit()
	{
	}

	public virtual void TransportUpdate(int numPlace)
	{
	}

	public virtual void AnimateDriver(int numDriver, PlayerScript driver)
	{
	}

	public virtual void LateAnimateDriver(int numDriver, PlayerScript driver)
	{
	}

	public virtual void TransportGUI(int numPlace)
	{
	}

	private void Awake()
	{
		this.Init();
		this.audioSource = base.GetComponent<AudioSource>();
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.TransportAwake();
	}

	protected virtual void TransportAwake()
	{
	}

	private void OnDestroy()
	{
		for (int i = 0; i < this.maxDrivers; i++)
		{
			this.ExitDrive(this.driversId[i]);
		}
	}

	public void ExitDrive(int playerId)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ExitDrive", PhotonTargets.All, new object[]
			{
				playerId
			});
		}
		else
		{
			this._ExitDrive(playerId, null);
		}
	}

	public virtual void SetView(int numPlace, bool view3face)
	{
	}

	[PunRPC]
	public void _ExitDrive(int playerId, PhotonMessageInfo info)
	{
		for (int i = 0; i < this.maxDrivers; i++)
		{
			if (this.driversId[i] == playerId)
			{
				this.driversId[i] = 0;
				this.driversGO[i] = null;
				GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].GetComponent<PlayerScript>().onlineId == playerId)
					{
						array[j].SendMessage("ExitTransport", this.driverExitVector[i]);
						break;
					}
				}
				this.SetView(i, true);
				if (i == 0 && base.GetComponent<AudioSource>() != null)
				{
					base.GetComponent<AudioSource>().Stop();
				}
				break;
			}
		}
	}

	private void ExitAll()
	{
		for (int i = 0; i < this.maxDrivers; i++)
		{
			if (this.driversId[i] > 0)
			{
				this._ExitDrive(this.driversId[i], null);
			}
		}
	}

	public void TryToDrive(int playerId)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_TryToDrive", PhotonTargets.MasterClient, new object[]
			{
				playerId
			});
		}
		else
		{
			this._TryToDrive(playerId, null);
		}
	}

	[PunRPC]
	public void _TryToDrive(int playerId, PhotonMessageInfo info)
	{
		if (PhotonNetwork.isMasterClient)
		{
			bool flag = true;
			int placeToDrive = 0;
			for (int i = 0; i < this.maxDrivers; i++)
			{
				if (this.driversId[i] == 0)
				{
					flag = false;
					placeToDrive = i;
					break;
				}
			}
			if (flag)
			{
				this.SendNoPlaceToDrive(playerId);
			}
			else
			{
				this.GetInTransport(playerId, placeToDrive);
			}
		}
	}

	public void GetInTransport(int playerId, int placeToDrive)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_GetInTransport", PhotonTargets.All, new object[]
			{
				playerId,
				placeToDrive
			});
		}
		else
		{
			this._GetInTransport(playerId, placeToDrive, null);
		}
	}

	public void GetInTransport(PlayerScript ps, int placeToDrive)
	{
		if (ps.transportToDriveScript != null)
		{
			return;
		}
		this.driversGO[placeToDrive] = ps.gameObject;
		this.driversId[placeToDrive] = ps.onlineId;
		ps.DriveTransport(this, placeToDrive);
	}

	[PunRPC]
	public void _GetInTransport(int playerId, int placeToDrive, PhotonMessageInfo info)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].GetComponent<PlayerScript>().onlineId == playerId)
			{
				this.driversGO[placeToDrive] = array[i];
				this.driversId[placeToDrive] = playerId;
				array[i].GetComponent<PlayerScript>().DriveTransport(this.objectId, placeToDrive);
				break;
			}
		}
		if (base.GetComponent<AudioSource>() != null && this.driversId[0] != 0 && !base.GetComponent<AudioSource>().isPlaying)
		{
			base.GetComponent<AudioSource>().Play();
		}
	}

	public void TryChangePlace(int oldPlace, int newPlace)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_TryChangePlace", PhotonTargets.MasterClient, new object[]
			{
				oldPlace,
				newPlace
			});
		}
		else
		{
			this._TryChangePlace(oldPlace, newPlace, null);
		}
	}

	[PunRPC]
	public void _TryChangePlace(int oldPlace, int newPlace, PhotonMessageInfo info)
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (newPlace >= this.maxDrivers)
			{
				this.SendNoPlaceToDrive(this.driversId[oldPlace]);
			}
			else if (this.driversId[newPlace] != 0)
			{
				this.SendNoPlaceToDrive(this.driversId[oldPlace]);
			}
			else
			{
				this.ChangePlace(oldPlace, newPlace, this.driversId[oldPlace]);
			}
		}
	}

	public void ChangePlace(int oldPlace, int newPlace, int playerId)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ChangePlace", PhotonTargets.All, new object[]
			{
				oldPlace,
				newPlace,
				playerId
			});
		}
		else
		{
			this._ChangePlace(oldPlace, newPlace, playerId, null);
		}
	}

	[PunRPC]
	public void _ChangePlace(int oldPlace, int newPlace, int playerId, PhotonMessageInfo info)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].GetComponent<PlayerScript>().onlineId == playerId)
			{
				this.driversGO[newPlace] = array[i];
				this.driversId[newPlace] = playerId;
				this.driversGO[oldPlace] = null;
				this.driversId[oldPlace] = 0;
				array[i].GetComponent<PlayerScript>().DriveTransport(this.objectId, newPlace);
				break;
			}
		}
		if (base.GetComponent<AudioSource>() != null && this.driversId[0] != 0 && !base.GetComponent<AudioSource>().isPlaying)
		{
			base.GetComponent<AudioSource>().Play();
		}
	}

	public void SendNoPlaceToDrive(int playerId)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SendNoPlaceToDrive", PhotonTargets.MasterClient, new object[]
			{
				playerId
			});
		}
		else
		{
			this._SendNoPlaceToDrive(playerId, null);
		}
	}

	[PunRPC]
	public void _SendNoPlaceToDrive(int playerId, PhotonMessageInfo info)
	{
		if (Kube.BCS.onlineId == playerId)
		{
			Kube.GPS.printMessage(Localize.no_place_to_drive, Color.red);
		}
	}

	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.driversGO = new GameObject[this.maxDrivers];
		this.driversId = new int[this.maxDrivers];
		this.TransportInit();
		this.initialized = true;
	}

	private void ApplyFlash(Vector3 pos)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < this.maxDrivers; i++)
		{
			for (int j = 0; j < array.Length; j++)
			{
				PlayerScript component = array[j].GetComponent<PlayerScript>();
				if (component != null && this.driversId[i] == component.onlineId)
				{
					component.gameObject.SendMessage("ApplyFlash", pos, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	private void Start()
	{
		this.Init();
		this.maxHealth = this.initMaxHealth;
		this.health = this.maxHealth;
		if (!base.photonView.isMine)
		{
			this.SendMeParams();
		}
		base.InvokeRepeating("TransportNetSend", 0.5f, 0.25f);
		base.InvokeRepeating("ClearEmptyDrivers", 5f, 5f);
		this.TransportStart();
	}

	protected virtual void TransportStart()
	{
	}

	private void ClearEmptyDrivers()
	{
		for (int i = 0; i < this.maxDrivers; i++)
		{
			if (this.driversId[i] != 0 && this.driversGO[i] == null)
			{
				this.driversId[i] = 0;
			}
		}
	}

	public virtual void NetSender(int numPlace)
	{
	}

	public void TransportNetSend()
	{
		for (int i = 0; i < this.maxDrivers; i++)
		{
			if (Kube.BCS.onlineId == this.driversId[i] || (base.photonView.isMine && this.driversId[i] == 0))
			{
				this.NetSender(i);
			}
		}
	}

	public void SendMeParams()
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SendMeParams", PhotonTargets.All, new object[0]);
		}
		else
		{
			this._SendMeParams(null);
		}
	}

	[PunRPC]
	public void _SendMeParams(PhotonMessageInfo info)
	{
		if (base.photonView.isMine)
		{
			this.HereAreMyParams(this.objectId, this.transportOwner, this.transportHealth, this.driversId);
		}
	}

	public void HereAreMyParams(int _transportId, int _transportOwner, float _transportHealth, int[] _driversId)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_HereAreMyParams", PhotonTargets.All, new object[]
			{
				_transportId,
				_transportOwner,
				_transportHealth,
				_driversId
			});
		}
		else
		{
			this._HereAreMyParams(_transportId, _transportOwner, _transportHealth, _driversId, null);
		}
	}

	[PunRPC]
	public void _HereAreMyParams(int _transportId, int _transportOwner, float _transportHealth, int[] _driversId, PhotonMessageInfo info)
	{
		this.Init();
		if (!base.photonView.isMine)
		{
			this.objectId = _transportId;
			this.transportOwner = _transportOwner;
			this.transportHealth = _transportHealth;
			for (int i = 0; i < this.maxDrivers; i++)
			{
				if (this.driversId[i] != _driversId[i])
				{
					if (this.driversId[i] != 0)
					{
					}
					this.GetInTransport(_driversId[i], i);
				}
			}
		}
	}

	public virtual void SerializeWrite(PhotonStream stream)
	{
	}

	public virtual void SerializeRead(PhotonStream stream)
	{
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.connected || stream.isWriting)
		{
		}
	}

	public new void SetHealthMultiplier(int value)
	{
	}

	public new void SetDamageMultiplier(int value)
	{
	}

	private void FixedUpdate()
	{
		bool flag = false;
		int num = -1;
		for (int i = 0; i < this.maxDrivers; i++)
		{
			if (this.driversId[i] == Kube.BCS.onlineId)
			{
				flag = true;
				num = i;
				break;
			}
		}
		if (flag)
		{
			this.TransportDrive(num);
		}
		else
		{
			this.TransportSlave();
		}
		this.TransportUpdate(num);
		if (num >= 0)
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
			{
				this.TryChangePlace(num, 0);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
			{
				this.TryChangePlace(num, 1);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
			{
				this.TryChangePlace(num, 2);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
			{
				this.TryChangePlace(num, 3);
			}
		}
	}

	protected virtual void TransportSlave()
	{
	}

	private void LateUpdate()
	{
		int numPlace = -1;
		for (int i = 0; i < this.maxDrivers; i++)
		{
			if (this.driversId[i] == Kube.BCS.onlineId)
			{
				numPlace = i;
				break;
			}
		}
		this.TransportLateUpdate(numPlace);
	}

	public virtual void TransportLateUpdate(int numPlace)
	{
		if (numPlace == -1)
		{
			return;
		}
		bool flag = !this.driverIsHidden[numPlace] || (this.driverFPSCameraTransform.Length > numPlace && this.driverFPSCameraTransform[numPlace] != null);
		bool flag2 = Kube.IS.ps.view3face;
		if (!flag && !flag2)
		{
			flag2 = true;
		}
		if (numPlace > 0)
		{
			flag2 = true;
		}
		if (flag2 && this.driverCameraTransform[numPlace] != null)
		{
			Kube.IS.ps.cameraComp.transform.position = this.driverCameraTransform[numPlace].position;
			Kube.IS.ps.cameraComp.transform.rotation = this.driverCameraTransform[numPlace].rotation;
		}
		else if (this.driverFPSCameraTransform != null && this.driverFPSCameraTransform.Length > 0 && this.driverFPSCameraTransform[numPlace] != null)
		{
			Kube.IS.ps.cameraComp.transform.position = this.driverFPSCameraTransform[numPlace].position;
			Kube.IS.ps.cameraComp.transform.rotation = this.driverFPSCameraTransform[numPlace].rotation;
		}
	}

	public Transform GetDriveTransform(int driverNum)
	{
		return this.driverTransform[driverNum].transform;
	}

	public new void SetRespawnNum(int _id)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Transport");
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].GetComponent<SyncObjectScript>().objectId == _id)
			{
				PhotonNetwork.Destroy(base.gameObject);
				return;
			}
		}
		this.objectId = _id;
	}

	private static void CopyTransformsRecurse(Transform src, Transform dst)
	{
		dst.position = src.position;
		dst.rotation = src.rotation;
		if (dst.gameObject.GetComponent<Rigidbody>() != null)
		{
			dst.gameObject.GetComponent<Rigidbody>().Sleep();
		}
		foreach (object obj in dst)
		{
			Transform transform = (Transform)obj;
			Transform transform2 = src.Find(transform.name);
			if (transform2)
			{
				TransportScript.CopyTransformsRecurse(transform2, transform);
			}
		}
	}

	public void ApplyDamage(DamageMessage dm)
	{
		if (this.isDead)
		{
			return;
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ApplyDamage", PhotonTargets.All, new object[]
			{
				dm.damage,
				dm.id_killer,
				dm.team,
				dm.weaponType
			});
		}
		else
		{
			this._ApplyDamage(dm.damage, dm.id_killer, dm.team, dm.weaponType, null);
		}
		if (!this._damageDrivers)
		{
			return;
		}
		dm.damage = (short)((float)dm.damage / this.defenceRate);
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < this.maxDrivers; i++)
		{
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j].GetComponent<PlayerScript>().onlineId == this.driversId[i])
				{
					array[j].SendMessage("ApplyDamage", dm);
					break;
				}
			}
		}
	}

	[PunRPC]
	public void _ApplyDamage(short _damage, int _id_killer, int _team, short _weaponType, PhotonMessageInfo info)
	{
		if (this.isDead)
		{
			return;
		}
		if (base.photonView.isMine)
		{
			this.health -= (int)_damage;
			if (this.health <= 0)
			{
				this.Die(_id_killer, this.pointsForKillMe);
			}
		}
	}

	private void Die(int id_killer, int myPoints)
	{
		if (this.isDead)
		{
			return;
		}
		if (this.objectId >= 0)
		{
			Kube.BCS.NO.TransportDead(this.objectId);
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_Die", PhotonTargets.All, new object[]
			{
				id_killer,
				myPoints
			});
		}
		else
		{
			this._Die(id_killer, myPoints, null);
		}
	}

	[PunRPC]
	public void _Die(int id_killer, int myPoints, PhotonMessageInfo info)
	{
		this.Init();
		if (this.isDead)
		{
			return;
		}
		this.isDead = true;
		if (this.ragDoll != null)
		{
			this._ragDoll = (UnityEngine.Object.Instantiate(this.ragDoll, base.transform.position, base.transform.rotation) as GameObject);
			TransportScript.CopyTransformsRecurse(base.transform, this._ragDoll.transform);
		}
		if (Kube.BCS.onlineId == id_killer)
		{
			(UnityEngine.Object.Instantiate(Kube.OH.pointsText, base.transform.position + Vector3.up * 2f, Quaternion.identity) as GameObject).SendMessage("SetText", "+" + myPoints);
			BattleControllerScript bcs = Kube.BCS;
			bcs.bonusCounters.transportKilled = bcs.bonusCounters.transportKilled + 1;
		}
		this.ExitAll();
		if (base.photonView.isMine)
		{
			base.Invoke("DestroyPhotonView", 2f);
		}
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(false);
		}
	}

	private void DestroyPhotonView()
	{
		PhotonNetwork.Destroy(base.gameObject);
	}

	public new void SaveCodeVars()
	{
		this.codeVarsRandom = UnityEngine.Random.Range(10, 1000);
		this._health2 = this.health + this.codeVarsRandom;
		this._maxHealth2 = this.maxHealth + this.codeVarsRandom;
	}

	public new void LoadCodeVars()
	{
		this.health = this._health2 - this.codeVarsRandom;
		this.maxHealth = this._maxHealth2 - this.codeVarsRandom;
	}

	private void OnGUI()
	{
		bool flag = false;
		int numPlace = -1;
		for (int i = 0; i < this.maxDrivers; i++)
		{
			if (this.driversId[i] == Kube.BCS.onlineId)
			{
				flag = true;
				numPlace = i;
				break;
			}
		}
		if (flag)
		{
			this.TransportGUI(numPlace);
		}
	}

	public virtual bool HasFPS(int transportToDrivePlace)
	{
		return !this.driverIsHidden[transportToDrivePlace];
	}

	public virtual void AppplyPosition(Transform transform, int transportToDrivePlace)
	{
		transform.position = this.GetDriveTransform(transportToDrivePlace).position;
		transform.rotation = this.GetDriveTransform(transportToDrivePlace).rotation;
	}

	public virtual bool isDisableController(int transportToDrivePlace)
	{
		return true;
	}

	public virtual bool isHideWeapon(int transportToDrivePlace)
	{
		return !this.driverCanUseOwnWeapon[transportToDrivePlace];
	}

	public int transportType;

	public int transportOwner;

	public float transportHealth;

	public int maxDrivers;

	protected GameObject[] driversGO;

	[HideInInspector]
	public int[] driversId;

	public GameObject[] driverTransform;

	public Vector3[] driverExitVector;

	public bool[] driverCanUseOwnWeapon;

	public bool[] driverIsHidden;

	public Transform[] driverCameraTransform;

	public Transform[] driverFPSCameraTransform;

	public float defenceRate = 3f;

	private int _health;

	public int initMaxHealth;

	private int _maxHealth;

	public GameObject ragDoll;

	public int pointsForKillMe = 30;

	protected AudioSource audioSource;

	protected new Rigidbody rigidbody;

	private bool initialized;

	protected bool _damageDrivers = true;

	protected bool isDead;

	private GameObject _ragDoll;

	private int codeVarsRandom;

	private int _health2;

	private int _maxHealth2;
}
