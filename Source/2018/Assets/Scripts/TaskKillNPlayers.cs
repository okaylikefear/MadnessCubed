using System;
using System.Collections.Generic;
using kube;
using kube.data;

public class TaskKillNPlayers : TaskBase
{
	private void PlayerDeadByMe(object[] pp)
	{
		if (this.headshot && !(bool)pp[2])
		{
			return;
		}
		short num = (short)pp[1];
		int num2 = -1;
		if (num >= 0)
		{
			num2 = (int)Kube.IS.weaponParams[(int)num].weaponGroup;
		}
		if (this.weaponId != -1 && (int)num != this.weaponId)
		{
			return;
		}
		if (this.weaponGroup != -1 && num2 != this.weaponGroup)
		{
			return;
		}
		this.playersKilled++;
	}

	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
	}

	public override void configure(object[] config)
	{
		this.playersKilled = 0;
		this.initialized = true;
		this.frags = (int)config[0];
		if (config.Length > 1)
		{
			if (config[1] is int)
			{
				this.weaponId = (int)config[1];
			}
			else
			{
				this.weaponGroup = (int)(((string)config[1])[1] - '0');
			}
		}
		if (config.Length > 2)
		{
			this.headshot = ((int)config[2] == 1);
		}
	}

	private void Start()
	{
		this.Init();
	}

	private void Update()
	{
		if (Kube.BCS == null)
		{
			return;
		}
		if (Kube.BCS.gameProcess != BattleControllerScript.GameProcess.game)
		{
			return;
		}
		this.UpdateHUD();
		if (this.playersKilled >= this.frags)
		{
			base.Done();
		}
	}

	private void UpdateHUD()
	{
	}

	public override void EndGame(bool gameComplete)
	{
		this.taskDesc.progress[0] = (int)this.taskDesc.progress[0] + this.playersKilled;
		this.playersKilled = 0;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["task"] = this.taskDesc.id.ToString();
		dictionary["score"] = this.taskDesc.score.ToString();
		dictionary["progress"] = TaskBox.EncodeProgress(this.taskDesc.progress);
		Kube.SS.Request(301, dictionary, null);
	}

	public int playersKilled;

	protected int frags;

	protected int weaponId = -1;

	protected int weaponGroup = -1;

	protected bool headshot;

	private bool initialized;
}
