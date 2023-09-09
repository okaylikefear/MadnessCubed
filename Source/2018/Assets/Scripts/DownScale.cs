using System;
using UnityEngine;

[ExecuteInEditMode]
public class DownScale : MonoBehaviour
{
	private void Start()
	{
		this.mRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		float num = (float)Cub2UI.activeWidth - 1000f;
		float num2 = (float)Cub2UI.activeHeight - 600f;
		if (num <= 0f)
		{
			num = 1f;
		}
		if (num2 <= 0f)
		{
			num2 = 1f;
		}
		if (this.tx)
		{
			this.tx.border = new Vector4(-num, -num2, -num, -num2);
		}
	}

	public UITexture tx;

	private UIRoot mRoot;

	public GameObject border;

	protected float lastX;

	protected float lastY;

	public bool onlyIfLess;

	public bool onlyY;
}
