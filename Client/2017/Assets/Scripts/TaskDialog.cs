using System;
using System.Collections.Generic;
using kube;
using kube.data;
using UnityEngine;

public class TaskDialog : TaskDialogBase
{
	private void Awake()
	{
		this.TaskTypeDesc = new TaskDialogBase.TaskDescGet[]
		{
			null,
			null,
			new TaskDialogBase.TaskDescGet(base.TaskHoldNSecond),
			new TaskDialogBase.TaskDescGet(base.TaskKillNMonsters),
			new TaskDialogBase.TaskDescGet(base.TaskKillNPlayers),
			null,
			new TaskDialogBase.TaskDescGet(base.TaskKillNMonstersNSecond),
			new TaskDialogBase.TaskDescGet(base.TaskBuyWeapon),
			new TaskDialogBase.TaskDescGet(base.TaskBuyItem)
		};
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public string GetTaskDesc(ObjectsHolderScript OH, TaskDesc taskDesc)
	{
		string result;
		if (this.TaskTypeDesc[(int)taskDesc.type] != null)
		{
			result = this.TaskTypeDesc[(int)taskDesc.type](taskDesc.config);
		}
		else
		{
			result = Localize.TaskType[(int)taskDesc.type];
		}
		return result;
	}

	public new void Open(TaskDesc taskDesc)
	{
		this.taskDesc = taskDesc;
		base.gameObject.SetActive(true);
	}

	public void OnEnable()
	{
		if (Kube.SS != null)
		{
			Kube.RM.require("Assets2_MenuItems", null);
		}
		this.title.text = this.taskDesc.title;
		if (this.TaskTypeDesc[(int)this.taskDesc.type] != null)
		{
			this.desc.text = this.TaskTypeDesc[(int)this.taskDesc.type](this.taskDesc.config);
		}
		else
		{
			this.desc.text = Localize.TaskType[(int)this.taskDesc.type];
		}
		this.money1.text = this.taskDesc.money.ToString();
		this.icon.spriteName = this.taskDesc.ico;
		if (this.taskDesc.bonus == null)
		{
			return;
		}
		KGUITools.removeAllChildren(this.container.gameObject, true);
		foreach (KeyValuePair<BonusDesc, int> keyValuePair in this.taskDesc.bonus)
		{
			BonusDesc key = keyValuePair.Key;
			GameObject gameObject = this.container.gameObject.AddChild(this.itemPrefab);
			string itemname = string.Empty;
			if (Kube.ASS2)
			{
				if (key.type == 0)
				{
					gameObject.GetComponentInChildren<ItemDescIcon>().tx.mainTexture = Kube.ASS2.gameItemsTex[key.id];
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
		TaskBase.Validate();
	}

	public GameObject itemPrefab;

	public UILabel title;

	public UILabel desc;

	public UILabel money1;

	public GameObject prize2;

	public UISprite icon;

	public GameObject container;

	public int index;
}
