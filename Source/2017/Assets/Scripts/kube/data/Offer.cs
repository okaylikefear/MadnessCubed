using System;

namespace kube.data
{
	public class Offer
	{
		public virtual void parse(string par1)
		{
			char[] separator = new char[]
			{
				'='
			};
			string[] array = par1.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(separator);
				if (array2[0][0] == 'c')
				{
				}
			}
		}

		public int type;

		public DateTime expire;

		public int expireSeconds;
	}
}
