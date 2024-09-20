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
			this.team,
			this.dm,
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

	public void OnShop()
	{
		if (Kube.BCS.gameMode == GameMode.creating)
		{
			Cub2Menu.instance.OpenTab("Decor_menu");
		}
		else
		{
			Cub2Menu.instance.OpenTab("Arsenal_menu");
		}
	}

	private void onExitSaveCb()
	{
		if (MessageBox.current.modalResult == 1)
		{
			Kube.BCS.SaveMapAndExit();
		}
		else
		{
			Kube.BCS.ExitGame();
		}
	}

	public void OnExitCb()
	{
		if (MessageBox.current.modalResult == 0)
		{
			return;
		}
		if (Kube.BCS.gameType == GameType.survival || Kube.BCS.gameType == GameType.hunger)
		{
			Kube.BCS.EndGame(BattleControllerScript.EndGameType.exit);
		}
		else if (Kube.BCS.gameType == GameType.creating && Kube.BCS.isMapOwner)
		{
			MessageBox messageBox = Cub2UI.YesNo(Localize.save_changes, new EventDelegate.Callback(this.onExitSaveCb));
		}
		else
		{
			Kube.BCS.ExitGame();
		}
	}

	public void OnExit()
	{
		MessageBox messageBox = Cub2UI.FindAndOpenDialog<MessageBox>("dialog_yesno");
		messageBox.title.text = Localize.exitGame;
		EventDelegate.Callback call = new EventDelegate.Callback(this.OnExitCb);
		messageBox.handler = new EventDelegate(call);
	}

	public GameObject creating;

	public GameObject dm;

	public GameObject survival;

	public GameObject team;

	public GameObject mission;

	protected GameObject[] tabs;

	protected GameObject _tab;
}
