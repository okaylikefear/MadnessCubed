using System;
using System.Collections;
using kube;
using kube.data;
using UnityEngine;

public class UIPackItem : MonoBehaviour
{
	private void Start()
	{
		if (this.info == null)
		{
			return;
		}
		int num = 1;
		if (this.info.items[0].Type == 4)
		{
			num = 2;
		}
		else if (this.info.kind == PackKind.box)
		{
			num = 3;
		}
		string text = "pack_ico_" + num;
		if (!string.IsNullOrEmpty(this.info.icon))
		{
			if (this.sprite.atlas.GetSprite(this.info.icon) != null)
			{
				text = this.info.icon;
			}
			else
			{
				Cub2Menu.instance.StartCoroutine(this._loadTx(this.info.icon));
			}
		}
		this.sprite.spriteName = text.ToString();
	}

	private IEnumerator _loadTx(string icon)
	{
		Kube.RM.require("Assets2", null);
		while (Kube.ASS2 == null)
		{
			yield return new WaitForSeconds(2f);
		}
		char kind = this.info.icon[0];
		int id = int.Parse(this.info.icon.Substring(1));
		char c = kind;
		UITexture tx;
		if (c != 'i')
		{
			if (c == 'w')
			{
				tx = this.sprite.gameObject.AddComponent<UITexture>();
				tx.mainTexture = Kube.ASS2.inventarWeaponsTex[id];
				tx.width = this.sprite.width;
				tx.height = this.sprite.height;
				tx.depth = this.sprite.depth;
				this.sprite.spriteName = string.Empty;
				this.sprite.enabled = false;
				goto IL_225;
			}
		}
		tx = this.sprite.gameObject.AddComponent<UITexture>();
		tx.mainTexture = Kube.OH.gameItemsTex[id];
		tx.width = this.sprite.width;
		tx.height = this.sprite.height;
		tx.depth = this.sprite.depth;
		this.sprite.spriteName = string.Empty;
		this.sprite.enabled = false;
		IL_225:
		yield break;
	}

	private void OnClick()
	{
		HomeMenu component = base.transform.parent.parent.GetComponent<HomeMenu>();
		component.ShowPack(this.info);
	}

	public void Validate()
	{
		base.gameObject.SetActive(this.info.Validate());
	}

	public UISprite sprite;

	public PackInfo info;
}
