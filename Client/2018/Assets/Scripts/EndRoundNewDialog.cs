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
		int deltaKills = endGameStats.deltaKills;
		this.time = endGameTime;
		this.exp = endGameStats.deltaExp;
		this.money = endGameStats.deltaMoney;
		this.fragLabel.text = deltaKills.ToString();
		this.timeLabel.text = this.time.ToString() + Localize.sec;
		this.moneyLabel.text = this.money.ToString();
		this.TitleLable.text = endGameTitle;
		this.startExp = Kube.GPS.playerExp;
		int playerLevel = endGameStats.playerLevel;
		this.expLabel.text = string.Concat(new object[]
		{
			Localize.BCS_exp,
			":",
			Kube.OH.GetExpFromLevelUp(this.startExp, -1),
			"/",
			Kube.OH.GetExpToLevelUp(playerLevel)
		});
		this.expSlider.value = Kube.OH.GetExpToLevelUpAlpha(this.startExp, -1);
		int num = endGameStats.playerLevel;
		if (num >= Localize.RankName.Length)
		{
			num = Localize.RankName.Length - 1;
		}
		this.rankTexture.mainTexture = Kube.ASS2.RankTex[num].mainTexture;
		this.rankName.text = string.Concat(new object[]
		{
			string.Empty,
			playerLevel,
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
			GameObject gameObject = this.bonusesContainer.gameObject.AddChild(this.bonusItemPrefab);
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
			newLevelDialog.goldGive = this.stats.newLevel - this.stats.playerLevel;
			newLevelDialog.onContinue = new EventDelegate(new EventDelegate.Callback(this._exitDialog));
		}
		else
		{
			this._exitDialog();
		}
	}

	public void _exitDialog()
	{
		PhotonNetwork.LeaveRoom();
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
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
			if (num2 >= 1f)
			{
				num2 = 1f;
			}
			float f = (float)this.exp * num2;
			long num3 = (long)((ulong)this.startExp + (ulong)((long)this.exp));
			if (num3 <= (long)((ulong)-1))
			{
				this.curExp = (uint)num3;
			}
			else
			{
				this.curExp = this.startExp;
			}
			this.curLevel = Kube.OH.GetLevel(this.curExp);
			int num4 = this.curLevel;
			if (num4 >= Localize.RankName.Length)
			{
				num4 = Localize.RankName.Length - 1;
			}
			this.expSlider.value = Kube.OH.GetExpToLevelUpAlpha(this.curExp, -1);
			this.rankTexture.mainTexture = Kube.ASS2.RankTex[num4].mainTexture;
			this.rankName.text = string.Concat(new object[]
			{
				string.Empty,
				this.curLevel,
				". ",
				Localize.RankName[num4]
			});
			this.expLabel.text = string.Concat(new object[]
			{
				Localize.BCS_exp,
				": ",
				Mathf.RoundToInt(f),
				"  (",
				Kube.OH.GetExpFromLevelUp(this.curExp, this.curLevel),
				"/",
				Kube.OH.GetExpToLevelUp(this.curLevel),
				")"
			});
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

	private uint startExp;

	private EndGameStats stats;

	private uint curExp;

	private int curLevel;
}
