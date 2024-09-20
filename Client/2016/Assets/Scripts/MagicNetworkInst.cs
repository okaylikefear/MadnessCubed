using System;
using kube;
using UnityEngine;

public class MagicNetworkInst : MonoBehaviour
{
	private void SetParametersPos(Vector3 _pos)
	{
		this.pos = _pos;
	}

	private void SetParametersPoint(Vector3 _point)
	{
		this.shotPoint = _point;
	}

	private void SetParameters(int _playerId)
	{
		this.playerId = _playerId;
	}

	private void Start()
	{
		if (this.playerId == Kube.BCS.onlineId)
		{
			GameObject gameObject = PhotonNetwork.Instantiate(this.resourceName, this.pos, Quaternion.LookRotation(this.shotPoint - this.pos), 0);
			gameObject.SendMessage("SetPlayerId", this.playerId);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
	}

	private int playerId;

	public string resourceName;

	private Vector3 pos;

	private Vector3 shotPoint;
}
