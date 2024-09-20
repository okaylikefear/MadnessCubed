using System;
using System.Collections;
using kube;
using UnityEngine;

public class RentItemDialog : MonoBehaviour
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
		base.StartCoroutine(this._loadTx());
		string[] array = new string[]
		{
			Localize.is_one_day,
			Localize.is_one_week,
			Localize.is_unlimit
		};
		for (int i = 0; i < this.buttons.Length; i++)
		{
			GameObject gameObject = this.buttons[i];
			if (Kube.GPS.specItemsPrice1[this.itemId, i] == 0 && Kube.GPS.specItemsPrice2[this.itemId, i] == 0)
			{
				gameObject.SetActive(false);
			}
			else
			{
				gameObject.SetActive(true);
				PriceButton componentInChildren = gameObject.GetComponentInChildren<PriceButton>();
				if (Kube.GPS.specItemsPrice1[this.itemId, i] > 0)
				{
					componentInChildren.text.text = array[i] + " - " + Kube.GPS.specItemsPrice1[this.itemId, i];
					componentInChildren.isGold = false;
				}
				else
				{
					componentInChildren.text.text = array[i] + " - " + Kube.GPS.specItemsPrice2[this.itemId, i];
					componentInChildren.isGold = true;
				}
			}
		}
		this.title.text = Localize.specItemsName[this.itemId];
	}

	private void onBuyClick()
	{
		GameObject gameObject = UIButton.current.gameObject;
		int num = Array.IndexOf<GameObject>(this.buttons, gameObject);
		int num2 = Kube.GPS.specItemsPrice1[this.itemId, num];
		bool flag = false;
		if (num2 == 0)
		{
			num2 = Kube.GPS.specItemsPrice2[this.itemId, num];
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
			Kube.SS.BuySpecItem(this.itemId, num, Kube.IS.gameObject, "BuySpecItemDone");
			base.gameObject.SetActive(false);
		}
	}

	private IEnumerator _loadTx()
	{
		while (Kube.ASS2 == null)
		{
			yield return new WaitForSeconds(2f);
		}
		Texture texture = Kube.ASS2.specItemsInvTex[this.itemId];
		this.tx.mainTexture = texture;
		this.tx.width = texture.width;
		this.tx.height = texture.height;
		yield break;
	}

	public int itemId;

	public UITexture tx;

	public GameObject[] buttons;

	public UILabel title;

	public UILabel desc;
}
