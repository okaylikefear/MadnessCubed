using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace kube.data
{
	public class TaskBox
	{
		public static bool isMenuTask(TaskType type)
		{
			return TaskBox._isMenuTask[(int)type];
		}

		public static void invalidate()
		{
			TaskBox.isValid = false;
		}

		public static object[] parseTaskParams(int type, string par1)
		{
			string[] array = par1.Split(new char[]
			{
				';'
			});
			int num = Math.Max(array.Length, 6);
			object[] array2 = new object[num];
			for (int i = 0; i < array.Length; i++)
			{
				int num2 = 0;
				if (int.TryParse(array[i], out num2))
				{
					array2[i] = num2;
				}
				else
				{
					array2[i] = array[i];
				}
			}
			return array2;
		}

		public static void updateTask(int id, int value)
		{
			for (int i = 0; i < TaskBox.tasks.Length; i++)
			{
				if (TaskBox.tasks[i].id == id)
				{
					TaskBox.tasks[i].score = value;
					TaskBox.tasks[i].dt = DateTime.Now;
					break;
				}
			}
		}

		public static TaskDesc[] selectTasks()
		{
			List<TaskDesc> list = new List<TaskDesc>();
			int num = -1;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < TaskBox.tasks.Length; j++)
				{
					if (TaskBox.tasks[j].episode == i + 1)
					{
						TaskDesc taskDesc = TaskBox.tasks[j];
						if (taskDesc.score <= 0)
						{
							list.Add(taskDesc);
							break;
						}
					}
				}
			}
			if (list.Count < 2)
			{
				bool flag = false;
				List<TaskDesc> list2 = new List<TaskDesc>();
				TaskDesc taskDesc2 = null;
				for (int k = 0; k < TaskBox.tasks.Length; k++)
				{
					TaskDesc taskDesc3 = TaskBox.tasks[k];
					if (TaskBox.tasks[k].daily)
					{
						if (TaskBox.tasks[k].isCompleted)
						{
							if (TaskBox.tasks[k].dt.Date == DateTime.Now.Date)
							{
								flag = true;
								break;
							}
							if (TaskBox.tasks[k].dt.AddHours(8.0) >= DateTime.Now)
							{
								goto IL_1A0;
							}
							taskDesc3.score = 0;
							taskDesc3.progress = new object[]
							{
								0,
								0,
								0,
								0
							};
						}
						else if (TaskBox.tasks[k].dt.Date == DateTime.Now.Date)
						{
							taskDesc2 = taskDesc3;
						}
						list2.Add(taskDesc3);
					}
					IL_1A0:;
				}
				if (taskDesc2 != null)
				{
					list.Add(taskDesc2);
				}
				else if (!flag && list2.Count > 0)
				{
					int index = UnityEngine.Random.Range(0, list2.Count);
					list2[index].dt = DateTime.Now.Date;
					list.Add(list2[index]);
				}
			}
			if (num != -1)
			{
				TaskDesc value = list[num];
				list[num] = value;
			}
			return list.ToArray();
		}

		public static TaskStoryDesc FindStory(int id)
		{
			TaskStoryDesc taskStoryDesc = new TaskStoryDesc();
			List<TaskDesc> list = new List<TaskDesc>();
			for (int i = 0; i < TaskBox.tasks.Length; i++)
			{
				if (TaskBox.tasks[i].parrent == id)
				{
					list.Add(TaskBox.tasks[i]);
				}
			}
			taskStoryDesc.story = list.ToArray();
			return taskStoryDesc;
		}

		public static TaskDesc FindTaskById(int id)
		{
			for (int i = 0; i < TaskBox.tasks.Length; i++)
			{
				if (TaskBox.tasks[i].id == id)
				{
					return TaskBox.tasks[i];
				}
			}
			return TaskBox.tasks[0];
		}

		public static void parse(JsonData data)
		{
			int count = data.Count;
			TaskBox.tasks = new TaskDesc[count];
			for (int i = 0; i < data.Count; i++)
			{
				JsonData jsonData = data[i];
				TaskBox.tasks[i] = new TaskDesc();
				TaskBox.tasks[i].id = int.Parse(jsonData["id"].ToString());
				string text = null;
				if (jsonData.Keys.Contains("name") && jsonData["name"] != null)
				{
					text = jsonData["name"].ToString();
				}
				if (text == null || text == string.Empty)
				{
					text = Localize.task_name[TaskBox.tasks[i].id];
				}
				TaskBox.tasks[i].title = text;
				TaskBox.tasks[i].ico = jsonData["ico"].ToString();
				if (jsonData["score"] != null)
				{
					TaskBox.tasks[i].score = int.Parse(jsonData["score"].ToString());
				}
				if (jsonData["dt"] != null)
				{
					TaskBox.tasks[i].dt = DateTime.Parse(jsonData["dt"].ToString());
				}
				TaskBox.tasks[i].id = int.Parse(jsonData["id"].ToString());
				TaskBox.tasks[i].episode = int.Parse(jsonData["grp_id"].ToString());
				if (jsonData["daily"] != null)
				{
					TaskBox.tasks[i].daily = (int.Parse(jsonData["daily"].ToString()) == 1);
				}
				TaskBox.tasks[i].type = (TaskType)int.Parse(jsonData["type"].ToString());
				TaskDesc taskDesc = TaskBox.tasks[i];
				object[] config;
				if (jsonData["params"] != null)
				{
					config = TaskBox.parseTaskParams((int)TaskBox.tasks[i].type, jsonData["params"].ToString());
				}
				else
				{
					object[] array = new object[2];
					array[0] = 0;
					config = array;
					array[1] = 0;
				}
				taskDesc.config = config;
				TaskBox.tasks[i].progress = new object[]
				{
					0,
					0,
					0,
					0
				};
				if (jsonData.Keys.Contains("progress") && jsonData["progress"] != null)
				{
					TaskBox.tasks[i].progress = TaskBox.parseTaskParams((int)TaskBox.tasks[i].type, jsonData["progress"].ToString());
				}
				if (jsonData.Keys.Contains("parrent"))
				{
					TaskBox.tasks[i].parrent = int.Parse(jsonData["parrent"].ToString());
				}
				else
				{
					TaskBox.tasks[i].parrent = TaskBox.tasks[i].episode;
				}
				TaskBox.tasks[i].money = int.Parse(jsonData["money"].ToString());
				TaskBox.tasks[i].gold = int.Parse(jsonData["gold"].ToString());
				if (jsonData["bonus"] != null)
				{
					TaskBox.tasks[i].bonus = TaskHelper.parseBonus(jsonData["bonus"].ToString());
				}
			}
			TaskBox.isValid = true;
			while (TaskBox._eventStack.Count > 0)
			{
				VoidCallback voidCallback = TaskBox._eventStack.Pop();
				voidCallback();
			}
		}

		public static string EncodeProgress(object[] p)
		{
			string text = string.Empty;
			for (int i = 0; i < p.Length; i++)
			{
				if (i > 0)
				{
					text += ";";
				}
				if (p[i] != null)
				{
					text += p[i].ToString();
				}
			}
			return text;
		}

		private static bool[] _isMenuTask = new bool[]
		{
			default(bool),
			default(bool),
			default(bool),
			default(bool),
			default(bool),
			default(bool),
			default(bool),
			true,
			true
		};

		protected static TaskDesc[] tasks;

		protected static bool isValid = false;

		private static Stack<VoidCallback> _eventStack = new Stack<VoidCallback>();
	}
}
