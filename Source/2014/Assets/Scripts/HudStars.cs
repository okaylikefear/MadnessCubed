using System;
using UnityEngine;

public class HudStars : HUDStatus
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void ShowStars(int nn)
	{
		KGUITools.removeAllChildren(this.grid.gameObject, true);
		this.stars = new UISprite[nn];
		for (int i = 0; i < nn; i++)
		{
			GameObject gameObject = NGUITools.AddChild(this.grid.gameObject, this.prefab);
			this.stars[i] = gameObject.GetComponent<UISprite>();
		}
		this.bg.width = nn * 40;
		this.grid.Reposition();
	}

	public void ToggleStar(int index, int team)
	{
		this.stars[index].spriteName = "star_" + (team + 1);
	}

	public GameObject prefab;

	public UISprite bg;

	public UIGrid grid;

	protected UISprite[] stars;
}
