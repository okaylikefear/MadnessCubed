using System;
using UnityEngine;

public class MaptopMenu : MonoBehaviour
{
	private void Start()
	{
	}

	private void OnEnable()
	{
	}

	private void Awake()
	{
		for (int i = 0; i < this.filters.Length; i++)
		{
			this.filters[i].onChange.Add(new EventDelegate(new EventDelegate.Callback(this.onFilter)));
		}
	}

	public void onFilter()
	{
		if (!UIToggle.current.value)
		{
			return;
		}
		int num = Array.IndexOf<UIToggle>(this.filters, UIToggle.current);
		if (num != -1)
		{
			for (int i = 0; i < this.tabs.Length; i++)
			{
				this.tabs[i].SetActive(i == num);
			}
		}
	}

	public void onFind()
	{
		FindDialog findDialog = Cub2UI.FindAndOpenDialog<FindDialog>("dialog_find");
		findDialog.roomType = GameType.test;
	}

	public UIToggle[] filters;

	public GameObject[] tabs;
}
