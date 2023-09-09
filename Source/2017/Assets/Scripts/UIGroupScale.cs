using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Scale")]
public class UIGroupScale : MonoBehaviour
{
	private void Start()
	{
		if (!this.mStarted)
		{
			this.mStarted = true;
			if (this.tweenTarget == null)
			{
				this.tweenTarget = base.transform;
			}
			this.mScale = this.tweenTarget.localScale;
		}
	}

	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
	}

	private void OnDisable()
	{
		if (this.mStarted && this.tweenTarget != null)
		{
			TweenScale component = this.tweenTarget.GetComponent<TweenScale>();
			if (component != null)
			{
				component.value = this.mScale;
				component.enabled = false;
			}
		}
	}

	private void OnPress(bool isPressed)
	{
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenScale.Begin(this.tweenTarget.gameObject, this.duration, (!isPressed) ? ((!UICamera.IsHighlighted(base.gameObject)) ? this.mScale : Vector3.Scale(this.mScale, this.hover)) : Vector3.Scale(this.mScale, this.pressed)).method = UITweener.Method.EaseInOut;
		}
	}

	private void OnHover(bool isOver)
	{
		if (!isOver && UICamera.hoveredObject != null)
		{
			if (this.groupCollider != null)
			{
				Transform transform = UICamera.hoveredObject.transform;
				while (transform)
				{
					if (transform == base.transform)
					{
						return;
					}
					transform = transform.parent;
				}
			}
			else
			{
				if (UICamera.hoveredObject.transform.parent == base.transform.parent)
				{
					return;
				}
				Transform transform2 = UICamera.hoveredObject.transform;
				while (transform2)
				{
					if (transform2 == base.transform)
					{
						return;
					}
					transform2 = transform2.parent;
				}
			}
		}
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenScale.Begin(this.tweenTarget.gameObject, this.duration, (!isOver) ? this.mScale : Vector3.Scale(this.mScale, this.hover)).method = UITweener.Method.EaseInOut;
		}
		this.misOver = isOver;
	}

	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			this.OnHover(isSelected);
		}
	}

	private void Update()
	{
		if (this.misOver && UICamera.hoveredObject != this)
		{
			this.OnHover(false);
		}
	}

	public Transform tweenTarget;

	public Vector3 hover = new Vector3(1.1f, 1.1f, 1.1f);

	public Vector3 pressed = new Vector3(1.05f, 1.05f, 1.05f);

	public float duration = 0.2f;

	private Vector3 mScale;

	private bool mStarted;

	public Collider groupCollider;

	private bool misOver;
}
