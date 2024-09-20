using System;
using ExitGames.Client.Photon;
using UnityEngine;

public class Room : RoomInfo
{
	internal Room(string roomName, Hashtable properties) : base(roomName, properties)
	{
		this.propertiesListedInLobby = new string[0];
	}

	internal Room(string roomName, Hashtable properties, bool isVisible, bool isOpen, int maxPlayers, bool autoCleanUp, string[] propsListedInLobby) : base(roomName, properties)
	{
		this.visibleField = isVisible;
		this.openField = isOpen;
		this.autoCleanUpField = autoCleanUp;
		if (maxPlayers > 255)
		{
			UnityEngine.Debug.LogError("Error: Room() called with " + maxPlayers + " maxplayers. This has been reverted to the max of 255 players, because internally a 'byte' is used.");
			maxPlayers = 255;
		}
		this.maxPlayersField = (byte)maxPlayers;
		if (propsListedInLobby != null)
		{
			this.propertiesListedInLobby = propsListedInLobby;
		}
		else
		{
			this.propertiesListedInLobby = new string[0];
		}
	}

	public new int playerCount
	{
		get
		{
			if (PhotonNetwork.playerList != null)
			{
				return PhotonNetwork.playerList.Length;
			}
			return 0;
		}
	}

	public new string name
	{
		get
		{
			return this.nameField;
		}
		internal set
		{
			this.nameField = value;
		}
	}

	public new int maxPlayers
	{
		get
		{
			return (int)this.maxPlayersField;
		}
		set
		{
			if (!this.Equals(PhotonNetwork.room))
			{
				PhotonNetwork.networkingPeer.DebugReturn(DebugLevel.WARNING, "Can't set room properties when not in that room.");
			}
			if (value > 255)
			{
				UnityEngine.Debug.LogError("Error: room.maxPlayers called with value " + value + ". This has been reverted to the max of 255 players, because internally a 'byte' is used.");
				value = 255;
			}
			if (value != (int)this.maxPlayersField && !PhotonNetwork.offlineMode)
			{
				PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable
				{
					{
						byte.MaxValue,
						(byte)value
					}
				}, true, 0);
			}
			this.maxPlayersField = (byte)value;
		}
	}

	public new bool open
	{
		get
		{
			return this.openField;
		}
		set
		{
			if (!this.Equals(PhotonNetwork.room))
			{
				PhotonNetwork.networkingPeer.DebugReturn(DebugLevel.WARNING, "Can't set room properties when not in that room.");
			}
			if (value != this.openField && !PhotonNetwork.offlineMode)
			{
				PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable
				{
					{
						253,
						value
					}
				}, true, 0);
			}
			this.openField = value;
		}
	}

	public new bool visible
	{
		get
		{
			return this.visibleField;
		}
		set
		{
			if (!this.Equals(PhotonNetwork.room))
			{
				PhotonNetwork.networkingPeer.DebugReturn(DebugLevel.WARNING, "Can't set room properties when not in that room.");
			}
			if (value != this.visibleField && !PhotonNetwork.offlineMode)
			{
				PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable
				{
					{
						254,
						value
					}
				}, true, 0);
			}
			this.visibleField = value;
		}
	}

	public string[] propertiesListedInLobby { get; private set; }

	public bool autoCleanUp
	{
		get
		{
			return this.autoCleanUpField;
		}
	}

	public void SetCustomProperties(Hashtable propertiesToSet)
	{
		if (propertiesToSet == null)
		{
			return;
		}
		base.customProperties.MergeStringKeys(propertiesToSet);
		base.customProperties.StripKeysWithNullValues();
		Hashtable gameProperties = propertiesToSet.StripToStringKeys();
		PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfRoom(gameProperties, true, 0);
		NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, new object[0]);
	}

	public void SetPropertiesListedInLobby(string[] propsListedInLobby)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[250] = propsListedInLobby;
		PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(hashtable, false, 0);
		this.propertiesListedInLobby = propsListedInLobby;
	}
}
