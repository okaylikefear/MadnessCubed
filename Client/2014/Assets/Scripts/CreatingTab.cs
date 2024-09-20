using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class CreatingTab : Tab
{
	public void ChangeCanBuildStatus(int id, bool canBuild)
	{
		Kube.BCS.ChangeCanBuildStatus(id, true);
	}

	public void BanPlayer(int id)
	{
		Kube.BCS.BanPlayer(id);
	}

	private void Start()
	{
		this.newplayeraccess.states = new string[]
		{
			Localize.BCS_noobs_to_build + " " + Localize.BCS_allowed,
			Localize.BCS_noobs_to_build + " " + Localize.BCS_notallowed
		};
		if (base.gameObject.activeSelf)
		{
			this.OnEnable();
		}
	}

	private void OnEnable()
	{
		this.newplayeraccess.index = ((!Kube.BCS.newPlayersCanBuild) ? 1 : 0);
	}

	public void OnSaveButton()
	{
		Kube.BCS.SaveMap();
	}

	public void OnTestButton()
	{
		ToggleButton component = UIButton.current.GetComponent<ToggleButton>();
		UILabel componentInChildren = UIButton.current.GetComponentInChildren<UILabel>();
		if (!component.value)
		{
			Kube.BCS.StartTestMission();
		}
		else
		{
			Kube.BCS.EndTestMission();
		}
		componentInChildren.text = ((!component.value) ? Localize.BCS_end_test : Localize.BCS_start_test);
		component.value = !component.value;
		Cub2Menu.instance.gameObject.SetActive(false);
	}

	public void OnChangeNoobsBuild()
	{
		Kube.BCS.newPlayersCanBuild = (this.newplayeraccess.index == 0);
	}

	private void Update()
	{
		if (Kube.BCS.playersInfo == null)
		{
			return;
		}
		BattleControllerScript.PlayerInfo[] playersInfo = Kube.BCS.playersInfo;
		List<GameObject> list = new List<GameObject>();
		foreach (object obj in this.container.gameObject.transform)
		{
			Transform transform = (Transform)obj;
			list.Add(transform.gameObject);
		}
		float num = 0f;
		for (int i = 0; i < playersInfo.Length; i++)
		{
			int id = playersInfo[i].Id;
			GameObject gameObject;
			if (!this._dict.ContainsKey(id))
			{
				gameObject = NGUITools.AddChild(this.container, this.rowPrefab);
				this._dict[id] = gameObject;
			}
			else
			{
				gameObject = this._dict[id];
			}
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(gameObject.transform, false);
			list.Remove(gameObject);
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.y = num;
			num -= bounds.size.y;
			gameObject.transform.localPosition = localPosition;
			TabRowCreating component = gameObject.GetComponent<TabRowCreating>();
			component.id = id;
			component.UID = playersInfo[i].UID;
			component.name.text = AuxFunc.DecodeRussianName(playersInfo[i].Name);
			bool isMapOwner = Kube.BCS.isMapOwner;
			component.ban.isEnabled = (isMapOwner && id != Kube.GPS.playerId);
			component.allow.isEnabled = (isMapOwner && id != Kube.GPS.playerId);
			int num2 = Mathf.Min(playersInfo[i].Level, Kube.ASS2.RankTex.Length - 1);
			component.rank.mainTexture = Kube.ASS2.RankTex[num2].mainTexture;
		}
		for (int j = 0; j < list.Count; j++)
		{
			GameObject gameObject2 = list[j];
			gameObject2.SetActive(false);
			UnityEngine.Object.Destroy(gameObject2);
		}
	}

	public LRButton newplayeraccess;

	protected Dictionary<int, GameObject> _dict = new Dictionary<int, GameObject>();
}
