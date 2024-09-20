using System;
using UnityEngine;

public class ToggleState : MonoBehaviour
{
	public int state
	{
		get
		{
			return this._state;
		}
		set
		{
			if (value < 0 || value >= this.objects.Length)
			{
				return;
			}
			this._state = value;
			this.invalidate();
		}
	}

	private void Start()
	{
		this.invalidate();
	}

	public GameObject current
	{
		get
		{
			return this.objects[this._state];
		}
	}

	private void invalidate()
	{
		for (int i = 0; i < this.objects.Length; i++)
		{
			if (i == this._state)
			{
				this.objects[i].SetActive(true);
			}
			else
			{
				this.objects[i].SetActive(false);
			}
		}
	}

	private void Update()
	{
	}

	public GameObject[] objects = new GameObject[0];

	public int _state;
}
