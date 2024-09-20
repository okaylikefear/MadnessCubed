using System;
using System.Collections.Generic;
using kube;
using kube.data;

public class TaskDayli3 : TaskBase
{
	private void PlayerDeadByMe(object[] pp)
	{
		this.playersKilled++;
		short num = (short)pp[1];
		if (num == -3)
		{
			this.playersKilledGrenade++;
		}
		if ((bool)pp[2])
		{
			this.playersKilledHeadshot++;
			this.playersKilledHeadshotBy[(int)Kube.IS.weaponParams[(int)((short)pp[1])].weaponGroup]++;
		}
		if ((short)pp[1] >= 0)
		{
			this.playersKilledBy[(int)Kube.IS.weaponParams[(int)((short)pp[1])].weaponGroup]++;
		}
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
		List<SubTaskDesc> list = new List<SubTaskDesc>();
		for (int i = 0; i < config.Length; i++)
		{
			if (config[i] == null)
			{
				break;
			}
			string text = (string)config[i];
			char c = text[0];
			int p = 0;
			int num = text.IndexOf(':');
			int p2;
			if (num != -1)
			{
				p = int.Parse(text.Substring(1, num - 1));
				p2 = int.Parse(text.Substring(num + 1));
			}
			else
			{
				p2 = int.Parse(text.Substring(1));
			}
			list.Add(new SubTaskDesc((int)(c - 'a'), p, p2));
		}
		this.subtask = list.ToArray();
	}

	private void Start()
	{
		this.Init();
	}

	public override void EndGame(bool gameComplete)
	{
		for (int i = 0; i < this.subtask.Length; i++)
		{
			if (this.subtask[i].type == 0)
			{
				this.taskDesc.progress[i] = (int)this.taskDesc.progress[i] + this.playersKilled;
			}
			if (this.subtask[i].type == 1)
			{
				this.taskDesc.progress[i] = (int)this.taskDesc.progress[i] + this.playersKilledBy[this.subtask[i].kind];
			}
			if (this.subtask[i].type == 2)
			{
				this.taskDesc.progress[i] = (int)this.taskDesc.progress[i] + this.playersKilledHeadshot;
			}
			if (this.subtask[i].type == 3)
			{
				this.taskDesc.progress[i] = (int)this.taskDesc.progress[i] + this.playersKilledHeadshotBy[this.subtask[i].kind];
			}
			if (this.subtask[i].type == 4)
			{
				this.taskDesc.progress[i] = (int)this.taskDesc.progress[i] + this.playersKilledGrenade;
			}
		}
		this.playersKilled = 0;
		this.playersKilledBy = new int[this.playersKilledBy.Length];
		this.playersKilledHeadshot = 0;
		this.playersKilledHeadshotBy = new int[this.playersKilledHeadshotBy.Length];
		this.playersKilledGrenade = 0;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["task"] = this.taskDesc.id.ToString();
		dictionary["score"] = this.taskDesc.score.ToString();
		dictionary["progress"] = TaskBox.EncodeProgress(this.taskDesc.progress);
		Kube.SS.Request(301, dictionary, null);
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
	}

	private void UpdateHUD()
	{
	}

	public int playersKilled;

	public int playersKilledHeadshot;

	public int playersKilledGrenade;

	public int[] playersKilledBy = new int[10];

	public int[] playersKilledHeadshotBy = new int[10];

	public SubTaskDesc[] subtask;

	private bool initialized;

	public enum SubTaskType
	{
		kill,
		killBy,
		killHead,
		killHeadBy,
		killGren
	}
}
