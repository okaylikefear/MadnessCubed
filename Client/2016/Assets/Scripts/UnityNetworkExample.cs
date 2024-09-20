using System;
using UnityEngine;

public class UnityNetworkExample : MonoBehaviour
{
	private void OnGUI()
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			if (GUI.Button(new Rect(10f, 10f, 100f, 30f), "Connect"))
			{
				Network.Connect(this.remoteIP, this.remotePort);
			}
			if (GUI.Button(new Rect(10f, 50f, 100f, 30f), "Start Server"))
			{
				Network.InitializeServer(32, this.listenPort, true);
			}
			this.remoteIP = GUI.TextField(new Rect(120f, 10f, 100f, 20f), this.remoteIP);
			this.remotePort = int.Parse(GUI.TextField(new Rect(230f, 10f, 40f, 20f), this.remotePort.ToString()));
		}
		else if (GUI.Button(new Rect(10f, 10f, 100f, 50f), "Disconnect"))
		{
			Network.Disconnect(200);
		}
	}

	private void OnConnectedToServer()
	{
		Network.Instantiate(this.PlayerObject, UnityEngine.Random.insideUnitSphere * 5f, Quaternion.identity, 0);
	}

	private void OnServerInitialized()
	{
		Network.Instantiate(this.PlayerObject, UnityEngine.Random.insideUnitSphere * 5f, Quaternion.identity, 0);
	}

	public GameObject PlayerObject;

	private string remoteIP = "127.0.0.1";

	private int remotePort = 25000;

	private int listenPort = 25000;
}
