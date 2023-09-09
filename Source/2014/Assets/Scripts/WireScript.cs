using System;
using kube;
using kube.ui;
using UnityEngine;

public class WireScript : GameMapItem
{
	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		if (Kube.BCS == null)
		{
			Kube.BCS = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControllerScript>();
		}
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
		this.initialized = true;
	}

	public override void SaveMap(KubeStream bw)
	{
		bw.WriteShort((ushort)this.triggerId);
		bw.WriteByte((byte)this.delay);
		bw.WriteByte((byte)this.targetType);
		bw.WriteByte((byte)this.xt);
		bw.WriteByte((byte)this.yt);
		bw.WriteByte((byte)this.zt);
		bw.WriteByte((byte)this.id);
	}

	public override void LoadMap(KubeStream br)
	{
		this.triggerId = (int)br.ReadShort();
		this.delay = (int)br.ReadByte();
		this.targetType = (TriggerTargetType)br.ReadByte();
		this.xt = (int)br.ReadByte();
		this.yt = (int)br.ReadByte();
		this.zt = (int)br.ReadByte();
		this.id = (int)br.ReadByte();
		Kube.WHS.WireId(base.gameObject, this.id);
		this.SetParameters(this.triggerId & 255, this.triggerId >> 8 & 255, this.delay, (int)this.targetType, this.xt, this.yt, this.zt, this.id);
	}

	private void ReposWire()
	{
		Vector3 vector;
		if (this.targetType == TriggerTargetType.noTarget)
		{
			vector = base.transform.position;
		}
		else if (this.targetType == TriggerTargetType.AA)
		{
			vector = Kube.WHS.GetAAPos(this.xt + this.yt * 256 + this.zt * 65536);
			if (vector == Vector3.zero)
			{
				this.DeleteItem();
				return;
			}
		}
		else if (this.targetType == TriggerTargetType.trigger)
		{
			vector = Kube.WHS.GetTriggerPos(this.xt + this.yt * 256 + this.zt * 65536);
			if (vector == Vector3.zero)
			{
				this.DeleteItem();
				return;
			}
		}
		else if (this.targetType == TriggerTargetType.coords)
		{
			vector = new Vector3((float)this.xt, (float)this.yt, (float)this.zt);
		}
		else
		{
			vector = base.transform.position;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, vector, 0.5f);
		base.transform.rotation = Quaternion.LookRotation(vector - base.transform.position);
		float num = Mathf.Max(Vector3.Distance(base.transform.position, vector) * 2f, 1f);
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Clear();
			componentsInChildren[i].gameObject.transform.localScale = new Vector3(0.1f, 0.1f, num);
			componentsInChildren[i].emissionRate = num;
		}
		base.gameObject.transform.localScale = new Vector3(1f, 1f, num);
	}

	private void SetParameters(int playerId)
	{
		this.Init();
		Vector3 position = base.transform.position;
		this.triggerId = Kube.WHS.GetTriggerId(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
		if (this.triggerId == -1)
		{
			Kube.GPS.printMessage(Localize.wire_put_on_switch, Color.cyan);
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		base.transform.rotation = Quaternion.identity;
		this.delay = 0;
		this.targetType = TriggerTargetType.noTarget;
		this.xt = (this.yt = (this.zt = 0));
		if (this.id == -1)
		{
			this.id = Kube.WHS.GetNewWireId(base.gameObject);
		}
		this.NO.CreateNewWire(this.triggerId % 256, (this.triggerId >> 8) % 256, this.delay, (int)this.targetType, this.xt, this.yt, this.zt, this.id, Kube.GPS.playerId);
		this.SetupItem();
		if (Kube.BCS == null)
		{
			Kube.BCS = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControllerScript>();
		}
		if (Kube.BCS != null && Kube.BCS.gameType != GameType.creating)
		{
			base.gameObject.layer = 14;
		}
	}

	public void SetParameters(int triggerId_1, int triggerId_2, int _delay, int _targetType, int _xt, int _yt, int _zt, int _id)
	{
		this.Init();
		this.triggerId = triggerId_1 + 256 * triggerId_2;
		this.delay = _delay;
		this.targetType = (TriggerTargetType)_targetType;
		this.xt = _xt;
		this.yt = _yt;
		this.zt = _zt;
		this.id = _id;
		base.transform.position = Kube.WHS.GetTriggerPos(this.triggerId);
		this.ReposWire();
		if (Kube.BCS == null)
		{
			Kube.BCS = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControllerScript>();
		}
		if (Kube.BCS != null && Kube.BCS.gameType != GameType.creating)
		{
			base.gameObject.layer = 14;
		}
	}

	public void SaveWire()
	{
		this.Init();
		this.NO.SaveWire(this.triggerId % 256, (this.triggerId >> 8) % 256, this.delay, (int)this.targetType, this.xt, this.yt, this.zt, this.id, Kube.GPS.playerId);
	}

	private void DeleteItem()
	{
		this.NO.DeleteWire(this.id);
		if (Kube.OH.hasMenu(new DrawCall(this.setupGUI)))
		{
			Kube.OH.closeMenu(new DrawCall(this.setupGUI));
		}
	}

	private void SetupItem()
	{
		this.Init();
		Kube.OH.openMenu(new DrawCall(this.setupGUI), true, false);
	}

	public void Activate()
	{
		base.Invoke("PlayTrigger", (float)this.delay / 5f);
	}

	private void PlayTrigger()
	{
		Kube.WHS.PlayTrigger(this.triggerId, (int)this.targetType, this.xt, this.yt, this.zt);
	}

	private void Start()
	{
	}

	private void Update()
	{
		this.Init();
		if (this.isConnecting && UnityEngine.Input.GetAxis("Fire1") != 0f)
		{
			Ray ray = Kube.IS.ps.cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, 10f, 24832))
			{
				ActionAreaScript component = raycastHit.collider.gameObject.transform.root.gameObject.GetComponent<ActionAreaScript>();
				TriggerScript component2 = raycastHit.collider.gameObject.transform.root.gameObject.GetComponent<TriggerScript>();
				if (component != null)
				{
					this.isConnecting = false;
					Kube.IS.ps.onlyMove = false;
					Kube.IS.ps.paused = false;
					Screen.lockCursor = true;
					this.targetType = TriggerTargetType.AA;
					this.xt = component.id % 256;
					this.yt = (component.id >> 8) % 256;
					this.zt = (component.id >> 16) % 256;
					this.SaveWire();
				}
				else if (component2 != null)
				{
					this.isConnecting = false;
					Kube.IS.ps.onlyMove = false;
					Kube.IS.ps.paused = false;
					Screen.lockCursor = true;
					this.targetType = TriggerTargetType.trigger;
					this.xt = component2.id % 256;
					this.yt = (component2.id >> 8) % 256;
					this.zt = (component2.id >> 16) % 256;
					this.SaveWire();
				}
				else if (raycastHit.collider.gameObject.layer == 8)
				{
					this.isConnecting = false;
					Kube.IS.ps.onlyMove = false;
					Kube.IS.ps.paused = false;
					Screen.lockCursor = true;
					this.targetType = TriggerTargetType.coords;
					this.xt = Mathf.RoundToInt(raycastHit.point.x + raycastHit.normal.x / 2f);
					this.yt = Mathf.RoundToInt(raycastHit.point.y + raycastHit.normal.y / 2f);
					this.zt = Mathf.RoundToInt(raycastHit.point.z + raycastHit.normal.z / 2f);
					this.SaveWire();
				}
				else
				{
					this.isConnecting = false;
					Kube.IS.ps.onlyMove = false;
					Kube.IS.ps.paused = false;
					Screen.lockCursor = true;
					this.targetType = TriggerTargetType.coords;
					this.xt = Mathf.RoundToInt(raycastHit.collider.gameObject.transform.position.x);
					this.yt = Mathf.RoundToInt(raycastHit.collider.gameObject.transform.position.y);
					this.zt = Mathf.RoundToInt(raycastHit.collider.gameObject.transform.position.z);
					this.SaveWire();
				}
			}
		}
	}

	private void setupGUI()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		if (!this.isConnecting)
		{
			float num3 = 0.5f * num - 350f;
			float num4 = num2 - 320f;
			GUI.skin = Kube.ASS1.mainSkin;
			GUI.DrawTexture(new Rect(num3, num4, 700f, 240f), Kube.ASS3.setupItemTex);
			GUI.skin = Kube.ASS1.bigWhiteLabel;
			GUI.Label(new Rect(num3 + 20f, num4 + 10f, 300f, 40f), Localize.wire_options);
			GUI.skin = Kube.ASS1.triggerSkin;
			GUI.Label(new Rect(num3 + 10f, num4 + 50f, 250f, 30f), Localize.wire_signal_delay + ": ");
			this.delay = (int)GUI.HorizontalScrollbar(new Rect(num3 + 10f, num4 + 80f, 512f, 20f), (float)this.delay, 2f, 0f, 255f);
			GUI.Label(new Rect(num3 + 260f, num4 + 50f, 100f, 30f), string.Concat(new object[]
			{
				string.Empty,
				(float)this.delay / 5f,
				" ",
				Localize.sec
			}));
			GUI.Label(new Rect(num3 + 10f, num4 + 120f, 290f, 30f), Localize.wire_connected_with + ": ");
			if (this.targetType == TriggerTargetType.noTarget)
			{
				GUI.Label(new Rect(num3 + 300f, num4 + 120f, 200f, 30f), Localize.wire_not_connected);
			}
			else if (this.targetType == TriggerTargetType.AA)
			{
				GUI.Label(new Rect(num3 + 300f, num4 + 120f, 200f, 30f), Localize.wire_connected_moveable_cubes);
			}
			else if (this.targetType == TriggerTargetType.trigger)
			{
				GUI.Label(new Rect(num3 + 300f, num4 + 120f, 200f, 30f), Localize.wire_connected_switch);
			}
			else if (this.targetType == TriggerTargetType.item)
			{
				GUI.Label(new Rect(num3 + 300f, num4 + 120f, 200f, 30f), Localize.wire_connected_item);
			}
			else if (this.targetType == TriggerTargetType.coords)
			{
				GUI.Label(new Rect(num3 + 300f, num4 + 120f, 200f, 30f), Localize.wire_connected_coords);
			}
			if (GUI.Button(new Rect(num3 + 10f, num4 + 150f, 250f, 40f), Localize.wire_connect_to))
			{
				this.isConnecting = true;
				Kube.IS.ps.onlyMove = true;
				Kube.IS.ps.paused = false;
				Kube.OH.closeMenu(null);
			}
			if (GUI.Button(new Rect(num3 + 480f, num4 + 180f, 200f, 40f), Localize.apply))
			{
				this.SaveWire();
				Kube.OH.closeMenu(null);
			}
		}
	}

	private void OnGUI()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		if (this.isConnecting)
		{
			GUI.skin = Kube.ASS1.mainSkin;
			GUI.Box(new Rect(num * 0.5f - 300f, num2 - 150f, 600f, 90f), Localize.wire_choose_connect_to);
		}
	}

	public int triggerId;

	private int delay;

	private TriggerTargetType targetType;

	private int xt;

	private int yt;

	private int zt;

	private int id = -1;

	private bool initialized;

	private NetworkObjectScript NO;

	private bool isConnecting;
}
