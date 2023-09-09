using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using kube;
using kube.data;
using kube.ui;
using UnityEngine;

public class BattleControllerScript : MonoBehaviour
{
	public void BanPlayer(int id)
	{
		this.NO.BanPlayer(id);
		this.RefreshPlayersTable();
	}

	public void BanMeImCheater()
	{
		Kube.BCS.EndGame(BattleControllerScript.EndGameType.ban);
	}

	public void ChangeCanBuildStatus(int id, bool canBuild)
	{
		this.NO.ChangeCanBuildStatus(id, canBuild);
		this.RefreshPlayersTable();
	}

	public int onlineId
	{
		get
		{
			if (this.ps)
			{
				return this.ps.onlineId;
			}
			return -1;
		}
	}

	public bool shotingMode
	{
		get
		{
			return this.gameType == GameType.mission || this.gameType == GameType.shooter || this.gameType == GameType.survival || this.gameType == GameType.teams || this.gameType == GameType.test;
		}
	}

	public void RecountBonuses(BonusVariableType bvt, int lastValue, int newValue)
	{
		if (newValue == 0)
		{
			return;
		}
		for (int i = 0; i < Kube.IS.bonusParams.Length; i++)
		{
			if (Kube.IS.bonusParams[i].bonusVariable == bvt && lastValue < Kube.IS.bonusParams[i].needForGetBonus && newValue >= Kube.IS.bonusParams[i].needForGetBonus)
			{
				this.bonusesInRound[i]++;
				BattleControllerScript.ShowBonusesStruct showBonusesStruct = default(BattleControllerScript.ShowBonusesStruct);
				showBonusesStruct.beginShowTime = Time.time;
				showBonusesStruct.bonusType = i;
				this.showBonuses.Add(showBonusesStruct);
			}
		}
	}

	private void CMD_plist()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("=======================\r\n");
		for (int i = 0; i < this.playersInfo.Length; i++)
		{
			stringBuilder.Append(this.playersInfo[i].UID + " " + AuxFunc.DecodeRussianName(this.playersInfo[i].Name) + "\r\n");
		}
		string text = stringBuilder.ToString();
		TextEditor textEditor = new TextEditor();
		textEditor.content = new GUIContent(text);
		textEditor.SelectAll();
		textEditor.Copy();
		UnityEngine.Debug.Log(text);
	}

	public bool isBuiltinMap
	{
		get
		{
			return this.mapId <= 0L;
		}
	}

	private void Start()
	{
		this.menu = Cub2Menu.instance.gameObject;
		this.hud = ((GameMenu)Cub2Menu.instance).hud.GetComponent<UIHUD>();
		this.firstPage = Cub2UI.FindMenu<TeamStartMenu>("start_teams");
		this.endRound = Cub2UI.FindMenu<EndRoundMenu>("EndRoundPVP");
		this.finalUI = Cub2UI.FindMenu<EndMissionDialog>("proidino_misiia");
		this.endRoundScoresUI = Cub2UI.FindMenu<EndRoundNewDialog>("konec_raunda_NEW");
		this.levelUpUI = Cub2UI.FindMenu<NewLevelDialog>("dialog_levelup");
		this.bonusesInRound = new int[Kube.IS.bonusParams.Length];
		for (int i = 0; i < this.bonusesInRound.Length; i++)
		{
			this.bonusesInRound[i] = 0;
		}
		this.showBonuses = new ArrayList();
		this.hud.gameObject.SetActive(false);
		Kube.OH.EndLoading();
		this.gameProcess = BattleControllerScript.GameProcess.start;
		this.NO = PhotonNetwork.Instantiate("NetworkObject", Vector3.zero, Quaternion.identity, 0).GetComponent<NetworkObjectScript>();
		this.mapId = Kube.OH.tempMap.Id;
		if (this.mapId > 0L)
		{
			int num = Mathf.FloorToInt((float)(this.mapId % 20L));
			this.ownerId = Convert.ToInt32((double)(this.mapId - (long)num) / 20.0);
		}
		else
		{
			this.ownerId = 0;
		}
		this.newPlayersCanBuild = false;
		this.gameType = Kube.OH.tempMap.GameType;
		if (!PhotonNetwork.offlineMode && Kube.GPS.isVIP)
		{
			this.uSpeakerGO = PhotonNetwork.Instantiate("photonUSpeaker", Vector3.zero, Quaternion.identity, 0).GetComponent<USpeaker>();
		}
		if (this.gameType == GameType.mission)
		{
			if (!PhotonNetwork.offlineMode)
			{
				this.jetPackAwail = (bool)PhotonNetwork.room.customProperties["jet"];
			}
			else
			{
				this.jetPackAwail = true;
			}
		}
		else if (this.gameType == GameType.bomb || this.gameType == GameType.hunger)
		{
			this.jetPackAwail = false;
		}
		else
		{
			this.jetPackAwail = true;
		}
		if (Kube.OH.tempMap.GameType == GameType.mission)
		{
			this.gameType = GameType.mission;
		}
		if (this.gameType == GameType.mission)
		{
			this._missionId = Kube.OH.tempMap.missionId;
		}
		if (this.gameType == GameType.shooter || this.gameType == GameType.survival)
		{
			this.canChangeWorld = false;
		}
		if (this.gameType == GameType.creating)
		{
			this.canChangeWorld = true;
			this.canUseWeapons = false;
		}
		if (this.gameType == GameType.teams)
		{
			this.canChangeWorld = true;
			this.canUseWeapons = true;
		}
		if (this.gameType == GameType.mission)
		{
			this.canChangeWorld = false;
			this.canUseWeapons = true;
		}
		if (Kube.OH.tempMap.CanBreak == 0)
		{
			this.mapCanBreak = false;
		}
		else
		{
			this.mapCanBreak = true;
		}
		this.dayLightType = Kube.OH.tempMap.DayLight;
		this.adviceNum = UnityEngine.Random.Range(0, Localize.advices.Length);
		for (int j = 0; j < 10; j++)
		{
		}
		this.olddt = DateTime.Now;
		this.oldTick = (long)Environment.TickCount;
		base.InvokeRepeating("invSpeedHack", 5f, 5f);
		PhotonNetwork.isMessageQueueRunning = true;
		this.LoadMap();
		this.FPStimeleft = this.FPSupdateInterval;
		this.FPSarray = new float[20];
		for (int k = 0; k < this.FPSarray.Length; k++)
		{
			this.FPSarray[k] = 60f;
		}
		base.Invoke("CancelPending", this.cancelPendingTimeout);
	}

	private void Awake()
	{
		Kube.BCS = this;
	}

	private void OnDestroy()
	{
		Kube.BCS = null;
	}

	public void MonsterDead()
	{
		this.survivalKilledMonsters++;
	}

	private void OnDisconnectedFromPhoton()
	{
		if (this.isLoadingWorldChanges && !PhotonNetwork.offlineMode)
		{
			this.LoadMainMenu();
		}
		if (this.gameProcess == BattleControllerScript.GameProcess.game)
		{
			this.EndGame(BattleControllerScript.EndGameType.netError);
		}
	}

	private void CancelPending()
	{
		UnityEngine.Debug.Log("Timeout: CancelPending");
		if (this.isLoadingWorldChanges && !PhotonNetwork.offlineMode)
		{
			this.LoadMainMenu();
		}
	}

	private void LoadMap()
	{
		bool flag = !Kube.BCS.mapCanBreak && !Kube.BCS.canChangeWorld;
		this.waintingForMap = 1;
		if (flag || PhotonNetwork.offlineMode || (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient))
		{
			this.LoadMapFromServer();
			return;
		}
		base.InvokeRepeating("RequestMap", 0.1f, 10f);
	}

	private void OnMasterClientSwitched()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		if (this.waintingForMap == 1)
		{
			this.LoadMap();
		}
	}

	private void RequestMap()
	{
		this.NO.RequestMap();
	}

	public void OnMapLoaded(byte[] mapData)
	{
		if (this.waintingForMap != 3)
		{
			if (Kube.WHS.LoadWorld(mapData) == 1)
			{
				PhotonNetwork.LeaveRoom();
				Kube.GPS.printMessage(Localize.error_empty_map, Color.black);
				UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
			}
			this.waintingForMap = 3;
		}
	}

	public void LoadMapFromServer()
	{
		base.CancelInvoke("RequestMap");
		this.waintingForMap = 2;
		if (Kube.OH.tempMap.Id > 0L)
		{
			MonoBehaviour.print("Loading map number: " + Kube.OH.tempMap.Id);
			Kube.SS.LoadMap(Kube.OH.tempMap.Id);
		}
		else
		{
			MonoBehaviour.print("Loading buildinMap: " + Kube.OH.tempMap.Id);
			Kube.RM.downloadMap(Kube.OH.tempMap.Id);
		}
	}

	public PlayerScript CreatePlayer(Vector3 respawnPlace, Quaternion rot)
	{
		PlayerScript component = PhotonNetwork.Instantiate("characterType3", respawnPlace, Quaternion.identity, 0).GetComponent<PlayerScript>();
		component.Init();
		return component;
	}

	public void EnterGame()
	{
		if (this.gameProcess == BattleControllerScript.GameProcess.game)
		{
			return;
		}
		if (PhotonNetwork.isMasterClient)
		{
			ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.room.customProperties;
			customProperties["started"] = true;
			PhotonNetwork.room.SetCustomProperties(customProperties, null, false);
			this.NO.EnterGame();
		}
		this.battleCamera.SetActive(false);
		Vector3 respawnPlace = this.FindRespawnPlace(true);
		GameObject[] array = GameObject.FindGameObjectsWithTag("Respawn");
		if (array.Length != 0)
		{
			respawnPlace = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
		}
		this.ps = Kube.BCS.CreatePlayer(respawnPlace, Quaternion.identity);
		KubeScreen.lockCursor = true;
		this.gameProcess = BattleControllerScript.GameProcess.game;
	}

	public Vector3 FindRespawnPlace(bool findGO = true)
	{
		Vector3 position = new Vector3((float)(Kube.WHS.sizeX / 2), 40f, (float)(Kube.WHS.sizeZ / 2));
		for (int i = Kube.WHS.sizeY - 2; i > 0; i--)
		{
			if (Kube.WHS.cubeTypes[Kube.WHS.sizeX / 2, i, Kube.WHS.sizeZ / 2] != 0)
			{
				position = new Vector3((float)Kube.WHS.sizeX / 2f, (float)i + 1f, (float)Kube.WHS.sizeZ / 2f);
				break;
			}
		}
		if (!findGO)
		{
			return position;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Respawn");
		if (array.Length != 0)
		{
			position = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
		}
		return position;
	}

	private void BeginGame()
	{
		base.CancelInvoke("CancelPending");
		this.menu.SetActive(false);
		this.hud.Init();
		Kube.WHS.SetDayLight((this.dayLightType != 2) ? 1f : 0f, false);
		if (Kube.WHS.skybox == 2)
		{
			GameObject original = (GameObject)Kube.Load("RainSnow/Snow", typeof(GameObject));
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original);
			gameObject.SetActive(true);
			gameObject.transform.position = new Vector3((float)(Kube.WHS.sizeX / 2), 96f, (float)(Kube.WHS.sizeY / 2));
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			component.shape.box = new Vector3((float)Kube.WHS.sizeX, 1f, (float)Kube.WHS.sizeY);
		}
		GameObject.FindGameObjectWithTag("Music").SendMessage("ChangeMusic", UnityEngine.Random.Range(1, 4), SendMessageOptions.DontRequireReceiver);
		Kube.WHS.RedrawWorld(true, false, true);
		if (PhotonNetwork.isMasterClient)
		{
			this.gameStartTime = Time.time;
		}
		else
		{
			this.gameStartTime = Time.time;
		}
		this.gameEndTime = (int)this.gameStartTime + Kube.OH.gameMaxTime[(int)this.gameType];
		this.gameMode = this.SelectGameMode(this.gameType);
		if (this.gameType == GameType.creating || this.gameType == GameType.shooter || this.gameType == GameType.test || this.gameType == GameType.survival)
		{
			this.battleCamera.SetActive(false);
			Vector3 respawnPlace = this.FindRespawnPlace(true);
			this.ps = this.CreatePlayer(respawnPlace, Quaternion.identity);
			if (this.gameType == GameType.creating)
			{
				Kube.IS.ShowFastPanel(true);
			}
			KubeScreen.lockCursor = true;
			this.gameProcess = BattleControllerScript.GameProcess.game;
			this.survivalBeginTime = Time.realtimeSinceStartup;
			this.survivalMonstersPerWave = -1;
			if (this.gameType == GameType.shooter)
			{
				this.gameTypeController = base.gameObject.AddComponent<ShooterController>();
			}
			else if (this.gameType == GameType.survival)
			{
				this.gameTypeController = base.gameObject.AddComponent<SurvivalController>();
			}
		}
		else if (this.gameType == GameType.teams)
		{
			this.gameTypeController = base.gameObject.AddComponent<TeamShooterController>();
		}
		else if (this.gameType == GameType.captureTheFlag)
		{
			this.gameTypeController = base.gameObject.AddComponent<CaptureTheFlagController>();
		}
		else if (this.gameType == GameType.bomb)
		{
			this.gameTypeController = base.gameObject.AddComponent<BombController>();
			this.gameTypeController.Initialize();
		}
		else if (this.gameType == GameType.dominating)
		{
			this.gameTypeController = base.gameObject.AddComponent<DominatingController>();
			this.gameTypeController.Initialize();
		}
		else if (this.gameType == GameType.hunger)
		{
			this.gameTypeController = base.gameObject.AddComponent<HungerController>();
			this.gameTypeController.Initialize();
		}
		else if (this.gameType == GameType.mission)
		{
			this.missionType = Kube.OH.tempMap.missionType;
			if (this.missionType == ObjectsHolderScript.MissionType.reachTheExit)
			{
				this.gameTypeController = base.gameObject.AddComponent<MissionReachTheExit>();
			}
			else if (this.missionType == ObjectsHolderScript.MissionType.killNMonsters)
			{
				this.gameTypeController = base.gameObject.AddComponent<MissionKillNMonsters>();
			}
			else if (this.missionType == ObjectsHolderScript.MissionType.holdNSeconds)
			{
				this.gameTypeController = base.gameObject.AddComponent<MissionHoldNSecond>();
			}
			else if (this.missionType == ObjectsHolderScript.MissionType.findNitems)
			{
				this.gameTypeController = base.gameObject.AddComponent<MissionFindNItems>();
			}
			else if (this.missionType == ObjectsHolderScript.MissionType.findNitemsInMSeconds)
			{
				this.gameTypeController = base.gameObject.AddComponent<MissionFindItemsInTime>();
			}
			else if (this.missionType == ObjectsHolderScript.MissionType.killNMonstersInMSeconds)
			{
				this.gameTypeController = base.gameObject.AddComponent<MissionKillMonstersInTime>();
			}
			else if (this.missionType == ObjectsHolderScript.MissionType.reachTheExitInTime)
			{
				this.gameTypeController = base.gameObject.AddComponent<MissionExitInTime>();
			}
			this.gameTypeController.configure(Kube.OH.tempMap.missionConfig);
			if (!((MissionBase)this.gameTypeController).syncStart)
			{
				this.EnterGame();
			}
			this.survivalBeginTime = Time.realtimeSinceStartup;
			this.survivalMonstersPerWave = -1;
		}
		this.NO.SendMeGameParams((int)this.gameType);
		if (this.gameType == GameType.dominating || this.gameType == GameType.teams || this.gameType == GameType.captureTheFlag || this.gameType == GameType.bomb)
		{
			Cub2UI.currentMenu = this.firstPage.gameObject;
			this.firstPage.BeginPlay();
		}
		if (this.gameType == GameType.creating && Kube.GPS.needTrainingBuild)
		{
			this.StartTutorial();
		}
		else if (this.gameType == GameType.mission && this._missionId == 1)
		{
			this.StartTutorial();
		}
		this.oldLevel = Kube.GPS.playerLevel;
	}

	private GameMode SelectGameMode(GameType gameType)
	{
		switch (gameType)
		{
		case GameType.shooter:
			return GameMode.shooter;
		case GameType.survival:
		case GameType.mission:
			return GameMode.cooperative;
		case GameType.teams:
		case GameType.dominating:
		case GameType.bomb:
			return GameMode.teams;
		}
		return GameMode.shooter;
	}

	private void StartTutorial()
	{
		GameObject gameObject = (GameObject)Resources.Load("TutorialGO");
		gameObject = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		if (this.gameType == GameType.mission)
		{
			gameObject.SendMessage("StartMissionTutor");
		}
		else
		{
			gameObject.SendMessage("StartCreatingTutor");
		}
		this.tutorialGO = gameObject;
	}

	public bool isMapOwner
	{
		get
		{
			return this.ownerId == Kube.SS.serverId || (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient && this.ownerId == 0);
		}
	}

	public void StartTestMission()
	{
		if (this.isMapOwner)
		{
			this.NO.ToggleTestMission(true);
		}
	}

	public void EndTestMission()
	{
		if (this.isMapOwner)
		{
			this.NO.ToggleTestMission(false);
		}
	}

	public void DoStartTestMission()
	{
		this.gameType = GameType.test;
		this.showMenu = false;
		this.ps.paused = false;
		Kube.OH.closeMenuAll();
		this.ps.Respawn();
		this.gameStartTime = Time.time;
		Kube.IS.ShowFastPanel(false);
	}

	public void DoEndTestMission()
	{
		this.gameType = GameType.creating;
		Kube.IS.ChoseFastInventar(0);
		Kube.IS.ShowFastPanel(true);
		GameObject[] array = GameObject.FindGameObjectsWithTag("Monster");
		foreach (GameObject targetGo in array)
		{
			PhotonNetwork.Destroy(targetGo);
		}
		array = GameObject.FindGameObjectsWithTag("Transport");
		foreach (GameObject targetGo2 in array)
		{
			PhotonNetwork.Destroy(targetGo2);
		}
		if (this.ps != null)
		{
			this.ps.ChangeWeapon(-1, -1);
		}
	}

	public void MonsterRespawnTick()
	{
		if (Time.time - this.lastCheckMonstersTime > this.lastCheckMonstersDeltaTime && PhotonNetwork.isMasterClient)
		{
			this.monsters = GameObject.FindGameObjectsWithTag("Monster");
			for (int i = 0; i < this.monsters.Length; i++)
			{
				MonoBehaviour.print(string.Concat(new object[]
				{
					"monster: ",
					i,
					" ",
					this.monsters[i].name,
					" ",
					this.monsters[i].transform.root.name
				}));
			}
			this.monstersScript = new MonsterScript[this.monsters.Length];
			for (int j = 0; j < this.monsters.Length; j++)
			{
				this.monstersScript[j] = this.monsters[j].GetComponent<MonsterScript>();
			}
			for (int k = 0; k < Kube.WHS.monsterRespawnS.Length; k++)
			{
				if (!(Kube.WHS.monsterRespawnS[k] == null))
				{
					if (Time.time < Kube.WHS.monsterLastDieTime[k])
					{
						bool flag = false;
						for (int l = 0; l < this.monsters.Length; l++)
						{
							MonsterScript monsterScript = this.monstersScript[l];
							if (!(monsterScript == null))
							{
								if (monsterScript.createdFromRespawnNum == k)
								{
									flag = true;
									break;
								}
							}
						}
						if (!flag)
						{
							Kube.WHS.monsterLastDieTime[k] = 0f;
						}
					}
				}
			}
			for (int m = 0; m < Kube.WHS.monsterRespawnS.Length; m++)
			{
				if (Kube.WHS.monsterRespawnS[m])
				{
					if (Time.time - Kube.WHS.monsterLastDieTime[m] > (float)Kube.WHS.monsterRespawnS[m].secToRespawn[Kube.WHS.monsterRespawnS[m].respawnTime])
					{
						this.NO.RequestToRespawnMonster(m);
					}
				}
			}
			this.lastCheckMonstersTime = Time.time;
		}
	}

	public void TransportRespawnTick()
	{
		if (Time.time - this.lastCheckTransportTime > this.lastCheckTransportDeltaTime && PhotonNetwork.isMasterClient)
		{
			this.transports = GameObject.FindGameObjectsWithTag("Transport");
			this.transportsScript = new TransportScript[this.transports.Length];
			for (int i = 0; i < this.transports.Length; i++)
			{
				this.transportsScript[i] = this.transports[i].GetComponent<TransportScript>();
			}
			for (int j = 0; j < Kube.WHS.transportRespawnS.Length; j++)
			{
				if (Kube.WHS.transportRespawnS[j])
				{
					if (Time.time < Kube.WHS.transportLastDieTime[j])
					{
						bool flag = false;
						for (int k = 0; k < this.transports.Length; k++)
						{
							TransportScript transportScript = this.transportsScript[k];
							if (!(transportScript == null))
							{
								if (transportScript.objectId == j)
								{
									flag = true;
									break;
								}
							}
						}
						if (!flag)
						{
							Kube.WHS.transportLastDieTime[j] = 0f;
						}
					}
				}
			}
			for (int l = 0; l < Kube.WHS.transportRespawnS.Length; l++)
			{
				if (Kube.WHS.transportRespawnS[l])
				{
					if (Time.time - Kube.WHS.transportLastDieTime[l] > (float)Kube.WHS.transportRespawnS[l].secToRespawn[Kube.WHS.transportRespawnS[l].respawnTime])
					{
						this.NO.RequestToRespawnTransport(l);
					}
				}
			}
			this.lastCheckTransportTime = Time.time;
		}
	}

	private void SurvivalRespawnThink()
	{
		string text = string.Empty;
		float num = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2f + 2f);
		float num2 = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2.1f - 2f);
		float num3 = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2.2f - 4f);
		float num4 = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2.3f - 3f);
		float num5 = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2.4f - 5f);
		float num6 = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2.5f - 7f);
		float num7 = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2.6f - 8f);
		float num8 = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2.7f - 9f);
		float num9 = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2.8f - 10f);
		float num10 = Mathf.Max(new float[]
		{
			num,
			num2,
			num3,
			num4,
			num6,
			num5,
			num7,
			num8,
			num9
		});
		GameObject[] array = GameObject.FindGameObjectsWithTag("RespawnHumanoid");
		bool flag = array.Length == 0 || (double)UnityEngine.Random.value > 0.5;
		if (flag)
		{
			int num11 = 0;
			if (num10 == num2)
			{
				num11 = 1;
			}
			else if (num10 == num3)
			{
				num11 = 2;
			}
			else if (num10 == num4)
			{
				num11 = 3;
			}
			else if (num10 == num6)
			{
				num11 = 4;
			}
			else if (num10 == num5)
			{
				num11 = 5;
			}
			else if (num10 == num7)
			{
				num11 = 7;
			}
			else if (num10 == num8)
			{
				num11 = 8;
			}
			else if (num10 == num9)
			{
				num11 = 15;
			}
			MonsterRespawnScript monsterRespawnScript = null;
			for (int i = 0; i < Kube.WHS.monsterRespawnS.Length; i++)
			{
				if (!(Kube.WHS.monsterRespawnS[i] == null))
				{
					if (Kube.WHS.monsterRespawnS[i].type == num11)
					{
						monsterRespawnScript = Kube.WHS.monsterRespawnS[i];
						if ((double)UnityEngine.Random.value > 0.5)
						{
							break;
						}
					}
				}
			}
			if (monsterRespawnScript != null)
			{
				text = Kube.OH.monsterPrefabName[num11];
				GameObject gameObject = PhotonNetwork.Instantiate(text, monsterRespawnScript.transform.position, Quaternion.identity, 0);
				gameObject.SendMessage("SetAngry", true);
				gameObject.SendMessage("SetMonsterNum", num11);
				this.survivalLastMonsterSpawnTime = Time.time;
				return;
			}
		}
		if (num10 == num)
		{
			text = "Zombie";
		}
		if (num10 == num2)
		{
			text = "Agent";
		}
		if (num10 == num3)
		{
			text = "Soldat";
		}
		if (num10 == num4)
		{
			text = "ZombieAxes";
		}
		if (num10 == num5)
		{
			text = "ZombieSaw";
		}
		if (num10 == num6)
		{
			text = "Demon";
		}
		else if (num10 == num7)
		{
			text = "Assets8/Agent2";
		}
		else if (num10 == num8)
		{
			text = "Assets8/Stalker";
		}
		else if (num10 == num9)
		{
			text = "FlySoldat";
		}
		int num12 = UnityEngine.Random.Range(0, array.Length);
		if (array.Length != 0)
		{
			GameObject gameObject2 = PhotonNetwork.Instantiate(text, array[num12].transform.position, Quaternion.identity, 0);
			gameObject2.SendMessage("SetAngry", true);
			gameObject2.SendMessage("SetMonsterNum", Array.IndexOf<string>(Kube.OH.monsterPrefabName, text));
			gameObject2.SendMessage("SetHealthMultiplier", this.survivalWaveNum / 5);
			gameObject2.SendMessage("SetDamageMultiplier", this.survivalWaveNum / 5);
		}
		this.survivalLastMonsterSpawnTime = Time.time;
	}

	private void Update()
	{
		if (this.isLoadingWorldChanges)
		{
			if (this.waintingForMap == 3)
			{
				this.BeginGame();
				this.isLoadingWorldChanges = false;
			}
		}
		else
		{
			this.currentGameTime = Time.realtimeSinceStartup - this.gameStartTime;
			if (this.dayLightType == 1 && (this.gameType != GameType.creating || !Kube.GPS.needTrainingBuild))
			{
				int num = Mathf.FloorToInt(this.currentGameTime / this.dayPeriod) % 2;
				float num2 = this.currentGameTime - Mathf.Floor(this.currentGameTime / this.dayPeriod) * this.dayPeriod;
				if (num2 > this.dayPeriod * this.dayNightPerc)
				{
					float tLight;
					if (num == 0)
					{
						tLight = (this.dayPeriod - num2) / ((1f - this.dayNightPerc) * this.dayPeriod);
					}
					else
					{
						tLight = 1f - (this.dayPeriod - num2) / ((1f - this.dayNightPerc) * this.dayPeriod);
					}
					Kube.WHS.SetDayLight(tLight, true);
				}
				else if (num == 0)
				{
					Kube.WHS.SetDayLight(1f, true);
				}
				else
				{
					Kube.WHS.SetDayLight(0f, true);
				}
			}
			if (Time.time - this.lastCountPlayersTime > this.countPlayersDeltaTime && (this.gameProcess == BattleControllerScript.GameProcess.game || this.gameProcess == BattleControllerScript.GameProcess.start))
			{
				this.RefreshPlayersTable();
				this.monsters = GameObject.FindGameObjectsWithTag("Monster");
				this.monstersScript = new MonsterScript[this.monsters.Length];
				for (int i = 0; i < this.monsters.Length; i++)
				{
					this.monstersScript[i] = this.monsters[i].GetComponent<MonsterScript>();
				}
				this.currentNumMonsters = this.monsters.Length;
				if (this.ps != null)
				{
					this.tempPsKills = this.ps.kills + this.ps.frags;
					this.tempPsPoints = this.ps.points;
				}
				this.lastCountPlayersTime = Time.time;
			}
			if (UnityEngine.Input.GetKey(KeyCode.Tab))
			{
				this.hud.score.SetActive(true);
			}
			else
			{
				this.hud.score.SetActive(false);
			}
			bool flag = Kube.OH.isMenu || this.gameProcess != BattleControllerScript.GameProcess.game;
			if (this.ps && this.ps.paused)
			{
				flag = true;
			}
			KubeScreen.lockCursor = !flag;
			if (this.ps != null && this.gameProcess == BattleControllerScript.GameProcess.game)
			{
				this.ps.paused = Kube.OH.isMenu;
			}
			bool flag2 = UnityEngine.Input.GetKeyDown(KeyCode.Escape) || UnityEngine.Input.GetKeyDown(KeyCode.BackQuote);
			if (flag2 && this.gameProcess == BattleControllerScript.GameProcess.game)
			{
				this.showMenu = Kube.OH.isMenu;
				if (this.ps != null)
				{
					this.ps.paused = this.showMenu;
				}
				if (!Kube.OH.isMenu)
				{
					if (this.gameProcess != BattleControllerScript.GameProcess.end)
					{
						this.menu.SetActive(true);
					}
				}
				else if (flag2)
				{
					this.menu.SetActive(false);
				}
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.F10))
			{
				this.SaveMap();
			}
			if (this.gameProcess == BattleControllerScript.GameProcess.game && this.ps == null)
			{
				this.EndGame(BattleControllerScript.EndGameType.netError);
			}
			if (this.gameType == GameType.teams && this.gameProcess == BattleControllerScript.GameProcess.game)
			{
				this.MonsterRespawnTick();
				this.TransportRespawnTick();
			}
			if (this.gameType == GameType.survival && this.gameProcess == BattleControllerScript.GameProcess.game)
			{
				this.survivalTime = Time.realtimeSinceStartup - this.survivalBeginTime;
				float num3 = 1f;
				for (int j = 1; j < this.currentNumPlayers; j++)
				{
					num3 += Mathf.Pow(0.7f, (float)j);
				}
				if (this.survivalTime > this.survivalPrewaveTime && this.survivalMonstersPerWave == -1)
				{
					this.survivalMonstersPerWave = this.GetNumMonstersPerWave(this.survivalWaveNum);
					this.survivalMaxMonsters = this.GetMaxMonstersPerWave(this.survivalWaveNum);
					this.survivalKilledMonsters = 0;
				}
				else if (this.survivalKilledMonsters + 4 >= (int)((float)this.survivalMonstersPerWave * num3) && this.currentNumMonsters <= 4 && this.survivalMonstersPerWave > 0)
				{
					if (PhotonNetwork.isMasterClient || PhotonNetwork.offlineMode)
					{
						this.NO.SurvivalStartNewWave(this.survivalWaveNum + 1);
					}
				}
				else
				{
					if (Time.time - this.survivalLastMonsterSpawnTime > this.survivalMonsterSpawnDeltaTime && this.currentNumMonsters < (int)((float)this.survivalMaxMonsters * num3) && this.survivalKilledMonsters < (int)((float)this.survivalMonstersPerWave * num3) && this.currentNumMonsters < 60)
					{
						this.SurvivalRespawnThink();
					}
					this.TransportRespawnTick();
				}
				if (this.currentNumDeadPlayers == this.currentNumPlayers && this.currentNumMonsters != 0)
				{
					this.EndGame(BattleControllerScript.EndGameType.exit);
				}
			}
			if (this.gameProcess == BattleControllerScript.GameProcess.game && this.gameType == GameType.test)
			{
				this.MonsterRespawnTick();
				this.TransportRespawnTick();
			}
			this.FPStimeleft -= Time.deltaTime;
			this.FPSaccum += Time.timeScale / Time.deltaTime;
			this.FPSframes++;
			if ((double)this.FPStimeleft <= 0.0)
			{
				float num4 = this.FPSaccum / (float)this.FPSframes;
				this.FPSarray[this.FPSnumInArray] = num4;
				this.FPSnumInArray++;
				if (this.FPSnumInArray >= this.FPSarray.Length)
				{
					this.FPSnumInArray = 0;
				}
				float num5 = 0f;
				for (int k = 0; k < this.FPSarray.Length; k++)
				{
					num5 += this.FPSarray[k];
				}
				num5 /= (float)this.FPSarray.Length;
				if (this.FPSworst > num5)
				{
					this.FPSworst = num5;
				}
				this.FPStimeleft = this.FPSupdateInterval;
				this.FPSframes = 0;
				this.FPSaccum = 0f;
			}
			for (int l = 0; l < this.showBonuses.Count; l++)
			{
				if (((BattleControllerScript.ShowBonusesStruct)this.showBonuses[l]).beginShowTime + this.bonusShowTime < Time.time)
				{
					this.showBonuses.RemoveAt(l);
					break;
				}
			}
		}
	}

	public void SurvivalStartNewWave(int numWave)
	{
		this.survivalBeginTime = Time.realtimeSinceStartup;
		this.survivalWaveNum = numWave;
		this.survivalMonstersPerWave = -1;
		if (!Kube.IS.ps.dead)
		{
			this.bonusCounters.survivalWave = this.bonusCounters.survivalWave + 1;
		}
		if (Kube.IS.ps.dead)
		{
			Kube.IS.ps.SendMessage("Respawn");
		}
		if (numWave > 2)
		{
			PhotonNetwork.room.visible = false;
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer otherPlayer)
	{
		this.RefreshPlayersTable();
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		this.RefreshPlayersTable();
	}

	public void RefreshPlayersTable()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		this.currentNumPlayers = array.Length;
		this.currentNumDeadPlayers = 0;
		for (int i = 0; i < array.Length; i++)
		{
			PlayerScript component = array[i].GetComponent<PlayerScript>();
			if (component && component.dead)
			{
				this.currentNumDeadPlayers++;
			}
		}
		this.playersInfo = new BattleControllerScript.PlayerInfo[array.Length];
		this.playersInTeam = new int[8];
		for (int j = 0; j < this.playersInTeam.Length; j++)
		{
			this.playersInTeam[j] = 0;
		}
		for (int k = 0; k < array.Length; k++)
		{
			PlayerScript component2 = array[k].GetComponent<PlayerScript>();
			if (component2 && component2.type != -1)
			{
				this.playersInfo[k].ps = component2;
				this.playersInfo[k].Name = component2.playerName;
				this.playersInfo[k].id = component2.onlineId;
				this.playersInfo[k].serverId = component2.serverId;
				if (Kube.BCS.gameType == GameType.survival)
				{
					this.playersInfo[k].Frags = component2.kills;
				}
				else
				{
					this.playersInfo[k].Frags = component2.frags;
				}
				this.playersInfo[k].Deaths = component2.deadTimes;
				this.playersInfo[k].CanBuild = component2.canBuild;
				this.playersInfo[k].Level = component2.level;
				this.playersInfo[k].Score = 0;
				this.playersInfo[k].Team = component2.team;
				this.playersInfo[k].UID = component2.uid;
				this.playersInfo[k].sn = component2.sn;
				for (int l = 0; l < k; l++)
				{
					if (!(component2.sn != this.playersInfo[l].sn))
					{
						if (component2.serverId != 0 && component2.serverId == this.playersInfo[l].serverId)
						{
							this.NO.BanPlayer(component2.serverId);
							break;
						}
					}
				}
				if (component2.team >= 0)
				{
					this.playersInTeam[component2.team]++;
				}
			}
		}
		int[] array2 = new int[array.Length];
		if (this.gameMode == GameMode.cooperative)
		{
			for (int m = 0; m < this.currentNumPlayers; m++)
			{
				array2[m] = this.playersInfo[m].Frags - this.playersInfo[m].Deaths;
			}
		}
		else
		{
			for (int n = 0; n < this.currentNumPlayers; n++)
			{
				array2[n] = this.playersInfo[n].Frags;
			}
		}
		Array.Sort<int, BattleControllerScript.PlayerInfo>(array2, this.playersInfo);
		Array.Reverse(this.playersInfo);
		this.playersRIP = GameObject.FindGameObjectsWithTag("PlayerRIP");
	}

	public void SaveMap()
	{
		this._SaveMap(null, string.Empty);
	}

	public void SaveMapAndExit()
	{
		this._SaveMap(base.gameObject, "ExitGame");
	}

	private void _SaveMap(GameObject go = null, string message = "")
	{
		if (this.gameType == GameType.creating || this.gameType == GameType.test)
		{
			if (this.ownerId == Kube.SS.serverId)
			{
				if (Kube.OH.tempMap.CreatedGame)
				{
					Kube.SS.SaveMap(this.mapId, Kube.WHS.SaveWorld(), go, message);
					if (Kube.GPS.needTrainingBuild)
					{
						Kube.TS.SendMessage("MapSaved");
					}
					this.ps.SendMessage("SetAllTokenItems", SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					Kube.GPS.printMessage(Localize.BCS_cant_save_if_joined, Color.white);
				}
			}
			else
			{
				Kube.GPS.printMessage(Localize.BCS_only_owner_can_save, Color.white);
			}
		}
		else
		{
			Kube.GPS.printMessage(Localize.BCS_cant_save_in_battle, Color.white);
		}
	}

	public void SurvivalSetParams(float _survivalTime, int _survivalNumWave, int _survivalKilledMonsters)
	{
		this.survivalWaveNum = _survivalNumWave;
		this.survivalBeginTime = Time.realtimeSinceStartup - _survivalTime;
		this.survivalKilledMonsters = _survivalKilledMonsters;
		this.survivalMonstersPerWave = this.GetNumMonstersPerWave(this.survivalWaveNum);
		this.survivalMaxMonsters = this.GetMaxMonstersPerWave(this.survivalWaveNum);
	}

	public void MissionSetParams(float goneGameTime)
	{
		this.gameStartTime = Time.realtimeSinceStartup - goneGameTime;
		this.gameEndTime = (int)this.gameStartTime + Kube.OH.gameMaxTime[2];
		MonoBehaviour.print(string.Concat(new object[]
		{
			Time.realtimeSinceStartup,
			" ",
			this.gameStartTime,
			" ",
			this.gameEndTime
		}));
	}

	public int tempPsKills
	{
		get
		{
			return -this._tempPsKills + 1;
		}
		set
		{
			this._tempPsKills = 1 - value;
		}
	}

	public int tempPsPoints
	{
		get
		{
			return -this._tempPsPoints + 1;
		}
		set
		{
			this._tempPsPoints = 1 - value;
		}
	}

	public EndGameStats CalcGameStats()
	{
		int deltaFrags = 0;
		if (this.ps)
		{
			deltaFrags = this.ps.frags;
		}
		this.SumRoundBonuses();
		this.endGameExp = this.gameTypeController.CalcGameExp();
		for (int i = 0; i < this.sumBonusesExp.Count; i++)
		{
			this.endGameExp += this.sumBonusesExp[i];
		}
		this.endGameKills = this.tempPsKills;
		if (Kube.GPS.vipEnd - Time.time > 0f)
		{
			this.endGameExp += (int)((float)this.endGameExp * (float)Kube.GPS.vipBonus / 100f);
		}
		this.newLevel = Kube.OH.GetLevel((uint)((ulong)Kube.GPS.playerExp + (ulong)((long)this.endGameExp)));
		this.endGameTime = (int)Time.timeSinceLevelLoad;
		this.endGameFragsPerSec = Mathf.Round(600f * (float)this.endGameKills / (float)this.endGameTime) / 10f;
		this.endGameMoney = (int)(Kube.OH.pointsToMoney * (float)this.endGameExp);
		if (OfferBox.special.ContainsKey("drop"))
		{
			this.endGameMoney *= 2;
			this.endGameExp *= 2;
		}
		this.fragsTotal += this.endGameKills;
		this.moneyTotal += this.endGameMoney;
		this.expTotal += this.endGameExp;
		return new EndGameStats(Kube.GPS.playerExp, this.endGameExp, Kube.GPS.playerFrags, deltaFrags, this.tempPsKills, Kube.GPS.playerMoney1, this.endGameMoney, Kube.GPS.playerLevel, this.newLevel, this.bonusesInRound);
	}

	public void SendGameResultToServer()
	{
		Kube.SS.SendEndLevel(this.CalcGameStats(), Kube.OH.gameObject, "SendLevelDoneDone");
	}

	public void EndRound()
	{
		Kube.BCS.endRound.Open();
		this.gameProcess = BattleControllerScript.GameProcess.end;
	}

	public void EndGame(BattleControllerScript.EndGameType endGameType)
	{
		if (this.gameProcess == BattleControllerScript.GameProcess.exit)
		{
			return;
		}
		this.lastEndGameType = endGameType;
		KubeScreen.lockCursor = false;
		this.gameProcess = BattleControllerScript.GameProcess.exit;
		if (this.ps)
		{
			this.tempPsKills = this.ps.kills;
		}
		if (this.FPSworst < 5f)
		{
			Kube.SS.SendStat("FPS_0_5");
		}
		else if (this.FPSworst < 10f)
		{
			Kube.SS.SendStat("FPS_5_10");
		}
		else if (this.FPSworst < 20f)
		{
			Kube.SS.SendStat("FPS_10_20");
		}
		else
		{
			Kube.SS.SendStat("FPS_20");
		}
		if (PhotonNetwork.isMasterClient && PhotonNetwork.room != null && endGameType != BattleControllerScript.EndGameType.exit)
		{
			PhotonNetwork.room.visible = false;
		}
		if (((this.gameType != GameType.creating && this.gameType != GameType.mission) || (this.gameType == GameType.mission && endGameType == BattleControllerScript.EndGameType.exitTrigger)) && endGameType != BattleControllerScript.EndGameType.ban)
		{
			EndGameStats endGameStats = this.CalcGameStats();
			this.endGameKills = endGameStats.deltaFrags;
			this.endGameExp = endGameStats.deltaExp;
			this.endGameMoney = endGameStats.deltaMoney;
			this.endGameTime = (int)Time.timeSinceLevelLoad;
			this.endGameFragsPerSec = Mathf.Round(600f * (float)this.endGameKills / (float)this.endGameTime) / 10f;
			if (endGameType == BattleControllerScript.EndGameType.time)
			{
				this.endGameCapture = Localize.BCS_endGame_timeout;
			}
			else if (endGameType == BattleControllerScript.EndGameType.ban)
			{
				this.endGameCapture = Localize.BCS_endGame_ban;
			}
			else if (endGameType == BattleControllerScript.EndGameType.exit)
			{
				if (this.gameType == GameType.survival)
				{
					this.endGameCapture = Localize.BCS_endGame_noSuvivours;
				}
				else
				{
					this.endGameCapture = Localize.BCS_endGame_gameOver;
				}
			}
			else if (endGameType == BattleControllerScript.EndGameType.netError)
			{
				this.endGameCapture = Localize.BCS_endGame_lostConnection;
			}
			else if (endGameType == BattleControllerScript.EndGameType.lose)
			{
				this.endGameCapture = Localize.BCS_endGame_tryAgain;
			}
			else if (endGameType == BattleControllerScript.EndGameType.endRound)
			{
				this.endGameCapture = Localize.BCS_end_round;
			}
			this.SendGameResultToServer();
			if (Kube.BCS.gameType == GameType.mission)
			{
				if (this._missionId != 0)
				{
					this.finalUI.Open(Kube.BCS._missionId, Kube.BCS.lastEndGameType == BattleControllerScript.EndGameType.exitTrigger, endGameStats);
				}
				else
				{
					Kube.BCS.ExitGame();
				}
			}
			else
			{
				this.endRoundScoresUI.Open(endGameStats, this.endGameTime, this.endGameCapture);
			}
			if (this.gameType == GameType.mission || this.gameType == GameType.survival)
			{
				if (Kube.IS.ps != null)
				{
					Kube.IS.ps.ChatMessage(Kube.IS.ps.playerName + " " + Localize.player_exit);
				}
				if (Kube.IS.ps != null)
				{
					PhotonNetwork.Destroy(Kube.IS.ps.gameObject);
				}
			}
			else if (Kube.IS.ps != null)
			{
				Kube.IS.ps.paused = true;
				Kube.IS.ps.cameraComp.enabled = false;
				Kube.IS.ps.playerView.enabled = false;
			}
			KubeScreen.lockCursor = false;
			this.battleCamera.SetActive(true);
			if (this.ps)
			{
				this.ps.gameObject.SetActive(false);
			}
			Kube.OH.closeMenuAll();
			Kube.BCS.hud.isVisible = false;
			return;
		}
		if (endGameType == BattleControllerScript.EndGameType.ban)
		{
			Kube.GPS.printMessage(Localize.BCS_ban_from_server, Color.red);
		}
		PhotonNetwork.LeaveRoom();
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
	}

	public void ExitGame()
	{
		if (Kube.IS.ps != null)
		{
			Kube.IS.ps.ChatMessage(Kube.IS.ps.playerName + " " + Localize.player_exit);
		}
		if (Kube.IS.ps != null)
		{
			PhotonNetwork.Destroy(Kube.IS.ps.gameObject);
		}
		PhotonNetwork.LeaveRoom();
		this.LoadMainMenu();
	}

	private void ExitToMainMenu()
	{
		this.EndGame(BattleControllerScript.EndGameType.exit);
	}

	private void LoadMainMenu()
	{
		PhotonNetwork.LeaveRoom();
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
	}

	private void drawPlayerScores()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		GUI.DrawTexture(new Rect(0.5f * num - 300f, 0.5f * num2 - 100f, 600f, 41f + 25f * (float)this.playersInfo.Length), Kube.ASS1.tabTex);
		GUI.skin = Kube.ASS1.emptySkin;
		GUI.Box(new Rect(0.5f * num - 265f, 0.5f * num2 - 100f, 198f, 41f + 25f * (float)this.playersInfo.Length), Localize.BCS_name);
		GUI.Box(new Rect(0.5f * num - 65f, 0.5f * num2 - 100f, 98f, 41f + 25f * (float)this.playersInfo.Length), Localize.BCS_frags);
		GUI.Box(new Rect(0.5f * num + 35f, 0.5f * num2 - 100f, 98f, 41f + 25f * (float)this.playersInfo.Length), Localize.BCS_deathes);
		for (int i = 0; i < this.playersInfo.Length; i++)
		{
			GUI.skin = Kube.ASS1.mainSkinSmall;
			GUI.DrawTexture(new Rect(0.5f * num - 300f, 0.5f * num2 - 65f + 25f * (float)i, 32f, 32f), Kube.ASS2.RankTex[Mathf.Min(this.playersInfo[i].Level, Kube.ASS2.RankTex.Length - 1)].mainTexture);
			if (GUI.Button(new Rect(0.5f * num - 265f, 0.5f * num2 - 65f + 25f * (float)i, 200f, 28f), AuxFunc.DecodeRussianName(this.playersInfo[i].Name)))
			{
				Kube.SN.gotoUserByUID(this.playersInfo[i].UID);
			}
			GUI.skin = Kube.ASS1.bigBlackLabel;
			GUI.Label(new Rect(0.5f * num - 65f, 0.5f * num2 - 65f + 25f * (float)i, 100f, 28f), string.Empty + this.playersInfo[i].Frags);
			GUI.Label(new Rect(0.5f * num + 35f, 0.5f * num2 - 65f + 25f * (float)i, 100f, 28f), string.Empty + this.playersInfo[i].Deaths);
		}
	}

	private void drawTeamScores()
	{
	}

	private void saveProgressGUI()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		if (Kube.SS.savingMap)
		{
			GUI.skin = Kube.ASS1.blueButtonSkin;
			GUI.Box(new Rect(0.5f * num - 150f, 0.5f * num2 - 20f, 300f, 40f), Localize.BCS_wait_while_saving);
			return;
		}
		Kube.OH.closeMenu(new DrawCall(this.saveProgressGUI));
	}

	private void OnGUI()
	{
		GUI.depth = -1;
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		if (this.isLoadingWorldChanges)
		{
			GUI.skin = Kube.ASS1.bigWhiteLabel;
			GUI.DrawTexture(new Rect(0f, 0f, num, num2), Kube.OH.loadTex);
			GUI.Label(new Rect(0.5f * num - 100f, 20f, 200f, 40f), Localize.BCS_loading);
			GUI.Label(new Rect(0.5f * num - 250f, num2 - 100f, 500f, 90f), Localize.BCS_advice + "\n" + Localize.advices[this.adviceNum]);
			return;
		}
		if (Kube.OH.emptyScreen)
		{
			return;
		}
		if (Kube.ASS2 == null)
		{
			return;
		}
		if (this.gameProcess == BattleControllerScript.GameProcess.start)
		{
		}
		if (this.gameProcess == BattleControllerScript.GameProcess.game)
		{
			if (this.showBonuses.Count > 0)
			{
				Color color = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, Mathf.Min(3f * (-Time.time + ((BattleControllerScript.ShowBonusesStruct)this.showBonuses[this.showBonuses.Count - 1]).beginShowTime + this.bonusShowTime) / this.bonusShowTime, 1f));
				GUI.DrawTexture(new Rect(0.5f * num - (float)Kube.ASS3.bonusesBackground.width / 2f, 0.2f * num2, (float)Kube.ASS3.bonusesBackground.width, (float)Kube.ASS3.bonusesBackground.height), Kube.ASS3.bonusesBackground);
				GUI.color = color;
			}
			for (int i = 0; i < this.showBonuses.Count; i++)
			{
				Color color2 = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, Mathf.Min(3f * (-Time.time + ((BattleControllerScript.ShowBonusesStruct)this.showBonuses[i]).beginShowTime + this.bonusShowTime) / this.bonusShowTime, 1f));
				float num3 = 0.5f * num - 234f * (float)this.showBonuses.Count / 2f + 234f * (float)i;
				GUI.DrawTexture(new Rect(num3, 0.2f * num2 + 5f, (float)Kube.ASS2.bonusTex[((BattleControllerScript.ShowBonusesStruct)this.showBonuses[i]).bonusType].width, (float)Kube.ASS2.bonusTex[((BattleControllerScript.ShowBonusesStruct)this.showBonuses[i]).bonusType].height), Kube.ASS2.bonusTex[((BattleControllerScript.ShowBonusesStruct)this.showBonuses[i]).bonusType]);
				GUI.skin = Kube.ASS1.bigWhiteLabel;
				string text = Localize.bonusName[((BattleControllerScript.ShowBonusesStruct)this.showBonuses[i]).bonusType] + "\n";
				Color color3;
				if (Kube.IS.bonusParams[((BattleControllerScript.ShowBonusesStruct)this.showBonuses[i]).bonusType].experience >= 0)
				{
					text += "+";
					color3 = new Color(0f, 1f, 0f, Mathf.Min(3f * (-Time.time + ((BattleControllerScript.ShowBonusesStruct)this.showBonuses[i]).beginShowTime + this.bonusShowTime) / this.bonusShowTime, 1f));
				}
				else
				{
					color3 = new Color(1f, 0.1f, 0.1f, Mathf.Min(3f * (-Time.time + ((BattleControllerScript.ShowBonusesStruct)this.showBonuses[i]).beginShowTime + this.bonusShowTime) / this.bonusShowTime, 1f));
				}
				text = text + string.Empty + Kube.IS.bonusParams[((BattleControllerScript.ShowBonusesStruct)this.showBonuses[i]).bonusType].experience;
				GUI.color = new Color(0f, 0f, 0f, Mathf.Min(3f * (-Time.time + ((BattleControllerScript.ShowBonusesStruct)this.showBonuses[i]).beginShowTime + this.bonusShowTime) / this.bonusShowTime, 1f));
				GUI.Label(new Rect(num3 - 2f, 0.2f * num2 + 110f + 2f, 234f, 70f), text);
				GUI.Label(new Rect(num3 - 2f, 0.2f * num2 + 110f - 2f, 234f, 70f), text);
				GUI.Label(new Rect(num3 + 2f, 0.2f * num2 + 110f + 2f, 234f, 70f), text);
				GUI.Label(new Rect(num3 + 2f, 0.2f * num2 + 110f - 2f, 234f, 70f), text);
				GUI.color = color3;
				GUI.Label(new Rect(num3, 0.2f * num2 + 110f, 234f, 70f), text);
				GUI.color = color2;
			}
			if (this.gameType == GameType.survival && Kube.IS.ps != null && this.playersRIP != null)
			{
				for (int j = 0; j < this.playersRIP.Length; j++)
				{
					if (Vector3.Angle(Kube.IS.ps.cameraComp.transform.TransformDirection(Vector3.forward), this.playersRIP[j].transform.position - Kube.IS.ps.cameraComp.transform.position) <= 90f)
					{
						Vector3 vector = Kube.IS.ps.cameraComp.WorldToViewportPoint(this.playersRIP[j].transform.position);
						Color color4 = GUI.color;
						GUI.color = new Color(1f, 1f, 1f, 1f);
						GUI.DrawTexture(new Rect(vector.x * num - 29f, num2 - vector.y * num2 - 29f, 58f, 58f), Kube.ASS3.playerRIPTex[0]);
						float num4 = Mathf.Max(0f, Mathf.Sin(Time.time * 5f));
						GUI.color = new Color(1f, 1f, 1f, 0.6f * num4);
						GUI.DrawTexture(new Rect(vector.x * num - 29f, num2 - vector.y * num2 - 29f, 58f, 58f), Kube.ASS3.playerRIPTex[1]);
						num4 = Mathf.Max(0f, Mathf.Sin(Time.time * 5f - 0.5f));
						GUI.color = new Color(1f, 1f, 1f, 0.3f * num4);
						GUI.DrawTexture(new Rect(vector.x * num - 29f, num2 - vector.y * num2 - 29f, 58f, 58f), Kube.ASS3.playerRIPTex[2]);
						GUI.color = color4;
					}
				}
			}
			if (this.showBonuses.Count > 0)
			{
				Color color5 = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, Mathf.Min(3f * (-Time.time + ((BattleControllerScript.ShowBonusesStruct)this.showBonuses[this.showBonuses.Count - 1]).beginShowTime + this.bonusShowTime) / this.bonusShowTime, 1f));
				GUI.DrawTexture(new Rect(0.5f * num - (float)Kube.ASS3.bonusesBackground.width / 2f, 0.2f * num2, (float)Kube.ASS3.bonusesBackground.width, (float)Kube.ASS3.bonusesBackground.height), Kube.ASS3.bonusesBackground);
				GUI.color = color5;
			}
			for (int k = 0; k < this.showBonuses.Count; k++)
			{
				Color color6 = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, Mathf.Min(3f * (-Time.time + ((BattleControllerScript.ShowBonusesStruct)this.showBonuses[k]).beginShowTime + this.bonusShowTime) / this.bonusShowTime, 1f));
				float num5 = 0.5f * num - 234f * (float)this.showBonuses.Count / 2f + 234f * (float)k;
				GUI.DrawTexture(new Rect(num5, 0.2f * num2 + 5f, (float)Kube.ASS2.bonusTex[((BattleControllerScript.ShowBonusesStruct)this.showBonuses[k]).bonusType].width, (float)Kube.ASS2.bonusTex[((BattleControllerScript.ShowBonusesStruct)this.showBonuses[k]).bonusType].height), Kube.ASS2.bonusTex[((BattleControllerScript.ShowBonusesStruct)this.showBonuses[k]).bonusType]);
				GUI.skin = Kube.ASS1.bigWhiteLabel;
				string text2 = Localize.bonusName[((BattleControllerScript.ShowBonusesStruct)this.showBonuses[k]).bonusType] + "\n";
				Color color7;
				if (Kube.IS.bonusParams[((BattleControllerScript.ShowBonusesStruct)this.showBonuses[k]).bonusType].experience >= 0)
				{
					text2 += "+";
					color7 = new Color(0f, 1f, 0f, Mathf.Min(3f * (-Time.time + ((BattleControllerScript.ShowBonusesStruct)this.showBonuses[k]).beginShowTime + this.bonusShowTime) / this.bonusShowTime, 1f));
				}
				else
				{
					color7 = new Color(1f, 0.1f, 0.1f, Mathf.Min(3f * (-Time.time + ((BattleControllerScript.ShowBonusesStruct)this.showBonuses[k]).beginShowTime + this.bonusShowTime) / this.bonusShowTime, 1f));
				}
				text2 = text2 + string.Empty + Kube.IS.bonusParams[((BattleControllerScript.ShowBonusesStruct)this.showBonuses[k]).bonusType].experience;
				GUI.color = new Color(0f, 0f, 0f, Mathf.Min(3f * (-Time.time + ((BattleControllerScript.ShowBonusesStruct)this.showBonuses[k]).beginShowTime + this.bonusShowTime) / this.bonusShowTime, 1f));
				GUI.Label(new Rect(num5 - 2f, 0.2f * num2 + 110f + 2f, 234f, 70f), text2);
				GUI.Label(new Rect(num5 - 2f, 0.2f * num2 + 110f - 2f, 234f, 70f), text2);
				GUI.Label(new Rect(num5 + 2f, 0.2f * num2 + 110f + 2f, 234f, 70f), text2);
				GUI.Label(new Rect(num5 + 2f, 0.2f * num2 + 110f - 2f, 234f, 70f), text2);
				GUI.color = color7;
				GUI.Label(new Rect(num5, 0.2f * num2 + 110f, 234f, 70f), text2);
				GUI.color = color6;
			}
		}
	}

	private int GetNumMonstersPerWave(int waveNum)
	{
		waveNum++;
		float num = 2f;
		float num2 = 3f;
		float num3 = 8f;
		return (int)num3 + (int)((2f * num + ((float)waveNum - 2f) * num2) * ((float)waveNum - 1f) / 2f);
	}

	private int GetMaxMonstersPerWave(int waveNum)
	{
		waveNum++;
		float num = 1f;
		float num2 = 2f;
		float num3 = 3f;
		return (int)num3 + (int)((2f * num + ((float)waveNum - 2f) * num2) * ((float)waveNum - 1f) / 2f);
	}

	private void invSpeedHack()
	{
		TimeSpan timeSpan = DateTime.Now - this.olddt;
		this.olddt = DateTime.Now;
		long num = (long)Environment.TickCount - this.oldTick;
		this.oldTick = (long)Environment.TickCount;
		if (timeSpan.TotalMilliseconds * 1.2999999523162842 < (double)num)
		{
			this.errorCount++;
		}
		if (this.errorCount > 5)
		{
			Kube.GPS.printMessage(Localize.speedHackDetected, Color.red);
			Kube.SS.SendStat("speedHack");
			PhotonNetwork.LeaveRoom();
			this.LoadMainMenu();
		}
	}

	public bool IsNormPing(float ping)
	{
		return ping <= this.meanPing * 2f;
	}

	public void CollectPing(float ping)
	{
		this.numPing++;
		this.collectPing += ping;
		this.meanPing = this.collectPing / (float)this.numPing;
	}

	public void printPing()
	{
		for (int i = 0; i < 20; i++)
		{
			float num = (float)i * this.maxPing / 20f;
			float num2 = (float)(i + 1) * this.maxPing / 20f;
			string text = string.Concat(new object[]
			{
				"Ping from ",
				num,
				" to ",
				num2,
				": "
			});
			int num3 = 0;
			for (int j = 0; j < this.pingList.Count; j++)
			{
				if (this.pingList[j] >= num && this.pingList[j] <= num2)
				{
					num3++;
				}
			}
			text = text + string.Empty + num3;
			MonoBehaviour.print(text);
		}
	}

	public int FragsToExp(int frags, int expForFrag)
	{
		return -Mathf.RoundToInt((float)expForFrag * (Mathf.Pow(0.95f, (float)frags) - 1f) / 0.05f);
	}

	public void SumRoundBonuses()
	{
		if (this.sumBonusesTex == null)
		{
			this.sumBonusesTex = new List<Texture>();
		}
		this.sumBonusesTex.Clear();
		if (this.sumBonusesStr == null)
		{
			this.sumBonusesStr = new List<string>();
		}
		this.sumBonusesStr.Clear();
		if (this.sumBonusesExp == null)
		{
			this.sumBonusesExp = new List<int>();
		}
		this.sumBonusesExp.Clear();
		this.sumBonusesTex.Add(Kube.ASS2.frags);
		float num = 1f;
		if (this.gameType == GameType.survival)
		{
			num = 0.2f;
		}
		this.sumBonusesExp.Add(Mathf.RoundToInt(num * (float)this.FragsToExp(this.tempPsKills, Mathf.RoundToInt((float)this.ps.points / (float)this.tempPsKills))));
		this.sumBonusesStr.Add(string.Concat(new object[]
		{
			Localize.frags_killed,
			": ",
			this.tempPsKills,
			"\n",
			(this.sumBonusesExp[this.sumBonusesExp.Count - 1] < 0) ? string.Empty : "+",
			this.sumBonusesExp[this.sumBonusesExp.Count - 1]
		}));
		for (int i = 0; i < this.bonusesInRound.Length; i++)
		{
			if (this.bonusesInRound[i] != 0)
			{
				this.sumBonusesTex.Add(Kube.ASS2.bonusTex[i]);
				this.sumBonusesExp.Add(this.bonusesInRound[i] * Kube.IS.bonusParams[i].experience);
				string text = Kube.IS.bonusParams[i].name;
				if (Localize.bonusName.Length > i)
				{
					text = Localize.bonusName[i];
				}
				this.sumBonusesStr.Add(string.Concat(new object[]
				{
					string.Empty,
					text,
					(this.bonusesInRound[i] <= 1) ? string.Empty : (" X" + this.bonusesInRound[i]),
					"\n",
					(this.sumBonusesExp[this.sumBonusesExp.Count - 1] < 0) ? string.Empty : "+",
					this.sumBonusesExp[this.sumBonusesExp.Count - 1]
				}));
			}
		}
	}

	public void PlayerSpawned(PlayerScript playerScript)
	{
		if (this.gameTypeController)
		{
			this.gameTypeController.PlayerSpawned(playerScript);
		}
	}

	public PlayerScript FindPlayerByPV(PhotonPlayer photonPlayer)
	{
		for (int i = 0; i < this.playersInfo.Length; i++)
		{
			PlayerScript playerScript = this.playersInfo[i].ps;
			if (playerScript.photonView.owner == photonPlayer)
			{
				return playerScript;
			}
		}
		return null;
	}

	public PlayerScript FindPlayerByOnlineId(int _id_killer)
	{
		for (int i = 0; i < this.playersInfo.Length; i++)
		{
			PlayerScript playerScript = this.playersInfo[i].ps;
			if (!(playerScript == null))
			{
				if (playerScript.onlineId == _id_killer)
				{
					return playerScript;
				}
			}
		}
		return null;
	}

	public int PintsForPlayer(PlayerScript killer, int p, int level)
	{
		if (killer == null || false)
		{
			return p;
		}
		if (this.isBuiltinMap && this.gameMode == GameMode.cooperative)
		{
			return p * level;
		}
		return Mathf.Max(1, p * level - Mathf.RoundToInt((float)killer.level / 2f));
	}

	protected const int MAX_ITEMS_IN_GUIROW = 4;

	public GameObject menu;

	public UIHUD hud;

	public TeamStartMenu firstPage;

	public EndRoundMenu endRound;

	[NonSerialized]
	public NetworkObjectScript NO;

	public bool isLoadingWorldChanges = true;

	public bool canChangeWorld = true;

	public bool canUseWeapons = true;

	public long mapId;

	public int ownerId;

	public int creatorId;

	public bool showMenu;

	public PlayerScript ps;

	[NonSerialized]
	public GameMode gameMode;

	[NonSerialized]
	public GameType gameType;

	public ObjectsHolderScript.MissionType missionType;

	public bool mapCanBreak;

	public GameObject battleCamera;

	public bool newPlayersCanBuild;

	public int gameEndTime;

	public float gameStartTime;

	[NonSerialized]
	public USpeaker uSpeakerGO;

	public float survivalPrewaveTime = 15f;

	[NonSerialized]
	public float survivalTime;

	[NonSerialized]
	public float survivalBeginTime = 1E+11f;

	[NonSerialized]
	public int survivalWaveNum;

	private int survivalMaxMonsters;

	private int survivalKilledMonsters;

	private int survivalMonstersPerWave;

	private float survivalLastMonsterSpawnTime;

	public float survivalMonsterSpawnDeltaTime = 1f;

	[NonSerialized]
	public int currentNumPlayers;

	[NonSerialized]
	public int currentNumDeadPlayers;

	[NonSerialized]
	public int currentNumMonsters;

	public int adviceNum;

	private float currentGameTime;

	private int dayLightType;

	public int[] teamScore = new int[4];

	public int _missionId;

	public float FPSupdateInterval = 1f;

	private float FPSaccum;

	private int FPSframes;

	private float FPStimeleft;

	private float[] FPSarray;

	private float FPSworst = 60f;

	private int FPSnumInArray;

	public int[] bonusesInRound;

	public float bonusShowTime = 1.5f;

	private ArrayList showBonuses;

	public BattleControllerScript.BonusCounters bonusCounters;

	public bool jetPackAwail;

	public float cancelPendingTimeout = 90f;

	protected int waintingForMap;

	[HideInInspector]
	public GameTypeControllerBase gameTypeController;

	private int oldLevel;

	[NonSerialized]
	public GameObject tutorialGO;

	private float lastCountPlayersTime;

	private float countPlayersDeltaTime = 2f;

	private float dayPeriod = 140f;

	private float dayNightPerc = 0.9f;

	private float lastCheckMonstersTime;

	private float lastCheckMonstersDeltaTime = 2f;

	public GameObject[] monsters;

	private MonsterScript[] monstersScript;

	private float lastCheckTransportTime;

	private float lastCheckTransportDeltaTime = 2f;

	private GameObject[] transports;

	private TransportScript[] transportsScript;

	[HideInInspector]
	public BattleControllerScript.PlayerInfo[] playersInfo;

	[HideInInspector]
	public int[] playersInTeam;

	private GameObject[] playersRIP;

	public BattleControllerScript.GameProcess gameProcess;

	private string endGameCapture = Localize.BCS_end_round;

	public int newLevel;

	private int endGameKills;

	private int endGameMoney;

	private int endGameExp;

	[NonSerialized]
	public int endGameTime;

	private float endGameFragsPerSec;

	private int _tempPsKills;

	private int _tempPsPoints;

	private int fragsTotal;

	private int moneyTotal;

	private int expTotal;

	public EndMissionDialog finalUI;

	public EndRoundNewDialog endRoundScoresUI;

	public NewLevelDialog levelUpUI;

	public BattleControllerScript.EndGameType lastEndGameType;

	protected BattleControllerScript.MissionResult _missionResult;

	private DateTime olddt;

	private long oldTick;

	private int errorCount;

	private List<float> pingList;

	private float maxPing;

	private float collectPing;

	private int numPing;

	public float meanPing;

	public List<Texture> sumBonusesTex;

	public List<string> sumBonusesStr;

	public List<int> sumBonusesExp;

	private struct ShowBonusesStruct
	{
		public float beginShowTime;

		public int bonusType;
	}

	public struct BonusCounters
	{
		public int kills
		{
			get
			{
				return this._kills;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.kills, this.kills, value);
				this._kills = value;
			}
		}

		public int headshots
		{
			get
			{
				return this._headshots;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.headshots, this.headshots, value);
				this._headshots = value;
			}
		}

		public int saves
		{
			get
			{
				return this._saves;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.saves, this.saves, value);
				this._saves = value;
			}
		}

		public int nearFights
		{
			get
			{
				return this._nearFights;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.nearFights, this.nearFights, value);
				this._nearFights = value;
			}
		}

		public int selfKill
		{
			get
			{
				return this._selfKill;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.selfKill, this.selfKill, value);
				this._selfKill = value;
			}
		}

		public int explosions
		{
			get
			{
				return this._explosions;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.explosions, this.explosions, value);
				this._explosions = value;
			}
		}

		public int grenades
		{
			get
			{
				return this._grenades;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.grenades, this.grenades, value);
				this._grenades = value;
			}
		}

		public int winnerTeam
		{
			get
			{
				return this._winnerTeam;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.winnerTeam, this.winnerTeam, value);
				this._winnerTeam = value;
			}
		}

		public int firstPlace
		{
			get
			{
				return this._firstPlace;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.firstPlace, this.firstPlace, value);
				this._firstPlace = value;
			}
		}

		public int secondPlace
		{
			get
			{
				return this._secondPlace;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.secondPlace, this.secondPlace, value);
				this._secondPlace = value;
			}
		}

		public int thirdPlace
		{
			get
			{
				return this._thirdPlace;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.thirdPlace, this.thirdPlace, value);
				this._thirdPlace = value;
			}
		}

		public int capturedTheFlag
		{
			get
			{
				return this._capturedTheFlag;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.capturedTheFlag, this.capturedTheFlag, value);
				this._capturedTheFlag = value;
			}
		}

		public int missionComplited
		{
			get
			{
				return this._missionComplited;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.missionComplited, this.missionComplited, value);
				this._missionComplited = value;
			}
		}

		public int placedItem
		{
			get
			{
				return this._placedItem;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.placedItem, this.placedItem, value);
				this._placedItem = value;
			}
		}

		public int transportKilled
		{
			get
			{
				return this._transportKilled;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.transportKilled, this.transportKilled, value);
				this._transportKilled = value;
			}
		}

		public int survivalWave
		{
			get
			{
				return this._survivalWave;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.survivalWave, this.survivalWave, value);
				this._survivalWave = value;
			}
		}

		public int zombieKill
		{
			get
			{
				return this._zombieKill;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.zombieKill, this.zombieKill, value);
				this._zombieKill = value;
			}
		}

		public int zombieExplosion
		{
			get
			{
				return this._zombieExplosion;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.zombieExplosion, this.zombieExplosion, value);
				this._zombieExplosion = value;
			}
		}

		public int demonKilled
		{
			get
			{
				return this._demonKilled;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.demonKilled, this.demonKilled, value);
				this._demonKilled = value;
			}
		}

		public int cubesPlaced
		{
			get
			{
				return this._cubesPlaced;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.cubesPlaced, this.cubesPlaced, value);
				this._cubesPlaced = value;
			}
		}

		public int mecanismPlaced
		{
			get
			{
				return this._mecanismPlaced;
			}
			set
			{
				Kube.BCS.RecountBonuses(BonusVariableType.mecanismPlaced, this.mecanismPlaced, value);
				this._mecanismPlaced = value;
			}
		}

		public int _kills;

		public int _headshots;

		public int _saves;

		public int _nearFights;

		public int _selfKill;

		public int _explosions;

		public int _grenades;

		public int _winnerTeam;

		public int _firstPlace;

		public int _secondPlace;

		public int _thirdPlace;

		public int _capturedTheFlag;

		public int _missionComplited;

		public int _placedItem;

		public int _transportKilled;

		public int _survivalWave;

		public int _zombieKill;

		public int _zombieExplosion;

		public int _demonKilled;

		public int _cubesPlaced;

		public int _mecanismPlaced;
	}

	public struct PlayerInfo
	{
		public string Name;

		public int id;

		public int serverId;

		public int Frags;

		public int Deaths;

		public int Score;

		public bool CanBuild;

		public int Level;

		public int Team;

		public string UID;

		public string sn;

		public PlayerScript ps;
	}

	public enum EndGameType
	{
		time,
		ban,
		exit,
		netError,
		exitTrigger,
		lose,
		endRound
	}

	public enum GameProcess
	{
		start,
		game,
		end,
		exit
	}

	protected class MissionResult
	{
		public bool firstTime;

		public int endGameMoney;

		public int endGameGold;

		public KeyValuePair<int, int>[] items;
	}
}
