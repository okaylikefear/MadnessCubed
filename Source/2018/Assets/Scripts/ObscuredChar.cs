using System;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredChar : IEquatable<ObscuredChar>
	{
		private ObscuredChar(char value)
		{
			this.currentCryptoKey = ObscuredChar.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = '\0';
			this.inited = true;
		}

		public static void SetNewCryptoKey(char newKey)
		{
			ObscuredChar.cryptoKey = newKey;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredChar.cryptoKey)
			{
				this.hiddenValue = ObscuredChar.EncryptDecrypt(this.InternalDecrypt(), ObscuredChar.cryptoKey);
				this.currentCryptoKey = ObscuredChar.cryptoKey;
			}
		}

		public static char EncryptDecrypt(char value)
		{
			return ObscuredChar.EncryptDecrypt(value, '\0');
		}

		public static char EncryptDecrypt(char value, char key)
		{
			if (key == '\0')
			{
				return value ^ ObscuredChar.cryptoKey;
			}
			return value ^ key;
		}

		public char GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(char encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.isRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private char InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredChar.cryptoKey;
				this.hiddenValue = ObscuredChar.EncryptDecrypt('\0');
				this.fakeValue = '\0';
				this.inited = true;
			}
			char key = ObscuredChar.cryptoKey;
			if (this.currentCryptoKey != ObscuredChar.cryptoKey)
			{
				key = this.currentCryptoKey;
			}
			char c = ObscuredChar.EncryptDecrypt(this.hiddenValue, key);
			if (ObscuredCheatingDetector.isRunning && this.fakeValue != '\0' && c != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return c;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredChar))
			{
				return false;
			}
			ObscuredChar obscuredChar = (ObscuredChar)obj;
			return this.hiddenValue == obscuredChar.hiddenValue;
		}

		public bool Equals(ObscuredChar obj)
		{
			return this.hiddenValue == obj.hiddenValue;
		}

		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		public static implicit operator ObscuredChar(char value)
		{
			ObscuredChar result = new ObscuredChar(ObscuredChar.EncryptDecrypt(value));
			if (ObscuredCheatingDetector.isRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator char(ObscuredChar value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredChar operator ++(ObscuredChar input)
		{
			char value = input.InternalDecrypt() + '\u0001';
			input.hiddenValue = ObscuredChar.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredChar operator --(ObscuredChar input)
		{
			char value = input.InternalDecrypt() - '\u0001';
			input.hiddenValue = ObscuredChar.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		private static char cryptoKey = '—';

		private char currentCryptoKey;

		private char hiddenValue;

		private char fakeValue;

		private bool inited;
	}
}
