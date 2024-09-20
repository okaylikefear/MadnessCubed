using System;
using UnityEngine;

[Serializable]
public class BitData
{
	public BitData()
	{
	}

	public BitData(int x, int y, int z)
	{
		this.EnsureAllocated(x, y, z);
	}

	public void EnsureAllocated(int x, int y, int z)
	{
		this.sx = Mathf.CeilToInt((float)(x / 16)) + 1;
		this.sy = Mathf.CeilToInt((float)(y / 16)) + 1;
		this.sz = Mathf.CeilToInt((float)(z / 16)) + 1;
		this.data = new BitArea[this.sx * this.sy * this.sz];
	}

	public bool this[int x, int y, int z]
	{
		get
		{
			bool result = false;
			int num = x & 15;
			int num2 = y & 15;
			int num3 = z & 15;
			int num4 = x >> 4;
			int num5 = y >> 4;
			int num6 = z >> 4;
			int num7 = num | num3 << 4 | num2 << 8;
			int num8 = num7 >> 5;
			int num9 = num7 & 31;
			if (this.data.Length < num4 + num5 * this.sx + num6 * this.sy * this.sx)
			{
				UnityEngine.Debug.LogError("fdf");
			}
			BitArea bitArea = this.data[num4 + num5 * this.sx + num6 * this.sy * this.sx];
			if (bitArea != null)
			{
				return (bitArea.data[num8] & 1 << num9) != 0;
			}
			return result;
		}
		set
		{
			int num = x & 15;
			int num2 = y & 15;
			int num3 = z & 15;
			int num4 = x >> 4;
			int num5 = y >> 4;
			int num6 = z >> 4;
			int num7 = num | num3 << 4 | num2 << 8;
			int num8 = num7 >> 5;
			int num9 = num7 & 31;
			BitArea bitArea = this.data[num4 + num5 * this.sx + num6 * this.sy * this.sx];
			if (bitArea == null)
			{
				if (!value)
				{
					return;
				}
				bitArea = new BitArea();
				bitArea.data = new int[128];
				this.data[num4 + num5 * this.sx + num6 * this.sy * this.sx] = bitArea;
			}
			int num10 = 1 << num9;
			if (value)
			{
				bitArea.data[num8] |= num10;
			}
			else
			{
				bitArea.data[num8] &= ~num10;
			}
		}
	}

	[HideInInspector]
	public BitArea[] data;

	[SerializeField]
	private int sx;

	[SerializeField]
	private int sy;

	[SerializeField]
	private int sz;
}
