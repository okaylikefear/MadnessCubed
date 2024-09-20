using System;
using UnityEngine;

public class HUDTouch : MonoBehaviour
{
	private void OnPress(bool down)
	{
		if (this.toggle)
		{
			if (!down)
			{
				return;
			}
			this._state = !this._state;
			down = this._state;
		}
		if (this.key == KeyCode.None)
		{
			KubeInput.Press(this.axis, down);
		}
		else
		{
			KubeInput.Press(this.key, down);
		}
		if (this.exec != null && this.exec.isValid)
		{
			this.exec.Execute();
		}
	}

	public KeyCode key;

	public string axis;

	public EventDelegate exec;

	public bool toggle;

	private bool _state;
}
