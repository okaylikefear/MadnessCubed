using System;
using UnityEngine;

public class NetFlashScript : NetGrenadeScript
{
	private void Start()
	{
		this.correctPlayerPos = base.transform.position;
		this.correctPlayerRot = base.transform.rotation;
		base.Invoke("Detonate", 3f);
	}

	private void OnCollisionEnter(Collision col)
	{
	}

	private void Detonate()
	{
		if (this.explosion != null)
		{
			UnityEngine.Object.Instantiate(this.explosion, base.transform.position, base.transform.rotation);
		}
		if (!base.photonView.isMine)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.explosionRadius);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.transform.root.SendMessage("ApplyFlash", base.transform.position, SendMessageOptions.DontRequireReceiver);
		}
		if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}
}
