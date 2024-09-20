using System;
using UnityEngine;

public class WeaponParams : MonoBehaviour
{
	public float value
	{
		get
		{
			return this._value;
		}
		set
		{
			this.slider.value = value / this.maxValue;
			this.label.text = value.ToString();
			this._value = value;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public UISlider slider;

	public UILabel label;

	public UIButton button;

	public float maxValue = 100f;

	public float _value;
}
