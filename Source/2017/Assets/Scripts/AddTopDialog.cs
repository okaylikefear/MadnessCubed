using System;
using System.Collections.Generic;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class AddTopDialog : MonoBehaviour
{
	public TopInfo info
	{
		set
		{
			this.Init();
			this._info = value;
			if (this._info == null)
			{
				this.title.value = string.Empty;
				return;
			}
			int index = (int)(value.roomMapNumber - (long)Kube.SS.serverId * 20L);
			this.map.value = this.map.items[index];
			index = Array.IndexOf<int>(this._itemsIndex, value.roomType);
			this.gametype.value = this.gametype.items[index];
			this.title.value = value.name;
			this.day.state = value.dayLight;
			this.noBreak.value = (value.mapCanBreak == 0);
		}
	}

	private void Init()
	{
		if (this._init)
		{
			return;
		}
		this._init = true;
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		for (int i = 2; i < Localize.gameTypeStr.Length; i++)
		{
			list.Add(Localize.gameTypeStr[i]);
			list2.Add(i);
		}
		this.gametype.items = list;
		this._itemsIndex = list2.ToArray();
		this.gametype.value = list[0];
		this.gametype.onChange.Add(new EventDelegate(new EventDelegate.Callback(this.onGameType)));
		list = new List<string>();
		for (int j = 0; j < Kube.GPS.playerNumMaps; j++)
		{
			list.Add(Localize.map + " " + (j + 1));
		}
		this.map.items = list;
		this.map.value = list[0];
	}

	private void Start()
	{
		this.Init();
	}

	private void Update()
	{
	}

	public void onGameType()
	{
	}

	public void onMapName()
	{
	}

	public void onBuiltinCheck()
	{
	}

	private void onSend(string response)
	{
		if (response != "1")
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			GameParamsScript gps = Kube.GPS;
			gps.playerMoney2 -= (int)jsonData["price"];
			this.owner.LoadAndShow();
		}
	}

	public void onCreateClick()
	{
		if (this.title.value == string.Empty)
		{
			Cub2UI.MessageBox("Введите имя карты", null);
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		int roomType = this._itemsIndex[this.gametype.items.IndexOf(this.gametype.value)];
		int num = this.map.items.IndexOf(this.map.value);
		long roomMapNumber = (long)Kube.SS.serverId * 20L + (long)num;
		TopInfo topInfo = this.owner.hasRecord(roomMapNumber, roomType);
		if (topInfo != this._info)
		{
			Cub2UI.MessageBox("Такая карта уже есть", null);
			return;
		}
		dictionary["mapid"] = roomMapNumber.ToString();
		dictionary["name"] = this.title.value;
		dictionary["type"] = roomType.ToString();
		dictionary["canbreak"] = ((!this._canBreak) ? "0" : "1");
		dictionary["daytime"] = this.day.state.ToString();
		if (this._info != null)
		{
			dictionary["oid"] = this._info.id.ToString();
		}
		Kube.SS.Request(802, dictionary, new ServerCallback(this.onSend));
		base.gameObject.SetActive(false);
	}

	public void onToggleBreak()
	{
		this._canBreak = !UIToggle.current.value;
		MonoBehaviour.print("OK");
	}

	public UIPopupListEx gametype;

	public UIPopupListEx map;

	public DayToggle day;

	public UIToggle noBreak;

	public UIInput title;

	public MaptopMyTab owner;

	private int[] _itemsIndex;

	protected TopInfo _info;

	private bool _init;

	private bool _canBreak = true;
}
