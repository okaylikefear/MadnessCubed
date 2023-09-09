using System;
using UnityEngine;

[Serializable]
public class SFE_randoMMoveInBox1 : MonoBehaviour
{
	public SFE_randoMMoveInBox1()
	{
		this.xspeed = (float)1;
		this.yspeed = 1.5f;
		this.zspeed = (float)2;
		this.xDim = 0.3f;
		this.yDim = 0.3f;
		this.zDim = 0.3f;
	}

	public virtual void Start()
	{
		this.transform.localPosition = new Vector3((float)0, (float)0, (float)0);
		this.xspeed += (float)UnityEngine.Random.Range(-1, 1) * this.speedDeviation;
		this.yspeed += (float)UnityEngine.Random.Range(-1, 1) * this.speedDeviation;
		this.zspeed += (float)UnityEngine.Random.Range(-1, 1) * this.speedDeviation;
	}

	public virtual void Update()
	{
		this.transform.Translate(new Vector3(this.xspeed, this.yspeed, this.zspeed) * Time.deltaTime);
		if (this.transform.localPosition.x > this.xDim)
		{
			this.xspeed = -Mathf.Abs(this.xspeed);
		}
		if (this.transform.localPosition.x < -this.xDim)
		{
			this.xspeed = Mathf.Abs(this.xspeed);
		}
		if (this.transform.localPosition.y > this.yDim)
		{
			this.yspeed = -Mathf.Abs(this.yspeed);
		}
		if (this.transform.localPosition.y < -this.yDim)
		{
			this.yspeed = Mathf.Abs(this.yspeed);
		}
		if (this.transform.localPosition.z > this.zDim)
		{
			this.zspeed = -Mathf.Abs(this.zspeed);
		}
		if (this.transform.localPosition.z < -this.zDim)
		{
			this.zspeed = Mathf.Abs(this.zspeed);
		}
	}

	public virtual void Main()
	{
	}

	public float xspeed;

	public float yspeed;

	public float zspeed;

	public float speedDeviation;

	public float xDim;

	public float yDim;

	public float zDim;
}
