using System;
using System.Collections.Generic;
using LitJson;

namespace kube.data
{
	public class MapTop
	{
		public static TopInfo[] parse(JsonData data)
		{
			List<TopInfo> list = new List<TopInfo>();
			for (int i = 0; i < data.Count; i++)
			{
				list.Add(new TopInfo
				{
					id = int.Parse(data[i]["id"].ToString()),
					roomMapNumber = long.Parse(data[i]["mapid"].ToString()),
					name = data[i]["name"].ToString(),
					roomType = int.Parse(data[i]["type"].ToString()),
					mapCanBreak = int.Parse(data[i]["canbreak"].ToString()),
					dayLight = int.Parse(data[i]["daytime"].ToString()),
					hits = int.Parse(data[i]["hits"].ToString())
				});
			}
			return list.ToArray();
		}
	}
}
