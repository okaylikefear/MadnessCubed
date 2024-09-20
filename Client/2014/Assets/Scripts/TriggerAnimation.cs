using System;
using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
	public void SetState(int state)
	{
		if (state == 0)
		{
			GetComponent<Animation>().Play(this.animationOFF);
		}
		else
		{
			GetComponent<Animation>().Play(this.animationON);
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
