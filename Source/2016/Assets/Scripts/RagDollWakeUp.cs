using System;
using UnityEngine;

public class RagDollWakeUp : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("WakeUp", 0.03f);
		this.lagArray = new float[6];
		for (int i = 0; i < this.lagArray.Length; i++)
		{
			this.lagArray[i] = 25f;
		}
	}

	private void WakeUp()
	{
		Rigidbody[] componentsInChildren = base.gameObject.GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].WakeUp();
		}
	}

	private void Update()
	{
		this.lagArray[this.currentLagPos] = 1f / Time.deltaTime;
		this.currentLagPos++;
		if (this.currentLagPos >= this.lagArray.Length)
		{
			this.currentLagPos = 0;
		}
		float num = 0f;
		for (int i = 0; i < this.lagArray.Length; i++)
		{
			num += this.lagArray[i];
		}
		num /= (float)this.lagArray.Length;
		if (num <= 20f && this.isFineRagdoll)
		{
			this.SetRagdollFine(false);
		}
	}

	private void SetRagdollFineTrue()
	{
		this.SetRagdollFine(true);
	}

	private void SetRagdollFine(bool isFine = true)
	{
		if (isFine)
		{
			if (this.fineRagdoll == null || this.fastRagdoll == null)
			{
				return;
			}
			this.isFineRagdoll = true;
			this.fineRagdoll.SetActive(true);
			this.fastRagdoll.SetActive(false);
		}
		else
		{
			if (this.fineRagdoll == null || this.fastRagdoll == null)
			{
				return;
			}
			this.isFineRagdoll = false;
			this.fineRagdoll.SetActive(false);
			this.fastRagdoll.SetActive(true);
		}
	}

	public GameObject fineRagdoll;

	public GameObject fastRagdoll;

	private float[] lagArray;

	private int currentLagPos;

	private bool isFineRagdoll = true;
}
