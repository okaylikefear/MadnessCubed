using System;

namespace CodeStage.AntiCheat.Utils
{
	internal class xxHash
	{
		public static uint CalculateHash(byte[] buf, int len, uint seed = 0u)
		{
			int i = 0;
			uint num7;
			if (len >= 16)
			{
				int num = len - 16;
				uint num2 = seed + 2654435761u + 2246822519u;
				uint num3 = seed + 2246822519u;
				uint num4 = seed;
				uint num5 = seed - 2654435761u;
				do
				{
					uint num6 = (uint)((int)buf[i++] | (int)buf[i++] << 8 | (int)buf[i++] << 16 | (int)buf[i++] << 24);
					num2 += num6 * 2246822519u;
					num2 = (num2 << 13 | num2 >> 19);
					num2 *= 2654435761u;
					num6 = (uint)((int)buf[i++] | (int)buf[i++] << 8 | (int)buf[i++] << 16 | (int)buf[i++] << 24);
					num3 += num6 * 2246822519u;
					num3 = (num3 << 13 | num3 >> 19);
					num3 *= 2654435761u;
					num6 = (uint)((int)buf[i++] | (int)buf[i++] << 8 | (int)buf[i++] << 16 | (int)buf[i++] << 24);
					num4 += num6 * 2246822519u;
					num4 = (num4 << 13 | num4 >> 19);
					num4 *= 2654435761u;
					num6 = (uint)((int)buf[i++] | (int)buf[i++] << 8 | (int)buf[i++] << 16 | (int)buf[i++] << 24);
					num5 += num6 * 2246822519u;
					num5 = (num5 << 13 | num5 >> 19);
					num5 *= 2654435761u;
				}
				while (i <= num);
				num7 = (num2 << 1 | num2 >> 31) + (num3 << 7 | num3 >> 25) + (num4 << 12 | num4 >> 20) + (num5 << 18 | num5 >> 14);
			}
			else
			{
				num7 = seed + 374761393u;
			}
			num7 += (uint)len;
			while (i <= len - 4)
			{
				num7 += (uint)(((int)buf[i++] | (int)buf[i++] << 8 | (int)buf[i++] << 16 | (int)buf[i++] << 24) * -1028477379);
				num7 = (num7 << 17 | num7 >> 15) * 668265263u;
			}
			while (i < len)
			{
				num7 += (uint)buf[i] * 374761393u;
				num7 = (num7 << 11 | num7 >> 21) * 2654435761u;
				i++;
			}
			num7 ^= num7 >> 15;
			num7 *= 2246822519u;
			num7 ^= num7 >> 13;
			num7 *= 3266489917u;
			return num7 ^ num7 >> 16;
		}

		private const uint PRIME32_1 = 2654435761u;

		private const uint PRIME32_2 = 2246822519u;

		private const uint PRIME32_3 = 3266489917u;

		private const uint PRIME32_4 = 668265263u;

		private const uint PRIME32_5 = 374761393u;
	}
}
