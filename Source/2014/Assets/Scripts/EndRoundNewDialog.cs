using System;
using kube;
using UnityEngine;

public class EndRoundNewDialog : MonoBehaviour
{
	private string RankName(int id)
	{
		if (id >= Localize.RankName.Length)
		{
			id = Localize.RankName.Length - 1;
		}
		return Localize.RankName[id];
	}

	public void Open(EndGameStats endGameStats, int endGameTime, string endGameTitle)
	{
		this.stats = endGameStats;
		this.frags = endGameStats.deltaFrags;
		this.time = endGameTime;
		this.exp = endGameStats.deltaExp;
		this.money = endGameStats.deltaMoney;
		this.fragLabel.text = this.frags.ToString();
		this.timeLabel.text = this.time.ToString() + Localize.sec;
		this.moneyLabel.text = this.money.ToString();
		this.TitleLable.text = endGameTitle;
		this.startExp = Kube.GPS.playerExp;
		this.expLabel.text = string.Concat(new object[]
		{
			Localize.BCS_exp,
			":",
			Kube.OH.GetExpFromLevelUp(this.startExp),
			"/",
			Kube.OH.GetExpToLevelUp(this.startExp)
		});
		this.expSlider.value = Kube.OH.GetExpToLevelUpAlpha(this.startExp);
		int num = Kube.OH.GetLevel(this.startExp);
		if (num >= Localize.RankName.Length)
		{
			num = Localize.RankName.Length - 1;
		}
		this.rankTexture.mainTexture = Kube.ASS2.RankTex[num].mainTexture;
		this.rankName.text = string.Concat(new object[]
		{
			string.Empty,
			Kube.OH.GetLevel(this.startExp),
			". ",
			Localize.RankName[num]
		});
		this.startTime = Time.realtimeSinceStartup;
		base.gameObject.SetActive(true);
	}

	private void OnEnable()
	{
		KGUITools.removeAllChildren(this.bonusesContainer.gameObject, true);
		for (int i = 0; i < Kube.BCS.sumBonusesTex.Count; i++)
		{
			GameObject gameObject = NGUITools.AddChild(this.bonusesContainer.gameObject, this.bonusItemPrefab);
			BonusItem component = gameObject.GetComponent<BonusItem>();
			component.tx.mainTexture = Kube.BCS.sumBonusesTex[i];
			component.label.text = Kube.BCS.sumBonusesStr[i];
		}
		this.bonusesContainer.GetComponent<PagePanel>().Reposition();
	}

	private void OnDisable()
	{
	}

	public void exitDialog()
	{
		base.gameObject.SetActive(false);
		if (this.stats.playerLevel < this.stats.newLevel)
		{
			NewLevelDialog newLevelDialog = Cub2UI.FindAndOpenMenu<NewLevelDialog>("dialog_levelup");
			newLevelDialog.newlevel = this.stats.newLevel;
			newLevelDialog.onContinue = new EventDelegate(new EventDelegate.Callback(this._exitDialog));
		}
		else
		{
			this._exitDialog();
		}
	}

	public void _exitDialog()
	{
		if (Kube.BCS.gameType == GameType.mission)
		{
			this.finalUI.Open(Kube.BCS._missionId, Kube.BCS.lastEndGameType == BattleControllerScript.EndGameType.exitTrigger, this.stats);
		}
		else if (Kube.BCS.gameType == GameType.teams || Kube.BCS.gameType == GameType.dominating || Kube.BCS.gameType == GameType.captureTheFlag || Kube.BCS.gameType == GameType.shooter)
		{
			Cub2UI.currentMenu = Kube.BCS.endRound.gameObject;
		}
		else
		{
			PhotonNetwork.LeaveRoom();
			UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		float num = Time.realtimeSinceStartup - this.startTime;
		if (num > 1f)
		{
			float num2 = (num - 1f) / this.showFullTime;
			if (num2 >= 0f && num2 <= 1f)
			{
				float num3 = Mathf.Lerp((float)this.startExp, (float)(this.startExp + this.exp), num2);
				float f = Mathf.Lerp(0f, (float)this.exp, num2);
				int num4 = Kube.OH.GetLevel((int)num3);
				if (num4 >= Localize.RankName.Length)
				{
					num4 = Localize.RankName.Length - 1;
				}
				this.expSlider.value = Kube.OH.GetExpToLevelUpAlpha((int)num3);
				this.rankTexture.mainTexture = Kube.ASS2.RankTex[num4].mainTexture;
				this.rankName.text = string.Concat(new object[]
				{
					string.Empty,
					Kube.OH.GetLevel((int)num3),
					". ",
					Localize.RankName[num4]
				});
				this.expLabel.text = string.Concat(new object[]
				{
					Localize.BCS_exp,
					": ",
					Mathf.RoundToInt(f),
					"  (",
					Kube.OH.GetExpFromLevelUp((int)num3),
					"/",
					Kube.OH.GetExpToLevelUp((int)num3),
					")"
				});
			}
		}
	}

	public UISlider expSlider;

	public UILabel expLabel;

	public UITexture rankTexture;

	public UILabel rankName;

	public UILabel fragLabel;

	public UILabel timeLabel;

	public UILabel moneyLabel;

	public UIPanel bonusesContainer;

	public GameObject bonusItemPrefab;

	public UILabel TitleLable;

	public EndMissionDialog finalUI;

	private int frags;

	private int time;

	private int exp;

	private int money;

	private float startTime;

	public float showFullTime;

	private int startExp;

	private EndGameStats stats;
}
