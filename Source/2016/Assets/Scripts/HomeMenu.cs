using System;
using kube;
using kube.data;
using UnityEngine;

public class HomeMenu : MonoBehaviour
{
	private void onMissions()
	{
		MissionDesc[] array = MissionBox.selectMissions(100);
		bool flag = false;
		if (array.Length > 0)
		{
			flag = true;
			this.secretMissionBtn.gameObject.SetActive(true);
			this.secretMissionBtn.GetComponentInChildren<UILabel>().text = array[0].title;
		}
		this.quickMathBtn.gameObject.SetActive(!flag);
	}

	private void Start()
	{
		if (Kube.GPS == null)
		{
			return;
		}
		Kube.RM.require("Assets2", null);
		this.version.text = "A.5.2 O62 R" + Kube.OH.build;
		if (!this.viral_dialog)
		{
			this.viral_dialog = Cub2Menu.Find<ViralDialog>("dialog_viral");
		}
		this.UpgradeParamRecountBonuces();
		this.nickname.text = Kube.GPS.decodePlayerName;
		this.nickname.label.text = Kube.GPS.decodePlayerName;
		int level = Kube.OH.GetLevel(Kube.GPS.playerExp);
		int num = Mathf.Min(level, Localize.RankName.Length - 1);
		this.rank.label.text = Localize.RankName[num];
		this.rank.labelLevel.text = string.Concat(new object[]
		{
			"(",
			Localize.player_level,
			" ",
			level,
			")"
		});
		if (Kube.ASS2)
		{
			this.rank.tx.mainTexture = Kube.ASS2.RankTex[num].mainTexture;
		}
		int expFromLevelUp = Kube.OH.GetExpFromLevelUp(Kube.GPS.playerExp, level);
		int expToLevelUp = Kube.OH.GetExpToLevelUp(level);
		this.rank.progressLabel.text = expFromLevelUp.ToString() + "/" + expToLevelUp.ToString();
		this.rank.progress.value = (float)expFromLevelUp / (float)expToLevelUp;
		KGUITools.removeAllChildren(this.offers.gameObject, true);
		if (!Kube.SN.isQuestDone())
		{
			this.viral.gameObject.SetActive(true);
			this.viral.transform.parent = this.offers.gameObject.transform;
		}
		Offer[] array = OfferBox.list();
		for (int i = 0; i < array.Length; i++)
		{
			int num2 = array[i].type - 1;
			GameObject gameObject = this.offers.gameObject.AddChild(this.offerPrefab);
			UIOfferItem component = gameObject.GetComponent<UIOfferItem>();
			component.offer = array[i];
		}
		PackInfo[] array2 = PackBox.list();
		int num3 = 0;
		while (num3 < array2.Length && this.offers.transform.childCount < 4)
		{
			GameObject gameObject2 = this.offers.gameObject.AddChild(this.packPrefab);
			UIPackItem component2 = gameObject2.GetComponent<UIPackItem>();
			component2.info = array2[num3];
			num3++;
		}
		this.offers.Reposition();
		MissionBox.request(new VoidCallback(this.onMissions), false);
	}

	public void UpgradeParamRecountBonuces()
	{
		int num = 0;
		int num2 = 0;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		num += (int)Kube.GPS.skinBonus[Kube.GPS.playerSkin, 0];
		num2 += (int)Kube.GPS.skinBonus[Kube.GPS.playerSkin, 1];
		num3 += Kube.GPS.skinBonus[Kube.GPS.playerSkin, 2];
		num4 += Kube.GPS.skinBonus[Kube.GPS.playerSkin, 3];
		num5 += Kube.GPS.skinBonus[Kube.GPS.playerSkin, 4] * 0.01f;
		for (int i = 0; i < Kube.GPS.playerClothes.Length; i++)
		{
			if (Kube.GPS.playerClothes[i] >= 0)
			{
				num += (int)Kube.GPS.clothesBonus[Kube.GPS.playerClothes[i], 0];
				num2 += (int)Kube.GPS.clothesBonus[Kube.GPS.playerClothes[i], 1];
				num3 += Kube.GPS.clothesBonus[Kube.GPS.playerClothes[i], 2];
				num4 += Kube.GPS.clothesBonus[Kube.GPS.playerClothes[i], 3];
				num5 += Kube.GPS.clothesBonus[Kube.GPS.playerClothes[i], 4] * 0.01f;
			}
		}
		int num6 = 0;
		if (Kube.GPS.vipEnd - Time.time > 0f)
		{
			num6 += 2;
		}
		int num7 = (int)Kube.GPS.charParamsPrice[0, Mathf.Min(Kube.GPS.playerHealth + num6, 7), 4];
		int num8 = (int)Kube.GPS.charParamsPrice[1, Mathf.Min(Kube.GPS.playerArmor + num6, 7), 4];
		float num9 = (float)((int)Kube.GPS.charParamsPrice[2, Mathf.Min(Kube.GPS.playerSpeed + num6, 7), 4]);
		float num10 = (float)((int)Kube.GPS.charParamsPrice[3, Mathf.Min(Kube.GPS.playerJump + num6, 7), 4]);
		float num11 = Kube.GPS.charParamsPrice[4, Mathf.Min(Kube.GPS.playerDefend + num6, 7), 4] * 0.01f;
		object[] array = new object[]
		{
			(float)num7,
			(float)num8,
			num9,
			num10,
			num11 * 100f
		};
		object[] array2 = new object[]
		{
			(float)num,
			(float)num2,
			num3,
			num4,
			num5 * 100f
		};
		string[] array3 = new string[]
		{
			Localize.params_health,
			Localize.params_armor,
			Localize.param_speed,
			Localize.param_jump,
			Localize.param_defend
		};
		float[] array4 = new float[]
		{
			300f,
			300f,
			10f,
			10f,
			100f
		};
		for (int j = 0; j < this.playerProgress.Length; j++)
		{
			this.playerProgress[j].value.text = string.Concat(new object[]
			{
				string.Empty,
				(int)((float)array[j]),
				"(+",
				(int)((float)array2[j]),
				")"
			});
			this.playerProgress[j].title.text = array3[j];
			this.playerProgress[j].slider.value = ((float)array[j] + (float)array2[j]) / array4[j];
		}
	}

	public void onAssetsLoaded(int id)
	{
		if (Kube.ASS2 == null)
		{
			return;
		}
		int num = Mathf.Min(Kube.GPS.playerLevel, Kube.ASS2.RankTex.Length - 1);
		this.rank.label.text = Localize.RankName[num];
		this.rank.tx.mainTexture = Kube.ASS2.RankTex[num].mainTexture;
	}

	private void Update()
	{
		if (UIInput.selection != this.nickname && this.nickname.value != Kube.GPS.decodePlayerName)
		{
			this.onNicknameSubmit();
		}
	}

	public void OnUpgradePlayerParam(PlayerProgress pp)
	{
		int num = Array.IndexOf<PlayerProgress>(this.playerProgress, pp);
		if (num == -1)
		{
			return;
		}
		if (!CharRang.needUnlock(num))
		{
			this.upgrade_dialog.GetComponent<UpgradePlayerDialog>().Show(num);
			return;
		}
		UnlockDialog unlockDialog = Cub2UI.FindAndOpenDialog<UnlockDialog>("dialog_unlock");
		unlockDialog.itemCode = CharRang.itemCode(num);
		unlockDialog.needLevel = CharRang.needLevel(num);
		unlockDialog.Show();
	}

	public void onSecretPlay()
	{
		MissionDesc[] array = MissionBox.selectMissions(100);
		OnlineManager.instance.PlayMission(array[0], array[0].offline);
	}

	public void onQuickPlay()
	{
		OnlineManager.instance.QuickPlay();
	}

	public void onViral()
	{
		this.viral_dialog.gameObject.SetActive(true);
	}

	public void onViralDeadzone()
	{
		Cub2UI.FindAndOpenDialog("dialog_ANDROID");
	}

	public void onNicknameSubmit()
	{
		string value = this.nickname.value;
		Kube.GPS.playerName = AuxFunc.CodeRussianName(value);
		if (Kube.GPS.decodePlayerName.Length >= 3)
		{
			Kube.SS.SaveNewName(Kube.SS.serverId, Kube.GPS.playerName);
		}
	}

	internal void ShowOffer(Offer offer)
	{
		GameObject gameObject = this.offer_dialog[offer.type - 1];
		OfferDialog component = gameObject.GetComponent<OfferDialog>();
		component.offer = offer;
		gameObject.SetActive(true);
	}

	public void ShowPack(PackInfo info)
	{
		PackDialog packDialog = Cub2UI.FindDialog<PackDialog>("dialog_pack");
		packDialog.info = info;
		packDialog.gameObject.SetActive(true);
	}

	private void NotifyUpdate()
	{
		foreach (object obj in this.offers.transform)
		{
			Transform transform = (Transform)obj;
			UIPackItem component = transform.GetComponent<UIPackItem>();
			if (component != null)
			{
				component.Validate();
			}
		}
		this.offers.Reposition();
	}

	public PlayerProgress[] playerProgress;

	public ViralDialog viral_dialog;

	public UIButton viral;

	public GameObject homeBtn;

	public UIInput nickname;

	public Rank rank;

	public UIGrid offers;

	public GameObject offerPrefab;

	public GameObject packPrefab;

	public GameObject taskPrefab;

	public GameObject[] offer_dialog;

	public GameObject upgrade_dialog;

	public UIButton quickMathBtn;

	public UIButton secretMissionBtn;

	public UILabel version;
}
