using System;
using UnityEngine;

public class WeaponOnTheMapScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		base.transform.localPosition = new Vector3(0f, this.height + this.flyingHeightAmp * Mathf.Sin(Time.time * 2f * 3.14159274f / this.flyingHeightPeriod), 0f);
		base.transform.RotateAround(Vector3.up, this.flyingRotationSpeed * Time.deltaTime);
	}

	public float flyingHeightAmp = 0.2f;

	public float flyingHeightPeriod = 2f;

	public float flyingRotationSpeed = 1.5f;

	public float height = 0.5f;
}
