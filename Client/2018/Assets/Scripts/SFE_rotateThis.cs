using System;
using UnityEngine;

[Serializable]
public class SFE_rotateThis : MonoBehaviour
{
	public SFE_rotateThis()
	{
		this.rotationSpeedX = (float)90;
		this.rotationVector = new Vector3(this.rotationSpeedX, this.rotationSpeedY, this.rotationSpeedZ);
	}

	public virtual void Update()
	{
		this.transform.Rotate(new Vector3(this.rotationSpeedX, this.rotationSpeedY, this.rotationSpeedZ) * Time.deltaTime);
	}

	public virtual void Main()
	{
	}

	public float rotationSpeedX;

	public float rotationSpeedY;

	public float rotationSpeedZ;

	private Vector3 rotationVector;
}
