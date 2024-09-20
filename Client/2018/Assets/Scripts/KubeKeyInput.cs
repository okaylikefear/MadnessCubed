using System;
using UnityEngine;

public struct KubeKeyInput
{
	public KubeKeyInput(string axis, KeyCode key, bool native = false)
	{
		this.axis = axis;
		this.key = key;
		this.native = native;
		this.val = 0f;
		this.time = 0f;
		this.up = false;
		this.down = false;
		this.pressed = false;
	}

	public string axis;

	public KeyCode key;

	public float val;

	public float time;

	public bool native;

	public bool down;

	public bool up;

	public bool pressed;
}
