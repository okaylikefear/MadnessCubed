using System;
using System.Collections.Generic;
using kube;
using kube.data;
using UnityEngine;

public class TaskDoneDialog : MonoBehaviour
{
	private void Start()
	{
	}

	public void Open(TaskDesc task, EndGameStats endGameStats)
	{
		this._taskId = task.id;
		this.tadskDesc = task;
		this._endGameStats = endGameStats;
		base.gameObject.SetActive(true);
	}

	private string int_join(char separator, int[] arr)
	{
		string text = string.Empty;
		for (int i = 0; i < arr.Length; i++)
		{
			if (i > 0)
			{
				text += ";";
			}
			text += arr[i].ToString();
		}
		return text;
	}

	private void Invalidate()
	{
		int num = 0;
		TaskDesc taskDesc = this.tadskDesc;
		foreach (KeyValuePair<BonusDesc, int> keyValuePair in taskDesc.bonus)
		{
			BonusDesc key = keyValuePair.Key;
			GameObject gameObject = this.container.gameObject.AddChild(this.itemPrefab);
			if (key.type == 0)
			{
				Texture texture = Kube.ASS2.gameItemsTex[key.id];
			}
			else
			{
				Texture texture = Kube.ASS2.inventarWeaponsTex[key.id];
			}
			ItemDescIcon component = gameObject.GetComponent<ItemDescIcon>();
			component.count = keyValuePair.Value;
			component.itemType = key.type;
			component.itemId = key.id;
			num++;
		}
		if (taskDesc.score <= 0)
		{
			this.money1.text = taskDesc.money.ToString();
		}
		else
		{
			this.money1.text = (taskDesc.money / 10).ToString();
		}
		foreach (KeyValuePair<BonusDesc, int> keyValuePair2 in taskDesc.bonus)
		{
			BonusDesc key2 = keyValuePair2.Key;
			if (key2.type == 0)
			{
				GameParamsScript.InventarItems inventarItems2;
				GameParamsScript.InventarItems inventarItems = inventarItems2 = Kube.GPS.inventarItems;
				int num2;
				int index = num2 = key2.id;
				num2 = inventarItems2[num2];
				inventarItems[index] = num2 + keyValuePair2.Value;
			}
			else
			{
				Kube.GPS.inventarWeapons[key2.id] = 1;
			}
		}
		this.container.GetComponentInChildren<UIGrid>().Reposition();
	}

	private void OnEnable()
	{
		KGUITools.removeAllChildren(this.container.gameObject, true);
		this.Invalidate();
		int num = this._endGameStats.deltaExp;
		if (num <= 0)
		{
			num = 100;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["id"] = Kube.SS.serverId.ToString();
		dictionary["score"] = num.ToString();
		dictionary["task"] = this._taskId.ToString();
		TaskBox.updateTask(this._taskId, num);
		Kube.SS.Request(300, dictionary, delegate(string str)
		{
			Kube.OH.SendMessage("TaskEnd", str);
		});
	}

	private void Update()
	{
	}

	public void postAndExit()
	{
		this.exitDialog();
	}

	public void exitDialog()
	{
		if (Kube.BCS != null)
		{
			PhotonNetwork.LeaveRoom();
			UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	protected const int MAX_ITEMS_IN_GUIROW = 4;

	public UILabel money1;

	protected TaskDesc tadskDesc;

	public GameObject container;

	public GameObject itemPrefab;

	private int _taskId;

	private string endGameCapture;

	private EndGameStats _endGameStats;
}
