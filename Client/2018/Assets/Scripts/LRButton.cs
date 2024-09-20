using System;
using UnityEngine;

public class LRButton : MonoBehaviour
{
	public int index
	{
		get
		{
			return this._index;
		}
		set
		{
			this._index = value;
			if (this._index < 0)
			{
				this._index = 0;
			}
			if (this._index >= this.states.Length)
			{
				this._index = this.states.Length - 1;
			}
			this.Invalidate();
		}
	}

	public string[] states
	{
		get
		{
			return this._states;
		}
		set
		{
			this._states = value;
			this.Invalidate();
		}
	}

	private void Start()
	{
		this.left.onClick.Add(new EventDelegate(new EventDelegate.Callback(this.onLeft)));
		this.right.onClick.Add(new EventDelegate(new EventDelegate.Callback(this.onRight)));
	}

	private void onLeft()
	{
		this._index--;
		if (this._index < 0)
		{
			this._index = this.states.Length - 1;
		}
		this.Invalidate();
	}

	private void onRight()
	{
		this._index++;
		if (this._index >= this.states.Length)
		{
			this._index = 0;
		}
		this.Invalidate();
	}

	private void Invalidate()
	{
		if (this.states == null)
		{
			return;
		}
		if (this.label && this.states != null && this._index >= 0 && this._index < this.states.Length)
		{
			this.label.text = this.states[this._index];
		}
		LRButton.current = this;
		this.onChange.Execute();
	}

	public UIButton left;

	public UIButton right;

	public UILabel label;

	public EventDelegate onChange;

	public int _index;

	public string[] _states;

	public static LRButton current;
}
