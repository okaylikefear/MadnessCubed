using System;
using kube;
using kube.data;
using UnityEngine;

public class MyClanDialog : MonoBehaviour
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
			bool flag = false;
			if (this._info.id != 0)
			{
				flag = true;
			}
			this.save.gameObject.SetActive(flag);
			this.join.gameObject.SetActive(!flag);
			this.title.text = value.name;
			this.shortName.text = value.shortName;
			this.home.text = value.home;
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
		if (this.shortName.value.Length < 3 || this.title.value.Length < 3)
		{
			Cub2UI.MessageBox("Введи название клана и сокращение", null);
			return;
		}
		if ((this._info == null || this._info.id == 0 || this._info.shortName != this.shortName.value) && !Clans.checkShortName(this.shortName.value))
		{
			Cub2UI.MessageBox("Короткое имя уже занято", null);
			return;
		}
		if (!Kube.SN.checkGroupLink(this.home.value))
		{
			Cub2UI.MessageBox(Localize.clan_url_default, null);
			return;
		}
		ClanInfo clanInfo = new ClanInfo();
		clanInfo.name = this.title.value;
		clanInfo.shortName = this.shortName.value;
		clanInfo.home = this.home.value;
		if (this._info == null || this._info.id == 0)
		{
			this.owner.createClan(clanInfo);
		}
		else
		{
			clanInfo.id = this._info.id;
			this.owner.updateClan(clanInfo);
		}
		base.gameObject.SetActive(false);
	}

	public UIInput title;

	public UIInput shortName;

	public UIInput home;

	public UIButton join;

	public UIButton save;

	[NonSerialized]
	public ClansMyTab owner;

	protected ClanInfo _info;

	private bool _init;
}
