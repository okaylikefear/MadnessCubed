using System;
using kube;
using UnityEngine;

public class RealPriceButton : MonoBehaviour
{
	public int value
	{
		set
		{
			string text = value.ToString();
			if (Kube.SN.moneyIconTx == null)
			{
				text = text + " " + Kube.SN.moneyName;
			}
			this.text.text = string.Format(this.str, text);
			if (this.center)
			{
				this.Reposition();
			}
		}
	}

	public string valueStr
	{
		set
		{
			this.text.text = value;
			if (this.center)
			{
				this.Reposition();
			}
		}
	}

	private void Start()
	{
		if (Kube.SN.moneyIconTx)
		{
			this.gold.mainTexture = Kube.SN.moneyIconTx;
		}
		else
		{
			this.gold.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (this.center)
		{
			this.Reposition();
		}
	}

	[ContextMenu("Reposition")]
	private void Reposition()
	{
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(this.text.transform, false);
		Bounds bounds2 = NGUIMath.CalculateRelativeWidgetBounds(this.gold.transform, false);
		float num = 8f;
		float num2 = bounds.max.x - bounds.min.x;
		float num3 = bounds2.max.x - bounds2.min.x;
		float num4 = (num2 + num3 + num) / 2f;
		this.gold.transform.localPosition = new Vector3(-num4 + num2 + num + num3 / 2f, 0f, 0f);
		this.text.transform.localPosition = new Vector3(-num4 + num2 / 2f, 0f, 0f);
	}

	public string str;

	public UITexture gold;

	public UILabel text;

	public bool center;
}
