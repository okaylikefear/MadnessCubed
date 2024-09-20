using System;
using kube;
using UnityEngine;

public class UpgradePlayerDialog : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Show(int numParam)
	{
		this.numParam = numParam;
		int[] array = new int[]
		{
			Kube.GPS.playerHealth,
			Kube.GPS.playerArmor,
			Kube.GPS.playerSpeed,
			Kube.GPS.playerJump,
			Kube.GPS.playerDefend
		};
		this.param = array[numParam];
		if (this.param >= 7)
		{
			this.maximumLabel.gameObject.SetActive(true);
			this.btn.gameObject.SetActive(false);
			this.price.gameObject.SetActive(false);
			this.priceUnlock.gameObject.SetActive(false);
		}
		else
		{
			this.maximumLabel.gameObject.SetActive(false);
			this.btn.gameObject.SetActive(true);
			this.price.gameObject.SetActive(true);
			this.priceUnlock.gameObject.SetActive(true);
		}
		string[] array2 = new string[]
		{
			Localize.params_health,
			Localize.params_armor,
			Localize.param_speed,
			Localize.param_jump,
			Localize.param_defend
		};
		this.goodLevel = (Kube.GPS.playerLevel >= (int)Kube.GPS.charParamsPrice[numParam, this.param, 0]);
		this.CheckMoney();
		this.title.text = string.Concat(new object[]
		{
			array2[numParam],
			" - ",
			Localize.needParamsToBuyLevel,
			" ",
			this.param + 1
		});
		base.gameObject.SetActive(true);
		if (this.goodLevel)
		{
			this.btnText.text = Localize.Upgrade;
		}
		else
		{
			this.btnText.text = Localize.Unlock;
		}
		this.btn.isEnabled = true;
		this.minLevelLabel.text = string.Format(Localize.need_level, (int)Kube.GPS.charParamsPrice[numParam, this.param, 0]);
		this.minLevelLabel.gameObject.SetActive(!this.goodLevel && this.param < 7);
	}

	private void UpgradeParamDone(string str)
	{
		Kube.IS.SendMessage("UpgradeParamDone", str);
		base.gameObject.SetActive(false);
	}

	private void CheckMoney()
	{
		int num = Mathf.FloorToInt(Kube.GPS.charParamsPrice[this.numParam, this.param, 1]);
		if (num == 0)
		{
			num = Mathf.FloorToInt(Kube.GPS.charParamsPrice[this.numParam, this.param, 2]);
			this.isGold = true;
		}
		else
		{
			this.isGold = false;
		}
		if (!this.goodLevel)
		{
			num *= 2;
		}
		if (this.isGold)
		{
			this.canBuy = (Kube.GPS.playerMoney2 >= num);
		}
		else
		{
			this.canBuy = (Kube.GPS.playerMoney1 >= num);
		}
		this.price.isGold = this.isGold;
		this.price.text.text = num.ToString();
		this.priceUnlock.isGold = this.isGold;
		this.priceUnlock.text.text = num.ToString();
		if (this.param < 7)
		{
			if (this.goodLevel)
			{
				this.price.gameObject.SetActive(true);
				this.priceUnlock.gameObject.SetActive(false);
			}
			else
			{
				this.priceUnlock.gameObject.SetActive(true);
				this.price.gameObject.SetActive(false);
			}
		}
	}

	public void OnUpgradeClick()
	{
		this.CheckMoney();
		if (this.canBuy)
		{
			if (this.goodLevel)
			{
				Kube.SS.UpgradeParam(this.numParam, base.gameObject, "UpgradeParamDone");
			}
			else
			{
				Kube.SS.UpgradeParamUnlock(this.numParam, base.gameObject, "UpgradeParamDone");
			}
			this.btn.isEnabled = false;
		}
		else
		{
			MainMenu.ShowBank();
		}
	}

	public PriceButton price;

	public UILabel title;

	public UIButton btn;

	public UILabel btnText;

	public PriceButton priceUnlock;

	public UILabel minLevelLabel;

	public UILabel maximumLabel;

	protected int numParam;

	protected bool canBuy;

	private bool goodLevel;

	private int param;

	private bool isGold;
}
