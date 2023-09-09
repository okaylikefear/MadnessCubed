using System;
using UnityEngine;

public class KubeInput
{
	public static void ProcessInput()
	{
		if (KubeInput._keyInput == null)
		{
			KubeInput._keyInput = (KubeKeyInput[])KubeInput.keyState.Clone();
		}
		for (int i = 0; i < KubeInput.keyState.Length; i++)
		{
			KubeInput.keyState[i].val = KubeInput._keyInput[i].val;
			KubeInput.keyState[i].time = Time.time;
			KubeInput.keyState[i].down = KubeInput._keyInput[i].down;
			KubeInput.keyState[i].up = KubeInput._keyInput[i].up;
			KubeInput.keyState[i].pressed = KubeInput._keyInput[i].pressed;
			if (KubeInput._keyInput[i].down)
			{
				KubeInput.keyState[i].pressed = true;
			}
			if (!KubeInput._keyInput[i].pressed)
			{
				KubeInput._keyInput[i].val = 0f;
			}
			KubeInput._keyInput[i].down = false;
			KubeInput._keyInput[i].up = false;
		}
	}

	public static void Press(string axis, bool down)
	{
		float time = Time.time;
		for (int i = 0; i < KubeInput.keyState.Length; i++)
		{
			if (KubeInput.keyState[i].axis == axis)
			{
				KubeInput._keyInput[i].time = time;
				KubeInput._keyInput[i].pressed = down;
				if (down)
				{
					KubeInput._keyInput[i].down = true;
					KubeInput._keyInput[i].val = 1f;
				}
				else
				{
					KubeInput._keyInput[i].up = true;
				}
			}
		}
	}

	public static void Press(KeyCode key, bool down)
	{
		for (int i = 0; i < KubeInput.keyState.Length; i++)
		{
			if (KubeInput.keyState[i].key == key)
			{
				KubeInput._keyInput[i].time = Time.time;
				KubeInput._keyInput[i].pressed = down;
				if (down)
				{
					KubeInput._keyInput[i].down = true;
					KubeInput._keyInput[i].val = 1f;
				}
				else
				{
					KubeInput._keyInput[i].up = true;
				}
			}
		}
	}

	public static bool GetKey(KeyCode key)
	{
		return UnityEngine.Input.GetKey(key);
	}

	public static bool GetButtonDown(string k)
	{
		return Input.GetButtonDown(k);
	}

	public static bool GetButton(string k)
	{
		return Input.GetButton(k);
	}

	public static bool ScreenTouch()
	{
		return false;
	}

	public static float GetAxis(string axis)
	{
		return UnityEngine.Input.GetAxis(axis);
	}

	public static bool GetKeyDown(KeyCode key)
	{
		if (UIInput.selection && UIInput.selection.isActiveAndEnabled)
		{
			return false;
		}
		if (UnityEngine.Input.GetKeyDown(key))
		{
			return true;
		}
		float time = Time.time;
		for (int i = 0; i < KubeInput.keyState.Length; i++)
		{
			if (KubeInput.keyState[i].key == key)
			{
				return (KubeInput.keyState[i].native && UnityEngine.Input.GetKeyDown(key)) || KubeInput.keyState[i].down;
			}
		}
		return false;
	}

	public const int STICK_WALK = 0;

	public const int ZONE_FIRE = 0;

	public const int ZONE_BUTTON = 1;

	public static KubeKeyInput[] keyState = new KubeKeyInput[]
	{
		new KubeKeyInput("Fire1", KeyCode.None, false),
		new KubeKeyInput("Fire2", KeyCode.Mouse1, false),
		new KubeKeyInput("Fire3", KeyCode.None, false),
		new KubeKeyInput("Jump", KeyCode.Space, true),
		new KubeKeyInput("Sprint", KeyCode.Q, false),
		new KubeKeyInput("Use", KeyCode.E, true),
		new KubeKeyInput("UseF", KeyCode.F, true),
		new KubeKeyInput("Setup", KeyCode.T, true),
		new KubeKeyInput("Escape", KeyCode.Escape, true),
		new KubeKeyInput("Weapons", KeyCode.C, false),
		new KubeKeyInput("Reload", KeyCode.R, false),
		new KubeKeyInput("Weapons", KeyCode.Alpha1, false),
		new KubeKeyInput("Weapons", KeyCode.Alpha2, false),
		new KubeKeyInput("Weapons", KeyCode.Alpha3, false),
		new KubeKeyInput("Weapons", KeyCode.Alpha4, false),
		new KubeKeyInput("Weapons", KeyCode.Alpha5, false),
		new KubeKeyInput("Weapons", KeyCode.Alpha6, false),
		new KubeKeyInput("Weapons", KeyCode.Alpha7, false),
		new KubeKeyInput("Weapons", KeyCode.Alpha8, false),
		new KubeKeyInput("Weapons", KeyCode.Alpha9, false),
		new KubeKeyInput("Weapons", KeyCode.Alpha0, false),
		new KubeKeyInput("Mouse ScrollWheel", KeyCode.None, false),
		new KubeKeyInput("Tab", KeyCode.Tab, true)
	};

	protected static KubeKeyInput[] _keyInput;
}
