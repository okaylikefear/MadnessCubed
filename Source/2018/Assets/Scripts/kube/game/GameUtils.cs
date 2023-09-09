using System;
using UnityEngine;

namespace kube.game
{
	public class GameUtils
	{
		public static string AssetName(string path)
		{
			string[] array = path.Split(GameUtils.s);
			return array[array.Length - 1];
		}

		public static void ChangeLayersRecursively(Transform trans, string name)
		{
			int num = LayerMask.NameToLayer(name);
			foreach (object obj in trans)
			{
				Transform transform = (Transform)obj;
				transform.gameObject.layer = num;
				GameUtils.ChangeLayersRecursively(transform, num);
			}
		}

		public static void ChangeLayersRecursively(Transform trans, int lid)
		{
			foreach (object obj in trans)
			{
				Transform transform = (Transform)obj;
				transform.gameObject.layer = lid;
				GameUtils.ChangeLayersRecursively(transform, lid);
			}
		}

		public static Transform FindChildRecursively(Transform trans, string name)
		{
			if (trans.gameObject.name == name)
			{
				return trans;
			}
			foreach (object obj in trans)
			{
				Transform trans2 = (Transform)obj;
				Transform transform = GameUtils.FindChildRecursively(trans2, name);
				if (transform)
				{
					return transform;
				}
			}
			return null;
		}

		public static GameObject FindChildRecursively(GameObject go, string name)
		{
			Transform transform = GameUtils.FindChildRecursively(go.transform, name);
			if (transform != null)
			{
				return transform.gameObject;
			}
			return null;
		}

		private static char[] s = new char[]
		{
			'/'
		};
	}
}
