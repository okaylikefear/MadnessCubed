using System;
using kube;
using kube.data;
using UnityEngine;

public class HomeMenu : MonoBehaviour
{
	private void Start()
	{
		if (Kube.GPS == null)
		{
			return;
		}
		Kube.SS.require("Assets2");
		this.version.text = "2.7.5 O17 R" + Kube.OH.build;
		if (!this.viral_dialog)
		{
			this.viral_dialog = Cub2Menu.Find<ViralDialog>("dialog_viral");
		}
		this.UpgradeParamRecountBonuces();
		if (!Kube.SN.isQuestDone())
		{
			this.viral.gameObject.SetActive(true);
		}
		this.nickname.text = Kube.GPS.decodePlayerName;
		int num = Mathf.Min(Kube.GPS.playerLevel, Localize.RankName.Length - 1);
		this.rank.label.text = Localize.RankName[num];
		this.rank.labelLevel.text = string.Concat(new object[]
		{
			"(",
			Localize.needParamsToBuyLevel,
			" ",
			num,
			")"
		});
		if (Kube.ASS2)
		{
			this.rank.tx.mainTexture = Kube.ASS2.RankTex[num].mainTexture;
		}
		int expFromLevelUp = Kube.OH.GetExpFromLevelUp(Kube.GPS.playerExp);
		int expToLevelUp = Kube.OH.GetExpToLevelUp(Kube.GPS.playerExp);
		this.rank.progressLabel.text = expFromLevelUp.ToString() + "/" + expToLevelUp.ToString();
		this.rank.progress.value = (float)expFromLevelUp / (float)expToLevelUp;
		KGUITools.removeAllChildren(this.offers.gameObject, true);
		Offer[] array = OfferBox.list();
		for (int i = 0; i < array.Length; i++)
		{
			int num2 = array[i].type - 1;
			GameObject gameObject = NGUITools.AddChild(this.offers.gameObject, this.offerPrefab);
			UIOfferItem component = gameObject.GetComponent<UIOfferItem>();
			component.offer = array[i];
		}
		this.offers.Reposition();
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
		this.upgrade_dialog.GetComponent<UpgradePlayerDialog>().Show(num);
	}

	public void onQuickPlay()
	{
		OnlineManager.instance.QuickPlay();
	}

	public void onViral()
	{
		this.viral_dialog.gameObject.SetActive(true);
	}

	public void onNicknameSubmit()
	{
		string value = this.nickname.value;
		Kube.GPS.playerName = AuxFunc.CodeRussianName(value);
		if (Kube.GPS.decodePlayerName.Length >= 3)
		{
			Kube.SS.SaveNewName(Kube.GPS.playerId, Kube.GPS.playerName);
		}
	}

	internal void ShowOffer(Offer offer)
	{
		GameObject gameObject = this.offer_dialog[offer.type - 1];
		OfferDialog component = gameObject.GetComponent<OfferDialog>();
		component.offer = offer;
		gameObject.SetActive(true);
	}

	public PlayerProgress[] playerProgress;

	public ViralDialog viral_dialog;

	public UIButton viral;

	public GameObject homeBtn;

	public UIInput nickname;

	public Rank rank;

	public UIGrid offers;

	public GameObject offerPrefab;

	public GameObject[] offer_dialog;

	public GameObject upgrade_dialog;

	public UILabel version;
}
