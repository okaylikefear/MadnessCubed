using System;
using UnityEngine;

public class BonusBox : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public int part
	{
		set
		{
			bool flag = value != 0;
			this.zp.gameObject.SetActive(flag);
			this.label.gameObject.SetActive(!flag);
			if (value != 0)
			{
				this.zp.spriteName = "zp" + value.ToString();
			}
		}
	}

	[ContextMenu("label")]
	public virtual void labelize()
	{
		UILabel uilabel = new GameObject
		{
			name = "Label",
			transform = 
			{
				localPosition = Vector3.zero,
				parent = base.transform
			}
		}.AddComponent<UILabel>();
		uilabel.text = "?";
	}

	[ContextMenu("collect")]
	public virtual void collect()
	{
		this.label = base.GetComponentInChildren<UILabel>();
		this.zp = base.transform.Find("ramka").GetComponent<UISprite>();
	}

	public UILabel label;

	public UISprite zp;
}
