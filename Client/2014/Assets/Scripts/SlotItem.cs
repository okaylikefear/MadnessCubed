using System;
using System.Collections;
using kube;
using UnityEngine;

public class SlotItem : MonoBehaviour
{
	private IEnumerator _loadTx()
	{
		while (Kube.ASS2 == null)
		{
			yield return new WaitForSeconds(2f);
		}
		if (this._invItem.Type == 0)
		{
			this.tx.mainTexture = Kube.ASS2.inventarCubesTex[this._invItem.Num];
			this.itemname = "Куб";
			if (this.cnt)
			{
				this.cnt.alpha = 0f;
			}
		}
		else if (this._invItem.Type == 1 || this._invItem.Type == 3)
		{
			this.tx.mainTexture = Kube.ASS2.gameItemsTex[this._invItem.Num];
			this.itemname = Localize.gameItemsNames[this._invItem.Num];
			this.UpdateCount();
		}
		else if (this._invItem.Type == 4)
		{
			this.tx.mainTexture = Kube.ASS2.inventarWeaponsTex[this._invItem.Num];
			this.itemname = Localize.weaponNames[this._invItem.Num];
		}
		else
		{
			this.tx.mainTexture = null;
			if (this.cnt)
			{
				this.cnt.alpha = 0f;
			}
		}
		yield break;
	}

	public void UpdateCount()
	{
		if (this._invItem.Type == 1 || this._invItem.Type == 3)
		{
			int itemNN = Kube.GPS.inventarItems[this._invItem.Num];
			if (Kube.BCS && Kube.BCS.ps)
			{
				itemNN = Kube.BCS.ps.itemCnt(this._invItem.Num, itemNN);
			}
			if (this.cnt)
			{
				this.cnt.text = itemNN.ToString();
				this.cnt.alpha = 1f;
			}
		}
	}

	private void Update()
	{
		this.UpdateCount();
	}

	public FastInventar invItem
	{
		set
		{
			if (this._invItem == value && this.tx.mainTexture != null)
			{
				return;
			}
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.tx == null)
			{
				UnityEngine.Debug.Log("BAD");
			}
			this._invItem = value;
			base.StartCoroutine(this._loadTx());
		}
	}

	public void OnTooltip(bool show)
	{
		if (show)
		{
			UITooltip.ShowText(this.itemname);
		}
		else
		{
			UITooltip.ShowText(null);
		}
	}

	public int cntvalue
	{
		get
		{
			return this._cnt;
		}
		set
		{
			if (this._cnt == value)
			{
				return;
			}
			this._cnt = value;
			this.cnt.text = this._cnt.ToString();
			this.cnt.alpha = 1f;
		}
	}

	private void OnClick()
	{
		SlotItem.current = this;
		this.onClick.Execute();
		SlotItem.current = null;
	}

	public UILabel id;

	public UITexture tx;

	public UILabel cnt;

	private FastInventar _invItem;

	private string itemname;

	public EventDelegate onClick;

	protected int _cnt;

	public static SlotItem current;
}
