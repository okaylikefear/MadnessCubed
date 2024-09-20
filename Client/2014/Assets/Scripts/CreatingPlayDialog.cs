using System;
using kube;
using UnityEngine;

public class CreatingPlayDialog : MonoBehaviour
{
	private void Start()
	{
	}

	private void OnEnable()
	{
		this.passwordGO.SetActive(this.offline.value);
	}

	public void onLoad()
	{
		this.owner.LoadMap(this.mySelectedMapId, !this.offline.value, this.password.value, CreatingPlayDialog.dayState[this.day.state]);
		if (this.title.value != this.preloadMapName)
		{
			Kube.SS.SetMapName(this.mySelectedMapId, AuxFunc.CodeRussianName(this.title.value));
		}
	}

	public void onReset()
	{
		this.owner.ResetMap();
	}

	public void onOfflineChange()
	{
		this.passwordGO.SetActive(this.offline.value);
	}

	public long mySelectedMapId;

	public UIInput title;

	public UIInput password;

	public UILabel slotLabel;

	public DayToggle day;

	public UIToggle offline;

	public bool isMyMap;

	public GameObject passwordGO;

	public CreatingMyTab owner;

	public string preloadMapName;

	private static int[] dayState = new int[]
	{
		1,
		0,
		2
	};
}
