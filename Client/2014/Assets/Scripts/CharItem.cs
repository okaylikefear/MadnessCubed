using System;
using UnityEngine;

public class CharItem : MonoBehaviour
{
	public bool selected
	{
		get
		{
			return this._selected;
		}
		set
		{
			this._selected = value;
			this.checkmark.alpha = ((!this._selected) ? 0f : 255f);
		}
	}

	private void Start()
	{
		this.checkmark.alpha = ((!this._selected) ? 0f : 255f);
	}

	private void OnClick()
	{
		base.transform.parent.parent.GetComponent<CharMenu>().onItemSelect(this);
	}

	public int itemId;

	public int type;

	public bool isSet;

	public bool _selected;

	public UISprite checkmark;
}
