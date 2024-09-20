using System;
using kube;
using kube.data;
using UnityEngine;

public class PackDialog : MonoBehaviour
{
	private void OnEnable()
	{
		UnityEngine.Debug.Log("En Dialog");
		this.PackInit();
	}

	protected void PackInit()
	{
		FastInventar[] items = this.info.items;
		KGUITools.removeAllChildren(this.cont, true);
		for (int i = 0; i < items.Length; i++)
		{
			GameObject gameObject = this.cont.AddChild(this.itemPrefab);
			ItemDescIcon component = gameObject.GetComponent<ItemDescIcon>();
			component.fi = items[i];
			if (items[i].Type == 4)
			{
				this.isWeapon = true;
				component.countText = Localize.is_unlimit;
			}
			if (items[i].Type == 5)
			{
				component.countText = Localize.is_unlimit;
			}
			else
			{
				component.countText = "x" + this.info.cnt[i];
			}
		}
		this.btn.valueStr = string.Format(Localize.ui_buy_for, Kube.SN.MoneyNameForPack(this.info));
		this.cont.GetComponent<UITable>().Reposition();
	}

	public void OnButton()
	{
		base.gameObject.SetActive(false);
		Kube.SN.BuyPack(this.info);
	}

	public GameObject itemPrefab;

	public GameObject cont;

	public RealPriceButton btn;

	protected bool isWeapon;

	public PackInfo info;
}
