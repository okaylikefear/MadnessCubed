using System;
using kube;
using UnityEngine;

public class DieEffectScipt : MonoBehaviour
{
	private void Start()
	{
		this.startEffectTime = Time.time + this.startEffect;
		this.renderers = base.gameObject.GetComponentsInChildren<Renderer>();
		this.bones = base.gameObject.GetComponentsInChildren<Rigidbody>();
	}

	private void Update()
	{
		if (!this.effectStarted)
		{
			if (Time.time > this.startEffectTime)
			{
				this.effectStarted = true;
				for (int i = 0; i < this.bones.Length; i++)
				{
					this.bones[i].useGravity = false;
					this.bones[i].AddForce(Vector3.up * UnityEngine.Random.value * 0.5f, ForceMode.VelocityChange);
					this.bones[i].AddTorque(UnityEngine.Random.insideUnitSphere, ForceMode.VelocityChange);
				}
				for (int j = 0; j < this.renderers.Length; j++)
				{
					if (this.renderers[j])
					{
						this.renderers[j].material.shader = Kube.OH.dieEffectMaterial.shader;
					}
				}
			}
		}
		else
		{
			float num = (Time.time - this.startEffectTime) / this.effectLength;
			for (int k = 0; k < this.renderers.Length; k++)
			{
				if (!(this.renderers[k] == null))
				{
					Color color = this.renderers[k].material.GetColor("_Color");
					color.a = 1f - num;
					if (this.renderers[k])
					{
						this.renderers[k].material.SetColor("_Color", color);
					}
				}
			}
		}
	}

	public float startEffect = 15f;

	public float effectLength = 20f;

	private float startEffectTime;

	private bool effectStarted;

	private Renderer[] renderers;

	private Rigidbody[] bones;
}
