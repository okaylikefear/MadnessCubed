using System;
using kube;
using UnityEngine;

public class BankDialog : MonoBehaviour
{
	private void Start()
	{
		if (base.enabled)
		{
			this.OnEnable();
		}
	}

	private void OnEnable()
	{
		this.onScroll();
		this.convertGroup.SetActive(Kube.GPS.playerMoney2 > 0);
		Texture texture = null;
		if (Kube.SN.hasMoneyIcon)
		{
			texture = Kube.SN.moneyIconTx;
		}
		string moneyName = Kube.SN.moneyName;
		for (int i = 0; i < this.buttons.Length; i++)
		{
			int num = this.buttons.Length - 1 - i;
			BankButton component = this.buttons[i].GetComponent<BankButton>();
			string text = string.Empty;
			float num2 = Kube.GPS.exchangeSpec[num, 0] * Kube.SN.moneyValue;
			if (texture)
			{
				component.tx.mainTexture = texture;
			}
			component.tx.gameObject.SetActive(texture);
			component.moneyName.gameObject.SetActive(!texture);
			component.moneyName.text = moneyName;
			if (num2 != Mathf.Round(num2))
			{
				text = num2.ToString("0.#");
			}
			else
			{
				text = num2.ToString("0");
			}
			component.money1.text = text;
			component.money2.text = Kube.GPS.exchangeSpec[num, 3].ToString();
			component.Reposition();
		}
		if (Kube.GPS.playerVoices <= 0)
		{
			this.firstTwice.gameObject.SetActive(true);
		}
		else
		{
			this.firstTwice.gameObject.SetActive(false);
		}
	}

	public void onScroll()
	{
		int num = Mathf.FloorToInt(this.scroll.value * (float)Kube.GPS.playerMoney2);
		this.money1.text = (num * Kube.GPS.specToMoney).ToString();
		this.money2.text = num.ToString();
	}

	public void onConvert()
	{
		int numGold = Mathf.FloorToInt(this.scroll.value * (float)Kube.GPS.playerMoney2);
		Kube.SS.GoldToMoney(numGold, Kube.IS.gameObject, "GoldToMoneyDone");
		this.scroll.value = 0f;
	}

	public void onBuy()
	{
		int num = Array.IndexOf<UIButton>(this.buttons, UIButton.current);
		int num2 = this.buttons.Length - 1 - num;
		Kube.SN.ShowPayment(num2, Kube.IS.gameObject, "PaymentAnswer");
		Kube.SS.SendStat("monet" + num2);
	}

	public UIButton[] buttons;

	public UILabel money1;

	public UILabel money2;

	public UIScrollBar scroll;

	public GameObject convertGroup;

	public UILabel firstTwice;
}
