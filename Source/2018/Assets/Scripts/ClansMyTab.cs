using System;
using System.Collections.Generic;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class ClansMyTab : MonoBehaviour
{
	private void Awake()
	{
		this.loading.SetActive(false);
		this.LoadAndShow();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void onLoaded(string response)
	{
		this.loading.SetActive(false);
		JsonData jsonData = JsonMapper.ToObject(response);
		this._addprice = (int)jsonData["price"];
		if (!jsonData.Keys.Contains("info"))
		{
			if (Kube.GPS.clan != null)
			{
				this.clanname.text = string.Format("{0} [{1}]", Kube.GPS.clan.name, Kube.GPS.clan.shortName);
			}
			this.Invalidate();
			return;
		}
		JsonData jsonData2 = jsonData["info"];
		if (!jsonData.Keys.Contains("info"))
		{
			return;
		}
		this.info = Clans.parseClan(jsonData2);
		this.items = Clans.parseMembers(jsonData["items"]);
		this.clanname.text = string.Format("{0} [{1}]", jsonData2["name"], jsonData2["sname"]);
		this.Invalidate();
	}

	private void OnEnable()
	{
		this.LoadAndShow();
	}

	public void LoadAndShow()
	{
		bool flag = false;
		if (Kube.GPS.clan != null)
		{
			flag = true;
		}
		this.loading.SetActive(true);
		Kube.SS.Request(832, null, new ServerCallback(this.onLoaded));
		KGUITools.removeAllChildren(this.container.gameObject, false);
		this.container.ResetPosition();
		this.myclan.SetActive(flag);
		this.editclan.SetActive(flag && Kube.GPS.clan.owner == Kube.SS.serverId);
	}

	private void BuyNewMapDone()
	{
		this.Invalidate();
	}

	private void Invalidate()
	{
		if (Kube.GPS == null)
		{
			return;
		}
		KGUITools.removeAllChildren(this.container.gameObject, false);
		if (Kube.GPS.clan == null)
		{
			this.addslot.SetActive(true);
			this.addslot.GetComponentInChildren<UILabel>().text = string.Format(Localize.createForX, this._addprice);
		}
		else
		{
			this.leave.SetActive(Kube.GPS.clan.owner != Kube.SS.serverId);
		}
		if (this.items == null)
		{
			return;
		}
		int height = this.itemPrefab.GetComponent<UIWidget>().height;
		int num = 0;
		Vector3 localPosition = Vector3.zero;
		int num2 = this.items.Length;
		for (int i = 0; i < num2; i++)
		{
			GameObject gameObject;
			if (this._hash.ContainsKey(this.items[i].id))
			{
				gameObject = this._hash[this.items[i].id];
				gameObject.SetActive(true);
			}
			else
			{
				gameObject = this.container.gameObject.AddChild(this.itemPrefab);
				this._hash[this.items[i].id] = gameObject;
			}
			localPosition = gameObject.transform.localPosition;
			localPosition.y = (float)num;
			num -= height + 5;
			gameObject.transform.localPosition = localPosition;
			MemberItem component = gameObject.GetComponent<MemberItem>();
			component.info = this.items[i];
			component.title.text = this.items[i].name;
			component.id.text = this.items[i].uid.ToString();
			if (this.items[i].id == Kube.SS.serverId)
			{
				component.no.gameObject.SetActive(false);
			}
			if (this.items[i].id == Kube.SS.serverId || this.items[i].type == 1)
			{
				component.yes.gameObject.SetActive(false);
			}
		}
		this.container.ResetPosition();
	}

	public void onBuySlot()
	{
		MyClanDialog myClanDialog = Cub2UI.FindAndOpenDialog<MyClanDialog>("dialog_new_clan");
		myClanDialog.owner = this;
		myClanDialog.info = null;
	}

	private void onCreated(string responce)
	{
		JsonData jsonData = JsonMapper.ToObject(responce);
		UnityEngine.Debug.Log(responce);
		if ((int)jsonData["r"] == 0)
		{
			Cub2UI.MessageBox(Localize.clan_fail_new, null);
			return;
		}
		if (!jsonData.Keys.Contains("cid") || jsonData["cid"].ToString() == "0")
		{
			Cub2UI.MessageBox(Localize.clan_fail_new, null);
			return;
		}
		this.info.id = int.Parse(jsonData["cid"].ToString());
		this.info.owner = Kube.SS.serverId;
		Kube.GPS.clan = this.info;
		this.addslot.SetActive(false);
		this.LoadAndShow();
	}

	public void createClan(ClanInfo info)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["name"] = info.name.ToString();
		dictionary["sname"] = info.shortName.ToString();
		dictionary["home"] = info.home.ToString();
		this.info = info;
		Kube.SS.Request(831, dictionary, new ServerCallback(this.onCreated));
	}

	public void updateClan(ClanInfo info)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["name"] = info.name.ToString();
		dictionary["sname"] = info.shortName.ToString();
		dictionary["clan"] = info.id.ToString();
		dictionary["home"] = info.home.ToString();
		this.info = info;
		Kube.SS.Request(834, dictionary, null);
	}

	private void changeMember(int id, int type)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["other"] = id.ToString();
		dictionary["t"] = type.ToString();
		Kube.SS.Request(833, dictionary, null);
	}

	public void onYesMember(MemberItem memberItem)
	{
		this.changeMember(memberItem.info.id, 1);
		memberItem.yes.gameObject.SetActive(false);
	}

	public void onNoMember(MemberItem memberItem)
	{
		this.changeMember(memberItem.info.id, 2);
		memberItem.no.gameObject.SetActive(false);
	}

	public void onMember(MemberItem memberItem)
	{
		Kube.SN.gotoUserByUID(memberItem.info.uid);
	}

	public void onEdit()
	{
		MyClanDialog myClanDialog = Cub2UI.FindAndOpenDialog<MyClanDialog>("dialog_new_clan");
		myClanDialog.owner = this;
		myClanDialog.info = this.info;
	}

	public void onDelete()
	{
		Dictionary<string, string> p = new Dictionary<string, string>();
		p["clan"] = Kube.GPS.clan.id.ToString();
		Cub2UI.MessageBox(Localize.ys_delete, delegate()
		{
			Kube.GPS.clan = null;
			Kube.SS.Request(837, p, delegate(string ans)
			{
				this.LoadAndShow();
			});
		});
	}

	private void onLeaveAns(string responce)
	{
		Kube.GPS.clan = null;
		this.OnEnable();
	}

	public void onLeaveClick()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["cid"] = Kube.GPS.clan.id.ToString();
		Kube.SS.Request(836, dictionary, new ServerCallback(this.onLeaveAns));
	}

	private const int MAX_MAPS = 20;

	public UILabel clanname;

	public GameObject addslot;

	public GameObject leave;

	public GameObject myclan;

	public GameObject editclan;

	protected ClanInfo info = new ClanInfo();

	public UIScrollView container;

	public GameObject hint;

	protected int _addprice = 50;

	public GameObject loading;

	public GameObject itemPrefab;

	private ClanMember[] items;

	private Dictionary<int, GameObject> _hash = new Dictionary<int, GameObject>();
}
