using System;
using kube;
using UnityEngine;

public class GetWorldLightColorScript : MonoBehaviour
{
	private void Start()
	{
		this.renderers = base.GetComponentsInChildren<Renderer>(true);
		this.GetWorldColor();
		this.color = this.newColor;
		this.ChangeColor();
		this.colorDeltaTime = 0.5f;
		this.lastColorTime = Time.time - 10f;
	}

	private void GetWorldColor()
	{
		if (Kube.WHS != null)
		{
			Color32 worldLightAtPoint = Kube.WHS.GetWorldLightAtPoint(base.transform.position);
			this.newColor = new Color((float)worldLightAtPoint.r / 255f, (float)worldLightAtPoint.g / 255f, (float)worldLightAtPoint.b / 255f, 1f);
		}
	}

	private void Update()
	{
		if (Time.time - this.lastColorTime > this.colorDeltaTime)
		{
			this.GetWorldColor();
			this.lastColorTime = Time.time;
		}
		if (Mathf.Abs(this.color.r - this.newColor.r) > 0.01f || Mathf.Abs(this.color.g - this.newColor.g) > 0.01f || Mathf.Abs(this.color.b - this.newColor.b) > 0.01f)
		{
			this.color = new Color(Mathf.Lerp(this.color.r, this.newColor.r, Time.deltaTime * 5f), Mathf.Lerp(this.color.g, this.newColor.g, Time.deltaTime * 5f), Mathf.Lerp(this.color.b, this.newColor.b, Time.deltaTime * 5f), 1f);
			this.ChangeColor();
		}
	}

	private void ChangeColor()
	{
		for (int i = 0; i < this.renderers.Length; i++)
		{
			if (this.renderers[i] != null)
			{
				Material[] materials = this.renderers[i].materials;
				for (int j = 0; j < materials.Length; j++)
				{
					materials[j].SetColor("_Color", this.color);
				}
			}
		}
	}

	private Renderer[] renderers;

	private Color color = default(Color);

	private Color newColor = default(Color);

	private float lastColorTime;

	private float colorDeltaTime = 0.5f;
}
