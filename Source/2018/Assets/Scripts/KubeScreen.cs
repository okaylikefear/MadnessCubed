using System;
using UnityEngine;

public class KubeScreen
{
	public static bool lockCursor
	{
		get
		{
			return Cursor.lockState == CursorLockMode.Locked;
		}
		set
		{
			Cursor.lockState = ((!value) ? CursorLockMode.None : CursorLockMode.Locked);
			Cursor.visible = !value;
		}
	}
}
