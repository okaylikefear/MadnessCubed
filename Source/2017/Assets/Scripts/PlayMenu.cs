using System;
using System.Collections;
using kube;
using kube.data;
using UnityEngine;

public class PlayMenu : MonoBehaviour
{
	private void Awake()
	{
		int num = Math.Min(Kube.GPS.playerNumMaps, 20);
		this._slotNames = new PlayMenu.SlotInfo[num];
		PlayMenu.playMenu = this;
	}

	private void OnDestroy()
	{
		PlayMenu.playMenu = null;
	}

	private void Start()
	{
		base.StartCoroutine(this._LoadAllMaps());
		this.loading.SetActive(false);
	}

	public PlayMenu.SlotInfo[] slotNames
	{
		get
		{
			return this._slotNames;
		}
		set
		{
			this._slotNames = value;
		}
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
			if (text == "NoName")
			{
				text = Localize.buildinMapName[TryConvert.ToInt32(array[4], 33)];
			}
		}
		else
		{
			text = Localize.buildinMapName[33];
		}
		this._slotNames[this._currentLoadingSlot] = new PlayMenu.SlotInfo();
		this._slotNames[this._currentLoadingSlot].name = text;
	}

	public void BuySlot(int slot)
	{
		if (this._slotNames.Length <= slot)
		{
			Array.Resize<PlayMenu.SlotInfo>(ref this._slotNames, slot + 1);
		}
	}

	public void LoadAllMaps()
	{
		base.StartCoroutine(this._LoadAllMaps());
	}

	private IEnumerator _LoadAllMaps()
	{
		int nn = Math.Min(Kube.GPS.playerNumMaps, 20);
		if (this._slotNames.Length != nn)
		{
			Array.Resize<PlayMenu.SlotInfo>(ref this._slotNames, nn);
		}
		for (int i = 0; i < nn; i++)
		{
			if (this._slotNames[i] == null)
			{
				long mySelectedMapId = (long)Kube.SS.serverId * 20L + (long)i;
				this._currentLoadingSlot = i;
				Kube.SS.LoadIsMap(mySelectedMapId, base.gameObject, "LoadIsMapDone");
				while (this._slotNames[i] == null)
				{
					yield return 1;
				}
			}
		}
		base.BroadcastMessage("LoadMapsDone", SendMessageOptions.DontRequireReceiver);
		yield break;
	}

	public GameObject loading;

	public static PlayMenu playMenu;

	private PlayMenu.SlotInfo[] _slotNames;

	protected int _currentLoadingSlot;

	public class SlotInfo
	{
		public string name;

		public int mapType;
	}
}
