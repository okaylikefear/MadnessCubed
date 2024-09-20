using System;
using System.Collections.Generic;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class MaptopOnlineTab : MonoBehaviour
{
	// Note: this type is marked as 'beforefieldinit'.
	static MaptopOnlineTab()
	{
		int[] array = new int[4];
		array[0] = 1;
		array[1] = 7;
		array[2] = 30;
		MaptopOnlineTab.daycount = array;
	}

	private void Awake()
	{
		this._hash = new Dictionary<int, GameObject>();
	}

	private void Update()
	{
		if (!this.valid)
		{
			this.Invalidate();
		}
	}

	public void onDayToggle()
	{
		this.LoadItems(MaptopOnlineTab.daycount[DayToggle.current.state]);
	}

	private void onLoaded(string response)
	{
		this.container.ResetPosition();
		JsonData jsonData = JsonMapper.ToObject(response);
		this.items = MapTop.parse(jsonData["items"]);
		this.valid = false;
		this.Invalidate();
	}

	private void LoadItems(int i)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["d"] = i.ToString();
		Kube.SS.Request(801, dictionary, new ServerCallback(this.onLoaded));
	}

	private void OnEnable()
	{
		OnlineManager.instance.Connect();
		this.valid = false;
		this.LoadItems(1);
	}

	private void OnUpdatedFriendList()
	{
		if (OnlineManager.instance.numGamesWithFriends != this.numGamesWithFriends)
		{
			this.valid = false;
			return;
		}
	}

	private void Hit(int id)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["oid"] = id.ToString();
		Kube.SS.Request(804, dictionary, null);
	}

	private void OnReceivedRoomListUpdate()
	{
		if (Time.time > this.fullUpdate)
		{
			this.valid = false;
			this.fullUpdate = Time.time + 30f;
			return;
		}
		TopInfo[] array = this.selectRooms();
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (this._hash.ContainsKey(array[i].id))
			{
				GameObject gameObject = this._hash[array[i].id];
				if (gameObject)
				{
					MaptopItem component = gameObject.GetComponent<MaptopItem>();
					component.nnplayers.text = array[i].players.ToString();
				}
			}
		}
	}

	private void Invalidate()
	{
		if (this.valid)
		{
			return;
		}
		TopInfo[] array = this.selectRooms();
		List<GameObject> list = new List<GameObject>();
		foreach (object obj in this.container.gameObject.transform)
		{
			Transform transform = (Transform)obj;
			list.Add(transform.gameObject);
		}
		if (array == null)
		{
			return;
		}
		int num = Math.Min(100, array.Length);
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = null;
			if (this._hash.ContainsKey(array[i].id))
			{
				gameObject = this._hash[array[i].id];
				if (gameObject)
				{
					gameObject.SetActive(true);
					list.Remove(gameObject);
				}
			}
			if (!gameObject)
			{
				gameObject = this.container.gameObject.AddChild(this.itemPrefab);
				this._hash[array[i].id] = gameObject;
				EventDelegate.Add(gameObject.GetComponent<UIButton>().onClick, new EventDelegate(new EventDelegate.Callback(this.onItemClick)));
			}
			MaptopItem component = gameObject.GetComponent<MaptopItem>();
			component.title.text = array[i].name;
			component.nnplayers.text = array[i].players.ToString();
			component.id = array[i].id;
			if (array[i].roomType < MaptopOnlineTab.modeSprites.Length)
			{
				component.mode.spriteName = MaptopOnlineTab.modeSprites[array[i].roomType];
			}
			component.info = array[i];
			gameObject.name = i.ToString("D6");
		}
		for (int j = 0; j < list.Count; j++)
		{
			GameObject gameObject2 = list[j];
			MaptopItem component2 = gameObject2.GetComponent<MaptopItem>();
			this._hash.Remove(component2.id);
			gameObject2.SetActive(false);
			UnityEngine.Object.Destroy(gameObject2);
		}
		this.container.GetComponent<UIGrid>().Reposition();
		this.container.UpdatePosition();
		this.valid = true;
	}

	private OnlineManager.RoomsInfo FindRoom(TopInfo room)
	{
		OnlineManager.RoomsInfo[] rooms = OnlineManager.instance.rooms;
		for (int i = 0; i < rooms.Length; i++)
		{
			if (!(rooms[i].roomPassword != string.Empty))
			{
				if (rooms[i].roomType == room.roomType)
				{
					if (!rooms[i].buildInMap && rooms[i].roomMapNumber == room.roomMapNumber)
					{
						if (rooms[i].maxPlayers - rooms[i].players >= 2)
						{
							return rooms[i];
						}
					}
				}
			}
		}
		return new OnlineManager.RoomsInfo
		{
			buildInMap = false,
			roomMapNumber = room.roomMapNumber,
			roomType = room.roomType,
			mapCanBreak = room.mapCanBreak,
			dayLight = room.dayLight
		};
	}

	private void onItemClick()
	{
		MaptopItem component = UIButton.current.GetComponent<MaptopItem>();
		OnlineManager.RoomsInfo roomsInfo = this.FindRoom(component.info);
		this.Hit(component.info.id);
		if (roomsInfo.players > 0)
		{
			OnlineManager.instance.joinRoom(roomsInfo);
		}
		else
		{
			OnlineManager.instance.createRoom(roomsInfo, false);
		}
	}

	private TopInfo[] selectRooms()
	{
		List<TopInfo> list = new List<TopInfo>();
		OnlineManager.RoomsInfo[] rooms = OnlineManager.instance.rooms;
		if (rooms == null)
		{
			return list.ToArray();
		}
		if (this.items == null || this.items.Length == 0)
		{
			return this.items;
		}
		for (int i = 0; i < this.items.Length; i++)
		{
			TopInfo topInfo = this.items[i];
			topInfo.players = 0;
			for (int j = 0; j < rooms.Length; j++)
			{
				if (rooms[j].roomType == this.items[i].roomType && rooms[j].roomMapNumber == this.items[i].roomMapNumber)
				{
					topInfo.players += rooms[j].players;
				}
			}
			list.Add(topInfo);
		}
		list.Sort(delegate(TopInfo x, TopInfo y)
		{
			if (y.hits == x.hits)
			{
				return y.players - x.players;
			}
			return y.hits - x.hits;
		});
		return list.ToArray();
	}

	public UIScrollView container;

	public static string[] modeSprites = new string[]
	{
		string.Empty,
		"4_oo",
		"2_oo",
		"1_oo",
		"3_oo",
		"mission_0",
		"flag",
		"domin_1",
		"flag"
	};

	public DayToggle daytoggle;

	private static int[] daycount;

	private TopInfo[] items;

	private bool valid;

	private float fullUpdate;

	private int numGamesWithFriends;

	public GameObject itemPrefab;

	private Dictionary<int, GameObject> _hash;
}
