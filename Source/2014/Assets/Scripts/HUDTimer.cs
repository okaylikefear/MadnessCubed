using System;
using UnityEngine;

public class HUDTimer : MonoBehaviour
{
	private void Invalidate()
	{
		int num = this._timer / 60;
		int num2 = this._timer % 60;
		this.label.text = string.Format("{0:00}:{1:00}", num, num2);
	}

	public int timer
	{
		get
		{
			return this._timer;
		}
		set
		{
			if (this._timer != value)
			{
				this._timer = value;
				if (this._timer < 0)
				{
					this._timer = 0;
				}
				this.Invalidate();
			}
		}
	}

	public UILabel label;

	protected int _timer;
}
