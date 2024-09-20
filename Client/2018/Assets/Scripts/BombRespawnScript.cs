using System;
using kube;
using kube.ui;
using UnityEngine;

public class BombRespawnScript : GameMapItem
{
	public override void SaveMap(KubeStream bw)
	{
		bw.WriteByte((byte)this.x);
		bw.WriteByte((byte)this.y);
		bw.WriteByte((byte)this.z);
	}

	public override void LoadMap(KubeStream br)
	{
		this.x = (int)br.ReadByte();
		this.y = (int)br.ReadByte();
		this.z = (int)br.ReadByte();
		this.SetParameters(this.x, this.y, this.z);
	}

	public void SetParameters(int _x, int _y, int _z)
	{
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
	}

	private void Start()
	{
		int key = Kube.WHS.FindGameItemType(base.gameObject);
		base.transform.Find("GameObject/monstertex").GetComponent<Renderer>().material.mainTexture = Kube.OH.gameItemsTex[key];
		this.col = base.GetComponent<BoxCollider>();
	}

	private void Update()
	{
		this.col.enabled = (Kube.BCS.gameType == GameType.creating);
	}

	public int x;

	public int y;

	public int z;

	private BoxCollider col;
}
