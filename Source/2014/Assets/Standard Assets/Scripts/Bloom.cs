using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Bloom (4.0, HDR, Lens Flares)")]
[Serializable]
public class Bloom : PostEffectsBase
{
	public Bloom()
	{
		this.screenBlendMode = Bloom.BloomScreenBlendMode.Add;
		this.hdr = Bloom.HDRBloomMode.Auto;
		this.sepBlurSpread = 2.5f;
		this.quality = Bloom.BloomQuality.High;
		this.bloomIntensity = 0.5f;
		this.bloomThreshhold = 0.5f;
		this.bloomThreshholdColor = Color.white;
		this.bloomBlurIterations = 2;
		this.hollywoodFlareBlurIterations = 2;
		this.lensflareMode = Bloom.LensFlareStyle.Anamorphic;
		this.hollyStretchWidth = 2.5f;
		this.lensflareThreshhold = 0.3f;
		this.lensFlareSaturation = 0.75f;
		this.flareColorA = new Color(0.4f, 0.4f, 0.8f, 0.75f);
		this.flareColorB = new Color(0.4f, 0.8f, 0.8f, 0.75f);
		this.flareColorC = new Color(0.8f, 0.4f, 0.8f, 0.75f);
		this.flareColorD = new Color(0.8f, 0.4f, (float)0, 0.75f);
		this.blurWidth = 1f;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(false);
		this.screenBlend = this.CheckShaderAndCreateMaterial(this.screenBlendShader, this.screenBlend);
		this.lensFlareMaterial = this.CheckShaderAndCreateMaterial(this.lensFlareShader, this.lensFlareMaterial);
		this.blurAndFlaresMaterial = this.CheckShaderAndCreateMaterial(this.blurAndFlaresShader, this.blurAndFlaresMaterial);
		this.brightPassFilterMaterial = this.CheckShaderAndCreateMaterial(this.brightPassFilterShader, this.brightPassFilterMaterial);
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
			this.doHdr = false;
			if (this.hdr == Bloom.HDRBloomMode.Auto)
			{
				bool flag;
				if (flag = (source.format == RenderTextureFormat.ARGBHalf))
				{
					flag = this.camera.hdr;
				}
				this.doHdr = flag;
			}
			else
			{
				this.doHdr = (this.hdr == Bloom.HDRBloomMode.On);
			}
			bool supportHDRTextures;
			if (supportHDRTextures = this.doHdr)
			{
				supportHDRTextures = this.supportHDRTextures;
			}
			this.doHdr = supportHDRTextures;
			Bloom.BloomScreenBlendMode bloomScreenBlendMode = this.screenBlendMode;
			if (this.doHdr)
			{
				bloomScreenBlendMode = Bloom.BloomScreenBlendMode.Add;
			}
			RenderTextureFormat format = (!this.doHdr) ? RenderTextureFormat.Default : RenderTextureFormat.ARGBHalf;
			RenderTexture temporary = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
			RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);
			RenderTexture temporary3 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);
			RenderTexture temporary4 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);
			float num = 1f * (float)source.width / (1f * (float)source.height);
			float num2 = 0.001953125f;
			if (this.quality > Bloom.BloomQuality.Cheap)
			{
				Graphics.Blit(source, temporary, this.screenBlend, 2);
				Graphics.Blit(temporary, temporary3, this.screenBlend, 2);
				Graphics.Blit(temporary3, temporary2, this.screenBlend, 6);
			}
			else
			{
				Graphics.Blit(source, temporary);
				Graphics.Blit(temporary, temporary2, this.screenBlend, 6);
			}
			this.BrightFilter(this.bloomThreshhold * this.bloomThreshholdColor, temporary2, temporary3);
			if (this.bloomBlurIterations < 1)
			{
				this.bloomBlurIterations = 1;
			}
			else if (this.bloomBlurIterations > 10)
			{
				this.bloomBlurIterations = 10;
			}
			for (int i = 0; i < this.bloomBlurIterations; i++)
			{
				float num3 = (1f + (float)i * 0.25f) * this.sepBlurSpread;
				this.blurAndFlaresMaterial.SetVector("_Offsets", new Vector4((float)0, num3 * num2, (float)0, (float)0));
				Graphics.Blit(temporary3, temporary4, this.blurAndFlaresMaterial, 4);
				if (this.quality > Bloom.BloomQuality.Cheap)
				{
					this.blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(num3 / num * num2, (float)0, (float)0, (float)0));
					Graphics.Blit(temporary4, temporary3, this.blurAndFlaresMaterial, 4);
					if (i == 0)
					{
						Graphics.Blit(temporary3, temporary2);
					}
					else
					{
						Graphics.Blit(temporary3, temporary2, this.screenBlend, 10);
					}
				}
				else
				{
					this.blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(num3 / num * num2, (float)0, (float)0, (float)0));
					Graphics.Blit(temporary4, temporary3, this.blurAndFlaresMaterial, 4);
				}
			}
			if (this.quality > Bloom.BloomQuality.Cheap)
			{
				Graphics.Blit(temporary2, temporary3, this.screenBlend, 6);
			}
			if (this.lensflareIntensity > 1.401298E-45f)
			{
				if (this.lensflareMode == Bloom.LensFlareStyle.Ghosting)
				{
					this.BrightFilter(this.lensflareThreshhold, temporary3, temporary4);
					if (this.quality > Bloom.BloomQuality.Cheap)
					{
						this.blurAndFlaresMaterial.SetVector("_Offsets", new Vector4((float)0, 1.5f / (1f * (float)temporary2.height), (float)0, (float)0));
						Graphics.Blit(temporary4, temporary2, this.blurAndFlaresMaterial, 4);
						this.blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(1.5f / (1f * (float)temporary2.width), (float)0, (float)0, (float)0));
						Graphics.Blit(temporary2, temporary4, this.blurAndFlaresMaterial, 4);
					}
					this.Vignette(0.975f, temporary4, temporary4);
					this.BlendFlares(temporary4, temporary3);
				}
				else
				{
					float num4 = 1f * Mathf.Cos(this.flareRotation);
					float num5 = 1f * Mathf.Sin(this.flareRotation);
					float num6 = this.hollyStretchWidth * 1f / num * num2;
					float num7 = this.hollyStretchWidth * num2;
					this.blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(num4, num5, (float)0, (float)0));
					this.blurAndFlaresMaterial.SetVector("_Threshhold", new Vector4(this.lensflareThreshhold, 1f, (float)0, (float)0));
					this.blurAndFlaresMaterial.SetVector("_TintColor", new Vector4(this.flareColorA.r, this.flareColorA.g, this.flareColorA.b, this.flareColorA.a) * this.flareColorA.a * this.lensflareIntensity);
					this.blurAndFlaresMaterial.SetFloat("_Saturation", this.lensFlareSaturation);
					Graphics.Blit(temporary4, temporary2, this.blurAndFlaresMaterial, 2);
					Graphics.Blit(temporary2, temporary4, this.blurAndFlaresMaterial, 3);
					this.blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(num4 * num6, num5 * num6, (float)0, (float)0));
					this.blurAndFlaresMaterial.SetFloat("_StretchWidth", this.hollyStretchWidth);
					Graphics.Blit(temporary4, temporary2, this.blurAndFlaresMaterial, 1);
					this.blurAndFlaresMaterial.SetFloat("_StretchWidth", this.hollyStretchWidth * 2f);
					Graphics.Blit(temporary2, temporary4, this.blurAndFlaresMaterial, 1);
					this.blurAndFlaresMaterial.SetFloat("_StretchWidth", this.hollyStretchWidth * 4f);
					Graphics.Blit(temporary4, temporary2, this.blurAndFlaresMaterial, 1);
					for (int i = 0; i < this.hollywoodFlareBlurIterations; i++)
					{
						num6 = this.hollyStretchWidth * 2f / num * num2;
						this.blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(num6 * num4, num6 * num5, (float)0, (float)0));
						Graphics.Blit(temporary2, temporary4, this.blurAndFlaresMaterial, 4);
						this.blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(num6 * num4, num6 * num5, (float)0, (float)0));
						Graphics.Blit(temporary4, temporary2, this.blurAndFlaresMaterial, 4);
					}
					if (this.lensflareMode == Bloom.LensFlareStyle.Anamorphic)
					{
						this.AddTo(1f, temporary2, temporary3);
					}
					else
					{
						this.Vignette(1f, temporary2, temporary4);
						this.BlendFlares(temporary4, temporary2);
						this.AddTo(1f, temporary2, temporary3);
					}
				}
			}
			int pass = (int)bloomScreenBlendMode;
			this.screenBlend.SetFloat("_Intensity", this.bloomIntensity);
			this.screenBlend.SetTexture("_ColorBuffer", source);
			if (this.quality > Bloom.BloomQuality.Cheap)
			{
				Graphics.Blit(temporary3, temporary);
				Graphics.Blit(temporary, destination, this.screenBlend, pass);
			}
			else
			{
				Graphics.Blit(temporary3, destination, this.screenBlend, pass);
			}
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
			RenderTexture.ReleaseTemporary(temporary3);
			RenderTexture.ReleaseTemporary(temporary4);
		}
	}

	private void AddTo(float intensity_, RenderTexture from, RenderTexture to)
	{
		this.screenBlend.SetFloat("_Intensity", intensity_);
		Graphics.Blit(from, to, this.screenBlend, 9);
	}

	private void BlendFlares(RenderTexture from, RenderTexture to)
	{
		this.lensFlareMaterial.SetVector("colorA", new Vector4(this.flareColorA.r, this.flareColorA.g, this.flareColorA.b, this.flareColorA.a) * this.lensflareIntensity);
		this.lensFlareMaterial.SetVector("colorB", new Vector4(this.flareColorB.r, this.flareColorB.g, this.flareColorB.b, this.flareColorB.a) * this.lensflareIntensity);
		this.lensFlareMaterial.SetVector("colorC", new Vector4(this.flareColorC.r, this.flareColorC.g, this.flareColorC.b, this.flareColorC.a) * this.lensflareIntensity);
		this.lensFlareMaterial.SetVector("colorD", new Vector4(this.flareColorD.r, this.flareColorD.g, this.flareColorD.b, this.flareColorD.a) * this.lensflareIntensity);
		Graphics.Blit(from, to, this.lensFlareMaterial);
	}

	private void BrightFilter(float thresh, RenderTexture from, RenderTexture to)
	{
		this.brightPassFilterMaterial.SetVector("_Threshhold", new Vector4(thresh, thresh, thresh, thresh));
		Graphics.Blit(from, to, this.brightPassFilterMaterial, 0);
	}

	private void BrightFilter(Color threshColor, RenderTexture from, RenderTexture to)
	{
		this.brightPassFilterMaterial.SetVector("_Threshhold", threshColor);
		Graphics.Blit(from, to, this.brightPassFilterMaterial, 1);
	}

	private void Vignette(float amount, RenderTexture from, RenderTexture to)
	{
		if (this.lensFlareVignetteMask)
		{
			this.screenBlend.SetTexture("_ColorBuffer", this.lensFlareVignetteMask);
			Graphics.Blit((!(from == to)) ? from : null, to, this.screenBlend, (!(from == to)) ? 3 : 7);
		}
		else if (from != to)
		{
			Graphics.Blit(from, to);
		}
	}

	public override void Main()
	{
	}

	public Bloom.TweakMode tweakMode;

	public Bloom.BloomScreenBlendMode screenBlendMode;

	public Bloom.HDRBloomMode hdr;

	private bool doHdr;

	public float sepBlurSpread;

	public Bloom.BloomQuality quality;

	public float bloomIntensity;

	public float bloomThreshhold;

	public Color bloomThreshholdColor;

	public int bloomBlurIterations;

	public int hollywoodFlareBlurIterations;

	public float flareRotation;

	public Bloom.LensFlareStyle lensflareMode;

	public float hollyStretchWidth;

	public float lensflareIntensity;

	public float lensflareThreshhold;

	public float lensFlareSaturation;

	public Color flareColorA;

	public Color flareColorB;

	public Color flareColorC;

	public Color flareColorD;

	public float blurWidth;

	public Texture2D lensFlareVignetteMask;

	public Shader lensFlareShader;

	private Material lensFlareMaterial;

	public Shader screenBlendShader;

	private Material screenBlend;

	public Shader blurAndFlaresShader;

	private Material blurAndFlaresMaterial;

	public Shader brightPassFilterShader;

	private Material brightPassFilterMaterial;

	[Serializable]
	public enum LensFlareStyle
	{
		Ghosting,
		Anamorphic,
		Combined
	}

	[Serializable]
	public enum TweakMode
	{
		Basic,
		Complex
	}

	[Serializable]
	public enum HDRBloomMode
	{
		Auto,
		On,
		Off
	}

	[Serializable]
	public enum BloomScreenBlendMode
	{
		Screen,
		Add
	}

	[Serializable]
	public enum BloomQuality
	{
		Cheap,
		High
	}
}
