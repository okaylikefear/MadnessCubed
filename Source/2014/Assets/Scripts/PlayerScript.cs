using System;
using System.Collections;
using System.Collections.Generic;
using kube;
using kube.ui;
using UnityEngine;

public class PlayerScript : Pawn
{
	public int availableCubes
	{
		get
		{
			this.Init();
			return -this._availableCubes + Kube.GPS.codeI;
		}
		set
		{
			this.Init();
			this._availableCubes = Kube.GPS.codeI - value;
		}
	}

	public int currentWeapon
	{
		get
		{
			this.Init();
			return -this._currentWeapon + Kube.GPS.codeI;
		}
		set
		{
			this.Init();
			this._currentWeapon = Kube.GPS.codeI - value;
		}
	}

	public int kills
	{
		get
		{
			this.Init();
			return -this._kills + Kube.GPS.codeI;
		}
		set
		{
			this.Init();
			this._kills = Kube.GPS.codeI - value;
		}
	}

	public int points
	{
		get
		{
			this.Init();
			return -this._points + Kube.GPS.codeI;
		}
		set
		{
			this.Init();
			this._points = Kube.GPS.codeI - value;
		}
	}

	public int playerSkin
	{
		get
		{
			this.Init();
			return -this._playerSkin + Kube.GPS.codeI;
		}
		set
		{
			this.Init();
			this._playerSkin = Kube.GPS.codeI - value;
		}
	}

	public int level
	{
		get
		{
			this.Init();
			return -this._level + Kube.GPS.codeI;
		}
		set
		{
			this.Init();
			this._level = Kube.GPS.codeI - value;
		}
	}

	public int health
	{
		get
		{
			this.Init();
			return -this._health3 + Kube.GPS.codeI;
		}
		set
		{
			this.Init();
			this._health3 = Kube.GPS.codeI - value;
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

	public int armor
	{
		get
		{
			this.Init();
			return -this._armor + Kube.GPS.codeI;
		}
		set
		{
			this.Init();
			this._armor = Kube.GPS.codeI - value;
		}
	}

	private int GetConstantsCash()
	{
		return Mathf.RoundToInt(this.runSpeed) + Mathf.RoundToInt(this.jumpSpeed) + Mathf.RoundToInt(this.runSpeedBonus) + Mathf.RoundToInt(this.jumpSpeedBonus) + Mathf.RoundToInt(this.reduceDamage * 100f) + Mathf.RoundToInt(this.reduceDamageBonus * 100f) + Mathf.RoundToInt((float)this.maxHealth) + Mathf.RoundToInt((float)this.maxArmor);
	}

	private void RecountConstantsCash()
	{
		this.constantsCash = this.GetConstantsCash();
	}

	public int UseItemMagic(int itemNum)
	{
		float time = Time.time;
		if (this.inventarItems.ContainsKey(itemNum))
		{
			this._nextItemUse = this.inventarItems[itemNum].nextUse;
		}
		if (time < this._nextItemUse)
		{
			return 2;
		}
		this._nextItemUse = Time.time + 2f;
		if (itemNum == 104)
		{
			this._nextItemUse += 8f;
		}
		if (this.inventarItems.ContainsKey(itemNum))
		{
			if (this.inventarItems[itemNum].cnt <= 0)
			{
				return 2;
			}
			PlayerScript.InventarItems value = this.inventarItems[itemNum];
			value.cnt--;
			value.nextUse = this._nextItemUse;
			this.inventarItems[itemNum] = value;
		}
		return Kube.IS.UseItem(itemNum);
	}

	public int itemCnt(int itemNum, int itemNN)
	{
		int num = Kube.GPS.inventarItems[itemNum];
		if (this.inventarItems.ContainsKey(itemNum))
		{
			return Math.Min(num, this.inventarItems[itemNum].cnt);
		}
		return num;
	}

	public float nextItemUse(int itemNum)
	{
		if (this.inventarItems.ContainsKey(itemNum))
		{
			return this.inventarItems[itemNum].nextUse;
		}
		return this._nextItemUse;
	}

	public void Init()
	{
		if (this.initialized)
		{
			return;
		}
		if (Kube.BCS == null)
		{
			Kube.BCS = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControllerScript>();
		}
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
		this.monstersStartleDeltaTime = 3f;
		this.cameraComp = base.gameObject.GetComponentInChildren<Camera>();
		if (PhotonNetwork.offlineMode || base.photonView.isMine)
		{
			this.playerView = new GameObject("CamFPS")
			{
				transform = 
				{
					parent = base.transform.Find("CameraObj").transform,
					localPosition = Vector3.zero,
					localRotation = Quaternion.identity
				}
			}.AddComponent<Camera>();
			this.playerView.enabled = false;
		}
		this.initialized = true;
	}

	private void Start()
	{
		if (base.photonView.owner.name != string.Empty)
		{
			this.uid = base.photonView.owner.name;
		}
		this._neck = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Neck");
		this.headTransform = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Neck/Bip01 Head");
		this.rightHandTransform = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Neck/Bip01 R Clavicle");
		base.animation[this.animSwordAttack[0]].layer = 5;
		base.animation[this.animSwordAttack[1]].layer = 5;
		base.animation[this.animSwordAttack[2]].layer = 5;
		base.animation[this.animSwordAttack[0]].speed = 2.2f;
		base.animation[this.animSwordAttack[1]].speed = 2.2f;
		base.animation[this.animSwordAttack[2]].speed = 2.2f;
		base.animation[this.animIdleEmpty].speed = 0.5f;
		base.animation[this.animIdleSword].speed = 0.5f;
		base.animation[this.animIdleWeapon].speed = 0.5f;
		base.animation[this.animSwordAttack[0]].AddMixingTransform(this._neck);
		base.animation[this.animSwordAttack[1]].AddMixingTransform(this._neck);
		base.animation[this.animSwordAttack[2]].AddMixingTransform(this._neck);
		base.animation[this.animWeaponShoot].layer = 5;
		base.animation[this.animWeaponShoot].AddMixingTransform(this._neck);
		for (int i = 0; i < this.weaponAnim1face.Length; i++)
		{
			if (this.weaponAnim1face[i].Length != 0)
			{
				base.animation[this.weaponAnim1face[i]].AddMixingTransform(base.transform.Find("CameraObj"));
				base.animation[this.weaponAnim1face[i]].layer = 10;
				base.animation[this.weaponAnim1face[i]].speed = 1f;
			}
		}
		for (int j = 0; j < this.animDecor.Length; j++)
		{
			if (this.animDecor[j].Length != 0)
			{
				base.animation[this.animDecor[j]].layer = 20;
			}
		}
		this.canBuild = false;
		this.controller = base.GetComponent<CharacterController>();
		this.Init();
		int num = 0;
		this.points = num;
		this.kills = num;
		if (Kube.BCS.gameType != GameType.creating && Kube.BCS.gameType != GameType.test)
		{
			this.cameraComp.cullingMask -= 16384;
		}
		if (PhotonNetwork.offlineMode || base.photonView.isMine)
		{
			this.playerView.clearFlags = CameraClearFlags.Depth;
			this.playerView.depth = 2f;
			this.playerView.cullingMask = 1 << LayerMask.NameToLayer("FPSWeapon");
			this.playerView.enabled = true;
			this.cameraComp.depth = 1f;
			this.cameraComp.cullingMask &= ~(1 << LayerMask.NameToLayer("FPSWeapon"));
			this.cameraComp.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
			this.cameraComp.cullingMask &= ~(1 << LayerMask.NameToLayer("MenuRoom"));
		}
		for (int k = 0; k < 50; k++)
		{
			this.lastShotTimeNew[k] = 0f;
		}
		this.bullets = new PlayerScript.Bullets();
		this.clips = new PlayerScript.Clips();
		if (PhotonNetwork.offlineMode || base.photonView.isMine)
		{
			base.gameObject.layer = 9;
			this.type = 0;
			this.id = Kube.GPS.playerId;
			this.playerName = Kube.GPS.playerName;
			base.transform.Find("TextName").gameObject.SetActive(false);
			Kube.IS.ps = this;
			this.PlayerDressSkin();
			this.armor = this.maxArmor;
			this.level = Kube.GPS.playerLevel;
			if (Kube.ASS2)
			{
				this.rankPlane.renderer.material = Kube.ASS2.RankTex[Mathf.Min(this.level, Kube.ASS2.RankTex.Length - 1)];
			}
			if (Kube.BCS.gameType == GameType.creating)
			{
				if (Kube.BCS.ownerId == this.id)
				{
					this.canBuild = true;
				}
				if (Kube.BCS.creatorId == this.id)
				{
					this.canBuild = true;
				}
				if (PhotonNetwork.isMasterClient)
				{
					this.canBuild = true;
					Kube.BCS.creatorId = this.id;
				}
				if (!Kube.BCS.canChangeWorld)
				{
					this.canBuild = false;
				}
			}
			this.SetView(false);
			this.RecountBonuces();
			this.health = this.maxHealth;
			Kube.IS.ChoseFastInventar(0);
			this.ChatMessage(this.playerName + " " + Localize.player_joined);
			this.availableCubes = Kube.GPS.maxAvailableCubes;
			if (Kube.BCS.gameType == GameType.teams)
			{
				this.canBuildBlock = Kube.BCS.mapCanBreak;
			}
			Kube.IS.resetInventory();
			if (Kube.BCS.gameType == GameType.creating)
			{
				this._targetCube = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("TargetCube2"));
				this._targetCube.SetActive(false);
				this._targetPlane = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("TargetCube"));
				this._targetPlane.SetActive(false);
			}
			this.Spawn();
			Kube.BCS.hud.BeginGame();
		}
		else
		{
			this.cameraComp.gameObject.SetActive(false);
			base.gameObject.layer = 10;
			this.type = 1;
			this.id = (int)base.photonView.owner.customProperties["id"];
			this.canBuild = false;
			this.SetView(true);
			this.NO.SynhronizePlayers();
		}
	}

	public void DoUseMagic(int fastInvNum)
	{
		ItemPropsScript component = Kube.ASS3.gameItemsGO[fastInvNum].GetComponent<ItemPropsScript>();
		if (component.magic)
		{
			Ray ray = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			Vector3 shotPoint = this.calcShotPoint(ray.origin, ray.direction, 1000f);
			GameObject gameObject = Kube.ASS3.gameItemsGO[fastInvNum];
			MagicGrenade component2 = gameObject.GetComponent<MagicGrenade>();
			int num = this.UseItemMagic(fastInvNum);
			if (num != 1)
			{
				if (num == 0)
				{
					if (component2 != null)
					{
						component2.Use(this);
					}
					else
					{
						this.NO.CreateMagic(fastInvNum, base.transform.position + Vector3.up * 1.5f + base.transform.TransformDirection(Vector3.forward * 0.7f), shotPoint, this.id);
					}
				}
			}
		}
	}

	public bool HasWeaponPickup(int id)
	{
		return this._weaponPickup.ContainsKey(id);
	}

	private void Spawn()
	{
		this.inventarItems = new Dictionary<int, PlayerScript.InventarItems>();
		this._weaponPickup = new Dictionary<int, int>();
		this.inventarItems[98] = new PlayerScript.InventarItems
		{
			cnt = 2
		};
		this.inventarItems[99] = new PlayerScript.InventarItems
		{
			cnt = 2
		};
		this.inventarItems[104] = new PlayerScript.InventarItems
		{
			cnt = 4
		};
		this.inventarItems[106] = new PlayerScript.InventarItems
		{
			cnt = 2
		};
		if (this.type == 0)
		{
			Kube.BCS.bonusCounters.kills = 0;
			Kube.BCS.bonusCounters.headshots = 0;
			Kube.BCS.bonusCounters.explosions = 0;
			Kube.BCS.bonusCounters.nearFights = 0;
			Kube.BCS.bonusCounters.saves = 0;
			Kube.BCS.bonusCounters.selfKill = 0;
			Kube.BCS.bonusCounters.grenades = 0;
			Kube.BCS.bonusCounters.capturedTheFlag = 0;
			Kube.BCS.bonusCounters.cubesPlaced = 0;
			Kube.BCS.bonusCounters.demonKilled = 0;
			Kube.BCS.bonusCounters.firstPlace = 0;
			Kube.BCS.bonusCounters.mecanismPlaced = 0;
			Kube.BCS.bonusCounters.missionComplited = 0;
			Kube.BCS.bonusCounters.placedItem = 0;
			Kube.BCS.bonusCounters.secondPlace = 0;
			Kube.BCS.bonusCounters.thirdPlace = 0;
			Kube.BCS.bonusCounters.survivalWave = 0;
			Kube.BCS.bonusCounters.transportKilled = 0;
			Kube.BCS.bonusCounters.winnerTeam = 0;
			Kube.BCS.bonusCounters.zombieExplosion = 0;
			Kube.BCS.bonusCounters.zombieKill = 0;
		}
		if (this.type == 0)
		{
			for (int i = 0; i < Kube.IS.bulletParams.Length; i++)
			{
				this.bullets[i] = Kube.IS.bulletParams[i].initialAmount;
			}
			for (int j = 0; j < Kube.IS.weaponParams.Length; j++)
			{
				this.clips[j] = Kube.IS.weaponParams[j].clipSize[Kube.IS.weaponParams[j].currentClipSizeIndex];
			}
		}
	}

	public void SetTeam(int _team)
	{
		this.team = _team;
		if (this.team < 0 || this.team > 4)
		{
			return;
		}
		GameObject gameObject = base.transform.Find("TextName/TextName").gameObject;
		gameObject.renderer.material.SetColor("_Color", Kube.OH.teamColor[this.team]);
	}

	public void ShowMyTeam()
	{
		ArrayList arrayList = new ArrayList();
		arrayList.Add(Kube.OH.teamColor[this.team]);
		arrayList.Add(50);
		arrayList.Add(0.3f);
		arrayList.Add(0.5f);
		arrayList.Add(Localize.your_team_is + Localize.teamName[this.team]);
		(UnityEngine.Object.Instantiate(Kube.OH.pointsText, base.transform.position + Vector3.up * 2f, Quaternion.identity) as GameObject).SendMessage("SetText", arrayList);
	}

	private void RecountBonuces()
	{
		this.maxHealthBonus = 0;
		this.maxArmorBonus = 0;
		this.runSpeedBonus = 0f;
		this.jumpSpeedBonus = 0f;
		this.reduceDamageBonus = 0f;
		this.maxHealthBonus += (int)Kube.GPS.skinBonus[this.playerSkin, 0];
		this.maxArmorBonus += (int)Kube.GPS.skinBonus[this.playerSkin, 1];
		this.runSpeedBonus += Kube.GPS.skinBonus[this.playerSkin, 2];
		this.jumpSpeedBonus += Kube.GPS.skinBonus[this.playerSkin, 3];
		this.reduceDamageBonus += Kube.GPS.skinBonus[this.playerSkin, 4] * 0.01f;
		for (int i = 0; i < Kube.GPS.playerClothes.Length; i++)
		{
			if (Kube.GPS.playerClothes[i] >= 0)
			{
				this.maxHealthBonus += (int)Kube.GPS.clothesBonus[Kube.GPS.playerClothes[i], 0];
				this.maxArmorBonus += (int)Kube.GPS.clothesBonus[Kube.GPS.playerClothes[i], 1];
				this.runSpeedBonus += Kube.GPS.clothesBonus[Kube.GPS.playerClothes[i], 2];
				this.jumpSpeedBonus += Kube.GPS.clothesBonus[Kube.GPS.playerClothes[i], 3];
				this.reduceDamageBonus += Kube.GPS.clothesBonus[Kube.GPS.playerClothes[i], 4] * 0.01f;
			}
		}
		int num = 0;
		if (Kube.GPS.vipEnd - Time.time > 0f)
		{
			num += 2;
		}
		this.maxHealth = (int)Kube.GPS.charParamsPrice[0, Mathf.Min(Kube.GPS.playerHealth + num, 7), 4] + this.maxHealthBonus;
		this.maxArmor = (int)Kube.GPS.charParamsPrice[1, Mathf.Min(Kube.GPS.playerArmor + num, 7), 4] + this.maxArmorBonus;
		this.runSpeed = (float)((int)Kube.GPS.charParamsPrice[2, Mathf.Min(Kube.GPS.playerSpeed + num, 7), 4]) + this.runSpeedBonus;
		this.jumpSpeed = (float)((int)Kube.GPS.charParamsPrice[3, Mathf.Min(Kube.GPS.playerJump + num, 7), 4]) + this.jumpSpeedBonus;
		this.reduceDamage = Kube.GPS.charParamsPrice[4, Mathf.Min(Kube.GPS.playerDefend + num, 7), 4] * 0.01f + this.reduceDamageBonus;
		this.RecountConstantsCash();
	}

	private void SynhronizePlayer()
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SynhronizePlayer", PhotonTargets.All, new object[]
			{
				this.playerName,
				this.id,
				this.canBuild,
				this.playerSkin,
				this.playerClothes,
				this.kills,
				this.deadTimes,
				this.level,
				this.team
			});
		}
		else
		{
			this._SynhronizePlayer(this.playerName, this.id, this.canBuild, this.playerSkin, this.playerClothes, this.kills, this.deadTimes, this.level, this.team, null);
		}
	}

	[RPC]
	private void _SynhronizePlayer(string _playerName, int _id, bool _canBuild, int _playerSkin, string _playerClothes, int _kills, int _deadTimes, int _level, int _team, PhotonMessageInfo info)
	{
		if (!base.photonView.isMine)
		{
			this.playerName = _playerName;
		}
		if (this.type != 0)
		{
			GameObject gameObject = base.transform.Find("TextName").gameObject;
			gameObject.GetComponentInChildren<TextMesh>().text = AuxFunc.DecodeRussianName(this.playerName);
		}
		if (this.id == 0)
		{
		}
		this.id = _id;
		this.level = _level;
		this.rankPlane.renderer.material = Kube.ASS2.RankTex[Mathf.Min(this.level, Kube.ASS2.RankTex.Length - 1)];
		this.canBuild = _canBuild;
		this.kills = _kills;
		this.deadTimes = _deadTimes;
		this.SetTeam(_team);
		this.playerSkin = _playerSkin;
		this.playerClothes = _playerClothes;
		base.gameObject.SendMessage("DressSkin", string.Concat(new object[]
		{
			string.Empty,
			this.playerSkin,
			";",
			Kube.GPS.skinItems[this.playerSkin],
			";",
			this.playerClothes
		}));
	}

	private void DressJetPack(bool _jetPackOn)
	{
		this.jetPackGO.SetActive(_jetPackOn);
	}

	public bool rifleAim
	{
		get
		{
			return this._rifleAim;
		}
		set
		{
			if (this._rifleAim == value)
			{
				return;
			}
			if (this.currentWeapon != -1)
			{
				this.weaponGOScript.HideWeapon(value);
			}
			this._rifleAim = value;
		}
	}

	public void onAssetsLoaded(int id)
	{
		if (Kube.ASS2)
		{
			this.rankPlane.renderer.material = Kube.ASS2.RankTex[Mathf.Min(this.level, Kube.ASS2.RankTex.Length - 1)];
		}
	}

	private void DeadUpdate()
	{
		if (Time.time - this.deadTime < 2f)
		{
			if (Time.time - this.deadTime < 0.1f)
			{
				Time.timeScale = 1f;
			}
			else if (Time.time - this.deadTime < 0.3f)
			{
				Time.timeScale = 0.2f;
			}
			else
			{
				Time.timeScale = Mathf.Min(1f, Mathf.Lerp(Time.timeScale, 1f, (Time.time - this.deadTime - 0.3f) / 1.7f));
			}
		}
		else
		{
			Time.timeScale = 1f;
		}
		if (!this.paused)
		{
			float axis = UnityEngine.Input.GetAxis("Horizontal");
			if (axis < -0.2f)
			{
				this.controlLeft = true;
				this.controlRight = false;
			}
			else if (axis > 0.2f)
			{
				this.controlLeft = false;
				this.controlRight = true;
			}
			else
			{
				this.controlLeft = false;
				this.controlRight = false;
			}
			float axis2 = UnityEngine.Input.GetAxis("Vertical");
			if (axis2 < -0.2f)
			{
				this.controlBackward = true;
				this.controlForward = false;
			}
			else if (axis2 > 0.2f)
			{
				this.controlBackward = false;
				this.controlForward = true;
			}
			else
			{
				this.controlBackward = false;
				this.controlForward = false;
			}
		}
		this.rotationX = base.transform.localEulerAngles.y + UnityEngine.Input.GetAxis("Mouse X") * this.sensitivityX;
		this.rotationY += UnityEngine.Input.GetAxis("Mouse Y") * this.sensitivityY;
		this.newRotationY = (this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY));
		this.cameraComp.transform.parent.localEulerAngles = new Vector3(-this.rotationY, 0f, 0f);
		base.transform.localEulerAngles = new Vector3(0f, this.rotationX, 0f);
		this.moveDirection = Vector3.zero;
		if (this.controlForward)
		{
			this.moveDirection += this.cameraComp.transform.TransformDirection(Vector3.forward);
		}
		if (this.controlBackward)
		{
			this.moveDirection -= this.cameraComp.transform.TransformDirection(Vector3.forward);
		}
		if (this.controlLeft)
		{
			this.moveDirection -= this.cameraComp.transform.TransformDirection(Vector3.right);
		}
		if (this.controlRight)
		{
			this.moveDirection += this.cameraComp.transform.TransformDirection(Vector3.right);
		}
		CollisionFlags collisionFlags = this.controller.Move(this.moveDirection * Time.deltaTime * 10f);
		this.grounded = ((collisionFlags & CollisionFlags.Below) != CollisionFlags.None);
		if (UnityEngine.Input.GetAxis("Jump") > 0f && this.type == 0 && Kube.BCS.gameType != GameType.survival)
		{
			if (Kube.BCS.gameType == GameType.mission)
			{
				if (this._canRespawn)
				{
					this.Respawn();
				}
			}
			else
			{
				this.Respawn();
			}
		}
		if (Kube.BCS.gameType == GameType.survival && Time.time > this.survivalRespawnTime)
		{
			this.Respawn();
		}
		if (((Kube.BCS.gameType == GameType.survival && Time.time < this.survivalRespawnTime) || Kube.BCS.gameType == GameType.mission) && UnityEngine.Input.GetKeyDown(KeyCode.X) && Kube.GPS.inventarItems[109] > 0)
		{
			if (Kube.BCS.gameType == GameType.survival)
			{
				this.survivalRespawnTime = Time.time + 30f;
			}
			else
			{
				this._canRespawn = true;
				this.Respawn();
			}
			Kube.IS.UseItem(109);
		}
	}

	private void ReloadGun()
	{
		if (Kube.BCS.gameType == GameType.creating)
		{
			return;
		}
		this.rifleAim = false;
		if (this.currentWeapon != -1 && Kube.IS.weaponParams[this.currentWeapon].UsingBullets > 0 && this.clips[this.currentWeapon] < Kube.IS.weaponParams[this.currentWeapon].clipSize[Kube.IS.weaponParams[this.currentWeapon].currentClipSizeIndex] && this.bullets[Kube.IS.weaponParams[this.currentWeapon].BulletsType] > 0)
		{
			this.rechargingWeapon = true;
			this.rechargingWeaponStart = Time.time;
			this.rechargingWeaponType = this.currentWeapon;
			base.animation.CrossFade(this.weaponRechargeBeginAnim, 0.05f);
			this.CreateRechargeSound(this.currentWeapon);
			if (Kube.BCS.tutorialGO != null)
			{
				Kube.BCS.tutorialGO.SendMessage("ReloadedGun");
			}
		}
	}

	private int GeometryCode(int _geom, RaycastHit rch)
	{
		int num = 0;
		if ((double)Mathf.Round(rch.normal.z) == 1.0)
		{
			num = 0;
		}
		else if ((double)Mathf.Round(rch.normal.z) == -1.0)
		{
			num = 3;
		}
		else if ((double)Mathf.Round(rch.normal.x) == 1.0)
		{
			num = 1;
		}
		else if ((double)Mathf.Round(rch.normal.x) == -1.0)
		{
			num = 2;
		}
		else
		{
			Vector3 normalized = base.transform.forward.normalized;
			if (Mathf.Abs(normalized.x) > Mathf.Abs(normalized.z))
			{
				normalized.z = 0f;
			}
			else
			{
				normalized.x = 0f;
			}
			if (Mathf.Round(normalized.z) < 0f)
			{
				num = 0;
			}
			else if (Mathf.Round(normalized.z) > 0f)
			{
				num = 3;
			}
			else if (Mathf.Round(normalized.x) < 0f)
			{
				num = 1;
			}
			else if (Mathf.Round(normalized.x) > 0f)
			{
				num = 2;
			}
		}
		int num2 = PlayerScript.geometryIds[_geom];
		if (num2 > 3)
		{
			num2 += num;
		}
		return num2;
	}

	private void CreatingUpdate()
	{
		if (!Kube.GPS.isVIP)
		{
			return;
		}
		int num = this._geom;
		if (UnityEngine.Input.GetKeyDown(KeyCode.Z))
		{
			num--;
		}
		else if (UnityEngine.Input.GetKeyDown(KeyCode.X))
		{
			num++;
		}
		if (num < 0)
		{
			num = 3;
		}
		else if (num > 3)
		{
			num = 0;
		}
		this._geom = num;
		Kube.BCS.hud.modes.SetCube(this._geom);
	}

	private void PlaceNewCube(Vector3 newCubePlace, int fastInvNum, int geom)
	{
		this.NO.PlaceNewCube(newCubePlace, fastInvNum, geom);
		if (Kube.GPS.needTrainingBuild)
		{
			Kube.TS.SendMessage("PlacedCube");
		}
		if (Kube.BCS.gameType == GameType.teams)
		{
			this.availableCubes--;
		}
		BattleControllerScript bcs = Kube.BCS;
		bcs.bonusCounters.cubesPlaced = bcs.bonusCounters.cubesPlaced + 1;
	}

	private void LocalUpdate()
	{
		Kube.BCS.hud.isVisible = (!Kube.OH.emptyScreen && !Kube.OH.isMenu);
		Kube.BCS.hud.jetpack.gameObject.SetActive(this.jetPackOn);
		if (this.jetPackOn)
		{
			Kube.BCS.hud.jetpack.lable.text = (this.jetPackFuel * 100f).ToString("0") + "%";
		}
		if (this.hud.Count == 0 && UnityEngine.Input.GetKeyDown(KeyCode.Return))
		{
			this.chatMessage = string.Empty;
			this.hud.Add(new DrawCall(this.DrawChat));
			if (Kube.BCS.gameProcess == BattleControllerScript.GameProcess.game)
			{
				this.paused = true;
			}
		}
		if (this.dead)
		{
			this.DeadUpdate();
		}
		else
		{
			if (Kube.BCS.gameType == GameType.creating)
			{
				this.CreatingUpdate();
			}
			this.DrawAims();
			Time.timeScale = 1f;
			bool flag = true;
			if (this.isDriveTransport && !this.transportToDriveScript.driverCanUseOwnWeapon[this.transportToDrivePlace])
			{
				flag = false;
			}
			if (!this.paused && !this.isDriveTransport)
			{
				float axis = UnityEngine.Input.GetAxis("Horizontal");
				if (axis < -0.2f)
				{
					this.controlLeft = true;
					this.controlRight = false;
				}
				else if (axis > 0.2f)
				{
					this.controlLeft = false;
					this.controlRight = true;
				}
				else
				{
					this.controlLeft = false;
					this.controlRight = false;
				}
				float axis2 = UnityEngine.Input.GetAxis("Vertical");
				if (axis2 < -0.2f)
				{
					this.controlBackward = true;
					this.controlForward = false;
				}
				else if (axis2 > 0.2f)
				{
					this.controlBackward = false;
					this.controlForward = true;
				}
				else
				{
					this.controlBackward = false;
					this.controlForward = false;
				}
			}
			if (!this.paused && flag)
			{
				if (!this.isDriveTransport)
				{
					this.rotationX = base.transform.localEulerAngles.y + UnityEngine.Input.GetAxis("Mouse X") * this.sensitivityX;
				}
				else
				{
					this.rotationX += UnityEngine.Input.GetAxis("Mouse X") * this.sensitivityX;
				}
				this.rotationY += UnityEngine.Input.GetAxis("Mouse Y") * this.sensitivityY;
				this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
				if (!this.isDriveTransport)
				{
					this.cameraComp.transform.parent.localEulerAngles = new Vector3(-this.rotationY, 0f, 0f);
				}
				else
				{
					this.cameraComp.transform.parent.localEulerAngles = new Vector3(-this.rotationY, this.rotationX, 0f);
				}
				base.transform.localEulerAngles = new Vector3(0f, this.rotationX, 0f);
			}
			if (this.freezed)
			{
				return;
			}
			if (Kube.GPS.inventarSpecItems[0] >= 0 && Kube.BCS.gameType != GameType.captureTheFlag && !this.jetPackOn)
			{
				this.jetPackOn = true;
				this.DressJetPack(true);
			}
			if (this.controlForward)
			{
				this.forwardRun = Mathf.Lerp(this.forwardRun, this.runSpeed + this.runSpeedBonus, Time.time * 20f);
			}
			else if (this.controlBackward)
			{
				this.forwardRun = Mathf.Lerp(this.forwardRun, -(this.runSpeed + this.runSpeedBonus), Time.time * 20f);
			}
			else
			{
				this.forwardRun = Mathf.Lerp(this.forwardRun, 0f, Time.time * 20f);
			}
			if (this.controlLeft)
			{
				this.sideRun = Mathf.Lerp(this.sideRun, -(this.runSpeed + this.runSpeedBonus), Time.time * 20f);
			}
			else if (this.controlRight)
			{
				this.sideRun = Mathf.Lerp(this.sideRun, this.runSpeed + this.runSpeedBonus, Time.time * 20f);
			}
			else
			{
				this.sideRun = Mathf.Lerp(this.sideRun, 0f, Time.time * 20f);
			}
			this.typePhys = Kube.WHS.GetCubePhysType(base.transform.position + Vector3.up * 0.5f);
			if (this.typePhys == CubePhys.air || this.typePhys == CubePhys.solid)
			{
				if (!this.grounded && !this.jetPackWork)
				{
					this.moveDirection.y = this.moveDirection.y + Kube.OH.gravity * Time.deltaTime;
				}
				else if (this.grounded)
				{
					if (Mathf.Abs(this.moveDirection.y) > this.saveFallVelocity && Kube.BCS.gameType != GameType.creating)
					{
						this.ApplyDamage(new DamageMessage
						{
							damage = (short)(this.fallDamage * (Mathf.Abs(this.moveDirection.y) - this.saveFallVelocity)),
							id_killer = 0,
							weaponType = 0,
							team = 10
						});
					}
					this.moveDirection.y = 0f;
				}
				if (!this.isDriveTransport && this.jetPackOn && Input.GetButton("Jump") && !this.grounded && Kube.BCS.gameType != GameType.creating)
				{
					this.jetPackFuel = Mathf.Max(0f, this.jetPackFuel - Time.deltaTime * 0.8f);
				}
				this.jetPackWork = false;
				if (this.grounded && Input.GetButton("Jump") && !this.paused)
				{
					this.moveDirection.y = this.moveDirection.y + (this.jumpSpeed + this.jumpSpeedBonus);
				}
				else if (!this.isDriveTransport && this.jetPackOn && this.jetPackFuel >= 0.05f && Input.GetButton("Jump") && !this.paused)
				{
					this.jetPackGO.SendMessage("PlayStop", true, SendMessageOptions.DontRequireReceiver);
					this.jetPackWork = true;
					this.moveDirection.y = Mathf.Min(this.moveDirection.y + Time.deltaTime * 8f, 8f);
				}
				if (this.moveDirection.y > 0f)
				{
				}
				if (!Input.GetButton("Jump"))
				{
					this.jetPackGO.SendMessage("PlayStop", false, SendMessageOptions.DontRequireReceiver);
					this.jetPackWork = false;
				}
				this.moveDirection = new Vector3(this.sideRun, this.moveDirection.y, this.forwardRun);
			}
			else if (this.typePhys == CubePhys.water)
			{
				if (!this.grounded)
				{
					this.moveDirection.y = Kube.OH.gravity * Time.deltaTime * 20f;
				}
				else
				{
					this.moveDirection.y = 0f;
				}
				if (Input.GetButton("Jump") && !this.paused)
				{
					this.moveDirection.y = (this.jumpSpeed + this.jumpSpeedBonus) * 0.6f;
				}
				this.moveDirection = new Vector3(this.sideRun * 0.5f, this.moveDirection.y, this.forwardRun * 0.5f);
			}
			else if (this.typePhys == CubePhys.ledder)
			{
				if (Input.GetButton("Jump") && !this.paused)
				{
					this.moveDirection.y = (this.jumpSpeed + this.jumpSpeedBonus) * 1f;
				}
				else
				{
					this.moveDirection.y = 0f;
				}
				this.moveDirection = new Vector3(this.sideRun * 0.5f, this.moveDirection.y, this.forwardRun * 0.5f);
			}
			else if (this.typePhys == CubePhys.liftOn)
			{
				this.moveDirection.y = 5f;
				if (Input.GetButton("Jump") && !this.paused)
				{
					this.moveDirection.y = this.moveDirection.y + (this.jumpSpeed + this.jumpSpeedBonus) * 1f;
				}
				this.moveDirection = new Vector3(this.sideRun * 0.5f, this.moveDirection.y, this.forwardRun * 0.5f);
			}
			CubePhys cubePhysType = Kube.WHS.GetCubePhysType(base.transform.position - Vector3.up * 0.5f);
			if (!this.isDriveTransport && cubePhysType != CubePhys.air && (Mathf.Abs(this.forwardRun) > 0.1f || Mathf.Abs(this.sideRun) > 0.1f) && Time.time - this.lastStepTime > this.stepDeltaTime)
			{
				Kube.WHS.PlayCubeHit(base.transform.position - Vector3.up * 0.5f, SoundHitType.footSteps);
				this.lastStepTime = Time.time;
			}
			if (!this.isDriveTransport && cubePhysType == CubePhys.water && this.currentTypePhysFloor == CubePhys.air)
			{
				UnityEngine.Object.Instantiate(Kube.ASS4.soundWaterSplash, base.transform.position, Quaternion.identity);
			}
			this.currentTypePhysFloor = cubePhysType;
			if (this.paused)
			{
				this.moveDirection.x = (this.moveDirection.z = 0f);
			}
			if (!this.isDriveTransport)
			{
				CollisionFlags collisionFlags = this.controller.Move(base.transform.TransformDirection(this.moveDirection) * Time.deltaTime);
				this.grounded = ((collisionFlags & CollisionFlags.Below) != CollisionFlags.None);
			}
			else
			{
				base.transform.position = this.transportToDriveScript.GetDriveTransform(this.transportToDrivePlace).position;
				base.transform.rotation = this.transportToDriveScript.GetDriveTransform(this.transportToDrivePlace).rotation;
			}
			if (base.transform.position.y < 0f)
			{
				this.ApplyDamage(new DamageMessage
				{
					damage = 1000,
					id_killer = 0,
					weaponType = 0,
					team = 10
				});
			}
			if (Kube.BCS.gameType == GameType.creating)
			{
				Ray ray = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
				int layerMask = 256;
				int num = Kube.GPS.fastInventar[Kube.IS.chosenFastInventar].Type;
				if (num != 0)
				{
					layerMask = 8448;
				}
				RaycastHit raycastHit;
				if (!this.isCracking && Physics.Raycast(ray, out raycastHit, 10f, layerMask))
				{
					Vector3 position = new Vector3(Mathf.Round(raycastHit.point.x - raycastHit.normal.x * 0.001f), Mathf.Round(raycastHit.point.y - raycastHit.normal.y * 0.001f), Mathf.Round(raycastHit.point.z - raycastHit.normal.z * 0.001f));
					Vector3 a = new Vector3(Mathf.Round(raycastHit.point.x + raycastHit.normal.x / 2f), Mathf.Round(raycastHit.point.y + raycastHit.normal.y / 2f), Mathf.Round(raycastHit.point.z + raycastHit.normal.z / 2f));
					Vector3 position2 = a - raycastHit.normal * 0.49f;
					int cubeFill = (int)Kube.WHS.GetCubeFill((int)position.x, (int)position.y, (int)position.z);
					int cubeData = (int)Kube.WHS.GetCubeData((int)position.x, (int)position.y, (int)position.z);
					if (this._targetCursor)
					{
						this._targetCursor.SetActive(false);
					}
					if (cubeFill != 128 || cubeData == 0)
					{
						this._targetCursor = this._targetPlane;
						this._targetCursor.transform.position = position2;
						this._targetCursor.transform.rotation = Quaternion.FromToRotation(Vector3.back, raycastHit.normal);
					}
					else
					{
						this._targetCursor = this._targetCube;
						this._targetCursor.transform.position = position;
					}
					this._targetCursor.SetActive(true);
				}
				else if (this._targetCursor)
				{
					this._targetCursor.SetActive(false);
				}
			}
			else if (this._targetCursor != null)
			{
				this._targetCursor.SetActive(false);
				this._targetCursor = null;
			}
			this.guiItemText = string.Empty;
			if (!this.isDriveTransport && this.moveItem && (UnityEngine.Input.GetAxis("Fire1") != 0f || UnityEngine.Input.GetAxis("Fire2") != 0f))
			{
				Ray ray2 = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
				RaycastHit raycastHit2;
				if (Physics.Raycast(ray2, out raycastHit2, 10f, 8448))
				{
					ItemPropsScript component = this.gameObjectToMove.GetComponent<ItemPropsScript>();
					if (component.placeType == ItemPlaceType.onTheItem && raycastHit2.collider.gameObject.layer != 13)
					{
						Kube.GPS.printMessage(Localize.put_on_items, Color.white);
					}
					else if (component.placeType == ItemPlaceType.onTheItem && raycastHit2.collider.gameObject.layer == 13)
					{
						ItemPropsScript component2 = raycastHit2.collider.gameObject.GetComponent<ItemPropsScript>();
						this.gameObjectToMove.BroadcastMessage("MoveItem", component2.collider.transform.position);
						this.onlyMove = false;
						this.moveItem = false;
					}
					else
					{
						Vector3 vector = new Vector3(Mathf.Round(raycastHit2.point.x + raycastHit2.normal.x * 0.02f), Mathf.Round(raycastHit2.point.y + raycastHit2.normal.y * 0.02f), Mathf.Round(raycastHit2.point.z + raycastHit2.normal.z * 0.02f));
						ushort cubeFill2 = Kube.WHS.GetCubeFill((int)vector.x, (int)vector.y, (int)vector.z);
						if (cubeFill2 != 0 && cubeFill2 != 128)
						{
							Kube.GPS.printMessage(Localize.cube_occupied, Color.white);
							MonoBehaviour.print("Filled with: " + Kube.WHS.cubes[(int)vector.x, (int)vector.y, (int)vector.z].type);
						}
						else if (!component.magic)
						{
							this.gameObjectToMove.BroadcastMessage("MoveItem", vector);
							this.onlyMove = false;
							this.moveItem = false;
						}
					}
				}
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.H))
			{
				if (Kube.OH.hasMenu(new DrawCall(this.DrawActivitiesMenu)))
				{
					Kube.OH.closeMenu(new DrawCall(this.DrawActivitiesMenu));
				}
				else
				{
					Kube.OH.openMenu(new DrawCall(this.DrawActivitiesMenu), true, false);
				}
			}
			if (this.onlyMove || Kube.OH.isMenu)
			{
				return;
			}
			Ray ray3 = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			bool flag2 = true;
			if (Kube.BCS.gameType == GameType.teams && this.availableCubes <= 0)
			{
				flag2 = false;
			}
			int layerMask2 = 40960;
			if (Kube.BCS.gameType == GameType.creating)
			{
				layerMask2 = 57344;
			}
			RaycastHit raycastHit3;
			if (!this.isDriveTransport && Physics.Raycast(ray3, out raycastHit3, 10f, layerMask2))
			{
				if (raycastHit3.collider.gameObject.transform.root.gameObject.layer == 13)
				{
					ItemPropsScript component3 = raycastHit3.collider.gameObject.transform.root.gameObject.GetComponent<ItemPropsScript>();
					bool flag3 = raycastHit3.collider.gameObject.layer != 14 || Kube.BCS.gameType == GameType.creating;
					if (this.canBuild && Kube.BCS.canChangeWorld && component3.canTake && flag3 && Kube.BCS.gameType == GameType.creating)
					{
						if (this.guiItemText.Length != 0)
						{
							this.guiItemText += "\n";
						}
						this.guiItemText += Localize.to_delete_press;
						if (UnityEngine.Input.GetKeyDown(KeyCode.Delete))
						{
							this.gameObjectToDelete = raycastHit3.collider.gameObject;
							ActionAreaScript component4 = this.gameObjectToDelete.transform.root.gameObject.GetComponent<ActionAreaScript>();
							WireScript component5 = this.gameObjectToDelete.transform.root.gameObject.GetComponent<WireScript>();
							if (component4 != null || component5 != null)
							{
								this.gameObjectToDelete.transform.root.gameObject.SendMessage("DeleteItem");
							}
							else
							{
								for (int i = 0; i < Kube.WHS.gameItems.Count; i++)
								{
									if (Kube.WHS.gameItems[i].gameObject == this.gameObjectToDelete.transform.root.gameObject)
									{
										this.NO.RemoveGameItem(Kube.WHS.gameItems[i].id);
										break;
									}
								}
							}
							this.gameObjectToDelete = null;
							this.paused = false;
							Screen.lockCursor = true;
						}
					}
					if (this.canBuild && Kube.BCS.canChangeWorld && component3.canTake && flag3 && Kube.BCS.gameType == GameType.creating)
					{
						if (this.guiItemText.Length != 0)
						{
							this.guiItemText += "\n";
						}
						this.guiItemText += Localize.to_move_press;
						if (UnityEngine.Input.GetKeyDown(KeyCode.E))
						{
							this.onlyMove = true;
							this.moveItem = true;
							this.gameObjectToMove = raycastHit3.collider.gameObject.transform.root.gameObject;
						}
					}
					if (component3.canActivate)
					{
						if (this.guiItemText.Length != 0)
						{
							this.guiItemText += "\n";
						}
						this.guiItemText += Localize.to_activate_press;
						if (UnityEngine.Input.GetKeyDown(KeyCode.F))
						{
							component3.gameObject.BroadcastMessage("Activate", base.gameObject.GetComponent<PlayerScript>(), SendMessageOptions.RequireReceiver);
						}
					}
					if (this.canBuild && Kube.BCS.canChangeWorld && component3.canRotate && Kube.BCS.gameType == GameType.creating)
					{
						if (this.guiItemText.Length != 0)
						{
							this.guiItemText += "\n";
						}
						this.guiItemText += Localize.to_rotate_press;
						if (UnityEngine.Input.GetKeyDown(KeyCode.R))
						{
							for (int j = 0; j < Kube.WHS.gameItems.Count; j++)
							{
								if (Kube.WHS.gameItems[j].gameObject == raycastHit3.collider.gameObject.transform.root.gameObject)
								{
									this.NO.RotateGameItem(Kube.WHS.gameItems[j].id);
									break;
								}
							}
						}
					}
					if (this.canBuild && Kube.BCS.canChangeWorld && component3.canSetup && Kube.BCS.gameType == GameType.creating)
					{
						if (this.guiItemText.Length != 0)
						{
							this.guiItemText += "\n";
						}
						this.guiItemText += Localize.to_edit_press;
						if (UnityEngine.Input.GetKeyDown(KeyCode.T))
						{
							component3.gameObject.BroadcastMessage("SetupItem", base.gameObject.GetComponent<PlayerScript>(), SendMessageOptions.RequireReceiver);
						}
					}
				}
				else if (raycastHit3.collider.gameObject.transform.root.gameObject.layer == 15)
				{
					if (this.guiItemText.Length != 0)
					{
						this.guiItemText += "\n";
					}
					this.guiItemText += Localize.to_drive_press;
					if (UnityEngine.Input.GetKeyDown(KeyCode.E))
					{
						raycastHit3.collider.gameObject.transform.root.gameObject.SendMessage("TryToDrive", this.id, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			if (this.isDriveTransport && UnityEngine.Input.GetKeyDown(KeyCode.X))
			{
				this.transportToDriveScript.ExitDrive(this.id);
			}
			int num2;
			int num3;
			if (Kube.BCS.gameType != GameType.creating)
			{
				num2 = Kube.GPS.fastInventarWeapon[Kube.IS.chosenFastInventar].Type;
				num3 = Kube.GPS.fastInventarWeapon[Kube.IS.chosenFastInventar].Num;
			}
			else
			{
				num2 = Kube.GPS.fastInventar[Kube.IS.chosenFastInventar].Type;
				num3 = Kube.GPS.fastInventar[Kube.IS.chosenFastInventar].Num;
			}
			bool flag4 = false;
			if (Input.GetButtonDown("Fire2") && !this.paused)
			{
				if (num2 == 1)
				{
					ItemPropsScript component6 = Kube.ASS3.gameItemsGO[num3].GetComponent<ItemPropsScript>();
					if (component6.magic)
					{
						Ray ray4 = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
						RaycastHit raycastHit4;
						if (Physics.Raycast(ray4, out raycastHit4, 1000f, 64768))
						{
							this.NO.CreateMagic(num3, base.transform.position + Vector3.up * 1.5f + base.transform.TransformDirection(Vector3.forward * 0.7f), raycastHit4.point, this.id);
							if (Kube.IS.UseItem(num3) == 1)
							{
							}
							flag4 = true;
						}
					}
				}
				else if (num2 == 3)
				{
					if (Kube.BCS.gameType == GameType.creating)
					{
						this.DoUseMagic(num3);
						flag4 = true;
					}
				}
				else if (num2 == 4 && num3 == 0)
				{
					num2 = 0;
					num3 = 5;
					flag4 = false;
				}
			}
			if (this.jetPackOn)
			{
				this.jetPackFuel = Mathf.Min(1f, this.jetPackFuel + Time.deltaTime * 0.04f);
			}
			this.cameraComp.fieldOfView = 60f;
			this.playerView.fieldOfView = 60f;
			Vector3 localPosition = this.weaponObjCamera.transform.localPosition;
			localPosition.x = 0.361f;
			this.weaponObjCamera.transform.localPosition = localPosition;
			this.sensitivityX = (this.sensitivityY = 4f);
			if (UnityEngine.Input.GetAxis("Fire2") > 0f && !this.paused)
			{
				if (this.currentWeapon != -1 && !this.rechargingWeapon)
				{
					if (this.currentWeapon == 11 || this.currentWeapon == 23 || this.currentWeapon == 31)
					{
						this.rifleAim = true;
						this.cameraComp.fieldOfView = 15f;
						this.sensitivityX = (this.sensitivityY = 1.5f);
					}
					else if (Kube.IS.weaponParams[this.currentWeapon].UsingBullets != 0)
					{
						this.playerView.fieldOfView = 30f;
						this.cameraComp.fieldOfView = 30f;
						this.sensitivityX = (this.sensitivityY = 2f);
					}
				}
			}
			else
			{
				this.rifleAim = false;
			}
			if (!this.isDriveTransport && Input.GetButtonDown("Fire2") && !this.paused && !flag4 && Kube.BCS.gameType == GameType.creating)
			{
				bool flag5 = this.canBuildBlock || this.canBuild;
				if ((num2 == 0 || num2 == 1 || num2 == 3) && !flag5)
				{
					Kube.GPS.printMessage(Localize.cant_build_ask_admin, Color.yellow);
				}
				else if ((num2 == 0 || num2 == 1 || num2 == 3) && !Kube.BCS.canChangeWorld)
				{
					Kube.GPS.printMessage(Localize.cant_change_world, Color.yellow);
				}
				else if (!flag2)
				{
					Kube.GPS.printMessage(Localize.not_enougth_cubes, Color.yellow);
				}
				else if (num2 == 0)
				{
					Ray ray5 = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
					RaycastHit rch;
					if (Physics.Raycast(ray5, out rch, 10f, 256))
					{
						Vector3 vector2 = new Vector3(Mathf.Round(rch.point.x + rch.normal.x * 0.02f), Mathf.Round(rch.point.y + rch.normal.y * 0.02f), Mathf.Round(rch.point.z + rch.normal.z * 0.02f));
						ushort cubeFill3 = Kube.WHS.GetCubeFill((int)vector2.x, (int)vector2.y, (int)vector2.z);
						byte b = (byte)this.GeometryCode(this._geom, rch);
						if (cubeFill3 != 0)
						{
							byte cubeData2 = Kube.WHS.GetCubeData((int)vector2.x, (int)vector2.y, (int)vector2.z);
							if (b == cubeData2 && (cubeData2 == 1 || cubeData2 == 2))
							{
								this.PlaceNewCube(vector2, num3, 0);
							}
							else
							{
								Kube.GPS.printMessage(Localize.cube_occupied, Color.white);
								MonoBehaviour.print("Filled with: " + Kube.WHS.cubes[(int)vector2.x, (int)vector2.y, (int)vector2.z].type);
								MonoBehaviour.print(string.Concat(new object[]
								{
									(int)vector2.x,
									" ",
									(int)vector2.y,
									" ",
									(int)vector2.z
								}));
							}
						}
						else if (Vector3.Distance(vector2, base.transform.position + Vector3.up) > 1.5f)
						{
							this.PlaceNewCube(vector2, num3, (int)b);
						}
					}
				}
				else if (num2 == 1 || num2 == 3)
				{
					Ray ray6 = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
					RaycastHit raycastHit5;
					if (Physics.Raycast(ray6, out raycastHit5, 10f, 8448))
					{
						ItemPropsScript component7 = Kube.ASS3.gameItemsGO[num3].GetComponent<ItemPropsScript>();
						if (component7.placeType == ItemPlaceType.onTheItem && raycastHit5.collider.gameObject.layer != 13)
						{
							Kube.GPS.printMessage(Localize.put_on_items, Color.white);
						}
						else if (component7.placeType == ItemPlaceType.onTheItem && raycastHit5.collider.gameObject.layer == 13)
						{
							ItemPropsScript component8 = raycastHit5.collider.gameObject.GetComponent<ItemPropsScript>();
							this.NO.CreateGameItem(num3, 0, Mathf.RoundToInt(component8.gameObject.transform.position.x), Mathf.RoundToInt(component8.gameObject.transform.position.y), Mathf.RoundToInt(component8.gameObject.transform.position.z), this.id);
							BattleControllerScript bcs = Kube.BCS;
							bcs.bonusCounters.placedItem = bcs.bonusCounters.placedItem + 1;
							if (Kube.IS.UseItem(num3) == 1)
							{
							}
							if (Kube.BCS.gameType == GameType.teams)
							{
								this.availableCubes--;
							}
						}
						else
						{
							Vector3 vector3 = new Vector3(Mathf.Round(raycastHit5.point.x + raycastHit5.normal.x * 0.02f), Mathf.Round(raycastHit5.point.y + raycastHit5.normal.y * 0.02f), Mathf.Round(raycastHit5.point.z + raycastHit5.normal.z * 0.02f));
							ushort cubeFill4 = Kube.WHS.GetCubeFill((int)vector3.x, (int)vector3.y, (int)vector3.z);
							bool flag6 = true;
							if (cubeFill4 != 0 && cubeFill4 != 128)
							{
								Kube.GPS.printMessage(Localize.cube_occupied, Color.white);
								flag6 = false;
							}
							if ((int)vector3.x < 0 || (int)vector3.x >= Kube.WHS.sizeX || (int)vector3.y < 0 || (int)vector3.y >= Kube.WHS.sizeY || (int)vector3.z < 0 || (int)vector3.z >= Kube.WHS.sizeZ)
							{
								Kube.GPS.printMessage(Localize.beside_world, Color.white);
								flag6 = false;
							}
							if (flag6)
							{
								if (component7.buildMagic && !component7.magic)
								{
									int num4 = -1;
									if ((double)Mathf.Round(raycastHit5.normal.z) == 1.0)
									{
										num4 = 0;
									}
									else if ((double)Mathf.Round(raycastHit5.normal.z) == -1.0)
									{
										num4 = 3;
									}
									else if ((double)Mathf.Round(raycastHit5.normal.x) == 1.0)
									{
										num4 = 1;
									}
									else if ((double)Mathf.Round(raycastHit5.normal.x) == -1.0)
									{
										num4 = 2;
									}
									else if ((double)Mathf.Round(raycastHit5.normal.y) == -1.0)
									{
										num4 = 5;
									}
									else if ((double)Mathf.Round(raycastHit5.normal.y) == 1.0)
									{
										num4 = 4;
									}
									this.NO.CreateGameItem(num3, (byte)num4, Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z), this.id);
									BattleControllerScript bcs2 = Kube.BCS;
									bcs2.bonusCounters.placedItem = bcs2.bonusCounters.placedItem + 1;
									if (Kube.IS.UseItem(num3) == 1)
									{
									}
									if (Kube.BCS.gameType == GameType.teams)
									{
										this.availableCubes--;
									}
								}
								else if (!component7.magic)
								{
									MonoBehaviour.print("place item");
									if (component7.placeType == ItemPlaceType.fourRotations)
									{
										int num5 = -1;
										if ((double)Mathf.Round(raycastHit5.normal.z) == 1.0)
										{
											num5 = 0;
										}
										else if ((double)Mathf.Round(raycastHit5.normal.z) == -1.0)
										{
											num5 = 3;
										}
										else if ((double)Mathf.Round(raycastHit5.normal.x) == 1.0)
										{
											num5 = 1;
										}
										else if ((double)Mathf.Round(raycastHit5.normal.x) == -1.0)
										{
											num5 = 2;
										}
										if (num5 != -1)
										{
											this.NO.CreateGameItem(num3, (byte)num5, Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z), this.id);
											BattleControllerScript bcs3 = Kube.BCS;
											bcs3.bonusCounters.placedItem = bcs3.bonusCounters.placedItem + 1;
											if (Kube.IS.UseItem(num3) == 1)
											{
											}
											if (Kube.BCS.gameType == GameType.teams)
											{
												this.availableCubes--;
											}
										}
										else
										{
											Kube.GPS.printMessage(Localize.place_on_cube_side, Color.yellow);
										}
									}
									else if (component7.placeType == ItemPlaceType.onTheCeil)
									{
										int num6 = -1;
										if ((double)Mathf.Round(raycastHit5.normal.y) == -1.0)
										{
											num6 = 0;
										}
										if (num6 != -1)
										{
											this.NO.CreateGameItem(num3, (byte)num6, Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z), this.id);
											BattleControllerScript bcs4 = Kube.BCS;
											bcs4.bonusCounters.placedItem = bcs4.bonusCounters.placedItem + 1;
											if (Kube.IS.UseItem(num3) == 1)
											{
											}
											if (Kube.BCS.gameType == GameType.teams)
											{
												this.availableCubes--;
											}
										}
										else
										{
											Kube.GPS.printMessage(Localize.place_on_ceil, Color.yellow);
										}
									}
									if (component7.placeType == ItemPlaceType.likeCube)
									{
										this.NO.CreateGameItem(num3, 0, Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z), this.id);
										BattleControllerScript bcs5 = Kube.BCS;
										bcs5.bonusCounters.placedItem = bcs5.bonusCounters.placedItem + 1;
										if (Kube.IS.UseItem(num3) == 1)
										{
										}
										if (Kube.BCS.gameType == GameType.teams)
										{
											this.availableCubes--;
										}
										if (Kube.GPS.needTrainingBuild)
										{
											Kube.TS.SendMessage("PlacedCubelikeItem");
										}
									}
								}
							}
						}
					}
				}
			}
			if (UnityEngine.Input.GetAxis("Fire1") > 0f && !this.paused)
			{
				bool flag7 = Kube.BCS.gameType == GameType.creating && (num2 == 0 || num2 == 1 || num2 == 3);
				if (flag7 && !this.canBuild)
				{
					Kube.GPS.printMessage(Localize.cant_build_ask_admin, Color.yellow);
				}
				else if (flag7 && !Kube.BCS.canChangeWorld)
				{
					Kube.GPS.printMessage(Localize.cant_change_world, Color.yellow);
				}
				else if (flag7 && !flag2)
				{
					Kube.GPS.printMessage(Localize.cant_already_remove, Color.yellow);
				}
				else if (Kube.BCS.gameType == GameType.creating && (num2 == 0 || num2 == 1 || num2 == 3 || num2 == -1))
				{
					Ray ray7 = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
					RaycastHit raycastHit6;
					if (Physics.Raycast(ray7, out raycastHit6, 10f, 256))
					{
						Vector3 vector4 = new Vector3(Mathf.Round(raycastHit6.point.x - raycastHit6.normal.x * 0.02f), Mathf.Round(raycastHit6.point.y - raycastHit6.normal.y * 0.02f), Mathf.Round(raycastHit6.point.z - raycastHit6.normal.z * 0.02f));
						int num7 = (int)Kube.WHS.cubes[(int)vector4.x, (int)vector4.y, (int)vector4.z].type;
						if (raycastHit6.collider.gameObject.layer == 8)
						{
							if (!this.isCracking)
							{
								this.isCracking = true;
								this.crackingPos = vector4;
								this.crackingStartTime = Time.time;
								this.crackingTime = Mathf.Max(Kube.OH.cubesStrength[num7] * 0.15f, 0.4f);
								Kube.OH.crackCube.SetActive(true);
								Kube.OH.crackCube.transform.position = this.crackingPos;
							}
							if (this.isCracking)
							{
								if (this.crackingPos != vector4)
								{
									this.crackingPos = vector4;
									this.crackingStartTime = Time.time;
									this.crackingTime = Mathf.Max(Kube.OH.cubesStrength[num7] * 0.15f, 0.4f);
									Kube.OH.crackCube.transform.position = this.crackingPos;
								}
								if ((Time.time - this.crackingStartTime) / this.crackingTime >= 1f)
								{
									this.isCracking = false;
									Kube.OH.crackCube.SetActive(false);
									Kube.WHS.PlayCubeHit(vector4, SoundHitType.breaking);
									this.NO.PlaceNewCube(vector4, 0, 0);
									if (Kube.BCS.gameType == GameType.teams)
									{
										this.availableCubes--;
									}
									if (Kube.GPS.needTrainingBuild)
									{
										Kube.TS.SendMessage("DestroyedCube");
									}
								}
								else
								{
									Kube.OH.crackCube.renderer.material = Kube.ASS3.crackCubeMats[Mathf.FloorToInt(10f * (Time.time - this.crackingStartTime) / this.crackingTime)];
								}
							}
						}
						else if (this.isCracking)
						{
							this.isCracking = false;
							Kube.OH.crackCube.SetActive(false);
						}
					}
				}
				else if (this.currentWeapon != -1 && flag && !this.rechargingWeapon && Time.time - this.lastShotTimeNew[this.currentWeapon] >= Kube.IS.weaponParams[this.currentWeapon].DeltaShot)
				{
					int bulletsType = Kube.IS.weaponParams[this.currentWeapon].BulletsType;
					if (this.clips[this.currentWeapon] >= Kube.IS.weaponParams[this.currentWeapon].UsingBullets)
					{
						int num8 = this.clips[this.currentWeapon];
						PlayerScript.Clips clips2;
						PlayerScript.Clips clips = clips2 = this.clips;
						int num9;
						int index = num9 = this.currentWeapon;
						num9 = clips2[num9];
						clips[index] = num9 - Kube.IS.weaponParams[this.currentWeapon].UsingBullets;
						this.lastShotTimeNew[this.currentWeapon] = Time.time;
						Ray ray8 = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
						this.CreateShot(ray8.origin, ray8.direction, this.currentWeapon);
						if (num8 - Kube.IS.weaponParams[this.currentWeapon].UsingBullets != this.clips[this.currentWeapon])
						{
							Kube.OH.usedCheat = true;
							this.NO.BanPlayer(Kube.GPS.playerId);
						}
					}
					else
					{
						this.CreateEmptyClipEvent(this.currentWeapon);
						this.lastShotTimeNew[this.currentWeapon] = Time.time;
						this.ReloadGun();
					}
				}
			}
			else if (this.isCracking)
			{
				this.isCracking = false;
				Kube.OH.crackCube.SetActive(false);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.R))
			{
				this.ReloadGun();
			}
			if (this.rechargingWeapon && Time.time > this.rechargingWeaponStart + Kube.IS.weaponParams[this.rechargingWeaponType].reloadTime[Kube.IS.weaponParams[this.rechargingWeaponType].currentReloadTimeIndex])
			{
				this.rechargingWeapon = false;
				base.animation.CrossFade(this.weaponRechargeEndAnim, 0.05f);
				int num10 = Kube.IS.weaponParams[this.rechargingWeaponType].clipSize[Kube.IS.weaponParams[this.rechargingWeaponType].currentClipSizeIndex] - this.clips[this.rechargingWeaponType];
				num10 = Mathf.Min(num10, this.bullets[Kube.IS.weaponParams[this.rechargingWeaponType].BulletsType]);
				PlayerScript.Clips clips4;
				PlayerScript.Clips clips3 = clips4 = this.clips;
				int num9;
				int index2 = num9 = this.rechargingWeaponType;
				num9 = clips4[num9];
				clips3[index2] = num9 + num10;
				PlayerScript.Bullets bullets2;
				PlayerScript.Bullets bullets = bullets2 = this.bullets;
				int index3 = num9 = Kube.IS.weaponParams[this.rechargingWeaponType].BulletsType;
				num9 = bullets2[num9];
				bullets[index3] = num9 - num10;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.F2))
			{
				bool flag8 = true;
				if (this.isDriveTransport && this.transportToDriveScript.driverIsHidden[this.transportToDrivePlace])
				{
					flag8 = false;
				}
				if (flag8)
				{
					this.view3face = !this.view3face;
					this.SetView(this.view3face);
				}
			}
		}
	}

	public Ray getCamRay()
	{
		return this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
	}

	private void Update()
	{
		this.painAlpha -= 0.2f * Time.deltaTime;
		if (this.painAlpha < 0f)
		{
			this.painAlpha = 0f;
		}
		if (this.type == 0)
		{
			this.LocalUpdate();
		}
		if (this.type == 1)
		{
			Vector3 vector = Vector3.Lerp(base.transform.position, this.correctPlayerPos, Time.deltaTime * (float)PhotonNetwork.sendRateOnSerialize);
			CubePhys cubePhysType = Kube.WHS.GetCubePhysType(base.transform.position - Vector3.up * 0.5f);
			if ((base.transform.position - vector).magnitude / Time.deltaTime > 0.4f && Time.time - this.lastStepTime > this.stepDeltaTime && cubePhysType != CubePhys.air)
			{
				Kube.WHS.PlayCubeHit(base.transform.position - Vector3.up * 0.5f, SoundHitType.footSteps);
				this.lastStepTime = Time.time;
			}
			if (cubePhysType == CubePhys.water && this.currentTypePhysFloor == CubePhys.air)
			{
				UnityEngine.Object.Instantiate(Kube.ASS4.soundWaterSplash, base.transform.position, Quaternion.identity);
			}
			this.currentTypePhysFloor = cubePhysType;
			base.transform.position = vector;
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.correctPlayerRot, Time.deltaTime * (float)PhotonNetwork.sendRateOnSerialize);
		}
		if (!this.dead && (this.type != 0 || this.view3face))
		{
			if (this.isDriveTransport)
			{
				this.transportToDriveScript.AnimateDriver(this.transportToDrivePlace, this);
			}
			else
			{
				Vector3 direction = (base.transform.position - this.lastPos) / Time.deltaTime;
				this.lastPos = base.transform.position;
				CubePhys cubePhysType2 = Kube.WHS.GetCubePhysType(base.transform.position - Vector3.up * 0.5f);
				if (cubePhysType2 == CubePhys.solid || cubePhysType2 == CubePhys.ledder)
				{
					direction = base.transform.InverseTransformDirection(direction);
					if (direction.magnitude > 0.5f)
					{
						if (Mathf.Abs(direction.z) > Mathf.Abs(direction.x) * 0.8f)
						{
							if (this.currentWeapon == -1)
							{
								base.animation.CrossFade(this.animRunEmpty);
								base.animation[this.animRunEmpty].speed = direction.z / 5f;
							}
							else if (Kube.IS.weaponParams[this.currentWeapon].Type == 0)
							{
								base.animation.CrossFade(this.animRunSword);
								base.animation[this.animRunSword].speed = direction.z / 5f;
							}
							else if (Kube.IS.weaponParams[this.currentWeapon].Type == 1)
							{
								base.animation.CrossFade(this.animRunWeapon);
								base.animation[this.animRunWeapon].speed = direction.z / 5f;
							}
						}
						else if (direction.x < 0f)
						{
							if (this.currentWeapon == -1)
							{
								base.animation.CrossFade(this.animRunLeftEmpty);
								base.animation[this.animRunLeftEmpty].speed = -direction.x / 5f;
							}
							else if (Kube.IS.weaponParams[this.currentWeapon].Type == 0)
							{
								base.animation.CrossFade(this.animRunLeftSword);
								base.animation[this.animRunLeftSword].speed = -direction.x / 5f;
							}
							else if (Kube.IS.weaponParams[this.currentWeapon].Type == 1)
							{
								base.animation.CrossFade(this.animRunLeftWeapon);
								base.animation[this.animRunLeftWeapon].speed = -direction.x / 5f;
							}
						}
						else if (direction.x > 0f)
						{
							if (this.currentWeapon == -1)
							{
								base.animation.CrossFade(this.animRunRightEmpty);
								base.animation[this.animRunRightEmpty].speed = direction.x / 5f;
							}
							else if (Kube.IS.weaponParams[this.currentWeapon].Type == 0)
							{
								base.animation.CrossFade(this.animRunRightSword);
								base.animation[this.animRunRightSword].speed = direction.x / 5f;
							}
							else if (Kube.IS.weaponParams[this.currentWeapon].Type == 1)
							{
								base.animation.CrossFade(this.animRunRightWeapon);
								base.animation[this.animRunRightWeapon].speed = direction.x / 5f;
							}
						}
					}
					else if (this.currentWeapon == -1)
					{
						base.animation.CrossFade(this.animIdleEmpty);
					}
					else if (Kube.IS.weaponParams[this.currentWeapon].Type == 0)
					{
						base.animation.CrossFade(this.animIdleSword);
					}
					else if (Kube.IS.weaponParams[this.currentWeapon].Type == 1)
					{
						base.animation.CrossFade(this.animIdleWeapon);
					}
				}
				else if (this.currentWeapon < 0 || this.currentWeapon >= Kube.IS.weaponParams.Length)
				{
					base.animation.CrossFade(this.animIdleEmpty);
				}
				else if (Kube.IS.weaponParams[this.currentWeapon].Type == 0)
				{
					base.animation.CrossFade(this.animIdleSword);
				}
				else if (Kube.IS.weaponParams[this.currentWeapon].Type == 1)
				{
					base.animation.CrossFade(this.animIdleWeapon);
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (this.transportToDriveScript == null)
		{
			this.ExitTransport(Vector3.zero);
		}
		if (this.isDriveTransport)
		{
			base.transform.position = this.transportToDriveScript.GetDriveTransform(this.transportToDrivePlace).position;
			base.transform.rotation = this.transportToDriveScript.GetDriveTransform(this.transportToDrivePlace).rotation;
			this.transportToDriveScript.LateAnimateDriver(this.transportToDrivePlace, this);
		}
		else if (this.view3face && !this.dead)
		{
			Vector3 axis = base.transform.TransformDirection(Vector3.right);
			this.newRotationY = Mathf.Lerp(this.newRotationY, this.rotationY, Time.deltaTime * 5f);
			this.headTransform.RotateAround(axis, Mathf.Min(Mathf.Max(-this.newRotationY * 0.0174532924f - 0.3f, -1.5f), 1.5f));
			if (this.currentWeapon >= 0 && this.currentWeapon < Kube.IS.weaponParams.Length)
			{
				if (Kube.IS.weaponParams[this.currentWeapon].Type == 0)
				{
					this.rightHandTransform.RotateAround(axis, -Mathf.Max(this.rotationY, -25f) * 0.0174532924f);
				}
				else
				{
					this.rightHandTransform.RotateAround(axis, -this.newRotationY * 0.0174532924f);
				}
			}
		}
	}

	private void SetView(bool _isFP)
	{
		this.view3face = _isFP;
		if (this.view3face)
		{
			this.skin.SetActive(true);
			this.bones.SetActive(true);
			if (base.photonView.isMine)
			{
				this.cameraComp.SendMessage("SetPosition", new Vector3(0.5f, 0f, -2.5f));
			}
			else
			{
				this.cameraComp.transform.parent.gameObject.SetActive(false);
			}
		}
		else if (!this.view3face)
		{
			this.skin.SetActive(false);
			this.bones.SetActive(false);
			if (base.photonView.isMine)
			{
				this.cameraComp.SendMessage("SetPosition", Vector3.zero);
			}
			else
			{
				this.cameraComp.transform.parent.gameObject.SetActive(false);
			}
		}
		this.RedrawWeapon();
	}

	private void CreateRechargeSound(int numWeapon)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_CreateRechargeSound", PhotonTargets.All, new object[]
			{
				numWeapon
			});
		}
		else
		{
			this._CreateRechargeSound(numWeapon, null);
		}
	}

	[RPC]
	private void _CreateRechargeSound(int numWeapon, PhotonMessageInfo info)
	{
		this.weaponGOScript.WeaponReloadSound();
	}

	private void CreateEmptyClipEvent(int numWeapon)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_CreateEmptyClipEvent", PhotonTargets.All, new object[]
			{
				numWeapon
			});
		}
		else
		{
			this._CreateEmptyClipEvent(numWeapon, null);
		}
	}

	[RPC]
	private void _CreateEmptyClipEvent(int numWeapon, PhotonMessageInfo info)
	{
		this.weaponGOScript.WeaponEmptyClip();
	}

	private Vector3 calcShotPoint(Vector3 rayOrigin, Vector3 rayDirection, float Distance)
	{
		Ray ray = new Ray(rayOrigin, rayDirection);
		int num = 38657;
		if (PhotonNetwork.offlineMode || base.photonView.isMine)
		{
			num -= 512;
		}
		RaycastHit raycastHit;
		Vector3 result;
		if (Physics.Raycast(ray, out raycastHit, Distance, num))
		{
			if (raycastHit.distance > 1f)
			{
				result = raycastHit.point;
			}
			else
			{
				result = ray.origin + ray.direction * Distance;
			}
		}
		else
		{
			result = ray.origin + ray.direction * Distance;
		}
		return result;
	}

	private void CreateShot(Vector3 rayOrigin, Vector3 rayDirection, int numWeapon)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_CreateShot", PhotonTargets.All, new object[]
			{
				rayOrigin,
				rayDirection,
				numWeapon
			});
		}
		else
		{
			this._CreateShot(rayOrigin, rayDirection, numWeapon, null);
		}
	}

	[RPC]
	private void _CreateShot(Vector3 rayOrigin, Vector3 rayDirection, int numWeapon, PhotonMessageInfo info)
	{
		Vector3 shotPoint = this.calcShotPoint(rayOrigin, rayDirection, Kube.IS.weaponParams[numWeapon].Distance);
		DamageMessage damageMessage = new DamageMessage();
		if (PhotonNetwork.offlineMode || base.photonView.isMine)
		{
			damageMessage.damage = (short)Kube.IS.weaponParams[numWeapon].Damage[Kube.IS.weaponParams[numWeapon].currentDamageIndex];
		}
		else
		{
			damageMessage.damage = 0;
		}
		damageMessage.id_killer = this.id;
		damageMessage.team = this.team;
		damageMessage.weaponType = (short)numWeapon;
		this.weaponGOScript.WeaponShot(Kube.ASS6.weaponsBulletPrefab[numWeapon], shotPoint, damageMessage);
		if (this.type == 0 && !this.view3face && this.weaponAnim1face[numWeapon].Length != 0)
		{
			base.animation.Rewind(this.weaponAnim1face[numWeapon]);
			base.animation.Play(this.weaponAnim1face[numWeapon]);
		}
		if (this.currentWeapon != -1 && (this.type == 1 || (this.type == 0 && this.view3face)) && Kube.IS.weaponParams[this.currentWeapon].Type == 0)
		{
			base.animation.CrossFade(this.animSwordAttack[UnityEngine.Random.Range(0, this.animSwordAttack.Length)], 0.1f);
		}
		if (Time.time - this.lastMonstersStartle > this.monstersStartleDeltaTime && Kube.BCS.gameType != GameType.survival)
		{
			this.lastMonstersStartle = Time.time;
			GameObject[] array = GameObject.FindGameObjectsWithTag("Monster");
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SendMessage("Startle", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void DrawAims()
	{
		if (this.dead)
		{
			return;
		}
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		Texture texture = null;
		if (this.currentWeapon > Kube.IS.weaponParams.Length)
		{
			this.currentWeapon = -1;
		}
		if (Kube.BCS.gameType != GameType.creating)
		{
			if (this.rifleAim && this.currentWeapon == 11)
			{
				texture = Kube.ASS3.rifleAimTex;
			}
			else if (this.rifleAim && this.currentWeapon == 23)
			{
				texture = Kube.ASS3.spaceRifleAimTex;
			}
			else if (this.rifleAim && this.currentWeapon == 31)
			{
				texture = Kube.ASS3.tacticRifleAimTex;
			}
			else if (!this.rifleAim && !Kube.OH.emptyScreen && this.currentWeapon != -1 && Kube.IS.weaponParams[this.currentWeapon].aimTex.Length >= 2)
			{
				if (Time.time - this.lastShotTimeNew[this.currentWeapon] < 0.15f)
				{
					texture = Kube.IS.weaponParams[this.currentWeapon].aimTex[1];
				}
				else
				{
					texture = Kube.IS.weaponParams[this.currentWeapon].aimTex[0];
				}
			}
		}
		int num3;
		if (Kube.BCS.gameType != GameType.creating)
		{
			num3 = Kube.GPS.fastInventarWeapon[Kube.IS.chosenFastInventar].Type;
			int num4 = Kube.GPS.fastInventarWeapon[Kube.IS.chosenFastInventar].Num;
		}
		else
		{
			num3 = Kube.GPS.fastInventar[Kube.IS.chosenFastInventar].Type;
			int num4 = Kube.GPS.fastInventar[Kube.IS.chosenFastInventar].Num;
		}
		if (texture == null && !this.rifleAim && !Kube.OH.emptyScreen && num3 != 4)
		{
			texture = Kube.ASS3.aimTex;
		}
		if (texture != null)
		{
			UIHUD uihud = Kube.BCS.hud;
			Kube.BCS.hud.aim.mainTexture = texture;
			if (!this.rifleAim)
			{
				Kube.BCS.hud.aim.width = texture.width;
				Kube.BCS.hud.aim.height = texture.height;
			}
			else
			{
				Kube.BCS.hud.aim.width = Cub2UI.activeWidth;
				Kube.BCS.hud.aim.height = Cub2UI.activeHeight;
			}
			return;
		}
	}

	private void DrawChat()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		bool flag = true;
		GUI.skin = Kube.ASS1.mainSkinSmall;
		GUI.SetNextControlName("chatMessage");
		if (Event.current.Equals(Event.KeyboardEvent("return")))
		{
			if (this.chatMessage.Length != 0)
			{
				string text = this.playerName;
				if (this.dead)
				{
					text += "(RIP)";
				}
				text = text + ": " + AuxFunc.CodeRussianName(this.chatMessage);
				this.ChatMessage(text);
			}
			flag = false;
			this.paused = false;
		}
		this.chatMessage = GUI.TextField(new Rect(0.2f * num, 0.2f * num2, 0.6f * num, 0.08f * num2), this.chatMessage, 64);
		GUI.FocusControl("chatMessage");
		if (GUI.Button(new Rect(0.8f * num, 0.2f * num2, 0.1f * num, 0.08f * num2), "Enter"))
		{
			if (this.chatMessage.Length != 0)
			{
				string text2 = this.playerName;
				if (this.dead)
				{
					text2 += "(RIP)";
				}
				text2 = text2 + ": " + AuxFunc.CodeRussianName(this.chatMessage);
				this.ChatMessage(text2);
			}
			flag = false;
			this.paused = false;
		}
		if (!flag)
		{
			this.hud.Remove(new DrawCall(this.DrawChat));
		}
	}

	private void OnGUI()
	{
		if (this.type != 0)
		{
			return;
		}
		if (this._flashTime > Time.time)
		{
			this.DrawFlash();
		}
		if (Kube.OH.emptyScreen)
		{
			return;
		}
		if (this.type != 0)
		{
			return;
		}
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		if (Kube.ASS2 == null)
		{
			Kube.SS.require("Assets2");
			return;
		}
		if (this.onlyMove && this.moveItem)
		{
			GUI.skin = Kube.ASS1.mainSkin;
			GUI.Box(new Rect(num * 0.5f - 300f, num2 - 150f, 600f, 90f), Localize.ps_choose_new_item_place);
		}
		if (this.type == 0)
		{
			if (this.hud.Count > 0)
			{
				this.hud[this.hud.Count - 1]();
			}
			if (this.dead)
			{
				if (Kube.BCS.gameType == GameType.mission)
				{
					GUI.skin = Kube.ASS1.mainSkinSmall;
					if (this._canRespawn)
					{
						GUI.Box(new Rect(0.5f * num - 200f, num2 - 150f, 400f, 35f), Localize.ps_press_for_respawn);
					}
					else if (Kube.GPS.inventarItems[109] == 0)
					{
						GUI.Box(new Rect(0.5f * num - 300f, num2 - 150f, 600f, 100f), Localize.ps_you_dead_try_again + "\n" + Localize.ps_use_vita_water);
					}
					else
					{
						GUI.Box(new Rect(0.5f * num - 300f, num2 - 150f, 600f, 100f), string.Concat(new object[]
						{
							Localize.ps_you_dead_try_again,
							"\n",
							Localize.ps_press_for_use_vita_water,
							"(",
							Kube.GPS.inventarItems[109],
							")"
						}));
					}
				}
				else if (Kube.BCS.gameType != GameType.survival)
				{
					GUI.skin = Kube.ASS1.mainSkinSmall;
					GUI.Box(new Rect(0.5f * num - 200f, num2 - 150f, 400f, 35f), Localize.ps_press_for_respawn);
				}
				else
				{
					GUI.skin = Kube.ASS1.mainSkinSmall;
					string text = Localize.ps_use_vita_water;
					if (Kube.GPS.inventarItems[109] > 0)
					{
						text = string.Concat(new object[]
						{
							Localize.ps_press_for_use_vita_water,
							"(",
							Kube.GPS.inventarItems[109],
							")"
						});
					}
					GUI.Box(new Rect(0.5f * num - 300f, num2 - 150f, 600f, 80f), string.Concat(new object[]
					{
						Localize.ps_survival_dead,
						"\n",
						Localize.ps_before_respawn_secs,
						": ",
						Mathf.RoundToInt(this.survivalRespawnTime - Time.time),
						Localize.sec,
						"\n",
						text
					}));
				}
				float num3 = Mathf.Max(0f, Mathf.Min(this.painAlpha, 1f));
				if (num3 > 0.02f)
				{
					GUI.color = new Color(1f, 0f, 0f, num3);
					GUI.DrawTexture(new Rect(0f, 0f, num, num2), Kube.ASS3.darkness);
				}
			}
			else if (!this.paused)
			{
				if (this.rechargingWeapon)
				{
					GUI.DrawTexture(new Rect(0.5f * num - 50f, 0.5f * num2 + 20f, 100f, 16f), Kube.ASS1.levelLine);
					float num4 = (Time.time - this.rechargingWeaponStart) / Kube.IS.weaponParams[this.rechargingWeaponType].reloadTime[Kube.IS.weaponParams[this.rechargingWeaponType].currentReloadTimeIndex];
					num4 = Mathf.Min(1f, num4);
					GUI.DrawTexture(new Rect(0.5f * num - 48f, 0.5f * num2 + 22f, 96f * num4, 12f), Kube.ASS1.levelProgress);
				}
				GUI.skin = Kube.ASS1.smallWhiteSkin;
				GUI.Label(new Rect(0.5f * num, 0.5f * num2, 400f, 120f), this.guiItemText);
				if (this.isDriveTransport)
				{
					GUI.skin = Kube.ASS1.smallWhiteSkin;
					GUI.Label(new Rect(0.5f * num, 0.5f * num2, 400f, 120f), Localize.press_to_end_drive);
				}
				if (this.carryingTheFlag && Kube.BCS.gameType == GameType.captureTheFlag)
				{
					Color color = GUI.color;
					GUI.color = new Color(1f, 1f, 0f, 0.7f + Mathf.Sin(Time.time * 6f) * 0.3f);
					GUI.Label(new Rect(0.2f * num, num2 - 180f, 0.6f * num, 30f), Localize.you_have_flag);
					GUI.color = color;
				}
				float num5 = Mathf.Max(0f, Mathf.Min(this.painAlpha, 1f));
				if (num5 > 0.02f)
				{
					GUI.color = new Color(1f, 0f, 0f, num5);
					GUI.DrawTexture(new Rect(0f, 0f, num, num2), Kube.ASS3.darkness);
				}
			}
			if (this.freezed)
			{
				GUI.color = new Color(0f, 0f, 1f, 0.2f);
				GUI.DrawTexture(new Rect(0f, 0f, num, num2), Kube.ASS3.darkness);
			}
		}
	}

	private void DrawActivitiesMenu()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		GUI.skin = Kube.ASS1.bigWhiteLabel;
		float num3 = 70f + 35f * (float)Kube.IS.charMovesNums.Length;
		float num4 = 0.5f * num - 163f;
		float num5 = 0.5f * num2 - num3 / 2f;
		GUI.DrawTexture(new Rect(num4, num5, 326f, num3), Kube.ASS1.menuBack);
		GUI.Label(new Rect(num4 + 10f, num5 + 10f, 306f, 50f), Localize.activities_title);
		for (int i = 0; i < Kube.IS.charMovesNums.Length; i++)
		{
			if (Kube.GPS.inventarSpecItems[Kube.IS.charMovesNums[i]] > 0)
			{
				if (GUI.Button(new Rect(num4 + 10f, num5 + 70f + (float)i * 35f, 300f, 30f), Localize.specItemsName[Kube.IS.charMovesNums[i]]))
				{
					if (!this.view3face)
					{
						this.SetView(true);
					}
					this.PlayActivity(Kube.IS.charMovesNums[i]);
					Kube.OH.closeMenu(new DrawCall(this.DrawActivitiesMenu));
				}
			}
			else if (GUI.Button(new Rect(num4 + 10f, num5 + 70f + (float)i * 35f, 300f, 30f), Localize.specItemsName[Kube.IS.charMovesNums[i]] + " (" + Localize.move_learn + ")"))
			{
				Kube.OH.closeMenu(new DrawCall(this.DrawActivitiesMenu));
				Kube.IS.SendMessage("ToggleInventarCharMoves", Kube.IS.charMovesNums[i]);
			}
		}
	}

	public void PlayActivity(int numActivity)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_PlayActivity", PhotonTargets.All, new object[]
			{
				numActivity
			});
		}
		else
		{
			this._PlayActivity(numActivity, null);
		}
	}

	[RPC]
	private void _PlayActivity(int numActivity, PhotonMessageInfo info)
	{
		if (numActivity == 1)
		{
			base.animation.Play(this.animDecor[0]);
		}
		else if (numActivity == 2)
		{
			base.animation.Play(this.animDecor[1]);
		}
		else if (numActivity == 3)
		{
			base.animation.Play(this.animDecor[2]);
		}
		else if (numActivity == 4)
		{
			base.animation.Play(this.animDecor[3]);
		}
		else if (numActivity == 5)
		{
			base.animation.Play(this.animDecor[4]);
		}
		else if (numActivity == 6)
		{
			base.animation.Play(this.animDecor[5]);
		}
		else if (numActivity == 7)
		{
			base.animation.Play(this.animDecor[6]);
		}
		else if (numActivity == 8)
		{
			base.animation.Play(this.animDecor[7]);
		}
	}

	public void ChatMessage(string message)
	{
		if (message == string.Empty)
		{
			return;
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ChatMessage", PhotonTargets.All, new object[]
			{
				message
			});
		}
		else
		{
			this._ChatMessage(message, null);
		}
	}

	[RPC]
	private void _ChatMessage(string _message, PhotonMessageInfo info)
	{
		Kube.GPS.printMessage(AuxFunc.DecodeRussianName(_message), Color.white);
	}

	public void DriveTransport(int _transportId, int _placeToDrive)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Transport");
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].GetComponent<TransportScript>().transportId == _transportId)
			{
				this.transportToDriveScript = array[i].GetComponent<TransportScript>();
				this.transportToDrivePlace = _placeToDrive;
				this.isDriveTransport = true;
				this.controller.enabled = false;
				if (this.transportToDriveScript.driverIsHidden[this.transportToDrivePlace])
				{
					this.SetView(false);
				}
				if (base.photonView.isMine && !this.transportToDriveScript.driverCanUseOwnWeapon[this.transportToDrivePlace])
				{
					this.playerView.enabled = false;
				}
				break;
			}
		}
	}

	private void ExitTransport(Vector3 exitVector)
	{
		if (!this.isDriveTransport)
		{
			return;
		}
		if (this.transportToDriveScript.driverIsHidden[this.transportToDrivePlace] && this.type == 1)
		{
			this.SetView(true);
		}
		this.isDriveTransport = false;
		this.transportToDrivePlace = 0;
		this.transportToDriveScript = null;
		this.controller.enabled = true;
		base.transform.position += exitVector;
		this.moveDirection = Vector3.zero;
		if (base.photonView.isMine)
		{
			this.cameraComp.SendMessage("UnsetTemporaryTransform");
			this.playerView.enabled = true;
		}
	}

	public void PlayerDressSkin()
	{
		this.playerSkin = Kube.GPS.playerSkin;
		this.playerClothes = Kube.GPS.playerClothesStr;
		this.RecountBonuces();
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_PlayerDressSkin", PhotonTargets.All, new object[]
			{
				this.playerSkin,
				this.playerClothes
			});
		}
		else
		{
			this._PlayerDressSkin(this.playerSkin, this.playerClothes, null);
		}
	}

	[RPC]
	private void _PlayerDressSkin(int newSkin, string newClothes, PhotonMessageInfo info)
	{
		if (base.gameObject != null)
		{
			this.playerSkin = newSkin;
			this.playerClothes = newClothes;
			base.gameObject.SendMessage("DressSkin", string.Concat(new object[]
			{
				string.Empty,
				this.playerSkin,
				";",
				Kube.GPS.skinItems[this.playerSkin],
				";",
				this.playerClothes
			}));
		}
	}

	public void ChangeLayersRecursively(Transform trans, string name)
	{
		foreach (object obj in trans)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.layer = LayerMask.NameToLayer(name);
			this.ChangeLayersRecursively(transform, name);
		}
	}

	public void ChangeWeapon(int _numWeapon)
	{
		if (this.currentWeapon == _numWeapon)
		{
			if (this.currentWeapon == -1)
			{
				return;
			}
			int weaponGroup = (int)Kube.IS.weaponParams[_numWeapon].weaponGroup;
			_numWeapon = Kube.IS.findNextWeapon(this.currentWeapon, weaponGroup);
			if (_numWeapon == -1)
			{
				return;
			}
			Kube.GPS.fastInventarWeapon[weaponGroup] = new FastInventar(InventarType.weapons, _numWeapon);
		}
		this.rifleAim = false;
		this.currentWeapon = _numWeapon;
		this.showFastInventoryTime = Time.time;
		this.rechargingWeapon = false;
		if (this.weaponGO != null)
		{
			UnityEngine.Object.Destroy(this.weaponGO);
			this.weaponGOScript = null;
		}
		if (this.dead)
		{
			return;
		}
		if (_numWeapon >= 0 && _numWeapon < Kube.ASS6.charWeaponsGO.Length)
		{
			this.weaponGO = (UnityEngine.Object.Instantiate(Kube.ASS6.charWeaponsGO[_numWeapon], Vector3.zero, Quaternion.identity) as GameObject);
			this.weaponGOScript = this.weaponGO.GetComponent<WeaponScript>();
			this.weaponGOScript.owner = this;
			this.weaponGOScript.accuarcy = Kube.IS.weaponParams[_numWeapon].accuarcy;
			this.weaponGOScript.fatalDistance = Kube.IS.weaponParams[_numWeapon].fatalDistance;
		}
		this.RedrawWeapon();
	}

	private void RedrawWeapon()
	{
		if (this.weaponGO == null)
		{
			return;
		}
		if (!this.view3face)
		{
			this.weaponGO.transform.parent = this.weaponObjCamera.transform;
			this.weaponGO.transform.localPosition = Vector3.zero;
			this.weaponGO.transform.localRotation = Quaternion.identity;
			base.animation.Rewind(this.changeWeaponAnim);
			base.animation.Play(this.changeWeaponAnim);
			if (this.playerView != null)
			{
				this.playerView.enabled = true;
				this.weaponGO.layer = LayerMask.NameToLayer("FPSWeapon");
				this.ChangeLayersRecursively(this.weaponGO.transform, "FPSWeapon");
			}
		}
		else
		{
			this.weaponGO.transform.parent = this.weaponObjHand.transform;
			this.weaponGO.transform.localPosition = Vector3.zero;
			this.weaponGO.transform.localRotation = Quaternion.identity;
			if (this.playerView != null)
			{
				this.playerView.enabled = false;
			}
			this.weaponGO.layer = LayerMask.NameToLayer("Default");
			this.ChangeLayersRecursively(this.weaponGO.transform, "Default");
		}
	}

	private void ApplyDamage(DamageMessage dm)
	{
		if (!Kube.BCS.IsNormPing(this.currentPing))
		{
			MonoBehaviour.print("Lagger damage");
			return;
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ApplyDamage", PhotonTargets.All, new object[]
			{
				dm.damage,
				dm.id_killer,
				dm.team,
				dm.weaponType,
				dm.damagePos
			});
		}
		else
		{
			this._ApplyDamage(dm.damage, dm.id_killer, dm.team, dm.weaponType, dm.damagePos, null);
		}
	}

	private void ApplyFlash(Vector3 pos)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ApplyFlash", PhotonTargets.All, new object[]
			{
				pos
			});
		}
		else
		{
			this._ApplyFlash(pos, null);
		}
	}

	private bool InfiniteCameraCanSeePoint(Camera camera, Vector3 point)
	{
		Vector3 point2 = camera.WorldToViewportPoint(point);
		bool result;
		if (point2.z > 0f)
		{
			Rect rect = new Rect(0f, 0f, 1f, 1f);
			result = rect.Contains(point2);
		}
		else
		{
			result = false;
		}
		return result;
	}

	[RPC]
	private void _ApplyFlash(Vector3 pos, PhotonMessageInfo info)
	{
		if (this.InfiniteCameraCanSeePoint(this.cameraComp, pos))
		{
			this._flashTime = Time.time + 20f;
		}
	}

	private void DrawFlash()
	{
		if (this.DrawFlashTx == null)
		{
			this.DrawFlashTx = new Texture2D(1, 1);
			this.DrawFlashTx.SetPixel(0, 0, new Color(1f, 1f, 1f));
			this.DrawFlashTx.Apply();
		}
		float a = (this._flashTime - Time.time) / 20f;
		GUI.color = new Color(1f, 1f, 1f, a);
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.DrawFlashTx);
		GUI.color = new Color(1f, 1f, 1f);
	}

	[RPC]
	private void _ApplyDamage(short _damage, int _id_killer, int _team, short _weaponType, Vector3 _damagePos, PhotonMessageInfo info)
	{
		if (((Kube.BCS.gameType == GameType.mission && _team == this.team) || (Kube.BCS.gameType == GameType.teams && _team == this.team) || (Kube.BCS.gameType == GameType.survival && _id_killer > 0) || (Kube.BCS.gameType == GameType.captureTheFlag && _team == this.team) || (Kube.BCS.gameType == GameType.dominating && _team == this.team)) && _id_killer != this.id)
		{
			return;
		}
		if (this.type == 0)
		{
			bool isHeadshot = false;
			if (this.dead)
			{
				return;
			}
			if (_damagePos.y - base.transform.position.y > 1.1f && _damagePos.y - base.transform.position.y < 1.8f)
			{
				isHeadshot = true;
				_damage = (short)((float)_damage * 1.5f);
			}
			int num = this.health + this.armor;
			float num2 = this.reduceDamage + this.reduceDamageBonus;
			_damage = (short)((float)_damage * (1f - num2));
			this.armor -= (int)(_damage * 3);
			if (this.armor < 0)
			{
				this.health += this.armor / 3;
				this.armor = 0;
			}
			if (_damage > 0 && num <= this.health + this.armor)
			{
				Kube.OH.usedCheat = true;
				this.NO.BanPlayer(Kube.GPS.playerId);
			}
			this.painAlpha += 0.02f * (float)_damage;
			if (this.painAlpha > 1f)
			{
				this.painAlpha = 1f;
			}
			if (this.health <= 0)
			{
				this.painAlpha += 0.05f * (float)_damage;
				if (this.painAlpha > 1f)
				{
					this.painAlpha = 1f;
				}
				this.Die(_id_killer, this.pointsForKillMe, isHeadshot, _weaponType, _damage);
			}
		}
	}

	private void LoseFlag(FlagState newState = FlagState.dropped)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Flag");
		for (int i = 0; i < array.Length; i++)
		{
			FlagScript component = array[i].GetComponent<FlagScript>();
			if (component.flagState.state == FlagState.captured && component.flagState.playerCaptured == this.id)
			{
				Kube.BCS.NO.ChangeFlagState(component.flagState.team, newState, this.id);
				break;
			}
		}
	}

	private void OnDestroy()
	{
		this.LoseFlag(FlagState.dropped);
	}

	private void Die(int id_killer, int myPoints, bool isHeadshot, short weaponType, short damage)
	{
		this.LoseFlag(FlagState.dropped);
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_Die", PhotonTargets.All, new object[]
			{
				id_killer,
				myPoints,
				isHeadshot,
				weaponType,
				damage
			});
		}
		else
		{
			this._Die(id_killer, myPoints, isHeadshot, weaponType, damage, null);
		}
	}

	[RPC]
	private void _Die(int id_killer, int myPoints, bool isHeadshot, short weaponType, short damage, PhotonMessageInfo info)
	{
		if (this.dead)
		{
			return;
		}
		if (base.photonView.isMine)
		{
			Kube.IS.resetInventory();
		}
		if (Kube.IS.ps && Kube.IS.ps.id == id_killer)
		{
			BattleControllerScript bcs = Kube.BCS;
			bcs.bonusCounters.kills = bcs.bonusCounters.kills + 1;
			if (this.id == id_killer)
			{
				BattleControllerScript bcs2 = Kube.BCS;
				bcs2.bonusCounters.selfKill = bcs2.bonusCounters.selfKill + 1;
			}
			else
			{
				if (isHeadshot)
				{
					BattleControllerScript bcs3 = Kube.BCS;
					bcs3.bonusCounters.headshots = bcs3.bonusCounters.headshots + 1;
				}
				if (weaponType == 6 || weaponType == 7 || weaponType == 17 || weaponType == 19 || weaponType == 26 || weaponType == 27)
				{
					BattleControllerScript bcs4 = Kube.BCS;
					bcs4.bonusCounters.explosions = bcs4.bonusCounters.explosions + 1;
				}
				if (this.id == id_killer)
				{
					BattleControllerScript bcs5 = Kube.BCS;
					bcs5.bonusCounters.selfKill = bcs5.bonusCounters.selfKill + 1;
				}
				if (weaponType >= 0 && (int)weaponType < Kube.IS.weaponParams.Length && Kube.IS.weaponParams[(int)weaponType].Type == 0)
				{
					BattleControllerScript bcs6 = Kube.BCS;
					bcs6.bonusCounters.nearFights = bcs6.bonusCounters.nearFights + 1;
				}
			}
		}
		this.ChangeWeapon(-1);
		this.dead = true;
		if (Kube.BCS.gameType == GameType.mission)
		{
			this._canRespawn = Kube.BCS.gameTypeController.canRespawn;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(this.ragdoll, base.transform.position, base.transform.rotation) as GameObject;
		PlayerScript.CopyTransformsRecurse(base.transform, gameObject.transform);
		gameObject.SendMessage("DressSkin", string.Concat(new object[]
		{
			string.Empty,
			this.playerSkin,
			";",
			Kube.GPS.skinItems[this.playerSkin],
			";",
			this.playerClothes
		}));
		this._ragDollTrans = gameObject.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine");
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		base.gameObject.layer = 2;
		this.deadTimes++;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int j = 0; j < array.Length; j++)
		{
			PlayerScript component = array[j].GetComponent<PlayerScript>();
			if (!(component == null))
			{
				if (component.id == id_killer && id_killer != this.id)
				{
					array[j].GetComponent<PlayerScript>().YouKilledPlayerFull(id_killer, this.id, 0, myPoints, isHeadshot);
					if (base.photonView.isMine && component.team != this.team)
					{
						this.availableCubes = Kube.GPS.maxAvailableCubes;
						if (Kube.BCS.gameType == GameType.teams)
						{
							this.NO.ChangeTeamScore(1, component.team);
						}
					}
					break;
				}
			}
		}
		if (id_killer == 0)
		{
			Kube.GPS.printSystemMessage(AuxFunc.DecodeRussianName(this.playerName) + " " + Localize.dead_by_nature, new Color(1f, 1f, 1f, 0.5f));
		}
		else if (id_killer == this.id)
		{
			Kube.GPS.printSystemMessage(AuxFunc.DecodeRussianName(this.playerName) + " " + Localize.dead_himself, new Color(1f, 1f, 1f, 0.5f));
		}
		else if (id_killer < 0)
		{
			Kube.GPS.printSystemMessage(AuxFunc.DecodeRussianName(this.playerName) + " " + Localize.dead_by_zombie, new Color(1f, 1f, 1f, 0.5f));
		}
		if (base.photonView.isMine || PhotonNetwork.offlineMode)
		{
			for (int k = 0; k < 10; k++)
			{
				if (Kube.GPS.fastInventarWeapon[k].Type == 4 && Kube.GPS.inventarWeapons[Kube.GPS.fastInventarWeapon[k].Num] == 0)
				{
					Kube.GPS.fastInventarWeapon[k].Type = -1;
					Kube.GPS.fastInventarWeapon[k].Num = 0;
				}
			}
			this.deadTime = Time.time;
			base.transform.position -= this.cameraComp.transform.TransformDirection(Vector3.forward) * 5f;
			Kube.BCS.gameObject.SendMessage("PlayerDie", SendMessageOptions.DontRequireReceiver);
			if (this.isDriveTransport)
			{
				this.transportToDriveScript.ExitDrive(this.id);
			}
		}
		if (Kube.BCS.gameType == GameType.survival)
		{
			if (Kube.GPS.vipEnd - Time.time > 0f)
			{
				this.survivalRespawnTime = Time.time + 10f;
			}
			else
			{
				this.survivalRespawnTime = Time.time + 30f;
			}
			this.survivalRespawnGO = (UnityEngine.Object.Instantiate(this.survivalRespawnPrefab, base.transform.position, base.transform.rotation) as GameObject);
			this.survivalRespawnGO.SendMessage("SetPlayerGO", base.gameObject);
		}
		if (Kube.GPS.playerId == id_killer && this.id != Kube.GPS.playerId)
		{
			(UnityEngine.Object.Instantiate(Kube.OH.pointsText, base.transform.position + Vector3.up * 2f, Quaternion.identity) as GameObject).SendMessage("SetText", "+" + myPoints);
		}
		this.rifleAim = false;
		this.cameraComp.fieldOfView = 60f;
		if (this.playerView != null)
		{
			this.playerView.fieldOfView = 60f;
		}
		Vector3 localPosition = this.weaponObjCamera.transform.localPosition;
		localPosition.x = 0.361f;
		this.weaponObjCamera.transform.localPosition = localPosition;
		this.sensitivityX = (this.sensitivityY = 5f);
	}

	private void SurvivalRespawn(Vector3 pos)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_Respawn", PhotonTargets.All, new object[]
			{
				pos
			});
		}
		else
		{
			this._Respawn(pos, null);
		}
	}

	public void Respawn()
	{
		Vector3 position = new Vector3(1f, 40f, 1f);
		GameObject[] array = new GameObject[0];
		Time.timeScale = 1f;
		if (Kube.BCS.gameType == GameType.mission && !this._canRespawn)
		{
			return;
		}
		if (Kube.BCS.gameType == GameType.creating || Kube.BCS.gameType == GameType.shooter || Kube.BCS.gameType == GameType.test || Kube.BCS.gameType == GameType.survival)
		{
			array = GameObject.FindGameObjectsWithTag("Respawn");
			if (array.Length != 0)
			{
				position = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
			}
			if (PhotonNetwork.room != null)
			{
				base.photonView.RPC("_Respawn", PhotonTargets.All, new object[]
				{
					position
				});
			}
			else
			{
				this._Respawn(position, null);
			}
		}
		else if (Kube.BCS.gameType == GameType.teams || Kube.BCS.gameType == GameType.captureTheFlag || Kube.BCS.gameType == GameType.dominating)
		{
			if (this.team == 0)
			{
				array = GameObject.FindGameObjectsWithTag("RespawnRed");
			}
			if (this.team == 1)
			{
				array = GameObject.FindGameObjectsWithTag("RespawnBlue");
			}
			if (this.team == 2)
			{
				array = GameObject.FindGameObjectsWithTag("RespawnGreen");
			}
			if (this.team == 3)
			{
				array = GameObject.FindGameObjectsWithTag("RespawnYellow");
			}
			if (array.Length != 0)
			{
				position = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
			}
			this.LoseFlag(FlagState.onBase);
			if (PhotonNetwork.room != null)
			{
				base.photonView.RPC("_Respawn", PhotonTargets.All, new object[]
				{
					position
				});
			}
			else
			{
				this._Respawn(position, null);
			}
		}
		else if (Kube.BCS.gameType == GameType.mission || Kube.BCS.gameType == GameType.test)
		{
			array = GameObject.FindGameObjectsWithTag("Respawn");
			if (array.Length != 0)
			{
				position = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
			}
			if (PhotonNetwork.room != null)
			{
				base.photonView.RPC("_Respawn", PhotonTargets.All, new object[]
				{
					position
				});
			}
			else
			{
				this._Respawn(position, null);
			}
		}
	}

	[RPC]
	private void _Respawn(Vector3 position, PhotonMessageInfo info)
	{
		if (this.survivalRespawnGO != null)
		{
			UnityEngine.Object.Destroy(this.survivalRespawnGO);
		}
		this.dead = false;
		this.carryingTheFlag = false;
		this.health = this.maxHealth;
		this.armor = this.maxArmor;
		base.transform.position = position;
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = true;
		}
		if (PhotonNetwork.offlineMode || base.photonView.isMine)
		{
			base.gameObject.layer = 9;
			this.type = 0;
			this.id = Kube.GPS.playerId;
			Kube.IS.ps = this;
			Kube.IS.ChoseFastInventar(0);
			this.SetView(false);
			Kube.BCS.gameObject.SendMessage("PlayerRespawn", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			this.cameraComp.gameObject.SetActive(false);
			base.gameObject.layer = 10;
			this.type = 1;
			this.id = (int)base.photonView.owner.customProperties["id"];
			this.SetView(true);
		}
		Time.timeScale = 1f;
		this.Spawn();
	}

	private static void CopyTransformsRecurse(Transform src, Transform dst)
	{
		dst.position = src.position;
		dst.rotation = src.rotation;
		if (dst.gameObject.rigidbody != null)
		{
			dst.gameObject.rigidbody.Sleep();
		}
		foreach (object obj in dst)
		{
			Transform transform = (Transform)obj;
			Transform transform2 = src.Find(transform.name);
			if (transform2)
			{
				PlayerScript.CopyTransformsRecurse(transform2, transform);
			}
		}
	}

	private void YouKilledMonster(int _points)
	{
		this.kills++;
		this.points += _points;
		Kube.BCS.gameObject.SendMessage("KilledMonster", SendMessageOptions.DontRequireReceiver);
	}

	private void YouKilledPlayer(int _points)
	{
		this.kills++;
		this.points += _points;
		Kube.BCS.gameObject.SendMessage("KilledPlayer", SendMessageOptions.DontRequireReceiver);
	}

	public void YouKilledPlayerFull(int killer_id, int dead_id, short weaponType, int _points, bool isHeadshot)
	{
		this.kills++;
		this.points += _points;
		Kube.BCS.gameObject.SendMessage("KilledPlayer", SendMessageOptions.DontRequireReceiver);
		if (killer_id == Kube.GPS.playerId)
		{
			string str = string.Empty;
			for (int i = 0; i < Kube.BCS.playersInfo.Length; i++)
			{
				if (Kube.BCS.playersInfo[i].Id == dead_id)
				{
					str = AuxFunc.DecodeRussianName(Kube.BCS.players[i].GetComponent<PlayerScript>().playerName);
				}
			}
			ArrayList arrayList = new ArrayList();
			arrayList.Add(Color.white);
			arrayList.Add(22);
			arrayList.Add(0.75f);
			arrayList.Add(0.5f);
			arrayList.Add(Localize.you_killed + " " + str + ((!isHeadshot) ? string.Empty : (" " + Localize.headshot)));
			(UnityEngine.Object.Instantiate(Kube.OH.pointsText, base.transform.position + Vector3.up * 2f, Quaternion.identity) as GameObject).SendMessage("SetText", arrayList);
		}
		else if (dead_id == Kube.GPS.playerId)
		{
			string str2 = string.Empty;
			for (int j = 0; j < Kube.BCS.playersInfo.Length; j++)
			{
				if (Kube.BCS.playersInfo[j].Id == killer_id)
				{
					str2 = AuxFunc.DecodeRussianName(Kube.BCS.players[j].GetComponent<PlayerScript>().playerName);
				}
			}
			ArrayList arrayList2 = new ArrayList();
			arrayList2.Add(Color.red);
			arrayList2.Add(30);
			arrayList2.Add(0.75f);
			arrayList2.Add(0.5f);
			arrayList2.Add(Localize.you_was_killed_by + " " + str2 + ((!isHeadshot) ? string.Empty : (" " + Localize.headshot)));
			(UnityEngine.Object.Instantiate(Kube.OH.pointsText, base.transform.position + Vector3.up * 2f, Quaternion.identity) as GameObject).SendMessage("SetText", arrayList2);
		}
		string text = string.Empty;
		for (int k = 0; k < Kube.BCS.playersInfo.Length; k++)
		{
			if (Kube.BCS.playersInfo[k].Id == dead_id)
			{
				text = AuxFunc.DecodeRussianName(Kube.BCS.players[k].GetComponent<PlayerScript>().playerName);
			}
		}
		string text2 = string.Empty;
		for (int l = 0; l < Kube.BCS.playersInfo.Length; l++)
		{
			if (Kube.BCS.playersInfo[l].Id == killer_id)
			{
				text2 = AuxFunc.DecodeRussianName(Kube.BCS.players[l].GetComponent<PlayerScript>().playerName);
			}
		}
		Kube.GPS.printSystemMessage(string.Concat(new string[]
		{
			text2,
			" ",
			Localize.killed,
			" ",
			text,
			(!isHeadshot) ? string.Empty : (" " + Localize.headshot)
		}), new Color(1f, 1f, 1f, 0.5f));
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (PhotonNetwork.connected)
		{
			if (stream.isWriting)
			{
				if (Time.time - this.lastSendProps > 5f)
				{
					stream.SendNext(1);
					stream.SendNext(this.id);
					this.lastSendProps = Time.time;
				}
				else
				{
					stream.SendNext(2);
				}
				stream.SendNext(base.transform.position);
				stream.SendNext(base.transform.rotation);
				stream.SendNext((short)this.currentWeapon);
				stream.SendNext(this.jetPackOn);
				stream.SendNext(this.jetPackWork);
				byte b = (byte)Mathf.RoundToInt(this.rotationY + 90f);
				stream.SendNext(b);
			}
			else
			{
				this.currentPing = Time.realtimeSinceStartup - this.lastPingTime;
				if (this.lastPingTime != 0f)
				{
					Kube.BCS.CollectPing(this.currentPing);
				}
				this.lastPingTime = Time.realtimeSinceStartup;
				byte b2 = (byte)stream.ReceiveNext();
				if (b2 == 1)
				{
					this.id = (int)stream.ReceiveNext();
				}
				this.correctPlayerPos = (Vector3)stream.ReceiveNext();
				this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
				short num = (short)stream.ReceiveNext();
				if ((int)num != this.currentWeapon)
				{
					this.ChangeWeapon((int)num);
				}
				bool flag = (bool)stream.ReceiveNext();
				if (flag != this.jetPackOn)
				{
					this.jetPackOn = flag;
					this.DressJetPack(this.jetPackOn);
				}
				bool flag2 = (bool)stream.ReceiveNext();
				if (this.jetPackOn)
				{
					this.jetPackGO.SendMessage("PlayStop", flag2, SendMessageOptions.DontRequireReceiver);
				}
				byte b3 = (byte)stream.ReceiveNext();
				this.rotationY = (float)b3 - 90f;
			}
		}
	}

	public void GetNewBullets(int bulletsType, int bulletsAmount)
	{
		UnityEngine.Object.Instantiate(Kube.ASS4.soundGetItem, base.transform.position, base.transform.rotation);
		if (bulletsType < 10)
		{
			for (int i = 0; i < Kube.IS.bulletParams.Length; i++)
			{
				if (Kube.IS.bulletParams[i].bulletGroup == (InventoryScript.BulletGroup)bulletsType)
				{
					PlayerScript.Bullets bullets2;
					PlayerScript.Bullets bullets = bullets2 = this.bullets;
					int num;
					int index = num = i;
					num = bullets2[num];
					bullets[index] = num + Kube.IS.bulletParams[i].puckupAmount;
				}
			}
		}
		else if (bulletsType == 10)
		{
			this.health += bulletsAmount;
			if (this.health > this.maxHealth)
			{
				this.health = this.maxHealth;
			}
		}
		else if (bulletsType == 11)
		{
			this.armor += bulletsAmount;
			if (this.armor > this.maxArmor)
			{
				this.armor = this.maxArmor;
			}
		}
	}

	public void GetNewWeapon(int weaponType, int bulletsAmount)
	{
		UnityEngine.Object.Instantiate(Kube.ASS4.soundGetItem, base.transform.position, base.transform.rotation);
		for (int i = 0; i < Kube.IS.bulletParams.Length; i++)
		{
			if (i == Kube.IS.weaponParams[weaponType].BulletsType)
			{
				PlayerScript.Bullets bullets2;
				PlayerScript.Bullets bullets = bullets2 = this.bullets;
				int num;
				int index = num = i;
				num = bullets2[num];
				bullets[index] = num + bulletsAmount;
			}
			this.bullets[i] = Math.Min(this.bullets[i], Kube.IS.bulletParams[i].initialAmount);
		}
		this._weaponPickup[weaponType] = 1;
		for (int j = 0; j < 10; j++)
		{
			if (Kube.GPS.fastInventarWeapon[j].Type == 4 && Kube.GPS.fastInventarWeapon[j].Num == weaponType)
			{
				return;
			}
		}
		int weaponGroup = (int)Kube.IS.weaponParams[weaponType].weaponGroup;
		if (Kube.GPS.fastInventarWeapon[weaponGroup].Type == -1)
		{
			Kube.GPS.fastInventarWeapon[weaponGroup].Type = 4;
			Kube.GPS.fastInventarWeapon[weaponGroup].Num = weaponType;
			Kube.IS.ChoseFastInventar(weaponGroup);
		}
	}

	public void RestoreBullets(string bulletsToRestore)
	{
		int[] array = new int[4];
		array[0] += Kube.OH.DecodeServerCode(bulletsToRestore.Substring(0, 2));
		array[1] += Kube.OH.DecodeServerCode(bulletsToRestore.Substring(2, 2));
		array[2] += Kube.OH.DecodeServerCode(bulletsToRestore.Substring(4, 2));
		array[3] += Kube.OH.DecodeServerCode(bulletsToRestore.Substring(6, 2));
		for (int i = 0; i < array.Length; i++)
		{
			for (int j = 0; j < Kube.IS.bulletParams.Length; j++)
			{
				if (Kube.IS.bulletParams[j].bulletGroup == (InventoryScript.BulletGroup)i)
				{
					PlayerScript.Bullets bullets2;
					PlayerScript.Bullets bullets = bullets2 = this.bullets;
					int num;
					int index = num = j;
					num = bullets2[num];
					bullets[index] = num + Math.Min(Kube.IS.bulletParams[j].puckupAmount, array[i]);
				}
			}
		}
	}

	public void RestoreHealth()
	{
		this.health = this.maxHealth;
		this.armor = this.maxArmor;
		Kube.GPS.printMessage(Localize.ps_health_and_armor_restored, Color.green);
	}

	public void InventarCheat()
	{
		Kube.OH.usedCheat = true;
		this.NO.BanPlayer(Kube.GPS.playerId);
	}

	public bool HaveKeys(bool _red, bool _green, bool _blue, bool _gold)
	{
		return true;
	}

	private void Teleport(Vector3 pos)
	{
		for (int i = 0; i < 3; i++)
		{
			Vector3 vector = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y + (float)i), Mathf.Round(pos.z));
			if ((int)vector.x < 0 || (int)vector.x >= Kube.WHS.sizeX || (int)vector.y < 0 || (int)vector.y >= Kube.WHS.sizeY || (int)vector.z < 0 || (int)vector.z >= Kube.WHS.sizeZ)
			{
				return;
			}
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_Teleport", PhotonTargets.All, new object[]
			{
				pos
			});
		}
		else
		{
			this._Teleport(pos, null);
		}
	}

	[RPC]
	private void _Teleport(Vector3 position, PhotonMessageInfo info)
	{
		UnityEngine.Object.Instantiate(Kube.ASS4.teleportSound, base.transform.position, Quaternion.identity);
		base.transform.position = position;
		UnityEngine.Object.Instantiate(Kube.ASS4.teleportSound, base.transform.position, Quaternion.identity);
	}

	private void Freeze(FreezeStruct fs)
	{
		if (Kube.BCS.gameType == GameType.mission && fs.team == this.team)
		{
			return;
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_Freeze", PhotonTargets.All, new object[]
			{
				fs.freezeTime
			});
		}
		else
		{
			this._Freeze(fs.freezeTime, null);
		}
	}

	[RPC]
	private void _Freeze(float freezeTime, PhotonMessageInfo info)
	{
		if (base.photonView.isMine)
		{
			base.Invoke("UnFreeze", freezeTime);
		}
		this.freezed = true;
		this.rechargingWeapon = false;
	}

	private void UnFreeze()
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_UnFreeze", PhotonTargets.All, new object[0]);
		}
		else
		{
			this._UnFreeze(null);
		}
	}

	[RPC]
	private void _UnFreeze(PhotonMessageInfo info)
	{
		this.freezed = false;
	}

	private void SaveCodeVars()
	{
		this.codeVarsRandom = UnityEngine.Random.Range(10, 1000);
		this._availableCubes2 = this.availableCubes + this.codeVarsRandom;
		this._currentWeapon2 = this.currentWeapon + this.codeVarsRandom;
		this._health2 = this.health + this.codeVarsRandom;
		this._maxHealth2 = this.maxHealth + this.codeVarsRandom;
		this._armor2 = this.armor + this.codeVarsRandom;
		this._kills2 = this.kills + this.codeVarsRandom;
		this._points2 = this.points + this.codeVarsRandom;
		this._playerSkin2 = this.playerSkin + this.codeVarsRandom;
		this._level2 = this.level + this.codeVarsRandom;
		for (int i = 0; i < 12; i++)
		{
			this._bullets2[i] = this.bullets[i] + this.codeVarsRandom;
		}
		for (int j = 0; j < 64; j++)
		{
			this._lastShotTimeNew2[j] = this.lastShotTimeNew[j] + (float)this.codeVarsRandom;
		}
		for (int k = 0; k < 64; k++)
		{
			this._clips2[k] = this.clips[k] + this.codeVarsRandom;
		}
	}

	private void LoadCodeVars()
	{
		this.availableCubes = this._availableCubes2 - this.codeVarsRandom;
		this.currentWeapon = this._currentWeapon2 - this.codeVarsRandom;
		this.health = this._health2 - this.codeVarsRandom;
		this.maxHealth = this._maxHealth2 - this.codeVarsRandom;
		this.armor = this._armor2 - this.codeVarsRandom;
		this.kills = this._kills2 - this.codeVarsRandom;
		this.points = this._points2 - this.codeVarsRandom;
		this.playerSkin = this._playerSkin2 - this.codeVarsRandom;
		this.level = this._level2 - this.codeVarsRandom;
		for (int i = 0; i < 12; i++)
		{
			this.bullets[i] = this._bullets2[i] - this.codeVarsRandom;
		}
		for (int j = 0; j < 64; j++)
		{
			this.lastShotTimeNew[j] = this._lastShotTimeNew2[j] - (float)this.codeVarsRandom;
		}
		for (int k = 0; k < 64; k++)
		{
			this.clips[k] = this._clips2[k] - this.codeVarsRandom;
		}
	}

	protected const float ITEM_USE_TIMEOUT = 2f;

	private const int MAX_GEOM = 3;

	private const float FLASH_DURATION = 20f;

	public string animIdleEmpty;

	public string animIdleSword;

	public string animIdleWeapon;

	public string animRunEmpty;

	public string animRunSword;

	public string animRunWeapon;

	public string animRunLeftEmpty;

	public string animRunRightEmpty;

	public string animRunLeftSword;

	public string animRunRightSword;

	public string animRunLeftWeapon;

	public string animRunRightWeapon;

	public string animAction;

	public string[] animSwordAttack;

	public string animWeaponShoot;

	public string[] animDecor;

	public string animQuadroSit;

	public string weaponRechargeBeginAnim;

	public string weaponRechargeEndAnim;

	public float saveFallVelocity = 10f;

	public float fallDamage = 10f;

	public float runSpeed = 5f;

	public float runSpeedBonus;

	public float jumpSpeed = 10f;

	public float jumpSpeedBonus;

	private CharacterController controller;

	private NetworkObjectScript NO;

	public int type;

	public float sensitivityX = 15f;

	public float sensitivityY = 15f;

	public float minimumY = -90f;

	public float maximumY = 90f;

	[HideInInspector]
	public float rotationY;

	[HideInInspector]
	public float rotationX;

	[HideInInspector]
	public float newRotationY;

	[HideInInspector]
	public float newRotationX;

	public Camera cameraComp;

	public GameObject skin;

	public GameObject bones;

	public GameObject weaponObjCamera;

	public GameObject weaponObjHand;

	public GameObject weaponGO;

	private WeaponScript weaponGOScript;

	private bool isCracking;

	private Vector3 crackingPos;

	private float crackingStartTime;

	private float crackingTime;

	public bool paused;

	public bool onlyMove;

	public Transform flagHolder;

	public bool carryingTheFlag;

	public int numCarryingFlag;

	private bool rechargingWeapon;

	private int rechargingWeaponType;

	private float rechargingWeaponStart;

	private int _availableCubes;

	private GameObject gameObjectToDelete;

	public PlayerScript.Bullets bullets = new PlayerScript.Bullets();

	public PlayerScript.Clips clips = new PlayerScript.Clips();

	private int _currentWeapon;

	public int id;

	public GameObject ragdoll;

	public bool dead;

	private int _health;

	public int maxArmor = 100;

	public int maxArmorBonus;

	public float painAlpha;

	public float reduceDamage;

	public float reduceDamageBonus;

	public int pointsForKillMe = 10;

	public int deadTimes;

	public PlayerScript.LastShotTime lastShotTimeNew = new PlayerScript.LastShotTime();

	private int _kills;

	private int _points;

	public bool canBuild;

	public bool canBuildBlock;

	public string playerName = string.Empty;

	private string chatMessage = string.Empty;

	private int _playerSkin;

	public string[] weaponAnim1face;

	public string changeWeaponAnim;

	public float stepDeltaTime = 0.3f;

	private float lastStepTime;

	public GameObject survivalRespawnPrefab;

	public GameObject rankPlane;

	private GameObject survivalRespawnGO;

	public int _level;

	private int _health3;

	public int _maxHealth;

	public int maxHealthBonus;

	private int _armor;

	public int team = -1;

	private bool jetPackOn;

	private bool jetPackWork;

	private float jetPackFuel = 1f;

	public GameObject jetPackGO;

	private int constantsCash;

	public Camera playerView;

	protected float _nextItemUse;

	protected Transform _neck;

	public string uid = string.Empty;

	private GameObject _targetCursor;

	private GameObject _targetCube;

	private GameObject _targetPlane;

	private bool initialized;

	protected Dictionary<int, int> _weaponPickup;

	private Dictionary<int, PlayerScript.InventarItems> inventarItems;

	private bool grounded = true;

	private Vector3 moveDirection;

	private float forwardRun;

	private float sideRun;

	public float rotateDirMax = 20f;

	public float rotateSensivity = 10f;

	private bool view3face;

	private bool _rifleAim;

	private bool moveItem;

	private GameObject gameObjectToMove;

	private float deadTime;

	private bool controlLeft;

	private bool controlRight;

	private bool controlForward;

	private bool controlBackward;

	private CubePhys currentTypePhysFloor;

	[HideInInspector]
	protected CubePhys typePhys;

	[HideInInspector]
	protected List<DrawCall> hud = new List<DrawCall>();

	protected int _geom;

	protected static int[] geometryIds = new int[]
	{
		0,
		2,
		3,
		4
	};

	[HideInInspector]
	public Transform headTransform;

	[HideInInspector]
	public Transform rightHandTransform;

	private Vector3 lastPos;

	private float lastMonstersStartle;

	private float monstersStartleDeltaTime = 3f;

	private string guiItemText = string.Empty;

	public bool isDriveTransport;

	public TransportScript transportToDriveScript;

	private int transportToDrivePlace;

	private string playerClothes = string.Empty;

	protected float showFastInventoryTime;

	protected float _flashTime;

	private Texture2D DrawFlashTx;

	private float survivalRespawnTime;

	private Transform _ragDollTrans;

	protected bool _canRespawn;

	private int killedWithoutDeath;

	private int killedMultiTimes;

	private float killedMultiTimesLastTime;

	public float killedMultiTimesMaxDeltaTime = 1.5f;

	private Vector3 correctPlayerPos = new Vector3(-10000f, -10000f, 0f);

	private Quaternion correctPlayerRot = Quaternion.identity;

	private float lastSendProps;

	private float lastPingTime;

	private float currentPing;

	private bool freezed;

	private int codeVarsRandom;

	private int _availableCubes2;

	private int _currentWeapon2;

	private int _health2;

	private int _maxHealth2;

	private int _armor2;

	private int _kills2;

	private int _points2;

	private int _playerSkin2;

	private int _level2;

	private int[] _bullets2 = new int[12];

	private int[] _clips2 = new int[64];

	private float[] _lastShotTimeNew2 = new float[64];

	public class Bullets
	{
		public int this[int index]
		{
			get
			{
				return -this._bullets[index] + Kube.GPS.codeI;
			}
			set
			{
				this._bullets[index] = Kube.GPS.codeI - value;
			}
		}

		private int[] _bullets = new int[12];
	}

	public class Clips
	{
		public int this[int index]
		{
			get
			{
				return -this._clips[index] + Kube.GPS.codeI;
			}
			set
			{
				this._clips[index] = Kube.GPS.codeI - value;
			}
		}

		private int[] _clips = new int[64];
	}

	public class LastShotTime
	{
		public float this[int index]
		{
			get
			{
				return -this._lastShotTimeNew[index] + Kube.GPS.codeF;
			}
			set
			{
				this._lastShotTimeNew[index] = Kube.GPS.codeF - value;
			}
		}

		private float[] _lastShotTimeNew = new float[64];
	}

	private struct InventarItems
	{
		public float nextUse;

		public int cnt;
	}
}
