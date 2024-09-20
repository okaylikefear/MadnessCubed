using System;
using MoPhoGames.USpeak.Interface;
using UnityEngine;

public class LocalUSpeakSender : MonoBehaviour, ISpeechDataHandler
{
	public void USpeakOnSerializeAudio(byte[] data)
	{
		USpeaker.Get(this).ReceiveAudio(data);
	}

	public void USpeakInitializeSettings(int data)
	{
		USpeaker.Get(this).InitializeSettings(data);
	}
}
