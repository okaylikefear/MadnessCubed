using System;
using UnityEngine;

public class LevelUpEffectScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Time.time > this.nextFireWorkTime)
		{
			UnityEngine.Object.Instantiate(this.fireWorks, base.transform.position + UnityEngine.Random.insideUnitSphere * this.radius, Quaternion.identity);
			this.nextFireWorkTime = Time.time + UnityEngine.Random.Range(0.7f, 1.3f) * this.time / (float)this.numFireWorks;
		}
	}

	public GameObject fireWorks;

	public float radius = 30f;

	public float time = 4f;

	public int numFireWorks = 50;

	private float nextFireWorkTime;
}
