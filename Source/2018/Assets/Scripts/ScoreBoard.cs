using System;
using kube;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
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
		this.tabs = new Tab[]
		{
			null,
			this.creating,
			this.dm,
			this.survival,
			this.team,
			this.dm,
			this.team,
			this.team,
			this.team,
			this.dm,
			this.team
		};
		for (int i = 0; i < this.tabs.Length; i++)
		{
			if (this.tabs[i])
			{
				this.tabs[i].gameObject.SetActive(false);
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
			this._tab.gameObject.SetActive(false);
		}
		this._tab = this.tabs[gameType];
		if (this.tabs[gameType] == null)
		{
			return;
		}
		this._tab.gameObject.SetActive(true);
	}

	private void OnEnable()
	{
		this.BeginGame();
	}

	public Tab creating;

	public Tab dm;

	public Tab survival;

	public Tab team;

	protected Tab[] tabs;

	protected Tab _tab;
}
