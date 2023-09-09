using System;
using UnityEngine;

public class KubeApp
{
	public static PlatformType detectPlatform()
	{
		RuntimePlatform platform = Application.platform;
		switch (platform)
		{
		case RuntimePlatform.IPhonePlayer:
		case RuntimePlatform.Android:
			return PlatformType.Mobile;
		default:
			if (platform == RuntimePlatform.WindowsPlayer)
			{
				return PlatformType.PC;
			}
			if (platform != RuntimePlatform.WebGLPlayer)
			{
				return PlatformType.Web;
			}
			return PlatformType.Web;
		}
	}
}
