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
				Cub2Menu._instance = UnityEngine.Object.FindObjectOfType<Cub2Menu>();
			}
			return Cub2Menu._instance;
		}
	}

	public static T Find<T>(string name) where T : Component
	{
		return (T)((object)Cub2Menu.instance.transform.Find(name).GetComponent(typeof(T)));
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
		int num = Array.IndexOf<GameObject>(componentInChildren.tab, gameObject);
		if (num == -1)
		{
			return;
		}
		componentInChildren.btn[num].value = true;
	}

	public HeadPanel head;

	public GameObject loadingPrefab;

	private static Cub2Menu _instance;
}
