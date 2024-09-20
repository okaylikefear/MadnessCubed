using System;
using kube;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
	private void SetTemporaryTransform(Transform tempTransform)
	{
		if (tempTransform == null)
		{
			return;
		}
		this.isTargetTemporaryTransform = true;
		this.temporaryTransform = tempTransform;
	}

	private void UnsetTemporaryTransform()
	{
		this.isTargetTemporaryTransform = false;
		base.transform.localPosition = this.initPosition;
		base.transform.localRotation = this.initRotation;
	}

	private void Start()
	{
		this.initPosition = base.transform.localPosition;
		this.initRotation = base.transform.localRotation;
	}

	private void Update()
	{
		if (Kube.OH != null)
		{
			this.lastPos = base.transform.position;
			this.lastRot = base.transform.rotation;
		}
		if (this.mustCamPos != Vector3.zero)
		{
			Ray ray = new Ray(base.transform.parent.position, base.transform.TransformDirection(this.mustCamPos));
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, this.mustCamPos.magnitude, 511))
			{
				base.transform.position = raycastHit.point - base.transform.InverseTransformDirection(this.mustCamPos.normalized) * 0.5f;
			}
			else
			{
				base.transform.localPosition = this.mustCamPos;
			}
		}
		else
		{
			base.transform.localPosition = Vector3.zero;
		}
	}

	private void LateUpdate()
	{
		if (this.isTargetTemporaryTransform)
		{
			base.transform.position = this.temporaryTransform.position;
			base.transform.rotation = this.temporaryTransform.rotation;
		}
	}

	private void SetPosition(Vector3 camPos)
	{
		this.mustCamPos = camPos;
	}

	public float rotationKoeff = 5f;

	public float posKoeff = 5f;

	private Vector3 initPosition;

	private Quaternion initRotation;

	private bool isTargetTemporaryTransform;

	private Transform temporaryTransform;

	private Vector3 lastPos;

	private Quaternion lastRot;

	public Vector3 mustCamPos;
}
