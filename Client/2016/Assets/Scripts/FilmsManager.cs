using System;
using System.Collections;
using UnityEngine;

public class FilmsManager : MonoBehaviour
{
	public void AddSceneTrack(AltodorFilmsAnimTrack afat)
	{
		this.AltodorFilmsAnimTrackAL.Add(afat);
	}

	public void AddSceneTrack(AltodorFilmsMoveTrack afmt)
	{
		this.AltodorFilmsMoveTrackAL.Add(afmt);
	}

	public void AddSceneTrack(AltodorFilmsCommandTrack afct)
	{
		this.AltodorFilmsCommandTrackAL.Add(afct);
	}

	public void AddSceneTrack(AltodorFilmsCreatePrefabTrack afcpt)
	{
		this.AltodorFilmsCreatePrefabTrackAL.Add(afcpt);
	}

	public void AddSceneTrack(AltodorFilmsTimeTrack aftt)
	{
		this.AltodorFilmsTimeTrackAL.Add(aftt);
	}

	public void PlayScene(string sceneName)
	{
		foreach (object obj in this.AltodorFilmsAnimTrackAL)
		{
			AltodorFilmsAnimTrack altodorFilmsAnimTrack = (AltodorFilmsAnimTrack)obj;
			if (altodorFilmsAnimTrack != null)
			{
				altodorFilmsAnimTrack.StopAllTracks();
			}
		}
		foreach (object obj2 in this.AltodorFilmsMoveTrackAL)
		{
			AltodorFilmsMoveTrack altodorFilmsMoveTrack = (AltodorFilmsMoveTrack)obj2;
			if (altodorFilmsMoveTrack != null)
			{
				altodorFilmsMoveTrack.StopAllTracks();
			}
		}
		foreach (object obj3 in this.AltodorFilmsCommandTrackAL)
		{
			AltodorFilmsCommandTrack altodorFilmsCommandTrack = (AltodorFilmsCommandTrack)obj3;
			if (altodorFilmsCommandTrack != null)
			{
				altodorFilmsCommandTrack.StopAllTracks();
			}
		}
		foreach (object obj4 in this.AltodorFilmsAnimTrackAL)
		{
			AltodorFilmsAnimTrack altodorFilmsAnimTrack2 = (AltodorFilmsAnimTrack)obj4;
			if (altodorFilmsAnimTrack2 != null)
			{
				altodorFilmsAnimTrack2.PlayTrack(sceneName);
			}
		}
		foreach (object obj5 in this.AltodorFilmsMoveTrackAL)
		{
			AltodorFilmsMoveTrack altodorFilmsMoveTrack2 = (AltodorFilmsMoveTrack)obj5;
			if (altodorFilmsMoveTrack2 != null)
			{
				altodorFilmsMoveTrack2.PlayTrack(sceneName);
			}
		}
		foreach (object obj6 in this.AltodorFilmsCommandTrackAL)
		{
			AltodorFilmsCommandTrack altodorFilmsCommandTrack2 = (AltodorFilmsCommandTrack)obj6;
			if (altodorFilmsCommandTrack2 != null)
			{
				altodorFilmsCommandTrack2.PlayTrack(sceneName);
			}
		}
		foreach (object obj7 in this.AltodorFilmsCreatePrefabTrackAL)
		{
			AltodorFilmsCreatePrefabTrack altodorFilmsCreatePrefabTrack = (AltodorFilmsCreatePrefabTrack)obj7;
			if (altodorFilmsCreatePrefabTrack != null)
			{
				altodorFilmsCreatePrefabTrack.PlayTrack(sceneName);
			}
		}
		foreach (object obj8 in this.AltodorFilmsTimeTrackAL)
		{
			AltodorFilmsTimeTrack altodorFilmsTimeTrack = (AltodorFilmsTimeTrack)obj8;
			if (altodorFilmsTimeTrack != null)
			{
				altodorFilmsTimeTrack.PlayTrack(sceneName);
			}
		}
		this.isPlayingScene = true;
		this.currentSceneBeginTime = Time.time;
		this.currentSceneName = sceneName;
		MonoBehaviour.print("Play scene: " + sceneName);
	}

	public void StopAllScenes()
	{
		foreach (object obj in this.AltodorFilmsAnimTrackAL)
		{
			AltodorFilmsAnimTrack altodorFilmsAnimTrack = (AltodorFilmsAnimTrack)obj;
			if (altodorFilmsAnimTrack != null)
			{
				altodorFilmsAnimTrack.StopAllTracks();
			}
		}
		foreach (object obj2 in this.AltodorFilmsMoveTrackAL)
		{
			AltodorFilmsMoveTrack altodorFilmsMoveTrack = (AltodorFilmsMoveTrack)obj2;
			if (altodorFilmsMoveTrack != null)
			{
				altodorFilmsMoveTrack.StopAllTracks();
			}
		}
		foreach (object obj3 in this.AltodorFilmsCommandTrackAL)
		{
			AltodorFilmsCommandTrack altodorFilmsCommandTrack = (AltodorFilmsCommandTrack)obj3;
			if (altodorFilmsCommandTrack != null)
			{
				altodorFilmsCommandTrack.StopAllTracks();
			}
		}
		foreach (object obj4 in this.AltodorFilmsCreatePrefabTrackAL)
		{
			AltodorFilmsCreatePrefabTrack altodorFilmsCreatePrefabTrack = (AltodorFilmsCreatePrefabTrack)obj4;
			if (altodorFilmsCreatePrefabTrack != null)
			{
				altodorFilmsCreatePrefabTrack.StopAllTracks();
			}
		}
		foreach (object obj5 in this.AltodorFilmsTimeTrackAL)
		{
			AltodorFilmsTimeTrack altodorFilmsTimeTrack = (AltodorFilmsTimeTrack)obj5;
			if (altodorFilmsTimeTrack != null)
			{
				altodorFilmsTimeTrack.StopAllTracks();
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!this.isPlayingScene)
		{
			return;
		}
		this.currentSceneTime = Time.time - this.currentSceneBeginTime;
	}

	private ArrayList AltodorFilmsAnimTrackAL = new ArrayList();

	private ArrayList AltodorFilmsMoveTrackAL = new ArrayList();

	private ArrayList AltodorFilmsCommandTrackAL = new ArrayList();

	private ArrayList AltodorFilmsCreatePrefabTrackAL = new ArrayList();

	private ArrayList AltodorFilmsTimeTrackAL = new ArrayList();

	public string currentSceneName = "no scene";

	public float currentSceneTime;

	private float currentSceneBeginTime;

	private bool isPlayingScene;
}
