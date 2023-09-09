using System;
using UnityEngine;

public class MagicFreezeScript : MonoBehaviour
{
	private void Start()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.explosionRadius);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerScript component = array[i].gameObject.GetComponent<PlayerScript>();
			if (!(component != null) || component.id != this._killer)
			{
				FreezeStruct freezeStruct;
				freezeStruct.freezeTime = this.freezeTime;
				freezeStruct.team = this._team;
				array[i].gameObject.SendMessage("Freeze", freezeStruct, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void Update()
	{
	}

	private void SetDamageParam(DamageMessage _dm)
	{
		this._killer = _dm.id_killer;
		this._team = _dm.team;
	}

	public float explosionRadius = 2f;

	public float freezeTime = 3f;

	protected int _killer;

	protected int _team;
}
