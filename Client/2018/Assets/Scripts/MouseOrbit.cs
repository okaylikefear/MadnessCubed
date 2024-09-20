using System;
using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Orbit")]
[Serializable]
public class MouseOrbit : MonoBehaviour
{
	public MouseOrbit()
	{
		this.distance = 10f;
		this.xSpeed = 250f;
		this.ySpeed = 120f;
		this.yMinLimit = -20;
		this.yMaxLimit = 80;
		this.zoomSpeed = 10;
	}

	public virtual void Start()
	{
		Vector3 eulerAngles = this.transform.eulerAngles;
		this.x = eulerAngles.y;
		this.y = eulerAngles.x;
		if (this.GetComponent<Rigidbody>())
		{
			this.GetComponent<Rigidbody>().freezeRotation = true;
		}
	}

	public virtual void LateUpdate()
	{
		if (Input.GetMouseButton(2))
		{
			this.distance += UnityEngine.Input.GetAxis("Mouse Y") * (float)this.zoomSpeed * 0.02f;
		}
		if (UnityEngine.Input.GetKey("a") || UnityEngine.Input.GetAxis("Mouse ScrollWheel") > (float)0)
		{
			this.distance -= (float)this.zoomSpeed * 0.02f;
		}
		if (UnityEngine.Input.GetKey("z") || UnityEngine.Input.GetAxis("Mouse ScrollWheel") < (float)0)
		{
			this.distance += (float)this.zoomSpeed * 0.02f;
		}
		if (Input.GetMouseButton(0))
		{
			this.x += UnityEngine.Input.GetAxis("Mouse X") * this.xSpeed * 0.02f;
			this.y -= UnityEngine.Input.GetAxis("Mouse Y") * this.ySpeed * 0.02f;
			this.y = MouseOrbit.ClampAngle(this.y, (float)this.yMinLimit, (float)this.yMaxLimit);
		}
		if (UnityEngine.Input.GetKey("up") || UnityEngine.Input.GetKey("down"))
		{
			this.y += UnityEngine.Input.GetAxis("Vertical") * this.ySpeed * 0.02f;
			this.y = MouseOrbit.ClampAngle(this.y, (float)this.yMinLimit, (float)this.yMaxLimit);
		}
		if (UnityEngine.Input.GetKey("left") || UnityEngine.Input.GetKey("right"))
		{
			this.x -= UnityEngine.Input.GetAxis("Horizontal") * this.xSpeed * 0.02f;
		}
		if (this.target)
		{
			Quaternion rotation = Quaternion.Euler(this.y, this.x, (float)0);
			Vector3 position = rotation * new Vector3((float)0, (float)0, -this.distance) + this.target.position;
			this.transform.rotation = rotation;
			this.transform.position = position;
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < (float)-360)
		{
			angle += (float)360;
		}
		if (angle > (float)360)
		{
			angle -= (float)360;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public virtual void Main()
	{
	}

	public Transform target;

	public float distance;

	public float xSpeed;

	public float ySpeed;

	public int yMinLimit;

	public int yMaxLimit;

	public int zoomSpeed;

	private float x;

	private float y;
}
