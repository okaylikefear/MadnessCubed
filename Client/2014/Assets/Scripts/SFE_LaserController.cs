using System;
using UnityEngine;

[Serializable]
public class SFE_LaserController : MonoBehaviour
{
	public SFE_LaserController()
	{
		this.damage = (float)3;
	}

	public virtual void Start()
	{
		UnityEngine.Object.Destroy(this.boxCollider, 0.08f);
		if (this.muzzleEffect)
		{
			UnityEngine.Object.Instantiate(this.muzzleEffect, this.transform.position, this.transform.rotation);
		}
		Vector3 direction = this.transform.TransformDirection(Vector3.up);
		RaycastHit raycastHit = default(RaycastHit);
		if (Physics.Raycast(this.transform.position, direction, out raycastHit))
		{
			float distance = raycastHit.distance;
			Vector3 size = this.boxCollider.size;
			float num = size.y = distance;
			Vector3 vector = this.boxCollider.size = size;
			float y = this.boxCollider.center.y + raycastHit.distance / (float)2;
			Vector3 center = this.boxCollider.center;
			float num2 = center.y = y;
			Vector3 vector2 = this.boxCollider.center = center;
			if (this.impactEffect)
			{
				UnityEngine.Object.Instantiate(this.impactEffect, raycastHit.point, raycastHit.transform.rotation);
			}
		}
		else
		{
			UnityEngine.Object.Destroy(this.boxCollider);
		}
	}

	public virtual void Update()
	{
	}

	public virtual void OnCollisionEnter(Collision collision)
	{
		UnityEngine.Object.Destroy(this.boxCollider);
	}

	public virtual void Main()
	{
	}

	public BoxCollider boxCollider;

	public float damage;

	public GameObject impactEffect;

	public GameObject muzzleEffect;
}
