using System;
using UnityEngine;

public class ToggleButton : MonoBehaviour
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
	}

	public UIButton button;

	public string mNormalSprite;

	protected string _SavedNormalSprite;

	private bool _value;
}
