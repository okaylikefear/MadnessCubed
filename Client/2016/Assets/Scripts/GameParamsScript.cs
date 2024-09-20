using System;
using System.Collections;
using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.ObscuredTypes;
using kube;
using kube.cheat;
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

	public ClanInfo clan { get; set; }

	public string locale
	{
		get
		{
			return this._locale;
		}
	}

	public void SetLocale(string locale)
	{
		MonoBehaviour.print("SetLocale(" + locale + ")");
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
		WWW www = Kube.RM.WWWLoad(Kube.SN.locale + ".txt");
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
		this.inventarCubesPrice1 = new int[6, 3];
		this.inventarCubesPrice2 = new int[6, 3];
		this.cubesTimeOfEnd = new float[6];
		int num = 250;
		this.inventarItems = new GameParamsScript.InventarItems(num);
		this.inventarItemPrice1 = new int[num];
		this.inventarItemPrice2 = new int[num];
		this.inventarWeapons = new ObscuredIntAB[80];
		this.inventarSpecItems = new int[22];
		this.specItemsPrice1 = new int[this.inventarSpecItems.Length, 3];
		this.specItemsPrice2 = new int[this.inventarSpecItems.Length, 3];
		this.skinPrice = new int[32, 3];
		this.specBonusesPrice = new int[20, 20];
		this.clothesPrice = new int[86, 3];
		this.clothesBonus = new float[86, 16];
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
		this.playerIsClothes = new int[86];
		this.playerClothes = new int[86];
		for (int k = 0; k < this.playerClothes.Length; k++)
		{
			this.playerClothes[k] = -1;
		}
		this.maxAvailableCubes = 20;
		this.initialized = true;
		for (int l = 0; l < this.weaponsCurrentSkin.Length; l++)
		{
			this.weaponsCurrentSkin[l] = -1;
		}
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		this.codeI = UnityEngine.Random.Range(0, 99999);
		this.codeF = UnityEngine.Random.value * 10000f;
		base.InvokeRepeating("ChangeCodes", 5f, 5f);
		ObscuredCheatingDetector.StartDetection(new Action(Kube.Ban));
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
			array[i].SendMessage("SaveCodeVars", SendMessageOptions.DontRequireReceiver);
		}
		GameObject[] array2 = GameObject.FindGameObjectsWithTag("Transport");
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j].SendMessage("SaveCodeVars", SendMessageOptions.DontRequireReceiver);
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
			array[k].SendMessage("LoadCodeVars", SendMessageOptions.DontRequireReceiver);
		}
		for (int l = 0; l < array2.Length; l++)
		{
			array2[l].SendMessage("LoadCodeVars", SendMessageOptions.DontRequireReceiver);
		}
		this.inventarItems.ApplyNewCryptoKey();
	}

	public void printLog(string str)
	{
		UnityEngine.Debug.Log(str);
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
		if (Kube.OH.emptyScreen)
		{
			return;
		}
		GUI.depth = -3;
		if (this.showMessages)
		{
			Color color = GUI.color;
			for (int i = 0; i < this.MessagesStrs.Count; i++)
			{
				int index = this.MessagesStrs.Count - i - 1;
				GUI.color = ((GameParamsScript.MessagesStruct)this.MessagesStrs[index]).color;
				GUI.Label(new Rect(0.05f * num, num2 - 175f - (float)i * 25f, 750f, 28f), ((GameParamsScript.MessagesStruct)this.MessagesStrs[index]).message);
			}
			GUI.color = color;
		}
		else
		{
			if (this.MessagesStrs.Count > 0)
			{
				GUI.skin = Kube.ASS1.emptySkin;
				Color color2 = GUI.color;
				for (int j = 0; j < this.MessagesStrs.Count; j++)
				{
					int index2 = this.MessagesStrs.Count - j - 1;
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
						GUI.Label(new Rect(0.05f * num, num2 - 175f - (float)j * 18f, 750f, 28f), ((GameParamsScript.MessagesStruct)this.MessagesStrs[index2]).message);
					}
				}
				GUI.color = color2;
			}
			if (this.SystemMessagesStrs.Count > 0)
			{
				GUI.skin = Kube.ASS1.emptySkin;
				Color color4 = GUI.color;
				for (int k = 0; k < this.SystemMessagesStrs.Count; k++)
				{
					int index3 = this.SystemMessagesStrs.Count - k - 1;
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
						GUI.Label(new Rect(20f, 150f + (float)k * 18f, 750f, 28f), ((GameParamsScript.MessagesStruct)this.SystemMessagesStrs[index3]).message);
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
		if (!this.inventarItems.check_crc())
		{
			Kube.Ban();
		}
		ObscuredInt.SetNewCryptoKey((int)(Time.realtimeSinceStartup * 100f) + UnityEngine.Random.Range(1, 100));
		ObscuredIntAB.SetNewCryptoKey((int)(Time.realtimeSinceStartup * 100f) + UnityEngine.Random.Range(1, 100));
	}

	public int maxPlayersLimit;

	public int maxPlayersInMission;

	public int maxPlayersSurvival;

	public string user;

	public int bonusDay = -1;

	public int dayNum;

	public int[] parts;

	private string _playerName;

	public string decodePlayerName;

	private string _locale = LocaleEnum.BAD.ToString();

	public int playerNumMaps;

	public float[] cubesTimeOfEnd;

	public ObscuredInt[] weaponsSkinPrice1 = new ObscuredInt[128];

	public ObscuredInt[] weaponsSkinPrice2 = new ObscuredInt[128];

	public GameParamsScript.InventarItems inventarItems;

	public ObscuredIntAB[] inventarWeapons;

	public ObscuredInt playerMoney1;

	public ObscuredInt playerMoney2;

	public ObscuredInt[] weaponsSkin = new ObscuredInt[128];

	[NonSerialized]
	public ObscuredInt[] weaponsCurrentSkin = new ObscuredInt[128];

	public int playerHealth;

	public int playerArmor;

	public int playerSpeed;

	public int playerJump;

	public int playerDefend;

	public int playerExpPoints;

	public uint playerExp;

	public int playerFrags;

	public int playerLevel;

	public ObscuredInt[] playerSkins = new ObscuredInt[32];

	public int playerVoices;

	public float vipEnd;

	public int playerSkin;

	public int[] inventarSpecItems;

	public int moderType;

	public int moderLastContest;

	public int[] playerIsClothes = new int[64];

	public int[] playerClothes = new int[32];

	public int[,] inventarCubesPrice1;

	public int[,] inventarCubesPrice2;

	public int[] inventarItemPrice1;

	public int[] inventarItemPrice2;

	public int[,] weaponsPrice1;

	public int[,] weaponsPrice2;

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

	public int currentSpecBonusNum = -1;

	public bool needTraining;

	public bool needTrainingBuild;

	private bool initialized;

	private ArrayList MessagesStrs = new ArrayList();

	private ArrayList SystemMessagesStrs = new ArrayList();

	private bool showMessages;

	private float messageTime = 7f;

	private float systemMessageTime = 3f;

	private int codeVarsRandom;

	private int _maxAvailableCubes2;

	public float mouseSens = 15f;

	[NonSerialized]
	public IntHash weaponUnlock = new IntHash();

	[NonSerialized]
	public IntHash itemUnlock = new IntHash();

	[NonSerialized]
	public IntHash specUnlock = new IntHash();

	[NonSerialized]
	public IntHash missionUnlock = new IntHash();

	[NonSerialized]
	public IntHash charUnlock = new IntHash();

	public class InventarItems
	{
		public InventarItems(int length)
		{
			this._inventarItems = new ObscuredIntAB[length];
		}

		public int this[int index]
		{
			get
			{
				return this._inventarItems[index];
			}
			set
			{
				this._inventarItems[index] = value;
				this._crc = this.make_crc();
			}
		}

		private int make_crc()
		{
			int num = this._inventarItems.Length;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				num2 += this._inventarItems[i];
			}
			return num2;
		}

		public bool check_crc()
		{
			return this._crc == this.make_crc();
		}

		public int Length
		{
			get
			{
				return this._inventarItems.Length;
			}
		}

		public void ApplyNewCryptoKey()
		{
			int num = this._inventarItems.Length;
			for (int i = 0; i < num; i++)
			{
				this._inventarItems[i].ApplyNewCryptoKey();
			}
		}

		protected ObscuredIntAB[] _inventarItems;

		protected ObscuredIntAB _crc;
	}

	private struct MessagesStruct
	{
		public string message;

		public Color color;

		public float time;
	}
}
