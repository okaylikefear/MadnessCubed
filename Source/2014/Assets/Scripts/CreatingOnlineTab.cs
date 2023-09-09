using System;
using System.Collections.Generic;
using UnityEngine;

public class CreatingOnlineTab : MonoBehaviour
{
	private void Start()
	{
		this._hash = new Dictionary<string, GameObject>();
	}

	private void Update()
	{
		if (!this.valid)
		{
			this.Invalidate();
		}
	}

	private void OnEnable()
	{
		OnlineManager.instance.Connect();
		this.valid = false;
	}

	private void OnUpdatedFriendList()
	{
		if (OnlineManager.instance.numGamesWithFriends != this.numGamesWithFriends)
		{
			this.valid = false;
			return;
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

	private void Invalidate()
	{
		if (this.valid)
		{
			return;
		}
		OnlineManager.RoomsInfo[] array = this.selectRooms();
		if (array == null)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>();
		foreach (object obj in this.container.gameObject.transform)
		{
			Transform transform = (Transform)obj;
			list.Add(transform.gameObject);
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
				gameObject = NGUITools.AddChild(this.container.gameObject, this.itemPrefab);
				this._hash[array[i].name] = gameObject;
				EventDelegate.Add(gameObject.GetComponent<UIButton>().onClick, new EventDelegate(new EventDelegate.Callback(this.onItemClick)));
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
			component.mode.spriteName = "4_oo";
			component.room = array[i];
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

	private void onItemClick()
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

	private OnlineManager.RoomsInfo[] selectRooms()
	{
		List<OnlineManager.RoomsInfo> list = new List<OnlineManager.RoomsInfo>();
		OnlineManager.RoomsInfo[] rooms = OnlineManager.instance.rooms;
		if (rooms == null)
		{
			return null;
		}
		for (int i = 0; i < rooms.Length; i++)
		{
			if (rooms[i].gameWithFriends)
			{
				if (rooms[i].roomType == 1)
				{
					list.Add(rooms[i]);
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

	public UIScrollView container;

	private bool valid;

	private float fullUpdate;

	private int numGamesWithFriends;

	public GameObject itemPrefab;

	private Dictionary<string, GameObject> _hash;
}
