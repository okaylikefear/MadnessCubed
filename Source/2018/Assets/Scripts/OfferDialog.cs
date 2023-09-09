using System;
using kube.data;
using UnityEngine;

public class OfferDialog : MonoBehaviour
{
	protected virtual void OfferInit()
	{
	}

	private void Start()
	{
		UnityEngine.Debug.Log("Start Dialog");
	}

	private void OnEnable()
	{
		UnityEngine.Debug.Log("En Dialog");
		this.OfferInit();
	}

	private void Update()
	{
	}

	public Offer offer;
}
