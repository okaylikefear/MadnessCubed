using System;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
	private void Awake()
	{
		this.isUpdate = true;
	}

	private void Start()
	{
		this.Init();
	}

	private void Init()
	{
		for (int i = 0; i < this.btn.Length; i++)
		{
			EventDelegate.Add(this.btn[i].onClick, new EventDelegate(new EventDelegate.Callback(this.onMenu)));
		}
		this.select(null);
	}

	public void onMenu()
	{
		UIButton current = UIButton.current;
		this.select(current);
	}

	public void MenuName(string name)
	{
		for (int i = 0; i < this.tab.Length; i++)
		{
			if (this.tab[i].name == name)
			{
				this.select(this.btn[i]);
				break;
			}
		}
	}

	public void CloseAll()
	{
		this.select(null);
	}

	private void select(UIButton next)
	{
		int num = Array.IndexOf<UIButton>(this.btn, next);
		bool flag = num == -1;
		bool flag2 = !flag;
		HeadTab headTab = null;
		if (!flag)
		{
			headTab = this.tab[num].GetComponent<HeadTab>();
		}
		if (headTab)
		{
			flag2 = (flag2 && !headTab.hideCloseButton);
		}
		Cub2Menu.instance.head.select((num != -1) ? this.tab[num].gameObject : null, flag, flag2);
	}

	public UIButton[] btn;

	public GameObject[] tab;

	private bool isUpdate;
}
