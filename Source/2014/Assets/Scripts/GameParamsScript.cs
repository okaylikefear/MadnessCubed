using System;
using System.Collections;
using kube;
using kube.data;
using UnityEngine;

public class GameParamsScript : MonoBehaviour
{
	public string playerName
	{
		get
		{
			return this._playerName;
		}
		set
		{
			this._playerName = value;
			this.decodePlayerName = AuxFunc.DecodeRussianName(value);
		}
	}

	public string locale
	{
		get
		{
			return this._locale;
		}
	}

	public void SetLocale(string locale)
	{
		if (this._locale == locale)
		{
			return;
		}
		TextAsset textAsset = Resources.Load(locale) as TextAsset;
		if (textAsset != null)
		{
			LocalizeUtils.load(textAsset.bytes);
			this._locale = locale;
		}
		else
		{
			base.StartCoroutine(this._SetLocale(locale));
		}
	}

	private IEnumerator _SetLocale(string locale)
	{
		WWW www = new WWW(Kube.SS.assetPath + Kube.SN.locale + ".txt");
		yield return www;
		if (www.error == null)
		{
			LocalizeUtils.load(www.bytes);
			this._locale = locale;
		}
		yield break;
	}

	public bool isVIP
	{
		get
		{
			return Kube.GPS.vipEnd > Time.time;
		}
	}

	public int maxAvailableCubes
	{
		get
		{
			return -this._maxAvailableCubes + this.codeI;
		}
		set
		{
			this._maxAvailableCubes = this.codeI - value;
		}
	}

	public string playerClothesStr
	{
		get
		{
			string text = string.Empty;
			for (int i = 0; i < this.playerClothes.Length; i++)
			{
				if (text.Length != 0)
				{
					text += ";";
				}
				text = text + string.Empty + this.playerClothes[i];
			}
			return text;
		}
		set
		{
		}
	}

	public void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.inventarCubesPrice = new int[6, 3];
		this.cubesTimeOfEnd = new float[6];
		this.inventarItemsLength = 160;
		this.inventarItems = new int[this.inventarItemsLength];
		this.inventarItemPrice1 = new int[this.inventarItemsLength];
		this.inventarItemPrice2 = new int[this.inventarItemsLength];
		this.inventarWeapons = new int[50];
		this.inventarSpecItems = new int[20];
		this.specItemsPrice1 = new int[this.inventarSpecItems.Length, 3];
		this.specItemsPrice2 = new int[this.inventarSpecItems.Length, 3];
		this.skinPrice = new int[32, 3];
		this.specBonusesPrice = new int[20, 20];
		this.clothesPrice = new int[80, 3];
		this.clothesBonus = new float[80, 16];
		this.upgradePrice = new PriceValue[this.inventarWeapons.Length, 6, 8];
		this.fastInventar = new FastInventar[11];
		this.fastInventarWeapon = new FastInventar[11];
		for (int i = 0; i < 11; i++)
		{
			this.fastInventar[i].Type = 0;
			this.fastInventar[i].Num = Kube.IS.cubesNatureNums[i];
		}
		for (int j = 0; j < 11; j++)
		{
			this.fastInventarWeapon[j].Type = -1;
		}
		this.weaponsPrice1 = new int[this.inventarWeapons.Length, 3];
		this.weaponsPrice2 = new int[this.inventarWeapons.Length, 3];
		this.playerIsClothes = new int[80];
		this.playerClothes = new int[80];
		for (int k = 0; k < this.playerClothes.Length; k++)
		{
			this.playerClothes[k] = -1;
		}
		this.maxAvailableCubes = 20;
		this.initialized = true;
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		this.codeI = UnityEngine.Random.Range(0, 99999);
		this.codeF = UnityEngine.Random.value * 10000f;
		base.InvokeRepeating("ChangeCodes", 5f, 5f);
	}

	private void ChangeCodes()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		base.gameObject.SendMessage("SaveCodeVars");
		if (Kube.BCS != null)
		{
			Kube.BCS.BroadcastMessage("SaveCodeVars", SendMessageOptions.DontRequireReceiver);
		}
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SendMessage("SaveCodeVars");
		}
		GameObject[] array2 = GameObject.FindGameObjectsWithTag("Transport");
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j].SendMessage("SaveCodeVars");
		}
		this.codeI = UnityEngine.Random.Range(0, 99999);
		this.codeF = UnityEngine.Random.value * 10000f;
		base.gameObject.SendMessage("LoadCodeVars");
		if (Kube.BCS != null)
		{
			Kube.BCS.BroadcastMessage("LoadCodeVars", SendMessageOptions.DontRequireReceiver);
		}
		for (int k = 0; k < array.Length; k++)
		{
			array[k].SendMessage("LoadCodeVars");
		}
		for (int l = 0; l < array2.Length; l++)
		{
			array2[l].SendMessage("LoadCodeVars");
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.L))
		{
			this.showLog = true;
		}
		if (UnityEngine.Input.GetKeyUp(KeyCode.L))
		{
			this.showLog = false;
		}
	}

	public void printLog(string str)
	{
		this.LOGStrs.Add(str);
		if (this.LOGStrs.Count > 32)
		{
			this.LOGStrs.RemoveAt(0);
		}
	}

	public void printMessage(string str, Color color)
	{
		if (this.MessagesStrs.Count != 0 && ((GameParamsScript.MessagesStruct)this.MessagesStrs[this.MessagesStrs.Count - 1]).message == str && Time.time - ((GameParamsScript.MessagesStruct)this.MessagesStrs[this.MessagesStrs.Count - 1]).time < this.messageTime)
		{
			return;
		}
		GameParamsScript.MessagesStruct messagesStruct = default(GameParamsScript.MessagesStruct);
		messagesStruct.message = str;
		messagesStruct.color = color;
		messagesStruct.time = Time.time;
		if (messagesStruct.color == Color.white)
		{
			messagesStruct.time += this.messageTime;
		}
		this.MessagesStrs.Add(messagesStruct);
		if (this.MessagesStrs.Count > 16)
		{
			this.MessagesStrs.RemoveAt(0);
		}
	}

	public void ClearMessages()
	{
		this.MessagesStrs.Clear();
	}

	public void printSystemMessage(string str, Color color)
	{
		if (this.SystemMessagesStrs.Count != 0 && ((GameParamsScript.MessagesStruct)this.SystemMessagesStrs[this.SystemMessagesStrs.Count - 1]).message == str && Time.time - ((GameParamsScript.MessagesStruct)this.SystemMessagesStrs[this.SystemMessagesStrs.Count - 1]).time < this.messageTime)
		{
			return;
		}
		GameParamsScript.MessagesStruct messagesStruct = default(GameParamsScript.MessagesStruct);
		messagesStruct.message = str;
		messagesStruct.color = color;
		messagesStruct.time = Time.time;
		if (messagesStruct.color == Color.white)
		{
			messagesStruct.time += this.messageTime;
		}
		this.SystemMessagesStrs.Add(messagesStruct);
		if (this.SystemMessagesStrs.Count > 16)
		{
			this.SystemMessagesStrs.RemoveAt(0);
		}
	}

	private void OnGUI()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		GUI.depth = -3;
		if (this.showLog)
		{
			string text = string.Empty;
			for (int i = this.LOGStrs.Count - 1; i >= 0; i--)
			{
				if (i != this.LOGStrs.Count - 1)
				{
					text += "\n";
				}
				text += (string)this.LOGStrs[i];
			}
			GUI.Box(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), text);
		}
		if (this.showMessages)
		{
			Color color = GUI.color;
			for (int j = 0; j < this.MessagesStrs.Count; j++)
			{
				int index = this.MessagesStrs.Count - j - 1;
				GUI.color = ((GameParamsScript.MessagesStruct)this.MessagesStrs[index]).color;
				GUI.Label(new Rect(0.05f * num, num2 - 175f - (float)j * 25f, 750f, 28f), ((GameParamsScript.MessagesStruct)this.MessagesStrs[index]).message);
			}
			GUI.color = color;
		}
		else
		{
			if (this.MessagesStrs.Count > 0)
			{
				GUI.skin = Kube.ASS1.emptySkin;
				Color color2 = GUI.color;
				for (int k = 0; k < this.MessagesStrs.Count; k++)
				{
					int index2 = this.MessagesStrs.Count - k - 1;
					if (Time.time - ((GameParamsScript.MessagesStruct)this.MessagesStrs[index2]).time < this.messageTime)
					{
						Color color3 = ((GameParamsScript.MessagesStruct)this.MessagesStrs[index2]).color;
						float a = 1f;
						if (Time.time - ((GameParamsScript.MessagesStruct)this.MessagesStrs[index2]).time > this.messageTime - 2f)
						{
							a = this.messageTime - (Time.time - ((GameParamsScript.MessagesStruct)this.MessagesStrs[index2]).time);
						}
						color3.a = a;
						GUI.color = color3;
						GUI.Label(new Rect(0.05f * num, num2 - 175f - (float)k * 18f, 750f, 28f), ((GameParamsScript.MessagesStruct)this.MessagesStrs[index2]).message);
					}
				}
				GUI.color = color2;
			}
			if (this.SystemMessagesStrs.Count > 0)
			{
				GUI.skin = Kube.ASS1.emptySkin;
				Color color4 = GUI.color;
				for (int l = 0; l < this.SystemMessagesStrs.Count; l++)
				{
					int index3 = this.SystemMessagesStrs.Count - l - 1;
					if (Time.time - ((GameParamsScript.MessagesStruct)this.SystemMessagesStrs[index3]).time < this.systemMessageTime)
					{
						Color color5 = ((GameParamsScript.MessagesStruct)this.SystemMessagesStrs[index3]).color;
						float a2 = color5.a;
						if (Time.time - ((GameParamsScript.MessagesStruct)this.SystemMessagesStrs[index3]).time > this.messageTime - 2f)
						{
							a2 = this.messageTime - (Time.time - ((GameParamsScript.MessagesStruct)this.SystemMessagesStrs[index3]).time);
						}
						color5.a = Mathf.Min(a2, color5.a);
						GUI.color = color5;
						GUI.Label(new Rect(20f, 150f + (float)l * 18f, 750f, 28f), ((GameParamsScript.MessagesStruct)this.SystemMessagesStrs[index3]).message);
					}
				}
				GUI.color = color4;
			}
		}
	}

	private void Awake()
	{
		Kube.GPS = this;
	}

	private void OnDestroy()
	{
		Kube.GPS = null;
	}

	private void SaveCodeVars()
	{
		this.codeVarsRandom = UnityEngine.Random.Range(10, 1000);
		this._maxAvailableCubes2 = this.maxAvailableCubes + this.codeVarsRandom;
	}

	private void LoadCodeVars()
	{
		this.maxAvailableCubes = this._maxAvailableCubes2 - this.codeVarsRandom;
	}

	public string user;

	public int playerId;

	public int bonusDay = -1;

	public int specBonusDay = -1;

	public int dayNum;

	public int[] parts;

	private string _playerName;

	public string decodePlayerName;

	private string _locale = "ru_RU";

	public int playerNumMaps;

	public float[] cubesTimeOfEnd;

	public int inventarItemsLength = 160;

	public int[] inventarItems;

	public int playerMoney1;

	public int playerMoney2;

	public int[] inventarWeapons;

	public int playerHealth;

	public int playerArmor;

	public int playerSpeed;

	public int playerJump;

	public int playerDefend;

	public int playerExpPoints;

	public int playerExp;

	public int playerFrags;

	public int playerLevel;

	public int[] playerSkins = new int[32];

	public int playerVoices;

	public float vipEnd;

	public int playerSkin;

	public int[] inventarSpecItems;

	public int moderType;

	public int moderLastContest;

	public int[] playerIsClothes = new int[64];

	public int[] playerClothes = new int[32];

	public int[,] inventarCubesPrice;

	public int[] inventarItemPrice1;

	public int[] inventarItemPrice2;

	public int[,] weaponsPrice1;

	public int[,] weaponsPrice2;

	public float[,] exchangeMoney = new float[5, 3];

	public float[,] exchangeSpec = new float[6, 4];

	public int specToMoney;

	public int[,] hatPrice = new int[32, 3];

	public string[] BonusTypeCode = new string[]
	{
		"health",
		"armor",
		"speed",
		"jump",
		"defend",
		"qwe",
		"qwe",
		"qwe",
		"qwe",
		"qwe",
		"qwe",
		"qew",
		"qwe",
		"qwe",
		"qwe",
		"qwe"
	};

	public float[,] hatBonus = new float[32, 16];

	public int[,] skinPrice;

	public float[,] skinBonus = new float[32, 16];

	public int[,] ammunitionPrice = new int[32, 3];

	public float[,] ammunitionBonus = new float[32, 16];

	public float[,,] charParamsPrice = new float[5, 8, 5];

	public int playerBaseHealth;

	public int playerBaseArmor;

	public int playerBaseSpeed;

	public int playerBaseJump;

	public int playerBaseDefend;

	public int[,,] bulletsPrice = new int[12, 3, 5];

	public float newMapPrice;

	public int[,] bonusesPrice;

	public int[,] vipPrice = new int[3, 2];

	public int vipBonus;

	public int[,] specItemsPrice1;

	public int[,] specItemsPrice2;

	public int[,] specBonusesPrice;

	private int _maxAvailableCubes;

	public int[,] moderContests = new int[5, 6];

	public int[,] clothesPrice;

	public float[,] clothesBonus;

	public PriceValue[,,] upgradePrice;

	public FastInventar[] fastInventar;

	public FastInventar[] fastInventarWeapon;

	public int codeI;

	public float codeF;

	public float radarZoom = 0.5f;

	public int[] skinItems;

	public int currentSpecBonusNum = -1;

	public bool needTraining;

	public bool needTrainingBuild;

	private bool initialized;

	private ArrayList LOGStrs = new ArrayList();

	private bool showLog;

	private ArrayList MessagesStrs = new ArrayList();

	private ArrayList SystemMessagesStrs = new ArrayList();

	private bool showMessages;

	private float messageTime = 7f;

	private float systemMessageTime = 3f;

	private int codeVarsRandom;

	private int _maxAvailableCubes2;

	private struct MessagesStruct
	{
		public string message;

		public Color color;

		public float time;
	}
}
