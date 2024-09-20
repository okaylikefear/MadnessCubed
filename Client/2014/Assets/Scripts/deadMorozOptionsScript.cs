using System;
using UnityEngine;

public class deadMorozOptionsScript : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Animation>()["walk"].speed = 3.5f;
		GetComponent<Animation>()["magic"].speed = 0.5f;
		AnimationEvent animationEvent = new AnimationEvent();
		animationEvent.functionName = "StepEvent";
		animationEvent.time = 1.2f;
		GetComponent<Animation>()["walk"].clip.AddEvent(animationEvent);
		AnimationEvent animationEvent2 = new AnimationEvent();
		animationEvent2.functionName = "StepEvent";
		animationEvent2.time = 3.1f;
		GetComponent<Animation>()["walk"].clip.AddEvent(animationEvent2);
		AnimationEvent animationEvent3 = new AnimationEvent();
		animationEvent3.functionName = "PosohShoot";
		animationEvent3.time = 0.5f;
		GetComponent<Animation>()["flash"].clip.AddEvent(animationEvent3);
	}

	private void StepEvent()
	{
		if (this.stepSound != null)
		{
			UnityEngine.Object.Instantiate(this.stepSound, base.transform.position, base.transform.rotation);
		}
	}

	private void PosohShoot()
	{
		UnityEngine.Object.Instantiate(this.posohShootGO, this.posohShootPoint.transform.position, this.posohShootPoint.transform.rotation);
		if (this.posohShootSound != null)
		{
			UnityEngine.Object.Instantiate(this.posohShootSound, this.posohShootPoint.transform.position, this.posohShootPoint.transform.rotation);
		}
	}

	private void Update()
	{
	}

	public GameObject stepSound;

	public GameObject posohShootPoint;

	public GameObject posohShootSound;

	public GameObject posohShootGO;
}
