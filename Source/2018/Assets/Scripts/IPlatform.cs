using System;
using kube.data;
using LitJson;
using UnityEngine;

public interface IPlatform
{
	platformType platform { get; }

	socialNetType sn { get; }

	string playerUID { get; }

	Texture moneyIconTx { get; }

	T Get<T>() where T : class;

	bool hasMoneyIcon { get; }

	string moneyName { get; }

	float moneyValue { get; }

	string refSite { get; }

	int sex { get; }

	int age { get; }

	string locale { get; }

	string sessionKey { get; }

	string secret { get; }

	bool isVIPPlatform { get; }

	void Init(GameObject go, string func);

	void gotoViralTask(int i, int task);

	bool isViralTaskDone(int i, int task);

	void EventDone(int i, VoidCallback onTakeBonus);

	void PostProcessLocale();

	ViralEvent getViralEvent(int i);

	bool isViralEventDone(int i, IntCallback cb = null);

	bool isQuestDone();

	void QuestDone();

	bool isMissionDone(int i);

	SnMissionDesc[] getMissions();

	void OnMissionDone(string data);

	SocialQuest socialQuest { get; }

	void missionsFromServer(JsonData data);

	void gotoMission(int par1);

	void InviteFrends();

	void ShowPayment(int id, GameObject go, string func);

	void OnUserInfo(string data);

	void OnOrderSuccess();

	void OnOrderSuccess2(string payment_id);

	void gotoUserByUID(string uid);

	void FillFriendsRating(GameObject go, string method);

	void TakeScreenshot();

	void TakeScreenshot(GameObject go, string method);

	void APICallback(string data);

	void PostLevelUpOnWall(int levelNum);

	void PostWeaponOnWall(int weaponNum);

	void PostItemOnWall(int itemNum);

	void PostMissionOnWall(int missionNum);

	void PostBonusOnWall();

	void PostMapSlot(int playerNumMaps);

	void openRangTable();

	bool checkGroupLink(string home);

	void openURL(string url);

	void BuyPack(PackInfo info);

	string MoneyNameForPack(PackInfo info);

	bool canPostOnWall { get; }

	FriendRecord playerInfo { get; }

	bool hasFreeGold { get; }

	void PostWeaponSkinOnWall(int skinId);

	void Logout();

	void ShowGift(string uid);

	void InitPayments();

	void ShowFreeGold();
}
