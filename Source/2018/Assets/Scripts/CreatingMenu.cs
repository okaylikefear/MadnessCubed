using System;
using UnityEngine;

public class CreatingMenu : MonoBehaviour
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

	private void FindRoom()
	{
		OnlineManager.RoomsInfo room = default(OnlineManager.RoomsInfo);
		room.buildInMap = false;
		room.dayLight = 0;
		room.mapCanBreak = 1;
		room.maxPlayers = 4;
		room.roomMapNumber = long.Parse(FindDialog.instance.input.value);
		room.roomType = 1;
		OnlineManager.instance.playRoom(room, false);
	}

	public void onFind()
	{
		FindDialog findDialog = Cub2UI.FindDialog<FindDialog>("dialog_find");
		findDialog.Open(Localize.findMap, new global::AsyncCallback(this.FindRoom), UIInput.Validation.Integer);
	}

	public UIToggle[] filters;

	public GameObject[] tabs;
}
