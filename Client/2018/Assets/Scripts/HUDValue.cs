using System;
using UnityEngine;

public class HUDValue : MonoBehaviour
{
	public object value
	{
		set
		{
			if (value != this._savedvalue)
			{
				this.lable.text = value.ToString();
			}
		}
	}

	public float valuePercent
	{
		set
		{
			if (this._savedvalue == null || value != (float)this._savedvalue)
			{
				this.lable.text = (value * 100f).ToString("0") + '%';
				this._savedvalue = value;
			}
		}
	}

	public UILabel lable;

	public UISprite sprite;

	protected object _savedvalue;
}
