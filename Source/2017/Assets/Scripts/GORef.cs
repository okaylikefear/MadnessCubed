using System;
using kube;
using UnityEngine;

[Serializable]
public class GORef
{
	public GameObject go
	{
		get
		{
			if (this._go == null && !string.IsNullOrEmpty(this.path))
			{
				this._go = (GameObject)Kube.LoadAssetAtPath(this.path, typeof(GameObject));
			}
			return this._go;
		}
	}

	public string path;

	private GameObject _go;
}
