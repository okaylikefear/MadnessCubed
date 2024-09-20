using System;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredSByte : IFormattable, IEquatable<ObscuredSByte>
	{
		private ObscuredSByte(sbyte value)
		{
			this.currentCryptoKey = ObscuredSByte.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0;
			this.inited = true;
		}

		public static void SetNewCryptoKey(sbyte newKey)
		{
			ObscuredSByte.cryptoKey = newKey;
		}

		public void ApplyNewCryptoKey()
		{
			if ((int)this.currentCryptoKey != (int)ObscuredSByte.cryptoKey)
			{
				this.hiddenValue = ObscuredSByte.EncryptDecrypt(this.InternalDecrypt(), ObscuredSByte.cryptoKey);
				this.currentCryptoKey = ObscuredSByte.cryptoKey;
			}
		}

		public static sbyte EncryptDecrypt(sbyte value)
		{
			return ObscuredSByte.EncryptDecrypt(value, 0);
		}

		public static sbyte EncryptDecrypt(sbyte value, sbyte key)
		{
			if ((int)key == 0)
			{
				return (sbyte)((int)value ^ (int)ObscuredSByte.cryptoKey);
			}
			return (sbyte)((int)value ^ (int)key);
		}

		public sbyte GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(sbyte encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.isRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private sbyte InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredSByte.cryptoKey;
				this.hiddenValue = ObscuredSByte.EncryptDecrypt(0);
				this.fakeValue = 0;
				this.inited = true;
			}
			sbyte key = ObscuredSByte.cryptoKey;
			if ((int)this.currentCryptoKey != (int)ObscuredSByte.cryptoKey)
			{
				key = this.currentCryptoKey;
			}
			sbyte b = ObscuredSByte.EncryptDecrypt(this.hiddenValue, key);
			if (ObscuredCheatingDetector.isRunning && (int)this.fakeValue != 0 && (int)b != (int)this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return b;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredSByte))
			{
				return false;
			}
			ObscuredSByte obscuredSByte = (ObscuredSByte)obj;
			return (int)this.hiddenValue == (int)obscuredSByte.hiddenValue;
		}

		public bool Equals(ObscuredSByte obj)
		{
			return (int)this.hiddenValue == (int)obj.hiddenValue;
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

		public static implicit operator ObscuredSByte(sbyte value)
		{
			ObscuredSByte result = new ObscuredSByte(ObscuredSByte.EncryptDecrypt(value));
			if (ObscuredCheatingDetector.isRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator sbyte(ObscuredSByte value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredSByte operator ++(ObscuredSByte input)
		{
			sbyte value = (sbyte)((int)input.InternalDecrypt() + 1);
			input.hiddenValue = ObscuredSByte.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredSByte operator --(ObscuredSByte input)
		{
			sbyte value = (sbyte)((int)input.InternalDecrypt() - 1);
			input.hiddenValue = ObscuredSByte.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		private static sbyte cryptoKey = 112;

		private sbyte currentCryptoKey;

		private sbyte hiddenValue;

		private sbyte fakeValue;

		private bool inited;
	}
}
