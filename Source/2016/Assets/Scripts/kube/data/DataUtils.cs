using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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

		public static object Clone(object obj)
		{
			object obj2 = Activator.CreateInstance(obj.GetType());
			FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				object obj3 = fieldInfo.GetValue(obj);
				if (obj3 is Array)
				{
					obj3 = ((ICloneable)obj3).Clone();
				}
				fieldInfo.SetValue(obj2, obj3);
			}
			return obj2;
		}

		public static int IntParseFast(string value)
		{
			int num = 0;
			foreach (char c in value)
			{
				if (c <= ':' && c >= '0')
				{
					num = 10 * num + (int)(c - '0');
				}
			}
			return num;
		}

		public static int TimeAdd(int obscuredIntAB, int p)
		{
			if (obscuredIntAB == 1)
			{
				return obscuredIntAB;
			}
			if (obscuredIntAB <= 0)
			{
				obscuredIntAB = (int)Time.time;
			}
			obscuredIntAB += p;
			return obscuredIntAB;
		}

		public static uint TimeAdd(uint obscuredIntAB, uint p)
		{
			if (obscuredIntAB == 1u)
			{
				return obscuredIntAB;
			}
			if (obscuredIntAB <= 0u)
			{
				obscuredIntAB = (uint)Time.time;
			}
			obscuredIntAB += p;
			return obscuredIntAB;
		}
	}
}
