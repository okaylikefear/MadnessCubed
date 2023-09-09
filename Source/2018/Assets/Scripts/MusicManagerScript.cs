using System;
using System.Collections;
using kube;
using UnityEngine;

public class MusicManagerScript : MonoBehaviour
{
	private void ChangeMusic(int clip)
	{
		base.StartCoroutine(this._ChangeMusic(clip));
	}

	private IEnumerator _ChangeMusic(int clip)
	{
		if (clip >= this.musicClips.Length)
		{
			yield break;
		}
		if (this._musicClips[clip] == null)
		{
			WWW req = Kube.RM.WWWLoad(this.musicClips[clip]);
			yield return req;
			this._musicClips[clip] = req.GetAudioClip(false, true);
			base.GetComponent<AudioSource>().loop = true;
		}
		this.changed = false;
		this.musicToChange = clip;
		this.timeToChange = Time.time + this.halfChangingTime;
		yield break;
	}

	public void Mute(bool muteOn)
	{
		if (muteOn)
		{
			base.GetComponent<AudioSource>().volume = 0.05f;
		}
		else
		{
			base.GetComponent<AudioSource>().volume = this.maxVolume;
		}
		this.isMute = muteOn;
	}

	private void Awake()
	{
		this._musicClips = new AudioClip[this.musicClips.Length];
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (this.isMute)
		{
			return;
		}
		if (this._musicClips.Length <= 0)
		{
			return;
		}
		if (!this.changed)
		{
			float num = this.timeToChange - Time.time;
			if (num > 0f)
			{
				base.GetComponent<AudioSource>().volume = num / this.halfChangingTime * this.maxVolume;
			}
			if (this._musicClips[this.musicToChange] == null)
			{
				return;
			}
			AudioClip audioClip = this._musicClips[this.musicToChange];
			if (audioClip.length <= 0f)
			{
				return;
			}
			if (!audioClip.isReadyToPlay)
			{
				return;
			}
			if (num < 0f)
			{
				base.GetComponent<AudioSource>().clip = this._musicClips[this.musicToChange];
				base.GetComponent<AudioSource>().Play();
				this.changed = true;
			}
		}
		else if (Time.time - this.timeToChange < this.halfChangingTime + 1f)
		{
			float num2 = Time.time - this.timeToChange;
			base.GetComponent<AudioSource>().volume = Mathf.Min(this.maxVolume, num2 / this.halfChangingTime * this.maxVolume);
		}
	}

	public string[] musicClips;

	private AudioClip[] _musicClips;

	private float halfChangingTime = 2f;

	private bool changed = true;

	private float timeToChange;

	private int musicToChange;

	public float maxVolume = 0.5f;

	public bool isMute;
}
