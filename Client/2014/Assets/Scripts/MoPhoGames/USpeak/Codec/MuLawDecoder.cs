using System;

namespace MoPhoGames.USpeak.Codec
{
	public class MuLawDecoder
	{
		static MuLawDecoder()
		{
			for (byte b = 0; b < 255; b += 1)
			{
				MuLawDecoder.muLawToPcmMap[(int)b] = MuLawDecoder.Decode(b);
			}
		}

		public static short[] MuLawDecode(byte[] data)
		{
			int num = data.Length;
			short[] array = new short[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = MuLawDecoder.muLawToPcmMap[(int)data[i]];
			}
			return array;
		}

		private static short Decode(byte mulaw)
		{
			mulaw = ~mulaw;
			int num = (int)(mulaw & 128);
			int num2 = (mulaw & 112) >> 4;
			int num3 = (int)(mulaw & 15);
			num3 |= 16;
			num3 <<= 1;
			num3++;
			num3 <<= num2 + 2;
			num3 -= 132;
			return (short)((num != 0) ? (-(short)num3) : num3);
		}

		private static readonly short[] muLawToPcmMap = new short[256];
	}
}
