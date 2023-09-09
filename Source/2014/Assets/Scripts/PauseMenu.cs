using System;
using kube;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	private void Start()
	{
		if (this.tabs == null)
		{
			this.Init();
		}
	}

	private void Init()
	{
		this.tabs = new GameObject[]
		{
			this.creating,
			this.creating,
			this.dm,
			this.survival,
			this.team,
			this.mission,
			this.team,
			this.team,
			this.team
		};
		for (int i = 0; i < this.tabs.Length; i++)
		{
			if (this.tabs[i])
			{
				this.tabs[i].SetActive(false);
			}
		}
	}

	private void BeginGame()
	{
		if (this.tabs == null)
		{
			this.Init();
		}
		int gameType = (int)Kube.BCS.gameType;
		if (this._tab)
		{
			this._tab.SetActive(false);
		}
		this._tab = this.tabs[gameType];
		if (this.tabs[gameType] == null)
		{
			return;
		}
		this._tab.SetActive(true);
	}

	private void OnEnable()
	{
		this.BeginGame();
	}

	public GameObject creating;

	public GameObject dm;

	public GameObject survival;

	public GameObject team;

	public GameObject mission;

	protected GameObject[] tabs;

	protected GameObject _tab;
}
