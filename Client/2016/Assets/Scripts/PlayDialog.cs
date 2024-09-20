using System;
using System.Collections.Generic;
using kube;
using kube.data;
using UnityEngine;

public class PlayDialog : MonoBehaviour
{
	// Note: this type is marked as 'beforefieldinit'.
	static PlayDialog()
	{
		PlayDialog.MissionDescGet[] array = new PlayDialog.MissionDescGet[8];
		array[2] = new PlayDialog.MissionDescGet(PlayDialog.MissionHoldNSecond);
		array[3] = new PlayDialog.MissionDescGet(PlayDialog.MissionKillNMonsters);
		array[4] = new PlayDialog.MissionDescGet(PlayDialog.MissionFindDesc);
		array[5] = new PlayDialog.MissionDescGet(PlayDialog.MissionFindDesc2);
		array[6] = new PlayDialog.MissionDescGet(PlayDialog.MissionKillNMonstersNSecond);
		PlayDialog.MissionTypeDesc = array;
		PlayDialog.SecondNames_RU = new string[]
		{
			Localize.seconds,
			Localize.secondu,
			Localize.secondy,
			Localize.secondy,
			Localize.secondy,
			Localize.seconds,
			Localize.seconds,
			Localize.seconds,
			Localize.seconds,
			Localize.seconds
		};
		PlayDialog.MinuteNames_RU = new string[]
		{
			Localize.minutes,
			Localize.minutu,
			Localize.minuty,
			Localize.minuty,
			Localize.minuty,
			Localize.minutes,
			Localize.minutes,
			Localize.minutes,
			Localize.minutes,
			Localize.minutes
		};
		PlayDialog.MonsterNames_RU = new string[]
		{
			Localize.monsters,
			Localize.monstra,
			Localize.monstra,
			Localize.monstra,
			Localize.monstra,
			Localize.monsters,
			Localize.monsters,
			Localize.monsters,
			Localize.monsters,
			Localize.monsters
		};
	}

	private static int calcNameIndex(int index)
	{
		int result = index % 10;
		if (index > 9 && index < 21)
		{
			return 0;
		}
		return result;
	}

	private static string formatTime(int index)
	{
		int num = PlayDialog.calcNameIndex(index);
		if (index > 60)
		{
			index /= 60;
			num = PlayDialog.calcNameIndex(index);
			return string.Format("{0} {1} ", index, PlayDialog.MinuteNames_RU[num]);
		}
		return string.Format("{0} {1} ", index, PlayDialog.SecondNames_RU[num]);
	}

	private static string MissionHoldNSecond(ObjectsHolderScript OH, object[] config)
	{
		int num = (int)config[1];
		return string.Format(Localize.MissionType[2], PlayDialog.formatTime(num));
	}

	private static string MissionKillNMonsters(ObjectsHolderScript OH, object[] config)
	{
		int num = (int)config[0];
		int num2 = PlayDialog.calcNameIndex(num);
		return string.Format(Localize.MissionType[3], num, PlayDialog.MonsterNames_RU[num2]);
	}

	private static string MissionFindDesc(ObjectsHolderScript OH, object[] config)
	{
		int num = (int)config[0];
		return string.Format(Localize.MissionType[4], Localize.findPrefabsNames[num]);
	}

	private static string MissionFindDesc2(ObjectsHolderScript OH, object[] config)
	{
		int num = (int)config[0];
		int num2 = (int)config[1];
		return string.Format(Localize.MissionType[5], Localize.findPrefabsNames[num], PlayDialog.formatTime(num2));
	}

	private static string MissionKillNMonstersNSecond(ObjectsHolderScript OH, object[] config)
	{
		int num = (int)config[0];
		int num2 = PlayDialog.calcNameIndex(num);
		int num3 = (int)config[1];
		return string.Format(Localize.MissionType[6], num, PlayDialog.MonsterNames_RU[num2], PlayDialog.formatTime(num3));
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public static string GetMissionDesc(ObjectsHolderScript OH, MissionDesc missionDesc)
	{
		string result;
		if (PlayDialog.MissionTypeDesc[missionDesc.type] != null)
		{
			result = PlayDialog.MissionTypeDesc[missionDesc.type](OH, missionDesc.config);
		}
		else
		{
			result = Localize.MissionType[missionDesc.type];
		}
		return result;
	}

	public void OnEnable()
	{
		if (Kube.SS != null)
		{
			Kube.RM.require("Assets2_MenuItems", null);
		}
		if (this.missionDesc.bonus == null)
		{
			return;
		}
		this.title.text = this.missionDesc.title;
		if (PlayDialog.MissionTypeDesc[this.missionDesc.type] != null)
		{
			this.desc.text = PlayDialog.MissionTypeDesc[this.missionDesc.type](Kube.OH, this.missionDesc.config);
		}
		else
		{
			this.desc.text = Localize.MissionType[this.missionDesc.type];
		}
		if (this.missionDesc.gold > 0 && this.missionDesc.score <= 0)
		{
			this.prize.value = this.missionDesc.gold;
			this.prize.isGold = true;
		}
		else
		{
			this.prize.value = this.missionDesc.money;
			this.prize.isGold = false;
		}
		KGUITools.removeAllChildren(this.container.gameObject, true);
		foreach (KeyValuePair<BonusDesc, int> keyValuePair in this.missionDesc.bonus)
		{
			BonusDesc key = keyValuePair.Key;
			GameObject gameObject = this.container.gameObject.AddChild(this.itemPrefab);
			string itemname = string.Empty;
			if (Kube.ASS2)
			{
				if (key.type == 0)
				{
					gameObject.GetComponentInChildren<ItemDescIcon>().tx.mainTexture = Kube.OH.gameItemsTex[key.id];
					itemname = Localize.gameItemsNames[keyValuePair.Key.id];
				}
				else
				{
					gameObject.GetComponentInChildren<ItemDescIcon>().tx.mainTexture = Kube.ASS2.inventarWeaponsTex[key.id];
					itemname = Localize.weaponNames[keyValuePair.Key.id];
				}
			}
			if (key.type == 0)
			{
				itemname = Localize.gameItemsNames[keyValuePair.Key.id];
			}
			else
			{
				itemname = Localize.weaponNames[keyValuePair.Key.id];
			}
			ItemDescIcon component = gameObject.GetComponent<ItemDescIcon>();
			component.itemType = key.type;
			component.count = keyValuePair.Value;
			component.itemId = key.id;
			component.itemname = itemname;
		}
		this.container.GetComponentInChildren<UIGrid>().Reposition();
	}

	public void onClick()
	{
		OnlineManager.instance.PlayMission(this.missionDesc, this.missionDesc.offline);
	}

	public GameObject itemPrefab;

	private static PlayDialog.MissionDescGet[] MissionTypeDesc;

	private static string[] SecondNames_RU;

	private static string[] MinuteNames_RU;

	private static string[] MonsterNames_RU;

	public UILabel title;

	public UILabel desc;

	public PriceButton prize;

	public UIPanel container;

	[NonSerialized]
	public MissionDesc missionDesc;

	public int index;

	private delegate string MissionDescGet(ObjectsHolderScript OH, object[] config);
}
