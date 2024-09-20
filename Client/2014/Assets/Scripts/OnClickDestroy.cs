using System;
using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnClickDestroy : Photon.MonoBehaviour
{
	private void OnClick()
	{
		if (!this.DestroyByRpc)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		else
		{
			base.photonView.RPC("DestroyRpc", PhotonTargets.AllBuffered, new object[0]);
		}
	}

	[RPC]
	public void DestroyRpc()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		PhotonNetwork.UnAllocateViewID(base.photonView.viewID);
	}

	public bool DestroyByRpc;
}
