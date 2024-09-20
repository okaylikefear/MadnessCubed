using System;
using kube;
using UnityEngine;

public class EndRoundMenu : MonoBehaviour
{
	public void onContionue()
	{
		Kube.BCS.ExitGame();
	}
}
