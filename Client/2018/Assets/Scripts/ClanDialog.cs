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

	public void Show(bool value)
	{
		this.btn.gameObject.SetActive(value);
		bool flag = false;
		if (Kube.GPS.clan != null)
		{
			flag = (this._info.id != Kube.GPS.clan.id);
		}
		this.warBtn.gameObject.SetActive(!value && flag);
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

	public void onWarBtn()
	{
		OnlineManager.RoomsInfo room = default(OnlineManager.RoomsInfo);
		room.maxPlayers = 20;
		room.roomType = 10;
		room.roomMapNumber = -1L;
		room.buildInMap = true;
		OnlineManager.instance.PlayClanWar(room, Kube.GPS.clan.id, this._info.id, Kube.GPS.clan.owner == Kube.SS.serverId);
	}

	public UILabel title;

	public UILabel shortName;

	public UILabel home;

	public UIButton btn;

	public UIButton warBtn;

	[NonSerialized]
	public ClansListTab owner;

	protected ClanInfo _info;

	private bool _init;
}
