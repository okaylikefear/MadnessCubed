using System;
using UnityEngine;

namespace Photon
{
	public class MonoBehaviour : MonoBehaviour
	{
		public PhotonView photonView
		{
			get
			{
				if (this.pvCache == null)
				{
					this.pvCache = PhotonView.Get(this);
				}
				return this.pvCache;
			}
		}

		[Obsolete("Use a photonView")]
		public new PhotonView networkView
		{
			get
			{
				UnityEngine.Debug.LogWarning("Why are you still using networkView? should be PhotonView?");
				return PhotonView.Get(this);
			}
		}

		private PhotonView pvCache;
	}
}
