using System;
using kube;
using kube.data;
using UnityEngine;

public class MissionPause : MonoBehaviour
{
	private void OnEnable()
	{
		MissionDesc missionDesc = MissionBox.FindMissionById(Kube.OH.tempMap.missionId);
		this.mission.text = PlayDialog.GetMissionDesc(Kube.OH, missionDesc);
	}

	public UILabel mission;

	public UITexture tx;
}
