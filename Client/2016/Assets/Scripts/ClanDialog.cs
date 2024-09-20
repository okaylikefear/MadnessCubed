using System;
using kube;
using kube.data;
using UnityEngine;

public class ClanDialog : MonoBehaviour
{
	public ClanInfo info
	{
		set
		{
			this.Init();
			this._info = value;
			if (this._info == null)
			{
				this.title.text = string.Empty;
				return;
			}
			this.title.text = value.name;
			this.shortName.text = value.shortName;
			this.home.text = value.home;
		}
	}

	public bool canJoin
	{
		set
		{
			this.btn.gameObject.SetActive(value);
		}
	}

	private void Init()
	{
		if (this._init)
		{
			return;
		}
		this._init = true;
	}

	private void Start()
	{
		this.Init();
	}

	private void Update()
	{
	}

	public void onJoin()
	{
		this.owner.join(this._info.id);
		base.gameObject.SetActive(false);
	}

	public void onHomeClick()
	{
		if (!string.IsNullOrEmpty(this.home.text))
		{
			Kube.SN.openURL(this.home.text);
		}
	}

	public UILabel title;

	public UILabel shortName;

	public UILabel home;

	public UIButton btn;

	[NonSerialized]
	public ClansListTab owner;

	protected ClanInfo _info;

	private bool _init;
}
