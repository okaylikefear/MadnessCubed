using System;
using UnityEngine;

public class AltodorFilmsCreatePrefabTrack : MonoBehaviour
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
			MonoBehaviour.print("AltodorFilmCreatePrefabTrack: " + base.gameObject.name + " cannot find FilmsManager");
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
		this.isCreated = new bool[this.prefabsToCreate.Length];
		for (int i = 0; i < this.prefabsToCreate.Length; i++)
		{
			this.isCreated[i] = false;
		}
	}

	private void Update()
	{
		if (!this.isPlayingScene)
		{
			return;
		}
		float num = Time.time - this.sceneBeginTime;
		for (int i = 0; i < this.isCreated.Length; i++)
		{
			if (!this.isCreated[i])
			{
				if (num >= this.timesOfCreation[i])
				{
					UnityEngine.Object.Instantiate(this.prefabsToCreate[i], this.placeOfCreation[i].position, this.placeOfCreation[i].rotation);
					this.isCreated[i] = true;
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

	public GameObject[] prefabsToCreate;

	public Transform[] placeOfCreation;

	public float[] timesOfCreation;

	public bool[] isCreated;

	private FilmsManager FM;
}
