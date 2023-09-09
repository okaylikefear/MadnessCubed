using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredInt : IFormattable, IEquatable<ObscuredInt>
	{
		private ObscuredInt(int value)
		{
			this.currentCryptoKey = ObscuredInt.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0;
			this.inited = true;
		}

		public static void SetNewCryptoKey(int newKey)
		{
			ObscuredInt.cryptoKey = newKey;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredInt.cryptoKey)
			{
				this.hiddenValue = ObscuredInt.Encrypt(this.InternalDecrypt(), ObscuredInt.cryptoKey);
				this.currentCryptoKey = ObscuredInt.cryptoKey;
			}
		}

		public static int Encrypt(int value)
		{
			return ObscuredInt.Encrypt(value, 0);
		}

		public static int Encrypt(int value, int key)
		{
			if (key == 0)
			{
				return value ^ ObscuredInt.cryptoKey;
			}
			return value ^ key;
		}

		public static int Decrypt(int value)
		{
			return ObscuredInt.Decrypt(value, 0);
		}

		public static int Decrypt(int value, int key)
		{
			if (key == 0)
			{
				return value ^ ObscuredInt.cryptoKey;
			}
			return value ^ key;
		}

		public int GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(int encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.isRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private int InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredInt.cryptoKey;
				this.hiddenValue = ObscuredInt.Encrypt(0);
				this.fakeValue = 0;
				this.inited = true;
			}
			int key = ObscuredInt.cryptoKey;
			if (this.currentCryptoKey != ObscuredInt.cryptoKey)
			{
				key = this.currentCryptoKey;
			}
			int num = ObscuredInt.Decrypt(this.hiddenValue, key);
			if (ObscuredCheatingDetector.isRunning && this.fakeValue != 0 && num != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredInt))
			{
				return false;
			}
			ObscuredInt obscuredInt = (ObscuredInt)obj;
			return this.hiddenValue == obscuredInt.hiddenValue;
		}

		public bool Equals(ObscuredInt obj)
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

		public static implicit operator ObscuredInt(int value)
		{
			ObscuredInt result = new ObscuredInt(ObscuredInt.Encrypt(value));
			if (ObscuredCheatingDetector.isRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator int(ObscuredInt value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredInt operator ++(ObscuredInt input)
		{
			int value = input.InternalDecrypt() + 1;
			input.hiddenValue = ObscuredInt.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredInt operator --(ObscuredInt input)
		{
			int value = input.InternalDecrypt() - 1;
			input.hiddenValue = ObscuredInt.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		private static int cryptoKey = 447444;

		[SerializeField]
		private int fakeValue;

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private int hiddenValue;

		[SerializeField]
		private bool inited;
	}
}
