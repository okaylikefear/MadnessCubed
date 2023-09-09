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
		int num = Math.Min(Kube.GPS.playerNumMaps, this._slots.Length);
		this.loading.SetActive(false);
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
		base.StartCoroutine(this._LoadIsMap((long)mapItem.mapId));
	}

	private IEnumerator _LoadIsMap(long id)
	{
		if (PlayMenu.playMenu.slotNames == null || PlayMenu.playMenu.slotNames[(int)(checked((IntPtr)id))] == null)
		{
			long mySelectedMapId = (long)Kube.SS.serverId * 20L + id;
			Kube.SS.LoadIsMap(mySelectedMapId, base.gameObject, "LoadIsMapDone");
		}
		while (PlayMenu.playMenu.slotNames == null)
		{
			yield return 1;
		}
		checked
		{
			while (PlayMenu.playMenu.slotNames[(int)((IntPtr)id)] == null)
			{
				yield return 1;
			}
			this.creating_play.gameObject.SetActive(true);
			this.creating_play.preloadMapName = PlayMenu.playMenu.slotNames[(int)((IntPtr)id)].name;
			this.creating_play.title.text = PlayMenu.playMenu.slotNames[(int)((IntPtr)id)].name;
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
	}

	private void OnEnable()
	{
		this.Invalidate();
		this.container.ResetPosition();
	}

	private void RenameSlotDone()
	{
		this.Invalidate();
	}

	private void BuyNewMapDone()
	{
		PlayMenu.playMenu.LoadAllMaps();
		this.Invalidate();
	}

	private void LoadMapsDone()
	{
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
			component.title.text = string.Concat(new object[]
			{
				(j + 1).ToString(),
				". ",
				Localize.c_slot,
				" ",
				j + 1
			});
			component.id.text = Localize.c_map_id + ((long)Kube.SS.serverId * 20L + (long)j).ToString();
			if (PlayMenu.playMenu.slotNames != null && PlayMenu.playMenu.slotNames[j] != null)
			{
				component.title.text = (j + 1).ToString() + ". " + PlayMenu.playMenu.slotNames[j].name;
			}
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

	public const int MAX_MAPS = 20;

	public GameObject addslot;

	private GameObject[] _slots;

	public UIScrollView container;

	public GameObject hint;

	public CreatingPlayDialog creating_play;

	public GameObject newMap;

	public GameObject regenerateMap;

	public GameObject loading;

	protected int _NewMapType;

	public GameObject itemPrefab;
}
