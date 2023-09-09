using System;
using kube;
using UnityEngine;

public class Top10Item : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnClick()
	{
		Kube.SN.gotoUserByUID(this.uid);
	}

	public UILabel title;

	public UILabel nnplayers;

	public UISprite mode;

	public string uid;
}
