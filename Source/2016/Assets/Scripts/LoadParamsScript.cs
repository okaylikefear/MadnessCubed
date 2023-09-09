using System;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class LoadParamsScript : MonoBehaviour
{
	private void Start()
	{
		Application.runInBackground = true;
		this.InitPlatform();
		MusicManagerScript component = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManagerScript>();
		AudioListener.volume = PlayerPrefs.GetFloat("soundVol", 1f);
		component.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("musicVol", 1f);
		Kube.GPS.mouseSens = PlayerPrefs.GetFloat("mouseSens", 4f);
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

	private void InitPlatform()
	{
		Kube.SN.Init(base.gameObject, "LoadDataFromNetwork");
	}

	private void LoadDataFromNetwork()
	{
		Kube.GPS.user = Kube.SN.playerUID;
		GameObject.FindGameObjectWithTag("Music").SendMessage("ChangeMusic", 0, SendMessageOptions.DontRequireReceiver);
		Kube.SS.LoadPlayersParams(base.gameObject, "ParamsLoaded");
	}

	private uint TryConvertToUInt32(string val, uint def = 0u)
	{
		uint result;
		try
		{
			result = Convert.ToUInt32(val);
		}
		catch
		{
			result = def;
		}
		return result;
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

	private FastInventar[] DecodeFastInventar(JsonData jsonData)
	{
		string text = jsonData.ToString();
		FastInventar[] array = new FastInventar[12];
		if (string.IsNullOrEmpty(text))
		{
			return array;
		}
		if (text[0] == '+')
		{
			for (int i = 0; i < 10; i++)
			{
				array[i].Type = Kube.OH.DecodeServerCode(text.Substring(2 + i * 3, 1));
				if (array[i].Type > 7)
				{
					array[i].Type = -1;
				}
				array[i].Num = Kube.OH.DecodeServerCode(text.Substring(2 + i * 3 + 1, 2));
			}
		}
		else
		{
			byte[] array2 = Convert.FromBase64String(text);
			int num = 0;
			for (int j = 0; j < array2.Length; j += 2)
			{
				array[num].Type = ((array2[j] != byte.MaxValue) ? ((int)array2[j]) : -1);
				array[num].Num = (int)array2[j + 1];
				num++;
			}
		}
		return array;
	}

	private void ParamsLoaded(JsonData data)
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
		if (data.Keys.Contains("offer"))
		{
			OfferBox.init(data["offer"]);
		}
		if (data.Keys.Contains("sq"))
		{
			Kube.SN.missionsFromServer(data["sq"]);
		}
		if (data.Keys.Contains("bp") && data["bp"] != null)
		{
			Kube.GPS.parts = this.decodeJsonIntArray(data["bp"]);
		}
		JsonData jsonData = data["r"];
		if (jsonData.Count <= 1)
		{
			this.isCheater = true;
			Kube.SS.SendStat("startcheat");
			return;
		}
		string[] array = Kube.SS.DecodePlayerData(jsonData);
		this.isPending = false;
		Kube.GPS.Init();
		int num = this.TryConvertToInt32(data["t"].ToString(), 0);
		Kube.GPS.dayNum = Mathf.FloorToInt((float)((double)num / 86400.0));
		MonoBehaviour.print(num + " " + Kube.GPS.dayNum);
		Kube.SS.serverTime = (float)num;
		Kube.GPS.bonusDay = this.TryConvertToInt32(data["d"].ToString(), 0) % 10;
		Kube.GPS.playerName = array[3];
		Kube.GPS.playerNumMaps = this.TryConvertToInt32(array[4], 0);
		string[] array2 = array[5].Split(separator);
		for (int i = 0; i < array2.Length; i++)
		{
			Kube.GPS.cubesTimeOfEnd[i] = (float)this.TryConvertToInt32(array2[i], 0);
			if (Kube.GPS.cubesTimeOfEnd[i] == 1f)
			{
				Kube.GPS.cubesTimeOfEnd[i] = 1E+07f;
			}
			else
			{
				Kube.GPS.cubesTimeOfEnd[i] = Time.time + (Kube.GPS.cubesTimeOfEnd[i] - (float)num);
			}
		}
		Kube.GPS.cubesTimeOfEnd[0] = Time.time + 1E+07f;
		string[] array3 = array[6].Split(separator);
		int num2 = 0;
		while (num2 < array3.Length && num2 < Kube.GPS.inventarItems.Length)
		{
			Kube.GPS.inventarItems[num2] = this.TryConvertToInt32(array3[num2], 0);
			num2++;
		}
		Kube.GPS.playerMoney1 = this.TryConvertToInt32(array[7], 0);
		Kube.GPS.playerMoney2 = this.TryConvertToInt32(array[8], 0);
		string[] array4 = array[9].Split(separator);
		for (int j = 0; j < array4.Length; j++)
		{
			uint num3 = this.TryConvertToUInt32(array4[j], 0u);
			if (num3 == 1u)
			{
				Kube.GPS.inventarWeapons[j] = (int)Time.time + 10000000;
			}
			else
			{
				Kube.GPS.inventarWeapons[j] = (int)(Time.time + (uint)((ulong)num3 - (ulong)((long)num)));
			}
		}
		string[] array5 = array[10].Split(separator);
		Kube.GPS.playerHealth = this.TryConvertToInt32(array5[0], 0);
		Kube.GPS.playerArmor = this.TryConvertToInt32(array5[1], 0);
		Kube.GPS.playerSpeed = this.TryConvertToInt32(array5[2], 0);
		Kube.GPS.playerJump = this.TryConvertToInt32(array5[3], 0);
		Kube.GPS.playerDefend = this.TryConvertToInt32(array5[4], 0);
		Kube.GPS.playerExpPoints = this.TryConvertToInt32(array[11], 0);
		Kube.GPS.playerExp = this.TryConvertToUInt32(array[12], 0u);
		Kube.GPS.playerFrags = this.TryConvertToInt32(array[13], 0);
		Kube.GPS.playerLevel = this.TryConvertToInt32(array[14], 0);
		string[] array6 = array[15].Split(separator);
		for (int k = 0; k < array6.Length; k++)
		{
			Kube.GPS.playerSkins[k] = this.TryConvertToInt32(array6[k], 0);
		}
		string[] array7 = array[16].Split(separator);
		int num4 = Math.Min(array7.Length, Kube.IS.bulletParams.Length);
		for (int l = 0; l < num4; l++)
		{
			string[] array8 = array7[l].Split(separator2);
			Kube.IS.bulletParams[l].initialAmount = Mathf.Max(Kube.IS.bulletParams[l].initialAmount, this.TryConvertToInt32(array8[0], 0));
		}
		Kube.GPS.playerVoices = this.TryConvertToInt32(array[18], 0);
		if (array[19] == "1")
		{
			Kube.GPS.vipEnd = Time.time + 604800f;
		}
		else
		{
			Kube.GPS.vipEnd = Time.time + (float)(this.TryConvertToInt32(array[19], 0) - num);
		}
		Kube.GPS.playerSkin = this.TryConvertToInt32(array[20], 0);
		string[] array9 = array[21].Split(separator);
		int num5 = 0;
		while (num5 < array9.Length && num5 < Kube.GPS.inventarSpecItems.Length)
		{
			uint num6 = this.TryConvertToUInt32(array9[num5], 0u);
			if (num6 == 1u)
			{
				Kube.GPS.inventarSpecItems[num5] = (int)Time.time + 10000;
			}
			else
			{
				Kube.GPS.inventarSpecItems[num5] = (int)Time.time + (int)((ulong)num6 - (ulong)((long)num));
			}
			num5++;
		}
		if (Kube.GPS.isVIP || Kube.SN.isVIPPlatform)
		{
			for (int m = 0; m < Kube.IS.specItemDesc.Length; m++)
			{
				if (Kube.IS.specItemDesc[m].page == InventoryScript.ItemPage.Forms)
				{
					Kube.GPS.inventarSpecItems[m] = (int)Time.time + 5000;
				}
			}
		}
		Kube.GPS.moderType = this.TryConvertToInt32(array[23], 0);
		Kube.GPS.moderLastContest = (int)Time.time + (this.TryConvertToInt32(array[24], 0) - num);
		string[] array10 = array[25].Split(separator);
		int num7 = 0;
		while (num7 < array10.Length && num7 < Kube.GPS.playerIsClothes.Length)
		{
			Kube.GPS.playerIsClothes[num7] = this.TryConvertToInt32(array10[num7], 0);
			num7++;
		}
		string[] array11 = array[26].Split(separator);
		int num8 = 0;
		while (num8 < array11.Length && num8 < Kube.GPS.playerClothes.Length)
		{
			if (array11[num8].Length != 0)
			{
				Kube.GPS.playerClothes[num8] = this.TryConvertToInt32(array11[num8], -1);
			}
			num8++;
		}
		JsonData jsonData2 = data["price"];
		string[] array12 = new string[jsonData2.Count];
		for (int n = 0; n < jsonData2.Count; n++)
		{
			array12[n] = jsonData2[n].ToString();
		}
		if (array12.Length > 292)
		{
			string[] array13 = array12[jsonData2.Count - 1].Split(separator);
			int num9 = this.TryConvertToInt32(array13[0], 0);
			if (num9 <= 0)
			{
				num9 = 50;
			}
			int num10 = this.TryConvertToInt32(array13[1], 0);
			int num11 = this.TryConvertToInt32(array13[4], 0);
			int num12 = 1;
			for (int num13 = 0; num13 < 5; num13++)
			{
				string[] array14 = array12[num12++].Split(separator);
			}
			for (int num14 = 0; num14 < 6; num14++)
			{
				string[] array15 = array12[num12++].Split(separator);
				Kube.GPS.exchangeSpec[num14, 0] = this.TryConvertToFloat(array15[0], 0f);
				Kube.GPS.exchangeSpec[num14, 1] = this.TryConvertToFloat(array15[1], 0f);
				Kube.GPS.exchangeSpec[num14, 2] = this.TryConvertToFloat(array15[2], 0f);
				Kube.GPS.exchangeSpec[num14, 3] = this.TryConvertToFloat(array15[1], 0f) + (float)this.TryConvertToInt32(array15[2], 0);
			}
			Kube.GPS.specToMoney = this.TryConvertToInt32(array12[num12++], 0);
			string[] array16 = array12[num12++].Split(separator);
			for (int num15 = 0; num15 < array16.Length; num15++)
			{
				string[] array17 = array16[num15].Split(separator2);
				Kube.GPS.hatPrice[num15, 0] = this.TryConvertToInt32(array17[1], 0);
				Kube.GPS.hatPrice[num15, 1] = this.TryConvertToInt32(array17[2], 0);
				Kube.GPS.hatPrice[num15, 2] = this.TryConvertToInt32(array17[3], 0);
				for (int num16 = 4; num16 < array17.Length; num16++)
				{
					string[] array18 = array17[num16].Split(separator3);
					for (int num17 = 0; num17 < 16; num17++)
					{
						if (array18[0] == Kube.GPS.BonusTypeCode[num17])
						{
							Kube.GPS.hatBonus[num15, num17] = (float)this.TryConvertToDouble(array18[1]);
						}
					}
				}
			}
			string[] array19 = array12[num12++].Split(separator);
			for (int num18 = 0; num18 < array19.Length; num18++)
			{
				string[] array20 = array19[num18].Split(separator2);
				Kube.GPS.skinPrice[num18, 0] = this.TryConvertToInt32(array20[1], 0);
				Kube.GPS.skinPrice[num18, 1] = this.TryConvertToInt32(array20[2], 0);
				Kube.GPS.skinPrice[num18, 2] = this.TryConvertToInt32(array20[3], 0);
				for (int num19 = 4; num19 < array20.Length; num19++)
				{
					string[] array21 = array20[num19].Split(separator3);
					for (int num20 = 0; num20 < 16; num20++)
					{
						if (array21[0] == Kube.GPS.BonusTypeCode[num20])
						{
							Kube.GPS.skinBonus[num18, num20] = (float)this.TryConvertToDouble(array21[1]);
						}
					}
				}
			}
			string[] array22 = array12[num12++].Split(separator);
			for (int num21 = 0; num21 < array22.Length; num21++)
			{
				string[] array23 = array22[num21].Split(separator2);
				Kube.GPS.ammunitionPrice[num21, 0] = this.TryConvertToInt32(array23[1], 0);
				Kube.GPS.ammunitionPrice[num21, 1] = this.TryConvertToInt32(array23[2], 0);
				Kube.GPS.ammunitionPrice[num21, 2] = this.TryConvertToInt32(array23[3], 0);
				for (int num22 = 4; num22 < array23.Length; num22++)
				{
					string[] array24 = array23[num22].Split(separator3);
					for (int num23 = 0; num23 < 16; num23++)
					{
						if (array24[0] == Kube.GPS.BonusTypeCode[num23])
						{
							Kube.GPS.ammunitionBonus[num21, num23] = (float)this.TryConvertToDouble(array24[1]);
						}
					}
				}
			}
			string text = array12[num12++].Replace('*', ':');
			string[] array25 = text.Split(separator2);
			for (int num24 = 0; num24 < 5; num24++)
			{
				for (int num25 = 0; num25 < 8; num25++)
				{
					string[] array26 = array25[num24 * 9 + num25 + 1].Split(separator);
					Kube.GPS.charParamsPrice[num24, num25, 0] = (float)this.TryConvertToDouble(array26[0]);
					Kube.GPS.charParamsPrice[num24, num25, 1] = (float)this.TryConvertToDouble(array26[1]);
					Kube.GPS.charParamsPrice[num24, num25, 2] = (float)this.TryConvertToDouble(array26[2]);
					Kube.GPS.charParamsPrice[num24, num25, 3] = (float)this.TryConvertToDouble(array26[3]);
					Kube.GPS.charParamsPrice[num24, num25, 4] = (float)this.TryConvertToDouble(array26[4]);
				}
			}
			string[] array27 = array12[num12++].Split(separator);
			Kube.GPS.playerBaseHealth = this.TryConvertToInt32(array27[0], 0);
			Kube.GPS.playerBaseArmor = this.TryConvertToInt32(array27[1], 0);
			Kube.GPS.playerBaseSpeed = this.TryConvertToInt32(array27[2], 0);
			Kube.GPS.playerBaseJump = this.TryConvertToInt32(array27[3], 0);
			Kube.GPS.playerBaseDefend = this.TryConvertToInt32(array27[4], 0);
			string[] array28 = array12[num12++].Split(separator);
			for (int num26 = 0; num26 < array28.Length; num26++)
			{
				string[] array29 = array28[num26].Split(separator2);
				for (int num27 = 0; num27 < array29.Length - 1; num27++)
				{
					string[] array30 = array29[num27 + 1].Split(separator3);
					Kube.GPS.bulletsPrice[num26, num27, 0] = this.TryConvertToInt32(array30[0], 0);
					Kube.GPS.bulletsPrice[num26, num27, 1] = this.TryConvertToInt32(array30[1], 0);
					Kube.GPS.bulletsPrice[num26, num27, 2] = this.TryConvertToInt32(array30[2], 0);
					Kube.GPS.bulletsPrice[num26, num27, 3] = this.TryConvertToInt32(array30[3], 0);
					Kube.GPS.bulletsPrice[num26, num27, 4] = this.TryConvertToInt32(array30[4], 0);
				}
			}
			for (int num28 = 0; num28 < Kube.IS.bulletParams.Length; num28++)
			{
				for (int num29 = 0; num29 < Kube.GPS.bulletsPrice.GetLength(1); num29++)
				{
					Kube.IS.bulletParams[num28].initialAmountArray[num29 + 1] = Kube.GPS.bulletsPrice[num28, num29, 3];
				}
				for (int num30 = 0; num30 < Kube.IS.bulletParams[num28].initialAmountArray.Length; num30++)
				{
					if (Kube.IS.bulletParams[num28].initialAmountArray[num30] <= Kube.IS.bulletParams[num28].initialAmount)
					{
						Kube.IS.bulletParams[num28].initialAmountIndex = num30;
					}
				}
				Kube.IS.bulletParams[num28].initialAmount = Kube.IS.bulletParams[num28].initialAmountArray[Kube.IS.bulletParams[num28].initialAmountIndex];
			}
			Kube.GPS.newMapPrice = (float)this.TryConvertToInt32(array12[num12++], 0);
			num12++;
			string[] array31 = array12[num12++].Split(separator);
			Kube.GPS.bonusesPrice = new int[array31.Length, 2];
			for (int num31 = 0; num31 < array31.Length; num31++)
			{
				string[] array32 = array31[num31].Split(separator2);
				Kube.GPS.bonusesPrice[num31, 0] = this.TryConvertToInt32(array32[0], 0);
				Kube.GPS.bonusesPrice[num31, 1] = this.TryConvertToInt32(array32[1], 0);
			}
			string[] array33 = array12[num12++].Split(separator);
			for (int num32 = 0; num32 < array33.Length; num32++)
			{
				string[] array34 = array33[num32].Split(separator2);
				Kube.GPS.vipPrice[num32, 0] = this.TryConvertToInt32(array34[0], 0);
				Kube.GPS.vipPrice[num32, 1] = this.TryConvertToInt32(array34[1], 0);
			}
			Kube.GPS.vipBonus = this.TryConvertToInt32(array12[num12++], 0);
			num12++;
			num12 += 2;
			Kube.GPS.maxAvailableCubes = this.TryConvertToInt32(array12[num12++], 0);
			string[] array35 = array12[num12++].Split(separator);
			for (int num33 = 0; num33 < array35.Length; num33++)
			{
				string[] array36 = array35[num33].Split(separator2);
				Kube.GPS.moderContests[num33, 0] = this.TryConvertToInt32(array36[0], 0);
				Kube.GPS.moderContests[num33, 1] = this.TryConvertToInt32(array36[1], 0);
				Kube.GPS.moderContests[num33, 2] = this.TryConvertToInt32(array36[2], 0);
				Kube.GPS.moderContests[num33, 3] = this.TryConvertToInt32(array36[3], 0);
				Kube.GPS.moderContests[num33, 4] = this.TryConvertToInt32(array36[4], 0);
				Kube.GPS.moderContests[num33, 5] = this.TryConvertToInt32(array36[5], 0);
			}
			for (int num34 = 0; num34 < 6; num34++)
			{
				string[] array37 = array12[num12++].Split(separator);
				if (array37.Length > 3)
				{
					Kube.GPS.inventarCubesPrice1[num34, 0] = this.TryConvertToInt32(array37[0], 0);
					Kube.GPS.inventarCubesPrice1[num34, 1] = this.TryConvertToInt32(array37[2], 0);
					Kube.GPS.inventarCubesPrice1[num34, 2] = this.TryConvertToInt32(array37[4], 0);
					Kube.GPS.inventarCubesPrice2[num34, 0] = this.TryConvertToInt32(array37[1], 0);
					Kube.GPS.inventarCubesPrice2[num34, 1] = this.TryConvertToInt32(array37[3], 0);
					Kube.GPS.inventarCubesPrice2[num34, 2] = this.TryConvertToInt32(array37[5], 0);
				}
				else
				{
					Kube.GPS.inventarCubesPrice1[num34, 0] = this.TryConvertToInt32(array37[0], 0);
					Kube.GPS.inventarCubesPrice1[num34, 1] = this.TryConvertToInt32(array37[1], 0);
					Kube.GPS.inventarCubesPrice1[num34, 2] = this.TryConvertToInt32(array37[2], 0);
				}
			}
			if (num10 < Kube.IS.itemDesc.Length)
			{
				Array.Resize<InventoryScript.ItemDesc>(ref Kube.IS.itemDesc, num10);
			}
			for (int num35 = 0; num35 < num10; num35++)
			{
				string text2 = array12[num12++];
				if (text2.Length != 0)
				{
					string[] array38 = text2.Split(separator);
					Kube.GPS.inventarItemPrice1[num35] = this.TryConvertToInt32(array38[1], 0);
					Kube.GPS.inventarItemPrice2[num35] = this.TryConvertToInt32(array38[2], 0);
				}
			}
			for (int num36 = 0; num36 < num9; num36++)
			{
				string[] array39 = array12[num12++].Split(separator);
				if (array39.Length >= 7)
				{
					Kube.GPS.weaponsPrice1[num36, 2] = this.TryConvertToInt32(array39[1], 0);
					Kube.GPS.weaponsPrice2[num36, 2] = this.TryConvertToInt32(array39[2], 0);
					Kube.GPS.weaponsPrice1[num36, 1] = this.TryConvertToInt32(array39[3], 0);
					Kube.GPS.weaponsPrice2[num36, 1] = this.TryConvertToInt32(array39[4], 0);
					Kube.GPS.weaponsPrice1[num36, 0] = this.TryConvertToInt32(array39[5], 0);
					Kube.GPS.weaponsPrice2[num36, 0] = this.TryConvertToInt32(array39[6], 0);
				}
			}
			for (int num37 = 0; num37 < Kube.GPS.inventarSpecItems.Length; num37++)
			{
				string[] array40 = array12[num12++].Split(separator);
				if (array40.Length >= 7)
				{
					Kube.GPS.specItemsPrice1[num37, 2] = this.TryConvertToInt32(array40[1], 0);
					Kube.GPS.specItemsPrice2[num37, 2] = this.TryConvertToInt32(array40[2], 0);
					Kube.GPS.specItemsPrice1[num37, 1] = this.TryConvertToInt32(array40[3], 0);
					Kube.GPS.specItemsPrice2[num37, 1] = this.TryConvertToInt32(array40[4], 0);
					Kube.GPS.specItemsPrice1[num37, 0] = this.TryConvertToInt32(array40[5], 0);
					Kube.GPS.specItemsPrice2[num37, 0] = this.TryConvertToInt32(array40[6], 0);
				}
			}
			for (int num38 = 0; num38 < num11; num38++)
			{
				string text3 = array12[num12++];
				if (text3.Length != 0)
				{
					string[] array41 = text3.Split(separator);
					Kube.GPS.clothesPrice[num38, 0] = this.TryConvertToInt32(array41[1], 0);
					Kube.GPS.clothesPrice[num38, 1] = this.TryConvertToInt32(array41[2], 0);
					Kube.GPS.clothesPrice[num38, 2] = this.TryConvertToInt32(array41[3], 0);
					for (int num39 = 4; num39 < array41.Length; num39++)
					{
						string[] array42 = array41[num39].Split(separator3);
						for (int num40 = 0; num40 < 16; num40++)
						{
							if (array42[0] == Kube.GPS.BonusTypeCode[num40])
							{
								Kube.GPS.clothesBonus[num38, num40] = (float)this.TryConvertToDouble(array42[1]);
							}
						}
					}
				}
			}
			if (array12.Length > 291)
			{
				string[] array43 = array12[num12++].Split(separator);
				for (int num41 = 0; num41 < array43.Length; num41++)
				{
					if (num41 >= Kube.IS.weaponParams.Length)
					{
						break;
					}
					string[] array44 = array43[num41].Replace("\r\n", "\n").Replace('\r', '\n').Split(separator4);
					for (int num42 = 0; num42 < array44.Length; num42++)
					{
						string[] array45 = array44[num42].Split(separator2);
						for (int num43 = 1; num43 < array45.Length; num43++)
						{
							Kube.GPS.upgradePrice[num41, num42, num43 - 1] = PriceValue.Parse(array45[num43].Trim());
						}
					}
				}
			}
			if (array12.Length > num12 + 1)
			{
				string[] array46 = array12[num12++].Split(separator2);
				for (int num44 = 0; num44 < array46.Length; num44++)
				{
					if (num44 >= Kube.IS.weaponSkins.Length)
					{
						break;
					}
					string[] array47 = array46[num44].Split(separator);
					Kube.GPS.weaponsSkinPrice1[num44] = this.TryConvertToInt32(array47[0], 0);
					Kube.GPS.weaponsSkinPrice2[num44] = this.TryConvertToInt32(array47[1], 0);
				}
			}
			if (data.Keys.Contains("fi") && data["fi"].IsObject)
			{
				if (data["fi"].Keys.Contains("a") && data["fi"]["a"] != null)
				{
					Kube.GPS.fastInventar = this.DecodeFastInventar(data["fi"]["a"]);
					for (int num45 = 0; num45 < 10; num45++)
					{
						FastInventar fastInventar = Kube.GPS.fastInventar[num45];
						if (fastInventar.Type == 3 || fastInventar.Type == 1)
						{
							if (Kube.GPS.inventarItems[fastInventar.Num] > 0)
							{
								Kube.GPS.fastInventarWeapon[num45] = fastInventar;
							}
							else
							{
								fastInventar = FastInventar.NONE;
							}
						}
						Kube.GPS.fastInventar[num45] = fastInventar;
					}
				}
				if (data["fi"].Keys.Contains("b") && data["fi"]["b"] != null)
				{
					Kube.GPS.fastInventarWeapon = this.DecodeFastInventar(data["fi"]["b"]);
					for (int num46 = 0; num46 < 10; num46++)
					{
						FastInventar fastInventar2 = Kube.GPS.fastInventarWeapon[num46];
						if (num46 < 6 && fastInventar2.Type == 4)
						{
							if (fastInventar2.Num >= Kube.GPS.inventarWeapons.Length)
							{
								fastInventar2 = FastInventar.NONE;
							}
							if (Kube.GPS.inventarWeapons[fastInventar2.Num] > 0 && Kube.IS.weaponParams[fastInventar2.Num].weaponGroup == (InventoryScript.WeaponGroup)num46)
							{
								Kube.GPS.fastInventarWeapon[num46] = fastInventar2;
							}
							else
							{
								fastInventar2 = FastInventar.NONE;
							}
						}
						else if (num46 >= 6 && fastInventar2.Type == 3)
						{
							if (Kube.GPS.inventarItems[fastInventar2.Num] > 0)
							{
								Kube.GPS.fastInventarWeapon[num46] = fastInventar2;
							}
							else
							{
								fastInventar2 = FastInventar.NONE;
							}
						}
						else
						{
							fastInventar2 = FastInventar.NONE;
						}
						Kube.GPS.fastInventarWeapon[num46] = fastInventar2;
					}
				}
			}
			if (data.Keys.Contains("wpu"))
			{
				JsonData wpu = data["wpu"];
				WeaponUpgrade.Parse(wpu);
				WeaponSkins.Parse(wpu);
			}
			if (data.Keys.Contains("clan") && data["clan"] != null)
			{
				Kube.GPS.clan = Clans.parseClan(data["clan"]);
			}
			if (data.Keys.Contains("unlock"))
			{
				JsonData unl = data["unlock"];
				ItemUnlock.Parse(unl);
			}
			if (data.Keys.Contains("pcs"))
			{
				PackBox.parse(data["pcs"]);
			}
			if (this.TryConvertToInt32(data["f"].ToString(), 0) == 1)
			{
				this.askName = true;
				GUI.FocusControl("charName");
				Kube.GPS.needTraining = true;
			}
			else
			{
				UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
			}
			return;
		}
		UnityEngine.Debug.Log("Bad price array");
		this.Error();
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
					Kube.SS.SaveNewName(Kube.SS.serverId, Kube.GPS.playerName);
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
			if (GUI.Button(new Rect(0.5f * num - 50f, 0.5f * num2 - 10f, 100f, 40f), Localize.ok))
			{
				if (this.charName.Length >= 3)
				{
					Kube.GPS.playerName = AuxFunc.CodeRussianName(this.charName);
					Kube.SS.SaveNewName(Kube.SS.serverId, Kube.GPS.playerName);
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
		this.InitPlatform();
	}

	private void OnFailedToConnectToPhoton()
	{
		PhotonNetwork.offlineMode = true;
		Kube.GPS.printLog("Not connected To Photon");
		this.InitPlatform();
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
