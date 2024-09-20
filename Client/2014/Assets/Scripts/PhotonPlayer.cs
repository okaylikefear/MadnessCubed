using System;
using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonPlayer
{
	public PhotonPlayer(bool isLocal, int actorID, string name)
	{
		this.customProperties = new Hashtable();
		this.isLocal = isLocal;
		this.actorID = actorID;
		this.nameField = name;
	}

	protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
	{
		this.customProperties = new Hashtable();
		this.isLocal = isLocal;
		this.actorID = actorID;
		this.InternalCacheProperties(properties);
	}

	public int ID
	{
		get
		{
			return this.actorID;
		}
	}

	public string name
	{
		get
		{
			return this.nameField;
		}
		set
		{
			if (!this.isLocal)
			{
				UnityEngine.Debug.LogError("Error: Cannot change the name of a remote player!");
				return;
			}
			this.nameField = value;
		}
	}

	public bool isMasterClient
	{
		get
		{
			return PhotonNetwork.networkingPeer.mMasterClient == this;
		}
	}

	public Hashtable customProperties { get; private set; }

	public Hashtable allProperties
	{
		get
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Merge(this.customProperties);
			hashtable[byte.MaxValue] = this.name;
			return hashtable;
		}
	}

	internal void InternalCacheProperties(Hashtable properties)
	{
		if (properties == null || properties.Count == 0 || this.customProperties.Equals(properties))
		{
			return;
		}
		if (properties.ContainsKey(255))
		{
			this.nameField = (string)properties[byte.MaxValue];
		}
		this.customProperties.MergeStringKeys(properties);
		this.customProperties.StripKeysWithNullValues();
	}

	public override string ToString()
	{
		return (this.name != null) ? this.name : string.Empty;
	}

	public override bool Equals(object p)
	{
		PhotonPlayer photonPlayer = p as PhotonPlayer;
		return photonPlayer != null && this.GetHashCode() == photonPlayer.GetHashCode();
	}

	public override int GetHashCode()
	{
		return this.ID;
	}

	internal void InternalChangeLocalID(int newID)
	{
		if (!this.isLocal)
		{
			UnityEngine.Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
			return;
		}
		this.actorID = newID;
	}

	public void SetCustomProperties(Hashtable propertiesToSet)
	{
		if (propertiesToSet == null)
		{
			return;
		}
		this.customProperties.MergeStringKeys(propertiesToSet);
		this.customProperties.StripKeysWithNullValues();
		Hashtable actorProperties = propertiesToSet.StripToStringKeys();
		if (this.actorID > 0)
		{
			PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfActor(this.actorID, actorProperties, true, 0);
		}
		NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, new object[]
		{
			this
		});
	}

	public static PhotonPlayer Find(int ID)
	{
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			PhotonPlayer photonPlayer = PhotonNetwork.playerList[i];
			if (photonPlayer.ID == ID)
			{
				return photonPlayer;
			}
		}
		return null;
	}

	private int actorID = -1;

	private string nameField = string.Empty;

	public readonly bool isLocal;
}
