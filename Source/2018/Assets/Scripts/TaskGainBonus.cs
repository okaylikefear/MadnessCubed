using System;
using System.Collections.Generic;
using kube;
using kube.data;

public class TaskGainBonus : TaskBase
{
	private void GainBonus(int k)
	{
		if (k == this.bonus)
		{
			this.gained++;
		}
	}

	public override void configure(object[] config)
	{
		this.bonus = (int)config[1];
	}

	public override void EndGame(bool gameComplete)
	{
		this.taskDesc.progress[0] = (int)this.taskDesc.progress[0] + this.gained;
		this.gained = 0;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["task"] = this.taskDesc.id.ToString();
		dictionary["score"] = this.taskDesc.score.ToString();
		dictionary["progress"] = TaskBox.EncodeProgress(this.taskDesc.progress);
		Kube.SS.Request(301, dictionary, null);
	}

	private int bonus;

	private int gained;
}
