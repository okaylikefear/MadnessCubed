using System;
using UnityEngine;

public class PageScrollList : MonoBehaviour
{
	private void Start()
	{
		if (!this.mInitDone)
		{
			this.Init();
		}
	}

	public void onLeft()
	{
		this.page = Mathf.RoundToInt(this._view.panel.baseClipRegion.x / this._view.panel.baseClipRegion.z);
		if (this.page <= 0)
		{
			return;
		}
		this.page--;
		this.onPage.Execute();
		this.Shift((float)this.page);
	}

	public void onRight()
	{
		this.page = Mathf.RoundToInt(this._view.panel.clipOffset.x / this._view.panel.baseClipRegion.z);
		if (this.page >= this.mpages - 1)
		{
			return;
		}
		this.page++;
		this.onPage.Execute();
		this.Shift((float)this.page);
	}

	private void Shift(float p)
	{
		float num = p * this._view.panel.baseClipRegion.z - (this._view.panel.clipOffset.x + this._view.panel.baseClipRegion.z / 2f);
		SpringPanel.Begin(this._view.panel.gameObject, this._view.transform.localPosition + new Vector3(-num, 0f, 0f), 8f);
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

	private void onDragFinished()
	{
		this.page = Mathf.RoundToInt(this._view.panel.clipOffset.x / this._view.panel.baseClipRegion.z);
		this.UpdateButtons();
	}

	private void Init()
	{
		if (this._view == null)
		{
			this._view = base.GetComponent<UIScrollView>();
		}
		this._view.onDragFinished = new UIScrollView.OnDragNotification(this.onDragFinished);
		this.newOffsetX = this._view.panel.clipOffset.x;
		this.mpages = Mathf.RoundToInt(this._view.bounds.size.x / this._view.panel.baseClipRegion.z);
		this.mInitDone = true;
		this.UpdateButtons();
	}

	public UIScrollView _view;

	public UIButton left;

	public UIButton right;

	private float newOffsetX;

	private float newTrX;

	private float clipOffsetX;

	public EventDelegate onPage;

	protected bool mInitDone;

	protected int page;

	protected int mpages;
}
