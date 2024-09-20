using System;
using UnityEngine;

public class LightSwordScript : MonoBehaviour
{
	private void Start()
	{
		this.lightSwordMat = base.gameObject.GetComponent<LineRenderer>().material;
		this.col = this.lightSwordMat.GetColor("_TintColor");
	}

	private void Update()
	{
		this.lightSwordMat.SetColor("_TintColor", new Color(this.col.r, this.col.g, this.col.b, 0.75f + 0.25f * Mathf.Sin(Time.time * 40f)));
	}

	private Material lightSwordMat;

	private Color col;
}
