using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ServerSettings : ScriptableObject
{
	public static int FindRegionForServerAddress(string server)
	{
		int result = 0;
		for (int i = 0; i < ServerSettings.CloudServerRegionPrefixes.Length; i++)
		{
			if (server.StartsWith(ServerSettings.CloudServerRegionPrefixes[i]))
			{
				return i;
			}
		}
		return result;
	}

	public static string FindServerAddressForRegion(int regionIndex)
	{
		return "app-eu.exitgamescloud.com".Replace("app-eu", ServerSettings.CloudServerRegionPrefixes[regionIndex]);
	}

	public static string FindServerAddressForRegion(CloudServerRegion regionIndex)
	{
		return "app-eu.exitgamescloud.com".Replace("app-eu", ServerSettings.CloudServerRegionPrefixes[(int)regionIndex]);
	}

	public void UseCloud(string cloudAppid, int regionIndex)
	{
		this.HostType = ServerSettings.HostingOption.PhotonCloud;
		this.AppID = cloudAppid;
		this.ServerAddress = ServerSettings.FindServerAddressForRegion(regionIndex);
		this.ServerPort = 5055;
	}

	public void UseMyServer(string serverAddress, int serverPort, string application)
	{
		this.HostType = ServerSettings.HostingOption.SelfHosted;
		this.AppID = ((application == null) ? "Master" : application);
		this.ServerAddress = serverAddress;
		this.ServerPort = serverPort;
	}

	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"ServerSettings: ",
			this.HostType,
			" ",
			this.ServerAddress
		});
	}

	public const string DefaultCloudServerUrl = "app-eu.exitgamescloud.com";

	public const string DefaultServerAddress = "127.0.0.1";

	public const int DefaultMasterPort = 5055;

	public const string DefaultAppID = "Master";

	public static readonly string[] CloudServerRegionPrefixes = new string[]
	{
		"app-eu",
		"app-us",
		"app-asia",
		"app-jp"
	};

	public ServerSettings.HostingOption HostType;

	public string ServerAddress = "127.0.0.1";

	public int ServerPort = 5055;

	public string AppID = string.Empty;

	public List<string> RpcList;

	[HideInInspector]
	public bool DisableAutoOpenWizard;

	public enum HostingOption
	{
		NotSet,
		PhotonCloud,
		SelfHosted,
		OfflineMode
	}
}
