using System;
using System.Collections.Generic;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class ClansListTab : MonoBehaviour
{
	// Note: this type is marked as 'beforefieldinit'.
	static ClansListTab()
	{
		int[] array = new int[4];
		array[0] = 1;
		array[1] = 7;
		array[2] = 30;
		ClansListTab.daycount = array;
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
		this.LoadItems(ClansListTab.daycount[DayToggle.current.state]);
	}

	private void onLoaded(string response)
	{
		this.container.ResetPosition();
		JsonData jsonData = JsonMapper.ToObject(response);
		this.items = Clans.parse(jsonData["items"]);
		this.xref = Clans.parseXRef(jsonData["xref"]);
		this.valid = false;
		this.Invalidate();
	}

	private void LoadItems(int i)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["d"] = i.ToString();
		Kube.SS.Request(830, dictionary, new ServerCallback(this.onLoaded));
	}

	private void Find(string text)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["find"] = text;
		Kube.SS.Request(830, dictionary, new ServerCallback(this.onLoaded));
	}

	private void OnEnable()
	{
		this.valid = false;
		this.LoadItems(1);
	}

	private void Hit(int id)
	{
	}

	private string bignumber(int value)
	{
		if (value > 1000000)
		{
			return (value / 1000000).ToString() + "M";
		}
		if (value > 1000)
		{
			return (value / 1000).ToString() + "K";
		}
		return value.ToString();
	}

	public void onScroll()
	{
		if (!this.valid)
		{
			return;
		}
		if (this.items != null && this.items.Length > this.visibleLimit && UIProgressBar.current.value == 1f)
		{
			UIProgressBar.current.value = (float)((this.visibleLimit + 100) / this.visibleLimit);
			this.visibleLimit += 100;
			this.valid = false;
			this.Invalidate();
		}
	}

	private void Invalidate()
	{
		if (this.valid)
		{
			return;
		}
		ClanInfo[] array = this.selectRooms();
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
		int num = Math.Min(this.visibleLimit, array.Length);
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
			ClanItem component = gameObject.GetComponent<ClanItem>();
			component.title.text = string.Format("{0}. {1} [{2}]", i + 1, array[i].name, array[i].shortName.ToUpper());
			component.nnplayers.text = array[i].players.ToString();
			component.id = array[i].id;
			component.nnfrags.text = this.bignumber(array[i].frags);
			component.nnkills.text = this.bignumber(array[i].kills);
			component.info = array[i];
			gameObject.name = i.ToString("D6");
		}
		for (int j = 0; j < list.Count; j++)
		{
			GameObject gameObject2 = list[j];
			ClanItem component2 = gameObject2.GetComponent<ClanItem>();
			this._hash.Remove(component2.id);
			gameObject2.SetActive(false);
			UnityEngine.Object.Destroy(gameObject2);
		}
		this.container.GetComponent<UIGrid>().Reposition();
		this.container.UpdatePosition();
		this.valid = true;
	}

	private void onItemClick()
	{
		ClanItem component = UIButton.current.GetComponent<ClanItem>();
		this.Hit(component.info.id);
		ClanDialog clanDialog = Cub2UI.FindAndOpenDialog<ClanDialog>("dialog_clan");
		clanDialog.owner = this;
		clanDialog.canJoin = (this.xref.Count < 3 && Kube.GPS.clan == null && !this.xref.ContainsKey(component.info.id));
		clanDialog.info = component.info;
	}

	private ClanInfo[] selectRooms()
	{
		return this.items;
	}

	private void onJoined(string responce)
	{
		JsonData jsonData = JsonMapper.ToObject(responce);
		UnityEngine.Debug.Log(responce);
	}

	public void join(int id)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["cid"] = id.ToString();
		this.xref[id] = true;
		Kube.SS.Request(835, dictionary, new ServerCallback(this.onJoined));
	}

	public UIScrollView container;

	public static string[] modeSprites = new string[]
	{
		string.Empty,
		"4_oo",
		"2_oo",
		"1_oo",
		"3_oo",
		"flag",
		"flag",
		"domin_1",
		"flag"
	};

	public DayToggle daytoggle;

	private static int[] daycount;

	private ClanInfo[] items;

	private Dictionary<int, bool> xref;

	private bool valid;

	private float fullUpdate;

	private int numGamesWithFriends;

	public GameObject itemPrefab;

	private Dictionary<int, GameObject> _hash;

	public int visibleLimit = 100;
}
