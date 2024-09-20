using System;
using kube;
using UnityEngine;

public class RentVIPDialog : MonoBehaviour
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
		string[] array = new string[]
		{
			Localize.is_one_day,
			Localize.is_one_week,
			Localize.is_one_mounth
		};
		for (int i = 0; i < this.buttons.Length; i++)
		{
			GameObject gameObject = this.buttons[i];
			PriceButton componentInChildren = gameObject.GetComponentInChildren<PriceButton>();
			componentInChildren.text.text = array[i] + " - " + Kube.GPS.vipPrice[i, 0];
			componentInChildren.isGold = true;
		}
	}

	private void onBuyClick()
	{
		GameObject gameObject = UIButton.current.gameObject;
		int num = Array.IndexOf<GameObject>(this.buttons, gameObject);
		int num2 = Kube.GPS.vipPrice[num, 0];
		bool flag = false;
		if (num2 == 0)
		{
			num2 = Kube.GPS.vipPrice[num, 1];
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
			Kube.SS.BuyVIP(num, Kube.IS.gameObject, "BuyVIPDone");
			base.gameObject.SetActive(false);
		}
	}

	public UITexture tx;

	public GameObject[] buttons;

	public UILabel title;

	public UILabel desc;
}
