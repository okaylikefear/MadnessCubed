using System;
using ExitGames.Client.Photon;
using UnityEngine;

public static class TeamExtensions
{
	public static PunTeams.Team GetTeam(this PhotonPlayer player)
	{
		object obj;
		if (player.customProperties.TryGetValue("team", out obj))
		{
			return (PunTeams.Team)((byte)obj);
		}
		return PunTeams.Team.none;
	}

	public static void SetTeam(this PhotonPlayer player, PunTeams.Team team)
	{
		if (!PhotonNetwork.connectedAndReady)
		{
			UnityEngine.Debug.LogWarning("JoinTeam was called in state: " + PhotonNetwork.connectionStateDetailed + ". Not connectedAndReady.");
		}
		PunTeams.Team team2 = PhotonNetwork.player.GetTeam();
		if (team2 != team)
		{
			PhotonNetwork.player.SetCustomProperties(new Hashtable
			{
				{
					"team",
					(byte)team
				}
			}, null, false);
		}
	}
}
