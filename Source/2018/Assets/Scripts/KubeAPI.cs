using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using kube;
using kube.data;
using kube.ui;
using LitJson;
using UnityEngine;

public class KubeAPI : MonoBehaviour
{
	public int serverId
	{
		get
		{
			return this._serverId;
		}
	}

	public bool savingMap
	{
		get
		{
			return this._savingMap;
		}
	}

	public bool loadingMap
	{
		get
		{
			return this._loadingMap;
		}
	}

	public string phpSecret
	{
		get
		{
			return this._phpSecret;
		}
	}

	public float serverTime
	{
		get
		{
			return this._serverTime;
		}
		set
		{
			this._serverTime = value;
		}
	}

	private bool payer
	{
		get
		{
			return Kube.GPS.playerVoices > 0;
		}
	}

	public void Init(string phpServer, string mainPhpScript)
	{
		if (this.initialized)
		{
			return;
		}
		this._phpSecret = AuxFunc.GetMD5("privetvsemhakeram!!pliznapishitekakvzlomali_altodor@rambler.ru");
		this.phpServer = phpServer;
		this.mainPhpScript = mainPhpScript;
		base.InvokeRepeating("ImHere", 10f, 300f);
		base.StartCoroutine(this.ExecuteSheduled());
		this.initialized = true;
	}

	private void Awake()
	{
	}

	private void OnApplicationQuit()
	{
	}

	public IEnumerator ExecuteSheduled()
	{
		for (;;)
		{
			if (this._sheduled.Count > 0)
			{
				IEnumerator current = this._sheduled[0];
				this._sheduled.RemoveAt(0);
				yield return base.StartCoroutine(current);
			}
			else
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
		yield break;
	}

	protected void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		Kube.SS = null;
	}

	private void Update()
	{
	}

	private void SheduleStartCoroutine(IEnumerator cr)
	{
		this._sheduled.Add(cr);
	}

	public void SaveMap(long mapId, byte[] mapData, GameObject go = null, string method = "")
	{
		if (this.waitingForAnswer)
		{
			return;
		}
		base.StartCoroutine(this._SaveMap(mapId, mapData, go, method));
	}

	private IEnumerator _SaveMap(long mapId, byte[] mapData, GameObject go, string method)
	{
		this._savingMap = (this.waitingForAnswer = true);
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		if (this.secret_token != string.Empty)
		{
			str = "token=" + this.secret_token;
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
		requestSig += this._phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		Kube.GPS.printLog(requestStr);
		WWW newWWW = new WWW(requestStr, mapData);
		yield return newWWW;
		this._savingMap = (this.waitingForAnswer = false);
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
		this._loadingMap = true;
		base.StartCoroutine(this._LoadMap(mapId));
	}

	private IEnumerator _LoadMap(long mapId)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=4";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "mapid=" + mapId;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this._phpSecret;
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
			byte[] newWorldData = newWWW.bytes;
			if (newWorldData.Length == 2)
			{
				int NewMapType = (int)newWorldData[1] << 8 | (int)newWorldData[0];
				if (NewMapType < 20)
				{
					ObjectsHolderScript.BuiltInMap[] bmi = Kube.OH.findMaps(GameType.creating);
					NewMapType = bmi[NewMapType].Id;
				}
				UnityEngine.Debug.Log("Redirect to builtin default map: " + NewMapType);
				yield return base.StartCoroutine(Kube.RM._downloadMap((long)NewMapType));
				this.waitingForAnswer = false;
				yield break;
			}
			Kube.BCS.OnMapLoaded(newWorldData);
		}
		this._loadingMap = false;
		this.waitingForAnswer = false;
		Kube.GPS.printLog("Map size=" + newWWW.text.Length);
		yield break;
	}

	public void LoadPlayersParams(YieldCallback cb)
	{
		base.StartCoroutine(this._LoadPlayersParams(cb));
	}

	protected IEnumerator _LoadPlayersParams(YieldCallback cb)
	{
		this.waitingForAnswer = true;
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
		str = "v=" + "68".ToString();
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "sn=" + Kube.SN.sn;
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
		requestSig += this._phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		Kube.GPS.printLog(requestStr);
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			UnityEngine.Debug.LogError(newWWW.error);
			JsonData err = new JsonData("error");
			yield return base.StartCoroutine(cb("error"));
			yield break;
		}
		if (newWWW.text.StartsWith("error"))
		{
			UnityEngine.Debug.LogError(newWWW.text);
			yield return base.StartCoroutine(cb("error"));
			yield break;
		}
		if (newWWW.text.StartsWith("ban"))
		{
			UnityEngine.Debug.LogError(newWWW.text);
			JsonData err2 = new JsonData("ban");
			yield return base.StartCoroutine(cb("error"));
			yield break;
		}
		byte[] bytes = newWWW.bytes;
		int len = BitConverter.ToInt32(bytes, 0);
		string jsonData = Encoding.UTF8.GetString(bytes, 4, len);
		string sig = Encoding.UTF8.GetString(bytes, 4 + len, 32);
		if (AuxFunc.GetMD5(jsonData + this.phpSecret) != sig)
		{
			UnityEngine.Debug.LogError("Bad Sig");
			yield return base.StartCoroutine(cb("error"));
			yield break;
		}
		JsonData data = JsonMapper.ToObject(jsonData);
		if (data.Keys.Contains("token"))
		{
			this.secret_token = data["token"].ToString();
		}
		this._serverId = TryConvert.ToInt32(data["id"].ToString(), 0);
		if (cb != null)
		{
			yield return base.StartCoroutine(cb(data));
		}
		this.waitingForAnswer = false;
		yield break;
	}

	public string[] DecodePlayerData(JsonData playerData)
	{
		string[] array = new string[playerData.Count + 2];
		for (int i = 0; i < playerData.Count; i++)
		{
			array[i + 2] = playerData[i].ToString();
		}
		array[3] = Encoding.ASCII.GetString(Convert.FromBase64String(array[3]));
		return array;
	}

	public void BuyCubes(int numCubes, int numDays, GameObject go, string method)
	{
		this.SheduleStartCoroutine(this._BuyCubes(numCubes, numDays, go, method));
	}

	private void ValidateAndCall(string text, GameObject go, string method)
	{
		string[] array = text.Split(KubeAPI.dc);
		string text2 = string.Empty + Kube.SS.serverId;
		for (int i = 2; i < array.Length - 2; i++)
		{
			text2 += array[i];
		}
		text2 = text2 + array[array.Length - 1] + string.Empty + Kube.SS.phpSecret;
		if (AuxFunc.GetMD5(text2) == array[array.Length - 2])
		{
			go.SendMessage(method, array);
		}
	}

	private void _ValidateAndCall(string text, GameObject go, string method)
	{
		string[] array = text.Split(KubeAPI.dc);
		string text2 = string.Empty;
		for (int i = 2; i < array.Length - 2; i++)
		{
			text2 += array[i];
		}
		text2 += array[array.Length - 1];
		if (AuxFunc.GetMD5(text2) == array[array.Length - 2])
		{
			go.SendMessage(method, array);
		}
	}

	private IEnumerator _BuyCubes(int numCubes, int numDays, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
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
		requestSig += this._phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		this.ValidateAndCall(newWWW.text, go, method);
		this.waitingForAnswer = false;
		yield break;
	}

	public void BuyItem(int numItem, int itemsCount, GameObject go, string method)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["itemnum"] = numItem.ToString();
		dictionary["itemscount"] = itemsCount.ToString();
		ServerCallback cb = delegate(string s)
		{
			this.ValidateAndCall(s, go, method);
		};
		this.Request(6, dictionary, cb);
	}

	public void BuyWeapon(int numWeapon, int tarif, GameObject go, string method)
	{
		this.SheduleStartCoroutine(this._BuyWeapon(numWeapon, tarif, go, method));
		this.SendStatIoTrack("WeaponMoney", Kube.GPS.weaponsPrice1[numWeapon, tarif]);
		this.SendStatIoTrack("MONEY-", Kube.GPS.weaponsPrice1[numWeapon, tarif]);
		this.SendStatIoTrack("WeaponGold", Kube.GPS.weaponsPrice2[numWeapon, tarif]);
		this.SendStatIoTrack("GOLD-", Kube.GPS.weaponsPrice2[numWeapon, tarif]);
	}

	private IEnumerator _BuyWeapon(int numWeapon, int tarif, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
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
		requestSig += this._phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		this.ValidateAndCall(newWWW.text, go, method);
		this.waitingForAnswer = false;
		yield break;
	}

	public void BuySpecItem(int numSpecItem, int tarif, GameObject go, string method)
	{
		this.SheduleStartCoroutine(this._BuySpecItem(numSpecItem, tarif, go, method));
	}

	private IEnumerator _BuySpecItem(int numSpecItem, int tarif, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
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
		requestSig += this._phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		this.ValidateAndCall(newWWW.text, go, method);
		this.waitingForAnswer = false;
		yield break;
	}

	public void GetPlayerMoney(GameObject go, string method)
	{
		base.StartCoroutine(this._GetPlayerMoney(go, method));
	}

	private IEnumerator _GetPlayerMoney(GameObject go, string method)
	{
		Kube.GPS.printLog("ServerScript _GetPlayerMoney");
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=8";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this._phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		this._ValidateAndCall(newWWW.text, go, method);
		this.waitingForAnswer = false;
		yield break;
	}

	public void UpgradeParam(int numParam, GameObject go, string method)
	{
		this.SheduleStartCoroutine(this._UpgradeParam(numParam, go, method));
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

	private IEnumerator _UpgradeParam(int numParam, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=9";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "paramnum=" + numParam;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this._phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		this.ValidateAndCall(newWWW.text, go, method);
		this.waitingForAnswer = false;
		yield break;
	}

	public void UpgradeParamUnlock(int numParam, GameObject go, string method)
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

	private IEnumerator _UpgradeParamUnlock(int numParam, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=34";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "paramnum=" + numParam;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this._phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		this.ValidateAndCall(newWWW.text, go, method);
		this.waitingForAnswer = false;
		yield break;
	}

	public void UpgradeParamAllUnlock(int needHealth, int needArmor, int needSpeed, int needJump, int needDefend, int upgradeMoney, GameObject go, string method)
	{
		base.StartCoroutine(this._UpgradeParamAllUnlock(needHealth, needArmor, needSpeed, needJump, needDefend, go, method));
		this.SendStatIoTrack("UpgradeParamUNLOCK_GOLD", upgradeMoney);
		this.SendStatIoTrack("GOLD-", upgradeMoney);
		this.SendStatIoTrack("WeaponsAllParams", upgradeMoney);
		this.SendStatIoTrack("WeaponsAllParams_N", 1);
	}

	private IEnumerator _UpgradeParamAllUnlock(int needHealth, int needArmor, int needSpeed, int needJump, int needDefend, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
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
		requestSig += this._phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		this.ValidateAndCall(newWWW.text, go, method);
		this.waitingForAnswer = false;
		yield break;
	}

	public void BuySkin(int numSkin, GameObject go, string method)
	{
		this.SheduleStartCoroutine(this._BuySkin(numSkin, go, method));
	}

	private IEnumerator _BuySkin(int numSkin, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=10";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "skinnum=" + numSkin;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this._phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		this.ValidateAndCall(newWWW.text, go, method);
		this.waitingForAnswer = false;
		yield break;
	}

	public void GoldToMoney(int numGold, GameObject go, string method)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["numgold"] = numGold.ToString();
		ServerCallback cb = delegate(string s)
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
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=11";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "numgold=" + numGold;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this._phpSecret;
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
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["newname"] = AuxFunc.CodeRussianName(newName);
		this.Request(12, dictionary, delegate(string text)
		{
			string[] array = text.Split(new char[]
			{
				';'
			});
			if (array[0] == "1")
			{
				Kube.GPS.playerName = AuxFunc.CodeRussianName(newName);
			}
			Kube.SendMonoMessage("UpdateName", new object[]
			{
				text
			});
		});
	}

	public void BuyBullets(int typeBullets, int numTarif, GameObject go, string method)
	{
		base.StartCoroutine(this._BuyBullets(typeBullets, numTarif, go, method));
	}

	private IEnumerator _BuyBullets(int typeBullets, int numTarif, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
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
		requestSig += this._phpSecret;
		string requestSigMD5 = AuxFunc.GetMD5(requestSig);
		requestStr = requestStr + "&sig=" + requestSigMD5;
		WWW newWWW = new WWW(requestStr);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		this.ValidateAndCall(newWWW.text, go, method);
		this.waitingForAnswer = false;
		yield break;
	}

	public void SendEndLevel(EndGameStats endGameStats, GameObject go, string method)
	{
		this.SheduleStartCoroutine(this._SendEndLevel(endGameStats.playerExp, endGameStats.deltaExp, endGameStats.playerFrags, endGameStats.deltaFrags, endGameStats.deltaKills, endGameStats.playerMoney1, endGameStats.deltaMoney, endGameStats.playerLevel, endGameStats.newLevel, endGameStats.bonuses, go, method));
	}

	private string cub2_crc(int value)
	{
		string text = (value + 143).ToString("X");
		int num = 0;
		for (int i = 0; i < text.Length; i++)
		{
			num += (int)text[i];
		}
		return num.ToString("X");
	}

	private IEnumerator _SendEndLevel(uint oldExp, int deltaExp, int oldFrags, int deltaFrags, int deltaKills, int oldMoney, int deltaMoney, int oldLevel, int newLevel, int[] bonuses, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		MonoBehaviour.print(string.Concat(new object[]
		{
			"SendEndLevel:",
			(long)((ulong)oldExp + (ulong)((long)deltaExp)),
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
		str = "id=" + Kube.SS.serverId;
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
		str = "deltakills=" + deltaKills;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "crc=" + this.cub2_crc(deltaFrags + deltaKills);
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
		requestSig += this._phpSecret;
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
		char[] dc = new char[]
		{
			'^'
		};
		string[] strs = newWWW.text.Split(dc);
		go.SendMessage(method, strs);
		this.waitingForAnswer = false;
		yield break;
	}

	public int UnixTime()
	{
		return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
	}

	public void BuyNewMap(int maptype, ServerCallback cb)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["maptype"] = maptype.ToString();
		this.Request(616, dictionary, cb);
	}

	public void UseItem(int numItem)
	{
		this.SheduleStartCoroutine(this._UseItem(numItem));
	}

	private IEnumerator _UseItem(int numItem)
	{
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=17";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "numitem=" + numItem;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this._phpSecret;
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
		base.StartCoroutine(this._TakeItem(numItem, itemCountNow, go, method));
	}

	private IEnumerator _TakeItem(int numItem, int itemCountNow, GameObject go, string method)
	{
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=18";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "numitem=" + numItem;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += itemCountNow;
		requestSig += this._phpSecret;
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

	private IEnumerator _Request(int q, Dictionary<string, string> paramData, ServerCallback cb)
	{
		this.waitingForAnswer = true;
		if (paramData == null)
		{
			paramData = new Dictionary<string, string>();
		}
		WWWForm form = new WWWForm();
		paramData["requestCode"] = q.ToString();
		if (this.secret_token != string.Empty)
		{
			paramData["token"] = this.secret_token;
		}
		if (!paramData.ContainsKey("id") && Kube.SS.serverId != 0)
		{
			paramData["id"] = Kube.SS.serverId.ToString();
		}
		List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>(paramData);
		myList.Sort((KeyValuePair<string, string> keyfirst, KeyValuePair<string, string> keylast) => keyfirst.Key.CompareTo(keylast.Key));
		StringBuilder md5sig = new StringBuilder();
		foreach (KeyValuePair<string, string> rec in myList)
		{
			md5sig.Append(rec.Key + "=" + rec.Value);
			form.AddField(rec.Key, rec.Value);
		}
		md5sig.Append(this._phpSecret);
		form.AddField("sig", AuxFunc.GetMD5(md5sig.ToString()));
		WWW newWWW = new WWW(this.phpServer + this.mainPhpScript, form);
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		try
		{
			if (cb != null)
			{
				cb(newWWW.text);
			}
		}
		catch (Exception ex)
		{
			Exception e = ex;
			UnityEngine.Debug.LogException(e);
		}
		this.waitingForAnswer = false;
		yield break;
	}

	public void Request(int q, object param, ServerCallback cb)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["object"] = param.ToString();
		base.StartCoroutine(this._Request(q, dictionary, cb));
	}

	public void Request(int q, Dictionary<string, string> paramData, ServerCallback cb)
	{
		this.SheduleStartCoroutine(this._Request(q, paramData, cb));
	}

	public void LoadIsMap(long mapId, GameObject go, string method)
	{
		base.StartCoroutine(this._LoadIsMap(mapId, go, method));
	}

	protected IEnumerator _LoadIsMap(long mapId, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=19";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "mapid=" + mapId;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this._phpSecret;
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
		base.StartCoroutine(this._SetMapName(mapId, mapName));
	}

	private IEnumerator _SetMapName(long mapId, string mapName)
	{
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
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
		if (this.secret_token != string.Empty)
		{
			str = "token=" + this.secret_token;
			requestSig += str;
			requestStr = requestStr + "&" + str;
		}
		requestSig += this._phpSecret;
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
		if (this.sendStat)
		{
			base.StartCoroutine(this._SendStat(statName));
		}
	}

	private IEnumerator _SendStat(string statName)
	{
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
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
		requestSig += this._phpSecret;
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
		if (this.sendStat)
		{
			base.StartCoroutine(this._SendStatCount(statName, count));
		}
	}

	private IEnumerator _SendStatCount(string statName, int count)
	{
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
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
		requestSig += this._phpSecret;
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
		base.StartCoroutine(this._BuyVIP(numVIP, go, method));
		this.SendStatIoTrack("VIP_GOLD", Kube.GPS.vipPrice[numVIP, 1]);
		this.SendStatIoTrack("GOLD-", Kube.GPS.vipPrice[numVIP, 1]);
	}

	private IEnumerator _BuyVIP(int numVIP, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=23";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "vipnum=" + numVIP;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this._phpSecret;
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

	public void RegenerateMap(int maptype, long numMap, ServerCallback cb)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["nummap"] = numMap.ToString();
		dictionary["maptype"] = maptype.ToString();
		if (this.secret_token != string.Empty)
		{
			dictionary["token"] = this.secret_token;
		}
		this.Request(624, dictionary, cb);
	}

	public void SetSkin(int numSkin)
	{
		base.StartCoroutine(this._SetSkin(numSkin));
	}

	private IEnumerator _SetSkin(int numSkin)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=26";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "skinnum=" + numSkin;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this._phpSecret;
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
		base.StartCoroutine(this._SetClothes(clothes));
	}

	private IEnumerator _SetClothes(string clothes)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=31";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "clothes=" + clothes;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this._phpSecret;
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
		base.StartCoroutine(this._SendContest(ids, moneys, golds, go, method));
	}

	private IEnumerator _SendContest(string ids, string moneys, string golds, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
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
		requestSig += this._phpSecret;
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
		base.StartCoroutine(this._BuyClothes(numClothes, go, method));
		this.SendStatIoTrack("ClothesMoney", Kube.GPS.clothesPrice[numClothes, 1]);
		this.SendStatIoTrack("MONEY-", Kube.GPS.clothesPrice[numClothes, 1]);
		this.SendStatIoTrack("ClothesGold", Kube.GPS.clothesPrice[numClothes, 2]);
		this.SendStatIoTrack("GOLD-", Kube.GPS.clothesPrice[numClothes, 2]);
	}

	private void SaveFastInventoryBC(string res)
	{
		Kube.SendMonoMessage("SavedInventory", new object[0]);
		UnityEngine.Debug.Log(res);
	}

	public void SaveFastInventory(int type, FastInventar[] inventory, ServerCallback cb)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["type"] = type.ToString();
		string text = "++";
		for (int i = 0; i < 10; i++)
		{
			text = text + Kube.OH.GetServerCode((int)((byte)inventory[i].Type), 1) + Kube.OH.GetServerCode(inventory[i].Num, 2);
		}
		dictionary["data"] = text;
		dictionary["id"] = Kube.SS.serverId.ToString();
		this.Request(668, dictionary, new ServerCallback(this.SaveFastInventoryBC));
	}

	private IEnumerator _BuyClothes(int numClothes, GameObject go, string method)
	{
		this.waitingForAnswer = true;
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=30";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		str = "clothesnum=" + numClothes;
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this._phpSecret;
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
		base.StartCoroutine(this._ImHere());
	}

	private IEnumerator _ImHere()
	{
		string requestSig = string.Empty;
		string requestStr = this.phpServer + this.mainPhpScript + "?";
		string str = string.Empty;
		str = "id=" + Kube.SS.serverId;
		requestSig += str;
		requestStr += str;
		str = "requestCode=32";
		requestSig += str;
		requestStr = requestStr + "&" + str;
		requestSig += this._phpSecret;
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

	public void LoadStatistics(int dayFrom, int dayTo, GameObject go, string method)
	{
		base.StartCoroutine(this._LoadStatistics(dayFrom, dayTo, go, method));
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
		str = "id=" + Kube.SS.serverId;
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
		requestSig += this._phpSecret;
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

	public void UpgradeWeapon(int bt, int q, JSONServerCallback upgradeWeaponDone)
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

	private void OnGUI()
	{
		GUI.depth = -10;
		if (this.waitingForAnswer)
		{
			int num = (int)(Time.time * 10f) % this.waitingTex.Length;
			GUI.DrawTexture(new Rect((float)KUI.width - 64f, 0f, 64f, 64f), this.waitingTex[num]);
		}
	}

	public void SendStatIoTrack(string statName, int inc = 1)
	{
	}

	public void LoadMissions(JSONServerCallback missionLoadDone)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["id"] = Kube.SS.serverId.ToString();
		ServerCallback cb = delegate(string s)
		{
			missionLoadDone(JsonMapper.ToObject(s));
		};
		Kube.SS.Request(666, dictionary, cb);
	}

	public void EndMission(int _missionId, EndGameStats endGameStats, ServerCallback onMissionEnd)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		int deltaExp = endGameStats.deltaExp;
		dictionary["id"] = Kube.SS.serverId.ToString();
		dictionary["score"] = deltaExp.ToString();
		dictionary["mission"] = _missionId.ToString();
		dictionary["frags"] = endGameStats.deltaFrags.ToString();
		dictionary["money"] = endGameStats.deltaMoney.ToString();
		dictionary["l"] = endGameStats.newLevel.ToString();
		dictionary["b"] = StringUtils.int_join(';', endGameStats.bonuses);
		UnityEngine.Debug.Log("BONUS: " + dictionary["b"]);
		Kube.SS.Request(667, dictionary, onMissionEnd);
	}

	public void BuyWeaponSkin(int weaponId, int index, GameObject gameObject, string p)
	{
		bool isGold = Kube.GPS.weaponsSkinPrice2[index] != 0;
		int value = (!isGold) ? Kube.GPS.weaponsSkinPrice1[index] : Kube.GPS.weaponsSkinPrice2[index];
		ServerCallback cb = delegate(string s)
		{
			JsonData jsonData = JsonMapper.ToObject(s);
			if (jsonData.Keys.Contains("error"))
			{
				return;
			}
			Kube.GPS.weaponsSkin[index] = 1;
			if (isGold)
			{
				GameParamsScript gps = Kube.GPS;
				gps.playerMoney2 -= value;
			}
			else
			{
				GameParamsScript gps2 = Kube.GPS;
				gps2.playerMoney1 -= value;
			}
			gameObject.SendMessage(p);
		};
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["weaponId"] = weaponId.ToString();
		dictionary["index"] = index.ToString();
		Kube.SS.Request(701, dictionary, cb);
		gameObject.SendMessage(p);
	}

	public void UseWeaponSkin(int weaponId, int index, GameObject gameObject, string p)
	{
		Kube.GPS.weaponsCurrentSkin[weaponId] = index;
		ServerCallback cb = delegate(string s)
		{
			gameObject.SendMessage(p);
		};
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["weaponId"] = weaponId.ToString();
		dictionary["index"] = index.ToString();
		Kube.SS.Request(702, dictionary, cb);
	}

	[NonSerialized]
	public string secret_token;

	[NonSerialized]
	public string phpServer;

	[HideInInspector]
	protected string _phpSecret = string.Empty;

	private bool _loadingMap;

	[NonSerialized]
	public bool waitingForAnswer;

	private bool _savingMap;

	private int _serverId;

	public Texture[] waitingTex;

	private float _serverTime;

	[NonSerialized]
	public bool sendStat;

	[NonSerialized]
	public bool sendStatPay;

	[NonSerialized]
	public bool justPaid;

	[NonSerialized]
	public string mainPhpScript = "mainScript.php";

	protected bool initialized;

	private List<IEnumerator> _sheduled = new List<IEnumerator>();

	private static char[] dc = new char[]
	{
		'^'
	};

	private static char[] dc2 = new char[]
	{
		';'
	};

	private NetworkObjectScript NO;
}
