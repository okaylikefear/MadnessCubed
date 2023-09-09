using System;
using UnityEngine;

public class LightningSphereScript : MonoBehaviour
{
	private void Start()
	{
		this.startTime = Time.time;
	}

	private void Update()
	{
		base.transform.RotateAround(Vector3.up, this.rotationSpeed * Time.deltaTime);
		Vector3 localScale = base.transform.localScale * (1f + this.scaleSpeed * Time.deltaTime);
		base.transform.localScale = localScale;
		Color color = GetComponent<Renderer>().material.color;
		color.a = Mathf.Lerp(1f, 0f, (Time.time - this.startTime) / this.lifeTime);
		GetComponent<Renderer>().material.color = color;
	}

	public float rotationSpeed = 360f;

	public float scaleSpeed = 5f;

	public float lifeTime = 2f;

	private float startTime;
}
