using System;
using UnityEngine;

[ExecuteInEditMode]
public class KubeAnchor : MonoBehaviour
{
	private void Awake()
	{
		this.mTrans = base.transform;
		this.mAnim = base.GetComponent<Animation>();
		UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Combine(UICamera.onScreenResize, new UICamera.OnScreenResize(this.ScreenSizeChanged));
	}

	private void OnDestroy()
	{
		UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Remove(UICamera.onScreenResize, new UICamera.OnScreenResize(this.ScreenSizeChanged));
	}

	private void ScreenSizeChanged()
	{
		if (this.mStarted && this.runOnlyOnce)
		{
			this.Update();
		}
	}

	private void Start()
	{
		this.mRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		this.ds = NGUITools.FindInParents<DownScale>(base.transform);
		if (this.uiCamera == null)
		{
			this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		this.Update();
		this.mStarted = true;
	}

	private void Update()
	{
		if (this.mAnim != null && this.mAnim.enabled && this.mAnim.isPlaying)
		{
			return;
		}
		float num = (float)Cub2UI.activeWidth;
		float num2 = (float)Cub2UI.activeHeight;
		if (!Application.isPlaying)
		{
			num = (float)Screen.width;
			num2 = (float)Screen.height;
		}
		float num3 = num * 0.5f;
		float num4 = num2 * 0.5f;
		Vector3 vector = new Vector3(0f, 0f, 0f);
		if (this.side != KubeAnchor.Side.Center)
		{
			if (this.side == KubeAnchor.Side.Right || this.side == KubeAnchor.Side.TopRight || this.side == KubeAnchor.Side.BottomRight)
			{
				vector.x = num3;
			}
			else if (this.side == KubeAnchor.Side.Top || this.side == KubeAnchor.Side.Center || this.side == KubeAnchor.Side.Bottom)
			{
				vector.x = 0f;
			}
			else
			{
				vector.x = -num3;
			}
			if (this.side == KubeAnchor.Side.Top || this.side == KubeAnchor.Side.TopRight || this.side == KubeAnchor.Side.TopLeft)
			{
				vector.y = num4;
			}
			else if (this.side == KubeAnchor.Side.Left || this.side == KubeAnchor.Side.Center || this.side == KubeAnchor.Side.Right)
			{
				vector.y = 0f;
			}
			else
			{
				vector.y = -num4;
			}
		}
		vector.x += this.pixelOffset.x + this.relativeOffset.x * num;
		vector.y += this.pixelOffset.y + this.relativeOffset.y * num2;
		if (this.uiCamera.orthographic)
		{
			vector.x = Mathf.Round(vector.x);
			vector.y = Mathf.Round(vector.y);
		}
		if (this.mTrans.localPosition != vector)
		{
			this.mTrans.localPosition = vector;
		}
		if (this.runOnlyOnce && Application.isPlaying)
		{
			base.enabled = false;
		}
	}

	public Camera uiCamera;

	public KubeAnchor.Side side = KubeAnchor.Side.Center;

	public bool runOnlyOnce = true;

	public Vector2 relativeOffset = Vector2.zero;

	public Vector2 pixelOffset = Vector2.zero;

	private Transform mTrans;

	private Animation mAnim;

	private Rect mRect = default(Rect);

	private UIRoot mRoot;

	private bool mStarted;

	private DownScale ds;

	public enum Side
	{
		BottomLeft,
		Left,
		TopLeft,
		Top,
		TopRight,
		Right,
		BottomRight,
		Bottom,
		Center
	}
}
