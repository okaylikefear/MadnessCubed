using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class SurvTab : Tab
{
	private void Update()
	{
		if (Kube.BCS.playersInfo == null)
		{
			return;
		}
		this.head.info.text = Kube.BCS.survivalWaveNum.ToString();
		base.UpdateTimer();
		BattleControllerScript.PlayerInfo[] playersInfo = Kube.BCS.playersInfo;
		List<GameObject> list = new List<GameObject>();
		foreach (object obj in this.container.gameObject.transform)
		{
			Transform transform = (Transform)obj;
			list.Add(transform.gameObject);
		}
		float num = 0f;
		string b = Kube.SN.platform.ToString();
		for (int i = 0; i < playersInfo.Length; i++)
		{
			int serverId = playersInfo[i].serverId;
			GameObject gameObject;
			if (!this._dict.ContainsKey(serverId))
			{
				gameObject = this.container.AddChild(this.rowPrefab);
				this._dict[serverId] = gameObject;
			}
			else
			{
				gameObject = this._dict[serverId];
			}
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(gameObject.transform, false);
			list.Remove(gameObject);
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.y = num;
			num -= bounds.size.y;
			gameObject.transform.localPosition = localPosition;
			TabRow component = gameObject.GetComponent<TabRow>();
			component.id = serverId;
			if (playersInfo[i].sn == b)
			{
				component.UID = playersInfo[i].UID;
			}
			component.name.text = AuxFunc.DecodeRussianName(playersInfo[i].Name);
			component.isCurrent = (playersInfo[i].serverId == Kube.SS.serverId);
			int num2 = Mathf.Min(playersInfo[i].Level, Kube.ASS2.RankTex.Length - 1);
			component.rank.mainTexture = Kube.ASS2.RankTex[num2].mainTexture;
			component.cols[2].text = playersInfo[i].Score.ToString();
			component.cols[3].text = playersInfo[i].Frags.ToString();
			component.cols[4].text = playersInfo[i].Deaths.ToString();
		}
		for (int j = 0; j < list.Count; j++)
		{
			GameObject gameObject2 = list[j];
			gameObject2.SetActive(false);
			UnityEngine.Object.Destroy(gameObject2);
		}
	}

	public TabHead head;

	protected Dictionary<int, GameObject> _dict = new Dictionary<int, GameObject>();
}
