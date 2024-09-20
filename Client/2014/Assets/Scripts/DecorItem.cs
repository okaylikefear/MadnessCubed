using System;
using System.Collections;
using kube;
using UnityEngine;

public class DecorItem : MonoBehaviour
{
	public int itemId
	{
		get
		{
			return this._fi.Num;
		}
		set
		{
			this._fi.Num = value;
			this._fi.Type = 3;
		}
	}

	public bool value
	{
		get
		{
			return this._value;
		}
		set
		{
			if (this.checkmark)
			{
				this.checkmark.alpha = ((!value) ? 0f : 255f);
			}
			this._value = value;
		}
	}

	public bool current
	{
		get
		{
			return this._current;
		}
		set
		{
			this.currentmark.alpha = ((!value) ? 0f : 255f);
			this._current = value;
		}
	}

	private void Start()
	{
		this.Invalidate();
	}

	public FastInventar fi
	{
		get
		{
			return this._fi;
		}
		set
		{
			this._fi = value;
		}
	}

	private void Invalidate()
	{
		if (this.loading == null)
		{
			this.loading = NGUITools.AddChild(this.tx.gameObject, Cub2Menu.instance.loadingPrefab);
		}
		this.loading.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		if (Kube.ASS2 == null)
		{
			Kube.SS.require("Assets2");
		}
		Cub2Menu.instance.StartCoroutine(this._loadTx());
		int num = 0;
		int num2 = 0;
		bool flag = true;
		if (this._fi.Type == 3)
		{
			this.title.text = Localize.gameItemsNames[this.itemId];
			num = Kube.GPS.inventarItemPrice1[this.itemId];
			num2 = Kube.GPS.inventarItemPrice2[this.itemId];
			this.cnt.text = Kube.GPS.inventarItems[this.itemId].ToString();
		}
		else if (this._fi.Type == 7)
		{
			this.title.text = Localize.specItemsName[this.itemId];
			num = Kube.GPS.specItemsPrice1[this.itemId, 0];
			num2 = Kube.GPS.specItemsPrice2[this.itemId, 0];
			this.cnt.gameObject.SetActive(false);
			if (Kube.GPS.inventarSpecItems[this.itemId] > 0)
			{
				flag = false;
			}
		}
		if (!flag)
		{
			this.btn.gameObject.SetActive(false);
		}
		if (num > 0)
		{
			this.btn.text.text = num.ToString();
			this.btn.isGold = false;
		}
		else
		{
			this.btn.text.text = num2.ToString();
			this.btn.isGold = true;
		}
		this.btn.GetComponent<UIButton>().onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnBuyClick)));
	}

	private IEnumerator _loadTx()
	{
		while (Kube.ASS2 == null)
		{
			yield return new WaitForSeconds(2f);
		}
		if (this.tx.mainTexture == null)
		{
			if (this._fi.Type == 3)
			{
				this.tx.mainTexture = Kube.ASS2.gameItemsTex[this.itemId];
			}
			else if (this._fi.Type == 7)
			{
				this.tx.mainTexture = Kube.ASS2.specItemsInvTex[this.itemId];
			}
		}
		if (this.tx.mainTexture)
		{
			int h = this.tx.height;
			float aspect = (float)(this.tx.mainTexture.width / this.tx.mainTexture.height);
			this.tx.width = Mathf.FloorToInt((float)h * aspect);
		}
		if (this.loading)
		{
			this.loading.SetActive(false);
		}
		yield break;
	}

	private void Update()
	{
		if (this.cnt && this._fi.Type == 3)
		{
			this.cnt.text = Kube.GPS.inventarItems[this.itemId].ToString();
		}
	}

	public void ItemsCubesUpdate()
	{
		this.Invalidate();
	}

	private void OnBuyClick()
	{
		base.transform.parent.parent.GetComponent<ShopMenu>().onBuyKube(this.itemId);
	}

	private void OnClick()
	{
		base.transform.parent.parent.GetComponent<ShopMenu>().onSelectItem(this);
		base.transform.parent.parent.GetComponent<ShopMenu>().onSelectKube(this.itemId);
	}

	public UISprite checkmark;

	public UISprite currentmark;

	public bool _value;

	public bool _current;

	public UITexture tx;

	public UILabel title;

	public UILabel cnt;

	public PriceButton btn;

	private GameObject loading;

	private FastInventar _fi;
}
