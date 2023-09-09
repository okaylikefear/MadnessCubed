using System;
using kube;
using UnityEngine;

public class MagicBullets : MonoBehaviour
{
	private void SetParameters(int _playerId)
	{
		this.playerId = _playerId;
	}

	private void Start()
	{
		if (this.playerId == Kube.GPS.playerId)
		{
			Kube.IS.ps.RestoreBullets(Kube.OH.GetServerCode(this.bulletsToRestore[0], 2) + Kube.OH.GetServerCode(this.bulletsToRestore[1], 2) + Kube.OH.GetServerCode(this.bulletsToRestore[2], 2) + Kube.OH.GetServerCode(this.bulletsToRestore[3], 2));
			base.transform.position = Kube.IS.ps.transform.position;
		}
		if (GetComponent<AudioSource>())
		{
			GetComponent<AudioSource>().Stop();
			GetComponent<AudioSource>().Play();
		}
		UnityEngine.Object.Destroy(base.gameObject, 1f);
	}

	private void Update()
	{
	}

	private int playerId;

	public int[] bulletsToRestore;
}
