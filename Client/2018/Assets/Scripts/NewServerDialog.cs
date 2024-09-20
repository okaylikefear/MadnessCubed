using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class NewServerDialog : MonoBehaviour
{
	private void Start()
	{
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		for (int i = 2; i < Localize.gameTypeStr.Length; i++)
		{
			if (i != 5)
			{
				list.Add(Localize.gameTypeStr[i]);
				list2.Add(i);
			}
		}
		this.gametype.items = list;
		this._itemsIndex = list2.ToArray();
		this.gametype.value = list[0];
		this.gametype.onChange.Add(new EventDelegate(new EventDelegate.Callback(this.onGameType)));
		this.FillMaps();
	}

	private void FillMaps()
	{
		List<string> list = new List<string>();
		if (this.builtinToggle && this.builtinToggle.value)
		{
			GameType gameType = (GameType)this._itemsIndex[this.gametype.items.IndexOf(this.gametype.value)];
			this.bmm = Kube.OH.findMaps(gameType);
			for (int i = 0; i < this.bmm.Length; i++)
			{
				string item = Localize.buildinMapName[this.bmm[i].Id];
				list.Add(item);
			}
		}
		else
		{
			for (int j = 0; j < Kube.GPS.playerNumMaps; j++)
			{
				string item = Localize.map + " " + (j + 1);
				if (PlayMenu.playMenu.slotNames.Length > j && PlayMenu.playMenu.slotNames[j] != null)
				{
					item = PlayMenu.playMenu.slotNames[j].name;
				}
				list.Add(item);
			}
		}
		this.map.items = list;
		this.map.value = list[0];
	}

	private void Update()
	{
	}

	public void onGameType()
	{
		this.FillMaps();
	}

	public void onMapName()
	{
	}

	public void onBuiltinCheck()
	{
		this.FillMaps();
	}

	public void onCreateClick()
	{
		OnlineManager.RoomsInfo roomsInfo = default(OnlineManager.RoomsInfo);
		roomsInfo.roomType = this._itemsIndex[this.gametype.items.IndexOf(this.gametype.value)];
		long roomMapNumber;
		if (this.builtinToggle && this.builtinToggle.value)
		{
			int num = this.map.items.IndexOf(this.map.value);
			roomMapNumber = (long)this.bmm[num].Id;
			roomsInfo.buildInMap = true;
		}
		else
		{
			int num2 = this.map.items.IndexOf(this.map.value);
			roomMapNumber = (long)Kube.SS.serverId * 20L + (long)num2;
			roomsInfo.roomTitle = Localize.c_slot + " " + (num2 + 1);
			if (PlayMenu.playMenu.slotNames != null && PlayMenu.playMenu.slotNames[num2] != null)
			{
				roomsInfo.roomTitle = PlayMenu.playMenu.slotNames[num2].name;
			}
		}
		roomsInfo.roomMapNumber = roomMapNumber;
		roomsInfo.mapCanBreak = ((!this.noBreak.value) ? 1 : 0);
		roomsInfo.dayLight = 2 - this.day.state;
		roomsInfo.roomPassword = this.password.value;
		OnlineManager.instance.createRoom(roomsInfo, false);
	}

	public void onToggleBreak()
	{
		MonoBehaviour.print("OK");
	}

	public UIPopupListEx gametype;

	public UIPopupListEx map;

	public DayToggle day;

	public UIToggle noBreak;

	public UIInput password;

	private int[] _itemsIndex;

	private ObjectsHolderScript.BuiltInMap[] bmm;

	public UIToggle builtinToggle;

	private bool _canBreak = true;
}
