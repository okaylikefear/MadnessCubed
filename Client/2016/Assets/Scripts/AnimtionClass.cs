using System;
using UnityEngine;

[Serializable]
public class AnimtionClass
{
	public AnimtionClass()
	{
		this.anim = anims.Idle;
		this.frameStep = 1;
		this.frameRate = 10f;
	}

	[NonSerialized]
	public TextMesh TM;

	[NonSerialized]
	public Renderer TMR;

	[NonSerialized]
	public anims anim;

	[NonSerialized]
	public int frame;

	[NonSerialized]
	public int frameStep;

	public float frameRate;

	[NonSerialized]
	public float lastAnimFrameTime;
}
