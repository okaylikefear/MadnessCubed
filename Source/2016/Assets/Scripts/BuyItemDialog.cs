using System;
using kube;
using UnityEngine;

public class BuyItemDialog : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		this.itemCount = 1;
		this.title.text = Localize.gameItemsNames[this.itemId];
		this.tx.mainTexture = Kube.OH.gameItemsTex[this.itemId];
		this.slider.value = 0f;
		this.Price = Kube.GPS.inventarItemPrice1[this.itemId];
		if (this.Price == 0)
		{
			this.Price = Kube.GPS.inventarItemPrice2[this.itemId];
			this.btn.isGold = true;
		}
		else
		{
			this.btn.isGold = false;
		}
		this.UpdateText(true);
	}

	public void onSlider()
	{
		if (this._changing)
		{
			return;
		}
		this.itemCount = 1 + Mathf.RoundToInt(this.slider.value * 100f);
		this.UpdateText(true);
	}

	public void onEditCount()
	{
		if (this._changing)
		{
			return;
		}
		int.TryParse(this.cnt.text, out this.itemCount);
		this.UpdateText(false);
	}

	public void onInc()
	{
		if (this._changing)
		{
			return;
		}
		this.itemCount++;
		this.UpdateText(true);
	}

	public void onDec()
	{
		if (this._changing)
		{
			return;
		}
		if (this.itemCount <= 1)
		{
			return;
		}
		this.itemCount--;
		this.UpdateText(true);
	}

	private void UpdateText(bool flag = true)
	{
		if (this._changing)
		{
			return;
		}
		this._changing = true;
		if (flag)
		{
			this.cnt.text = this.itemCount.ToString();
		}
		this.btn.text.text = (this.itemCount * this.Price).ToString();
		this.slider.value = (float)(this.itemCount - 1) / 100f;
		this._changing = false;
	}

	public void onBuy()
	{
		int num;
		if (this.btn.isGold)
		{
			num = Kube.GPS.playerMoney2;
		}
		else
		{
			num = Kube.GPS.playerMoney1;
		}
		if (this.itemCount * this.Price > num)
		{
			MainMenu.ShowBank();
			return;
		}
		Kube.SS.BuyItem(this.itemId, this.itemCount, Kube.IS.gameObject, "BuyItemDone");
		Kube.SN.PostItemOnWall(this.itemId);
		base.gameObject.SetActive(false);
	}

	public int itemId;

	public UILabel title;

	public UITexture tx;

	public UIInput cnt;

	public UISlider slider;

	public PriceButton btn;

	public int itemCount;

	private int Price;

	protected bool _changing;
}
