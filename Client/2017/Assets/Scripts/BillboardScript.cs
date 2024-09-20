using System;
using kube;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
	private void Start()
	{
		if (base.gameObject.layer == 14 && Kube.BCS.gameType != GameType.creating)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Update()
	{
		if (base.gameObject.layer == 14 && Kube.BCS.gameType != GameType.creating)
		{
			return;
		}
		Camera main = Camera.main;
		if (main != null)
		{
			base.transform.LookAt(main.transform.position);
		}
	}
}
