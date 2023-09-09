using System;
using UnityEngine;

[Serializable]
public class SFE_spaceshipScript : MonoBehaviour
{
	public virtual void Start()
	{
	}

	public virtual void Update()
	{
		float x = this.myTarget.transform.position.x;
		Vector3 position = this.transform.position;
		float num = position.x = x;
		Vector3 vector = this.transform.position = position;
	}

	public virtual void Main()
	{
	}

	public GameObject myTarget;
}
