using System;
using kube;
using UnityEngine;

public class AsyncObjectScript : MonoBehaviour
{
	private void Start()
	{
		Kube.RM.requireResource(this.path, new global::AsyncCallback(this.onLoaded));
	}

	private void onLoaded()
	{
		GameObject gameObject = (GameObject)Kube.RM.loadResource(this.path, typeof(GameObject));
		gameObject = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
	}

	public string path;
}
