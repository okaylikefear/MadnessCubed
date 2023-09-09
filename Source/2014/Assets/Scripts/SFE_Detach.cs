using System;
using UnityEngine;

[Serializable]
public class SFE_Detach : MonoBehaviour
{
	public virtual void Start()
	{
		this.transform.parent = null;
	}

	public virtual void Update()
	{
	}

	public virtual void Main()
	{
	}
}
