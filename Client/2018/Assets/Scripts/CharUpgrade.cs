using System;
using kube;
using kube.data;
using UnityEngine;

public class CharUpgrade : MonoBehaviour
{
	public void UpdateChar()
	{
		this.UpgradeParamRecountBonuces();
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

	private void Start()
	{
		this.UpgradeParamRecountBonuces();
	}

	private void OnEnable()
	{
		this.UpgradeParamRecountBonuces();
	}

	public void OnUpgradePlayerParam(PlayerProgress pp)
	{
		int num = Array.IndexOf<PlayerProgress>(this.playerProgress, pp);
		if (num == -1)
		{
			return;
		}
		UpgradePlayerDialog upgradePlayerDialog = Cub2UI.FindDialog<UpgradePlayerDialog>("dialog_upgrade");
		if (!CharRang.needUnlock(num))
		{
			upgradePlayerDialog.GetComponent<UpgradePlayerDialog>().Show(num);
			return;
		}
		UnlockDialog unlockDialog = Cub2UI.FindAndOpenDialog<UnlockDialog>("dialog_unlock");
		unlockDialog.itemCode = CharRang.itemCode(num);
		unlockDialog.needLevel = CharRang.needLevel(num);
		unlockDialog.Show();
	}

	public PlayerProgress[] playerProgress;
}
