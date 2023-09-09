using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using kube;
using UnityEngine;

public static class PhotonNetwork
{
	static PhotonNetwork()
	{
		Application.runInBackground = true;
		GameObject gameObject = new GameObject();
		PhotonNetwork.photonMono = gameObject.AddComponent<PhotonHandler>();
		gameObject.AddComponent<PingCloudRegions>();
		gameObject.name = "PhotonMono";
		gameObject.hideFlags = HideFlags.HideInHierarchy;
		PhotonNetwork.networkingPeer = new NetworkingPeer(PhotonNetwork.photonMono, string.Empty, ConnectionProtocol.Udp);
		PhotonNetwork.networkingPeer.LimitOfUnreliableCommands = 40;
		CustomTypes.Register();
	}

	public static string ServerAddress
	{
		get
		{
			return (PhotonNetwork.networkingPeer == null) ? "<not connected>" : PhotonNetwork.networkingPeer.ServerAddress;
		}
	}

	public static bool connected
	{
		get
		{
			return PhotonNetwork.offlineMode || PhotonNetwork.connectionState == ConnectionState.Connected;
		}
	}

	public static ConnectionState connectionState
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return ConnectionState.Connected;
			}
			if (PhotonNetwork.networkingPeer == null)
			{
				return ConnectionState.Disconnected;
			}
			PeerStateValue peerState = PhotonNetwork.networkingPeer.PeerState;
			switch (peerState)
			{
			case PeerStateValue.Disconnected:
				return ConnectionState.Disconnected;
			case PeerStateValue.Connecting:
				return ConnectionState.Connecting;
			default:
				if (peerState != PeerStateValue.InitializingApplication)
				{
					return ConnectionState.Disconnected;
				}
				return ConnectionState.InitializingApplication;
			case PeerStateValue.Connected:
				return ConnectionState.Connected;
			case PeerStateValue.Disconnecting:
				return ConnectionState.Disconnecting;
			}
		}
	}

	public static PeerState connectionStateDetailed
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return PeerState.Connected;
			}
			if (PhotonNetwork.networkingPeer == null)
			{
				return PeerState.Disconnected;
			}
			return PhotonNetwork.networkingPeer.State;
		}
	}

	public static AuthenticationValues AuthValues
	{
		get
		{
			return (PhotonNetwork.networkingPeer == null) ? null : PhotonNetwork.networkingPeer.AuthValues;
		}
		set
		{
			if (PhotonNetwork.networkingPeer != null)
			{
				PhotonNetwork.networkingPeer.AuthValues = value;
			}
		}
	}

	public static Room room
	{
		get
		{
			if (!PhotonNetwork.isOfflineMode)
			{
				return PhotonNetwork.networkingPeer.mCurrentGame;
			}
			if (PhotonNetwork.offlineMode_inRoom)
			{
				return new Room("OfflineRoom", new Hashtable());
			}
			return null;
		}
	}

	public static PhotonPlayer player
	{
		get
		{
			if (PhotonNetwork.networkingPeer == null)
			{
				return null;
			}
			return PhotonNetwork.networkingPeer.mLocalActor;
		}
	}

	public static PhotonPlayer masterClient
	{
		get
		{
			if (PhotonNetwork.networkingPeer == null)
			{
				return null;
			}
			return PhotonNetwork.networkingPeer.mMasterClient;
		}
	}

	public static bool SetMasterClient(PhotonPlayer player)
	{
		return PhotonNetwork.VerifyCanUseNetwork() && PhotonNetwork.isMasterClient && PhotonNetwork.networkingPeer.SetMasterClient(player.ID, true);
	}

	public static string playerName
	{
		get
		{
			return PhotonNetwork.networkingPeer.PlayerName;
		}
		set
		{
			PhotonNetwork.networkingPeer.PlayerName = value;
		}
	}

	public static PhotonPlayer[] playerList
	{
		get
		{
			if (PhotonNetwork.networkingPeer == null)
			{
				return new PhotonPlayer[0];
			}
			return PhotonNetwork.networkingPeer.mPlayerListCopy;
		}
	}

	public static PhotonPlayer[] otherPlayers
	{
		get
		{
			if (PhotonNetwork.networkingPeer == null)
			{
				return new PhotonPlayer[0];
			}
			return PhotonNetwork.networkingPeer.mOtherPlayerListCopy;
		}
	}

	public static List<FriendInfo> Friends { get; set; }

	public static int FriendsListAge
	{
		get
		{
			return (PhotonNetwork.networkingPeer == null) ? 0 : PhotonNetwork.networkingPeer.FriendsListAge;
		}
	}

	public static bool offlineMode
	{
		get
		{
			return PhotonNetwork.isOfflineMode;
		}
		set
		{
			if (value == PhotonNetwork.isOfflineMode)
			{
				return;
			}
			if (value && PhotonNetwork.connected)
			{
				UnityEngine.Debug.LogError("Can't start OFFLINE mode while connected!");
			}
			else
			{
				PhotonNetwork.networkingPeer.Disconnect();
				PhotonNetwork.isOfflineMode = value;
				if (PhotonNetwork.isOfflineMode)
				{
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToPhoton, new object[0]);
					PhotonNetwork.networkingPeer.ChangeLocalID(1);
					PhotonNetwork.networkingPeer.mMasterClient = PhotonNetwork.player;
				}
				else
				{
					PhotonNetwork.networkingPeer.ChangeLocalID(-1);
					PhotonNetwork.networkingPeer.mMasterClient = null;
				}
			}
		}
	}

	public static void RemoteMessage(string methodName, PhotonTargets target = PhotonTargets.All, object[] parameters = null)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[0] = methodName;
		if (parameters != null && parameters.Length > 0)
		{
			hashtable[1] = parameters;
		}
		if (target == PhotonTargets.All)
		{
			PhotonNetwork.networkingPeer.OpRaiseEvent(199, hashtable, true, 0);
			Kube.SendMonoMessage(methodName, parameters);
		}
		if (target == PhotonTargets.AllBuffered)
		{
			PhotonNetwork.networkingPeer.OpRaiseEvent(199, hashtable, true, 0, EventCaching.AddToRoomCache, ReceiverGroup.Others);
			Kube.SendMonoMessage(methodName, parameters);
		}
		else if (target == PhotonTargets.Others)
		{
			PhotonNetwork.networkingPeer.OpRaiseEvent(200, 0, hashtable, true, 0);
		}
		else if (target == PhotonTargets.OthersBuffered)
		{
			PhotonNetwork.networkingPeer.OpRaiseEvent(200, hashtable, true, 0, EventCaching.AddToRoomCache, ReceiverGroup.Others);
		}
		else if (target == PhotonTargets.MasterClient)
		{
			PhotonNetwork.networkingPeer.OpRaiseEvent(199, hashtable, true, 0, EventCaching.DoNotCache, ReceiverGroup.MasterClient);
		}
	}

	[Obsolete("Used for compatibility with Unity networking only.")]
	public static int maxConnections
	{
		get
		{
			if (PhotonNetwork.room == null)
			{
				return 0;
			}
			return PhotonNetwork.room.maxPlayers;
		}
		set
		{
			PhotonNetwork.room.maxPlayers = value;
		}
	}

	public static bool automaticallySyncScene
	{
		get
		{
			return PhotonNetwork._mAutomaticallySyncScene;
		}
		set
		{
			PhotonNetwork._mAutomaticallySyncScene = value;
			if (PhotonNetwork._mAutomaticallySyncScene && PhotonNetwork.room != null)
			{
				PhotonNetwork.networkingPeer.AutomaticallySyncScene();
			}
		}
	}

	public static bool autoCleanUpPlayerObjects
	{
		get
		{
			return PhotonNetwork.m_autoCleanUpPlayerObjects;
		}
		set
		{
			if (PhotonNetwork.room != null)
			{
				UnityEngine.Debug.LogError("Setting autoCleanUpPlayerObjects while in a room is not supported.");
			}
			PhotonNetwork.m_autoCleanUpPlayerObjects = value;
		}
	}

	public static bool autoJoinLobby
	{
		get
		{
			return PhotonNetwork.autoJoinLobbyField;
		}
		set
		{
			PhotonNetwork.autoJoinLobbyField = value;
		}
	}

	public static bool insideLobby
	{
		get
		{
			return PhotonNetwork.networkingPeer.insideLobby;
		}
	}

	public static int sendRate
	{
		get
		{
			return 1000 / PhotonNetwork.sendInterval;
		}
		set
		{
			PhotonNetwork.sendInterval = 1000 / value;
			if (PhotonNetwork.photonMono != null)
			{
				PhotonNetwork.photonMono.updateInterval = PhotonNetwork.sendInterval;
			}
			if (value < PhotonNetwork.sendRateOnSerialize)
			{
				PhotonNetwork.sendRateOnSerialize = value;
			}
		}
	}

	public static int sendRateOnSerialize
	{
		get
		{
			return 1000 / PhotonNetwork.sendIntervalOnSerialize;
		}
		set
		{
			if (value > PhotonNetwork.sendRate)
			{
				UnityEngine.Debug.LogError("Error, can not set the OnSerialize SendRate more often then the overall SendRate");
				value = PhotonNetwork.sendRate;
			}
			PhotonNetwork.sendIntervalOnSerialize = 1000 / value;
			if (PhotonNetwork.photonMono != null)
			{
				PhotonNetwork.photonMono.updateIntervalOnSerialize = PhotonNetwork.sendIntervalOnSerialize;
			}
		}
	}

	public static bool isMessageQueueRunning
	{
		get
		{
			return PhotonNetwork.m_isMessageQueueRunning;
		}
		set
		{
			if (value)
			{
				PhotonHandler.StartFallbackSendAckThread();
			}
			PhotonNetwork.networkingPeer.IsSendingOnlyAcks = !value;
			PhotonNetwork.m_isMessageQueueRunning = value;
		}
	}

	public static int unreliableCommandsLimit
	{
		get
		{
			return PhotonNetwork.networkingPeer.LimitOfUnreliableCommands;
		}
		set
		{
			PhotonNetwork.networkingPeer.LimitOfUnreliableCommands = value;
		}
	}

	public static double time
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return (double)Time.time;
			}
			return PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds / 1000.0;
		}
	}

	public static bool isMasterClient
	{
		get
		{
			return PhotonNetwork.offlineMode || PhotonNetwork.networkingPeer.mMasterClient == PhotonNetwork.networkingPeer.mLocalActor;
		}
	}

	public static bool isNonMasterClientInRoom
	{
		get
		{
			return !PhotonNetwork.isMasterClient && PhotonNetwork.room != null;
		}
	}

	public static int countOfPlayersOnMaster
	{
		get
		{
			return PhotonNetwork.networkingPeer.mPlayersOnMasterCount;
		}
	}

	public static int countOfPlayersInRooms
	{
		get
		{
			return PhotonNetwork.networkingPeer.mPlayersInRoomsCount;
		}
	}

	public static int countOfPlayers
	{
		get
		{
			return PhotonNetwork.networkingPeer.mPlayersInRoomsCount + PhotonNetwork.networkingPeer.mPlayersOnMasterCount;
		}
	}

	public static int countOfRooms
	{
		get
		{
			if (PhotonNetwork.insideLobby)
			{
				return PhotonNetwork.GetRoomList().Length;
			}
			return PhotonNetwork.networkingPeer.mGameCount;
		}
	}

	public static bool NetworkStatisticsEnabled
	{
		get
		{
			return PhotonNetwork.networkingPeer.TrafficStatsEnabled;
		}
		set
		{
			PhotonNetwork.networkingPeer.TrafficStatsEnabled = value;
		}
	}

	public static int ResentReliableCommands
	{
		get
		{
			return PhotonNetwork.networkingPeer.ResentReliableCommands;
		}
	}

	public static void NetworkStatisticsReset()
	{
		PhotonNetwork.networkingPeer.TrafficStatsReset();
	}

	public static string NetworkStatisticsToString()
	{
		if (PhotonNetwork.networkingPeer == null || PhotonNetwork.offlineMode)
		{
			return "Offline or in OfflineMode. No VitalStats available.";
		}
		return PhotonNetwork.networkingPeer.VitalStatsToString(false);
	}

	public static void InternalCleanPhotonMonoFromSceneIfStuck()
	{
		PhotonHandler[] array = UnityEngine.Object.FindObjectsOfType(typeof(PhotonHandler)) as PhotonHandler[];
		if (array != null && array.Length > 0)
		{
			UnityEngine.Debug.Log("Cleaning up hidden PhotonHandler instances in scene. Please save it. This is not an issue.");
			foreach (PhotonHandler photonHandler in array)
			{
				photonHandler.gameObject.hideFlags = (HideFlags)0;
				if (photonHandler.gameObject != null && photonHandler.gameObject.name == "PhotonMono")
				{
					UnityEngine.Object.DestroyImmediate(photonHandler.gameObject);
				}
				UnityEngine.Object.DestroyImmediate(photonHandler);
			}
		}
	}

	public static void ConnectUsingSettings(string gameVersion)
	{
		if (PhotonNetwork.PhotonServerSettings == null)
		{
			UnityEngine.Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return;
		}
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			PhotonNetwork.offlineMode = true;
			return;
		}
		PhotonNetwork.Connect(PhotonNetwork.PhotonServerSettings.ServerAddress, PhotonNetwork.PhotonServerSettings.ServerPort, PhotonNetwork.PhotonServerSettings.AppID, gameVersion);
	}

	public static void ConnectToBestCloudServer(string gameVersion)
	{
		if (PhotonNetwork.PhotonServerSettings == null)
		{
			UnityEngine.Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return;
		}
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			PhotonNetwork.offlineMode = true;
			return;
		}
		PingCloudRegions.ConnectToBestRegion(gameVersion);
	}

	public static void OverrideBestCloudServer(CloudServerRegion region)
	{
		PingCloudRegions.OverrideRegion(region);
	}

	public static void RefreshCloudServerRating()
	{
		PingCloudRegions.RefreshCloudServerRating();
	}

	public static void Connect(string serverAddress, int port, string appID, string gameVersion)
	{
		if (serverAddress.Length <= 2)
		{
			UnityEngine.Debug.LogError("Aborted Connect: invalid serverAddress: " + serverAddress);
			return;
		}
		if (PhotonNetwork.networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("Connect() only works when disconnected. Current state: " + PhotonNetwork.networkingPeer.PeerState);
			return;
		}
		if (PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineMode = false;
			UnityEngine.Debug.LogWarning("Shut down offline mode due to a connect attempt");
		}
		if (!PhotonNetwork.isMessageQueueRunning)
		{
			PhotonNetwork.isMessageQueueRunning = true;
			UnityEngine.Debug.LogWarning("Forced enabling of isMessageQueueRunning because of a Connect()");
		}
		PhotonNetwork.networkingPeer.mAppVersion = gameVersion + "_1.24";
		PhotonNetwork.networkingPeer.MasterServerAddress = serverAddress + ":" + port;
		PhotonNetwork.networkingPeer.Connect(PhotonNetwork.networkingPeer.MasterServerAddress, appID);
	}

	public static void Disconnect()
	{
		if (PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineMode = false;
			PhotonNetwork.networkingPeer.State = PeerState.Disconnecting;
			PhotonNetwork.networkingPeer.OnStatusChanged(StatusCode.Disconnect);
			return;
		}
		if (PhotonNetwork.networkingPeer == null)
		{
			return;
		}
		PhotonNetwork.networkingPeer.Disconnect();
	}

	[Obsolete("Used for compatibility with Unity networking only. Encryption is automatically initialized while connecting.")]
	public static void InitializeSecurity()
	{
	}

	public static bool FindFriends(string[] friendsToFind)
	{
		return PhotonNetwork.networkingPeer != null && !PhotonNetwork.isOfflineMode && PhotonNetwork.networkingPeer.OpFindFriends(friendsToFind);
	}

	public static void CreateRoom(string roomName)
	{
		PhotonNetwork.CreateRoom(roomName, true, true, 0, null, null);
	}

	public static void CreateRoom(string roomName, bool isVisible, bool isOpen, int maxPlayers)
	{
		PhotonNetwork.CreateRoom(roomName, isVisible, isOpen, maxPlayers, null, null);
	}

	public static void CreateRoom(string roomName, bool isVisible, bool isOpen, int maxPlayers, Hashtable customRoomProperties, string[] propsToListInLobby)
	{
		if (PhotonNetwork.connectionStateDetailed == PeerState.Joining || PhotonNetwork.connectionStateDetailed == PeerState.Joined || PhotonNetwork.connectionStateDetailed == PeerState.ConnectedToGameserver)
		{
			UnityEngine.Debug.LogError("CreateRoom aborted: You can only create a room while not currently connected/connecting to a room.");
		}
		else if (PhotonNetwork.room != null)
		{
			UnityEngine.Debug.LogError("CreateRoom aborted: You are already in a room!");
		}
		else if (PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineMode_inRoom = true;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom, new object[0]);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom, new object[0]);
		}
		else
		{
			if (maxPlayers > 255)
			{
				UnityEngine.Debug.LogError("Error: CreateRoom called with " + maxPlayers + " maxplayers. This has been reverted to the max of 255 players because internally a 'byte' is used.");
				maxPlayers = 255;
			}
			PhotonNetwork.networkingPeer.OpCreateGame(roomName, isVisible, isOpen, (byte)maxPlayers, PhotonNetwork.autoCleanUpPlayerObjects, customRoomProperties, propsToListInLobby);
		}
	}

	public static void JoinRoom(string roomName)
	{
		PhotonNetwork.JoinRoom(roomName, false);
	}

	public static void JoinRoom(string roomName, bool createIfNotExists)
	{
		if (PhotonNetwork.connectionStateDetailed == PeerState.Joining || PhotonNetwork.connectionStateDetailed == PeerState.Joined || PhotonNetwork.connectionStateDetailed == PeerState.ConnectedToGameserver)
		{
			UnityEngine.Debug.LogError("JoinRoom aborted: You can only join a room while not currently connected/connecting to a room.");
		}
		else if (PhotonNetwork.room != null)
		{
			UnityEngine.Debug.LogError("JoinRoom aborted: You are already in a room!");
		}
		else if (roomName == string.Empty)
		{
			UnityEngine.Debug.LogError("JoinRoom aborted: You must specifiy a room name!");
		}
		else if (PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineMode_inRoom = true;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom, new object[0]);
		}
		else
		{
			PhotonNetwork.networkingPeer.OpJoin(roomName, createIfNotExists);
		}
	}

	public static void JoinRandomRoom()
	{
		PhotonNetwork.JoinRandomRoom(null, 0);
	}

	public static void JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
	{
		PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom);
	}

	public static void JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchingType)
	{
		if (PhotonNetwork.connectionStateDetailed == PeerState.Joining || PhotonNetwork.connectionStateDetailed == PeerState.Joined || PhotonNetwork.connectionStateDetailed == PeerState.ConnectedToGameserver)
		{
			UnityEngine.Debug.LogError("JoinRandomRoom aborted: You can only join a room while not currently connected/connecting to a room.");
			return;
		}
		if (PhotonNetwork.room != null)
		{
			UnityEngine.Debug.LogError("JoinRandomRoom aborted: You are already in a room!");
			return;
		}
		if (PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineMode_inRoom = true;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom, new object[0]);
		}
		else
		{
			Hashtable hashtable = new Hashtable();
			hashtable.MergeStringKeys(expectedCustomRoomProperties);
			if (expectedMaxPlayers > 0)
			{
				hashtable[byte.MaxValue] = expectedMaxPlayers;
			}
			PhotonNetwork.networkingPeer.OpJoinRandomRoom(hashtable, 0, null, matchingType);
		}
	}

	public static void LeaveRoom()
	{
		if (!PhotonNetwork.offlineMode && PhotonNetwork.connectionStateDetailed != PeerState.Joined)
		{
			UnityEngine.Debug.LogError("PhotonNetwork: Error, you cannot leave a room if you're not in a room!(1)");
			return;
		}
		if (PhotonNetwork.room == null)
		{
			UnityEngine.Debug.LogError("PhotonNetwork: Error, you cannot leave a room if you're not in a room!(2)");
			return;
		}
		if (PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineMode_inRoom = false;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom, new object[0]);
		}
		else
		{
			PhotonNetwork.networkingPeer.OpLeave();
		}
	}

	public static RoomInfo[] GetRoomList()
	{
		if (PhotonNetwork.offlineMode)
		{
			return new RoomInfo[0];
		}
		if (PhotonNetwork.networkingPeer == null)
		{
			return new RoomInfo[0];
		}
		return PhotonNetwork.networkingPeer.mGameListCopy;
	}

	public static void SetPlayerCustomProperties(Hashtable customProperties)
	{
		if (customProperties == null)
		{
			customProperties = new Hashtable();
			foreach (object obj in PhotonNetwork.player.customProperties.Keys)
			{
				customProperties[(string)obj] = null;
			}
		}
		if (PhotonNetwork.room != null && PhotonNetwork.room.isLocalClientInside)
		{
			PhotonNetwork.player.SetCustomProperties(customProperties);
		}
		else
		{
			PhotonNetwork.player.InternalCacheProperties(customProperties);
		}
	}

	public static int AllocateViewID()
	{
		int num = PhotonNetwork.AllocateViewID(PhotonNetwork.player.ID);
		PhotonNetwork.manuallyAllocatedViewIds.Add(num);
		return num;
	}

	public static void UnAllocateViewID(int viewID)
	{
		PhotonNetwork.manuallyAllocatedViewIds.Remove(viewID);
		if (PhotonNetwork.networkingPeer.photonViewList.ContainsKey(viewID))
		{
			UnityEngine.Debug.LogWarning(string.Format("Unallocated manually used viewID: {0} but found it used still in a PhotonView: {1}", viewID, PhotonNetwork.networkingPeer.photonViewList[viewID]));
		}
	}

	private static int AllocateViewID(int ownerId)
	{
		if (ownerId == 0)
		{
			int num = PhotonNetwork.lastUsedViewSubIdStatic;
			int num2 = ownerId * PhotonNetwork.MAX_VIEW_IDS;
			for (int i = 1; i < PhotonNetwork.MAX_VIEW_IDS; i++)
			{
				num = (num + 1) % PhotonNetwork.MAX_VIEW_IDS;
				if (num != 0)
				{
					int num3 = num + num2;
					if (!PhotonNetwork.networkingPeer.photonViewList.ContainsKey(num3))
					{
						PhotonNetwork.lastUsedViewSubIdStatic = num;
						return num3;
					}
				}
			}
			throw new Exception(string.Format("AllocateViewID() failed. Room (user {0}) is out of subIds, as all room viewIDs are used.", ownerId));
		}
		int num4 = PhotonNetwork.lastUsedViewSubId;
		int num5 = ownerId * PhotonNetwork.MAX_VIEW_IDS;
		for (int j = 1; j < PhotonNetwork.MAX_VIEW_IDS; j++)
		{
			num4 = (num4 + 1) % PhotonNetwork.MAX_VIEW_IDS;
			if (num4 != 0)
			{
				int num6 = num4 + num5;
				if (!PhotonNetwork.networkingPeer.photonViewList.ContainsKey(num6) && !PhotonNetwork.manuallyAllocatedViewIds.Contains(num6))
				{
					PhotonNetwork.lastUsedViewSubId = num4;
					return num6;
				}
			}
		}
		throw new Exception(string.Format("AllocateViewID() failed. User {0} is out of subIds, as all viewIDs are used.", ownerId));
	}

	private static int[] AllocateSceneViewIDs(int countOfNewViews)
	{
		int[] array = new int[countOfNewViews];
		for (int i = 0; i < countOfNewViews; i++)
		{
			array[i] = PhotonNetwork.AllocateViewID(0);
		}
		return array;
	}

	public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group)
	{
		return PhotonNetwork.Instantiate(prefabName, position, rotation, group, null);
	}

	public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			UnityEngine.Debug.LogError("PhotonNetwork error: Could not Instantiate the prefab [" + prefabName + "] as the game is not connected.");
			return null;
		}
		GameObject gameObject;
		if (!PhotonNetwork.UsePrefabCache || !PhotonNetwork.PrefabCache.TryGetValue(prefabName, out gameObject))
		{
			gameObject = (GameObject)Kube.Load(prefabName, typeof(GameObject));
			if (PhotonNetwork.UsePrefabCache)
			{
				PhotonNetwork.PrefabCache.Add(prefabName, gameObject);
			}
		}
		if (gameObject == null)
		{
			UnityEngine.Debug.LogError("PhotonNetwork error: Could not Instantiate the prefab [" + prefabName + "]. Please verify you have this gameobject in a Resources folder (and not in a subfolder)");
			return null;
		}
		if (gameObject.GetComponent<PhotonView>() == null)
		{
			UnityEngine.Debug.LogError("PhotonNetwork error: Could not Instantiate the prefab [" + prefabName + "] as it has no PhotonView attached to the root.");
			return null;
		}
		Component[] componentsInChildren = gameObject.GetComponentsInChildren<PhotonView>(true);
		int[] array = new int[componentsInChildren.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = PhotonNetwork.AllocateViewID(PhotonNetwork.player.ID);
		}
		Hashtable evData = PhotonNetwork.networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, false);
		return PhotonNetwork.networkingPeer.DoInstantiate(evData, PhotonNetwork.networkingPeer.mLocalActor, gameObject);
	}

	public static GameObject InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return null;
		}
		if (!PhotonNetwork.isMasterClient)
		{
			UnityEngine.Debug.LogError("PhotonNetwork error [InstantiateSceneObject]: Only the master client can Instantiate scene objects");
			return null;
		}
		GameObject gameObject;
		if (!PhotonNetwork.UsePrefabCache || !PhotonNetwork.PrefabCache.TryGetValue(prefabName, out gameObject))
		{
			gameObject = (GameObject)Kube.Load(prefabName, typeof(GameObject));
			if (PhotonNetwork.UsePrefabCache)
			{
				PhotonNetwork.PrefabCache.Add(prefabName, gameObject);
			}
		}
		if (gameObject == null)
		{
			UnityEngine.Debug.LogError("PhotonNetwork error [InstantiateSceneObject]: Could not Instantiate the prefab [" + prefabName + "]. Please verify you have this gameobject in a Resources folder (and not in a subfolder)");
			return null;
		}
		if (gameObject.GetComponent<PhotonView>() == null)
		{
			UnityEngine.Debug.LogError("PhotonNetwork error [InstantiateSceneObject]: Could not Instantiate the prefab [" + prefabName + "] as it has no PhotonView attached to the root.");
			return null;
		}
		Component[] photonViewsInChildren = gameObject.GetPhotonViewsInChildren();
		int[] array = PhotonNetwork.AllocateSceneViewIDs(photonViewsInChildren.Length);
		if (array == null)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"PhotonNetwork error [InstantiateSceneObject]: Could not Instantiate the prefab [",
				prefabName,
				"] as no ViewIDs are free to use. Max is: ",
				PhotonNetwork.MAX_VIEW_IDS
			}));
			return null;
		}
		Hashtable evData = PhotonNetwork.networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, true);
		return PhotonNetwork.networkingPeer.DoInstantiate(evData, PhotonNetwork.networkingPeer.mLocalActor, gameObject);
	}

	public static int GetPing()
	{
		return PhotonNetwork.networkingPeer.RoundTripTime;
	}

	public static void FetchServerTimestamp()
	{
		if (PhotonNetwork.networkingPeer != null)
		{
			PhotonNetwork.networkingPeer.FetchServerTimestamp();
		}
	}

	public static void SendOutgoingCommands()
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		while (PhotonNetwork.networkingPeer.SendOutgoingCommands())
		{
		}
	}

	public static void CloseConnection(PhotonPlayer kickPlayer)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		if (!PhotonNetwork.player.isMasterClient)
		{
			UnityEngine.Debug.LogError("CloseConnection: Only the masterclient can kick another player.");
		}
		if (kickPlayer == null)
		{
			UnityEngine.Debug.LogError("CloseConnection: No such player connected!");
		}
		else
		{
			int[] targetActors = new int[]
			{
				kickPlayer.ID
			};
			PhotonNetwork.networkingPeer.OpRaiseEvent(203, null, true, 0, targetActors);
		}
	}

	public static void Destroy(PhotonView targetView)
	{
		if (targetView != null)
		{
			PhotonNetwork.networkingPeer.RemoveInstantiatedGO(targetView.gameObject, false);
		}
		else
		{
			UnityEngine.Debug.LogError("Destroy(targetPhotonView) failed, cause targetPhotonView is null.");
		}
	}

	public static void Destroy(GameObject targetGo)
	{
		PhotonNetwork.networkingPeer.RemoveInstantiatedGO(targetGo, false);
	}

	public static void DestroyPlayerObjects(PhotonPlayer targetPlayer)
	{
		if (PhotonNetwork.player == null)
		{
			UnityEngine.Debug.LogError("DestroyPlayerObjects() failed, cause parameter 'targetPlayer' was null.");
		}
		PhotonNetwork.DestroyPlayerObjects(targetPlayer.ID);
	}

	public static void DestroyPlayerObjects(int targetPlayerId)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		if (PhotonNetwork.player.isMasterClient || targetPlayerId == PhotonNetwork.player.ID)
		{
			PhotonNetwork.networkingPeer.DestroyPlayerObjects(targetPlayerId, false);
		}
		else
		{
			UnityEngine.Debug.LogError("DestroyPlayerObjects() failed, cause players can only destroy their own GameObjects. A Master Client can destroy anyone's. This is master: " + PhotonNetwork.isMasterClient);
		}
	}

	public static void DestroyAll()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.networkingPeer.DestroyAll(false);
		}
		else
		{
			UnityEngine.Debug.LogError("Couldn't call RemoveAllInstantiatedObjects as only the master client is allowed to call this.");
		}
	}

	public static void RemoveRPCs(PhotonPlayer targetPlayer)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		if (!targetPlayer.isLocal && !PhotonNetwork.isMasterClient)
		{
			UnityEngine.Debug.LogError("Error; Only the MasterClient can call RemoveRPCs for other players.");
			return;
		}
		PhotonNetwork.networkingPeer.OpCleanRpcBuffer(targetPlayer.ID);
	}

	public static void RemoveRPCs(PhotonView targetPhotonView)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.CleanRpcBufferIfMine(targetPhotonView);
	}

	public static void RemoveRPCsInGroup(int targetGroup)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.RemoveRPCsInGroup(targetGroup);
	}

	internal static void RPC(PhotonView view, string methodName, PhotonTargets target, params object[] parameters)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		if (PhotonNetwork.room == null)
		{
			UnityEngine.Debug.LogWarning("Cannot send RPCs in Lobby! RPC dropped.");
			return;
		}
		if (PhotonNetwork.networkingPeer != null)
		{
			PhotonNetwork.networkingPeer.RPC(view, methodName, target, parameters);
		}
		else
		{
			UnityEngine.Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
		}
	}

	internal static void RPC(PhotonView view, string methodName, PhotonPlayer targetPlayer, params object[] parameters)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		if (PhotonNetwork.room == null)
		{
			UnityEngine.Debug.LogWarning("Cannot send RPCs in Lobby, only processed locally");
			return;
		}
		if (PhotonNetwork.player == null)
		{
			UnityEngine.Debug.LogError("Error; Sending RPC to player null! Aborted \"" + methodName + "\"");
		}
		if (PhotonNetwork.networkingPeer != null)
		{
			PhotonNetwork.networkingPeer.RPC(view, methodName, targetPlayer, parameters);
		}
		else
		{
			UnityEngine.Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
		}
	}

	public static void SetReceivingEnabled(int group, bool enabled)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.SetReceivingEnabled(group, enabled);
	}

	public static void SetSendingEnabled(int group, bool enabled)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.SetSendingEnabled(group, enabled);
	}

	public static void SetLevelPrefix(short prefix)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.SetLevelPrefix(prefix);
	}

	private static bool VerifyCanUseNetwork()
	{
		if (PhotonNetwork.networkingPeer != null && (PhotonNetwork.offlineMode || PhotonNetwork.connected))
		{
			return true;
		}
		UnityEngine.Debug.LogError("Cannot send messages when not connected; Either connect to Photon OR use offline mode!");
		return false;
	}

	public static void LoadLevel(int levelNumber)
	{
		PhotonNetwork.isMessageQueueRunning = false;
		PhotonNetwork.networkingPeer.loadingLevelAndPausedNetwork = true;
		UnityEngine.SceneManagement.SceneManager.LoadScene(levelNumber);
	}

	public static void LoadLevel(string levelTitle)
	{
		PhotonNetwork.isMessageQueueRunning = false;
		PhotonNetwork.networkingPeer.loadingLevelAndPausedNetwork = true;
		UnityEngine.SceneManagement.SceneManager.LoadScene(levelTitle);
	}

	public const string versionPUN = "1.24";

	public const string serverSettingsAssetFile = "PhotonServerSettings";

	public const string serverSettingsAssetPath = "Assets/Photon Unity Networking/Resources/PhotonServerSettings.asset";

	internal static readonly PhotonHandler photonMono;

	internal static readonly NetworkingPeer networkingPeer;

	public static readonly int MAX_VIEW_IDS = 1000;

	public static ServerSettings PhotonServerSettings = (ServerSettings)Resources.Load("PhotonServerSettings", typeof(ServerSettings));

	public static float precisionForVectorSynchronization = 9.9E-05f;

	public static float precisionForQuaternionSynchronization = 1f;

	public static float precisionForFloatSynchronization = 0.01f;

	public static PhotonLogLevel logLevel = PhotonLogLevel.ErrorsOnly;

	public static bool UsePrefabCache = true;

	public static Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();

	private static bool isOfflineMode = false;

	private static bool offlineMode_inRoom = false;

	public static HashSet<GameObject> SendMonoMessageTargets;

	private static bool _mAutomaticallySyncScene = false;

	private static bool m_autoCleanUpPlayerObjects = true;

	private static bool autoJoinLobbyField = true;

	private static int sendInterval = 50;

	private static int sendIntervalOnSerialize = 100;

	private static bool m_isMessageQueueRunning = true;

	internal static int lastUsedViewSubId = 0;

	internal static int lastUsedViewSubIdStatic = 0;

	internal static List<int> manuallyAllocatedViewIds = new List<int>();
}
