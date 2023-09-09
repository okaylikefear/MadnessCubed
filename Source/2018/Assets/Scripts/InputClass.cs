using System;

[Serializable]
public class InputClass
{
	public InputClass()
	{
		this.jumpReleased = true;
	}

	[NonSerialized]
	public bool jumpPressed;

	[NonSerialized]
	public bool jumpReleased;

	[NonSerialized]
	public bool crouchPressed;

	[NonSerialized]
	public bool movePressed;

	[NonSerialized]
	public bool moveChanged;

	[NonSerialized]
	public float v;

	[NonSerialized]
	public float h;
}
