using System;
using kube;
using UnityEngine;

public class TutorialNGUItween : MonoBehaviour
{
	private void Update()
	{
		if (this.tutorS == null)
		{
			this.tutorS = Kube.TS;
		}
		if (this.tutorS == null)
		{
			return;
		}
		if (this.tutorS.currentNumOfTutor == this.activateTutorStep)
		{
			bool flag = true;
			if (this.ifButtonOFF && this.buttonToCheckOff != null)
			{
				flag = !this.buttonToCheckOff.value;
			}
			bool flag2 = true;
			if (this.activateTutorSubstep >= 0 && this.activateTutorSubstep != this.tutorS.currentStepOfTutor)
			{
				flag2 = false;
			}
			if (flag && flag2)
			{
				if (this.goToActivate != null)
				{
					this.goToActivate.SetActive(true);
				}
				if (this.tweenerToActivate != null)
				{
					this.tweenerToActivate.enabled = true;
				}
			}
			else
			{
				if (this.goToActivate != null)
				{
					this.goToActivate.SetActive(false);
				}
				if (this.tweenerToActivate != null)
				{
					this.tweenerToActivate.enabled = false;
					this.tweenerToActivate.tweenFactor = 0f;
				}
			}
		}
		else
		{
			if (this.goToActivate != null)
			{
				this.goToActivate.SetActive(false);
			}
			if (this.tweenerToActivate != null)
			{
				this.tweenerToActivate.enabled = false;
				this.tweenerToActivate.tweenFactor = 0f;
			}
		}
	}

	public GameObject goToActivate;

	public UITweener tweenerToActivate;

	public bool ifButtonOFF;

	public UIToggle buttonToCheckOff;

	public int activateTutorStep = -1;

	public int activateTutorSubstep = -1;

	private TutorialScript tutorS;
}
