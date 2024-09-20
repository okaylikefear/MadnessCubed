using System;
using UnityEngine;

public class RocketPTURS : MonoBehaviour
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
		if (this.dm.damage != 0)
		{
			GameObject gameObject = PhotonNetwork.Instantiate(this.netRocketName, base.transform.position, base.transform.rotation, 0);
			gameObject.SendMessage("SetDamageParam", this.dm);
		}
	}

	private void Update()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public string netRocketName;

	public DamageMessage dm;
}
