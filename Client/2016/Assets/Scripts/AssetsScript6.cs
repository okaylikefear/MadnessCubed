using System;
using kube;
using UnityEngine;

public class AssetsScript6 : AssetBase
{
	private void Awake()
	{
		Kube.ASS6 = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		for (int i = 0; i < this.charWeaponsGO.Length; i++)
		{
			if (this.charWeaponsGO[i] != null)
			{
				Kube.OH.charWeaponsGO[i] = this.charWeaponsGO[i];
			}
		}
		for (int j = 0; j < this.gameItemsGO.Length; j++)
		{
			if (this.gameItemsGO[j] != null)
			{
				Kube.OH.gameItemsGO[j] = this.gameItemsGO[j];
			}
		}
		for (int k = 0; k < this.weaponsBulletPrefab.Length; k++)
		{
			if (this.weaponsBulletPrefab[k] != null)
			{
				Kube.OH.weaponsBulletPrefab[k] = this.weaponsBulletPrefab[k];
			}
		}
		for (int l = 0; l < this.weaponsSkins.Length; l++)
		{
			if (this.weaponsSkins[l] != null)
			{
				Kube.OH.weaponsSkin[l] = this.weaponsSkins[l];
			}
		}
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		Kube.ASS6 = null;
	}

	public GameObject[] gameItemsGO;

	public GameObject[] charWeaponsGO;

	public GameObject[] weaponsBulletPrefab;

	public Material[] weaponsSkins;
}
