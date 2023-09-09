using System;
using System.Collections;
using System.Collections.Generic;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class TaskDialogStory : TaskDialogBase
{
	private void Awake()
	{
		this.TaskTypeDesc = new TaskDialogBase.TaskDescGet[]
		{
			null,
			new TaskDialogBase.TaskDescGet(base.TaskGainBonus),
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
		if (Kube.RM != null)
		{
			Kube.RM.require("Assets2_MenuItems", null);
		}
		this.story = TaskBox.FindStory(this.taskDesc.parrent);
		string text;
		if (this.TaskTypeDesc[(int)this.taskDesc.type] != null)
		{
			text = this.TaskTypeDesc[(int)this.taskDesc.type](this.taskDesc.config);
		}
		else
		{
			text = Localize.TaskType[(int)this.taskDesc.type];
		}
		this.desc.text = text;
		int num = Array.IndexOf<TaskDesc>(this.story.story, this.taskDesc);
		TaskDesc taskDesc = this.story.story[this.story.story.Length - 1];
		BonusDesc[] array = new BonusDesc[5];
		taskDesc.bonus.Keys.CopyTo(array, 0);
		this.alltx.mainTexture = Kube.ASS2.inventarWeaponsTex[array[1].id];
		this.alldesc.text = string.Format(Localize.task_story, Localize.weaponNames[array[1].id]);
		this.progress.text = string.Format("{0}/{1}", this.taskDesc.progress[0], this.taskDesc.config[0]);
		this.title.text = Localize.T(this.taskDesc.title, null) + " " + string.Format("{0}/{1}", num, this.story.story.Length);
		this.prize2.GetComponent<UITexture>().mainTexture = Kube.ASS2.gameItemsTex[189];
		if (this.taskDesc.bonus == null)
		{
			return;
		}
		bool isEnabled = (int)this.taskDesc.progress[0] >= (int)this.taskDesc.config[0];
		if (this.taskDesc.score > 0)
		{
			isEnabled = false;
		}
		this.done.isEnabled = isEnabled;
	}

	private void onScore(string ans)
	{
		JsonData jsonData = JsonMapper.ToObject(ans);
		this.OnEnable();
		if (jsonData.Keys.Contains("money"))
		{
			GameParamsScript gps = Kube.GPS;
			gps.playerMoney1 += int.Parse(jsonData["money"].ToString());
		}
		if (jsonData.Keys.Contains("bonus"))
		{
			char[] separator = new char[]
			{
				';'
			};
			string[] arr = jsonData["bonus"].ToString().Split(separator);
			base.StartCoroutine(this._BonusQue(arr));
		}
		Kube.SendMonoMessage("InvalidateTasks", new object[0]);
	}

	private IEnumerator _BonusQue(string[] arr)
	{
		int nn = arr.Length / 2;
		for (int i = 0; i < nn; i++)
		{
			FastInventar fi = FastInventar.Parse(arr[i * 2]);
			CaseGiftDialog dlg = Cub2UI.FindAndOpenDialog<CaseGiftDialog>("dialog_casegift");
			dlg.fi = fi;
			if (fi.Type == 4)
			{
				Kube.GPS.inventarWeapons[fi.Num] = 1;
			}
			else
			{
				GameParamsScript.InventarItems inventarItems2;
				GameParamsScript.InventarItems inventarItems = inventarItems2 = Kube.GPS.inventarItems;
				int num;
				int index = num = fi.Num;
				num = inventarItems2[num];
				inventarItems[index] = num + 1;
			}
			dlg.Open(false);
			yield return base.StartCoroutine(this._WaitDialog(dlg));
		}
		base.gameObject.SetActive(false);
		yield break;
	}

	private IEnumerator _WaitDialog(CaseGiftDialog dlg)
	{
		yield return new WaitForSeconds(1f);
		while (dlg.gameObject.activeSelf)
		{
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	public void onClick()
	{
		if (this.taskDesc.score > 0)
		{
			return;
		}
		if ((int)this.taskDesc.progress[0] < (int)this.taskDesc.config[0])
		{
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		this.taskDesc.score = 597;
		dictionary["task"] = this.taskDesc.id.ToString();
		dictionary["score"] = this.taskDesc.score.ToString();
		dictionary["progress"] = TaskBox.EncodeProgress(this.taskDesc.progress);
		Kube.SS.Request(300, dictionary, new ServerCallback(this.onScore));
	}

	public UILabel title;

	public UILabel desc;

	public UILabel alldesc;

	public UITexture alltx;

	public UILabel progress;

	public GameObject prize2;

	public UIButton done;

	protected TaskStoryDesc story;
}
