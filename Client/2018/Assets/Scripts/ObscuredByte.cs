using System;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredByte : IEquatable<ObscuredByte>, IFormattable
	{
		private ObscuredByte(byte value)
		{
			this.currentCryptoKey = ObscuredByte.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0;
			this.inited = true;
		}

		public static void SetNewCryptoKey(byte newKey)
		{
			ObscuredByte.cryptoKey = newKey;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredByte.cryptoKey)
			{
				this.hiddenValue = ObscuredByte.EncryptDecrypt(this.InternalDecrypt(), ObscuredByte.cryptoKey);
				this.currentCryptoKey = ObscuredByte.cryptoKey;
			}
		}

		public static byte EncryptDecrypt(byte value)
		{
			return ObscuredByte.EncryptDecrypt(value, 0);
		}

		public static byte EncryptDecrypt(byte value, byte key)
		{
			if (key == 0)
			{
				return value ^ ObscuredByte.cryptoKey;
			}
			return value ^ key;
		}

		public byte GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(byte encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.isRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private byte InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredByte.cryptoKey;
				this.hiddenValue = ObscuredByte.EncryptDecrypt(0);
				this.fakeValue = 0;
				this.inited = true;
			}
			byte key = ObscuredByte.cryptoKey;
			if (this.currentCryptoKey != ObscuredByte.cryptoKey)
			{
				key = this.currentCryptoKey;
			}
			byte b = ObscuredByte.EncryptDecrypt(this.hiddenValue, key);
			if (ObscuredCheatingDetector.isRunning && this.fakeValue != 0 && b != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return b;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredByte))
			{
				return false;
			}
			ObscuredByte obscuredByte = (ObscuredByte)obj;
			return this.hiddenValue == obscuredByte.hiddenValue;
		}

		public bool Equals(ObscuredByte obj)
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

		public static implicit operator ObscuredByte(byte value)
		{
			ObscuredByte result = new ObscuredByte(ObscuredByte.EncryptDecrypt(value));
			if (ObscuredCheatingDetector.isRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator byte(ObscuredByte value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredByte operator ++(ObscuredByte input)
		{
			byte value = input.InternalDecrypt() + 1;
			input.hiddenValue = ObscuredByte.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredByte operator --(ObscuredByte input)
		{
			byte value = input.InternalDecrypt() - 1;
			input.hiddenValue = ObscuredByte.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		private static byte cryptoKey = 244;

		private byte currentCryptoKey;

		private byte hiddenValue;

		private byte fakeValue;

		private bool inited;
	}
}
