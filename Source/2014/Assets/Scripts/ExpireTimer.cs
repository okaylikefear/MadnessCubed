using System;
using UnityEngine;

public class ExpireTimer : MonoBehaviour
{
	public int value
	{
		set
		{
			this.label.text = VIPMenu.ExpriteTime((float)value);
		}
	}

	public UILabel label;
}
