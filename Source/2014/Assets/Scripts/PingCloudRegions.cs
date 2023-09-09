using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class PingCloudRegions : MonoBehaviour
{
	private void Awake()
	{
		PingCloudRegions.SP = this;
		if (PlayerPrefs.GetString("PUNCloudBestRegion", string.Empty) != string.Empty)
		{
			string @string = PlayerPrefs.GetString("PUNCloudBestRegion", string.Empty);
			PingCloudRegions.closestRegion = (CloudServerRegion)((int)Enum.Parse(typeof(CloudServerRegion), @string, true));
			return;
		}
		base.StartCoroutine(this.PingAllRegions());
	}

	public static void OverrideRegion(CloudServerRegion region)
	{
		PingCloudRegions.SetRegion(region);
	}

	public static void RefreshCloudServerRating()
	{
		if (PingCloudRegions.SP != null)
		{
			PingCloudRegions.SP.StartCoroutine(PingCloudRegions.SP.PingAllRegions());
		}
	}

	public static void ConnectToBestRegion(string gameVersion)
	{
		PingCloudRegions.SP.StartCoroutine(PingCloudRegions.SP.ConnectToBestRegionInternal(gameVersion));
	}

	public IEnumerator PingAllRegions()
	{
		ServerSettings settings = (ServerSettings)Resources.Load("PhotonServerSettings", typeof(ServerSettings));
		if (settings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			yield break;
		}
		this.isPinging = true;
		foreach (object obj in Enum.GetValues(typeof(CloudServerRegion)))
		{
			CloudServerRegion region = (CloudServerRegion)((int)obj);
			yield return base.StartCoroutine(this.PingRegion(region));
		}
		this.isPinging = false;
		yield break;
	}

	private IEnumerator PingRegion(CloudServerRegion region)
	{
		string hostname = ServerSettings.FindServerAddressForRegion(region);
		string regionIp = PingCloudRegions.ResolveHost(hostname);
		if (string.IsNullOrEmpty(regionIp))
		{
			UnityEngine.Debug.LogError("Could not resolve host: " + hostname);
			yield break;
		}
		int averagePing = 0;
		int tries = 3;
		int skipped = 0;
		float timeout = 0.5f;
		for (int i = 0; i < tries; i++)
		{
			float startTime = Time.time;
			Ping ping = new Ping(regionIp);
			while (!ping.isDone && Time.time < startTime + timeout)
			{
				yield return 0;
			}
			if (ping.time == -1)
			{
				if (skipped > 5)
				{
					averagePing += (int)(timeout * 1000f) * tries;
					break;
				}
				i--;
				skipped++;
			}
			else
			{
				averagePing += ping.time;
			}
		}
		int regionAverage = averagePing / tries;
		if (regionAverage < this.lowestRegionAverage || this.lowestRegionAverage == -1)
		{
			this.lowestRegionAverage = regionAverage;
			PingCloudRegions.SetRegion(region);
		}
		yield break;
	}

	private static void SetRegion(CloudServerRegion region)
	{
		PingCloudRegions.closestRegion = region;
		PlayerPrefs.SetString("PUNCloudBestRegion", region.ToString());
	}

	private IEnumerator ConnectToBestRegionInternal(string gameVersion)
	{
		while (this.isPinging)
		{
			yield return 0;
		}
		ServerSettings settings = (ServerSettings)Resources.Load("PhotonServerSettings", typeof(ServerSettings));
		if (settings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			PhotonNetwork.ConnectUsingSettings(gameVersion);
		}
		else
		{
			PhotonNetwork.Connect(ServerSettings.FindServerAddressForRegion(PingCloudRegions.closestRegion), settings.ServerPort, settings.AppID, gameVersion);
		}
		yield break;
	}

	public static string ResolveHost(string hostString)
	{
		try
		{
			foreach (IPAddress ipaddress in Dns.GetHostAddresses(hostString))
			{
				if (ipaddress != null && ipaddress.AddressFamily == AddressFamily.InterNetwork)
				{
					return ipaddress.ToString();
				}
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log("Exception caught! " + ex.Source + " Message: " + ex.Message);
		}
		return string.Empty;
	}

	private const string playerPrefsKey = "PUNCloudBestRegion";

	public static CloudServerRegion closestRegion = CloudServerRegion.US;

	public static PingCloudRegions SP;

	private bool isPinging;

	private int lowestRegionAverage = -1;
}
