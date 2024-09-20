using System;
using UnityEngine;

[Serializable]
public class SFE_Pickupable : MonoBehaviour
{
	public virtual void Start()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void OnCollisionEnter(Collision collision)
	{
		if (this.onPickupGenerate)
		{
			UnityEngine.Object.Instantiate(this.onPickupGenerate, this.transform.position, this.transform.rotation);
		}
		UnityEngine.Object.Destroy(this.gameObject);
	}

	public virtual void Main()
	{
	}

	public GameObject onPickupGenerate;
}
