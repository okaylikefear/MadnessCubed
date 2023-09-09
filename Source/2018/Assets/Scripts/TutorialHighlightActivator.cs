using System;
using kube;
using UnityEngine;

public class TutorialHighlightActivator : MonoBehaviour
{
	private TutorialScript tutorS
	{
		get
		{
			if (this._tutorS)
			{
				return this._tutorS;
			}
			GameObject gameObject = GameObject.FindGameObjectWithTag("SystemGO");
			if (!gameObject)
			{
				return null;
			}
			this._tutorS = gameObject.GetComponent<TutorialScript>();
			return this._tutorS;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		bool flag = false;
		if (this.activateIf == TutorialHighlightActivator.ActivateIf.KirkaInHands)
		{
			if (this.tutorS == null)
			{
				return;
			}
			if (this.tutorS.currentNumOfTutor == 5 && Kube.GPS.fastInventarWeapon[0].Num == 0)
			{
				flag = true;
			}
		}
		else if (this.activateIf == TutorialHighlightActivator.ActivateIf.needTraining_GAME)
		{
			if (Kube.GPS.needTraining && base.transform.parent.gameObject.GetComponent<UIToggle>() != null && !base.transform.parent.gameObject.GetComponent<UIToggle>().value)
			{
				flag = true;
			}
		}
		else if (this.activateIf == TutorialHighlightActivator.ActivateIf.needTraining_PLAY && Kube.GPS.needTraining)
		{
			flag = true;
		}
		if (flag)
		{
			this.goToActivate.SetActive(true);
		}
		else
		{
			this.goToActivate.SetActive(false);
		}
	}

	public TutorialHighlightActivator.ActivateIf activateIf;

	public GameObject goToActivate;

	private TutorialScript _tutorS;

	public enum ActivateIf
	{
		nothing,
		KirkaInHands,
		needTraining_GAME,
		needTraining_PLAY
	}
}
