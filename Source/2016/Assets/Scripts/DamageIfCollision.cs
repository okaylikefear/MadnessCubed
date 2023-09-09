using System;
using UnityEngine;

public class DamageIfCollision : MonoBehaviour
{
	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.transform.root.gameObject.tag != "Player")
		{
			return;
		}
		if (Time.time - this.lastDamageTime < this.damageDeltaTime)
		{
			return;
		}
		DamageMessage damageMessage = new DamageMessage();
		damageMessage.damage = (short)(this.damagePerSec * this.damageDeltaTime);
		damageMessage.id_killer = -1;
		damageMessage.team = -1;
		damageMessage.weaponType = -1;
		other.gameObject.transform.root.SendMessage("ApplyDamage", damageMessage);
		this.lastDamageTime = Time.time;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public float damagePerSec = 50f;

	private float lastDamageTime;

	public float damageDeltaTime = 0.25f;
}
