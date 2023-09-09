using System;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredULong : IFormattable, IEquatable<ObscuredULong>
	{
		private ObscuredULong(ulong value)
		{
			this.currentCryptoKey = ObscuredULong.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0UL;
			this.inited = true;
		}

		public static void SetNewCryptoKey(ulong newKey)
		{
			ObscuredULong.cryptoKey = newKey;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredULong.cryptoKey)
			{
				this.hiddenValue = ObscuredULong.Encrypt(this.InternalDecrypt(), ObscuredULong.cryptoKey);
				this.currentCryptoKey = ObscuredULong.cryptoKey;
			}
		}

		public static ulong Encrypt(ulong value)
		{
			return ObscuredULong.Encrypt(value, 0UL);
		}

		public static ulong Decrypt(ulong value)
		{
			return ObscuredULong.Decrypt(value, 0UL);
		}

		public static ulong Encrypt(ulong value, ulong key)
		{
			if (key == 0UL)
			{
				return value ^ ObscuredULong.cryptoKey;
			}
			return value ^ key;
		}

		public static ulong Decrypt(ulong value, ulong key)
		{
			if (key == 0UL)
			{
				return value ^ ObscuredULong.cryptoKey;
			}
			return value ^ key;
		}

		public ulong GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(ulong encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.isRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private ulong InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredULong.cryptoKey;
				this.hiddenValue = ObscuredULong.Encrypt(0UL);
				this.fakeValue = 0UL;
				this.inited = true;
			}
			ulong key = ObscuredULong.cryptoKey;
			if (this.currentCryptoKey != ObscuredULong.cryptoKey)
			{
				key = this.currentCryptoKey;
			}
			ulong num = ObscuredULong.Decrypt(this.hiddenValue, key);
			if (ObscuredCheatingDetector.isRunning && this.fakeValue != 0UL && num != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredULong))
			{
				return false;
			}
			ObscuredULong obscuredULong = (ObscuredULong)obj;
			return this.hiddenValue == obscuredULong.hiddenValue;
		}

		public bool Equals(ObscuredULong obj)
		{
			return this.hiddenValue == obj.hiddenValue;
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

		public static implicit operator ObscuredULong(ulong value)
		{
			ObscuredULong result = new ObscuredULong(ObscuredULong.Encrypt(value));
			if (ObscuredCheatingDetector.isRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator ulong(ObscuredULong value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredULong operator ++(ObscuredULong input)
		{
			ulong value = input.InternalDecrypt() + 1UL;
			input.hiddenValue = ObscuredULong.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredULong operator --(ObscuredULong input)
		{
			ulong value = input.InternalDecrypt() - 1UL;
			input.hiddenValue = ObscuredULong.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		private static ulong cryptoKey = 444443UL;

		private ulong currentCryptoKey;

		private ulong hiddenValue;

		private ulong fakeValue;

		private bool inited;
	}
}
