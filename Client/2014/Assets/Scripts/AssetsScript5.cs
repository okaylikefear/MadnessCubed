using System;
using kube;
using UnityEngine;

public class AssetsScript5 : AssetBase
{
	private void Awake()
	{
		Kube.ASS5 = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
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
