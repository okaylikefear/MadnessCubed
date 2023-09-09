using System;
using System.Collections;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class LoadParamsScript : MonoBehaviour
{
	private void Start()
	{
		UnityEngine.Debug.Log("@Start::1");
		Application.runInBackground = true;
		UnityEngine.Debug.Log("@Start::2");
		this.InitPlatform();
		MusicManagerScript component = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManagerScript>();
		AudioListener.volume = PlayerPrefs.GetFloat("soundVol", 1f);
		component.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("musicVol", 1f);
		Kube.GPS.mouseSens = PlayerPrefs.GetFloat("mouseSens", 4f);
		Kube.OH.autoaim = (PlayerPrefs.GetInt("autoaim", (!Kube.OH.autoaim) ? 0 : 1) > 0);
		Kube.GPS.mouseSens = PlayerPrefs.GetFloat("mouseSens", 4f);
		if (Screen.resolutions != null && Screen.resolutions.Length > 0)
		{
			int num = PlayerPrefs.GetInt("screen", 1);
			num = Math.Min(num, Screen.resolutions.Length - 1);
			if (Screen.resolutions.Length > num)
			{
				Kube.OH.screenResolution = Screen.resolutions[num];
			}
		}
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
		Kube.SS.LoadPlayersParams(new YieldCallback(this.ParamsLoaded));
	}

	private int[] decodeJsonIntArray(JsonData par1)
	{
		int count = par1.Count;
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = TryConvert.ToInt32(par1[i].ToString(), 0);
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

	private IEnumerator ParamsLoaded(JsonData data)
	{
		char[] dc2 = new char[]
		{
			';'
		};
		char[] dc3 = new char[]
		{
			':'
		};
		char[] dc4 = new char[]
		{
			'='
		};
		char[] dc5 = new char[]
		{
			'\n'
		};
		if (data.Keys.Contains("offer"))
		{
			OfferBox.init(data["offer"]);
		}
		yield return 1;
		if (data.Keys.Contains("sq"))
		{
			Kube.SN.missionsFromServer(data["sq"]);
		}
		yield return 1;
		if (data.Keys.Contains("bp") && data["bp"] != null)
		{
			Kube.GPS.parts = this.decodeJsonIntArray(data["bp"]);
		}
		JsonData playerData = data["r"];
		if (playerData.Count <= 1)
		{
			this.isCheater = true;
			Kube.SS.SendStat("startcheat");
			yield break;
		}
		string[] strs = Kube.SS.DecodePlayerData(playerData);
		this.isPending = false;
		Kube.GPS.Init();
		int currentServerTime = TryConvert.ToInt32(data["t"].ToString(), 0);
		Kube.GPS.dayNum = Mathf.FloorToInt((float)((double)currentServerTime / 86400.0));
		MonoBehaviour.print(currentServerTime + " " + Kube.GPS.dayNum);
		Kube.SS.serverTime = (float)currentServerTime;
		Kube.GPS.bonusDay = TryConvert.ToInt32(data["d"].ToString(), 0) % 10;
		Kube.GPS.playerName = strs[3];
		Kube.GPS.playerNumMaps = TryConvert.ToInt32(strs[4], 0);
		string[] cubesStrs = strs[5].Split(dc2);
		for (int i = 0; i < cubesStrs.Length; i++)
		{
			int cubesTimeOfEnd = TryConvert.ToInt32(cubesStrs[i], 0);
			if (cubesTimeOfEnd == 1)
			{
				cubesTimeOfEnd = 10000000;
			}
			else if (cubesTimeOfEnd > currentServerTime)
			{
				cubesTimeOfEnd = (int)Time.time + (cubesTimeOfEnd - currentServerTime);
			}
			Kube.GPS.cubesTimeOfEnd[i] = cubesTimeOfEnd;
		}
		Kube.GPS.cubesTimeOfEnd[0] = (int)Time.time + 10000000;
		yield return 1;
		string[] itemStrs = strs[6].Split(dc2);
		int j = 0;
		while (j < itemStrs.Length && j < Kube.GPS.inventarItems.Length)
		{
			Kube.GPS.inventarItems[j] = TryConvert.ToInt32(itemStrs[j], 0);
			j++;
		}
		yield return 1;
		Kube.GPS.playerMoney1 = TryConvert.ToInt32(strs[7], 0);
		Kube.GPS.playerMoney2 = TryConvert.ToInt32(strs[8], 0);
		yield return 1;
		string[] weaponsStrs = strs[9].Split(dc2);
		int NNweapons = Mathf.Min(weaponsStrs.Length, Kube.GPS.inventarWeapons.Length);
		for (int k = 0; k < NNweapons; k++)
		{
			uint _inventarWeapons = TryConvert.ToUInt32(weaponsStrs[k], 0u);
			if (_inventarWeapons == 1u)
			{
				Kube.GPS.inventarWeapons[k] = (int)Time.time + 10000000;
			}
			else if ((ulong)_inventarWeapons > (ulong)((long)currentServerTime))
			{
				Kube.GPS.inventarWeapons[k] = (int)(Time.time + (uint)((ulong)_inventarWeapons - (ulong)((long)currentServerTime)));
			}
		}
		yield return 1;
		string[] paramsStrs = strs[10].Split(dc2);
		Kube.GPS.playerHealth = TryConvert.ToInt32(paramsStrs[0], 0);
		Kube.GPS.playerArmor = TryConvert.ToInt32(paramsStrs[1], 0);
		Kube.GPS.playerSpeed = TryConvert.ToInt32(paramsStrs[2], 0);
		Kube.GPS.playerJump = TryConvert.ToInt32(paramsStrs[3], 0);
		Kube.GPS.playerDefend = TryConvert.ToInt32(paramsStrs[4], 0);
		Kube.GPS.playerExp = TryConvert.ToUInt32(strs[12], 0u);
		Kube.GPS.playerFrags = TryConvert.ToInt32(strs[13], 0);
		Kube.GPS.playerLevel = TryConvert.ToInt32(strs[14], 0);
		yield return 1;
		string[] skinsStrs = strs[15].Split(dc2);
		int skinsNN = Math.Min(skinsStrs.Length, Kube.GPS.playerSkins.Length);
		for (int l = 0; l < skinsNN; l++)
		{
			Kube.GPS.playerSkins[l] = TryConvert.ToInt32(skinsStrs[l], 0);
		}
		yield return 1;
		string[] bulletsStrs = strs[16].Split(dc2);
		int buletsNN = Math.Min(bulletsStrs.Length, Kube.IS.bulletParams.Length);
		for (int m = 0; m < buletsNN; m++)
		{
			string[] bulletsStrs2 = bulletsStrs[m].Split(dc3);
			Kube.IS.bulletParams[m].initialAmount = Mathf.Max(Kube.IS.bulletParams[m].initialAmount, TryConvert.ToInt32(bulletsStrs2[0], 0));
		}
		yield return 1;
		Kube.GPS.playerVoices = TryConvert.ToInt32(strs[18], 0);
		if (strs[19] == "1")
		{
			Kube.GPS.vipEnd = Time.time + 6.048E+08f;
		}
		else
		{
			Kube.GPS.vipEnd = Time.time + (float)(TryConvert.ToInt32(strs[19], 0) - currentServerTime);
		}
		Kube.GPS.playerSkin = TryConvert.ToInt32(strs[20], 0);
		string[] specItemsStrs = strs[21].Split(dc2);
		int n = 0;
		while (n < specItemsStrs.Length && n < Kube.GPS.inventarSpecItems.Length)
		{
			uint specTime = TryConvert.ToUInt32(specItemsStrs[n], 0u);
			if (specTime == 1u)
			{
				Kube.GPS.inventarSpecItems[n] = (int)Time.time + 10000;
			}
			else if ((ulong)specTime > (ulong)((long)currentServerTime))
			{
				Kube.GPS.inventarSpecItems[n] = (int)Time.time + (int)((ulong)specTime - (ulong)((long)currentServerTime));
			}
			n++;
		}
		yield return 1;
		Kube.GPS.nextUnbox = DataUtils.UnitToDatetime(TryConvert.ToInt32(strs[22], 0));
		if (Kube.GPS.isVIP || Kube.SN.isVIPPlatform)
		{
			for (int k2 = 0; k2 < Kube.IS.specItemDesc.Length; k2++)
			{
				if (Kube.IS.specItemDesc[k2].page == InventoryScript.ItemPage.Forms)
				{
					Kube.GPS.inventarSpecItems[k2] = (int)Time.time + 5000;
				}
			}
		}
		Kube.GPS.moderType = TryConvert.ToInt32(strs[23], 0);
		string[] playerClothesStrs = strs[25].Split(dc2);
		int k3 = 0;
		while (k3 < playerClothesStrs.Length && k3 < Kube.GPS.playerIsClothes.Length)
		{
			Kube.GPS.playerIsClothes[k3] = TryConvert.ToInt32(playerClothesStrs[k3], 0);
			k3++;
		}
		string[] playerCurClothesStrs = strs[26].Split(dc2);
		int k4 = 0;
		while (k4 < playerCurClothesStrs.Length && k4 < Kube.GPS.playerClothes.Length)
		{
			if (playerCurClothesStrs[k4].Length != 0)
			{
				Kube.GPS.playerClothes[k4] = TryConvert.ToInt32(playerCurClothesStrs[k4], -1);
			}
			k4++;
		}
		yield return 1;
		JsonData priceData = data["price"];
		int nnitems = 120;
		int nnclothes = 66;
		int nnweapons = 50;
		int nnspecitems = 22;
		string[] priceStrs = new string[priceData.Count];
		for (int i2 = 0; i2 < priceData.Count; i2++)
		{
			priceStrs[i2] = priceData[i2].ToString();
		}
		if (priceStrs.Length > 292)
		{
			string[] sizes = priceStrs[priceData.Count - 1].Split(dc2);
			nnweapons = TryConvert.ToInt32(sizes[0], 0);
			if (nnweapons <= 0)
			{
				nnweapons = 50;
			}
			nnitems = TryConvert.ToInt32(sizes[1], 0);
			nnspecitems = TryConvert.ToInt32(sizes[2], 0);
			if (nnspecitems <= 0)
			{
				nnspecitems = 22;
			}
			nnclothes = TryConvert.ToInt32(sizes[4], 0);
			int poffset = 1;
			int num;
			for (int k5 = 0; k5 < 5; k5++)
			{
				string[] array = priceStrs;
				poffset = (num = poffset) + 1;
				string[] priceStrs2 = array[num].Split(dc2);
			}
			for (int k6 = 0; k6 < 6; k6++)
			{
				string[] array2 = priceStrs;
				poffset = (num = poffset) + 1;
				string[] priceStrs3 = array2[num].Split(dc2);
				Kube.GPS.exchangeSpec[k6, 0] = TryConvert.ToFloat(priceStrs3[0], 0f);
				Kube.GPS.exchangeSpec[k6, 1] = TryConvert.ToFloat(priceStrs3[1], 0f);
				Kube.GPS.exchangeSpec[k6, 2] = TryConvert.ToFloat(priceStrs3[2], 0f);
				Kube.GPS.exchangeSpec[k6, 3] = TryConvert.ToFloat(priceStrs3[1], 0f) + (float)TryConvert.ToInt32(priceStrs3[2], 0);
			}
			yield return 1;
			GameParamsScript gps = Kube.GPS;
			string[] array3 = priceStrs;
			poffset = (num = poffset) + 1;
			gps.specToMoney = TryConvert.ToInt32(array3[num], 0);
			string[] array4 = priceStrs;
			poffset = (num = poffset) + 1;
			string[] hatStrs = array4[num].Split(dc2);
			for (int k7 = 0; k7 < hatStrs.Length; k7++)
			{
				string[] hatStrs2 = hatStrs[k7].Split(dc3);
				Kube.GPS.hatPrice[k7, 0] = TryConvert.ToInt32(hatStrs2[1], 0);
				Kube.GPS.hatPrice[k7, 1] = TryConvert.ToInt32(hatStrs2[2], 0);
				Kube.GPS.hatPrice[k7, 2] = TryConvert.ToInt32(hatStrs2[3], 0);
				for (int q = 4; q < hatStrs2.Length; q++)
				{
					string[] hatStrs3 = hatStrs2[q].Split(dc4);
					for (int i3 = 0; i3 < 16; i3++)
					{
						if (hatStrs3[0] == Kube.GPS.BonusTypeCode[i3])
						{
							Kube.GPS.hatBonus[k7, i3] = (float)TryConvert.ToDouble(hatStrs3[1]);
						}
					}
				}
			}
			yield return 1;
			string[] array5 = priceStrs;
			poffset = (num = poffset) + 1;
			string[] skinStrs = array5[num].Split(dc2);
			for (int k8 = 0; k8 < skinStrs.Length; k8++)
			{
				string[] skinStrs2 = skinStrs[k8].Split(dc3);
				Kube.GPS.skinPrice[k8, 0] = TryConvert.ToInt32(skinStrs2[1], 0);
				Kube.GPS.skinPrice[k8, 1] = TryConvert.ToInt32(skinStrs2[2], 0);
				Kube.GPS.skinPrice[k8, 2] = TryConvert.ToInt32(skinStrs2[3], 0);
				for (int q2 = 4; q2 < skinStrs2.Length; q2++)
				{
					string[] skinStrs3 = skinStrs2[q2].Split(dc4);
					for (int i4 = 0; i4 < 16; i4++)
					{
						if (skinStrs3[0] == Kube.GPS.BonusTypeCode[i4])
						{
							Kube.GPS.skinBonus[k8, i4] = (float)TryConvert.ToDouble(skinStrs3[1]);
						}
					}
				}
			}
			string[] array6 = priceStrs;
			poffset = (num = poffset) + 1;
			string[] ammunitionStrs = array6[num].Split(dc2);
			for (int k9 = 0; k9 < ammunitionStrs.Length; k9++)
			{
				string[] ammunitionStrs2 = ammunitionStrs[k9].Split(dc3);
				Kube.GPS.ammunitionPrice[k9, 0] = TryConvert.ToInt32(ammunitionStrs2[1], 0);
				Kube.GPS.ammunitionPrice[k9, 1] = TryConvert.ToInt32(ammunitionStrs2[2], 0);
				Kube.GPS.ammunitionPrice[k9, 2] = TryConvert.ToInt32(ammunitionStrs2[3], 0);
				for (int q3 = 4; q3 < ammunitionStrs2.Length; q3++)
				{
					string[] ammunitionStrs3 = ammunitionStrs2[q3].Split(dc4);
					for (int i5 = 0; i5 < 16; i5++)
					{
						if (ammunitionStrs3[0] == Kube.GPS.BonusTypeCode[i5])
						{
							Kube.GPS.ammunitionBonus[k9, i5] = (float)TryConvert.ToDouble(ammunitionStrs3[1]);
						}
					}
				}
			}
			string[] array7 = priceStrs;
			poffset = (num = poffset) + 1;
			string charParamsFix = array7[num].Replace('*', ':');
			string[] paramsStrs2 = charParamsFix.Split(dc3);
			for (int k10 = 0; k10 < 5; k10++)
			{
				for (int q4 = 0; q4 < 8; q4++)
				{
					string[] paramsStrs3 = paramsStrs2[k10 * 9 + q4 + 1].Split(dc2);
					Kube.GPS.charParamsPrice[k10, q4, 0] = (float)TryConvert.ToDouble(paramsStrs3[0]);
					Kube.GPS.charParamsPrice[k10, q4, 1] = (float)TryConvert.ToDouble(paramsStrs3[1]);
					Kube.GPS.charParamsPrice[k10, q4, 2] = (float)TryConvert.ToDouble(paramsStrs3[2]);
					Kube.GPS.charParamsPrice[k10, q4, 3] = (float)TryConvert.ToDouble(paramsStrs3[3]);
					Kube.GPS.charParamsPrice[k10, q4, 4] = (float)TryConvert.ToDouble(paramsStrs3[4]);
				}
			}
			string[] array8 = priceStrs;
			poffset = (num = poffset) + 1;
			string[] paramsStrsBase = array8[num].Split(dc2);
			Kube.GPS.playerBaseHealth = TryConvert.ToInt32(paramsStrsBase[0], 0);
			Kube.GPS.playerBaseArmor = TryConvert.ToInt32(paramsStrsBase[1], 0);
			Kube.GPS.playerBaseSpeed = TryConvert.ToInt32(paramsStrsBase[2], 0);
			Kube.GPS.playerBaseJump = TryConvert.ToInt32(paramsStrsBase[3], 0);
			Kube.GPS.playerBaseDefend = TryConvert.ToInt32(paramsStrsBase[4], 0);
			string[] array9 = priceStrs;
			poffset = (num = poffset) + 1;
			string[] bulletsPrice = array9[num].Split(dc2);
			yield return 1;
			for (int k11 = 0; k11 < bulletsPrice.Length; k11++)
			{
				string[] bulletsPrice2 = bulletsPrice[k11].Split(dc3);
				for (int q5 = 0; q5 < bulletsPrice2.Length - 1; q5++)
				{
					string[] bulletsPrice3 = bulletsPrice2[q5 + 1].Split(dc4);
					Kube.GPS.bulletsPrice[k11, q5, 0] = TryConvert.ToInt32(bulletsPrice3[0], 0);
					Kube.GPS.bulletsPrice[k11, q5, 1] = TryConvert.ToInt32(bulletsPrice3[1], 0);
					Kube.GPS.bulletsPrice[k11, q5, 2] = TryConvert.ToInt32(bulletsPrice3[2], 0);
					Kube.GPS.bulletsPrice[k11, q5, 3] = TryConvert.ToInt32(bulletsPrice3[3], 0);
					Kube.GPS.bulletsPrice[k11, q5, 4] = TryConvert.ToInt32(bulletsPrice3[4], 0);
				}
			}
			yield return 1;
			for (int k12 = 0; k12 < Kube.IS.bulletParams.Length; k12++)
			{
				for (int q6 = 0; q6 < Kube.GPS.bulletsPrice.GetLength(1); q6++)
				{
					Kube.IS.bulletParams[k12].initialAmountArray[q6 + 1] = Kube.GPS.bulletsPrice[k12, q6, 3];
				}
				for (int q7 = 0; q7 < Kube.IS.bulletParams[k12].initialAmountArray.Length; q7++)
				{
					if (Kube.IS.bulletParams[k12].initialAmountArray[q7] <= Kube.IS.bulletParams[k12].initialAmount)
					{
						Kube.IS.bulletParams[k12].initialAmountIndex = q7;
					}
				}
				Kube.IS.bulletParams[k12].initialAmount = Kube.IS.bulletParams[k12].initialAmountArray[Kube.IS.bulletParams[k12].initialAmountIndex];
			}
			GameParamsScript gps2 = Kube.GPS;
			string[] array10 = priceStrs;
			poffset = (num = poffset) + 1;
			gps2.newMapPrice = (float)TryConvert.ToInt32(array10[num], 0);
			poffset++;
			string[] array11 = priceStrs;
			poffset = (num = poffset) + 1;
			string[] bonusesPrice = array11[num].Split(dc2);
			Kube.GPS.bonusesPrice = new int[bonusesPrice.Length, 2];
			for (int k13 = 0; k13 < bonusesPrice.Length; k13++)
			{
				string[] bonusStrs2 = bonusesPrice[k13].Split(dc3);
				Kube.GPS.bonusesPrice[k13, 0] = TryConvert.ToInt32(bonusStrs2[0], 0);
				Kube.GPS.bonusesPrice[k13, 1] = TryConvert.ToInt32(bonusStrs2[1], 0);
			}
			string[] array12 = priceStrs;
			poffset = (num = poffset) + 1;
			string[] vipPrice = array12[num].Split(dc2);
			for (int k14 = 0; k14 < vipPrice.Length; k14++)
			{
				string[] vipStrs2 = vipPrice[k14].Split(dc3);
				Kube.GPS.vipPrice[k14, 0] = TryConvert.ToInt32(vipStrs2[0], 0);
				Kube.GPS.vipPrice[k14, 1] = TryConvert.ToInt32(vipStrs2[1], 0);
			}
			GameParamsScript gps3 = Kube.GPS;
			string[] array13 = priceStrs;
			poffset = (num = poffset) + 1;
			gps3.vipBonus = TryConvert.ToInt32(array13[num], 0);
			poffset++;
			poffset += 2;
			GameParamsScript gps4 = Kube.GPS;
			string[] array14 = priceStrs;
			poffset = (num = poffset) + 1;
			gps4.maxAvailableCubes = TryConvert.ToInt32(array14[num], 0);
			yield return 1;
			poffset++;
			for (int k15 = 0; k15 < 6; k15++)
			{
				string[] array15 = priceStrs;
				poffset = (num = poffset) + 1;
				string[] priceStrs4 = array15[num].Split(dc2);
				if (priceStrs4.Length > 3)
				{
					Kube.GPS.inventarCubesPrice1[k15, 0] = TryConvert.ToInt32(priceStrs4[0], 0);
					Kube.GPS.inventarCubesPrice1[k15, 1] = TryConvert.ToInt32(priceStrs4[2], 0);
					Kube.GPS.inventarCubesPrice1[k15, 2] = TryConvert.ToInt32(priceStrs4[4], 0);
					Kube.GPS.inventarCubesPrice2[k15, 0] = TryConvert.ToInt32(priceStrs4[1], 0);
					Kube.GPS.inventarCubesPrice2[k15, 1] = TryConvert.ToInt32(priceStrs4[3], 0);
					Kube.GPS.inventarCubesPrice2[k15, 2] = TryConvert.ToInt32(priceStrs4[5], 0);
				}
				else
				{
					Kube.GPS.inventarCubesPrice1[k15, 0] = TryConvert.ToInt32(priceStrs4[0], 0);
					Kube.GPS.inventarCubesPrice1[k15, 1] = TryConvert.ToInt32(priceStrs4[1], 0);
					Kube.GPS.inventarCubesPrice1[k15, 2] = TryConvert.ToInt32(priceStrs4[2], 0);
				}
			}
			yield return 1;
			if (nnitems < Kube.IS.itemDesc.Length)
			{
				Array.Resize<ItemDescObj>(ref Kube.IS.itemDesc, nnitems);
			}
			for (int k16 = 0; k16 < nnitems; k16++)
			{
				string[] array16 = priceStrs;
				poffset = (num = poffset) + 1;
				string itemPrice = array16[num];
				if (itemPrice.Length != 0)
				{
					string[] priceStrs5 = itemPrice.Split(dc2);
					Kube.GPS.inventarItemPrice1[k16] = TryConvert.ToInt32(priceStrs5[1], 0);
					Kube.GPS.inventarItemPrice2[k16] = TryConvert.ToInt32(priceStrs5[2], 0);
				}
			}
			yield return 1;
			for (int k17 = 0; k17 < nnweapons; k17++)
			{
				string[] array17 = priceStrs;
				poffset = (num = poffset) + 1;
				string[] priceStrs6 = array17[num].Split(dc2);
				if (priceStrs6.Length >= 7)
				{
					Kube.GPS.weaponsPrice1[k17, 2] = TryConvert.ToInt32(priceStrs6[1], 0);
					Kube.GPS.weaponsPrice2[k17, 2] = TryConvert.ToInt32(priceStrs6[2], 0);
					Kube.GPS.weaponsPrice1[k17, 1] = TryConvert.ToInt32(priceStrs6[3], 0);
					Kube.GPS.weaponsPrice2[k17, 1] = TryConvert.ToInt32(priceStrs6[4], 0);
					Kube.GPS.weaponsPrice1[k17, 0] = TryConvert.ToInt32(priceStrs6[5], 0);
					Kube.GPS.weaponsPrice2[k17, 0] = TryConvert.ToInt32(priceStrs6[6], 0);
				}
			}
			yield return 1;
			for (int k18 = 0; k18 < nnspecitems; k18++)
			{
				string[] array18 = priceStrs;
				poffset = (num = poffset) + 1;
				string[] priceStrs7 = array18[num].Split(dc2);
				if (priceStrs7.Length >= 7)
				{
					Kube.GPS.specItemsPrice1[k18, 2] = TryConvert.ToInt32(priceStrs7[1], 0);
					Kube.GPS.specItemsPrice2[k18, 2] = TryConvert.ToInt32(priceStrs7[2], 0);
					Kube.GPS.specItemsPrice1[k18, 1] = TryConvert.ToInt32(priceStrs7[3], 0);
					Kube.GPS.specItemsPrice2[k18, 1] = TryConvert.ToInt32(priceStrs7[4], 0);
					Kube.GPS.specItemsPrice1[k18, 0] = TryConvert.ToInt32(priceStrs7[5], 0);
					Kube.GPS.specItemsPrice2[k18, 0] = TryConvert.ToInt32(priceStrs7[6], 0);
				}
			}
			yield return 1;
			for (int k19 = 0; k19 < nnclothes; k19++)
			{
				string[] array19 = priceStrs;
				poffset = (num = poffset) + 1;
				string cprice = array19[num];
				if (cprice.Length != 0)
				{
					string[] clothesStrs = cprice.Split(dc2);
					Kube.GPS.clothesPrice[k19, 0] = TryConvert.ToInt32(clothesStrs[1], 0);
					Kube.GPS.clothesPrice[k19, 1] = TryConvert.ToInt32(clothesStrs[2], 0);
					Kube.GPS.clothesPrice[k19, 2] = TryConvert.ToInt32(clothesStrs[3], 0);
					for (int q8 = 4; q8 < clothesStrs.Length; q8++)
					{
						string[] clothesStrs2 = clothesStrs[q8].Split(dc4);
						for (int i6 = 0; i6 < 16; i6++)
						{
							if (clothesStrs2[0] == Kube.GPS.BonusTypeCode[i6])
							{
								Kube.GPS.clothesBonus[k19, i6] = (float)TryConvert.ToDouble(clothesStrs2[1]);
							}
						}
					}
				}
			}
			yield return 1;
			if (priceStrs.Length > 291)
			{
				string[] array20 = priceStrs;
				poffset = (num = poffset) + 1;
				string[] wpu_prices = array20[num].Split(dc2);
				for (int k20 = 0; k20 < wpu_prices.Length; k20++)
				{
					if (k20 >= Kube.IS.weaponParams.Length)
					{
						break;
					}
					string[] wpu_prices_item = wpu_prices[k20].Replace("\r\n", "\n").Replace('\r', '\n').Split(dc5);
					for (int q9 = 0; q9 < wpu_prices_item.Length; q9++)
					{
						string[] wpu_prices_param = wpu_prices_item[q9].Split(dc3);
						for (int i7 = 1; i7 < wpu_prices_param.Length; i7++)
						{
							Kube.GPS.upgradePrice[k20, q9, i7 - 1] = PriceValue.Parse(wpu_prices_param[i7].Trim());
						}
					}
				}
			}
			if (priceStrs.Length > poffset + 1)
			{
				string[] array21 = priceStrs;
				poffset = (num = poffset) + 1;
				string[] wps_prices = array21[num].Split(dc3);
				for (int k21 = 0; k21 < wps_prices.Length; k21++)
				{
					if (k21 >= Kube.IS.weaponSkins.Length)
					{
						break;
					}
					string[] wps_prices_param = wps_prices[k21].Split(dc2);
					Kube.GPS.weaponsSkinPrice1[k21] = TryConvert.ToInt32(wps_prices_param[0], 0);
					Kube.GPS.weaponsSkinPrice2[k21] = TryConvert.ToInt32(wps_prices_param[1], 0);
				}
			}
			yield return 1;
			if (data.Keys.Contains("fi") && data["fi"].IsObject)
			{
				if (data["fi"].Keys.Contains("a") && data["fi"]["a"] != null)
				{
					Kube.GPS.fastInventarCreating = this.DecodeFastInventar(data["fi"]["a"]);
					for (int i8 = 0; i8 < 10; i8++)
					{
						FastInventar fi = Kube.GPS.fastInventarCreating[i8];
						if (fi.Type == 3 || fi.Type == 1)
						{
							if (Kube.GPS.inventarItems[fi.Num] > 0)
							{
								Kube.GPS.fastInventarWeapon[i8] = fi;
							}
							else
							{
								fi = FastInventar.NONE;
							}
						}
						Kube.GPS.fastInventarCreating[i8] = fi;
					}
				}
				if (data["fi"].Keys.Contains("b") && data["fi"]["b"] != null)
				{
					Kube.GPS.fastInventarWeapon = this.DecodeFastInventar(data["fi"]["b"]);
					for (int i9 = 0; i9 < 10; i9++)
					{
						FastInventar fi2 = Kube.GPS.fastInventarWeapon[i9];
						if (i9 < 6 && fi2.Type == 4)
						{
							if (fi2.Num >= Kube.GPS.inventarWeapons.Length)
							{
								fi2 = FastInventar.NONE;
							}
							if (Kube.GPS.inventarWeapons[fi2.Num] > 0 && Kube.IS.weaponParams[fi2.Num].weaponGroup == (InventoryScript.WeaponGroup)i9)
							{
								Kube.GPS.fastInventarWeapon[i9] = fi2;
							}
							else
							{
								fi2 = FastInventar.NONE;
							}
						}
						else if (i9 >= 6 && fi2.Type == 3)
						{
							if (Kube.GPS.inventarItems[fi2.Num] > 0)
							{
								Kube.GPS.fastInventarWeapon[i9] = fi2;
							}
							else
							{
								fi2 = FastInventar.NONE;
							}
						}
						else if (i9 >= 6 && fi2.Type == 7)
						{
							if (Kube.GPS.inventarSpecItems[fi2.Num] > 0)
							{
								Kube.GPS.fastInventarWeapon[i9] = fi2;
							}
							else
							{
								fi2 = FastInventar.NONE;
							}
						}
						else
						{
							fi2 = FastInventar.NONE;
						}
						Kube.GPS.fastInventarWeapon[i9] = fi2;
					}
				}
			}
			yield return 1;
			if (data.Keys.Contains("wpu"))
			{
				JsonData wpu = data["wpu"];
				WeaponUpgrade.Parse(wpu);
				WeaponSkins.Parse(wpu);
			}
			yield return 1;
			if (data.Keys.Contains("clan") && data["clan"] != null)
			{
				Kube.GPS.clan = Clans.parseClan(data["clan"]);
			}
			yield return 1;
			if (data.Keys.Contains("unlock"))
			{
				JsonData unl = data["unlock"];
				ItemUnlock.Parse(unl);
			}
			yield return 1;
			if (data.Keys.Contains("pcs"))
			{
				PackBox.parse(data["pcs"]);
			}
			yield return 1;
			if (data.Keys.Contains("tss"))
			{
				TaskBox.parse(data["tss"]);
				Kube.OH.tasks = TaskBox.selectTasks();
			}
			if (TryConvert.ToInt32(data["f"].ToString(), 0) == 1)
			{
				Kube.GPS.needTraining = true;
			}
			yield return 1;
			Kube.SN.InitPayments();
			UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
			yield break;
		}
		UnityEngine.Debug.Log("Bad price array");
		this.Error();
		yield break;
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

	private bool isCheater;

	private bool isError;

	private bool isBan;

	protected bool isPending = true;
}
