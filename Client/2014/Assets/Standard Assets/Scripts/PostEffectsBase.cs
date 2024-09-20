using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[Serializable]
public class PostEffectsBase : MonoBehaviour
{
	public PostEffectsBase()
	{
		this.supportHDRTextures = true;
		this.isSupported = true;
	}

	public virtual Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
	{
		Material result;
		if (!s)
		{
			UnityEngine.Debug.Log("Missing shader in " + this.ToString());
			this.enabled = false;
			result = null;
		}
		else if (s.isSupported && m2Create && m2Create.shader == s)
		{
			result = m2Create;
		}
		else if (!s.isSupported)
		{
			this.NotSupported();
			UnityEngine.Debug.Log("The shader " + s.ToString() + " on effect " + this.ToString() + " is not supported on this platform!");
			result = null;
		}
		else
		{
			m2Create = new Material(s);
			m2Create.hideFlags = HideFlags.DontSave;
			result = ((!m2Create) ? null : m2Create);
		}
		return result;
	}

	public virtual Material CreateMaterial(Shader s, Material m2Create)
	{
		Material result;
		if (!s)
		{
			UnityEngine.Debug.Log("Missing shader in " + this.ToString());
			result = null;
		}
		else if (m2Create && m2Create.shader == s && s.isSupported)
		{
			result = m2Create;
		}
		else if (!s.isSupported)
		{
			result = null;
		}
		else
		{
			m2Create = new Material(s);
			m2Create.hideFlags = HideFlags.DontSave;
			result = ((!m2Create) ? null : m2Create);
		}
		return result;
	}

	public virtual void OnEnable()
	{
		this.isSupported = true;
	}

	public virtual bool CheckSupport()
	{
		return this.CheckSupport(false);
	}

	public virtual bool CheckResources()
	{
		UnityEngine.Debug.LogWarning("CheckResources () for " + this.ToString() + " should be overwritten.");
		return this.isSupported;
	}

	public virtual void Start()
	{
		this.CheckResources();
	}

	public virtual bool CheckSupport(bool needDepth)
	{
		this.isSupported = true;
		this.supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
		bool flag;
		if (flag = (SystemInfo.graphicsShaderLevel >= 50))
		{
			flag = SystemInfo.supportsComputeShaders;
		}
		this.supportDX11 = flag;
		bool result;
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			this.NotSupported();
			result = false;
		}
		else if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			this.NotSupported();
			result = false;
		}
		else
		{
			if (needDepth)
			{
				this.camera.depthTextureMode = (this.camera.depthTextureMode | DepthTextureMode.Depth);
			}
			result = true;
		}
		return result;
	}

	public virtual bool CheckSupport(bool needDepth, bool needHdr)
	{
		bool result;
		if (!this.CheckSupport(needDepth))
		{
			result = false;
		}
		else if (needHdr && !this.supportHDRTextures)
		{
			this.NotSupported();
			result = false;
		}
		else
		{
			result = true;
		}
		return result;
	}

	public virtual bool Dx11Support()
	{
		return this.supportDX11;
	}

	public virtual void ReportAutoDisable()
	{
		UnityEngine.Debug.LogWarning("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
	}

	public virtual bool CheckShader(Shader s)
	{
		UnityEngine.Debug.Log("The shader " + s.ToString() + " on effect " + this.ToString() + " is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package.");
		bool result;
		if (!s.isSupported)
		{
			this.NotSupported();
			result = false;
		}
		else
		{
			result = false;
		}
		return result;
	}

	public virtual void NotSupported()
	{
		this.enabled = false;
		this.isSupported = false;
	}

	public virtual void DrawBorder(RenderTexture dest, Material material)
	{
		float x = 0f;
		float x2 = 0f;
		float y = 0f;
		float y2 = 0f;
		RenderTexture.active = dest;
		bool flag = true;
		GL.PushMatrix();
		GL.LoadOrtho();
		for (int i = 0; i < material.passCount; i++)
		{
			material.SetPass(i);
			float y3 = 0f;
			float y4 = 0f;
			if (flag)
			{
				y3 = 1f;
				y4 = (float)0;
			}
			else
			{
				y3 = (float)0;
				y4 = 1f;
			}
			x = (float)0;
			x2 = (float)0 + 1f / ((float)dest.width * 1f);
			y = (float)0;
			y2 = 1f;
			GL.Begin(7);
			GL.TexCoord2((float)0, y3);
			GL.Vertex3(x, y, 0.1f);
			GL.TexCoord2(1f, y3);
			GL.Vertex3(x2, y, 0.1f);
			GL.TexCoord2(1f, y4);
			GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2((float)0, y4);
			GL.Vertex3(x, y2, 0.1f);
			x = 1f - 1f / ((float)dest.width * 1f);
			x2 = 1f;
			y = (float)0;
			y2 = 1f;
			GL.TexCoord2((float)0, y3);
			GL.Vertex3(x, y, 0.1f);
			GL.TexCoord2(1f, y3);
			GL.Vertex3(x2, y, 0.1f);
			GL.TexCoord2(1f, y4);
			GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2((float)0, y4);
			GL.Vertex3(x, y2, 0.1f);
			x = (float)0;
			x2 = 1f;
			y = (float)0;
			y2 = (float)0 + 1f / ((float)dest.height * 1f);
			GL.TexCoord2((float)0, y3);
			GL.Vertex3(x, y, 0.1f);
			GL.TexCoord2(1f, y3);
			GL.Vertex3(x2, y, 0.1f);
			GL.TexCoord2(1f, y4);
			GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2((float)0, y4);
			GL.Vertex3(x, y2, 0.1f);
			x = (float)0;
			x2 = 1f;
			y = 1f - 1f / ((float)dest.height * 1f);
			y2 = 1f;
			GL.TexCoord2((float)0, y3);
			GL.Vertex3(x, y, 0.1f);
			GL.TexCoord2(1f, y3);
			GL.Vertex3(x2, y, 0.1f);
			GL.TexCoord2(1f, y4);
			GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2((float)0, y4);
			GL.Vertex3(x, y2, 0.1f);
			GL.End();
		}
		GL.PopMatrix();
	}

	public virtual void Main()
	{
	}

	protected bool supportHDRTextures;

	protected bool supportDX11;

	protected bool isSupported;
}
