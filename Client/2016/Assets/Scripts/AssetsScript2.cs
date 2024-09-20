using System;
using kube;
using UnityEngine;

public class AssetsScript2 : AssetBase
{
	private void Awake()
	{
		Kube.ASS2 = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Kube.OH.AAselectMat = UnityEngine.Object.Instantiate<Material>(this.AAselectMat);
		for (int i = 0; i < this.gameItemsTex.Length; i++)
		{
			if (this.gameItemsTex[i] != null && !Kube.OH.gameItemsTex.ContainsKey(i))
			{
				Kube.OH.gameItemsTex[i] = this.gameItemsTex[i];
			}
		}
		for (int j = 0; j < this.inventarSkinsTex.Length; j++)
		{
			if (this.inventarSkinsTex[j] != null)
			{
				Kube.OH.inventarSkinsTex[j] = this.inventarSkinsTex[j];
			}
		}
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		Kube.ASS2 = null;
	}

	public static int IntParseFast(string value)
	{
		int num = 0;
		foreach (char c in value)
		{
			if (c <= ':' && c >= '0')
			{
				num = 10 * num + (int)(c - '0');
			}
		}
		return num;
	}

	public Material AAselectMat;

	public Texture[] AAselectTex;

	public Texture[] gameItemsTex;

	public Texture[] specItemsInvTex;

	public Material[] RankTex;

	public Texture vipTex;

	public GameObject bloodSplash;

	public Texture logoScreen;

	public Texture[] inventarWeaponsTex;

	public Texture[] inventarWeaponsSkinTex;

	public Texture[] inventarBulletsTex;

	public Texture itemClosedTex;

	public Texture[] bonusTex;

	public Texture frags;

	public Texture[] inventarSkinsTex;

	public Texture[] inventarClothesTex;

	public Texture[] inventarCubesTex;
}
