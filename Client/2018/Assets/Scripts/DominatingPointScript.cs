using System;
using kube;
using UnityEngine;

public class DominatingPointScript : MonoBehaviour
{
	public int teamCaptured
	{
		get
		{
			return this._teamCaptured - 1;
		}
		set
		{
			this._teamCaptured = value + 1;
		}
	}

	private void Start()
	{
		if (Kube.BCS.gameType != GameType.creating && Kube.BCS.gameType != GameType.test && Kube.BCS.gameType != GameType.dominating)
		{
			base.transform.root.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider c)
	{
		if (c.gameObject.layer == LayerMask.NameToLayer("ThisPlayer"))
		{
			PlayerScript component = c.gameObject.GetComponent<PlayerScript>();
			Kube.BCS.NO.ChangeDominatingPointState(base.transform.root.gameObject.GetComponent<ItemPropsScript>().id, component.team);
		}
	}

	public void ChangeTeam(int newTeam)
	{
		if (newTeam == this.teamCaptured)
		{
			return;
		}
		this.teamCaptured = newTeam;
		if (newTeam == -1)
		{
			this.dominatingPointRenderer.GetComponent<Renderer>().material.color = Color.white;
		}
		else
		{
			this.dominatingPointRenderer.GetComponent<Renderer>().material.color = Kube.OH.teamColor[newTeam];
		}
		this.light.color = this.dominatingPointRenderer.GetComponent<Renderer>().material.color;
		this.light2.color = this.dominatingPointRenderer.GetComponent<Renderer>().material.color;
		UnityEngine.Object.Instantiate(Kube.ASS4.soundDominating, Vector3.zero, Quaternion.identity);
	}

	private int _teamCaptured;

	public GameObject dominatingPointRenderer;

	public new Light light;

	public Light light2;
}
