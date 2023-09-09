using System;
using kube;
using UnityEngine;

public class TaskBuyItem : TaskBase
{
	private void Start()
	{
		this.itemId = (int)this.desc.config[0];
		this.itemType = (int)this.desc.config[1];
		this.itemCnt = (int)this.desc.config[2];
		this.Check();
	}

	protected void Check()
	{
		if (Kube.GPS.inventarItems[this.itemId] >= this.itemCnt)
		{
			this.completed = true;
		}
	}

	private void Update()
	{
		this.Check();
		if (this.completed)
		{
			base.Done();
			EndGameStats endGameStats = default(EndGameStats);
			UnityEngine.Object.Destroy(base.gameObject, 2f);
		}
	}

	private int itemId;

	private int itemCnt;

	private int itemType;
}
