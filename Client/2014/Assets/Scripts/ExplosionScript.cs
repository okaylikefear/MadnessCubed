using System;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("DoExplosion", 0.05f);
	}

	private void DoExplosion()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.explosionRadius, 65535);
		for (int i = 0; i < array.Length; i++)
		{
			float d = Mathf.Max(1f - Vector3.Distance(base.transform.position, array[i].transform.position) / this.explosionRadius, 0f);
			if (array[i].rigidbody != null)
			{
				array[i].rigidbody.AddForce((array[i].transform.position - base.transform.position).normalized * this.force * d, ForceMode.Impulse);
			}
			array[i].gameObject.SendMessage("PushChar", (array[i].transform.position - base.transform.position).normalized * this.force * d, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void Update()
	{
		if (this.light)
		{
			this.light.intensity -= 2.5f * Time.deltaTime;
		}
	}

	public float explosionRadius = 10f;

	public float force = 100f;

	public new Light light;
}
