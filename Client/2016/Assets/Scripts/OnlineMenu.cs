using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class OnlineMenu : MonoBehaviour
{
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
		OnlineManager.RoomsInfo[] rooms = OnlineManager.instance.rooms;
		List<int> list = new List<int>();
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < rooms.Length; j++)
			{
				if (!(rooms[j].roomPassword != string.Empty))
				{
					if (rooms[j].maxPlayers >= rooms[j].players + 2)
					{
						if (this._canBreak || rooms[j].mapCanBreak != 1)
						{
							int num = Mathf.Min(Kube.GPS.playerLevel, Localize.RankName.Length - 1);
							if (Math.Abs(rooms[j].roomPlayerLevel - num) <= 5 || i != 0)
							{
								if (rooms[j].buildInMap && rooms[j].roomMapNumber == component.room.roomMapNumber && rooms[j].roomType == component.room.roomType)
								{
									list.Add(j);
								}
							}
						}
					}
				}
			}
			if (list.Count > 0)
			{
				break;
			}
		}
		if (list.Count > 0)
		{
			int num2 = list[UnityEngine.Random.Range(0, list.Count - 1)];
			OnlineManager.instance.joinRoom(rooms[num2]);
		}
		else
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
					component.nnplayers.text = array[i].players.ToString();
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
				if (rooms[i].roomType != 1 && rooms[i].roomType != 5)
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
		for (int i = 0; i < Kube.OH.builtInMaps.Length; i++)
		{
			OnlineManager.RoomsInfo item = default(OnlineManager.RoomsInfo);
			for (int j = 0; j < array.Length; j++)
			{
				int num = 0;
				if (Kube.OH.builtInMaps[i].gameTypes.Length > array[j] && Kube.OH.builtInMaps[i].gameTypes[array[j]])
				{
					if (array[j] != 0 && array[j] != 1)
					{
						for (int k = 0; k < rooms.Length; k++)
						{
							if (rooms[k].buildInMap && rooms[k].roomMapNumber == (long)Kube.OH.builtInMaps[i].Id)
							{
								if (rooms[k].mapCanBreak != 1 || this._canBreak)
								{
									if (this.dayLight == 0 || rooms[k].dayLight == this.dayLight - 1)
									{
										if (array[j] == rooms[k].roomType)
										{
											num += rooms[k].players;
										}
									}
								}
							}
						}
						item.buildInMap = true;
						item.roomMapNumber = (long)Kube.OH.builtInMaps[i].Id;
						item.players = num;
						item.roomType = array[j];
						item.name = Kube.OH.builtInMaps[i].Id.ToString() + " " + array[j].ToString();
						item.mapCanBreak = ((!this._canBreak) ? 0 : 1);
						list.Add(item);
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
				component.title.text = Localize.onl_unknown_map;
			}
			component.nnplayers.text = array[i].players.ToString();
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

	private Dictionary<string, GameObject> _hash;
}
