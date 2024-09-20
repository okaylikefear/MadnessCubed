using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
	[ContextMenu("Print Weapons Power")]
	private void PrintWeaponsPower()
	{
		for (int i = 0; i < 6; i++)
		{
			string text = string.Empty + ((InventoryScript.WeaponGroup)i).ToString() + ": ";
			for (int j = 0; j < this.weaponParams.Length; j++)
			{
				InventoryScript.WeaponParams weaponParams = this.weaponParams[j];
				if (weaponParams.weaponGroup == (InventoryScript.WeaponGroup)i)
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						weaponParams.name,
						"(",
						j,
						")=",
						weaponParams.Damage[0] / weaponParams.DeltaShotArray[0],
						" "
					});
				}
			}
			MonoBehaviour.print(text);
		}
	}

	[ContextMenu("buildWeaponParams")]
	private void CollectValues()
	{
		int num = 6;
		for (int i = 0; i < this.weaponParams.Length; i++)
		{
			InventoryScript.WeaponParams weaponParams = this.weaponParams[i];
			if (weaponParams.UsingBullets != 0)
			{
				int num2 = weaponParams.clipSize[0];
				weaponParams.clipSize = new int[num];
				for (int j = 0; j < num; j++)
				{
					weaponParams.clipSize[j] = Mathf.RoundToInt((float)num2 * (1f + 1f * (float)j / (float)(num - 1)));
				}
				float num3 = weaponParams.reloadTime[0];
				weaponParams.reloadTime = new float[num];
				for (int k = 0; k < num; k++)
				{
					weaponParams.reloadTime[k] = num3 * (1f - 0.6f * (float)k / (float)(num - 1));
				}
				float num4 = weaponParams.Accuracy[0];
				weaponParams.Accuracy = new float[num];
				for (int l = 0; l < num; l++)
				{
					weaponParams.Accuracy[l] = num4 * (1f - 0.5f * (float)l / (float)(num - 1));
				}
			}
			float num5 = weaponParams.DeltaShotArray[0];
			weaponParams.DeltaShotArray = new float[num];
			for (int m = 0; m < num; m++)
			{
				weaponParams.DeltaShotArray[m] = num5 * (1f - 0.4f * (float)m / (float)(num - 1));
			}
			float num6 = weaponParams.Damage[0];
			weaponParams.Damage = new float[num];
			for (int n = 0; n < num; n++)
			{
				weaponParams.Damage[n] = Mathf.Round(num6 * (1f + 0.3f * (float)n / (float)(num - 1)));
			}
			if (weaponParams.UsingBullets == 0)
			{
				weaponParams.clipSize = new int[1];
				weaponParams.reloadTime = new float[1];
				weaponParams.Accuracy = new float[1];
			}
			if (weaponParams.UsingBullets != 0)
			{
				if (weaponParams.clipSize[weaponParams.clipSize.Length - 1] - weaponParams.clipSize[0] < weaponParams.clipSize.Length - 1)
				{
					UnityEngine.Debug.Log("Bad clip: " + weaponParams.name);
				}
				if ((weaponParams.Damage[weaponParams.Damage.Length - 1] - weaponParams.Damage[0]) / (float)(weaponParams.Damage.Length - 1) < 1f)
				{
					UnityEngine.Debug.Log("Bad damage: " + weaponParams.name);
				}
			}
		}
	}

	public InventarMenu inventaryType
	{
		get
		{
			return this._inventaryType;
		}
		set
		{
			if (value == this._inventaryType)
			{
				return;
			}
			this.selectedInventarItem = default(FastInventar);
			this.selectedInventarItem.Num = -1;
			this.selectedInventarItem.Type = (int)this.menuXref[(int)value];
			this.chosenInventarItem.Num = -1;
			this.chosenInventarItem.Type = (int)this.menuXref[(int)value];
			this.chosenSkin = Kube.GPS.playerSkin;
			this.clothesTypeNum = 0;
			this.inventoryPageType = 0;
			this._inventaryType = value;
		}
	}

	public int inventoryPageType
	{
		get
		{
			return this._inventoryPageType;
		}
		set
		{
			if (value == this._inventoryPageType)
			{
				return;
			}
			this.selectedInventarItem = default(FastInventar);
			this.selectedInventarItem.Num = -1;
			this.selectedInventarItem.Type = value;
			this.chosenInventarItem.Num = -1;
			this.chosenInventarItem.Type = value;
			this._inventoryPageType = value;
			this.clothesTypeNum = 0;
		}
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
		this.inventarCubes = new int[Kube.ASS2.cubesTex.Length];
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
					Kube.BCS.menu.GetComponent<GameMenu>().head.onMenuNum(1);
				}
				else
				{
					Kube.BCS.menu.GetComponent<GameMenu>().head.onMenuNum(4);
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
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
			{
				this.ChoseFastInventar(0);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
			{
				this.ChoseFastInventar(1);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
			{
				this.ChoseFastInventar(2);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
			{
				this.ChoseFastInventar(3);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5))
			{
				this.ChoseFastInventar(4);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha6))
			{
				this.ChoseFastInventar(5);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha7))
			{
				this.ChoseFastInventar(6);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha8))
			{
				this.ChoseFastInventar(7);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha9))
			{
				this.ChoseFastInventar(8);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0))
			{
				this.ChoseFastInventar(9);
			}
			if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				this.ChoseFastInventarWheel((this.chosenFastInventar + 1) % 10);
			}
			if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0f)
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
				this.ps.ChangeWeapon(-1);
			}
			Kube.BCS.hud.ChoseCube(num);
		}
		else if (Kube.GPS.fastInventarWeapon[num].Type != 4)
		{
			if (this.ps != null)
			{
				if (num <= 6)
				{
					this.ps.ChangeWeapon(-1);
				}
				this.ps.DoUseMagic(Kube.GPS.fastInventarWeapon[num].Num);
			}
		}
		else if (Kube.GPS.fastInventarWeapon[num].Type == 4 && this.ps != null)
		{
			Kube.BCS.hud.ChoseWeapon(num);
			this.ps.ChangeWeapon(Kube.GPS.fastInventarWeapon[num].Num);
		}
	}

	public int findNextWeapon(int currentWeapon, int group)
	{
		int num = -1;
		for (int i = 0; i < this.weaponParams.Length; i++)
		{
			if (this.weaponParams[i].weaponGroup == (InventoryScript.WeaponGroup)group)
			{
				if (Kube.GPS.inventarWeapons[i] >= 0 || !this.ps || this.ps.HasWeaponPickup(i))
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
			if ((Kube.GPS.fastInventar[i].Type == 1 || Kube.GPS.fastInventar[i].Type == 3) && Kube.GPS.inventarItems[Kube.GPS.fastInventar[i].Num] <= 0)
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
			if ((Kube.GPS.fastInventarWeapon[j].Type == 1 || Kube.GPS.fastInventarWeapon[j].Type == 3) && Kube.GPS.inventarItems[Kube.GPS.fastInventarWeapon[j].Num] <= 0)
			{
				Kube.GPS.fastInventarWeapon[j].Type = -1;
				Kube.GPS.fastInventarWeapon[j].Num = 0;
			}
		}
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

	private void UpgradeParamDone(string str)
	{
		MonoBehaviour.print(str);
		char[] separator = new char[]
		{
			'^'
		};
		char[] separator2 = new char[]
		{
			';'
		};
		string[] array = str.Split(separator);
		string text = string.Empty + Kube.GPS.playerId;
		for (int i = 2; i < array.Length - 2; i++)
		{
			text += array[i];
		}
		text = text + array[array.Length - 1] + string.Empty + Kube.SS.phpSecret;
		if (AuxFunc.GetMD5(text) == array[array.Length - 2])
		{
			Kube.GPS.playerMoney1 = Convert.ToInt32(array[2]);
			Kube.GPS.playerMoney2 = Convert.ToInt32(array[3]);
			Kube.GPS.playerExpPoints = Convert.ToInt32(array[4]);
			string[] array2 = array[5].Split(separator2);
			Kube.GPS.playerHealth = Convert.ToInt32(array2[0]);
			Kube.GPS.playerArmor = Convert.ToInt32(array2[1]);
			Kube.GPS.playerSpeed = Convert.ToInt32(array2[2]);
			Kube.GPS.playerJump = Convert.ToInt32(array2[3]);
			Kube.GPS.playerDefend = Convert.ToInt32(array2[4]);
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

	private void BuyBulletsDone(string str)
	{
		char[] separator = new char[]
		{
			'^'
		};
		char[] separator2 = new char[]
		{
			';'
		};
		char[] separator3 = new char[]
		{
			':'
		};
		string[] array = str.Split(separator);
		string text = string.Empty + Kube.GPS.playerId;
		for (int i = 2; i < array.Length - 2; i++)
		{
			text += array[i];
		}
		text = text + array[array.Length - 1] + string.Empty + Kube.SS.phpSecret;
		if (AuxFunc.GetMD5(text) == array[array.Length - 2])
		{
			Kube.GPS.playerMoney1 = Convert.ToInt32(array[2]);
			Kube.GPS.playerMoney2 = Convert.ToInt32(array[3]);
			int num = Convert.ToInt32(array[array.Length - 1]);
			string[] array2 = array[4].Split(separator2);
			int num2 = Math.Min(array2.Length, Kube.IS.bulletParams.Length);
			for (int j = 0; j < num2; j++)
			{
				string[] array3 = array2[j].Split(separator3);
				Kube.IS.bulletParams[j].initialAmount = Math.Max(Kube.IS.bulletParams[j].initialAmountArray[0], Convert.ToInt32(array3[0]));
				for (int k = 0; k < Kube.IS.bulletParams[j].initialAmountArray.Length; k++)
				{
					if (Kube.IS.bulletParams[j].initialAmount >= Kube.IS.bulletParams[j].initialAmountArray[k])
					{
						Kube.IS.bulletParams[j].initialAmountIndex = k;
					}
				}
			}
		}
		else
		{
			Kube.SS.SendStat("charles");
		}
	}

	private void BuySkinDone(string str)
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
		string text = string.Empty + Kube.GPS.playerId;
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
			while (num < Kube.GPS.playerSkins.Length && num < array2.Length)
			{
				Kube.GPS.playerSkins[num] = Convert.ToInt32(array2[num]);
				num++;
			}
			if (Kube.GPS.playerSkins[this.chosenSkin] != 0)
			{
				Kube.GPS.playerSkin = this.chosenSkin;
				Kube.SS.SetSkin(Kube.GPS.playerSkin);
				GameObject x = GameObject.FindGameObjectWithTag("Menu");
				if (x != null)
				{
				}
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
		string text = string.Empty + Kube.GPS.playerId;
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

	private void BuyCubesDone(string str)
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
		string text = string.Empty + Kube.GPS.playerId;
		for (int i = 2; i < array.Length - 2; i++)
		{
			text += array[i];
		}
		text = text + array[array.Length - 1] + string.Empty + Kube.SS.phpSecret;
		if (AuxFunc.GetMD5(text) == array[array.Length - 2])
		{
			Kube.GPS.playerMoney1 = Convert.ToInt32(array[2]);
			int num = Convert.ToInt32(array[array.Length - 1]);
			string[] array2 = array[3].Split(separator2);
			int num2 = 0;
			while (num2 < array2.Length && num2 < Kube.GPS.cubesTimeOfEnd.Length)
			{
				Kube.GPS.cubesTimeOfEnd[num2] = Time.time + (float)(Convert.ToInt32(array2[num2]) - num);
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

	private void BuyItemDone(string str)
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
		string text = string.Empty + Kube.GPS.playerId;
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
			while (num < array2.Length && num < Kube.GPS.inventarItemsLength)
			{
				Kube.GPS.inventarItems[num] = Convert.ToInt32(array2[num]);
				num++;
			}
			int num2 = Convert.ToInt32(array[array.Length - 1]);
			Kube.SS.serverTime = (float)num2;
			Kube.SendMonoMessage("ItemsCubesUpdate", new object[0]);
		}
		else
		{
			Kube.SS.SendStat("charles");
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
		string text = string.Empty + Kube.GPS.playerId;
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

	private void BuyWeaponDone(string str)
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
		string text = string.Empty + Kube.GPS.playerId;
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
			int num = Convert.ToInt32(array[array.Length - 1]);
			int num2 = 0;
			while (num2 < array2.Length && num2 < Kube.GPS.inventarWeapons.Length)
			{
				Kube.GPS.inventarWeapons[num2] = Convert.ToInt32(array2[num2]);
				if (Kube.GPS.inventarWeapons[num2] == 1)
				{
					Kube.GPS.inventarWeapons[num2] = num + 10000000;
				}
				Kube.GPS.inventarWeapons[num2] = (int)Time.time + (Kube.GPS.inventarWeapons[num2] - num);
				num2++;
			}
			Kube.SS.serverTime = (float)num;
			if (Kube.GPS.inventarWeapons[this.selectedInventarItem.Num] > (int)Time.time)
			{
				this.chosenInventarItem.Num = this.selectedInventarItem.Num;
			}
			Kube.SendMonoMessage("WeaponsUpdate", new object[0]);
		}
		else
		{
			Kube.SS.SendStat("charles");
		}
	}

	private void BuySpecItemDone(string str)
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
		string text = string.Empty + Kube.GPS.playerId;
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
			int num = Convert.ToInt32(array[array.Length - 1]);
			int num2 = 0;
			while (num2 < array2.Length && num2 < Kube.GPS.inventarSpecItems.Length)
			{
				Kube.GPS.inventarSpecItems[num2] = Convert.ToInt32(array2[num2]);
				Kube.GPS.inventarSpecItems[num2] = (int)Time.time + (Kube.GPS.inventarSpecItems[num2] - num);
				num2++;
			}
			Kube.SS.serverTime = (float)num;
			Kube.SendMonoMessage("ItemsCubesUpdate", new object[0]);
		}
		else
		{
			Kube.SS.SendStat("charles");
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

	public int UseItem(int itemNum)
	{
		int num = Kube.GPS.inventarItems[itemNum];
		if (Kube.GPS.inventarItems[itemNum] > 0)
		{
			Kube.GPS.inventarItems[itemNum]--;
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
			while (num < array2.Length && num < Kube.GPS.inventarItemsLength)
			{
				Kube.GPS.inventarItems[num] = Convert.ToInt32(array2[num]);
				num++;
			}
		}
	}

	public Texture2D[] popTex;

	public int[] inventarCubes;

	public int[] cubesNatureNums;

	public int[] cubesBuilderNums;

	public int[] cubesDecorNums;

	public int[] cubesGlassNums;

	public int[] cubesWaterNums;

	public int[] cubesDifferentNums;

	public int[] inventarWeaponsNums;

	public InventoryScript.WeaponParams[] weaponParams;

	public InventoryScript.BonusParams[] bonusParams;

	public InventoryScript.BulletParams[] bulletParams;

	public int[] inventarBullets;

	public InventarType[] menuXref = new InventarType[]
	{
		InventarType.cubes,
		InventarType.decor,
		InventarType.items,
		InventarType.items,
		InventarType.weapons,
		InventarType.none,
		InventarType.none,
		InventarType.none
	};

	[HideInInspector]
	public InventoryScript.GameItemGOLoader gameItemsGO;

	[HideInInspector]
	public InventoryScript.WeaponGOLoader charWeaponsGO;

	public int[] decorLightNums;

	public int[] decorFurnitureNums;

	public int[] decorDoorsNums;

	public int[] decorLeddersNums;

	public int[] decorGreenNums;

	public int[] decorDecorNums;

	public int[] decorLocationNums;

	public int[] decorRoadNums;

	public int[] decorWeaponsNums;

	public int[] decorMonstersNums;

	public int[] itemsAbilsNums;

	public int[] itemsBattleAbilsNums;

	public int[] itemsAANums;

	public int[] itemsSwitchNums;

	public int[] devicesOtherNums;

	public int[] charMovesNums;

	public int[] specItemsNums;

	public int[] shopHats;

	public int[] shopTors;

	public int[] shopBack;

	public int[] shopArms;

	public int[] shopFoots;

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

	private InventarMenu _inventaryType;

	private int _inventoryPageType;

	public FastInventar selectedInventarItem;

	public FastInventar chosenInventarItem;

	private Vector2 invWeaponScroll = Vector2.zero;

	private int chosenSkin;

	private int chosenClothesIndex = -1;

	private float goldToMoneySlider;

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

	public Texture buyForMoneyLight;

	public Texture buyForGoldLight;

	public Texture bankGold;

	public Texture bankMoney;

	public Texture fastInventarTex;

	public Texture fastInventarDarkTex;

	public Texture fastInventarTex0;

	public Texture charParamsTex;

	public Texture[] stars;

	public Texture charParamsBuyTex;

	public Texture expStar;

	public Texture moneyTex;

	public Texture goldTex;

	public Texture buttonArrows;

	public Texture HIT;

	public Texture x2Tex;

	public Texture needLevel;

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

	private string playerName;

	public PlayerScript ps;

	[Serializable]
	public enum WeaponGroup
	{
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

		public int weaponType;

		public int UsingBullets;

		public int[] clipSize;

		public int currentClipSizeIndex;

		public float[] reloadTime;

		public int currentReloadTimeIndex;

		public int BulletsType;

		public float[] DeltaShotArray;

		public int currentDeltaShotIndex;

		public float DeltaShot;

		public float[] Damage;

		public int currentDamageIndex;

		public float[] Accuracy;

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

	public class GameItemGOLoader
	{
		public GameObject this[int index]
		{
			get
			{
				if (Kube.ASS3 != null && Kube.ASS3.gameItemsGO[index] != null)
				{
					return Kube.ASS3.gameItemsGO[index];
				}
				if (this.hash.ContainsKey(index))
				{
					return this.hash[index];
				}
				GameObject gameObject = Kube.SS.FindItemAsset(index);
				this.hash[index] = gameObject;
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

	public class WeaponGOLoader
	{
		public GameObject this[int index]
		{
			get
			{
				if (Kube.ASS6 != null && Kube.ASS6.charWeaponsGO[index] != null)
				{
					return Kube.ASS6.charWeaponsGO[index];
				}
				GameObject gameObject = null;
				if (this.hash.ContainsKey(index))
				{
					gameObject = this.hash[index];
				}
				if (gameObject == null)
				{
					gameObject = Kube.SS.FindAsset("WeaponGO", index);
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
