using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public interface IBaseServer
{
	bool savingMap { get; }

	bool loadingMap { get; }

	float serverTime { get; set; }

	int serverId { get; }

	string phpSecret { get; }

	void Init(string phpServer, string mainPhpScript);

	string[] DecodePlayerData(JsonData playerData);

	void SaveMap(long mapId, byte[] mapData, GameObject go = null, string method = "");

	void LoadMap(long mapId);

	void LoadPlayersParams(GameObject go, string funcName);

	void BuyCubes(int numCubes, int numDays, GameObject go, string method);

	void BuyItem(int numItem, int itemsCount, GameObject go, string method);

	void BuyWeapon(int numWeapon, int tarif, GameObject go, string method);

	void BuySpecItem(int numSpecItem, int tarif, GameObject go, string method);

	void GetPlayerMoney(GameObject go, string method);

	void UpgradeParam(int numParam, GameObject go, string method);

	void UpgradeParamUnlock(int numParam, GameObject go, string method);

	void UpgradeParamAllUnlock(int needHealth, int needArmor, int needSpeed, int needJump, int needDefend, int upgradeMoney, GameObject go, string method);

	void BuySkin(int numSkin, GameObject go, string method);

	void GoldToMoney(int numGold, GameObject go, string method);

	void SaveNewName(int id, string newName);

	void BuyBullets(int typeBullets, int numTarif, GameObject go, string method);

	void SendEndLevel(EndGameStats endGameStats, GameObject go, string method);

	int UnixTime();

	void BuyNewMap(int maptype, ServerCallback cb);

	void UseItem(int numItem);

	void TakeItem(int numItem, int itemCountNow, GameObject go, string method);

	void Request(int q, object param, ServerCallback cb);

	void Request(int q, Dictionary<string, string> paramData, ServerCallback cb);

	void LoadIsMap(long mapId, GameObject go, string method);

	void SetMapName(long mapId, string mapName);

	void SendStat(string statName);

	void SendStatCount(string statName, int count);

	void BuyVIP(int numVIP, GameObject go, string method);

	void RegenerateMap(int maptype, long numMap, ServerCallback cb);

	void SetSkin(int numSkin);

	void SetClothes(string clothes);

	void BuyClothes(int numClothes, GameObject go, string method);

	void SaveFastInventory(int type, FastInventar[] inventory, ServerCallback cb);

	void LoadStatistics(int dayFrom, int dayTo, GameObject go, string method);

	void UpgradeWeapon(int bt, int q, JSONServerCallback upgradeWeaponDone);

	void SendStatIoTrack(string statName, int inc = 1);

	void LoadMissions(JSONServerCallback missionLoadDone);

	void EndMission(int missionId, EndGameStats endGameStats, ServerCallback onMissionEnd);

	void BuyWeaponSkin(int weaponId, int index, GameObject gameObject, string p);

	void UseWeaponSkin(int weaponId, int index, GameObject gameObject, string p);
}
