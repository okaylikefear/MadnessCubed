using System;
using kube;
using kube.ui;
using UnityEngine;

public class JumperScript : GameMapItem
{
	public override void SaveMap(KubeStream bw)
	{
		bw.WriteByte((byte)this.power);
	}

	public override void LoadMap(KubeStream br)
	{
		this.power = (int)br.ReadByte();
	}

	private void OnTriggerEnter(Collider other)
	{
		Vector3 a = base.transform.rotation * this.dir;
		if (other.gameObject.transform.root.gameObject.layer == 9)
		{
			PlayerScript component = other.gameObject.transform.root.gameObject.GetComponent<PlayerScript>();
			component.Push(a * (float)this.power);
		}
	}

	private void SetupItem()
	{
		Kube.OH.openMenu(new DrawCall(this.setupGUI), true, false);
	}

	private void setupGUI()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		float num3 = 0.5f * num - 350f;
		float num4 = num2 - 320f;
		GUI.skin = Kube.ASS1.mainSkin;
		GUI.DrawTexture(new Rect(num3, num4, 700f, 240f), Kube.ASS3.setupItemTex);
		GUI.skin = Kube.ASS1.bigWhiteLabel;
		GUI.Label(new Rect(num3 + 20f, num4 + 10f, 300f, 40f), Localize.jumper_options);
		GUI.Label(new Rect(num3 + 300f, num4 + 45f, 250f, 30f), Localize.jumper_options_height + this.power);
		this.power = (int)GUI.HorizontalScrollbar(new Rect(num3 + 300f, num4 + 35f, 300f, 20f), (float)this.power, 1f, 0f, 30f);
		if (GUI.Button(new Rect(num3 + 500f, num4 + 140f, 180f, 50f), Localize.apply))
		{
			Kube.BCS.NO.SaveMapItem(this);
			Kube.OH.closeMenu(null);
		}
	}

	public Vector3 dir = new Vector3(0f, 1f, 0f);

	protected int power = 10;
}
