using System;
using kube.data;
using UnityEngine;

public class EpisodeItem : MonoBehaviour
{
	private void Start()
	{
		string text = string.Format(Localize.episode_name, this.index);
		if (this.ep.title != null)
		{
			text = this.ep.title;
		}
		this.label.text = text;
	}

	public int index;

	public UILabel label;

	public EpisodeDesc ep;
}
