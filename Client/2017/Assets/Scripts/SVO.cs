using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using kube;
using UnityEngine;

public class SVO
{
	public SVO()
	{
		for (int i = 0; i < 32; i++)
		{
			this.head[i] = 0u;
		}
		this.buildOctree(0, 0, 0, Kube.WHS.sizeX, 0);
		this._octree = this.finalize();
	}

	public ulong buildOctree(int x, int y, int z, int size, int pos)
	{
		int num = size >> 1;
		int[] array = new int[]
		{
			x + num,
			x,
			x + num,
			x,
			x + num,
			x,
			x + num,
			x
		};
		int[] array2 = new int[]
		{
			y + num,
			y + num,
			y,
			y,
			y + num,
			y + num,
			y,
			y
		};
		int[] array3 = new int[]
		{
			z + num,
			z + num,
			z + num,
			z + num,
			z,
			z,
			z,
			z
		};
		int num2 = 0;
		uint num3 = 0u;
		uint num4;
		if (num == 1)
		{
			num4 = 0u;
			for (int i = 0; i < 8; i++)
			{
				if (Kube.WHS.cubeContainsVoxelsDestructive(array[i], array2[i], array3[i], num))
				{
					num3 |= (uint)(128 >> i);
					num2++;
				}
			}
			if (num3 != 0u)
			{
				this.allocator.Add((uint)(pos << 16 | (int)((int)num3 << 8) | (int)num4));
				this.head[pos] += 1u;
			}
			return (ulong)((long)num2);
		}
		ulong num5 = 0UL;
		for (int j = 0; j < 8; j++)
		{
			if (num == 64)
			{
				UnityEngine.Debug.Log(num3);
			}
			ulong num6 = this.buildOctree(array[j], array2[j], array3[j], num, pos + 1);
			num5 += num6;
			if (num == 64)
			{
				UnityEngine.Debug.Log(num5);
			}
			if (num6 > 0UL)
			{
				num3 |= (uint)(128 >> j);
				num2++;
			}
		}
		num4 = num3;
		if (num3 != 0u)
		{
			if (y > 64)
			{
				UnityEngine.Debug.Log(num3);
			}
			this.allocator.Add((uint)(pos << 16 | (int)((int)num3 << 8) | (int)num4));
			this.head[pos] += 1u;
		}
		return num5;
	}

	private uint[] finalize()
	{
		uint[] array = new uint[this.allocator.Count];
		uint[] array2 = new uint[this.head.Length + 1];
		uint num = 0u;
		for (int i = 0; i < this.head.Length; i++)
		{
			num += this.head[i];
			array2[i + 1] = num;
			this.head[i] = 0u;
		}
		for (int j = this.allocator.Count - 1; j >= 0; j--)
		{
			uint num2 = this.allocator[j];
			num = (num2 >> 16 & 65535u);
			uint num3 = array2[(int)((UIntPtr)num)] + this.head[(int)((UIntPtr)num)];
			if (array[(int)((UIntPtr)num3)] != 0u)
			{
				UnityEngine.Debug.Log("relop");
			}
			num2 = (array2[(int)((UIntPtr)(num + 1u))] + this.head[(int)((UIntPtr)(num + 1u))] - num3 << 16 | (num2 & 65535u));
			array[(int)((UIntPtr)num3)] = num2;
			this.head[(int)((UIntPtr)num)] += 1u;
		}
		return array;
	}

	private static float uintBitsToFloat(int i)
	{
		SVO.unionfloat unionfloat = default(SVO.unionfloat);
		unionfloat.i = (uint)i;
		return unionfloat.f;
	}

	private static float uintBitsToFloat(uint i)
	{
		SVO.unionfloat unionfloat = default(SVO.unionfloat);
		unionfloat.i = i;
		return unionfloat.f;
	}

	private static uint floatBitsToUint(float f)
	{
		SVO.unionfloat unionfloat = default(SVO.unionfloat);
		unionfloat.f = f;
		return unionfloat.i;
	}

	public bool trace(Vector3 p, Vector3 d, float rayScale, out uint normal, out float t)
	{
		float num = 1f / (float)Kube.WHS.sizeX;
		float num2 = 1f + p.x * num;
		float num3 = 1f + p.y * num;
		float num4 = 1f + p.z * num;
		float num5 = d.x;
		float num6 = d.y;
		float num7 = d.z;
		if (Mathf.Abs(num5) < 0.0001f)
		{
			num5 = 0.0001f;
		}
		if (Mathf.Abs(num6) < 0.0001f)
		{
			num6 = 0.0001f;
		}
		if (Mathf.Abs(num7) < 0.0001f)
		{
			num7 = 0.0001f;
		}
		float num8 = 1f / -Mathf.Abs(num5);
		float num9 = 1f / -Mathf.Abs(num6);
		float num10 = 1f / -Mathf.Abs(num7);
		float num11 = num8 * num2;
		float num12 = num9 * num3;
		float num13 = num10 * num4;
		byte b = 7;
		if (num5 > 0f)
		{
			b ^= 1;
			num11 = 3f * num8 - num11;
		}
		if (num6 > 0f)
		{
			b ^= 2;
			num12 = 3f * num9 - num12;
		}
		if (num7 > 0f)
		{
			b ^= 4;
			num13 = 3f * num10 - num13;
		}
		float num14 = Mathf.Max(2f * num8 - num11, Mathf.Max(2f * num9 - num12, 2f * num10 - num13));
		float num15 = Mathf.Min(num8 - num11, Mathf.Min(num9 - num12, num10 - num13));
		num14 = Mathf.Max(num14, 0f);
		uint num16 = 0u;
		ulong num17 = 0UL;
		int num18 = 0;
		float num19 = 1f;
		float num20 = 1f;
		float num21 = 1f;
		int i = 22;
		float num22 = 0.5f;
		if (1.5f * num8 - num11 > num14)
		{
			num18 ^= 1;
			num19 = 1.5f;
		}
		if (1.5f * num9 - num12 > num14)
		{
			num18 ^= 2;
			num20 = 1.5f;
		}
		if (1.5f * num10 - num13 > num14)
		{
			num18 ^= 4;
			num21 = 1.5f;
		}
		normal = 0u;
		t = 100000f;
		while (i < 23)
		{
			if (num16 == 0u)
			{
				num16 = this._octree[(int)(checked((IntPtr)num17))];
			}
			float num23 = num19 * num8 - num11;
			float num24 = num20 * num9 - num12;
			float num25 = num21 * num10 - num13;
			float num26 = Mathf.Min(num23, Mathf.Min(num24, num25));
			int num27 = num18 ^ (int)b;
			uint num28 = num16 << num27;
			if ((num28 & 32768u) != 0u && num14 <= num15)
			{
				if (num26 * rayScale >= num22)
				{
					t = num26;
					return true;
				}
				float num29 = Mathf.Min(num15, num26);
				num29 = Mathf.Max(num29, 0f);
				float num30 = num22 * 0.5f;
				float num31 = num30 * num8 + num23;
				float num32 = num30 * num9 + num24;
				float num33 = num30 * num10 + num25;
				if (num14 <= num29)
				{
					uint num34 = num16 >> 16;
					if ((num28 & 128u) == 0u)
					{
						normal = 0u;
						break;
					}
					this.rayStack[i].offset = num17;
					this.rayStack[i].maxT = num15;
					uint num35 = SVO.BitCount[(int)((UIntPtr)(num28 & 127u))];
					num17 += (ulong)(num34 + num35);
					num18 = 0;
					i--;
					num22 = num30;
					if (num31 > num14)
					{
						num18 ^= 1;
						num19 += num22;
					}
					if (num32 > num14)
					{
						num18 ^= 2;
						num20 += num22;
					}
					if (num33 > num14)
					{
						num18 ^= 4;
						num21 += num22;
					}
					num15 = num29;
					num16 = 0u;
					continue;
				}
			}
			int num36 = 0;
			if (num23 <= num26)
			{
				num36 ^= 1;
				num19 -= num22;
			}
			if (num24 <= num26)
			{
				num36 ^= 2;
				num20 -= num22;
			}
			if (num25 <= num26)
			{
				num36 ^= 4;
				num21 -= num22;
			}
			num14 = num26;
			num18 ^= num36;
			if ((num18 & num36) != 0)
			{
				uint num37 = 0u;
				if ((num36 & 1) != 0)
				{
					num37 |= (SVO.floatBitsToUint(num19) ^ SVO.floatBitsToUint(num19 + num22));
				}
				if ((num36 & 2) != 0)
				{
					num37 |= (SVO.floatBitsToUint(num20) ^ SVO.floatBitsToUint(num20 + num22));
				}
				if ((num36 & 4) != 0)
				{
					num37 |= (SVO.floatBitsToUint(num21) ^ SVO.floatBitsToUint(num21 + num22));
				}
				i = (int)((SVO.floatBitsToUint(num37) >> 23) - 127u);
				num22 = SVO.uintBitsToFloat(i - 23 + 127 << 23);
				if (i >= 23)
				{
					return false;
				}
				num17 = this.rayStack[i].offset;
				num15 = this.rayStack[i].maxT;
				int num38 = (int)SVO.floatBitsToUint(num19) >> i;
				int num39 = (int)SVO.floatBitsToUint(num20) >> i;
				int num40 = (int)SVO.floatBitsToUint(num21) >> i;
				num19 = SVO.uintBitsToFloat(num38 << i);
				num20 = SVO.uintBitsToFloat(num39 << i);
				num21 = SVO.uintBitsToFloat(num40 << i);
				num18 = ((num38 & 1) | (num39 & 1) << 1 | (num40 & 1) << 2);
				num16 = 0u;
			}
		}
		if (i >= 23)
		{
			return false;
		}
		t = num14;
		return true;
	}

	private const int MaxScale = 23;

	private const float TreeMiss = 100000f;

	public uint[] _octree;

	private List<uint> allocator = new List<uint>();

	private uint[] head = new uint[32];

	private SVO.StackEntry[] rayStack = new SVO.StackEntry[24];

	private static uint[] BitCount = new uint[]
	{
		0u,
		1u,
		1u,
		2u,
		1u,
		2u,
		2u,
		3u,
		1u,
		2u,
		2u,
		3u,
		2u,
		3u,
		3u,
		4u,
		1u,
		2u,
		2u,
		3u,
		2u,
		3u,
		3u,
		4u,
		2u,
		3u,
		3u,
		4u,
		3u,
		4u,
		4u,
		5u,
		1u,
		2u,
		2u,
		3u,
		2u,
		3u,
		3u,
		4u,
		2u,
		3u,
		3u,
		4u,
		3u,
		4u,
		4u,
		5u,
		2u,
		3u,
		3u,
		4u,
		3u,
		4u,
		4u,
		5u,
		3u,
		4u,
		4u,
		5u,
		4u,
		5u,
		5u,
		6u,
		1u,
		2u,
		2u,
		3u,
		2u,
		3u,
		3u,
		4u,
		2u,
		3u,
		3u,
		4u,
		3u,
		4u,
		4u,
		5u,
		2u,
		3u,
		3u,
		4u,
		3u,
		4u,
		4u,
		5u,
		3u,
		4u,
		4u,
		5u,
		4u,
		5u,
		5u,
		6u,
		2u,
		3u,
		3u,
		4u,
		3u,
		4u,
		4u,
		5u,
		3u,
		4u,
		4u,
		5u,
		4u,
		5u,
		5u,
		6u,
		3u,
		4u,
		4u,
		5u,
		4u,
		5u,
		5u,
		6u,
		4u,
		5u,
		5u,
		6u,
		5u,
		6u,
		6u,
		7u,
		1u,
		2u,
		2u,
		3u,
		2u,
		3u,
		3u,
		4u,
		2u,
		3u,
		3u,
		4u,
		3u,
		4u,
		4u,
		5u,
		2u,
		3u,
		3u,
		4u,
		3u,
		4u,
		4u,
		5u,
		3u,
		4u,
		4u,
		5u,
		4u,
		5u,
		5u,
		6u,
		2u,
		3u,
		3u,
		4u,
		3u,
		4u,
		4u,
		5u,
		3u,
		4u,
		4u,
		5u,
		4u,
		5u,
		5u,
		6u,
		3u,
		4u,
		4u,
		5u,
		4u,
		5u,
		5u,
		6u,
		4u,
		5u,
		5u,
		6u,
		5u,
		6u,
		6u,
		7u,
		2u,
		3u,
		3u,
		4u,
		3u,
		4u,
		4u,
		5u,
		3u,
		4u,
		4u,
		5u,
		4u,
		5u,
		5u,
		6u,
		3u,
		4u,
		4u,
		5u,
		4u,
		5u,
		5u,
		6u,
		4u,
		5u,
		5u,
		6u,
		5u,
		6u,
		6u,
		7u,
		3u,
		4u,
		4u,
		5u,
		4u,
		5u,
		5u,
		6u,
		4u,
		5u,
		5u,
		6u,
		5u,
		6u,
		6u,
		7u,
		4u,
		5u,
		5u,
		6u,
		5u,
		6u,
		6u,
		7u,
		5u,
		6u,
		6u,
		7u,
		6u,
		7u,
		7u,
		8u
	};

	private struct StackEntry
	{
		public ulong offset;

		public float maxT;
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct unionfloat
	{
		[FieldOffset(0)]
		public float f;

		[FieldOffset(0)]
		public uint i;
	}
}
