using System;
using System.Collections.Generic;
using UnityEngine;

public class PagePanel : MonoBehaviour
{
	private void Start()
	{
		if (!this.mInitDone)
		{
			this.Init();
		}
	}

	private void FixedUpdate()
	{
		Vector2 clipOffset = this.panel.clipOffset;
		Vector2 v = this.panel.transform.localPosition;
		if (Mathf.Abs(clipOffset.x - this.newOffsetX) > 0.5f)
		{
			float num = 10f * Time.deltaTime;
			clipOffset.x += (this.newOffsetX - clipOffset.x) * num;
			v.x += (this.newTrX - v.x) * num;
			this.panel.clipOffset = clipOffset;
			this.panel.transform.localPosition = v;
		}
	}

	public void onLeft()
	{
		if (this.page <= 0)
		{
			return;
		}
		this.page--;
		this.Shift((this.w + this.padding.x * 2f) * (float)this.cols * (float)this.page);
	}

	public void onRight()
	{
		if (this.page >= this.mpages - 1)
		{
			return;
		}
		this.page++;
		this.Shift((this.w + this.padding.x * 2f) * (float)this.cols * (float)this.page);
	}

	private void Shift(float w)
	{
		float num = w - this.clipOffsetX;
		this.clipOffsetX = w;
		this.newOffsetX += num;
		this.newTrX -= num;
		this.UpdateButtons();
	}

	private void UpdateButtons()
	{
		if (this.left != null)
		{
			this.left.isEnabled = (this.page > 0);
		}
		if (this.right != null)
		{
			this.right.isEnabled = (this.page < this.mpages - 1);
		}
	}

	private void Init()
	{
		if (this.panel == null)
		{
			this.panel = base.GetComponent<UIPanel>();
		}
		this.newOffsetX = this.panel.clipOffset.x;
		this.newTrX = this.panel.transform.localPosition.x;
		this.mInitDone = true;
		this.Reposition();
	}

	[ContextMenu("Execute")]
	public virtual void Reposition()
	{
		if (!this.mInitDone)
		{
			this.Init();
		}
		this.page = 0;
		this.Shift(0f);
		List<GameObject> list = new List<GameObject>();
		foreach (object obj in base.gameObject.transform)
		{
			Transform transform = (Transform)obj;
			if (NGUITools.GetActive(transform.gameObject))
			{
				list.Add(transform.gameObject);
			}
		}
		float num = 0f;
		float num2 = 0f;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject = list[i];
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(gameObject.transform, false);
			Bounds bounds2 = bounds;
			Bounds bounds3 = bounds;
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.x = num + bounds.extents.x - bounds.center.x;
			localPosition.x += bounds.min.x - bounds2.min.x + this.padding.x;
			localPosition.y = -num2 - bounds.extents.y - bounds.center.y;
			localPosition.y += (bounds.max.y - bounds.min.y - bounds3.max.y + bounds3.min.y) * 0.5f - this.padding.y;
			num += this.w + this.padding.x * 2f;
			num3++;
			if (num3 >= this.cols)
			{
				num3 = 0;
				num2 += this.h + this.padding.y * 2f;
				num4++;
				if (num4 >= this.rows)
				{
					num2 = 0f;
					num4 = 0;
					num5++;
				}
				num = (float)(num5 * this.cols) * (this.w + this.padding.x * 2f);
			}
			gameObject.transform.localPosition = localPosition;
		}
		this.mpages = (int)Mathf.Ceil((float)list.Count / (float)(this.cols * this.rows));
		this.UpdateButtons();
		this.panel.Update();
	}

	public UIPanel panel;

	public UIButton left;

	public UIButton right;

	private float newOffsetX;

	private float newTrX;

	private float clipOffsetX;

	protected bool mInitDone;

	public Vector2 padding = Vector2.zero;

	public int page;

	private int mpages;

	public float w = 200f;

	public float h = 220f;

	public int cols = 3;

	public int rows = 2;
}
