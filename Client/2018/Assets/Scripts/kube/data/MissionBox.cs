using System;
using System.Collections.Generic;
using LitJson;

namespace kube.data
{
	public class MissionBox
	{
		public static void invalidate()
		{
			MissionBox.isValid = false;
		}

		public static object[] parseMissionParams(int type, string par1)
		{
			string[] array = par1.Split(new char[]
			{
				';'
			});
			int num = Math.Max(array.Length, 6);
			object[] array2 = new object[num];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = int.Parse(array[i]);
			}
			return array2;
		}

		public static MissionDesc[] selectMissions(int k)
		{
			List<MissionDesc> list = new List<MissionDesc>();
			int num = 3;
			int index = 0;
			for (int i = 0; i < MissionBox.missions.Length; i++)
			{
				if (MissionBox.missions[i].episode == k)
				{
					int num2 = (int)Math.Min(3.0, Math.Round(3.0 * (double)MissionBox.missions[i].score / (double)MissionBox.missions[i].maxscore));
					MissionBox.missions[i].index = list.Count;
					MissionDesc item = MissionBox.missions[i];
					if (item.score > 0)
					{
						item.bonus = new Dictionary<BonusDesc, int>();
						item.gold = 0;
						item.money /= 10;
					}
					if (list.Count > 0 && num < 2)
					{
						item.enabled = false;
						item.score = 0;
					}
					else
					{
						item.enabled = true;
						item.nnstars = num2;
						index = list.Count;
					}
					if (num > 2)
					{
						num = num2;
					}
					list.Add(item);
				}
			}
			if (list.Count > 0)
			{
				MissionDesc value = list[index];
				value.current = true;
				list[index] = value;
			}
			return list.ToArray();
		}

		public static MissionDesc FindMissionById(int id)
		{
			for (int i = 0; i < MissionBox.missions.Length; i++)
			{
				if (MissionBox.missions[i].id == id)
				{
					return MissionBox.missions[i];
				}
			}
			return new MissionDesc
			{
				id = 0,
				title = "Миссия",
				episode = 999,
				config = new object[]
				{
					0,
					0
				},
				type = 1
			};
		}

		public static EpisodeDesc FindEpisodeById(int id)
		{
			for (int i = 0; i < MissionBox.episodes.Length; i++)
			{
				if (MissionBox.episodes[i].id == id)
				{
					return MissionBox.episodes[i];
				}
			}
			return MissionBox.episodes[0];
		}

		public static void request(VoidCallback cb, bool invalidate = false)
		{
			if (MissionBox.isValid && !invalidate)
			{
				cb();
				return;
			}
			MissionBox._eventStack.Push(cb);
			if (MissionBox._eventStack.Count > 1)
			{
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["id"] = Kube.SS.serverId.ToString();
			Kube.SS.Request(666, dictionary, new ServerCallback(MissionBox.missionLoadDone));
		}

		protected static void missionLoadDone(string response)
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			JsonData jsonData2 = jsonData["mss"];
			JsonData jsonData3 = jsonData["ess"];
			int count = jsonData2.Count;
			MissionBox.missions = new MissionDesc[count];
			int num = 1;
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData4 = jsonData2[i];
				MissionBox.missions[i].id = int.Parse(jsonData4["id"].ToString());
				string text = jsonData4["mapname"].ToString();
				MissionBox.missions[i].title = Localize.T(text, text);
				if (jsonData4["score"] != null)
				{
					MissionBox.missions[i].score = int.Parse(jsonData4["score"].ToString());
				}
				MissionBox.missions[i].id = int.Parse(jsonData4["id"].ToString());
				MissionBox.missions[i].episode = int.Parse(jsonData4["grp_id"].ToString());
				MissionBox.missions[i].mapId = long.Parse(jsonData4["map_id"].ToString());
				MissionBox.missions[i].maxscore = int.Parse(jsonData4["maxscore"].ToString());
				MissionBox.missions[i].type = int.Parse(jsonData4["type"].ToString());
				MissionDesc[] array = MissionBox.missions;
				int num2 = i;
				object[] config;
				if (jsonData4["params"] != null)
				{
					config = MissionBox.parseMissionParams(MissionBox.missions[i].type, jsonData4["params"].ToString());
				}
				else
				{
					object[] array2 = new object[2];
					array2[0] = 0;
					config = array2;
					array2[1] = 0;
				}
				array[num2].config = config;
				MissionBox.missions[i].dayTime = int.Parse(jsonData4["dayTime"].ToString());
				MissionBox.missions[i].canBreak = int.Parse(jsonData4["can_break"].ToString());
				MissionBox.missions[i].money = int.Parse(jsonData4["money"].ToString());
				MissionBox.missions[i].gold = int.Parse(jsonData4["gold"].ToString());
				MissionBox.missions[i].offline = (int.Parse(jsonData4["online"].ToString()) == 0);
				if (jsonData4.Keys.Contains("jet"))
				{
					MissionBox.missions[i].isJetPack = (int.Parse(jsonData4["jet"].ToString()) == 1);
				}
				else
				{
					MissionBox.missions[i].isJetPack = true;
				}
				if (jsonData4["bonus"] != null)
				{
					MissionBox.missions[i].bonus = MissionHelper.parseBonus(jsonData4["bonus"].ToString());
				}
				if (MissionBox.missions[i].episode > num && MissionBox.missions[i].episode != 100)
				{
					num = MissionBox.missions[i].episode;
				}
			}
			if (jsonData3.Count == 0)
			{
				num = Math.Min(MissionBox._episodes.Length, num);
				MissionBox.episodes = new EpisodeDesc[num];
				for (int j = 0; j < num; j++)
				{
					MissionBox.episodes[j] = MissionBox._episodes[j];
				}
			}
			else
			{
				MissionBox.episodes = new EpisodeDesc[jsonData3.Count];
			}
			for (int k = 0; k < jsonData3.Count; k++)
			{
				JsonData jsonData5 = jsonData3[k];
				MissionBox.episodes[k].id = int.Parse(jsonData5["id"].ToString());
				string title = jsonData5["name"].ToString();
				if (Localize.T("episode" + MissionBox.episodes[k].id, null) != null)
				{
					title = Localize.T("episode" + MissionBox.episodes[k].id, null);
				}
				MissionBox.episodes[k].title = title;
			}
			MissionBox.isValid = true;
			while (MissionBox._eventStack.Count > 0)
			{
				VoidCallback voidCallback = MissionBox._eventStack.Pop();
				voidCallback();
			}
		}

		public const int SECRET_EPISODE_ID = 100;

		protected static MissionDesc[] missions;

		protected static bool isValid = false;

		public static EpisodeDesc[] _episodes = new EpisodeDesc[]
		{
			new EpisodeDesc(Localize.episode1, 1),
			new EpisodeDesc(Localize.episode2, 2),
			new EpisodeDesc(Localize.episode3, 3),
			new EpisodeDesc(Localize.episode4, 4),
			new EpisodeDesc(Localize.episode5, 5),
			new EpisodeDesc(Localize.episode6, 6),
			new EpisodeDesc(Localize.episode7, 7),
			new EpisodeDesc(Localize.episode8, 8)
		};

		public static EpisodeDesc[] episodes;

		private static Stack<VoidCallback> _eventStack = new Stack<VoidCallback>();
	}
}
