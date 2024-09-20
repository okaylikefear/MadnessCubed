using System;
using kube;
using UnityEngine;

public class TaskHoldNSecond : TaskBase
{
	private void Start()
	{
		this.endTime = (float)((int)Kube.OH.task.config[1]);
	}

	private void Update()
	{
		if (Kube.BCS.gameProcess != BattleControllerScript.GameProcess.game)
		{
			return;
		}
		this.UpdateHUD();
		if (!this.playerDead)
		{
			this.aliveTime += Time.deltaTime;
		}
		if (this.aliveTime > this.endTime)
		{
			base.Done();
			return;
		}
	}

	private void UpdateHUD()
	{
		int num = (int)(this.endTime - Time.time);
	}

	private void PlayerDie()
	{
		this.playerDead = true;
	}

	private void PlayerRespawn()
	{
		this.playerDead = false;
	}

	private float aliveTime;

	private float endTime;

	protected bool playerDead;
}
