using System;
using System.Collections.Generic;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class UnlockDialog : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public FastInventar fi
	{
		set
		{
			this._fi = value;
			this.needLevel = Kube.IS.needLevel(value);
			this.label.text = string.Format(Localize.need_level, this.needLevel);
			this.price.text = this.needLevel.ToString();
		}
	}

	public void Show()
	{
		this.label.text = string.Format(Localize.need_level, this.needLevel);
		this.price.text = this.needLevel.ToString();
	}

	private void onUnlocked(string response)
	{
		if (response == "0")
		{
			return;
		}
		JsonData unl = JsonMapper.ToObject(response);
		ItemUnlock.Parse(unl);
		GameParamsScript gps = Kube.GPS;
		gps.playerMoney2 -= this.needLevel;
		Kube.SendMonoMessage("UnlockEvent", new object[0]);
	}

	public void onUnlock()
	{
		if (Kube.GPS.playerMoney2 < this.needLevel)
		{
			MainMenu.ShowBank();
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["itemCode"] = this.itemCode;
		dictionary["needLevel"] = this.needLevel.ToString();
		Kube.SS.Request(36, dictionary, new ServerCallback(this.onUnlocked));
		base.gameObject.SetActive(false);
	}

	public UILabel label;

	public UILabel price;

	public int needLevel;

	protected FastInventar _fi;

	[NonSerialized]
	public string itemCode;
}
