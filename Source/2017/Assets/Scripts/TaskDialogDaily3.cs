using System;
using System.Collections.Generic;
using System.Linq;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class TaskDialogDaily3 : TaskDialogBase
{
	public void Awake()
	{
		this.items = base.GetComponentsInChildren<UISubTaskItem>();
	}

	public void OnEnable()
	{
		TaskDayli3 taskDayli = (TaskDayli3)TaskBase.Get(this.taskDesc);
		for (int i = 0; i < this.items.Length; i++)
		{
			this.GetTextTask(this.items[i], taskDayli.subtask[i], (int)this.taskDesc.progress[i], this.taskDesc.bonus.Values.ElementAt(i), (this.taskDesc.score & 1 << i) != 0);
		}
	}

	private void GetTextTask(UISubTaskItem item, SubTaskDesc st, int c1, int value, bool collected)
	{
		bool active = !collected && c1 >= st.target;
		if (c1 == -1)
		{
			c1 = st.target;
		}
		item.TextTask.text = string.Format(Localize.DailyTask[st.type], Localize.T("ui_wp" + st.kind, null));
		item.PriseTask.text = value.ToString();
		if (!collected)
		{
			item.ProgressTask.text = c1.ToString() + "/" + st.target;
		}
		else
		{
			item.ProgressTask.text = Localize.ui_daily3_done;
			item.complete.value = true;
		}
		item.btn.gameObject.SetActive(active);
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
		if (jsonData.Keys.Contains("gift"))
		{
			char[] separator = new char[]
			{
				';'
			};
			string[] array = jsonData["gift"].ToString().Split(separator);
			FastInventar fi = FastInventar.Parse(array[0]);
			CaseGiftDialog caseGiftDialog = Cub2UI.FindAndOpenDialog<CaseGiftDialog>("dialog_casegift");
			caseGiftDialog.fi = fi;
			GameParamsScript.InventarItems inventarItems2;
			GameParamsScript.InventarItems inventarItems = inventarItems2 = Kube.GPS.inventarItems;
			int num;
			int index = num = fi.Num;
			num = inventarItems2[num];
			inventarItems[index] = num + 1;
			caseGiftDialog.Open(false);
		}
	}

	public void onBtn()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < this.items.Length; i++)
		{
			if (UIButton.current == this.items[i].btn)
			{
				this.taskDesc.score |= 1 << i;
				break;
			}
		}
		dictionary["task"] = this.taskDesc.id.ToString();
		dictionary["score"] = this.taskDesc.score.ToString();
		dictionary["progress"] = TaskBox.EncodeProgress(this.taskDesc.progress);
		Kube.SS.Request(301, dictionary, new ServerCallback(this.onScore));
	}

	public GameObject Task1;

	public GameObject Task2;

	public GameObject Task3;

	protected UISubTaskItem[] items;
}
