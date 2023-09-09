using System;
using System.Collections;
using kube;
using UnityEngine;

public class PointTextGUIScript : MonoBehaviour
{
	private void SetText(string _text)
	{
		this.yPos = 0.65f;
		this.text = _text;
	}

	private void SetText(ArrayList list)
	{
		this.mainColor = (Color)list[0];
		this.fontSize = (int)list[1];
		this.yPos = (float)list[2];
		this.xPos = (float)list[3];
		this.text = (string)list[4];
	}

	private void Start()
	{
		this.startTime = Time.time;
	}

	private void Update()
	{
		if (Time.time - this.startTime > this.lifeTime)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnGUI()
	{
		if (Kube.OH.emptyScreen)
		{
			return;
		}
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		float num3 = (Time.time - this.startTime) / this.lifeTime;
		float num4 = num3 * num2 * 0.07f;
		GUISkin missionSkin = Kube.ASS1.missionSkin;
		GUIStyle style = missionSkin.GetStyle("EPBODY");
		style.fontSize = this.fontSize;
		style.alignment = TextAnchor.MiddleCenter;
		Color black = Color.black;
		black.a = 1f - num3;
		GUI.color = black;
		GUI.Label(new Rect(2f, num2 * this.yPos - num4, num - 2f, 30f), this.text, style);
		black = this.mainColor;
		black.a = 1f - num3;
		GUI.color = black;
		GUI.Label(new Rect(0f, num2 * this.yPos - num4 - 2f, num - 2f, 30f), this.text, style);
	}

	private string text;

	private Color mainColor = Color.white;

	private float yPos = 0.2f;

	private float xPos = 0.5f;

	private int fontSize = 50;

	private float startTime;

	private float lifeTime = 2f;
}
