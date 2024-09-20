using System;
using UnityEngine;

public class BikeAntiRollScript : MonoBehaviour
{
	private void FixedUpdate()
	{
		Vector3 vector = base.transform.TransformPoint(Vector3.right);
		Vector3 vector2 = base.transform.TransformPoint(-Vector3.right);
		float num = this.AntiRoll;
		if (vector.y < vector2.y)
		{
			num = this.AntiRoll * Mathf.Abs(vector2.y - vector.y);
			base.GetComponent<Rigidbody>().AddRelativeTorque(0f, 0f, num);
		}
		else if (vector.y > vector2.y)
		{
			num = this.AntiRoll * Mathf.Abs(vector.y - vector2.y);
			base.GetComponent<Rigidbody>().AddRelativeTorque(0f, 0f, -num);
		}
	}

	public float AntiRoll = 5000f;
}
