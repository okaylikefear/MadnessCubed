using System;
using UnityEngine;

public class GrenadeScript : RocketScript
{
	protected new void Start()
	{
		base.Start();
		base.Invoke("Explode", this.explosionDelay);
	}

	private void OnCollisionEnter(Collision col)
	{
		base.gameObject.layer = 0;
		bool flag = false;
		if (col.collider.gameObject.layer != 8)
		{
			flag = true;
		}
		for (int i = 0; i < col.contacts.Length; i++)
		{
			ContactPoint contactPoint = col.contacts[i];
			if (contactPoint.otherCollider.gameObject.layer != 8)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		this.Explode();
	}

	private void Explode()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.explosionRadius);
		for (int i = 0; i < array.Length; i++)
		{
			if (this.dm.damage != 0)
			{
				if (array[i].gameObject.layer != 8)
				{
					float num = 1f - Vector3.Distance(base.transform.position, array[i].ClosestPointOnBounds(base.transform.position)) / this.explosionRadius;
					DamageMessage damageMessage = new DamageMessage();
					damageMessage.damage = (short)((float)this.dm.damage * num);
					damageMessage.id_killer = this.dm.id_killer;
					damageMessage.weaponType = this.dm.weaponType;
					damageMessage.team = this.dm.team;
					array[i].gameObject.transform.root.SendMessage("ApplyDamage", damageMessage, SendMessageOptions.DontRequireReceiver);
					if (this.canFreeze)
					{
						FreezeStruct freezeStruct;
						freezeStruct.freezeTime = 7f;
						freezeStruct.team = this.dm.team;
						array[i].gameObject.SendMessage("Freeze", freezeStruct, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}
		array = Physics.OverlapSphere(base.transform.position, this.explosionRadius);
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].gameObject.rigidbody != null)
			{
				array[j].gameObject.rigidbody.AddForceAtPosition(0.01f * (float)this.dm.damage * (array[j].transform.position - base.transform.position).normalized, base.transform.position, ForceMode.Impulse);
			}
		}
		if (this.explosion != null)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.explosion, base.transform.position, base.transform.rotation);
			gameObject.SendMessage("SetDamageParam", this.dm, SendMessageOptions.DontRequireReceiver);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public float explosionDelay = 10f;

	public bool canFreeze;
}
