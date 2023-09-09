using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class OnlineMenu : MonoBehaviour
{
	// Note: this type is marked as 'beforefieldinit'.
	static OnlineMenu()
	{
		int[] array = new int[3];
		array[1] = 1;
		OnlineMenu.daylightRemap = array;
	}

	private void Start()
	{
		this._hash = new Dictionary<string, GameObject>();
		for (int i = 0; i < this.modes.Length; i++)
		{
			IndexItem component = this.modes[i].GetComponent<IndexItem>();
			if (component.index > 0)
			{
				this.modes[i].GetComponentInChildren<UILabel>().text = Localize.gameTypeStr[component.index];
			}
			this.modes[i].GetComponent<UIToggle>().onChange.Add(new EventDelegate(new EventDelegate.Callback(this.onGameTypeClick)));
		}
	}

	public void onDayToggle()
	{
		this.dayLight = DayToggle.current.state;
		this.valid = false;
	}

	private void Update()
	{
		if (PhotonNetwork.connected)
		{
			this.onlineLabel.text = string.Format(Localize.onl_players_online, PhotonNetwork.countOfPlayers);
		}
		this.Invalidate();
	}

	public void onGameTypeClick()
	{
		if (!UIToggle.current.value)
		{
			return;
		}
		int num = Array.IndexOf<UIToggle>(this.modes, UIToggle.current);
		if (num == -1)
		{
			return;
		}
		this.currentMode = UIToggle.current.GetComponent<IndexItem>().index;
		this.valid = false;
		this.container.ResetPosition();
	}

	public void onItemClick()
	{
		RoomItem component = UIButton.current.GetComponent<RoomItem>();
		if (component.room.roomPassword != string.Empty)
		{
			OnlineManager.ShowPasswordRequest(component.room);
		}
		else
		{
			OnlineManager.instance.joinRoom(component.room);
		}
	}

	public void onItemClick2()
	{
		RoomItem component = UIButton.current.GetComponent<RoomItem>();
		if (component.room.maxPlayers <= component.room.players)
		{
			return;
		}
		OnlineManager.RoomsInfo[] rooms = OnlineManager.instance.rooms;
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < rooms.Length; j++)
			{
				if (rooms[j].maxPlayers > rooms[j].players)
				{
					if (this._canBreak || rooms[j].mapCanBreak != 1)
					{
						if (component.room.name == rooms[j].name)
						{
							if (rooms[j].roomPassword != string.Empty)
							{
								OnlineManager.ShowPasswordRequest(rooms[j]);
							}
							else
							{
								OnlineManager.instance.joinRoom(rooms[j]);
							}
							return;
						}
						int num = Mathf.Min(Kube.GPS.playerLevel, Localize.RankName.Length - 1);
					}
				}
			}
		}
		ObjectsHolderScript.BuiltInMap builtInMap = Kube.OH.findMapInfo((long)((int)component.room.roomMapNumber));
		if (component.room.maxPlayers >= component.room.players + builtInMap.playersMax)
		{
			OnlineManager.instance.createRoom(component.room, false);
		}
	}

	public void onToggleBreak()
	{
		this._canBreak = !UIToggle.current.value;
		this.valid = false;
	}

	public void onToggleFriends()
	{
		this._onlyFriends = UIToggle.current.value;
		this.valid = false;
	}

	private void OnUpdatedFriendList()
	{
		if (OnlineManager.instance.numGamesWithFriends != this.numGamesWithFriends)
		{
			this.valid = false;
		}
	}

	private void OnReceivedRoomListUpdate()
	{
		if (Time.time > this.fullUpdate)
		{
			this.valid = false;
			this.fullUpdate = Time.time + 30f;
			return;
		}
		OnlineManager.RoomsInfo[] array = this.selectRooms();
		for (int i = 0; i < array.Length; i++)
		{
			if (this._hash.ContainsKey(array[i].name))
			{
				GameObject gameObject = this._hash[array[i].name];
				if (gameObject)
				{
					RoomItem component = gameObject.GetComponent<RoomItem>();
					component.room = array[i];
					UILabel nnplayers = component.nnplayers;
					string text = array[i].players.ToString() + "/" + array[i].maxPlayers;
					component.nnplayers.text = text;
					nnplayers.text = text;
				}
			}
		}
	}

	private void OnEnable()
	{
		OnlineManager.instance.Connect();
		this.Invalidate();
		this.valid = false;
	}

	private OnlineManager.RoomsInfo[] selectRooms()
	{
		if (this._onlyFriends)
		{
			return this.selectRoomsFriends();
		}
		return this.selectRoomsGroup();
	}

	private OnlineManager.RoomsInfo[] selectRoomsFriends()
	{
		List<OnlineManager.RoomsInfo> list = new List<OnlineManager.RoomsInfo>();
		OnlineManager.RoomsInfo[] rooms = OnlineManager.instance.rooms;
		if (rooms == null)
		{
			return null;
		}
		for (int i = 0; i < rooms.Length; i++)
		{
			if (rooms[i].mapCanBreak != 1 || this._canBreak)
			{
				if (rooms[i].roomType != 10 && rooms[i].roomType != 1 && rooms[i].roomType != 5)
				{
					if (this.currentMode == 0 || rooms[i].roomType == this.currentMode)
					{
						if (rooms[i].gameWithFriends)
						{
							list.Add(rooms[i]);
						}
					}
				}
			}
		}
		list.Sort(delegate(OnlineManager.RoomsInfo x, OnlineManager.RoomsInfo y)
		{
			if (x.players == y.players)
			{
				return 0;
			}
			if (x.players < y.players)
			{
				return 1;
			}
			if (x.players > y.players)
			{
				return -1;
			}
			return 0;
		});
		return list.ToArray();
	}

	private OnlineManager.RoomsInfo[] selectRoomsGroup()
	{
		List<OnlineManager.RoomsInfo> list = new List<OnlineManager.RoomsInfo>();
		OnlineManager.RoomsInfo[] rooms = OnlineManager.instance.rooms;
		if (rooms == null)
		{
			return null;
		}
		int[] array;
		if (this.currentMode == 0)
		{
			array = (int[])Enum.GetValues(typeof(GameType));
		}
		else
		{
			array = new int[]
			{
				this.currentMode
			};
		}
		if (this._builtinMapPlayerCnt == null)
		{
			this._builtinMapPlayerCnt = new int[16, 128];
		}
		if (this._builtinMapCapacity == null)
		{
			this._builtinMapCapacity = new int[16, 128];
		}
		for (int i = 0; i < rooms.Length; i++)
		{
			OnlineManager.RoomsInfo roomsInfo = default(OnlineManager.RoomsInfo);
			if (!(rooms[i].roomPassword != string.Empty))
			{
				if (rooms[i].buildInMap)
				{
					if (rooms[i].roomType != 5)
					{
						if (rooms[i].roomType != 1)
						{
							if (Array.IndexOf<int>(array, rooms[i].roomType) != -1)
							{
								if (this.dayLight == 0 || rooms[i].dayLight == OnlineMenu.daylightRemap[this.dayLight])
								{
									if (rooms[i].buildInMap || !(rooms[i].sn != Kube.SN.sn.ToString()))
									{
										if (rooms[i].maxPlayers > rooms[i].players)
										{
											list.Add(rooms[i]);
										}
										long roomMapNumber = rooms[i].roomMapNumber;
										if (rooms[i].buildInMap)
										{
											this._builtinMapCapacity[rooms[i].roomType, (int)(checked((IntPtr)roomMapNumber))] += rooms[i].maxPlayers - rooms[i].players;
											this._builtinMapPlayerCnt[rooms[i].roomType, (int)(checked((IntPtr)roomMapNumber))] += rooms[i].players;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		int num = PhotonNetwork.countOfPlayers / Kube.OH.builtInMaps.Length;
		list.Sort(delegate(OnlineManager.RoomsInfo x, OnlineManager.RoomsInfo y)
		{
			int result = 0;
			if (x.players == y.players)
			{
				result = 0;
			}
			else if (x.players < y.players)
			{
				result = 1;
			}
			else if (x.players > y.players)
			{
				result = -1;
			}
			if (!x.buildInMap || !y.buildInMap)
			{
				return (!x.buildInMap) ? 1 : -1;
			}
			return result;
		});
		for (int j = 0; j < Kube.OH.builtInMaps.Length; j++)
		{
			for (int k = 0; k < array.Length; k++)
			{
				if (array[k] != 1)
				{
					if (Kube.OH.builtInMaps[j].gameTypes.Length > array[k])
					{
						if (Kube.OH.builtInMaps[j].gameTypes[array[k]])
						{
							if (this._builtinMapCapacity[array[k], j] <= 0)
							{
								if (this._builtinMapPlayerCnt[array[k], j] <= num)
								{
									if (array[k] != 10)
									{
										if (array[k] != 0)
										{
											if (this.currentMode != 0 || UnityEngine.Random.value <= 0.5f)
											{
												list.Add(new OnlineManager.RoomsInfo
												{
													roomMapNumber = (long)Kube.OH.builtInMaps[j].Id,
													players = 0,
													roomType = array[k],
													name = Kube.OH.builtInMaps[j].Id.ToString() + " " + array[k].ToString(),
													maxPlayers = Kube.OH.builtInMaps[j].playersMax,
													buildInMap = true,
													dayLight = ((this.dayLight != 0) ? OnlineMenu.daylightRemap[this.dayLight] : 1),
													mapCanBreak = ((!this._canBreak) ? 0 : 1)
												});
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return list.ToArray();
	}

	private void Invalidate()
	{
		if (this.valid)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>();
		foreach (object obj in this.container.gameObject.transform)
		{
			Transform transform = (Transform)obj;
			list.Add(transform.gameObject);
		}
		OnlineManager.RoomsInfo[] array = this.selectRooms();
		if (array == null)
		{
			return;
		}
		int num = Math.Min(100, array.Length);
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = null;
			if (this._hash.ContainsKey(array[i].name))
			{
				gameObject = this._hash[array[i].name];
				if (gameObject)
				{
					gameObject.SetActive(true);
					list.Remove(gameObject);
				}
			}
			if (!gameObject)
			{
				gameObject = this.container.gameObject.AddChild(this.itemPrefab);
				this._hash[array[i].name] = gameObject;
				gameObject.GetComponent<UIButton>().onClick.Clear();
				if (this._onlyFriends)
				{
					EventDelegate.Add(gameObject.GetComponent<UIButton>().onClick, new EventDelegate(new EventDelegate.Callback(this.onItemClick)));
				}
				else
				{
					EventDelegate.Add(gameObject.GetComponent<UIButton>().onClick, new EventDelegate(new EventDelegate.Callback(this.onItemClick2)));
				}
			}
			RoomItem component = gameObject.GetComponent<RoomItem>();
			if (array[i].buildInMap && (long)Localize.buildinMapName.Length > array[i].roomMapNumber)
			{
				component.title.text = Localize.buildinMapName[(int)(checked((IntPtr)array[i].roomMapNumber))];
			}
			else
			{
				component.title.text = ((!string.IsNullOrEmpty(array[i].roomTitle)) ? array[i].roomTitle : Localize.onl_unknown_map);
			}
			UILabel nnplayers = component.nnplayers;
			string text = array[i].players.ToString() + "/" + array[i].maxPlayers;
			component.nnplayers.text = text;
			nnplayers.text = text;
			if (array[i].roomType < this.modeSprites.Length)
			{
				component.mode.spriteName = this.modeSprites[array[i].roomType];
			}
			component.room = array[i];
			gameObject.name = i.ToString("D6");
		}
		for (int j = 0; j < list.Count; j++)
		{
			GameObject gameObject2 = list[j];
			RoomItem component2 = gameObject2.GetComponent<RoomItem>();
			this._hash.Remove(component2.room.name);
			gameObject2.SetActive(false);
			UnityEngine.Object.Destroy(gameObject2);
		}
		this.container.GetComponent<UIGrid>().Reposition();
		this.container.UpdatePosition();
		this.valid = true;
	}

	public void onCreateServer()
	{
		if (Kube.GPS.isVIP)
		{
			Cub2UI.FindAndOpenDialog("dialog_new_server_vip");
		}
		else
		{
			Cub2UI.FindAndOpenDialog("dialog_new_server");
		}
	}

	public UIScrollView container;

	public UIToggle[] modes;

	public string[] modeSprites;

	public GameObject itemPrefab;

	public DayToggle dayToggle;

	protected int dayLight;

	public GameObject newserver_dialog;

	public UILabel onlineLabel;

	private int currentMode;

	private bool _canBreak = true;

	private bool _onlyFriends;

	private float fullUpdate;

	private int numGamesWithFriends;

	private bool valid;

	protected int[,] _builtinMapPlayerCnt;

	protected int[,] _builtinMapCapacity;

	protected static int[] daylightRemap;

	private Dictionary<string, GameObject> _hash;
}
