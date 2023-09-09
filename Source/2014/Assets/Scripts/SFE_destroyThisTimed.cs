using System;
using UnityEngine;

[Serializable]
public class SFE_destroyThisTimed : MonoBehaviour
{
	public SFE_destroyThisTimed()
	{
		this.destroyTime = (float)5;
	}

	public virtual void Start()
	{
		UnityEngine.Object.Destroy(this.gameObject, this.destroyTime);
	}

	public virtual void Update()
	{
	}

	public virtual void Main()
	{
	}

	public float destroyTime;
}
