using System;
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
	private void Start()
	{
		if (this.btn)
		{
			this.btn.onClick.Add(new EventDelegate(new EventDelegate.Callback(this.onClick)));
		}
	}

	private void Update()
	{
	}

	public void onClick()
	{
		base.GetComponentInParent<CharUpgrade>().OnUpgradePlayerParam(this);
	}

	public UISlider slider;

	public UILabel value;

	public UILabel title;

	public UIButton btn;
}
