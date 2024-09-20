using System;
using System.Collections;
using System.Text;
using kube.data;
using LitJson;
using UnityEngine;

public class PlatformVK : PlatformBase
{
	public PlatformVK(SocialNet owner, GameObject go, string func) : base(owner)
	{
		this.viralConfig = Resources.Load<SNViral>("SN/vk");
	}

	public override void openRangTable()
	{
		Application.ExternalCall("OpenUrl", new object[]
		{
			"http://vk.com/club52386046?z=photo-52386046_302845891%2Falbum-52386046_00%2Frev"
		});
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
					name = Localize.social_tell_friends
				},
				new SnMissionDesc
				{
					id = 2,
					name = Localize.social_menu
				},
				new SnMissionDesc
				{
					id = 3,
					name = Localize.social_invite_num_friends
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

	public override void gotoMission(int id)
	{
		if (id == 1)
		{
			Application.ExternalCall("PostInvite", new object[]
			{
				"photo-52386046_372794088"
			});
		}
		else if (id == 2)
		{
			Application.ExternalCall("InstallMenu", new object[0]);
		}
		else if (id == 3)
		{
			Application.ExternalCall("InviteFrends", new object[0]);
		}
		else
		{
			Application.ExternalCall("OpenUrl", new object[]
			{
				"https://vk.com/club52386046"
			});
		}
	}

	public override void Init(GameObject go, string func)
	{
		this.goToMessage = go;
		this.messageFunc = func;
	}

	public override void GetWebData(string ans)
	{
		this.initialized = true;
		if (Application.isEditor)
		{
			this.playerUID = this._owner.debugID;
			this._owner.OnUserInfo("{ 'bdate': '28.3' , 'sex':2 }");
			return;
		}
		char[] array = new char[]
		{
			'&'
		};
		string[] array2 = ans.Split(array);
		for (int i = 0; i < array2.Length; i++)
		{
			array[0] = '=';
			string[] array3 = array2[i].Split(array);
			if (array3[0] == "api_url")
			{
				this.api_url = array3[1];
			}
			if (array3[0] == "api_id")
			{
				this.api_id = Convert.ToInt32(array3[1]);
			}
			if (array3[0] == "viewer_id")
			{
				this.playerUID = array3[1];
			}
			if (array3[0] == "access_token")
			{
				this.access_token = array3[1];
			}
			if (array3[0] == "auth_key")
			{
				this._owner.secret = array3[1];
			}
		}
		Application.ExternalCall("GetUserInfo", new object[]
		{
			this.playerUID
		});
	}

	public override void OnUserInfo(JsonData data)
	{
		if (data.Keys.Contains("sex"))
		{
			this.sex = (int)data["sex"];
		}
		if (data.Keys.Contains("bdate"))
		{
			string text = data["bdate"].ToString();
			string[] array = text.Split(new char[]
			{
				'.'
			});
			if (array.Length > 2)
			{
				int num = int.Parse(array[2]);
				this.age = DateTime.Now.Year - num;
			}
		}
		this._owner.InitDone();
	}

	public override void ShowPayment(float socialNetMoney, float goldQuantity, GameObject go, string func)
	{
		this.goToMessage = go;
		this.messageFunc = func;
		Screen.fullScreen = false;
		Application.ExternalEval("var params = {type: \"votes\",votes: " + (int)socialNetMoney + "};\nVK.callMethod(\"showOrderBox\", params);");
	}

	public void OnOrderSuccess()
	{
		this.goToMessage.SendMessage(this.messageFunc);
	}

	protected override string GetUserUrl(string uid)
	{
		return "http://vk.com/id" + uid;
	}

	public override void FillFriendsRating(GameObject go, string method)
	{
		this._owner.StartCoroutine(this._FillFriendsRating(go, method));
	}

	private void tryLoadTexture(Texture2D texture, WWW www)
	{
		try
		{
			texture.LoadImage(www.bytes);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log("Bad texture domain " + www.url);
		}
	}

	private IEnumerator _FillFriendsRating(GameObject go, string method)
	{
		UnityEngine.Debug.Log("Request friends...");
		WWW newWWW = new WWW("https://api.vk.com/method/friends.getAppUsers?access_token=" + this.access_token);
		yield return newWWW;
		string Response = newWWW.text;
		JsonData json = JsonMapper.ToObject(Response);
		if (json.Keys.Contains("error"))
		{
			yield break;
		}
		if (json.Keys.Contains("response"))
		{
			json = json["response"];
		}
		UnityEngine.Debug.Log("Got friends! " + Response);
		string friendsIds = string.Empty;
		string uids = string.Empty;
		if (!json.IsArray)
		{
			yield break;
		}
		for (int i = 0; i < json.Count; i++)
		{
			if (i != 0)
			{
				friendsIds += "^";
				uids += ",";
			}
			friendsIds = friendsIds + string.Empty + Convert.ToInt32(json[i].ToString());
		}
		go.SendMessage(method, friendsIds);
		int nnFrinds = json.Count;
		if (nnFrinds >= 3)
		{
			this._owner.OnMissionDone("3");
		}
		int nnOffset = 0;
		int nnLeft = 0;
		JsonData friendsJSON = new JsonData();
		friendsJSON.SetJsonType(JsonType.Array);
		while (nnOffset < json.Count)
		{
			uids = string.Empty;
			nnLeft = Math.Min(100, json.Count - nnOffset);
			int j = 0;
			while (j < nnLeft)
			{
				if (j != 0)
				{
					uids += ",";
				}
				uids = uids + string.Empty + Convert.ToInt32(json[nnOffset].ToString());
				j++;
				nnOffset++;
			}
			WWW newWWW2 = new WWW("https://api.vk.com/method/getProfiles?uids=" + uids + "&fields=first_name,last_name,photo&https=1");
			yield return newWWW2;
			Response = newWWW2.text;
			JsonData jsonProfiles = JsonMapper.ToObject(Response);
			if (jsonProfiles.Keys.Contains("error"))
			{
				yield break;
			}
			if (jsonProfiles.Keys.Contains("response"))
			{
				jsonProfiles = jsonProfiles["response"];
				for (int k = 0; k < jsonProfiles.Count; k++)
				{
					friendsJSON.Add(jsonProfiles[k]);
				}
			}
		}
		json = friendsJSON;
		int nninfo = json.Count;
		string[] friendsListName = new string[nninfo];
		Texture[] friendsListTex = new Texture[nninfo];
		WWW[] texWWW = new WWW[nninfo];
		for (int l = 0; l < nninfo; l++)
		{
			string first_name = json[l]["first_name"].ToString();
			string last_name = json[l]["last_name"].ToString();
			string photo = json[l]["photo"].ToString();
			friendsListName[l] = first_name + " " + last_name;
			friendsListTex[l] = new Texture2D(1, 1, TextureFormat.RGB24, false);
			texWWW[l] = new WWW(photo);
		}
		go.SendMessage(method, friendsListName);
		for (int m = 0; m < nninfo; m++)
		{
			yield return texWWW[m];
			if (texWWW[m].error == null || !(texWWW[m].error != string.Empty))
			{
				this.tryLoadTexture((Texture2D)friendsListTex[m], texWWW[m]);
				go.SendMessage(method, friendsListTex);
			}
		}
		yield break;
	}

	public override void TakeScreenshot(GameObject go, string method)
	{
		this._owner.StartCoroutine(this._VKTakeScreenshot_1());
	}

	private IEnumerator _VKTakeScreenshot_1()
	{
		WWW newWWW = new WWW(string.Concat(new string[]
		{
			"https://api.vk.com/method/photos.getWallUploadServer?uid=",
			this.playerUID,
			"&gid=",
			this.playerUID,
			"&save_big=1&access_token=",
			this.access_token,
			"&https=1"
		}));
		yield return newWWW;
		UnityEngine.Debug.Log(newWWW.text);
		string uploadUrl = string.Empty;
		char[] dc = new char[]
		{
			'"'
		};
		string[] strs = newWWW.text.Split(dc);
		for (int i = 0; i < strs.Length; i++)
		{
			if (strs[i] == "upload_url")
			{
				uploadUrl = strs[i + 2];
				break;
			}
		}
		yield return new WaitForEndOfFrame();
		uploadUrl = uploadUrl.Replace("\\/", "/");
		UnityEngine.Debug.Log(uploadUrl);
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		yield return new WaitForEndOfFrame();
		tex.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
		tex.Apply();
		UnityEngine.Debug.Log("ReadPixels done");
		byte[] bytes = tex.EncodeToPNG();
		UnityEngine.Object.Destroy(tex);
		WWWForm form = new WWWForm();
		form.AddBinaryData("photo", bytes, "screenShot.png", "image/png");
		WWW sendWWW = new WWW(uploadUrl, form);
		yield return sendWWW;
		string response = Encoding.ASCII.GetString(sendWWW.bytes);
		UnityEngine.Debug.Log(response);
		int photoIndex = response.IndexOf("photo");
		int hashIndex = response.IndexOf("hash");
		string photoStr = response.Substring(8 + photoIndex, hashIndex - 3 - 8 - photoIndex);
		char[] dc2 = new char[]
		{
			'\\'
		};
		string[] strs2 = photoStr.Split(dc2);
		photoStr = string.Empty;
		for (int j = 0; j < strs2.Length; j++)
		{
			photoStr += strs2[j];
		}
		strs2 = photoStr.Split(dc);
		photoStr = string.Empty;
		for (int k = 0; k < strs2.Length; k++)
		{
			if (k != 0)
			{
				photoStr += "\"";
			}
			photoStr += strs2[k];
		}
		char[] dc3 = new char[]
		{
			'"',
			':',
			','
		};
		string[] strs3 = response.Split(dc3);
		string serverStr = string.Empty;
		string hashStr = string.Empty;
		for (int l = 0; l < strs3.Length; l++)
		{
			if (strs3[l] == "server")
			{
				serverStr = strs3[l + 2];
			}
			if (strs3[l] == "hash")
			{
				hashStr = strs3[l + 3];
			}
		}
		string request = string.Concat(new string[]
		{
			"https://api.vk.com/method/photos.saveWallPhoto?uid=",
			this.playerUID,
			"&gid=",
			this.playerUID,
			"&photo=",
			photoStr,
			"&server=",
			serverStr,
			"&hash=",
			hashStr,
			"&access_token=",
			this.access_token
		});
		UnityEngine.Debug.Log(request);
		WWW saveWWW = new WWW(request);
		yield return saveWWW;
		UnityEngine.Debug.Log(saveWWW.text);
		string photoAddr = string.Empty;
		string[] saveStrs = saveWWW.text.Split(dc3);
		for (int m = 0; m < strs3.Length; m++)
		{
			if (saveStrs[m] == "id")
			{
				photoAddr = saveStrs[m + 3];
				break;
			}
		}
		UnityEngine.Debug.Log(photoAddr);
		Application.ExternalCall("PostOnWallLevelUp", new object[]
		{
			this.playerUID,
			photoAddr
		});
		Screen.fullScreen = false;
		yield break;
	}

	public override void PostItemOnWall(int itemNum)
	{
		if (itemNum < this.viralConfig.item.Length)
		{
			Screen.fullScreen = false;
			if (!string.IsNullOrEmpty(this.viralConfig.item[itemNum]))
			{
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					this.viralConfig.item[itemNum]
				});
			}
		}
	}

	public override void PostBonusOnWall()
	{
		Application.ExternalCall("PostOnWallLevelUp", new object[]
		{
			this.playerUID,
			"photo-52386046_372794088"
		});
	}

	public override void PostMissionOnWall(int missionNum)
	{
		Application.ExternalCall("PostOnWallLevelUp", new object[]
		{
			this.playerUID,
			"photo-52386046_372794088"
		});
	}

	public override void PostWeaponOnWall(int weaponNum)
	{
		if (weaponNum < this.viralConfig.weapon.Length)
		{
			Screen.fullScreen = false;
			if (!string.IsNullOrEmpty(this.viralConfig.weapon[weaponNum]))
			{
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					this.viralConfig.weapon[weaponNum]
				});
			}
		}
	}

	public override void PostLevelUpOnWall(int levelNum)
	{
		if (levelNum < this.viralConfig.rank.Length)
		{
			Screen.fullScreen = false;
			if (!string.IsNullOrEmpty(this.viralConfig.rank[levelNum]))
			{
				Application.ExternalCall("PostOnWallLevelUp", new object[]
				{
					this.playerUID,
					this.viralConfig.rank[levelNum]
				});
			}
		}
	}

	public override void PostViralOnWall(int missionNum)
	{
		Application.ExternalCall("PostOnWallLevelUp", new object[]
		{
			this.playerUID,
			"photo-92406730_379329227"
		});
	}

	public override bool checkGroupLink(string home)
	{
		return home.StartsWith("http://vk.com/") || home.StartsWith("https://vk.com/");
	}

	public override void BuyPack(PackInfo info)
	{
		int id = info.id;
		Screen.fullScreen = false;
		Application.ExternalEval("var params = {type: \"item\",item: \"item" + id.ToString() + "\"};\nVK.callMethod(\"showOrderBox\", params);");
	}

	private const int FRIENDSMAX = 100;

	public string api_url;

	public int api_id;

	public bool initialized;

	private GameObject goToMessage;

	private string messageFunc;

	public Texture[] moneyIcon;

	private string access_token = "0d1c0f55ad77cc88b7f985738ee0ba475ff9594e47b5998167cfdc38c5ee7b3716d63f8abe9341fdf313c";

	protected SNViral viralConfig;

	private SnMissionDesc[] _missionNames;
}
