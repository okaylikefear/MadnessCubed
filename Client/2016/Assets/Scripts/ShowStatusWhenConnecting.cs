using System;
using UnityEngine;

public class ShowStatusWhenConnecting : MonoBehaviour
{
	private void OnGUI()
	{
		if (this.Skin != null)
		{
			GUI.skin = this.Skin;
		}
		float num = 400f;
		float num2 = 100f;
		Rect screenRect = new Rect(((float)Screen.width - num) / 2f, ((float)Screen.height - num2) / 2f, num, num2);
		GUILayout.BeginArea(screenRect, GUI.skin.box);
		GUILayout.Label("Connecting" + this.GetConnectingDots(), GUI.skin.customStyles[0], new GUILayoutOption[0]);
		GUILayout.Label("Status: " + PhotonNetwork.connectionStateDetailed, new GUILayoutOption[0]);
		GUILayout.EndArea();
		if (PhotonNetwork.connectionStateDetailed == PeerState.Joined)
		{
			base.enabled = false;
		}
	}

	private string GetConnectingDots()
	{
		string text = string.Empty;
		int num = Mathf.FloorToInt(Time.timeSinceLevelLoad * 3f % 4f);
		for (int i = 0; i < num; i++)
		{
			text += " .";
		}
		return text;
	}

	public GUISkin Skin;
}
