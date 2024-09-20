using System;
using UnityEngine;

public class ExplosionTrailScript : MonoBehaviour
{
	private void Start()
	{
		this.startTime = Time.time;
		this.dir = UnityEngine.Random.onUnitSphere;
	}

	private void Update()
	{
		float num = (Time.time - this.startTime) / this.timeOfLife;
		if (num > 0f)
		{
			base.transform.Translate((this.dir * this.speed * (1f - num) - Vector3.up * this.gravitation) * Time.deltaTime, Space.World);
		}
	}

	public float speed = 10f;

	public float timeOfLife = 3f;

	public float gravitation = 1f;

	private float startTime;

	private Vector3 dir;
}
