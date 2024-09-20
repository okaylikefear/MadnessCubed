using System;
using kube;
using UnityEngine;

public class MissionHoldNSecond : MissionBase
{
	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.endTime = Time.time + (float)((int)Kube.OH.tempMap.missionConfig[1]);
		this.initialized = true;
		if (Kube.OH.tempMap.missionConfig[2] != null)
		{
			this.monsterLimit = Mathf.Min(5, (int)Kube.OH.tempMap.missionConfig[2]);
		}
	}

	private void Start()
	{
		this.Init();
		Kube.BCS.hud.timer.gameObject.SetActive(true);
		Kube.BCS.hud.curstat.gameObject.SetActive(false);
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
			if (!this.playerDead)
			{
				Kube.BCS.EndGame(BattleControllerScript.EndGameType.exitTrigger);
			}
			else
			{
				Kube.BCS.EndGame(BattleControllerScript.EndGameType.time);
			}
			return;
		}
		if (this.playerDead)
		{
			return;
		}
		base.MonsterRespawnTick();
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
						base.NO.RequestToRespawnTransport(l);
					}
				}
			}
			this.lastCheckTransportTime = Time.time;
		}
	}

	private void UpdateHUD()
	{
		if (this.playerDead)
		{
			return;
		}
		Kube.BCS.hud.timer.timer = (int)(this.endTime - Time.time);
	}

	private void PlayerDie()
	{
		this.playerDead = true;
	}

	private void PlayerRespawn()
	{
		this.playerDead = false;
	}

	private float endTime;

	private bool initialized;

	private new float lastCheckTransportTime;

	private float lastCheckTransportDeltaTime = 2f;

	protected bool playerDead;
}
