using System;
using UnityEngine;

public class Cub2Input
{
	public static bool GetKeyDown(KeyCode key)
	{
		return !UIInput.selection && UnityEngine.Input.GetKeyDown(key);
	}
}
