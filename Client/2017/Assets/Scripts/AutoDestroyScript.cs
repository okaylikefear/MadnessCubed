using System;
using kube.game;
using UnityEngine;

public class AutoDestroyScript : MonoBehaviour
{
	private void Start()
	{
		if (this.random)
		{
			this.timeToDestroy = UnityEngine.Random.Range(this.randomMin, this.randomMax);
		}
		CachedObject.Destroy(base.gameObject, this.timeToDestroy);
	}

	private void Update()
	{
	}

	public float timeToDestroy = 1f;

	public bool random;

	public float randomMin = 1f;

	public float randomMax = 2f;
}
