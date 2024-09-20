using System;
using UnityEngine;

public class JetPackScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void PlayStop(bool _play)
	{
		if (_play && !this.playing)
		{
			this.playing = true;
			GetComponent<AudioSource>().Play();
			ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Play();
			}
		}
		if (!_play && this.playing)
		{
			this.playing = false;
			GetComponent<AudioSource>().Stop();
			ParticleSystem[] componentsInChildren2 = base.GetComponentsInChildren<ParticleSystem>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].Stop();
			}
		}
	}

	private bool playing;
}
