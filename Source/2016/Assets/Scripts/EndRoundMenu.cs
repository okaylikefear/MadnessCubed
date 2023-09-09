using System;
using kube;
using UnityEngine;

public class EndRoundMenu : MonoBehaviour
{
	public void onContionue()
	{
		Kube.BCS.EndGame(BattleControllerScript.EndGameType.exit);
	}

	public void Open()
	{
		Cub2UI.currentMenu = base.gameObject;
	}

	private void Update()
	{
		if (Kube.BCS.gameProcess != BattleControllerScript.GameProcess.end)
		{
			base.gameObject.SetActive(false);
		}
	}
}
