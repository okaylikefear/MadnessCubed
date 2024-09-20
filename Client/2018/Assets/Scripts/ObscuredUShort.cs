using System;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredUShort : IFormattable, IEquatable<ObscuredUShort>
	{
		private ObscuredUShort(ushort value)
		{
			this.currentCryptoKey = ObscuredUShort.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0;
			this.inited = true;
		}

		public static void SetNewCryptoKey(ushort newKey)
		{
			ObscuredUShort.cryptoKey = newKey;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredUShort.cryptoKey)
			{
				this.hiddenValue = ObscuredUShort.EncryptDecrypt(this.InternalDecrypt(), ObscuredUShort.cryptoKey);
				this.currentCryptoKey = ObscuredUShort.cryptoKey;
			}
		}

		public static ushort EncryptDecrypt(ushort value)
		{
			return ObscuredUShort.EncryptDecrypt(value, 0);
		}

		public static ushort EncryptDecrypt(ushort value, ushort key)
		{
			if (key == 0)
			{
				return value ^ ObscuredUShort.cryptoKey;
			}
			return value ^ key;
		}

		public ushort GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(ushort encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.isRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private ushort InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredUShort.cryptoKey;
				this.hiddenValue = ObscuredUShort.EncryptDecrypt(0);
				this.fakeValue = 0;
				this.inited = true;
			}
			ushort key = ObscuredUShort.cryptoKey;
			if (this.currentCryptoKey != ObscuredUShort.cryptoKey)
			{
				key = this.currentCryptoKey;
			}
			ushort num = ObscuredUShort.EncryptDecrypt(this.hiddenValue, key);
			if (ObscuredCheatingDetector.isRunning && this.fakeValue != 0 && num != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredUShort))
			{
				return false;
			}
			ObscuredUShort obscuredUShort = (ObscuredUShort)obj;
			return this.hiddenValue == obscuredUShort.hiddenValue;
		}

		public bool Equals(ObscuredUShort obj)
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

		public static implicit operator ObscuredUShort(ushort value)
		{
			ObscuredUShort result = new ObscuredUShort(ObscuredUShort.EncryptDecrypt(value));
			if (ObscuredCheatingDetector.isRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator ushort(ObscuredUShort value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredUShort operator ++(ObscuredUShort input)
		{
			ushort value = input.InternalDecrypt() + 1;
			input.hiddenValue = ObscuredUShort.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredUShort operator --(ObscuredUShort input)
		{
			ushort value = input.InternalDecrypt() - 1;
			input.hiddenValue = ObscuredUShort.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		private static ushort cryptoKey = 224;

		private ushort currentCryptoKey;

		private ushort hiddenValue;

		private ushort fakeValue;

		private bool inited;
	}
}
