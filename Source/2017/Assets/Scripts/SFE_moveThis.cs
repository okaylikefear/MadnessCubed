using System;
using UnityEngine;

[Serializable]
public class SFE_moveThis : MonoBehaviour
{
	public SFE_moveThis()
	{
		this.translationSpeedY = (float)1;
		this.local = true;
	}

	public virtual void Update()
	{
		if (this.local)
		{
			this.transform.Translate(new Vector3(this.translationSpeedX, this.translationSpeedY, this.translationSpeedZ) * Time.deltaTime);
		}
		if (!this.local)
		{
			this.transform.Translate(new Vector3(this.translationSpeedX, this.translationSpeedY, this.translationSpeedZ) * Time.deltaTime, Space.World);
		}
	}

	public virtual void Main()
	{
	}

	public float translationSpeedX;

	public float translationSpeedY;

	public float translationSpeedZ;

	public bool local;
}
