using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class AnimatedSprite : MonoBehaviour
{
	public AnimatedSprite()
	{
		this.animNames = new string[]
		{
			"Idle",
			"Run",
			"Stop",
			"Turn",
			"Crouch",
			"UnCrouch",
			"Jump",
			"Land",
			"Fall"
		};
		this.frameStep = 1;
	}

	public virtual void Start()
	{
		this.TMR = (Renderer)this.GetComponent(typeof(Renderer));
		this.TMR.material = this.spriteMaterial;
		for (int i = 0; i < this.animations.Length; i++)
		{
			this.animations[i].material = this.spriteMaterial;
		}
		this.TM = (TextMesh)this.GetComponent(typeof(TextMesh));
		this.TM.font = this.animations[this.anim];
	}

	public virtual void animateSprite()
	{
		if (Time.time > this.lastAnimFrameTime + 0.1f)
		{
			this.lastAnimFrameTime = Time.time;
			this.frame += this.frameStep;
			if (this.frame > this.animations[this.anim].characterInfo.Length - 2 || (this.frameStep == -1 && (float)this.frame < this.animations[this.anim].characterInfo[0].uv.y))
			{
				theLoopBehaviour index = (theLoopBehaviour)this.animations[this.anim].characterInfo[0].index;
				if (index == theLoopBehaviour.Loop)
				{
					this.frame = (int)this.animations[this.anim].characterInfo[0].uv.y;
				}
				else if (index == theLoopBehaviour.PingPong)
				{
					this.frameStep *= -1;
					this.frame += this.frameStep * 2;
				}
				else if (index == theLoopBehaviour.OnceAndHold)
				{
					this.frame -= this.frameStep;
				}
				else if (index == theLoopBehaviour.OnceAndChange)
				{
					this.changeAnim(UnityBuiltins.parseInt(this.animations[this.anim].characterInfo[0].uv.x), UnityBuiltins.parseInt(this.animations[this.anim].characterInfo[0].uv.y));
				}
			}
			char c = (char)(this.frame + 33);
			this.TM.text = string.Empty + c;
		}
	}

	public virtual void changeAnim(int a, int f)
	{
		this.anim = a;
		this.TM.font = this.animations[this.anim];
		this.frame = f;
		char c = (char)(this.frame + 33);
		this.TM.text = string.Empty + c;
		this.lastAnimFrameTime = Time.time;
		this.frameStep = 1;
	}

	public virtual void Update()
	{
		this.animateSprite();
	}

	public virtual void OnGUI()
	{
		this.anim = GUI.SelectionGrid(new Rect((float)10, (float)10, (float)270, (float)60), this.anim, this.animNames, 3);
		if (GUI.changed)
		{
			this.changeAnim(this.anim, 0);
		}
	}

	public virtual void Main()
	{
	}

	public Material spriteMaterial;

	public Font[] animations;

	private string[] animNames;

	private float lastAnimFrameTime;

	private int frame;

	private int frameStep;

	private int anim;

	private TextMesh TM;

	private Renderer TMR;
}
