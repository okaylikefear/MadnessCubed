using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace kube.data
{
	public class LocalizeUtils
	{
		public static int IntParseFast(string value)
		{
			int num = 0;
			foreach (char c in value)
			{
				if (c == ']')
				{
					break;
				}
				num = 10 * num + (int)(c - '0');
			}
			return num;
		}

		private static string extractString(string str)
		{
			int length = str.Length;
			if (str == string.Empty)
			{
				return str;
			}
			if (str[0] == '"')
			{
				str = str.Substring(1, length - 2);
				str = str.Replace("\"\"", "\"");
			}
			return str;
		}

		public static void load(byte[] csv)
		{
			MemoryStream stream = new MemoryStream(csv);
			StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
			Type typeFromHandle = typeof(Localize);
			streamReader.ReadLine();
			while (!streamReader.EndOfStream)
			{
				string text = streamReader.ReadLine();
				int num = text.IndexOf(";");
				if (num != -1)
				{
					string[] array = new string[]
					{
						string.Empty,
						string.Empty
					};
					array[0] = LocalizeUtils.extractString(text.Substring(0, num));
					array[1] = LocalizeUtils.extractString(text.Substring(num + 1));
					int num2 = array[0].IndexOf('[');
					int num3 = 0;
					array[1] = array[1].Replace("\\n", "\n");
					if (num2 != -1)
					{
						num3 = LocalizeUtils.IntParseFast(array[0].Substring(num2 + 1));
						array[0] = array[0].Substring(0, num2);
					}
					FieldInfo field = typeFromHandle.GetField(array[0]);
					if (field == null)
					{
						Localize.hash[array[0]] = array[1];
					}
					else if (field.FieldType == typeof(string[]))
					{
						string[] array2 = (string[])field.GetValue(null);
						if (array2.Length <= num3)
						{
							UnityEngine.Debug.Log(text);
						}
						if (array[1] != string.Empty)
						{
							array2[num3] = array[1];
						}
					}
					else
					{
						field.SetValue(null, array[1]);
					}
				}
			}
		}
	}
}
