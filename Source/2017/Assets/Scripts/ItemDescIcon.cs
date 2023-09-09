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
			else if (value.Type == 5)
			{
				this.itemType = 2;
				this.itemname = Localize.weaponNames[Kube.IS.weaponSkins[this.itemId].weaponId];
			}
			else if (value.Type == 7)
			{
				this.itemType = 3;
				this.itemname = Localize.specItemsName[this.itemId];
			}
			else if (value.Type == 8)
			{
				this.itemType = 4;
				this.itemname = Localize.skinName[this.itemId];
			}
			else if (value.Type == 9)
			{
				this.itemType = 5;
				this.itemname = Localize.clothesName[this.itemId];
			}
			this.LoadTexture();
		}
	}

	private void SetTexture()
	{
		if (this.itemType == 0)
		{
			this.tx.mainTexture = Kube.OH.gameItemsTex[this._itemId];
		}
		else if (this.itemType == 1)
		{
			this.tx.mainTexture = Kube.ASS2.inventarWeaponsTex[this._itemId];
		}
		else if (this.itemType == 3)
		{
			this.tx.mainTexture = Kube.ASS2.specItemsInvTex[this._itemId];
		}
		else if (this.itemType == 4)
		{
			this.tx.mainTexture = ((!Kube.OH.inventarSkinsTex.ContainsKey(this._itemId)) ? null : Kube.OH.inventarSkinsTex[this._itemId]);
		}
		else if (this.itemType == 5)
		{
			this.tx.mainTexture = ((!Kube.OH.inventarClothesTex.ContainsKey(this._itemId)) ? null : Kube.OH.inventarClothesTex[this._itemId]);
		}
		else
		{
			this.tx.mainTexture = Kube.ASS2.inventarWeaponsSkinTex[this._itemId];
		}
	}

	private void LoadTexture()
	{
		if (Kube.ASS2 != null)
		{
			this.SetTexture();
		}
		else
		{
			this.loading = base.gameObject.AddChild(Cub2Menu.instance.loadingPrefab);
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

	public string countText
	{
		set
		{
			this.countLabel.text = value.ToString();
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
		if (this.tx.mainTexture != null)
		{
			return;
		}
		if (Kube.ASS2 != null)
		{
			this.SetTexture();
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
