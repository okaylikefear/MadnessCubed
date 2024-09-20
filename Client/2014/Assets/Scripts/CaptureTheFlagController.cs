using System;
using System.Collections;
using kube;
using UnityEngine;

public class CaptureTheFlagController : TeamControllerBase
{
	public override void Initialize()
	{
		if (this.initialized)
		{
			return;
		}
		this.respawnsRed = GameObject.FindGameObjectsWithTag("RespawnRed");
		this.respawnsBlue = GameObject.FindGameObjectsWithTag("RespawnBlue");
		this.respawnsGreen = GameObject.FindGameObjectsWithTag("RespawnGreen");
		this.respawnsYellow = GameObject.FindGameObjectsWithTag("RespawnYellow");
		this.initialized = true;
	}

	private void Start()
	{
		this.Initialize();
		Kube.BCS.hud.timer.gameObject.SetActive(true);
	}

	private void UpdateHUD()
	{
		Kube.BCS.hud.timer.timer = Mathf.FloorToInt((float)Kube.BCS.gameEndTime - Time.realtimeSinceStartup);
	}

	private void Update()
	{
		if (Kube.BCS.gameProcess == BattleControllerScript.GameProcess.game)
		{
			this.UpdateHUD();
			if ((float)Kube.BCS.gameEndTime < Time.realtimeSinceStartup)
			{
				if (Kube.IS.ps == null)
				{
					return;
				}
				bool flag = true;
				for (int i = 0; i < 4; i++)
				{
					if (i != Kube.IS.ps.team)
					{
						if (Kube.BCS.teamScore[Kube.IS.ps.team] <= Kube.BCS.teamScore[i])
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					BattleControllerScript bcs = Kube.BCS;
					bcs.bonusCounters.winnerTeam = bcs.bonusCounters.winnerTeam + 1;
				}
				base.EndRound();
			}
			if (!PhotonNetwork.isMasterClient)
			{
				return;
			}
			if (Time.time - this.lastCheckTransportTime > this.lastCheckTransportDeltaTime && PhotonNetwork.isMasterClient)
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag("Transport");
				TransportScript[] array2 = new TransportScript[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array2[j] = array[j].GetComponent<TransportScript>();
				}
				for (int k = 0; k < Kube.WHS.transportRespawnS.Length; k++)
				{
					if (Kube.WHS.transportRespawnS[k])
					{
						if (Time.time < Kube.WHS.transportLastDieTime[k])
						{
							bool flag2 = false;
							for (int l = 0; l < array.Length; l++)
							{
								TransportScript transportScript = array2[l];
								if (!(transportScript == null))
								{
									if (transportScript.transportId == k)
									{
										flag2 = true;
										break;
									}
								}
							}
							if (!flag2)
							{
								Kube.WHS.transportLastDieTime[k] = 0f;
							}
						}
					}
				}
				for (int m = 0; m < Kube.WHS.transportRespawnS.Length; m++)
				{
					if (Kube.WHS.transportRespawnS[m])
					{
						if (Time.time - Kube.WHS.transportLastDieTime[m] > (float)Kube.WHS.transportRespawnS[m].secToRespawn[Kube.WHS.transportRespawnS[m].respawnTime])
						{
							Kube.BCS.NO.RequestToRespawnTransport(m);
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
		if (Kube.IS.ps == null)
		{
			return;
		}
		Kube.BCS.gameStartTime = Time.realtimeSinceStartup;
		Kube.BCS.gameEndTime = (int)Kube.BCS.gameStartTime + Kube.OH.gameMaxTime[(int)Kube.BCS.gameType];
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
		for (int j = 0; j < 4; j++)
		{
			Kube.BCS.teamScore[j] = 0;
		}
	}

	public override void EnterGame()
	{
		int[] array = new int[4];
		for (int i = 0; i < 4; i++)
		{
			array[i] = 0;
			if (i != 0 || this.respawnsRed != null)
			{
				if (i != 0 || this.respawnsRed.Length != 0)
				{
					if (i != 1 || this.respawnsBlue != null)
					{
						if (i != 1 || this.respawnsBlue.Length != 0)
						{
							if (i != 2 || this.respawnsGreen != null)
							{
								if (i != 2 || this.respawnsGreen.Length != 0)
								{
									if (i != 3 || this.respawnsYellow != null)
									{
										if (i != 3 || this.respawnsYellow.Length != 0)
										{
											array[i] = Kube.BCS.teamScore[i] + 1;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		int num = AuxFunc.RandomSelectWithChance(array);
		Kube.BCS.battleCamera.SetActive(false);
		Vector3 position = new Vector3(1f, 40f, 1f);
		GameObject[] array2 = new GameObject[0];
		if (num == 0)
		{
			array2 = GameObject.FindGameObjectsWithTag("RespawnRed");
		}
		if (num == 1)
		{
			array2 = GameObject.FindGameObjectsWithTag("RespawnBlue");
		}
		if (num == 2)
		{
			array2 = GameObject.FindGameObjectsWithTag("RespawnGreen");
		}
		if (num == 3)
		{
			array2 = GameObject.FindGameObjectsWithTag("RespawnYellow");
		}
		if (array2.Length != 0)
		{
			position = array2[UnityEngine.Random.Range(0, array2.Length)].transform.position;
		}
		Kube.BCS.ps = Kube.BCS.CreatePlayer(position, Quaternion.identity);
		Kube.BCS.ps.SetTeam(num);
		Kube.BCS.ps.ShowMyTeam();
		Kube.IS.ps = Kube.BCS.ps;
		if (Kube.BCS.gameType == GameType.creating)
		{
			Kube.IS.ShowFastPanel(true);
		}
		Kube.BCS.gameProcess = BattleControllerScript.GameProcess.game;
		if ((float)Kube.BCS.gameEndTime < Time.realtimeSinceStartup)
		{
			Kube.BCS.battleCamera.SetActive(true);
			Kube.BCS.gameProcess = BattleControllerScript.GameProcess.end;
			Kube.IS.ps.cameraComp.enabled = false;
			Kube.IS.ps.playerView.enabled = false;
			Kube.IS.ps.paused = true;
			if (Kube.IS.ps.isDriveTransport)
			{
				Kube.IS.ps.transportToDriveScript.ExitDrive(Kube.IS.ps.id);
			}
		}
	}

	private bool CheckFlagsLoaded()
	{
		if (this.flags != null)
		{
			return true;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Flag");
		if (array.Length > 0)
		{
			this.flags = new FlagScript[array.Length];
			for (int i = 0; i < this.flags.Length; i++)
			{
				this.flags[i] = array[i].GetComponent<FlagScript>();
			}
		}
		return this.flags != null;
	}

	public void FlagCaptured(int playerCaptured, int teamCaptured, int teamLose)
	{
		Kube.BCS.teamScore[teamCaptured]++;
		ArrayList arrayList = new ArrayList();
		arrayList.Add(Kube.OH.teamColor[teamCaptured]);
		arrayList.Add(40);
		arrayList.Add(0.75f);
		arrayList.Add(0.5f);
		arrayList.Add(Localize.teamName[teamCaptured] + " " + Localize.flag_has_been_captured);
		(UnityEngine.Object.Instantiate(Kube.OH.pointsText, base.transform.position + Vector3.up * 2f, Quaternion.identity) as GameObject).SendMessage("SetText", arrayList);
		UnityEngine.Object.Instantiate(Kube.ASS4.soundFlagCaptured, Vector3.zero, Quaternion.identity);
	}

	private GameObject[] respawnsRed;

	private GameObject[] respawnsBlue;

	private GameObject[] respawnsGreen;

	private GameObject[] respawnsYellow;

	private float lastCheckTransportTime;

	private float lastCheckTransportDeltaTime = 2f;

	private bool initialized;

	private FlagScript[] flags;
}
