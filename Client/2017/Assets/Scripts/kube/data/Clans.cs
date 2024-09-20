using System;
using System.Collections.Generic;
using LitJson;

namespace kube.data
{
	public class Clans
	{
		public static bool checkShortName(string value)
		{
			if (Clans._list == null)
			{
				return true;
			}
			for (int i = 0; i < Clans._list.Count; i++)
			{
				if (string.Compare(Clans._list[i].shortName, value, StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					return false;
				}
			}
			return true;
		}

		public static ClanInfo parseClan(JsonData data)
		{
			return new ClanInfo
			{
				id = int.Parse(data["id"].ToString()),
				name = data["name"].ToString(),
				shortName = data["sname"].ToString(),
				players = int.Parse(data["players"].ToString()),
				frags = int.Parse(data["frags"].ToString()),
				kills = int.Parse(data["kills"].ToString()),
				owner = int.Parse(data["owner"].ToString()),
				home = data["home"].ToString()
			};
		}

		public static Dictionary<int, bool> parseXRef(JsonData items)
		{
			Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
			for (int i = 0; i < items.Count; i++)
			{
				int key = int.Parse(items[i]["clan"].ToString());
				dictionary[key] = true;
			}
			return dictionary;
		}

		public static ClanInfo[] parse(JsonData data)
		{
			List<ClanInfo> list = new List<ClanInfo>();
			for (int i = 0; i < data.Count; i++)
			{
				ClanInfo item = Clans.parseClan(data[i]);
				list.Add(item);
			}
			Clans._list = list;
			return list.ToArray();
		}

		public static ClanMember[] parseMembers(JsonData data)
		{
			List<ClanMember> list = new List<ClanMember>();
			for (int i = 0; i < data.Count; i++)
			{
				ClanMember clanMember = new ClanMember();
				clanMember.id = int.Parse(data[i]["player"].ToString());
				clanMember.type = int.Parse(data[i]["type"].ToString());
				if (data[i]["name"] != null)
				{
					clanMember.name = AuxFunc.DecodeRussianName(data[i]["name"].ToString());
					clanMember.uid = data[i]["uid"].ToString();
					list.Add(clanMember);
				}
			}
			return list.ToArray();
		}

		private static List<ClanInfo> _list;
	}
}
