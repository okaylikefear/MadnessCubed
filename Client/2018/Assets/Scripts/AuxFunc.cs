using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class AuxFunc
{
	public static void CopyTo(Stream src, Stream dest)
	{
		byte[] array = new byte[4096];
		int count;
		while ((count = src.Read(array, 0, array.Length)) != 0)
		{
			dest.Write(array, 0, count);
		}
	}

	public static byte[] Zip(string str)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(str);
		byte[] result;
		using (MemoryStream memoryStream = new MemoryStream(bytes))
		{
			using (MemoryStream memoryStream2 = new MemoryStream())
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream2, CompressionMode.Compress))
				{
					AuxFunc.CopyTo(memoryStream, gzipStream);
				}
				result = memoryStream2.ToArray();
			}
		}
		return result;
	}

	public static string Unzip(byte[] bytes)
	{
		string @string;
		using (MemoryStream memoryStream = new MemoryStream(bytes))
		{
			using (MemoryStream memoryStream2 = new MemoryStream())
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					AuxFunc.CopyTo(gzipStream, memoryStream2);
				}
				@string = Encoding.UTF8.GetString(memoryStream2.ToArray());
			}
		}
		return @string;
	}

	public static string GetMD5(byte[] input)
	{
		MD5 md = MD5.Create();
		byte[] array = md.ComputeHash(input);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	public static string GetMD5(string input)
	{
		MD5 md = MD5.Create();
		byte[] array = md.ComputeHash(Encoding.UTF8.GetBytes(input));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	public static string CodeRussianName(string name)
	{
		if (name.Length == 0)
		{
			return string.Empty;
		}
		char[] separator = new char[]
		{
			'_',
			'*',
			';',
			'^'
		};
		string[] array = name.Split(separator);
		name = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			if (i != 0)
			{
				name += string.Empty;
			}
			name += array[i];
		}
		string text = string.Empty;
		for (int j = 0; j < name.Length; j++)
		{
			if (name.Substring(j, 1) == "А")
			{
				text += "_00";
			}
			else if (name.Substring(j, 1) == "Б")
			{
				text += "_01";
			}
			else if (name.Substring(j, 1) == "В")
			{
				text += "_02";
			}
			else if (name.Substring(j, 1) == "Г")
			{
				text += "_03";
			}
			else if (name.Substring(j, 1) == "Д")
			{
				text += "_04";
			}
			else if (name.Substring(j, 1) == "Е")
			{
				text += "_05";
			}
			else if (name.Substring(j, 1) == "Ё")
			{
				text += "_06";
			}
			else if (name.Substring(j, 1) == "Ж")
			{
				text += "_07";
			}
			else if (name.Substring(j, 1) == "З")
			{
				text += "_08";
			}
			else if (name.Substring(j, 1) == "И")
			{
				text += "_09";
			}
			else if (name.Substring(j, 1) == "Й")
			{
				text += "_10";
			}
			else if (name.Substring(j, 1) == "К")
			{
				text += "_11";
			}
			else if (name.Substring(j, 1) == "Л")
			{
				text += "_12";
			}
			else if (name.Substring(j, 1) == "М")
			{
				text += "_13";
			}
			else if (name.Substring(j, 1) == "Н")
			{
				text += "_14";
			}
			else if (name.Substring(j, 1) == "О")
			{
				text += "_15";
			}
			else if (name.Substring(j, 1) == "П")
			{
				text += "_16";
			}
			else if (name.Substring(j, 1) == "Р")
			{
				text += "_17";
			}
			else if (name.Substring(j, 1) == "С")
			{
				text += "_18";
			}
			else if (name.Substring(j, 1) == "Т")
			{
				text += "_19";
			}
			else if (name.Substring(j, 1) == "У")
			{
				text += "_20";
			}
			else if (name.Substring(j, 1) == "Ф")
			{
				text += "_21";
			}
			else if (name.Substring(j, 1) == "Х")
			{
				text += "_22";
			}
			else if (name.Substring(j, 1) == "Ц")
			{
				text += "_23";
			}
			else if (name.Substring(j, 1) == "Ч")
			{
				text += "_24";
			}
			else if (name.Substring(j, 1) == "Ш")
			{
				text += "_25";
			}
			else if (name.Substring(j, 1) == "Щ")
			{
				text += "_26";
			}
			else if (name.Substring(j, 1) == "Ъ")
			{
				text += "_27";
			}
			else if (name.Substring(j, 1) == "Ы")
			{
				text += "_28";
			}
			else if (name.Substring(j, 1) == "Ь")
			{
				text += "_29";
			}
			else if (name.Substring(j, 1) == "Э")
			{
				text += "_30";
			}
			else if (name.Substring(j, 1) == "Ю")
			{
				text += "_31";
			}
			else if (name.Substring(j, 1) == "Я")
			{
				text += "_32";
			}
			else if (name.Substring(j, 1) == "а")
			{
				text += "_40";
			}
			else if (name.Substring(j, 1) == "б")
			{
				text += "_41";
			}
			else if (name.Substring(j, 1) == "в")
			{
				text += "_42";
			}
			else if (name.Substring(j, 1) == "г")
			{
				text += "_43";
			}
			else if (name.Substring(j, 1) == "д")
			{
				text += "_44";
			}
			else if (name.Substring(j, 1) == "е")
			{
				text += "_45";
			}
			else if (name.Substring(j, 1) == "ё")
			{
				text += "_46";
			}
			else if (name.Substring(j, 1) == "ж")
			{
				text += "_47";
			}
			else if (name.Substring(j, 1) == "з")
			{
				text += "_48";
			}
			else if (name.Substring(j, 1) == "и")
			{
				text += "_49";
			}
			else if (name.Substring(j, 1) == "й")
			{
				text += "_50";
			}
			else if (name.Substring(j, 1) == "к")
			{
				text += "_51";
			}
			else if (name.Substring(j, 1) == "л")
			{
				text += "_52";
			}
			else if (name.Substring(j, 1) == "м")
			{
				text += "_53";
			}
			else if (name.Substring(j, 1) == "н")
			{
				text += "_54";
			}
			else if (name.Substring(j, 1) == "о")
			{
				text += "_55";
			}
			else if (name.Substring(j, 1) == "п")
			{
				text += "_56";
			}
			else if (name.Substring(j, 1) == "р")
			{
				text += "_57";
			}
			else if (name.Substring(j, 1) == "с")
			{
				text += "_58";
			}
			else if (name.Substring(j, 1) == "т")
			{
				text += "_59";
			}
			else if (name.Substring(j, 1) == "у")
			{
				text += "_60";
			}
			else if (name.Substring(j, 1) == "ф")
			{
				text += "_61";
			}
			else if (name.Substring(j, 1) == "х")
			{
				text += "_62";
			}
			else if (name.Substring(j, 1) == "ц")
			{
				text += "_63";
			}
			else if (name.Substring(j, 1) == "ч")
			{
				text += "_64";
			}
			else if (name.Substring(j, 1) == "ш")
			{
				text += "_65";
			}
			else if (name.Substring(j, 1) == "щ")
			{
				text += "_66";
			}
			else if (name.Substring(j, 1) == "ъ")
			{
				text += "_67";
			}
			else if (name.Substring(j, 1) == "ы")
			{
				text += "_68";
			}
			else if (name.Substring(j, 1) == "ь")
			{
				text += "_69";
			}
			else if (name.Substring(j, 1) == "э")
			{
				text += "_70";
			}
			else if (name.Substring(j, 1) == "ю")
			{
				text += "_71";
			}
			else if (name.Substring(j, 1) == "я")
			{
				text += "_72";
			}
			else if (name.Substring(j, 1) == " ")
			{
				text += "_73";
			}
			else
			{
				text += name[j];
			}
		}
		return text;
	}

	public static string DecodeRussianName(string name)
	{
		string text = string.Empty;
		if (name == null)
		{
			return string.Empty;
		}
		for (int i = 0; i < name.Length; i++)
		{
			if (name[i] == '_' && name.Length > i + 2)
			{
				if (name[i + 1] == '0' && name[i + 2] == '0')
				{
					text += "А";
				}
				else if (name[i + 1] == '0' && name[i + 2] == '1')
				{
					text += "Б";
				}
				else if (name[i + 1] == '0' && name[i + 2] == '2')
				{
					text += "В";
				}
				else if (name[i + 1] == '0' && name[i + 2] == '3')
				{
					text += "Г";
				}
				else if (name[i + 1] == '0' && name[i + 2] == '4')
				{
					text += "Д";
				}
				else if (name[i + 1] == '0' && name[i + 2] == '5')
				{
					text += "Е";
				}
				else if (name[i + 1] == '0' && name[i + 2] == '6')
				{
					text += "Ё";
				}
				else if (name[i + 1] == '0' && name[i + 2] == '7')
				{
					text += "Ж";
				}
				else if (name[i + 1] == '0' && name[i + 2] == '8')
				{
					text += "З";
				}
				else if (name[i + 1] == '0' && name[i + 2] == '9')
				{
					text += "И";
				}
				else if (name[i + 1] == '1' && name[i + 2] == '0')
				{
					text += "Й";
				}
				else if (name[i + 1] == '1' && name[i + 2] == '1')
				{
					text += "К";
				}
				else if (name[i + 1] == '1' && name[i + 2] == '2')
				{
					text += "Л";
				}
				else if (name[i + 1] == '1' && name[i + 2] == '3')
				{
					text += "М";
				}
				else if (name[i + 1] == '1' && name[i + 2] == '4')
				{
					text += "Н";
				}
				else if (name[i + 1] == '1' && name[i + 2] == '5')
				{
					text += "О";
				}
				else if (name[i + 1] == '1' && name[i + 2] == '6')
				{
					text += "П";
				}
				else if (name[i + 1] == '1' && name[i + 2] == '7')
				{
					text += "Р";
				}
				else if (name[i + 1] == '1' && name[i + 2] == '8')
				{
					text += "С";
				}
				else if (name[i + 1] == '1' && name[i + 2] == '9')
				{
					text += "Т";
				}
				else if (name[i + 1] == '2' && name[i + 2] == '0')
				{
					text += "У";
				}
				else if (name[i + 1] == '2' && name[i + 2] == '1')
				{
					text += "Ф";
				}
				else if (name[i + 1] == '2' && name[i + 2] == '2')
				{
					text += "Х";
				}
				else if (name[i + 1] == '2' && name[i + 2] == '3')
				{
					text += "Ц";
				}
				else if (name[i + 1] == '2' && name[i + 2] == '4')
				{
					text += "Ч";
				}
				else if (name[i + 1] == '2' && name[i + 2] == '5')
				{
					text += "Ш";
				}
				else if (name[i + 1] == '2' && name[i + 2] == '6')
				{
					text += "Щ";
				}
				else if (name[i + 1] == '2' && name[i + 2] == '7')
				{
					text += "Ъ";
				}
				else if (name[i + 1] == '2' && name[i + 2] == '8')
				{
					text += "Ы";
				}
				else if (name[i + 1] == '2' && name[i + 2] == '9')
				{
					text += "Ь";
				}
				else if (name[i + 1] == '3' && name[i + 2] == '0')
				{
					text += "Э";
				}
				else if (name[i + 1] == '3' && name[i + 2] == '1')
				{
					text += "Ю";
				}
				else if (name[i + 1] == '3' && name[i + 2] == '2')
				{
					text += "Я";
				}
				else if (name[i + 1] == '4' && name[i + 2] == '0')
				{
					text += "а";
				}
				else if (name[i + 1] == '4' && name[i + 2] == '1')
				{
					text += "б";
				}
				else if (name[i + 1] == '4' && name[i + 2] == '2')
				{
					text += "в";
				}
				else if (name[i + 1] == '4' && name[i + 2] == '3')
				{
					text += "г";
				}
				else if (name[i + 1] == '4' && name[i + 2] == '4')
				{
					text += "д";
				}
				else if (name[i + 1] == '4' && name[i + 2] == '5')
				{
					text += "е";
				}
				else if (name[i + 1] == '4' && name[i + 2] == '6')
				{
					text += "ё";
				}
				else if (name[i + 1] == '4' && name[i + 2] == '7')
				{
					text += "ж";
				}
				else if (name[i + 1] == '4' && name[i + 2] == '8')
				{
					text += "з";
				}
				else if (name[i + 1] == '4' && name[i + 2] == '9')
				{
					text += "и";
				}
				else if (name[i + 1] == '5' && name[i + 2] == '0')
				{
					text += "й";
				}
				else if (name[i + 1] == '5' && name[i + 2] == '1')
				{
					text += "к";
				}
				else if (name[i + 1] == '5' && name[i + 2] == '2')
				{
					text += "л";
				}
				else if (name[i + 1] == '5' && name[i + 2] == '3')
				{
					text += "м";
				}
				else if (name[i + 1] == '5' && name[i + 2] == '4')
				{
					text += "н";
				}
				else if (name[i + 1] == '5' && name[i + 2] == '5')
				{
					text += "о";
				}
				else if (name[i + 1] == '5' && name[i + 2] == '6')
				{
					text += "п";
				}
				else if (name[i + 1] == '5' && name[i + 2] == '7')
				{
					text += "р";
				}
				else if (name[i + 1] == '5' && name[i + 2] == '8')
				{
					text += "с";
				}
				else if (name[i + 1] == '5' && name[i + 2] == '9')
				{
					text += "т";
				}
				else if (name[i + 1] == '6' && name[i + 2] == '0')
				{
					text += "у";
				}
				else if (name[i + 1] == '6' && name[i + 2] == '1')
				{
					text += "ф";
				}
				else if (name[i + 1] == '6' && name[i + 2] == '2')
				{
					text += "х";
				}
				else if (name[i + 1] == '6' && name[i + 2] == '3')
				{
					text += "ц";
				}
				else if (name[i + 1] == '6' && name[i + 2] == '4')
				{
					text += "ч";
				}
				else if (name[i + 1] == '6' && name[i + 2] == '5')
				{
					text += "ш";
				}
				else if (name[i + 1] == '6' && name[i + 2] == '6')
				{
					text += "щ";
				}
				else if (name[i + 1] == '6' && name[i + 2] == '7')
				{
					text += "ъ";
				}
				else if (name[i + 1] == '6' && name[i + 2] == '8')
				{
					text += "ы";
				}
				else if (name[i + 1] == '6' && name[i + 2] == '9')
				{
					text += "ь";
				}
				else if (name[i + 1] == '7' && name[i + 2] == '0')
				{
					text += "э";
				}
				else if (name[i + 1] == '7' && name[i + 2] == '1')
				{
					text += "ю";
				}
				else if (name[i + 1] == '7' && name[i + 2] == '2')
				{
					text += "я";
				}
				else if (name[i + 1] == '7' && name[i + 2] == '3')
				{
					text += " ";
				}
				i += 2;
			}
			else
			{
				text += name[i];
			}
		}
		return text;
	}

	public static int RandomSelectWithChance(int[] arr)
	{
		int num = 0;
		int[] array = new int[arr.Length];
		int[] array2 = new int[arr.Length];
		for (int i = 0; i < arr.Length; i++)
		{
			num += arr[i];
		}
		for (int j = 0; j < arr.Length; j++)
		{
			if (arr[j] != 0)
			{
				array[j] = num - arr[j];
			}
		}
		num = 0;
		string arg = "RamdomTeams = {";
		for (int k = 0; k < arr.Length; k++)
		{
			num += array[k];
			array2[k] = num;
			arg = arg + " " + array2[k];
		}
		arg = arg + " } sum=" + num;
		float num2 = UnityEngine.Random.Range(0f, (float)num);
		for (int l = 0; l < 4; l++)
		{
			if (arr[l] > 0)
			{
				float num3 = (float)((l != 0) ? array2[l - 1] : 0);
				float num4 = (float)array2[l];
				if (num2 >= num3 && num2 <= num4)
				{
					return l;
				}
			}
		}
		return 0;
	}
}
