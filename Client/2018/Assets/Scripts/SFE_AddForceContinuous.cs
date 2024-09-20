using System;
using UnityEngine;

[Serializable]
public class SFE_AddForceContinuous : MonoBehaviour
{
	public SFE_AddForceContinuous()
	{
		this.force = (float)10;
	}

	public virtual void Start()
	{
	}

	public virtual void Update()
	{
		this.GetComponent<Rigidbody>().AddForce(this.transform.up * this.force * Time.deltaTime, ForceMode.Impulse);
	}

	public virtual void Main()
	{
	}

	public float force;
}
