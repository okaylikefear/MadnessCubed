using System;

namespace kube.cheat
{
	public struct ObscuredUIntAB : IFormattable, IEquatable<ObscuredUIntAB>
	{
		private ObscuredUIntAB(uint value)
		{
			this.hiddenValueA = (~value & ObscuredUIntAB.cryptoKey);
			this.hiddenValueB = (~value & ~ObscuredUIntAB.cryptoKey);
			this.fakeValue = value;
		}

		public static uint SetNewCryptoKey(uint newKey)
		{
			ObscuredUIntAB.cryptoKey = newKey;
			return ObscuredUIntAB.cryptoKey;
		}

		public void ApplyNewCryptoKey()
		{
			uint num = this.InternalDecrypt();
			this.hiddenValueA = (~num & ObscuredUIntAB.cryptoKey);
			this.hiddenValueB = (~num & ~ObscuredUIntAB.cryptoKey);
		}

		private uint InternalDecrypt()
		{
			uint num = ~(this.hiddenValueA | this.hiddenValueB);
			if (this.fakeValue == 0u)
			{
				return this.fakeValue;
			}
			if (num != this.fakeValue)
			{
				Kube.Ban();
			}
			return this.fakeValue;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredUIntAB))
			{
				return false;
			}
			ObscuredUIntAB obscuredUIntAB = (ObscuredUIntAB)obj;
			return this.InternalDecrypt() == obscuredUIntAB.InternalDecrypt();
		}

		public bool Equals(ObscuredUIntAB obj)
		{
			return this.InternalDecrypt() == obj.InternalDecrypt();
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

		public static implicit operator ObscuredUIntAB(uint value)
		{
			ObscuredUIntAB result = new ObscuredUIntAB(value);
			return result;
		}

		public static implicit operator uint(ObscuredUIntAB value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredUIntAB operator ++(ObscuredUIntAB input)
		{
			uint num = input.InternalDecrypt() + 1u;
			input.fakeValue = num;
			input.hiddenValueA = (~num & ObscuredUIntAB.cryptoKey);
			input.hiddenValueB = (~num & ~ObscuredUIntAB.cryptoKey);
			return input;
		}

		public static ObscuredUIntAB operator --(ObscuredUIntAB input)
		{
			uint num = input.InternalDecrypt() - 1u;
			input.fakeValue = num;
			input.hiddenValueA = (~num & ObscuredUIntAB.cryptoKey);
			input.hiddenValueB = (~num & ~ObscuredUIntAB.cryptoKey);
			return input;
		}

		private static uint cryptoKey = ObscuredUIntAB.SetNewCryptoKey(555737u);

		private uint fakeValue;

		private uint hiddenValueA;

		private uint hiddenValueB;
	}
}
