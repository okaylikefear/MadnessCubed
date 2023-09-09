using System;
using kube;
using UnityEngine;

public class BonusDayDialog : MonoBehaviour
{
	private void Start()
	{
		this.tip.text = string.Format(Localize.daily_tip, Localize.plazm_grenade_gun);
	}

	public void Show(int bonusDay)
	{
		this._bonusDay = bonusDay;
		this.progress.fillAmount = 0.25f * (float)bonusDay;
		base.gameObject.SetActive(true);
		for (int i = 0; i < 5; i++)
		{
			if (bonusDay == i)
			{
				this.days[i].spriteName = "curday";
				this.bonusbox[i].gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			}
			else
			{
				this.days[i].spriteName = "ram";
			}
		}
		this.parts_avail = new int[Kube.GPS.parts.Length - 1];
		Array.Copy(Kube.GPS.parts, 1, this.parts_avail, 0, this.parts_avail.Length);
		this.nextpart.part = Kube.GPS.parts[0] + 1;
		for (int j = 0; j < 5; j++)
		{
			int num = Kube.GPS.bonusesPrice[j, 0];
			if (num == 0)
			{
				num = Kube.GPS.bonusesPrice[j, 1];
				this.bonusbox[j].isGold = true;
			}
			this.bonusbox[j].text.text = num.ToString();
		}
		for (int k = 0; k < 4; k++)
		{
			if (this.parts_avail.Length > k)
			{
				this.parts[k].part = this.parts_avail[k] + 1;
			}
		}
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
	}

	public void onTakeClick()
	{
		if (this.viral.value)
		{
			Kube.SN.PostBonusOnWall();
		}
		base.gameObject.SetActive(false);
	}

	public UILabel tip;

	public UIToggle viral;

	public BonusBox[] parts;

	public BonusBox nextpart;

	protected int[] parts_avail;

	public PriceButton[] bonusbox;

	public UISprite[] days;

	public UISprite progress;

	private int _bonusDay;
}
