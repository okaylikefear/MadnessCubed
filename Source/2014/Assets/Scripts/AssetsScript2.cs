using System;
using System.Collections.Generic;
using System.IO;
using kube;
using UnityEngine;

public class AssetsScript2 : AssetBase
{
	private void Awake()
	{
		Kube.ASS2 = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
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

	private Texture[] LoadAssetAtPath(string path)
	{
		string[] files = Directory.GetFiles(Application.dataPath + path, "*.png");
		int[] array = new int[files.Length];
		for (int i = 0; i < array.Length; i++)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(files[i]);
			if (fileNameWithoutExtension.Length > 2 && fileNameWithoutExtension[1] == '_')
			{
				array[i] = AssetsScript2.IntParseFast(fileNameWithoutExtension.Substring(2));
			}
			else
			{
				array[i] = AssetsScript2.IntParseFast(fileNameWithoutExtension);
			}
		}
		Array.Sort<int, string>(array, files);
		List<Texture> list = new List<Texture>();
		foreach (string text in files)
		{
			string text2 = "Assets" + text.Replace(Application.dataPath, string.Empty).Replace('\\', '/');
			UnityEngine.Debug.Log(text2);
			list.Add((Texture)Resources.LoadAssetAtPath(text2, typeof(Texture)));
		}
		return list.ToArray();
	}

	[ContextMenu("Execute")]
	private void collect()
	{
		Texture[] array = this.LoadAssetAtPath("/GUI2/Char/skin");
		this.inventarSkinsTex = array;
		array = this.LoadAssetAtPath("/GUI2/Char/hat");
		this.inventarClothesHeadTex = array;
		array = this.LoadAssetAtPath("/GUI2/Char/armor");
		this.inventarClothesTorsoTex = array;
		array = this.LoadAssetAtPath("/GUI2/Char/satchel");
		this.inventarClothesSpineTex = array;
		array = this.LoadAssetAtPath("/GUI2/Char/hand");
		this.inventarClothesHandTex = array;
		array = this.LoadAssetAtPath("/GUI2/Char/food");
		this.inventarClothesLegTex = array;
		array = this.LoadAssetAtPath("/GUI2/Char/shield");
		this.inventarClothesClavTex = array;
	}

	[ContextMenu("Execute Cubes")]
	private void collectcub()
	{
		this.inventarCubesTex = new Texture[this.cubesTex.Length];
		for (int i = 0; i < this.cubesTex.Length; i++)
		{
			UnityEngine.Debug.Log(this.cubesTex[i].name);
			this.inventarCubesTex[i] = (Texture)Resources.LoadAssetAtPath("Assets/GUI2/cubes/" + this.cubesTex[i].name + ".png", typeof(Texture));
		}
	}

	public Texture[] monsterTex;

	public Texture[] transportTex;

	public Texture[] cubesTex;

	public Material[] miniCubesMat;

	public Material AAselectMat;

	public Texture[] AAselectTex;

	public Texture[] gameItemsTex;

	public Texture[] specItemsInvTex;

	public Material[] RankTex;

	public Texture vipTex;

	public GameObject bloodSplash;

	public Texture logoScreen;

	public Texture[] inventarWeaponsTex;

	public Texture[] inventarBulletsTex;

	public Texture itemClosedTex;

	public Texture[] bonusTex;

	public Texture frags;

	public Texture[] inventarSkinsTex;

	public Texture[] inventarClothesHeadTex;

	public Texture[] inventarClothesTorsoTex;

	public Texture[] inventarClothesSpineTex;

	public Texture[] inventarClothesHandTex;

	public Texture[] inventarClothesLegTex;

	public Texture[] inventarClothesClavTex;

	public Texture[] inventarCubesTex;

	public Texture[] inventarWeaponTexReady;
}
