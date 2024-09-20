using System;
using System.Collections.Generic;
using kube;
using kube.data;
using LitJson;
using UnityEngine;

public class EndMissionDialog : MonoBehaviour
{
	private void Start()
	{
	}

	public void Open(int missionId, bool win, EndGameStats endGameStats)
	{
		this.endGameExp = endGameStats.deltaExp;
		if (this.endGameExp <= 0)
		{
			this.endGameExp = 1;
		}
		this._missionId = missionId;
		this.xp.text = this.endGameExp.ToString();
		this.endGameMoney = Math.Max(0, endGameStats.deltaMoney);
		if (win)
		{
			this.endGameCapture = Localize.mission_done;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["id"] = Kube.GPS.playerId.ToString();
			dictionary["score"] = this.endGameExp.ToString();
			dictionary["mission"] = this._missionId.ToString();
			dictionary["frags"] = endGameStats.deltaFrags.ToString();
			dictionary["money"] = endGameStats.deltaMoney.ToString();
			dictionary["l"] = endGameStats.newLevel.ToString();
			dictionary["b"] = this.int_join(';', endGameStats.bonuses);
			UnityEngine.Debug.Log("BONUS: " + dictionary["b"]);
			Kube.SS.Request(667, dictionary, new ServerScript.ServerCallback(this.onMissionEnd));
		}
		else
		{
			this.endGameCapture = Localize.game_fail;
			this._missionResult = new MissionResult();
		}
		base.gameObject.SetActive(true);
	}

	private string int_join(char separator, int[] arr)
	{
		string text = string.Empty;
		for (int i = 0; i < arr.Length; i++)
		{
			if (i > 0)
			{
				text += ";";
			}
			text += arr[i].ToString();
		}
		return text;
	}

	private void onMissionEnd(string response)
	{
		JsonData jsonData = JsonMapper.ToObject(response);
		this._missionResult = new MissionResult();
		this._missionResult.firstTime = (bool)jsonData["firsttime"];
		this._missionResult.endGameMoney = int.Parse(jsonData["money"].ToString());
		this._missionResult.endGameGold = int.Parse(jsonData["gold"].ToString());
		Kube.GPS.playerExp = int.Parse(jsonData["exp"].ToString());
		Kube.GPS.playerFrags = int.Parse(jsonData["frags"].ToString());
		Kube.GPS.playerLevel = int.Parse(jsonData["level"].ToString());
		this.endGameMoney += this._missionResult.endGameMoney;
		if (this._missionResult.endGameGold > 0)
		{
			Kube.GPS.playerMoney2 += this._missionResult.endGameGold;
		}
		if (this._missionResult.endGameMoney > 0)
		{
			Kube.GPS.playerMoney1 += this._missionResult.endGameMoney;
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
					Kube.GPS.inventarItems[key.id] += keyValuePair.Value;
				}
				else if ((float)Kube.GPS.inventarWeapons[key.id] > Time.time)
				{
					Kube.GPS.inventarWeapons[key.id] += keyValuePair.Value * 86400;
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
				GameObject gameObject = NGUITools.AddChild(this.container.gameObject, this.itemPrefab);
				if (key.type == 0)
				{
					Texture texture = Kube.ASS2.gameItemsTex[key.id];
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
		PhotonNetwork.LeaveRoom();
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
	}

	protected const int MAX_ITEMS_IN_GUIROW = 4;

	public UILabel money1;

	public UILabel money2;

	public UILabel xp;

	public GameObject container;

	public GameObject itemPrefab;

	private int _missionId;

	private string endGameCapture;

	private int endGameExp;

	private int endGameMoney;

	protected MissionResult _missionResult;
}
