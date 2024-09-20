using System;
using System.Collections;
using kube;
using UnityEngine;

public class KubeItem : MonoBehaviour
{
	private void Start()
	{
		if (this.loading == null)
		{
			this.loading = NGUITools.AddChild(this.tx.gameObject, Cub2Menu.instance.loadingPrefab);
		}
		this.loading.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		if (Kube.ASS2 == null)
		{
			Kube.SS.require("Assets2");
		}
		Cub2Menu.instance.StartCoroutine(this._loadTx());
	}

	private IEnumerator _loadTx()
	{
		while (Kube.ASS2 == null)
		{
			yield return new WaitForSeconds(2f);
		}
		if (this.tx.mainTexture == null)
		{
			this.tx.mainTexture = Kube.ASS2.inventarCubesTex[this.kubeId];
		}
		UnityEngine.Object.Destroy(this.loading);
		this.loading = null;
		yield break;
	}

	private void Update()
	{
	}

	private void OnClick()
	{
		base.transform.parent.parent.GetComponent<KubeMenu>().onSelectKube(this.kubeId);
	}

	public UITexture tx;

	public int kubeId;

	private GameObject loading;
}
