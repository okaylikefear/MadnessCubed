using System;
using kube;
using UnityEngine;

public class SurvivalController : GameTypeControllerBase
{
	private void Start()
	{
		this.hud = Kube.BCS.hud;
		this.hud.timer.gameObject.SetActive(false);
	}

	private new void UpdateHUD()
	{
		int num = Mathf.FloorToInt((Time.time - Kube.BCS.gameStartTime) / 60f);
		this.hud.curstat.values[0].value = Kube.BCS.currentNumPlayers;
		this.hud.curstat.values[1].value = Kube.BCS.survivalWaveNum + 1;
		this.hud.curstat.values[2].value = Kube.BCS.currentNumDeadPlayers;
		this.hud.curstat.values[3].value = Kube.BCS.currentNumMonsters;
		if (Kube.BCS.survivalTime < Kube.BCS.survivalPrewaveTime)
		{
			this.hud.SurvTimer.gameObject.SetActive(true);
			this.hud.SurvTimer.timer = (int)(Kube.BCS.survivalPrewaveTime - Kube.BCS.survivalTime) - 60 * num;
		}
		else
		{
			this.hud.SurvTimer.gameObject.SetActive(false);
			this.hud.SurvTimer.timer = Mathf.FloorToInt((float)Kube.BCS.gameEndTime - Time.realtimeSinceStartup);
		}
	}

	private void Update()
	{
		this.UpdateHUD();
	}

	protected UIHUD hud;
}
