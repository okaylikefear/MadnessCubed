using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class ViralDialog : MonoBehaviour
{
	private void Start()
	{
		this._missions = Kube.SN.getMissions();
		for (int i = 0; i < 4; i++)
		{
			UIToggle componentInChildren = this.missions[i].GetComponentInChildren<UIToggle>();
			UILabel componentInChildren2 = this.missions[i].GetComponentInChildren<UILabel>();
			UIButton componentInChildren3 = this.missions[i].GetComponentInChildren<UIButton>();
			componentInChildren.value = Kube.SN.isMissionDone(i);
			componentInChildren2.text = this._missions[i].name;
			EventDelegate.Add(componentInChildren3.onClick, new EventDelegate(new EventDelegate.Callback(this.onMissionClick)));
		}
	}

	private void onMissionClick()
	{
		int num = Array.IndexOf<GameObject>(this.missions, UIButton.current.gameObject);
		Kube.SN.gotoMission(this._missions[num].id);
	}

	private void OnEnable()
	{
		Kube.SS.require("Assets2");
		bool flag = true;
		for (int i = 0; i < 4; i++)
		{
			UIToggle componentInChildren = this.missions[i].GetComponentInChildren<UIToggle>();
			componentInChildren.value = Kube.SN.isMissionDone(i);
			flag &= Kube.SN.isMissionDone(i);
		}
		this._items = Kube.SN.socialQuest.bonus;
		for (int j = 0; j < this._items.Length; j++)
		{
			KeyValuePair<int, int> keyValuePair = this._items[j];
			int key = keyValuePair.Key;
			this.items[j].fi = new FastInventar(3, key);
			this.items[j].count = keyValuePair.Value;
		}
		this.money1.text = Kube.SN.socialQuest.money;
		this.money2.text = Kube.SN.socialQuest.gold;
		if (Kube.SN.isQuestDone())
		{
			flag = false;
		}
		this.collect.isEnabled = flag;
	}

	protected void OnTakeBonus(string data)
	{
		if (data != "ok")
		{
			return;
		}
		Kube.SN.QuestDone();
		Kube.GPS.playerMoney1 += 2500;
		Kube.GPS.playerMoney2 += 2;
		for (int i = 0; i < this.items.Length; i++)
		{
			int key = this._items[i].Key;
			int value = this._items[i].Value;
			Kube.GPS.inventarItems[key] += value;
		}
	}

	protected void TakeBonus()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["id"] = Kube.GPS.playerId.ToString();
		Kube.SS.Request(902, dictionary, new ServerScript.ServerCallback(this.OnTakeBonus));
	}

	public void onPostClick()
	{
		this.TakeBonus();
		base.gameObject.SetActive(false);
	}

	public GameObject[] missions;

	private SnMissionDesc[] _missions;

	public UILabel money1;

	public UILabel money2;

	public ItemDescIcon[] items;

	public UIButton collect;

	private KeyValuePair<int, int>[] _items;
}
