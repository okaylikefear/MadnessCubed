using System;
using kube;
using UnityEngine;

public class ShooterController : RoundGameType
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Kube.BCS.gameProcess == BattleControllerScript.GameProcess.game)
		{
			if ((float)Kube.BCS.gameEndTime < Time.realtimeSinceStartup)
			{
				int num = Kube.BCS.playersInfo.Length - 1;
				for (int i = 0; i < Kube.BCS.playersInfo.Length; i++)
				{
					if (Kube.IS.ps.id == Kube.BCS.playersInfo[i].Id)
					{
						num = i;
						break;
					}
				}
				if (num == 0)
				{
					BattleControllerScript bcs = Kube.BCS;
					bcs.bonusCounters.firstPlace = bcs.bonusCounters.firstPlace + 1;
				}
				else if (num == 1)
				{
					BattleControllerScript bcs2 = Kube.BCS;
					bcs2.bonusCounters.secondPlace = bcs2.bonusCounters.secondPlace + 1;
				}
				else if (num == 2)
				{
					BattleControllerScript bcs3 = Kube.BCS;
					bcs3.bonusCounters.thirdPlace = bcs3.bonusCounters.thirdPlace + 1;
				}
				base.EndRound();
			}
			if (!PhotonNetwork.isMasterClient)
			{
				return;
			}
			if (Time.time - this.lastCheckMonstersTime > this.lastCheckMonstersDeltaTime)
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag("Monster");
				for (int j = 0; j < Kube.WHS.monsterRespawnS.Length; j++)
				{
					if (Kube.WHS.monsterRespawnS[j])
					{
						if (Time.time < Kube.WHS.monsterLastDieTime[j])
						{
							bool flag = false;
							for (int k = 0; k < array.Length; k++)
							{
								MonsterScript component = array[k].GetComponent<MonsterScript>();
								if (component.createdFromRespawnNum == j)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								Kube.WHS.monsterLastDieTime[j] = 0f;
							}
						}
					}
				}
				for (int l = 0; l < Kube.WHS.monsterRespawnS.Length; l++)
				{
					if (Kube.WHS.monsterRespawnS[l])
					{
						if (Time.time - Kube.WHS.monsterLastDieTime[l] > (float)Kube.WHS.monsterRespawnS[l].secToRespawn[Kube.WHS.monsterRespawnS[l].respawnTime])
						{
							Kube.BCS.NO.RequestToRespawnMonster(l);
						}
					}
				}
				this.lastCheckMonstersTime = Time.time;
			}
			if (Time.time - this.lastCheckTransportTime > this.lastCheckTransportDeltaTime && PhotonNetwork.isMasterClient)
			{
				GameObject[] array2 = GameObject.FindGameObjectsWithTag("Transport");
				TransportScript[] array3 = new TransportScript[array2.Length];
				for (int m = 0; m < array2.Length; m++)
				{
					array3[m] = array2[m].GetComponent<TransportScript>();
				}
				for (int n = 0; n < Kube.WHS.transportRespawnS.Length; n++)
				{
					if (Kube.WHS.transportRespawnS[n])
					{
						if (Time.time < Kube.WHS.transportLastDieTime[n])
						{
							bool flag2 = false;
							for (int num2 = 0; num2 < array2.Length; num2++)
							{
								TransportScript transportScript = array3[num2];
								if (!(transportScript == null))
								{
									if (transportScript.transportId == n)
									{
										flag2 = true;
										break;
									}
								}
							}
							if (!flag2)
							{
								Kube.WHS.transportLastDieTime[n] = 0f;
							}
						}
					}
				}
				for (int num3 = 0; num3 < Kube.WHS.transportRespawnS.Length; num3++)
				{
					if (Kube.WHS.transportRespawnS[num3])
					{
						if (Time.time - Kube.WHS.transportLastDieTime[num3] > (float)Kube.WHS.transportRespawnS[num3].secToRespawn[Kube.WHS.transportRespawnS[num3].respawnTime])
						{
							Kube.BCS.NO.RequestToRespawnTransport(num3);
						}
					}
				}
				this.lastCheckTransportTime = Time.time;
			}
		}
		else if (Kube.BCS.gameProcess == BattleControllerScript.GameProcess.end)
		{
			base.EndRoundUpdate();
		}
	}

	protected override void _Restart()
	{
		Kube.BCS.gameStartTime = Time.realtimeSinceStartup;
		Kube.BCS.gameEndTime = (int)Kube.BCS.gameStartTime + Kube.OH.gameMaxTime[2];
		Kube.BCS.battleCamera.SetActive(false);
		Kube.BCS.gameProcess = BattleControllerScript.GameProcess.game;
		Kube.BCS.endRound.gameObject.SetActive(false);
		Kube.BCS.endRoundScoresUI.gameObject.SetActive(false);
		Kube.IS.ps.cameraComp.enabled = true;
		Kube.IS.ps.playerView.enabled = true;
		Kube.IS.ps.paused = false;
		Kube.IS.ps.Respawn();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			PlayerScript component = array[i].GetComponent<PlayerScript>();
			component.kills = 0;
			component.deadTimes = 0;
		}
	}

	private void EndRoundMenu()
	{
	}

	private float lastCheckMonstersTime;

	private float lastCheckMonstersDeltaTime = 2f;

	private float lastCheckTransportTime;

	private float lastCheckTransportDeltaTime = 2f;
}
