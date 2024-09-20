using System;
using ExitGames.Client.Photon;

public class RoomInfo
{
	protected internal RoomInfo(string roomName, Hashtable properties)
	{
		this.CacheProperties(properties);
		this.nameField = roomName;
	}

	public bool removedFromList { get; internal set; }

	public Hashtable customProperties
	{
		get
		{
			return this.customPropertiesField;
		}
	}

	public string name
	{
		get
		{
			return this.nameField;
		}
	}

	public int playerCount { get; private set; }

	public bool isLocalClientInside { get; set; }

	public byte maxPlayers
	{
		get
		{
			return this.maxPlayersField;
		}
	}

	public bool open
	{
		get
		{
			return this.openField;
		}
	}

	public bool visible
	{
		get
		{
			return this.visibleField;
		}
	}

	public override bool Equals(object p)
	{
		Room room = p as Room;
		return room != null && this.nameField.Equals(room.nameField);
	}

	public override int GetHashCode()
	{
		return this.nameField.GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("Room: '{0}' visible: {1} open: {2} max: {3} count: {4}\ncustomProps: {5}", new object[]
		{
			this.nameField,
			this.visibleField,
			this.openField,
			this.maxPlayersField,
			this.playerCount,
			this.customPropertiesField.ToStringFull()
		});
	}

	protected internal void CacheProperties(Hashtable propertiesToCache)
	{
		if (propertiesToCache == null || propertiesToCache.Count == 0 || this.customPropertiesField.Equals(propertiesToCache))
		{
			return;
		}
		if (propertiesToCache.ContainsKey(251))
		{
			this.removedFromList = (bool)propertiesToCache[251];
			if (this.removedFromList)
			{
				return;
			}
		}
		if (propertiesToCache.ContainsKey(255))
		{
			this.maxPlayersField = (byte)propertiesToCache[byte.MaxValue];
		}
		if (propertiesToCache.ContainsKey(253))
		{
			this.openField = (bool)propertiesToCache[253];
		}
		if (propertiesToCache.ContainsKey(254))
		{
			this.visibleField = (bool)propertiesToCache[254];
		}
		if (propertiesToCache.ContainsKey(252))
		{
			this.playerCount = (int)((byte)propertiesToCache[252]);
		}
		if (propertiesToCache.ContainsKey(249))
		{
			this.autoCleanUpField = (bool)propertiesToCache[249];
		}
		this.customPropertiesField.MergeStringKeys(propertiesToCache);
	}

	private Hashtable customPropertiesField = new Hashtable();

	protected byte maxPlayersField;

	protected bool openField = true;

	protected bool visibleField = true;

	protected bool autoCleanUpField;

	protected string nameField;
}
