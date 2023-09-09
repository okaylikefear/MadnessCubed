using System;
using System.Text;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class LoadParamsScript : MonoBehaviour
{
	private void Start()
	{
		Application.runInBackground = true;
		Kube.SS.SendStatIoTrack("UnityLaunched", 1);
		this.InitSocialNet();
		MusicManagerScript component = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManagerScript>();
		AudioListener.volume = PlayerPrefs.GetFloat("soundVol", 1f);
		component.audio.volume = PlayerPrefs.GetFloat("musicVol", 1f);
		int num = PlayerPrefs.GetInt("screen", 1);
		num = Math.Min(num, Screen.resolutions.Length - 1);
		Kube.OH.screenResolution = Screen.resolutions[num];
	}

	private void Awake()
	{
		Kube.OH.BeginLoading();
	}

	private void Error()
	{
		this.isError = true;
	}

	private void Ban()
	{
		this.isBan = true;
	}

	private void InitSocialNet()
	{
		Kube.SN.Init(base.gameObject, "LoadDataFromNetwork");
	}

	private void LoadDataFromNetwork()
	{
		Kube.GPS.user = Kube.SN.playerUID;
		Kube.GPS.printLog("id = " + Kube.GPS.playerId);
		Kube.GPS.printLog("path:" + Application.absoluteURL + Application.webSecurityEnabled);
		GameObject.FindGameObjectWithTag("Music").SendMessage("ChangeMusic", 0, SendMessageOptions.DontRequireReceiver);
		Kube.SS.LoadPlayersParams(base.gameObject, "ParamsLoaded");
	}

	private int TryConvertToInt32(string val, int def = 0)
	{
		int result;
		try
		{
			result = Convert.ToInt32(val);
		}
		catch
		{
			result = def;
		}
		return result;
	}

	private float TryConvertToFloat(string val, float def = 0f)
	{
		float result;
		try
		{
			result = Convert.ToSingle(val);
		}
		catch
		{
			result = def;
		}
		return result;
	}

	private double TryConvertToDouble(string val)
	{
		double result;
		try
		{
			result = Convert.ToDouble(val);
		}
		catch
		{
			result = 0.0;
		}
		return result;
	}

	private int[] decodeJsonIntArray(JsonData par1)
	{
		int count = par1.Count;
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = this.TryConvertToInt32(par1[i].ToString(), 0);
		}
		return array;
	}

	private void ParamsLoaded(byte[] bytes)
	{
		char[] separator = new char[]
		{
			';'
		};
		char[] separator2 = new char[]
		{
			':'
		};
		char[] separator3 = new char[]
		{
			'='
		};
		char[] separator4 = new char[]
		{
			'\n'
		};
		int num = BitConverter.ToInt32(bytes, 0);
		string @string = Encoding.UTF8.GetString(bytes, 4, num);
		string string2 = Encoding.UTF8.GetString(bytes, 4 + num, 32);
		JsonData jsonData = JsonMapper.ToObject(@string);
		if (jsonData.Keys.Contains("token"))
		{
			Kube.SS.secret_token = jsonData["token"].ToString();
		}
		if (jsonData.Keys.Contains("offer"))
		{
			OfferBox.init(jsonData["offer"]);
		}
		if (jsonData.Keys.Contains("sq"))
		{
			Kube.SN.missionsFromServer(jsonData["sq"]);
		}
		if (jsonData.Keys.Contains("bp") && jsonData["bp"] != null)
		{
			Kube.GPS.parts = this.decodeJsonIntArray(jsonData["bp"]);
		}
		JsonData jsonData2 = jsonData["r"];
		if (AuxFunc.GetMD5(@string + Kube.SS.phpSecret) != string2)
		{
			this.isCheater = true;
			Kube.SS.SendStat("startcheat");
			return;
		}
		if (jsonData2.Count <= 1)
		{
			this.isCheater = true;
			Kube.SS.SendStat("startcheat");
			return;
		}
		string[] array = new string[jsonData2.Count + 2];
		for (int i = 0; i < jsonData2.Count; i++)
		{
			array[i + 2] = jsonData2[i].ToString();
		}
		MonoBehaviour.print(@string);
		this.isPending = false;
		Kube.GPS.Init();
		Kube.GPS.playerId = this.TryConvertToInt32(jsonData["id"].ToString(), 0);
		int num2 = this.TryConvertToInt32(jsonData["t"].ToString(), 0);
		Kube.GPS.dayNum = Mathf.FloorToInt((float)((double)num2 / 86400.0));
		MonoBehaviour.print(num2 + " " + Kube.GPS.dayNum);
		Kube.SS.serverTime = (float)num2;
		Kube.SS.serverLocalTime = Time.realtimeSinceStartup;
		Kube.GPS.bonusDay = this.TryConvertToInt32(jsonData["d"].ToString(), 0) % 10;
		Kube.GPS.specBonusDay = Mathf.RoundToInt((float)(this.TryConvertToInt32(array[1], 0) - Kube.GPS.bonusDay) / 10f);
		Kube.GPS.playerName = Encoding.ASCII.GetString(Convert.FromBase64String(jsonData2[1].ToString()));
		Kube.GPS.playerNumMaps = this.TryConvertToInt32(jsonData2[2].ToString(), 0);
		string[] array2 = array[5].Split(separator);
		for (int j = 0; j < array2.Length; j++)
		{
			Kube.GPS.cubesTimeOfEnd[j] = Time.time + (float)(this.TryConvertToInt32(array2[j], 0) - num2);
		}
		Kube.GPS.cubesTimeOfEnd[0] = Time.time + 1E+07f;
		string[] array3 = array[6].Split(separator);
		int num3 = 0;
		while (num3 < array3.Length && num3 < Kube.GPS.inventarItemsLength)
		{
			Kube.GPS.inventarItems[num3] = this.TryConvertToInt32(array3[num3], 0);
			num3++;
		}
		Kube.GPS.playerMoney1 = this.TryConvertToInt32(array[7], 0);
		Kube.GPS.playerMoney2 = this.TryConvertToInt32(array[8], 0);
		string[] array4 = array[9].Split(separator);
		for (int k = 0; k < array4.Length; k++)
		{
			Kube.GPS.inventarWeapons[k] = this.TryConvertToInt32(array4[k], 0);
			if (Kube.GPS.inventarWeapons[k] == 1)
			{
				Kube.GPS.inventarWeapons[k] = num2 + 10000000;
			}
			Kube.GPS.inventarWeapons[k] = (int)Time.time + (Kube.GPS.inventarWeapons[k] - num2);
		}
		string[] array5 = array[10].Split(separator);
		Kube.GPS.playerHealth = this.TryConvertToInt32(array5[0], 0);
		Kube.GPS.playerArmor = this.TryConvertToInt32(array5[1], 0);
		Kube.GPS.playerSpeed = this.TryConvertToInt32(array5[2], 0);
		Kube.GPS.playerJump = this.TryConvertToInt32(array5[3], 0);
		Kube.GPS.playerDefend = this.TryConvertToInt32(array5[4], 0);
		Kube.GPS.playerExpPoints = this.TryConvertToInt32(array[11], 0);
		Kube.GPS.playerExp = this.TryConvertToInt32(array[12], 0);
		Kube.GPS.playerFrags = this.TryConvertToInt32(array[13], 0);
		Kube.GPS.playerLevel = this.TryConvertToInt32(array[14], 0);
		string[] array6 = array[15].Split(separator);
		for (int l = 0; l < array6.Length; l++)
		{
			Kube.GPS.playerSkins[l] = this.TryConvertToInt32(array6[l], 0);
		}
		string[] array7 = array[16].Split(separator);
		int num4 = Math.Min(array7.Length, Kube.IS.bulletParams.Length);
		for (int m = 0; m < num4; m++)
		{
			string[] array8 = array7[m].Split(separator2);
			Kube.IS.bulletParams[m].initialAmount = Mathf.Max(Kube.IS.bulletParams[m].initialAmount, this.TryConvertToInt32(array8[0], 0));
		}
		Kube.GPS.playerVoices = this.TryConvertToInt32(array[18], 0);
		if (Kube.GPS.playerVoices > 0)
		{
			Kube.SS.payer = true;
		}
		Kube.GPS.vipEnd = Time.time + (float)(this.TryConvertToInt32(array[19], 0) - num2);
		Kube.GPS.playerSkin = this.TryConvertToInt32(array[20], 0);
		string[] array9 = array[21].Split(separator);
		int num5 = 0;
		while (num5 < array9.Length && num5 < Kube.GPS.inventarSpecItems.Length)
		{
			Kube.GPS.inventarSpecItems[num5] = this.TryConvertToInt32(array9[num5], 0);
			Kube.GPS.inventarSpecItems[num5] = (int)Time.time + (Kube.GPS.inventarSpecItems[num5] - num2);
			num5++;
		}
		Kube.GPS.moderType = this.TryConvertToInt32(array[23], 0);
		Kube.GPS.moderLastContest = (int)Time.time + (this.TryConvertToInt32(array[24], 0) - num2);
		string[] array10 = array[25].Split(separator);
		int num6 = 0;
		while (num6 < array10.Length && num6 < Kube.GPS.playerIsClothes.Length)
		{
			Kube.GPS.playerIsClothes[num6] = this.TryConvertToInt32(array10[num6], 0);
			num6++;
		}
		string[] array11 = array[26].Split(separator);
		int num7 = 0;
		while (num7 < array11.Length && num7 < Kube.GPS.playerClothes.Length)
		{
			if (array11[num7].Length != 0)
			{
				Kube.GPS.playerClothes[num7] = this.TryConvertToInt32(array11[num7], -1);
			}
			num7++;
		}
		JsonData jsonData3 = jsonData["price"];
		string[] array12 = new string[jsonData3.Count];
		for (int n = 0; n < jsonData3.Count; n++)
		{
			array12[n] = jsonData3[n].ToString();
		}
		for (int num8 = 0; num8 < 6; num8++)
		{
			string[] array13 = array12[num8 + 1].Split(separator);
			Kube.GPS.inventarCubesPrice[num8, 0] = this.TryConvertToInt32(array13[0], 0);
			Kube.GPS.inventarCubesPrice[num8, 1] = this.TryConvertToInt32(array13[1], 0);
			Kube.GPS.inventarCubesPrice[num8, 2] = this.TryConvertToInt32(array13[2], 0);
		}
		for (int num9 = 0; num9 < Kube.GPS.inventarItemsLength; num9++)
		{
			if (array12[num9 + 7].Length != 0)
			{
				string[] array14 = array12[num9 + 7].Split(separator);
				Kube.GPS.inventarItemPrice1[num9] = this.TryConvertToInt32(array14[1], 0);
				Kube.GPS.inventarItemPrice2[num9] = this.TryConvertToInt32(array14[2], 0);
			}
		}
		for (int num10 = 0; num10 < Kube.GPS.inventarWeapons.Length; num10++)
		{
			string[] array15 = array12[num10 + 127].Split(separator);
			if (array15.Length >= 7)
			{
				Kube.GPS.weaponsPrice1[num10, 2] = this.TryConvertToInt32(array15[1], 0);
				Kube.GPS.weaponsPrice2[num10, 2] = this.TryConvertToInt32(array15[2], 0);
				Kube.GPS.weaponsPrice1[num10, 1] = this.TryConvertToInt32(array15[3], 0);
				Kube.GPS.weaponsPrice2[num10, 1] = this.TryConvertToInt32(array15[4], 0);
				Kube.GPS.weaponsPrice1[num10, 0] = this.TryConvertToInt32(array15[5], 0);
				Kube.GPS.weaponsPrice2[num10, 0] = this.TryConvertToInt32(array15[6], 0);
			}
		}
		for (int num11 = 0; num11 < 5; num11++)
		{
			string[] array16 = array12[num11 + 177].Split(separator);
			Kube.GPS.exchangeMoney[num11, 0] = this.TryConvertToFloat(array16[0], 0f);
			Kube.GPS.exchangeMoney[num11, 1] = this.TryConvertToFloat(array16[1], 0f);
			Kube.GPS.exchangeMoney[num11, 2] = this.TryConvertToFloat(array16[2], 0f);
		}
		for (int num12 = 0; num12 < 6; num12++)
		{
			string[] array17 = array12[num12 + 182].Split(separator);
			Kube.GPS.exchangeSpec[num12, 0] = this.TryConvertToFloat(array17[0], 0f);
			Kube.GPS.exchangeSpec[num12, 1] = this.TryConvertToFloat(array17[1], 0f);
			Kube.GPS.exchangeSpec[num12, 2] = this.TryConvertToFloat(array17[2], 0f);
			Kube.GPS.exchangeSpec[num12, 3] = this.TryConvertToFloat(array17[1], 0f) + (float)this.TryConvertToInt32(array17[2], 0);
		}
		Kube.GPS.specToMoney = this.TryConvertToInt32(array12[188], 0);
		string[] array18 = array12[189].Split(separator);
		for (int num13 = 0; num13 < array18.Length; num13++)
		{
			string[] array19 = array18[num13].Split(separator2);
			Kube.GPS.hatPrice[num13, 0] = this.TryConvertToInt32(array19[1], 0);
			Kube.GPS.hatPrice[num13, 1] = this.TryConvertToInt32(array19[2], 0);
			Kube.GPS.hatPrice[num13, 2] = this.TryConvertToInt32(array19[3], 0);
			for (int num14 = 4; num14 < array19.Length; num14++)
			{
				string[] array20 = array19[num14].Split(separator3);
				for (int num15 = 0; num15 < 16; num15++)
				{
					if (array20[0] == Kube.GPS.BonusTypeCode[num15])
					{
						Kube.GPS.hatBonus[num13, num15] = (float)this.TryConvertToDouble(array20[1]);
					}
				}
			}
		}
		string[] array21 = array12[190].Split(separator);
		for (int num16 = 0; num16 < array21.Length; num16++)
		{
			string[] array22 = array21[num16].Split(separator2);
			Kube.GPS.skinPrice[num16, 0] = this.TryConvertToInt32(array22[1], 0);
			Kube.GPS.skinPrice[num16, 1] = this.TryConvertToInt32(array22[2], 0);
			Kube.GPS.skinPrice[num16, 2] = this.TryConvertToInt32(array22[3], 0);
			for (int num17 = 4; num17 < array22.Length; num17++)
			{
				string[] array23 = array22[num17].Split(separator3);
				for (int num18 = 0; num18 < 16; num18++)
				{
					if (array23[0] == Kube.GPS.BonusTypeCode[num18])
					{
						Kube.GPS.skinBonus[num16, num18] = (float)this.TryConvertToDouble(array23[1]);
					}
				}
			}
		}
		string[] array24 = array12[191].Split(separator);
		for (int num19 = 0; num19 < array24.Length; num19++)
		{
			string[] array25 = array24[num19].Split(separator2);
			Kube.GPS.ammunitionPrice[num19, 0] = this.TryConvertToInt32(array25[1], 0);
			Kube.GPS.ammunitionPrice[num19, 1] = this.TryConvertToInt32(array25[2], 0);
			Kube.GPS.ammunitionPrice[num19, 2] = this.TryConvertToInt32(array25[3], 0);
			for (int num20 = 4; num20 < array25.Length; num20++)
			{
				string[] array26 = array25[num20].Split(separator3);
				for (int num21 = 0; num21 < 16; num21++)
				{
					if (array26[0] == Kube.GPS.BonusTypeCode[num21])
					{
						Kube.GPS.ammunitionBonus[num19, num21] = (float)this.TryConvertToDouble(array26[1]);
					}
				}
			}
		}
		string text = array12[192].Replace('*', ':');
		string[] array27 = text.Split(separator2);
		for (int num22 = 0; num22 < 5; num22++)
		{
			for (int num23 = 0; num23 < 8; num23++)
			{
				string[] array28 = array27[num22 * 9 + num23 + 1].Split(separator);
				Kube.GPS.charParamsPrice[num22, num23, 0] = (float)this.TryConvertToDouble(array28[0]);
				Kube.GPS.charParamsPrice[num22, num23, 1] = (float)this.TryConvertToDouble(array28[1]);
				Kube.GPS.charParamsPrice[num22, num23, 2] = (float)this.TryConvertToDouble(array28[2]);
				Kube.GPS.charParamsPrice[num22, num23, 3] = (float)this.TryConvertToDouble(array28[3]);
				Kube.GPS.charParamsPrice[num22, num23, 4] = (float)this.TryConvertToDouble(array28[4]);
			}
		}
		string[] array29 = array12[193].Split(separator);
		Kube.GPS.playerBaseHealth = this.TryConvertToInt32(array29[0], 0);
		Kube.GPS.playerBaseArmor = this.TryConvertToInt32(array29[1], 0);
		Kube.GPS.playerBaseSpeed = this.TryConvertToInt32(array29[2], 0);
		Kube.GPS.playerBaseJump = this.TryConvertToInt32(array29[3], 0);
		Kube.GPS.playerBaseDefend = this.TryConvertToInt32(array29[4], 0);
		string[] array30 = array12[194].Split(separator);
		for (int num24 = 0; num24 < array30.Length; num24++)
		{
			string[] array31 = array30[num24].Split(separator2);
			for (int num25 = 0; num25 < array31.Length - 1; num25++)
			{
				string[] array32 = array31[num25 + 1].Split(separator3);
				Kube.GPS.bulletsPrice[num24, num25, 0] = this.TryConvertToInt32(array32[0], 0);
				Kube.GPS.bulletsPrice[num24, num25, 1] = this.TryConvertToInt32(array32[1], 0);
				Kube.GPS.bulletsPrice[num24, num25, 2] = this.TryConvertToInt32(array32[2], 0);
				Kube.GPS.bulletsPrice[num24, num25, 3] = this.TryConvertToInt32(array32[3], 0);
				Kube.GPS.bulletsPrice[num24, num25, 4] = this.TryConvertToInt32(array32[4], 0);
			}
		}
		for (int num26 = 0; num26 < Kube.IS.bulletParams.Length; num26++)
		{
			for (int num27 = 0; num27 < Kube.GPS.bulletsPrice.GetLength(1); num27++)
			{
				Kube.IS.bulletParams[num26].initialAmountArray[num27 + 1] = Kube.GPS.bulletsPrice[num26, num27, 3];
			}
			for (int num28 = 0; num28 < Kube.IS.bulletParams[num26].initialAmountArray.Length; num28++)
			{
				if (Kube.IS.bulletParams[num26].initialAmountArray[num28] <= Kube.IS.bulletParams[num26].initialAmount)
				{
					Kube.IS.bulletParams[num26].initialAmountIndex = num28;
				}
			}
			Kube.IS.bulletParams[num26].initialAmount = Kube.IS.bulletParams[num26].initialAmountArray[Kube.IS.bulletParams[num26].initialAmountIndex];
		}
		Kube.GPS.newMapPrice = (float)this.TryConvertToInt32(array12[195], 0);
		if (this.TryConvertToInt32(array12[196], 0) == 1)
		{
			Kube.SS.sendStat = true;
		}
		else
		{
			Kube.SS.sendStat = false;
		}
		string[] array33 = array12[197].Split(separator);
		Kube.GPS.bonusesPrice = new int[array33.Length, 2];
		for (int num29 = 0; num29 < array33.Length; num29++)
		{
			string[] array34 = array33[num29].Split(separator2);
			Kube.GPS.bonusesPrice[num29, 0] = this.TryConvertToInt32(array34[0], 0);
			Kube.GPS.bonusesPrice[num29, 1] = this.TryConvertToInt32(array34[1], 0);
		}
		string[] array35 = array12[198].Split(separator);
		for (int num30 = 0; num30 < array35.Length; num30++)
		{
			string[] array36 = array35[num30].Split(separator2);
			Kube.GPS.vipPrice[num30, 0] = this.TryConvertToInt32(array36[0], 0);
			Kube.GPS.vipPrice[num30, 1] = this.TryConvertToInt32(array36[1], 0);
		}
		Kube.GPS.vipBonus = this.TryConvertToInt32(array12[199], 0);
		for (int num31 = 0; num31 < Kube.GPS.inventarSpecItems.Length; num31++)
		{
			string[] array37 = array12[num31 + 200].Split(separator);
			if (array37.Length >= 7)
			{
				Kube.GPS.specItemsPrice1[num31, 2] = this.TryConvertToInt32(array37[1], 0);
				Kube.GPS.specItemsPrice2[num31, 2] = this.TryConvertToInt32(array37[2], 0);
				Kube.GPS.specItemsPrice1[num31, 1] = this.TryConvertToInt32(array37[3], 0);
				Kube.GPS.specItemsPrice2[num31, 1] = this.TryConvertToInt32(array37[4], 0);
				Kube.GPS.specItemsPrice1[num31, 0] = this.TryConvertToInt32(array37[5], 0);
				Kube.GPS.specItemsPrice2[num31, 0] = this.TryConvertToInt32(array37[6], 0);
			}
		}
		if (this.TryConvertToInt32(array12[220], 0) == 1)
		{
			Kube.SS.sendStatPay = true;
		}
		else
		{
			Kube.SS.sendStatPay = false;
		}
		Kube.GPS.maxAvailableCubes = this.TryConvertToInt32(array12[223], 0);
		string[] array38 = array12[224].Split(separator);
		for (int num32 = 0; num32 < array38.Length; num32++)
		{
			string[] array39 = array38[num32].Split(separator2);
			Kube.GPS.moderContests[num32, 0] = this.TryConvertToInt32(array39[0], 0);
			Kube.GPS.moderContests[num32, 1] = this.TryConvertToInt32(array39[1], 0);
			Kube.GPS.moderContests[num32, 2] = this.TryConvertToInt32(array39[2], 0);
			Kube.GPS.moderContests[num32, 3] = this.TryConvertToInt32(array39[3], 0);
			Kube.GPS.moderContests[num32, 4] = this.TryConvertToInt32(array39[4], 0);
			Kube.GPS.moderContests[num32, 5] = this.TryConvertToInt32(array39[5], 0);
		}
		for (int num33 = 0; num33 < Kube.GPS.clothesPrice.Length; num33++)
		{
			if (225 + num33 < array12.Length)
			{
				if (array12[225 + num33].Length != 0)
				{
					string[] array40 = array12[225 + num33].Split(separator);
					Kube.GPS.clothesPrice[num33, 0] = this.TryConvertToInt32(array40[1], 0);
					Kube.GPS.clothesPrice[num33, 1] = this.TryConvertToInt32(array40[2], 0);
					Kube.GPS.clothesPrice[num33, 2] = this.TryConvertToInt32(array40[3], 0);
					for (int num34 = 4; num34 < array40.Length; num34++)
					{
						string[] array41 = array40[num34].Split(separator3);
						for (int num35 = 0; num35 < 16; num35++)
						{
							if (array41[0] == Kube.GPS.BonusTypeCode[num35])
							{
								Kube.GPS.clothesBonus[num33, num35] = (float)this.TryConvertToDouble(array41[1]);
							}
						}
					}
				}
			}
		}
		if (array12.Length > 291)
		{
			string[] array42 = array12[291].Split(separator);
			for (int num36 = 0; num36 < array42.Length; num36++)
			{
				string[] array43 = array42[num36].Replace("\r\n", "\n").Replace('\r', '\n').Split(separator4);
				for (int num37 = 0; num37 < array43.Length; num37++)
				{
					string[] array44 = array43[num37].Split(separator2);
					for (int num38 = 1; num38 < array44.Length; num38++)
					{
						Kube.GPS.upgradePrice[num36, num37, num38 - 1] = PriceValue.Parse(array44[num38].Trim());
					}
				}
			}
		}
		if (jsonData.Keys.Contains("fi") && jsonData["fi"].IsObject)
		{
			if (jsonData["fi"].Keys.Contains("a") && jsonData["fi"]["a"] != null)
			{
				byte[] array45 = Convert.FromBase64String(jsonData["fi"]["a"].ToString());
				int num39 = 0;
				for (int num40 = 0; num40 < array45.Length; num40 += 2)
				{
					Kube.GPS.fastInventar[num39].Type = ((array45[num40] != byte.MaxValue) ? ((int)array45[num40]) : -1);
					Kube.GPS.fastInventar[num39].Num = (int)array45[num40 + 1];
					num39++;
				}
			}
			if (jsonData["fi"].Keys.Contains("b") && jsonData["fi"]["b"] != null)
			{
				byte[] array46 = Convert.FromBase64String(jsonData["fi"]["b"].ToString());
				int num41 = 0;
				for (int num42 = 0; num42 < array46.Length; num42 += 2)
				{
					FastInventar none = FastInventar.NONE;
					none.Type = ((array46[num42] != byte.MaxValue) ? ((int)array46[num42]) : -1);
					none.Num = (int)array46[num42 + 1];
					if (num41 < 6 && none.Type == 4)
					{
						if (Kube.GPS.inventarWeapons[none.Num] > 0 && Kube.IS.weaponParams[none.Num].weaponGroup == (InventoryScript.WeaponGroup)num41)
						{
							Kube.GPS.fastInventarWeapon[num41] = none;
						}
						else
						{
							none = FastInventar.NONE;
						}
					}
					else if (num41 >= 6 && none.Type == 3)
					{
						if (Kube.GPS.inventarItems[none.Num] > 0)
						{
							Kube.GPS.fastInventarWeapon[num41] = none;
						}
						else
						{
							none = FastInventar.NONE;
						}
					}
					else
					{
						none = FastInventar.NONE;
					}
					Kube.GPS.fastInventarWeapon[num41] = none;
					num41++;
				}
			}
		}
		if (jsonData.Keys.Contains("wpu"))
		{
			JsonData wpu = jsonData["wpu"];
			WeaponUpgrade.Parse(wpu);
		}
		if (this.TryConvertToInt32(jsonData["f"].ToString(), 0) == 1)
		{
			this.askName = true;
			GUI.FocusControl("charName");
			Kube.GPS.needTraining = true;
		}
		else
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
		}
	}

	private void OnGUI()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		GUI.depth = -2;
		if (this.isCheater)
		{
			GUI.skin = Kube.ASS1.mainSkinSmall;
			GUI.Box(new Rect(0.5f * num - 150f, 0.5f * num2 - 100f, 300f, 60f), Localize.hello_chiter);
		}
		else if (this.isBan)
		{
			GUI.skin = Kube.ASS1.mainSkinSmall;
			GUI.Box(new Rect(0.5f * num - 150f, 0.5f * num2 - 100f, 300f, 60f), Localize.ban_cheater);
		}
		else if (this.isError)
		{
			GUI.skin = Kube.ASS1.mainSkinSmall;
			GUI.Box(new Rect(0.5f * num - 150f, 0.5f * num2 - 100f, 300f, 60f), Localize.server_error);
		}
		else if (this.isPending)
		{
			GUI.skin = Kube.ASS1.yellowButton;
			GUI.Box(new Rect(0.5f * num - 150f, num2 - 100f, 300f, 60f), Localize.loading_data);
			return;
		}
		if (this.askName)
		{
			GUI.Box(new Rect(0.5f * num - 149f, 0.5f * num2 - 99f, 298f, 147f), string.Empty);
			GUI.Box(new Rect(0.5f * num - 149f, 0.5f * num2 - 99f, 298f, 147f), string.Empty);
			GUI.Box(new Rect(0.5f * num - 149f, 0.5f * num2 - 99f, 298f, 147f), string.Empty);
			GUI.skin = Kube.ASS1.buttonArrowSkin;
			GUI.Box(new Rect(0.5f * num - 150f, 0.5f * num2 - 100f, 300f, 150f), Localize.init_nickname);
			GUI.SetNextControlName("charName");
			this.charName = GUI.TextField(new Rect(0.5f * num - 100f, 0.5f * num2 - 60f, 200f, 35f), this.charName, 32);
			Event current = Event.current;
			if (current.isKey && current.keyCode == KeyCode.Return)
			{
				if (this.charName.Length >= 3)
				{
					Kube.GPS.playerName = AuxFunc.CodeRussianName(this.charName);
					Kube.SS.SaveNewName(Kube.GPS.playerId, Kube.GPS.playerName);
					UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
				}
				else
				{
					Kube.GPS.printMessage(Localize.short_name, Color.white);
				}
			}
			if (!this.nameFocused)
			{
				GUI.FocusControl("charName");
				this.nameFocused = true;
			}
			char[] separator = new char[]
			{
				'^',
				':',
				'_',
				'%',
				'?',
				'@',
				'/',
				'\\',
				';',
				'*',
				'"',
				'|',
				' '
			};
			string[] array = this.charName.Split(separator);
			this.charName = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				if (i != 0)
				{
					this.charName += string.Empty;
				}
				this.charName += array[i];
			}
			GUI.skin = Kube.ASS1.mainSkinSmall;
			if (GUI.Button(new Rect(0.5f * num - 50f, 0.5f * num2 - 10f, 100f, 40f), "ОК"))
			{
				if (this.charName.Length >= 3)
				{
					Kube.GPS.playerName = AuxFunc.CodeRussianName(this.charName);
					Kube.SS.SaveNewName(Kube.GPS.playerId, Kube.GPS.playerName);
					UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
				}
				else
				{
					Kube.GPS.printMessage(Localize.short_name, Color.white);
				}
			}
		}
	}

	private void OnConnectedToPhoton()
	{
		Kube.GPS.printLog("Connected To Photon");
		PhotonNetwork.networkingPeer.WarningSize = 500;
		PhotonNetwork.networkingPeer.LimitOfUnreliableCommands = 500;
		this.InitSocialNet();
	}

	private void OnFailedToConnectToPhoton()
	{
		PhotonNetwork.offlineMode = true;
		Kube.GPS.printLog("Not connected To Photon");
		this.InitSocialNet();
	}

	private void OnDisconnectedFromPhoton()
	{
		Kube.GPS.printLog("Disconnected From Photon");
		PhotonNetwork.offlineMode = true;
	}

	private void Update()
	{
	}

	private bool isCheater;

	private bool isError;

	private bool isBan;

	protected bool isPending = true;

	private bool askName;

	private string charName = string.Empty;

	private bool nameFocused;
}
