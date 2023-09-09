using System;
using UnityEngine;

public class Cub2Menu : Cub2MenuBase
{
	public static Cub2Menu instance
	{
		get
		{
			if (Cub2Menu._instance == null)
			{
				Cub2Menu._instance = Cub2UI.FindObject<Cub2Menu>();
			}
			return Cub2Menu._instance;
		}
	}

	public static T Find<T>() where T : Component
	{
		T[] componentsInChildren = Cub2Menu.instance.GetComponentsInChildren<T>(true);
		if (componentsInChildren.Length > 0)
		{
			return componentsInChildren[0];
		}
		return (T)((object)null);
	}

	public static T Find<T>(string name) where T : Component
	{
		T[] componentsInChildren = Cub2Menu.instance.GetComponentsInChildren<T>(true);
		if (componentsInChildren.Length > 0)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].name == name)
				{
					return componentsInChildren[i];
				}
			}
		}
		return (T)((object)null);
	}

	public void Awake()
	{
		Cub2Menu._instance = this;
	}

	public void OpenTab(string name)
	{
		HeadPanel componentInChildren = base.GetComponentInChildren<HeadPanel>();
		Transform transform = base.transform.FindChild(name);
		if (!transform)
		{
			return;
		}
		GameObject gameObject = transform.gameObject;
		componentInChildren.select(gameObject, false, true);
	}

	public HeadPanel head;

	public GameObject home;

	public GameObject loadingPrefab;

	private static Cub2Menu _instance;
}
