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
		Kube.BCS = base.gameObject.GetComponent<BattleControllerScript>();
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
		if (Time.time - this.lastCheckMonstersTime > this.lastCheckMonstersDeltaTime)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Monster");
			for (int i = 0; i < Kube.WHS.monsterRespawnS.Length; i++)
			{
				if (Kube.WHS.monsterRespawnS[i])
				{
					if (Time.time < Kube.WHS.monsterLastDieTime[i])
					{
						bool flag = false;
						for (int j = 0; j < array.Length; j++)
						{
							MonsterScript component = array[j].GetComponent<MonsterScript>();
							if (component.createdFromRespawnNum == i)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							Kube.WHS.monsterLastDieTime[i] = 0f;
						}
					}
				}
			}
			for (int k = 0; k < Kube.WHS.monsterRespawnS.Length; k++)
			{
				if (Kube.WHS.monsterRespawnS[k])
				{
					if (Time.time - Kube.WHS.monsterLastDieTime[k] > (float)Kube.WHS.monsterRespawnS[k].secToRespawn[Kube.WHS.monsterRespawnS[k].respawnTime])
					{
						this.NO.RequestToRespawnMonster(k);
						this.frags--;
					}
				}
			}
			this.lastCheckMonstersTime = Time.time;
		}
		if (Time.time - this.lastCheckTransportTime > this.lastCheckTransportDeltaTime && PhotonNetwork.isMasterClient)
		{
			GameObject[] array2 = GameObject.FindGameObjectsWithTag("Transport");
			TransportScript[] array3 = new TransportScript[array2.Length];
			for (int l = 0; l < array2.Length; l++)
			{
				array3[l] = array2[l].GetComponent<TransportScript>();
			}
			for (int m = 0; m < Kube.WHS.transportRespawnS.Length; m++)
			{
				if (Kube.WHS.transportRespawnS[m])
				{
					if (Time.time < Kube.WHS.transportLastDieTime[m])
					{
						bool flag2 = false;
						for (int n = 0; n < array2.Length; n++)
						{
							TransportScript transportScript = array3[n];
							if (!(transportScript == null))
							{
								if (transportScript.transportId == m)
								{
									flag2 = true;
									break;
								}
							}
						}
						if (!flag2)
						{
							Kube.WHS.transportLastDieTime[m] = 0f;
						}
					}
				}
			}
			for (int num = 0; num < Kube.WHS.transportRespawnS.Length; num++)
			{
				if (Kube.WHS.transportRespawnS[num])
				{
					if (Time.time - Kube.WHS.transportLastDieTime[num] > (float)Kube.WHS.transportRespawnS[num].secToRespawn[Kube.WHS.transportRespawnS[num].respawnTime])
					{
						this.NO.RequestToRespawnTransport(num);
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
		Kube.BCS.EndGame(BattleControllerScript.EndGameType.exitTrigger, true);
	}

	private NetworkObjectScript NO;

	private bool initialized;

	protected int frags;

	protected int limit;

	private float lastCheckMonstersTime;

	private float lastCheckMonstersDeltaTime = 2f;

	private float lastCheckTransportTime;

	private float lastCheckTransportDeltaTime = 2f;
}
