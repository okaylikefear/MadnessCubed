using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using kube;
using UnityEngine;

internal class NetworkingPeer : LoadbalancingPeer, IPhotonPeerListener
{
	public NetworkingPeer(IPhotonPeerListener listener, string playername, ConnectionProtocol connectionProtocol) : base(listener, connectionProtocol)
	{
		base.Listener = this;
		this.externalListener = listener;
		this.PlayerName = playername;
		this.mLocalActor = new PhotonPlayer(true, -1, this.playername);
		this.AddNewPlayer(this.mLocalActor.ID, this.mLocalActor);
		this.rpcShortcuts = new Dictionary<string, int>(PhotonNetwork.PhotonServerSettings.RpcList.Count);
		for (int i = 0; i < PhotonNetwork.PhotonServerSettings.RpcList.Count; i++)
		{
			string key = PhotonNetwork.PhotonServerSettings.RpcList[i];
			this.rpcShortcuts[key] = i;
		}
		this.State = global::PeerState.PeerCreated;
	}

	public AuthenticationValues AuthValues { get; set; }

	public string MasterServerAddress { get; protected internal set; }

	public string PlayerName
	{
		get
		{
			return this.playername;
		}
		set
		{
			if (string.IsNullOrEmpty(value) || value.Equals(this.playername))
			{
				return;
			}
			if (this.mLocalActor != null)
			{
				this.mLocalActor.name = value;
			}
			this.playername = value;
			if (this.mCurrentGame != null)
			{
				this.SendPlayerName();
			}
		}
	}

	public PeerState State { get; internal set; }

	public Room mCurrentGame
	{
		get
		{
			if (this.mRoomToGetInto != null && this.mRoomToGetInto.isLocalClientInside)
			{
				return this.mRoomToGetInto;
			}
			return null;
		}
	}

	internal Room mRoomToGetInto { get; set; }

	public PhotonPlayer mLocalActor { get; internal set; }

	public string mGameserver { get; internal set; }

	public int mQueuePosition { get; internal set; }

	public int mPlayersOnMasterCount { get; internal set; }

	public int mGameCount { get; internal set; }

	public int mPlayersInRoomsCount { get; internal set; }

	public override bool Connect(string serverAddress, string appID)
	{
		if (PhotonNetwork.connectionStateDetailed == global::PeerState.Disconnecting)
		{
			UnityEngine.Debug.LogError("ERROR: Cannot connect to Photon while Disconnecting. Connection failed.");
			return false;
		}
		if (string.IsNullOrEmpty(this.MasterServerAddress))
		{
			this.MasterServerAddress = serverAddress;
		}
		this.mAppId = appID.Trim();
		bool flag = base.Connect(serverAddress, string.Empty);
		this.State = ((!flag) ? global::PeerState.Disconnected : global::PeerState.Connecting);
		return flag;
	}

	public override void Disconnect()
	{
		if (base.PeerState == PeerStateValue.Disconnected)
		{
			if (base.DebugOut >= DebugLevel.WARNING)
			{
				this.DebugReturn(DebugLevel.WARNING, string.Format("Can't execute Disconnect() while not connected. Nothing changed. State: {0}", this.State));
			}
			return;
		}
		this.State = global::PeerState.Disconnecting;
		base.Disconnect();
		this.LeftRoomCleanup();
		this.LeftLobbyCleanup();
	}

	private void DisconnectFromMaster()
	{
		this.State = global::PeerState.DisconnectingFromMasterserver;
		base.Disconnect();
		this.LeftLobbyCleanup();
	}

	private void DisconnectFromGameServer()
	{
		this.State = global::PeerState.DisconnectingFromGameserver;
		base.Disconnect();
		this.LeftRoomCleanup();
	}

	private void LeftLobbyCleanup()
	{
		if (!this.insideLobby)
		{
			return;
		}
		NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftLobby, new object[0]);
		this.insideLobby = false;
		this.isFetchingFriends = false;
	}

	private void LeftRoomCleanup()
	{
		bool flag = this.mRoomToGetInto != null;
		bool flag2 = (this.mRoomToGetInto == null) ? PhotonNetwork.autoCleanUpPlayerObjects : this.mRoomToGetInto.autoCleanUp;
		this.hasSwitchedMC = false;
		this.mRoomToGetInto = null;
		this.mActors = new Dictionary<int, PhotonPlayer>();
		this.mPlayerListCopy = new PhotonPlayer[0];
		this.mOtherPlayerListCopy = new PhotonPlayer[0];
		this.mMasterClient = null;
		this.allowedReceivingGroups = new HashSet<int>();
		this.blockSendingGroups = new HashSet<int>();
		this.mGameList = new Dictionary<string, RoomInfo>();
		this.mGameListCopy = new RoomInfo[0];
		this.isFetchingFriends = false;
		this.ChangeLocalID(-1);
		if (flag2)
		{
			this.LocalCleanupAnythingInstantiated(true);
			PhotonNetwork.manuallyAllocatedViewIds = new List<int>();
		}
		if (flag)
		{
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom, new object[0]);
		}
	}

	protected internal void LocalCleanupAnythingInstantiated(bool destroyInstantiatedGameObjects)
	{
		if (this.tempInstantiationData.Count > 0)
		{
			UnityEngine.Debug.LogWarning("It seems some instantiation is not completed, as instantiation data is used. You should make sure instantiations are paused when calling this method. Cleaning now, despite this.");
		}
		if (destroyInstantiatedGameObjects)
		{
			HashSet<GameObject> hashSet = new HashSet<GameObject>(this.instantiatedObjects.Values);
			foreach (GameObject go in hashSet)
			{
				this.RemoveInstantiatedGO(go, true);
			}
		}
		this.tempInstantiationData.Clear();
		this.instantiatedObjects = new Dictionary<int, GameObject>();
		PhotonNetwork.lastUsedViewSubId = 0;
		PhotonNetwork.lastUsedViewSubIdStatic = 0;
	}

	private void ReadoutProperties(ExitGames.Client.Photon.Hashtable gameProperties, ExitGames.Client.Photon.Hashtable pActorProperties, int targetActorNr)
	{
		if (this.mCurrentGame != null && gameProperties != null)
		{
			this.mCurrentGame.CacheProperties(gameProperties);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, new object[0]);
			if (PhotonNetwork.automaticallySyncScene)
			{
				this.AutomaticallySyncScene();
			}
		}
		if (pActorProperties != null && pActorProperties.Count > 0)
		{
			if (targetActorNr > 0)
			{
				PhotonPlayer playerWithID = this.GetPlayerWithID(targetActorNr);
				if (playerWithID != null)
				{
					playerWithID.InternalCacheProperties(this.GetActorPropertiesForActorNr(pActorProperties, targetActorNr));
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, new object[]
					{
						playerWithID
					});
				}
			}
			else
			{
				foreach (object obj in pActorProperties.Keys)
				{
					int num = (int)obj;
					ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)pActorProperties[obj];
					string name = (string)hashtable[byte.MaxValue];
					PhotonPlayer photonPlayer = this.GetPlayerWithID(num);
					if (photonPlayer == null)
					{
						photonPlayer = new PhotonPlayer(false, num, name);
						this.AddNewPlayer(num, photonPlayer);
					}
					photonPlayer.InternalCacheProperties(hashtable);
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, new object[]
					{
						photonPlayer
					});
				}
			}
		}
	}

	private void AddNewPlayer(int ID, PhotonPlayer player)
	{
		if (!this.mActors.ContainsKey(ID))
		{
			this.mActors[ID] = player;
			this.RebuildPlayerListCopies();
		}
		else
		{
			UnityEngine.Debug.LogError("Adding player twice: " + ID);
		}
	}

	private void RemovePlayer(int ID, PhotonPlayer player)
	{
		this.mActors.Remove(ID);
		if (!player.isLocal)
		{
			this.RebuildPlayerListCopies();
		}
	}

	private void RebuildPlayerListCopies()
	{
		this.mPlayerListCopy = new PhotonPlayer[this.mActors.Count];
		this.mActors.Values.CopyTo(this.mPlayerListCopy, 0);
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		foreach (PhotonPlayer photonPlayer in this.mPlayerListCopy)
		{
			if (!photonPlayer.isLocal)
			{
				list.Add(photonPlayer);
			}
		}
		this.mOtherPlayerListCopy = list.ToArray();
	}

	private void ResetPhotonViewsOnSerialize()
	{
		foreach (PhotonView photonView in this.photonViewList.Values)
		{
			photonView.lastOnSerializeDataSent = null;
		}
	}

	private void HandleEventLeave(int actorID)
	{
		if (base.DebugOut >= DebugLevel.INFO)
		{
			this.DebugReturn(DebugLevel.INFO, "HandleEventLeave actorNr: " + actorID);
		}
		if (actorID < 0 || !this.mActors.ContainsKey(actorID))
		{
			if (base.DebugOut >= DebugLevel.ERROR)
			{
				this.DebugReturn(DebugLevel.ERROR, string.Format("Received event Leave for unknown actorNumber: {0}", actorID));
			}
			return;
		}
		PhotonPlayer playerWithID = this.GetPlayerWithID(actorID);
		if (playerWithID == null)
		{
			UnityEngine.Debug.LogError("Error: HandleEventLeave for actorID=" + actorID + " has no PhotonPlayer!");
		}
		this.CheckMasterClient(actorID);
		if (this.mCurrentGame != null && this.mCurrentGame.autoCleanUp)
		{
			this.DestroyPlayerObjects(actorID, true);
		}
		this.RemovePlayer(actorID, playerWithID);
		NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerDisconnected, new object[]
		{
			playerWithID
		});
	}

	private void CheckMasterClient(int leavingPlayerId)
	{
		bool flag = this.mMasterClient != null && this.mMasterClient.ID == leavingPlayerId;
		bool flag2 = leavingPlayerId > 0;
		if (flag2 && !flag)
		{
			return;
		}
		if (this.mActors.Count <= 1)
		{
			this.mMasterClient = this.mLocalActor;
		}
		else
		{
			int num = int.MaxValue;
			foreach (int num2 in this.mActors.Keys)
			{
				if (num2 < num && num2 != leavingPlayerId)
				{
					num = num2;
				}
			}
			this.mMasterClient = this.mActors[num];
		}
		if (flag2)
		{
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, new object[]
			{
				this.mMasterClient
			});
		}
	}

	private static int ReturnLowestPlayerId(PhotonPlayer[] players, int playerIdToIgnore)
	{
		if (players == null || players.Length == 0)
		{
			return -1;
		}
		int num = int.MaxValue;
		foreach (PhotonPlayer photonPlayer in players)
		{
			if (photonPlayer.ID != playerIdToIgnore)
			{
				if (photonPlayer.ID < num)
				{
					num = photonPlayer.ID;
				}
			}
		}
		return num;
	}

	protected internal bool SetMasterClient(int playerId, bool sync)
	{
		bool flag = this.mMasterClient != null && this.mMasterClient.ID != playerId;
		if (!flag || !this.mActors.ContainsKey(playerId))
		{
			return false;
		}
		if (sync && !this.OpRaiseEvent(208, new ExitGames.Client.Photon.Hashtable
		{
			{
				1,
				playerId
			}
		}, true, 0))
		{
			return false;
		}
		this.hasSwitchedMC = true;
		this.mMasterClient = this.mActors[playerId];
		NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, new object[]
		{
			this.mMasterClient
		});
		return true;
	}

	private ExitGames.Client.Photon.Hashtable GetActorPropertiesForActorNr(ExitGames.Client.Photon.Hashtable actorProperties, int actorNr)
	{
		if (actorProperties.ContainsKey(actorNr))
		{
			return (ExitGames.Client.Photon.Hashtable)actorProperties[actorNr];
		}
		return actorProperties;
	}

	private PhotonPlayer GetPlayerWithID(int number)
	{
		if (this.mActors != null && this.mActors.ContainsKey(number))
		{
			return this.mActors[number];
		}
		return null;
	}

	private void SendPlayerName()
	{
		if (this.State == global::PeerState.Joining)
		{
			this.mPlayernameHasToBeUpdated = true;
			return;
		}
		if (this.mLocalActor != null)
		{
			this.mLocalActor.name = this.PlayerName;
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[byte.MaxValue] = this.PlayerName;
			if (this.mLocalActor.ID > 0)
			{
				base.OpSetPropertiesOfActor(this.mLocalActor.ID, hashtable, true, 0);
				this.mPlayernameHasToBeUpdated = false;
			}
		}
	}

	private void GameEnteredOnGameServer(OperationResponse operationResponse)
	{
		if (operationResponse.ReturnCode != 0)
		{
			switch (operationResponse.OperationCode)
			{
			case 225:
				this.DebugReturn(DebugLevel.WARNING, "Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
				if (operationResponse.ReturnCode == 32758)
				{
					UnityEngine.Debug.Log("Most likely the game became empty during the switch to GameServer.");
				}
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed, new object[0]);
				break;
			case 226:
				this.DebugReturn(DebugLevel.WARNING, "Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
				if (operationResponse.ReturnCode == 32758)
				{
					UnityEngine.Debug.Log("Most likely the game became empty during the switch to GameServer.");
				}
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed, new object[0]);
				break;
			case 227:
				this.DebugReturn(DebugLevel.ERROR, "Create failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed, new object[0]);
				break;
			}
			this.DisconnectFromGameServer();
			return;
		}
		this.State = global::PeerState.Joined;
		this.mRoomToGetInto.isLocalClientInside = true;
		ExitGames.Client.Photon.Hashtable pActorProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[249];
		ExitGames.Client.Photon.Hashtable gameProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[248];
		this.ReadoutProperties(gameProperties, pActorProperties, 0);
		int newID = (int)operationResponse[254];
		this.ChangeLocalID(newID);
		this.CheckMasterClient(-1);
		if (this.mPlayernameHasToBeUpdated)
		{
			this.SendPlayerName();
		}
		switch (operationResponse.OperationCode)
		{
		case 227:
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom, new object[0]);
			break;
		}
	}

	private ExitGames.Client.Photon.Hashtable GetLocalActorProperties()
	{
		if (PhotonNetwork.player != null)
		{
			return PhotonNetwork.player.allProperties;
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[byte.MaxValue] = this.PlayerName;
		return hashtable;
	}

	public void ChangeLocalID(int newID)
	{
		if (this.mLocalActor == null)
		{
			UnityEngine.Debug.LogWarning(string.Format("Local actor is null or not in mActors! mLocalActor: {0} mActors==null: {1} newID: {2}", this.mLocalActor, this.mActors == null, newID));
		}
		if (this.mActors.ContainsKey(this.mLocalActor.ID))
		{
			this.mActors.Remove(this.mLocalActor.ID);
		}
		this.mLocalActor.InternalChangeLocalID(newID);
		this.mActors[this.mLocalActor.ID] = this.mLocalActor;
		this.RebuildPlayerListCopies();
	}

	public bool OpCreateGame(string gameID, bool isVisible, bool isOpen, byte maxPlayers, bool autoCleanUp, ExitGames.Client.Photon.Hashtable customGameProperties, string[] propsListedInLobby)
	{
		this.mRoomToGetInto = new Room(gameID, customGameProperties, isVisible, isOpen, (int)maxPlayers, autoCleanUp, propsListedInLobby);
		this.mLastJoinType = JoinType.CreateGame;
		bool flag = this.State == global::PeerState.Joining;
		return base.OpCreateRoom(gameID, isVisible, isOpen, maxPlayers, autoCleanUp, customGameProperties, (!flag) ? null : this.GetLocalActorProperties(), propsListedInLobby);
	}

	public bool OpJoin(string gameID, bool createIfNotExists)
	{
		this.mRoomToGetInto = new Room(gameID, null);
		this.mLastJoinType = ((!createIfNotExists) ? JoinType.JoinGame : JoinType.JoinOrCreateOnDemand);
		bool flag = this.State == global::PeerState.Joining;
		return this.OpJoinRoom(gameID, (!flag) ? null : this.GetLocalActorProperties(), createIfNotExists);
	}

	public override bool OpJoinRandomRoom(ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, ExitGames.Client.Photon.Hashtable playerProperties, MatchmakingMode matchingType)
	{
		this.mRoomToGetInto = new Room(null, null);
		this.mLastJoinType = JoinType.JoinRandomGame;
		return base.OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, playerProperties, matchingType);
	}

	public virtual bool OpLeave()
	{
		if (this.State != global::PeerState.Joined)
		{
			this.DebugReturn(DebugLevel.ERROR, "NetworkingPeer::leaveGame() - ERROR: no game is currently joined");
			return false;
		}
		return this.OpCustom(254, null, true, 0);
	}

	public override bool OpRaiseEvent(byte eventCode, bool sendReliable, object customEventContent)
	{
		return !PhotonNetwork.offlineMode && base.OpRaiseEvent(eventCode, sendReliable, customEventContent);
	}

	public override bool OpRaiseEvent(byte eventCode, bool sendReliable, object customEventContent, byte channelId, EventCaching cache, int[] targetActors, ReceiverGroup receivers, byte interestGroup)
	{
		return !PhotonNetwork.offlineMode && base.OpRaiseEvent(eventCode, sendReliable, customEventContent, channelId, cache, targetActors, receivers, interestGroup);
	}

	public override bool OpRaiseEvent(byte eventCode, byte interestGroup, ExitGames.Client.Photon.Hashtable evData, bool sendReliable, byte channelId)
	{
		return !PhotonNetwork.offlineMode && base.OpRaiseEvent(eventCode, interestGroup, evData, sendReliable, channelId);
	}

	public override bool OpRaiseEvent(byte eventCode, ExitGames.Client.Photon.Hashtable evData, bool sendReliable, byte channelId, int[] targetActors, EventCaching cache)
	{
		return !PhotonNetwork.offlineMode && base.OpRaiseEvent(eventCode, evData, sendReliable, channelId, targetActors, cache);
	}

	public override bool OpRaiseEvent(byte eventCode, ExitGames.Client.Photon.Hashtable evData, bool sendReliable, byte channelId, EventCaching cache, ReceiverGroup receivers)
	{
		return !PhotonNetwork.offlineMode && base.OpRaiseEvent(eventCode, evData, sendReliable, channelId, cache, receivers);
	}

	public void DebugReturn(DebugLevel level, string message)
	{
		this.externalListener.DebugReturn(level, message);
	}

	public void OnOperationResponse(OperationResponse operationResponse)
	{
		if (PhotonNetwork.networkingPeer.State == global::PeerState.Disconnecting)
		{
			if (base.DebugOut >= DebugLevel.INFO)
			{
				this.DebugReturn(DebugLevel.INFO, "OperationResponse ignored while disconnecting: " + operationResponse.OperationCode);
			}
			return;
		}
		if (operationResponse.ReturnCode == 0)
		{
			if (base.DebugOut >= DebugLevel.INFO)
			{
				this.DebugReturn(DebugLevel.INFO, operationResponse.ToString());
			}
		}
		else if (base.DebugOut >= DebugLevel.WARNING)
		{
			if (operationResponse.ReturnCode == -3)
			{
				this.DebugReturn(DebugLevel.WARNING, "Operation could not be executed yet. Wait for state JoinedLobby or ConnectedToMaster and their respective callbacks before calling OPs. Client must be authorized.");
			}
			this.DebugReturn(DebugLevel.WARNING, operationResponse.ToStringFull());
		}
		byte operationCode = operationResponse.OperationCode;
		switch (operationCode)
		{
		case 222:
		{
			bool[] array = operationResponse[1] as bool[];
			string[] array2 = operationResponse[2] as string[];
			if (array != null && array2 != null && PhotonNetwork.Friends != null && array.Length == PhotonNetwork.Friends.Count)
			{
				for (int i = 0; i < PhotonNetwork.Friends.Count; i++)
				{
					FriendInfo friendInfo = PhotonNetwork.Friends[i];
					friendInfo.Room = array2[i];
					friendInfo.IsOnline = array[i];
				}
			}
			else
			{
				this.DebugReturn(DebugLevel.ERROR, "FindFriends failed to apply the result, as a required value wasn't provided or the friend list length differed from result.");
			}
			this.isFetchingFriends = false;
			this.friendListTimestamp = Environment.TickCount;
			if (this.friendListTimestamp == 0)
			{
				this.friendListTimestamp = 1;
			}
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnUpdatedFriendList, new object[0]);
			break;
		}
		default:
			switch (operationCode)
			{
			case 251:
			{
				ExitGames.Client.Photon.Hashtable pActorProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[249];
				ExitGames.Client.Photon.Hashtable gameProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[248];
				this.ReadoutProperties(gameProperties, pActorProperties, 0);
				break;
			}
			case 252:
				break;
			case 253:
				break;
			case 254:
				this.DisconnectFromGameServer();
				break;
			default:
				if (base.DebugOut >= DebugLevel.ERROR)
				{
					this.DebugReturn(DebugLevel.ERROR, string.Format("operationResponse unhandled: {0}", operationResponse.ToString()));
				}
				break;
			}
			break;
		case 225:
			if (operationResponse.ReturnCode != 0)
			{
				if (operationResponse.ReturnCode == 32760)
				{
					this.DebugReturn(DebugLevel.INFO, "JoinRandom failed: No open game. Calling: OnPhotonRandomJoinFailed() and staying on master server.");
				}
				else if (base.DebugOut >= DebugLevel.ERROR)
				{
					this.DebugReturn(DebugLevel.ERROR, string.Format("JoinRandom failed: {0}.", operationResponse.ToStringFull()));
				}
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed, new object[0]);
			}
			else
			{
				string name = (string)operationResponse[byte.MaxValue];
				this.mRoomToGetInto.name = name;
				this.mGameserver = (string)operationResponse[230];
				this.DisconnectFromMaster();
			}
			break;
		case 226:
			if (this.State != global::PeerState.Joining)
			{
				if (operationResponse.ReturnCode != 0)
				{
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed, new object[0]);
					if (base.DebugOut >= DebugLevel.WARNING)
					{
						this.DebugReturn(DebugLevel.WARNING, string.Format("JoinRoom failed (room maybe closed by now). Client stays on masterserver: {0}. State: {1}", operationResponse.ToStringFull(), this.State));
					}
				}
				else
				{
					this.mGameserver = (string)operationResponse[230];
					this.DisconnectFromMaster();
				}
			}
			else
			{
				this.GameEnteredOnGameServer(operationResponse);
			}
			break;
		case 227:
			if (this.State != global::PeerState.Joining)
			{
				if (operationResponse.ReturnCode != 0)
				{
					if (base.DebugOut >= DebugLevel.ERROR)
					{
						this.DebugReturn(DebugLevel.ERROR, string.Format("createGame failed, client stays on masterserver: {0}.", operationResponse.ToStringFull()));
					}
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed, new object[0]);
				}
				else
				{
					string text = (string)operationResponse[byte.MaxValue];
					if (!string.IsNullOrEmpty(text))
					{
						this.mRoomToGetInto.name = text;
					}
					this.mGameserver = (string)operationResponse[230];
					this.DisconnectFromMaster();
				}
			}
			else
			{
				this.GameEnteredOnGameServer(operationResponse);
			}
			break;
		case 228:
			this.State = global::PeerState.Authenticated;
			this.LeftLobbyCleanup();
			break;
		case 229:
			this.State = global::PeerState.JoinedLobby;
			this.insideLobby = true;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedLobby, new object[0]);
			break;
		case 230:
			if (operationResponse.ReturnCode != 0)
			{
				if (operationResponse.ReturnCode == -2)
				{
					this.DebugReturn(DebugLevel.ERROR, string.Format("If you host Photon yourself, make sure to start the 'Instance LoadBalancing'", new object[0]));
				}
				else if (operationResponse.ReturnCode == 32767)
				{
					this.DebugReturn(DebugLevel.ERROR, string.Format("The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account.", new object[0]));
				}
				else if (operationResponse.ReturnCode == 32755)
				{
					this.DebugReturn(DebugLevel.ERROR, string.Format("Custom Authentication failed (either due to user-input or configuration or AuthParameter string format). Calling: OnCustomAuthenticationFailed()", new object[0]));
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCustomAuthenticationFailed, new object[]
					{
						operationResponse.DebugMessage
					});
				}
				else if (base.DebugOut >= DebugLevel.ERROR)
				{
					this.DebugReturn(DebugLevel.ERROR, string.Format("Authentication failed: '{0}' Code: {1}", operationResponse.DebugMessage, operationResponse.ReturnCode));
				}
				this.Disconnect();
				this.State = global::PeerState.Disconnecting;
				if (operationResponse.ReturnCode == 32757)
				{
					this.DebugReturn(DebugLevel.ERROR, string.Format("Currently, the limit of users is reached for this title. Try again later. Disconnecting", new object[0]));
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonMaxCccuReached, new object[0]);
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, new object[]
					{
						DisconnectCause.MaxCcuReached
					});
				}
				else if (operationResponse.ReturnCode == 32756)
				{
					this.DebugReturn(DebugLevel.ERROR, string.Format("The used master server address is not available with the subscription currently used. Got to Photon Cloud Dashboard or change URL. Disconnecting", new object[0]));
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, new object[]
					{
						DisconnectCause.InvalidRegion
					});
				}
			}
			else if (this.State == global::PeerState.Connected || this.State == global::PeerState.ConnectedComingFromGameserver)
			{
				if (operationResponse.Parameters.ContainsKey(221))
				{
					if (this.AuthValues != null)
					{
						this.AuthValues.Secret = (operationResponse[221] as string);
					}
					else if (base.DebugOut >= DebugLevel.WARNING)
					{
						this.DebugReturn(DebugLevel.WARNING, "Server returned secret but AuthValues are null. Won't use this.");
					}
				}
				if (PhotonNetwork.autoJoinLobby)
				{
					this.OpJoinLobby();
					this.State = global::PeerState.Authenticated;
				}
				else
				{
					this.State = global::PeerState.ConnectedToMaster;
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster, new object[0]);
				}
			}
			else if (this.State == global::PeerState.ConnectedToGameserver)
			{
				this.State = global::PeerState.Joining;
				if (this.AuthValues != null)
				{
					this.AuthValues.Secret = null;
				}
				if (this.mLastJoinType == JoinType.JoinGame || this.mLastJoinType == JoinType.JoinRandomGame || this.mLastJoinType == JoinType.JoinOrCreateOnDemand)
				{
					this.OpJoin(this.mRoomToGetInto.name, this.mLastJoinType == JoinType.JoinOrCreateOnDemand);
				}
				else if (this.mLastJoinType == JoinType.CreateGame)
				{
					this.OpCreateGame(this.mRoomToGetInto.name, this.mRoomToGetInto.visible, this.mRoomToGetInto.open, (byte)this.mRoomToGetInto.maxPlayers, this.mRoomToGetInto.autoCleanUp, this.mRoomToGetInto.customProperties, this.mRoomToGetInto.propertiesListedInLobby);
				}
			}
			break;
		}
		this.externalListener.OnOperationResponse(operationResponse);
	}

	protected internal int FriendsListAge
	{
		get
		{
			return (!this.isFetchingFriends && this.friendListTimestamp != 0) ? (Environment.TickCount - this.friendListTimestamp) : 0;
		}
	}

	public override bool OpFindFriends(string[] friendsToFind)
	{
		if (this.isFetchingFriends)
		{
			return false;
		}
		this.isFetchingFriends = true;
		PhotonNetwork.Friends = new List<FriendInfo>(friendsToFind.Length);
		foreach (string name in friendsToFind)
		{
			PhotonNetwork.Friends.Add(new FriendInfo
			{
				Name = name
			});
		}
		return base.OpFindFriends(friendsToFind);
	}

	public void OnStatusChanged(StatusCode statusCode)
	{
		if (base.DebugOut >= DebugLevel.INFO)
		{
			this.DebugReturn(DebugLevel.INFO, string.Format("OnStatusChanged: {0}", statusCode.ToString()));
		}
		switch (statusCode)
		{
		case StatusCode.SecurityExceptionOnConnect:
		case StatusCode.ExceptionOnConnect:
			this.State = global::PeerState.PeerCreated;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, new object[]
			{
				(DisconnectCause)statusCode
			});
			goto IL_458;
		case StatusCode.Connect:
			if (this.State == global::PeerState.ConnectingToGameserver)
			{
				if (base.DebugOut >= DebugLevel.ALL)
				{
					this.DebugReturn(DebugLevel.ALL, "Connected to gameserver.");
				}
				this.State = global::PeerState.ConnectedToGameserver;
			}
			else
			{
				if (base.DebugOut >= DebugLevel.ALL)
				{
					this.DebugReturn(DebugLevel.ALL, "Connected to masterserver.");
				}
				if (this.State == global::PeerState.Connecting)
				{
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToPhoton, new object[0]);
					this.State = global::PeerState.Connected;
				}
				else
				{
					this.State = global::PeerState.ConnectedComingFromGameserver;
				}
			}
			if (this.requestSecurity || this.AuthValues != null)
			{
				base.EstablishEncryption();
			}
			else if (!this.OpAuthenticate(this.mAppId, this.mAppVersion, this.PlayerName, this.AuthValues))
			{
				this.externalListener.DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + this.State);
			}
			goto IL_458;
		case StatusCode.Disconnect:
			if (this.State == global::PeerState.DisconnectingFromMasterserver)
			{
				if (this.Connect(this.mGameserver, this.mAppId))
				{
					this.State = global::PeerState.ConnectingToGameserver;
				}
			}
			else if (this.State == global::PeerState.DisconnectingFromGameserver)
			{
				if (this.Connect(this.MasterServerAddress, this.mAppId))
				{
					this.State = global::PeerState.ConnectingToMasterserver;
				}
			}
			else
			{
				this.LeftRoomCleanup();
				this.State = global::PeerState.PeerCreated;
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnDisconnectedFromPhoton, new object[0]);
			}
			goto IL_458;
		case StatusCode.Exception:
			if (this.State == global::PeerState.Connecting)
			{
				this.DebugReturn(DebugLevel.WARNING, "Exception while connecting to: " + base.ServerAddress + ". Check if the server is available.");
				if (base.ServerAddress == null || base.ServerAddress.StartsWith("127.0.0.1"))
				{
					this.DebugReturn(DebugLevel.WARNING, "The server address is 127.0.0.1 (localhost): Make sure the server is running on this machine. Android and iOS emulators have their own localhost.");
					if (base.ServerAddress == this.mGameserver)
					{
						this.DebugReturn(DebugLevel.WARNING, "This might be a misconfiguration in the game server config. You need to edit it to a (public) address.");
					}
				}
				this.State = global::PeerState.PeerCreated;
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, new object[]
				{
					(DisconnectCause)statusCode
				});
			}
			else
			{
				this.State = global::PeerState.PeerCreated;
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, new object[]
				{
					(DisconnectCause)statusCode
				});
			}
			this.Disconnect();
			goto IL_458;
		case StatusCode.QueueOutgoingReliableWarning:
		case StatusCode.QueueOutgoingUnreliableWarning:
		case StatusCode.QueueOutgoingAcksWarning:
		case StatusCode.QueueSentWarning:
			goto IL_458;
		case StatusCode.SendError:
			goto IL_458;
		case StatusCode.ExceptionOnReceive:
		case StatusCode.TimeoutDisconnect:
		case StatusCode.DisconnectByServer:
		case StatusCode.DisconnectByServerUserLimit:
		case StatusCode.DisconnectByServerLogic:
			if (this.State == global::PeerState.Connecting)
			{
				this.DebugReturn(DebugLevel.WARNING, string.Concat(new object[]
				{
					statusCode,
					" while connecting to: ",
					base.ServerAddress,
					". Check if the server is available."
				}));
				this.State = global::PeerState.PeerCreated;
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, new object[]
				{
					(DisconnectCause)statusCode
				});
			}
			else
			{
				this.State = global::PeerState.PeerCreated;
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, new object[]
				{
					(DisconnectCause)statusCode
				});
			}
			this.Disconnect();
			goto IL_458;
		case StatusCode.EncryptionEstablished:
			if (!this.OpAuthenticate(this.mAppId, this.mAppVersion, this.PlayerName, this.AuthValues))
			{
				this.externalListener.DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + this.State);
			}
			goto IL_458;
		case StatusCode.EncryptionFailedToEstablish:
			this.externalListener.DebugReturn(DebugLevel.ERROR, "Encryption wasn't established: " + statusCode + ". Going to authenticate anyways.");
			if (!this.OpAuthenticate(this.mAppId, this.mAppVersion, this.PlayerName, this.AuthValues))
			{
				this.externalListener.DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + this.State);
			}
			goto IL_458;
		}
		this.DebugReturn(DebugLevel.ERROR, "Received unknown status code: " + statusCode);
		IL_458:
		this.externalListener.OnStatusChanged(statusCode);
	}

	public static void ExecuteMonoMessage(ExitGames.Client.Photon.Hashtable par1, PhotonPlayer originatingPlayer)
	{
		string methodString = par1[0] as string;
		object[] parameters = par1[1] as object[];
		Kube.SendMonoMessage(methodString, parameters);
	}

	public void OnEvent(EventData photonEvent)
	{
		if (base.DebugOut >= DebugLevel.INFO)
		{
			this.DebugReturn(DebugLevel.INFO, string.Format("OnEvent: {0}", photonEvent.ToString()));
		}
		int num = -1;
		PhotonPlayer photonPlayer = null;
		if (photonEvent.Parameters.ContainsKey(254))
		{
			num = (int)photonEvent[254];
			if (this.mActors.ContainsKey(num))
			{
				photonPlayer = this.mActors[num];
			}
		}
		byte code = photonEvent.Code;
		switch (code)
		{
		case 199:
			NetworkingPeer.ExecuteMonoMessage(photonEvent[245] as ExitGames.Client.Photon.Hashtable, photonPlayer);
			break;
		case 200:
			this.ExecuteRPC(photonEvent[245] as ExitGames.Client.Photon.Hashtable, photonPlayer);
			break;
		case 201:
		case 206:
		{
			ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int networkTime = (int)hashtable[0];
			short correctPrefix = -1;
			short num2 = 1;
			if (hashtable.ContainsKey(1))
			{
				correctPrefix = (short)hashtable[1];
				num2 = 2;
			}
			short num3 = num2;
			while ((int)num3 < hashtable.Count)
			{
				this.OnSerializeRead(hashtable[num3] as ExitGames.Client.Photon.Hashtable, photonPlayer, networkTime, correctPrefix);
				num3 += 1;
			}
			break;
		}
		case 202:
			this.DoInstantiate((ExitGames.Client.Photon.Hashtable)photonEvent[245], photonPlayer, null);
			break;
		case 203:
			if (photonPlayer == null || !photonPlayer.isMasterClient)
			{
				UnityEngine.Debug.LogError("Error: Someone else(" + photonPlayer + ") then the masterserver requests a disconnect!");
			}
			else
			{
				PhotonNetwork.LeaveRoom();
			}
			break;
		case 204:
		{
			ExitGames.Client.Photon.Hashtable hashtable2 = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int num4 = (int)hashtable2[0];
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Ev Destroy for viewId: ",
				num4,
				" sent by owner: ",
				num4 / PhotonNetwork.MAX_VIEW_IDS == num,
				" this client is owner: ",
				num4 / PhotonNetwork.MAX_VIEW_IDS == this.mLocalActor.ID
			}));
			GameObject gameObject = null;
			this.instantiatedObjects.TryGetValue(num4, out gameObject);
			if (gameObject == null || photonPlayer == null)
			{
				UnityEngine.Debug.LogError(string.Concat(new object[]
				{
					"Can't execute received Destroy request for view ID=",
					num4,
					" as GO can't be foudn. From player/actorNr: ",
					num,
					" goToDestroyLocally=",
					gameObject,
					"  originating Player=",
					photonPlayer
				}));
			}
			else
			{
				this.RemoveInstantiatedGO(gameObject, true);
			}
			break;
		}
		default:
			switch (code)
			{
			case 226:
				this.mPlayersInRoomsCount = (int)photonEvent[229];
				this.mPlayersOnMasterCount = (int)photonEvent[227];
				this.mGameCount = (int)photonEvent[228];
				break;
			default:
				switch (code)
				{
				case 253:
				{
					int num5 = (int)photonEvent[253];
					ExitGames.Client.Photon.Hashtable gameProperties = null;
					ExitGames.Client.Photon.Hashtable pActorProperties = null;
					if (num5 == 0)
					{
						gameProperties = (ExitGames.Client.Photon.Hashtable)photonEvent[251];
					}
					else
					{
						pActorProperties = (ExitGames.Client.Photon.Hashtable)photonEvent[251];
					}
					this.ReadoutProperties(gameProperties, pActorProperties, num5);
					break;
				}
				case 254:
					this.HandleEventLeave(num);
					break;
				case 255:
				{
					ExitGames.Client.Photon.Hashtable properties = (ExitGames.Client.Photon.Hashtable)photonEvent[249];
					if (photonPlayer == null)
					{
						bool isLocal = this.mLocalActor.ID == num;
						this.AddNewPlayer(num, new PhotonPlayer(isLocal, num, properties));
						this.ResetPhotonViewsOnSerialize();
					}
					if (this.mActors[num] == this.mLocalActor)
					{
						int[] array = (int[])photonEvent[252];
						foreach (int num6 in array)
						{
							if (this.mLocalActor.ID != num6 && !this.mActors.ContainsKey(num6))
							{
								this.AddNewPlayer(num6, new PhotonPlayer(false, num6, string.Empty));
							}
						}
						if (this.mLastJoinType == JoinType.JoinOrCreateOnDemand && this.mLocalActor.ID == 1)
						{
							NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom, new object[0]);
						}
						else
						{
							NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom, new object[0]);
						}
					}
					else
					{
						NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerConnected, new object[]
						{
							this.mActors[num]
						});
					}
					break;
				}
				default:
					UnityEngine.Debug.LogError("Error. Unhandled event: " + photonEvent);
					break;
				}
				break;
			case 228:
				if (photonEvent.Parameters.ContainsKey(223))
				{
					this.mQueuePosition = (int)photonEvent[223];
				}
				else
				{
					this.DebugReturn(DebugLevel.ERROR, "Event QueueState must contain position!");
				}
				if (this.mQueuePosition == 0)
				{
					if (PhotonNetwork.autoJoinLobby)
					{
						this.OpJoinLobby();
						this.State = global::PeerState.Authenticated;
					}
					else
					{
						this.State = global::PeerState.ConnectedToMaster;
						NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster, new object[0]);
					}
				}
				break;
			case 229:
			{
				ExitGames.Client.Photon.Hashtable hashtable3 = (ExitGames.Client.Photon.Hashtable)photonEvent[222];
				foreach (DictionaryEntry dictionaryEntry in hashtable3)
				{
					string text = (string)dictionaryEntry.Key;
					Room room = new Room(text, (ExitGames.Client.Photon.Hashtable)dictionaryEntry.Value);
					if (room.removedFromList)
					{
						this.mGameList.Remove(text);
					}
					else
					{
						this.mGameList[text] = room;
					}
				}
				this.mGameListCopy = new RoomInfo[this.mGameList.Count];
				this.mGameList.Values.CopyTo(this.mGameListCopy, 0);
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate, new object[0]);
				break;
			}
			case 230:
			{
				this.mGameList = new Dictionary<string, RoomInfo>();
				ExitGames.Client.Photon.Hashtable hashtable4 = (ExitGames.Client.Photon.Hashtable)photonEvent[222];
				foreach (DictionaryEntry dictionaryEntry2 in hashtable4)
				{
					string text2 = (string)dictionaryEntry2.Key;
					this.mGameList[text2] = new RoomInfo(text2, (ExitGames.Client.Photon.Hashtable)dictionaryEntry2.Value);
				}
				this.mGameListCopy = new RoomInfo[this.mGameList.Count];
				this.mGameList.Values.CopyTo(this.mGameListCopy, 0);
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate, new object[0]);
				break;
			}
			}
			break;
		case 207:
		{
			ExitGames.Client.Photon.Hashtable hashtable2 = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int num7 = (int)hashtable2[0];
			if (num7 >= 0)
			{
				this.DestroyPlayerObjects(num7, true);
			}
			else
			{
				UnityEngine.Debug.Log("Ev DestroyAll! By PlayerId: " + num);
				this.DestroyAll(true);
			}
			break;
		}
		case 208:
		{
			ExitGames.Client.Photon.Hashtable hashtable2 = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int playerId = (int)hashtable2[1];
			this.SetMasterClient(playerId, false);
			break;
		}
		}
		this.externalListener.OnEvent(photonEvent);
	}

	public static void SendMonoMessage(PhotonNetworkingMessage methodString, params object[] parameters)
	{
		HashSet<GameObject> hashSet;
		if (PhotonNetwork.SendMonoMessageTargets != null)
		{
			hashSet = PhotonNetwork.SendMonoMessageTargets;
		}
		else
		{
			hashSet = new HashSet<GameObject>();
			Component[] array = (Component[])UnityEngine.Object.FindObjectsOfType(typeof(MonoBehaviour));
			for (int i = 0; i < array.Length; i++)
			{
				hashSet.Add(array[i].gameObject);
			}
		}
		string methodName = methodString.ToString();
		foreach (GameObject gameObject in hashSet)
		{
			if (parameters != null && parameters.Length == 1)
			{
				gameObject.SendMessage(methodName, parameters[0], SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				gameObject.SendMessage(methodName, parameters, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void ExecuteRPC(ExitGames.Client.Photon.Hashtable rpcData, PhotonPlayer sender)
	{
		if (rpcData == null || !rpcData.ContainsKey(0))
		{
			this.DebugReturn(DebugLevel.ERROR, "Malformed RPC; this should never occur.");
			return;
		}
		int num = (int)rpcData[0];
		int num2 = 0;
		if (rpcData.ContainsKey(1))
		{
			num2 = (int)((short)rpcData[1]);
		}
		string text;
		if (rpcData.ContainsKey(5))
		{
			int num3 = (int)((byte)rpcData[5]);
			if (num3 > PhotonNetwork.PhotonServerSettings.RpcList.Count - 1)
			{
				UnityEngine.Debug.LogError("Could not find RPC with index: " + num3 + ". Going to ignore! Check PhotonServerSettings.RpcList");
				return;
			}
			text = PhotonNetwork.PhotonServerSettings.RpcList[num3];
		}
		else
		{
			text = (string)rpcData[3];
		}
		object[] array = null;
		if (rpcData.ContainsKey(4))
		{
			array = (object[])rpcData[4];
		}
		if (array == null)
		{
			array = new object[0];
		}
		PhotonView photonView = this.GetPhotonView(num);
		if (photonView == null)
		{
			int num4 = num / PhotonNetwork.MAX_VIEW_IDS;
			bool flag = num4 == this.mLocalActor.ID;
			bool flag2 = num4 == sender.ID;
			if (flag)
			{
				UnityEngine.Debug.LogWarning(string.Concat(new object[]
				{
					"Received RPC \"",
					text,
					"\" for viewID ",
					num,
					" but this PhotonView does not exist! View was/is ours.",
					(!flag2) ? " Remote called." : " Owner called."
				}));
			}
			else
			{
				UnityEngine.Debug.LogError(string.Concat(new object[]
				{
					"Received RPC \"",
					text,
					"\" for viewID ",
					num,
					" but this PhotonView does not exist! Was remote PV.",
					(!flag2) ? " Remote called." : " Owner called."
				}));
			}
			return;
		}
		if (photonView.prefix != num2)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Received RPC \"",
				text,
				"\" on viewID ",
				num,
				" with a prefix of ",
				num2,
				", our prefix is ",
				photonView.prefix,
				". The RPC has been ignored."
			}));
			return;
		}
		if (text == string.Empty)
		{
			this.DebugReturn(DebugLevel.ERROR, "Malformed RPC; this should never occur.");
			return;
		}
		if (base.DebugOut >= DebugLevel.ALL)
		{
			this.DebugReturn(DebugLevel.ALL, "Received RPC; " + text);
		}
		if (photonView.group != 0 && !this.allowedReceivingGroups.Contains(photonView.group))
		{
			return;
		}
		Type[] array2 = new Type[0];
		if (array.Length > 0)
		{
			array2 = new Type[array.Length];
			int num5 = 0;
			foreach (object obj in array)
			{
				if (obj == null)
				{
					array2[num5] = null;
				}
				else
				{
					array2[num5] = obj.GetType();
				}
				num5++;
			}
		}
		int num6 = 0;
		int num7 = 0;
		foreach (MonoBehaviour monoBehaviour in photonView.GetComponents<MonoBehaviour>())
		{
			if (monoBehaviour == null)
			{
				UnityEngine.Debug.LogError("ERROR You have missing MonoBehaviours on your gameobjects!");
			}
			else
			{
				Type type = monoBehaviour.GetType();
				List<MethodInfo> list = null;
				if (this.monoRPCMethodsCache.ContainsKey(type))
				{
					list = this.monoRPCMethodsCache[type];
				}
				if (list == null)
				{
					List<MethodInfo> methods = SupportClass.GetMethods(type, typeof(RPC));
					this.monoRPCMethodsCache[type] = methods;
					list = methods;
				}
				if (list != null)
				{
					for (int k = 0; k < list.Count; k++)
					{
						MethodInfo methodInfo = list[k];
						if (methodInfo.Name == text)
						{
							num7++;
							ParameterInfo[] parameters = methodInfo.GetParameters();
							if (parameters.Length == array2.Length)
							{
								if (this.CheckTypeMatch(parameters, array2))
								{
									num6++;
									object obj2 = methodInfo.Invoke(monoBehaviour, array);
									if (methodInfo.ReturnType == typeof(IEnumerator))
									{
										PhotonHandler.SP.StartCoroutine((IEnumerator)obj2);
									}
								}
							}
							else if (parameters.Length - 1 == array2.Length)
							{
								if (this.CheckTypeMatch(parameters, array2) && parameters[parameters.Length - 1].ParameterType == typeof(PhotonMessageInfo))
								{
									num6++;
									int timestamp = (int)rpcData[2];
									object[] array3 = new object[array.Length + 1];
									array.CopyTo(array3, 0);
									array3[array3.Length - 1] = new PhotonMessageInfo(sender, timestamp, photonView);
									object obj3 = methodInfo.Invoke(monoBehaviour, array3);
									if (methodInfo.ReturnType == typeof(IEnumerator))
									{
										PhotonHandler.SP.StartCoroutine((IEnumerator)obj3);
									}
								}
							}
							else if (parameters.Length == 1 && parameters[0].ParameterType.IsArray)
							{
								num6++;
								object obj4 = methodInfo.Invoke(monoBehaviour, new object[]
								{
									array
								});
								if (methodInfo.ReturnType == typeof(IEnumerator))
								{
									PhotonHandler.SP.StartCoroutine((IEnumerator)obj4);
								}
							}
						}
					}
				}
			}
		}
		if (num6 != 1)
		{
			string text2 = string.Empty;
			foreach (Type type2 in array2)
			{
				if (text2 != string.Empty)
				{
					text2 += ", ";
				}
				if (type2 == null)
				{
					text2 += "null";
				}
				else
				{
					text2 += type2.Name;
				}
			}
			if (num6 == 0)
			{
				if (num7 == 0)
				{
					this.DebugReturn(DebugLevel.ERROR, string.Concat(new object[]
					{
						"PhotonView with ID ",
						num,
						" has no method \"",
						text,
						"\" marked with the [RPC](C#) or @RPC(JS) property! Args: ",
						text2
					}));
				}
				else
				{
					this.DebugReturn(DebugLevel.ERROR, string.Concat(new object[]
					{
						"PhotonView with ID ",
						num,
						" has no method \"",
						text,
						"\" that takes ",
						array2.Length,
						" argument(s): ",
						text2
					}));
				}
			}
			else
			{
				this.DebugReturn(DebugLevel.ERROR, string.Concat(new object[]
				{
					"PhotonView with ID ",
					num,
					" has ",
					num6,
					" methods \"",
					text,
					"\" that takes ",
					array2.Length,
					" argument(s): ",
					text2,
					". Should be just one?"
				}));
			}
		}
	}

	private bool CheckTypeMatch(ParameterInfo[] methodParameters, Type[] callParameterTypes)
	{
		if (methodParameters.Length < callParameterTypes.Length)
		{
			return false;
		}
		for (int i = 0; i < callParameterTypes.Length; i++)
		{
			Type parameterType = methodParameters[i].ParameterType;
			if (callParameterTypes[i] != null && !parameterType.Equals(callParameterTypes[i]))
			{
				return false;
			}
		}
		return true;
	}

	internal ExitGames.Client.Photon.Hashtable SendInstantiate(string prefabName, Vector3 position, Quaternion rotation, int group, int[] viewIDs, object[] data, bool isGlobalObject)
	{
		int num = viewIDs[0];
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[0] = prefabName;
		if (position != Vector3.zero)
		{
			hashtable[1] = position;
		}
		if (rotation != Quaternion.identity)
		{
			hashtable[2] = rotation;
		}
		if (group != 0)
		{
			hashtable[3] = group;
		}
		if (viewIDs.Length > 1)
		{
			hashtable[4] = viewIDs;
		}
		if (data != null)
		{
			hashtable[5] = data;
		}
		if (this.currentLevelPrefix > 0)
		{
			hashtable[8] = this.currentLevelPrefix;
		}
		hashtable[6] = base.ServerTimeInMilliSeconds;
		hashtable[7] = num;
		EventCaching cache = (!isGlobalObject) ? EventCaching.AddToRoomCache : EventCaching.AddToRoomCacheGlobal;
		this.OpRaiseEvent(202, hashtable, true, 0, cache, ReceiverGroup.Others);
		return hashtable;
	}

	internal GameObject DoInstantiate(ExitGames.Client.Photon.Hashtable evData, PhotonPlayer photonPlayer, GameObject resourceGameObject)
	{
		string text = (string)evData[0];
		int timestamp = (int)evData[6];
		int num = (int)evData[7];
		Vector3 position;
		if (evData.ContainsKey(1))
		{
			position = (Vector3)evData[1];
		}
		else
		{
			position = Vector3.zero;
		}
		Quaternion rotation = Quaternion.identity;
		if (evData.ContainsKey(2))
		{
			rotation = (Quaternion)evData[2];
		}
		int num2 = 0;
		if (evData.ContainsKey(3))
		{
			num2 = (int)evData[3];
		}
		short prefix = 0;
		if (evData.ContainsKey(8))
		{
			prefix = (short)evData[8];
		}
		int[] array;
		if (evData.ContainsKey(4))
		{
			array = (int[])evData[4];
		}
		else
		{
			array = new int[]
			{
				num
			};
		}
		object[] instantiationData;
		if (evData.ContainsKey(5))
		{
			instantiationData = (object[])evData[5];
		}
		else
		{
			instantiationData = null;
		}
		if (num2 != 0 && !this.allowedReceivingGroups.Contains(num2))
		{
			return null;
		}
		if (resourceGameObject == null)
		{
			if (!NetworkingPeer.UsePrefabCache || !NetworkingPeer.PrefabCache.TryGetValue(text, out resourceGameObject))
			{
				resourceGameObject = (GameObject)Kube.Load(text, typeof(GameObject));
				if (NetworkingPeer.UsePrefabCache)
				{
					NetworkingPeer.PrefabCache.Add(text, resourceGameObject);
				}
			}
			if (resourceGameObject == null)
			{
				UnityEngine.Debug.LogError("PhotonNetwork error: Could not Instantiate the prefab [" + text + "]. Please verify you have this gameobject in a Resources folder.");
				return null;
			}
		}
		PhotonView[] photonViewsInChildren = resourceGameObject.GetPhotonViewsInChildren();
		if (photonViewsInChildren.Length != array.Length)
		{
			throw new Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
		}
		for (int i = 0; i < array.Length; i++)
		{
			photonViewsInChildren[i].viewID = array[i];
			photonViewsInChildren[i].prefix = (int)prefix;
			photonViewsInChildren[i].instantiationId = num;
		}
		this.StoreInstantiationData(num, instantiationData);
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(resourceGameObject, position, rotation);
		for (int j = 0; j < array.Length; j++)
		{
			photonViewsInChildren[j].viewID = 0;
			photonViewsInChildren[j].prefix = -1;
			photonViewsInChildren[j].prefixBackup = -1;
			photonViewsInChildren[j].instantiationId = -1;
		}
		this.RemoveInstantiationData(num);
		if (this.instantiatedObjects.ContainsKey(num))
		{
			GameObject gameObject2 = this.instantiatedObjects[num];
			string text2 = string.Empty;
			if (gameObject2 != null)
			{
				PhotonView[] photonViewsInChildren2 = gameObject2.GetPhotonViewsInChildren();
				foreach (PhotonView photonView in photonViewsInChildren2)
				{
					if (!(photonView == null))
					{
						text2 = text2 + photonView.ToString() + ", ";
					}
				}
			}
			UnityEngine.Debug.LogError(string.Format("DoInstantiate re-defines a GameObject. Destroying old entry! New: '{0}' (instantiationID: {1}) Old: {3}. PhotonViews on old: {4}. instantiatedObjects.Count: {2}. PhotonNetwork.lastUsedViewSubId: {5} PhotonNetwork.lastUsedViewSubIdStatic: {6} this.photonViewList.Count {7}.)", new object[]
			{
				gameObject,
				num,
				this.instantiatedObjects.Count,
				gameObject2,
				text2,
				PhotonNetwork.lastUsedViewSubId,
				PhotonNetwork.lastUsedViewSubIdStatic,
				this.photonViewList.Count
			}));
			this.RemoveInstantiatedGO(gameObject2, true);
		}
		this.instantiatedObjects.Add(num, gameObject);
		object[] parameters = new object[]
		{
			new PhotonMessageInfo(photonPlayer, timestamp, null)
		};
		foreach (MonoBehaviour monoBehaviour in gameObject.GetComponentsInChildren<MonoBehaviour>())
		{
			MethodInfo methodInfo;
			if (NetworkingPeer.GetMethod(monoBehaviour, PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), out methodInfo))
			{
				object obj = methodInfo.Invoke(monoBehaviour, parameters);
				if (methodInfo.ReturnType == typeof(IEnumerator))
				{
					PhotonHandler.SP.StartCoroutine((IEnumerator)obj);
				}
			}
		}
		return gameObject;
	}

	private void StoreInstantiationData(int instantiationId, object[] instantiationData)
	{
		this.tempInstantiationData[instantiationId] = instantiationData;
	}

	public object[] FetchInstantiationData(int instantiationId)
	{
		object[] result = null;
		if (instantiationId == 0)
		{
			return null;
		}
		this.tempInstantiationData.TryGetValue(instantiationId, out result);
		return result;
	}

	private void RemoveInstantiationData(int instantiationId)
	{
		this.tempInstantiationData.Remove(instantiationId);
	}

	public void RemoveAllInstantiatedObjects()
	{
		GameObject[] array = new GameObject[this.instantiatedObjects.Count];
		this.instantiatedObjects.Values.CopyTo(array, 0);
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject == null))
			{
				this.RemoveInstantiatedGO(gameObject, false);
			}
		}
		if (this.instantiatedObjects.Count > 0)
		{
			UnityEngine.Debug.LogError("RemoveAllInstantiatedObjects() this.instantiatedObjects.Count should be 0 by now.");
		}
		this.instantiatedObjects = new Dictionary<int, GameObject>();
	}

	public void DestroyPlayerObjects(int playerId, bool localOnly)
	{
		if (playerId <= 0)
		{
			UnityEngine.Debug.LogError("Failed to Destroy objects of playerId: " + playerId);
			return;
		}
		if (!localOnly)
		{
			this.OpRemoveFromServerInstantiationsOfPlayer(playerId);
			this.OpCleanRpcBuffer(playerId);
			this.SendDestroyOfPlayer(playerId);
		}
		Queue<GameObject> queue = new Queue<GameObject>();
		int num = playerId * PhotonNetwork.MAX_VIEW_IDS;
		int num2 = num + PhotonNetwork.MAX_VIEW_IDS;
		foreach (KeyValuePair<int, GameObject> keyValuePair in this.instantiatedObjects)
		{
			if (keyValuePair.Key > num && keyValuePair.Key < num2)
			{
				queue.Enqueue(keyValuePair.Value);
			}
		}
		foreach (GameObject go in queue)
		{
			this.RemoveInstantiatedGO(go, true);
		}
	}

	public void DestroyAll(bool localOnly)
	{
		if (!localOnly)
		{
			this.OpRemoveCompleteCache();
			this.SendDestroyOfAll();
		}
		this.LocalCleanupAnythingInstantiated(true);
	}

	public void RemoveInstantiatedGO(GameObject go, bool localOnly)
	{
		if (go == null)
		{
			if (base.DebugOut == DebugLevel.ERROR)
			{
				this.DebugReturn(DebugLevel.ERROR, "Failed to 'network-remove' GameObject because it's null.");
			}
			return;
		}
		PhotonView[] componentsInChildren = go.GetComponentsInChildren<PhotonView>();
		if (componentsInChildren == null || componentsInChildren.Length <= 0)
		{
			if (base.DebugOut == DebugLevel.ERROR)
			{
				this.DebugReturn(DebugLevel.ERROR, "Failed to 'network-remove' GameObject because has no PhotonView components: " + go);
			}
			return;
		}
		PhotonView photonView = componentsInChildren[0];
		int ownerActorNr = photonView.OwnerActorNr;
		int instantiationId = photonView.instantiationId;
		if (!localOnly && !photonView.isMine && (!this.mLocalActor.isMasterClient || this.mActors.ContainsKey(ownerActorNr)))
		{
			if (base.DebugOut == DebugLevel.ERROR)
			{
				this.DebugReturn(DebugLevel.ERROR, "Failed to 'network-remove' GameObject. Client is neither owner nor masterClient taking over for owner who left: " + photonView);
			}
			return;
		}
		if (instantiationId < 1)
		{
			if (base.DebugOut == DebugLevel.ERROR)
			{
				this.DebugReturn(DebugLevel.ERROR, "Failed to 'network-remove' GameObject because it is missing a valid InstantiationId on view: " + photonView + ". Not Destroying GameObject or PhotonViews!");
			}
			return;
		}
		if (!localOnly)
		{
			this.ServerCleanInstantiateAndDestroy(instantiationId, ownerActorNr);
		}
		this.instantiatedObjects.Remove(instantiationId);
		for (int i = componentsInChildren.Length - 1; i >= 0; i--)
		{
			PhotonView photonView2 = componentsInChildren[i];
			if (!(photonView2 == null))
			{
				if (photonView2.instantiationId >= 1)
				{
					this.LocalCleanPhotonView(photonView2);
				}
				if (!localOnly)
				{
					this.OpCleanRpcBuffer(photonView2);
				}
			}
		}
		if (base.DebugOut >= DebugLevel.ALL)
		{
			this.DebugReturn(DebugLevel.ALL, "Network destroy Instantiated GO: " + go.name);
		}
		UnityEngine.Object.Destroy(go);
	}

	public int GetInstantiatedObjectsId(GameObject go)
	{
		int result = -1;
		if (go == null)
		{
			this.DebugReturn(DebugLevel.ERROR, "GetInstantiatedObjectsId() for GO == null.");
			return result;
		}
		PhotonView[] photonViewsInChildren = go.GetPhotonViewsInChildren();
		if (photonViewsInChildren != null && photonViewsInChildren.Length > 0 && photonViewsInChildren[0] != null)
		{
			return photonViewsInChildren[0].instantiationId;
		}
		if (base.DebugOut == DebugLevel.ALL)
		{
			this.DebugReturn(DebugLevel.ALL, "GetInstantiatedObjectsId failed for GO: " + go);
		}
		return result;
	}

	private void ServerCleanInstantiateAndDestroy(int instantiateId, int actorNr)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[7] = instantiateId;
		this.OpRaiseEvent(202, hashtable, true, 0, new int[]
		{
			actorNr
		}, EventCaching.RemoveFromRoomCache);
		ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
		hashtable2[0] = instantiateId;
		this.OpRaiseEvent(204, hashtable2, true, 0, EventCaching.DoNotCache, ReceiverGroup.Others);
	}

	private void SendDestroyOfPlayer(int actorNr)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[0] = actorNr;
		this.OpRaiseEvent(207, hashtable, true, 0, EventCaching.DoNotCache, ReceiverGroup.Others);
	}

	private void SendDestroyOfAll()
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[0] = -1;
		this.OpRaiseEvent(207, hashtable, true, 0, EventCaching.DoNotCache, ReceiverGroup.Others);
	}

	private void OpRemoveFromServerInstantiationsOfPlayer(int actorNr)
	{
		this.OpRaiseEvent(202, null, true, 0, new int[]
		{
			actorNr
		}, EventCaching.RemoveFromRoomCache);
	}

	public void LocalCleanPhotonView(PhotonView view)
	{
		view.destroyedByPhotonNetworkOrQuit = true;
		this.photonViewList.Remove(view.viewID);
	}

	public PhotonView GetPhotonView(int viewID)
	{
		PhotonView photonView = null;
		this.photonViewList.TryGetValue(viewID, out photonView);
		if (photonView == null)
		{
			PhotonView[] array = UnityEngine.Object.FindObjectsOfType(typeof(PhotonView)) as PhotonView[];
			foreach (PhotonView photonView2 in array)
			{
				if (photonView2.viewID == viewID)
				{
					UnityEngine.Debug.LogWarning("Had to lookup view that wasn't in dict: " + photonView2);
					return photonView2;
				}
			}
		}
		return photonView;
	}

	public void RegisterPhotonView(PhotonView netView)
	{
		if (!Application.isPlaying)
		{
			this.photonViewList = new Dictionary<int, PhotonView>();
			return;
		}
		if (netView.subId == 0)
		{
			return;
		}
		if (this.photonViewList.ContainsKey(netView.viewID))
		{
			if (netView != this.photonViewList[netView.viewID])
			{
				UnityEngine.Debug.LogError(string.Format("PhotonView ID duplicate found: {0}. New: {1} old: {2}. Maybe one wasn't destroyed on scene load?! Check for 'DontDestroyOnLoad'. Destroying old entry, adding new.", netView.viewID, netView, this.photonViewList[netView.viewID]));
			}
			this.RemoveInstantiatedGO(this.photonViewList[netView.viewID].gameObject, true);
		}
		this.photonViewList.Add(netView.viewID, netView);
		if (base.DebugOut >= DebugLevel.ALL)
		{
			this.DebugReturn(DebugLevel.ALL, "Registered PhotonView: " + netView.viewID);
		}
	}

	public void OpCleanRpcBuffer(int actorNumber)
	{
		this.OpRaiseEvent(200, null, true, 0, new int[]
		{
			actorNumber
		}, EventCaching.RemoveFromRoomCache);
	}

	public void OpRemoveCompleteCacheOfPlayer(int actorNumber)
	{
		this.OpRaiseEvent(0, null, true, 0, new int[]
		{
			actorNumber
		}, EventCaching.RemoveFromRoomCache);
	}

	public void OpRemoveCompleteCache()
	{
		this.OpRaiseEvent(0, null, true, 0, EventCaching.RemoveFromRoomCache, ReceiverGroup.MasterClient);
	}

	private void RemoveCacheOfLeftPlayers()
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[244] = 0;
		dictionary[247] = 7;
		this.OpCustom(253, dictionary, true, 0);
	}

	public void CleanRpcBufferIfMine(PhotonView view)
	{
		if (view.ownerId != this.mLocalActor.ID && !this.mLocalActor.isMasterClient)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Cannot remove cached RPCs on a PhotonView thats not ours! ",
				view.owner,
				" scene: ",
				view.isSceneView
			}));
			return;
		}
		this.OpCleanRpcBuffer(view);
	}

	public void OpCleanRpcBuffer(PhotonView view)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[0] = view.viewID;
		this.OpRaiseEvent(200, hashtable, true, 0, EventCaching.RemoveFromRoomCache, ReceiverGroup.Others);
	}

	public void RemoveRPCsInGroup(int group)
	{
		foreach (KeyValuePair<int, PhotonView> keyValuePair in this.photonViewList)
		{
			PhotonView value = keyValuePair.Value;
			if (value.group == group)
			{
				this.CleanRpcBufferIfMine(value);
			}
		}
	}

	public void SetLevelPrefix(short prefix)
	{
		this.currentLevelPrefix = prefix;
	}

	internal void RPC(PhotonView view, string methodName, PhotonPlayer player, params object[] parameters)
	{
		if (this.blockSendingGroups.Contains(view.group))
		{
			return;
		}
		if (view.viewID < 1)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Illegal view ID:",
				view.viewID,
				" method: ",
				methodName,
				" GO:",
				view.gameObject.name
			}));
		}
		if (base.DebugOut >= DebugLevel.INFO)
		{
			this.DebugReturn(DebugLevel.INFO, string.Concat(new object[]
			{
				"Sending RPC \"",
				methodName,
				"\" to player[",
				player,
				"]"
			}));
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[0] = view.viewID;
		if (view.prefix > 0)
		{
			hashtable[1] = (short)view.prefix;
		}
		hashtable[2] = base.ServerTimeInMilliSeconds;
		int num = 0;
		if (this.rpcShortcuts.TryGetValue(methodName, out num))
		{
			hashtable[5] = (byte)num;
		}
		else
		{
			hashtable[3] = methodName;
		}
		if (parameters != null && parameters.Length > 0)
		{
			hashtable[4] = parameters;
		}
		if (this.mLocalActor == player)
		{
			this.ExecuteRPC(hashtable, player);
		}
		else
		{
			int[] targetActors = new int[]
			{
				player.ID
			};
			this.OpRaiseEvent(200, hashtable, true, 0, targetActors);
		}
	}

	internal void RPC(PhotonView view, string methodName, PhotonTargets target, params object[] parameters)
	{
		if (this.blockSendingGroups.Contains(view.group))
		{
			return;
		}
		if (view.viewID < 1)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Illegal view ID:",
				view.viewID,
				" method: ",
				methodName,
				" GO:",
				view.gameObject.name
			}));
		}
		if (base.DebugOut >= DebugLevel.INFO)
		{
			this.DebugReturn(DebugLevel.INFO, string.Concat(new object[]
			{
				"Sending RPC \"",
				methodName,
				"\" to ",
				target
			}));
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[0] = view.viewID;
		if (view.prefix > 0)
		{
			hashtable[1] = (short)view.prefix;
		}
		hashtable[2] = base.ServerTimeInMilliSeconds;
		int num = 0;
		if (this.rpcShortcuts.TryGetValue(methodName, out num))
		{
			hashtable[5] = (byte)num;
		}
		else
		{
			hashtable[3] = methodName;
		}
		if (parameters != null && parameters.Length > 0)
		{
			hashtable[4] = parameters;
		}
		if (target == PhotonTargets.All)
		{
			this.OpRaiseEvent(200, (byte)view.group, hashtable, true, 0);
			this.ExecuteRPC(hashtable, this.mLocalActor);
		}
		else if (target == PhotonTargets.Others)
		{
			this.OpRaiseEvent(200, (byte)view.group, hashtable, true, 0);
		}
		else if (target == PhotonTargets.AllBuffered)
		{
			this.OpRaiseEvent(200, hashtable, true, 0, EventCaching.AddToRoomCache, ReceiverGroup.Others);
			this.ExecuteRPC(hashtable, this.mLocalActor);
		}
		else if (target == PhotonTargets.OthersBuffered)
		{
			this.OpRaiseEvent(200, hashtable, true, 0, EventCaching.AddToRoomCache, ReceiverGroup.Others);
		}
		else if (target == PhotonTargets.MasterClient)
		{
			if (this.mMasterClient == this.mLocalActor)
			{
				this.ExecuteRPC(hashtable, this.mLocalActor);
			}
			else
			{
				this.OpRaiseEvent(200, hashtable, true, 0, EventCaching.DoNotCache, ReceiverGroup.MasterClient);
			}
		}
		else
		{
			UnityEngine.Debug.LogError("Unsupported target enum: " + target);
		}
	}

	public void SetReceivingEnabled(int group, bool enabled)
	{
		if (group <= 0)
		{
			UnityEngine.Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + group + ". The group number should be at least 1.");
			return;
		}
		if (enabled)
		{
			if (!this.allowedReceivingGroups.Contains(group))
			{
				this.allowedReceivingGroups.Add(group);
				byte[] groupsToAdd = new byte[]
				{
					(byte)group
				};
				this.OpChangeGroups(null, groupsToAdd);
			}
		}
		else if (this.allowedReceivingGroups.Contains(group))
		{
			this.allowedReceivingGroups.Remove(group);
			byte[] groupsToRemove = new byte[]
			{
				(byte)group
			};
			this.OpChangeGroups(groupsToRemove, null);
		}
	}

	public void SetSendingEnabled(int group, bool enabled)
	{
		if (!enabled)
		{
			this.blockSendingGroups.Add(group);
		}
		else
		{
			this.blockSendingGroups.Remove(group);
		}
	}

	public void NewSceneLoaded()
	{
		if (this.loadingLevelAndPausedNetwork && !PhotonNetwork.isMessageQueueRunning)
		{
			this.loadingLevelAndPausedNetwork = false;
			PhotonNetwork.isMessageQueueRunning = true;
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, PhotonView> keyValuePair in this.photonViewList)
		{
			PhotonView value = keyValuePair.Value;
			if (value == null)
			{
				list.Add(keyValuePair.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			int key = list[i];
			this.photonViewList.Remove(key);
		}
		if (list.Count > 0 && base.DebugOut >= DebugLevel.INFO)
		{
			this.DebugReturn(DebugLevel.INFO, "Removed " + list.Count + " scene view IDs from last scene.");
		}
	}

	public void RunViewUpdate()
	{
		if (!PhotonNetwork.connected || PhotonNetwork.offlineMode)
		{
			return;
		}
		if (this.mActors == null || this.mActors.Count <= 1)
		{
			return;
		}
		Dictionary<int, ExitGames.Client.Photon.Hashtable> dictionary = new Dictionary<int, ExitGames.Client.Photon.Hashtable>();
		Dictionary<int, ExitGames.Client.Photon.Hashtable> dictionary2 = new Dictionary<int, ExitGames.Client.Photon.Hashtable>();
		foreach (KeyValuePair<int, PhotonView> keyValuePair in this.photonViewList)
		{
			PhotonView value = keyValuePair.Value;
			if (value.observed != null && value.synchronization != ViewSynchronization.Off)
			{
				if (value.owner == this.mLocalActor || (value.isSceneView && this.mMasterClient == this.mLocalActor))
				{
					if (value.gameObject.activeInHierarchy)
					{
						if (!this.blockSendingGroups.Contains(value.group))
						{
							ExitGames.Client.Photon.Hashtable hashtable = this.OnSerializeWrite(value);
							if (hashtable != null)
							{
								if (value.synchronization == ViewSynchronization.ReliableDeltaCompressed || value.mixedModeIsReliable)
								{
									if (hashtable.ContainsKey(1) || hashtable.ContainsKey(2))
									{
										if (!dictionary.ContainsKey(value.group))
										{
											dictionary[value.group] = new ExitGames.Client.Photon.Hashtable();
											dictionary[value.group][0] = base.ServerTimeInMilliSeconds;
											if (this.currentLevelPrefix >= 0)
											{
												dictionary[value.group][1] = this.currentLevelPrefix;
											}
										}
										ExitGames.Client.Photon.Hashtable hashtable2 = dictionary[value.group];
										hashtable2.Add((short)hashtable2.Count, hashtable);
									}
								}
								else
								{
									if (!dictionary2.ContainsKey(value.group))
									{
										dictionary2[value.group] = new ExitGames.Client.Photon.Hashtable();
										dictionary2[value.group][0] = base.ServerTimeInMilliSeconds;
										if (this.currentLevelPrefix >= 0)
										{
											dictionary2[value.group][1] = this.currentLevelPrefix;
										}
									}
									ExitGames.Client.Photon.Hashtable hashtable3 = dictionary2[value.group];
									hashtable3.Add((short)hashtable3.Count, hashtable);
								}
							}
						}
					}
				}
			}
		}
		foreach (KeyValuePair<int, ExitGames.Client.Photon.Hashtable> keyValuePair2 in dictionary)
		{
			this.OpRaiseEvent(206, (byte)keyValuePair2.Key, keyValuePair2.Value, true, 0);
		}
		foreach (KeyValuePair<int, ExitGames.Client.Photon.Hashtable> keyValuePair3 in dictionary2)
		{
			this.OpRaiseEvent(201, (byte)keyValuePair3.Key, keyValuePair3.Value, false, 0);
		}
	}

	private ExitGames.Client.Photon.Hashtable OnSerializeWrite(PhotonView view)
	{
		List<object> list = new List<object>();
		if (view.observed is MonoBehaviour)
		{
			PhotonStream photonStream = new PhotonStream(true, null);
			PhotonMessageInfo info = new PhotonMessageInfo(this.mLocalActor, base.ServerTimeInMilliSeconds, view);
			view.ExecuteOnSerialize(photonStream, info);
			if (photonStream.Count == 0)
			{
				return null;
			}
			list = photonStream.data;
		}
		else if (view.observed is Transform)
		{
			Transform transform = (Transform)view.observed;
			if (view.onSerializeTransformOption == OnSerializeTransform.OnlyPosition || view.onSerializeTransformOption == OnSerializeTransform.PositionAndRotation || view.onSerializeTransformOption == OnSerializeTransform.All)
			{
				list.Add(transform.localPosition);
			}
			else
			{
				list.Add(null);
			}
			if (view.onSerializeTransformOption == OnSerializeTransform.OnlyRotation || view.onSerializeTransformOption == OnSerializeTransform.PositionAndRotation || view.onSerializeTransformOption == OnSerializeTransform.All)
			{
				list.Add(transform.localRotation);
			}
			else
			{
				list.Add(null);
			}
			if (view.onSerializeTransformOption == OnSerializeTransform.OnlyScale || view.onSerializeTransformOption == OnSerializeTransform.All)
			{
				list.Add(transform.localScale);
			}
		}
		else
		{
			if (!(view.observed is Rigidbody))
			{
				UnityEngine.Debug.LogError("Observed type is not serializable: " + view.observed.GetType());
				return null;
			}
			Rigidbody rigidbody = (Rigidbody)view.observed;
			if (view.onSerializeRigidBodyOption != OnSerializeRigidBody.OnlyAngularVelocity)
			{
				list.Add(rigidbody.velocity);
			}
			else
			{
				list.Add(null);
			}
			if (view.onSerializeRigidBodyOption != OnSerializeRigidBody.OnlyVelocity)
			{
				list.Add(rigidbody.angularVelocity);
			}
		}
		object[] array = list.ToArray();
		if (view.synchronization == ViewSynchronization.UnreliableOnChange)
		{
			if (this.AlmostEquals(array, view.lastOnSerializeDataSent))
			{
				if (view.mixedModeIsReliable)
				{
					return null;
				}
				view.mixedModeIsReliable = true;
				view.lastOnSerializeDataSent = array;
			}
			else
			{
				view.mixedModeIsReliable = false;
				view.lastOnSerializeDataSent = array;
			}
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[0] = view.viewID;
		hashtable[1] = array;
		if (view.synchronization == ViewSynchronization.ReliableDeltaCompressed)
		{
			bool flag = this.DeltaCompressionWrite(view, hashtable);
			view.lastOnSerializeDataSent = array;
			if (!flag)
			{
				return null;
			}
		}
		return hashtable;
	}

	private void OnSerializeRead(ExitGames.Client.Photon.Hashtable data, PhotonPlayer sender, int networkTime, short correctPrefix)
	{
		int num = (int)data[0];
		PhotonView photonView = this.GetPhotonView(num);
		if (photonView == null)
		{
			UnityEngine.Debug.LogWarning(string.Concat(new object[]
			{
				"Received OnSerialization for view ID ",
				num,
				". We have no such PhotonView! Ignored this if you're leaving a room. State: ",
				this.State
			}));
			return;
		}
		if (photonView.prefix > 0 && (int)correctPrefix != photonView.prefix)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Received OnSerialization for view ID ",
				num,
				" with prefix ",
				correctPrefix,
				". Our prefix is ",
				photonView.prefix
			}));
			return;
		}
		if (photonView.group != 0 && !this.allowedReceivingGroups.Contains(photonView.group))
		{
			return;
		}
		if (photonView.synchronization == ViewSynchronization.ReliableDeltaCompressed)
		{
			if (!this.DeltaCompressionRead(photonView, data))
			{
				this.DebugReturn(DebugLevel.INFO, string.Concat(new object[]
				{
					"Skipping packet for ",
					photonView.name,
					" [",
					photonView.viewID,
					"] as we haven't received a full packet for delta compression yet. This is OK if it happens for the first few frames after joining a game."
				}));
				return;
			}
			photonView.lastOnSerializeDataReceived = (data[1] as object[]);
		}
		if (photonView.observed is MonoBehaviour)
		{
			object[] incomingData = data[1] as object[];
			PhotonStream pStream = new PhotonStream(false, incomingData);
			PhotonMessageInfo info = new PhotonMessageInfo(sender, networkTime, photonView);
			photonView.ExecuteOnSerialize(pStream, info);
		}
		else if (photonView.observed is Transform)
		{
			object[] array = data[1] as object[];
			Transform transform = (Transform)photonView.observed;
			if (array.Length >= 1 && array[0] != null)
			{
				transform.localPosition = (Vector3)array[0];
			}
			if (array.Length >= 2 && array[1] != null)
			{
				transform.localRotation = (Quaternion)array[1];
			}
			if (array.Length >= 3 && array[2] != null)
			{
				transform.localScale = (Vector3)array[2];
			}
		}
		else if (photonView.observed is Rigidbody)
		{
			object[] array2 = data[1] as object[];
			Rigidbody rigidbody = (Rigidbody)photonView.observed;
			if (array2.Length >= 1 && array2[0] != null)
			{
				rigidbody.velocity = (Vector3)array2[0];
			}
			if (array2.Length >= 2 && array2[1] != null)
			{
				rigidbody.angularVelocity = (Vector3)array2[1];
			}
		}
		else
		{
			UnityEngine.Debug.LogError("Type of observed is unknown when receiving.");
		}
	}

	private bool AlmostEquals(object[] lastData, object[] currentContent)
	{
		if (lastData == null && currentContent == null)
		{
			return true;
		}
		if (lastData == null || currentContent == null || lastData.Length != currentContent.Length)
		{
			return false;
		}
		for (int i = 0; i < currentContent.Length; i++)
		{
			object one = currentContent[i];
			object two = lastData[i];
			if (!this.ObjectIsSameWithInprecision(one, two))
			{
				return false;
			}
		}
		return true;
	}

	private bool DeltaCompressionWrite(PhotonView view, ExitGames.Client.Photon.Hashtable data)
	{
		if (view.lastOnSerializeDataSent == null)
		{
			return true;
		}
		object[] lastOnSerializeDataSent = view.lastOnSerializeDataSent;
		object[] array = data[1] as object[];
		if (array == null)
		{
			return false;
		}
		if (lastOnSerializeDataSent.Length != array.Length)
		{
			return true;
		}
		object[] array2 = new object[array.Length];
		int num = 0;
		List<int> list = new List<int>();
		for (int i = 0; i < array2.Length; i++)
		{
			object obj = array[i];
			object two = lastOnSerializeDataSent[i];
			if (this.ObjectIsSameWithInprecision(obj, two))
			{
				num++;
			}
			else
			{
				array2[i] = array[i];
				if (obj == null)
				{
					list.Add(i);
				}
			}
		}
		if (num > 0)
		{
			data.Remove(1);
			if (num == array.Length)
			{
				return false;
			}
			data[2] = array2;
			if (list.Count > 0)
			{
				data[3] = list.ToArray();
			}
		}
		return true;
	}

	private bool DeltaCompressionRead(PhotonView view, ExitGames.Client.Photon.Hashtable data)
	{
		if (data.ContainsKey(1))
		{
			return true;
		}
		if (view.lastOnSerializeDataReceived == null)
		{
			return false;
		}
		object[] array = data[2] as object[];
		if (array == null)
		{
			return false;
		}
		int[] array2 = data[3] as int[];
		if (array2 == null)
		{
			array2 = new int[0];
		}
		object[] lastOnSerializeDataReceived = view.lastOnSerializeDataReceived;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == null && !array2.Contains(i))
			{
				object obj = lastOnSerializeDataReceived[i];
				array[i] = obj;
			}
		}
		data[1] = array;
		return true;
	}

	private bool ObjectIsSameWithInprecision(object one, object two)
	{
		if (one == null || two == null)
		{
			return one == null && two == null;
		}
		if (!one.Equals(two))
		{
			if (one is Vector3)
			{
				Vector3 target = (Vector3)one;
				Vector3 second = (Vector3)two;
				if (target.AlmostEquals(second, PhotonNetwork.precisionForVectorSynchronization))
				{
					return true;
				}
			}
			else if (one is Vector2)
			{
				Vector2 target2 = (Vector2)one;
				Vector2 second2 = (Vector2)two;
				if (target2.AlmostEquals(second2, PhotonNetwork.precisionForVectorSynchronization))
				{
					return true;
				}
			}
			else if (one is Quaternion)
			{
				Quaternion target3 = (Quaternion)one;
				Quaternion second3 = (Quaternion)two;
				if (target3.AlmostEquals(second3, PhotonNetwork.precisionForQuaternionSynchronization))
				{
					return true;
				}
			}
			else if (one is float)
			{
				float target4 = (float)one;
				float second4 = (float)two;
				if (target4.AlmostEquals(second4, PhotonNetwork.precisionForFloatSynchronization))
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	protected internal static bool GetMethod(MonoBehaviour monob, string methodType, out MethodInfo mi)
	{
		mi = null;
		if (monob == null || string.IsNullOrEmpty(methodType))
		{
			return false;
		}
		List<MethodInfo> methods = SupportClass.GetMethods(monob.GetType(), null);
		for (int i = 0; i < methods.Count; i++)
		{
			MethodInfo methodInfo = methods[i];
			if (methodInfo.Name.Equals(methodType))
			{
				mi = methodInfo;
				return true;
			}
		}
		return false;
	}

	protected internal void AutomaticallySyncScene()
	{
		if (PhotonNetwork.room != null && PhotonNetwork.automaticallySyncScene && !PhotonNetwork.isMasterClient)
		{
			string text = (string)PhotonNetwork.room.customProperties["curScn"];
			if (!string.IsNullOrEmpty(text))
			{
				if (text != UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
				{
					PhotonNetwork.LoadLevel(text);
				}
				else if (base.DebugOut >= DebugLevel.WARNING)
				{
					this.DebugReturn(DebugLevel.WARNING, "Skipped re-loading level due to scene syncing. Level already loaded.");
				}
			}
		}
	}

	protected internal const string CurrentSceneProperty = "curScn";

	public string mAppVersion;

	private string mAppId;

	private string playername = string.Empty;

	private IPhotonPeerListener externalListener;

	private JoinType mLastJoinType;

	private bool mPlayernameHasToBeUpdated;

	public Dictionary<int, PhotonPlayer> mActors = new Dictionary<int, PhotonPlayer>();

	public PhotonPlayer[] mOtherPlayerListCopy = new PhotonPlayer[0];

	public PhotonPlayer[] mPlayerListCopy = new PhotonPlayer[0];

	public PhotonPlayer mMasterClient;

	public bool hasSwitchedMC;

	public bool requestSecurity = true;

	private Dictionary<Type, List<MethodInfo>> monoRPCMethodsCache = new Dictionary<Type, List<MethodInfo>>();

	public static bool UsePrefabCache = true;

	public static Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();

	public Dictionary<string, RoomInfo> mGameList = new Dictionary<string, RoomInfo>();

	public RoomInfo[] mGameListCopy = new RoomInfo[0];

	public bool insideLobby;

	public Dictionary<int, GameObject> instantiatedObjects = new Dictionary<int, GameObject>();

	private HashSet<int> allowedReceivingGroups = new HashSet<int>();

	private HashSet<int> blockSendingGroups = new HashSet<int>();

	protected internal Dictionary<int, PhotonView> photonViewList = new Dictionary<int, PhotonView>();

	protected internal short currentLevelPrefix;

	private readonly Dictionary<string, int> rpcShortcuts;

	private int friendListTimestamp;

	private bool isFetchingFriends;

	private Dictionary<int, object[]> tempInstantiationData = new Dictionary<int, object[]>();

	protected internal bool loadingLevelAndPausedNetwork;
}
