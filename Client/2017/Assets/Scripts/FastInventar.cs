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

	public static FastInventar Parse(string type)
	{
		if (string.IsNullOrEmpty(type))
		{
			type = "0";
		}
		InventarType t;
		int n;
		if (type[0] == 'w')
		{
			t = InventarType.weapons;
			n = int.Parse(type.Substring(1));
		}
		else if (type[0] == 'q')
		{
			t = InventarType.weaponSkin;
			n = int.Parse(type.Substring(1));
		}
		else if (type[0] == 'i')
		{
			t = InventarType.items;
			n = int.Parse(type.Substring(1));
		}
		else if (type[0] == 'k')
		{
			t = InventarType.skins;
			n = int.Parse(type.Substring(1));
		}
		else if (type[0] == 'c')
		{
			t = InventarType.dressItems;
			n = int.Parse(type.Substring(1));
		}
		else
		{
			t = InventarType.items;
			n = int.Parse(type);
		}
		return new FastInventar((int)t, n);
	}

	public override string ToString()
	{
		string arg = "i";
		switch (this.Type)
		{
		case 4:
			arg = "w";
			break;
		case 5:
			arg = "q";
			break;
		case 7:
			arg = "s";
			break;
		case 8:
			arg = "k";
			break;
		case 9:
			arg = "c";
			break;
		}
		return arg + this.Num;
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
