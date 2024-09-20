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
		int num = 0;
		int num2 = int.MaxValue;
		for (int i = 0; i < 4; i++)
		{
			if (this._teamAvail[i])
			{
				num = Math.Max(Kube.BCS.playersInTeam[i], num);
				num2 = Math.Min(Kube.BCS.playersInTeam[i], num2);
			}
		}
		bool flag = false;
		if (Math.Abs(num - num2) > 2)
		{
			flag = true;
		}
		for (int j = 0; j < 4; j++)
		{
			TeamShortList teamShortList = this.teams[j];
			bool active = true;
			if (flag)
			{
				active = (Kube.BCS.playersInTeam[j] < num);
			}
			teamShortList.startButton.gameObject.SetActive(active);
			GameObject container = teamShortList.container;
			float num3 = 0f;
			for (int k = 0; k < playersInfo.Length; k++)
			{
				if (Kube.BCS.playersInfo[k].Team == j)
				{
					int serverId = playersInfo[k].serverId;
					GameObject gameObject2;
					if (!this._dict.ContainsKey(serverId))
					{
						gameObject2 = container.AddChild(this.rowPrefab);
						this._dict[serverId] = gameObject2;
					}
					else
					{
						gameObject2 = this._dict[serverId];
					}
					Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(gameObject2.transform, false);
					list.Remove(gameObject2);
					Vector3 localPosition = gameObject2.transform.localPosition;
					localPosition.y = num3;
					num3 -= bounds.size.y;
					gameObject2.transform.localPosition = localPosition;
					TabRow component = gameObject2.GetComponent<TabRow>();
					component.id = serverId;
					component.UID = playersInfo[k].UID;
					component.name.text = AuxFunc.DecodeRussianName(playersInfo[k].Name);
					component.isCurrent = (playersInfo[k].serverId == Kube.SS.serverId);
					int num4 = Mathf.Min(playersInfo[k].Level, Kube.ASS2.RankTex.Length - 1);
					component.rank.mainTexture = Kube.ASS2.RankTex[num4].mainTexture;
				}
			}
		}
		for (int l = 0; l < list.Count; l++)
		{
			GameObject gameObject3 = list[l];
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
		int team = Array.IndexOf<TeamShortList>(this.teams, UIButton.current.transform.parent.GetComponent<TeamShortList>());
		if (teamControllerBase)
		{
			teamControllerBase.EnterGame(team);
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
