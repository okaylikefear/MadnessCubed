using System;
using UnityEngine;

public class BoltScript : MonoBehaviour
{
	private void SetDamageParam(DamageMessage _dm)
	{
		this.dm = new DamageMessage();
		this.dm.damage = _dm.damage;
		this.dm.id_killer = _dm.id_killer;
		this.dm.weaponType = _dm.weaponType;
		this.dm.team = _dm.team;
	}

	private void Start()
	{
		GetComponent<Rigidbody>().AddForce(base.transform.TransformDirection(Vector3.forward) * this.speed, ForceMode.Impulse);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (this.dm.damage != 0)
		{
			collision.gameObject.transform.root.SendMessage("ApplyDamage", this.dm, SendMessageOptions.DontRequireReceiver);
		}
		UnityEngine.Object.Destroy(base.gameObject.rigidbody);
		UnityEngine.Object.Destroy(base.gameObject.collider);
		base.transform.parent = collision.gameObject.transform;
		base.Invoke("FeedBackPhys", 0.05f);
	}

	private void FeedBackPhys()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.25f, 65535);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].rigidbody != null)
			{
				array[i].rigidbody.AddForce(this.lastVelocity.normalized * 3f, ForceMode.Impulse);
			}
		}
	}

	private void Update()
	{
		if (GetComponent<Rigidbody>() != null)
		{
			this.lastVelocity = GetComponent<Rigidbody>().velocity;
		}
	}

	public float speed = 20f;

	public DamageMessage dm;

	private NetworkObjectScript NO;

	private Vector3 lastVelocity;
}
