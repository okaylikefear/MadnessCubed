using System;

namespace kube.cheat
{
	public struct ObscuredIntAB : IFormattable, IEquatable<ObscuredIntAB>
	{
		private ObscuredIntAB(int value)
		{
			this.hiddenValueA = (~value & ObscuredIntAB.cryptoKey);
			this.hiddenValueB = (~value & ~ObscuredIntAB.cryptoKey);
			this.fakeValue = value;
		}

		public static int SetNewCryptoKey(int newKey)
		{
			ObscuredIntAB.cryptoKey = newKey;
			return ObscuredIntAB.cryptoKey;
		}

		public void ApplyNewCryptoKey()
		{
			int num = this.InternalDecrypt();
			this.hiddenValueA = (~num & ObscuredIntAB.cryptoKey);
			this.hiddenValueB = (~num & ~ObscuredIntAB.cryptoKey);
		}

		private int InternalDecrypt()
		{
			int num = ~(this.hiddenValueA | this.hiddenValueB);
			if (this.fakeValue == 0)
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
			if (!(obj is ObscuredIntAB))
			{
				return false;
			}
			ObscuredIntAB obscuredIntAB = (ObscuredIntAB)obj;
			return this.InternalDecrypt() == obscuredIntAB.InternalDecrypt();
		}

		public bool Equals(ObscuredIntAB obj)
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

		public static implicit operator ObscuredIntAB(int value)
		{
			ObscuredIntAB result = new ObscuredIntAB(value);
			return result;
		}

		public static implicit operator int(ObscuredIntAB value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredIntAB operator ++(ObscuredIntAB input)
		{
			int num = input.InternalDecrypt() + 1;
			input.fakeValue = num;
			input.hiddenValueA = (~num & ObscuredIntAB.cryptoKey);
			input.hiddenValueB = (~num & ~ObscuredIntAB.cryptoKey);
			return input;
		}

		public static ObscuredIntAB operator --(ObscuredIntAB input)
		{
			int num = input.InternalDecrypt() - 1;
			input.fakeValue = num;
			input.hiddenValueA = (~num & ObscuredIntAB.cryptoKey);
			input.hiddenValueB = (~num & ~ObscuredIntAB.cryptoKey);
			return input;
		}

		private static int cryptoKey = ObscuredIntAB.SetNewCryptoKey(557573);

		private int fakeValue;

		private int hiddenValueB;

		private int hiddenValueA;
	}
}
