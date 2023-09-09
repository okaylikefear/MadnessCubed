using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class AnimatedSprite_test : MonoBehaviour
{
	public AnimatedSprite_test()
	{
		this.debugstr = string.Empty;
	}

	public virtual void Start()
	{
		this.controller = (((CharacterController)this.GetComponent(typeof(CharacterController))) as CharacterController);
		this.animate.TMR = (Renderer)this.GetComponent(typeof(Renderer));
		this.animate.TMR.material = this.spriteMaterial;
		for (int i = 0; i < this.animations.Length; i++)
		{
			this.animations[i].material = this.spriteMaterial;
		}
		this.animate.TM = (TextMesh)this.GetComponent(typeof(TextMesh));
		this.animate.TM.font = this.animations[(int)this.animate.anim];
	}

	public virtual void animateSprite()
	{
		if (Time.time > this.animate.lastAnimFrameTime + 1f / (this.animate.frameRate * (this.animations[(int)this.animate.anim].characterInfo[0].uv.width + (float)1)))
		{
			this.animate.lastAnimFrameTime = Time.time;
			this.animate.frame = this.animate.frame + this.animate.frameStep;
			if (this.animate.frame > this.animations[(int)this.animate.anim].characterInfo.Length - 2 || (this.animate.frameStep == -1 && (float)this.animate.frame < this.animations[(int)this.animate.anim].characterInfo[0].uv.y))
			{
				LoopBehaviour index = (LoopBehaviour)this.animations[(int)this.animate.anim].characterInfo[0].index;
				if (index == LoopBehaviour.Loop)
				{
					this.animate.frame = (int)this.animations[(int)this.animate.anim].characterInfo[0].uv.y;
				}
				else if (index == LoopBehaviour.PingPong)
				{
					this.animate.frameStep = this.animate.frameStep * -1;
					this.animate.frame = this.animate.frame + this.animate.frameStep * 2;
				}
				else if (index == LoopBehaviour.OnceAndHold)
				{
					this.animate.frame = this.animate.frame - this.animate.frameStep;
				}
				else if (index == LoopBehaviour.OnceAndChange)
				{
					this.changeAnim(UnityBuiltins.parseInt(this.animations[(int)this.animate.anim].characterInfo[0].uv.x), UnityBuiltins.parseInt(this.animations[(int)this.animate.anim].characterInfo[0].uv.y));
				}
			}
			this.debugstr = "frame:" + this.animate.frame + " in anim:" + this.animate.anim;
			char c = (char)(this.animate.frame + 33);
			this.animate.TM.text = string.Empty + c;
		}
	}

	public virtual void changeAnim(int a, int f)
	{
		this.animate.anim = (anims)a;
		this.animate.TM.font = this.animations[(int)this.animate.anim];
		this.animate.frame = f;
		char c = (char)(this.animate.frame + 33);
		this.animate.TM.text = string.Empty + c;
		this.animate.lastAnimFrameTime = Time.time;
		this.animate.frameStep = 1;
		this.debugstr = "frame:" + this.animate.frame + " in anim:" + this.animate.anim;
	}

	public virtual void Update()
	{
		this.buttons.h = UnityEngine.Input.GetAxisRaw("Horizontal");
		this.buttons.v = UnityEngine.Input.GetAxisRaw("Vertical");
		if (Mathf.Abs(this.buttons.h) > 0.1f)
		{
			this.buttons.movePressed = true;
			if (this.movement.currentDirection != new Vector3(this.buttons.h, (float)0, (float)0) && !this.movement.crouching)
			{
				this.movement.currentDirection = new Vector3(this.buttons.h, (float)0, (float)0);
				this.buttons.moveChanged = true;
			}
		}
		else
		{
			this.buttons.movePressed = false;
		}
		if (this.buttons.v > 0.1f)
		{
			if (!this.movement.jumping && !this.movement.falling && this.buttons.jumpReleased)
			{
				this.buttons.jumpPressed = true;
				this.buttons.crouchPressed = false;
				this.buttons.jumpReleased = false;
			}
		}
		else if (this.buttons.v < -0.1f)
		{
			if (this.controller.isGrounded)
			{
				this.buttons.crouchPressed = true;
				this.buttons.jumpPressed = false;
			}
		}
		else
		{
			this.buttons.crouchPressed = false;
			this.buttons.jumpReleased = true;
			this.buttons.jumpPressed = false;
		}
		if (this.buttons.movePressed)
		{
			if (!this.movement.moving && !this.movement.crouching)
			{
				this.movement.moving = true;
				if (!this.movement.jumping && !this.movement.falling)
				{
					this.changeAnim(1, 0);
				}
			}
			if (this.buttons.moveChanged)
			{
				int num = (this.buttons.h <= (float)0) ? -1 : 1;
				Vector3 localScale = this.transform.localScale;
				float num2 = localScale.x = (float)num;
				Vector3 vector = this.transform.localScale = localScale;
				this.buttons.moveChanged = false;
				if (!this.movement.jumping && !this.movement.falling)
				{
					this.changeAnim(3, 0);
				}
			}
		}
		else if (this.movement.moving)
		{
			this.movement.moving = false;
			if (!this.movement.jumping && !this.movement.falling)
			{
				this.changeAnim(2, 0);
			}
		}
		if (this.controller.isGrounded)
		{
			this.movement.timeLastGrounded = Time.time;
			this.movement.currentVerticalSpeed = -0.5f;
			if (this.movement.falling)
			{
				this.movement.falling = false;
				if (this.movement.moving)
				{
					this.changeAnim(1, 15);
				}
				else
				{
					this.changeAnim(7, 0);
				}
			}
			if (this.buttons.jumpPressed)
			{
				if (!this.movement.jumping)
				{
					this.movement.jumping = true;
					this.movement.currentVerticalSpeed = this.movement.jumpPower;
					this.changeAnim(6, 0);
					this.buttons.jumpPressed = false;
				}
			}
			else if (this.buttons.crouchPressed)
			{
				if (!this.movement.crouching)
				{
					this.movement.crouching = true;
					this.movement.moving = false;
					this.changeAnim(4, 0);
				}
			}
			else if (this.movement.crouching)
			{
				this.movement.crouching = false;
				this.changeAnim(5, 0);
			}
		}
		else if (Time.time > this.movement.timeLastGrounded + 0.15f && !this.movement.jumping && !this.movement.falling)
		{
			this.changeAnim(8, 0);
			this.movement.falling = true;
			this.movement.timeLastGrounded = Time.time;
		}
		this.movement.currentVerticalSpeed = this.movement.currentVerticalSpeed - this.movement.gravity * Time.deltaTime;
		if (this.movement.currentVerticalSpeed < (float)1 && this.movement.jumping)
		{
			this.changeAnim(8, 0);
			this.movement.jumping = false;
			this.movement.falling = true;
		}
		if (this.movement.moving)
		{
			this.movement.targetSpeed = Mathf.Min(Mathf.Abs(this.buttons.h), 1f);
		}
		else
		{
			this.movement.targetSpeed = (float)0;
		}
		this.movement.targetSpeed = this.movement.targetSpeed * this.movement.maxSpeed;
		this.movement.currentSpeed = Mathf.Lerp(this.movement.currentSpeed, this.movement.targetSpeed, 0.1f);
		Vector3 vector2 = this.movement.currentDirection * this.movement.currentSpeed + new Vector3((float)0, this.movement.currentVerticalSpeed, (float)0);
		vector2 *= Time.deltaTime;
		this.movement.collisionFlags = this.controller.Move(vector2);
		this.animateSprite();
		if (UnityEngine.Input.GetKey(KeyCode.B))
		{
			Time.timeScale = 0.2f;
		}
		else
		{
			Time.timeScale = 1f;
		}
	}

	public virtual void OnGUI()
	{
		GUILayout.Label(this.debugstr, new GUILayoutOption[0]);
		GUI.color = ((!this.movement.moving) ? Color.red : Color.green);
		GUILayout.Label("moving", new GUILayoutOption[0]);
		GUI.color = ((!this.movement.jumping) ? Color.red : Color.green);
		GUILayout.Label("jumping", new GUILayoutOption[0]);
		GUI.color = ((!this.movement.crouching) ? Color.red : Color.green);
		GUILayout.Label("crouching", new GUILayoutOption[0]);
		GUI.color = ((!this.movement.falling) ? Color.red : Color.green);
		GUILayout.Label("falling", new GUILayoutOption[0]);
		GUI.color = ((!this.buttons.movePressed) ? Color.red : Color.green);
		GUILayout.Label("pressing move", new GUILayoutOption[0]);
		GUI.color = ((!this.buttons.jumpPressed) ? Color.red : Color.green);
		GUILayout.Label("pressing jump", new GUILayoutOption[0]);
		GUI.color = ((!this.buttons.crouchPressed) ? Color.red : Color.green);
		GUILayout.Label("pressing crouch", new GUILayoutOption[0]);
		GUI.color = ((!this.controller.isGrounded) ? Color.green : Color.red);
		GUILayout.Label("Airborn", new GUILayoutOption[0]);
		GUI.color = Color.white;
		GUILayout.Label("ground time:" + this.movement.timeLastGrounded, new GUILayoutOption[0]);
		GUILayout.Label("time:" + Time.time, new GUILayoutOption[0]);
		GUILayout.Label("direction:" + this.movement.currentDirection, new GUILayoutOption[0]);
		GUILayout.Label("Input:" + new Vector3(this.buttons.h, this.buttons.v, (float)0), new GUILayoutOption[0]);
	}

	public virtual void Main()
	{
	}

	public Material spriteMaterial;

	public Font[] animations;

	private CharacterController controller;

	public MovementClass movement;

	public AnimtionClass animate;

	public InputClass buttons;

	private string debugstr;
}
