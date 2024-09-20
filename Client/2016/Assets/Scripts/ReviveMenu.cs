using System;
using kube;
using UnityEngine;

public class ReviveMenu : MonoBehaviour
{
	public void onRevive()
	{
		if (Kube.GPS.inventarItems[109] > 0)
		{
			Kube.BCS.ps.RespawnFromRevive();
			Kube.IS.UseItem(109);
			base.gameObject.SetActive(false);
		}
	}

	public void onVideo()
	{
	}

	private void Start()
	{
		this.reviveTx.mainTexture = Kube.OH.gameItemsTex[109];
		this.cnt.text = Kube.GPS.inventarItems[109].ToString();
	}

	public void onOk()
	{
		base.gameObject.SetActive(false);
		Kube.BCS.ExitGame();
	}

	public UILabel cnt;

	public UITexture reviveTx;

	public UIButton btnRevive;

	public UIButton btnVideo;
}
