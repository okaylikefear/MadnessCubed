using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class SFE_prefabGenerator : MonoBehaviour
{
	public SFE_prefabGenerator()
	{
		this.thisManyTimes = 3;
		this.overThisTime = 1f;
		this.yRotMax = (float)180;
		this.detachToWorld = true;
	}

	public virtual void Start()
	{
		if (this.thisManyTimes < 1)
		{
			this.thisManyTimes = 1;
		}
		this.trigger = this.overThisTime / (float)this.thisManyTimes;
	}

	public virtual void Update()
	{
		this.timeCounter += Time.deltaTime;
		if (this.generateOneInstantly)
		{
			this.timeCounter = this.trigger;
			this.generateOneInstantly = false;
		}
		if (this.timeCounter >= this.trigger && this.effectCounter < this.thisManyTimes)
		{
			this.rndNr = Mathf.Floor(UnityEngine.Random.value * (float)Extensions.get_length(this.createThis));
			this.x_cur = this.transform.position.x + UnityEngine.Random.value * this.xWidth - this.xWidth * 0.5f;
			this.y_cur = this.transform.position.y + UnityEngine.Random.value * this.yWidth - this.yWidth * 0.5f;
			this.z_cur = this.transform.position.z + UnityEngine.Random.value * this.zWidth - this.zWidth * 0.5f;
			if (!this.allUseSameRotation || !this.allRotationDecided)
			{
				this.xRotCur = this.transform.rotation.x + UnityEngine.Random.value * this.xRotMax * (float)2 - this.xRotMax;
				this.yRotCur = this.transform.rotation.y + UnityEngine.Random.value * this.yRotMax * (float)2 - this.yRotMax;
				this.zRotCur = this.transform.rotation.z + UnityEngine.Random.value * this.zRotMax * (float)2 - this.zRotMax;
				this.allRotationDecided = true;
			}
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.createThis[(int)this.rndNr], new Vector3(this.x_cur, this.y_cur, this.z_cur), this.transform.rotation);
			gameObject.transform.Rotate(this.xRotCur, this.yRotCur, this.zRotCur);
			if (!this.detachToWorld)
			{
				gameObject.transform.parent = this.transform;
			}
			this.timeCounter -= this.trigger;
			this.effectCounter++;
		}
	}

	public virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color((float)1, (float)1, (float)1, 0.2f);
		Gizmos.DrawCube(this.transform.position, new Vector3(this.xWidth, this.yWidth, this.zWidth));
		if (this.xWidth == (float)0 && this.yWidth == (float)0 && this.zWidth == (float)0)
		{
			Gizmos.DrawSphere(this.transform.position, 0.5f);
		}
	}

	public virtual void Main()
	{
	}

	public GameObject[] createThis;

	private float rndNr;

	public int thisManyTimes;

	public float overThisTime;

	public bool generateOneInstantly;

	public float xWidth;

	public float yWidth;

	public float zWidth;

	public float xRotMax;

	public float yRotMax;

	public float zRotMax;

	public bool allUseSameRotation;

	private bool allRotationDecided;

	public bool detachToWorld;

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
