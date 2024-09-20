using System;
using UnityEngine;

[Serializable]
public class SFE_AddForceImpulse : MonoBehaviour
{
	public virtual void Start()
	{
		this.rigidbody.AddForce(this.transform.up * (float)10, ForceMode.Impulse);
	}

	public virtual void Update()
	{
	}

	public virtual void Main()
	{
	}
}
