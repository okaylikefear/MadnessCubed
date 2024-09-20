using System;
using UnityEngine;

public class TorchLightScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		base.light.intensity = this.i0 + this.a1 * Mathf.Sin(this.f1 * Time.time) + this.a2 * Mathf.Sin(this.f2 * Time.time);
	}

	public float i0 = 1.2f;

	public float a1 = 0.3f;

	public float f1 = 1f;

	public float a2 = 0.1f;

	public float f2 = 2.1f;
}
