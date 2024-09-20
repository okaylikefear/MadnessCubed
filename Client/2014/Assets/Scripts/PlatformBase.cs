using System;
using LitJson;
using UnityEngine;

public abstract class PlatformBase
{
	public PlatformBase(SocialNet owner)
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

	public virtual void PostBonusOnWall()
	{
	}

	public virtual void PostMapSlot(int playerNumMaps)
	{
	}

	public virtual void PostItemOnWall(int itemNum)
	{
	}

	public string playerUID;

	public int sex;

	public int age;

	protected SocialNet _owner;

	private SnMissionDesc[] _missionNames = new SnMissionDesc[]
	{
		new SnMissionDesc
		{
			id = 1,
			name = Localize.social_tell_friends,
			url = string.Empty
		},
		new SnMissionDesc
		{
			id = 2,
			name = Localize.social_menu,
			url = string.Empty
		},
		new SnMissionDesc
		{
			id = 3,
			name = Localize.social_invite_friends,
			url = string.Empty
		},
		new SnMissionDesc
		{
			id = 4,
			name = Localize.social_group,
			url = string.Empty
		}
	};
}
