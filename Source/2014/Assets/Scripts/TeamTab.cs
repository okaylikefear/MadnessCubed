using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class TeamTab : Tab
{
	private void Start()
	{
		this._headrows = new GameObject[4];
		for (int i = 0; i < this._headrows.Length; i++)
		{
			GameObject gameObject = NGUITools.AddChild(this.container, this.headerRowPrefab);
			this._headrows[i] = gameObject;
		}
	}

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
			if (transform.GetComponent<TabRow>())
			{
				list.Add(transform.gameObject);
			}
		}
		float num = 0f;
		for (int i = 0; i < 4; i++)
		{
			GameObject gameObject = this._headrows[i];
			gameObject.SetActive(Kube.BCS.playersInTeam[i] > 0);
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(gameObject.transform, false);
			TabHead component = gameObject.GetComponent<TabHead>();
			component.bg.spriteName = this.colorSprites[i];
			component.title.text = Localize.teamName[i];
			component.info.text = Kube.BCS.teamScore[i].ToString();
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.y = num;
			num -= bounds.size.y;
			gameObject.transform.localPosition = localPosition;
			for (int j = 0; j < playersInfo.Length; j++)
			{
				if (Kube.BCS.playersInfo[j].Team == i)
				{
					int id = playersInfo[j].Id;
					if (!this._dict.ContainsKey(id))
					{
						gameObject = NGUITools.AddChild(this.container, this.rowPrefab);
						this._dict[id] = gameObject;
					}
					else
					{
						gameObject = this._dict[id];
					}
					bounds = NGUIMath.CalculateRelativeWidgetBounds(gameObject.transform, false);
					list.Remove(gameObject);
					localPosition = gameObject.transform.localPosition;
					localPosition.y = num;
					num -= bounds.size.y;
					gameObject.transform.localPosition = localPosition;
					TabRow component2 = gameObject.GetComponent<TabRow>();
					component2.id = id;
					component2.UID = playersInfo[j].UID;
					component2.name.text = AuxFunc.DecodeRussianName(playersInfo[j].Name);
					component2.isCurrent = (playersInfo[j].Id == Kube.GPS.playerId);
					int num2 = Mathf.Min(playersInfo[j].Level, Kube.ASS2.RankTex.Length - 1);
					component2.rank.mainTexture = Kube.ASS2.RankTex[num2].mainTexture;
					component2.cols[2].text = playersInfo[j].Score.ToString();
					component2.cols[3].text = playersInfo[j].Frags.ToString();
					component2.cols[4].text = playersInfo[j].Deaths.ToString();
				}
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			GameObject gameObject2 = list[k];
			gameObject2.SetActive(false);
			UnityEngine.Object.Destroy(gameObject2);
		}
	}

	protected Dictionary<int, GameObject> _dict = new Dictionary<int, GameObject>();

	protected GameObject[] _headrows;

	public string[] colorSprites;
}
