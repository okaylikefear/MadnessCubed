using System;
using kube;
using UnityEngine;

public class VIPMenu : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public static string ExpriteTime(float par1)
	{
		int num = Mathf.CeilToInt((par1 - Time.time) / 86400f);
		int num2 = Mathf.FloorToInt((par1 - Time.time) / 3600f);
		int num3 = Mathf.FloorToInt((par1 - Time.time) % 3600f / 60f);
		string str;
		if (num > 1)
		{
			str = string.Format(Localize.is_days_left, num);
		}
		else
		{
			str = string.Format(Localize.is_hours_left, num2, num3);
		}
		if (num > 30)
		{
			return Localize.is_unlimit;
		}
		return Localize.is_time_left + str;
	}

	private void EventBuyVIPDone()
	{
		this.OnEnable();
	}

	private void OnEnable()
	{
		this.label.text = string.Format(Localize.vip_info, Kube.GPS.vipBonus);
		bool flag = false;
		if (Kube.GPS.vipEnd > Time.time)
		{
			flag = true;
		}
		this.activate.gameObject.SetActive(!flag);
		this.vipactive.SetActive(flag);
		this.price.text.text = Localize.is_one_day + " - " + Kube.GPS.vipPrice[0, 0];
		this.price.isGold = true;
		this.price.gameObject.SetActive(!flag);
		this.expireLabel.text = VIPMenu.ExpriteTime(Kube.GPS.vipEnd);
	}

	public void onActivate()
	{
		Cub2UI.FindAndOpenDialog("dialog_rent_vip");
	}

	public UILabel label;

	public UIButton activate;

	public PriceButton price;

	public GameObject vipactive;

	public UILabel expireLabel;
}
