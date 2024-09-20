using System;
using UnityEngine;

public class GameMenu : Cub2Menu
{
	public void Resume()
	{
		base.gameObject.SetActive(false);
	}

	public GameObject hud;
}
