using System;
using kube;
using UnityEngine;

public class AssetsScript5 : AssetBase
{
	private void Awake()
	{
		Kube.ASS5 = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		for (int i = 0; i < this.clothesGO.Length; i++)
		{
			if (this.clothesGO[i] != null)
			{
				Kube.OH.clothesGO[i] = this.clothesGO[i];
			}
		}
		for (int j = 0; j < this.skinMats.Length; j++)
		{
			if (this.skinMats[j] != null)
			{
				Kube.OH.skinMats[j] = this.skinMats[j];
			}
		}
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		Kube.ASS5 = null;
	}

	public Material[] skinMats;

	public GameObject[] clothesGO;
}
