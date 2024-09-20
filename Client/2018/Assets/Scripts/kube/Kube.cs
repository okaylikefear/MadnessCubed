using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

		public static void AddListener(GameObject t)
		{
			if (Kube.SendMonoMessageTargets == null)
			{
				Kube.SendMonoMessageTargets = new HashSet<GameObject>();
			}
			Kube.SendMonoMessageTargets.Add(t);
		}

		public static void RemoveListener(GameObject t)
		{
			if (Kube.SendMonoMessageTargets == null)
			{
				return;
			}
			Kube.SendMonoMessageTargets.Remove(t);
		}

		private static void InvokeIfExists(object objectToCheck, string methodName, params object[] parameters)
		{
			Type type = objectToCheck.GetType();
			MethodInfo method = type.GetMethod(methodName);
			if (method != null && method.GetParameters().Length == parameters.Length)
			{
				method.Invoke(objectToCheck, parameters);
			}
		}

		private static void SendMessageToAll(GameObject gameobject, string methodName, object[] parameters)
		{
			MonoBehaviour[] componentsInChildren = gameobject.GetComponentsInChildren<MonoBehaviour>(true);
			foreach (MonoBehaviour objectToCheck in componentsInChildren)
			{
				Kube.InvokeIfExists(objectToCheck, methodName, parameters);
			}
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
			if (Kube.SendMonoMessageTargets == null)
			{
				return;
			}
			foreach (GameObject gameObject in Kube.SendMonoMessageTargets)
			{
				if (!hashSet.Contains(gameObject))
				{
					Kube.SendMessageToAll(gameObject, methodString, parameters);
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

		public const string VERSION = "C.4.1";

		public const string ONLINE_VERSION = "68";

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

		public static HashSet<GameObject> SendMonoMessageTargets;
	}
}
