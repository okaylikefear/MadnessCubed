using System;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

internal class PhotonHandler : Photon.MonoBehaviour, IPhotonPeerListener
{
	protected void Awake()
	{
		if (PhotonHandler.SP != null && PhotonHandler.SP != this && PhotonHandler.SP.gameObject != null)
		{
			UnityEngine.Object.DestroyImmediate(PhotonHandler.SP.gameObject);
		}
		PhotonHandler.SP = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		this.updateInterval = 1000 / PhotonNetwork.sendRate;
		this.updateIntervalOnSerialize = 1000 / PhotonNetwork.sendRateOnSerialize;
		PhotonHandler.StartFallbackSendAckThread();
	}

	protected void OnApplicationQuit()
	{
		PhotonNetwork.Disconnect();
		PhotonHandler.StopFallbackSendAckThread();
	}

	protected void Update()
	{
		if (PhotonNetwork.networkingPeer == null)
		{
			UnityEngine.Debug.LogError("NetworkPeer broke!");
			return;
		}
		if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated || PhotonNetwork.connectionStateDetailed == PeerState.Disconnected || PhotonNetwork.offlineMode)
		{
			return;
		}
		if (!PhotonNetwork.isMessageQueueRunning)
		{
			return;
		}
		bool flag = true;
		while (PhotonNetwork.isMessageQueueRunning && flag)
		{
			flag = PhotonNetwork.networkingPeer.DispatchIncomingCommands();
		}
		int num = (int)(Time.realtimeSinceStartup * 1000f);
		if (PhotonNetwork.isMessageQueueRunning && num > this.nextSendTickCountOnSerialize)
		{
			PhotonNetwork.networkingPeer.RunViewUpdate();
			this.nextSendTickCountOnSerialize = num + this.updateIntervalOnSerialize;
			this.nextSendTickCount = 0;
		}
		num = (int)(Time.realtimeSinceStartup * 1000f);
		if (num > this.nextSendTickCount)
		{
			bool flag2 = true;
			while (PhotonNetwork.isMessageQueueRunning && flag2)
			{
				flag2 = PhotonNetwork.networkingPeer.SendOutgoingCommands();
			}
			this.nextSendTickCount = num + this.updateInterval;
		}
	}

	protected void OnLevelWasLoaded(int level)
	{
		PhotonNetwork.networkingPeer.NewSceneLoaded();
		if (PhotonNetwork.automaticallySyncScene)
		{
			this.SetSceneInProps();
		}
	}

	protected void OnJoinedRoom()
	{
		PhotonNetwork.networkingPeer.AutomaticallySyncScene();
	}

	protected void OnCreatedRoom()
	{
		if (PhotonNetwork.automaticallySyncScene)
		{
			this.SetSceneInProps();
		}
	}

	protected internal void SetSceneInProps()
	{
		if (PhotonNetwork.isMasterClient)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["curScn"] = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
			PhotonNetwork.room.SetCustomProperties(hashtable);
		}
	}

	public static void StartFallbackSendAckThread()
	{
		if (PhotonHandler.sendThreadShouldRun)
		{
			return;
		}
		PhotonHandler.sendThreadShouldRun = true;
		SupportClass.CallInBackground(new Func<bool>(PhotonHandler.FallbackSendAckThread));
	}

	public static void StopFallbackSendAckThread()
	{
		PhotonHandler.sendThreadShouldRun = false;
	}

	public static bool FallbackSendAckThread()
	{
		if (PhotonHandler.sendThreadShouldRun && PhotonNetwork.networkingPeer != null)
		{
			PhotonNetwork.networkingPeer.SendAcksOnly();
		}
		return PhotonHandler.sendThreadShouldRun;
	}

	public void DebugReturn(DebugLevel level, string message)
	{
		if (level == DebugLevel.ERROR)
		{
			UnityEngine.Debug.LogError(message);
		}
		else if (level == DebugLevel.WARNING)
		{
			UnityEngine.Debug.LogWarning(message);
		}
		else if (level == DebugLevel.INFO && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			UnityEngine.Debug.Log(message);
		}
		else if (level == DebugLevel.ALL && PhotonNetwork.logLevel == PhotonLogLevel.Full)
		{
			UnityEngine.Debug.Log(message);
		}
	}

	public void OnOperationResponse(OperationResponse operationResponse)
	{
	}

	public void OnStatusChanged(StatusCode statusCode)
	{
	}

	public void OnEvent(EventData photonEvent)
	{
	}

	public static PhotonHandler SP;

	public int updateInterval;

	public int updateIntervalOnSerialize;

	private int nextSendTickCount;

	private int nextSendTickCountOnSerialize;

	private static bool sendThreadShouldRun;
}
