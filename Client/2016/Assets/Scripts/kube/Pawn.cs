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

		protected void DestroyPhotonView()
		{
			PhotonNetwork.Destroy(base.gameObject);
		}

		public static void CopyTransformsRecurse(Transform src, Transform dst)
		{
			dst.position = src.position;
			dst.rotation = src.rotation;
			if (dst.gameObject.GetComponent<Rigidbody>() != null)
			{
				dst.gameObject.GetComponent<Rigidbody>().Sleep();
			}
			foreach (object obj in dst)
			{
				Transform transform = (Transform)obj;
				Transform transform2 = src.Find(transform.name);
				if (transform2)
				{
					Pawn.CopyTransformsRecurse(transform2, transform);
				}
			}
		}

		public virtual int getTeam()
		{
			return -2;
		}

		public bool dead;
	}
}
