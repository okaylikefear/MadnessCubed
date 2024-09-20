using System;
using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
	public void SetState(int state)
	{
		if (state == 0)
		{
			base.GetComponent<Animation>().Play(this.animationOFF);
		}
		else
		{
			base.GetComponent<Animation>().Play(this.animationON);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public string animationON;

	public string animationOFF;
}
