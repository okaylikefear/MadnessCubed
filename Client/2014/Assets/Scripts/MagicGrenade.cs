using System;
using UnityEngine;

public class MagicGrenade : MonoBehaviour
{
	private void SetParametersPos(Vector3 _pos)
	{
		this.pos = _pos;
	}

	private void SetParametersPoint(Vector3 _point)
	{
		this.shotPoint = _point;
	}

	private void SetParameters(int _playerId)
	{
		this.playerId = _playerId;
	}

	public void Use(PlayerScript player)
	{
		Ray camRay = player.getCamRay();
		int num = 20;
		Vector3 origin = camRay.origin;
		Vector3 direction = camRay.direction;
		GameObject gameObject = PhotonNetwork.Instantiate(this.resourceName, origin, Quaternion.LookRotation(direction), 0);
		NetGrenadeScript component = gameObject.GetComponent<NetGrenadeScript>();
		GameObject gameObject2 = player.gameObject;
		component.Throw(direction * (float)num * gameObject.rigidbody.mass);
		gameObject.SendMessage("SetDamageParam", new DamageMessage
		{
			id_killer = player.id,
			team = player.team,
			damage = (short)this.damage
		});
	}

	private void Update()
	{
	}

	private int playerId;

	public string resourceName;

	private Vector3 pos;

	private Vector3 shotPoint;

	public float damage = 260f;
}
