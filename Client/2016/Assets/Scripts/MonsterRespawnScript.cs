using System;
using kube;
using kube.ui;
using UnityEngine;

public class MonsterRespawnScript : GameMapItem
{
	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		if (this.id == -1)
		{
			this.id = Kube.WHS.GetNewMonsterRespawnId(base.gameObject);
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
		Kube.WHS.monsterRespawnS[this.id] = this;
		this.SetParameters(this.x, this.y, this.z, this.type, this.state, this.respawnTime, this.healthMultiplier, this.damageMultiplier, this.id);
	}

	public void OrderToCreateMonster()
	{
		if (string.IsNullOrEmpty(this.monsterPrefabName))
		{
			this.monsterPrefabName = Kube.OH.monsterPrefabName[Kube.WHS.monsterRespawnS[this.id].type];
		}
		GameObject gameObject = PhotonNetwork.Instantiate(this.monsterPrefabName, base.transform.position, base.transform.rotation, 0);
		gameObject.SendMessage("SetMonsterNum", Kube.WHS.monsterRespawnS[this.id].type);
		gameObject.SendMessage("SetRespawnNum", this.id);
		gameObject.SendMessage("SetHealthMultiplier", this.healthMultiplier);
		gameObject.SendMessage("SetDamageMultiplier", this.damageMultiplier);
	}

	private void SetupItem()
	{
		this.Init();
		Kube.OH.openMenu(new DrawCall(this.setupGUI), true, false);
	}

	public void SaveMonsterRespawn()
	{
		this.Init();
		this.NO.SaveMonsterRespawn(this.x, this.y, this.z, this.type, this.state, this.respawnTime, this.healthMultiplier, this.damageMultiplier, this.id);
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

	private void OnDestroy()
	{
		if (Kube.OH && Kube.OH.hasMenu(new DrawCall(this.setupGUI)))
		{
			Kube.OH.closeMenu(null);
		}
	}

	private void setupGUI()
	{
		this.Init();
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		float num3 = 0.5f * num - 350f;
		float num4 = num2 - 320f;
		GUI.skin = Kube.ASS1.mainSkin;
		GUI.DrawTexture(new Rect(num3, num4, 700f, 240f), Kube.ASS1.tabTex);
		GUI.skin = Kube.ASS1.bigWhiteLabel;
		GUI.Label(new Rect(num3 + 20f, num4 + 10f, 550f, 40f), Localize.monster_options);
		if (Kube.GPS.moderType != 0)
		{
			int num5 = 0;
			GUI.skin = Kube.ASS1.triggerSkin;
			string text = this.FindMonsterName();
			if (KUI.LRButton(new Rect(num3 + 10f, num4 + 50f, 350f, 30f), text, out num5))
			{
				this.type += num5;
				if (this.type < 0)
				{
					this.type = 0;
				}
				if (this.type >= Localize.monsterName.Length)
				{
					this.type = Localize.monsterName.Length - 1;
				}
			}
		}
		else
		{
			GUI.skin = Kube.ASS1.smallWhiteSkin;
			GUI.Label(new Rect(num3 + 10f, num4 + 50f, 350f, 30f), Localize.monster_type + ": " + this.FindMonsterName());
		}
		if (Kube.GPS.isVIP)
		{
			GUI.Label(new Rect(num3 + 10f, num4 + 80f, 300f, 30f), Localize.ressurection_time + ": " + Localize.respawnTimeStr[this.respawnTime]);
			this.respawnTime = (int)GUI.HorizontalScrollbar(new Rect(num3 + 340f, num4 + 85f, 200f, 20f), (float)this.respawnTime, 1f, 0f, (float)Localize.respawnTimeStr.Length);
			GUI.Label(new Rect(num3 + 10f, num4 + 110f, 300f, 30f), Localize.monster_health_mult + ": x" + (int)Mathf.Pow(2f, (float)this.healthMultiplier));
			this.healthMultiplier = (int)GUI.HorizontalScrollbar(new Rect(num3 + 340f, num4 + 115f, 200f, 20f), (float)this.healthMultiplier, 1f, 0f, 16f);
			GUI.Label(new Rect(num3 + 10f, num4 + 140f, 300f, 30f), Localize.monster_damage_mult + ": x" + Mathf.Pow(2f, (float)this.damageMultiplier / 4f));
			this.damageMultiplier = (int)GUI.HorizontalScrollbar(new Rect(num3 + 340f, num4 + 145f, 200f, 20f), (float)this.damageMultiplier, 1f, 0f, 16f);
		}
		else
		{
			GUI.Label(new Rect(num3 + 10f, num4 + 80f, 300f, 30f), Localize.ressurection_time + ": " + Localize.need_vip);
			this.respawnTime = Array.IndexOf<int>(this.secToRespawn, 60);
			if (this.respawnTime == -1)
			{
				UnityEngine.Debug.LogError("Bad respawn time");
			}
			GUI.Label(new Rect(num3 + 10f, num4 + 80f, 300f, 30f), Localize.monster_health_mult + ": " + Localize.need_vip);
			this.healthMultiplier = 0;
			GUI.Label(new Rect(num3 + 10f, num4 + 80f, 300f, 30f), Localize.monster_damage_mult + ": " + Localize.need_vip);
			this.damageMultiplier = 0;
		}
		if (this.healthMultiplier >= 3 || this.damageMultiplier >= 4)
		{
			GUI.Label(new Rect(num3 + 10f, num4 + 170f, 600f, 30f), Localize.monster_state + ": " + Localize.boss);
		}
		if (GUI.Button(new Rect(num3 + 600f, num4 + 150f, 100f, 30f), Localize.save))
		{
			this.SaveMonsterRespawn();
			Kube.OH.closeMenu(null);
		}
	}

	private string FindMonsterName()
	{
		string result = Localize.monsterName[0];
		if (this.type < Localize.monsterName.Length)
		{
			result = Localize.monsterName[this.type];
		}
		else
		{
			int num = Kube.WHS.FindGameItemType(base.gameObject);
			result = Localize.gameItemsNames[num];
		}
		return result;
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

	public int id = -1;

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

	public string monsterPrefabName;
}
