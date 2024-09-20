using System;
using UnityEngine;

public class AltodorFilmsCommandTrack : MonoBehaviour
{
	private void Awake()
	{
		this.commandTimes = new float[this.commands.Length];
		this.commandVars = new string[this.commands.Length];
		this.commandTags = new string[this.commands.Length];
		this.commandFunc = new string[this.commands.Length];
		this.isCommandDone = new bool[this.commands.Length];
		for (int i = 0; i < this.commands.Length; i++)
		{
			char[] separator = new char[]
			{
				'^'
			};
			string[] array = this.commands[i].Split(separator);
			this.commandTimes[i] = (float)Convert.ToDouble(array[0]);
			this.commandTags[i] = array[1];
			this.commandFunc[i] = array[2];
			this.commandVars[i] = array[3];
			this.isCommandDone[i] = false;
		}
	}

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
		this.isPlayingScene = true;
		this.sceneBeginTime = Time.time;
	}

	public void StopAllTracks()
	{
		this.isPlayingScene = false;
	}

	private void Update()
	{
		if (!this.isPlayingScene)
		{
			return;
		}
		this.localTime = Time.time - this.sceneBeginTime;
		for (int i = 0; i < this.commandTimes.Length; i++)
		{
			if (!this.isCommandDone[i])
			{
				if (this.commandTimes[i] <= this.localTime)
				{
					GameObject gameObject;
					if (this.toThisGO)
					{
						gameObject = base.transform.gameObject;
					}
					else
					{
						gameObject = GameObject.FindGameObjectWithTag(this.commandTags[i]);
					}
					if (gameObject != null)
					{
						gameObject.SendMessage(this.commandFunc[i], this.commandVars[i], SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						MonoBehaviour.print("CommandsTrack: no go found!");
					}
					this.isCommandDone[i] = true;
				}
			}
		}
	}

	public string nameOfScene;

	private bool isPlayingScene;

	private float sceneBeginTime;

	private float localTime;

	public string[] commands = new string[1];

	public bool toThisGO;

	private float[] commandTimes;

	private string[] commandTags;

	private string[] commandVars;

	private bool[] isCommandDone;

	private string[] commandFunc;

	private FilmsManager FM;
}
