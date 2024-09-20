using System;
using kube;
using UnityEngine;

public class VIPDialog : MonoBehaviour
{
	private void Start()
	{
		this._subs = Kube.SN.Get<ISubscriptions>();
		string[] array = new string[]
		{
			Localize.is_one_day,
			Localize.is_one_week,
			Localize.is_one_mounth
		};
		for (int i = 0; i < this.buttons.Length; i++)
		{
			GameObject gameObject = this.buttons[i];
			PriceButton componentInChildren = gameObject.GetComponentInChildren<PriceButton>(true);
			componentInChildren.text.text = array[i] + " - " + Kube.GPS.vipPrice[i, 0];
			componentInChildren.isGold = true;
			if (this._subs != null && i == 2)
			{
				PriceButton componentInChildren2 = this.buttons[2].GetComponentInChildren<PriceButton>(true);
				GameObject gameObject2 = componentInChildren2.gameObject;
				RealPriceButton realPriceButton = gameObject2.AddComponent<RealPriceButton>();
				realPriceButton.text = componentInChildren2.text;
				realPriceButton.gold = componentInChildren2.gold.gameObject.AddComponent<UITexture>();
				realPriceButton.gold.width = componentInChildren2.gold.height;
				realPriceButton.gold.height = componentInChildren2.gold.height;
				realPriceButton.center = true;
				realPriceButton.str = array[i] + " - {0}";
				realPriceButton.value = Kube.GPS.vipPrice[i, 0];
				realPriceButton.gold.depth = componentInChildren2.gold.depth;
				UnityEngine.Object.Destroy(componentInChildren2.gold);
				UnityEngine.Object.Destroy(componentInChildren2);
			}
		}
	}

	private void Update()
	{
	}

	public static string ExpriteTime(float par1)
	{
		int num = Mathf.RoundToInt((par1 - Time.time) / 86400f);
		int num2 = Mathf.FloorToInt((par1 - Time.time) / 3600f);
		int num3 = Mathf.FloorToInt((par1 - Time.time) % 3600f / 60f);
		string str;
		if (num >= 1)
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
		this.expireLabel.text = VIPDialog.ExpriteTime(Kube.GPS.vipEnd);
	}

	public void onActivate()
	{
		GameObject gameObject = UIButton.current.gameObject;
		int num = Array.IndexOf<GameObject>(this.buttons, gameObject);
		if (this._subs != null && num == 2)
		{
			this._subs.BuySubs();
			return;
		}
		int num2 = Kube.GPS.vipPrice[num, 0];
		int num3 = Kube.GPS.playerMoney2;
		if (num2 > num3)
		{
			MainMenu.ShowBank();
			return;
		}
		if (num != -1)
		{
			Kube.SS.BuyVIP(num, Kube.IS.gameObject, "BuyVIPDone");
			base.gameObject.SetActive(false);
		}
	}

	public GameObject activate;

	public PriceButton price;

	public GameObject vipactive;

	public UILabel expireLabel;

	public GameObject[] buttons;

	protected ISubscriptions _subs;
}
