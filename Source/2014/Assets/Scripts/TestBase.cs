using System;
using Photon;

public class TestBase : MonoBehaviour
{
	public virtual void Start()
	{
		PhotonNetwork.autoJoinLobby = false;
	}

	private void Update()
	{
		if (this.ConnectInUpdate && this.AutoConnect)
		{
			this.ConnectInUpdate = false;
			PhotonNetwork.ConnectUsingSettings("1");
		}
	}

	public virtual void OnConnectedToMaster()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	public virtual void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom(null, true, true, 4);
	}

	public bool AutoConnect;

	public int GuiSpace;

	private bool ConnectInUpdate = true;
}
