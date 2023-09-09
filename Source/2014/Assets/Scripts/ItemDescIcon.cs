using System;
using kube;
using UnityEngine;

public class ItemDescIcon : MonoBehaviour
{
	public int itemId
	{
		get
		{
			return this._itemId;
		}
		set
		{
			this._itemId = value;
			this.LoadTexture();
		}
	}

	public FastInventar fi
	{
		set
		{
			this._itemId = value.Num;
			if (value.Type == 3)
			{
				this.itemType = 0;
				this.itemname = Localize.gameItemsNames[this.itemId];
			}
			else if (value.Type == 4)
			{
				this.itemType = 1;
				this.itemname = Localize.weaponNames[this.itemId];
			}
			this.LoadTexture();
		}
	}

	private void LoadTexture()
	{
		if (Kube.ASS2 != null)
		{
			if (this.itemType == 0)
			{
				this.tx.mainTexture = Kube.ASS2.gameItemsTex[this._itemId];
			}
			else
			{
				this.tx.mainTexture = Kube.ASS2.inventarWeaponsTex[this._itemId];
			}
		}
		else
		{
			this.loading = NGUITools.AddChild(base.gameObject, Cub2Menu.instance.loadingPrefab);
			this.loading.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		}
	}

	public int count
	{
		set
		{
			if (this.itemType == 0)
			{
				this.countLabel.text = value.ToString();
			}
			else
			{
				this.countLabel.text = string.Empty;
			}
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

	public void onAssetsLoaded(int id)
	{
		if (Kube.ASS2 != null)
		{
			if (this.itemType == 0)
			{
				this.tx.mainTexture = Kube.ASS2.gameItemsTex[this.itemId];
			}
			else
			{
				this.tx.mainTexture = Kube.ASS2.inventarWeaponsTex[this._itemId];
			}
			UnityEngine.Object.Destroy(this.loading);
		}
	}

	public UILabel countLabel;

	public UITexture tx;

	private int _itemId;

	public int itemType;

	public GameObject loading;

	public string itemname;
}
