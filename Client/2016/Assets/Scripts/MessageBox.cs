using System;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
	private void Awake()
	{
		for (int i = 0; i < this.buttons.Length; i++)
		{
			EventDelegate.Add(this.buttons[i].onClick, new EventDelegate.Callback(this.onClick));
		}
	}

	private void onClick()
	{
		MessageBox.current = this;
		this.modalResult = Array.IndexOf<UIButton>(this.buttons, UIButton.current);
		this.handler.Execute();
		base.gameObject.SetActive(false);
	}

	public UIButton[] buttons;

	public static MessageBox current;

	public EventDelegate handler;

	public int modalResult;

	public UILabel label;

	public UILabel title;
}
