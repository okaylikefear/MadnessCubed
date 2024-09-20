using System;
using UnityEngine;

public class CharItemParam : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	[ContextMenu("collect")]
	private void collect()
	{
		this.sliderMain = base.GetComponentsInChildren<UISlider>()[1];
	}

	public UISlider slider;

	public UISlider sliderMain;

	public UILabel title;

	public UILabel value;

	public UILabel increment;
}
