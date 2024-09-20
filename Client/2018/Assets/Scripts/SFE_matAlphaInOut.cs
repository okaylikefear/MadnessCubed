using System;
using UnityEngine;

[Serializable]
public class SFE_matAlphaInOut : MonoBehaviour
{
	public SFE_matAlphaInOut()
	{
		this.alphaIn = (float)2;
		this.alphaStay = (float)1;
		this.alphaOut = (float)3;
		this.otherColors = 0.5f;
		this.killObjectOnEnd = true;
	}

	public virtual void Start()
	{
		if (this.alphaIn <= (float)0)
		{
			this.alphaIn = 0.1f;
			UnityEngine.Debug.Log("Please don't set AlphaIn to zero or below...(matAlphaInOut script)");
		}
		if (this.alphaOut <= (float)0)
		{
			this.alphaOut = 0.1f;
			UnityEngine.Debug.Log("Please don't set AlphaOut to zero or below...(matAlphaInOut script)");
		}
		this.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(this.otherColors, this.otherColors, this.otherColors, this.alpha));
	}

	public virtual void Update()
	{
		this.time += Time.deltaTime;
		if (this.time < this.alphaIn)
		{
			this.alpha = this.time / this.alphaIn;
		}
		if (this.time >= this.alphaIn && this.time < this.alphaIn + this.alphaStay)
		{
			this.alpha = (float)1;
		}
		if (this.time >= this.alphaIn + this.alphaStay && this.time < this.alphaIn + this.alphaStay + this.alphaOut)
		{
			this.alpha = (float)1 - (this.time - (this.alphaIn + this.alphaStay)) / this.alphaOut;
		}
		this.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(this.otherColors, this.otherColors, this.otherColors, this.alpha));
		if (this.time >= this.alphaIn + this.alphaStay + this.alphaOut && this.killObjectOnEnd)
		{
			UnityEngine.Object.Destroy(this.gameObject);
		}
	}

	public virtual void Main()
	{
	}

	public float alphaIn;

	public float alphaStay;

	public float alphaOut;

	public float otherColors;

	private float time;

	private float alpha;

	public bool killObjectOnEnd;
}
