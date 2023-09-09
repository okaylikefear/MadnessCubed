using System;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredFloat : IEquatable<ObscuredFloat>, IFormattable
	{
		private ObscuredFloat(byte[] value)
		{
			this.currentCryptoKey = ObscuredFloat.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0f;
			this.inited = true;
		}

		public static void SetNewCryptoKey(int newKey)
		{
			ObscuredFloat.cryptoKey = newKey;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredFloat.cryptoKey)
			{
				this.hiddenValue = ObscuredFloat.InternalEncrypt(this.InternalDecrypt(), ObscuredFloat.cryptoKey);
				this.currentCryptoKey = ObscuredFloat.cryptoKey;
			}
		}

		public static int Encrypt(float value)
		{
			return ObscuredFloat.Encrypt(value, ObscuredFloat.cryptoKey);
		}

		public static int Encrypt(float value, int key)
		{
			ObscuredFloat.FloatIntBytesUnion floatIntBytesUnion = default(ObscuredFloat.FloatIntBytesUnion);
			floatIntBytesUnion.f = value;
			floatIntBytesUnion.i ^= key;
			return floatIntBytesUnion.i;
		}

		private static byte[] InternalEncrypt(float value)
		{
			return ObscuredFloat.InternalEncrypt(value, 0);
		}

		private static byte[] InternalEncrypt(float value, int key)
		{
			int num = key;
			if (num == 0)
			{
				num = ObscuredFloat.cryptoKey;
			}
			ObscuredFloat.FloatIntBytesUnion floatIntBytesUnion = default(ObscuredFloat.FloatIntBytesUnion);
			floatIntBytesUnion.f = value;
			floatIntBytesUnion.i ^= num;
			return new byte[]
			{
				floatIntBytesUnion.b1,
				floatIntBytesUnion.b2,
				floatIntBytesUnion.b3,
				floatIntBytesUnion.b4
			};
		}

		public static float Decrypt(int value)
		{
			return ObscuredFloat.Decrypt(value, ObscuredFloat.cryptoKey);
		}

		public static float Decrypt(int value, int key)
		{
			ObscuredFloat.FloatIntBytesUnion floatIntBytesUnion = default(ObscuredFloat.FloatIntBytesUnion);
			floatIntBytesUnion.i = (value ^ key);
			return floatIntBytesUnion.f;
		}

		public int GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			ObscuredFloat.FloatIntBytesUnion floatIntBytesUnion = default(ObscuredFloat.FloatIntBytesUnion);
			floatIntBytesUnion.b1 = this.hiddenValue[0];
			floatIntBytesUnion.b2 = this.hiddenValue[1];
			floatIntBytesUnion.b3 = this.hiddenValue[2];
			floatIntBytesUnion.b4 = this.hiddenValue[3];
			return floatIntBytesUnion.i;
		}

		public void SetEncrypted(int encrypted)
		{
			this.inited = true;
			ObscuredFloat.FloatIntBytesUnion floatIntBytesUnion = default(ObscuredFloat.FloatIntBytesUnion);
			floatIntBytesUnion.i = encrypted;
			this.hiddenValue = new byte[]
			{
				floatIntBytesUnion.b1,
				floatIntBytesUnion.b2,
				floatIntBytesUnion.b3,
				floatIntBytesUnion.b4
			};
			if (ObscuredCheatingDetector.isRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private float InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredFloat.cryptoKey;
				this.hiddenValue = ObscuredFloat.InternalEncrypt(0f);
				this.fakeValue = 0f;
				this.inited = true;
			}
			int num = ObscuredFloat.cryptoKey;
			if (this.currentCryptoKey != ObscuredFloat.cryptoKey)
			{
				num = this.currentCryptoKey;
			}
			ObscuredFloat.FloatIntBytesUnion floatIntBytesUnion = default(ObscuredFloat.FloatIntBytesUnion);
			floatIntBytesUnion.b1 = this.hiddenValue[0];
			floatIntBytesUnion.b2 = this.hiddenValue[1];
			floatIntBytesUnion.b3 = this.hiddenValue[2];
			floatIntBytesUnion.b4 = this.hiddenValue[3];
			floatIntBytesUnion.i ^= num;
			float f = floatIntBytesUnion.f;
			if (ObscuredCheatingDetector.isRunning && this.fakeValue != 0f && Math.Abs(f - this.fakeValue) > ObscuredCheatingDetector.Instance.floatEpsilon)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return f;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredFloat))
			{
				return false;
			}
			float num = ((ObscuredFloat)obj).InternalDecrypt();
			float num2 = this.InternalDecrypt();
			return (double)num == (double)num2 || (float.IsNaN(num) && float.IsNaN(num2));
		}

		public bool Equals(ObscuredFloat obj)
		{
			float num = obj.InternalDecrypt();
			float num2 = this.InternalDecrypt();
			return (double)num == (double)num2 || (float.IsNaN(num) && float.IsNaN(num2));
		}

		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
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

		public static implicit operator ObscuredFloat(float value)
		{
			ObscuredFloat result = new ObscuredFloat(ObscuredFloat.InternalEncrypt(value));
			if (ObscuredCheatingDetector.isRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator float(ObscuredFloat value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredFloat operator ++(ObscuredFloat input)
		{
			float value = input.InternalDecrypt() + 1f;
			input.hiddenValue = ObscuredFloat.InternalEncrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredFloat operator --(ObscuredFloat input)
		{
			float value = input.InternalDecrypt() - 1f;
			input.hiddenValue = ObscuredFloat.InternalEncrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		private static int cryptoKey = 230887;

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private byte[] hiddenValue;

		[SerializeField]
		private float fakeValue;

		[SerializeField]
		private bool inited;

		[StructLayout(LayoutKind.Explicit)]
		private struct FloatIntBytesUnion
		{
			[FieldOffset(0)]
			public float f;

			[FieldOffset(0)]
			public int i;

			[FieldOffset(0)]
			public byte b1;

			[FieldOffset(1)]
			public byte b2;

			[FieldOffset(2)]
			public byte b3;

			[FieldOffset(3)]
			public byte b4;
		}
	}
}
