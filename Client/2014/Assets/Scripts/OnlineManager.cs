using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using kube;
using kube.data;
using UnityEngine;

public class OnlineManager : MonoBehaviour
{
	public static OnlineManager instance
	{
		get
		{
			if (OnlineManager._instance == null)
			{
				OnlineManager._instance = UnityEngine.Object.FindObjectOfType<OnlineManager>();
			}
			return OnlineManager._instance;
		}
	}

	public static void ShowPasswordRequest(OnlineManager.RoomsInfo room)
	{
		PasswordDialog component = OnlineManager.instance.password_dialog.GetComponent<PasswordDialog>();
		component.room = room;
		OnlineManager.instance.password_dialog.SetActive(true);
	}

	private void Start()
	{
	}

	private void FindFriends()
	{
		string[] array = new string[Kube.OH.friends.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Kube.OH.friends[i].uid;
		}
		PhotonNetwork.FindFriends(array);
	}

	private void Update()
	{
		if (this.popup.activeSelf && this._process != OnlineManager.Process.none && PhotonNetwork.connectionState == ConnectionState.Disconnected)
		{
			this.EndAllActivity();
		}
		if (PhotonNetwork.connected && PhotonNetwork.insideLobby && (PhotonNetwork.Friends == null || PhotonNetwork.FriendsListAge > 5000))
		{
			this.FindFriends();
		}
	}

	private void ConnectUsingSettings()
	{
		PhotonNetwork.offlineMode = false;
		PhotonNetwork.playerName = Kube.SN.playerUID;
		PhotonNetwork.player.customProperties["id"] = Kube.GPS.playerId;
		try
		{
			if (Kube.SN.platform == SocialNet.platformType.fb)
			{
				PhotonNetwork.ConnectToBestCloudServer("17." + Kube.SN.platform.ToString());
			}
			else
			{
				PhotonNetwork.ConnectUsingSettings("17." + Kube.SN.platform.ToString());
			}
		}
		catch
		{
			this.EndAllActivity();
		}
	}

	public void Connect()
	{
		PhotonNetwork.offlineMode = false;
		if (!PhotonNetwork.connected)
		{
			this.ConnectUsingSettings();
			this.popup.SetActive(true);
		}
		this._process = OnlineManager.Process.connect;
		base.StartCoroutine(this._Connect());
	}

	private IEnumerator _Connect()
	{
		while (!PhotonNetwork.connected)
		{
			if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
			{
				yield break;
			}
			yield return new WaitForSeconds(1f);
		}
		while (this.rooms == null)
		{
			yield return new WaitForSeconds(1f);
		}
		if (this._process == OnlineManager.Process.connect)
		{
			this.popup.SetActive(false);
			this._process = OnlineManager.Process.none;
		}
		yield break;
	}

	private void OnConnectedToPhoton()
	{
	}

	public void playRoom(OnlineManager.RoomsInfo room, bool b)
	{
		this.popup.SetActive(true);
		base.StartCoroutine(this._playRoom(room, b));
	}

	public IEnumerator _playRoom(OnlineManager.RoomsInfo room, bool offline)
	{
		if (!PhotonNetwork.connected)
		{
			this._process = OnlineManager.Process.quickPlay;
			this.ConnectUsingSettings();
			yield return base.StartCoroutine(this._Connect());
		}
		while (this._rooms == null)
		{
			yield return new WaitForSeconds(1f);
		}
		for (int i = 0; i < this._rooms.Length; i++)
		{
			if (this.rooms[i].maxPlayers - this.rooms[i].players >= 2)
			{
				if (room.roomType <= 0 || this.rooms[i].roomType == room.roomType)
				{
					if (!this.rooms[i].buildInMap && this.rooms[i].roomMapNumber == room.roomMapNumber)
					{
						yield return base.StartCoroutine(this._JoinRoom(this.rooms[i]));
						yield break;
					}
				}
			}
		}
		yield return base.StartCoroutine(this._CreateRoom((GameType)room.roomType, room.buildInMap, room.roomMapNumber, room.mapCanBreak, room.dayLight, offline, 0, room.roomPassword));
		yield break;
	}

	public void joinRoom(OnlineManager.RoomsInfo room)
	{
		this.popup.SetActive(true);
		base.StartCoroutine(this._JoinRoom(room));
	}

	private IEnumerator _JoinRoom(OnlineManager.RoomsInfo room)
	{
		if (!PhotonNetwork.connected)
		{
			this._process = OnlineManager.Process.quickPlay;
			this.ConnectUsingSettings();
			yield return base.StartCoroutine(this._Connect());
		}
		Kube.OH.tempMap.GameType = (GameType)room.roomType;
		Kube.OH.tempMap.CanBreak = room.mapCanBreak;
		Kube.OH.tempMap.DayLight = room.dayLight;
		Kube.OH.tempMap.CreatedGame = false;
		PhotonNetwork.JoinRoom(room.name);
		this.creatingRoom = true;
		yield break;
	}

	public void createRoom(OnlineManager.RoomsInfo roomsInfo, bool offline = false)
	{
		this.popup.SetActive(true);
		long newGameMap;
		if (roomsInfo.buildInMap)
		{
			newGameMap = -roomsInfo.roomMapNumber;
		}
		else
		{
			newGameMap = roomsInfo.roomMapNumber;
		}
		base.StartCoroutine(this._CreateRoom((GameType)roomsInfo.roomType, roomsInfo.buildInMap, newGameMap, roomsInfo.mapCanBreak, roomsInfo.dayLight, offline, 0, roomsInfo.roomPassword));
	}

	private IEnumerator _CreateRoom(int filterGameType, int filterMapName, int mapCanBreak, int newGameDayLight)
	{
		GameType _newGameType = GameType.survival;
		if (filterGameType == 0)
		{
			int rand = UnityEngine.Random.Range(0, 100);
			if (rand >= 0 && rand < 25)
			{
				_newGameType = GameType.shooter;
			}
			else if (rand >= 25 && rand < 85)
			{
				_newGameType = GameType.survival;
			}
			else if (rand >= 85 && rand < 100)
			{
				_newGameType = GameType.teams;
			}
		}
		else
		{
			_newGameType = this.filterGameTypeType[filterGameType];
		}
		int maxMaps = Localize.buildinMapName.Length;
		int _newGameMap = UnityEngine.Random.Range(0, maxMaps);
		if (filterMapName != 0)
		{
			_newGameMap = filterMapName - 1;
		}
		if (_newGameType == GameType.teams)
		{
			_newGameMap += 20;
		}
		int _newGameCanBreak = mapCanBreak - 1;
		if (_newGameCanBreak < 0)
		{
			_newGameCanBreak = UnityEngine.Random.Range(0, 2);
		}
		int _newGameLight = newGameDayLight - 1;
		if (_newGameLight < 0)
		{
			_newGameLight = UnityEngine.Random.Range(0, 2);
		}
		yield return base.StartCoroutine(this._CreateRoom(_newGameType, true, (long)_newGameMap, _newGameCanBreak, _newGameLight, false, 0, string.Empty));
		yield break;
	}

	private IEnumerator _CreateRoom(GameType _newGameType, bool builtin, long _newGameMap, int _newGameCanBreak, int _newGameLight, bool offline, int missionId = 0, string roomPassword = "")
	{
		MonoBehaviour.print(string.Concat(new object[]
		{
			"New server, gameType=",
			_newGameType,
			" mapNum=",
			_newGameMap
		}));
		if (!PhotonNetwork.connected && !offline)
		{
			this._process = OnlineManager.Process.quickPlay;
			this.ConnectUsingSettings();
			yield return base.StartCoroutine(this._Connect());
		}
		Kube.OH.tempMap.GameType = _newGameType;
		Kube.OH.tempMap.CanBreak = _newGameCanBreak;
		Kube.OH.tempMap.DayLight = _newGameLight;
		if (_newGameMap < 0L || builtin)
		{
			Kube.OH.tempMap.NewMapType = -1;
		}
		Kube.OH.tempMap.Id = _newGameMap;
		ExitGames.Client.Photon.Hashtable playingPlayersHash = new ExitGames.Client.Photon.Hashtable();
		playingPlayersHash["p"] = " ";
		Kube.OH.tempMap.CreatedGame = true;
		int newGameMapType = Kube.OH.tempMap.NewMapType;
		string roomParams = string.Concat(new string[]
		{
			Kube.OH.GetServerCode((int)Kube.OH.tempMap.GameType, 0),
			Kube.OH.GetServerCode((!builtin) ? 0 : 1, 0),
			Kube.OH.GetServerCode((!builtin) ? 0 : ((int)(-(int)Kube.OH.tempMap.Id)), 0),
			Kube.OH.GetServerCode(Kube.OH.tempMap.DayLight + Kube.OH.tempMap.CanBreak * 3, 0),
			Kube.OH.GetServerCode(UnityEngine.Random.Range(0, 4096), 2)
		});
		playingPlayersHash["m"] = Kube.OH.tempMap.Id;
		playingPlayersHash["b"] = Kube.OH.tempMap.NewMapType;
		if (_newGameType == GameType.mission)
		{
			MissionDesc mission = MissionBox.FindMissionById(missionId);
			playingPlayersHash["mcfg"] = mission.config;
			playingPlayersHash["mt"] = mission.type;
			roomParams = string.Concat(new string[]
			{
				Kube.OH.GetServerCode(5, 0),
				Kube.OH.GetServerCode(mission.episode, 0),
				Kube.OH.GetServerCode(mission.index, 0),
				Kube.OH.GetServerCode(Kube.OH.tempMap.DayLight + Kube.OH.tempMap.CanBreak * 3, 0),
				Kube.OH.GetServerCode(UnityEngine.Random.Range(0, 4096), 2)
			});
			Kube.OH.tempMap.missionConfig = mission.config;
			Kube.OH.tempMap.missionType = (ObjectsHolderScript.MissionType)mission.type;
			Kube.OH.tempMap.missionId = mission.id;
		}
		if (offline)
		{
			if (PhotonNetwork.connected)
			{
				PhotonNetwork.Disconnect();
			}
			PhotonNetwork.offlineMode = true;
		}
		int numPlayersMax = this.newGamePlayerNumsArray[3];
		if (_newGameType == GameType.survival)
		{
			numPlayersMax = this.newGamePlayerNumsArray[1];
		}
		ObjectsHolderScript.BuiltInMap bmi = Kube.OH.findMapInfo((long)((int)(-(int)Kube.OH.tempMap.Id)));
		if (bmi != null)
		{
			numPlayersMax = Math.Min(bmi.playersMax, numPlayersMax);
		}
		PhotonNetwork.CreateRoom("CD^" + roomPassword + "^" + roomParams, true, true, numPlayersMax, playingPlayersHash, OnlineManager.propsInLobby);
		this.creatingRoom = true;
		yield return new WaitForSeconds(1f);
		yield break;
	}

	public void selectRooms(int filterGameType, int filterMapName, int mapCanBreak, int newGameDayLight)
	{
		if (this.joinAbleRooms.Length < this.wholeNumRooms)
		{
			this.joinAbleRooms = new int[this.wholeNumRooms];
		}
		this.numJoinAbleGames = 0;
		for (int i = 0; i < this.wholeNumRooms; i++)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (filterGameType == 0 && (this.rooms[i].roomType == 2 || this.rooms[i].roomType == 3 || this.rooms[i].roomType == 4))
			{
				flag = true;
			}
			else if (filterGameType == 1 && this.rooms[i].roomType == 2)
			{
				flag = true;
			}
			else if (filterGameType == 2 && this.rooms[i].roomType == 1)
			{
				flag = true;
			}
			else if (filterGameType == 3 && this.rooms[i].roomType == 3)
			{
				flag = true;
			}
			else if (filterGameType == 4 && this.rooms[i].roomType == 4)
			{
				flag = true;
			}
			bool flag4 = false;
			if (filterMapName == 0)
			{
				flag4 = true;
			}
			else if (filterGameType == 0 || filterGameType == 1 || filterGameType == 2 || filterGameType == 3)
			{
				if (this.rooms[i].roomEpisodeNumber == 1 && this.rooms[i].roomMapNumber == (long)(filterMapName - 1))
				{
					flag4 = true;
				}
			}
			else if (filterGameType == 4 && this.rooms[i].roomEpisodeNumber == 1 && this.rooms[i].roomMapNumber == (long)(filterMapName - 1 + 20))
			{
				flag4 = true;
			}
			if (newGameDayLight == 0)
			{
				flag2 = true;
			}
			else if (newGameDayLight - 1 == this.rooms[i].dayLight)
			{
				flag2 = true;
			}
			if (mapCanBreak == 0)
			{
				flag3 = true;
			}
			else if (mapCanBreak - 1 == this.rooms[i].mapCanBreak)
			{
				flag3 = true;
			}
			if (flag && flag4 && flag2 && flag3 && this.rooms[i].buildInMap && this.rooms[i].roomPassword.Length == 0)
			{
				if (this.rooms[i].players < this.rooms[i].maxPlayers - 1)
				{
					this.joinAbleRooms[this.numJoinAbleGames] = i;
					this.numJoinAbleGames++;
				}
				this.playersOnTheServer += this.rooms[i].players;
			}
		}
	}

	private IEnumerator _QuickPlay()
	{
		this.popup.SetActive(true);
		this._process = OnlineManager.Process.quickPlay;
		Kube.SS.SendStat("fastgame");
		this.Connect();
		while (!PhotonNetwork.connected)
		{
			yield return new WaitForSeconds(1f);
		}
		UnityEngine.Debug.Log("_QuickPlay");
		while (this._rooms == null)
		{
			yield return new WaitForSeconds(1f);
		}
		UnityEngine.Debug.Log("_QuickPlay Rooms");
		this.selectRooms(0, 0, 0, 0);
		if (this.numJoinAbleGames != 0)
		{
			int randomRoom = UnityEngine.Random.Range(0, this.numJoinAbleGames);
			int selectedRoom = this.joinAbleRooms[randomRoom];
			yield return base.StartCoroutine(this._JoinRoom(this.rooms[selectedRoom]));
		}
		else
		{
			yield return base.StartCoroutine(this._CreateRoom(0, 0, 0, 0));
		}
		while (PhotonNetwork.room == null)
		{
			yield return new WaitForSeconds(1f);
		}
		UnityEngine.Debug.Log("_QuickPlay connected");
		yield break;
	}

	private IEnumerator _PlayMission(MissionDesc mission, bool offline)
	{
		this.popup.SetActive(true);
		this._process = OnlineManager.Process.quickPlay;
		if (!offline)
		{
			this.Connect();
			while (!PhotonNetwork.connected)
			{
				yield return new WaitForSeconds(1f);
			}
			UnityEngine.Debug.Log("_PlayMission Connect ");
			while (this._rooms == null)
			{
				yield return new WaitForSeconds(1f);
			}
			UnityEngine.Debug.Log("_QuickPlay Rooms");
		}
		int numJoinAbleGames = 0;
		int[] joinAbleRooms = new int[512];
		if (this.rooms != null && !offline)
		{
			for (int i = 0; i < this.wholeNumRooms; i++)
			{
				if (this.rooms[i].roomType == 5)
				{
					if (this.rooms[i].roomEpisodeNumber == mission.episode)
					{
						if (this.rooms[i].roomMapNumber == (long)mission.index)
						{
							if (this.rooms[i].players < this.rooms[i].maxPlayers - 1)
							{
								joinAbleRooms[numJoinAbleGames] = i;
								numJoinAbleGames++;
							}
							if (numJoinAbleGames >= 512)
							{
								break;
							}
						}
					}
				}
			}
		}
		if (numJoinAbleGames != 0)
		{
			int randomRoom = UnityEngine.Random.Range(0, numJoinAbleGames);
			int selectedRoom = joinAbleRooms[randomRoom];
			Kube.OH.tempMap.missionId = mission.id;
			Kube.OH.tempMap.missionType = (ObjectsHolderScript.MissionType)mission.type;
			Kube.OH.tempMap.missionConfig = mission.config;
			yield return base.StartCoroutine(this._JoinRoom(this.rooms[selectedRoom]));
		}
		else
		{
			yield return base.StartCoroutine(this._CreateRoom(GameType.mission, true, mission.mapId, mission.canBreak, mission.dayTime, offline, mission.id, string.Empty));
		}
		while (PhotonNetwork.room == null)
		{
			yield return new WaitForSeconds(1f);
		}
		UnityEngine.Debug.Log("_QuickPlay connected");
		yield break;
	}

	public void QuickPlay()
	{
		base.StartCoroutine(this._QuickPlay());
	}

	public void PlayMission(MissionDesc missionDesc, bool b)
	{
		base.StartCoroutine(this._PlayMission(missionDesc, b));
	}

	private void OnReceivedRoomListUpdate()
	{
		this.CreateRoomList();
	}

	private void OnReceivedRoomList()
	{
		this.CreateRoomList();
	}

	private void OnUpdatedFriendList()
	{
		this.CreateRoomList();
	}

	private void OnPhotonJoinRoomFailed()
	{
		base.StopAllCoroutines();
		this.popup.SetActive(false);
	}

	public void EndAllActivity()
	{
		base.StopAllCoroutines();
		this.popup.SetActive(false);
	}

	private void OnPhotonCreateRoomFailed()
	{
		base.StopAllCoroutines();
		this.popup.SetActive(false);
	}

	private void CreateRoomList()
	{
		this._rooms = PhotonNetwork.GetRoomList();
		this.wholeNumRooms = this._rooms.Length;
		char[] separator = new char[]
		{
			'^'
		};
		int num = 0;
		if (this.rooms == null || this.rooms.Length < this._rooms.Length)
		{
			this.rooms = new OnlineManager.RoomsInfo[this._rooms.Length];
		}
		List<int> list = new List<int>();
		List<FriendInfo> friends = PhotonNetwork.Friends;
		for (int i = 0; i < this.wholeNumRooms; i++)
		{
			num += this._rooms[i].name.Length;
			string[] array = this._rooms[i].name.Split(separator);
			if (array.Length <= 1)
			{
				this.rooms[i] = default(OnlineManager.RoomsInfo);
			}
			else
			{
				this.rooms[i].name = this._rooms[i].name;
				this.rooms[i].roomPassword = AuxFunc.DecodeRussianName(array[1]);
				string text = array[2];
				this.rooms[i].roomType = Kube.OH.DecodeServerCode(text.Substring(0, 1));
				this.rooms[i].roomEpisodeNumber = Kube.OH.DecodeServerCode(text.Substring(1, 1));
				this.rooms[i].roomMapNumber = (long)Kube.OH.DecodeServerCode(text.Substring(2, 1));
				this.rooms[i].dayLight = Kube.OH.DecodeServerCode(text.Substring(3, 1)) % 3;
				this.rooms[i].mapCanBreak = (Kube.OH.DecodeServerCode(text.Substring(3, 1)) - this.rooms[i].dayLight) / 3;
				this.rooms[i].random = Kube.OH.DecodeServerCode(text.Substring(4, 2));
				this.rooms[i].players = this._rooms[i].playerCount;
				this.rooms[i].maxPlayers = (int)this._rooms[i].maxPlayers;
				this.rooms[i].gameWithFriends = false;
				this.rooms[i].playersStr = string.Empty;
				this.rooms[i].friendsStr = string.Empty;
				if (this._rooms[i].customProperties.ContainsKey("m"))
				{
					this.rooms[i].roomMapNumber = Math.Abs((long)this._rooms[i].customProperties["m"]);
				}
				list.Clear();
				if (friends != null)
				{
					for (int j = 0; j < friends.Count; j++)
					{
						if (friends[j].Room == this._rooms[i].name)
						{
							for (int k = 0; k < Kube.OH.friends.Length; k++)
							{
								if (Kube.OH.friends[k].uid == friends[j].Name)
								{
									this.rooms[i].gameWithFriends = true;
									if (this.rooms[i].friendsStr.Length != 0)
									{
										OnlineManager.RoomsInfo[] array2 = this.rooms;
										int num2 = i;
										array2[num2].friendsStr = array2[num2].friendsStr + ";";
									}
									OnlineManager.RoomsInfo[] array3 = this.rooms;
									int num3 = i;
									array3[num3].friendsStr = array3[num3].friendsStr + string.Empty + k;
									list.Add(Kube.OH.friends[k].Id);
									break;
								}
							}
						}
					}
				}
				this.rooms[i].friendsIds = list.ToArray();
				if (list.Count > 0)
				{
					this.numGamesWithFriends++;
				}
				this.rooms[i].buildInMap = false;
				if (this.rooms[i].roomType == 5)
				{
					this.rooms[i].buildInMap = true;
				}
				else if (this.rooms[i].roomEpisodeNumber == 1)
				{
					this.rooms[i].buildInMap = true;
				}
			}
		}
		if (this.wholeNumRooms != 0)
		{
		}
	}

	public int[] newGamePlayerNumsArray = new int[]
	{
		6,
		8,
		12,
		16
	};

	public GameObject popup;

	public GameObject friendPrefab;

	public int numGamesWithFriends;

	private static string[] propsInLobby = new string[]
	{
		"p",
		"m"
	};

	private static OnlineManager _instance;

	public GameObject password_dialog;

	private OnlineManager.Process _process;

	private bool creatingRoom;

	private GameType[] filterGameTypeType = new GameType[]
	{
		GameType.test,
		GameType.shooter,
		GameType.creating,
		GameType.survival,
		GameType.teams
	};

	[NonSerialized]
	public int playersOnTheServer;

	private int numJoinAbleGames;

	private int[] joinAbleRooms = new int[128];

	[NonSerialized]
	public int wholeNumRooms;

	[NonSerialized]
	public OnlineManager.RoomsInfo[] rooms;

	private RoomInfo[] _rooms;

	private enum Process
	{
		none,
		connect,
		quickPlay
	}

	public struct RoomsInfo
	{
		public string name;

		public int roomType;

		public int roomEpisodeNumber;

		public long roomMapNumber;

		public string roomPassword;

		public int mapCanBreak;

		public string playersStr;

		public string friendsStr;

		public int players;

		public int maxPlayers;

		public int dayLight;

		public int random;

		public bool gameWithFriends;

		public bool buildInMap;

		public int[] friendsIds;
	}
}
