using System;
using System.Collections;
using kube;
using UnityEngine;

public class CreatingMyTab : MonoBehaviour
{
	public void ResetMap()
	{
		this.newMap.SetActive(true);
	}

	private void Awake()
	{
		this._slots = new GameObject[20];
		this.loading.SetActive(false);
	}

	private void LoadIsMapDone(string str)
	{
		char[] separator = new char[]
		{
			'^'
		};
		string[] array = str.Split(separator);
		string text = string.Empty;
		if (Convert.ToInt32(array[0]) == 0)
		{
			text = AuxFunc.DecodeRussianName(array[2]);
			MonoBehaviour.print("Str = " + array[4]);
			int.TryParse(array[4], out this._NewMapType);
		}
		else
		{
			text = Localize.newMapTypeName[0];
		}
		this.slotNames[this._currentLoadingSlot] = text;
	}

	public void onSelectSlot(MapItem mapItem)
	{
		long mySelectedMapId = (long)Kube.SS.serverId * 20L + (long)mapItem.mapId;
		if (this.creating_play)
		{
			this.creating_play.gameObject.SetActive(false);
		}
		this.creating_play.owner = this;
		this.creating_play.mySelectedMapId = mySelectedMapId;
		this.loading.SetActive(true);
		this.LoadIsMap((long)mapItem.mapId);
	}

	private void LoadIsMap(long id)
	{
		base.StartCoroutine(this._LoadIsMap(id));
	}

	private IEnumerator _LoadAllMaps()
	{
		int nn = Math.Min(Kube.GPS.playerNumMaps, this._slots.Length);
		this.slotNames = new string[nn];
		for (int i = 0; i < nn; i++)
		{
			long mySelectedMapId = (long)Kube.SS.serverId * 20L + (long)i;
			this._currentLoadingSlot = i;
			Kube.SS.LoadIsMap(mySelectedMapId, base.gameObject, "LoadIsMapDone");
			while (this.slotNames[i] == null)
			{
				yield return 1;
			}
			if (this._slots[i])
			{
				MapItem item = this._slots[i].GetComponent<MapItem>();
				item.mapId = i;
				item.title.text = (i + 1).ToString() + ". " + this.slotNames[i];
				item.id.text = Localize.c_map_id + ((long)Kube.SS.serverId * 20L + (long)i).ToString();
			}
		}
		yield break;
	}

	private IEnumerator _LoadIsMap(long id)
	{
		checked
		{
			while (this.slotNames[(int)((IntPtr)id)] == null)
			{
				yield return 1;
			}
			this.creating_play.gameObject.SetActive(true);
			this.creating_play.preloadMapName = this.slotNames[(int)((IntPtr)id)];
			this.creating_play.title.text = this.slotNames[(int)((IntPtr)id)];
			this.creating_play.isMyMap = true;
			this.loading.SetActive(false);
			yield break;
		}
	}

	public void LoadMap(string preloadMapName, long mySelectedMapId, bool offline = true, string password = "", int dayLight = 0, bool isMyMap = true)
	{
		OnlineManager.RoomsInfo roomsInfo = default(OnlineManager.RoomsInfo);
		roomsInfo.buildInMap = false;
		roomsInfo.roomMapNumber = mySelectedMapId;
		roomsInfo.roomType = 1;
		roomsInfo.mapCanBreak = 1;
		roomsInfo.dayLight = dayLight;
		roomsInfo.roomPassword = password;
		roomsInfo.roomTitle = preloadMapName;
		OnlineManager.instance.createRoom(roomsInfo, offline);
	}

	private void Start()
	{
		this.Invalidate();
		base.StartCoroutine(this._LoadAllMaps());
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		this.container.ResetPosition();
	}

	private void BuyNewMapDone()
	{
		base.StartCoroutine(this._LoadAllMaps());
		this.Invalidate();
	}

	private void Invalidate()
	{
		if (Kube.GPS == null)
		{
			return;
		}
		if (this.addslot.activeSelf)
		{
			this.addslot.GetComponentInChildren<UILabel>().text = string.Format(string.Concat(new object[]
			{
				Localize.c_new_slot,
				" ",
				Localize.is_buy_for,
				" ",
				Kube.GPS.newMapPrice
			}), new object[0]);
		}
		int height = this.itemPrefab.GetComponent<UIWidget>().height;
		int num = 0;
		Vector3 localPosition = Vector3.zero;
		for (int i = 0; i < 20; i++)
		{
			if (this._slots[i] != null)
			{
				this._slots[i].SetActive(false);
				UnityEngine.Object.Destroy(this._slots[i]);
				this._slots[i] = null;
			}
		}
		int num2 = Math.Min(Kube.GPS.playerNumMaps, this._slots.Length);
		for (int j = 0; j < num2; j++)
		{
			GameObject gameObject = this.container.gameObject.AddChild(this.itemPrefab);
			localPosition = gameObject.transform.localPosition;
			localPosition.y = (float)num;
			num -= height + 5;
			gameObject.transform.localPosition = localPosition;
			MapItem component = gameObject.GetComponent<MapItem>();
			component.mapId = j;
			component.title.text = Localize.c_slot + " " + (j + 1);
			component.id.text = Localize.c_map_id + ((long)Kube.SS.serverId * 20L + (long)j).ToString();
			this._slots[j] = gameObject;
		}
		if (Kube.GPS.playerNumMaps < 20)
		{
			localPosition = this.addslot.transform.localPosition;
			localPosition.y = (float)num;
			this.addslot.transform.localPosition = localPosition;
		}
		else
		{
			this.addslot.SetActive(false);
		}
		this.container.UpdatePosition();
	}

	public void onResetSlot(MapItem mapItem)
	{
		NewMapDialog component = this.regenerateMap.GetComponent<NewMapDialog>();
		component.owner = this;
		component.slot = mapItem.mapId;
		component.gameObject.SetActive(true);
	}

	public void onBuySlot()
	{
		NewMapDialog component = this.newMap.GetComponent<NewMapDialog>();
		component.owner = this;
		component.slot = Kube.GPS.playerNumMaps;
		this.newMap.SetActive(true);
	}

	private const int MAX_MAPS = 20;

	public GameObject addslot;

	private GameObject[] _slots;

	public UIScrollView container;

	public GameObject hint;

	public CreatingPlayDialog creating_play;

	public GameObject newMap;

	public GameObject regenerateMap;

	public GameObject loading;

	protected int _NewMapType;

	protected string[] slotNames;

	public GameObject itemPrefab;

	protected int _currentLoadingSlot;
}
