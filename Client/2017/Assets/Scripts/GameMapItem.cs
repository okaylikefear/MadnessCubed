using System;
using UnityEngine;

public class GameMapItem : MonoBehaviour
{
	public virtual void SaveMap(KubeStream bw)
	{
	}

	public virtual void LoadMap(KubeStream br)
	{
	}

	public virtual bool CanPlace(Vector3 pos)
	{
		return true;
	}
}
