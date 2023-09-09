using System;
using System.Collections.Generic;

namespace kube.data
{
	public class DataUtils
	{
		public static KeyValuePair<int, int>[] StringToKwp(string par1)
		{
			string[] array = par1.Split(new char[]
			{
				';'
			});
			int num = array.Length / 2;
			KeyValuePair<int, int>[] array2 = new KeyValuePair<int, int>[num];
			int i = 0;
			int num2 = 0;
			while (i < array.Length)
			{
				array2[num2] = new KeyValuePair<int, int>(int.Parse(array[i]), int.Parse(array[i + 1]));
				i += 2;
				num2++;
			}
			return array2;
		}
	}
}
