using System;
using kube.map;

public class CubeTracer
{
	public static void init(CubeGrid cubegrid)
	{
		CubeTracer._cubegrid = cubegrid;
	}

	public static int get(int x, int y, int z)
	{
		int num = x >> 4;
		int num2 = y >> 4;
		int num3 = z >> 4;
		int num4 = x & 15;
		int num5 = y & 15;
		int num6 = z & 15;
		Chunk chunk = CubeTracer._cubegrid.chunks[num, num2, num3];
		if (chunk == null)
		{
			return 0;
		}
		if (chunk.type == null)
		{
			return 0;
		}
		int num7 = num4 | num6 << 4 | num5 << 8;
		if (chunk.xtype == null)
		{
			return (int)chunk.type[num7];
		}
		int num8 = num7 & 1;
		int num9 = num7 >> 1;
		if (num8 == 0)
		{
			return (int)(chunk.xtype[num9] & 15) << 8 | (int)chunk.type[num7];
		}
		return (int)(chunk.xtype[num9] & 240) << 4 | (int)chunk.type[num7];
	}

	private static CubeGrid _cubegrid;

	private static int _cx;

	private static int _cy;

	private static int _cz;
}
