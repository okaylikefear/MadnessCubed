using System;
using kube;
using UnityEngine;

public class RoundGameType : GameTypeControllerBase
{
	public void Restart()
	{
		if (this.rgp != RoundGameType.RoundGameProcess.restart)
		{
			return;
		}
		this.rgp = RoundGameType.RoundGameProcess.game;
		this._Restart();
		if (Cub2UI.currentMenu)
		{
			Cub2UI.currentMenu.SetActive(false);
		}
	}

	protected virtual void _Restart()
	{
	}

	public void EndRound()
	{
		if (this.rgp != RoundGameType.RoundGameProcess.game)
		{
			return;
		}
		this.rgp = RoundGameType.RoundGameProcess.end;
		Kube.BCS.EndGame(BattleControllerScript.EndGameType.endRound, true);
	}

	protected void EndRoundUpdate()
	{
		if (this.rgp != RoundGameType.RoundGameProcess.end)
		{
			return;
		}
		if ((float)Kube.BCS.gameEndTime < Time.realtimeSinceStartup && (float)(Kube.BCS.gameEndTime + this.timeBetweenRounds) < Time.realtimeSinceStartup)
		{
			this.rgp = RoundGameType.RoundGameProcess.restart;
			if (PhotonNetwork.isMasterClient)
			{
				Kube.BCS.NO.RequestToRestart();
			}
		}
	}

	public int timeBetweenRounds = 30;

	public RoundGameType.RoundGameProcess rgp;

	public enum RoundGameProcess
	{
		game,
		end,
		restart
	}
}
