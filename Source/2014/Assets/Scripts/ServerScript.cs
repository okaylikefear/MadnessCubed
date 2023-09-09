using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class ServerScript : MonoBehaviour
{
	public void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.phpSecret = AuxFunc.GetMD5("privetvsemhakeram!!pliznapishitekakvzlomali_altodor@rambler.ru");
		this.initialized = true;
		Kube.SS = this;
	}

	private void Awake()
	{
		Kube.SS = this;
	}

	private void OnApplicationQuit()
	{
		this.ReleaseAssets();
	}

	public void require(string name)
	{
		for (int i = 0; i < this.asyncDownloadInfo.Length; i++)
		{
			if (this.asyncDownloadInfo[i].name == name || this.asyncDownloadInfo[i].name.StartsWith(name))
			{
				if (this._pending.IndexOf(this.asyncDownloadInfo[i]) == -1)
				{
					this._pending.Add(this.asyncDownloadInfo[i]);
					base.StartCoroutine(this.DownloadAsset(this.asyncDownloadInfo[i], false));
				}
				return;
			}
		}
	}

	public void requireByTag(string tag)
	{
		for (int i = 0; i < this.asyncDownloadInfo.Length; i++)
		{
			if (this.asyncDownloadInfo[i].tag == tag)
			{
				if (this._pending.IndexOf(this.asyncDownloadInfo[i]) == -1)
				{
					this._pending.Add(this.asyncDownloadInfo[i]);
					base.StartCoroutine(this.DownloadAsset(this.asyncDownloadInfo[i], false));
				}
				return;
			}
		}
	}

	public bool downloadReady
	{
		get
		{
			return this._downloadReady;
		}
	}

	public GameObject FindItemAsset(int index)
	{
		return this.FindAsset("ItemGO", index);
	}

	public GameObject FindAsset(string prefix, int index)
	{
		string text = prefix + index;
		GameObject gameObject = null;
		foreach (ServerScript.DownloadInfo[] array2 in new object[]
		{
			this.downloadInfo,
			this.asyncDownloadInfo
		})
		{
			for (int j = 0; j < array2.Length; j++)
			{
				string name = array2[j].name;
				if (array2[j].ready && array2[j].isPackage)
				{
					if (!this.debugDownloadWWW && Application.isEditor)
					{
						gameObject = (GameObject)Resources.LoadAssetAtPath(string.Concat(new string[]
						{
							"Assets/",
							name,
							"/",
							text,
							".prefab"
						}), typeof(GameObject));
					}
					else
					{
						gameObject = (GameObject)this.downloadInfo[j].ab.Load(text, typeof(GameObject));
					}
					if (gameObject != null)
					{
						break;
					}
				}
			}
		}
		return gameObject;
	}

	public UnityEngine.Object loadResource(string path, Type type)
	{
		int num = path.IndexOf("/");
		string text = path;
		if (num != -1)
		{
			text = path.Substring(0, num);
			path = path.Substring(num + 1);
		}
		UnityEngine.Object result = null;
		for (int i = 0; i < this.downloadInfo.Length; i++)
		{
			if (this.downloadInfo[i].name.Contains(text))
			{
				if (this.downloadInfo[i].ready && this.downloadInfo[i].isPackage)
				{
					if (!this.debugDownloadWWW && Application.isEditor)
					{
						result = Resources.LoadAssetAtPath(string.Concat(new string[]
						{
							"Assets/",
							text,
							"/",
							path,
							".prefab"
						}), type);
					}
					else
					{
						result = this.downloadInfo[i].ab.Load(path, type);
					}
					break;
				}
			}
		}
		return result;
	}

	private void Start()
	{
		this.Init();
		base.InvokeRepeating("ImHere", 10f, 300f);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		Kube.SS = null;
		this.ReleaseAssets();
	}

	private IEnumerator DownloadOH()
	{
		for (int i = 0; i < this.downloadInfo.Length; i++)
		{
			if (!(this.downloadInfo[i].ab != null))
			{
				if (this.downloadInfo[i].www == null)
				{
					yield return base.StartCoroutine(this.DownloadAsset(this.downloadInfo[i], true));
				}
			}
		}
		yield return new WaitForSeconds(2f);
		this.OHReady();
		this.isDownload = false;
		this._downloadReady = true;
		yield break;
	}

	private IEnumerator DownloadAsset(ServerScript.DownloadInfo downloadInfo, bool showProgress = false)
	{
		if (GameObject.Find(downloadInfo.name))
		{
			UnityEngine.Debug.Log("skip " + downloadInfo.name);
			yield break;
		}
		UnityEngine.Debug.Log("load " + downloadInfo.name + " from " + downloadInfo.path);
		if (!this.debugDownloadWWW && Application.isEditor)
		{
			if (downloadInfo.isPackage)
			{
				UnityEngine.Debug.Log("package: " + downloadInfo.name);
				downloadInfo.ready = true;
				yield break;
			}
			GameObject pf = (GameObject)Resources.LoadAssetAtPath("Assets/Prefabs/" + downloadInfo.name + ".prefab", typeof(GameObject));
			yield return 0;
			if (pf != null)
			{
				GameObject obj = (GameObject)UnityEngine.Object.Instantiate(pf);
				UnityEngine.Object.DontDestroyOnLoad(obj);
			}
			yield return 0;
		}
		else
		{
			int rev = this.assetRevision;
			if (downloadInfo.assetRevision > this.assetRevision)
			{
				rev = downloadInfo.assetRevision;
			}
			string url = string.Concat(new string[]
			{
				this.assetPath,
				"v",
				rev.ToString(),
				"/",
				downloadInfo.path
			});
			WWW www = WWW.LoadFromCacheOrDownload(url, rev);
			downloadInfo.www = www;
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				UnityEngine.Debug.LogError("error " + www.error + " " + www.url);
				yield break;
			}
			yield return 0;
			GameObject obj2 = null;
			if (www.assetBundle.mainAsset != null)
			{
				obj2 = (GameObject)UnityEngine.Object.Instantiate(www.assetBundle.mainAsset);
			}
			UnityEngine.Object.DontDestroyOnLoad(obj2);
			yield return 0;
			downloadInfo.www = null;
			downloadInfo.ab = www.assetBundle;
		}
		UnityEngine.Debug.Log("end load " + downloadInfo.name + " from " + downloadInfo.path);
		downloadInfo.ready = true;
		yield return new WaitForSeconds(0.2f);
		Kube.SendMonoMessage("onAssetsLoaded", new object[]
		{
			0
		});
		yield break;
	}

	private void OHReady()
	{
		this.isDownloadReady = true;
	}

	private void Update()
	{
	}

	public void SaveMap(long mapId, byte[] mapData, GameObject go = null, string method = "")
	{
		if (this.waitingForAnswer)
		{
			return;
		}
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._SaveMap(mapId, mapData, go, method));
		}
	}

	private IEnumerator _SaveMap(long mapId, byte[] mapData, GameObject go, string method)
	{
		this.Init();
		this.savingMap = (this.waitingForAnswer = true);
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		if (Kube.SS.secret_token != string.Empty)
		{
			str = "token=" + Kube.SS.secret_token;
			requestSig += str;
			requestStr = requestStr + "&" + str;
		}
		str = "requestCode=3";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "mapid=" + mapId;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += AuxFunc.GetMD5(mapData);
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		Kube.GPS.printLog(requestStr);
		WWW newWWW = new WWW(requestStr, mapData);
		yield return newWWW;
		this.savingMap = (this.waitingForAnswer = false);
		UnityEngine.Debug.Log(newWWW.text);
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			UnityEngine.Debug.LogError(newWWW.error);
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		if (go != null)
		{
			go.SendMessage(method, newWWW.text);
		}
		yield break;
	}

	public void LoadMap(long mapId)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			this.loadingMap = true;
			base.StartCoroutine(this._LoadMap(mapId));
		}
	}

	private IEnumerator _LoadMap(long mapId)
	{
		this.Init();
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=4";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "mapid=" + mapId;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		MonoBehaviour.print(requestStr);
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		if (Kube.WHS != null)
		{
			Kube.WHS.LoadWorld(newWWW.bytes);
		}
		this.loadingMap = false;
		this.waitingForAnswer = false;
		Kube.GPS.printLog("Map size=" + newWWW.text.Length);
		yield break;
	}

	public void downloadMap(long id)
	{
		this.loadingMap = true;
		base.StartCoroutine(this._downloadMap(-id));
	}

	private IEnumerator _downloadMap(long id)
	{
		int mapid = (int)id;
		bool loadFromAsset = false;
		if (Kube.ASS3 && mapid < 100)
		{
			yield return new WaitForSeconds(0.2f);
			if (Kube.WHS != null)
			{
				if (mapid < 20)
				{
					if (mapid < Kube.ASS3.buildinMaps.Length)
					{
						Kube.WHS.LoadWorld(Kube.ASS3.buildinMaps[mapid].bytes);
						loadFromAsset = true;
					}
				}
				else
				{
					mapid -= 20;
					if (mapid < Kube.ASS3.buildinMapsTeams.Length)
					{
						Kube.WHS.LoadWorld(Kube.ASS3.buildinMapsTeams[mapid].bytes);
						loadFromAsset = true;
					}
				}
				if (loadFromAsset)
				{
					this.loadingMap = false;
					yield break;
				}
			}
		}
		WWW newWWW = new WWW(this.assetPath + "m" + id.ToString() + ".bytes");
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		if (Kube.WHS != null)
		{
			Kube.WHS.LoadWorld(newWWW.bytes);
		}
		this.loadingMap = false;
		yield break;
	}

	private void onLogin(string response)
	{
		JsonData jsonData = JsonMapper.ToObject(response);
		if (!(bool)jsonData["ok"])
		{
			UnityEngine.Debug.Log("Bad login!");
			return;
		}
	}

	public void DownloadGameData()
	{
		if (this.isDownload)
		{
			return;
		}
		if (this.isDownloadReady)
		{
			return;
		}
		this.isDownload = true;
		base.StartCoroutine(this.DownloadOH());
	}

	public void LoadPlayersParams(GameObject go, string funcName)
	{
		this.Init();
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._LoadPlayersParams(go, funcName));
		}
	}

	private IEnumerator _LoadPlayersParams(GameObject go, string funcName)
	{
		this.Init();
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "uid=" + Kube.SN.playerUID;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "sex=" + Kube.SN.sex.ToString();
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "age=" + Kube.SN.age.ToString();
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "requestCode=1";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "session_key=" + Kube.SN.sessionKey;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		if (Kube.SN.refSite != string.Empty)
		{
			str = "ref=" + Kube.SN.refSite;
			requestSig += str;
			requestStr = requestStr + "&" + str;
		}
		str = "secret=" + Kube.SN.secret;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		Kube.GPS.printLog(requestStr);
		MonoBehaviour.print(requestStr);
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			UnityEngine.Debug.Log(newWWW.error);
			go.SendMessage("Error", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		if (newWWW.text.StartsWith("error"))
		{
			UnityEngine.Debug.LogError(newWWW.text);
			go.SendMessage("Error", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		if (newWWW.text.StartsWith("ban"))
		{
			UnityEngine.Debug.LogError(newWWW.text);
			go.SendMessage("Ban", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		MonoBehaviour.print(newWWW.text);
		go.SendMessage(funcName, newWWW.bytes);
		yield break;
	}

	public void BuyCubes(int numCubes, int numDays, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._BuyCubes(numCubes, numDays, go, method));
		}
	}

	private IEnumerator _BuyCubes(int numCubes, int numDays, GameObject go, string method)
	{
		this.Init();
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=5";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "cubesnum=" + numCubes;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "cubestime=" + numDays;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void BuyItem(int numItem, int itemsCount, GameObject go, string method)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["itemnum"] = numItem.ToString();
		dictionary["itemscount"] = itemsCount.ToString();
		ServerScript.ServerCallback cb = delegate(string s)
		{
			go.SendMessage(method, s, SendMessageOptions.DontRequireReceiver);
		};
		this.Request(6, dictionary, cb);
	}

	private IEnumerator _BuyItem(int numItem, int itemsCount, GameObject go, string method)
	{
		this.Init();
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=6";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "itemnum=" + numItem;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "itemscount=" + itemsCount;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void BuyWeapon(int numWeapon, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._BuyWeapon(numWeapon, go, method));
		}
	}

	private IEnumerator _BuyWeapon(int numWeapon, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=7";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "weaponnum=" + numWeapon;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void BuyWeapon(int numWeapon, int tarif, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._BuyWeapon(numWeapon, tarif, go, method));
			this.SendStatIoTrack("WeaponMoney", Kube.GPS.weaponsPrice1[numWeapon, tarif]);
			this.SendStatIoTrack("MONEY-", Kube.GPS.weaponsPrice1[numWeapon, tarif]);
			this.SendStatIoTrack("WeaponGold", Kube.GPS.weaponsPrice2[numWeapon, tarif]);
			this.SendStatIoTrack("GOLD-", Kube.GPS.weaponsPrice2[numWeapon, tarif]);
		}
	}

	private IEnumerator _BuyWeapon(int numWeapon, int tarif, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=27";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "weaponnum=" + numWeapon;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "tarif=" + tarif;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void BuySpecItem(int numSpecItem, int tarif, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._BuySpecItem(numSpecItem, tarif, go, method));
		}
	}

	private IEnumerator _BuySpecItem(int numSpecItem, int tarif, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=28";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "specitemnum=" + numSpecItem;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "tarif=" + tarif;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void GetPlayerMoney(GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._GetPlayerMoney(go, method));
		}
	}

	private IEnumerator _GetPlayerMoney(GameObject go, string method)
	{
		Kube.GPS.printLog("ServerScript _GetPlayerMoney");
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=8";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void UpgradeParam(int numParam, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._UpgradeParam(numParam, go, method));
			int[] array = new int[]
			{
				Kube.GPS.playerHealth,
				Kube.GPS.playerArmor,
				Kube.GPS.playerSpeed,
				Kube.GPS.playerJump,
				Kube.GPS.playerDefend
			};
			int num = array[numParam];
			int num2 = Mathf.FloorToInt(Kube.GPS.charParamsPrice[numParam, num, 1]);
			bool flag;
			if (num2 == 0)
			{
				num2 = Mathf.FloorToInt(Kube.GPS.charParamsPrice[numParam, num, 2]);
				flag = true;
			}
			else
			{
				flag = false;
			}
			if (Kube.GPS.playerLevel < (int)Kube.GPS.charParamsPrice[numParam, num, 0])
			{
				num2 *= 2;
			}
			if (flag)
			{
				this.SendStatIoTrack("UpgradeParamGOLD", num2);
				this.SendStatIoTrack("GOLD-", num2);
			}
			else
			{
				this.SendStatIoTrack("UpgradeParamMONEY", num2);
				this.SendStatIoTrack("MONEY-", num2);
			}
		}
	}

	private IEnumerator _UpgradeParam(int numParam, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=9";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "paramnum=" + numParam;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void UpgradeParamUnlock(int numParam, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._UpgradeParamUnlock(numParam, go, method));
			int[] array = new int[]
			{
				Kube.GPS.playerHealth,
				Kube.GPS.playerArmor,
				Kube.GPS.playerSpeed,
				Kube.GPS.playerJump,
				Kube.GPS.playerDefend
			};
			int num = array[numParam];
			int num2 = Mathf.FloorToInt(Kube.GPS.charParamsPrice[numParam, num, 1]);
			bool flag;
			if (num2 == 0)
			{
				num2 = Mathf.FloorToInt(Kube.GPS.charParamsPrice[numParam, num, 2]);
				flag = true;
			}
			else
			{
				flag = false;
			}
			if (Kube.GPS.playerLevel < (int)Kube.GPS.charParamsPrice[numParam, num, 0])
			{
				num2 *= 2;
			}
			if (flag)
			{
				this.SendStatIoTrack("UpgradeParamUNLOCK_GOLD", num2);
				this.SendStatIoTrack("GOLD-", num2);
			}
			else
			{
				this.SendStatIoTrack("UpgradeParamUNLOCK_MONEY", num2);
				this.SendStatIoTrack("MONEY-", num2);
			}
		}
	}

	private IEnumerator _UpgradeParamUnlock(int numParam, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=34";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "paramnum=" + numParam;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void UpgradeParamAllUnlock(int needHealth, int needArmor, int needSpeed, int needJump, int needDefend, int upgradeMoney, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._UpgradeParamAllUnlock(needHealth, needArmor, needSpeed, needJump, needDefend, go, method));
			this.SendStatIoTrack("UpgradeParamUNLOCK_GOLD", upgradeMoney);
			this.SendStatIoTrack("GOLD-", upgradeMoney);
			this.SendStatIoTrack("WeaponsAllParams", upgradeMoney);
			this.SendStatIoTrack("WeaponsAllParams_N", 1);
		}
	}

	private IEnumerator _UpgradeParamAllUnlock(int needHealth, int needArmor, int needSpeed, int needJump, int needDefend, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=35";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "needhealth=" + needHealth;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "needarmor=" + needArmor;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "needspeed=" + needSpeed;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "needjump=" + needJump;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "needdefend=" + needDefend;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void BuySkin(int numSkin, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._BuySkin(numSkin, go, method));
		}
	}

	private IEnumerator _BuySkin(int numSkin, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=10";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "skinnum=" + numSkin;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void GoldToMoney(int numGold, GameObject go, string method)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["numgold"] = numGold.ToString();
		ServerScript.ServerCallback cb = delegate(string s)
		{
			go.SendMessage(method, s);
		};
		this.Request(11, dictionary, cb);
	}

	private IEnumerator _GoldToMoney(int numGold, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=11";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "numgold=" + numGold;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void SaveNewName(int id, string newName)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._SaveNewName(id, newName));
		}
	}

	private IEnumerator _SaveNewName(int id, string newName)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=12";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "newname=" + newName;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		this.waitingForAnswer = false;
		yield break;
	}

	public void BuyBullets(int typeBullets, int numTarif, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._BuyBullets(typeBullets, numTarif, go, method));
		}
	}

	private IEnumerator _BuyBullets(int typeBullets, int numTarif, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=14";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "typebullets=" + typeBullets;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "tarif=" + numTarif;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void SendEndLevel(EndGameStats endGameStats, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._SendEndLevel(endGameStats.playerExp, endGameStats.deltaExp, endGameStats.playerFrags, endGameStats.deltaFrags, endGameStats.playerMoney1, endGameStats.deltaMoney, endGameStats.playerLevel, endGameStats.newLevel, endGameStats.bonuses, go, method));
		}
	}

	private IEnumerator _SendEndLevel(int oldExp, int deltaExp, int oldFrags, int deltaFrags, int oldMoney, int deltaMoney, int oldLevel, int newLevel, int[] bonuses, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		MonoBehaviour.print(string.Concat(new object[]
		{
			"SendEndLevel:",
			oldExp + deltaExp,
			"(",
			deltaExp,
			") ",
			oldFrags + deltaFrags,
			" ",
			oldMoney + deltaMoney
		}));
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=15";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "oldexp=" + oldExp;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "deltaexp=" + deltaExp;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "oldfrags=" + oldFrags;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "deltafrags=" + deltaFrags;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "oldmoney=" + oldMoney;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "deltamoney=" + deltaMoney;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "oldlevel=" + oldLevel;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "newlevel=" + newLevel;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "bonuses=";
		for (int i = 0; i < bonuses.Length; i++)
		{
			if (i != 0)
			{
				str += ";";
			}
			str = str + string.Empty + bonuses[i];
		}
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "timestamp=" + this.UnixTime().ToString();
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		MonoBehaviour.print(requestSig);
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		MonoBehaviour.print("SendEndLevel Ans: " + newWWW.text);
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public int UnixTime()
	{
		return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
	}

	public void BuyNewMap(int maptype, ServerScript.ServerCallback cb)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["maptype"] = maptype.ToString();
			this.Request(616, dictionary, cb);
		}
	}

	public void UseItem(int numItem)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._UseItem(numItem));
		}
	}

	private IEnumerator _UseItem(int numItem)
	{
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=17";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "numitem=" + numItem;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		if (newWWW.text == "cannotUseItem")
		{
			UnityEngine.Debug.LogError("Error: cannotUseItem");
		}
		yield break;
	}

	public void TakeItem(int numItem, int itemCountNow, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._TakeItem(numItem, itemCountNow, go, method));
		}
	}

	private IEnumerator _TakeItem(int numItem, int itemCountNow, GameObject go, string method)
	{
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=18";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "numitem=" + numItem;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += itemCountNow;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		yield break;
	}

	private IEnumerator _Request(int q, Dictionary<string, string> paramData, ServerScript.ServerCallback cb)
	{
		this.waitingForAnswer = true;
		if (paramData == null)
		{
			paramData = new Dictionary<string, string>();
		}
		WWWForm form = new WWWForm();
		paramData["requestCode"] = q.ToString();
		if (Kube.SS.secret_token != string.Empty)
		{
			paramData["token"] = Kube.SS.secret_token;
		}
		if (!paramData.ContainsKey("id") && Kube.GPS.playerId != 0)
		{
			paramData["id"] = Kube.GPS.playerId.ToString();
		}
		List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>(paramData);
		myList.Sort((KeyValuePair<string, string> keyfirst, KeyValuePair<string, string> keylast) => keyfirst.Key.CompareTo(keylast.Key));
		StringBuilder md5sig = new StringBuilder();
		foreach (KeyValuePair<string, string> rec in myList)
		{
			md5sig.Append(rec.Key + "=" + rec.Value);
			form.AddField(rec.Key, rec.Value);
		}
		md5sig.Append(this.phpSecret);
		form.AddField("sig", AuxFunc.GetMD5(md5sig.ToString()));
		WWW newWWW = new WWW(this.phpServer + this.mainPhpScript, form);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		if (cb != null)
		{
			cb(newWWW.text);
		}
		this.waitingForAnswer = false;
		yield break;
	}

	public void Request(int q, object param, ServerScript.ServerCallback cb)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["object"] = param.ToString();
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._Request(q, dictionary, cb));
		}
	}

	public void Request(int q, Dictionary<string, string> paramData, ServerScript.ServerCallback cb)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._Request(q, paramData, cb));
		}
	}

	public void LoadIsMap(long mapId, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._LoadIsMap(mapId, go, method));
		}
	}

	private IEnumerator _LoadIsMap(long mapId, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=19";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "mapid=" + mapId;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		MonoBehaviour.print(requestStr);
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		MonoBehaviour.print(newWWW.text);
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void SetMapName(long mapId, string mapName)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._SetMapName(mapId, mapName));
		}
	}

	private IEnumerator _SetMapName(long mapId, string mapName)
	{
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=20";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "mapid=" + mapId;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "mapname=" + mapName;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		if (Kube.SS.secret_token != string.Empty)
		{
			str = "token=" + Kube.SS.secret_token;
			requestSig += str;
			requestStr = requestStr + "&" + str;
		}
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		yield break;
	}

	public void SendStat(string statName)
	{
		if (this.sendStat && this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._SendStat(statName));
		}
	}

	private IEnumerator _SendStat(string statName)
	{
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=21";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "statname=" + statName;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "payer=" + ((!this.payer) ? 0 : 1);
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "justpaid=" + ((!this.justPaid) ? 0 : 1);
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		yield break;
	}

	public void SendStatCount(string statName, int count)
	{
		if (this.sendStat && this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._SendStatCount(statName, count));
		}
	}

	private IEnumerator _SendStatCount(string statName, int count)
	{
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=22";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "statname=" + statName;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "statcount=" + count;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "payer=" + ((!this.payer) ? 0 : 1);
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "justpaid=" + ((!this.justPaid) ? 0 : 1);
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		yield break;
	}

	public void BuyVIP(int numVIP, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._BuyVIP(numVIP, go, method));
			this.SendStatIoTrack("VIP_GOLD", Kube.GPS.vipPrice[numVIP, 1]);
			this.SendStatIoTrack("GOLD-", Kube.GPS.vipPrice[numVIP, 1]);
		}
	}

	private IEnumerator _BuyVIP(int numVIP, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=23";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "vipnum=" + numVIP;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void RegenerateMap(int maptype, long numMap, ServerScript.ServerCallback cb)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["nummap"] = numMap.ToString();
			dictionary["maptype"] = maptype.ToString();
			if (Kube.SS.secret_token != string.Empty)
			{
				dictionary["token"] = Kube.SS.secret_token;
			}
			this.Request(624, dictionary, cb);
		}
	}

	public void SetSkin(int numSkin)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._SetSkin(numSkin));
		}
	}

	private IEnumerator _SetSkin(int numSkin)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=26";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "skinnum=" + numSkin;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		Kube.GPS.playerSkin = numSkin;
		Kube.SendMonoMessage("UpdateChar", new object[0]);
		this.waitingForAnswer = false;
		yield break;
	}

	public void SetClothes(string clothes)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._SetClothes(clothes));
		}
	}

	private IEnumerator _SetClothes(string clothes)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=31";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "clothes=" + clothes;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		MonoBehaviour.print(newWWW.text);
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		string[] cls = clothes.Split(new char[]
		{
			';'
		});
		for (int i = 0; i < Kube.GPS.playerClothes.Length; i++)
		{
			Kube.GPS.playerClothes[i] = int.Parse(cls[i]);
		}
		Kube.SendMonoMessage("UpdateChar", new object[0]);
		this.waitingForAnswer = false;
		yield break;
	}

	public void SendContest(string ids, string moneys, string golds, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._SendContest(ids, moneys, golds, go, method));
		}
	}

	private IEnumerator _SendContest(string ids, string moneys, string golds, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=29";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "ids=" + ids;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "moneys=" + moneys;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "golds=" + golds;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		UnityEngine.Debug.Log(requestStr);
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void BuyClothes(int numClothes, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._BuyClothes(numClothes, go, method));
			this.SendStatIoTrack("ClothesMoney", Kube.GPS.clothesPrice[numClothes, 1]);
			this.SendStatIoTrack("MONEY-", Kube.GPS.clothesPrice[numClothes, 1]);
			this.SendStatIoTrack("ClothesGold", Kube.GPS.clothesPrice[numClothes, 2]);
			this.SendStatIoTrack("GOLD-", Kube.GPS.clothesPrice[numClothes, 2]);
		}
	}

	private void SaveFastInventoryBC(string res)
	{
		UnityEngine.Debug.Log(res);
	}

	public void SaveFastInventory(int type, FastInventar[] inventory, ServerScript.ServerCallback cb)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["type"] = type.ToString();
		List<byte> list = new List<byte>();
		for (int i = 0; i < inventory.Length; i++)
		{
			byte item = (byte)inventory[i].Type;
			list.Add(item);
			list.Add(Convert.ToByte(inventory[i].Num));
		}
		dictionary["data"] = Convert.ToBase64String(list.ToArray());
		dictionary["id"] = Kube.GPS.playerId.ToString();
		this.Request(668, dictionary, new ServerScript.ServerCallback(this.SaveFastInventoryBC));
	}

	private IEnumerator _BuyClothes(int numClothes, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=30";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "clothesnum=" + numClothes;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void ImHere()
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._ImHere());
		}
	}

	private IEnumerator _ImHere()
	{
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=32";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		yield break;
	}

	public void LoadSpecBonusTex(int numTex, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._LoadSpecBonusTex(numTex, go, method));
		}
	}

	private IEnumerator _LoadSpecBonusTex(int numTex, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		WWW newWWW = new WWW(string.Concat(new object[]
		{
			this.assetPath,
			"/bonusTex/",
			numTex,
			".jpg"
		}));
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		go.SendMessage(method, newWWW.texture);
		this.waitingForAnswer = false;
		yield break;
	}

	public void LoadStatistics(int dayFrom, int dayTo, GameObject go, string method)
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			base.StartCoroutine(this._LoadStatistics(dayFrom, dayTo, go, method));
		}
	}

	private IEnumerator _LoadStatistics(int dayFrom, int dayTo, GameObject go, string method)
	{
		MonoBehaviour.print(string.Concat(new object[]
		{
			string.Empty,
			dayFrom,
			" - ",
			dayTo
		}));
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.GPS.playerId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=33";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "dayfrom=" + dayFrom;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "dayto=" + dayTo;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this.phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		MonoBehaviour.print(requestStr);
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		MonoBehaviour.print(newWWW.text);
		go.SendMessage(method, newWWW.text);
		this.waitingForAnswer = false;
		yield break;
	}

	public void UpgradeWeapon(int bt, int q, ServerScript.JSONServerCallback upgradeWeaponDone)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["weapon"] = bt.ToString();
		dictionary["q"] = q.ToString();
		int num = 0;
		if (q == 0)
		{
			num = Kube.IS.weaponParams[bt].currentDamageIndex;
		}
		else if (q == 1)
		{
			num = Kube.IS.weaponParams[bt].currentAccuracyIndex;
		}
		else if (q == 2)
		{
			num = Kube.IS.weaponParams[bt].currentDeltaShotIndex;
		}
		else if (q == 3)
		{
			num = Kube.IS.weaponParams[bt].currentClipSizeIndex;
		}
		if (Kube.GPS.upgradePrice[bt, q, num].isGold)
		{
			this.SendStatIoTrack("UpgradeWeaponGold", Kube.GPS.upgradePrice[bt, q, num].price);
			this.SendStatIoTrack("GOLD-", Kube.GPS.upgradePrice[bt, q, num].price);
		}
		else
		{
			this.SendStatIoTrack("UpgradeWeaponMoney", Kube.GPS.upgradePrice[bt, q, num].price);
			this.SendStatIoTrack("MONEY-", Kube.GPS.upgradePrice[bt, q, num].price);
		}
		this.Request(700, dictionary, delegate(string ans)
		{
			JsonData jsonData = JsonMapper.ToObject(ans);
			if (jsonData.Keys.Contains("error"))
			{
				return;
			}
			Kube.GPS.playerMoney1 = (int)jsonData["money"][0];
			Kube.GPS.playerMoney2 = (int)jsonData["money"][1];
			WeaponUpgrade.Parse(jsonData["wp"]);
			upgradeWeaponDone(jsonData["wp"]);
		});
	}

	public int GetUTC()
	{
		if (this.workType == ServerScript.ServerWorkType.netServer)
		{
			return (int)(this.serverTime + (Time.realtimeSinceStartup - this.serverLocalTime));
		}
		return 0;
	}

	public void DrawLoading()
	{
		if (!this.isDownload)
		{
			return;
		}
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		int num3 = this.downloadInfo.Length;
		float num4 = 0f;
		for (int i = 0; i < this.downloadInfo.Length; i++)
		{
			if (this.downloadInfo[i].ready)
			{
				num4 += 1f;
			}
			else if (this.downloadInfo[i].www != null)
			{
				num4 += this.downloadInfo[i].www.progress;
			}
		}
		num4 /= (float)num3;
		float num5 = Mathf.Floor(num4 * 100f);
		GUI.Label(new Rect(0.5f * num - 150f, num2 - 100f, 300f, 60f), string.Concat(new object[]
		{
			Localize.ss_loading,
			" ",
			num5,
			"%"
		}));
		GUI.DrawTexture(new Rect(0.5f * (num - 318f), num2 - 100f, 318f, 25f), this.pb_bgTex);
		GUI.DrawTextureWithTexCoords(new Rect(0.5f * (num - 318f), num2 - 100f, num4 * 318f, 25f), this.pb_fillTex, new Rect(0f, 0f, num4, 1f));
		GUI.DrawTexture(new Rect(0.5f * (num - 318f), num2 - 100f, 318f, 25f), this.pb_borderTex);
	}

	private void OnGUI()
	{
		GUI.depth = -10;
		if (this.waitingForAnswer)
		{
			int num = (int)(Time.time * 10f) % this.waitingTex.Length;
			GUI.DrawTexture(new Rect((float)Screen.width - 64f, 0f, 64f, 64f), this.waitingTex[num]);
		}
	}

	public void ReleaseAssets()
	{
		Kube.ASS1 = null;
		Kube.ASS2 = null;
		Kube.ASS3 = null;
		Kube.ASS4 = null;
		Kube.ASS5 = null;
		for (int i = 0; i < this.asyncDownloadInfo.Length; i++)
		{
			if (this.asyncDownloadInfo[i].ab != null)
			{
				this.asyncDownloadInfo[i].ab.Unload(false);
			}
		}
		for (int j = 0; j < this.downloadInfo.Length; j++)
		{
			if (this.downloadInfo[j].ab != null)
			{
				this.downloadInfo[j].ab.Unload(false);
			}
		}
	}

	public void SendStatIoTrack(string statName, int inc = 1)
	{
		string text = string.Concat(new object[]
		{
			"http://t.onthe.io/t?k=205:",
			statName,
			"&s=",
			AuxFunc.GetMD5(statName + "Nql9x4AEhGcTPWJ7tfVg-TyPN6CR1gn9"),
			"&v=",
			inc
		});
		Application.ExternalCall("sendStats", new object[]
		{
			text
		});
	}

	public string secret_token;

	public bool debugDownloadWWW;

	public ServerScript.ServerWorkType workType;

	public string phpServer;

	public string assetPath;

	[HideInInspector]
	public string phpSecret = string.Empty;

	public bool loadingMap;

	public bool waitingForAnswer;

	public bool savingMap;

	public Texture[] waitingTex;

	public float serverTime;

	public float serverLocalTime;

	public bool sendStat;

	public bool sendStatPay;

	public bool payer;

	public bool justPaid;

	public string mainPhpScript = "mainScript.php";

	public int assetRevision;

	public Texture pb_bgTex;

	public Texture pb_fillTex;

	public Texture pb_borderTex;

	private bool initialized;

	public ServerScript.DownloadInfo[] downloadInfo;

	public ServerScript.DownloadInfo[] asyncDownloadInfo;

	private WWW[] _www;

	private List<ServerScript.DownloadInfo> _pending = new List<ServerScript.DownloadInfo>();

	private bool _downloadReady;

	private bool isDownloadReady;

	private bool isDownload;

	private BattleControllerScript BCS;

	private NetworkObjectScript NO;

	public enum ServerWorkType
	{
		netServer,
		playerPrefs
	}

	[Serializable]
	public class DownloadInfo
	{
		public string name;

		public string path;

		public int assetRevision;

		public string tag;

		public bool isPackage;

		[HideInInspector]
		public AssetBundle ab;

		[HideInInspector]
		public WWW www;

		[HideInInspector]
		public bool ready;
	}

	public delegate void ServerCallback(string text);

	public delegate void JSONServerCallback(JsonData json);
}
