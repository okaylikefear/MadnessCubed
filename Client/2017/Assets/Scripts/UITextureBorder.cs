using System;
using UnityEngine;

[ExecuteInEditMode]
public class UITextureBorder : UITexture
{
	private new void Update()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		UIRoot component = base.transform.root.GetComponent<UIRoot>();
		float num = (float)component.manualHeight / (float)Screen.height;
		float num2 = (float)Screen.width - 1000f / num;
		float num3 = (float)Screen.height - 600f / num;
		if (num2 <= 0f)
		{
			num2 = 1f;
		}
		if (num3 <= 0f)
		{
			num3 = 1f;
		}
		num2 *= num;
		num3 *= num;
		this.border = new Vector4(-num2, -num3, -num2, -num3);
		base.width = 1000;
		base.height = 600;
	}
}
