using System;
using System.Collections.Generic;
using kube;
using kube.data;
using UnityEngine;

public class TaskBase : MonoBehaviour
{
	public static void Validate()
	{
		if (Kube.OH.tasks == null)
		{
			return;
		}
		if (TaskBase._go == null)
		{
			TaskBase._go = new GameObject("Tasks");
			UnityEngine.Object.DontDestroyOnLoad(TaskBase._go);
			TaskBase._go.transform.parent = Kube.OH.transform;
		}
		foreach (TaskBase taskBase in TaskBase._go.GetComponentsInChildren<TaskBase>())
		{
			if (taskBase.taskDesc.isCompleted)
			{
				UnityEngine.Object.Destroy(taskBase);
			}
		}
		for (int j = 0; j < Kube.OH.tasks.Length; j++)
		{
			TaskDesc taskDesc = Kube.OH.tasks[j];
			TaskType type = taskDesc.type;
			if (TaskBase._tasks.ContainsKey(taskDesc))
			{
				TaskBase taskBase2 = TaskBase._tasks[taskDesc];
				if (taskDesc.isCompleted)
				{
					UnityEngine.Object.Destroy(taskBase2);
				}
			}
			else
			{
				TaskBase taskBase2;
				if (type == TaskType.gainBonus)
				{
					taskBase2 = TaskBase._go.AddComponent<TaskGainBonus>();
				}
				else if (type == TaskType.holdNSeconds)
				{
					taskBase2 = TaskBase._go.AddComponent<TaskHoldNSecond>();
				}
				else if (type == TaskType.killNPlayers)
				{
					taskBase2 = TaskBase._go.AddComponent<TaskKillNPlayers>();
				}
				else if (type == TaskType.killNMonsters)
				{
					taskBase2 = TaskBase._go.AddComponent<TaskKillNMonsters>();
				}
				else if (taskDesc.type == TaskType.buyItem)
				{
					taskBase2 = TaskBase._go.AddComponent<TaskBuyItem>();
				}
				else if (taskDesc.type == TaskType.buyWeapon)
				{
					taskBase2 = TaskBase._go.AddComponent<TaskBuyWeapon>();
				}
				else if (taskDesc.type == TaskType.daily3)
				{
					taskBase2 = TaskBase._go.AddComponent<TaskDayli3>();
				}
				else
				{
					taskBase2 = TaskBase._go.AddComponent<TaskBase>();
				}
				taskBase2.taskDesc = taskDesc;
				taskBase2.configure(taskDesc.config);
				TaskBase._tasks[taskDesc] = taskBase2;
			}
		}
	}

	public static TaskBase Get(TaskDesc taskDesc)
	{
		return TaskBase._tasks[taskDesc];
	}

	protected void Done()
	{
		this.completed = true;
		if (Kube.BCS != null)
		{
			Kube.BCS.hud.tasks.timer.gameObject.SetActive(false);
			Kube.BCS.hud.tasks.counter.gameObject.SetActive(false);
		}
		base.enabled = false;
	}

	public virtual void configure(object[] config)
	{
	}

	public virtual void EndGame(bool gameComplete)
	{
	}

	public TaskDesc desc;

	protected static GameObject _go;

	protected static Dictionary<TaskDesc, TaskBase> _tasks = new Dictionary<TaskDesc, TaskBase>();

	public bool completed;

	protected TaskDesc taskDesc;
}
