using System;
using kube;
using UnityEngine;

public class HUDTeams : HUDStatus
{
	public void BeginGame()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("RespawnRed");
		GameObject[] array2 = GameObject.FindGameObjectsWithTag("RespawnBlue");
		GameObject[] array3 = GameObject.FindGameObjectsWithTag("RespawnGreen");
		GameObject[] array4 = GameObject.FindGameObjectsWithTag("RespawnYellow");
		PlayerScript ps = Kube.BCS.ps;
		bool[] array5 = new bool[]
		{
			array.Length > 0,
			array2.Length > 0,
			array3.Length > 0,
			array4.Length > 0
		};
		int num = 0;
		for (int i = 0; i < this.teams.Length; i++)
		{
			this.teams[i].gameObject.SetActive(array5[i]);
			this.teams[i].bg.alpha = ((ps.team != i) ? 0f : 1f);
			if (array5[i])
			{
				num++;
			}
		}
		this.grid.Reposition();
		this.bg.width = Mathf.RoundToInt((float)num * this.grid.cellWidth);
	}

	private void Update()
	{
		for (int i = 0; i < this.teams.Length; i++)
		{
			this.teams[i].value = Kube.BCS.teamScore[i];
		}
	}

	public HUDTeamScore[] teams;

	public UIGrid grid;

	public UISprite bg;
}
