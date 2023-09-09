using System;
using UnityEngine;

public class MenuCharScript : MonoBehaviour
{
	private void Start()
	{
		base.GetComponent<Animation>()["standEmpty"].speed = 0.15f;
	}

	private void Update()
	{
	}
}
