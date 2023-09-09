using System;
using System.Collections;
using MoPhoGames.USpeak.Interface;
using UnityEngine;

public class USpeakJitterTestSender : MonoBehaviour, ISpeechDataHandler
{
	public void USpeakOnSerializeAudio(byte[] data)
	{
		base.StartCoroutine(this.JitterSend(data));
	}

	public void USpeakInitializeSettings(int data)
	{
		USpeaker.Get(this).InitializeSettings(data);
	}

	private IEnumerator JitterSend(byte[] data)
	{
		if (UnityEngine.Random.Range(0f, 1f) <= this.ChanceOfPacketLoss)
		{
			yield break;
		}
		yield return new WaitForSeconds(UnityEngine.Random.Range(0f, (float)this.PingJitterMS / 1000f));
		USpeaker.Get(this).ReceiveAudio(data);
		yield break;
	}

	public int PingJitterMS = 100;

	public float ChanceOfPacketLoss = 0.1f;
}
