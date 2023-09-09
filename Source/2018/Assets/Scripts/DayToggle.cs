using System;
using UnityEngine;

public class DayToggle : MonoBehaviour
{
	private void Start()
	{
	}

	public int state
	{
		get
		{
			return this._state;
		}
		set
		{
			this._state = value;
			this.Invalidate();
		}
	}

	private void OnClick()
	{
		this._state++;
		if (this._state >= this.states.Length)
		{
			this._state = 0;
		}
		this.Invalidate();
		DayToggle.current = this;
		this.onChange.Execute();
	}

	private void Invalidate()
	{
		this.sprite.spriteName = this.states[this.state];
	}

	public void OnTooltip(bool show)
	{
		if (this.hints == null || this.hints.Length <= this.state)
		{
			return;
		}
		if (show)
		{
			UITooltip.ShowText(this.hints[this.state]);
		}
		else
		{
			UITooltip.ShowText(null);
		}
	}

	public static DayToggle current;

	public UISprite sprite;

	public string[] states;

	public int _state;

	public EventDelegate onChange;

	public string[] hints;
}
