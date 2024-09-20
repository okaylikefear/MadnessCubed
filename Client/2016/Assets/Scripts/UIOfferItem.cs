using System;
using kube.data;
using UnityEngine;

public class UIOfferItem : MonoBehaviour
{
	private void Start()
	{
		if (this.offer == null)
		{
			return;
		}
		this.label.gameObject.SetActive(this.offer.expireSeconds < 172800);
		base.InvokeRepeating("UpdateMinutes", 1f, 1f);
		this.sprite.spriteName = ("ico_offer_" + this.offer.type).ToString();
		this.UpdateMinutes();
	}

	private void UpdateMinutes()
	{
		if (this.offer.expireSeconds < 172800)
		{
			TimeSpan timeSpan = this.offer.expire - DateTime.UtcNow;
			string text = string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
			this.label.text = text;
		}
	}

	private void OnClick()
	{
		HomeMenu component = base.transform.parent.parent.GetComponent<HomeMenu>();
		component.ShowOffer(this.offer);
	}

	public UILabel label;

	public UISprite sprite;

	public Offer offer;
}
