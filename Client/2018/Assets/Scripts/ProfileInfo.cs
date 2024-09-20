using System;
using kube;
using UnityEngine;

public class ProfileInfo : MonoBehaviour
{
	private void Start()
	{
		this.name.text = Kube.SN.playerInfo.name;
		this.uid.text = Kube.SN.playerUID;
		this.tx.mainTexture = Kube.SN.playerInfo.tx;
	}

	private void Update()
	{
	}

	public UITexture tx;

	public new UILabel name;

	public UILabel uid;
}
