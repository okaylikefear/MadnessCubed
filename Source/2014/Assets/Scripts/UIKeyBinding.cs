using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Key Binding")]
public class UIKeyBinding : MonoBehaviour
{
	private void Start()
	{
		UIInput component = base.GetComponent<UIInput>();
		this.mIsInput = (component != null);
		if (component != null)
		{
			EventDelegate.Add(component.onSubmit, new EventDelegate.Callback(this.OnSubmit));
		}
	}

	private void OnSubmit()
	{
		if (UICamera.currentKey == this.keyCode && this.IsModifierActive())
		{
			this.mIgnoreUp = true;
		}
	}

	private bool IsModifierActive()
	{
		if (this.modifier == UIKeyBinding.Modifier.None)
		{
			return true;
		}
		if (this.modifier == UIKeyBinding.Modifier.Alt)
		{
			if (UnityEngine.Input.GetKey(KeyCode.LeftAlt) || UnityEngine.Input.GetKey(KeyCode.RightAlt))
			{
				return true;
			}
		}
		else if (this.modifier == UIKeyBinding.Modifier.Control)
		{
			if (UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl))
			{
				return true;
			}
		}
		else if (this.modifier == UIKeyBinding.Modifier.Shift && (UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift)))
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		if (this.keyCode == KeyCode.None || !this.IsModifierActive())
		{
			return;
		}
		if (this.action == UIKeyBinding.Action.PressAndClick)
		{
			if (UICamera.inputHasFocus)
			{
				return;
			}
			UICamera.currentTouch = UICamera.controller;
			UICamera.currentScheme = UICamera.ControlScheme.Mouse;
			UICamera.currentTouch.current = base.gameObject;
			if (UnityEngine.Input.GetKeyDown(this.keyCode))
			{
				this.mPress = true;
				UICamera.Notify(base.gameObject, "OnPress", true);
			}
			if (UnityEngine.Input.GetKeyUp(this.keyCode))
			{
				UICamera.Notify(base.gameObject, "OnPress", false);
				if (this.mPress)
				{
					UICamera.Notify(base.gameObject, "OnClick", null);
					this.mPress = false;
				}
			}
			UICamera.currentTouch.current = null;
		}
		else if (this.action == UIKeyBinding.Action.Select && UnityEngine.Input.GetKeyUp(this.keyCode))
		{
			if (this.mIsInput)
			{
				if (!this.mIgnoreUp && !UICamera.inputHasFocus)
				{
					UICamera.selectedObject = base.gameObject;
				}
				this.mIgnoreUp = false;
			}
			else
			{
				UICamera.selectedObject = base.gameObject;
			}
		}
	}

	public KeyCode keyCode;

	public UIKeyBinding.Modifier modifier;

	public UIKeyBinding.Action action;

	private bool mIgnoreUp;

	private bool mIsInput;

	private bool mPress;

	public enum Action
	{
		PressAndClick,
		Select
	}

	public enum Modifier
	{
		None,
		Shift,
		Control,
		Alt
	}
}
