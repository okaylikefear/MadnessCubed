using System;
using kube;
using UnityEngine;

public class SteamDialog : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("Close", 20f);
	}

	private void Update()
	{
	}

	public void Hide()
	{
		Kube.SN.openURL(this.url);
		base.gameObject.SetActive(false);
	}

	public void Close()
	{
		base.gameObject.SetActive(false);
	}

	public string url = "http://store.steampowered.com/app/453270";
}
