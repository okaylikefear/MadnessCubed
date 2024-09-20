using System;
using kube.data;
using LitJson;
using UnityEngine;

public abstract class PlatformBase
{
	public PlatformBase(PlatformWeb owner)
	{
		this._owner = owner;
	}

	public abstract void Init(GameObject go, string func);

	public abstract void ShowPayment(float socialNetMoney, float goldQuantity, GameObject go, string func);

	public abstract void GetWebData(string ans);

	public virtual void gotoUserByUID(string uid)
	{
		if (uid != string.Empty)
		{
			Application.ExternalCall("OpenUrl", new object[]
			{
				this.GetUserUrl(uid)
			});
		}
	}

	protected abstract string GetUserUrl(string uid);

	public abstract void FillFriendsRating(GameObject go, string method);

	public abstract void TakeScreenshot(GameObject go, string method);

	public virtual void APICallback(JsonData json)
	{
	}

	public virtual void OnUserInfo(JsonData data)
	{
	}

	public abstract void openRangTable();

	public virtual SnMissionDesc[] getMissions()
	{
		if (this._missionNames == null)
		{
			this._missionNames = new SnMissionDesc[]
			{
				new SnMissionDesc
				{
					id = 1,
					name = Localize.social_install_game,
					url = string.Empty
				},
				new SnMissionDesc
				{
					id = 2,
					name = Localize.social_tell_about_game,
					url = string.Empty
				},
				new SnMissionDesc
				{
					id = 3,
					name = Localize.social_invite_num_friends,
					url = string.Empty
				},
				new SnMissionDesc
				{
					id = 4,
					name = Localize.social_folow_fanpage
				}
			};
		}
		return this._missionNames;
	}

	public virtual void gotoMission(int id)
	{
		SnMissionDesc[] missions = this.getMissions();
		for (int i = 0; i < missions.Length; i++)
		{
			if (missions[i].id == id)
			{
				Application.ExternalCall("OpenUrl", new object[]
				{
					this._missionNames[i].url
				});
			}
			this._owner.OnMissionDone(id.ToString());
		}
	}

	public virtual void PostLevelUpOnWall(int levelNum)
	{
	}

	public virtual void PostWeaponOnWall(int weaponNum)
	{
	}

	public virtual void PostMissionOnWall(int missionNum)
	{
	}

	public virtual void PostViralOnWall(int missionNum)
	{
	}

	public virtual void PostBonusOnWall()
	{
	}

	public virtual void PostMapSlot(int playerNumMaps)
	{
	}

	public virtual void PostItemOnWall(int itemNum)
	{
	}

	public virtual bool checkGroupLink(string home)
	{
		return true;
	}

	public virtual void BuyPack(PackInfo info)
	{
	}

	public virtual string moneyNameForPack(PackInfo info)
	{
		return info.price.ToString();
	}

	public string playerUID;

	public int sex;

	public int age;

	public bool hasMoneyIcon = true;

	public string moneyName = "RUB";

	public float moneyValue = 1f;

	protected PlatformWeb _owner;

	private SnMissionDesc[] _missionNames;
}
