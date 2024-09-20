using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Color Correction (3D Lookup Texture)")]
[ExecuteInEditMode]
[Serializable]
public class ColorCorrectionLut : PostEffectsBase
{
	public ColorCorrectionLut()
	{
		this.basedOnTempTex = string.Empty;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(false);
		this.material = this.CheckShaderAndCreateMaterial(this.shader, this.material);
		if (!this.isSupported)
		{
			this.ReportAutoDisable();
		}
		return this.isSupported;
	}

	public virtual void OnDisable()
	{
		if (this.material)
		{
			UnityEngine.Object.DestroyImmediate(this.material);
			this.material = null;
		}
	}

	public virtual void OnDestroy()
	{
		if (this.converted3DLut)
		{
			UnityEngine.Object.DestroyImmediate(this.converted3DLut);
		}
		this.converted3DLut = null;
	}

	public virtual void SetIdentityLut()
	{
		int num = 16;
		Color[] array = new Color[num * num * num];
		float num2 = 1f / (1f * (float)num - 1f);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				for (int k = 0; k < num; k++)
				{
					array[i + j * num + k * num * num] = new Color((float)i * 1f * num2, (float)j * 1f * num2, (float)k * 1f * num2, 1f);
				}
			}
		}
		if (this.converted3DLut)
		{
			UnityEngine.Object.DestroyImmediate(this.converted3DLut);
		}
		this.converted3DLut = new Texture3D(num, num, num, TextureFormat.ARGB32, false);
		this.converted3DLut.SetPixels(array);
		this.converted3DLut.Apply();
		this.basedOnTempTex = string.Empty;
	}

	public virtual bool ValidDimensions(Texture2D tex2d)
	{
		bool result;
		if (!tex2d)
		{
			result = false;
		}
		else
		{
			int height = tex2d.height;
			result = (height == Mathf.FloorToInt(Mathf.Sqrt((float)tex2d.width)));
		}
		return result;
	}

	public virtual void Convert(Texture2D temp2DTex, string path)
	{
		if (temp2DTex)
		{
			int num = temp2DTex.width * temp2DTex.height;
			num = temp2DTex.height;
			if (!this.ValidDimensions(temp2DTex))
			{
				UnityEngine.Debug.LogWarning("The given 2D texture " + temp2DTex.name + " cannot be used as a 3D LUT.");
				this.basedOnTempTex = string.Empty;
			}
			else
			{
				Color[] pixels = temp2DTex.GetPixels();
				Color[] array = new Color[pixels.Length];
				for (int i = 0; i < num; i++)
				{
					for (int j = 0; j < num; j++)
					{
						for (int k = 0; k < num; k++)
						{
							int num2 = num - j - 1;
							array[i + j * num + k * num * num] = pixels[k * num + i + num2 * num * num];
						}
					}
				}
				if (this.converted3DLut)
				{
					UnityEngine.Object.DestroyImmediate(this.converted3DLut);
				}
				this.converted3DLut = new Texture3D(num, num, num, TextureFormat.ARGB32, false);
				this.converted3DLut.SetPixels(array);
				this.converted3DLut.Apply();
				this.basedOnTempTex = path;
			}
		}
		else
		{
			UnityEngine.Debug.LogError("Couldn't color correct with 3D LUT texture. Image Effect will be disabled.");
		}
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.CheckResources())
		{
			Graphics.Blit(source, destination);
		}
		else
		{
			if (this.converted3DLut == null)
			{
				this.SetIdentityLut();
			}
			int width = this.converted3DLut.width;
			this.converted3DLut.wrapMode = TextureWrapMode.Clamp;
			this.material.SetFloat("_Scale", (float)(width - 1) / (1f * (float)width));
			this.material.SetFloat("_Offset", 1f / (2f * (float)width));
			this.material.SetTexture("_ClutTex", this.converted3DLut);
			Graphics.Blit(source, destination, this.material, (QualitySettings.activeColorSpace != ColorSpace.Linear) ? 0 : 1);
		}
	}

	public override void Main()
	{
	}

	public Shader shader;

	private Material material;

	public Texture3D converted3DLut;

	public string basedOnTempTex;
}
