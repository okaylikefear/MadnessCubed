using System;
using UnityEngine;

public class HUDValue : MonoBehaviour
{
	public object value
	{
		set
		{
			if (value != null)
			{
				this.lable.text = value.ToString();
			}
		}
	}

	public UILabel lable;

	public UISprite sprite;
}
