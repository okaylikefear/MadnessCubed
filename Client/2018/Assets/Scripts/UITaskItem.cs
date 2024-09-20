using System;
using System.Collections;
using kube;
using kube.data;
using UnityEngine;

public class UITaskItem : MonoBehaviour
{
	private void Start()
	{
		string ico = this.desc.ico;
		if (this.frame && this.desc.daily)
		{
			this.frame.gameObject.SetActive(false);
		}
		if (!string.IsNullOrEmpty(this.desc.ico))
		{
			if (this.sprite.atlas.GetSprite(this.desc.ico) != null)
			{
				this.sprite.spriteName = this.desc.ico;
			}
			else
			{
				Cub2Menu.instance.StartCoroutine(this._loadTx(this.desc.ico));
			}
		}
	}

	private void OnClick()
	{
		HomeMenu component = base.transform.parent.parent.GetComponent<HomeMenu>();
		component.ShowTask(this.desc);
	}

	private IEnumerator _loadTx(string icon)
	{
		Kube.RM.require("Assets2", null);
		while (Kube.ASS2 == null)
		{
			yield return new WaitForSeconds(2f);
		}
		char kind = this.desc.ico[0];
		int id = -1;
		if (!int.TryParse(this.desc.ico.Substring(1), out id))
		{
			yield break;
		}
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
				goto IL_236;
			}
		}
		tx = this.sprite.gameObject.AddComponent<UITexture>();
		tx.mainTexture = Kube.OH.gameItemsTex[id];
		tx.width = this.sprite.width;
		tx.height = this.sprite.height;
		tx.depth = this.sprite.depth;
		this.sprite.spriteName = string.Empty;
		this.sprite.enabled = false;
		IL_236:
		yield break;
	}

	public UISprite sprite;

	public TaskDesc desc;

	public UISprite newone;

	public UISprite frame;
}
