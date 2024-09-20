using System;
using Photon;
using UnityEngine;

namespace kube
{
	public class Pawn : Photon.MonoBehaviour
	{
		public void PlayerBlood(Vector3 pos, Vector3 normal)
		{
			Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
			UnityEngine.Object.Instantiate(Kube.ASS3.bloodSplash, pos, rotation);
			if (pos.y - base.transform.position.y > 1.1f && pos.y - base.transform.position.y < 1.8f)
			{
				UnityEngine.Object.Instantiate(Kube.ASS3.bloodSplash, pos, rotation);
			}
		}
	}
}
