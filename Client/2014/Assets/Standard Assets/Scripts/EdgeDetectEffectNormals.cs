using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Edge Detection (Geometry)")]
[Serializable]
public class EdgeDetectEffectNormals : PostEffectsBase
{
	public EdgeDetectEffectNormals()
	{
		this.mode = EdgeDetectMode.SobelDepthThin;
		this.sensitivityDepth = 1f;
		this.sensitivityNormals = 1f;
		this.edgeExp = 1f;
		this.sampleDist = 1f;
		this.edgesOnlyBgColor = Color.white;
		this.oldMode = EdgeDetectMode.SobelDepthThin;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(true);
		this.edgeDetectMaterial = this.CheckShaderAndCreateMaterial(this.edgeDetectShader, this.edgeDetectMaterial);
		if (this.mode != this.oldMode)
		{
			this.SetCameraFlag();
		}
		this.oldMode = this.mode;
		if (!this.isSupported)
		{
			this.ReportAutoDisable();
		}
		return this.isSupported;
	}

	public override void Start()
	{
		this.oldMode = this.mode;
	}

	public virtual void SetCameraFlag()
	{
		if (this.mode > EdgeDetectMode.RobertsCrossDepthNormals)
		{
			this.camera.depthTextureMode = (this.camera.depthTextureMode | DepthTextureMode.Depth);
		}
		else
		{
			this.camera.depthTextureMode = (this.camera.depthTextureMode | DepthTextureMode.DepthNormals);
		}
	}

	public override void OnEnable()
	{
		this.SetCameraFlag();
	}

	[ImageEffectOpaque]
	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.CheckResources())
		{
			Graphics.Blit(source, destination);
		}
		else
		{
			Vector2 vector = new Vector2(this.sensitivityDepth, this.sensitivityNormals);
			this.edgeDetectMaterial.SetVector("_Sensitivity", new Vector4(vector.x, vector.y, 1f, vector.y));
			this.edgeDetectMaterial.SetFloat("_BgFade", this.edgesOnly);
			this.edgeDetectMaterial.SetFloat("_SampleDistance", this.sampleDist);
			this.edgeDetectMaterial.SetVector("_BgColor", this.edgesOnlyBgColor);
			this.edgeDetectMaterial.SetFloat("_Exponent", this.edgeExp);
			Graphics.Blit(source, destination, this.edgeDetectMaterial, (int)this.mode);
		}
	}

	public override void Main()
	{
	}

	public EdgeDetectMode mode;

	public float sensitivityDepth;

	public float sensitivityNormals;

	public float edgeExp;

	public float sampleDist;

	public float edgesOnly;

	public Color edgesOnlyBgColor;

	public Shader edgeDetectShader;

	private Material edgeDetectMaterial;

	private EdgeDetectMode oldMode;
}
