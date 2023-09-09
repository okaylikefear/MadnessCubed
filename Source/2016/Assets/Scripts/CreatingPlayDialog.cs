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
		string text = this.title.value;
		if (text.Length > 16)
		{
			text = text.Substring(0, 16);
		}
		this.owner.LoadMap(text, this.mySelectedMapId, !this.offline.value, this.password.value, CreatingPlayDialog.dayState[this.day.state], this.isMyMap);
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
