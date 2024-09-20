using System;
using UnityEngine;

public class LaserTraceScript : MonoBehaviour
{
	private void SetBulletTrace(Vector3 secondPos)
	{
		LineRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<LineRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetPosition(0, base.transform.position);
			componentsInChildren[i].SetPosition(1, secondPos);
		}
		UnityEngine.Object.Destroy(base.gameObject, this.lifeTime);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (base.transform.parent == null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public float lifeTime = 0.5f;
}
