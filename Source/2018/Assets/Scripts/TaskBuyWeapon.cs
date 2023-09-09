using System;
using kube;
using UnityEngine;

public class TaskBuyWeapon : TaskBase
{
	private void Start()
	{
		this.weaponId = (int)this.desc.config[0];
		this.Check();
	}

	protected void Check()
	{
		if (Kube.GPS.inventarWeapons[this.weaponId] > 0)
		{
			this.completed = true;
		}
	}

	private void Update()
	{
		this.Check();
		if (this.completed)
		{
			base.Done();
			TaskDoneDialog taskDoneDialog = Cub2Menu.Find<TaskDoneDialog>("dialog_task_done");
			EndGameStats endGameStats = default(EndGameStats);
			taskDoneDialog.Open(Kube.OH.task, endGameStats);
			Kube.OH.task = null;
			UnityEngine.Object.Destroy(base.gameObject, 2f);
		}
	}

	private int weaponId;
}
