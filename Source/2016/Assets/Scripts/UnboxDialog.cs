using System;
using System.Collections.Generic;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class UnboxDialog : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnEnable()
	{
		this.title.text = string.Empty;
		this.takeBtn.enabled = false;
		KGUITools.removeAllChildren(this.container.gameObject, true);
	}

	public void Invalidate()
	{
		if (Kube.SS != null)
		{
			Kube.RM.require("Assets2_MenuItems", null);
		}
		this.takeBtn.enabled = true;
		KGUITools.removeAllChildren(this.container.gameObject, true);
		foreach (KeyValuePair<FastInventar, int> keyValuePair in this.bonus)
		{
			FastInventar key = keyValuePair.Key;
			GameObject gameObject = this.container.gameObject.AddChild(this.itemPrefab);
			string text = string.Empty;
			if (Kube.ASS2)
			{
				if (key.Type == 3)
				{
					gameObject.GetComponentInChildren<ItemDescIcon>().tx.mainTexture = Kube.OH.gameItemsTex[key.Num];
					text = Localize.gameItemsNames[keyValuePair.Key.Num];
				}
				else if (key.Type == 4)
				{
					gameObject.GetComponentInChildren<ItemDescIcon>().tx.mainTexture = Kube.ASS2.inventarWeaponsTex[key.Num];
					text = Localize.weaponNames[keyValuePair.Key.Num];
				}
			}
			ItemDescIcon component = gameObject.GetComponent<ItemDescIcon>();
			component.fi = key;
			component.count = keyValuePair.Value;
		}
		this.container.GetComponentInChildren<UIGrid>().Reposition();
	}

	private int legacyType(int p)
	{
		switch (p)
		{
		case 3:
			return 0;
		case 4:
			return 1;
		case 5:
			return 2;
		default:
			return 0;
		}
	}

	public void onClick()
	{
		base.gameObject.SetActive(false);
	}

	protected void onUnbox(string response)
	{
		if (response.StartsWith("error"))
		{
			return;
		}
		string b = response.Substring(response.Length - 32);
		string text = response.Substring(0, response.Length - 32);
		if (AuxFunc.GetMD5(text + Kube.SS.phpSecret) != b)
		{
			Kube.Ban();
			return;
		}
		GameParamsScript.InventarItems inventarItems2;
		GameParamsScript.InventarItems inventarItems = inventarItems2 = Kube.GPS.inventarItems;
		int num;
		int index = num = this._box_id;
		num = inventarItems2[num];
		inventarItems[index] = num - 1;
		JsonData jsonData = JsonMapper.ToObject(text);
		this.bonus = MissionHelper.parseBonusFI(jsonData["arr"].ToString());
		foreach (KeyValuePair<FastInventar, int> keyValuePair in this.bonus)
		{
			FastInventar key = keyValuePair.Key;
			if (key.Type == 3)
			{
				GameParamsScript.InventarItems inventarItems4;
				GameParamsScript.InventarItems inventarItems3 = inventarItems4 = Kube.GPS.inventarItems;
				int index2 = num = keyValuePair.Key.Num;
				num = inventarItems4[num];
				inventarItems3[index2] = num + keyValuePair.Value;
			}
			else if (key.Type == 7)
			{
				Kube.GPS.inventarSpecItems[keyValuePair.Key.Num] = DataUtils.TimeAdd(Kube.GPS.inventarSpecItems[keyValuePair.Key.Num], keyValuePair.Value * 86400);
			}
			else if (key.Type == 4)
			{
				Kube.GPS.inventarWeapons[keyValuePair.Key.Num] = DataUtils.TimeAdd(Kube.GPS.inventarWeapons[keyValuePair.Key.Num], keyValuePair.Value * 86400);
			}
			else if (key.Type == 5)
			{
				Kube.GPS.weaponsSkin[keyValuePair.Key.Num] = 1;
			}
		}
		this.Invalidate();
	}

	public void Open(int p)
	{
		this._box_id = p;
		this.takeBtn.enabled = false;
		this.title.text = Localize.gameItemsNames[p];
		this.desc.text = Localize.gameItemsDesc[p];
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["box_id"] = p.ToString();
		Kube.SS.Request(905, dictionary, new ServerCallback(this.onUnbox));
	}

	public GameObject itemPrefab;

	public UILabel title;

	public UILabel desc;

	public UIButton takeBtn;

	public UIPanel container;

	protected Dictionary<FastInventar, int> bonus;

	protected int _box_id = -1;
}
