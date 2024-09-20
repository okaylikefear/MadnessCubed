using System;
using System.Collections.Generic;
using kube;
using LitJson;
using UnityEngine;

public class ServerScript : KubeAPI, IBaseServer
{
	private void Awake()
	{
		Kube.SS = this;
	}

	virtual void Init(string phpServer, string mainPhpScript)
	{
		base.Init(phpServer, mainPhpScript);
	}

	virtual string[] DecodePlayerData(JsonData playerData)
	{
		return base.DecodePlayerData(playerData);
	}

	virtual void SaveMap(long mapId, byte[] mapData, GameObject go, string method)
	{
		base.SaveMap(mapId, mapData, go, method);
	}

	virtual void LoadMap(long mapId)
	{
		base.LoadMap(mapId);
	}

	virtual void LoadPlayersParams(YieldCallback cb)
	{
		base.LoadPlayersParams(cb);
	}

	virtual void BuyCubes(int numCubes, int numDays, GameObject go, string method)
	{
		base.BuyCubes(numCubes, numDays, go, method);
	}

	virtual void BuyItem(int numItem, int itemsCount, GameObject go, string method)
	{
		base.BuyItem(numItem, itemsCount, go, method);
	}

	virtual void BuyWeapon(int numWeapon, int tarif, GameObject go, string method)
	{
		base.BuyWeapon(numWeapon, tarif, go, method);
	}

	virtual void BuySpecItem(int numSpecItem, int tarif, GameObject go, string method)
	{
		base.BuySpecItem(numSpecItem, tarif, go, method);
	}

	virtual void GetPlayerMoney(GameObject go, string method)
	{
		base.GetPlayerMoney(go, method);
	}

	virtual void UpgradeParam(int numParam, GameObject go, string method)
	{
		base.UpgradeParam(numParam, go, method);
	}

	virtual void UpgradeParamUnlock(int numParam, GameObject go, string method)
	{
		base.UpgradeParamUnlock(numParam, go, method);
	}

	virtual void UpgradeParamAllUnlock(int needHealth, int needArmor, int needSpeed, int needJump, int needDefend, int upgradeMoney, GameObject go, string method)
	{
		base.UpgradeParamAllUnlock(needHealth, needArmor, needSpeed, needJump, needDefend, upgradeMoney, go, method);
	}

	virtual void BuySkin(int numSkin, GameObject go, string method)
	{
		base.BuySkin(numSkin, go, method);
	}

	virtual void GoldToMoney(int numGold, GameObject go, string method)
	{
		base.GoldToMoney(numGold, go, method);
	}

	virtual void SaveNewName(int id, string newName)
	{
		base.SaveNewName(id, newName);
	}

	virtual void BuyBullets(int typeBullets, int numTarif, GameObject go, string method)
	{
		base.BuyBullets(typeBullets, numTarif, go, method);
	}

	virtual void SendEndLevel(EndGameStats endGameStats, GameObject go, string method)
	{
		base.SendEndLevel(endGameStats, go, method);
	}

	virtual int UnixTime()
	{
		return base.UnixTime();
	}

	virtual void BuyNewMap(int maptype, ServerCallback cb)
	{
		base.BuyNewMap(maptype, cb);
	}

	virtual void UseItem(int numItem)
	{
		base.UseItem(numItem);
	}

	virtual void TakeItem(int numItem, int itemCountNow, GameObject go, string method)
	{
		base.TakeItem(numItem, itemCountNow, go, method);
	}

	virtual void Request(int q, object param, ServerCallback cb)
	{
		base.Request(q, param, cb);
	}

	virtual void Request(int q, Dictionary<string, string> paramData, ServerCallback cb)
	{
		base.Request(q, paramData, cb);
	}

	virtual void LoadIsMap(long mapId, GameObject go, string method)
	{
		base.LoadIsMap(mapId, go, method);
	}

	virtual void SetMapName(long mapId, string mapName)
	{
		base.SetMapName(mapId, mapName);
	}

	virtual void SendStat(string statName)
	{
		base.SendStat(statName);
	}

	virtual void SendStatCount(string statName, int count)
	{
		base.SendStatCount(statName, count);
	}

	virtual void BuyVIP(int numVIP, GameObject go, string method)
	{
		base.BuyVIP(numVIP, go, method);
	}

	virtual void RegenerateMap(int maptype, long numMap, ServerCallback cb)
	{
		base.RegenerateMap(maptype, numMap, cb);
	}

	virtual void SetSkin(int numSkin)
	{
		base.SetSkin(numSkin);
	}

	virtual void SetClothes(string clothes)
	{
		base.SetClothes(clothes);
	}

	virtual void BuyClothes(int numClothes, GameObject go, string method)
	{
		base.BuyClothes(numClothes, go, method);
	}

	virtual void SaveFastInventory(int type, FastInventar[] inventory, ServerCallback cb)
	{
		base.SaveFastInventory(type, inventory, cb);
	}

	virtual void LoadStatistics(int dayFrom, int dayTo, GameObject go, string method)
	{
		base.LoadStatistics(dayFrom, dayTo, go, method);
	}

	virtual void UpgradeWeapon(int bt, int q, JSONServerCallback upgradeWeaponDone)
	{
		base.UpgradeWeapon(bt, q, upgradeWeaponDone);
	}

	virtual void SendStatIoTrack(string statName, int inc)
	{
		base.SendStatIoTrack(statName, inc);
	}

	virtual void LoadMissions(JSONServerCallback missionLoadDone)
	{
		base.LoadMissions(missionLoadDone);
	}

	virtual void EndMission(int missionId, EndGameStats endGameStats, ServerCallback onMissionEnd)
	{
		base.EndMission(missionId, endGameStats, onMissionEnd);
	}

	virtual void BuyWeaponSkin(int weaponId, int index, GameObject gameObject, string p)
	{
		base.BuyWeaponSkin(weaponId, index, gameObject, p);
	}

	virtual void UseWeaponSkin(int weaponId, int index, GameObject gameObject, string p)
	{
		base.UseWeaponSkin(weaponId, index, gameObject, p);
	}

	virtual bool get_savingMap()
	{
		return base.savingMap;
	}

	virtual bool get_loadingMap()
	{
		return base.loadingMap;
	}

	virtual float get_serverTime()
	{
		return base.serverTime;
	}

	virtual void set_serverTime(float value)
	{
		base.serverTime = value;
	}

	virtual int get_serverId()
	{
		return base.serverId;
	}

	virtual string get_phpSecret()
	{
		return base.phpSecret;
	}
}
