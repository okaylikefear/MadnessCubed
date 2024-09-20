using System;
using kube;
using kube.data;
using UnityEngine;

public class HomeMenu : MonoBehaviour
{
	private void onMissions()
	{
	}

	private void Start()
	{
		if (Kube.GPS == null)
		{
			return;
		}
		Kube.RM.require("Assets2", null);
		this.version.text = "C.4.1 O68 R" + Kube.OH.build;
		if (!this.viral_dialog)
		{
			this.viral_dialog = Cub2Menu.Find<ViralDialog>("dialog_viral");
		}
		this.nickname.text = Kube.GPS.decodePlayerName;
		this.nickname.label.text = Kube.GPS.decodePlayerName;
		KGUITools.removeAllChildren(this.offers.gameObject, true);
		Offer[] array = OfferBox.list();
		for (int i = 0; i < array.Length; i++)
		{
			int num = array[i].type - 1;
			GameObject gameObject = this.offers.gameObject.AddChild(this.offerPrefab);
			UIOfferItem component = gameObject.GetComponent<UIOfferItem>();
			component.offer = array[i];
		}
		PackInfo[] array2 = PackBox.list();
		int num2 = 0;
		while (num2 < array2.Length && this.offers.transform.childCount < 4)
		{
			GameObject gameObject2 = this.offers.gameObject.AddChild(this.packPrefab);
			UIPackItem component2 = gameObject2.GetComponent<UIPackItem>();
			component2.info = array2[num2];
			num2++;
		}
		this.offers.Reposition();
		MissionBox.request(new VoidCallback(this.onMissions), false);
		this.InvalidateTasks();
	}

	private void InvalidateTasks()
	{
		KGUITools.removeAllChildren(this.tasks.gameObject, false);
		Kube.OH.tasks = TaskBox.selectTasks();
		if (!Kube.SN.isQuestDone())
		{
			this.viral.gameObject.SetActive(true);
			this.viral.transform.parent = this.tasks.gameObject.transform;
		}
		TaskDesc[] array = Kube.OH.tasks;
		Kube.OH.tasks = array;
		int num = 0;
		while (num < array.Length && num < 3)
		{
			int type = (int)array[num].type;
			GameObject gameObject = this.tasks.gameObject.AddChild(this.taskPrefab);
			UITaskItem component = gameObject.GetComponent<UITaskItem>();
			component.desc = array[num];
			num++;
		}
		this.tasks.Reposition();
		TaskBase.Validate();
	}

	public void ShowTask(TaskDesc desc)
	{
		TaskDialogBase taskDialogBase;
		if (desc.type == TaskType.daily3)
		{
			taskDialogBase = Cub2UI.FindDialog<TaskDialogDaily3>("dialog_daily3");
		}
		else
		{
			taskDialogBase = Cub2UI.FindDialog<TaskDialogBase>("dialog_task");
		}
		taskDialogBase.Open(desc);
	}

	public void onAssetsLoaded(int id)
	{
	}

	private void Update()
	{
		if (UIInput.selection != this.nickname && this.nickname.value != Kube.GPS.decodePlayerName)
		{
			this.onNicknameSubmit();
		}
	}

	public void onSecretPlay()
	{
		MissionDesc[] array = MissionBox.selectMissions(100);
		OnlineManager.instance.PlayMission(array[0], array[0].offline);
	}

	public void onQuickPlay()
	{
		OnlineManager.instance.QuickPlay();
	}

	public void onViral()
	{
		this.viral_dialog.gameObject.SetActive(true);
	}

	public void onViralDeadzone()
	{
		Cub2UI.FindAndOpenDialog("dialog_ANDROID");
	}

	public void onNicknameSubmit()
	{
		string value = this.nickname.value;
		if (value.Length >= 3)
		{
			Kube.SS.SaveNewName(Kube.SS.serverId, value);
		}
		else
		{
			this.nickname.value = Kube.GPS.decodePlayerName;
		}
	}

	internal void ShowOffer(Offer offer)
	{
		GameObject gameObject = this.offer_dialog[offer.type - 1];
		OfferDialog component = gameObject.GetComponent<OfferDialog>();
		component.offer = offer;
		gameObject.SetActive(true);
	}

	public void ShowPack(PackInfo info)
	{
		PackDialog packDialog = Cub2UI.FindDialog<PackDialog>("dialog_pack");
		packDialog.info = info;
		packDialog.gameObject.SetActive(true);
	}

	private void NotifyUpdate()
	{
		foreach (object obj in this.offers.transform)
		{
			Transform transform = (Transform)obj;
			UIPackItem component = transform.GetComponent<UIPackItem>();
			if (component != null)
			{
				component.Validate();
			}
		}
		this.offers.Reposition();
	}

	private void UpdateName(string ans)
	{
		string[] array = ans.Split(new char[]
		{
			';'
		});
		if (array[0] != "1")
		{
			Cub2UI.MessageBox(Localize.nick_taken, null);
		}
		this.nickname.value = Kube.GPS.decodePlayerName;
	}

	public ViralDialog viral_dialog;

	public UIButton viral;

	public GameObject homeBtn;

	public UIInput nickname;

	public Rank rank;

	public UIGrid offers;

	public UIGrid tasks;

	public GameObject offerPrefab;

	public GameObject packPrefab;

	public GameObject taskPrefab;

	public GameObject boxPrefab;

	public GameObject[] offer_dialog;

	public GameObject upgrade_dialog;

	public UILabel version;
}
