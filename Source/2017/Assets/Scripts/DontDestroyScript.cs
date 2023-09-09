using System;
using UnityEngine;

public class DontDestroyScript : MonoBehaviour
{
	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
	}
}
