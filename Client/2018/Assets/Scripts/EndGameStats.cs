using System;

public struct EndGameStats
{
	public EndGameStats(uint playerExp, int deltaExp, int playerFrags, int deltaFrags, int deltaKills, int playerMoney1, int deltaMoney, int playerLevel, int newLevel, int[] _bonuses)
	{
		this.newLevel = newLevel;
		this.playerLevel = playerLevel;
		this.deltaMoney = deltaMoney;
		this.playerMoney1 = playerMoney1;
		this.deltaFrags = deltaFrags;
		this.deltaKills = deltaKills;
		this.playerFrags = playerFrags;
		this.deltaExp = deltaExp;
		this.playerExp = playerExp;
		this.bonuses = new int[_bonuses.Length];
		for (int i = 0; i < this.bonuses.Length; i++)
		{
			this.bonuses[i] = _bonuses[i];
		}
	}

	public uint playerExp;

	public int deltaExp;

	public int playerFrags;

	public int deltaFrags;

	public int deltaKills;

	public int playerMoney1;

	public int deltaMoney;

	public int playerLevel;

	public int newLevel;

	public int[] bonuses;
}
