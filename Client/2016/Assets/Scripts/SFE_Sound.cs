using System;
using UnityEngine;

[Serializable]
public class SFE_Sound : MonoBehaviour
{
	public SFE_Sound()
	{
		this.minPitch = 0.9f;
		this.maxPitch = 1.1f;
	}

	public virtual void Start()
	{
		this.PlayClipAt(this.soundEffect, this.transform.position);
	}

	public virtual void Update()
	{
	}

	public virtual AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
	{
		GameObject gameObject = new GameObject("TempAudio");
		gameObject.transform.position = pos;
		AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
		audioSource.clip = this.soundEffect;
		audioSource.pitch = UnityEngine.Random.Range(this.minPitch, this.maxPitch);
		audioSource.Play();
		UnityEngine.Object.Destroy(gameObject, clip.length);
		return audioSource;
	}

	public virtual void Main()
	{
	}

	public AudioClip soundEffect;

	public float minPitch;

	public float maxPitch;
}
