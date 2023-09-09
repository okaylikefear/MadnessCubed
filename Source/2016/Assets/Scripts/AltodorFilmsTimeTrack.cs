using System;
using UnityEngine;

public class AltodorFilmsTimeTrack : MonoBehaviour
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
			MonoBehaviour.print("AltodorFilmTimeTrack: " + base.gameObject.name + " cannot find FilmsManager");
		}
	}

	public void PlayTrack(string sceneName)
	{
		if (sceneName != this.nameOfScene)
		{
			return;
		}
		this.sceneBeginTime = Time.time;
		this.isPlayingScene = true;
	}

	private void Update()
	{
		if (!this.isPlayingScene)
		{
			return;
		}
		float num = Time.time - this.sceneBeginTime;
		for (int i = 0; i < this.timeScales.Length; i++)
		{
			if (i < this.timeScales.Length - 1 && num >= this.timeOfTimeScales[i] && num < this.timeOfTimeScales[i + 1])
			{
				Time.timeScale = Mathf.Lerp(this.timeScales[i], this.timeScales[i + 1], (num - this.timeOfTimeScales[i]) / (this.timeOfTimeScales[i + 1] - this.timeOfTimeScales[i]));
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

	public float[] timeScales = new float[2];

	public float[] timeOfTimeScales = new float[2];

	private FilmsManager FM;
}
