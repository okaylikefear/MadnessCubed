using System;
using kube;
using kube.ui;
using UnityEngine;

public class MissionExitInTime : MissionBase
{
	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
		this.frags = (int)Kube.OH.tempMap.missionConfig[0];
		this.NO = Kube.BCS.NO;
		this.endWaitTime = Time.time + 30f;
	}

	private void Awake()
	{
		this.syncStart = true;
	}

	private void FoundItem()
	{
		Kube.BCS.ps.points += 100;
	}

	private void Start()
	{
		this.Init();
		if (PhotonNetwork.room.customProperties.ContainsKey("started"))
		{
			this.started = (bool)PhotonNetwork.room.customProperties["started"];
		}
		Kube.BCS.hud.curstat.gameObject.SetActive(false);
		Kube.BCS.hud.timer.gameObject.SetActive(true);
	}

	private void OnGUI()
	{
		float num = (float)KUI.width;
		float num2 = (float)KUI.height;
		if (Kube.BCS.gameProcess != BattleControllerScript.GameProcess.game)
		{
			int num3 = (int)Mathf.Round(this.endWaitTime - Time.time);
			GUI.skin = Kube.ASS1.mainSkinSmall;
			if (num3 > 0)
			{
				GUI.Box(new Rect(0.5f * num - 200f, num2 - 150f, 400f, 35f), "Ожидаем игроков " + num3);
			}
		}
	}

	public override void EnterGame()
	{
		this.endTime = Time.time + (float)((int)Kube.OH.tempMap.missionConfig[1]);
		PhotonNetwork.room.visible = false;
		Kube.BCS.EnterGame();
	}

	private void Update()
	{
		this.UpdateHUD();
		if (Kube.BCS.gameProcess == BattleControllerScript.GameProcess.start)
		{
			if (this.started || Time.time > this.endWaitTime || PhotonNetwork.room.playerCount == PhotonNetwork.room.maxPlayers)
			{
				this.EnterGame();
			}
			return;
		}
		if (this.endTime - Time.time < 0f)
		{
			Kube.BCS.EndGame(BattleControllerScript.EndGameType.time);
		}
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		if (this.frags <= 0)
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

	private new void UpdateHUD()
	{
		Kube.BCS.hud.timer.timer = (int)(this.endTime - Time.time);
	}

	public override int CalcGameExp()
	{
		if (Kube.BCS.ps != null)
		{
			return Kube.BCS.ps.points;
		}
		return 0;
	}

	private void TriggerExitReached()
	{
		if (Kube.BCS.ps && Kube.BCS.ps.dead)
		{
			return;
		}
		Kube.BCS.ps.points += 500;
		this.Init();
		Kube.BCS.EndGame(BattleControllerScript.EndGameType.exitTrigger);
	}

	private new NetworkObjectScript NO;

	private float endTime;

	private float endWaitTime;

	private bool initialized;

	private bool started;

	protected int frags;

	protected int limit;

	private float lastCheckMonstersTime;

	private float lastCheckMonstersDeltaTime = 2f;

	private new float lastCheckTransportTime;

	private float lastCheckTransportDeltaTime = 2f;
}
