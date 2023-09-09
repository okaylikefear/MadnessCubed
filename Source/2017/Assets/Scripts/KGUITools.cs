using System;
using UnityEngine;

public class KGUITools
{
	public static void removeAllChildren(GameObject gameObject, bool destroy = true)
	{
		foreach (object obj in gameObject.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(false);
			if (destroy)
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
	}
}
