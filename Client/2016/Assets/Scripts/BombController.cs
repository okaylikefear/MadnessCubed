using System;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class BombController : TeamControllerBase
{
	public override void Initialize()
	{
		if (this.initialized)
		{
			return;
		}
		this.respawnsRed = GameObject.FindGameObjectsWithTag("RespawnRed");
		this.respawnsBlue = GameObject.FindGameObjectsWithTag("RespawnBlue");
		this.bombPlant = GameObject.FindGameObjectsWithTag("BombPlant");
		this.bomber = new PlayerScript[this.bombPlant.Length];
		this.initialized = true;
	}

	public bool canPlant(Vector3 pos)
	{
		for (int i = 0; i < this.bombPlant.Length; i++)
		{
			if ((this.bombPlant[i].transform.position - pos).magnitude < 2f)
			{
				return true;
			}
		}
		return false;
	}

	private void Start()
	{
		this.Initialize();
		Kube.BCS.hud.timer.gameObject.SetActive(true);
	}

	private void UpdateHUD()
	{
		Kube.BCS.hud.timer.timer = Mathf.FloorToInt((float)Kube.BCS.gameEndTime - Time.realtimeSinceStartup);
	}

	private void Update()
	{
		if (Kube.BCS.gameProcess == BattleControllerScript.GameProcess.game)
		{
			this.UpdateHUD();
		}
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		if (Kube.BCS.gameProcess == BattleControllerScript.GameProcess.game)
		{
			base.TransportRespawnTick();
			if ((float)Kube.BCS.gameEndTime < Time.realtimeSinceStartup)
			{
				if (Kube.IS.ps == null)
				{
					return;
				}
				bool flag = true;
				for (int i = 0; i < 4; i++)
				{
					if (i != Kube.IS.ps.team)
					{
						if (Kube.BCS.teamScore[Kube.IS.ps.team] <= Kube.BCS.teamScore[i])
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					BattleControllerScript bcs = Kube.BCS;
					bcs.bonusCounters.winnerTeam = bcs.bonusCounters.winnerTeam + 1;
				}
				base.EndRound();
			}
			for (int j = 0; j < Kube.BCS.playersInfo.Length; j++)
			{
				PlayerScript ps = Kube.BCS.playersInfo[j].ps;
				if (ps.team == 0)
				{
					this.TryGiveBomb(ps);
				}
			}
			this.playersDead[0] = 0;
			this.playersDead[1] = 0;
			for (int k = 0; k < Kube.BCS.playersInfo.Length; k++)
			{
				PlayerScript ps2 = Kube.BCS.playersInfo[k].ps;
				if (ps2.dead)
				{
					this.playersDead[ps2.team]++;
				}
			}
			int num = 0;
			for (int l = 0; l < 2; l++)
			{
				if (this.playersDead[l] >= Kube.BCS.playersInTeam[l] && Kube.BCS.playersInTeam[l] > 0)
				{
					num |= 1 << l;
				}
			}
			if (num > 0)
			{
				if ((num & 1) != 0 && Kube.BCS.playersInTeam[1] > 0)
				{
					Kube.BCS.NO.ChangeTeamScore(1, 1);
				}
				if ((num & 2) != 0 && Kube.BCS.playersInTeam[0] > 0)
				{
					Kube.BCS.NO.ChangeTeamScore(1, 0);
				}
				base.Invoke("RespawnAll", 1f);
				Kube.BCS.gameProcess = BattleControllerScript.GameProcess.end;
			}
		}
	}

	protected override void _Restart()
	{
		if (Kube.IS.ps == null)
		{
			return;
		}
		Kube.BCS.gameStartTime = Time.realtimeSinceStartup;
		Kube.BCS.gameEndTime = (int)Kube.BCS.gameStartTime + Kube.OH.gameMaxTime[(int)Kube.BCS.gameType];
		Kube.BCS.battleCamera.SetActive(false);
		Kube.BCS.gameProcess = BattleControllerScript.GameProcess.game;
		Kube.BCS.endRound.gameObject.SetActive(false);
		Kube.BCS.endRoundScoresUI.gameObject.SetActive(false);
		Kube.IS.ps.cameraComp.enabled = true;
		Kube.IS.ps.playerView.enabled = true;
		Kube.IS.ps.paused = false;
		Kube.IS.ps.Respawn();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			PlayerScript component = array[i].GetComponent<PlayerScript>();
			component.kills = 0;
			component.frags = 0;
			component.deadTimes = 0;
		}
		for (int j = 0; j < 4; j++)
		{
			Kube.BCS.teamScore[j] = 0;
		}
	}

	private Vector3 findSpawnPos(int team)
	{
		Vector3 position = new Vector3(1f, 40f, 1f);
		GameObject[] array = new GameObject[0];
		if (team == 0)
		{
			array = this.respawnsRed;
		}
		if (team == 1)
		{
			array = this.respawnsBlue;
		}
		if (array.Length != 0)
		{
			position = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
		}
		return position;
	}

	public override void EnterGame(int team = -1)
	{
		if ((float)Kube.BCS.gameEndTime < Time.realtimeSinceStartup)
		{
			if (PhotonNetwork.isMasterClient)
			{
				PhotonNetwork.room.visible = false;
			}
			Kube.BCS.ExitGame();
			return;
		}
		int[] array = new int[4];
		for (int i = 0; i < 4; i++)
		{
			array[i] = 0;
			if (i != 0 || this.respawnsRed != null)
			{
				if (i != 0 || this.respawnsRed.Length != 0)
				{
					if (i != 1 || this.respawnsBlue != null)
					{
						if (i != 1 || this.respawnsBlue.Length != 0)
						{
							array[i] = Kube.BCS.teamScore[i] + 1;
						}
					}
				}
			}
		}
		int team2 = AuxFunc.RandomSelectWithChance(array);
		if (team != -1)
		{
			team2 = team;
		}
		Kube.BCS.battleCamera.SetActive(false);
		Vector3 respawnPlace = this.findSpawnPos(team2);
		PlayerScript playerScript = Kube.BCS.CreatePlayer(respawnPlace, Quaternion.identity);
		Kube.BCS.ps = playerScript;
		playerScript.SetTeam(team2);
		playerScript.ShowMyTeam();
		Kube.BCS.gameProcess = BattleControllerScript.GameProcess.game;
		Kube.OH.closeMenu(null);
	}

	public override void PlayerSpawned(PlayerScript playerScript)
	{
	}

	public void BombExplode(BombScript bombScript)
	{
		PlayerScript playerScript = Kube.BCS.FindPlayerByPV(bombScript.photonView.owner);
		if (playerScript)
		{
			Kube.BCS.NO.ChangeTeamScore(1, playerScript.team);
		}
		if (PhotonNetwork.isMasterClient)
		{
			base.Invoke("RespawnAll", 1f);
		}
	}

	private void RespawnAll()
	{
		Kube.BCS.gameProcess = BattleControllerScript.GameProcess.game;
		for (int i = 0; i < Kube.BCS.playersInfo.Length; i++)
		{
			PlayerScript ps = Kube.BCS.playersInfo[i].ps;
			Vector3 pos = this.findSpawnPos(ps.team);
			ps.SurvivalRespawn(pos);
		}
		BombScript[] array = UnityEngine.Object.FindObjectsOfType<BombScript>();
		for (int j = 0; j < array.Length; j++)
		{
			array[j].Remove();
		}
		this.bomber = new PlayerScript[this.bombPlant.Length];
	}

	private void TryGiveBomb(PlayerScript ps)
	{
		for (int i = 0; i < this.bomber.Length; i++)
		{
			if (this.bomber[i] == this)
			{
				return;
			}
			if (!(this.bomber[i] != null))
			{
				if (UnityEngine.Random.value > 0.5f)
				{
					return;
				}
				ps.GiveLotOfDrop(new FastInventar[]
				{
					new FastInventar(InventarType.weapons, 73)
				});
				this.bomber[i] = ps;
			}
		}
	}

	private void BombPlanted()
	{
		Kube.GPS.printSystemMessage(Localize.bombPlanted, new Color(1f, 0f, 0f, 0.5f));
	}

	private void BombDisarm()
	{
		Kube.GPS.printSystemMessage(Localize.bombDisarmed, new Color(1f, 0f, 0f, 0.5f));
		if (PhotonNetwork.isMasterClient)
		{
			Kube.BCS.NO.ChangeTeamScore(1, 1);
			base.Invoke("RespawnAll", 1f);
			Kube.BCS.gameProcess = BattleControllerScript.GameProcess.end;
		}
	}

	public override bool DropStuff(PlayerScript playerScript, Dictionary<int, int> _weaponPickup)
	{
		if (playerScript.team == 1)
		{
			return false;
		}
		int key = 73;
		if (!_weaponPickup.ContainsKey(key))
		{
			return false;
		}
		WeaponBomb component = Kube.OH.charWeaponsGO[key].GetComponent<WeaponBomb>();
		string prefabName = component.prefabName;
		PhotonNetwork.Instantiate(prefabName, playerScript.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, 0, new object[]
		{
			false
		});
		return false;
	}

	private GameObject[] respawnsRed;

	private GameObject[] respawnsBlue;

	public GameObject[] bombPlant;

	private bool initialized;

	private int[] playersDead = new int[2];

	protected PlayerScript[] bomber;
}
