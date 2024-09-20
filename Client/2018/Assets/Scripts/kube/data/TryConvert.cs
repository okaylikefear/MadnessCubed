using System;

namespace kube.data
{
	public class TryConvert
	{
		public static int ToInt32(string val, int def = 0)
		{
			int result;
			try
			{
				result = Convert.ToInt32(val);
			}
			catch
			{
				result = def;
			}
			return result;
		}

		public static uint ToUInt32(string val, uint def = 0u)
		{
			uint result;
			try
			{
				result = Convert.ToUInt32(val);
			}
			catch
			{
				result = def;
			}
			return result;
		}

		public static float ToFloat(string val, float def = 0f)
		{
			float result;
			try
			{
				result = Convert.ToSingle(val);
			}
			catch
			{
				result = def;
			}
			return result;
		}

		public static double ToDouble(string val)
		{
			double result;
			try
			{
				result = Convert.ToDouble(val);
			}
			catch
			{
				result = 0.0;
			}
			return result;
		}
	}
}
