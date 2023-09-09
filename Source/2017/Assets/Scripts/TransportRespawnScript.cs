using System;
using kube;
using kube.ui;
using UnityEngine;

public class TransportRespawnScript : GameMapItem
{
	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		if (this.id == -1)
		{
			this.id = Kube.WHS.GetNewTransportRespawnId(base.gameObject);
			MonoBehaviour.print("New transportRespawn id: " + this.id);
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

	public override void SaveMap(KubeStream bw)
	{
		bw.WriteByte((byte)this.x);
		bw.WriteByte((byte)this.y);
		bw.WriteByte((byte)this.z);
		bw.WriteByte((byte)this.type);
		bw.WriteByte((byte)this.state);
		bw.WriteByte((byte)this.respawnTime);
		bw.WriteByte((byte)this.healthMultiplier);
		bw.WriteByte((byte)this.damageMultiplier);
		bw.WriteByte((byte)this.id);
	}

	public override void LoadMap(KubeStream br)
	{
		this.x = (int)br.ReadByte();
		this.y = (int)br.ReadByte();
		this.z = (int)br.ReadByte();
		this.type = (int)br.ReadByte();
		this.state = (int)br.ReadByte();
		this.respawnTime = (int)br.ReadByte();
		this.healthMultiplier = (int)br.ReadByte();
		this.damageMultiplier = (int)br.ReadByte();
		this.id = (int)br.ReadByte();
		Kube.WHS.transportRespawnS[this.id] = this;
		this.SetParameters(this.x, this.y, this.z, this.type, this.state, this.respawnTime, this.healthMultiplier, this.damageMultiplier, this.id);
	}

	private void SetupItem()
	{
		this.Init();
		if (Kube.BCS.gameType == GameType.creating)
		{
			Kube.OH.openMenu(new DrawCall(this.setupGUI), true, false);
		}
	}

	public void SaveTransportRespawn()
	{
		this.Init();
		this.NO.SaveTransportRespawn(this.x, this.y, this.z, this.type, this.state, this.respawnTime, this.healthMultiplier, this.damageMultiplier, this.id);
	}

	public void SetParameters(int _x, int _y, int _z, int _type, int _state, int _respawnTime, int _healthMultiplier, int _damageMultiplier, int _id)
	{
		this.id = _id;
		this.Init();
		this.x = _x;
		this.y = _y;
		this.z = _z;
		this.type = _type;
		this.state = _state;
		this.respawnTime = _respawnTime;
		this.healthMultiplier = _healthMultiplier;
		this.damageMultiplier = _damageMultiplier;
		Texture mainTexture = null;
		int key = Kube.WHS.FindGameItemType(base.gameObject);
		if (Kube.OH.gameItemsTex.ContainsKey(key))
		{
			mainTexture = Kube.OH.gameItemsTex[key];
		}
		base.transform.Find("GameObject/monstertex").GetComponent<Renderer>().material.mainTexture = mainTexture;
	}

	private void setupGUI()
	{
		int num = Kube.WHS.FindGameItemType(base.gameObject);
		this.Init();
		float num2 = (float)KUI.width;
		float num3 = (float)KUI.height;
		float num4 = 0.5f * num2 - 350f;
		float num5 = num3 - 320f;
		GUI.skin = Kube.ASS1.mainSkin;
		GUI.DrawTexture(new Rect(num4, num5, 700f, 240f), Kube.ASS1.tabTex);
		GUI.skin = Kube.ASS1.bigWhiteLabel;
		GUI.Label(new Rect(num4 + 20f, num5 + 10f, 550f, 40f), Localize.transport_options);
		GUI.skin = Kube.ASS1.smallWhiteSkin;
		GUI.Label(new Rect(num4 + 10f, num5 + 50f, 350f, 30f), Localize.transport_type + ": " + Localize.gameItemsNames[num]);
		GUI.Label(new Rect(num4 + 10f, num5 + 80f, 300f, 30f), Localize.ressurection_time + ": " + Localize.respawnTimeStr[this.respawnTime]);
		this.respawnTime = (int)GUI.HorizontalScrollbar(new Rect(num4 + 340f, num5 + 85f, 200f, 20f), (float)this.respawnTime, 1f, 0f, (float)Localize.respawnTimeStr.Length);
		GUI.Label(new Rect(num4 + 10f, num5 + 110f, 300f, 30f), Localize.monster_health_mult + ": x" + (int)Mathf.Pow(2f, (float)this.healthMultiplier));
		this.healthMultiplier = (int)GUI.HorizontalScrollbar(new Rect(num4 + 340f, num5 + 115f, 200f, 20f), (float)this.healthMultiplier, 1f, 0f, 16f);
		GUI.Label(new Rect(num4 + 10f, num5 + 140f, 300f, 30f), Localize.monster_damage_mult + ": x" + Mathf.Pow(2f, (float)this.damageMultiplier / 4f));
		this.damageMultiplier = (int)GUI.HorizontalScrollbar(new Rect(num4 + 340f, num5 + 145f, 200f, 20f), (float)this.damageMultiplier, 1f, 0f, 16f);
		if (GUI.Button(new Rect(num4 + 600f, num5 + 150f, 100f, 30f), Localize.save))
		{
			this.SaveTransportRespawn();
			Kube.OH.closeMenu(null);
		}
	}

	private void OnDestroy()
	{
		if (Kube.OH != null && Kube.OH.hasMenu(new DrawCall(this.setupGUI)))
		{
			Kube.OH.closeMenu(null);
		}
	}

	private void Start()
	{
		this.Init();
	}

	private void Update()
	{
	}

	public int x;

	public int y;

	public int z;

	private int rot;

	public int type;

	private int state;

	public int respawnTime;

	public int healthMultiplier;

	public int damageMultiplier;

	[HideInInspector]
	public int id = -1;

	public string transportPrefabName;

	public int[] secToRespawn = new int[]
	{
		10,
		30,
		60,
		120,
		300,
		600,
		1800,
		99999999
	};

	private bool initialized;

	private NetworkObjectScript NO;
}
