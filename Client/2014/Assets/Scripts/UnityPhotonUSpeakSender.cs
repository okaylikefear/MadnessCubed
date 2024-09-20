using System;
using MoPhoGames.USpeak.Interface;
using Photon;
using UnityEngine;

public class UnityPhotonUSpeakSender : Photon.MonoBehaviour, ISpeechDataHandler
{
	private void Start()
	{
		if (!base.photonView.isMine)
		{
			USpeaker.Get(this).SpeakerMode = SpeakerMode.Remote;
		}
	}

	public void USpeakOnSerializeAudio(byte[] data)
	{
		base.photonView.RPC("vc", PhotonTargets.All, new object[]
		{
			data
		});
	}

	public void USpeakInitializeSettings(int data)
	{
		base.photonView.RPC("init", PhotonTargets.AllBuffered, new object[]
		{
			data
		});
	}

	[RPC]
	private void vc(byte[] data)
	{
		USpeaker.Get(this).ReceiveAudio(data);
	}

	[RPC]
	private void init(int data)
	{
		USpeaker.Get(this).InitializeSettings(data);
	}
}
