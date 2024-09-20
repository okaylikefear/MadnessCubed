using System;
using kube;
using UnityEngine;

public class TabRow : MonoBehaviour
{
	public bool isCurrent
	{
		get
		{
			return this._isCurrent;
		}
		set
		{
			this._isCurrent = value;
			this.current.gameObject.SetActive(value);
		}
	}

	private void OnClick()
	{
		Kube.SN.gotoUserByUID(this.UID);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public int id;

	public string UID;

	public UITexture rank;

	public new UILabel name;

	public UILabel[] cols;

	public UISprite current;

	protected bool _isCurrent;
}
