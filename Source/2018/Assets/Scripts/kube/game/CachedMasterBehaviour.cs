using System;
using UnityEngine;

namespace kube.game
{
	public class CachedMasterBehaviour : MonoBehaviour
	{
		private void OnDestroy()
		{
			CachedObject.ClearCache();
		}
	}
}
