using System;
using kube;
using kube.data;
using UnityEngine;

public class Tab : MonoBehaviour
{
	private void Start()
	{
		this.UpdateTitle();
	}

	private void OnEnable()
	{
		this.UpdateTitle();
	}

	private void UpdateTitle()
	{
		if (!this.title)
		{
			return;
		}
		this.title.text = Localize.gameTypeStr[(int)Kube.BCS.gameType];
		if (Kube.OH.tempMap.Id < 0L)
		{
			long num = -Kube.OH.tempMap.Id;
			if (num < (long)Localize.buildinMapName.Length)
			{
				this.mapname.text = Localize.buildinMapName[(int)(checked((IntPtr)num))];
			}
			else
			{
				this.mapname.text = MissionBox.FindMissionById(Kube.OH.tempMap.missionId).title;
			}
		}
		else
		{
			this.mapname.text = Localize.self_map;
		}
	}

	protected void UpdateTimer()
	{
		if (!this.timer)
		{
			return;
		}
		int num = 0;
		if (Kube.BCS.gameProcess == BattleControllerScript.GameProcess.game)
		{
			num = Kube.BCS.gameEndTime - (int)Time.realtimeSinceStartup;
		}
		else if (Kube.BCS.gameProcess == BattleControllerScript.GameProcess.end)
		{
			int num2 = 0;
			if (Kube.BCS.gameTypeController is RoundGameType)
			{
				num2 = (Kube.BCS.gameTypeController as RoundGameType).timeBetweenRounds;
			}
			num = num2 - ((int)Time.realtimeSinceStartup - Kube.BCS.gameEndTime);
		}
		if (num < 0)
		{
			num = 0;
		}
		int num3 = num % 60;
		int num4 = num / 60;
		this.timer.text = string.Format("{0:00}:{1:00}", num4, num3);
	}

	public GameObject rowPrefab;

	public GameObject headerRowPrefab;

	public GameObject container;

	public UILabel title;

	public UILabel mapname;

	public UILabel timer;
}
