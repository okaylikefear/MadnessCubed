using System;
using kube;
using UnityEngine;

public class TaskKillNMonsters : TaskBase
{
	public int monsterKilled
	{
		get
		{
			return -this._monsterKilled + Kube.GPS.codeI;
		}
		set
		{
			this._monsterKilled = Kube.GPS.codeI - value;
		}
	}

	private void SaveCodeVars()
	{
		this.codeVarsRandom = UnityEngine.Random.Range(10, 1000);
		this._monsterKilled2 = this.monsterKilled + this.codeVarsRandom;
	}

	private void LoadCodeVars()
	{
		this.monsterKilled = this._monsterKilled2 - this.codeVarsRandom;
	}

	private void MonsterDeadByMe(MonsterScript m)
	{
		if (this._monsterType == -1 || this._monsterType == m.monsterNum)
		{
			this.monsterKilled++;
		}
	}

	public int monsterType
	{
		get
		{
			return this._monsterType;
		}
	}

	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.monsterKilled = 0;
		this.initialized = true;
		this.frags = (int)Kube.OH.task.config[0];
		if (Kube.OH.task.config.Length > 1)
		{
			this._monsterType = (int)Kube.OH.task.config[1];
		}
	}

	private void Start()
	{
		this.Init();
	}

	private void Update()
	{
		if (Kube.BCS.gameProcess != BattleControllerScript.GameProcess.game)
		{
			return;
		}
		this.UpdateHUD();
		if (this.monsterKilled >= this.frags)
		{
			base.Done();
		}
	}

	private void UpdateHUD()
	{
		int num = this.frags - this.monsterKilled;
	}

	private int _monsterKilled;

	private int codeVarsRandom;

	private int _monsterKilled2;

	protected int frags;

	protected int _monsterType = -1;

	private bool initialized;
}
