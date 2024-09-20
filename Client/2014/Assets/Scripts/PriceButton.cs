using System;
using UnityEngine;

public class PriceButton : MonoBehaviour
{
	public bool isGold
	{
		get
		{
			return this._isgold;
		}
		set
		{
			if (value)
			{
				this.gold.spriteName = "button_g";
			}
			else
			{
				this.gold.spriteName = "button_m";
			}
			this._isgold = value;
		}
	}

	public int value
	{
		set
		{
			this.text.text = value.ToString();
		}
	}

	private void Start()
	{
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
		this.text.transform.localPosition = new Vector3(-num4 + num2 / 2f, 0f, 0f);
		this.gold.transform.localPosition = new Vector3(-num4 + num2 + num + num3 / 2f, 0f, 0f);
	}

	[ContextMenu("collect")]
	public virtual void collect()
	{
		this.text = base.GetComponentInChildren<UILabel>();
		this.gold = base.GetComponentInChildren<UISprite>();
	}

	public UISprite gold;

	public UILabel text;

	private bool _isgold;

	public bool center;
}
