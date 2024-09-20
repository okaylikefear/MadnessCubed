using System;
using System.Collections;
using kube;
using UnityEngine;

public class GTClanWar : TeamShooterController
{
	public int[] clanids
	{
		get
		{
			return this._clanids;
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		this.nnplayers = 3;
	}

	private void OnPhotonPlayerConnected(PhotonPlayer otherPlayer)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		string text = (string)PhotonNetwork.room.customProperties["pw"];
		int num = Kube.OH.DecodeServerCode(text.Substring(1, 3));
		int num2 = Kube.OH.DecodeServerCode(text.Substring(5, 3));
		if (num == Kube.GPS.clan.id && num2 == 0 && PhotonNetwork.room.playerCount <= 2)
		{
			if (PhotonNetwork.player.customProperties.ContainsKey("clan"))
			{
				num2 = (int)otherPlayer.customProperties["clan"];
			}
			int num3 = (int)Enum.Parse(typeof(socialNetType), (string)otherPlayer.customProperties["sn"]);
			text = Kube.OH.GetServerCode((int)Kube.SN.sn, 1) + Kube.OH.GetServerCode(num, 3) + Kube.OH.GetServerCode(num3, 1) + Kube.OH.GetServerCode(num2, 3);
			PhotonNetwork.room.customProperties["pw"] = text;
			PhotonNetwork.room.SetCustomProperties(PhotonNetwork.room.customProperties, null, false);
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer other)
	{
		if (PhotonNetwork.isMasterClient || other.isMasterClient)
		{
			if (this._started)
			{
				return;
			}
			if (PhotonNetwork.room.playerCount > 1)
			{
				return;
			}
			int id = Kube.GPS.clan.id;
			int num = 0;
			string value = Kube.OH.GetServerCode((int)Kube.SN.sn, 1) + Kube.OH.GetServerCode(id, 3) + Kube.OH.GetServerCode(0, 1) + Kube.OH.GetServerCode(num, 3);
			PhotonNetwork.room.customProperties["pw"] = value;
			PhotonNetwork.room.SetCustomProperties(PhotonNetwork.room.customProperties, null, false);
			Kube.BCS.ps.SetTeam(0);
		}
	}

	public override void BeginGame()
	{
		base.StartCoroutine(this._BeginGame());
	}

	public IEnumerator _BeginGame()
	{
		int team = -1;
		for (;;)
		{
			string pw = (string)PhotonNetwork.room.customProperties["pw"];
			this._clanids[0] = Kube.OH.DecodeServerCode(pw.Substring(1, 3));
			this._clanids[1] = Kube.OH.DecodeServerCode(pw.Substring(5, 3));
			if (PhotonNetwork.isMasterClient)
			{
				break;
			}
			if (this._clanids[0] == Kube.GPS.clan.id || this._clanids[1] == Kube.GPS.clan.id)
			{
				break;
			}
			if (this._clanids[1] != 0)
			{
				break;
			}
			yield return new WaitForSeconds(0.1f);
		}
		if (Kube.GPS.clan.id == this._clanids[0])
		{
			team = 0;
		}
		else
		{
			if (Kube.GPS.clan.id != this._clanids[1])
			{
				yield break;
			}
			team = 1;
		}
		if (Kube.BCS.playersInTeam[team] >= this.nnplayers)
		{
			yield break;
		}
		this.EnterGame(team);
		yield break;
	}

	protected override void UpdateTick()
	{
		if (!this._started)
		{
			if (PhotonNetwork.room.playerCount < this.nnplayers * 2)
			{
				this.UpdateHUD();
				return;
			}
			if (PhotonNetwork.isMasterClient && Kube.BCS.playersInTeam[0] == Kube.BCS.playersInTeam[1] && Kube.BCS.playersInTeam[0] == this.nnplayers)
			{
				Kube.BCS.NO.RoundRestart(4);
			}
		}
		base.UpdateTick();
	}

	protected override void UpdateHUD()
	{
		if (this._started)
		{
			Kube.BCS.hud.timer.timer = Mathf.FloorToInt((float)Kube.BCS.gameEndTime - Time.realtimeSinceStartup);
		}
		else
		{
			Kube.BCS.hud.timer.label.text = "---";
		}
	}

	protected override IEnumerator _EndRoundAndRestart()
	{
		this._started = true;
		for (int i = 0; i < Kube.BCS.teamScore.Length; i++)
		{
			Kube.BCS.teamScore[i] = 0;
		}
		Kube.BCS.gameStartTime = (float)((int)Time.realtimeSinceStartup);
		Kube.BCS.gameEndTime = (int)Time.realtimeSinceStartup + Kube.OH.gameMaxTime[10];
		Kube.BCS.gameProcess = BattleControllerScript.GameProcess.game;
		PlayerScript ps = Kube.BCS.ps;
		Kube.BCS.ps.kills = 0;
		Vector3 pos = base.findSpawnPos(ps.team);
		Kube.BCS.ps.SurvivalRespawn(pos);
		if (PhotonNetwork.isMasterClient)
		{
			Kube.BCS.NO.SendMeGameParams(0);
		}
		yield break;
	}

	protected int[] _clanids = new int[2];

	protected bool _started;

	public int nnplayers = 1;
}
