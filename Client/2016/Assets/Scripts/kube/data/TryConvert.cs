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
	}
}
