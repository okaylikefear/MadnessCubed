using System;
using UnityEngine;

public class PointTextScript : MonoBehaviour
{
	private void SetText(string text)
	{
		base.GetComponent<TextMesh>().text = text;
	}

	private void Start()
	{
		this.mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}

	private void LateUpdate()
	{
		if (!this.mainCamera)
		{
			return;
		}
		if (this.flyUp)
		{
			base.transform.position += Vector3.up * Time.deltaTime;
		}
		base.transform.LookAt(this.mainCamera.transform);
		base.transform.Rotate(Vector3.up, 180f, Space.Self);
	}

	public bool flyUp = true;

	private GameObject mainCamera;
}
