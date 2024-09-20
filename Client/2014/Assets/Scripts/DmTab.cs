using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class DmTab : Tab
{
	private void Update()
	{
		if (Kube.BCS.playersInfo == null)
		{
			return;
		}
		base.UpdateTimer();
		BattleControllerScript.PlayerInfo[] playersInfo = Kube.BCS.playersInfo;
		List<GameObject> list = new List<GameObject>();
		foreach (object obj in this.container.gameObject.transform)
		{
			Transform transform = (Transform)obj;
			list.Add(transform.gameObject);
		}
		float num = 0f;
		for (int i = 0; i < playersInfo.Length; i++)
		{
			int id = playersInfo[i].Id;
			GameObject gameObject;
			if (!this._dict.ContainsKey(id))
			{
				gameObject = NGUITools.AddChild(this.container, this.rowPrefab);
				this._dict[id] = gameObject;
			}
			else
			{
				gameObject = this._dict[id];
			}
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(gameObject.transform, false);
			list.Remove(gameObject);
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.y = num;
			num -= bounds.size.y;
			gameObject.transform.localPosition = localPosition;
			TabRow component = gameObject.GetComponent<TabRow>();
			component.id = id;
			component.UID = playersInfo[i].UID;
			component.name.text = AuxFunc.DecodeRussianName(playersInfo[i].Name);
			component.isCurrent = (playersInfo[i].Id == Kube.GPS.playerId);
			int num2 = Mathf.Min(playersInfo[i].Level, Kube.ASS2.RankTex.Length - 1);
			component.rank.mainTexture = Kube.ASS2.RankTex[num2].mainTexture;
			component.cols[2].text = playersInfo[i].Frags.ToString();
			component.cols[3].text = playersInfo[i].Deaths.ToString();
		}
		for (int j = 0; j < list.Count; j++)
		{
			GameObject gameObject2 = list[j];
			gameObject2.SetActive(false);
			UnityEngine.Object.Destroy(gameObject2);
		}
	}

	protected Dictionary<int, GameObject> _dict = new Dictionary<int, GameObject>();
}
