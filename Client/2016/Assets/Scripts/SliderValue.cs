using System;
using UnityEngine;

public class SliderValue : MonoBehaviour
{
	private void Start()
	{
		if (Application.isPlaying)
		{
			UISlider component = base.GetComponent<UISlider>();
			component.onChange.Add(new EventDelegate(new EventDelegate.Callback(this.onChange)));
		}
	}

	private void Update()
	{
	}

	private void onChange()
	{
		float value = UIProgressBar.current.value;
		this.label.text = Mathf.FloorToInt(value * 100f).ToString() + "%";
	}

	public UILabel label;
}
