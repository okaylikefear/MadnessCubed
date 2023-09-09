using System;
using System.Collections.Generic;
using kube;
using LitJson;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
	public int[] getListNums(InventoryScript.ItemPage page)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < Kube.IS.itemDesc.Length; i++)
		{
			if (!Kube.IS.itemDesc[i].hidden || Kube.GPS.inventarItems[i] > 0)
			{
				if (Kube.IS.itemDesc[i].page == page)
				{
					list.Add(i);
				}
			}
		}
		return list.ToArray();
	}

	public int[] getSpecListNums(InventoryScript.ItemPage page)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < Kube.IS.specItemDesc.Length; i++)
		{
			if (!Kube.IS.specItemDesc[i].hidden || Kube.GPS.inventarSpecItems[i] > 0)
			{
				if (Kube.IS.specItemDesc[i].page == page)
				{
					list.Add(i);
				}
			}
		}
		return list.ToArray();
	}

	public string tempClothesStr
	{
		get
		{
			string text = string.Empty;
			for (int i = 0; i < this.tempClothes.Length; i++)
			{
				if (text.Length != 0)
				{
					text += ";";
				}
				text = text + string.Empty + this.tempClothes[i];
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
		this.initialized = true;
	}

	private void Awake()
	{
		Kube.IS = this;
		this.gameItemsGO = new InventoryScript.GameItemGOLoader();
	}

	private void OnDestroy()
	{
		Kube.IS = null;
	}

	private void Start()
	{
		this.Init();
		this.cubesNatureNums = this.collectCubes(ObjectsHolderScript.CubeGroup.cubesNature);
		this.cubesBuilderNums = this.collectCubes(ObjectsHolderScript.CubeGroup.cubesBuilder);
		this.cubesDecorNums = this.collectCubes(ObjectsHolderScript.CubeGroup.cubesDecor);
		this.cubesGlassNums = this.collectCubes(ObjectsHolderScript.CubeGroup.cubesGlass);
		this.cubesWaterNums = this.collectCubes(ObjectsHolderScript.CubeGroup.cubesWater);
		this.cubesDifferentNums = this.collectCubes(ObjectsHolderScript.CubeGroup.cubesDifferent);
		this.shopHats = this.collectClothes(InventoryScript.ClothesPages.Hats);
		this.shopTors = this.collectClothes(InventoryScript.ClothesPages.Tors);
		this.shopBack = this.collectClothes(InventoryScript.ClothesPages.Back);
		this.shopArms = this.collectClothes(InventoryScript.ClothesPages.Arms);
		this.shopFoots = this.collectClothes(InventoryScript.ClothesPages.Foots);
		this.shopShoulders = this.collectClothes(InventoryScript.ClothesPages.Shoulders);
		this.renderChar = new RenderTexture(256, 256, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
		this.renderWeapon = new RenderTexture(256, 256, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
		this.inventoryTypeStrs = Localize.is_tabs;
		this.inventoryCubesTypesStrs = Localize.CubesTypes;
		this.inventoryDecorTypesStrs = Localize.DecorTypes;
		this.inventoryItemsTypesStrs = Localize.ItemsTypes;
		this.inventoryWeaponTypesStrs = Localize.WeaponTypes;
		this.inventoryCharacterTypesStrs = Localize.CharacterPages;
		this.inventoryDeviceTypesStrs = Localize.DeviceTypes;
		this.clothesType = Localize.ClothesType;
		Kube.IS = this;
		Kube.GPS.Init();
		this.tempClothes = new int[Kube.GPS.playerIsClothes.Length];
		for (int i = 0; i < Kube.GPS.playerClothes.Length; i++)
		{
			this.tempClothes[i] = Kube.GPS.playerClothes[i];
		}
		this.onAssetsLoaded(0);
	}

	private int[] collectClothes(InventoryScript.ClothesPages p)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.clothesParams.Length; i++)
		{
			if (this.clothesParams[i].group == p)
			{
				list.Add(i);
			}
		}
		return list.ToArray();
	}

	private int[] collectCubes(ObjectsHolderScript.CubeGroup cubeGroup)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < Kube.OH.blockTypes.Length; i++)
		{
			if (Kube.OH.blockTypes[i].group == cubeGroup)
			{
				list.Add(i);
			}
		}
		return list.ToArray();
	}

	public void onAssetsLoaded(int id)
	{
		if (this.isReady)
		{
			return;
		}
		if (Kube.ASS2 == null)
		{
			return;
		}
		this.isReady = true;
		this.inventarCubes = new int[Kube.ASS2.inventarCubesTex.Length];
		for (int i = 0; i < this.inventarCubes.Length; i++)
		{
			this.inventarCubes[i] = 1;
		}
	}

	public void ShowInventar()
	{
		UnityEngine.Debug.Log("ShowInventar");
	}

	private void ToggleInventar()
	{
		if (Kube.BCS == null)
		{
			return;
		}
		UnityEngine.Debug.Log("ToggleInventar");
		bool isMenu = Kube.OH.isMenu;
		if (this.ps != null)
		{
			this.ps.paused = isMenu;
		}
		if (!Kube.OH.isMenu)
		{
			if (Kube.BCS.gameProcess != BattleControllerScript.GameProcess.end)
			{
				Kube.BCS.menu.SetActive(true);
				if (Kube.BCS.gameType == GameType.creating)
				{
					Kube.BCS.menu.GetComponent<GameMenu>().head.MenuName("Decor_menu");
				}
				else
				{
					Kube.BCS.menu.GetComponent<GameMenu>().head.MenuName("Arsenal_menu");
				}
			}
		}
		else
		{
			Kube.BCS.menu.SetActive(false);
		}
	}

	private void ToggleInventarBank(string message = "")
	{
		MainMenu.ShowBank();
	}

	private void ToggleInventarVIP()
	{
	}

	private void ToggleInventarItems()
	{
	}

	private void ToggleInventarCharMoves(int k = -1)
	{
	}

	private void ToggleinventarBullets(int numOfbullets)
	{
	}

	private void ToggleinventarHealth()
	{
		if (Kube.BCS == null)
		{
			return;
		}
		UnityEngine.Debug.Log("ToggleInventar");
		bool isMenu = Kube.OH.isMenu;
		if (this.ps != null)
		{
			this.ps.paused = isMenu;
		}
		if (!Kube.OH.isMenu)
		{
			if (Kube.BCS.gameProcess != BattleControllerScript.GameProcess.end)
			{
				Kube.BCS.menu.SetActive(true);
				Kube.BCS.menu.GetComponent<GameMenu>().head.onMenuNum(4);
			}
		}
		else
		{
			Kube.BCS.menu.SetActive(false);
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.C))
		{
			if (this.ps != null)
			{
				if (this.ps.dead)
				{
					this.ToggleinventarHealth();
				}
				else
				{
					this.ToggleInventar();
				}
			}
			else
			{
				this.ToggleInventar();
			}
		}
		if (this.ps != null && !this.ps.dead)
		{
			if (KubeInput.GetKeyDown(KeyCode.Alpha1))
			{
				this.ChoseFastInventar(0);
			}
			if (KubeInput.GetKeyDown(KeyCode.Alpha2))
			{
				this.ChoseFastInventar(1);
			}
			if (KubeInput.GetKeyDown(KeyCode.Alpha3))
			{
				this.ChoseFastInventar(2);
			}
			if (KubeInput.GetKeyDown(KeyCode.Alpha4))
			{
				this.ChoseFastInventar(3);
			}
			if (KubeInput.GetKeyDown(KeyCode.Alpha5))
			{
				this.ChoseFastInventar(4);
			}
			if (KubeInput.GetKeyDown(KeyCode.Alpha6))
			{
				this.ChoseFastInventar(5);
			}
			if (KubeInput.GetKeyDown(KeyCode.Alpha7))
			{
				this.ChoseFastInventar(6);
			}
			if (KubeInput.GetKeyDown(KeyCode.Alpha8))
			{
				this.ChoseFastInventar(7);
			}
			if (KubeInput.GetKeyDown(KeyCode.Alpha9))
			{
				this.ChoseFastInventar(8);
			}
			if (KubeInput.GetKeyDown(KeyCode.Alpha0))
			{
				this.ChoseFastInventar(9);
			}
			if (KubeInput.GetAxis("Mouse ScrollWheel") > 0f)
			{
				this.ChoseFastInventarWheel((this.chosenFastInventar + 1) % 10);
			}
			if (KubeInput.GetAxis("Mouse ScrollWheel") < 0f)
			{
				this.ChoseFastInventarWheel((this.chosenFastInventar - 1) % 10);
			}
		}
		if (this.ps == null && this.showFastPanel)
		{
			this.showFastPanel = false;
		}
	}

	public void ChoseFastInventarWheel(int num)
	{
		if (Time.time < this._nextWheeel)
		{
			return;
		}
		this._nextWheeel = Time.time + 0.5f;
		int num2 = num - this.chosenFastInventar;
		if (Kube.BCS.gameType == GameType.creating)
		{
			return;
		}
		for (;;)
		{
			if (num < 0)
			{
				num = 10 + num;
			}
			if (num >= 10)
			{
				num = 0;
			}
			if (Kube.GPS.fastInventarWeapon[num].Type == 4)
			{
				break;
			}
			if (num == this.chosenFastInventar)
			{
				return;
			}
			num += num2;
		}
		if (num != this.chosenFastInventar)
		{
			this.ChoseFastInventar(num);
		}
	}

	public void ChoseFastInventar(int num)
	{
		if (num < 0)
		{
			num = 10 + num;
		}
		if (num >= 10)
		{
			num = 0;
		}
		this.chosenFastInventar = num;
		if (Kube.BCS && Kube.BCS.gameType == GameType.creating)
		{
			if (this.ps != null)
			{
				this.ps.ChangeWeapon(-1, -1);
			}
			Kube.BCS.hud.ChoseCube(num);
		}
		else if (Kube.BCS.ps.fastInventar[num].Type != 4)
		{
			if (this.ps != null)
			{
				if (num <= 6)
				{
					this.ps.ChangeWeapon(-1, -1);
				}
				this.ps.DoUseMagic(Kube.BCS.ps.fastInventar[num].Num);
			}
		}
		else if (Kube.BCS.ps.fastInventar[num].Type == 4 && this.ps != null)
		{
			this.ps.SelectWeapon(num);
		}
	}

	public int findNextWeapon(int currentWeapon, int group)
	{
		int num = -1;
		for (int i = 0; i < this.weaponParams.Length; i++)
		{
			if (this.weaponParams[i].weaponGroup == (InventoryScript.WeaponGroup)group)
			{
				if (Kube.GPS.inventarWeapons[i] > 0 || !this.ps || this.ps.HasWeaponPickup(i))
				{
					if (i > currentWeapon)
					{
						return i;
					}
					if (i != currentWeapon && num == -1)
					{
						num = i;
					}
				}
			}
		}
		return num;
	}

	public void resetInventory()
	{
		for (int i = 0; i < Kube.GPS.fastInventar.Length; i++)
		{
			if (Kube.GPS.fastInventar[i].Type == 3 && Kube.GPS.inventarItems[Kube.GPS.fastInventar[i].Num] <= 0)
			{
				Kube.GPS.fastInventar[i].Type = -1;
				Kube.GPS.fastInventar[i].Num = 0;
			}
		}
		for (int j = 0; j < Kube.GPS.fastInventarWeapon.Length; j++)
		{
			if (Kube.GPS.fastInventarWeapon[j].Type == 4 && Kube.GPS.inventarWeapons[Kube.GPS.fastInventarWeapon[j].Num] <= 0)
			{
				Kube.GPS.fastInventarWeapon[j].Type = -1;
				Kube.GPS.fastInventarWeapon[j].Num = 0;
			}
			if (Kube.GPS.fastInventarWeapon[j].Type == 3 && Kube.GPS.inventarItems[Kube.GPS.fastInventarWeapon[j].Num] <= 0)
			{
				Kube.GPS.fastInventarWeapon[j].Type = -1;
				Kube.GPS.fastInventarWeapon[j].Num = 0;
			}
		}
		this.chosenFastInventar = 0;
	}

	public bool checkDublicate(FastInventar[] fastInventar)
	{
		for (int i = 0; i < 10; i++)
		{
			if (fastInventar[i].Type == this.chosenInventarItem.Type && fastInventar[i].Num == this.chosenInventarItem.Num)
			{
				fastInventar[i].Num = 0;
				fastInventar[i].Type = -1;
				return false;
			}
		}
		return true;
	}

	public int putToFastInvetar(int type, FastInventar item)
	{
		FastInventar[] array = Kube.GPS.fastInventar;
		if (type == 1)
		{
			array = Kube.GPS.fastInventarWeapon;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Type == item.Type && array[i].Num == item.Num)
			{
				Kube.SS.SaveFastInventory(type, array, null);
				return i;
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].Type == -1 || array[j].Num == -1)
			{
				array[j] = item;
				Kube.SS.SaveFastInventory(type, array, null);
				return j;
			}
		}
		return 0;
	}

	private void UpgradeParamDone(string[] strs)
	{
		if (strs[0] == "0")
		{
			Kube.GPS.playerMoney1 = Convert.ToInt32(strs[2]);
			Kube.GPS.playerMoney2 = Convert.ToInt32(strs[3]);
			Kube.GPS.playerExpPoints = Convert.ToInt32(strs[4]);
			string[] array = strs[5].Split(InventoryScript.dc2);
			Kube.GPS.playerHealth = Convert.ToInt32(array[0]);
			Kube.GPS.playerArmor = Convert.ToInt32(array[1]);
			Kube.GPS.playerSpeed = Convert.ToInt32(array[2]);
			Kube.GPS.playerJump = Convert.ToInt32(array[3]);
			Kube.GPS.playerDefend = Convert.ToInt32(array[4]);
			if (this.ps != null)
			{
				this.ps.SendMessage("RecountBonuces");
			}
			Kube.SendMonoMessage("UpgradeParamRecountBonuces", new object[0]);
		}
		else
		{
			Kube.SS.SendStat("charles");
		}
	}

	private void BuyBulletsDone(string[] strs)
	{
		if (strs[0] == "0")
		{
			Kube.GPS.playerMoney1 = Convert.ToInt32(strs[2]);
			Kube.GPS.playerMoney2 = Convert.ToInt32(strs[3]);
			int num = Convert.ToInt32(strs[strs.Length - 1]);
			string[] array = strs[4].Split(InventoryScript.dc2);
			int num2 = Math.Min(array.Length, Kube.IS.bulletParams.Length);
			for (int i = 0; i < num2; i++)
			{
				string[] array2 = array[i].Split(this.dc4);
				Kube.IS.bulletParams[i].initialAmount = Math.Max(Kube.IS.bulletParams[i].initialAmountArray[0], Convert.ToInt32(array2[0]));
				for (int j = 0; j < Kube.IS.bulletParams[i].initialAmountArray.Length; j++)
				{
					if (Kube.IS.bulletParams[i].initialAmount >= Kube.IS.bulletParams[i].initialAmountArray[j])
					{
						Kube.IS.bulletParams[i].initialAmountIndex = j;
					}
				}
			}
		}
		else
		{
			Kube.SS.SendStat("charles");
		}
	}

	private void BuySkinDone(string[] strs)
	{
		if (strs[0] == "0")
		{
			Kube.GPS.playerMoney1 = Convert.ToInt32(strs[2]);
			Kube.GPS.playerMoney2 = Convert.ToInt32(strs[3]);
			string[] array = strs[4].Split(InventoryScript.dc2);
			int num = 0;
			while (num < Kube.GPS.playerSkins.Length && num < array.Length)
			{
				Kube.GPS.playerSkins[num] = Convert.ToInt32(array[num]);
				num++;
			}
			if (Kube.GPS.playerSkins[this.chosenSkin] != 0)
			{
				Kube.GPS.playerSkin = this.chosenSkin;
				Kube.SS.SetSkin(Kube.GPS.playerSkin);
				GameObject gameObject = GameObject.FindGameObjectWithTag("Menu");
				if (this.ps != null)
				{
					this.ps.SendMessage("PlayerDressSkin");
				}
				Kube.SendMonoMessage("UpdateChar", new object[0]);
			}
		}
		else
		{
			Kube.SS.SendStat("charles");
		}
	}

	private void BuyVIPDone(string str)
	{
		char[] separator = new char[]
		{
			'^'
		};
		string[] array = str.Split(separator);
		string text = string.Empty + Kube.SS.serverId;
		for (int i = 2; i < array.Length - 2; i++)
		{
			text += array[i];
		}
		text = text + array[array.Length - 1] + string.Empty + Kube.SS.phpSecret;
		if (AuxFunc.GetMD5(text) == array[array.Length - 2])
		{
			Kube.GPS.playerMoney2 = Convert.ToInt32(array[2]);
			int num = Convert.ToInt32(array[array.Length - 1]);
			Kube.SS.serverTime = (float)num;
			Kube.GPS.vipEnd = Time.time + (float)(Convert.ToInt32(array[3]) - num);
			Kube.SendMonoMessage("EventBuyVIPDone", new object[0]);
		}
		else
		{
			Kube.SS.SendStat("charles");
		}
	}

	private void GoldToMoneyDone(string str)
	{
		char[] separator = new char[]
		{
			'^'
		};
		string[] array = str.Split(separator);
		Kube.GPS.playerMoney1 = Convert.ToInt32(array[2]);
		Kube.GPS.playerMoney2 = Convert.ToInt32(array[3]);
	}

	private void PaymentAnswer()
	{
		Kube.GPS.printLog("InventoryScript PaymentAnswer");
		Kube.SS.GetPlayerMoney(Kube.OH.gameObject, "GetPlayerMoneyDone");
	}

	private void BuyCubesDone(string[] strs)
	{
		if (strs[0] == "0")
		{
			Kube.GPS.playerMoney1 = Convert.ToInt32(strs[2]);
			Kube.GPS.playerMoney2 = Convert.ToInt32(strs[3]);
			int num = Convert.ToInt32(strs[strs.Length - 1]);
			string[] array = strs[4].Split(InventoryScript.dc2);
			int num2 = 0;
			while (num2 < array.Length && num2 < Kube.GPS.cubesTimeOfEnd.Length)
			{
				Kube.GPS.cubesTimeOfEnd[num2] = Time.time + (float)(Convert.ToInt32(array[num2]) - num);
				num2++;
			}
			Kube.GPS.cubesTimeOfEnd[0] = Time.time + 1E+07f;
			Kube.SS.serverTime = (float)num;
			Kube.SendMonoMessage("CubesUpdate", new object[0]);
		}
		else
		{
			Kube.SS.SendStat("charles");
		}
	}

	private void guiBuyItemDialog()
	{
	}

	private void BuyItemDone(string[] strs)
	{
		char[] separator = new char[]
		{
			';'
		};
		if (strs[0] == "0")
		{
			Kube.GPS.playerMoney1 = Convert.ToInt32(strs[2]);
			Kube.GPS.playerMoney2 = Convert.ToInt32(strs[3]);
			string[] array = strs[4].Split(separator);
			int num = 0;
			while (num < array.Length && num < Kube.GPS.inventarItems.Length)
			{
				Kube.GPS.inventarItems[num] = Convert.ToInt32(array[num]);
				num++;
			}
			Kube.SendMonoMessage("ItemsCubesUpdate", new object[0]);
		}
	}

	private void BuyClothesDone(string str)
	{
		char[] separator = new char[]
		{
			'^'
		};
		char[] separator2 = new char[]
		{
			';'
		};
		string[] array = str.Split(separator);
		string text = string.Empty + Kube.SS.serverId;
		for (int i = 2; i < array.Length - 2; i++)
		{
			text += array[i];
		}
		text = text + array[array.Length - 1] + string.Empty + Kube.SS.phpSecret;
		if (AuxFunc.GetMD5(text) == array[array.Length - 2])
		{
			Kube.GPS.playerMoney1 = Convert.ToInt32(array[2]);
			Kube.GPS.playerMoney2 = Convert.ToInt32(array[3]);
			string[] array2 = array[4].Split(separator2);
			int num = 0;
			while (num < array2.Length && num < Kube.GPS.playerIsClothes.Length)
			{
				Kube.GPS.playerIsClothes[num] = Convert.ToInt32(array2[num]);
				num++;
			}
			int num2 = Convert.ToInt32(array[array.Length - 1]);
			Kube.SS.serverTime = (float)num2;
			Kube.SendMonoMessage("UpdateChar", new object[0]);
		}
		else
		{
			Kube.SS.SendStat("charles");
		}
	}

	private void BuyWeaponDone(string[] strs)
	{
		if (strs[0] == "0")
		{
			Kube.GPS.playerMoney1 = Convert.ToInt32(strs[2]);
			Kube.GPS.playerMoney2 = Convert.ToInt32(strs[3]);
			string[] array = strs[4].Split(InventoryScript.dc2);
			int num = Convert.ToInt32(strs[strs.Length - 1]);
			int num2 = 0;
			while (num2 < array.Length && num2 < Kube.GPS.inventarWeapons.Length)
			{
				uint num3 = Convert.ToUInt32(array[num2]);
				if (num3 == 1u)
				{
					Kube.GPS.inventarWeapons[num2] = (int)Time.time + 10000000;
				}
				else
				{
					Kube.GPS.inventarWeapons[num2] = (int)Time.time + (int)((ulong)num3 - (ulong)((long)num));
				}
				num2++;
			}
			if (Kube.GPS.inventarWeapons[this.selectedInventarItem.Num] > (int)Time.time)
			{
				this.chosenInventarItem.Num = this.selectedInventarItem.Num;
			}
			Kube.SendMonoMessage("WeaponsUpdate", new object[0]);
		}
	}

	private void BuySpecItemDone(string[] strs)
	{
		if (strs[0] == "0")
		{
			Kube.GPS.playerMoney1 = Convert.ToInt32(strs[2]);
			Kube.GPS.playerMoney2 = Convert.ToInt32(strs[3]);
			string[] array = strs[4].Split(InventoryScript.dc2);
			int num = Convert.ToInt32(strs[strs.Length - 1]);
			int num2 = 0;
			while (num2 < array.Length && num2 < Kube.GPS.inventarSpecItems.Length)
			{
				uint num3 = Convert.ToUInt32(array[num2]);
				Kube.GPS.inventarSpecItems[num2] = (int)Time.time + (int)((ulong)num3 - (ulong)((long)num));
				num2++;
			}
			Kube.SendMonoMessage("ItemsCubesUpdate", new object[0]);
		}
	}

	public void ShowFastPanel(bool isShow)
	{
		this.showFastPanel = isShow;
	}

	public void ShowInventar(bool isShow)
	{
		this.showInventar = isShow;
	}

	public PlayerScript ps
	{
		get
		{
			if (Kube.BCS)
			{
				return Kube.BCS.ps;
			}
			return null;
		}
	}

	public int UseItem(int itemNum)
	{
		int num = Kube.GPS.inventarItems[itemNum];
		if (Kube.GPS.inventarItems[itemNum] > 0)
		{
			GameParamsScript.InventarItems inventarItems2;
			GameParamsScript.InventarItems inventarItems = inventarItems2 = Kube.GPS.inventarItems;
			int num2 = inventarItems2[itemNum];
			inventarItems[itemNum] = num2 - 1;
			Kube.SS.UseItem(itemNum);
			if (num - 1 != Kube.GPS.inventarItems[itemNum])
			{
				return 1;
			}
		}
		if (Kube.GPS.inventarItems[itemNum] <= 0)
		{
			Kube.GPS.inventarItems[itemNum] = 0;
			for (int i = 0; i < 10; i++)
			{
				if (Kube.GPS.fastInventar[i].Type != 7)
				{
					if (Kube.GPS.fastInventar[i].Type != 4)
					{
						if (Kube.GPS.fastInventar[i].Num == itemNum)
						{
							Kube.GPS.fastInventar[i].Type = -1;
							Kube.GPS.fastInventar[i].Num = 0;
						}
						else if (Kube.GPS.fastInventarWeapon[i].Num == itemNum)
						{
							Kube.GPS.fastInventarWeapon[i].Type = -1;
							Kube.GPS.fastInventarWeapon[i].Num = 0;
						}
					}
				}
			}
		}
		return 0;
	}

	private void AddItemDone(string str)
	{
		char[] separator = new char[]
		{
			'^'
		};
		char[] separator2 = new char[]
		{
			';'
		};
		string[] array = str.Split(separator);
		if (Convert.ToInt32(array[0]) == 0)
		{
			string[] array2 = array[2].Split(separator2);
			int num = 0;
			while (num < array2.Length && num < Kube.GPS.inventarItems.Length)
			{
				Kube.GPS.inventarItems[num] = Convert.ToInt32(array2[num]);
				num++;
			}
		}
	}

	public int needLevel(FastInventar fi)
	{
		if (fi.Type == 3)
		{
			return Kube.IS.itemDesc[fi.Num].needLevel;
		}
		if (fi.Type == 4)
		{
			return this.weaponParams[fi.Num].needLevel;
		}
		return Kube.IS.specItemDesc[fi.Num].needLevel;
	}

	public bool needUnlock(FastInventar fi)
	{
		return false;
	}

	public bool canBuy(FastInventar fi)
	{
		if (fi.Type == 3)
		{
			return Kube.GPS.itemUnlock[fi.Num] || Kube.IS.itemDesc[fi.Num].needLevel <= Kube.GPS.playerLevel + 1;
		}
		return Kube.GPS.specUnlock[fi.Num] || Kube.IS.specItemDesc[fi.Num].needLevel <= Kube.GPS.playerLevel + 1;
	}

	private void PackAnswer()
	{
		UnityEngine.Debug.Log("InventoryScript PackAnswer");
		Kube.SS.Request(1001, null, new ServerCallback(this.BuyPackDone));
	}

	private void BuyPackDone(string response)
	{
		if (string.IsNullOrEmpty(response))
		{
			return;
		}
		JsonData jsonData = JsonMapper.ToObject(response);
		char[] array = new char[]
		{
			';'
		};
		JsonData jsonData2 = jsonData["i"];
		int num = 0;
		while (num < jsonData2.Count && num < Kube.GPS.inventarItems.Length)
		{
			Kube.GPS.inventarItems[num] = int.Parse(jsonData2[num].ToString());
			num++;
		}
		JsonData jsonData3 = jsonData["w"];
		int num2 = 0;
		while (num2 < jsonData3.Count && num2 < Kube.GPS.inventarWeapons.Length)
		{
			uint num3 = uint.Parse(jsonData3[num2].ToString());
			if (num3 == 1u)
			{
				Kube.GPS.inventarWeapons[num2] = (int)Time.time + 10000000;
			}
			else
			{
				Kube.GPS.inventarWeapons[num2] = (int)((ulong)num3 - (ulong)((long)((int)Kube.SS.serverTime)));
			}
			num2++;
		}
		Kube.SendMonoMessage("ItemsCubesUpdate", new object[0]);
		Kube.SendMonoMessage("WeaponsUpdate", new object[0]);
		Kube.SendMonoMessage("NotifyUpdate", new object[0]);
	}

	private void BuyWeaponSkinDone()
	{
		Kube.SendMonoMessage("WeaponsUpdate", new object[0]);
		Kube.SendMonoMessage("NotifyUpdate", new object[0]);
	}

	private void UseWeaponSkinDone()
	{
		Kube.SendMonoMessage("WeaponsUpdate", new object[0]);
		Kube.SendMonoMessage("NotifyUpdate", new object[0]);
	}

	public Texture2D[] popTex;

	[NonSerialized]
	public int[] inventarCubes;

	[NonSerialized]
	public int[] cubesNatureNums;

	[NonSerialized]
	public int[] cubesBuilderNums;

	[NonSerialized]
	public int[] cubesDecorNums;

	[NonSerialized]
	public int[] cubesGlassNums;

	[NonSerialized]
	public int[] cubesWaterNums;

	[NonSerialized]
	public int[] cubesDifferentNums;

	public InventoryScript.ClothesParams[] clothesParams;

	public InventoryScript.WeaponParams[] weaponParams;

	public WeaponSkinDesc[] weaponSkins;

	public InventoryScript.ItemDesc[] itemDesc;

	public InventoryScript.ItemDesc[] specItemDesc;

	public InventoryScript.BonusParams[] bonusParams;

	public InventoryScript.BulletParams[] bulletParams;

	public int[] inventarBullets;

	[HideInInspector]
	public InventoryScript.GameItemGOLoader gameItemsGO;

	[HideInInspector]
	public InventoryScript.WeaponGOLoader charWeaponsGO;

	[NonSerialized]
	public int[] shopHats;

	[NonSerialized]
	public int[] shopTors;

	[NonSerialized]
	public int[] shopBack;

	[NonSerialized]
	public int[] shopArms;

	[NonSerialized]
	public int[] shopFoots;

	[NonSerialized]
	public int[] shopShoulders;

	private bool showFastPanel;

	private bool showInventar;

	[NonSerialized]
	public int chosenFastInventar = -1;

	public string[] clothesTransforms = new string[]
	{
		"Bip001 Head",
		"Bip001 R Hand",
		"Bip001 L Hand",
		"Bip001 R UpperArm",
		"Bip001 L UpperArm",
		"Bip001 R Foot",
		"Bip001 L Foot",
		"Bip001 Spine3",
		"Bip001 Spine3",
		"Bip001 Head"
	};

	[HideInInspector]
	private string[] inventoryTypeStrs;

	[HideInInspector]
	private string[] inventoryCubesTypesStrs;

	[HideInInspector]
	private string[] inventoryDecorTypesStrs;

	[HideInInspector]
	private string[] inventoryItemsTypesStrs;

	[HideInInspector]
	private string[] inventoryWeaponTypesStrs;

	[HideInInspector]
	private string[] inventoryCharacterTypesStrs;

	[HideInInspector]
	private string[] inventoryDeviceTypesStrs;

	[HideInInspector]
	private string[] clothesType;

	private int clothesTypeNum;

	public FastInventar selectedInventarItem;

	public FastInventar chosenInventarItem;

	private Vector2 invWeaponScroll = Vector2.zero;

	private int chosenSkin;

	private int chosenClothesIndex = -1;

	private float goldToMoneySlider;

	[NonSerialized]
	public int[] tempClothes = new int[32];

	public string[] hapName = new string[]
	{
		"Нет"
	};

	public string[] ammunitionName = new string[]
	{
		"Нет"
	};

	public int[] minGPSbullets = new int[4];

	public Texture fastInventarDarkTex;

	public Texture charParamsTex;

	public Texture[] stars;

	public Texture charParamsBuyTex;

	public Texture expStar;

	public Texture moneyTex;

	public Texture goldTex;

	public Texture buttonArrows;

	public Texture HIT;

	public Texture x2Tex;

	public Texture weaponAmmoTex;

	public GameObject testCharacterPrefab;

	public GameObject testWeaponPrefab;

	private GameObject testCharacter;

	private GameObject testWeapon;

	public RenderTexture renderChar;

	public RenderTexture renderWeapon;

	public Texture arrowDownTex;

	private bool initialized;

	private InventoryScript.TabDrawCall[] _tabs;

	private bool isReady;

	private int inventoryWeaponNeedbullets;

	private float inventoryWeaponNeedbulletsTime;

	protected float _nextWheeel;

	private string playerName;

	private char[] dc4 = new char[]
	{
		':'
	};

	private static char[] dc2 = new char[]
	{
		';'
	};

	[Serializable]
	public enum WeaponGroup
	{
		hidden = -1,
		melee,
		pistol,
		shotgun,
		assault,
		heavy,
		tactical
	}

	[Serializable]
	public class WeaponParams
	{
		public string name;

		public int order;

		public int weaponType;

		public int UsingBullets;

		public int[] clipSize;

		[NonSerialized]
		public int currentClipSizeIndex;

		public float[] reloadTime;

		[NonSerialized]
		public int currentReloadTimeIndex;

		public int BulletsType;

		public float[] DeltaShotArray;

		[NonSerialized]
		public int currentDeltaShotIndex;

		public float DeltaShot;

		public float[] Damage;

		[NonSerialized]
		public int currentDamageIndex;

		public float[] Accuracy;

		[NonSerialized]
		public int currentAccuracyIndex;

		public float Distance;

		public int Type;

		public Texture[] aimTex;

		public float fatalDistance;

		public float accuarcy;

		public InventoryScript.WeaponGroup weaponGroup;

		public bool hidden;

		public int needHealthLevel;

		public int needArmorLevel;

		public int needJumpLevel;

		public int needSpeedLevel;

		public int needResistLevel;

		public int needLevel;

		public bool sniper;
	}

	public enum ClothesPages
	{
		Hidden,
		Hats,
		Tors,
		Back,
		Arms,
		Foots,
		Shoulders
	}

	[Serializable]
	public class ClothesParams
	{
		public string name;

		public InventoryScript.ClothesPages group;
	}

	[Serializable]
	public class BonusParams
	{
		public string name;

		public int experience;

		public int bonusesCount;

		public BonusVariableType bonusVariable;

		public int needForGetBonus;
	}

	public enum BulletGroup
	{
		ammo,
		shells,
		rockets,
		energy,
		next,
		secret
	}

	[Serializable]
	public class BulletParams
	{
		public string name;

		public int initialAmount;

		public InventoryScript.BulletGroup bulletGroup;

		public int puckupAmount;

		public int[] initialAmountArray;

		public int initialAmountIndex;
	}

	public enum ItemPage
	{
		Hidden,
		Lights,
		Furniture,
		Doors,
		Ladders,
		Green,
		Decor,
		Location,
		Road,
		Weapons,
		Monsters,
		Abilis,
		Battle,
		AA,
		Switch,
		Transport,
		Guns,
		Other,
		Moves,
		Spec,
		Forms,
		Boxes
	}

	[Serializable]
	public class ItemDesc
	{
		public string name;

		public int needLevel;

		public bool hidden;

		public InventoryScript.ItemPage page;
	}

	public class GameItemGOLoader
	{
		public GameObject this[int index]
		{
			get
			{
				if (Kube.ASS3 != null && index < Kube.ASS3.gameItemsGO.Length && Kube.ASS3.gameItemsGO[index] != null)
				{
					return Kube.ASS3.gameItemsGO[index];
				}
				if (Kube.OH.gameItemsGO.ContainsKey(index) && Kube.OH.gameItemsGO[index] != null)
				{
					return Kube.OH.gameItemsGO[index];
				}
				if (this.hash.ContainsKey(index))
				{
					return this.hash[index];
				}
				GameObject gameObject = Kube.RM.FindItemAsset(index);
				if (gameObject)
				{
					this.hash[index] = gameObject;
				}
				return gameObject;
			}
		}

		public int Length
		{
			get
			{
				return 300;
			}
		}

		private Dictionary<int, GameObject> hash = new Dictionary<int, GameObject>();
	}

	public class WeaponGOLoader
	{
		public GameObject this[int index]
		{
			get
			{
				if (Kube.ASS6 != null && Kube.OH.charWeaponsGO[index] != null)
				{
					return Kube.OH.charWeaponsGO[index];
				}
				GameObject gameObject = null;
				if (this.hash.ContainsKey(index))
				{
					gameObject = this.hash[index];
				}
				if (gameObject == null)
				{
					gameObject = Kube.RM.FindAsset("WeaponGO", index);
					this.hash[index] = gameObject;
				}
				return gameObject;
			}
		}

		public int Length
		{
			get
			{
				return 200;
			}
		}

		private Dictionary<int, GameObject> hash = new Dictionary<int, GameObject>();
	}

	private delegate void TabDrawCall();
}
