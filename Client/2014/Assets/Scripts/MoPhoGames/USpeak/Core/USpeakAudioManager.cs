using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoPhoGames.USpeak.Core
{
	public class USpeakAudioManager
	{
		public static void PlayClipAtPoint(AudioClip clip, Vector3 position, ulong delay, bool calcPan = false)
		{
			AudioSource audioSource = USpeakAudioManager.GetAudioSource();
			audioSource.transform.position = position;
			audioSource.clip = clip;
			if (calcPan)
			{
				audioSource.pan = -Vector3.Dot(Vector3.Cross(Camera.main.transform.forward, Vector3.up).normalized, (position - Camera.main.transform.position).normalized);
			}
			audioSource.Play(delay);
		}

		public static void StopAll()
		{
			foreach (AudioSource audioSource in USpeakAudioManager.audioPool)
			{
				audioSource.Stop();
			}
		}

		private static AudioSource GetAudioSource()
		{
			AudioSource audioSource = USpeakAudioManager.FindInactiveAudioFromPool();
			if (audioSource == null)
			{
				audioSource = new GameObject
				{
					hideFlags = HideFlags.HideInHierarchy
				}.AddComponent<AudioSource>();
				USpeakAudioManager.audioPool.Add(audioSource);
			}
			return audioSource;
		}

		private static AudioSource FindInactiveAudioFromPool()
		{
			USpeakAudioManager.Cleanup();
			foreach (AudioSource audioSource in USpeakAudioManager.audioPool)
			{
				if (!audioSource.isPlaying)
				{
					return audioSource;
				}
			}
			return null;
		}

		private static void Cleanup()
		{
			for (int i = 0; i < USpeakAudioManager.audioPool.Count; i++)
			{
				if (USpeakAudioManager.audioPool[i] == null)
				{
					USpeakAudioManager.audioPool.RemoveAt(i);
				}
			}
		}

		private static List<AudioSource> audioPool = new List<AudioSource>();
	}
}
