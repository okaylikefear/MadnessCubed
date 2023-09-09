using System;
using kube;
using UnityEngine;

public class ClansMenu : MonoBehaviour
{
	private void Start()
	{
		this.clanwar.gameObject.SetActive(Kube.GPS.clan != null);
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
		this.filters[0].value = true;
		this.tabs[0].SendMessage("Find", FindDialog.instance.input.value);
	}

	public void onFind()
	{
		FindDialog findDialog = Cub2UI.FindDialog<FindDialog>("dialog_find");
		findDialog.Open(Localize.findClan, new global::AsyncCallback(this.FindRoom), UIInput.Validation.None);
	}

	public void onClanwar()
	{
		OnlineManager.RoomsInfo room = default(OnlineManager.RoomsInfo);
		room.maxPlayers = 20;
		room.roomType = 10;
		room.roomMapNumber = -1L;
		room.buildInMap = true;
		OnlineManager.instance.PlayClanWar(room, Kube.GPS.clan.id, 0, Kube.GPS.clan.owner == Kube.SS.serverId);
	}

	public UIToggle[] filters;

	public GameObject[] tabs;

	public UIButton clanwar;
}
