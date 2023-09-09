using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using kube;
using LitJson;
using UnityEngine;

public class PlatformFB : PlatformBase
{
    static Dictionary<string, int> _003C_003Ef__switch_0024map4;
	public PlatformFB(PlatformWeb owner, GameObject go, string func) : base(owner)
	{
		Kube.GPS.SetLocale("en_US", true);
	}

	public override void Init(GameObject go, string func)
	{
		this.InitFB();
	}

	private void InitFB()
	{
		if (Application.isEditor)
		{
			this.GetWebData("uid=" + this._owner.debugID + "&access_token=CAAD0Q8hFCYMBAPwZCCf9D3qaGqlFSZARBgma5YZABQRkBVWxhYOsbc3c8jF6MIBZCSGCz6R5rAOKf0p3G8VWJ5R4ewmC9eQsu6cy4kvWv7dGkKRE2ISTcFjJ9pLsLSrDeA1U12BSNGVhav2Eb7IZCHOFMqf03pVZCU1Kvgqqzudeo2nPGRElNgWo39PN7Yw1AZALmJCSoVpDQZDZD");
		}
	}

	public override SnMissionDesc[] getMissions()
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

	public override void gotoMission(int id)
	{
		if (id == 2)
		{
			Application.ExternalCall("KubePostOnWall", new object[]
			{
				"Вступай!",
				"img/50x50.jpg"
			});
		}
		else if (id == 3)
		{
			Application.ExternalCall("InviteFrends", new object[0]);
		}
		else
		{
			Application.ExternalCall("OpenUrl", new object[]
			{
				"https://www.facebook.com/cubewarscommunity"
			});
		}
	}

	public override void GetWebData(string ans)
	{
		char[] array = new char[]
		{
			'&'
		};
		string[] array2 = ans.Split(array);
		for (int i = 0; i < array2.Length; i++)
		{
			array[0] = '=';
			string[] array3 = array2[i].Split(array);
			if (array3[0] == "uid")
			{
				this.playerUID = array3[1];
			}
			if (array3[0] == "access_token")
			{
				this.access_token = array3[1];
			}
		}
		this.initialized = true;
		if (Application.isEditor)
		{
			this._owner.OnUserInfo("{ 'locale':'" + this.deflocale + "',  'gender':'male', 'age_range':{'min':13,'max':17}, 'currency':{'user_currency':'RUB','usd_exchange_inverse':'32.3'} }");
		}
		else
		{
			Application.ExternalCall("GetUserInfo", new object[]
			{
				this.playerUID
			});
		}
	}

	public override void OnUserInfo(JsonData data)
	{
		if (data.Keys.Contains("currency"))
		{
			this.currency = data["currency"]["user_currency"].ToString();
			this.usd_exchange_inverse = float.Parse(data["currency"]["usd_exchange_inverse"].ToString());
		}
		this.moneyName = this.currency;
		this.hasMoneyIcon = false;
		this.moneyValue = this.usd_exchange_inverse;
		if (data.Keys.Contains("locale"))
		{
			this._owner.locale = data["locale"].ToString();
		}
		if (data.Keys.Contains("gender"))
		{
			string text = data["gender"].ToString();
			string text2 = text;
			if (text2 != null)
			{
				if (PlatformFB._003C_003Ef__switch_0024map4 == null)
				{
					PlatformFB._003C_003Ef__switch_0024map4 = new Dictionary<string, int>(2)
					{
						{
							"male",
							0
						},
						{
							"female",
							1
						}
					};
				}
				int num;
				if (PlatformFB._003C_003Ef__switch_0024map4.TryGetValue(text2, out num))
				{
					if (num != 0)
					{
						if (num == 1)
						{
							this.sex = 1;
						}
					}
					else
					{
						this.sex = 2;
					}
				}
			}
		}
		if (data.Keys.Contains("birthday"))
		{
			string text3 = data["birthday"].ToString();
			string[] array = text3.Split(new char[]
			{
				'/'
			});
			if (array.Length > 2)
			{
				int num2 = int.Parse(array[2]);
				this.age = DateTime.Now.Year - num2;
			}
		}
		else if (data.Keys.Contains("age_range") && data["age_range"].IsObject && data["age_range"].Keys.Contains("min"))
		{
			int.TryParse(data["age_range"]["min"].ToString(), out this.age);
		}
		this._owner.InitDone();
	}

	public override void ShowPayment(float socialNetMoney, float goldQuantity, GameObject go, string func)
	{
		if ((Application.platform == RuntimePlatform.WebGLPlayer))
		{
			Screen.fullScreen = false;
			Application.ExternalCall("showPaymentBox", new object[]
			{
				goldQuantity
			});
		}
	}

	protected override string GetUserUrl(string uid)
	{
		return "http://facebook.com/profile.php?id=" + uid;
	}

	public override void FillFriendsRating(GameObject go, string method)
	{
		this._owner.StartCoroutine(this._FillFriendsRating(go, method));
	}

	private WWW wwwreq(string method, Dictionary<string, string> paramData)
	{
		StringBuilder stringBuilder = new StringBuilder(this.api_server + method + "/?");
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>(paramData);
		list.Sort((KeyValuePair<string, string> keyfirst, KeyValuePair<string, string> keylast) => keyfirst.Key.CompareTo(keylast.Key));
		foreach (KeyValuePair<string, string> keyValuePair in list)
		{
			stringBuilder.Append(keyValuePair.Key + "=" + WWW.EscapeURL(keyValuePair.Value) + "&");
		}
		stringBuilder.Append("access_token=" + WWW.EscapeURL(this.access_token));
		string text = stringBuilder.ToString();
		WWW result = new WWW(text);
		UnityEngine.Debug.Log("REQ:" + text);
		return result;
	}

	private IEnumerator _FillFriendsRating(GameObject go, string method)
	{
		UnityEngine.Debug.Log("Request friends...");
		Dictionary<string, string> paramData = new Dictionary<string, string>();
		paramData["fields"] = "installed,name";
		WWW newWWW = this.wwwreq("me/friends", paramData);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			yield break;
		}
		string Response = newWWW.text;
		UnityEngine.Debug.Log("Got friends! " + Response);
		JsonData data = JsonMapper.ToObject(Response);
		JsonData dataFriends = data["data"];
		string friendsIds = string.Empty;
		int nn = 0;
		for (int i = 0; i < dataFriends.Count; i++)
		{
			if (dataFriends[i].Keys.Contains("installed"))
			{
				if (i != 0)
				{
					friendsIds += "^";
				}
				friendsIds += (string)dataFriends[i]["id"];
				nn++;
			}
		}
		if (nn >= 3)
		{
			this._owner.OnMissionDone("3");
		}
		go.SendMessage(method, friendsIds);
		yield break;
	}

	public override void TakeScreenshot(GameObject go, string method)
	{
		this._owner.StartCoroutine(this._VKTakeScreenshot_1(go, method));
	}

	private IEnumerator _VKTakeScreenshot_1(GameObject go, string method)
	{
		string uploadUrl = this.api_server + "me/photos?access_token=" + WWW.EscapeURL(this.access_token);
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
		tex.Apply();
		byte[] bytes = tex.EncodeToPNG();
		UnityEngine.Object.Destroy(tex);
		WWWForm form = new WWWForm();
		form.AddBinaryData("source", bytes, "screenShot.png", "image/png");
		form.AddField("message", "screenshot");
		WWW sendWWW = new WWW(uploadUrl, form);
		yield return sendWWW;
		string Response = sendWWW.text;
		UnityEngine.Debug.Log("Got photo upload! " + Response);
		go.SendMessage(method);
		yield break;
	}

	public override void openRangTable()
	{
		Application.ExternalCall("OpenUrl", new object[]
		{
			"https://www.facebook.com/cubewarscommunity"
		});
	}

	public override void PostWeaponOnWall(int weaponNum)
	{
		if (weaponNum >= 2)
		{
			Application.ExternalCall("KubePostStory", new object[]
			{
				"buy",
				"weapon",
				"weapon" + weaponNum
			});
		}
	}

	public override void PostBonusOnWall()
	{
		Application.ExternalCall("KubePostStory", new object[]
		{
			"complete",
			"quest",
			"quest"
		});
	}

	public override void PostMissionOnWall(int missionNum)
	{
		Application.ExternalCall("KubePostStory", new object[]
		{
			"complete",
			"mission",
			"mission" + missionNum
		});
	}

	public override void PostViralOnWall(int missionNum)
	{
		Application.ExternalCall("KubePostStory", new object[]
		{
			"complete",
			"quest",
			"quest"
		});
	}

	public override void PostLevelUpOnWall(int levelNum)
	{
		Application.ExternalCall("KubePostStory", new object[]
		{
			"gain",
			"level",
			"level" + levelNum
		});
	}

	public override void PostMapSlot(int playerNumMaps)
	{
		Application.ExternalCall("KubePostStory", new object[]
		{
			"buy",
			"map",
			"map" + playerNumMaps
		});
	}

	public const string RANKTABLE = "https://www.facebook.com/cubewarscommunity";

	public const string GROUP_URL = "https://www.facebook.com/cubewarscommunity";

	private const string LOCALE = "en_US";

	public string api_server = "https://graph.facebook.com/";

	public int api_id;

	public string deflocale = LocaleEnum.en_US.ToString();

	public bool initialized;

	public Texture[] moneyIcon;

	private string access_token = string.Empty;

	private SnMissionDesc[] _missionNames;

	private string session_key;

	private string application_key;

	private string auth_sig;

	private string session_secret_key;

	private string currency = string.Empty;

	private float usd_exchange_inverse;
}
