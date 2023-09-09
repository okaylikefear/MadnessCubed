using System;
using System.Collections;
using System.Collections.Generic;
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

	public void ChangeCanBuildStatus(int id, bool canBuild)
	{
		this.NO.ChangeCanBuildStatus(id, canBuild);
		this.RefreshPlayersTable();
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

	private void Start()
	{
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
			this.ownerId = Convert.ToInt32((double)(this.mapId - 9L) / 20.0);
		}
		else
		{
			this.ownerId = 0;
		}
		this.newPlayersCanBuild = false;
		this.gameType = Kube.OH.tempMap.GameType;
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
			this.EndGame(BattleControllerScript.EndGameType.netError, true);
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
		this.isLoadingWorldChanges = true;
		this.NO.LoadMap();
	}

	public PlayerScript CreatePlayer(Vector3 respawnPlace, Quaternion rot)
	{
		PlayerScript component = PhotonNetwork.Instantiate("characterType3", respawnPlace, Quaternion.identity, 0).GetComponent<PlayerScript>();
		component.Init();
		return component;
	}

	private void BeginGame()
	{
		base.CancelInvoke("CancelPending");
		this.menu.SetActive(false);
		this.hud.Init();
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
		if (this.gameType == GameType.creating || this.gameType == GameType.shooter || this.gameType == GameType.test || this.gameType == GameType.survival)
		{
			this.battleCamera.SetActive(false);
			Vector3 position = new Vector3(1f, 40f, 1f);
			for (int i = 0; i < Kube.WHS.sizeY; i++)
			{
				if (Kube.WHS.cubes[Kube.WHS.sizeX / 2, i, Kube.WHS.sizeZ / 2].type == 0)
				{
					position = new Vector3((float)Kube.WHS.sizeX / 2f, (float)i, (float)Kube.WHS.sizeZ / 2f);
					break;
				}
			}
			GameObject[] array = GameObject.FindGameObjectsWithTag("Respawn");
			if (array.Length != 0)
			{
				position = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
			}
			this.ps = this.CreatePlayer(position, Quaternion.identity);
			Kube.IS.ps = this.ps;
			if (this.gameType == GameType.creating)
			{
				Kube.IS.ShowFastPanel(true);
			}
			Screen.lockCursor = true;
			this.gameProcess = BattleControllerScript.GameProcess.game;
			this.survivalBeginTime = Time.realtimeSinceStartup;
			this.survivalMonstersPerWave = -1;
			if (this.dayLightType == 0 || this.dayLightType == 1)
			{
				Kube.WHS.SetDayLight(1f);
			}
			if (this.dayLightType == 2)
			{
				Kube.WHS.SetDayLight(0f);
			}
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
		else if (this.gameType == GameType.dominating)
		{
			this.gameTypeController = base.gameObject.AddComponent<DominatingController>();
			this.gameTypeController.Initialize();
		}
		else if (this.gameType == GameType.mission)
		{
			this.battleCamera.SetActive(false);
			Vector3 position2 = new Vector3(1f, 40f, 1f);
			for (int j = 0; j < Kube.WHS.sizeY; j++)
			{
				if (Kube.WHS.cubes[Kube.WHS.sizeX / 2, j, Kube.WHS.sizeZ / 2].type == 0)
				{
					position2 = new Vector3((float)Kube.WHS.sizeX / 2f, (float)j, (float)Kube.WHS.sizeZ / 2f);
					break;
				}
			}
			GameObject[] array2 = GameObject.FindGameObjectsWithTag("Respawn");
			if (array2.Length != 0)
			{
				position2 = array2[UnityEngine.Random.Range(0, array2.Length)].transform.position;
			}
			this.ps = Kube.BCS.CreatePlayer(position2, Quaternion.identity);
			Kube.IS.ps = this.ps;
			Screen.lockCursor = true;
			this.gameProcess = BattleControllerScript.GameProcess.game;
			this.survivalBeginTime = Time.realtimeSinceStartup;
			this.survivalMonstersPerWave = -1;
			if (this.dayLightType == 0 || this.dayLightType == 1)
			{
				Kube.WHS.SetDayLight(1f);
			}
			if (this.dayLightType == 2)
			{
				Kube.WHS.SetDayLight(0f);
			}
			this.missionType = Kube.OH.tempMap.missionType;
			if (Kube.OH.tempMap.missionType == ObjectsHolderScript.MissionType.reachTheExit)
			{
				this.gameTypeController = base.gameObject.AddComponent<MissionReachTheExit>();
			}
			else if (Kube.OH.tempMap.missionType == ObjectsHolderScript.MissionType.killNMonsters)
			{
				this.gameTypeController = base.gameObject.AddComponent<MissionKillNMonsters>();
			}
			else if (Kube.OH.tempMap.missionType == ObjectsHolderScript.MissionType.holdNSeconds)
			{
				this.gameTypeController = base.gameObject.AddComponent<MissionHoldNSecond>();
			}
			else if (Kube.OH.tempMap.missionType == ObjectsHolderScript.MissionType.findNitems)
			{
				this.gameTypeController = base.gameObject.AddComponent<MissionFindNItems>();
			}
			else if (Kube.OH.tempMap.missionType == ObjectsHolderScript.MissionType.findNitemsInMSeconds)
			{
				this.gameTypeController = base.gameObject.AddComponent<MissionFindItemsInTime>();
			}
			else if (Kube.OH.tempMap.missionType == ObjectsHolderScript.MissionType.killNMonstersInMSeconds)
			{
				this.gameTypeController = base.gameObject.AddComponent<MissionKillMonstersInTime>();
			}
			this.gameTypeController.configure(Kube.OH.tempMap.missionConfig);
		}
		this.NO.SendMeGameParams((int)this.gameType);
		if (this.gameType == GameType.dominating || this.gameType == GameType.teams || this.gameType == GameType.captureTheFlag)
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

	private void StartTutorial()
	{
		GameObject gameObject = (GameObject)Resources.Load("TutorialGO");
		gameObject = (GameObject)UnityEngine.Object.Instantiate(gameObject);
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
			return this.ownerId == Kube.GPS.playerId || (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient && this.ownerId == 0);
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
			this.ps.ChangeWeapon(-1);
		}
	}

	private void MonsterRespawnTick()
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

	private void TransportRespawnTick()
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
								if (transportScript.transportId == j)
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

	private void Update()
	{
		if (this.isLoadingWorldChanges)
		{
			if (this.NO.worldChangesLoaded)
			{
				this.isLoadingWorldChanges = false;
				this.BeginGame();
			}
		}
		else
		{
			this.currentGameTime = Time.realtimeSinceStartup - this.gameStartTime;
			if (this.dayLightType == 1)
			{
				int num = Mathf.FloorToInt(this.currentGameTime / this.dayPeriod) % 2;
				float num2 = this.currentGameTime - Mathf.Floor(this.currentGameTime / this.dayPeriod) * this.dayPeriod;
				if (num2 > this.dayPeriod * this.dayNightPerc)
				{
					float dayLight;
					if (num == 0)
					{
						dayLight = (this.dayPeriod - num2) / ((1f - this.dayNightPerc) * this.dayPeriod);
					}
					else
					{
						dayLight = 1f - (this.dayPeriod - num2) / ((1f - this.dayNightPerc) * this.dayPeriod);
					}
					Kube.WHS.SetDayLight(dayLight);
				}
				else if (num == 0)
				{
					Kube.WHS.SetDayLight(1f);
				}
				else
				{
					Kube.WHS.SetDayLight(0f);
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
					this.tempPsKills = this.ps.kills;
					this.tempPsPoints = this.ps.points;
				}
				if (PhotonNetwork.isMasterClient)
				{
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
			Screen.lockCursor = !flag;
			if (this.ps != null && this.gameProcess == BattleControllerScript.GameProcess.game)
			{
				this.ps.paused = Kube.OH.isMenu;
			}
			bool flag2 = UnityEngine.Input.GetKeyDown(KeyCode.Escape) || UnityEngine.Input.GetKeyDown(KeyCode.BackQuote);
			if (UnityEngine.Input.GetKeyDown(KeyCode.T) && Application.isEditor && this.gameProcess == BattleControllerScript.GameProcess.game)
			{
				for (int j = 0; j < Kube.BCS.bonusesInRound.Length; j++)
				{
					int num3 = Mathf.RoundToInt((float)Mathf.Max(UnityEngine.Random.Range(-5, 5), 0));
					Kube.BCS.bonusesInRound[j] = num3;
				}
				Kube.BCS.gameEndTime = Mathf.RoundToInt(Time.realtimeSinceStartup);
			}
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
				this.EndGame(BattleControllerScript.EndGameType.netError, true);
			}
			if (this.gameType == GameType.teams && this.gameProcess == BattleControllerScript.GameProcess.game)
			{
				this.MonsterRespawnTick();
				this.TransportRespawnTick();
			}
			if (this.gameType == GameType.survival && this.gameProcess == BattleControllerScript.GameProcess.game)
			{
				this.survivalTime = Time.realtimeSinceStartup - this.survivalBeginTime;
				float num4 = 1f;
				for (int k = 1; k < this.currentNumPlayers; k++)
				{
					num4 += Mathf.Pow(0.7f, (float)k);
				}
				if (this.survivalTime > this.survivalPrewaveTime && this.survivalMonstersPerWave == -1)
				{
					this.survivalMonstersPerWave = this.GetNumMonstersPerWave(this.survivalWaveNum);
					this.survivalMaxMonsters = this.GetMaxMonstersPerWave(this.survivalWaveNum);
					this.survivalKilledMonsters = 0;
				}
				else if (this.survivalKilledMonsters + 4 >= (int)((float)this.survivalMonstersPerWave * num4) && this.currentNumMonsters <= 4 && this.survivalMonstersPerWave > 0)
				{
					if (PhotonNetwork.isMasterClient || PhotonNetwork.offlineMode)
					{
						this.NO.SurvivalStartNewWave(this.survivalWaveNum + 1);
					}
				}
				else
				{
					if (Time.time - this.survivalLastMonsterSpawnTime > this.survivalMonsterSpawnDeltaTime && this.currentNumMonsters < (int)((float)this.survivalMaxMonsters * num4) && this.survivalKilledMonsters < (int)((float)this.survivalMonstersPerWave * num4) && this.currentNumMonsters < 60)
					{
						float num5 = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2f + 2f);
						float num6 = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2.1f - 2f);
						float num7 = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2.2f - 4f);
						float num8 = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2.3f - 3f);
						float num9 = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2.4f - 5f);
						float num10 = UnityEngine.Random.Range(0f, (float)this.survivalWaveNum * 2.5f - 7f);
						float num11 = Mathf.Max(new float[]
						{
							num5,
							num6,
							num7,
							num8,
							num10,
							num9
						});
						string prefabName = string.Empty;
						if (num11 == num5)
						{
							prefabName = "Zombie";
						}
						if (num11 == num6)
						{
							prefabName = "Agent";
						}
						if (num11 == num7)
						{
							prefabName = "Soldat";
						}
						if (num11 == num8)
						{
							prefabName = "ZombieAxes";
						}
						if (num11 == num9)
						{
							prefabName = "ZombieSaw";
						}
						if (num11 == num10)
						{
							prefabName = "Demon";
						}
						GameObject[] array = GameObject.FindGameObjectsWithTag("RespawnHumanoid");
						int num12 = UnityEngine.Random.Range(0, array.Length);
						if (array.Length != 0)
						{
							PhotonNetwork.Instantiate(prefabName, array[num12].transform.position, Quaternion.identity, 0).SendMessage("SetAngry", true);
						}
						this.survivalLastMonsterSpawnTime = Time.time;
					}
					this.TransportRespawnTick();
				}
				if (this.currentNumDeadPlayers == this.currentNumPlayers && this.currentNumMonsters != 0)
				{
					this.EndGame(BattleControllerScript.EndGameType.exit, true);
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
				float num13 = this.FPSaccum / (float)this.FPSframes;
				this.FPSarray[this.FPSnumInArray] = num13;
				this.FPSnumInArray++;
				if (this.FPSnumInArray >= this.FPSarray.Length)
				{
					this.FPSnumInArray = 0;
				}
				float num14 = 0f;
				for (int l = 0; l < this.FPSarray.Length; l++)
				{
					num14 += this.FPSarray[l];
				}
				num14 /= (float)this.FPSarray.Length;
				if (this.FPSworst > num14)
				{
					this.FPSworst = num14;
				}
				this.FPStimeleft = this.FPSupdateInterval;
				this.FPSframes = 0;
				this.FPSaccum = 0f;
			}
			for (int m = 0; m < this.showBonuses.Count; m++)
			{
				if (((BattleControllerScript.ShowBonusesStruct)this.showBonuses[m]).beginShowTime + this.bonusShowTime < Time.time)
				{
					this.showBonuses.RemoveAt(m);
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

	private void RefreshPlayersTable()
	{
		this.players = GameObject.FindGameObjectsWithTag("Player");
		this.currentNumPlayers = this.players.Length;
		this.currentNumDeadPlayers = 0;
		for (int i = 0; i < this.players.Length; i++)
		{
			PlayerScript component = this.players[i].GetComponent<PlayerScript>();
			if (component && component.dead)
			{
				this.currentNumDeadPlayers++;
			}
		}
		this.playersInfo = new BattleControllerScript.PlayerInfo[this.players.Length];
		this.playersInTeam = new int[8];
		this.playersFragsOrder = new int[this.players.Length];
		for (int j = 0; j < this.players.Length; j++)
		{
			this.playersFragsOrder[j] = j;
		}
		for (int k = 0; k < this.playersInTeam.Length; k++)
		{
			this.playersInTeam[k] = 0;
		}
		for (int l = 0; l < this.players.Length; l++)
		{
			PlayerScript component2 = this.players[l].GetComponent<PlayerScript>();
			if (component2)
			{
				this.playersInfo[l].Name = component2.playerName;
				this.playersInfo[l].Id = component2.id;
				this.playersInfo[l].Frags = component2.kills;
				this.playersInfo[l].Deaths = component2.deadTimes;
				this.playersInfo[l].CanBuild = component2.canBuild;
				this.playersInfo[l].Level = component2.level;
				this.playersInfo[l].Score = 0;
				this.playersInfo[l].Team = component2.team;
				this.playersInfo[l].UID = component2.uid;
				for (int m = 0; m < l; m++)
				{
					if (component2.id == this.playersInfo[m].Id)
					{
						this.NO.BanPlayer(component2.id);
						break;
					}
				}
				if (component2.team >= 0)
				{
					this.playersInTeam[component2.team]++;
				}
			}
		}
		int[] array = new int[this.players.Length];
		for (int n = 0; n < this.players.Length; n++)
		{
			array[n] = this.playersInfo[n].Frags;
		}
		Array.Sort<int, int>(array, this.playersFragsOrder);
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
			if (this.ownerId == Kube.GPS.playerId)
			{
				if (Kube.OH.tempMap.CreatedGame)
				{
					Kube.SS.SaveMap(this.mapId, Kube.WHS.SaveWorld(), go, message);
					if (Kube.GPS.needTrainingBuild)
					{
						Kube.TS.SendMessage("MapSaved");
					}
					this.ps.SendMessage("SetAllTokenItems");
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

	public void ImIn(int id)
	{
		if ((this.gameType == GameType.creating || this.gameType == GameType.test) && id != Kube.GPS.playerId && Kube.GPS.playerId == this.ownerId)
		{
			this.NO.ChangeCanBuildStatus(id, this.newPlayersCanBuild);
		}
		if (this.gameType == GameType.survival && PhotonNetwork.isMasterClient)
		{
			this.NO.SendSurvivalParams(this.survivalTime, this.survivalWaveNum, this.survivalKilledMonsters);
		}
		if (this.gameType == GameType.mission && PhotonNetwork.isMasterClient)
		{
			this.NO.SendMissionParams(Time.realtimeSinceStartup - this.gameStartTime);
		}
		if (this.gameType == GameType.shooter && PhotonNetwork.isMasterClient)
		{
			this.NO.SendMissionParams(Time.realtimeSinceStartup - this.gameStartTime);
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
		this.SumRoundBonuses();
		this.endGameExp = 0;
		for (int i = 0; i < this.sumBonusesExp.Count; i++)
		{
			this.endGameExp += this.sumBonusesExp[i];
		}
		this.endGameKills = this.tempPsKills;
		if (Kube.GPS.vipEnd - Time.time > 0f)
		{
			this.endGameExp += (int)((float)this.endGameExp * (float)Kube.GPS.vipBonus / 100f);
		}
		this.newLevel = Kube.OH.GetLevel((int)((float)Kube.GPS.playerExp + (float)this.endGameExp));
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
		return new EndGameStats(Kube.GPS.playerExp, this.endGameExp, Kube.GPS.playerFrags, this.tempPsKills, Kube.GPS.playerMoney1, this.endGameMoney, Kube.GPS.playerLevel, this.newLevel, this.bonusesInRound);
	}

	public void SendGameResultToServer()
	{
		Kube.SS.SendEndLevel(this.CalcGameStats(), Kube.OH.gameObject, "SendLevelDoneDone");
	}

	public void EndGame(BattleControllerScript.EndGameType endGameType, bool sendParams = true)
	{
		if (this.gameProcess == BattleControllerScript.GameProcess.exit || this.gameProcess == BattleControllerScript.GameProcess.end)
		{
			return;
		}
		this.lastEndGameType = endGameType;
		Screen.lockCursor = false;
		this.gameProcess = BattleControllerScript.GameProcess.end;
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
			if (this.gameType != GameType.mission)
			{
				if (sendParams)
				{
					this.SendGameResultToServer();
				}
			}
			this.endRoundScoresUI.Open(endGameStats, this.endGameTime, this.endGameCapture);
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
			Screen.lockCursor = false;
			this.battleCamera.SetActive(true);
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
		this.EndGame(BattleControllerScript.EndGameType.exit, true);
	}

	private void LoadMainMenu()
	{
		PhotonNetwork.LeaveRoom();
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
	}

	private void ingameGUI()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		GUI.skin = Kube.ASS1.blueButtonSkin;
		if (this.uSpeakerGO != null)
		{
			if (GUI.Button(new Rect(0.5f * num + 175f, 0.5f * num2 - 200f, 200f, 30f), Localize.BCS_disable_miscrophone))
			{
				PhotonNetwork.Destroy(this.uSpeakerGO);
			}
			GUI.Box(new Rect(0.5f * num + 150f, 0.5f * num2 - 170f, 250f, 60f), Localize.BCS_to_speak_press);
		}
		if (this.uSpeakerGO == null && Kube.GPS.vipEnd - Time.time > 0f)
		{
			if (GUI.Button(new Rect(0.5f * num + 175f, 0.5f * num2 - 200f, 200f, 30f), Localize.BCS_enable_microphone))
			{
				this.uSpeakerGO = PhotonNetwork.Instantiate(this.uSpeakerPrefab, Vector3.zero, Quaternion.identity, 0);
				this.uSpeakerGO.transform.parent = this.ps.transform;
				this.uSpeakerGO.transform.localPosition = Vector3.zero;
			}
			GUI.Box(new Rect(0.5f * num + 150f, 0.5f * num2 - 170f, 250f, 60f), Localize.BCS_then_press_allow);
		}
		else if (Kube.GPS.vipEnd - Time.time <= 0f && GUI.Button(new Rect(0.5f * num + 175f, 0.5f * num2 - 200f, 200f, 80f), Localize.BCS_get_VIP_to_speak))
		{
			Kube.IS.SendMessage("ToggleInventarVIP");
		}
		if (this.gameType == GameType.creating)
		{
			GUI.skin = Kube.ASS1.blueButtonSkin;
			if (this.isMapOwner && GUI.Button(new Rect(30f, 70f, 200f, 60f), Localize.BCS_start_test))
			{
				this.StartTestMission();
			}
			if (GUI.Button(new Rect(0.5f * num - 140f, 0.5f * num2 - 260f, 280f, 30f), Localize.BCS_resume_game))
			{
				Kube.OH.closeMenu(null);
			}
			if (GUI.Button(new Rect(0.5f * num - 140f, 0.5f * num2 - 220f, 280f, 30f), Localize.BCS_save_map))
			{
				this.SaveMap();
			}
			if (GUI.Button(new Rect(0.5f * num - 140f, 0.5f * num2 - 180f, 280f, 30f), Localize.BCS_inventory_shop))
			{
				this.showMenu = !this.showMenu;
				this.ps.paused = this.showMenu;
				GameObject.FindGameObjectWithTag("ObjectsHolder").SendMessage("ToggleInventar");
			}
			if (GUI.Button(new Rect(0.5f * num - 140f, 0.5f * num2 - 140f, 280f, 30f), Localize.BCS_exit_from_game))
			{
				Kube.OH.closeMenu(null);
				if (this.ownerId != Kube.GPS.playerId)
				{
					this.EndGame(BattleControllerScript.EndGameType.exit, true);
				}
			}
			GUI.skin = Kube.ASS1.mainSkinSmall;
			GUI.DrawTexture(new Rect(0.5f * num - 300f, 0.5f * num2 - 100f, 600f, 41f + 25f * (float)this.playersInfo.Length), Kube.ASS1.tabTex);
			if (this.ownerId == Kube.IS.ps.id)
			{
				string str = (!this.newPlayersCanBuild) ? Localize.BCS_notallowed : Localize.BCS_allowed;
				GUI.skin = Kube.ASS1.buttonArrowSkin;
				if (GUI.Button(new Rect(0.5f * num - 200f, 0.5f * num2 - 100f, 400f, 30f), Localize.BCS_noobs_to_build + " " + str))
				{
					this.newPlayersCanBuild = !this.newPlayersCanBuild;
				}
			}
			for (int i = 0; i < this.playersInfo.Length; i++)
			{
				GUI.skin = Kube.ASS1.mainSkinSmall;
				GUI.DrawTexture(new Rect(0.5f * num - 300f, 0.5f * num2 - 65f + 25f * (float)i, 32f, 32f), Kube.ASS2.RankTex[Mathf.Min(this.playersInfo[i].Level, Kube.ASS2.RankTex.Length - 1)].mainTexture);
				if (GUI.Button(new Rect(0.5f * num - 265f, 0.5f * num2 - 65f + 25f * (float)i, 200f, 28f), AuxFunc.DecodeRussianName(this.playersInfo[i].Name)))
				{
					Kube.SN.gotoUserByUID(this.playersInfo[i].UID);
				}
				if ((this.ownerId == Kube.IS.ps.id || this.creatorId == Kube.IS.ps.id) && Kube.IS.ps.id != this.playersInfo[i].Id)
				{
					if (this.playersInfo[i].CanBuild)
					{
						if (GUI.Button(new Rect(0.5f * num - 90f, 0.5f * num2 - 65f + 25f * (float)i, 180f, 28f), Localize.BCS_forbidBuild))
						{
							this.NO.ChangeCanBuildStatus(this.playersInfo[i].Id, false);
							this.RefreshPlayersTable();
						}
					}
					else if (GUI.Button(new Rect(0.5f * num - 90f, 0.5f * num2 - 65f + 25f * (float)i, 180f, 28f), Localize.BCS_allowBuild))
					{
						this.NO.ChangeCanBuildStatus(this.playersInfo[i].Id, true);
						this.RefreshPlayersTable();
					}
					if (GUI.Button(new Rect(0.5f * num + 90f, 0.5f * num2 - 65f + 25f * (float)i, 50f, 28f), Localize.BCS_ban))
					{
						this.NO.BanPlayer(this.playersInfo[i].Id);
						this.RefreshPlayersTable();
					}
				}
			}
		}
		if (this.gameType == GameType.mission || this.gameType == GameType.shooter || this.gameType == GameType.survival || this.gameType == GameType.teams || this.gameType == GameType.test || this.gameType == GameType.captureTheFlag || this.gameType == GameType.dominating)
		{
			GUI.skin = Kube.ASS1.blueButtonSkin;
			if (this.gameType == GameType.test && GUI.Button(new Rect(30f, 130f, 200f, 60f), Localize.BCS_end_test))
			{
				this.EndTestMission();
			}
			if (GUI.Button(new Rect(0.5f * num - 140f, 0.5f * num2 - 220f, 280f, 30f), Localize.BCS_resume_game))
			{
				Kube.OH.closeMenu(null);
			}
			if (GUI.Button(new Rect(0.5f * num - 140f, 0.5f * num2 - 180f, 280f, 30f), Localize.BCS_inventory_shop))
			{
				this.showMenu = !this.showMenu;
				this.ps.paused = this.showMenu;
				GameObject.FindGameObjectWithTag("ObjectsHolder").SendMessage("ToggleInventar");
			}
			if (GUI.Button(new Rect(0.5f * num - 140f, 0.5f * num2 - 140f, 280f, 30f), Localize.BCS_exit_from_game))
			{
				Kube.OH.closeMenu(new DrawCall(this.ingameGUI));
				this.EndGame(BattleControllerScript.EndGameType.exit, true);
			}
		}
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

	protected const int MAX_ITEMS_IN_GUIROW = 4;

	public GameObject menu;

	public UIHUD hud;

	public TeamStartMenu firstPage;

	public EndRoundMenu endRound;

	[NonSerialized]
	public NetworkObjectScript NO;

	private bool isLoadingWorldChanges;

	public bool canChangeWorld = true;

	public bool canUseWeapons = true;

	public long mapId;

	public int ownerId;

	public int creatorId;

	public bool showMenu;

	public PlayerScript ps;

	public GameType gameType;

	public ObjectsHolderScript.MissionType missionType;

	public bool mapCanBreak;

	public GameObject battleCamera;

	public bool newPlayersCanBuild;

	public int gameEndTime;

	public float gameStartTime;

	public string uSpeakerPrefab;

	public GameObject uSpeakerGO;

	public Texture uSpeakerTex;

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

	public float cancelPendingTimeout = 90f;

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

	public GameObject[] players;

	[HideInInspector]
	public BattleControllerScript.PlayerInfo[] playersInfo;

	[HideInInspector]
	public int[] playersInTeam;

	[HideInInspector]
	public int[] playersFragsOrder;

	private GameObject[] playersRIP;

	public BattleControllerScript.GameProcess gameProcess;

	private string endGameCapture = Localize.BCS_end_round;

	public int newLevel;

	private int endGameKills;

	private int endGameExp;

	private int endGameMoney;

	private int endGameTime;

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

		public int Id;

		public int Frags;

		public int Deaths;

		public int Score;

		public bool CanBuild;

		public int Level;

		public int Team;

		public string UID;
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
