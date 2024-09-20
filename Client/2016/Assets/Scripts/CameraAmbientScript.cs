using System;
using kube;
using UnityEngine;

public class CameraAmbientScript : MonoBehaviour
{
	private void Start()
	{
		this.defaultFog = RenderSettings.fog;
		this.defaultFogColor = RenderSettings.fogColor;
		this.defaultFogDensity = RenderSettings.fogDensity;
	}

	private void Update()
	{
		CubePhys cubePhysType = Kube.WHS.GetCubePhysType(base.transform.position);
		if (cubePhysType == CubePhys.water && this.currentCubeType != CubePhys.water)
		{
			base.GetComponent<AudioSource>().Play();
			RenderSettings.fog = true;
			RenderSettings.fogColor = new Color(0f, 0.4f, 0.7f, 0.6f);
			RenderSettings.fogDensity = 0.08f;
			this.isUnderwater = true;
		}
		else if (cubePhysType != CubePhys.water && this.currentCubeType == CubePhys.water)
		{
			RenderSettings.fog = this.defaultFog;
			RenderSettings.fogColor = this.defaultFogColor;
			RenderSettings.fogDensity = this.defaultFogDensity;
			base.GetComponent<AudioSource>().Stop();
			this.isUnderwater = false;
		}
		this.currentCubeType = cubePhysType;
	}

	private void OnGUI()
	{
		GUI.depth = 0;
		if (this.isUnderwater)
		{
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), Kube.ASS3.underWaterTex);
		}
	}

	private bool defaultFog;

	private Color defaultFogColor;

	private float defaultFogDensity;

	private CubePhys currentCubeType;

	private bool isUnderwater;
}
