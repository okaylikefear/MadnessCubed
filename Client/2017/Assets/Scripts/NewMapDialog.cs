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
		this.creatingMaps = Kube.OH.findCreatingMaps(this.slot > 0);
		this.done.isEnabled = true;
		Kube.RM.require("Assets2", new global::AsyncCallback(this.OnChangeType));
		if (Kube.ASS2 != null)
		{
			this.tx.mainTexture = Kube.ASS2.newMapTypeTex[this.creatingMaps[0].Id];
		}
		if (!this.regenerateMode)
		{
			this.price.text.text = Kube.GPS.newMapPrice.ToString();
		}
		this.lr.states = this.getMapNames();
		this.lr.index = 0;
	}

	private string[] getMapNames()
	{
		string[] array = new string[this.creatingMaps.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Localize.buildinMapName[this.creatingMaps[i].Id];
		}
		return array;
	}

	public void BuySlot()
	{
		UIButton.current.isEnabled = false;
		PlayMenu.playMenu.BuySlot(this.slot);
		PlayMenu.playMenu.slotNames[this.slot] = null;
		if (this.regenerateMode)
		{
			long numMap = (long)Kube.SS.serverId * 20L + (long)this.slot;
			Kube.SS.RegenerateMap(this.creatingMaps[this.lr.index].Id, numMap, new ServerCallback(this.onRegenerateMsg));
		}
		else
		{
			Kube.SS.BuyNewMap(this.creatingMaps[this.lr.index].Id, new ServerCallback(this.onBuyNewMapMsg));
			Kube.SN.PostMapSlot(Kube.GPS.playerNumMaps);
		}
	}

	private void onRegenerateMsg(string str)
	{
		this.owner.SendMessage("BuyNewMapDone");
		base.gameObject.SetActive(false);
	}

	private void onBuyNewMapMsg(string str)
	{
		Kube.OH.SendMessage("BuyNewMapDone", str);
		this.owner.SendMessage("BuyNewMapDone");
		base.gameObject.SetActive(false);
	}

	public void OnChangeType()
	{
		int num = 0;
		if (LRButton.current != null)
		{
			num = LRButton.current.index;
		}
		if (Kube.ASS2 != null)
		{
			this.tx.mainTexture = Kube.ASS2.newMapTypeTex[this.creatingMaps[num].Id];
		}
	}

	public UITexture tx;

	public LRButton lr;

	public int slot;

	public bool regenerateMode;

	public CreatingMyTab owner;

	public PriceButton price;

	private ObjectsHolderScript.BuiltInMap[] creatingMaps;

	public UIButton done;
}
