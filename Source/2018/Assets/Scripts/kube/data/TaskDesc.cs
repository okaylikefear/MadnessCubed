using System;
using System.Collections.Generic;

namespace kube.data
{
	public class TaskDesc
	{
		public bool isCompleted
		{
			get
			{
				if (this.type == TaskType.daily3)
				{
					return (this.score & 7) == 7;
				}
				return this.score > 0;
			}
		}

		public int id;

		public string title;

		public int score;

		public int episode;

		public TaskType type;

		public object[] config;

		public object[] progress;

		public int gold;

		public int money;

		public Dictionary<BonusDesc, int> bonus;

		public string ico;

		public bool enabled;

		public DateTime dt;

		public bool daily;

		public int parrent;
	}
}
