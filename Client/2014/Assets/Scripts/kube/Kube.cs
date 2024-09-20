using System;
using System.Collections.Generic;
using UnityEngine;

namespace kube
{
	public class Kube
	{
		public static UnityEngine.Object Load(string path, Type systemTypeInstance)
		{
			if (Kube.ASS3 != null)
			{
				GameObject[] photonObjects = Kube.ASS3.photonObjects;
				for (int i = 0; i < photonObjects.Length; i++)
				{
					if (photonObjects[i].name == path)
					{
						return photonObjects[i];
					}
				}
			}
			UnityEngine.Object @object = Kube.SS.loadResource(path, systemTypeInstance);
			if (@object)
			{
				return @object;
			}
			return Resources.Load(path, systemTypeInstance);
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

		public const string VERSION = "2.7.5";

		public const string ONLINE_VERSION = "17";

		public static ObjectsHolderScript OH;

		public static ServerScript SS;

		public static InventoryScript IS;

		public static WorldHolderScript WHS;

		public static SocialNet SN;

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
