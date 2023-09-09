using System;
using kube;
using UnityEngine;

public class GameTypeControllerBase : MonoBehaviour
{
	public virtual void Initialize()
	{
	}

	public virtual void configure(object[] config)
	{
	}

	public virtual int CalcGameStats()
	{
		if (Kube.BCS.ps != null)
		{
			return Kube.BCS.ps.points;
		}
		return 0;
	}

	public bool canRespawn;
}
