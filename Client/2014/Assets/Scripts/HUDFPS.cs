using System;
using UnityEngine;

public class HUDFPS : MonoBehaviour
{
	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (!base.guiText)
		{
			UnityEngine.Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
			base.enabled = false;
			return;
		}
		this.timeleft = this.updateInterval;
	}

	private void Update()
	{
		this.timeleft -= Time.deltaTime;
		this.accum += Time.timeScale / Time.deltaTime;
		this.frames++;
		if ((double)this.timeleft <= 0.0)
		{
			float num = this.accum / (float)this.frames;
			string text = string.Format("{0:F2} FPS", num);
			base.guiText.text = text;
			if (num < 30f)
			{
				base.guiText.material.color = Color.yellow;
			}
			else if (num < 10f)
			{
				base.guiText.material.color = Color.red;
			}
			else
			{
				base.guiText.material.color = Color.green;
			}
			this.timeleft = this.updateInterval;
			this.accum = 0f;
			this.frames = 0;
		}
	}

	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;
}
