using System;
using UnityEngine;

public class FireTempScript : MonoBehaviour
{
	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.emitters = base.gameObject.GetComponentsInChildren<ParticleEmitter>();
		this.initialized = true;
	}

	private void Start()
	{
		this.Init();
		Collider[] array = Physics.OverlapSphere(base.transform.position, 2f);
		int num = -1;
		float num2 = 10000f;
		for (int i = 0; i < array.Length; i++)
		{
			float magnitude = array[i].ClosestPointOnBounds(base.transform.position).magnitude;
			if (magnitude < num2)
			{
				num2 = magnitude;
				num = i;
			}
		}
		if (num >= 0)
		{
			base.transform.parent = array[num].transform;
		}
		base.Invoke("CancelEmit", 10f);
	}

	private void CancelEmit()
	{
		this.emitters = base.gameObject.GetComponentsInChildren<ParticleEmitter>();
		for (int i = 0; i < this.emitters.Length; i++)
		{
			this.emitters[i].emit = false;
		}
		UnityEngine.Object.Destroy(base.gameObject, 3f);
	}

	private void Update()
	{
	}

	private ParticleEmitter[] emitters;

	private bool initialized;
}
