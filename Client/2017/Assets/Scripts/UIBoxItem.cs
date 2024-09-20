using System;
using System.Collections;
using kube;
using UnityEngine;

public class UIBoxItem : MonoBehaviour
{
	private void Start()
	{
		this.UpdateMinutes();
		base.InvokeRepeating("UpdateMinutes", 1f, 1f);
		Cub2Menu.instance.StartCoroutine(this._loadTx());
	}

	private void UpdateMinutes()
	{
		TimeSpan timeSpan;
		if (DateTime.UtcNow < Kube.GPS.nextUnbox)
		{
			timeSpan = Kube.GPS.nextUnbox - DateTime.UtcNow;
		}
		else
		{
			timeSpan = TimeSpan.Zero;
		}
		string text = string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
		this.timerLabel.text = text;
	}

	private IEnumerator _loadTx()
	{
		Kube.RM.require("Assets2", null);
		while (Kube.ASS2 == null)
		{
			yield return new WaitForSeconds(2f);
		}
		UITexture tx = this.sprite.gameObject.AddComponent<UITexture>();
		tx.mainTexture = Kube.OH.gameItemsTex[this.box];
		tx.width = this.sprite.width;
		tx.height = this.sprite.height;
		tx.depth = this.sprite.depth;
		this.sprite.spriteName = string.Empty;
		this.sprite.enabled = false;
		yield break;
	}

	private void OnClick()
	{
		UnboxDialog unboxDialog = Cub2UI.FindDialog<UnboxDialog>("dialog unbox");
		unboxDialog.Open(this.box);
	}

	public void Validate()
	{
	}

	public UISprite sprite;

	public UILabel timerLabel;

	[HideInInspector]
	public int box;
}
