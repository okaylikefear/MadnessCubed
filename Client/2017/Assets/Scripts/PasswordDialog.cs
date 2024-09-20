using System;
using UnityEngine;

public class PasswordDialog : MonoBehaviour
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

	public void OnClick()
	{
		if (this.label.text == this.room.roomPassword)
		{
			OnlineManager.instance.joinRoom(this.room);
		}
		base.gameObject.SetActive(false);
	}

	public OnlineManager.RoomsInfo room;

	public UILabel label;

	public UIInput input;
}
