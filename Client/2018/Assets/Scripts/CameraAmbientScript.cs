using System;
using kube;
using UnityEngine;

public class CameraAmbientScript : MonoBehaviour
{
	private void Awake()
	{
		this._audio = base.GetComponent<AudioSource>();
		this._cam = base.GetComponent<Camera>();
	}

	private void Start()
	{
		this.defaultFog = RenderSettings.fog;
		this.defaultFogColor = RenderSettings.fogColor;
		this.defaultFogDensity = RenderSettings.fogDensity;
		this.defaultFarClipPlane = this._cam.farClipPlane;
		this.OnQualitySettings();
	}

	private void OnQualitySettings()
	{
		float farClipPlane = Mathf.Clamp((float)(128 * (QualitySettings.GetQualityLevel() + 1)), 128f, this.defaultFarClipPlane);
		this._cam.farClipPlane = farClipPlane;
		float[] array = new float[32];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Mathf.Clamp((float)(32 * (QualitySettings.GetQualityLevel() + 1)), 32f, this.defaultFarClipPlane);
		}
		array[8] = 0f;
		array[4] = 0f;
		array[10] = 0f;
		this._cam.layerCullDistances = array;
		RenderSettings.fogEndDistance = array[0];
		this.defaultFogDensity = array[0] / this.defaultFarClipPlane;
		RenderSettings.fogDensity = this.defaultFogDensity;
	}

	private void Update()
	{
		CubePhys cubePhysType = Kube.WHS.GetCubePhysType(base.transform.position);
		if (this.currentCubeType == cubePhysType && Kube.WHS.DayLight == this.currentDayLight)
		{
			return;
		}
		this.currentDayLight = Kube.WHS.DayLight;
		if (cubePhysType == CubePhys.water)
		{
			this._audio.Play();
			RenderSettings.fog = true;
			RenderSettings.fogColor = new Color(0f, 0.4f, 0.7f, 0.6f);
			RenderSettings.fogDensity = 0.08f;
		}
		else if (cubePhysType != CubePhys.water)
		{
			RenderSettings.fog = this.defaultFog;
			if (Kube.WHS.DayLight == 0)
			{
				RenderSettings.fogColor = new Color(0f, 0f, 0f, 0.9f);
			}
			else
			{
				RenderSettings.fogColor = this.defaultFogColor;
			}
			RenderSettings.fogDensity = this.defaultFogDensity;
			this._audio.Stop();
		}
		this.currentCubeType = cubePhysType;
	}

	private void OnGUI()
	{
		GUI.depth = 0;
		if (this.currentCubeType == CubePhys.water)
		{
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), Kube.ASS3.underWaterTex);
		}
	}

	private bool defaultFog;

	private Color defaultFogColor;

	private float defaultFogDensity;

	private float defaultFarClipPlane;

	private AudioSource _audio;

	private Camera _cam;

	private CubePhys currentCubeType;

	private int currentDayLight = 1;

	private int currentFrame;
}
