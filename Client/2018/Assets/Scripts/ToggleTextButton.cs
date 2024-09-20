using System;
using UnityEngine;

public class ToggleTextButton : MonoBehaviour
{
	private void Start()
	{
		this.button = base.GetComponent<UIButton>();
	}

	public bool value
	{
		get
		{
			return this._value;
		}
		set
		{
			this._value = value;
			this.Invalidate();
		}
	}

	private void Invalidate()
	{
		if (this.value)
		{
			this._SavedNormalSprite = this.button.normalSprite;
			this.button.normalSprite = this.mNormalSprite;
		}
		else
		{
			this.button.normalSprite = this._SavedNormalSprite;
		}
		this.label.text = this.states[(!this.value) ? 0 : 1];
	}

	private void OnClick()
	{
		this._value = !this._value;
		ToggleTextButton.current = this;
		this.onChange.Execute();
		ToggleTextButton.current = null;
		this.Invalidate();
	}

	public UILabel label;

	public static ToggleTextButton current;

	public string[] states;

	public UIButton button;

	public EventDelegate onChange;

	public string mNormalSprite;

	protected string _SavedNormalSprite;

	private bool _value;
}
