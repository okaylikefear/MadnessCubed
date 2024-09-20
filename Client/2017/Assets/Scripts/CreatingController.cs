using System;
using kube;

public class CreatingController : GameTypeControllerBase
{
	private void Start()
	{
		this.hud = Kube.BCS.hud;
		this.hud.timer.gameObject.SetActive(false);
	}

	private new void UpdateHUD()
	{
		if (this.hud.curstat)
		{
			this.hud.curstat.values[0].value = Kube.BCS.currentNumPlayers;
		}
	}

	private void Update()
	{
		this.UpdateHUD();
	}

	protected UIHUD hud;
}
