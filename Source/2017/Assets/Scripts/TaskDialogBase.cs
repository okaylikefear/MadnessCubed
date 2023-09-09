using System;
using kube.data;
using UnityEngine;

public class TaskDialogBase : MonoBehaviour
{
	protected int calcNameIndex(int index)
	{
		int result = index % 10;
		if (index > 9 && index < 21)
		{
			return 0;
		}
		return result;
	}

	protected string formatTime(int index)
	{
		int num = this.calcNameIndex(index);
		if (index > 60)
		{
			index /= 60;
			num = this.calcNameIndex(index);
			return string.Format("{0} {1} ", index, this.MinuteNames_RU[num]);
		}
		return string.Format("{0} {1} ", index, this.SecondNames_RU[num]);
	}

	protected string TaskGainBonus(object[] config)
	{
		return string.Format(Localize.TaskType[1], Localize.bonusName[(int)config[1]]);
	}

	protected string TaskHoldNSecond(object[] config)
	{
		int index = (int)config[1];
		return string.Format(Localize.TaskType[2], this.formatTime(index));
	}

	protected string TaskKillNPlayers(object[] config)
	{
		int num = (int)config[0];
		int num2 = -1;
		int num3 = -1;
		if (config[1] is int)
		{
			num2 = (int)config[1];
		}
		else
		{
			num3 = (int)(((string)config[1])[1] - '0');
		}
		int num4 = (int)config[2];
		if (num4 > 0)
		{
			if (num3 >= 0)
			{
				return string.Format(Localize.DailyTask[3], Localize.T("ui_wp" + num3, null));
			}
			return string.Format(Localize.DailyTask[2], num);
		}
		else
		{
			if (num3 >= 0)
			{
				return string.Format(Localize.DailyTask[1], Localize.T("ui_wp" + num3, null));
			}
			if (num2 == -3)
			{
				return Localize.DailyTask[4];
			}
			if (num2 >= 0)
			{
				return string.Format(Localize.DailyTask[1], Localize.weaponNames[num2]);
			}
			return string.Format(Localize.TaskType[4], num);
		}
	}

	protected string TaskKillNMonsters(object[] config)
	{
		int num = (int)config[0];
		int num2 = this.calcNameIndex(num);
		if ((int)config[1] >= 0)
		{
			return string.Format(Localize.TaskType[3], num, this.MonsterNames_RU[num2] + " - " + Localize.monsterName[(int)config[1]]);
		}
		return string.Format(Localize.TaskType[3], num, this.MonsterNames_RU[num2]);
	}

	protected string TaskKillNMonstersNSecond(object[] config)
	{
		int num = (int)config[0];
		int num2 = this.calcNameIndex(num);
		int index = (int)config[1];
		return string.Format(Localize.TaskType[6], num, this.MonsterNames_RU[num2], this.formatTime(index));
	}

	protected string TaskBuyWeapon(object[] config)
	{
		int num = (int)config[0];
		return string.Format(Localize.TaskType[7], Localize.weaponNames[num]);
	}

	protected string TaskBuyItem(object[] config)
	{
		int num = (int)config[0];
		int num2 = (int)config[2];
		int num3 = this.calcNameIndex(num2);
		return string.Format(Localize.TaskType[8], num2, this.ItemNames_RU[num3], Localize.gameItemsNames[num]);
	}

	public void Open(TaskDesc taskDesc)
	{
		this.taskDesc = taskDesc;
		base.gameObject.SetActive(true);
	}

	protected TaskDialogBase.TaskDescGet[] TaskTypeDesc;

	protected string[] SecondNames_RU = new string[]
	{
		Localize.seconds,
		Localize.secondu,
		Localize.secondy,
		Localize.secondy,
		Localize.secondy,
		Localize.seconds,
		Localize.seconds,
		Localize.seconds,
		Localize.seconds,
		Localize.seconds
	};

	protected string[] MinuteNames_RU = new string[]
	{
		Localize.minutes,
		Localize.minutu,
		Localize.minuty,
		Localize.minuty,
		Localize.minuty,
		Localize.minutes,
		Localize.minutes,
		Localize.minutes,
		Localize.minutes,
		Localize.minutes
	};

	protected string[] MonsterNames_RU = new string[]
	{
		Localize.monsters,
		Localize.monstra,
		Localize.monstra,
		Localize.monstra,
		Localize.monstra,
		Localize.monsters,
		Localize.monsters,
		Localize.monsters,
		Localize.monsters,
		Localize.monsters
	};

	protected string[] ItemNames_RU = new string[0];

	[NonSerialized]
	public TaskDesc taskDesc;

	protected delegate string TaskDescGet(object[] config);
}
