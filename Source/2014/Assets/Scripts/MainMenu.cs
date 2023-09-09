using System;
using kube;
using kube.data;
using kube.game;
using UnityEngine;

public class MainMenu : Cub2Menu
{
	public static void ShowBank()
	{
		Cub2UI.FindAndOpenDialog("dialog_bank");
	}

	public void Start()
	{
		Screen.lockCursor = false;
		if (Kube.OH != null)
		{
			Kube.OH.EndLoading();
		}
		if (Kube.ASS5 == null)
		{
			Kube.SS.require("Assets5");
		}
		else
		{
			this.ApplyDress();
		}
		MissionBox.invalidate();
		if (Kube.OH.lastTempMap.GameType == GameType.mission)
		{
			base.OpenTab("play_menu");
		}
		Kube.IS.resetInventory();
		if (Kube.GPS.bonusDay != 0)
		{
			int num = Kube.GPS.bonusDay - 1;
			Kube.SS.SendStat("bonusDay" + num);
			Kube.GPS.bonusDay = 0;
			this.daily_dialog.Show(num);
		}
		Kube.SN.FillFriendsRating(Kube.OH.gameObject, "GotFriends");
	}

	public void ApplyDress()
	{
		if (Kube.ASS5 != null)
		{
			this.roomCharacter.SetActive(true);
			this.roomCharacter.SendMessage("DressSkin", string.Concat(new object[]
			{
				string.Empty,
				Kube.GPS.playerSkin,
				";",
				Kube.GPS.skinItems[Kube.GPS.playerSkin],
				";",
				Kube.GPS.playerClothesStr
			}));
			GameUtils.ChangeLayersRecursively(this.roomCharacter.transform, "MenuRoom");
		}
	}

	public void onAssetsLoaded(int id)
	{
		if (Kube.ASS5 != null)
		{
			this.ApplyDress();
		}
	}

	public GameObject roomCharacter;

	public BonusDayDialog daily_dialog;
}
