using System;
using UnityEngine;

public class HUDBar : MonoBehaviour
{
	public int value
	{
		get
		{
			return this._value;
		}
		set
		{
			if (this._value != value)
			{
				this._value = value;
				this.Invalidate();
			}
		}
	}

	private void Invalidate()
	{
		this.bar.value = (float)this._value / (float)this.maxvalue;
		this.label.text = this._value.ToString();
	}

	protected int _value;

	public UILabel label;

	public UIProgressBar bar;

	public int maxvalue = 100;
}
