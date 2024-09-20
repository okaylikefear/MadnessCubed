using System;
using UnityEngine;

public class AutoDestroyScript : MonoBehaviour
{
	private void Start()
	{
		if (!this.random)
		{
			UnityEngine.Object.Destroy(base.gameObject, this.timeToDestroy);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject, UnityEngine.Random.Range(this.randomMin, this.randomMax));
		}
	}

	private void Update()
	{
	}

	public float timeToDestroy = 1f;

	public bool random;

	public float randomMin = 1f;

	public float randomMax = 2f;
}
