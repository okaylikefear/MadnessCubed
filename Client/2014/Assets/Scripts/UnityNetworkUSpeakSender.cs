using System;
using MoPhoGames.USpeak.Interface;
using UnityEngine;

public class UnityNetworkUSpeakSender : MonoBehaviour, ISpeechDataHandler
{
	private void Start()
	{
		if (!base.networkView.isMine)
		{
			USpeaker.Get(this).SpeakerMode = SpeakerMode.Remote;
		}
	}

	public void USpeakOnSerializeAudio(byte[] data)
	{
		base.networkView.RPC("vc", RPCMode.All, new object[]
		{
			data
		});
	}

	public void USpeakInitializeSettings(int data)
	{
		base.networkView.RPC("init", RPCMode.AllBuffered, new object[]
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
