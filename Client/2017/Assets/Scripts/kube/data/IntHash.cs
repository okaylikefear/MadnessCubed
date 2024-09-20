using System;
using System.Collections.Generic;

namespace kube.data
{
	public class IntHash : Dictionary<int, bool>
	{
		public IntHash(int x) : base(x)
		{
		}

		public IntHash()
		{
		}

		public new bool this[int key]
		{
			get
			{
				bool result = false;
				base.TryGetValue(key, out result);
				return result;
			}
			set
			{
				base[key] = value;
			}
		}

		public override string ToString()
		{
			List<string> list = new List<string>();
			foreach (int num in base.Keys)
			{
				list.Add(string.Concat(new object[]
				{
					"(",
					num.GetType(),
					")",
					num,
					"=(",
					num.GetType(),
					")",
					this[num]
				}));
			}
			return string.Join(", ", list.ToArray());
		}
	}
}
