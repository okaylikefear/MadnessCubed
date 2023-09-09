using System;
using kube;
using Photon;
using UnityEngine;

public class NetworkObjectScript : Photon.MonoBehaviour
{
	private void Start()
	{
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
		if (this.waintingForMap == 2 && !Kube.SS.loadingMap)
		{
			this.waintingForMap = 0;
			this.worldChangesLoaded = true;
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

	[RPC]
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

	[RPC]
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

	[RPC]
	private void _FlagCaptured(int playerId, int team, int loseTeam, PhotonMessageInfo info)
	{
		if (Kube.BCS.gameTypeController != null)
		{
			((CaptureTheFlagController)Kube.BCS.gameTypeController).FlagCaptured(playerId, team, loseTeam);
		}
	}

	public void RequestToRespawnMonster(int id)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_RequestToRespawnMonster", PhotonTargets.All, new object[]
			{
				id
			});
		}
		else
		{
			this._RequestToRespawnMonster(id, null);
		}
	}

	[RPC]
	private void _RequestToRespawnMonster(int id, PhotonMessageInfo info)
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (!Kube.WHS.monsterRespawnS[id])
			{
				return;
			}
			if (Time.time - Kube.WHS.monsterLastDieTime[id] < (float)Kube.WHS.monsterRespawnS[id].secToRespawn[Kube.WHS.monsterRespawnS[id].respawnTime])
			{
				if (PhotonNetwork.room != null)
				{
					base.photonView.RPC("_MonsterAlifeYet", PhotonTargets.All, new object[]
					{
						id
					});
				}
				else
				{
					this._MonsterAlifeYet(id, null);
				}
			}
			else
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
				int num = UnityEngine.Random.Range(0, array.Length);
				PlayerScript component = array[num].GetComponent<PlayerScript>();
				if (PhotonNetwork.room != null)
				{
					base.photonView.RPC("_OrderToCreateMonster", PhotonTargets.All, new object[]
					{
						id,
						component.id
					});
				}
				else
				{
					this._OrderToCreateMonster(id, component.id, null);
				}
			}
		}
	}

	[RPC]
	private void _OrderToCreateMonster(int id, int playerId, PhotonMessageInfo info)
	{
		Kube.GPS.printLog("_OrderToCreateMonster id=" + id);
		Kube.WHS.monsterLastDieTime[id] = Time.time + 999999f;
		if (Kube.GPS.playerId == playerId)
		{
			GameObject gameObject = PhotonNetwork.Instantiate(Kube.OH.monsterPrefabName[Kube.WHS.monsterRespawnS[id].type], Kube.WHS.monsterRespawnS[id].transform.position, Kube.WHS.monsterRespawnS[id].transform.rotation, 0);
			gameObject.SendMessage("SetRespawnNum", id);
			gameObject.SendMessage("SetHealthMultiplier", Kube.WHS.monsterRespawnS[id].healthMultiplier);
			gameObject.SendMessage("SetDamageMultiplier", Kube.WHS.monsterRespawnS[id].damageMultiplier);
		}
	}

	[RPC]
	private void _MonsterAlifeYet(int id, PhotonMessageInfo info)
	{
		Kube.GPS.printLog("_MonsterAlifeYet id=" + id);
		Kube.WHS.monsterLastDieTime[id] = Time.time + 999999f;
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

	[RPC]
	private void _MonsterDead(int id, PhotonMessageInfo info)
	{
		Kube.GPS.printLog("_MonsterDead id=" + id);
		Kube.WHS.monsterLastDieTime[id] = Time.time;
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

	[RPC]
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
				this._OrderToCreateTransport(id, component.id, null);
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
			UnityEngine.MonoBehaviour.print(string.Concat(new object[]
			{
				i,
				" ",
				array[i].name,
				" ",
				array[i].transform.root.name
			}));
			if (array[i].GetComponent<TransportScript>().transportId == id)
			{
				return;
			}
		}
		if (Kube.GPS.playerId == playerId)
		{
			GameObject gameObject = PhotonNetwork.InstantiateSceneObject(Kube.OH.transportPrefabName[Kube.WHS.transportRespawnS[id].type], Kube.WHS.transportRespawnS[id].transform.position, Kube.WHS.transportRespawnS[id].transform.rotation, 0, null);
			gameObject.SendMessage("SetRespawnNum", id);
			gameObject.SendMessage("SetHealthMultiplier", Kube.WHS.transportRespawnS[id].healthMultiplier);
			gameObject.SendMessage("SetDamageMultiplier", Kube.WHS.transportRespawnS[id].damageMultiplier);
		}
	}

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
	public void _SendMeGameParams(int gameType, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		if (gameType == 4)
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
		}
		if (gameType == 2)
		{
			if (PhotonNetwork.room != null)
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
		if (gameType == 7)
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
	}

	[RPC]
	public void _SendGameParams_Teams(int t1Score, int t2Score, int t3Score, int t4Score, float timeSinceStart, PhotonMessageInfo info)
	{
		Kube.BCS.teamScore[0] = t1Score;
		Kube.BCS.teamScore[1] = t2Score;
		Kube.BCS.teamScore[2] = t3Score;
		Kube.BCS.teamScore[3] = t4Score;
		Kube.BCS.gameStartTime = Time.realtimeSinceStartup - timeSinceStart;
		Kube.BCS.gameEndTime = (int)Kube.BCS.gameStartTime + Kube.OH.gameMaxTime[(int)Kube.BCS.gameType];
	}

	[RPC]
	public void _SendGameParams_Shooter(float timeSinceStart, PhotonMessageInfo info)
	{
		Kube.BCS.gameStartTime = Time.realtimeSinceStartup - timeSinceStart;
		Kube.BCS.gameEndTime = (int)Kube.BCS.gameStartTime + Kube.OH.gameMaxTime[2];
		UnityEngine.MonoBehaviour.print("ShooterParams");
	}

	public void ChangeTeamScore(int deltaScore, int numTeam)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ChangeTeamScore", PhotonTargets.All, new object[]
			{
				deltaScore,
				numTeam
			});
		}
		else
		{
			this._ChangeTeamScore(deltaScore, numTeam, null);
		}
	}

	[RPC]
	public void _ChangeTeamScore(int deltaScore, int numTeam, PhotonMessageInfo info)
	{
		Kube.BCS.teamScore[numTeam] += deltaScore;
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
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

	[RPC]
	private void _CreateGameMagic(int numItem, byte rotation, int x, int y, int z, int playerId, PhotonMessageInfo info)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Kube.IS.gameItemsGO[numItem], new Vector3((float)x, (float)y, (float)z), Quaternion.LookRotation(Kube.OH.GameItemRotationVector[(int)rotation])) as GameObject;
		gameObject.SendMessage("SetParameters", playerId);
		Kube.WHS.CreateMagic(gameObject, numItem);
	}

	[RPC]
	private void _CreateGameMagic(int numItem, Vector3 pos, Vector3 shotPoint, int playerId, PhotonMessageInfo info)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Kube.IS.gameItemsGO[numItem], Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.SendMessage("SetParameters", playerId);
		gameObject.SendMessage("SetParametersPos", pos, SendMessageOptions.DontRequireReceiver);
		gameObject.SendMessage("SetParametersPoint", shotPoint, SendMessageOptions.DontRequireReceiver);
		Kube.WHS.CreateMagic(gameObject, numItem);
	}

	[RPC]
	private void _CreateGameItem(int numItem, byte rotation, int x, int y, int z, int id, PhotonMessageInfo info)
	{
		Kube.WHS.CreateGameItem(numItem, rotation, x, y, z, 0, id, true);
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

	[RPC]
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

	[RPC]
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

	[RPC]
	private void _PlaceNewCube(short[] data, PhotonMessageInfo info)
	{
		Kube.WHS.ChangeOneCube((int)data[0], (int)data[1], (int)data[2], (int)data[3], (int)data[4]);
	}

	public void LoadMap()
	{
		Kube.GPS.printLog(((!PhotonNetwork.isMasterClient) ? "Client" : "Server") + "-" + ((!base.photonView.isMine) ? "NotMine" : "Mine") + " LoadWorldChanges");
		if ((!Kube.BCS.mapCanBreak && !Kube.BCS.canChangeWorld) || PhotonNetwork.offlineMode || (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient))
		{
			this.LoadMapFromServer();
			return;
		}
		this.waintingForMap = 1;
		base.InvokeRepeating("RequestMap", 0.1f, 10f);
	}

	private void OnMasterClientSwitched()
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		if (this.waintingForMap == 1)
		{
			this.LoadMapFromServer();
		}
	}

	public void LoadMapFromServer()
	{
		base.CancelInvoke("RequestMap");
		if (Kube.OH.tempMap.NewMapType >= 0)
		{
			UnityEngine.MonoBehaviour.print("Loading map number: " + Kube.OH.tempMap.Id);
			if (Kube.GPS.playerNumMaps <= 1)
			{
				Kube.GPS.needTrainingBuild = true;
			}
			Kube.SS.LoadMap(Kube.OH.tempMap.Id);
		}
		else
		{
			UnityEngine.MonoBehaviour.print("Loading buildinMap: " + Kube.OH.tempMap.Id);
			Kube.SS.downloadMap(Kube.OH.tempMap.Id);
		}
		this.waintingForMap = 2;
	}

	private void RequestMap()
	{
		base.photonView.RPC("SendMeMap", PhotonTargets.MasterClient, new object[0]);
	}

	[RPC]
	private void SendMeMap(PhotonMessageInfo info)
	{
		Kube.GPS.printLog(((!PhotonNetwork.isMasterClient) ? "Client" : "Server") + "-" + ((!base.photonView.isMine) ? "NotMine" : "Mine") + " SendMeMap");
		if (PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("MapFromMaster", info.sender, new object[]
			{
				Kube.WHS.SaveWorld()
			});
		}
	}

	[RPC]
	private void MapFromMaster(byte[] mapData, PhotonMessageInfo info)
	{
		base.CancelInvoke("RequestMap");
		Kube.GPS.printLog(((!PhotonNetwork.isMasterClient) ? "Client" : "Server") + "-" + ((!base.photonView.isMine) ? "NotMine" : "Mine") + " MapFromMaster");
		if (this.waintingForMap == 1)
		{
			if (Kube.WHS.LoadWorld(mapData) == 1)
			{
				PhotonNetwork.LeaveRoom();
				Kube.GPS.printMessage(Localize.error_empty_map, Color.black);
				UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
			}
			this.waintingForMap = 0;
			this.worldChangesLoaded = true;
		}
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

	[RPC]
	private void _ChangeCanBuildStatus(int playerId, bool canBuild, PhotonMessageInfo info)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			PlayerScript component = array[i].GetComponent<PlayerScript>();
			if (component.id == playerId)
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

	public void BanPlayer(int playerId)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_BanPlayer", PhotonTargets.All, new object[]
			{
				playerId
			});
		}
		else
		{
			this._BanPlayer(playerId, null);
		}
	}

	[RPC]
	private void _BanPlayer(int playerId, PhotonMessageInfo info)
	{
		if (Kube.GPS.playerId != playerId)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			PlayerScript component = array[i].GetComponent<PlayerScript>();
			if (component.type == 0 && playerId == component.id)
			{
				Kube.BCS.EndGame(BattleControllerScript.EndGameType.ban, true);
			}
		}
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

	[RPC]
	private void _SynhronizePlayers(PhotonMessageInfo info)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			PlayerScript component = array[i].GetComponent<PlayerScript>();
			if (component.photonView.isMine)
			{
				component.gameObject.SendMessage("SynhronizePlayer");
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

	[RPC]
	private void _RequestToRestartShooter(PhotonMessageInfo info)
	{
		(Kube.BCS.gameTypeController as RoundGameType).Restart();
	}

	[RPC]
	private void _RequestToRestartTeamShooter(PhotonMessageInfo info)
	{
		this._RequestToRestartShooter(info);
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.connected || stream.isWriting)
		{
		}
	}

	public bool worldChangesLoaded;

	private int sendingRequestCountOfWorldChanges;

	public bool sendingWorldChanges;

	public int numSendedChange;

	public int numChangesToSend;

	public bool survivalModeReady;

	public int waintingForMap;
}
