using System;
using System.Collections.Generic;
using System.Reflection;
using kube;
using UnityEngine;

public class Console : MonoBehaviour
{
	private void OnEnable()
	{
		Application.RegisterLogCallback(new Application.LogCallback(this.HandleLog));
	}

	private void OnDisable()
	{
		Application.RegisterLogCallback(null);
	}

	private void Update()
	{
		if (Cub2Input.GetKeyDown(this.toggleKey))
		{
			this.show = !this.show;
			if (this.show)
			{
				Screen.lockCursor = false;
			}
			GUI.FocusControl("ConsoleInput");
			this.scrollPos = new Vector2(0f, (float)(this.entries.Count * 200));
		}
	}

	private void OnGUI()
	{
		if (!this.show)
		{
			return;
		}
		GUI.depth = -6;
		this.ConsoleWindow(0);
	}

	private void Execute(string args)
	{
		string[] array = args.Split(global::Console._separators);
		string str = array[0];
		MethodInfo method = base.GetType().GetMethod("CMD_" + str, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (method != null)
		{
			method.Invoke(this, new object[]
			{
				array
			});
		}
		else
		{
			Kube.SendMonoMessage("CMD_" + str, new object[]
			{
				array
			});
		}
		this.entries.Add(new global::Console.ConsoleMessage("> " + str, string.Empty, LogType.Log));
	}

	private void CMD_clr(string[] argv)
	{
		this.entries.Clear();
	}

	private void ConsoleWindow(int windowID)
	{
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
		{
			this.Execute(this.cmd);
			this.cmd = string.Empty;
			return;
		}
		GUI.Box(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), string.Empty);
		this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, new GUILayoutOption[]
		{
			GUILayout.Width((float)Screen.width),
			GUILayout.Height((float)(Screen.height - 100))
		});
		for (int i = 0; i < this.entries.Count; i++)
		{
			global::Console.ConsoleMessage consoleMessage = this.entries[i];
			if (!this.collapse || i <= 0 || !(consoleMessage.message == this.entries[i - 1].message))
			{
				switch (consoleMessage.type)
				{
				case LogType.Error:
				case LogType.Exception:
					GUI.contentColor = Color.red;
					GUILayout.Label(consoleMessage.stackTrace, new GUILayoutOption[0]);
					break;
				case LogType.Assert:
				case LogType.Log:
					goto IL_13D;
				case LogType.Warning:
					GUI.contentColor = Color.yellow;
					break;
				default:
					goto IL_13D;
				}
				IL_14C:
				GUILayout.Label(consoleMessage.message, new GUILayoutOption[0]);
				goto IL_15E;
				IL_13D:
				GUI.contentColor = Color.white;
				goto IL_14C;
			}
			IL_15E:;
		}
		GUI.contentColor = Color.white;
		GUILayout.EndScrollView();
		GUI.SetNextControlName("ConsoleInput");
		this.cmd = GUILayout.TextField(this.cmd, new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		this.collapse = GUILayout.Toggle(this.collapse, this.collapseLabel, new GUILayoutOption[]
		{
			GUILayout.ExpandWidth(false)
		});
		GUILayout.EndHorizontal();
	}

	private void HandleLog(string message, string stackTrace, LogType type)
	{
		global::Console.ConsoleMessage item = new global::Console.ConsoleMessage(message, stackTrace, type);
		this.entries.Add(item);
	}

	private const int margin = 20;

	public static readonly Version version = new Version(1, 0);

	public KeyCode toggleKey = KeyCode.BackQuote;

	private List<global::Console.ConsoleMessage> entries = new List<global::Console.ConsoleMessage>();

	private Vector2 scrollPos;

	private bool show;

	private bool collapse;

	private GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");

	private GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

	private string cmd = string.Empty;

	private static char[] _separators = new char[]
	{
		' '
	};

	private struct ConsoleMessage
	{
		public ConsoleMessage(string message, string stackTrace, LogType type)
		{
			this.message = message;
			this.stackTrace = stackTrace;
			this.type = type;
		}

		public readonly string message;

		public readonly string stackTrace;

		public readonly LogType type;
	}
}
