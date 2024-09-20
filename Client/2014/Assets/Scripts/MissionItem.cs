using System;
using kube;
using kube.data;
using UnityEngine;

public class MissionItem : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnClick()
	{
		UnityEngine.Debug.Log("click");
	}

	public void Show()
	{
		int nnstars = this.missionDesc.nnstars;
		for (int i = 0; i < this.stars.Length; i++)
		{
			if (nnstars > i)
			{
				this.stars[i].GetComponent<UIToggle>().value = true;
			}
		}
		if (this.missionDesc.current || this.missionDesc.enabled)
		{
			this.ts.state = 1;
			this.label.text = this.missionDesc.index.ToString();
			if (this.missionDesc.index == 0 && Kube.GPS.needTraining)
			{
				this.tutorToActivate.SetActive(true);
			}
		}
	}

	public GameObject[] stars;

	[NonSerialized]
	public MissionDesc missionDesc;

	[NonSerialized]
	public int index;

	public ToggleState ts;

	public UILabel label;

	public GameObject tutorToActivate;
}
