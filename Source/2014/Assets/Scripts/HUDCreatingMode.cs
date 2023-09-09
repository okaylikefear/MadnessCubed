using System;
using UnityEngine;

public class HUDCreatingMode : MonoBehaviour
{
	private void Start()
	{
		this.rama.transform.position = this.boxes[0].transform.position;
	}

	private void BeginPlay()
	{
	}

	public void SetCube(int index)
	{
		this.rama.transform.position = this.boxes[index].transform.position;
	}

	public GameObject[] boxes;

	public GameObject rama;

	public GameObject xb;

	public GameObject zb;
}
