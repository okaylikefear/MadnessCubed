using System;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredUInt : IFormattable, IEquatable<ObscuredUInt>
	{
		private ObscuredUInt(uint value)
		{
			this.currentCryptoKey = ObscuredUInt.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0u;
			this.inited = true;
		}

		public static void SetNewCryptoKey(uint newKey)
		{
			ObscuredUInt.cryptoKey = newKey;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredUInt.cryptoKey)
			{
				this.hiddenValue = ObscuredUInt.Encrypt(this.InternalDecrypt(), ObscuredUInt.cryptoKey);
				this.currentCryptoKey = ObscuredUInt.cryptoKey;
			}
		}

		public static uint Encrypt(uint value)
		{
			return ObscuredUInt.Encrypt(value, 0u);
		}

		public static uint Decrypt(uint value)
		{
			return ObscuredUInt.Decrypt(value, 0u);
		}

		public static uint Encrypt(uint value, uint key)
		{
			if (key == 0u)
			{
				return value ^ ObscuredUInt.cryptoKey;
			}
			return value ^ key;
		}

		public static uint Decrypt(uint value, uint key)
		{
			if (key == 0u)
			{
				return value ^ ObscuredUInt.cryptoKey;
			}
			return value ^ key;
		}

		public uint GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(uint encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.isRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private uint InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredUInt.cryptoKey;
				this.hiddenValue = ObscuredUInt.Encrypt(0u);
				this.fakeValue = 0u;
				this.inited = true;
			}
			uint key = ObscuredUInt.cryptoKey;
			if (this.currentCryptoKey != ObscuredUInt.cryptoKey)
			{
				key = this.currentCryptoKey;
			}
			uint num = ObscuredUInt.Decrypt(this.hiddenValue, key);
			if (ObscuredCheatingDetector.isRunning && this.fakeValue != 0u && num != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredUInt))
			{
				return false;
			}
			ObscuredUInt obscuredUInt = (ObscuredUInt)obj;
			return this.hiddenValue == obscuredUInt.hiddenValue;
		}

		public bool Equals(ObscuredUInt obj)
		{
			return this.hiddenValue == obj.hiddenValue;
		}

		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		public static implicit operator ObscuredUInt(uint value)
		{
			ObscuredUInt result = new ObscuredUInt(ObscuredUInt.Encrypt(value));
			if (ObscuredCheatingDetector.isRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator uint(ObscuredUInt value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredUInt operator ++(ObscuredUInt input)
		{
			uint value = input.InternalDecrypt() + 1u;
			input.hiddenValue = ObscuredUInt.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredUInt operator --(ObscuredUInt input)
		{
			uint value = input.InternalDecrypt() - 1u;
			input.hiddenValue = ObscuredUInt.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		private static uint cryptoKey = 240513u;

		private uint currentCryptoKey;

		private uint hiddenValue;

		private uint fakeValue;

		private bool inited;
	}
}
