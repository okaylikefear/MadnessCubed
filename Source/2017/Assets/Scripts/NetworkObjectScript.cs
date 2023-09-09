using System;
using System.Collections.Generic;
using kube;
using LitJson;
using Photon;
using UnityEngine;

public class NetworkObjectScript : Photon.MonoBehaviour
{
	public bool worldChangesLoaded
	{
		get
		{
			return this.worldChangesLoaded;
		}
	}

	private void Start()
	{
	}

	public void EnterGame()
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_EnterGame", PhotonTargets.Others, new object[0]);
		}
	}

	[PunRPC]
	private void _EnterGame(PhotonMessageInfo info)
	{
		Kube.BCS.gameTypeController.EnterGame();
	}

	private void Update()
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		if (!PhotonNetwork.connected && !PhotonNetwork.offlineMode)
		{
			return;
		}
	}

	public void ChangeDominatingPointState(int pointId, int newTeam)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ChangeDominatingPointState", PhotonTargets.All, new object[]
			{
				pointId,
				newTeam
			});
		}
		else
		{
			this._ChangeDominatingPointState(pointId, newTeam, null);
		}
	}

	[PunRPC]
	private void _ChangeDominatingPointState(int pointId, int newTeam, PhotonMessageInfo info)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("DominatingPoint");
		DominatingController dominatingController = Kube.BCS.gameTypeController as DominatingController;
		for (int i = 0; i < array.Length; i++)
		{
			ItemPropsScript component = array[i].transform.root.gameObject.GetComponent<ItemPropsScript>();
			if (component.id == pointId)
			{
				DominatingPointScript component2 = array[i].GetComponent<DominatingPointScript>();
				component2.ChangeTeam(newTeam);
				dominatingController.ChangeDominatingPointState(component2, newTeam);
			}
		}
	}

	public void ChangeFlagState(int team, FlagState flagState, int playerId)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ChangeFlagState", PhotonTargets.All, new object[]
			{
				team,
				(int)flagState,
				playerId
			});
		}
		else
		{
			this._ChangeFlagState(team, (int)flagState, playerId, null);
		}
	}

	[PunRPC]
	private void _ChangeFlagState(int team, int flagState, int playerId, PhotonMessageInfo info)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Flag");
		for (int i = 0; i < array.Length; i++)
		{
			FlagScript component = array[i].GetComponent<FlagScript>();
			component.ChangeFlagState(team, flagState, playerId);
		}
	}

	public void FlagCaptured(int playerId, int team, int loseTeam)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_FlagCaptured", PhotonTargets.All, new object[]
			{
				playerId,
				team,
				loseTeam
			});
		}
		else
		{
			this._FlagCaptured(playerId, team, loseTeam, null);
		}
	}

	[PunRPC]
	private void _FlagCaptured(int playerId, int team, int loseTeam, PhotonMessageInfo info)
	{
		if (Kube.BCS.gameTypeController != null)
		{
			((CaptureTheFlagController)Kube.BCS.gameTypeController).FlagCaptured(playerId, team, loseTeam);
		}
	}

	public void RequestToRespawnMonster(int id)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		int num = UnityEngine.Random.Range(0, array.Length);
		PlayerScript component = array[num].GetComponent<PlayerScript>();
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_OrderToCreateMonster", component.photonView.owner, new object[]
			{
				id,
				component.onlineId
			});
		}
		else
		{
			this._OrderToCreateMonster(id, component.onlineId, null);
		}
	}

	[PunRPC]
	private void _OrderToCreateMonster(int id, int playerId, PhotonMessageInfo info)
	{
		Kube.GPS.printLog("_OrderToCreateMonster id=" + id);
		Kube.WHS.monsterRespawnS[id].monsterLastDieTime = Time.time + 999999f;
		if (Kube.BCS.onlineId == playerId)
		{
			Kube.WHS.monsterRespawnS[id].OrderToCreateMonster();
		}
	}

	[PunRPC]
	private void _MonsterAlifeYet(int id, PhotonMessageInfo info)
	{
		Kube.GPS.printLog("_MonsterAlifeYet id=" + id);
		Kube.WHS.monsterRespawnS[id].monsterLastDieTime = Time.time + 999999f;
	}

	public void MonsterDead(int id)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_MonsterDead", PhotonTargets.All, new object[]
			{
				id
			});
		}
		else
		{
			this._MonsterDead(id, null);
		}
	}

	[PunRPC]
	private void _MonsterDead(int id, PhotonMessageInfo info)
	{
		Kube.GPS.printLog("_MonsterDead id=" + id);
		Kube.WHS.monsterRespawnS[id].monsterLastDieTime = Time.time;
	}

	public void SummonMonster(Vector3 pos, string monsterName)
	{
		PhotonNetwork.Instantiate(monsterName, pos, Quaternion.identity, 0);
	}

	public void RequestToRespawnTransport(int id)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_RequestToRespawnTransport", PhotonTargets.All, new object[]
			{
				id
			});
		}
		else
		{
			this._RequestToRespawnTransport(id, null);
		}
	}

	[PunRPC]
	private void _RequestToRespawnTransport(int id, PhotonMessageInfo info)
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (!Kube.WHS.transportRespawnS[id])
			{
				return;
			}
			if (Time.time - Kube.WHS.transportLastDieTime[id] < (float)Kube.WHS.transportRespawnS[id].secToRespawn[Kube.WHS.transportRespawnS[id].respawnTime])
			{
				if (PhotonNetwork.room != null)
				{
					base.photonView.RPC("_TransportAlifeYet", PhotonTargets.All, new object[]
					{
						id
					});
				}
				else
				{
					this._TransportAlifeYet(id, null);
				}
			}
			else
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
				int num = UnityEngine.Random.Range(0, array.Length);
				PlayerScript component = array[num].GetComponent<PlayerScript>();
				this._OrderToCreateTransport(id, component.onlineId, null);
			}
		}
	}

	private void _OrderToCreateTransport(int id, int playerId, PhotonMessageInfo info)
	{
		Kube.GPS.printLog("_OrderToCreateTransport id=" + id);
		Kube.WHS.transportLastDieTime[id] = Time.time + 999999f;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Transport");
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].GetComponent<SyncObjectScript>().objectId == id)
			{
				return;
			}
		}
		if (Kube.BCS.onlineId == playerId)
		{
			string text = Kube.WHS.transportRespawnS[id].transportPrefabName;
			if (string.IsNullOrEmpty(text))
			{
				text = Kube.OH.transportPrefabName[Kube.WHS.transportRespawnS[id].type];
			}
			GameObject gameObject = PhotonNetwork.InstantiateSceneObject(text, Kube.WHS.transportRespawnS[id].transform.position, Kube.WHS.transportRespawnS[id].transform.rotation, 0, null);
			gameObject.SendMessage("SetRespawnNum", id);
			gameObject.SendMessage("SetHealthMultiplier", Kube.WHS.transportRespawnS[id].healthMultiplier);
			gameObject.SendMessage("SetDamageMultiplier", Kube.WHS.transportRespawnS[id].damageMultiplier);
		}
	}

	[PunRPC]
	private void _TransportAlifeYet(int id, PhotonMessageInfo info)
	{
		Kube.GPS.printLog("_TransportAlifeYet id=" + id);
		Kube.WHS.transportLastDieTime[id] = Time.time + 999999f;
	}

	public void TransportDead(int id)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_TransportDead", PhotonTargets.All, new object[]
			{
				id
			});
		}
		else
		{
			this._TransportDead(id, null);
		}
	}

	[PunRPC]
	private void _TransportDead(int id, PhotonMessageInfo info)
	{
		Kube.GPS.printLog("_TransportDead id=" + id);
		Kube.WHS.transportLastDieTime[id] = Time.time;
	}

	public void ToggleTestMission(bool b)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ToggleTestMission", PhotonTargets.All, new object[]
			{
				b
			});
		}
		else
		{
			this._ToggleTestMission(b, null);
		}
	}

	[PunRPC]
	private void _ToggleTestMission(bool b, PhotonMessageInfo info)
	{
		if (b)
		{
			Kube.BCS.DoStartTestMission();
		}
		else
		{
			Kube.BCS.DoEndTestMission();
		}
	}

	public void SaveTrigger(int x, int y, int z, int type, int state, int delayTime, int condActivate, int condKey, int id)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SaveTrigger", PhotonTargets.All, new object[]
			{
				x,
				y,
				z,
				type,
				state,
				delayTime,
				condActivate,
				condKey,
				id
			});
		}
		else
		{
			this._SaveTrigger(x, y, z, type, state, delayTime, condActivate, condKey, id, null);
		}
	}

	[PunRPC]
	private void _SaveTrigger(int x, int y, int z, int type, int state, int delayTime, int condActivate, int condKey, int id, PhotonMessageInfo info)
	{
		Kube.WHS.SaveTrigger(x, y, z, type, state, delayTime, condActivate, condKey, id);
	}

	public void MoveItem(int id, Vector3 newPos)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_MoveItem", PhotonTargets.All, new object[]
			{
				id,
				newPos
			});
		}
		else
		{
			this._MoveItem(id, newPos, null);
		}
	}

	[PunRPC]
	private void _MoveItem(int id, Vector3 newPos, PhotonMessageInfo info)
	{
		Kube.WHS.MoveItem(id, newPos);
	}

	public void SaveMonsterRespawn(int x, int y, int z, int type, int state, int respawnTime, int healthMultiplier, int damageMultiplier, int id)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SaveMonsterRespawn", PhotonTargets.All, new object[]
			{
				x,
				y,
				z,
				type,
				state,
				respawnTime,
				healthMultiplier,
				damageMultiplier,
				id
			});
		}
		else
		{
			this._SaveMonsterRespawn(x, y, z, type, state, respawnTime, healthMultiplier, damageMultiplier, id, null);
		}
	}

	[PunRPC]
	private void _SaveMonsterRespawn(int x, int y, int z, int type, int state, int respawnTime, int healthMultiplier, int damageMultiplier, int id, PhotonMessageInfo info)
	{
		Kube.WHS.SaveMonsterRespawn(x, y, z, type, state, respawnTime, healthMultiplier, damageMultiplier, id);
	}

	public void SaveTransportRespawn(int x, int y, int z, int type, int state, int respawnTime, int healthMultiplier, int damageMultiplier, int id)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SaveTransportRespawn", PhotonTargets.All, new object[]
			{
				x,
				y,
				z,
				type,
				state,
				respawnTime,
				healthMultiplier,
				damageMultiplier,
				id
			});
		}
		else
		{
			this._SaveTransportRespawn(x, y, z, type, state, respawnTime, healthMultiplier, damageMultiplier, id, null);
		}
	}

	[PunRPC]
	private void _SaveTransportRespawn(int x, int y, int z, int type, int state, int respawnTime, int healthMultiplier, int damageMultiplier, int id, PhotonMessageInfo info)
	{
		Kube.WHS.SaveTransportRespawn(x, y, z, type, state, respawnTime, healthMultiplier, damageMultiplier, id);
	}

	public void CreateNewAA(int x1, int y1, int z1, int x2, int y2, int z2, int type, int materialType, int status, int coordState, int soundType, int prop1, int prop2, int prop3, int id, int idPlayer)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_CreateNewAA", PhotonTargets.All, new object[]
			{
				x1,
				y1,
				z1,
				x2,
				y2,
				z2,
				type,
				materialType,
				status,
				coordState,
				soundType,
				prop1,
				prop2,
				prop3,
				id,
				idPlayer
			});
		}
		else
		{
			this._CreateNewAA(x1, y1, z1, x2, y2, z2, type, materialType, status, coordState, soundType, prop1, prop2, prop3, id, idPlayer, null);
		}
	}

	[PunRPC]
	private void _CreateNewAA(int x1, int y1, int z1, int x2, int y2, int z2, int type, int materialType, int status, int coordState, int soundType, int prop1, int prop2, int prop3, int id, int idPlayer, PhotonMessageInfo info)
	{
		Kube.WHS.CreateNewAA(x1, y1, z1, x2, y2, z2, type, materialType, status, coordState, soundType, prop1, prop2, prop3, id);
	}

	public void DeleteAA(int id)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_DeleteAA", PhotonTargets.All, new object[]
			{
				id
			});
		}
		else
		{
			this._DeleteAA(id, null);
		}
	}

	[PunRPC]
	private void _DeleteAA(int id, PhotonMessageInfo info)
	{
		Kube.WHS.DeleteAA(id);
	}

	public void SetAAParameters(int x1, int y1, int z1, int x2, int y2, int z2, int type, int materialType, int status, int coordState, int soundType, int prop1, int prop2, int prop3, int id, int idPlayer)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SetAAParameters", PhotonTargets.All, new object[]
			{
				x1,
				y1,
				z1,
				x2,
				y2,
				z2,
				type,
				materialType,
				status,
				coordState,
				soundType,
				prop1,
				prop2,
				prop3,
				id,
				idPlayer
			});
		}
		else
		{
			this._SetAAParameters(x1, y1, z1, x2, y2, z2, type, materialType, status, coordState, soundType, prop1, prop2, prop3, id, idPlayer, null);
		}
	}

	[PunRPC]
	private void _SetAAParameters(int x1, int y1, int z1, int x2, int y2, int z2, int type, int materialType, int status, int coordState, int soundType, int prop1, int prop2, int prop3, int id, int idPlayer, PhotonMessageInfo info)
	{
		Kube.WHS.SaveAA(x1, y1, z1, x2, y2, z2, type, materialType, status, coordState, soundType, prop1, prop2, prop3, id);
	}

	public void CreateNewWire(int triggerId_1, int triggerId_2, int delay, int targetType, int xt, int yt, int zt, int id, int idPlayer)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_CreateNewWire", PhotonTargets.All, new object[]
			{
				triggerId_1,
				triggerId_2,
				delay,
				targetType,
				xt,
				yt,
				zt,
				id,
				idPlayer
			});
		}
		else
		{
			this._CreateNewWire(triggerId_1, triggerId_2, delay, targetType, xt, yt, zt, id, idPlayer, null);
		}
	}

	[PunRPC]
	private void _CreateNewWire(int triggerId_1, int triggerId_2, int delay, int targetType, int xt, int yt, int zt, int id, int idPlayer, PhotonMessageInfo info)
	{
		Kube.WHS.CreateNewWire(triggerId_1, triggerId_2, delay, targetType, xt, yt, zt, id);
	}

	public void DeleteWire(int id)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_DeleteWire", PhotonTargets.All, new object[]
			{
				id
			});
		}
		else
		{
			this._DeleteWire(id, null);
		}
	}

	[PunRPC]
	private void _DeleteWire(int id, PhotonMessageInfo info)
	{
		Kube.WHS.DeleteWire(id);
	}

	public void SaveWire(int triggerId_1, int triggerId_2, int delay, int targetType, int xt, int yt, int zt, int id, int idPlayer)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SaveWire", PhotonTargets.All, new object[]
			{
				triggerId_1,
				triggerId_2,
				delay,
				targetType,
				xt,
				yt,
				zt,
				id,
				idPlayer
			});
		}
		else
		{
			this._SaveWire(triggerId_1, triggerId_2, delay, targetType, xt, yt, zt, id, idPlayer, null);
		}
	}

	[PunRPC]
	private void _SaveWire(int triggerId_1, int triggerId_2, int delay, int targetType, int xt, int yt, int zt, int id, int idPlayer, PhotonMessageInfo info)
	{
		Kube.WHS.SaveWire(triggerId_1, triggerId_2, delay, targetType, xt, yt, zt, id);
	}

	public void SendMeGameParams(int gameType)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SendMeGameParams", PhotonTargets.MasterClient, new object[]
			{
				gameType
			});
		}
		else
		{
			this._SendMeGameParams(gameType, null);
		}
	}

	[PunRPC]
	public void _SendMeGameParams(int gameType, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		gameType = (int)Kube.BCS.gameType;
		if (Kube.BCS.gameMode == GameMode.teams)
		{
			if (PhotonNetwork.room != null)
			{
				base.photonView.RPC("_SendGameParams_Teams", PhotonTargets.Others, new object[]
				{
					Kube.BCS.teamScore[0],
					Kube.BCS.teamScore[1],
					Kube.BCS.teamScore[2],
					Kube.BCS.teamScore[3],
					Time.realtimeSinceStartup - Kube.BCS.gameStartTime
				});
			}
			else
			{
				this._SendGameParams_Teams(Kube.BCS.teamScore[0], Kube.BCS.teamScore[1], Kube.BCS.teamScore[2], Kube.BCS.teamScore[3], Time.realtimeSinceStartup - Kube.BCS.gameStartTime, null);
			}
			return;
		}
		if (gameType == 6)
		{
			if (PhotonNetwork.room != null)
			{
				base.photonView.RPC("_SendGameParams_Teams", PhotonTargets.Others, new object[]
				{
					Kube.BCS.teamScore[0],
					Kube.BCS.teamScore[1],
					Kube.BCS.teamScore[2],
					Kube.BCS.teamScore[3],
					Time.realtimeSinceStartup - Kube.BCS.gameStartTime
				});
			}
			else
			{
				this._SendGameParams_Teams(Kube.BCS.teamScore[0], Kube.BCS.teamScore[1], Kube.BCS.teamScore[2], Kube.BCS.teamScore[3], Time.realtimeSinceStartup - Kube.BCS.gameStartTime, null);
			}
			GameObject[] array = GameObject.FindGameObjectsWithTag("Flag");
			for (int i = 0; i < array.Length; i++)
			{
				FlagScript component = array[i].GetComponent<FlagScript>();
				this.ChangeFlagState(component.flagState.team, component.flagState.state, component.flagState.playerCaptured);
			}
		}
		else if (gameType == 7)
		{
			if (PhotonNetwork.room != null)
			{
				base.photonView.RPC("_SendGameParams_Teams", PhotonTargets.Others, new object[]
				{
					Kube.BCS.teamScore[0],
					Kube.BCS.teamScore[1],
					Kube.BCS.teamScore[2],
					Kube.BCS.teamScore[3],
					Time.realtimeSinceStartup - Kube.BCS.gameStartTime
				});
			}
			else
			{
				this._SendGameParams_Teams(Kube.BCS.teamScore[0], Kube.BCS.teamScore[1], Kube.BCS.teamScore[2], Kube.BCS.teamScore[3], Time.realtimeSinceStartup - Kube.BCS.gameStartTime, null);
			}
			GameObject[] array2 = GameObject.FindGameObjectsWithTag("DominatingPoint");
			for (int j = 0; j < array2.Length; j++)
			{
				this.ChangeDominatingPointState(array2[j].transform.root.gameObject.GetComponent<ItemPropsScript>().id, array2[j].GetComponent<DominatingPointScript>().teamCaptured);
			}
		}
		else if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SendGameParams_Shooter", PhotonTargets.Others, new object[]
			{
				Time.realtimeSinceStartup - Kube.BCS.gameStartTime
			});
		}
		else
		{
			this._SendGameParams_Shooter(Time.realtimeSinceStartup - Kube.BCS.gameStartTime, null);
		}
	}

	[PunRPC]
	public void _SendGameParams_Teams(int t1Score, int t2Score, int t3Score, int t4Score, float timeSinceStart, PhotonMessageInfo info)
	{
		Kube.BCS.teamScore[0] = t1Score;
		Kube.BCS.teamScore[1] = t2Score;
		Kube.BCS.teamScore[2] = t3Score;
		Kube.BCS.teamScore[3] = t4Score;
		Kube.BCS.gameStartTime = Time.realtimeSinceStartup - timeSinceStart;
		Kube.BCS.gameEndTime = (int)Time.realtimeSinceStartup + (int)((float)Kube.OH.gameMaxTime[(int)Kube.BCS.gameType] - timeSinceStart);
	}

	[PunRPC]
	public void _SendGameParams_Shooter(float timeSinceStart, PhotonMessageInfo info)
	{
		if (Kube.BCS.gameMode == GameMode.teams)
		{
			return;
		}
		int num = (int)((float)Kube.OH.gameMaxTime[(int)Kube.BCS.gameType] - timeSinceStart);
		Kube.BCS.gameStartTime = Time.realtimeSinceStartup - timeSinceStart;
		Kube.BCS.gameEndTime = (int)Time.realtimeSinceStartup + num;
		UnityEngine.MonoBehaviour.print("ShooterParams");
	}

	public void ChangeTeamScore(int deltaScore, int numTeam)
	{
		if (PhotonNetwork.isMasterClient || PhotonNetwork.room == null)
		{
			this._ChangeTeamScore(deltaScore, numTeam, null);
			return;
		}
		base.photonView.RPC("_ChangeTeamScore", PhotonTargets.MasterClient, new object[]
		{
			deltaScore,
			numTeam
		});
	}

	[PunRPC]
	public void _ChangeTeamScore(int deltaScore, int numTeam, PhotonMessageInfo info)
	{
		Kube.BCS.teamScore[numTeam] += deltaScore;
		this._SendMeGameParams((int)Kube.BCS.gameType, null);
	}

	public void SetSurvivalModeReady(bool isReady)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SetSurvivalModeReady", PhotonTargets.All, new object[]
			{
				isReady
			});
		}
		else
		{
			this._SetSurvivalModeReady(isReady, null);
		}
	}

	[PunRPC]
	public void _SetSurvivalModeReady(bool isReady, PhotonMessageInfo info)
	{
		this.survivalModeReady = isReady;
	}

	public void SurvivalStartNewWave(int numWave)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SurvivalStartNewWave", PhotonTargets.All, new object[]
			{
				numWave
			});
		}
		else
		{
			this._SurvivalStartNewWave(numWave, null);
		}
	}

	[PunRPC]
	public void _SurvivalStartNewWave(int numWave, PhotonMessageInfo info)
	{
		this.survivalModeReady = false;
		Kube.BCS.SurvivalStartNewWave(numWave);
	}

	public void SendSurvivalParams(float survivalTime, int survivalWaveNum, int survivalKilledMonsters)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SendSurvivalParams", PhotonTargets.All, new object[]
			{
				survivalTime,
				survivalWaveNum,
				survivalKilledMonsters
			});
		}
		else
		{
			this._SendSurvivalParams(survivalTime, survivalWaveNum, survivalKilledMonsters, null);
		}
	}

	[PunRPC]
	public void _SendSurvivalParams(float survivalTime, int survivalNumWave, int survivalKilledMonsters, PhotonMessageInfo info)
	{
		Kube.BCS.SurvivalSetParams(survivalTime, survivalNumWave, survivalKilledMonsters);
	}

	public void SendMissionParams(float gameGoneTime)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SendMissionParams", PhotonTargets.All, new object[]
			{
				gameGoneTime
			});
		}
		else
		{
			this._SendMissionParams(gameGoneTime, null);
		}
	}

	[PunRPC]
	public void _SendMissionParams(float gameGoneTime, PhotonMessageInfo info)
	{
		Kube.BCS.MissionSetParams(gameGoneTime);
	}

	public void ChangeItemState(int id, int newState)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ChangeItemState", PhotonTargets.All, new object[]
			{
				id,
				newState
			});
		}
		else
		{
			this._ChangeItemState(id, newState, null);
		}
	}

	[PunRPC]
	public void _ChangeItemState(int id, int newState, PhotonMessageInfo info)
	{
		Kube.WHS.ChangeItemState(id, newState);
	}

	public void RemoveGameItem(int id)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_RemoveGameItem", PhotonTargets.All, new object[]
			{
				id
			});
		}
		else
		{
			this._RemoveGameItem(id, null);
		}
	}

	[PunRPC]
	private void _RemoveGameItem(int id, PhotonMessageInfo info)
	{
		Kube.WHS.RemoveGameItem(id);
	}

	public void RotateGameItem(int id)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_RotateGameItem", PhotonTargets.All, new object[]
			{
				id
			});
		}
		else
		{
			this._RotateGameItem(id, null);
		}
	}

	[PunRPC]
	private void _RotateGameItem(int id, PhotonMessageInfo info)
	{
		Kube.WHS.RotateGameItem(id);
	}

	public void CreateGameItem(int numItem, byte rotation, int x, int y, int z, int playerId)
	{
		if (Kube.IS.gameItemsGO[numItem].GetComponent<ItemPropsScript>().buildMagic)
		{
			if (PhotonNetwork.room != null)
			{
				base.photonView.RPC("_CreateGameMagic", PhotonTargets.All, new object[]
				{
					numItem,
					rotation,
					x,
					y,
					z,
					playerId
				});
			}
			else
			{
				this._CreateGameMagic(numItem, rotation, x, y, z, playerId, null);
			}
		}
		else
		{
			int num = x + z * 256 + y * 256 * 256;
			if (PhotonNetwork.room != null)
			{
				base.photonView.RPC("_CreateGameItem", PhotonTargets.All, new object[]
				{
					numItem,
					rotation,
					x,
					y,
					z,
					num
				});
			}
			else
			{
				this._CreateGameItem(numItem, rotation, x, y, z, num, null);
			}
		}
	}

	public void CreateMagic(int numItem, Vector3 pos, Vector3 shotPoint, int playerId)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_CreateGameMagic", PhotonTargets.All, new object[]
			{
				numItem,
				pos,
				shotPoint,
				playerId
			});
		}
		else
		{
			this._CreateGameMagic(numItem, pos, shotPoint, playerId, null);
		}
	}

	[PunRPC]
	private void _CreateGameMagic(int numItem, byte rotation, int x, int y, int z, int playerId, PhotonMessageInfo info)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Kube.IS.gameItemsGO[numItem], new Vector3((float)x, (float)y, (float)z), Quaternion.LookRotation(Kube.OH.GameItemRotationVector[(int)rotation])) as GameObject;
		gameObject.SendMessage("SetParameters", playerId);
		if (info != null && info.sender == PhotonNetwork.player)
		{
			gameObject.SendMessage("SetupItem", SendMessageOptions.DontRequireReceiver);
		}
		Kube.WHS.CreateMagic(gameObject, numItem);
	}

	[PunRPC]
	private void _CreateGameMagic(int numItem, Vector3 pos, Vector3 shotPoint, int playerId, PhotonMessageInfo info)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Kube.IS.gameItemsGO[numItem], Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.SendMessage("SetParameters", playerId, SendMessageOptions.DontRequireReceiver);
		gameObject.SendMessage("SetParametersPos", pos, SendMessageOptions.DontRequireReceiver);
		gameObject.SendMessage("SetParametersPoint", shotPoint, SendMessageOptions.DontRequireReceiver);
		if (info != null && info.sender == PhotonNetwork.player)
		{
			gameObject.SendMessage("SetupItem", SendMessageOptions.DontRequireReceiver);
		}
		Kube.WHS.CreateMagic(gameObject, numItem);
	}

	[PunRPC]
	private void _CreateGameItem(int numItem, byte rotation, int x, int y, int z, int id, PhotonMessageInfo info)
	{
		GameObject gameObject = Kube.WHS.CreateGameItem(numItem, rotation, x, y, z, 0, id, true);
		if (info != null && info.sender == PhotonNetwork.player)
		{
			gameObject.SendMessage("SetupItem", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void ChangeCubesHealth(string cubesToChange)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ChangeCubesHealth", PhotonTargets.All, new object[]
			{
				cubesToChange
			});
		}
		else
		{
			this._ChangeCubesHealth(cubesToChange, null);
		}
	}

	[PunRPC]
	private void _ChangeCubesHealth(string cubesToChange, PhotonMessageInfo info)
	{
		Kube.WHS.ChangeCubesHealth(cubesToChange);
	}

	public void ChangeCubes(string cubesToChange)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ChangeCubes", PhotonTargets.All, new object[]
			{
				cubesToChange
			});
		}
		else
		{
			this._ChangeCubes(cubesToChange, null);
		}
	}

	[PunRPC]
	private void _ChangeCubes(string cubesToChange, PhotonMessageInfo info)
	{
		Kube.WHS.ChangeCubes(cubesToChange, true, true);
	}

	public void PlaceNewCube(Vector3 pos, int cubeType, int geom = 0)
	{
		short[] array = new short[]
		{
			(short)pos.x,
			(short)pos.y,
			(short)pos.z,
			(short)cubeType,
			(short)geom
		};
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_PlaceNewCube", PhotonTargets.All, new object[]
			{
				array
			});
		}
		else
		{
			this._PlaceNewCube(array, null);
		}
	}

	[PunRPC]
	private void _PlaceNewCube(short[] data, PhotonMessageInfo info)
	{
		Kube.WHS.ChangeOneCube((int)data[0], (int)data[1], (int)data[2], (int)data[3], (int)data[4]);
	}

	public void RequestMap()
	{
		base.photonView.RPC("SendMeMap", PhotonTargets.MasterClient, new object[0]);
	}

	[PunRPC]
	private void SendMeMap(PhotonMessageInfo info)
	{
		if (Kube.BCS.isWaintingForMap)
		{
			return;
		}
		Kube.GPS.printLog(((!PhotonNetwork.isMasterClient) ? "Client" : "Server") + "-" + ((!base.photonView.isMine) ? "NotMine" : "Mine") + " SendMeMap");
		if (PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("MapFromMaster", info.sender, new object[]
			{
				Kube.WHS.SaveWorld()
			});
		}
	}

	[PunRPC]
	private void MapFromMaster(byte[] mapData, PhotonMessageInfo info)
	{
		base.CancelInvoke("RequestMap");
		Kube.GPS.printLog(((!PhotonNetwork.isMasterClient) ? "Client" : "Server") + "-" + ((!base.photonView.isMine) ? "NotMine" : "Mine") + " MapFromMaster");
		Kube.BCS.OnMapLoaded(mapData);
	}

	public void ChangeCanBuildStatus(int playerId, bool canBuild)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ChangeCanBuildStatus", PhotonTargets.All, new object[]
			{
				playerId,
				canBuild
			});
		}
		else
		{
			this._ChangeCanBuildStatus(playerId, canBuild, null);
		}
	}

	[PunRPC]
	private void _ChangeCanBuildStatus(int playerId, bool canBuild, PhotonMessageInfo info)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			PlayerScript component = array[i].GetComponent<PlayerScript>();
			if (component.serverId == playerId)
			{
				component.canBuild = canBuild;
				if (canBuild)
				{
					Kube.GPS.printMessage(string.Concat(new string[]
					{
						"--- ",
						AuxFunc.DecodeRussianName(component.playerName),
						" ",
						Localize.player_can_build_now,
						" ---"
					}), Color.green);
				}
				else
				{
					Kube.GPS.printMessage(string.Concat(new string[]
					{
						"--- ",
						AuxFunc.DecodeRussianName(component.playerName),
						" ",
						Localize.player_cant_build_now,
						" ---"
					}), Color.red);
				}
				break;
			}
		}
	}

	public void BanPlayer(int serverId)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_BanPlayer", PhotonTargets.All, new object[]
			{
				serverId
			});
		}
		else
		{
			this._BanPlayer(serverId, null);
		}
	}

	[PunRPC]
	private void _BanPlayer(int playerId, PhotonMessageInfo info)
	{
		if (Kube.SS.serverId != playerId)
		{
			return;
		}
		Kube.BCS.EndGame(BattleControllerScript.EndGameType.ban);
	}

	public void SynhronizePlayers()
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SynhronizePlayers", PhotonTargets.All, new object[0]);
		}
		else
		{
			this._SynhronizePlayers(null);
		}
	}

	[PunRPC]
	private void _SynhronizePlayers(PhotonMessageInfo info)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			PlayerScript component = array[i].GetComponent<PlayerScript>();
			if (component.photonView.isMine)
			{
				component.SynhronizePlayer(info.sender);
				break;
			}
		}
	}

	public void RequestToRestart()
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_RequestToRestartShooter", PhotonTargets.All, new object[0]);
		}
		else
		{
			this._RequestToRestartShooter(null);
		}
	}

	[PunRPC]
	private void _RequestToRestartShooter(PhotonMessageInfo info)
	{
		(Kube.BCS.gameTypeController as RoundGameType).Restart();
	}

	[PunRPC]
	private void _RequestToRestartTeamShooter(PhotonMessageInfo info)
	{
		this._RequestToRestartShooter(info);
	}

	[PunRPC]
	private void _GiveLotOfDrop(PhotonMessageInfo info)
	{
		this._RequestToRestartShooter(info);
	}

	public void GiveLotOfDrop(PlayerScript ps, FastInventar[] weapons)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_RequestToRestartShooter", PhotonTargets.All, new object[0]);
		}
		else
		{
			this._RequestToRestartShooter(null);
		}
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.connected || stream.isWriting)
		{
		}
	}

	[PunRPC]
	private void _SaveMapItem(int id, byte[] data, PhotonMessageInfo info)
	{
		KubeStream br = new KubeStream(data);
		GameMapItem component = Kube.WHS.FindGameItem(id).GetComponent<GameMapItem>();
		component.LoadMap(br);
	}

	public void SaveMapItem(GameMapItem item)
	{
		if (PhotonNetwork.room == null)
		{
			return;
		}
		KubeStream kubeStream = new KubeStream(null);
		item.SaveMap(kubeStream);
		int num = Kube.WHS.FindGameItemId(item.gameObject);
		byte[] array = kubeStream.ToArray();
		base.photonView.RPC("_SaveMapItem", PhotonTargets.Others, new object[]
		{
			num,
			array
		});
	}

	internal void SetId(int p, int transportRespawnId)
	{
		throw new NotImplementedException();
	}

	public void RoundRestart(int endFlag)
	{
		PhotonNetwork.RemoveRPCs(base.photonView);
		base.photonView.RPC("_RoundRestart", PhotonTargets.AllBufferedViaServer, new object[]
		{
			endFlag
		});
	}

	[PunRPC]
	private void _RoundRestart(int endFlag)
	{
		RoundGameType roundGameType = Kube.BCS.gameTypeController as RoundGameType;
		if (endFlag != 1)
		{
			if (endFlag != 2)
			{
				if (endFlag == 4)
				{
				}
			}
		}
		roundGameType.EndRoundAndRestart();
	}

	public void SendClanWarEnd()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		GTClanWar gtclanWar = Kube.BCS.gameTypeController as GTClanWar;
		dictionary["c0"] = gtclanWar.clanids[0].ToString();
		dictionary["c1"] = gtclanWar.clanids[1].ToString();
		dictionary["s0"] = Kube.BCS.teamScore[0].ToString();
		dictionary["s1"] = Kube.BCS.teamScore[1].ToString();
		Kube.SS.Request(838, dictionary, delegate(string ans)
		{
			JsonData jsonData = JsonMapper.ToObject(ans);
			base.photonView.RPC("_SendClanWarEnd", PhotonTargets.Others, new object[]
			{
				(int)jsonData["clanid"]
			});
		});
	}

	[PunRPC]
	private void _SendClanWarEnd(int id)
	{
		GTClanWar gtclanWar = Kube.BCS.gameTypeController as GTClanWar;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["clanid"] = id.ToString();
		dictionary["c0"] = gtclanWar.clanids[0].ToString();
		dictionary["c1"] = gtclanWar.clanids[1].ToString();
		dictionary["s0"] = Kube.BCS.teamScore[0].ToString();
		dictionary["s1"] = Kube.BCS.teamScore[1].ToString();
		Kube.SS.Request(838, dictionary, null);
	}

	private int sendingRequestCountOfWorldChanges;

	public bool sendingWorldChanges;

	public int numSendedChange;

	public int numChangesToSend;

	public bool survivalModeReady;
}
