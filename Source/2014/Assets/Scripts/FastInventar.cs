using System;

public struct FastInventar
{
	public FastInventar(int t = -1, int n = -1)
	{
		this.Type = t;
		this.Num = n;
	}

	public FastInventar(InventarType t, int n)
	{
		this.Type = (int)t;
		this.Num = n;
	}

	public override bool Equals(object obj)
	{
		FastInventar fastInventar = (FastInventar)obj;
		return this.Num != fastInventar.Num || this.Type != fastInventar.Type;
	}

	public override int GetHashCode()
	{
		return this.Num * 10 + this.Type;
	}

	public static bool operator ==(FastInventar a, FastInventar b)
	{
		return a.Num == b.Num && a.Type == b.Type;
	}

	public static bool operator !=(FastInventar a, FastInventar b)
	{
		return a.Num != b.Num || a.Type != b.Type;
	}

	public int Type;

	public int Num;

	public static FastInventar NONE = new FastInventar(-1, 0);
}
