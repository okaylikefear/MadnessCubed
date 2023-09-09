using System;

public struct PriceValue
{
	public PriceValue(int price, bool gold = false)
	{
		this.prc = (uint)(price << 1 | ((!gold) ? 0 : 1));
	}

	public static PriceValue Parse(string ps)
	{
		bool flag = ps[ps.Length - 1] == 'g';
		if (flag)
		{
			ps = ps.Substring(0, ps.Length - 1);
		}
		return new PriceValue(int.Parse(ps), flag);
	}

	public int price
	{
		get
		{
			return (int)(this.prc >> 1);
		}
	}

	public bool isGold
	{
		get
		{
			return (this.prc & 1u) == 1u;
		}
	}

	private uint prc;
}
