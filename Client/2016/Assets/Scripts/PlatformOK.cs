using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using kube.data;
using LitJson;
using UnityEngine;

public class PlatformOK : PlatformBase
{
	public PlatformOK(SocialNet owner, GameObject go, string func) : base(owner)
	{
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
					name = Localize.social_group
				}
			};
		}
		return this._missionNames;
	}

	public override void APICallback(JsonData json)
	{
	}

	public override void openRangTable()
	{
		Application.ExternalCall("OpenUrl", new object[]
		{
			"http://odnoklassniki.ru/group/51761368727691"
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
				"http://odnoklassniki.ru/group/51761368727691"
			});
		}
	}

	public override void PostMissionOnWall(int missionNum)
	{
		Application.ExternalCall("KubePostOnWall", new object[]
		{
			"Миссия пройдена!",
			"img/50x50.jpg"
		});
	}

	public override void PostViralOnWall(int missionNum)
	{
		Application.ExternalCall("KubePostOnWall", new object[]
		{
			"Я играю в DeadZone и получаю награду 20 монет!",
			"img/50x50.jpg"
		});
	}

	public override void PostLevelUpOnWall(int levelNum)
	{
		string text = string.Format("У меня новое звание - {0}!", Localize.RankName[levelNum]);
		Application.ExternalCall("KubePostOnWall", new object[]
		{
			text,
			"ok/l/" + levelNum + ".png"
		});
	}

	public override void PostWeaponOnWall(int weaponNum)
	{
		string text = string.Format("Я купил новое оружие - {0}!", Localize.weaponNames[weaponNum]);
		Application.ExternalCall("KubePostOnWall", new object[]
		{
			text,
			"ok/w/" + weaponNum + ".png"
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

	public override void Init(GameObject go, string func)
	{
		this.InitOK();
	}

	private void InitOK()
	{
		if (Application.isEditor)
		{
			this.GetWebData(string.Concat(new string[]
			{
				"authorized=1&api_server=http://api.odnoklassniki.ru/&apiconnection=204601344_1400240762116&first_start=0&session_secret_key=",
				this.session_secret_key,
				"&clientLog=0&application_key=CBAJPCDNABABABABA&auth_sig=",
				this._owner.debugSig,
				"&session_key=",
				this._owner.debugKey,
				"&logged_user_id=",
				this._owner.debugID,
				"&web_server=www.odnoklassniki.ru&sig=d1192d91404eebadc4845da30c2c6c6a"
			}));
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
			if (array3[0] == "api_server")
			{
				this.api_server = array3[1];
			}
			if (array3[0] == "session_key")
			{
				this.session_key = array3[1];
			}
			if (array3[0] == "application_key")
			{
				this.application_key = array3[1];
			}
			if (array3[0] == "auth_sig")
			{
				this.auth_sig = array3[1];
			}
			if (array3[0] == "session_key")
			{
				this.session_key = array3[1];
			}
			if (array3[0] == "session_secret_key")
			{
				this.session_secret_key = array3[1];
			}
			if (array3[0] == "api_id")
			{
				this.api_id = Convert.ToInt32(array3[1]);
			}
			if (array3[0] == "logged_user_id")
			{
				this.playerUID = array3[1];
			}
		}
		this.initialized = true;
		JsonData jsonData = new JsonData();
		jsonData["session_key"] = this.session_key;
		jsonData["auth_sig"] = this.auth_sig;
		this._owner.secret = this.auth_sig;
		this._owner.sessionKey = this.session_key;
		this._owner.InitDone();
	}

	public override void ShowPayment(float socialNetMoney, float goldQuantity, GameObject go, string func)
	{
		if ((Application.platform == RuntimePlatform.WebGLPlayer))
		{
			Screen.fullScreen = false;
			Application.ExternalCall("showPaymentBox", new object[]
			{
				(int)socialNetMoney
			});
		}
	}

	protected override string GetUserUrl(string uid)
	{
		return "http://odnoklassniki.ru/profile/" + uid;
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

	private WWW wwwreq(Dictionary<string, string> paramData)
	{
		StringBuilder stringBuilder = new StringBuilder(this.api_server + "fb.do?");
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>(paramData);
		list.Sort((KeyValuePair<string, string> keyfirst, KeyValuePair<string, string> keylast) => keyfirst.Key.CompareTo(keylast.Key));
		StringBuilder stringBuilder2 = new StringBuilder();
		foreach (KeyValuePair<string, string> keyValuePair in list)
		{
			stringBuilder2.Append(keyValuePair.Key + "=" + keyValuePair.Value);
			stringBuilder.Append(keyValuePair.Key + "=" + WWW.EscapeURL(keyValuePair.Value) + "&");
		}
		stringBuilder2.Append(this.session_secret_key);
		stringBuilder.Append("sig=" + PlatformOK.GetMD5Hash(stringBuilder2.ToString()).ToLower());
		string text = stringBuilder.ToString();
		WWW result = new WWW(text);
		UnityEngine.Debug.Log("REQ:" + text);
		return result;
	}

	private IEnumerator _FillFriendsRating(GameObject go, string method)
	{
		UnityEngine.Debug.Log("Request friends...");
		Dictionary<string, string> paramData = new Dictionary<string, string>();
		paramData["application_key"] = this.application_key;
		paramData["session_key"] = this.session_key;
		paramData["format"] = "XML";
		paramData["method"] = "friends.getAppUsers";
		WWW newWWW = this.wwwreq(paramData);
		yield return newWWW;
		string Response = newWWW.text;
		UnityEngine.Debug.Log("Got friends! " + Response);
		Response = Regex.Replace(Response, "(<)([0-9]+)", "$1El$2");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(Response);
		XmlNodeList xmlNodeUID = xmlDoc.GetElementsByTagName("uid");
		string friendsIds = string.Empty;
		string uids = string.Empty;
		if (xmlNodeUID.Count == 0)
		{
			go.SendMessage(method, this.playerUID);
			yield break;
		}
		int nnFrinds = xmlNodeUID.Count;
		if (nnFrinds >= 3)
		{
			this._owner.OnMissionDone("3");
		}
		for (int i = 0; i < xmlNodeUID.Count; i++)
		{
			if (i != 0)
			{
				friendsIds += "^";
				uids += ",";
			}
			friendsIds = friendsIds + string.Empty + xmlNodeUID[i].InnerXml;
			uids = uids + string.Empty + xmlNodeUID[i].InnerXml;
		}
		go.SendMessage(method, friendsIds);
		paramData = new Dictionary<string, string>();
		paramData["application_key"] = this.application_key;
		paramData["session_key"] = this.session_key;
		paramData["format"] = "JSON";
		paramData["method"] = "users.getInfo";
		paramData["uids"] = uids;
		paramData["fields"] = "uid,name,pic_2";
		newWWW = this.wwwreq(paramData);
		yield return newWWW;
		JsonData data = JsonMapper.ToObject(newWWW.text);
		string[] friendsListName = new string[data.Count];
		Texture[] friendsListTex = new Texture[data.Count];
		WWW[] texWWW = new WWW[data.Count];
		for (int j = 0; j < data.Count; j++)
		{
			friendsListName[j] = data[j]["name"].ToString();
			texWWW[j] = new WWW(data[j]["pic_2"].ToString());
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
		string photo_id = string.Empty;
		Dictionary<string, string> paramData = new Dictionary<string, string>();
		paramData["application_key"] = this.application_key;
		paramData["session_key"] = this.session_key;
		paramData["format"] = "JSON";
		paramData["method"] = "photosV2.getUploadUrl";
		WWW newWWW = this.wwwreq(paramData);
		yield return newWWW;
		UnityEngine.Debug.Log(newWWW.text);
		string response = newWWW.text;
		JsonData data = JsonMapper.ToObject(response);
		string uploadUrl = string.Empty;
		yield return new WaitForEndOfFrame();
		uploadUrl = (string)data["upload_url"];
		photo_id = (string)data["photo_ids"][0];
		if (uploadUrl == string.Empty)
		{
			yield break;
		}
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
		tex.Apply();
		byte[] bytes = tex.EncodeToPNG();
		UnityEngine.Object.Destroy(tex);
		WWWForm form = new WWWForm();
		form.AddBinaryData("photo", bytes, "screenShot.png", "image/png");
		WWW sendWWW = new WWW(uploadUrl, form);
		yield return sendWWW;
		response = sendWWW.text;
		UnityEngine.Debug.Log(response);
		yield return new WaitForEndOfFrame();
		data = JsonMapper.ToObject(response);
		string token = string.Empty;
		JsonData dataPhotos = data["photos"];
		ICollection keys = ((IDictionary)dataPhotos).Keys;
		foreach (object obj in keys)
		{
			string elem = (string)obj;
			token = (string)dataPhotos[elem]["token"];
		}
		yield return new WaitForEndOfFrame();
		paramData = new Dictionary<string, string>();
		paramData["application_key"] = this.application_key;
		paramData["session_key"] = this.session_key;
		paramData["format"] = "JSON";
		paramData["method"] = "photosV2.commit";
		paramData["photo_id"] = photo_id;
		paramData["token"] = token;
		WWW saveWWW = this.wwwreq(paramData);
		yield return saveWWW;
		UnityEngine.Debug.Log(saveWWW.text);
		data = JsonMapper.ToObject(saveWWW.text);
		string photoAddr = (string)data["photos"][0]["assigned_photo_id"];
		Application.ExternalCall("PostOnWallLevelUp", new object[]
		{
			this.playerUID,
			photoAddr
		});
		Screen.fullScreen = false;
		if (go)
		{
			go.SendMessage(method);
		}
		yield break;
	}

	public override void BuyPack(PackInfo info)
	{
		int id = info.id;
		Screen.fullScreen = false;
		Application.ExternalEval(string.Concat(new object[]
		{
			"var params = {type: \"item\",item: \"item",
			id.ToString(),
			"\", price: ",
			info.price,
			"};\n showOrderBox( params );"
		}));
	}

	public string api_server;

	public int api_id;

	public bool initialized;

	public Texture[] moneyIcon;

	private SnMissionDesc[] _missionNames;

	private string session_key;

	private string application_key;

	private string auth_sig;

	private string session_secret_key = "582d21fbba650f5c8aa2b64d53de1997";
}
