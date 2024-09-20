using System;
using kube;
using UnityEngine;

public class DominatingController : TeamControllerBase
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
		this.dominatingPointTex = Kube.OH.gameItemsTex[89];
		this.dominatingScoreDeltaTime = 5f;
		this.CheckDominatingPointsLoaded();
		HudStars hudStars = Kube.BCS.hud.curstat as HudStars;
		if (this.dpss != null)
		{
			hudStars.ShowStars(this.dpss.Length);
		}
		this.initialized = true;
	}

	private void Start()
	{
		this.Initialize();
		this.CheckDominatingPointsLoaded();
		Kube.BCS.hud.timer.gameObject.SetActive(true);
	}

	private void StopRound()
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
		Kube.BCS.gameEndTime = (int)Time.realtimeSinceStartup;
		base.EndRound();
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
			for (int i = 0; i < 4; i++)
			{
				if (Kube.BCS.teamScore[i] >= 100)
				{
					this.StopRound();
					break;
				}
			}
			if (!PhotonNetwork.isMasterClient)
			{
				return;
			}
			if (Time.time - this.dominatingScoreLastTime > this.dominatingScoreDeltaTime)
			{
				this.dominatingScoreLastTime = Time.time;
				if (this.CheckDominatingPointsLoaded())
				{
					for (int j = 0; j < this.dpss.Length; j++)
					{
						if (this.dpss[j].teamCaptured != -1)
						{
							Kube.BCS.NO.ChangeTeamScore(1, this.dpss[j].teamCaptured);
						}
					}
				}
			}
			if (Time.time - this.lastCheckTransportTime > this.lastCheckTransportDeltaTime && PhotonNetwork.isMasterClient)
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag("Transport");
				TransportScript[] array2 = new TransportScript[array.Length];
				for (int k = 0; k < array.Length; k++)
				{
					array2[k] = array[k].GetComponent<TransportScript>();
				}
				for (int l = 0; l < Kube.WHS.transportRespawnS.Length; l++)
				{
					if (Kube.WHS.transportRespawnS[l])
					{
						if (Time.time < Kube.WHS.transportLastDieTime[l])
						{
							bool flag = false;
							for (int m = 0; m < array.Length; m++)
							{
								TransportScript transportScript = array2[m];
								if (!(transportScript == null))
								{
									if (transportScript.objectId == l)
									{
										flag = true;
										break;
									}
								}
							}
							if (!flag)
							{
								Kube.WHS.transportLastDieTime[l] = 0f;
							}
						}
					}
				}
				for (int n = 0; n < Kube.WHS.transportRespawnS.Length; n++)
				{
					if (Kube.WHS.transportRespawnS[n])
					{
						if (Time.time - Kube.WHS.transportLastDieTime[n] > (float)Kube.WHS.transportRespawnS[n].secToRespawn[Kube.WHS.transportRespawnS[n].respawnTime])
						{
							Kube.BCS.NO.RequestToRespawnTransport(n);
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
			component.frags = 0;
			component.deadTimes = 0;
		}
		for (int j = 0; j < 4; j++)
		{
			Kube.BCS.teamScore[j] = 0;
		}
	}

	public override void EnterGame(int team = -1)
	{
		if ((float)Kube.BCS.gameEndTime < Time.realtimeSinceStartup)
		{
			if (PhotonNetwork.isMasterClient)
			{
				PhotonNetwork.room.visible = false;
			}
			Kube.BCS.ExitGame();
			return;
		}
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
		if (team != -1)
		{
			num = team;
		}
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
		if (Kube.BCS.gameType == GameType.creating)
		{
			Kube.IS.ShowFastPanel(true);
		}
		Kube.BCS.gameProcess = BattleControllerScript.GameProcess.game;
		Kube.OH.closeMenu(null);
	}

	private bool CheckDominatingPointsLoaded()
	{
		if (this.dpss != null)
		{
			return true;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("DominatingPoint");
		if (array.Length > 0)
		{
			this.dpss = new DominatingPointScript[array.Length];
			for (int i = 0; i < this.dpss.Length; i++)
			{
				this.dpss[i] = array[i].GetComponent<DominatingPointScript>();
			}
		}
		return this.dpss != null;
	}

	public void ChangeDominatingPointState(DominatingPointScript dp, int newTeam)
	{
		this.CheckDominatingPointsLoaded();
		int num = Array.IndexOf<DominatingPointScript>(this.dpss, dp);
		if (num == -1)
		{
			return;
		}
		HudStars hudStars = Kube.BCS.hud.curstat as HudStars;
		if (hudStars)
		{
			hudStars.ToggleStar(num, newTeam);
		}
	}

	private GameObject[] respawnsRed;

	private GameObject[] respawnsBlue;

	private GameObject[] respawnsGreen;

	private GameObject[] respawnsYellow;

	private new float lastCheckTransportTime;

	private float lastCheckTransportDeltaTime = 2f;

	private Texture dominatingPointTex;

	private float dominatingScoreLastTime;

	private float dominatingScoreDeltaTime;

	private bool initialized;

	private DominatingPointScript[] dpss;
}
