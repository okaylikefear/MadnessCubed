using System;
using UnityEngine;

[Serializable]
public class CameraFollow : MonoBehaviour
{
	public CameraFollow()
	{
		this.camOffset = Vector3.zero;
	}

	public virtual void Start()
	{
		this.camOffset = this.transform.position - this.followTarget.position;
	}

	public virtual void Update()
	{
		this.transform.position = this.followTarget.position + this.camOffset;
	}

	public virtual void Main()
	{
	}

	private Vector3 camOffset;

	public Transform followTarget;
}
