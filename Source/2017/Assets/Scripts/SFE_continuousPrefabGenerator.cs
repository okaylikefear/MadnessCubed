using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class SFE_continuousPrefabGenerator : MonoBehaviour
{
	public SFE_continuousPrefabGenerator()
	{
		this.thisManyTimesPerSec = (float)1;
		this.yRotMax = (float)180;
	}

	public virtual void Start()
	{
		if (this.thisManyTimesPerSec < 0.1f)
		{
			this.thisManyTimesPerSec = 0.1f;
		}
		this.trigger = (float)1 / this.thisManyTimesPerSec;
	}

	public virtual void Update()
	{
		this.timeCounter += Time.deltaTime;
		if (this.timeCounter > this.trigger)
		{
			this.rndNr = Mathf.Floor(UnityEngine.Random.value * (float)Extensions.get_length(this.createThis));
			this.x_cur = this.transform.position.x + UnityEngine.Random.value * this.xWidth - this.xWidth * 0.5f;
			this.y_cur = this.transform.position.y + UnityEngine.Random.value * this.yWidth - this.yWidth * 0.5f;
			this.z_cur = this.transform.position.z + UnityEngine.Random.value * this.zWidth - this.zWidth * 0.5f;
			this.xRotCur = this.transform.rotation.x + UnityEngine.Random.value * this.xRotMax * (float)2 - this.xRotMax;
			this.yRotCur = this.transform.rotation.y + UnityEngine.Random.value * this.yRotMax * (float)2 - this.yRotMax;
			this.zRotCur = this.transform.rotation.z + UnityEngine.Random.value * this.zRotMax * (float)2 - this.zRotMax;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.createThis[(int)this.rndNr], new Vector3(this.x_cur, this.y_cur, this.z_cur), this.transform.rotation);
			gameObject.transform.Rotate(this.xRotCur, this.yRotCur, this.zRotCur);
			this.timeCounter -= this.trigger;
		}
	}

	public virtual void Main()
	{
	}

	public GameObject[] createThis;

	private float rndNr;

	public float thisManyTimesPerSec;

	public float xWidth;

	public float yWidth;

	public float zWidth;

	public float xRotMax;

	public float yRotMax;

	public float zRotMax;

	private float x_cur;

	private float y_cur;

	private float z_cur;

	private float xRotCur;

	private float yRotCur;

	private float zRotCur;

	private float timeCounter;

	private int effectCounter;

	private float trigger;
}
