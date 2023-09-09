using System;
using System.Collections;
using System.Collections.Generic;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class SocialNet : MonoBehaviour, IPlatform
{
	public bool isVIPPlatform
	{
		get
		{
			return false;
		}
	}

	public bool canPostOnWall
	{
		get
		{
			return true;
		}
	}

	public platformType platform
	{
		get
		{
			return this._platform;
		}
		set
		{
			this._platform = value;
		}
	}

	public string playerUID
	{
		get
		{
			return this._playerUID;
		}
		set
		{
			this._playerUID = value;
		}
	}

	public string locale
	{
		get
		{
			return this._locale;
		}
		set
		{
			this._locale = value;
		}
	}

	public string sessionKey
	{
		get
		{
			return this._sessionKey;
		}
		set
		{
			this._sessionKey = value;
		}
	}

	public string secret
	{
		get
		{
			return this._secret;
		}
		set
		{
			this._secret = value;
		}
	}

	public string refSite
	{
		get
		{
			return this._refSite;
		}
	}

	public Texture moneyIconTx
	{
		get
		{
			return this.moneyIcon[(int)this.platform];
		}
	}

	public bool hasMoneyIcon
	{
		get
		{
			return this.po.hasMoneyIcon;
		}
	}

	public string moneyName
	{
		get
		{
			return this.po.moneyName;
		}
	}

	public float moneyValue
	{
		get
		{
			return this.po.moneyValue;
		}
	}

	public int sex
	{
		get
		{
			return this.po.sex;
		}
	}

	public int age
	{
		get
		{
			return this.po.age;
		}
	}

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
	}

	public void gotoViralTask(int i, int task)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (task == 2)
		{
			this.po.PostViralOnWall(i);
		}
		else
		{
			this.openURL("https://play.google.com/store/apps/details?id=com.Nobodyshot.deadzone");
		}
		this._viralEvent[i].state |= 1 << task;
		dictionary["vid"] = i.ToString();
		dictionary["value"] = this._viralEvent[i].state.ToString();
		Kube.SS.Request(906, dictionary, null);
	}

	public bool isViralTaskDone(int i, int task)
	{
		return (this._viralEvent[i].state & 1 << task) != 0;
	}

	public void EventDone(int i, VoidCallback onTakeBonus)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		this._viralEvent[i].state = 131071;
		dictionary["vid"] = i.ToString();
		dictionary["value"] = this._viralEvent[i].state.ToString();
		Kube.SS.Request(906, dictionary, delegate(string ans)
		{
			JsonData jsonData = JsonMapper.ToObject(ans);
			this._viralEvent[i].state = int.Parse(jsonData["value"].ToString());
			if (onTakeBonus != null && this._viralEvent[i].state >= 65536)
			{
				onTakeBonus();
			}
		});
	}

	public ViralEvent getViralEvent(int i)
	{
		return this._viralEvent[i];
	}

	public bool isViralEventDone(int i, IntCallback cb = null)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["vid"] = i.ToString();
		bool flag = this._viralEvent[i].state >= 65536;
		if (cb != null && flag)
		{
			cb(this._viralEvent[i].state);
			return flag;
		}
		if (cb == null)
		{
			return flag;
		}
		Kube.SS.Request(906, dictionary, delegate(string ans)
		{
			JsonData jsonData = JsonMapper.ToObject(ans);
			this._viralEvent[i].state = int.Parse(jsonData["value"].ToString());
			if (cb != null)
			{
				cb(this._viralEvent[i].state);
			}
		});
		return false;
	}

	private void initPlatform()
	{
		if (this.platform == platformType.vk)
		{
			this.po = new PlatformVK(this, this.goToMessage, this.messageFunc);
		}
		else if (this.platform == platformType.ok)
		{
			this.po = new PlatformOK(this, this.goToMessage, this.messageFunc);
		}
		else if (this.platform == platformType.fb)
		{
			this.po = new PlatformFB(this, this.goToMessage, this.messageFunc);
		}
		else if (this.platform == platformType.mm)
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
		this.goToMessage.SendMessage(this.messageFunc);
	}

	public void UnityVars(string ans)
	{
		JsonData jsonData = JsonMapper.ToObject(ans);
		ICollection keys = ((IDictionary)jsonData).Keys;
		string value = jsonData["sn"].ToString();
		this.platform = (platformType)((int)Enum.Parse(typeof(platformType), value));
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
		ServerScript serverScript = Kube.SS as ServerScript;
		ResourceManager resourceManager = Kube.RM as ResourceManager;
		string phpServer = string.Empty;
		string mainPhpScript = "mainScript.php";
		if (jsonData.Keys.Contains("server"))
		{
			phpServer = jsonData["server"].ToString();
		}
		if (jsonData.Keys.Contains("mainPhpScript"))
		{
			mainPhpScript = jsonData["mainPhpScript"].ToString();
		}
		serverScript.Init(phpServer, mainPhpScript);
		if (jsonData.Keys.Contains("assets"))
		{
			resourceManager.Init(jsonData["assets"].ToString());
		}
		if (jsonData.Keys.Contains("ref"))
		{
			this._refSite = jsonData["ref"].ToString();
		}
		this.po.GetWebData(text);
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
		dictionary["id"] = Kube.SS.serverId.ToString();
		dictionary["mission"] = num.ToString();
		Kube.SS.Request(901, dictionary, null);
	}

	public SocialQuest socialQuest
	{
		get
		{
			return this._socialQuest;
		}
		set
		{
			this._socialQuest = value;
		}
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

	public void ShowPayment(int k, GameObject go, string func)
	{
		float socialNetMoney = Kube.GPS.exchangeSpec[k, 0];
		float goldQuantity = Kube.GPS.exchangeSpec[k, 3];
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
		if (this.platform == platformType.fb)
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
		this.po.PostLevelUpOnWall(levelNum);
	}

	public void PostWeaponOnWall(int weaponNum)
	{
		this.po.PostWeaponOnWall(weaponNum);
	}

	public void PostItemOnWall(int itemNum)
	{
		Screen.fullScreen = false;
		KubeScreen.lockCursor = false;
		this.po.PostItemOnWall(itemNum);
	}

	public void PostMissionOnWall(int missionNum)
	{
		Screen.fullScreen = false;
		KubeScreen.lockCursor = false;
		this.po.PostMissionOnWall(missionNum);
	}

	public void PostBonusOnWall()
	{
		Screen.fullScreen = false;
		KubeScreen.lockCursor = false;
		this.po.PostBonusOnWall();
	}

	public void PostMapSlot(int playerNumMaps)
	{
		this.po.PostMapSlot(playerNumMaps);
	}

	public void openRangTable()
	{
		this.po.openRangTable();
	}

	public bool checkGroupLink(string home)
	{
		return this.po.checkGroupLink(home);
	}

	public void openURL(string url)
	{
		Application.ExternalCall("OpenUrl", new object[]
		{
			url
		});
	}

	public void BuyPack(PackInfo info)
	{
		this._onPaymentDone = "PackAnswer";
		this._pgo = Kube.IS.gameObject;
		this.po.BuyPack(info);
		if (Application.isEditor)
		{
			base.SendMessage("OnOrderSuccess", "393243550806143");
		}
	}

	public string MoneyNameForPack(PackInfo info)
	{
		return this.po.moneyNameForPack(info);
	}

	public void PostWeaponSkinOnWall(int weaponId)
	{
	}

	public static JsonData ReadConfig(string filePath)
	{
		return null;
	}

	[SerializeField]
	private platformType _platform;

	[NonSerialized]
	private string _locale = LocaleEnum.ru_RU.ToString();

	public string api_url;

	public int api_id;

	public string _playerUID;

	public bool initialized;

	private GameObject goToMessage;

	private string messageFunc;

	public Texture[] moneyIcon;

	public PlatformBase po;

	public string _refSite;

	[NonSerialized]
	public string _secret;

	[NonSerialized]
	public string _sessionKey = string.Empty;

	[NonSerialized]
	public string debugID = "562189";

	[NonSerialized]
	public string debugKey = "562189";

	[NonSerialized]
	public string debugSig = "562189";

	protected ViralEvent[] _viralEvent = new ViralEvent[]
	{
		new ViralEvent()
	};

	protected bool[] _missions = new bool[32];

	protected bool _q;

	public SocialQuest _socialQuest = new SocialQuest();

	protected string _onPaymentDone;

	protected GameObject _pgo;
}
