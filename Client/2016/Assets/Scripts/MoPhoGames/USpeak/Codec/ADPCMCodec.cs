using System;

namespace MoPhoGames.USpeak.Codec
{
	internal class ADPCMCodec : ICodec
	{
		private void Init()
		{
			this.predictedSample = 0;
			this.stepsize = 7;
			this.index = 0;
			this.newSample = 0;
		}

		private short ADPCM_Decode(byte originalSample)
		{
			int num = this.stepsize * (int)originalSample / 4 + this.stepsize / 8;
			if ((originalSample & 4) == 4)
			{
				num += this.stepsize;
			}
			if ((originalSample & 2) == 2)
			{
				num += this.stepsize >> 1;
			}
			if ((originalSample & 1) == 1)
			{
				num += this.stepsize >> 2;
			}
			num += this.stepsize >> 3;
			if ((originalSample & 8) == 8)
			{
				num = -num;
			}
			this.newSample = num;
			if (this.newSample > 32767)
			{
				this.newSample = 32767;
			}
			else if (this.newSample < -32768)
			{
				this.newSample = -32768;
			}
			this.index += ADPCMCodec.indexTable[(int)originalSample];
			if (this.index < 0)
			{
				this.index = 0;
			}
			if (this.index > 88)
			{
				this.index = 88;
			}
			this.stepsize = ADPCMCodec.stepsizeTable[this.index];
			return (short)this.newSample;
		}

		private byte ADPCM_Encode(short originalSample)
		{
			int num = (int)originalSample - this.predictedSample;
			if (num >= 0)
			{
				this.newSample = 0;
			}
			else
			{
				this.newSample = 8;
				num = -num;
			}
			byte b = 4;
			int num2 = this.stepsize;
			for (int i = 0; i < 3; i++)
			{
				if (num >= num2)
				{
					this.newSample |= (int)b;
					num -= num2;
				}
				num2 >>= 1;
				b = (byte)(b >> 1);
			}
			num = this.stepsize >> 3;
			if ((this.newSample & 4) != 0)
			{
				num += this.stepsize;
			}
			if ((this.newSample & 2) != 0)
			{
				num += this.stepsize >> 1;
			}
			if ((this.newSample & 1) != 0)
			{
				num += this.stepsize >> 2;
			}
			if ((this.newSample & 8) != 0)
			{
				num = -num;
			}
			this.predictedSample += num;
			if (this.predictedSample > 32767)
			{
				this.predictedSample = 32767;
			}
			if (this.predictedSample < -32768)
			{
				this.predictedSample = -32768;
			}
			this.index += ADPCMCodec.indexTable[this.newSample];
			if (this.index < 0)
			{
				this.index = 0;
			}
			else if (this.index > 88)
			{
				this.index = 88;
			}
			this.stepsize = ADPCMCodec.stepsizeTable[this.index];
			return (byte)this.newSample;
		}

		public byte[] Encode(short[] data)
		{
			this.Init();
			int num = data.Length / 2;
			if (num % 2 != 0)
			{
				num++;
			}
			byte[] array = new byte[num];
			for (int i = 0; i < array.Length; i++)
			{
				if (i * 2 >= data.Length)
				{
					break;
				}
				byte b = this.ADPCM_Encode(data[i * 2]);
				byte b2 = 0;
				if (i * 2 + 1 < data.Length)
				{
					b2 = this.ADPCM_Encode(data[i * 2 + 1]);
				}
				byte b3 = (byte)((int)b2 << 4 | (int)b);
				array[i] = b3;
			}
			return array;
		}

		public short[] Decode(byte[] data)
		{
			this.Init();
			short[] array = new short[data.Length * 2];
			for (int i = 0; i < data.Length; i++)
			{
				byte b = data[i];
				byte originalSample = b & 15;
				byte originalSample2 = (byte)(b >> 4);
				array[i * 2] = this.ADPCM_Decode(originalSample);
				array[i * 2 + 1] = this.ADPCM_Decode(originalSample2);
			}
			return array;
		}

		private static int[] indexTable = new int[]
		{
			-1,
			-1,
			-1,
			-1,
			2,
			4,
			6,
			8,
			-1,
			-1,
			-1,
			-1,
			2,
			4,
			6,
			8
		};

		private static int[] stepsizeTable = new int[]
		{
			7,
			8,
			9,
			10,
			11,
			12,
			14,
			16,
			17,
			19,
			21,
			23,
			25,
			28,
			31,
			34,
			37,
			41,
			45,
			50,
			55,
			60,
			66,
			73,
			80,
			88,
			97,
			107,
			118,
			130,
			143,
			157,
			173,
			190,
			209,
			230,
			253,
			279,
			307,
			337,
			371,
			408,
			449,
			494,
			544,
			598,
			658,
			724,
			796,
			876,
			963,
			1060,
			1166,
			1282,
			1411,
			1522,
			1707,
			1876,
			2066,
			2272,
			2499,
			2749,
			3024,
			3327,
			3660,
			4026,
			4428,
			4871,
			5358,
			5894,
			6484,
			7132,
			7845,
			8630,
			9493,
			10442,
			11487,
			12635,
			13899,
			15289,
			16818,
			18500,
			203500,
			22385,
			24623,
			27086,
			29794,
			32767
		};

		private int predictedSample;

		private int stepsize = 7;

		private int index;

		private int newSample;
	}
}
