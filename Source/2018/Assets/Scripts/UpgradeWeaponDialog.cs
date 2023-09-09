using System;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class UpgradeWeaponDialog : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Show(int weaponId, int numParam)
	{
		this.numParam = numParam;
		this.weaponId = weaponId;
		if (numParam == 4)
		{
			this.ShowBullets(weaponId, numParam);
		}
		else
		{
			this.ShowUpgrade();
		}
		base.gameObject.SetActive(true);
	}

	private void ShowUpgrade()
	{
		this.canBuy = true;
		this.btn.isEnabled = true;
		WeaponParamsObj weaponParamsObj = Kube.IS.weaponParams[this.weaponId];
		WeaponUpgrade.getUpgradeData(this.weaponId);
		int[] array = new int[5];
		array[0] = weaponParamsObj.currentDamageIndex;
		array[1] = weaponParamsObj.currentAccuracyIndex;
		array[2] = weaponParamsObj.currentDeltaShotIndex;
		array[3] = weaponParamsObj.currentClipSizeIndex;
		int[] array2 = array;
		PriceValue priceValue = Kube.GPS.upgradePrice[this.weaponId, this.numParam, array2[this.numParam]];
		this.price.isGold = priceValue.isGold;
		this.price.text.text = priceValue.price.ToString();
		if (priceValue.isGold)
		{
			this.canBuy = (Kube.GPS.playerMoney2 >= priceValue.price);
		}
		else
		{
			this.canBuy = (Kube.GPS.playerMoney1 >= priceValue.price);
		}
		string[] weapon_upgrade_name = Localize.weapon_upgrade_name;
		this.title.text = string.Concat(new object[]
		{
			weapon_upgrade_name[this.numParam],
			" - ",
			Localize.needParamsToBuyLevel,
			" ",
			array2[this.numParam] + 1
		});
	}

	protected void ShowBullets(int weaponId, int numParam)
	{
		bool flag = false;
		this.bt = Kube.IS.weaponParams[weaponId].BulletsType;
		int initialAmount = Kube.IS.bulletParams[this.bt].initialAmount;
		int[] array = new int[]
		{
			Kube.GPS.playerHealth,
			Kube.GPS.playerArmor,
			Kube.GPS.playerSpeed,
			Kube.GPS.playerJump,
			Kube.GPS.playerDefend
		};
		int num = array[numParam];
		this.q = Kube.IS.bulletParams[this.bt].initialAmountIndex;
		int num2 = Mathf.FloorToInt((float)Kube.GPS.bulletsPrice[this.bt, this.q, 1]);
		if (num2 == 0)
		{
			num2 = Mathf.FloorToInt((float)Kube.GPS.bulletsPrice[this.bt, this.q, 2]);
			flag = true;
		}
		if (flag)
		{
			this.canBuy = (Kube.GPS.playerMoney2 >= num2);
		}
		else
		{
			this.canBuy = (Kube.GPS.playerMoney1 >= num2);
		}
		this.price.isGold = flag;
		this.price.text.text = num2.ToString();
		this.title.text = Localize.is_initial_ammo + " " + Localize.bulletsNames[this.bt];
		bool flag2 = true;
		this.btn.isEnabled = flag2;
		this.minLevelLabel.text = Localize.need + " " + (int)Kube.GPS.charParamsPrice[numParam, num, 0];
		this.minLevelLabel.gameObject.SetActive(!flag2);
	}

	private void BuyBulletsDone(string[] strs)
	{
		Kube.IS.SendMessage("BuyBulletsDone", strs);
		Kube.SendMonoMessage("WeaponUpgradeEvent", new object[0]);
		base.gameObject.SetActive(false);
	}

	protected void UpgradeWeaponDone(JsonData json)
	{
		Kube.SendMonoMessage("WeaponUpgradeEvent", new object[0]);
		base.gameObject.SetActive(false);
	}

	public void OnUpgradeClick()
	{
		if (this.canBuy)
		{
			if (this.numParam == 4)
			{
				Kube.SS.BuyBullets(this.bt, this.q, base.gameObject, "BuyBulletsDone");
			}
			else
			{
				Kube.SS.UpgradeWeapon(this.weaponId, this.numParam, new JSONServerCallback(this.UpgradeWeaponDone));
			}
		}
		else
		{
			MainMenu.ShowBank();
		}
		this.btn.isEnabled = false;
	}

	public PriceButton price;

	public UILabel title;

	public UIButton btn;

	public UILabel minLevelLabel;

	protected int numParam;

	protected int weaponId;

	protected bool canBuy;

	protected int q;

	protected int bt;
}
