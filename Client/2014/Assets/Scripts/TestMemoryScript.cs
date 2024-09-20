using System;
using UnityEngine;

public class TestMemoryScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.A))
		{
			if (this.testMemoryObj != null)
			{
				UnityEngine.Object.Destroy(this.testMemoryObj);
			}
			else
			{
				this.testMemoryObj = (UnityEngine.Object.Instantiate(this.testMemoryPrefab, Vector3.zero, Quaternion.identity) as GameObject);
			}
		}
	}

	public GameObject testMemoryPrefab;

	private GameObject testMemoryObj;
}
