using System;
using kube;
using UnityEngine;

public class BankMenu : MonoBehaviour
{
	private void Start()
	{
		if (base.enabled)
		{
			this.OnEnable();
		}
	}

	private void Awake()
	{
		int length = Kube.GPS.exchangeSpec.GetLength(0);
		Texture texture = null;
		string moneyName = Kube.SN.moneyName;
		bool flag = Kube.GPS.playerVoices <= 0;
		if (Kube.SN.hasMoneyIcon)
		{
			texture = Kube.SN.moneyIconTx;
		}
		this.buttons = new BankItem[length];
		this.freeGold.gameObject.SetActive(Kube.SN.hasFreeGold);
		for (int i = 0; i < length; i++)
		{
			int num = length - 1 - i;
			GameObject gameObject = this.container.gameObject.AddChild(this.itemPrefab);
			BankItem component = gameObject.GetComponent<BankItem>();
			component.sprite.spriteName = (num + 1).ToString() + "_new_gold_bank";
			string empty = string.Empty;
			float num2 = Kube.GPS.exchangeSpec[num, 0] * Kube.SN.moneyValue;
			if (texture)
			{
				component.tx.mainTexture = texture;
			}
			component.tx.gameObject.SetActive(texture);
			component.moneyName.gameObject.SetActive(!texture);
			component.moneyName.text = moneyName;
			UIButton componentInChildren = component.GetComponentInChildren<UIButton>();
			EventDelegate.Add(componentInChildren.onClick, new EventDelegate.Callback(this.onBuy));
			this.buttons[i] = component;
		}
		this.container.GetComponent<UIGrid>().Reposition();
	}

	private void OnEnable()
	{
		this.onScroll();
		bool flag = Kube.GPS.playerVoices <= 0;
		for (int i = 0; i < this.buttons.Length; i++)
		{
			int num = this.buttons.Length - 1 - i;
			BankItem bankItem = this.buttons[i];
			string text = string.Empty;
			float num2 = Kube.GPS.exchangeSpec[num, 0] * Kube.SN.moneyValue;
			if (num2 != Mathf.Round(num2))
			{
				text = num2.ToString("0.#");
			}
			else
			{
				text = num2.ToString("0");
			}
			float num3 = Kube.GPS.exchangeSpec[num, 3];
			float num4 = num3;
			if (flag)
			{
				num3 *= 2f;
			}
			bankItem.money1.text = text;
			bankItem.money2.text = num3.ToString();
			bankItem.oldPrice.gameObject.SetActive(flag);
			bankItem.oldPrice.text = num4.ToString();
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
		int num = Array.IndexOf<BankItem>(this.buttons, UIButton.current.GetComponentInParent<BankItem>());
		int num2 = this.buttons.Length - 1 - num;
		Kube.SN.ShowPayment(num2, Kube.IS.gameObject, "PaymentAnswer");
		Kube.SS.SendStat("monet" + num2);
	}

	public void onFree()
	{
		Kube.SN.ShowFreeGold();
	}

	protected BankItem[] buttons;

	public UIScrollView container;

	public GameObject itemPrefab;

	public UILabel money1;

	public UILabel money2;

	public UIScrollBar scroll;

	public GameObject convertGroup;

	public UILabel firstTwice;

	public UIButton freeGold;
}
