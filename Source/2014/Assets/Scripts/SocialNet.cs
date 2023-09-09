using System;
using System.Collections;
using System.Collections.Generic;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class SocialNet : MonoBehaviour
{
	private void Awake()
	{
		Kube.SN = this;
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		Kube.SN = null;
	}

	public void Init(GameObject go, string func)
	{
		this.goToMessage = go;
		this.messageFunc = func;
		if (!Application.isEditor)
		{
			Application.ExternalCall("SendSettingsToPlayer", new object[0]);
			return;
		}
		this.initPlatform();
	}

	private void initPlatform()
	{
		if (this.platform == SocialNet.platformType.vk)
		{
			this.po = new PlatformVK(this, this.goToMessage, this.messageFunc);
		}
		else if (this.platform == SocialNet.platformType.ok)
		{
			this.po = new PlatformOK(this, this.goToMessage, this.messageFunc);
		}
		else if (this.platform == SocialNet.platformType.fb)
		{
			this.po = new PlatformFB(this, this.goToMessage, this.messageFunc);
		}
		else if (this.platform == SocialNet.platformType.mm)
		{
			this.po = new PlatformMM(this, this.goToMessage, this.messageFunc);
		}
		if (this.po != null)
		{
			this.po.Init(this.goToMessage, this.messageFunc);
		}
	}

	public void InitDone()
	{
		this.initialized = true;
		Kube.GPS.SetLocale(this.locale);
		this.playerUID = this.po.playerUID;
		this.age = this.po.age;
		this.sex = this.po.sex;
		this.goToMessage.SendMessage(this.messageFunc);
	}

	public void UnityVars(string ans)
	{
		JsonData jsonData = JsonMapper.ToObject(ans);
		ICollection keys = ((IDictionary)jsonData).Keys;
		string value = jsonData["sn"].ToString();
		this.platform = (SocialNet.platformType)((int)Enum.Parse(typeof(SocialNet.platformType), value));
		this.initPlatform();
		string text = string.Empty;
		foreach (object obj in keys)
		{
			string text2 = (string)obj;
			string text3 = text;
			text = string.Concat(new object[]
			{
				text3,
				text2,
				"=",
				jsonData[text2],
				"&"
			});
		}
		if (jsonData.Keys.Contains("server"))
		{
			Kube.SS.phpServer = jsonData["server"].ToString();
		}
		if (jsonData.Keys.Contains("assets"))
		{
			Kube.SS.assetPath = jsonData["assets"].ToString();
		}
		if (jsonData.Keys.Contains("ref"))
		{
			this.refSite = jsonData["ref"].ToString();
		}
		this.GetWebData(text);
	}

	private void GetWebData(string ans)
	{
		this.po.GetWebData(ans);
	}

	public bool isQuestDone()
	{
		return this._q;
	}

	public void QuestDone()
	{
		this._q = true;
	}

	public bool isMissionDone(int i)
	{
		return this._missions[i];
	}

	public SnMissionDesc[] getMissions()
	{
		Application.ExternalCall("GetMissionStatus", new object[0]);
		return this.po.getMissions();
	}

	public void OnMissionDone(string data)
	{
		int num = Convert.ToInt32(data) - 1;
		this._missions[num] = true;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["id"] = Kube.GPS.playerId.ToString();
		dictionary["mission"] = num.ToString();
		Kube.SS.Request(901, dictionary, null);
	}

	public void missionsFromServer(JsonData data)
	{
		int num = Convert.ToInt32(data["m"].ToString());
		int num2 = Convert.ToInt32(data["c"].ToString());
		for (int i = 0; i < this._missions.Length; i++)
		{
			if ((num & 1 << i) != 0)
			{
				this._missions[i] = true;
			}
		}
		this._q = (num2 == 1);
		this.socialQuest.money = data["money"].ToString();
		this.socialQuest.gold = data["gold"].ToString();
		this.socialQuest.bonus = DataUtils.StringToKwp(data["bonus"].ToString());
	}

	public void gotoMission(int par1)
	{
		this.po.gotoMission(par1);
	}

	public void InviteFrends()
	{
		Application.ExternalCall("InviteFrends", new object[0]);
	}

	public void ShowPayment(float socialNetMoney, float goldQuantity, GameObject go, string func)
	{
		this._onPaymentDone = func;
		this._pgo = go;
		this.po.ShowPayment(socialNetMoney, goldQuantity, go, func);
		if (Application.isEditor)
		{
			base.SendMessage("OnOrderSuccess", "393243550806143");
		}
	}

	public void OnUserInfo(string data)
	{
		JsonData data2 = JsonMapper.ToObject(data);
		this.po.OnUserInfo(data2);
	}

	public void OnOrderSuccess()
	{
		this._pgo.SendMessage(this._onPaymentDone);
	}

	public void OnOrderSuccess2(string payment_id)
	{
		if (this.platform == SocialNet.platformType.fb)
		{
			Kube.SS.Request(903, payment_id, delegate(string responce)
			{
				this._pgo.SendMessage(this._onPaymentDone);
			});
		}
		else
		{
			this._pgo.SendMessage(this._onPaymentDone);
		}
	}

	public void gotoUserByUID(string uid)
	{
		this.po.gotoUserByUID(uid);
	}

	public void FillFriendsRating(GameObject go, string method)
	{
		this.po.FillFriendsRating(go, method);
	}

	public void TakeScreenshot()
	{
		this.TakeScreenshot(null, null);
	}

	public void TakeScreenshot(GameObject go, string method)
	{
		this.po.TakeScreenshot(go, method);
	}

	public void APICallback(string data)
	{
		JsonData json = JsonMapper.ToObject(data);
		this.po.APICallback(json);
	}

	public void PostLevelUpOnWall(int levelNum)
	{
		if (this.platform == SocialNet.platformType.vk)
		{
			switch (levelNum)
			{
			case 1:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305298991"
				});
				break;
			case 2:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305298992"
				});
				break;
			case 3:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305298993"
				});
				break;
			case 4:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305298994"
				});
				break;
			case 5:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305298995"
				});
				break;
			case 6:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305298996"
				});
				break;
			case 7:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305298997"
				});
				break;
			case 8:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305298998"
				});
				break;
			case 9:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305298999"
				});
				break;
			case 10:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299004"
				});
				break;
			case 11:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299005"
				});
				break;
			case 12:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299008"
				});
				break;
			case 13:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299011"
				});
				break;
			case 14:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299013"
				});
				break;
			case 15:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299016"
				});
				break;
			case 16:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299020"
				});
				break;
			case 17:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299026"
				});
				break;
			case 18:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299027"
				});
				break;
			case 19:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299030"
				});
				break;
			case 20:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299032"
				});
				break;
			case 21:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299034"
				});
				break;
			case 22:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299036"
				});
				break;
			case 23:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299037"
				});
				break;
			case 24:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299040"
				});
				break;
			case 25:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299041"
				});
				break;
			case 26:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299044"
				});
				break;
			case 27:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299046"
				});
				break;
			case 28:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299049"
				});
				break;
			case 29:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299050"
				});
				break;
			case 30:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299052"
				});
				break;
			case 31:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299054"
				});
				break;
			case 32:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299058"
				});
				break;
			case 33:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299062"
				});
				break;
			case 34:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299067"
				});
				break;
			case 35:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299068"
				});
				break;
			case 36:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299069"
				});
				break;
			case 37:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299071"
				});
				break;
			case 38:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299074"
				});
				break;
			case 39:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299076"
				});
				break;
			case 40:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299079"
				});
				break;
			case 41:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299080"
				});
				break;
			case 42:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299083"
				});
				break;
			case 43:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299087"
				});
				break;
			case 44:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299094"
				});
				break;
			case 45:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299096"
				});
				break;
			case 46:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305299096"
				});
				break;
			}
		}
		else
		{
			this.po.PostLevelUpOnWall(levelNum);
		}
	}

	public void PostWeaponOnWall(int weaponNum)
	{
		if (this.platform == SocialNet.platformType.vk)
		{
			switch (weaponNum)
			{
			case 2:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305194565"
				});
				break;
			case 3:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305194566"
				});
				break;
			case 4:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305194564"
				});
				break;
			case 5:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305194561"
				});
				break;
			case 6:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305194567"
				});
				break;
			case 7:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305194570"
				});
				break;
			case 8:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305194562"
				});
				break;
			case 9:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305194563"
				});
				break;
			case 10:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305194572"
				});
				break;
			case 12:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305194578"
				});
				break;
			case 13:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_305194576"
				});
				break;
			case 14:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_313338536"
				});
				break;
			case 15:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_313338537"
				});
				break;
			case 16:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_313338545"
				});
				break;
			case 17:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_313338538"
				});
				break;
			case 18:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_313338539"
				});
				break;
			case 19:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_313338535"
				});
				break;
			case 20:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_313338552"
				});
				break;
			case 21:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_313338541"
				});
				break;
			case 22:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_313338543"
				});
				break;
			case 23:
				Screen.fullScreen = false;
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					"photo562189_313338555"
				});
				break;
			}
		}
		else
		{
			this.po.PostWeaponOnWall(weaponNum);
		}
	}

	public void PostItemOnWall(int itemNum)
	{
		this.po.PostItemOnWall(itemNum);
	}

	public void PostMissionOnWall(int missionNum)
	{
		Screen.fullScreen = false;
		Screen.lockCursor = false;
		if (this.platform == SocialNet.platformType.vk)
		{
			Application.ExternalCall("PostOnWallLevelUp", new object[]
			{
				this.playerUID,
				"photo562189_313338546"
			});
		}
		else
		{
			this.po.PostMissionOnWall(missionNum);
		}
	}

	public void PostBonusOnWall()
	{
		Screen.fullScreen = false;
		Screen.lockCursor = false;
		if (this.platform == SocialNet.platformType.vk)
		{
			Application.ExternalCall("PostOnWallLevelUp", new object[]
			{
				this.playerUID,
				"photo562189_313338546"
			});
		}
		else
		{
			this.po.PostBonusOnWall();
		}
	}

	public void PostMapSlot(int playerNumMaps)
	{
		this.po.PostMapSlot(playerNumMaps);
	}

	public void openRangTable()
	{
		this.po.openRangTable();
	}

	public SocialNet.platformType platform;

	public string locale = "ru_RU";

	public string api_url;

	public int api_id;

	public string playerUID;

	public bool initialized;

	private GameObject goToMessage;

	private string messageFunc;

	public Texture[] moneyIcon;

	public bool hasMoneyIcon = true;

	public string moneyName = "RUB";

	public float moneyValue = 1f;

	protected PlatformBase po;

	public string refSite;

	public string secret;

	public string sessionKey = string.Empty;

	public string debugID = "562189";

	public string debugKey = "562189";

	public string debugSig = "562189";

	public int sex;

	public int age;

	protected bool[] _missions = new bool[32];

	protected bool _q;

	public SocialNet.SocialQuest socialQuest = new SocialNet.SocialQuest();

	protected string _onPaymentDone;

	protected GameObject _pgo;

	public enum platformType
	{
		editor,
		debug,
		vk,
		ok,
		fb,
		mm
	}

	public class SocialQuest
	{
		public string money;

		public string gold;

		public KeyValuePair<int, int>[] bonus;
	}
}
