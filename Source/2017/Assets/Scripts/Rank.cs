using System;
using kube;
using UnityEngine;

public class Rank : MonoBehaviour
{
	public void onAssetsLoaded(int id)
	{
		if (Kube.ASS2 == null)
		{
			return;
		}
		int num = Mathf.Min(Kube.GPS.playerLevel, Kube.ASS2.RankTex.Length - 1);
		this.label.text = Localize.RankName[num];
		this.tx.mainTexture = Kube.ASS2.RankTex[num].mainTexture;
	}

	private void Start()
	{
		int level = Kube.OH.GetLevel(Kube.GPS.playerExp);
		int num = Mathf.Min(level, Localize.RankName.Length - 1);
		this.label.text = Localize.RankName[num];
		if (this.labelLevel)
		{
			this.labelLevel.text = string.Concat(new object[]
			{
				"(",
				Localize.player_level,
				" ",
				level,
				")"
			});
		}
		if (Kube.ASS2)
		{
			this.tx.mainTexture = Kube.ASS2.RankTex[num].mainTexture;
		}
		uint expFromLevelUp = Kube.OH.GetExpFromLevelUp(Kube.GPS.playerExp, level);
		uint expToLevelUp = Kube.OH.GetExpToLevelUp(level);
		this.progressLabel.text = expFromLevelUp.ToString() + "/" + expToLevelUp.ToString();
		this.progress.value = expFromLevelUp / expToLevelUp;
	}

	private void Update()
	{
	}

	public void OnTooltip(bool show)
	{
		if (show)
		{
			string text = string.Empty;
			int level = Kube.OH.GetLevel(Kube.GPS.playerExp);
			uint num = Kube.OH.GetExpToLevelUp(level) - Kube.OH.GetExpFromLevelUp(Kube.GPS.playerExp, -1);
			text = text + this.label.text + "\n";
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				Localize.player_level,
				" ",
				level,
				"\n"
			});
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"  (",
				num,
				" ",
				Localize.xp_next,
				")"
			});
			UITooltip.ShowText(text);
		}
		else
		{
			UITooltip.ShowText(null);
		}
	}

	public UILabel label;

	public UILabel labelLevel;

	public UITexture tx;

	public UISlider progress;

	public UILabel progressLabel;
}
