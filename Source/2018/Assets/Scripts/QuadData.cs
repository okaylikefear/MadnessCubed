using System;
using UnityEngine;

[Serializable]
public class QuadData
{
	public QuadData()
	{
	}

	public QuadData(int x, int y, int z)
	{
		this.EnsureAllocated(x, y, z);
	}

	public void EnsureAllocated(int x, int y, int z)
	{
		this.sx = Mathf.CeilToInt((float)(x / 16)) + 1;
		this.sy = Mathf.CeilToInt((float)(y / 16)) + 1;
		this.sz = Mathf.CeilToInt((float)(z / 16)) + 1;
		this.data = new byte[this.sx, this.sy, this.sz][];
	}

	public byte this[int x, int y, int z]
	{
		get
		{
			int num = x >> 4;
			int num2 = y >> 4;
			int num3 = z >> 4;
			int num4 = x & 15;
			int num5 = y & 15;
			int num6 = z & 15;
			byte[] array = this.data[num, num2, num3];
			if (array == null)
			{
				return 0;
			}
			int num7 = num4 | num6 << 4 | num5 << 8;
			int num8 = num7 & 1;
			int num9 = num7 >> 1;
			byte result;
			if (num8 == 0)
			{
				result = (array[num9] & 15);
			}
			else
			{
				result = (byte)((array[num9] & 240) >> 4);
			}
			return result;
		}
		set
		{
			int num = x >> 4;
			int num2 = y >> 4;
			int num3 = z >> 4;
			int num4 = x & 15;
			int num5 = y & 15;
			int num6 = z & 15;
			byte[] array = this.data[num, num2, num3];
			if (array == null)
			{
				if (value == 0)
				{
					return;
				}
				array = new byte[2048];
				this.data[num, num2, num3] = array;
			}
			int num7 = num4 | num6 << 4 | num5 << 8;
			int num8 = num7 & 1;
			int num9 = num7 >> 1;
			if (num8 == 0)
			{
				array[num9] = ((array[num9] & 240) | (value & 15));
			}
			else
			{
				array[num9] = (byte)((int)(array[num9] & 15) | ((int)value << 4 & 240));
			}
		}
	}

	[HideInInspector]
	public byte[,,][] data;

	[SerializeField]
	private int sx;

	[SerializeField]
	private int sy;

	[SerializeField]
	private int sz;
}
