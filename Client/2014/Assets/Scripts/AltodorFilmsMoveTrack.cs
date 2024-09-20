using System;
using UnityEngine;

public class AltodorFilmsMoveTrack : MonoBehaviour
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
			MonoBehaviour.print("AltodorFilmMoveTrack: " + base.gameObject.name + " cannot find FilmsManager");
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
		int num;
		if (this.loop)
		{
			num = this.wayPoints.Length + 1;
		}
		else
		{
			num = this.wayPoints.Length;
		}
		this.moveSpeed = new float[num];
		this.wayPointTime = new float[num];
		this.wayPointsTrack = new Transform[num];
		this.maxAnimTime = 0f;
		for (int i = 0; i < this.wayPoints.Length; i++)
		{
			char[] separator = new char[]
			{
				'^'
			};
			string[] array = this.moveProperties[i].Split(separator);
			this.moveSpeed[i] = (float)Convert.ToDouble(array[0]);
			this.wayPointsTrack[i] = this.wayPoints[i];
			if (i == 0)
			{
				this.wayPointTime[i] = 0f;
			}
			else
			{
				this.wayPointTime[i] = this.wayPointTime[i - 1] + Vector3.Distance(this.wayPoints[i].position, this.wayPoints[i - 1].position) / this.moveSpeed[i - 1];
				this.maxAnimTime = this.wayPointTime[i] + 0.3f;
			}
		}
		if (this.loop)
		{
			this.wayPointsTrack[num - 1] = this.wayPoints[0];
			this.wayPointTime[num - 1] = this.wayPointTime[num - 2] + Vector3.Distance(this.wayPoints[num - 2].position, this.wayPoints[0].position) / this.moveSpeed[num - 2];
		}
	}

	private void Update()
	{
		if (!this.isPlayingScene)
		{
			return;
		}
		float num = Time.time - this.sceneBeginTime - this.moveBeginTime;
		if (num < 0f)
		{
			return;
		}
		if (!this.loop && num > this.maxAnimTime)
		{
			return;
		}
		if (num < 0.5f)
		{
			base.transform.rotation = this.wayPointsTrack[0].rotation;
		}
		Vector3 to = base.transform.position;
		Quaternion to2 = Quaternion.identity;
		int i;
		for (i = 0; i < this.wayPointsTrack.Length - 1; i++)
		{
			if (num >= this.wayPointTime[i] && num <= this.wayPointTime[i + 1])
			{
				float t = (num - this.wayPointTime[i]) / (this.wayPointTime[i + 1] - this.wayPointTime[i]);
				to = Vector3.Lerp(this.wayPointsTrack[i].position, this.wayPointsTrack[i + 1].position, t);
				if (this.interpolateRotation)
				{
					to2 = Quaternion.Slerp(this.wayPointsTrack[i].rotation, this.wayPointsTrack[i + 1].rotation, t);
				}
				else
				{
					to2 = this.wayPointsTrack[i].rotation;
				}
				base.transform.position = Vector3.Lerp(base.transform.position, to, this.moveLerpKoeffPos);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, to2, this.moveLerpKoeffRot);
				break;
			}
		}
		if (i == this.wayPointsTrack.Length - 1)
		{
			if (this.loop)
			{
				this.sceneBeginTime = Time.time - this.moveBeginTime;
			}
			else
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.wayPointsTrack[i].position, this.moveLerpKoeffPos);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.wayPointsTrack[i].rotation, this.moveLerpKoeffRot);
			}
		}
	}

	public void StopAllTracks()
	{
		this.isPlayingScene = false;
	}

	private void OnDrawGizmos()
	{
		for (int i = 0; i < this.wayPoints.Length - 1; i++)
		{
			Gizmos.DrawLine(this.wayPoints[i].position, this.wayPoints[i + 1].position);
		}
	}

	public string nameOfScene;

	private float sceneBeginTime;

	public float moveBeginTime;

	private bool isPlayingScene;

	public bool manipulateChild;

	public bool interpolateRotation;

	public bool loop;

	public Transform[] wayPoints = new Transform[1];

	public string[] moveProperties = new string[1];

	private Transform[] wayPointsTrack;

	public float moveLerpKoeffPos = 0.5f;

	public float moveLerpKoeffRot = 0.5f;

	private float[] moveSpeed;

	private float[] wayPointTime;

	public Vector3 lookAtPoint = Vector3.zero;

	private float maxAnimTime;

	private FilmsManager FM;
}
