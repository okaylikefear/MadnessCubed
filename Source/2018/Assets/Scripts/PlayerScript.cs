using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using kube;
using kube.ui;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

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

	public int frags
	{
		get
		{
			return this._frags >> 3;
		}
		set
		{
			this._frags = value << 3;
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

	public static PlayerScript FromId(int id_killer)
	{
		for (int i = 0; i < Kube.BCS.playersInfo.Length; i++)
		{
			PlayerScript ps = Kube.BCS.playersInfo[i].ps;
			if (ps.photonView.viewID == id_killer)
			{
				return ps;
			}
		}
		return null;
	}

	public static PlayerScript FromPhoton(PhotonPlayer owner)
	{
		for (int i = 0; i < Kube.BCS.playersInfo.Length; i++)
		{
			PlayerScript ps = Kube.BCS.playersInfo[i].ps;
			if (ps.photonView.owner == owner)
			{
				return ps;
			}
		}
		return null;
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
			this.playerView.renderingPath = RenderingPath.VertexLit;
		}
		this.initialized = true;
	}

	private void Awake()
	{
		this._light = base.GetComponentInChildren<Light>();
		this._anim = base.GetComponent<Animation>();
	}

	private void Start()
	{
		this.sensitivityX = (this.sensitivityY = Kube.GPS.mouseSens);
		if (base.photonView.owner.name != string.Empty)
		{
			this.uid = base.photonView.owner.name;
		}
		this._neck = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Neck");
		this.headTransform = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Neck/Bip01 Head");
		this.rightHandTransform = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Neck/Bip01 R Clavicle");
		this._anim[this.animSwordAttack[0]].layer = 5;
		this._anim[this.animSwordAttack[1]].layer = 5;
		this._anim[this.animSwordAttack[2]].layer = 5;
		this._anim[this.animSwordAttack[0]].speed = 2.2f;
		this._anim[this.animSwordAttack[1]].speed = 2.2f;
		this._anim[this.animSwordAttack[2]].speed = 2.2f;
		this._anim[this.animIdleEmpty].speed = 0.5f;
		this._anim[this.animIdleSword].speed = 0.5f;
		this._anim[this.animIdleWeapon].speed = 0.5f;
		this._anim[this.animSwordAttack[0]].AddMixingTransform(this._neck);
		this._anim[this.animSwordAttack[1]].AddMixingTransform(this._neck);
		this._anim[this.animSwordAttack[2]].AddMixingTransform(this._neck);
		this._anim[this.animWeaponShoot].layer = 5;
		this._anim[this.animWeaponShoot].AddMixingTransform(this._neck);
		if (this.weaponAnim1face.Length < Kube.IS.weaponParams.Length)
		{
			int num = this.weaponAnim1face.Length;
			Array.Resize<string>(ref this.weaponAnim1face, Kube.IS.weaponParams.Length);
			for (int i = num; i < Kube.IS.weaponParams.Length; i++)
			{
				if (Kube.IS.weaponParams[i].Type == 0)
				{
					this.weaponAnim1face[i] = "1faceAxeHit";
				}
				else
				{
					this.weaponAnim1face[i] = "charGun1face";
				}
			}
		}
		for (int j = 0; j < this.weaponAnim1face.Length; j++)
		{
			if (this.weaponAnim1face[j].Length != 0)
			{
				this._anim[this.weaponAnim1face[j]].AddMixingTransform(base.transform.Find("CameraObj"));
				this._anim[this.weaponAnim1face[j]].layer = 10;
				this._anim[this.weaponAnim1face[j]].speed = 1f;
			}
		}
		for (int k = 0; k < this.animDecor.Length; k++)
		{
			if (this.animDecor[k].Length != 0)
			{
				this._anim[this.animDecor[k]].layer = 20;
			}
		}
		this.canBuild = false;
		this.controller = base.GetComponent<CharacterController>();
		this.Init();
		int num2 = 0;
		this.points = num2;
		num2 = num2;
		this.kills = num2;
		this.frags = num2;
		if (Kube.BCS.gameType != GameType.creating && Kube.BCS.gameType != GameType.test)
		{
			this.cameraComp.cullingMask -= 16384;
		}
		if (PhotonNetwork.offlineMode || base.photonView.isMine)
		{
			this.jetPackEnabled = Kube.BCS.jetPackAwail;
			this.playerView.clearFlags = CameraClearFlags.Depth;
			this.playerView.depth = 2f;
			this.playerView.cullingMask = 1 << LayerMask.NameToLayer("FPSWeapon");
			this.cameraComp.cullingMask &= ~(1 << LayerMask.NameToLayer("NGUI"));
			this.playerView.enabled = true;
			this.cameraComp.backgroundColor = new Color(0f, 0f, 0f);
			this.cameraComp.depth = 1f;
			this.cameraComp.cullingMask &= ~(1 << LayerMask.NameToLayer("FPSWeapon"));
			this.cameraComp.cullingMask &= ~(1 << LayerMask.NameToLayer("NGUI"));
			this.cameraComp.cullingMask &= ~(1 << LayerMask.NameToLayer("MenuRoom"));
			Skybox component = this.cameraComp.GetComponent<Skybox>();
			if (Kube.WHS.skybox == 1)
			{
				component.material = (Material)Kube.Load("Assets11/ClassicSpaceMaterial", typeof(Material));
			}
			else
			{
				component.material = Kube.WHS.blendedSkybox;
			}
		}
		for (int l = 0; l < 128; l++)
		{
			this.lastShotTimeNew[l] = 0f;
		}
		this.bullets = new PlayerScript.Bullets();
		this.clips = new PlayerScript.Clips();
		this.onlineId = base.photonView.viewID;
		if (PhotonNetwork.offlineMode || base.photonView.isMine)
		{
			base.gameObject.layer = 9;
			this.type = 0;
			this.LocalStart();
			this.serverId = Kube.SS.serverId;
			this.sn = Kube.SN.sn.ToString();
			if (Kube.GPS.clan != null)
			{
				this.playerName = string.Format("{0}[{1}]", Kube.GPS.playerName, Kube.GPS.clan.shortName.ToUpper());
			}
			else
			{
				this.playerName = Kube.GPS.playerName;
			}
			base.transform.Find("TextName").gameObject.SetActive(false);
			this.PlayerDressSkin();
			this.armor = this.maxArmor;
			this.level = Kube.GPS.playerLevel;
			if (Kube.ASS2)
			{
				this.rankPlane.GetComponent<Renderer>().material = Kube.ASS2.RankTex[Mathf.Min(this.level, Kube.ASS2.RankTex.Length - 1)];
			}
			if (Kube.BCS.gameType == GameType.creating)
			{
				if (Kube.BCS.ownerId == this.serverId)
				{
					this.canBuild = true;
				}
				if (Kube.BCS.creatorId == this.serverId)
				{
					this.canBuild = true;
				}
				if (PhotonNetwork.isMasterClient)
				{
					this.canBuild = true;
					Kube.BCS.creatorId = this.serverId;
				}
				if (!Kube.BCS.canChangeWorld)
				{
					this.canBuild = false;
				}
			}
			this.SetView(false);
			this.RecountBonuces();
			this.health = this.maxHealth;
			this.currentWeapon = null;
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
			this.serverId = (int)base.photonView.owner.customProperties["id"];
			if (base.photonView.owner.customProperties.ContainsKey("sn"))
			{
				this.sn = (string)base.photonView.owner.customProperties["sn"];
			}
			this.canBuild = false;
			this.SetView(true);
			this.NO.SynhronizePlayers();
		}
	}

	public void DoUseSpec(int num)
	{
		if (num == 9 && Kube.GPS.inventarSpecItems[9] >= 0)
		{
			this._light.enabled = !this._light.enabled;
		}
	}

	public void DoUseMagic(int fastInvNum)
	{
		ItemPropsScript component = Kube.IS.gameItemsGO[fastInvNum].GetComponent<ItemPropsScript>();
		if (component.magic)
		{
			Ray ray = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			Vector3 shotPoint = this.calcShotPoint(ray.origin, ray.direction, 1000f);
			GameObject gameObject = Kube.IS.gameItemsGO[fastInvNum];
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
						this.NO.CreateMagic(fastInvNum, base.transform.position + Vector3.up * 1.5f + base.transform.TransformDirection(Vector3.forward * 0.7f), shotPoint, this.onlineId);
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
		Kube.IS.chosenFastInventar = -1;
		this.currentWeapon = null;
		this.inventarItems = new Dictionary<int, PlayerScript.InventarItems>();
		if (Kube.BCS.gameType == GameType.hunger)
		{
			this._inventory = new FastInventar[10];
			this._inventory[0] = new FastInventar(InventarType.weapons, 0);
		}
		else
		{
			this._inventory = (FastInventar[])Kube.GPS.fastInventarWeapon.Clone();
		}
		if (Kube.BCS.gameType != GameType.creating)
		{
			for (int i = 0; i < this._inventory.Length; i++)
			{
				if (this._inventory[i].Type == 4 && this._inventory[i].Num >= 0)
				{
					this.SelectWeapon(i);
					break;
				}
			}
		}
		else
		{
			Kube.IS.ChoseFastInventarKey(0);
		}
		this._weaponPickup = new Dictionary<int, int>();
		this.inventarItems[98] = new PlayerScript.InventarItems
		{
			nextUse = 0f,
			cnt = 2
		};
		this.inventarItems[99] = new PlayerScript.InventarItems
		{
			nextUse = 0f,
			cnt = 2
		};
		this.inventarItems[104] = new PlayerScript.InventarItems
		{
			nextUse = 0f,
			cnt = 4
		};
		this.inventarItems[106] = new PlayerScript.InventarItems
		{
			nextUse = 0f,
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
			for (int j = 0; j < Kube.IS.bulletParams.Length; j++)
			{
				this.bullets[j] = Kube.IS.bulletParams[j].initialAmount;
			}
			for (int k = 0; k < Kube.IS.weaponParams.Length; k++)
			{
				this.clips[k] = Kube.IS.weaponParams[k].clipSize[Kube.IS.weaponParams[k].currentClipSizeIndex];
			}
		}
		Kube.BCS.PlayerSpawned(this);
	}

	public void SetTeam(int _team)
	{
		this.team = _team;
		if (this.team < 0 || this.team > 4)
		{
			return;
		}
		GameObject gameObject = base.transform.Find("TextName/TextName").gameObject;
		gameObject.GetComponent<Renderer>().material.SetColor("_Color", Kube.OH.teamColor[this.team]);
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
		float num2 = (float)((int)Kube.GPS.charParamsPrice[3, Mathf.Min(Kube.GPS.playerJump + num, 7), 4]) + this.jumpSpeedBonus;
		this.reduceDamage = Kube.GPS.charParamsPrice[4, Mathf.Min(Kube.GPS.playerDefend + num, 7), 4] * 0.01f + this.reduceDamageBonus;
		float num3 = (float)((int)Kube.GPS.charParamsPrice[3, 0, 4]);
		this.jumpSpeed = num3 + (num2 - num3) * 0.5f;
		this._SafeFallVelocity = 10f + this.jumpSpeed;
		this.RecountConstantsCash();
	}

	public void SynhronizePlayer(PhotonPlayer photonPlayer)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SynhronizePlayer", photonPlayer, new object[]
			{
				this.playerName,
				0,
				this.canBuild,
				this.playerSkin,
				this.playerClothes,
				this.frags,
				this.kills,
				this.deadTimes,
				this.level,
				this.team
			});
		}
	}

	private void SynhronizePlayer()
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SynhronizePlayer", PhotonTargets.All, new object[]
			{
				this.playerName,
				0,
				this.canBuild,
				this.playerSkin,
				this.playerClothes,
				this.frags,
				this.kills,
				this.deadTimes,
				this.level,
				this.team
			});
		}
		else
		{
			this._SynhronizePlayer(this.playerName, 0, this.canBuild, this.playerSkin, this.playerClothes, this.frags, this.kills, this.deadTimes, this.level, this.team, null);
		}
	}

	[PunRPC]
	private void _SynhronizePlayer(string _playerName, int _id, bool _canBuild, int _playerSkin, string _playerClothes, int _frags, int _kills, int _deadTimes, int _level, int _team, PhotonMessageInfo info)
	{
		if (!base.photonView.isMine)
		{
			this.playerName = _playerName;
		}
		if (this.type != 0)
		{
			GameObject gameObject = base.transform.Find("TextName").gameObject;
			gameObject.GetComponentInChildren<TextMesh>().text = AuxFunc.DecodeRussianName(this.playerName);
			gameObject.transform.localPosition = new Vector3(0f, 1.8f, 0f);
		}
		this.level = _level;
		this.rankPlane.GetComponent<Renderer>().material = Kube.ASS2.RankTex[Mathf.Min(this.level, Kube.ASS2.RankTex.Length - 1)];
		this.canBuild = _canBuild;
		this.kills = _kills;
		this.frags = _frags;
		this.deadTimes = _deadTimes;
		this.SetTeam(_team);
		this.playerSkin = _playerSkin;
		this.playerClothes = _playerClothes;
		base.gameObject.SendMessage("DressSkin", string.Concat(new object[]
		{
			string.Empty,
			this.playerSkin,
			";",
			this.playerClothes
		}));
		this.ToggleRenderers(!this.dead);
		UnityEngine.Debug.Log("_SynhronizePlayer: " + _playerName);
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
			if (this.currentWeapon != null)
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
			this.rankPlane.GetComponent<Renderer>().material = Kube.ASS2.RankTex[Mathf.Min(this.level, Kube.ASS2.RankTex.Length - 1)];
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
		float num = 0f;
		float num2 = 0f;
		if (!this.paused)
		{
			num2 = KubeInput.GetAxis("Horizontal");
			if (num2 < -0.2f)
			{
				this.controlLeft = true;
				this.controlRight = false;
			}
			else if (num2 > 0.2f)
			{
				this.controlLeft = false;
				this.controlRight = true;
			}
			else
			{
				this.controlLeft = false;
				this.controlRight = false;
			}
			num = KubeInput.GetAxis("Vertical");
			if (num < -0.2f)
			{
				this.controlBackward = true;
				this.controlForward = false;
			}
			else if (num > 0.2f)
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
		this.rotationX = base.transform.localEulerAngles.y + KubeInput.GetAxis("Mouse X") * this.sensitivityX;
		this.rotationY += KubeInput.GetAxis("Mouse Y") * this.sensitivityY;
		this.newRotationY = (this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY));
		this.cameraComp.transform.parent.localEulerAngles = new Vector3(-this.rotationY, 0f, 0f);
		base.transform.localEulerAngles = new Vector3(0f, this.rotationX, 0f);
		this.velocity = Vector3.zero;
		if (Kube.BCS.gameType != GameType.mission)
		{
			if (this.controlForward)
			{
				this.velocity += this.cameraComp.transform.TransformDirection(Vector3.forward) * num;
			}
			if (this.controlBackward)
			{
				this.velocity -= this.cameraComp.transform.TransformDirection(Vector3.forward) * num;
			}
			if (this.controlLeft)
			{
				this.velocity -= this.cameraComp.transform.TransformDirection(Vector3.right) * num2;
			}
			if (this.controlRight)
			{
				this.velocity += this.cameraComp.transform.TransformDirection(Vector3.right) * num2;
			}
		}
		CollisionFlags collisionFlags = this.controller.Move(this.velocity * Time.deltaTime * 10f);
		this.grounded = ((collisionFlags & CollisionFlags.Below) != CollisionFlags.None);
		bool flag = KubeInput.GetAxis("Jump") > 0f;
		if (this._canRespawn && flag && this.type == 0 && Kube.BCS.gameType != GameType.survival)
		{
			this.Respawn();
		}
		if (Kube.BCS.gameType == GameType.survival && Time.time > this.survivalRespawnTime)
		{
			this.Respawn();
		}
		if (((Kube.BCS.gameType == GameType.survival && Time.time < this.survivalRespawnTime) || Kube.BCS.gameType == GameType.mission) && KubeInput.GetKeyDown(KeyCode.X) && Kube.GPS.inventarItems[109] > 0)
		{
			if (Kube.BCS.gameType == GameType.survival)
			{
				this.survivalRespawnTime = Time.time + 30f;
			}
			else
			{
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
		if (this.currentWeapon != null && this.currentWeapon.UsingBullets > 0 && this.clips[this.currentWeapon.id] < this.currentWeapon.clipSize[this.currentWeapon.currentClipSizeIndex] && this.bullets[this.currentWeapon.BulletsType] > 0)
		{
			this.rechargingWeapon = true;
			this.rechargingWeaponStart = Time.time;
			this.rechargingWeaponType = this.currentWeapon.id;
			this._anim.CrossFade(this.weaponRechargeBeginAnim, 0.05f);
			this.CreateRechargeSound(this.currentWeapon.id);
			if (Kube.BCS.tutorialGO != null)
			{
				Kube.BCS.tutorialGO.SendMessage("ReloadedGun");
			}
		}
	}

	private void LocalStart()
	{
		if (Kube.GPS.isVIP)
		{
			this.geometryIds = (int[])PlayerScript.geometryIdsCodes.Clone();
			this.MAX_GEOM = this.geometryIds.Length;
		}
		else
		{
			this.MAX_GEOM = 1;
			this.geometryIds = new int[PlayerScript.geometryIdsCodes.Length];
			for (int i = 11; i < 11 + PlayerScript.geometryIdsCodes.Length - 1; i++)
			{
				if (Kube.GPS.inventarSpecItems[i] > 0)
				{
					this.geometryIds[this.MAX_GEOM] = PlayerScript.geometryIdsCodes[1 + i - 11];
					this.MAX_GEOM++;
				}
			}
		}
		Antialiasing component = this.cameraComp.GetComponent<Antialiasing>();
		if (component)
		{
			component.enabled = (QualitySettings.GetQualityLevel() == QualitySettings.names.Length - 1);
		}
		bool flag = Kube.BCS.gameType == GameType.creating;
		Kube.BCS.hud.modes.gameObject.SetActive(this.MAX_GEOM > 1 && flag);
		Kube.BCS.hud.modes.SelectGeom();
	}

	private int GeometryCode(int _geom, RaycastHit rch)
	{
		int num = this.CalcRotation(rch);
		int num2 = this.geometryIds[_geom];
		if (num2 > 3)
		{
			num2 += num;
		}
		return num2;
	}

	private int CalcRotation(RaycastHit rch)
	{
		int result = 0;
		if ((double)Mathf.Round(rch.normal.z) == 1.0)
		{
			result = 0;
		}
		else if ((double)Mathf.Round(rch.normal.z) == -1.0)
		{
			result = 3;
		}
		else if ((double)Mathf.Round(rch.normal.x) == 1.0)
		{
			result = 1;
		}
		else if ((double)Mathf.Round(rch.normal.x) == -1.0)
		{
			result = 2;
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
				result = 0;
			}
			else if (Mathf.Round(normalized.z) > 0f)
			{
				result = 3;
			}
			else if (Mathf.Round(normalized.x) < 0f)
			{
				result = 1;
			}
			else if (Mathf.Round(normalized.x) > 0f)
			{
				result = 2;
			}
		}
		return result;
	}

	private void CreatingUpdate()
	{
		if (this.MAX_GEOM < 2)
		{
			return;
		}
		int num = this._geom;
		if (KubeInput.GetKeyDown(KeyCode.Z))
		{
			num--;
		}
		else if (KubeInput.GetKeyDown(KeyCode.X))
		{
			num++;
		}
		if (num < 0)
		{
			num = this.MAX_GEOM - 1;
		}
		else if (num >= this.MAX_GEOM)
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

	public void ChoseFastInventarWheel(int num)
	{
		if (Time.time < this._nextWheeel)
		{
			return;
		}
		this._nextWheeel = Time.time + 0.5f;
		int num2 = num - Kube.IS.chosenFastInventar;
		if (Kube.BCS.gameType == GameType.creating)
		{
			return;
		}
		for (;;)
		{
			if (num < 0)
			{
				num = 10 + num;
			}
			if (num >= 10)
			{
				num = 0;
			}
			if (this.fastInventar[num].Type == 4)
			{
				break;
			}
			if (num == Kube.IS.chosenFastInventar)
			{
				return;
			}
			num += num2;
		}
		if (num != Kube.IS.chosenFastInventar)
		{
			Kube.IS.ChoseFastInventarKey(num);
		}
	}

	private void LocalUpdate()
	{
		Kube.BCS.hud.isVisible = (!Kube.OH.emptyScreen && !Kube.OH.isMenu);
		Kube.BCS.hud.jetpack.gameObject.SetActive(this.jetPackOn);
		if (this.jetPackOn && Kube.BCS.gameMode != GameMode.creating)
		{
			Kube.BCS.hud.jetpack.valuePercent = this.jetPackFuel;
		}
		else
		{
			Kube.BCS.hud.jetpack.value = "---";
		}
		if (KubeInput.GetKeyDown(KeyCode.J) && Kube.BCS.jetPackAwail)
		{
			this.jetPackEnabled = !this.jetPackEnabled;
		}
		if (this.hud.Count == 0 && KubeInput.GetKeyDown(KeyCode.Return))
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
			float num = 0f;
			float num2 = 0f;
			if (!this.paused && !this.isDriveTransport)
			{
				num2 = KubeInput.GetAxis("Horizontal");
				if (num2 < -0.2f)
				{
					this.controlLeft = true;
					this.controlRight = false;
				}
				else if (num2 > 0.2f)
				{
					this.controlLeft = false;
					this.controlRight = true;
				}
				else
				{
					this.controlLeft = false;
					this.controlRight = false;
				}
				num = KubeInput.GetAxis("Vertical");
				if (num < -0.2f)
				{
					this.controlBackward = true;
					this.controlForward = false;
				}
				else if (num > 0.2f)
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
			Ray ray = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			if (!this.paused && flag)
			{
				if (!this.isDriveTransport)
				{
					this.rotationX = base.transform.localEulerAngles.y + KubeInput.GetAxis("Mouse X") * this.sensitivityX;
				}
				else
				{
					this.rotationX += KubeInput.GetAxis("Mouse X") * this.sensitivityX;
				}
				this.rotationY += KubeInput.GetAxis("Mouse Y") * this.sensitivityY;
				this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
			}
			if (!this.paused && flag)
			{
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
			if (Kube.BCS.jetPackAwail && !this.jetPackOn)
			{
				if (Kube.GPS.inventarSpecItems[0] >= 0 && Kube.BCS.gameType != GameType.captureTheFlag)
				{
					this.jetPackOn = true;
					this.DressJetPack(true);
				}
				if (Kube.GPS.inventarSpecItems[10] >= 0 && Kube.BCS.gameType == GameType.creating)
				{
					this.jetPackOn = true;
					this.DressJetPack(true);
				}
			}
			if (this.controlForward)
			{
				this.forwardRun = Mathf.Lerp(this.forwardRun, this.runSpeed * num, Time.time * 20f);
			}
			else if (this.controlBackward)
			{
				this.forwardRun = Mathf.Lerp(this.forwardRun, this.runSpeed * num, Time.time * 20f);
			}
			else
			{
				this.forwardRun = Mathf.Lerp(this.forwardRun, 0f, Time.time * 20f);
			}
			if (this.controlLeft)
			{
				this.sideRun = Mathf.Lerp(this.sideRun, this.runSpeed * num2, Time.time * 20f);
			}
			else if (this.controlRight)
			{
				this.sideRun = Mathf.Lerp(this.sideRun, this.runSpeed * num2, Time.time * 20f);
			}
			else
			{
				this.sideRun = Mathf.Lerp(this.sideRun, 0f, Time.time * 20f);
			}
			this.typePhys = Kube.WHS.GetCubePhysType(base.transform.position + Vector3.up * 0.5f);
			if (this.typePhys == CubePhys.lava && this.nextEnvDamage < Time.time)
			{
				this.ApplyDamage(new DamageMessage
				{
					damage = (short)Mathf.CeilToInt(5f),
					id_killer = 0,
					weaponType = 0,
					team = 10
				});
				this.nextEnvDamage = Time.time + 0.5f;
			}
			if (this.typePhys == CubePhys.air || this.typePhys == CubePhys.solid)
			{
				Vector3 vector = this.pushVelocity;
				float magnitude = vector.magnitude;
				if (magnitude > 0f)
				{
					float num3;
					if (this.grounded)
					{
						num3 = this.GROUND_FRICTION * Time.deltaTime;
					}
					else
					{
						num3 = 5f * Time.deltaTime;
					}
					float num4 = magnitude - num3;
					if (num4 < 0f)
					{
						num4 = 0f;
					}
					num4 /= magnitude;
					this.pushVelocity *= num4;
					this.pushVelocity.y = this.pushVelocity.y + Kube.OH.gravity * Time.deltaTime;
					if (this.grounded && this.pushVelocity.y < 0f)
					{
						this.pushVelocity = Vector3.zero;
					}
				}
				else
				{
					this.pushVelocity = Vector3.zero;
				}
				bool flag2 = !this.jetPackEnabled && Kube.BCS.gameType == GameType.creating && Kube.GPS.inventarSpecItems[10] >= 0;
				if (!this.grounded && !this.jetPackWork)
				{
					if (!flag2)
					{
						this.velocity.y = this.velocity.y + Kube.OH.gravity * Time.deltaTime;
					}
				}
				else if (this.grounded)
				{
					if (this.velocity.y < -this._SafeFallVelocity && Kube.BCS.gameType != GameType.creating)
					{
						this.ApplyDamage(new DamageMessage
						{
							damage = (short)(this.fallDamage * (Mathf.Abs(this.velocity.y) - this._SafeFallVelocity)),
							id_killer = 0,
							weaponType = 0,
							team = 10
						});
					}
					if (this.velocity.y < 0f)
					{
						this.velocity.y = 0f;
					}
				}
				if (!this.isDriveTransport && Kube.GPS.inventarSpecItems[9] >= 0 && KubeInput.GetKeyDown(KeyCode.L))
				{
					this._light.enabled = !this._light.enabled;
				}
				if (!this.isDriveTransport && this.jetPackOn && KubeInput.GetButton("Jump") && !this.grounded && Kube.BCS.gameType != GameType.creating)
				{
					this.jetPackFuel = Mathf.Max(0f, this.jetPackFuel - Time.deltaTime * 0.8f);
				}
				this.jetPackWork = false;
				if (this.grounded && this.nextJump < Time.time && KubeInput.GetButton("Jump") && !this.paused)
				{
					this.velocity.y = this.velocity.y + this.jumpSpeed * 1.2f;
					this.nextJump = Time.time + 0.5f;
				}
				else if (!this.isDriveTransport && this.jetPackOn && this.jetPackFuel >= 0.05f && KubeInput.GetButton("Jump") && !this.paused)
				{
					this.jetPackGO.SendMessage("PlayStop", true, SendMessageOptions.DontRequireReceiver);
					this.jetPackWork = true;
					this.velocity.y = Mathf.Min(this.velocity.y + Time.deltaTime * 8f, 8f);
				}
				else if (!this.isDriveTransport && flag2 && KubeInput.GetButton("Down") && !this.paused)
				{
					this.jetPackWork = true;
					this.velocity.y = -Mathf.Min(Mathf.Abs(this.velocity.y) + Time.deltaTime * 8f, 8f);
				}
				if (Kube.BCS.gameType == GameType.creating && flag2 && !this.jetPackWork)
				{
					this.velocity.y = 0f;
				}
				if (this.velocity.y > 0f)
				{
				}
				if (!KubeInput.GetButton("Jump"))
				{
					this.jetPackGO.SendMessage("PlayStop", false, SendMessageOptions.DontRequireReceiver);
					this.jetPackWork = false;
				}
				if (this.pushVelocity.magnitude == 0f || (this.jetPackWork && Kube.BCS.gameType == GameType.creating))
				{
					this.velocity = base.transform.TransformDirection(new Vector3(this.sideRun, this.velocity.y, this.forwardRun));
				}
				else
				{
					float num5 = this.runSpeed;
					Vector3 vector2 = base.transform.TransformDirection(new Vector3(this.sideRun, 0f, this.forwardRun));
					vector2.Normalize();
					float num6 = Vector3.Dot(this.velocity, vector2);
					float num7 = num5 - num6;
					if (num7 > 0f)
					{
						float num8;
						if (this.grounded)
						{
							num8 = num5 * this.GROUND_ACCELERATE * Time.deltaTime;
						}
						else if (this.jetPackWork)
						{
							num8 = num5 * 4f * Time.deltaTime;
						}
						else
						{
							num8 = num5 * 2.5f * Time.deltaTime;
						}
						if (num8 > num7)
						{
							num8 = num7;
						}
						this.pushVelocity += vector2 * num8;
					}
					this.velocity.x = this.pushVelocity.x;
					this.velocity.z = this.pushVelocity.z;
					this.velocity.y = this.pushVelocity.y;
				}
			}
			else if (this.typePhys == CubePhys.water || this.typePhys == CubePhys.lava)
			{
				this.pushVelocity = Vector3.zero;
				if (!this.grounded)
				{
					this.velocity.y = Kube.OH.gravity * Time.deltaTime * 20f;
				}
				else
				{
					this.velocity.y = 0f;
				}
				if (KubeInput.GetButton("Jump") && !this.paused)
				{
					this.velocity.y = this.jumpSpeed * 0.6f;
				}
				else if (KubeInput.GetButton("Down") && !this.paused)
				{
					this.velocity.y = -this.jumpSpeed * 0.6f;
				}
				this.velocity = base.transform.TransformDirection(new Vector3(this.sideRun * 0.5f, this.velocity.y, this.forwardRun * 0.5f));
			}
			else if (this.typePhys == CubePhys.ledder)
			{
				this.pushVelocity = Vector3.zero;
				if (KubeInput.GetButton("Jump") && !this.paused)
				{
					this.velocity.y = this.jumpSpeed * 1f;
				}
				else if (KubeInput.GetButton("Down") && !this.paused)
				{
					this.velocity.y = -this.jumpSpeed * 1f;
				}
				else
				{
					this.velocity.y = 0f;
				}
				this.velocity = base.transform.TransformDirection(new Vector3(this.sideRun * 0.5f, this.velocity.y, this.forwardRun * 0.5f));
			}
			else if (this.typePhys == CubePhys.liftOn)
			{
				this.pushVelocity = Vector3.zero;
				this.velocity.y = 5f;
				if (KubeInput.GetButton("Jump") && !this.paused)
				{
					this.velocity.y = this.velocity.y + this.jumpSpeed * 1f;
				}
				this.velocity = base.transform.TransformDirection(new Vector3(this.sideRun * 0.5f, this.velocity.y, this.forwardRun * 0.5f));
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
				float num9 = 0f;
				this.velocity.z = num9;
				this.velocity.x = num9;
			}
			if (!this.isDriveTransport)
			{
				CollisionFlags collisionFlags = this.controller.Move(this.velocity * Time.deltaTime);
				this.grounded = ((collisionFlags & CollisionFlags.Below) != CollisionFlags.None);
				if (this.velocity.y > 0f && (collisionFlags & CollisionFlags.Above) != CollisionFlags.None)
				{
					this.velocity.y = 0f;
					this.pushVelocity.y = 0f;
				}
			}
			else
			{
				this.transportToDriveScript.AppplyPosition(base.transform, this.transportToDrivePlace);
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
				Ray ray2 = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
				int layerMask = 273;
				int num10 = -1;
				if (Kube.IS.chosenFastInventar != -1)
				{
					num10 = Kube.GPS.fastInventarCreating[Kube.IS.chosenFastInventar].Type;
				}
				if (num10 != 0)
				{
					layerMask = 8465;
				}
				RaycastHit raycastHit;
				if (Physics.Raycast(ray2, out raycastHit, 10f, layerMask))
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
			this.guiItemText = 0;
			if (!this.isDriveTransport && this.moveItem && (KubeInput.GetButtonDown("Fire1") || KubeInput.GetButtonDown("Fire2")))
			{
				this._nextCreatingRepeat = Time.time + 5f;
				Ray ray3 = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
				RaycastHit raycastHit2;
				if (Physics.Raycast(ray3, out raycastHit2, 10f, 8465))
				{
					ItemPropsScript component = this.gameObjectToMove.GetComponent<ItemPropsScript>();
					if (component.placeType == ItemPlaceType.onTheItem && raycastHit2.collider.gameObject.layer != 13)
					{
						Kube.GPS.printMessage(Localize.put_on_items, Color.white);
					}
					else
					{
						if (component.placeType == ItemPlaceType.onTheItem && raycastHit2.collider.gameObject.layer == 13)
						{
							ItemPropsScript component2 = raycastHit2.collider.gameObject.GetComponent<ItemPropsScript>();
							this.gameObjectToMove.BroadcastMessage("MoveItem", component2.GetComponent<Collider>().transform.position);
							this.onlyMove = false;
							this.moveItem = false;
							return;
						}
						Vector3 vector3 = new Vector3(Mathf.Round(raycastHit2.point.x + raycastHit2.normal.x * 0.02f), Mathf.Round(raycastHit2.point.y + raycastHit2.normal.y * 0.02f), Mathf.Round(raycastHit2.point.z + raycastHit2.normal.z * 0.02f));
						ushort cubeFill2 = Kube.WHS.GetCubeFill((int)vector3.x, (int)vector3.y, (int)vector3.z);
						if (cubeFill2 != 0 && cubeFill2 != 128)
						{
							Kube.GPS.printMessage(Localize.cube_occupied, Color.white);
							MonoBehaviour.print("Filled with: " + Kube.WHS.cubeTypes[(int)vector3.x, (int)vector3.y, (int)vector3.z]);
						}
						else if (!component.magic)
						{
							this.gameObjectToMove.BroadcastMessage("MoveItem", vector3);
							this.onlyMove = false;
							this.moveItem = false;
							return;
						}
					}
				}
			}
			bool flag3 = KubeInput.GetKeyDown(KeyCode.F);
			if (KubeInput.GetKeyDown(KeyCode.H))
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
				this.sensitivityX = (this.sensitivityY = Kube.GPS.mouseSens);
				return;
			}
			if (this.moveItem)
			{
				return;
			}
			if (KubeInput.GetButtonDown("Fire1") || KubeInput.GetButtonDown("Fire2"))
			{
				this._nextCreatingRepeat = 0f;
			}
			bool flag4 = true;
			if (Kube.BCS.gameType == GameType.teams && this.availableCubes <= 0)
			{
				flag4 = false;
			}
			int layerMask2 = 46337;
			if (Kube.BCS.gameType == GameType.creating)
			{
				layerMask2 = 57344;
			}
			RaycastHit raycastHit3;
			bool flag5 = Physics.Raycast(ray, out raycastHit3, 10f, layerMask2);
			if (!this.isDriveTransport && flag5)
			{
				if (raycastHit3.collider.gameObject.transform.root.gameObject.layer == 13)
				{
					ItemPropsScript component3 = raycastHit3.collider.gameObject.transform.root.gameObject.GetComponent<ItemPropsScript>();
					bool flag6 = raycastHit3.collider.gameObject.layer != 14 || Kube.BCS.gameType == GameType.creating;
					if (this.canBuild && Kube.BCS.canChangeWorld && component3.canTake && flag6 && Kube.BCS.gameType == GameType.creating)
					{
						this.guiItemText++;
						if (KubeInput.GetKeyDown(KeyCode.Delete))
						{
							this._nextCreatingRepeat = Time.time + 0.6f;
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
							KubeScreen.lockCursor = true;
						}
					}
					if (this.canBuild && Kube.BCS.canChangeWorld && component3.canTake && flag6 && Kube.BCS.gameType == GameType.creating)
					{
						this.guiItemText += 2;
						if (KubeInput.GetKeyDown(KeyCode.E))
						{
							this.onlyMove = true;
							this.moveItem = true;
							this.gameObjectToMove = raycastHit3.collider.gameObject.transform.root.gameObject;
						}
					}
					if (component3 && component3.canActivate && raycastHit3.distance < 10f)
					{
						this.guiItemText += 4;
						if (flag3)
						{
							component3.gameObject.BroadcastMessage("Activate", base.gameObject.GetComponent<PlayerScript>(), SendMessageOptions.RequireReceiver);
						}
					}
					else
					{
						BombScript componentInChildren = raycastHit3.collider.gameObject.transform.root.gameObject.GetComponentInChildren<BombScript>();
						if (componentInChildren != null && componentInChildren.CanActivate(this))
						{
							this.guiItemText += 4;
							if (flag3)
							{
								componentInChildren.ActivateByPlayer(this);
							}
						}
					}
					if (this.canBuild && Kube.BCS.canChangeWorld && component3.canRotate && Kube.BCS.gameType == GameType.creating)
					{
						this.guiItemText += 8;
						if (KubeInput.GetKeyDown(KeyCode.R))
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
						this.guiItemText += 16;
						if (KubeInput.GetKeyDown(KeyCode.T))
						{
							component3.gameObject.BroadcastMessage("SetupItem", base.gameObject.GetComponent<PlayerScript>(), SendMessageOptions.RequireReceiver);
						}
					}
				}
				else if (raycastHit3.collider.gameObject.transform.root.gameObject.layer == 15)
				{
					this.guiItemText += 4;
					if (flag3)
					{
						raycastHit3.collider.gameObject.transform.root.gameObject.SendMessage("TryToDrive", this.onlineId, SendMessageOptions.DontRequireReceiver);
						flag3 = false;
					}
				}
			}
			if (this.isDriveTransport)
			{
				this.guiItemText += 4;
				if (flag3)
				{
					this.transportToDriveScript.ExitDrive(this.onlineId);
				}
			}
			int num11 = -1;
			int num12 = 0;
			if (Kube.IS.chosenFastInventar != -1 && Kube.BCS.gameType == GameType.creating)
			{
				num11 = Kube.GPS.fastInventarCreating[Kube.IS.chosenFastInventar].Type;
				num12 = Kube.GPS.fastInventarCreating[Kube.IS.chosenFastInventar].Num;
			}
			bool flag7 = false;
			if (KubeInput.GetButtonDown("Fire2") && !this.paused)
			{
				if (num11 == 1)
				{
					ItemPropsScript component6 = Kube.IS.gameItemsGO[num12].GetComponent<ItemPropsScript>();
					if (component6.magic)
					{
						Ray ray4 = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
						RaycastHit raycastHit4;
						if (Physics.Raycast(ray4, out raycastHit4, 1000f, 64784))
						{
							this.NO.CreateMagic(num12, base.transform.position + Vector3.up * 1.5f + base.transform.TransformDirection(Vector3.forward * 0.7f), raycastHit4.point, this.onlineId);
							if (Kube.IS.UseItem(num12) == 1)
							{
							}
							flag7 = true;
						}
					}
				}
				else if (num11 == 3)
				{
					if (Kube.BCS.gameType == GameType.creating)
					{
						this.DoUseMagic(num12);
						flag7 = true;
					}
				}
				else if (num11 == 4 && num12 == 0)
				{
					num11 = 0;
					num12 = 5;
					flag7 = false;
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
			this.sensitivityX = (this.sensitivityY = Kube.GPS.mouseSens);
			if (KubeInput.GetAxis("Fire2") > 0f && !this.paused)
			{
				if (this.currentWeapon != null && !this.rechargingWeapon)
				{
					if (this.currentWeapon.id == 11 || this.currentWeapon.id == 23 || this.currentWeapon.id == 31 || this.currentWeapon.sniper)
					{
						this.rifleAim = true;
						this.cameraComp.fieldOfView = 15f;
						this.sensitivityX = (this.sensitivityY = Kube.GPS.mouseSens * 0.35f);
					}
					else if (this.currentWeapon.UsingBullets != 0)
					{
						this.playerView.fieldOfView = 30f;
						this.cameraComp.fieldOfView = 30f;
						this.sensitivityX = (this.sensitivityY = Kube.GPS.mouseSens * 0.5f);
					}
				}
			}
			else
			{
				this.rifleAim = false;
			}
			if (!this.isDriveTransport && KubeInput.GetAxis("Fire2") > 0f && !this.paused && !flag7 && Kube.BCS.gameType == GameType.creating && this._nextCreatingRepeat < Time.time)
			{
				this._nextCreatingRepeat = Time.time + 0.6f;
				bool flag8 = this.canBuildBlock || this.canBuild;
				if ((num11 == 0 || num11 == 1 || num11 == 3) && !flag8)
				{
					Kube.GPS.printMessage(Localize.cant_build_ask_admin, Color.yellow);
				}
				else if ((num11 == 0 || num11 == 1 || num11 == 3) && !Kube.BCS.canChangeWorld)
				{
					Kube.GPS.printMessage(Localize.cant_change_world, Color.yellow);
				}
				else if (!flag4)
				{
					Kube.GPS.printMessage(Localize.not_enougth_cubes, Color.yellow);
				}
				else if (num11 == 0)
				{
					Ray ray5 = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
					RaycastHit rch;
					if (Physics.Raycast(ray5, out rch, 10f, 8465))
					{
						Vector3 vector4 = new Vector3(Mathf.Round(rch.point.x + rch.normal.x * 0.02f), Mathf.Round(rch.point.y + rch.normal.y * 0.02f), Mathf.Round(rch.point.z + rch.normal.z * 0.02f));
						ushort cubeFill3 = Kube.WHS.GetCubeFill((int)vector4.x, (int)vector4.y, (int)vector4.z);
						byte b = (byte)this.GeometryCode(this._geom, rch);
						if (cubeFill3 != 0)
						{
							byte cubeData2 = Kube.WHS.GetCubeData((int)vector4.x, (int)vector4.y, (int)vector4.z);
							if (b == cubeData2 && (cubeData2 == 1 || cubeData2 == 2))
							{
								this.PlaceNewCube(vector4, num12, 0);
								this._nextCreatingRepeat = Time.time + 0.6f;
							}
							else
							{
								Kube.GPS.printMessage(Localize.cube_occupied, Color.white);
								MonoBehaviour.print("Filled with: " + Kube.WHS.cubeTypes[(int)vector4.x, (int)vector4.y, (int)vector4.z]);
								MonoBehaviour.print(string.Concat(new object[]
								{
									(int)vector4.x,
									" ",
									(int)vector4.y,
									" ",
									(int)vector4.z
								}));
							}
						}
						else if (Vector3.Distance(vector4, base.transform.position + Vector3.up) > 1.5f)
						{
							if (rch.collider.gameObject.layer != 13)
							{
								this.PlaceNewCube(vector4, num12, (int)b);
								this._nextCreatingRepeat = Time.time + 0.6f;
							}
							else
							{
								Kube.GPS.printMessage(Localize.cube_occupied, Color.red);
							}
						}
					}
				}
				else if (num11 == 1 || num11 == 3)
				{
					Ray ray6 = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
					RaycastHit rch2;
					if (Physics.Raycast(ray6, out rch2, 10f, 8465))
					{
						ItemPropsScript component7 = Kube.IS.gameItemsGO[num12].GetComponent<ItemPropsScript>();
						GameMapItem component8 = Kube.IS.gameItemsGO[num12].GetComponent<GameMapItem>();
						if (component7.placeType == ItemPlaceType.onTheItem && rch2.collider.gameObject.layer != 13)
						{
							Kube.GPS.printMessage(Localize.put_on_items, Color.white);
						}
						else if (component7.placeType == ItemPlaceType.onTheItem && rch2.collider.gameObject.layer == 13)
						{
							ItemPropsScript component9 = rch2.collider.gameObject.GetComponent<ItemPropsScript>();
							if (!component8 || component8.CanPlace(component9.gameObject.transform.position))
							{
								this.NO.CreateGameItem(num12, (byte)this.CalcRotation(rch2), Mathf.RoundToInt(component9.gameObject.transform.position.x), Mathf.RoundToInt(component9.gameObject.transform.position.y), Mathf.RoundToInt(component9.gameObject.transform.position.z), this.onlineId);
								BattleControllerScript bcs = Kube.BCS;
								bcs.bonusCounters.placedItem = bcs.bonusCounters.placedItem + 1;
								if (Kube.IS.UseItem(num12) == 1)
								{
								}
								if (Kube.BCS.gameType == GameType.teams)
								{
									this.availableCubes--;
								}
							}
						}
						else
						{
							Vector3 vector5 = new Vector3(Mathf.Round(rch2.point.x + rch2.normal.x * 0.02f), Mathf.Round(rch2.point.y + rch2.normal.y * 0.02f), Mathf.Round(rch2.point.z + rch2.normal.z * 0.02f));
							ushort cubeFill4 = Kube.WHS.GetCubeFill((int)vector5.x, (int)vector5.y, (int)vector5.z);
							bool flag9 = true;
							if (cubeFill4 != 0 && cubeFill4 != 128)
							{
								Kube.GPS.printMessage(Localize.cube_occupied, Color.white);
								flag9 = false;
							}
							if ((int)vector5.x < 0 || (int)vector5.x >= Kube.WHS.sizeX || (int)vector5.y < 0 || (int)vector5.y >= Kube.WHS.sizeY || (int)vector5.z < 0 || (int)vector5.z >= Kube.WHS.sizeZ)
							{
								Kube.GPS.printMessage(Localize.beside_world, Color.white);
								flag9 = false;
							}
							if (flag9)
							{
								if (component7.buildMagic && !component7.magic)
								{
									int num13 = -1;
									if ((double)Mathf.Round(rch2.normal.z) == 1.0)
									{
										num13 = 0;
									}
									else if ((double)Mathf.Round(rch2.normal.z) == -1.0)
									{
										num13 = 3;
									}
									else if ((double)Mathf.Round(rch2.normal.x) == 1.0)
									{
										num13 = 1;
									}
									else if ((double)Mathf.Round(rch2.normal.x) == -1.0)
									{
										num13 = 2;
									}
									else if ((double)Mathf.Round(rch2.normal.y) == -1.0)
									{
										num13 = 5;
									}
									else if ((double)Mathf.Round(rch2.normal.y) == 1.0)
									{
										num13 = 4;
									}
									this.NO.CreateGameItem(num12, (byte)num13, Mathf.RoundToInt(vector5.x), Mathf.RoundToInt(vector5.y), Mathf.RoundToInt(vector5.z), this.onlineId);
									BattleControllerScript bcs2 = Kube.BCS;
									bcs2.bonusCounters.placedItem = bcs2.bonusCounters.placedItem + 1;
									if (Kube.IS.UseItem(num12) == 1)
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
										int num14 = -1;
										if ((double)Mathf.Round(rch2.normal.z) == 1.0)
										{
											num14 = 0;
										}
										else if ((double)Mathf.Round(rch2.normal.z) == -1.0)
										{
											num14 = 3;
										}
										else if ((double)Mathf.Round(rch2.normal.x) == 1.0)
										{
											num14 = 1;
										}
										else if ((double)Mathf.Round(rch2.normal.x) == -1.0)
										{
											num14 = 2;
										}
										if (num14 != -1)
										{
											this.NO.CreateGameItem(num12, (byte)num14, Mathf.RoundToInt(vector5.x), Mathf.RoundToInt(vector5.y), Mathf.RoundToInt(vector5.z), this.onlineId);
											BattleControllerScript bcs3 = Kube.BCS;
											bcs3.bonusCounters.placedItem = bcs3.bonusCounters.placedItem + 1;
											if (Kube.IS.UseItem(num12) == 1)
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
										int num15 = -1;
										if ((double)Mathf.Round(rch2.normal.y) == -1.0)
										{
											num15 = 0;
										}
										if (num15 != -1)
										{
											this.NO.CreateGameItem(num12, (byte)num15, Mathf.RoundToInt(vector5.x), Mathf.RoundToInt(vector5.y), Mathf.RoundToInt(vector5.z), this.onlineId);
											BattleControllerScript bcs4 = Kube.BCS;
											bcs4.bonusCounters.placedItem = bcs4.bonusCounters.placedItem + 1;
											if (Kube.IS.UseItem(num12) == 1)
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
										this.NO.CreateGameItem(num12, (byte)this.CalcRotation(rch2), Mathf.RoundToInt(vector5.x), Mathf.RoundToInt(vector5.y), Mathf.RoundToInt(vector5.z), this.onlineId);
										BattleControllerScript bcs5 = Kube.BCS;
										bcs5.bonusCounters.placedItem = bcs5.bonusCounters.placedItem + 1;
										if (Kube.IS.UseItem(num12) == 1)
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
			bool flag10 = KubeInput.GetAxis("Fire1") > 0f;
			if (flag10 && !this.paused && !this.moveItem)
			{
				bool flag11 = Kube.BCS.gameType == GameType.creating;
				if (!this.canBuild && flag11)
				{
					Kube.GPS.printMessage(Localize.cant_build_ask_admin, Color.yellow);
				}
				else if (!Kube.BCS.canChangeWorld && flag11)
				{
					Kube.GPS.printMessage(Localize.cant_change_world, Color.yellow);
				}
				else if (flag11 && this._nextCreatingRepeat < Time.time)
				{
					Ray ray7 = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
					RaycastHit raycastHit5;
					if (Physics.Raycast(ray7, out raycastHit5, 10f, 273))
					{
						Vector3 pos = new Vector3(Mathf.Round(raycastHit5.point.x - raycastHit5.normal.x * 0.02f), Mathf.Round(raycastHit5.point.y - raycastHit5.normal.y * 0.02f), Mathf.Round(raycastHit5.point.z - raycastHit5.normal.z * 0.02f));
						if (raycastHit5.collider.gameObject.layer == 8 || raycastHit5.collider.gameObject.layer == 4)
						{
							Kube.WHS.PlayCubeHit(pos, SoundHitType.breaking);
							this.NO.PlaceNewCube(pos, 0, 0);
							if (Kube.BCS.gameType == GameType.teams)
							{
								this.availableCubes--;
							}
							if (Kube.GPS.needTrainingBuild)
							{
								Kube.TS.SendMessage("DestroyedCube");
							}
							this._nextCreatingRepeat = Time.time + 0.6f;
						}
					}
				}
				else if (this.currentWeapon != null && flag && !this.rechargingWeapon && Time.time - this.lastShotTimeNew[this.currentWeapon.id] >= this.currentWeapon.DeltaShot)
				{
					int bulletsType = this.currentWeapon.BulletsType;
					if (this.clips[this.currentWeapon.id] >= this.currentWeapon.UsingBullets)
					{
						int num16 = this.clips[this.currentWeapon.id];
						PlayerScript.Clips clips2;
						PlayerScript.Clips clips = clips2 = this.clips;
						int num17;
						int index = num17 = this.currentWeapon.id;
						num17 = clips2[num17];
						clips[index] = num17 - this.currentWeapon.UsingBullets;
						this.lastShotTimeNew[this.currentWeapon.id] = Time.time;
						Ray ray8 = this.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
						this.CreateShot(ray8.origin, ray8.direction, this.currentWeapon.id);
						if (num16 - this.currentWeapon.UsingBullets != this.clips[this.currentWeapon.id])
						{
							Kube.OH.usedCheat = true;
							this.NO.BanPlayer(Kube.SS.serverId);
						}
					}
					else
					{
						this.CreateEmptyClipEvent(this.currentWeapon.id);
						this.lastShotTimeNew[this.currentWeapon.id] = Time.time;
						this.ReloadGun();
					}
				}
			}
			if (KubeInput.GetKeyDown(KeyCode.R))
			{
				this.ReloadGun();
			}
			if (this.rechargingWeapon && Time.time > this.rechargingWeaponStart + Kube.IS.weaponParams[this.rechargingWeaponType].reloadTime[Kube.IS.weaponParams[this.rechargingWeaponType].currentReloadTimeIndex])
			{
				this.rechargingWeapon = false;
				this._anim.CrossFade(this.weaponRechargeEndAnim, 0.05f);
				int num18 = Kube.IS.weaponParams[this.rechargingWeaponType].clipSize[Kube.IS.weaponParams[this.rechargingWeaponType].currentClipSizeIndex] - this.clips[this.rechargingWeaponType];
				num18 = Mathf.Min(num18, this.bullets[Kube.IS.weaponParams[this.rechargingWeaponType].BulletsType]);
				PlayerScript.Clips clips4;
				PlayerScript.Clips clips3 = clips4 = this.clips;
				int num17;
				int index2 = num17 = this.rechargingWeaponType;
				num17 = clips4[num17];
				clips3[index2] = num17 + num18;
				PlayerScript.Bullets bullets2;
				PlayerScript.Bullets bullets = bullets2 = this.bullets;
				int index3 = num17 = Kube.IS.weaponParams[this.rechargingWeaponType].BulletsType;
				num17 = bullets2[num17];
				bullets[index3] = num17 - num18;
			}
			if (KubeInput.GetKeyDown(KeyCode.F2))
			{
				bool flag12 = true;
				if (this.isDriveTransport && this.view3face && !this.transportToDriveScript.HasFPS(this.transportToDrivePlace))
				{
					flag12 = false;
				}
				if (flag12)
				{
					PlayerScript.userView3face = !this.view3face;
					this.SetView(PlayerScript.userView3face);
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
							if (this.currentWeapon == null)
							{
								this._anim.CrossFade(this.animRunEmpty);
								this._anim[this.animRunEmpty].speed = direction.z / 5f;
							}
							else if (this.currentWeapon.Type == 0)
							{
								this._anim.CrossFade(this.animRunSword);
								this._anim[this.animRunSword].speed = direction.z / 5f;
							}
							else if (this.currentWeapon.Type == 1)
							{
								this._anim.CrossFade(this.animRunWeapon);
								this._anim[this.animRunWeapon].speed = direction.z / 5f;
							}
						}
						else if (direction.x < 0f)
						{
							if (this.currentWeapon == null)
							{
								this._anim.CrossFade(this.animRunLeftEmpty);
								this._anim[this.animRunLeftEmpty].speed = -direction.x / 5f;
							}
							else if (this.currentWeapon.Type == 0)
							{
								this._anim.CrossFade(this.animRunLeftSword);
								this._anim[this.animRunLeftSword].speed = -direction.x / 5f;
							}
							else if (this.currentWeapon.Type == 1)
							{
								this._anim.CrossFade(this.animRunLeftWeapon);
								this._anim[this.animRunLeftWeapon].speed = -direction.x / 5f;
							}
						}
						else if (direction.x > 0f)
						{
							if (this.currentWeapon == null)
							{
								this._anim.CrossFade(this.animRunRightEmpty);
								this._anim[this.animRunRightEmpty].speed = direction.x / 5f;
							}
							else if (this.currentWeapon.Type == 0)
							{
								this._anim.CrossFade(this.animRunRightSword);
								this._anim[this.animRunRightSword].speed = direction.x / 5f;
							}
							else if (this.currentWeapon.Type == 1)
							{
								this._anim.CrossFade(this.animRunRightWeapon);
								this._anim[this.animRunRightWeapon].speed = direction.x / 5f;
							}
						}
					}
					else if (this.currentWeapon == null)
					{
						this._anim.CrossFade(this.animIdleEmpty);
					}
					else if (this.currentWeapon.Type == 0)
					{
						this._anim.CrossFade(this.animIdleSword);
					}
					else if (this.currentWeapon.Type == 1)
					{
						this._anim.CrossFade(this.animIdleWeapon);
					}
				}
				else if (this.currentWeapon == null)
				{
					this._anim.CrossFade(this.animIdleEmpty);
				}
				else if (this.currentWeapon.Type == 0)
				{
					this._anim.CrossFade(this.animIdleSword);
				}
				else if (this.currentWeapon.Type == 1)
				{
					this._anim.CrossFade(this.animIdleWeapon);
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
			this.transportToDriveScript.AppplyPosition(base.transform, this.transportToDrivePlace);
			this.transportToDriveScript.LateAnimateDriver(this.transportToDrivePlace, this);
		}
		else if (this.view3face && !this.dead)
		{
			Vector3 axis = base.transform.TransformDirection(Vector3.right);
			this.newRotationY = Mathf.Lerp(this.newRotationY, this.rotationY, Time.deltaTime * 5f);
			this.headTransform.RotateAround(axis, Mathf.Min(Mathf.Max(-this.newRotationY * 0.0174532924f - 0.3f, -1.5f), 1.5f));
			if (!(this.currentWeapon == null))
			{
				if (this.currentWeapon.Type == 0)
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

	private void SetView(bool view3face)
	{
		this.view3face = view3face;
		bool active = view3face;
		if (this.isDriveTransport && this.transportToDriveScript.driverIsHidden[this.transportToDrivePlace])
		{
			active = false;
		}
		this.skin.SetActive(active);
		this.bones.SetActive(active);
		this.cameraComp.transform.localRotation = Quaternion.identity;
		if (view3face)
		{
			if (base.photonView.isMine)
			{
				this.cameraComp.SendMessage("SetPosition", new Vector3(0.5f, 0f, -2.5f));
			}
			else
			{
				this.cameraComp.transform.parent.gameObject.SetActive(false);
			}
		}
		else if (!view3face)
		{
			if (base.photonView.isMine)
			{
				this.cameraComp.SendMessage("SetPosition", Vector3.zero);
			}
			else
			{
				this.cameraComp.transform.parent.gameObject.SetActive(false);
			}
		}
		if (this.transportToDriveScript)
		{
			this.transportToDriveScript.SetView(this.transportToDrivePlace, view3face);
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

	[PunRPC]
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

	[PunRPC]
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

	public override int getTeam()
	{
		return this.team;
	}

	[PunRPC]
	private void _CreateShot(Vector3 rayOrigin, Vector3 rayDirection, int numWeapon, PhotonMessageInfo info)
	{
		Vector3 shotPoint = this.calcShotPoint(rayOrigin, rayDirection, Kube.IS.weaponParams[numWeapon].Distance);
		if (!this.weaponGOScript)
		{
			return;
		}
		DamageMessage damageMessage = new DamageMessage();
		if (PhotonNetwork.offlineMode || base.photonView.isMine)
		{
			damageMessage.damage = (short)Kube.IS.weaponParams[numWeapon].Damage[Kube.IS.weaponParams[numWeapon].currentDamageIndex];
		}
		else
		{
			damageMessage.damage = 0;
		}
		damageMessage.id_killer = this.onlineId;
		damageMessage.team = this.team;
		damageMessage.weaponType = (short)numWeapon;
		if (this.type == 0 && !this.view3face && this.weaponAnim1face[numWeapon].Length != 0)
		{
			this.weaponGOScript.PrimaryAttack(numWeapon, shotPoint, damageMessage, this._anim);
		}
		else
		{
			this.weaponGOScript.WeaponShot(Kube.OH.weaponsBulletPrefab[numWeapon], shotPoint, damageMessage);
		}
		if (this.currentWeapon != null && (this.type == 1 || (this.type == 0 && this.view3face)) && this.currentWeapon.Type == 0)
		{
			this._anim.CrossFade(this.animSwordAttack[UnityEngine.Random.Range(0, this.animSwordAttack.Length)], 0.1f);
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
		float num = (float)KUI.width;
		float num2 = (float)KUI.height;
		Texture texture = null;
		int num3 = (!(this.currentWeapon != null)) ? -1 : this.currentWeapon.id;
		bool flag = false;
		if (this.currentWeapon != null && this.currentWeapon.sniper)
		{
			flag = true;
		}
		if (Kube.BCS.gameType != GameType.creating)
		{
			if (this.rifleAim && (num3 == 11 || flag))
			{
				texture = Kube.ASS3.rifleAimTex;
			}
			else if (this.rifleAim && num3 == 23)
			{
				texture = Kube.ASS3.spaceRifleAimTex;
			}
			else if (this.rifleAim && num3 == 31)
			{
				texture = Kube.ASS3.tacticRifleAimTex;
			}
			else if (!this.rifleAim && !Kube.OH.emptyScreen && this.currentWeapon != null && this.currentWeapon.aimTex.Length >= 2)
			{
				if (Time.time - this.lastShotTimeNew[this.currentWeapon.id] < 0.15f)
				{
					texture = this.currentWeapon.aimTex[1];
				}
				else
				{
					texture = this.currentWeapon.aimTex[0];
				}
			}
		}
		int num4;
		if (Kube.IS.chosenFastInventar == -1)
		{
			num4 = 0;
		}
		else if (Kube.BCS.gameType != GameType.creating)
		{
			num4 = this._inventory[Kube.IS.chosenFastInventar].Type;
			int num5 = this._inventory[Kube.IS.chosenFastInventar].Num;
		}
		else
		{
			num4 = Kube.GPS.fastInventarCreating[Kube.IS.chosenFastInventar].Type;
			int num5 = Kube.GPS.fastInventarCreating[Kube.IS.chosenFastInventar].Num;
		}
		if (texture == null && !this.rifleAim && !Kube.OH.emptyScreen && num4 != 4)
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

	public void PostChatMessage(string chatMessage)
	{
		if (chatMessage.Length != 0)
		{
			string text = this.playerName;
			if (this.dead)
			{
				text += "(RIP)";
			}
			text = text + ": " + AuxFunc.CodeRussianName(chatMessage);
			this.ChatMessage(text);
		}
	}

	private void DrawChat()
	{
		float num = (float)KUI.width;
		float num2 = (float)KUI.height;
		bool flag = true;
		GUI.skin = Kube.ASS1.mainSkinSmall;
		GUI.SetNextControlName("chatMessage");
		if (Event.current.Equals(Event.KeyboardEvent("return")))
		{
			this.PostChatMessage(this.chatMessage);
			flag = false;
			this.paused = false;
		}
		this.chatMessage = GUI.TextField(new Rect(0.2f * num, 0.2f * num2, 0.6f * num, 0.08f * num2), this.chatMessage, 64);
		GUI.FocusControl("chatMessage");
		if (GUI.Button(new Rect(0.8f * num, 0.2f * num2, 0.1f * num, 0.08f * num2), "Enter"))
		{
			this.PostChatMessage(this.chatMessage);
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
		KUI.DownScale();
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
		float num = (float)KUI.width;
		float num2 = (float)KUI.height;
		if (Kube.ASS2 == null)
		{
			Kube.RM.require("Assets2", null);
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
					if (this._canRespawn)
					{
						GUI.Box(new Rect(0.5f * num - 200f, num2 - 150f, 400f, 35f), Localize.ps_press_for_respawn);
					}
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
				int num5 = 0;
				for (int i = 0; i < 8; i++)
				{
					string text2 = string.Empty;
					PlayerScript.Activities activities = (PlayerScript.Activities)(1 << i & this.guiItemText);
					switch (activities)
					{
					case PlayerScript.Activities.to_delete_press:
						text2 = Localize.to_delete_press;
						break;
					case PlayerScript.Activities.to_move_press:
						text2 = Localize.to_move_press;
						break;
					default:
						if (activities == PlayerScript.Activities.to_edit_press)
						{
							text2 = Localize.to_edit_press;
						}
						break;
					case PlayerScript.Activities.to_activate_press:
						text2 = Localize.to_activate_press;
						break;
					case PlayerScript.Activities.to_rotate_press:
						text2 = Localize.to_rotate_press;
						break;
					}
					if (!(text2 == string.Empty))
					{
						GUI.Label(new Rect(0.5f * num, 0.5f * num2 + (float)num5 * 18f, 400f, 120f), text2);
						num5++;
					}
				}
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
				float num6 = Mathf.Max(0f, Mathf.Min(this.painAlpha, 1f));
				if (num6 > 0.02f)
				{
					GUI.color = new Color(1f, 0f, 0f, num6);
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
		if (this.charMovesNums == null)
		{
			this.charMovesNums = Kube.IS.getSpecListNums(InventoryScript.ItemPage.Moves);
		}
		float num = (float)KUI.width;
		float num2 = (float)KUI.height;
		GUI.skin = Kube.ASS1.bigWhiteLabel;
		float num3 = 70f + 35f * (float)this.charMovesNums.Length;
		float num4 = 0.5f * num - 163f;
		float num5 = 0.5f * num2 - num3 / 2f;
		GUI.DrawTexture(new Rect(num4, num5, 326f, num3), Kube.ASS1.menuBack);
		GUI.Label(new Rect(num4 + 10f, num5 + 10f, 306f, 50f), Localize.activities_title);
		for (int i = 0; i < this.charMovesNums.Length; i++)
		{
			if (Kube.GPS.inventarSpecItems[this.charMovesNums[i]] > 0)
			{
				if (GUI.Button(new Rect(num4 + 10f, num5 + 70f + (float)i * 35f, 300f, 30f), Localize.specItemsName[this.charMovesNums[i]]))
				{
					if (!this.view3face)
					{
						this.SetView(true);
					}
					this.PlayActivity(this.charMovesNums[i]);
					Kube.OH.closeMenu(new DrawCall(this.DrawActivitiesMenu));
				}
			}
			else if (GUI.Button(new Rect(num4 + 10f, num5 + 70f + (float)i * 35f, 300f, 30f), Localize.specItemsName[this.charMovesNums[i]] + " (" + Localize.move_learn + ")"))
			{
				Kube.OH.closeMenu(new DrawCall(this.DrawActivitiesMenu));
				Kube.IS.SendMessage("ToggleInventarCharMoves", this.charMovesNums[i]);
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

	[PunRPC]
	private void _PlayActivity(int numActivity, PhotonMessageInfo info)
	{
		if (numActivity == 1)
		{
			this._anim.Play(this.animDecor[0]);
		}
		else if (numActivity == 2)
		{
			this._anim.Play(this.animDecor[1]);
		}
		else if (numActivity == 3)
		{
			this._anim.Play(this.animDecor[2]);
		}
		else if (numActivity == 4)
		{
			this._anim.Play(this.animDecor[3]);
		}
		else if (numActivity == 5)
		{
			this._anim.Play(this.animDecor[4]);
		}
		else if (numActivity == 6)
		{
			this._anim.Play(this.animDecor[5]);
		}
		else if (numActivity == 7)
		{
			this._anim.Play(this.animDecor[6]);
		}
		else if (numActivity == 8)
		{
			this._anim.Play(this.animDecor[7]);
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

	[PunRPC]
	private void _ChatMessage(string _message, PhotonMessageInfo info)
	{
		Kube.GPS.printMessage(AuxFunc.DecodeRussianName(_message), Color.white);
	}

	public void DriveTransport(int _transportId, int _placeToDrive)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Transport");
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].GetComponent<SyncObjectScript>().objectId == _transportId)
			{
				TransportScript component = array[i].GetComponent<TransportScript>();
				this.DriveTransport(component, _placeToDrive);
				break;
			}
		}
	}

	public void DriveTransport(TransportScript ts, int _placeToDrive)
	{
		this.transportToDriveScript = ts.GetComponent<TransportScript>();
		this.transportToDrivePlace = _placeToDrive;
		this.isDriveTransport = true;
		if (this.transportToDriveScript.isDisableController(this.transportToDrivePlace))
		{
			this.controller.enabled = false;
		}
		bool flag = PlayerScript.userView3face;
		if (!flag && !this.transportToDriveScript.HasFPS(this.transportToDrivePlace))
		{
			flag = true;
		}
		if (base.photonView.isMine)
		{
			this.SetView(flag);
		}
		if (base.photonView.isMine && this.weaponGO && this.transportToDriveScript.isHideWeapon(this.transportToDrivePlace))
		{
			this.weaponGO.SetActive(false);
		}
	}

	private void ExitTransport(Vector3 exitVector)
	{
		if (!this.isDriveTransport)
		{
			return;
		}
		bool flag = this.weaponGO && !this.transportToDriveScript.driverCanUseOwnWeapon[this.transportToDrivePlace];
		this.isDriveTransport = false;
		this.transportToDrivePlace = 0;
		this.transportToDriveScript = null;
		if (base.photonView.isMine)
		{
			this.SetView(PlayerScript.userView3face);
			if (flag)
			{
				this.weaponGO.SetActive(true);
			}
		}
		base.transform.position += exitVector;
		this.controller.enabled = true;
		this.velocity = Vector3.zero;
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

	[PunRPC]
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
				this.playerClothes
			}));
			this.ToggleRenderers(!this.dead);
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

	public FastInventar[] fastInventar
	{
		get
		{
			return this._inventory;
		}
	}

	public void SavedInventory()
	{
		if (Kube.BCS.gameMode == GameMode.cooperative)
		{
			this._inventory = (FastInventar[])Kube.GPS.fastInventarWeapon.Clone();
		}
	}

	public void UpdateInventory(int numslot)
	{
		if (Kube.BCS.gameMode == GameMode.cooperative)
		{
			this._inventory = (FastInventar[])Kube.GPS.fastInventarWeapon.Clone();
			this.SelectWeapon(numslot);
		}
		else
		{
			Kube.GPS.printSystemMessage(Localize.weaponchangeafterrespawn, Color.red);
		}
	}

	public void SelectWeapon(int numSlot)
	{
		if (this.isDriveTransport && !this.transportToDriveScript.driverCanUseOwnWeapon[this.transportToDrivePlace])
		{
			return;
		}
		if (this.type != 0)
		{
			return;
		}
		int num = this._inventory[numSlot].Num;
		int num2 = (!(this.currentWeapon == null)) ? this.currentWeapon.id : -1;
		if (this.currentWeapon != null && Time.time - this.lastShotTimeNew[this.currentWeapon.id] < this.currentWeapon.DeltaShot)
		{
			return;
		}
		if (num2 == num)
		{
			if (!(this.currentWeapon != null))
			{
				return;
			}
			int weaponGroup = (int)Kube.IS.weaponParams[num].weaponGroup;
			if (Kube.BCS.gameType != GameType.hunger)
			{
				num = Kube.IS.findNextWeapon(num2, weaponGroup);
			}
			if (num == -1)
			{
				return;
			}
			this._inventory[weaponGroup] = new FastInventar(InventarType.weapons, num);
		}
		Kube.BCS.hud.ChoseWeapon(numSlot);
		Kube.IS.chosenFastInventar = numSlot;
		this.ChangeWeapon(num, -1);
	}

	public void ChangeWeapon(int _numWeapon, int numSkin = -1)
	{
		if (this.currentWeapon != null && this.currentWeapon.id == _numWeapon && numSkin == this.currentWeaponSkin)
		{
			return;
		}
		KubeInput.Reset();
		this.rifleAim = false;
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
		if (_numWeapon >= 0 && _numWeapon < Kube.IS.weaponParams.Length)
		{
			this.currentWeapon = Kube.IS.weaponParams[_numWeapon];
			if (numSkin == -1 && this.type == 0)
			{
				numSkin = Kube.GPS.weaponsCurrentSkin[this.currentWeapon.id];
			}
			this.currentWeaponSkin = numSkin;
			this.lastShotTimeNew[_numWeapon] = 0f;
			this.weaponGO = (UnityEngine.Object.Instantiate(Kube.OH.charWeaponsGO[_numWeapon], Vector3.zero, Quaternion.identity) as GameObject);
			this.weaponGOScript = this.weaponGO.GetComponent<WeaponScript>();
			this.weaponGOScript.owner = this;
			this.weaponGOScript.accuarcy = Kube.IS.weaponParams[_numWeapon].accuarcy;
			this.weaponGOScript.fatalDistance = Kube.IS.weaponParams[_numWeapon].fatalDistance;
			this._autoFire = Kube.OH.autoaim;
			if (Kube.IS.weaponParams[_numWeapon].weaponGroup == InventoryScript.WeaponGroup.melee || Kube.IS.weaponParams[_numWeapon].weaponGroup == InventoryScript.WeaponGroup.tactical || Kube.IS.weaponParams[_numWeapon].weaponGroup == InventoryScript.WeaponGroup.heavy)
			{
				this._autoFire = false;
			}
		}
		else
		{
			this.currentWeapon = null;
			this.currentWeaponSkin = -1;
		}
		this.RedrawWeapon();
	}

	private void RedrawWeapon()
	{
		if (this.weaponGO == null)
		{
			return;
		}
		if (this.currentWeapon != null)
		{
			bool flag = this.currentWeapon.weaponGroup == InventoryScript.WeaponGroup.melee;
		}
		if (!this.view3face)
		{
			this.weaponGO.transform.parent = this.weaponObjCamera.transform;
			this.weaponGO.transform.localPosition = Vector3.zero;
			this.weaponGO.transform.localRotation = Quaternion.identity;
			this._anim.Rewind(this.changeWeaponAnim);
			this._anim.Play(this.changeWeaponAnim);
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

	[PunRPC]
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

	[PunRPC]
	private void _ApplyDamage(short _damage, int _id_killer, int _team, short _weaponType, Vector3 _damagePos, PhotonMessageInfo info)
	{
		if (((Kube.BCS.gameType == GameType.mission && _team == this.team) || (Kube.BCS.gameMode == GameMode.teams && _team == this.team) || (Kube.BCS.gameType == GameType.survival && _id_killer > 0) || (Kube.BCS.gameType == GameType.captureTheFlag && _team == this.team) || (Kube.BCS.gameType == GameType.dominating && _team == this.team)) && _id_killer != this.onlineId)
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
				this.NO.BanPlayer(Kube.SS.serverId);
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
			if (component.flagState.state == FlagState.captured && component.flagState.playerCaptured == this.onlineId)
			{
				Kube.BCS.NO.ChangeFlagState(component.flagState.team, newState, this.onlineId);
				break;
			}
		}
	}

	private void OnDestroy()
	{
		this.LoseFlag(FlagState.dropped);
	}

	public void Die(int id_killer, int myPoints, bool isHeadshot, short weaponType, short damage)
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

	private void DropStuff()
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		if (Kube.BCS.gameTypeController && Kube.BCS.gameTypeController.DropStuff(this, this._weaponPickup))
		{
			return;
		}
		GameObject gameObject = PhotonNetwork.Instantiate("Assets7/StuffBox", base.transform.root.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, 0);
		DeadDropScript component = gameObject.GetComponent<DeadDropScript>();
		FastInventar[] inventory = this._inventory;
		List<FastInventar> list = new List<FastInventar>();
		for (int i = 0; i < inventory.Length; i++)
		{
			if (inventory[i].Type == 4)
			{
				int num = inventory[i].Num;
				if (num != 73)
				{
					if (Kube.IS.weaponParams[num].UsingBullets <= 0 || this.bullets[Kube.IS.weaponParams[num].BulletsType] > 0)
					{
						list.Add(inventory[i]);
					}
				}
			}
		}
		component.weapons = list.ToArray();
	}

	[PunRPC]
	private void _Die(int id_killer, int myPoints, bool isHeadshot, short weaponType, short damage, PhotonMessageInfo info)
	{
		if (this.dead)
		{
			return;
		}
		this.DropStuff();
		if (base.photonView.isMine)
		{
			Kube.IS.resetInventory();
		}
		if (Kube.BCS.ps && Kube.BCS.ps.onlineId == id_killer)
		{
			BattleControllerScript bcs = Kube.BCS;
			bcs.bonusCounters.kills = bcs.bonusCounters.kills + 1;
			if (this.onlineId == id_killer)
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
				if (this.onlineId == id_killer)
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
		this.ChangeWeapon(-1, -1);
		this.dead = true;
		if (Kube.BCS.gameType == GameType.mission)
		{
			this._canRespawn = Kube.BCS.gameTypeController.canRespawn;
		}
		else
		{
			this._canRespawn = true;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(this.ragdoll, base.transform.position, base.transform.rotation) as GameObject;
		PlayerScript.CopyTransformsRecurse(base.transform, gameObject.transform);
		gameObject.SendMessage("DressSkin", string.Concat(new object[]
		{
			string.Empty,
			this.playerSkin,
			";",
			this.playerClothes
		}));
		this._ragDollTrans = gameObject.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine");
		this.ToggleRenderers(false);
		base.gameObject.layer = 2;
		this.deadTimes++;
		PlayerScript killer = null;
		for (int i = 0; i < Kube.BCS.playersInfo.Length; i++)
		{
			PlayerScript ps = Kube.BCS.playersInfo[i].ps;
			if (!(ps == null))
			{
				if (ps.onlineId == id_killer && id_killer != this.onlineId)
				{
					killer = ps;
					ps.YouKilledPlayerFull(id_killer, this.onlineId, weaponType, myPoints, isHeadshot);
					if (base.photonView.isMine && ps.team != this.team)
					{
						this.availableCubes = Kube.GPS.maxAvailableCubes;
						Kube.BCS.gameTypeController.ChangeTeamScore(ps.team, 1);
					}
					break;
				}
			}
		}
		if (Kube.BCS.gameType == GameType.mission && this.type == 0)
		{
			MissionBase missionBase = Kube.BCS.gameTypeController as MissionBase;
			missionBase.respawnPos = base.transform.position;
		}
		if (id_killer == 0)
		{
			Kube.GPS.printSystemMessage(AuxFunc.DecodeRussianName(this.playerName) + " " + Localize.dead_by_nature, new Color(1f, 1f, 1f, 0.5f));
		}
		else if (id_killer == this.onlineId)
		{
			Kube.GPS.printSystemMessage(AuxFunc.DecodeRussianName(this.playerName) + " " + Localize.dead_himself, new Color(1f, 1f, 1f, 0.5f));
		}
		else if (id_killer < 0)
		{
			Kube.GPS.printSystemMessage(AuxFunc.DecodeRussianName(this.playerName) + " " + Localize.dead_by_zombie, new Color(1f, 1f, 1f, 0.5f));
		}
		if (base.photonView.isMine || PhotonNetwork.offlineMode)
		{
			for (int j = 0; j < 10; j++)
			{
				if (this._inventory[j].Type == 4 && Kube.GPS.inventarWeapons[this._inventory[j].Num] == 0)
				{
					this._inventory[j].Type = -1;
					this._inventory[j].Num = 0;
				}
			}
			this.deadTime = Time.time;
			base.transform.position -= this.cameraComp.transform.TransformDirection(Vector3.forward) * 5f;
			Kube.BCS.gameObject.SendMessage("PlayerDie", SendMessageOptions.DontRequireReceiver);
			if (this.isDriveTransport)
			{
				this.transportToDriveScript.ExitDrive(this.onlineId);
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
		if (Kube.BCS.onlineId == id_killer && this.onlineId != Kube.BCS.onlineId)
		{
			(UnityEngine.Object.Instantiate(Kube.OH.pointsText, base.transform.position + Vector3.up * 2f, Quaternion.identity) as GameObject).SendMessage("SetText", "+" + myPoints);
		}
		this.rifleAim = false;
		this.cameraComp.fieldOfView = 60f;
		if (this.playerView != null)
		{
			this.playerView.fieldOfView = 60f;
		}
		this.CameraDeadPosition(killer);
		this.sensitivityX = (this.sensitivityY = Kube.GPS.mouseSens);
	}

	private void CameraDeadPosition(PlayerScript killer)
	{
		if (Kube.BCS.gameType != GameType.mission)
		{
			Vector3 localPosition = this.weaponObjCamera.transform.localPosition;
			localPosition.x = 0.361f;
			this.weaponObjCamera.transform.localPosition = localPosition;
			return;
		}
		Vector3 position = Vector3.zero;
		Vector3 direction;
		Ray ray;
		if (killer)
		{
			direction = -(killer.transform.position - base.transform.position);
			ray = new Ray(base.transform.position + Vector3.up, direction);
		}
		else
		{
			direction = -this.cameraComp.transform.TransformDirection(Vector3.forward) * 5f;
			ray = new Ray(base.transform.position + Vector3.up, direction);
		}
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, direction.magnitude + 3f, 257))
		{
			position = raycastHit.point;
		}
		else
		{
			position = ray.origin + ray.direction.normalized * (direction.magnitude + 3f);
		}
		Vector3 position2 = base.transform.position;
		if (killer)
		{
			position2 = killer.transform.position;
		}
		base.transform.position = position;
		base.transform.LookAt(position2);
	}

	private void ToggleRenderers(bool p)
	{
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = p;
		}
	}

	public void SurvivalRespawn(Vector3 pos)
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
		Vector3 vector = Kube.BCS.FindRespawnPlace(false);
		GameObject[] array = new GameObject[0];
		Time.timeScale = 1f;
		if (Kube.BCS.gameType == GameType.creating || Kube.BCS.gameType == GameType.shooter || Kube.BCS.gameType == GameType.test || Kube.BCS.gameType == GameType.survival)
		{
			array = GameObject.FindGameObjectsWithTag("Respawn");
			if (array.Length != 0)
			{
				vector = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
			}
			if (PhotonNetwork.room != null)
			{
				base.photonView.RPC("_Respawn", PhotonTargets.All, new object[]
				{
					vector
				});
			}
			else
			{
				this._Respawn(vector, null);
			}
		}
		else if (Kube.BCS.gameMode == GameMode.teams)
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
				vector = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
			}
			this.LoseFlag(FlagState.onBase);
			if (PhotonNetwork.room != null)
			{
				base.photonView.RPC("_Respawn", PhotonTargets.All, new object[]
				{
					vector
				});
			}
			else
			{
				this._Respawn(vector, null);
			}
		}
		else if (Kube.BCS.gameType == GameType.mission || Kube.BCS.gameType == GameType.test)
		{
			array = GameObject.FindGameObjectsWithTag("Respawn");
			if (array.Length != 0)
			{
				vector = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
			}
			if (Kube.BCS.gameType == GameType.mission)
			{
				MissionBase missionBase = Kube.BCS.gameTypeController as MissionBase;
				if (missionBase.respawnPos != Vector3.zero)
				{
					vector = missionBase.respawnPos;
				}
			}
			if (PhotonNetwork.room != null)
			{
				base.photonView.RPC("_Respawn", PhotonTargets.All, new object[]
				{
					vector
				});
			}
			else
			{
				this._Respawn(vector, null);
			}
		}
	}

	[PunRPC]
	private void _Respawn(Vector3 position, PhotonMessageInfo info)
	{
		if (Kube.BCS.gameType == GameType.hunger && base.photonView.isMine && !this.dead && Kube.BCS.playersInfo.Length > 1)
		{
			BattleControllerScript bcs = Kube.BCS;
			bcs.bonusCounters.firstPlace = bcs.bonusCounters.firstPlace + 1;
		}
		if (this.survivalRespawnGO != null)
		{
			UnityEngine.Object.Destroy(this.survivalRespawnGO);
		}
		this.dead = false;
		this.carryingTheFlag = false;
		this.health = this.maxHealth;
		this.armor = this.maxArmor;
		base.transform.position = position;
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = true;
		}
		if (PhotonNetwork.offlineMode || base.photonView.isMine)
		{
			base.gameObject.layer = 9;
			this.type = 0;
			Kube.IS.ChoseFastInventarKey(0);
			this.SetView(false);
			Kube.BCS.gameObject.SendMessage("PlayerRespawn", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			this.cameraComp.gameObject.SetActive(false);
			base.gameObject.layer = 10;
			this.type = 1;
			this.SetView(true);
		}
		Time.timeScale = 1f;
		this.Spawn();
	}

	private new static void CopyTransformsRecurse(Transform src, Transform dst)
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
		this.frags++;
		this.points += _points;
		Kube.BCS.gameObject.SendMessage("KilledPlayer", SendMessageOptions.DontRequireReceiver);
	}

	public void YouKilledPlayerFull(int killer_id, int dead_id, short weaponType, int _points, bool isHeadshot)
	{
		this.frags++;
		PlayerScript playerScript = Kube.BCS.FindPlayerByOnlineId(dead_id);
		if (this.type == 0)
		{
			Kube.OH.gameObject.BroadcastMessage("PlayerDeadByMe", new object[]
			{
				playerScript,
				weaponType,
				isHeadshot
			}, SendMessageOptions.DontRequireReceiver);
		}
		this.points += _points;
		Kube.BCS.gameObject.SendMessage("KilledPlayer", SendMessageOptions.DontRequireReceiver);
		if (killer_id == Kube.BCS.onlineId)
		{
			string str = string.Empty;
			for (int i = 0; i < Kube.BCS.playersInfo.Length; i++)
			{
				if (Kube.BCS.playersInfo[i].id == dead_id)
				{
					str = AuxFunc.DecodeRussianName(Kube.BCS.playersInfo[i].ps.playerName);
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
		else if (dead_id == Kube.BCS.onlineId)
		{
			string str2 = string.Empty;
			for (int j = 0; j < Kube.BCS.playersInfo.Length; j++)
			{
				if (Kube.BCS.playersInfo[j].id == killer_id)
				{
					str2 = AuxFunc.DecodeRussianName(Kube.BCS.playersInfo[j].ps.playerName);
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
			if (Kube.BCS.playersInfo[k].id == dead_id)
			{
				text = AuxFunc.DecodeRussianName(Kube.BCS.playersInfo[k].ps.playerName);
			}
		}
		string text2 = string.Empty;
		for (int l = 0; l < Kube.BCS.playersInfo.Length; l++)
		{
			if (Kube.BCS.playersInfo[l].id == killer_id)
			{
				text2 = AuxFunc.DecodeRussianName(Kube.BCS.playersInfo[l].ps.playerName);
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
		int num = (!(this.currentWeapon != null)) ? -1 : this.currentWeapon.id;
		if (PhotonNetwork.connected)
		{
			if (stream.isWriting)
			{
				if (Time.time - this.lastSendProps > 5f)
				{
					stream.SendNext(1);
					stream.SendNext(this.onlineId);
					this.lastSendProps = Time.time;
				}
				else
				{
					stream.SendNext(2);
				}
				stream.SendNext(base.transform.position);
				stream.SendNext(base.transform.rotation);
				stream.SendNext((short)num);
				stream.SendNext((short)this.currentWeaponSkin);
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
					this.onlineId = (int)stream.ReceiveNext();
				}
				this.correctPlayerPos = (Vector3)stream.ReceiveNext();
				this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
				short num2 = (short)stream.ReceiveNext();
				short num3 = (short)stream.ReceiveNext();
				if ((int)num3 != num || (int)num2 != num)
				{
					this.ChangeWeapon((int)num2, (int)num3);
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
			if (this._inventory[j].Type == 4 && this._inventory[j].Num == weaponType)
			{
				return;
			}
		}
		int weaponGroup = (int)Kube.IS.weaponParams[weaponType].weaponGroup;
		if (this._inventory[weaponGroup].Type == -1 || weaponType == 73 || Kube.BCS.gameType == GameType.hunger)
		{
			this._inventory[weaponGroup].Type = 4;
			this._inventory[weaponGroup].Num = weaponType;
			Kube.IS.ChoseFastInventarKey(weaponGroup);
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
		this.NO.BanPlayer(Kube.SS.serverId);
	}

	public bool HaveKeys(bool _red, bool _green, bool _blue, bool _gold)
	{
		int num = 0;
		if (Kube.BCS.gameMode == GameMode.teams)
		{
			if (_red)
			{
				num = 0;
			}
			if (_green)
			{
				num = 1;
			}
			if (_blue)
			{
				num = 2;
			}
			if (_gold)
			{
				num = 3;
			}
			return this.team == num;
		}
		if (_red)
		{
			num |= 1;
		}
		if (_green)
		{
			num |= 2;
		}
		if (_blue)
		{
			num |= 4;
		}
		return (num & this.keymask) == num;
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

	[PunRPC]
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

	[PunRPC]
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

	[PunRPC]
	private void _UnFreeze(PhotonMessageInfo info)
	{
		this.freezed = false;
	}

	[PunRPC]
	private void _GiveLotOfDrop(object[] dataArray, PhotonMessageInfo info)
	{
		PhotonStream photonStream = new PhotonStream(false, dataArray);
		int num = (int)photonStream.ReceiveNext();
		for (int i = 0; i < num; i++)
		{
			int num2 = (int)photonStream.ReceiveNext();
			int bulletsType = Kube.IS.weaponParams[num2].BulletsType;
			this.GetNewWeapon(num2, Mathf.CeilToInt((float)Kube.IS.bulletParams[bulletsType].puckupAmount / 2f));
		}
	}

	public void GiveLotOfDrop(FastInventar[] weapons)
	{
		PhotonStream photonStream = new PhotonStream(true, null);
		photonStream.SendNext(weapons.Length);
		for (int i = 0; i < weapons.Length; i++)
		{
			photonStream.SendNext(weapons[i].Num);
		}
		object[] array = photonStream.ToArray();
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_GiveLotOfDrop", base.photonView.owner, new object[]
			{
				array
			});
		}
		else
		{
			this._GiveLotOfDrop(array, null);
		}
	}

	private void SaveCodeVars()
	{
		this.codeVarsRandom = UnityEngine.Random.Range(10, 1000);
		this._availableCubes2 = this.availableCubes + this.codeVarsRandom;
		this._health2 = this.health + this.codeVarsRandom;
		this._maxHealth2 = this.maxHealth + this.codeVarsRandom;
		this._armor2 = this.armor + this.codeVarsRandom;
		this._frags2 = this.frags + this.codeVarsRandom;
		this._kills2 = this.kills + this.codeVarsRandom;
		this._points2 = this.points + this.codeVarsRandom;
		this._playerSkin2 = this.playerSkin + this.codeVarsRandom;
		this._level2 = this.level + this.codeVarsRandom;
		for (int i = 0; i < 12; i++)
		{
			this._bullets2[i] = this.bullets[i] + this.codeVarsRandom;
		}
		for (int j = 0; j < 128; j++)
		{
			this._lastShotTimeNew2[j] = this.lastShotTimeNew[j] + (float)this.codeVarsRandom;
		}
		for (int k = 0; k < 128; k++)
		{
			this._clips2[k] = this.clips[k] + this.codeVarsRandom;
		}
		this._frags |= (this.codeVarsRandom & 7);
	}

	private void LoadCodeVars()
	{
		this.availableCubes = this._availableCubes2 - this.codeVarsRandom;
		this.health = this._health2 - this.codeVarsRandom;
		this.maxHealth = this._maxHealth2 - this.codeVarsRandom;
		this.armor = this._armor2 - this.codeVarsRandom;
		this.kills = this._kills2 - this.codeVarsRandom;
		this.frags = this._frags2 - this.codeVarsRandom;
		this.points = this._points2 - this.codeVarsRandom;
		this.playerSkin = this._playerSkin2 - this.codeVarsRandom;
		this.level = this._level2 - this.codeVarsRandom;
		for (int i = 0; i < 12; i++)
		{
			this.bullets[i] = this._bullets2[i] - this.codeVarsRandom;
		}
		for (int j = 0; j < 128; j++)
		{
			this.lastShotTimeNew[j] = this._lastShotTimeNew2[j] - (float)this.codeVarsRandom;
		}
		for (int k = 0; k < 128; k++)
		{
			this.clips[k] = this._clips2[k] - this.codeVarsRandom;
		}
	}

	public void Push(Vector3 dir)
	{
		this.pushVelocity = dir;
	}

	internal void RemoveWeapon(int p)
	{
		if (p == -1)
		{
			return;
		}
		this._weaponPickup.Remove(p);
		int weaponGroup = (int)Kube.IS.weaponParams[p].weaponGroup;
		int num = Kube.IS.findNextWeapon(p, weaponGroup);
		this._inventory[weaponGroup].Num = num;
		this.ChangeWeapon(num, -1);
	}

	public void GotKey(int p)
	{
		this.keymask |= 1 << p;
	}

	public void RespawnFromRevive()
	{
		this.Respawn();
	}

	protected const float ITEM_USE_TIMEOUT = 2f;

	private const float AIR_FRICTION = 5f;

	private const float AIR_ACCELERATE = 2.5f;

	private const float FLY_ACCELERATE = 4f;

	private const float SAFEFALLVELOCITY = 10f;

	private const float FLASH_DURATION = 20f;

	public PlayerConfig playerConfig;

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

	protected int _skybox;

	protected float _nextCreatingRepeat;

	public float fallDamage = 10f;

	public ObscuredFloat runSpeed = 5f;

	public ObscuredFloat runSpeedBonus = 0f;

	public ObscuredFloat jumpSpeed = 10f;

	public ObscuredFloat jumpSpeedBonus = 0f;

	public float nextJump;

	private CharacterController controller;

	private NetworkObjectScript NO;

	[NonSerialized]
	public int type = -1;

	[NonSerialized]
	public float sensitivityX = 15f;

	[NonSerialized]
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

	public WeaponParamsObj currentWeapon;

	[NonSerialized]
	public int onlineId;

	[NonSerialized]
	public int serverId;

	[NonSerialized]
	public string sn;

	public GameObject ragdoll;

	public int maxArmor = 100;

	public int maxArmorBonus;

	public float painAlpha;

	public ObscuredFloat reduceDamage = 0f;

	public ObscuredFloat reduceDamageBonus = 0f;

	public int pointsForKillMe = 10;

	public int deadTimes;

	public PlayerScript.LastShotTime lastShotTimeNew = new PlayerScript.LastShotTime();

	private ObscuredInt _kills;

	private ObscuredInt _frags;

	private ObscuredInt _points;

	public bool canBuild;

	public bool canBuildBlock;

	public string playerName = string.Empty;

	private string chatMessage = string.Empty;

	private ObscuredInt _playerSkin = 0;

	public string[] weaponAnim1face;

	public string changeWeaponAnim;

	public float stepDeltaTime = 0.3f;

	private float lastStepTime;

	public GameObject survivalRespawnPrefab;

	public GameObject rankPlane;

	private GameObject survivalRespawnGO;

	public ObscuredInt _level;

	[NonSerialized]
	private ObscuredInt _health3;

	[NonSerialized]
	public ObscuredInt _maxHealth;

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

	protected bool jetPackEnabled;

	protected Animation _anim;

	protected Dictionary<int, int> _weaponPickup;

	private Dictionary<int, PlayerScript.InventarItems> inventarItems = new Dictionary<int, PlayerScript.InventarItems>();

	private bool grounded = true;

	private ObscuredVector3 velocity;

	private float forwardRun;

	private float sideRun;

	public float rotateDirMax = 20f;

	public float rotateSensivity = 10f;

	[NonSerialized]
	public bool view3face;

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

	[NonSerialized]
	public int _geom;

	protected int[] geometryIds;

	protected int MAX_GEOM = 15;

	protected static int[] geometryIdsCodes = new int[]
	{
		0,
		1,
		2,
		3,
		4,
		8,
		12,
		16,
		20,
		24,
		28,
		32,
		36,
		40,
		44
	};

	public float GROUND_FRICTION = 35f;

	public float GROUND_ACCELERATE = 10f;

	protected float _SafeFallVelocity = 12f;

	protected float nextEnvDamage;

	protected Light _light;

	protected float _nextWheeel;

	[HideInInspector]
	public Transform headTransform;

	[HideInInspector]
	public Transform rightHandTransform;

	private Vector3 lastPos;

	protected static bool userView3face = false;

	private float lastMonstersStartle;

	private float monstersStartleDeltaTime = 3f;

	private int guiItemText;

	private int[] charMovesNums;

	public bool isDriveTransport;

	public TransportScript transportToDriveScript;

	private int transportToDrivePlace;

	private string playerClothes = string.Empty;

	protected float showFastInventoryTime;

	protected FastInventar[] _inventory = new FastInventar[10];

	public int currentWeaponSkin = -1;

	protected bool _autoFire;

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

	private int _health2;

	private int _maxHealth2;

	private int _armor2;

	private int _kills2;

	private int _points2;

	private int _playerSkin2;

	private int _level2;

	private int _frags2;

	private int[] _bullets2 = new int[12];

	private int[] _clips2 = new int[128];

	private float[] _lastShotTimeNew2 = new float[128];

	private Vector3 pushVelocity = Vector3.zero;

	protected int keymask;

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

		private int[] _clips = new int[128];
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

		private float[] _lastShotTimeNew = new float[128];
	}

	private struct InventarItems
	{
		public float nextUse;

		public int cnt;
	}

	private enum Activities
	{
		to_delete_press = 1,
		to_move_press,
		to_activate_press = 4,
		to_rotate_press = 8,
		to_edit_press = 16
	}
}
