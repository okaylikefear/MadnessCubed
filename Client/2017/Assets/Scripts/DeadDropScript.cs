using System;
using Photon;
using UnityEngine;

public class DeadDropScript : Photon.MonoBehaviour
{
	private void Start()
	{
		if (base.photonView.isMine)
		{
			base.Invoke("Remove", 300f);
		}
	}

	private void Remove()
	{
		PhotonNetwork.Destroy(base.gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		PlayerScript componentInChildren = other.GetComponentInChildren<PlayerScript>();
		if (componentInChildren == null || componentInChildren.dead)
		{
			return;
		}
		componentInChildren.GiveLotOfDrop(this.weapons);
		PhotonNetwork.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (!this.viewModel)
		{
			return;
		}
		this.viewModel.transform.localPosition = new Vector3(0f, this.height + this.flyingHeightAmp * Mathf.Sin(Time.time * 2f * 3.14159274f / this.flyingHeightPeriod), 0f);
		this.viewModel.transform.RotateAround(Vector3.up, this.flyingRotationSpeed * Time.deltaTime);
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
		}
		else
		{
			base.transform.position = (Vector3)stream.ReceiveNext();
		}
	}

	public float flyingHeightAmp = 0.2f;

	public float flyingHeightPeriod = 2f;

	public float flyingRotationSpeed = 1.5f;

	public float height = 0.5f;

	public FastInventar[] weapons;

	public GameObject viewModel;
}
