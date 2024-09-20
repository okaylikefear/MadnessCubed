using System;
using kube;
using UnityEngine;

public class ExitMenu : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnExitButton()
	{
		if (Kube.BCS.gameType == GameType.survival || Kube.BCS.gameType == GameType.hunger)
		{
			Kube.BCS.EndGame(BattleControllerScript.EndGameType.exit);
		}
		else if (Kube.BCS.gameType == GameType.creating && Kube.BCS.isMapOwner)
		{
			MessageBox messageBox = Cub2UI.FindAndOpenDialog<MessageBox>("dialog_savemap");
			messageBox.handler = new EventDelegate(new EventDelegate.Callback(this.onExit));
		}
		else
		{
			Kube.BCS.ExitGame();
		}
	}

	private void onExit()
	{
		if (MessageBox.current.modalResult == 0)
		{
			Kube.BCS.SaveMapAndExit();
		}
		else
		{
			Kube.BCS.ExitGame();
		}
	}

	public void OnCancelButton()
	{
		Cub2Menu.instance.head.CloseAll();
	}
}
