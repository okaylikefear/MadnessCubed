using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class GameTypeControllerBase : MonoBehaviour
{
	public virtual void Initialize()
	{
	}

	public virtual void EnterGame()
	{
		Kube.BCS.EnterGame();
	}

	public virtual void configure(object[] config)
	{
	}

	public virtual int CalcGameExp()
	{
		if (Kube.BCS.ps != null)
		{
			return Kube.BCS.ps.points;
		}
		return 0;
	}

	protected virtual void UpdateTick()
	{
	}

	protected virtual void UpdateHUD()
	{
	}

	public virtual void PlayerSpawned(PlayerScript playerScript)
	{
	}

	protected void TransportRespawnTick()
	{
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
						Kube.BCS.NO.RequestToRespawnTransport(l);
					}
				}
			}
			this.lastCheckTransportTime = Time.time;
		}
	}

	public virtual bool DropStuff(PlayerScript playerScript, Dictionary<int, int> _weaponPickup)
	{
		return false;
	}

	public virtual void BeginGame()
	{
		GameType gameType = Kube.BCS.gameType;
		TeamStartMenu teamStartMenu = Cub2UI.FindMenu<TeamStartMenu>("start_teams");
		if (gameType == GameType.dominating || gameType == GameType.teams || gameType == GameType.captureTheFlag || gameType == GameType.bomb)
		{
			Cub2UI.currentMenu = teamStartMenu.gameObject;
			teamStartMenu.BeginPlay();
		}
	}

	public virtual void ChangeTeamScore(int p, int p_2)
	{
	}

	public bool canRespawn;

	protected float lastCheckTransportTime;

	private float lastCheckTransportDeltaTime = 2f;
}
