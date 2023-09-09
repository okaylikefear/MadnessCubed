using System;
using System.Collections.Generic;
using kube;
using kube.data;
using UnityEngine;

public class OfferDialogBank : OfferDialog
{
	protected override void OfferInit()
	{
		using (Dictionary<int, float>.KeyCollection.Enumerator enumerator = OfferBox.bank.Keys.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				int first_item = enumerator.Current;
				this._first_item = first_item;
			}
		}
		this.GoldInfo(this._first_item);
	}

	protected void GoldInfo(int k)
	{
		Texture x = null;
		if (Kube.SN.hasMoneyIcon)
		{
			x = Kube.SN.moneyIconTx;
		}
		string moneyName = Kube.SN.moneyName;
		string text = string.Empty;
		float num = Kube.GPS.exchangeSpec[k, 0] * Kube.SN.moneyValue;
		if (num != Mathf.Round(num))
		{
			text = num.ToString("0.#");
		}
		else
		{
			text = num.ToString("0");
		}
		this.snValue.text = text;
		this.snIcon.gameObject.SetActive(x != null);
		this.goldValue.text = Kube.GPS.exchangeSpec[k, 3].ToString();
	}

	private void Update()
	{
	}

	public void onBank()
	{
		base.gameObject.SetActive(false);
		MainMenu.ShowBank();
	}

	protected int _first_item;

	public UILabel snValue;

	public UILabel goldValue;

	public UITexture snIcon;
}
