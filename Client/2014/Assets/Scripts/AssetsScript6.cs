using System;
using kube;
using UnityEngine;

public class AssetsScript6 : AssetBase
{
	private void Awake()
	{
		Kube.ASS6 = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		Kube.ASS6 = null;
	}

	public GameObject[] charWeaponsGO;

	public GameObject[] weaponsBulletPrefab;
}
