using System;

namespace MoPhoGames.USpeak.Codec
{
	public class MuLawEncoder
	{
		static MuLawEncoder()
		{
			for (int i = -32768; i <= 32767; i++)
			{
				MuLawEncoder.pcmToMuLawMap[i & 65535] = MuLawEncoder.encode(i);
			}
		}

		public static bool ZeroTrap
		{
			get
			{
				return MuLawEncoder.pcmToMuLawMap[33000] != 0;
			}
			set
			{
				byte b = (!value) ? 0 : 2;
				for (int i = 32768; i <= 33924; i++)
				{
					MuLawEncoder.pcmToMuLawMap[i] = b;
				}
			}
		}

		public static byte MuLawEncode(int pcm)
		{
			return MuLawEncoder.pcmToMuLawMap[pcm & 65535];
		}

		public static byte MuLawEncode(short pcm)
		{
			return MuLawEncoder.pcmToMuLawMap[(int)pcm & 65535];
		}

		public static byte[] MuLawEncode(int[] pcm)
		{
			int num = pcm.Length;
			byte[] array = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = MuLawEncoder.MuLawEncode(pcm[i]);
			}
			return array;
		}

		public static byte[] MuLawEncode(short[] pcm)
		{
			int num = pcm.Length;
			byte[] array = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = MuLawEncoder.MuLawEncode(pcm[i]);
			}
			return array;
		}

		private static byte encode(int pcm)
		{
			int num = (pcm & 32768) >> 8;
			if (num != 0)
			{
				pcm = -pcm;
			}
			if (pcm > 32635)
			{
				pcm = 32635;
			}
			pcm += 132;
			int num2 = 7;
			int num3 = 16384;
			while ((pcm & num3) == 0)
			{
				num2--;
				num3 >>= 1;
			}
			int num4 = pcm >> num2 + 3 & 15;
			byte b = (byte)(num | num2 << 4 | num4);
			return ~b;
		}

		public const int BIAS = 132;

		public const int MAX = 32635;

		private static byte[] pcmToMuLawMap = new byte[65536];
	}
}
