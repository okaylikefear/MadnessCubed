using System;
using MoPhoGames.USpeak.Interface;
using UnityEngine;

[AddComponentMenu("USpeak/Default Talk Controller")]
public class DefaultTalkController : MonoBehaviour, IUSpeakTalkController
{
	public void OnInspectorGUI()
	{
	}

	public bool ShouldSend()
	{
		if (this.ToggleMode == 0)
		{
			this.val = UnityEngine.Input.GetKey(this.TriggerKey);
		}
		else if (UnityEngine.Input.GetKeyDown(this.TriggerKey))
		{
			this.val = !this.val;
		}
		return this.val;
	}

	[SerializeField]
	[HideInInspector]
	public KeyCode TriggerKey;

	[HideInInspector]
	[SerializeField]
	public int ToggleMode;

	private bool val;
}
