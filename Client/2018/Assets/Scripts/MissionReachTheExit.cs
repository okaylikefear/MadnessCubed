using System;
using kube;
using UnityEngine;

public class MissionReachTheExit : MissionBase
{
	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.NO = Kube.BCS.NO;
		this.initialized = true;
		this.frags = (int)Kube.OH.tempMap.missionConfig[0];
	}

	private void FoundItem()
	{
		Kube.BCS.ps.points += 100;
	}

	private void Start()
	{
		this.Init();
		Kube.BCS.hud.curstat.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (this.frags <= 0)
		{
			return;
		}
		if (Kube.BCS.gameProcess != BattleControllerScript.GameProcess.game)
		{
			return;
		}
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		base.MonsterRespawnTick(false);
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
	}

	private void TriggerExitReached()
	{
		if (Kube.BCS.ps && Kube.BCS.ps.dead)
		{
			return;
		}
		this.Init();
		Kube.BCS.ps.points += 100;
		Kube.BCS.EndGame(BattleControllerScript.EndGameType.exitTrigger);
	}

	private new NetworkObjectScript NO;

	private bool initialized;

	protected int frags;

	protected int limit;

	private float lastCheckMonstersTime;

	private float lastCheckMonstersDeltaTime = 2f;

	private new float lastCheckTransportTime;

	private float lastCheckTransportDeltaTime = 2f;
}
