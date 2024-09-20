using System;
using UnityEngine;

public class AltodorFilmsAnimTrack : MonoBehaviour
{
	private void Start()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("FilmManager");
		if (gameObject != null)
		{
			this.FM = gameObject.GetComponent<FilmsManager>();
			this.FM.AddSceneTrack(this);
		}
		else
		{
			MonoBehaviour.print("AltodorFilmAnimTrack: " + base.gameObject.name + " cannot find FilmsManager");
		}
	}

	public void PlayTrack(string sceneName)
	{
		if (sceneName != this.nameOfScene)
		{
			return;
		}
		if (!this.manipulateChild)
		{
			this.manipulatedAnim = base.GetComponent<Animation>();
		}
		else
		{
			this.manipulatedAnim = base.transform.gameObject.GetComponentInChildren<Animation>();
		}
		this.sceneBeginTime = Time.time;
		this.isPlayingScene = true;
		this.animTimeBegin = new float[this.animProperties.Length];
		this.isStarted = new bool[this.animations.Length];
		for (int i = 0; i < this.animProperties.Length; i++)
		{
			char[] separator = new char[]
			{
				'^'
			};
			string[] array = this.animProperties[i].Split(separator);
			this.animTimeBegin[i] = (float)Convert.ToDouble(array[0]);
			this.isStarted[i] = false;
		}
	}

	private void Update()
	{
		if (!this.isPlayingScene)
		{
			return;
		}
		if (this.manipulatedAnim == null)
		{
			return;
		}
		float num = Time.time - this.sceneBeginTime;
		for (int i = 0; i < this.animations.Length; i++)
		{
			if (this.animations[i] != null)
			{
				if (!this.isStarted[i])
				{
					if (num >= this.animTimeBegin[i])
					{
						this.manipulatedAnim.CrossFade(this.animations[i]);
						this.isStarted[i] = true;
					}
				}
			}
		}
	}

	public void StopAllTracks()
	{
		this.isPlayingScene = false;
	}

	public string nameOfScene;

	private float sceneBeginTime;

	private bool isPlayingScene;

	public bool manipulateChild;

	public string[] animations = new string[1];

	public string[] animProperties = new string[1];

	private float[] animTimeBegin;

	public bool[] isStarted;

	private FilmsManager FM;

	private Animation manipulatedAnim;
}
