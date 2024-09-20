using System;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredDouble : IFormattable, IEquatable<ObscuredDouble>
	{
		private ObscuredDouble(byte[] value)
		{
			this.currentCryptoKey = ObscuredDouble.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0.0;
			this.inited = true;
		}

		public static void SetNewCryptoKey(long newKey)
		{
			ObscuredDouble.cryptoKey = newKey;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredDouble.cryptoKey)
			{
				this.hiddenValue = ObscuredDouble.InternalEncrypt(this.InternalDecrypt(), ObscuredDouble.cryptoKey);
				this.currentCryptoKey = ObscuredDouble.cryptoKey;
			}
		}

		public static long Encrypt(double value)
		{
			return ObscuredDouble.Encrypt(value, ObscuredDouble.cryptoKey);
		}

		public static long Encrypt(double value, long key)
		{
			ObscuredDouble.DoubleLongBytesUnion doubleLongBytesUnion = default(ObscuredDouble.DoubleLongBytesUnion);
			doubleLongBytesUnion.d = value;
			doubleLongBytesUnion.l ^= key;
			return doubleLongBytesUnion.l;
		}

		private static byte[] InternalEncrypt(double value)
		{
			return ObscuredDouble.InternalEncrypt(value, 0L);
		}

		private static byte[] InternalEncrypt(double value, long key)
		{
			long num = key;
			if (num == 0L)
			{
				num = ObscuredDouble.cryptoKey;
			}
			ObscuredDouble.DoubleLongBytesUnion doubleLongBytesUnion = default(ObscuredDouble.DoubleLongBytesUnion);
			doubleLongBytesUnion.d = value;
			doubleLongBytesUnion.l ^= num;
			return new byte[]
			{
				doubleLongBytesUnion.b1,
				doubleLongBytesUnion.b2,
				doubleLongBytesUnion.b3,
				doubleLongBytesUnion.b4,
				doubleLongBytesUnion.b5,
				doubleLongBytesUnion.b6,
				doubleLongBytesUnion.b7,
				doubleLongBytesUnion.b8
			};
		}

		public static double Decrypt(long value)
		{
			return ObscuredDouble.Decrypt(value, ObscuredDouble.cryptoKey);
		}

		public static double Decrypt(long value, long key)
		{
			ObscuredDouble.DoubleLongBytesUnion doubleLongBytesUnion = default(ObscuredDouble.DoubleLongBytesUnion);
			doubleLongBytesUnion.l = (value ^ key);
			return doubleLongBytesUnion.d;
		}

		public long GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			ObscuredDouble.DoubleLongBytesUnion doubleLongBytesUnion = default(ObscuredDouble.DoubleLongBytesUnion);
			doubleLongBytesUnion.b1 = this.hiddenValue[0];
			doubleLongBytesUnion.b2 = this.hiddenValue[1];
			doubleLongBytesUnion.b3 = this.hiddenValue[2];
			doubleLongBytesUnion.b4 = this.hiddenValue[3];
			doubleLongBytesUnion.b5 = this.hiddenValue[4];
			doubleLongBytesUnion.b6 = this.hiddenValue[5];
			doubleLongBytesUnion.b7 = this.hiddenValue[6];
			doubleLongBytesUnion.b8 = this.hiddenValue[7];
			return doubleLongBytesUnion.l;
		}

		public void SetEncrypted(long encrypted)
		{
			this.inited = true;
			ObscuredDouble.DoubleLongBytesUnion doubleLongBytesUnion = default(ObscuredDouble.DoubleLongBytesUnion);
			doubleLongBytesUnion.l = encrypted;
			this.hiddenValue = new byte[]
			{
				doubleLongBytesUnion.b1,
				doubleLongBytesUnion.b2,
				doubleLongBytesUnion.b3,
				doubleLongBytesUnion.b4,
				doubleLongBytesUnion.b5,
				doubleLongBytesUnion.b6,
				doubleLongBytesUnion.b7,
				doubleLongBytesUnion.b8
			};
			if (ObscuredCheatingDetector.isRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private double InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredDouble.cryptoKey;
				this.hiddenValue = ObscuredDouble.InternalEncrypt(0.0);
				this.fakeValue = 0.0;
				this.inited = true;
			}
			long num = ObscuredDouble.cryptoKey;
			if (this.currentCryptoKey != ObscuredDouble.cryptoKey)
			{
				num = this.currentCryptoKey;
			}
			ObscuredDouble.DoubleLongBytesUnion doubleLongBytesUnion = default(ObscuredDouble.DoubleLongBytesUnion);
			doubleLongBytesUnion.b1 = this.hiddenValue[0];
			doubleLongBytesUnion.b2 = this.hiddenValue[1];
			doubleLongBytesUnion.b3 = this.hiddenValue[2];
			doubleLongBytesUnion.b4 = this.hiddenValue[3];
			doubleLongBytesUnion.b5 = this.hiddenValue[4];
			doubleLongBytesUnion.b6 = this.hiddenValue[5];
			doubleLongBytesUnion.b7 = this.hiddenValue[6];
			doubleLongBytesUnion.b8 = this.hiddenValue[7];
			doubleLongBytesUnion.l ^= num;
			double d = doubleLongBytesUnion.d;
			if (ObscuredCheatingDetector.isRunning && this.fakeValue != 0.0 && Math.Abs(d - this.fakeValue) > 1E-06)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return d;
		}

		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredDouble))
			{
				return false;
			}
			double num = ((ObscuredDouble)obj).InternalDecrypt();
			double num2 = this.InternalDecrypt();
			return num == num2 || (double.IsNaN(num) && double.IsNaN(num2));
		}

		public bool Equals(ObscuredDouble obj)
		{
			double num = obj.InternalDecrypt();
			double num2 = this.InternalDecrypt();
			return num == num2 || (double.IsNaN(num) && double.IsNaN(num2));
		}

		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		public static implicit operator ObscuredDouble(double value)
		{
			ObscuredDouble result = new ObscuredDouble(ObscuredDouble.InternalEncrypt(value));
			if (ObscuredCheatingDetector.isRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator double(ObscuredDouble value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredDouble operator ++(ObscuredDouble input)
		{
			double value = input.InternalDecrypt() + 1.0;
			input.hiddenValue = ObscuredDouble.InternalEncrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredDouble operator --(ObscuredDouble input)
		{
			double value = input.InternalDecrypt() - 1.0;
			input.hiddenValue = ObscuredDouble.InternalEncrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		private static long cryptoKey = 210987L;

		private long currentCryptoKey;

		private byte[] hiddenValue;

		private double fakeValue;

		private bool inited;

		[StructLayout(LayoutKind.Explicit)]
		private struct DoubleLongBytesUnion
		{
			[FieldOffset(0)]
			public double d;

			[FieldOffset(0)]
			public long l;

			[FieldOffset(0)]
			public byte b1;

			[FieldOffset(1)]
			public byte b2;

			[FieldOffset(2)]
			public byte b3;

			[FieldOffset(3)]
			public byte b4;

			[FieldOffset(4)]
			public byte b5;

			[FieldOffset(5)]
			public byte b6;

			[FieldOffset(6)]
			public byte b7;

			[FieldOffset(7)]
			public byte b8;
		}
	}
}
