using System;

public enum PeerState
{
	Uninitialized,
	PeerCreated,
	Connecting,
	Connected,
	Queued,
	Authenticated,
	JoinedLobby,
	DisconnectingFromMasterserver,
	ConnectingToGameserver,
	ConnectedToGameserver,
	Joining,
	Joined,
	Leaving,
	DisconnectingFromGameserver,
	ConnectingToMasterserver,
	ConnectedComingFromGameserver,
	QueuedComingFromGameserver,
	Disconnecting,
	Disconnected,
	ConnectedToMaster
}
