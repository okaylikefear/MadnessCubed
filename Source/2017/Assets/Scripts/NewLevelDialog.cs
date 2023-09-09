using System;
using kube;
using UnityEngine;

public class NewLevelDialog : MonoBehaviour
{
	private void OnEnable()
	{
		this.OnOpen();
	}

	private void Start()
	{
		if (base.enabled)
		{
			this.OnOpen();
		}
	}

	private void Update()
	{
	}

	public void OnOpen()
	{
		UnityEngine.Object.Instantiate(Kube.ASS3.levelUpEffect, new Vector3(23f, 53f, 23f), Quaternion.identity);
		int num = this.newlevel;
		if (num >= Localize.RankName.Length)
		{
			num = Localize.RankName.Length - 1;
		}
		this.level.text = Localize.BCS_new_rang + " " + Localize.RankName[num];
		this.gold.text = this.goldGive.ToString();
		this.rank.mainTexture = Kube.ASS2.RankTex[num].mainTexture;
	}

	public void onContinueClick()
	{
		if (this.onContinue != null)
		{
			this.onContinue.Execute();
		}
	}

	public UILabel level;

	public UILabel gold;

	public UITexture rank;

	public int newlevel;

	public int goldGive;

	public EventDelegate onContinue;
}
