using System;
using UnityEngine;

public class FlagTouchScript : MonoBehaviour
{
	private void OnTriggerEnter(Collider c)
	{
		this.fs.MyOnCollisionEnter(c);
	}

	public FlagScript fs;
}
