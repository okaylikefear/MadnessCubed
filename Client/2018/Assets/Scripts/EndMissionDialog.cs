using System;
using System.Collections.Generic;
using kube;
using kube.cheat;
using kube.data;
using LitJson;
using UnityEngine;

public class EndMissionDialog : MonoBehaviour
{
	private void Start()
	{
		if (Kube.SN.canPostOnWall)
		{
			this.bigButtonLabel.text = Localize.postonwall;
		}
		else
		{
			this.bigButtonLabel.text = Localize.dialog_next;
		}
		this.bigButtonLabel.GetComponentInParent<UIButton>().isEnabled = false;
	}

	public void Open(int missionId, bool win, EndGameStats endGameStats)
	{
		this.endGameExp = endGameStats.deltaExp;
		this._endGameStats = endGameStats;
		this._missionId = missionId;
		this.endGameMoney = Math.Max(0, endGameStats.deltaMoney);
		if (win)
		{
			this.endGameCapture = Localize.mission_done;
			Kube.SS.EndMission(this._missionId, endGameStats, new ServerCallback(this.onMissionEnd));
		}
		else
		{
			this.endGameCapture = Localize.game_fail;
			this._missionResult = new MissionResult();
		}
		base.gameObject.SetActive(true);
	}

	private void onMissionEnd(string response)
	{
		JsonData jsonData = JsonMapper.ToObject(response);
		this._missionResult = new MissionResult();
		this._missionResult.firstTime = (bool)jsonData["firsttime"];
		this._missionResult.endGameMoney = int.Parse(jsonData["money"].ToString());
		this._missionResult.endGameGold = int.Parse(jsonData["gold"].ToString());
		Kube.GPS.playerExp = uint.Parse(jsonData["exp"].ToString());
		Kube.GPS.playerFrags = int.Parse(jsonData["frags"].ToString());
		Kube.GPS.playerLevel = int.Parse(jsonData["level"].ToString());
		this.endGameMoney += this._missionResult.endGameMoney;
		if (this._missionResult.endGameGold > 0)
		{
			GameParamsScript gps = Kube.GPS;
			gps.playerMoney2 += this._missionResult.endGameGold;
		}
		if (this._missionResult.endGameMoney > 0)
		{
			GameParamsScript gps2 = Kube.GPS;
			gps2.playerMoney1 += this._missionResult.endGameMoney;
		}
		if (this._missionResult.firstTime && jsonData["bonus"] != null)
		{
			string par = jsonData["bonus"].ToString();
			this._missionResult.items = MissionHelper.parseBonus(par);
			foreach (KeyValuePair<BonusDesc, int> keyValuePair in this._missionResult.items)
			{
				BonusDesc key = keyValuePair.Key;
				if (key.type == 0)
				{
					GameParamsScript.InventarItems inventarItems2;
					GameParamsScript.InventarItems inventarItems = inventarItems2 = Kube.GPS.inventarItems;
					int num;
					int index = num = key.id;
					num = inventarItems2[num];
					inventarItems[index] = num + keyValuePair.Value;
				}
				else if ((float)Kube.GPS.inventarWeapons[key.id] > Time.time)
				{
					ObscuredIntAB[] inventarWeapons = Kube.GPS.inventarWeapons;
					int id = key.id;
					inventarWeapons[id] += keyValuePair.Value * 86400;
				}
				else
				{
					Kube.GPS.inventarWeapons[key.id] = (int)Time.time + keyValuePair.Value * 86400;
					Kube.IS.putToFastInvetar(1, new FastInventar(4, key.id));
				}
			}
		}
		this.money1.text = this._missionResult.endGameMoney.ToString();
		this.money2.text = this._missionResult.endGameGold.ToString();
		this.money2.transform.parent.gameObject.SetActive(this._missionResult.firstTime);
		this.bigButtonLabel.GetComponentInParent<UIButton>().isEnabled = true;
		this.Invalidate();
	}

	private void Invalidate()
	{
		int num = 0;
		if (this._missionResult.items != null)
		{
			foreach (KeyValuePair<BonusDesc, int> keyValuePair in this._missionResult.items)
			{
				BonusDesc key = keyValuePair.Key;
				GameObject gameObject = this.container.gameObject.AddChild(this.itemPrefab);
				if (key.type == 0)
				{
					Texture texture = Kube.OH.gameItemsTex[key.id];
				}
				else
				{
					Texture texture = Kube.ASS2.inventarWeaponsTex[key.id];
				}
				ItemDescIcon component = gameObject.GetComponent<ItemDescIcon>();
				component.count = keyValuePair.Value;
				component.itemType = key.type;
				component.itemId = key.id;
				num++;
			}
			this.container.GetComponentInChildren<UIGrid>().Reposition();
		}
	}

	private void OnEnable()
	{
		KGUITools.removeAllChildren(this.container.gameObject, true);
	}

	private void Update()
	{
	}

	public void postAndExit()
	{
		MissionDesc missionDesc = MissionBox.FindMissionById(this._missionId);
		Kube.SN.PostMissionOnWall((missionDesc.episode - 1) * 10 + missionDesc.index);
		this.exitDialog();
	}

	public void exitDialog()
	{
		if (this._missionResult == null)
		{
			return;
		}
		Kube.BCS.endRoundScoresUI.Open(this._endGameStats, Kube.BCS.endGameTime, Localize.BCS_endGame_gameOver);
	}

	protected const int MAX_ITEMS_IN_GUIROW = 4;

	public UILabel money1;

	public UILabel money2;

	public UILabel bigButtonLabel;

	public GameObject container;

	public GameObject itemPrefab;

	private int _missionId;

	private string endGameCapture;

	private int endGameExp;

	private int endGameMoney;

	private EndGameStats _endGameStats;

	protected MissionResult _missionResult;
}
