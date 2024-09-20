using System;
using System.Collections.Generic;
using kube;
using LitJson;
using UnityEngine;

public class Top10Tab : MonoBehaviour
{
	private void OnEnable()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["mode"] = this.mode;
		Kube.SS.Request(201, dictionary, new ServerCallback(this.onTopDone));
	}

	public void onScroll()
	{
	}

	private void onTopDone(string ans)
	{
		JsonData jsonData = JsonMapper.ToObject(ans);
		JsonData jsonData2 = jsonData["data"];
		List<Top10Tab.Top10Record> list = new List<Top10Tab.Top10Record>();
		for (int i = 0; i < jsonData2.Count; i++)
		{
			JsonData jsonData3 = jsonData2[i];
			Top10Tab.Top10Record item = default(Top10Tab.Top10Record);
			if (jsonData3["name"] != null)
			{
				item.name = AuxFunc.DecodeRussianName(jsonData3["name"].ToString());
				item.exp = long.Parse(jsonData3["exp"].ToString());
				item.uid = jsonData3["uid"].ToString();
				list.Add(item);
			}
		}
		this.items = list.ToArray();
		this.Invalidate();
	}

	private static int sortXp(Top10Tab.Top10Record left, Top10Tab.Top10Record right)
	{
		return right.exp.CompareTo(left.exp);
	}

	private void onItemClick()
	{
		Top10Item component = UIButton.current.GetComponent<Top10Item>();
		Kube.SN.gotoUserByUID(component.uid);
	}

	private void Invalidate()
	{
		KGUITools.removeAllChildren(this.container.gameObject, true);
		if (this.items == null)
		{
			return;
		}
		Array.Sort<Top10Tab.Top10Record>(this.items, new Comparison<Top10Tab.Top10Record>(Top10Tab.sortXp));
		int num = Mathf.Min(300, this.items.Length);
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = this.container.gameObject.AddChild(this.itemPrefab);
			Top10Item component = gameObject.GetComponent<Top10Item>();
			component.title.text = (i + 1).ToString() + ". " + NGUIText.StripSymbols(this.items[i].name);
			component.nnplayers.text = this.items[i].exp.ToString();
			component.uid = this.items[i].uid;
			if (i < Top10Tab.modes.Length)
			{
				component.mode.spriteName = Top10Tab.modes[i];
			}
			else
			{
				component.mode.spriteName = string.Empty;
			}
			EventDelegate.Add(gameObject.GetComponent<UIButton>().onClick, new EventDelegate(new EventDelegate.Callback(this.onItemClick)));
		}
		this.container.GetComponent<UIGrid>().Reposition();
		this.container.ResetPosition();
	}

	public UIScrollView container;

	public GameObject itemPrefab;

	public string mode = "xp";

	private Top10Tab.Top10Record[] items;

	private static string[] modes = new string[]
	{
		"miss_8",
		"miss_9",
		"miss_10"
	};

	private struct Top10Record
	{
		public string uid;

		public string name;

		public long exp;

		public int kills;
	}
}
