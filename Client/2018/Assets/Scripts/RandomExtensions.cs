using System;

internal static class RandomExtensions
{
	public static T[] Shuffle<T>(this T[] array)
	{
		Random random = new Random();
		int i = array.Length;
		while (i > 1)
		{
			int num = random.Next(i);
			i--;
			T t = array[i];
			array[i] = array[num];
			array[num] = t;
		}
		return array;
	}
}
