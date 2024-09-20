using System;
using kube;
using UnityEngine;

public class MissionBase : GameTypeControllerBase
{
	protected NetworkObjectScript NO
	{
		get
		{
			return Kube.BCS.NO;
		}
	}

	protected void MonsterRespawnTick()
	{
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
			if (array.Length > this.monsterLimit * Kube.BCS.playersInfo.Length)
			{
				return;
			}
			int num = array.Length;
			for (int k = 0; k < Kube.WHS.monsterRespawnS.Length; k++)
			{
				if (num > this.monsterTotalLimit)
				{
					break;
				}
				if (Kube.WHS.monsterRespawnS[k])
				{
					if (Time.time - Kube.WHS.monsterLastDieTime[k] > (float)Kube.WHS.monsterRespawnS[k].secToRespawn[Kube.WHS.monsterRespawnS[k].respawnTime])
					{
						this.NO.RequestToRespawnMonster(k);
						num++;
					}
				}
			}
			this.lastCheckMonstersTime = Time.time;
		}
	}

	public void OnPhotonPlayerConnected()
	{
		if (PhotonNetwork.isMasterClient && PhotonNetwork.room.playerCount == PhotonNetwork.room.maxPlayers)
		{
			PhotonNetwork.room.visible = false;
			UnityEngine.Debug.Log("Maximum reached - hide room!");
		}
	}

	protected const int MAX_MONSTERS_PER_PLAYER = 5;

	protected const int MAX_MONSTERS = 20;

	public bool syncStart;

	public int monsterLimit = 5;

	public int monsterTotalLimit = 20;

	private float lastCheckMonstersTime;

	private float lastCheckMonstersDeltaTime = 2f;

	public Vector3 respawnPos = Vector3.zero;
}
