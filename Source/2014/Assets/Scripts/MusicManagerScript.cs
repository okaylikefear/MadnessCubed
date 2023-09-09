using System;
using kube;
using UnityEngine;

public class MusicManagerScript : MonoBehaviour
{
	private void ChangeMusic(int newMusic)
	{
		this.changed = false;
		this.timeToChange = Time.time + this.halfChangingTime;
		this.musicToChange = newMusic;
		this.StartDownload(newMusic);
	}

	public void Mute(bool muteOn)
	{
		if (muteOn)
		{
			GetComponent<AudioSource>().volume = 0.05f;
		}
		else
		{
			GetComponent<AudioSource>().volume = this.maxVolume;
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
		if (Application.isEditor)
		{
			this.maxVolume = 0f;
		}
	}

	private void StartDownload(int clip)
	{
		string assetPath = Kube.SS.assetPath;
		if (clip >= this.musicClips.Length)
		{
			return;
		}
		if (this._musicClips[clip] != null)
		{
			return;
		}
		WWW www = new WWW(assetPath + this.musicClips[clip]);
		this._musicClips[clip] = www.GetAudioClip(false, true);
		GetComponent<AudioSource>().loop = true;
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
		if (!GetComponent<AudioSource>().isPlaying && GetComponent<AudioSource>().clip != null)
		{
			GetComponent<AudioSource>().Play();
		}
		if (!this.changed)
		{
			if (this._musicClips[this.musicToChange] == null)
			{
				return;
			}
			AudioClip audioClip = this._musicClips[this.musicToChange];
			if (!audioClip.isReadyToPlay)
			{
				return;
			}
			float num = this.timeToChange - Time.time;
			if (num < 0f)
			{
				GetComponent<AudioSource>().clip = this._musicClips[this.musicToChange];
				GetComponent<AudioSource>().Play();
				this.changed = true;
			}
			else
			{
				GetComponent<AudioSource>().volume = num / this.halfChangingTime * this.maxVolume;
			}
		}
		else if (Time.time - this.timeToChange < this.halfChangingTime + 1f)
		{
			float num2 = Time.time - this.timeToChange;
			GetComponent<AudioSource>().volume = Mathf.Min(this.maxVolume, num2 / this.halfChangingTime * this.maxVolume);
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
