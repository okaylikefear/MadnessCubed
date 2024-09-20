using System;
using System.Collections;
using kube;
using UnityEngine;

public class WeaponDialog : MonoBehaviour
{
	private void Start()
	{
		for (int i = 0; i < this.buttons.Length; i++)
		{
			GameObject gameObject = this.buttons[i];
			gameObject.GetComponent<UIButton>().onClick.Add(new EventDelegate(new EventDelegate.Callback(this.onBuyClick)));
		}
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		base.StartCoroutine(this._loadTx());
		string[] array = new string[]
		{
			Localize.is_one_day,
			Localize.is_one_week,
			Localize.is_unlimit
		};
		for (int i = 0; i < this.buttons.Length; i++)
		{
			GameObject gameObject = this.buttons[i];
			if (Kube.GPS.weaponsPrice1[this.weaponId, i] == 0 && Kube.GPS.weaponsPrice2[this.weaponId, i] == 0)
			{
				gameObject.SetActive(false);
			}
			else
			{
				gameObject.SetActive(true);
				PriceButton componentInChildren = gameObject.GetComponentInChildren<PriceButton>();
				if (Kube.GPS.weaponsPrice1[this.weaponId, i] > 0)
				{
					componentInChildren.text.text = array[i] + " - " + Kube.GPS.weaponsPrice1[this.weaponId, i];
					componentInChildren.isGold = false;
				}
				else
				{
					componentInChildren.text.text = array[i] + " - " + Kube.GPS.weaponsPrice2[this.weaponId, i];
					componentInChildren.isGold = true;
				}
			}
		}
		this.title.text = Localize.weaponNames[this.weaponId];
		string text = Localize.needParamsToBuy1;
		int[,] array2 = new int[5, 2];
		array2[0, 0] = Kube.GPS.playerHealth;
		array2[0, 1] = Kube.IS.weaponParams[this.weaponId].needHealthLevel;
		array2[1, 0] = Kube.GPS.playerArmor;
		array2[1, 1] = Kube.IS.weaponParams[this.weaponId].needArmorLevel;
		array2[2, 0] = Kube.GPS.playerSpeed;
		array2[2, 1] = Kube.IS.weaponParams[this.weaponId].needSpeedLevel;
		array2[3, 0] = Kube.GPS.playerJump;
		array2[3, 1] = Kube.IS.weaponParams[this.weaponId].needJumpLevel;
		array2[4, 0] = Kube.GPS.playerDefend;
		array2[4, 1] = Kube.IS.weaponParams[this.weaponId].needResistLevel;
		bool flag = false;
		if (Kube.GPS.playerHealth < Kube.IS.weaponParams[this.weaponId].needHealthLevel)
		{
			flag = true;
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"\n",
				Localize.params_health,
				": ",
				Localize.needParamsToBuyLevel,
				" ",
				Kube.IS.weaponParams[this.weaponId].needHealthLevel
			});
		}
		if (Kube.GPS.playerArmor < Kube.IS.weaponParams[this.weaponId].needArmorLevel)
		{
			flag = true;
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"\n",
				Localize.params_armor,
				": ",
				Localize.needParamsToBuyLevel,
				" ",
				Kube.IS.weaponParams[this.weaponId].needArmorLevel
			});
		}
		if (Kube.GPS.playerSpeed < Kube.IS.weaponParams[this.weaponId].needSpeedLevel)
		{
			flag = true;
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"\n",
				Localize.param_speed,
				": ",
				Localize.needParamsToBuyLevel,
				" ",
				Kube.IS.weaponParams[this.weaponId].needSpeedLevel
			});
		}
		if (Kube.GPS.playerJump < Kube.IS.weaponParams[this.weaponId].needJumpLevel)
		{
			flag = true;
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"\n",
				Localize.param_jump,
				": ",
				Localize.needParamsToBuyLevel,
				" ",
				Kube.IS.weaponParams[this.weaponId].needJumpLevel
			});
		}
		if (Kube.GPS.playerDefend < Kube.IS.weaponParams[this.weaponId].needResistLevel)
		{
			flag = true;
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"\n",
				Localize.param_defend,
				": ",
				Localize.needParamsToBuyLevel,
				" ",
				Kube.IS.weaponParams[this.weaponId].needResistLevel
			});
		}
		if (flag)
		{
			for (int j = 0; j < this.buttons.Length; j++)
			{
				this.buttons[j].SetActive(false);
			}
			this.needParamsPanel.SetActive(true);
			this.needParamsLabel.text = text;
			this.unlockMoney = 0;
			for (int k = 0; k < 5; k++)
			{
				for (int l = array2[k, 0]; l < array2[k, 1]; l++)
				{
					int num = Mathf.FloorToInt(Kube.GPS.charParamsPrice[k, l, 2]);
					num *= 2;
					if (num == 0)
					{
						num = 1;
					}
					this.unlockMoney += num;
				}
			}
			this.unlockButtonLabel.text = Localize.needParamsUpgradeNow + " " + this.unlockMoney;
			this.unlockButtonLabel.transform.parent.gameObject.GetComponent<UIButton>().enabled = true;
			this.unlockButtonLabel.transform.parent.gameObject.GetComponent<UIButton>().onClick.Add(new EventDelegate(new EventDelegate.Callback(this.onUnlockClick)));
		}
		else
		{
			this.needParamsPanel.SetActive(false);
		}
	}

	private void onBuyClick()
	{
		GameObject gameObject = UIButton.current.gameObject;
		int num = Array.IndexOf<GameObject>(this.buttons, gameObject);
		int num2 = Kube.GPS.weaponsPrice1[this.weaponId, num];
		bool flag = false;
		if (num2 == 0)
		{
			num2 = Kube.GPS.weaponsPrice2[this.weaponId, num];
			flag = true;
		}
		int num3;
		if (flag)
		{
			num3 = Kube.GPS.playerMoney2;
		}
		else
		{
			num3 = Kube.GPS.playerMoney1;
		}
		if (num2 > num3)
		{
			MainMenu.ShowBank();
			return;
		}
		if (num != -1)
		{
			Kube.SS.BuyWeapon(this.weaponId, num, Kube.IS.gameObject, "BuyWeaponDone");
			Kube.SN.PostWeaponOnWall(this.weaponId);
			base.gameObject.SetActive(false);
		}
	}

	private void onUnlockClick()
	{
		if (Kube.GPS.playerMoney2 >= this.unlockMoney)
		{
			Kube.SS.UpgradeParamAllUnlock(Kube.IS.weaponParams[this.weaponId].needHealthLevel, Kube.IS.weaponParams[this.weaponId].needArmorLevel, Kube.IS.weaponParams[this.weaponId].needSpeedLevel, Kube.IS.weaponParams[this.weaponId].needJumpLevel, Kube.IS.weaponParams[this.weaponId].needResistLevel, this.unlockMoney, base.gameObject, "UpgradeParamDone");
			this.unlockButtonLabel.transform.parent.gameObject.GetComponent<UIButton>().enabled = false;
			return;
		}
		MainMenu.ShowBank();
	}

	private void UpgradeParamDone(string[] strs)
	{
		Kube.IS.SendMessage("UpgradeParamDone", strs);
		this.OnEnable();
	}

	private IEnumerator _loadTx()
	{
		while (Kube.ASS2 == null)
		{
			yield return new WaitForSeconds(2f);
		}
		Texture texture = Kube.ASS2.inventarWeaponsTex[this.weaponId];
		this.tx.mainTexture = texture;
		this.tx.width = texture.width;
		this.tx.height = texture.height;
		yield break;
	}

	public int weaponId;

	public UITexture tx;

	public GameObject[] buttons;

	public UILabel title;

	public UILabel desc;

	public GameObject needParamsPanel;

	public UILabel needParamsLabel;

	public UILabel unlockButtonLabel;

	private int unlockMoney;
}
