using System;
using kube;

public class TeamControllerBase : RoundGameType
{
	public virtual void EnterGame()
	{
		Kube.BCS.hud.timer.gameObject.SetActive(true);
	}
}
