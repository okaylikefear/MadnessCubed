using System;
using kube;
using UnityEngine;

public class HUDCreatingMode : HUDCreatingModeBase
{
	private void Start()
	{
		this.rama.transform.position = this.boxes[0].transform.position;
	}

	private void BeginPlay()
	{
	}

	public override void SetCube(int index)
	{
		int num = index % 3;
		int num2 = index / 3;
		if (this._page != num2)
		{
			for (int i = 0; i < 3; i++)
			{
				int num3 = num2 * 3 + i;
				string text = string.Empty;
				if (this.geomIds.Length > num3)
				{
					text = "geom" + this.geomIds[num3];
				}
				UISprite component = this.boxes[i].transform.GetChild(0).GetComponent<UISprite>();
				UITexture uitexture = this.boxes[i].transform.GetChild(0).GetComponent<UITexture>();
				if (component.atlas.GetSprite(text) != null)
				{
					component.spriteName = text;
					if (uitexture != null)
					{
						uitexture.enabled = false;
					}
				}
				else
				{
					component.spriteName = string.Empty;
					if (uitexture == null)
					{
						uitexture = this.boxes[i].transform.GetChild(0).gameObject.AddComponent<UITexture>();
					}
					uitexture.enabled = true;
					uitexture.depth = component.depth + 1;
					if (this.geomIds.Length > num3 && Kube.ASS2.specItemsInvTex.Length > 10 + this.geomIds[num3])
					{
						uitexture.mainTexture = Kube.ASS2.specItemsInvTex[10 + this.geomIds[num3]];
					}
					else
					{
						uitexture.mainTexture = null;
					}
					uitexture.width = component.width;
					uitexture.height = component.height;
				}
			}
			this._page = num2;
		}
		this.rama.transform.position = this.boxes[num].transform.position;
	}

	public GameObject[] boxes;

	public GameObject rama;

	public GameObject xb;

	public GameObject zb;

	protected int _page = -1;
}
