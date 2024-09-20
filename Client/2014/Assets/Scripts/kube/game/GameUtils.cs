using System;
using UnityEngine;

namespace kube.game
{
	public class GameUtils
	{
		public static void ChangeLayersRecursively(Transform trans, string name)
		{
			int layer = LayerMask.NameToLayer(name);
			foreach (object obj in trans)
			{
				Transform transform = (Transform)obj;
				transform.gameObject.layer = layer;
				GameUtils.ChangeLayersRecursively(transform, name);
			}
		}
	}
}
