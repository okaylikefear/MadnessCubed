using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace kube
{
	public class Kube
	{
		public static UnityEngine.Object Load(string path, Type systemTypeInstance)
		{
			string fileName = Path.GetFileName(path);
			List<GameObject> photonObjects = Kube.OH.photonObjects;
			for (int i = 0; i < photonObjects.Count; i++)
			{
				if (photonObjects[i] && photonObjects[i].name == fileName)
				{
					return photonObjects[i];
				}
			}
			UnityEngine.Object @object = Kube.RM.loadResource(path, systemTypeInstance);
			if (@object)
			{
				return @object;
			}
			return Resources.Load(path, systemTypeInstance);
		}

		public static UnityEngine.Object LoadAssetAtPath(string path, Type type)
		{
			char[] separator = new char[]
			{
				'/'
			};
			string[] array = path.Split(separator);
			string path2 = array[array.Length - 1].Replace(".prefab", string.Empty);
			if (array[1] != "bundles" && array[1] != "Resources")
			{
				UnityEngine.Debug.LogWarning("Bad resource path");
				return null;
			}
			return Kube.Load(path2, type);
		}

		public static void SendMonoMessage(string methodString, params object[] parameters)
		{
			HashSet<GameObject> hashSet = new HashSet<GameObject>();
			foreach (MonoBehaviour monoBehaviour in (MonoBehaviour[])UnityEngine.Object.FindObjectsOfType(typeof(MonoBehaviour)))
			{
				if (!hashSet.Contains(monoBehaviour.gameObject))
				{
					hashSet.Add(monoBehaviour.gameObject);
					if (parameters != null && parameters.Length == 1)
					{
						monoBehaviour.SendMessage(methodString, parameters[0], SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						monoBehaviour.SendMessage(methodString, parameters, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}

		public static void Ban()
		{
			Kube.OH.usedCheat = true;
			UnityEngine.Debug.Log("Ban");
			if (Kube.BCS)
			{
				Kube.BCS.BanMeImCheater();
			}
			UnityEngine.SceneManagement.SceneManager.LoadScene("Empty");
		}

		public const string VERSION = "A.5.2";

		public const string ONLINE_VERSION = "62";

		public static ObjectsHolderScript OH;

		public static IBaseServer SS;

		public static IBaseResource RM;

		public static InventoryScript IS;

		public static WorldHolderScript WHS;

		public static IPlatform SN;

		public static GameParamsScript GPS;

		public static TutorialScript TS;

		public static BattleControllerScript BCS;

		public static AssetsScript1 ASS1;

		public static AssetsScript2 ASS2;

		public static AssetsScript3 ASS3;

		public static AssetsScript4 ASS4;

		public static AssetsScript5 ASS5;

		public static AssetsScript6 ASS6;
	}
}
