using System;
using UnityEngine;

[Serializable]
public class SFE_AddRandomForceImpulse : MonoBehaviour
{
	public SFE_AddRandomForceImpulse()
	{
		this.rnd = (float)10;
	}

	public virtual void Start()
	{
		this.rigidbody.AddForce(UnityEngine.Random.Range(-this.rnd, this.rnd), UnityEngine.Random.Range(-this.rnd, this.rnd), UnityEngine.Random.Range(-this.rnd, this.rnd), ForceMode.Impulse);
	}

	public virtual void Update()
	{
	}

	public virtual void Main()
	{
	}

	public float rnd;
}
