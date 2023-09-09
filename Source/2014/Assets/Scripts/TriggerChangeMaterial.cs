using System;
using UnityEngine;

public class TriggerChangeMaterial : MonoBehaviour
{
	public void SetState(int state)
	{
		if (state == 0)
		{
			GetComponent<Renderer>().material = this.offMaterial;
		}
		else
		{
			GetComponent<Renderer>().material = this.onMaterial;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public Material onMaterial;

	public Material offMaterial;
}
