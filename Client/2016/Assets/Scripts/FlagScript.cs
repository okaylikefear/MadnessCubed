using System;
using System.Collections;
using kube;
using UnityEngine;

public class FlagScript : MonoBehaviour
{
	private void Start()
	{
		this.flagState.team = this.team;
		this.flagState.droppedTime = 0f;
		this.flagState.playerCaptured = 0;
		this.flagState.state = FlagState.onBase;
		if (Kube.BCS.gameType != GameType.creating && Kube.BCS.gameType != GameType.test && Kube.BCS.gameType != GameType.captureTheFlag)
		{
			base.transform.root.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (this.flagState.state == FlagState.dropped && Time.time > this.flagState.droppedTime + this.dropTimeToReturn)
		{
			Kube.BCS.NO.ChangeFlagState(this.flagState.team, FlagState.onBase, 0);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		int num = 0;
		for (int i = 0; i < Kube.BCS.playersInfo.Length; i++)
		{
			if (!(Kube.BCS.playersInfo[i].ps == null))
			{
				PlayerScript ps = Kube.BCS.playersInfo[i].ps;
				if (ps.team == this.flagState.team)
				{
					num++;
				}
			}
		}
		if (num == 0)
		{
			Kube.GPS.printMessage(Localize.cant_take_flag_no_players, Color.red);
			return;
		}
		if (other.gameObject.layer == LayerMask.NameToLayer("ThisPlayer"))
		{
			PlayerScript component = other.gameObject.GetComponent<PlayerScript>();
			if (this.flagState.state == FlagState.onBase && component.team != this.flagState.team && !component.carryingTheFlag)
			{
				Kube.BCS.NO.ChangeFlagState(this.flagState.team, FlagState.captured, component.onlineId);
			}
			else if (this.flagState.state == FlagState.onBase && component.team == this.flagState.team && component.carryingTheFlag)
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag("Flag");
				int loseTeam = 0;
				for (int j = 0; j < array.Length; j++)
				{
					FlagScript component2 = array[j].GetComponent<FlagScript>();
					if (component2.flagState.playerCaptured == component.onlineId)
					{
						Kube.BCS.NO.ChangeFlagState(component2.flagState.team, FlagState.onBase, component.onlineId);
						loseTeam = j;
						break;
					}
				}
				Kube.BCS.NO.FlagCaptured(component.onlineId, component.team, loseTeam);
				UnityEngine.Object.Instantiate(Kube.ASS3.flagCapturedEffect, base.transform.position + Vector3.up * 2f, Quaternion.identity);
				BattleControllerScript bcs = Kube.BCS;
				bcs.bonusCounters.capturedTheFlag = bcs.bonusCounters.capturedTheFlag + 1;
			}
		}
	}

	public void ChangeFlagState(int team, int state, int playerId)
	{
		if (this.flagState.team == team)
		{
			this.flagState.state = (FlagState)state;
			this.flagState.playerCaptured = playerId;
			if (this.flagState.state == FlagState.captured)
			{
				for (int i = 0; i < Kube.BCS.playersInfo.Length; i++)
				{
					if (!(Kube.BCS.playersInfo[i].ps == null))
					{
						PlayerScript ps = Kube.BCS.playersInfo[i].ps;
						if (ps.onlineId == playerId)
						{
							this.flag.transform.parent = ps.flagHolder;
							this.flag.transform.localPosition = Vector3.zero;
							this.flag.transform.localRotation = Quaternion.identity;
							ps.carryingTheFlag = true;
							Kube.GPS.printSystemMessage(string.Concat(new string[]
							{
								AuxFunc.DecodeRussianName(ps.playerName),
								" ",
								Localize.takes_flag,
								" ",
								Localize.flag_color_name[team],
								" ",
								Localize.flag
							}), new Color(1f, 1f, 1f, 0.5f));
							break;
						}
					}
				}
				this.flag.GetComponent<Collider>().enabled = false;
				this.flag.GetComponent<Rigidbody>().isKinematic = true;
				this.flagTouchGO.GetComponent<Collider>().enabled = false;
				UnityEngine.Object.Instantiate(Kube.ASS4.soundFlagAlert, Vector3.zero, Quaternion.identity);
			}
			if (this.flagState.state == FlagState.onBase)
			{
				this.flag.transform.parent = this.flagBase;
				this.flag.transform.localPosition = Vector3.zero;
				this.flag.transform.localRotation = Quaternion.identity;
				this.flag.GetComponent<Collider>().enabled = false;
				this.flag.GetComponent<Rigidbody>().isKinematic = true;
				this.flagTouchGO.GetComponent<Collider>().enabled = false;
				for (int j = 0; j < Kube.BCS.playersInfo.Length; j++)
				{
					if (!(Kube.BCS.playersInfo[j].ps == null))
					{
						PlayerScript ps2 = Kube.BCS.playersInfo[j].ps;
						if (ps2.onlineId == this.flagState.playerCaptured)
						{
							ps2.carryingTheFlag = false;
							break;
						}
					}
				}
			}
			if (this.flagState.state == FlagState.dropped)
			{
				this.flag.transform.parent = null;
				this.flag.GetComponent<Collider>().enabled = true;
				base.Invoke("MakeFlagRigidbody", 0.5f);
				this.flagTouchGO.GetComponent<Collider>().enabled = true;
				this.flagState.droppedTime = Time.time;
				Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					componentsInChildren[k].enabled = true;
				}
				for (int l = 0; l < Kube.BCS.playersInfo.Length; l++)
				{
					if (!(Kube.BCS.playersInfo[l].ps == null))
					{
						PlayerScript ps3 = Kube.BCS.playersInfo[l].ps;
						if (ps3.onlineId == playerId)
						{
							ps3.carryingTheFlag = false;
							Kube.GPS.printSystemMessage(string.Concat(new string[]
							{
								AuxFunc.DecodeRussianName(ps3.playerName),
								" ",
								Localize.dropped_flag,
								" ",
								Localize.flag_color_name[team],
								" ",
								Localize.flag
							}), new Color(1f, 1f, 1f, 0.5f));
							break;
						}
					}
				}
			}
		}
	}

	private void MakeFlagRigidbody()
	{
		this.flag.GetComponent<Rigidbody>().isKinematic = false;
		this.flag.GetComponent<Rigidbody>().AddForce(-Vector3.up, ForceMode.Impulse);
	}

	public void MyOnCollisionEnter(Collider c)
	{
		if (c.gameObject.layer == LayerMask.NameToLayer("ThisPlayer"))
		{
			PlayerScript component = c.gameObject.GetComponent<PlayerScript>();
			if (this.flagState.state == FlagState.dropped)
			{
				if (this.flagState.team == component.team)
				{
					Kube.BCS.NO.ChangeFlagState(this.flagState.team, FlagState.onBase, component.onlineId);
					ArrayList arrayList = new ArrayList();
					arrayList.Add(Color.white);
					arrayList.Add(40);
					arrayList.Add(0.75f);
					arrayList.Add(0.5f);
					arrayList.Add(Localize.you_returned_flag);
					(UnityEngine.Object.Instantiate(Kube.OH.pointsText, base.transform.position + Vector3.up * 2f, Quaternion.identity) as GameObject).SendMessage("SetText", arrayList);
				}
				else
				{
					Kube.BCS.NO.ChangeFlagState(this.flagState.team, FlagState.captured, component.onlineId);
				}
			}
		}
	}

	public GameObject flag;

	public Transform flagBase;

	public FlagStateStruct flagState;

	public int team;

	public GameObject flagTouchGO;

	public float dropTimeToReturn;
}
