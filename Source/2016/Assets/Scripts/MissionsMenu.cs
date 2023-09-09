using System;
using kube;
using kube.data;
using UnityEngine;

public class MissionsMenu : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void onItemClick()
	{
		PlayDialog component = this.dialog.GetComponent<PlayDialog>();
		MissionItem componentInChildren = UIButton.current.transform.parent.GetComponentInChildren<MissionItem>();
		component.index = componentInChildren.index;
		component.missionDesc = componentInChildren.missionDesc;
		MonoBehaviour.print(string.Concat(new object[]
		{
			"clickMission #",
			component.missionDesc.id,
			" name:",
			component.missionDesc.title
		}));
		this.dialog.SetActive(true);
	}

	private void OnEnable()
	{
		UnityEngine.Debug.Log("enable");
		MissionBox.request(new VoidCallback(this.onMissionsLoaded), false);
	}

	public void GoTo(int missionId)
	{
		this.lastMission = missionId;
	}

	private void onEpisodeClick()
	{
		int index = UIToggle.current.gameObject.GetComponent<EpisodeItem>().index;
		if (!UIToggle.current.value)
		{
			return;
		}
		if (index - 1 < Kube.OH.episodeDesc.Length && Kube.OH.episodeDesc[index - 1].minlevel > Kube.GPS.playerLevel && !Kube.GPS.missionUnlock[index - 1])
		{
			this.episodes[0].GetComponent<UIToggle>().value = true;
			UnlockDialog unlockDialog = Cub2UI.FindAndOpenDialog<UnlockDialog>("dialog_unlock");
			unlockDialog.needLevel = Kube.OH.episodeDesc[index - 1].minlevel;
			unlockDialog.itemCode = "m" + (index - 1);
			unlockDialog.Show();
			return;
		}
		this.episode = index;
		this.Redraw();
	}

	private void onMissionsLoaded()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		this.loaded = true;
		foreach (object obj in this.episodeContainer.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(false);
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		int num = MissionBox.episodes.Length;
		this.episodes = new GameObject[num];
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = this.episodeContainer.AddChild(this.episodePrefab);
			gameObject.GetComponentInChildren<UIToggle>().onChange.Add(new EventDelegate(new EventDelegate.Callback(this.onEpisodeClick)));
			if (i == 0)
			{
				gameObject.GetComponentInChildren<UIToggle>().value = true;
			}
			EpisodeDesc ep = default(EpisodeDesc);
			if (i < MissionBox.episodes.Length)
			{
				ep = MissionBox.episodes[i];
			}
			else
			{
				gameObject.GetComponentInChildren<UIButton>().isEnabled = false;
			}
			gameObject.GetComponent<EpisodeItem>().index = i + 1;
			gameObject.GetComponent<EpisodeItem>().ep = ep;
			this.episodes[i] = gameObject;
		}
		this.episodeContainer.GetComponent<UIGrid>().Reposition();
		if (this.lastMission != 0)
		{
			base.Invoke("ShowLastMission", 0.1f);
			return;
		}
		this.Redraw();
	}

	private void ShowLastMission()
	{
		this.episode = MissionBox.FindMissionById(this.lastMission).episode;
		EpisodeDesc episodeDesc = MissionBox.FindEpisodeById(this.episode);
		for (int i = 0; i < this.episodes.Length; i++)
		{
			EpisodeItem component = this.episodes[i].GetComponent<EpisodeItem>();
			if (component.index == this.episode)
			{
				this.episodes[i].GetComponentInChildren<UIToggle>().value = true;
				break;
			}
		}
		this.lastMission = 0;
	}

	private void Redraw()
	{
		if (!this.loaded)
		{
			return;
		}
		foreach (object obj in this.container.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(false);
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		MissionDesc[] array = MissionBox.selectMissions(this.episode);
		int lastEnabled = 0;
		int num = Mathf.CeilToInt((float)array.Length / 10f) * 10;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = this.container.AddChild(this.itemPrefab);
			MissionItem component = gameObject.GetComponent<MissionItem>();
			component.index = i;
			if (array.Length > i)
			{
				if (array[i].score > 0)
				{
					lastEnabled = i;
				}
				if (array[i].enabled)
				{
					EventDelegate.Set(gameObject.GetComponentInChildren<UIButton>().onClick, new EventDelegate(new EventDelegate.Callback(this.onItemClick)));
				}
				component.missionDesc = array[i];
				component.Show();
			}
		}
		this.container.GetComponent<PagePanel>().Reposition();
		this.container.GetComponent<PagePanel>().ShiftPage(lastEnabled);
	}

	public GameObject itemPrefab;

	public GameObject episodePrefab;

	public GameObject episodeContainer;

	public GameObject container;

	public GameObject dialog;

	public GameObject[] episodes;

	private int episode = 1;

	private int lastMission;

	private bool loaded;
}
