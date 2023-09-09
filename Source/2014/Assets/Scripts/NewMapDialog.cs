using System;
using kube;
using UnityEngine;

public class NewMapDialog : MonoBehaviour
{
	private void Start()
	{
		if (base.gameObject.activeSelf)
		{
			this.OnEnable();
		}
	}

	public void OnEnable()
	{
		this.done.isEnabled = true;
		this.tx.mainTexture = Kube.ASS1.newMapTypeTex[this.lr.index];
		if (!this.regenerateMode)
		{
			this.price.text.text = Kube.GPS.newMapPrice.ToString();
		}
		this.lr.states = Localize.newMapTypeName;
		this.lr.index = 0;
	}

	public void BuySlot()
	{
		UIButton.current.isEnabled = false;
		if (this.regenerateMode)
		{
			long numMap = (long)Kube.GPS.playerId * 20L + (long)this.slot;
			Kube.SS.RegenerateMap(this.lr.index, numMap, new ServerScript.ServerCallback(this.RegenerateDone));
		}
		else
		{
			Kube.SS.BuyNewMap(this.lr.index, new ServerScript.ServerCallback(this.BuyNewMapDone));
			Kube.SN.PostMapSlot(Kube.GPS.playerNumMaps);
		}
	}

	private void RegenerateDone(string str)
	{
		this.owner.SendMessage("BuyNewMapDone");
		base.gameObject.SetActive(false);
	}

	private void BuyNewMapDone(string str)
	{
		Kube.OH.SendMessage("BuyNewMapDone", str);
		this.owner.SendMessage("BuyNewMapDone");
		base.gameObject.SetActive(false);
	}

	public void OnChangeType()
	{
		this.tx.mainTexture = Kube.ASS1.newMapTypeTex[LRButton.current.index];
	}

	public UITexture tx;

	public LRButton lr;

	public int slot;

	public bool regenerateMode;

	public CreatingMyTab owner;

	public PriceButton price;

	public UIButton done;
}
