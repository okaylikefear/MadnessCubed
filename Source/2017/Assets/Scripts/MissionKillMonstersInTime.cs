using System;
using kube;
using UnityEngine;

public class MissionKillMonstersInTime : MissionBase
{
	public int monsterKilled
	{
		get
		{
			return -this._monsterKilled + Kube.GPS.codeI;
		}
		set
		{
			this._monsterKilled = Kube.GPS.codeI - value;
		}
	}

	private void SaveCodeVars()
	{
		this.codeVarsRandom = UnityEngine.Random.Range(10, 1000);
		this._monsterKilled2 = this.monsterKilled + this.codeVarsRandom;
	}

	private void LoadCodeVars()
	{
		this.monsterKilled = this._monsterKilled2 - this.codeVarsRandom;
	}

	private void KilledMonster()
	{
		this.monsterKilled++;
	}

	private new void OnPhotonPlayerConnected()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		if (PhotonNetwork.playerList.Length < PhotonNetwork.room.maxPlayers)
		{
			return;
		}
		PhotonNetwork.room.visible = false;
	}

	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.NO = Kube.BCS.NO;
		this.monsterKilled = 0;
		this.endTime = Time.time + (float)((int)Kube.OH.tempMap.missionConfig[1]);
		this.initialized = true;
		this.frags = (int)Kube.OH.tempMap.missionConfig[0];
	}

	private void Start()
	{
		this.Init();
		Kube.BCS.hud.timer.gameObject.SetActive(true);
	}

	private void Update()
	{
		if (Kube.BCS.gameProcess != BattleControllerScript.GameProcess.game)
		{
			return;
		}
		this.UpdateHUD();
		if (this.endTime - Time.time < 0f)
		{
			Kube.BCS.EndGame(BattleControllerScript.EndGameType.time);
		}
		base.MonsterRespawnTick(true);
		if (Time.time - this.lastCheckTransportTime > this.lastCheckTransportDeltaTime && PhotonNetwork.isMasterClient)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Transport");
			TransportScript[] array2 = new TransportScript[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].GetComponent<TransportScript>();
			}
			for (int j = 0; j < Kube.WHS.transportRespawnS.Length; j++)
			{
				if (Kube.WHS.transportRespawnS[j])
				{
					if (Time.time < Kube.WHS.transportLastDieTime[j])
					{
						bool flag = false;
						for (int k = 0; k < array.Length; k++)
						{
							TransportScript transportScript = array2[k];
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
		if (this.monsterKilled >= this.frags)
		{
			Kube.BCS.EndGame(BattleControllerScript.EndGameType.exitTrigger);
		}
	}

	private new void UpdateHUD()
	{
		Kube.BCS.hud.curstat.values[0].value = this.monsterKilled + "/" + this.frags;
		Kube.BCS.hud.timer.timer = (int)(this.endTime - Time.time);
	}

	private new NetworkObjectScript NO;

	private int _monsterKilled;

	private int codeVarsRandom;

	private int _monsterKilled2;

	private float endTime;

	protected int frags;

	private bool initialized;

	private float lastCheckMonstersTime;

	private float lastCheckMonstersDeltaTime = 2f;

	private new float lastCheckTransportTime;

	private float lastCheckTransportDeltaTime = 2f;
}
