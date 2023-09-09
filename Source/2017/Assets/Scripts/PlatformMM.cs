using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using LitJson;
using UnityEngine;

public class PlatformMM : PlatformBase
{
	public PlatformMM(PlatformWeb owner, GameObject go, string func) : base(owner)
	{
	}

	public override SnMissionDesc[] getMissions()
	{
		return this._missionNames;
	}

	public override void APICallback(JsonData json)
	{
	}

	public override void openRangTable()
	{
		Application.ExternalCall("OpenUrl", new object[]
		{
			"http://my.mail.ru/group/51761368727691"
		});
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
				"http://my.mail.ru/group/51761368727691"
			});
		}
	}

	public override void Init(GameObject go, string func)
	{
		this.InitOK();
	}

	private void InitOK()
	{
		if (Application.isEditor)
		{
			this.GetWebData("app_id=714347&session_key=05acbb608acdc0f0ccda62c342e3ef05&vid=" + this._owner.debugID);
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
			if (array3[0] == "session_key")
			{
				this.session_key = array3[1];
			}
			if (array3[0] == "app_id")
			{
				this.api_id = array3[1];
			}
			if (array3[0] == "vid")
			{
				this.playerUID = array3[1];
			}
		}
		this.initialized = true;
		this.moneyName = "мэйликов";
		this.hasMoneyIcon = false;
		this._owner.InitDone();
	}

	public override void ShowPayment(float socialNetMoney, float goldQuantity, GameObject go, string func)
	{
		Screen.fullScreen = false;
		Application.ExternalCall("showPaymentBox", new object[]
		{
			(int)socialNetMoney
		});
		MonoBehaviour.print("ShowPaymentBox");
	}

	protected override string GetUserUrl(string uid)
	{
		return string.Empty;
	}

	public override void FillFriendsRating(GameObject go, string method)
	{
		this._owner.StartCoroutine(this._FillFriendsRating(go, method));
	}

	public static string GetMD5Hash(string input)
	{
		MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = Encoding.UTF8.GetBytes(input);
		array = md5CryptoServiceProvider.ComputeHash(array);
		StringBuilder stringBuilder = new StringBuilder();
		foreach (byte b in array)
		{
			stringBuilder.Append(b.ToString("x2").ToLower());
		}
		return stringBuilder.ToString();
	}

	private WWW wwwreq(Dictionary<string, string> paramData, WWWForm form = null)
	{
		StringBuilder stringBuilder = new StringBuilder(this.api_server + "?");
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>(paramData);
		list.Sort((KeyValuePair<string, string> keyfirst, KeyValuePair<string, string> keylast) => keyfirst.Key.CompareTo(keylast.Key));
		StringBuilder stringBuilder2 = new StringBuilder(this.playerUID);
		foreach (KeyValuePair<string, string> keyValuePair in list)
		{
			stringBuilder2.Append(keyValuePair.Key + "=" + keyValuePair.Value);
			stringBuilder.Append(keyValuePair.Key + "=" + WWW.EscapeURL(keyValuePair.Value) + "&");
		}
		stringBuilder2.Append(this.private_key);
		stringBuilder.Append("sig=" + PlatformMM.GetMD5Hash(stringBuilder2.ToString()).ToLower());
		string text = stringBuilder.ToString();
		WWW result;
		if (form != null)
		{
			result = new WWW(text, form);
		}
		else
		{
			result = new WWW(text);
		}
		UnityEngine.Debug.Log("REQ:" + text);
		return result;
	}

	private IEnumerator _FillFriendsRating(GameObject go, string method)
	{
		UnityEngine.Debug.Log("Request friends...");
		Dictionary<string, string> paramData = new Dictionary<string, string>();
		paramData["app_id"] = this.api_id;
		paramData["session_key"] = this.session_key;
		paramData["format"] = "JSON";
		paramData["method"] = "friends.getAppUsers";
		WWW newWWW = this.wwwreq(paramData, null);
		yield return newWWW;
		JsonData fdata = JsonMapper.ToObject(newWWW.text);
		if (fdata.Count == 0)
		{
			go.SendMessage(method, this.playerUID);
			yield break;
		}
		int nnFrinds = fdata.Count;
		if (nnFrinds >= 3)
		{
			this._owner.OnMissionDone("3");
		}
		string friendsIds = string.Empty;
		string uids = string.Empty;
		for (int i = 0; i < fdata.Count; i++)
		{
			string id = fdata[i].ToString();
			if (i != 0)
			{
				friendsIds += "^";
				uids += ",";
			}
			friendsIds = friendsIds + string.Empty + id;
			uids = uids + string.Empty + id;
		}
		go.SendMessage(method, friendsIds);
		paramData = new Dictionary<string, string>();
		paramData["app_id"] = this.api_id;
		paramData["session_key"] = this.session_key;
		paramData["format"] = "JSON";
		paramData["method"] = "users.getInfo";
		paramData["uids"] = uids;
		newWWW = this.wwwreq(paramData, null);
		yield return newWWW;
		JsonData data = JsonMapper.ToObject(newWWW.text);
		string[] friendsListName = new string[data.Count];
		Texture[] friendsListTex = new Texture[data.Count];
		WWW[] texWWW = new WWW[data.Count];
		for (int j = 0; j < data.Count; j++)
		{
			friendsListName[j] = data[j]["nick"].ToString();
			texWWW[j] = new WWW(data[j]["pic_40"].ToString());
		}
		go.SendMessage(method, friendsListName);
		for (int k = 0; k < texWWW.Length; k++)
		{
			yield return texWWW[k];
			friendsListTex[k] = texWWW[k].texture;
		}
		go.SendMessage(method, friendsListTex);
		yield break;
	}

	public override void TakeScreenshot(GameObject go, string method)
	{
		this._owner.StartCoroutine(this._VKTakeScreenshot_1(go, method));
	}

	private IEnumerator _VKTakeScreenshot_1(GameObject go, string method)
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
		tex.Apply();
		byte[] bytes = tex.EncodeToPNG();
		UnityEngine.Object.Destroy(tex);
		Dictionary<string, string> paramData = new Dictionary<string, string>();
		paramData["aid"] = "1";
		paramData["app_id"] = this.api_id;
		paramData["session_key"] = this.session_key;
		paramData["format"] = "JSON";
		paramData["method"] = "photos.upload";
		WWWForm form = new WWWForm();
		form.AddBinaryData("img_file", bytes, "screenShot.png", "image/png");
		WWW newWWW = this.wwwreq(paramData, form);
		yield return newWWW;
		UnityEngine.Debug.Log(newWWW.text);
		Screen.fullScreen = false;
		if (go)
		{
			go.SendMessage(method);
		}
		yield break;
	}

	public override void PostMissionOnWall(int missionNum)
	{
		Application.ExternalCall("KubePostOnWall", new object[]
		{
			"Миссия пройдена!",
			"img/50x50.jpg"
		});
	}

	public override void PostLevelUpOnWall(int levelNum)
	{
		string text = string.Format("У меня новое звание - {0}!", Localize.RankName[levelNum]);
		Application.ExternalCall("KubePostOnWall", new object[]
		{
			text,
			"img/50x50.jpg"
		});
	}

	public override void PostWeaponOnWall(int weaponNum)
	{
		string text = string.Format("Я купил новое оружие - {0}!", Localize.weaponNames[weaponNum]);
		Application.ExternalCall("KubePostOnWall", new object[]
		{
			text,
			"img/50x50.jpg"
		});
	}

	public override void PostBonusOnWall()
	{
		Application.ExternalCall("KubePostOnWall", new object[]
		{
			"Я получил награду!",
			"img/50x50.jpg"
		});
	}

	public override void gotoUserByUID(string uid)
	{
		if (uid != string.Empty)
		{
			Application.ExternalCall("GotoUserByUID", new object[]
			{
				uid
			});
		}
	}

	private string private_key = "8f1fa09535eb3080745aa2307af8343d";

	private string api_server = "http://www.appsmail.ru/platform/api";

	public string api_id;

	public bool initialized;

	private SnMissionDesc[] _missionNames = new SnMissionDesc[]
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
			name = Localize.social_group
		}
	};

	private string session_key;
}
