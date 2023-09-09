using System;
using kube;
using UnityEngine;

public class TeamControllerBase : RoundGameType
{
	public virtual void EnterGame(int team = -1)
	{
		Kube.BCS.hud.timer.gameObject.SetActive(true);
	}

	protected Vector3 findSpawnPos(int team)
	{
		Vector3 position = new Vector3(1f, 40f, 1f);
		GameObject[] array = new GameObject[0];
		if (team == 0)
		{
			array = this.respawnsRed;
		}
		if (team == 1)
		{
			array = this.respawnsBlue;
		}
		if (array.Length != 0)
		{
			position = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
		}
		return position;
	}

	public override void ChangeTeamScore(int team, int score)
	{
		Kube.BCS.NO.ChangeTeamScore(1, team);
	}

	protected GameObject[] respawnsRed;

	protected GameObject[] respawnsBlue;

	protected GameObject[] respawnsGreen;

	protected GameObject[] respawnsYellow;
}
