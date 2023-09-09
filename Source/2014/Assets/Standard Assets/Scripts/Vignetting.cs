using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Vignette and Chromatic Aberration")]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[Serializable]
public class Vignetting : PostEffectsBase
{
	public Vignetting()
	{
		this.mode = Vignetting.AberrationMode.Simple;
		this.intensity = 0.375f;
		this.chromaticAberration = 0.2f;
		this.axialAberration = 0.5f;
		this.blurSpread = 0.75f;
		this.luminanceDependency = 0.25f;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(false);
		this.vignetteMaterial = this.CheckShaderAndCreateMaterial(this.vignetteShader, this.vignetteMaterial);
		this.separableBlurMaterial = this.CheckShaderAndCreateMaterial(this.separableBlurShader, this.separableBlurMaterial);
		this.chromAberrationMaterial = this.CheckShaderAndCreateMaterial(this.chromAberrationShader, this.chromAberrationMaterial);
		if (!this.isSupported)
		{
			this.ReportAutoDisable();
		}
		return this.isSupported;
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.CheckResources())
		{
			Graphics.Blit(source, destination);
		}
		else
		{
			bool flag = (Mathf.Abs(this.blur) > (float)0) ?? (Mathf.Abs(this.intensity) > (float)0);
			float num = 1f * (float)source.width / (1f * (float)source.height);
			float num2 = 0.001953125f;
			RenderTexture renderTexture = null;
			RenderTexture renderTexture2 = null;
			RenderTexture renderTexture3 = null;
			if (flag)
			{
				renderTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
				if (Mathf.Abs(this.blur) > (float)0)
				{
					renderTexture2 = RenderTexture.GetTemporary((int)((float)source.width / 2f), (int)((float)source.height / 2f), 0, source.format);
					renderTexture3 = RenderTexture.GetTemporary((int)((float)source.width / 2f), (int)((float)source.height / 2f), 0, source.format);
					Graphics.Blit(source, renderTexture2, this.chromAberrationMaterial, 0);
					for (int i = 0; i < 2; i++)
					{
						this.separableBlurMaterial.SetVector("offsets", new Vector4((float)0, this.blurSpread * num2, (float)0, (float)0));
						Graphics.Blit(renderTexture2, renderTexture3, this.separableBlurMaterial);
						this.separableBlurMaterial.SetVector("offsets", new Vector4(this.blurSpread * num2 / num, (float)0, (float)0, (float)0));
						Graphics.Blit(renderTexture3, renderTexture2, this.separableBlurMaterial);
					}
				}
				this.vignetteMaterial.SetFloat("_Intensity", this.intensity);
				this.vignetteMaterial.SetFloat("_Blur", this.blur);
				this.vignetteMaterial.SetTexture("_VignetteTex", renderTexture2);
				Graphics.Blit(source, renderTexture, this.vignetteMaterial, 0);
			}
			this.chromAberrationMaterial.SetFloat("_ChromaticAberration", this.chromaticAberration);
			this.chromAberrationMaterial.SetFloat("_AxialAberration", this.axialAberration);
			this.chromAberrationMaterial.SetFloat("_Luminance", 1f / (float.Epsilon + this.luminanceDependency));
			if (flag)
			{
				renderTexture.wrapMode = TextureWrapMode.Clamp;
			}
			else
			{
				source.wrapMode = TextureWrapMode.Clamp;
			}
			Graphics.Blit((!flag) ? source : renderTexture, destination, this.chromAberrationMaterial, (this.mode != Vignetting.AberrationMode.Advanced) ? 1 : 2);
			if (renderTexture)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
			}
			if (renderTexture2)
			{
				RenderTexture.ReleaseTemporary(renderTexture2);
			}
			if (renderTexture3)
			{
				RenderTexture.ReleaseTemporary(renderTexture3);
			}
		}
	}

	public override void Main()
	{
	}

	public Vignetting.AberrationMode mode;

	public float intensity;

	public float chromaticAberration;

	public float axialAberration;

	public float blur;

	public float blurSpread;

	public float luminanceDependency;

	public Shader vignetteShader;

	private Material vignetteMaterial;

	public Shader separableBlurShader;

	private Material separableBlurMaterial;

	public Shader chromAberrationShader;

	private Material chromAberrationMaterial;

	[Serializable]
	public enum AberrationMode
	{
		Simple,
		Advanced
	}
}
