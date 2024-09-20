using System;
using kube;

public class MissionBase : GameTypeControllerBase
{
	public override int CalcGameStats()
	{
		if (Kube.BCS.ps != null)
		{
			return Kube.BCS.ps.points;
		}
		return 0;
	}
}
