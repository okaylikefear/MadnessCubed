using System;
using UnityEngine;

public class FindDialog : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		this.input.value = string.Empty;
	}

	private void FindRoom()
	{
		OnlineManager.RoomsInfo room = default(OnlineManager.RoomsInfo);
		room.buildInMap = false;
		room.dayLight = 0;
		room.mapCanBreak = 1;
		room.maxPlayers = 4;
		room.roomMapNumber = long.Parse(this.input.value);
		room.roomType = (int)this.roomType;
		OnlineManager.instance.playRoom(room, false);
	}

	public void OnClick()
	{
		this.FindRoom();
		base.gameObject.SetActive(false);
	}

	public UILabel label;

	public UIInput input;

	public GameType roomType;
}
