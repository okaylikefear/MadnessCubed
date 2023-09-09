using System;
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
		this.dialog.SetActive(true);
	}

	private void OnEnable()
	{
		UnityEngine.Debug.Log("enable");
		MissionBox.request(new VoidCallback(this.onMissionsLoaded), false);
	}

	private void onEpisodeClick()
	{
		if (!UIToggle.current.value)
		{
			return;
		}
		this.episode = UIToggle.current.gameObject.GetComponent<EpisodeItem>().index;
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
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = NGUITools.AddChild(this.episodeContainer, this.episodePrefab);
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
		}
		this.episodeContainer.GetComponent<UIGrid>().Reposition();
		this.Redraw();
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
		for (int i = 0; i < 10; i++)
		{
			GameObject gameObject = NGUITools.AddChild(this.container, this.itemPrefab);
			MissionItem component = gameObject.GetComponent<MissionItem>();
			component.index = i;
			if (array.Length > i)
			{
				if (array[i].enabled)
				{
					gameObject.GetComponentInChildren<UIButton>().onClick.Add(new EventDelegate(new EventDelegate.Callback(this.onItemClick)));
				}
				component.missionDesc = array[i];
				component.Show();
			}
		}
		this.container.GetComponent<UIGrid>().Reposition();
	}

	public GameObject itemPrefab;

	public GameObject episodePrefab;

	public GameObject episodeContainer;

	public GameObject container;

	public GameObject dialog;

	public UIToggle[] episodes;

	private int episode = 1;

	private bool loaded;
}
