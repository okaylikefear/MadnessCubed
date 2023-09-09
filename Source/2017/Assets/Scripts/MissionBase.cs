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

	protected void MonsterRespawnTick(bool respawn = true)
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
					if (Time.time < Kube.WHS.monsterRespawnS[i].monsterLastDieTime)
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
						if (flag)
						{
							Kube.WHS.monsterRespawnS[i].monsterLastDieTime = Time.time;
						}
					}
				}
			}
			int num = Math.Min(this.monsterTotalLimit, this.monsterLimit * Kube.BCS.playersInfo.Length);
			if (array.Length > num)
			{
				return;
			}
			int num2 = array.Length;
			int num3 = 0;
			for (int k = 0; k < Kube.WHS.monsterRespawnS.Length; k++)
			{
				if (num2 >= num)
				{
					break;
				}
				if (num3 > this.monsterReqLimit)
				{
					return;
				}
				if (Kube.WHS.monsterRespawnS[k])
				{
					if (respawn || Kube.WHS.monsterRespawnS[k].spawnTime <= 0)
					{
						if (Time.time - Kube.WHS.monsterRespawnS[k].monsterLastDieTime > (float)Kube.WHS.monsterRespawnS[k].secToRespawn[Kube.WHS.monsterRespawnS[k].respawnTime])
						{
							this.NO.RequestToRespawnMonster(k);
							num2++;
							num3++;
						}
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

	private float lastCheckMonstersTime;

	private float lastCheckMonstersDeltaTime = 2f;

	public Vector3 respawnPos = Vector3.zero;

	protected int monsterLimit = 5;

	protected int monsterTotalLimit = 20;

	protected int monsterReqLimit = 3;
}
