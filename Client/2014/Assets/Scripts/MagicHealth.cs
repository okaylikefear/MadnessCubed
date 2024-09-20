using System;
using kube;
using UnityEngine;

public class MagicHealth : MonoBehaviour
{
	private void SetParameters(int _playerId)
	{
		this.playerId = _playerId;
	}

	private void Start()
	{
		if (this.playerId == Kube.GPS.playerId)
		{
			Kube.IS.ps.RestoreHealth();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
	}

	private int playerId;
}
