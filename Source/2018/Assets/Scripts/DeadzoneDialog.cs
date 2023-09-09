using System;
using System.Collections.Generic;
using kube;
using kube.data;
using UnityEngine;

public class DeadzoneDialog : MonoBehaviour
{
	private void Start()
	{
		for (int i = 0; i < this.missions.Length; i++)
		{
			UIButton componentInChildren = this.missions[i].GetComponentInChildren<UIButton>();
			EventDelegate.Add(componentInChildren.onClick, new EventDelegate(new EventDelegate.Callback(this.onMissionClick)));
		}
		this.desc.text = string.Format(Localize.instal_android_many, Kube.SN.getViralEvent(0).gold);
	}

	private void onMissionClick()
	{
		int num = Array.IndexOf<GameObject>(this.missions, UIButton.current.gameObject);
		Kube.SN.gotoViralTask(0, num);
		this.missions[num].GetComponentInChildren<UIToggle>().value = Kube.SN.isViralTaskDone(0, num);
		this.collect.isEnabled = (Kube.SN.getViralEvent(0).state == 7);
	}

	private void OnEnable()
	{
		Kube.RM.require("Assets2", null);
		bool flag = true;
		for (int i = 0; i < this.missions.Length; i++)
		{
			UIToggle componentInChildren = this.missions[i].GetComponentInChildren<UIToggle>();
			componentInChildren.value = Kube.SN.isViralTaskDone(0, i);
			flag &= Kube.SN.isViralTaskDone(0, i);
		}
		this.money2.text = Kube.SN.getViralEvent(0).gold.ToString();
		if (Kube.SN.isViralEventDone(0, null))
		{
			flag = false;
		}
		this.collect.isEnabled = flag;
	}

	protected void OnTakeBonus()
	{
		GameParamsScript gps = Kube.GPS;
		gps.playerMoney2 += Kube.SN.getViralEvent(0).gold;
	}

	protected void TakeBonus()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Kube.SN.EventDone(0, new VoidCallback(this.OnTakeBonus));
	}

	public void onPostClick()
	{
		this.TakeBonus();
		base.gameObject.SetActive(false);
	}

	public GameObject[] missions;

	public UILabel desc;

	public UILabel money2;

	public UIButton collect;
}
