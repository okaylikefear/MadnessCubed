using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Grayscale")]
[ExecuteInEditMode]
public class GrayscaleEffect : ImageEffectBase
{
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.material.SetTexture("_RampTex", this.textureRamp);
		base.material.SetFloat("_RampOffset", this.rampOffset);
		Graphics.Blit(source, destination, base.material);
	}

	public Texture textureRamp;

	public float rampOffset;
}
