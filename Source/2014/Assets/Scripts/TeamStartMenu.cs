using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class TeamStartMenu : MonoBehaviour
{
	public void Initialize()
	{
		if (this.initialized)
		{
			return;
		}
		this.respawnsRed = GameObject.FindGameObjectsWithTag("RespawnRed");
		this.respawnsBlue = GameObject.FindGameObjectsWithTag("RespawnBlue");
		this.respawnsGreen = GameObject.FindGameObjectsWithTag("RespawnGreen");
		this.respawnsYellow = GameObject.FindGameObjectsWithTag("RespawnYellow");
		this._teamAvail = new bool[]
		{
			this.respawnsRed.Length > 0,
			this.respawnsBlue.Length > 0,
			this.respawnsGreen.Length > 0,
			this.respawnsYellow.Length > 0
		};
		this.initialized = true;
	}

	private void Start()
	{
		this.Initialize();
	}

	public void BeginPlay()
	{
		this._teamAvail = new bool[]
		{
			this.respawnsRed.Length > 0,
			this.respawnsBlue.Length > 0,
			this.respawnsGreen.Length > 0,
			this.respawnsYellow.Length > 0
		};
	}

	private void Update()
	{
		if (Kube.BCS.playersInfo == null)
		{
			return;
		}
		BattleControllerScript.PlayerInfo[] playersInfo = Kube.BCS.playersInfo;
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject gameObject in this._dict.Values)
		{
			if (gameObject && gameObject.activeSelf)
			{
				list.Add(gameObject);
			}
		}
		for (int i = 0; i < 4; i++)
		{
			TeamShortList teamShortList = this.teams[i];
			GameObject container = teamShortList.container;
			float num = 0f;
			for (int j = 0; j < playersInfo.Length; j++)
			{
				if (Kube.BCS.playersInfo[j].Team == i)
				{
					int id = playersInfo[j].Id;
					GameObject gameObject2;
					if (!this._dict.ContainsKey(id))
					{
						gameObject2 = NGUITools.AddChild(container, this.rowPrefab);
						this._dict[id] = gameObject2;
					}
					else
					{
						gameObject2 = this._dict[id];
					}
					Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(gameObject2.transform, false);
					list.Remove(gameObject2);
					Vector3 localPosition = gameObject2.transform.localPosition;
					localPosition.y = num;
					num -= bounds.size.y;
					gameObject2.transform.localPosition = localPosition;
					TabRow component = gameObject2.GetComponent<TabRow>();
					component.id = id;
					component.UID = playersInfo[j].UID;
					component.name.text = AuxFunc.DecodeRussianName(playersInfo[j].Name);
					component.isCurrent = (playersInfo[j].Id == Kube.GPS.playerId);
					int num2 = Mathf.Min(playersInfo[j].Level, Kube.ASS2.RankTex.Length - 1);
					component.rank.mainTexture = Kube.ASS2.RankTex[num2].mainTexture;
				}
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			GameObject gameObject3 = list[k];
			gameObject3.SetActive(false);
			UnityEngine.Object.Destroy(gameObject3);
		}
	}

	private void OnEnable()
	{
		this.Initialize();
		for (int i = 0; i < this.teams.Length; i++)
		{
			this.teams[i].gameObject.SetActive(this._teamAvail[i]);
			this.teams[i].title.text = Localize.teamName[i];
		}
		base.GetComponentInChildren<UIGrid>().Reposition();
	}

	public void OnJoinTeam()
	{
		TeamControllerBase teamControllerBase = Kube.BCS.gameTypeController as TeamControllerBase;
		if (teamControllerBase)
		{
			teamControllerBase.EnterGame();
		}
		base.gameObject.SetActive(false);
	}

	protected bool initialized;

	protected GameObject[] respawnsRed;

	protected GameObject[] respawnsBlue;

	protected GameObject[] respawnsGreen;

	protected GameObject[] respawnsYellow;

	protected bool[] _teamAvail;

	public TeamShortList[] teams;

	public GameObject rowPrefab;

	protected Dictionary<int, GameObject> _dict = new Dictionary<int, GameObject>();
}
