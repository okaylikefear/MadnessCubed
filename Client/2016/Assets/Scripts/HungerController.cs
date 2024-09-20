using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class HungerController : RoundGameType
{
	public override void Initialize()
	{
		if (this.initialized)
		{
			return;
		}
		if (PhotonNetwork.room.customProperties.ContainsKey("started"))
		{
			this.started = (bool)PhotonNetwork.room.customProperties["started"];
		}
		this.endWaitTime = Time.time + this.waitTime;
		this.initialized = true;
	}

	private void Start()
	{
		this.Initialize();
		Kube.BCS.hud.timer.gameObject.SetActive(true);
	}

	public override void EnterGame()
	{
		PhotonNetwork.room.visible = false;
		Kube.BCS.EnterGame();
	}

	private void UpdateHUD()
	{
		Kube.BCS.hud.timer.timer = Mathf.FloorToInt((float)Kube.BCS.gameEndTime - Time.realtimeSinceStartup);
	}

	private void Update()
	{
		if (Kube.BCS.gameProcess == BattleControllerScript.GameProcess.start)
		{
			this.UpdateBeforeStart();
			return;
		}
		if (Kube.BCS.gameProcess == BattleControllerScript.GameProcess.game)
		{
			this.UpdateHUD();
		}
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		if (Kube.BCS.gameProcess == BattleControllerScript.GameProcess.game)
		{
			base.TransportRespawnTick();
			if ((float)Kube.BCS.gameEndTime < Time.realtimeSinceStartup)
			{
				if (Kube.IS.ps == null)
				{
					return;
				}
				base.EndRound();
			}
			this.playersDead = 0;
			for (int i = 0; i < Kube.BCS.playersInfo.Length; i++)
			{
				PlayerScript ps = Kube.BCS.playersInfo[i].ps;
				if (ps == null || ps.dead)
				{
					this.playersDead++;
				}
			}
			int num = 0;
			if (this.playersDead >= Kube.BCS.playersInfo.Length - 1 && Kube.BCS.playersInfo.Length > 1)
			{
				num = 1;
			}
			if (this.playersDead == Kube.BCS.playersInfo.Length)
			{
				base.Invoke("RespawnAll", 1f);
				Kube.BCS.gameProcess = BattleControllerScript.GameProcess.end;
			}
			if (num > 0)
			{
				Kube.BCS.RefreshPlayersTable();
				int num2 = Kube.BCS.playersInfo.Length - 1;
				for (int j = 0; j < Kube.BCS.playersInfo.Length; j++)
				{
					if (Kube.IS.ps.onlineId == Kube.BCS.playersInfo[j].serverId)
					{
						break;
					}
				}
				base.Invoke("RespawnAll", 1f);
				Kube.BCS.gameProcess = BattleControllerScript.GameProcess.end;
			}
		}
	}

	private void UpdateBeforeStart()
	{
		if (PhotonNetwork.room.customProperties.ContainsKey("started"))
		{
			this.started = (bool)PhotonNetwork.room.customProperties["started"];
		}
		if (this.started || Time.time > this.endWaitTime || PhotonNetwork.room.playerCount == PhotonNetwork.room.maxPlayers)
		{
			this.EnterGame();
		}
	}

	private void OnGUI()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		if (Kube.BCS.gameProcess == BattleControllerScript.GameProcess.start)
		{
			int num3 = (int)Mathf.Round(this.endWaitTime - Time.time);
			GUI.skin = Kube.ASS1.mainSkinSmall;
			if (num3 > 0)
			{
				GUI.Box(new Rect(0.5f * num - 200f, num2 - 150f, 400f, 35f), Localize.waitRoomPlayers + num3);
			}
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

	public override void PlayerSpawned(PlayerScript playerScript)
	{
	}

	public void BombExplode(BombScript bombScript)
	{
		PlayerScript playerScript = Kube.BCS.FindPlayerByPV(bombScript.photonView.owner);
		if (playerScript)
		{
			Kube.BCS.NO.ChangeTeamScore(1, playerScript.team);
		}
		if (PhotonNetwork.isMasterClient)
		{
			base.Invoke("RespawnAll", 1f);
		}
	}

	private void RespawnAll()
	{
		Kube.BCS.gameProcess = BattleControllerScript.GameProcess.game;
		for (int i = 0; i < Kube.BCS.playersInfo.Length; i++)
		{
			PlayerScript ps = Kube.BCS.playersInfo[i].ps;
			Vector3 pos = Kube.BCS.FindRespawnPlace(true);
			ps.SurvivalRespawn(pos);
		}
	}

	public override bool DropStuff(PlayerScript playerScript, Dictionary<int, int> _weaponPickup)
	{
		return false;
	}

	private float endWaitTime;

	private float waitTime = 30f;

	private bool initialized;

	private int playersDead;

	private bool started;

	protected PlayerScript[] bomber;
}
