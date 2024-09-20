using System;
using kube;
using kube.ui;
using UnityEngine;

public class TriggerScript : GameMapItem
{
	private void Start()
	{
		this.Init();
	}

	public override void SaveMap(KubeStream bw)
	{
		bw.WriteByte((byte)this.x);
		bw.WriteByte((byte)this.y);
		bw.WriteByte((byte)this.z);
		bw.WriteByte((byte)this.type);
		bw.WriteByte((byte)this.state);
		bw.WriteByte((byte)this.delayTime);
		bw.WriteByte((byte)this.condActivate);
		bw.WriteByte((byte)this.condKey);
		bw.WriteByte((byte)this.id);
	}

	public override void LoadMap(KubeStream br)
	{
		this.x = (int)br.ReadByte();
		this.y = (int)br.ReadByte();
		this.z = (int)br.ReadByte();
		this.type = (TriggerType)br.ReadByte();
		this.state = (int)br.ReadByte();
		this.delayTime = (int)br.ReadByte();
		this.condActivate = (int)br.ReadByte();
		this.condKey = (int)br.ReadByte();
		this.id = (int)br.ReadByte();
		Kube.WHS.triggerS[this.id] = this;
		this.SetParameters(this.x, this.y, this.z, (int)this.type, this.state, this.delayTime, this.condActivate, this.condKey, this.id);
	}

	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		if (this.activateMode == TriggerScript.CondActivate.PRESS)
		{
			this.cond1_press = true;
		}
		else if (this.activateMode == TriggerScript.CondActivate.TOUCH)
		{
			this.cond1_near = true;
		}
		else if (this.activateMode == TriggerScript.CondActivate.DAMAGE)
		{
			this.cond1_damage = true;
		}
		if (this.needKey)
		{
			this.cond2_red = true;
		}
		if (this.id == -1)
		{
			this.id = Kube.WHS.GetNewTriggerId(base.gameObject);
		}
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
		this.x = Mathf.RoundToInt(base.transform.position.x);
		this.y = Mathf.RoundToInt(base.transform.position.y);
		this.z = Mathf.RoundToInt(base.transform.position.z);
		this.initialized = true;
	}

	public void SaveTrigger()
	{
		this.Init();
		this.condActivate = 0;
		this.condActivate += ((!this.cond1_near) ? 0 : 1);
		this.condActivate += ((!this.cond1_press) ? 0 : 2);
		this.condActivate += ((!this.cond1_damage) ? 0 : 4);
		this.condKey = 0;
		this.condKey += ((!this.cond2_red) ? 0 : 1);
		this.condKey += ((!this.cond2_green) ? 0 : 2);
		this.condKey += ((!this.cond2_blue) ? 0 : 4);
		this.condKey += ((!this.cond2_gold) ? 0 : 8);
		this.NO.SaveTrigger(this.x, this.y, this.z, (int)this.type, this.state, this.delayTime, this.condActivate, this.condKey, this.id);
	}

	private void SetupItem()
	{
		this.Init();
		Kube.OH.openMenu(new DrawCall(this.setupGUI), true, false);
	}

	public virtual void SetParameters(int _x, int _y, int _z, int _type, int _state, int _delayTime, int _condActivate, int _condKey, int _id)
	{
		this.Init();
		if (this.state != _state)
		{
			base.BroadcastMessage("SetState", _state, SendMessageOptions.DontRequireReceiver);
		}
		this.x = _x;
		this.y = _y;
		this.z = _z;
		this.type = (TriggerType)_type;
		this.state = _state;
		this.delayTime = _delayTime;
		this.condActivate = _condActivate;
		this.condKey = _condKey;
		this.id = _id;
		this.cond1_near = (this.condActivate % 2 == 1);
		this.cond1_press = ((this.condActivate >> 1) % 2 == 1);
		this.cond1_damage = ((this.condActivate >> 2) % 2 == 1);
		this.cond2_red = (this.condKey % 2 == 1);
		this.cond2_green = ((this.condKey >> 1) % 2 == 1);
		this.cond2_blue = ((this.condKey >> 2) % 2 == 1);
		this.cond2_gold = ((this.condKey >> 3) % 2 == 1);
		base.BroadcastMessage("ApplyTriggerStyle", this.condActivate, SendMessageOptions.DontRequireReceiver);
	}

	private void setupGUI()
	{
		this.Init();
		float num = (float)KUI.width;
		float num2 = (float)KUI.height;
		float num3 = 0.5f * num - 350f;
		float num4 = num2 - 320f;
		GUI.skin = Kube.ASS1.mainSkin;
		GUI.DrawTexture(new Rect(num3, num4, 700f, 240f), Kube.ASS3.setupItemTex);
		GUI.skin = Kube.ASS1.bigWhiteLabel;
		GUI.Label(new Rect(num3 + 20f, num4 + 10f, 300f, 40f), Localize.trig_options);
		GUI.skin = Kube.ASS1.triggerSkin;
		GUI.Label(new Rect(num3 + 10f, num4 + 50f, 150f, 30f), Localize.trig_type);
		GUI.skin = Kube.ASS1.triggerSkinArrowLeft;
		if (GUI.Button(new Rect(num3 + 10f, num4 + 85f, 50f, 30f), string.Empty))
		{
			int num5 = (int)this.type;
			num5--;
			if (num5 < 0)
			{
				num5 = Localize.triggerTypeName.Length - 1;
			}
			this.type = (TriggerType)num5;
		}
		GUI.skin = Kube.ASS1.triggerSkinArrowRight;
		if (GUI.Button(new Rect(num3 + 310f, num4 + 85f, 50f, 30f), string.Empty))
		{
			int num6 = (int)this.type;
			num6++;
			if (num6 >= Localize.triggerTypeName.Length)
			{
				num6 = 0;
			}
			this.type = (TriggerType)num6;
		}
		GUI.skin = Kube.ASS1.triggerSkin;
		GUI.Label(new Rect(num3 + 60f, num4 + 85f, 250f, 30f), Localize.triggerTypeName[(int)this.type]);
		if (this.activateMode == TriggerScript.CondActivate.NONE)
		{
			GUI.Label(new Rect(num3 + 10f, num4 + 118f, 250f, 30f), Localize.trig_triggered_if);
			this.cond1_near = GUI.Toggle(new Rect(num3 + 10f, num4 + 145f, 150f, 30f), this.cond1_near, Localize.triggerConditionActivateName[0]);
			this.cond1_press = GUI.Toggle(new Rect(num3 + 160f, num4 + 145f, 150f, 30f), this.cond1_press, Localize.triggerConditionActivateName[1]);
			this.cond1_damage = GUI.Toggle(new Rect(num3 + 310f, num4 + 145f, 150f, 30f), this.cond1_damage, Localize.triggerConditionActivateName[2]);
		}
		if (this.needKey)
		{
			GUI.Label(new Rect(num3 + 10f, num4 + 178f, 250f, 30f), Localize.trig_need_for_triggering);
			this.cond2_red = GUI.Toggle(new Rect(num3 + 10f, num4 + 205f, 150f, 30f), this.cond2_red, Localize.triggerNeedKeyName[0]);
			this.cond2_green = GUI.Toggle(new Rect(num3 + 160f, num4 + 205f, 150f, 30f), this.cond2_green, Localize.triggerNeedKeyName[1]);
			this.cond2_blue = GUI.Toggle(new Rect(num3 + 310f, num4 + 205f, 150f, 30f), this.cond2_blue, Localize.triggerNeedKeyName[2]);
			this.cond2_gold = GUI.Toggle(new Rect(num3 + 460f, num4 + 205f, 150f, 30f), this.cond2_gold, Localize.triggerNeedKeyName[3]);
		}
		if (Kube.GPS.moderType != 0)
		{
			GUI.Label(new Rect(num3 + 300f, num4 + 5f, 250f, 30f), "РќРѕРјРµСЂ СЃРѕР±С‹С‚РёСЏ: " + this.delayTime);
			this.delayTime = (int)GUI.HorizontalScrollbar(new Rect(num3 + 300f, num4 + 35f, 300f, 20f), (float)this.delayTime, 1f, 0f, 100f);
		}
		if (GUI.Button(new Rect(num3 + 500f, num4 + 140f, 180f, 50f), Localize.apply))
		{
			this.SaveTrigger();
			Kube.OH.closeMenu(null);
		}
	}

	private void Update()
	{
	}

	private void TriggerToggleLocal(PlayerScript ps)
	{
		this.Init();
		if (Time.time - this.lastToggleTime < this.toggleDeltaTime)
		{
			return;
		}
		if (ps && ps.dead)
		{
			return;
		}
		if (this.type == TriggerType.on_off && this.delayTime > 0)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("SystemGO");
			if (gameObject != null)
			{
				gameObject.SendMessage("ChangeState", this.delayTime, SendMessageOptions.DontRequireReceiver);
			}
		}
		int num = this.state;
		if (this.type == TriggerType.on_off)
		{
			num = 1 - this.state;
		}
		else if (this.type == TriggerType.off)
		{
			num = 0;
		}
		else if (this.type == TriggerType.on)
		{
			num = 1;
		}
		else if (this.type == TriggerType.onByTime)
		{
			num = 1;
		}
		else if (this.type == TriggerType.period)
		{
			num = 1 - this.state;
		}
		else if (this.type == TriggerType.exit)
		{
			if (Kube.BCS.gameType == GameType.mission && ps && !ps.dead)
			{
				Kube.BCS.gameObject.SendMessage("TriggerExitReached");
			}
			if (Kube.BCS.gameType == GameType.test)
			{
				Kube.BCS.EndTestMission();
			}
			else
			{
				Kube.GPS.printMessage(Localize.trig_mission_exit, Color.green);
			}
		}
		this.NO.SaveTrigger(this.x, this.y, this.z, (int)this.type, num, this.delayTime, this.condActivate, this.condKey, this.id);
		Kube.WHS.ActivateWiresOfTrigger(this.id);
		if (base.GetComponent<AudioSource>())
		{
			base.GetComponent<AudioSource>().Stop();
			base.GetComponent<AudioSource>().Play();
		}
		this.lastToggleTime = Time.time;
	}

	public void PlayTrigger(int _targetType, int targetX, int targetY, int targetZ)
	{
		if (this.type == TriggerType.teleport && _targetType == 4)
		{
			this.lastGOInTrigger.SendMessage("Teleport", new Vector3((float)targetX, (float)targetY, (float)targetZ), SendMessageOptions.DontRequireReceiver);
		}
		else if (_targetType != 4)
		{
			GameObject gameObject = null;
			if (_targetType == 1)
			{
				gameObject = Kube.WHS.GetAAGO(targetX + targetY * 256 + targetZ * 256 * 256);
			}
			else if (_targetType == 2)
			{
				gameObject = Kube.WHS.GetTriggerGO(targetX + targetY * 256 + targetZ * 256 * 256);
			}
			if (gameObject != null)
			{
				if (this.type == TriggerType.on_off)
				{
					gameObject.SendMessage("Command_Toggle", SendMessageOptions.DontRequireReceiver);
				}
				else if (this.type == TriggerType.on)
				{
					gameObject.SendMessage("Command_On", SendMessageOptions.DontRequireReceiver);
				}
				else if (this.type == TriggerType.off)
				{
					gameObject.SendMessage("Command_Off", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	private void Command_On()
	{
		this.TriggerToggleLocal(null);
	}

	private void Command_Off()
	{
		this.TriggerToggleLocal(null);
	}

	private void Command_Toggle()
	{
		this.TriggerToggleLocal(null);
	}

	private void OnTriggerStay(Collider other)
	{
		if (Kube.IS.ps && other.transform.root.gameObject == Kube.IS.ps.gameObject && this.cond1_near)
		{
			if (!this.needKey || Kube.IS.ps.HaveKeys(this.cond2_red, this.cond2_green, this.cond2_blue, this.cond2_gold))
			{
				this.lastGOInTrigger = Kube.IS.ps.gameObject;
				this.TriggerToggleLocal(Kube.IS.ps);
			}
			else
			{
				this.PrintKeyRequest();
			}
		}
	}

	private void PrintKeyRequest()
	{
		if (this.cond2_red)
		{
			Kube.GPS.printMessage(Localize.triggerNeedKeyName[0], Color.red);
		}
		if (this.cond2_green)
		{
			Kube.GPS.printMessage(Localize.triggerNeedKeyName[1], Color.green);
		}
		if (this.cond2_blue)
		{
			Kube.GPS.printMessage(Localize.triggerNeedKeyName[2], Color.blue);
		}
	}

	private void Activate(PlayerScript ps)
	{
		if (this.cond1_press)
		{
			if (!this.needKey || Kube.IS.ps.HaveKeys(this.cond2_red, this.cond2_green, this.cond2_blue, this.cond2_gold))
			{
				this.lastGOInTrigger = Kube.IS.ps.gameObject;
				this.TriggerToggleLocal(ps);
			}
			else
			{
				this.PrintKeyRequest();
			}
		}
	}

	public int x;

	public int y;

	public int z;

	private TriggerType type;

	private int state;

	private int delayTime;

	private int condActivate;

	protected int condKey;

	public int id = -1;

	public bool needKey;

	public TriggerScript.CondActivate activateMode;

	private bool initialized;

	private NetworkObjectScript NO;

	protected bool cond1_near;

	protected bool cond1_press;

	protected bool cond1_damage;

	protected bool cond2_red;

	protected bool cond2_green;

	protected bool cond2_blue;

	protected bool cond2_gold;

	private float lastToggleTime;

	private float toggleDeltaTime = 0.5f;

	private GameObject lastGOInTrigger;

	public enum CondActivate
	{
		NONE,
		TOUCH,
		PRESS,
		DAMAGE = 4
	}
}
