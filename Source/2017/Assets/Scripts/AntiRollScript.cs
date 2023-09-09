using System;
using UnityEngine;

public class AntiRollScript : MonoBehaviour
{
	private void FixedUpdate()
	{
		float num = 1f;
		float num2 = 1f;
		WheelHit wheelHit;
		bool groundHit = this.WheelL.GetGroundHit(out wheelHit);
		if (groundHit)
		{
			num = (-this.WheelL.transform.InverseTransformPoint(wheelHit.point).y - this.WheelL.radius) / this.WheelL.suspensionDistance;
		}
		bool groundHit2 = this.WheelR.GetGroundHit(out wheelHit);
		if (groundHit2)
		{
			num2 = (-this.WheelR.transform.InverseTransformPoint(wheelHit.point).y - this.WheelR.radius) / this.WheelR.suspensionDistance;
		}
		float num3 = (num - num2) * this.AntiRoll;
		if (Mathf.Abs(num - num2) < 0.1f)
		{
			return;
		}
		if (groundHit)
		{
			base.GetComponent<Rigidbody>().AddForceAtPosition(this.WheelL.transform.up * -num3, this.WheelL.transform.position);
		}
		if (groundHit2)
		{
			base.GetComponent<Rigidbody>().AddForceAtPosition(this.WheelR.transform.up * num3, this.WheelR.transform.position);
		}
	}

	public WheelCollider WheelL;

	public WheelCollider WheelR;

	public float AntiRoll = 5000f;
}
