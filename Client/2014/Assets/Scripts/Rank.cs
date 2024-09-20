using System;
using kube;
using UnityEngine;

public class Rank : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnTooltip(bool show)
	{
		if (show)
		{
			string text = string.Empty;
			int num = Kube.OH.GetExpToLevelUp(Kube.GPS.playerExp) - Kube.OH.GetExpFromLevelUp(Kube.GPS.playerExp);
			text = text + this.label.text + "\n";
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				Localize.player_level,
				Kube.GPS.playerLevel,
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
