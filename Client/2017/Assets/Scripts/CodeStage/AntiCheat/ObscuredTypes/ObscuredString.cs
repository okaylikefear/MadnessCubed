using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public sealed class ObscuredString
	{
		private ObscuredString()
		{
		}

		private ObscuredString(byte[] value)
		{
			this.currentCryptoKey = ObscuredString.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = null;
			this.inited = true;
		}

		public static void SetNewCryptoKey(string newKey)
		{
			ObscuredString.cryptoKey = newKey;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredString.cryptoKey)
			{
				this.hiddenValue = ObscuredString.InternalEncrypt(this.InternalDecrypt());
				this.currentCryptoKey = ObscuredString.cryptoKey;
			}
		}

		public static string EncryptDecrypt(string value)
		{
			return ObscuredString.EncryptDecrypt(value, string.Empty);
		}

		public static string EncryptDecrypt(string value, string key)
		{
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			if (string.IsNullOrEmpty(key))
			{
				key = ObscuredString.cryptoKey;
			}
			int length = key.Length;
			int length2 = value.Length;
			char[] array = new char[length2];
			for (int i = 0; i < length2; i++)
			{
				array[i] = (value[i] ^ key[i % length]);
			}
			return new string(array);
		}

		public string GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return ObscuredString.GetString(this.hiddenValue);
		}

		public void SetEncrypted(string encrypted)
		{
			this.inited = true;
			this.hiddenValue = ObscuredString.GetBytes(encrypted);
			if (ObscuredCheatingDetector.isRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private static byte[] InternalEncrypt(string value)
		{
			return ObscuredString.GetBytes(ObscuredString.EncryptDecrypt(value, ObscuredString.cryptoKey));
		}

		private string InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredString.cryptoKey;
				this.hiddenValue = ObscuredString.InternalEncrypt(string.Empty);
				this.fakeValue = string.Empty;
				this.inited = true;
			}
			string text = ObscuredString.cryptoKey;
			if (this.currentCryptoKey != ObscuredString.cryptoKey)
			{
				text = this.currentCryptoKey;
			}
			if (string.IsNullOrEmpty(text))
			{
				text = ObscuredString.cryptoKey;
			}
			string text2 = ObscuredString.EncryptDecrypt(ObscuredString.GetString(this.hiddenValue), text);
			if (ObscuredCheatingDetector.isRunning && !string.IsNullOrEmpty(this.fakeValue) && text2 != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return text2;
		}

		public override string ToString()
		{
			return this.InternalDecrypt();
		}

		public override bool Equals(object obj)
		{
			ObscuredString obscuredString = obj as ObscuredString;
			string objB = null;
			if (obscuredString != null)
			{
				objB = ObscuredString.GetString(obscuredString.hiddenValue);
			}
			return object.Equals(this.hiddenValue, objB);
		}

		public bool Equals(ObscuredString value)
		{
			byte[] a = null;
			if (value != null)
			{
				a = value.hiddenValue;
			}
			return ObscuredString.ArraysEquals(this.hiddenValue, a);
		}

		public bool Equals(ObscuredString value, StringComparison comparisonType)
		{
			string b = null;
			if (value != null)
			{
				b = value.InternalDecrypt();
			}
			return string.Equals(this.InternalDecrypt(), b, comparisonType);
		}

		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		private static byte[] GetBytes(string str)
		{
			byte[] array = new byte[str.Length * 2];
			Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
			return array;
		}

		private static string GetString(byte[] bytes)
		{
			char[] array = new char[bytes.Length / 2];
			Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
			return new string(array);
		}

		private static bool ArraysEquals(byte[] a1, byte[] a2)
		{
			if (a1 == a2)
			{
				return true;
			}
			if (a1 == null || a2 == null)
			{
				return false;
			}
			if (a1.Length != a2.Length)
			{
				return false;
			}
			for (int i = 0; i < a1.Length; i++)
			{
				if (a1[i] != a2[i])
				{
					return false;
				}
			}
			return true;
		}

		public static implicit operator ObscuredString(string value)
		{
			if (value == null)
			{
				return null;
			}
			ObscuredString obscuredString = new ObscuredString(ObscuredString.InternalEncrypt(value));
			if (ObscuredCheatingDetector.isRunning)
			{
				obscuredString.fakeValue = value;
			}
			return obscuredString;
		}

		public static implicit operator string(ObscuredString value)
		{
			if (value == null)
			{
				return null;
			}
			return value.InternalDecrypt();
		}

		public static bool operator ==(ObscuredString a, ObscuredString b)
		{
			return object.ReferenceEquals(a, b) || (a != null && b != null && ObscuredString.ArraysEquals(a.hiddenValue, b.hiddenValue));
		}

		public static bool operator !=(ObscuredString a, ObscuredString b)
		{
			return !(a == b);
		}

		private static string cryptoKey = "4441";

		[SerializeField]
		private string currentCryptoKey;

		[SerializeField]
		private byte[] hiddenValue;

		[SerializeField]
		private string fakeValue;

		[SerializeField]
		private bool inited;
	}
}
