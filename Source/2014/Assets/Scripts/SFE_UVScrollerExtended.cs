using System;
using UnityEngine;

[Serializable]
public class SFE_UVScrollerExtended : MonoBehaviour
{
	public SFE_UVScrollerExtended()
	{
		this.velocityX = 0.5f;
	}

	public virtual void Start()
	{
		if (this.renderer)
		{
			this.enabled = false;
		}
	}

	public virtual void Update()
	{
		float y = this.renderer.materials[this.matNumber].mainTextureOffset.y + this.velocityY * Time.deltaTime;
		Vector2 mainTextureOffset = this.renderer.materials[this.matNumber].mainTextureOffset;
		float num = mainTextureOffset.y = y;
		Vector2 vector = this.renderer.materials[this.matNumber].mainTextureOffset = mainTextureOffset;
		float x = this.renderer.materials[this.matNumber].mainTextureOffset.x + this.velocityX * Time.deltaTime;
		Vector2 mainTextureOffset2 = this.renderer.materials[this.matNumber].mainTextureOffset;
		float num2 = mainTextureOffset2.x = x;
		Vector2 vector2 = this.renderer.materials[this.matNumber].mainTextureOffset = mainTextureOffset2;
	}

	public virtual void OnBecameVisible()
	{
		this.enabled = true;
	}

	public virtual void OnBecameInvisible()
	{
		this.enabled = false;
	}

	public virtual void Main()
	{
	}

	public float velocityY;

	public float velocityX;

	public int matNumber;
}
