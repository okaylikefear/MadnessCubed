using System;
using System.IO;
using System.Reflection;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace CodeStage.AntiCheat.Detectors
{
	[DisallowMultipleComponent]
	public class InjectionDetector : ActDetectorBase
	{
		private InjectionDetector()
		{
		}

		public static InjectionDetector Instance { get; private set; }

		private static InjectionDetector GetOrCreateInstance
		{
			get
			{
				if (InjectionDetector.Instance == null)
				{
					InjectionDetector injectionDetector = UnityEngine.Object.FindObjectOfType<InjectionDetector>();
					if (injectionDetector != null)
					{
						InjectionDetector.Instance = injectionDetector;
					}
					else
					{
						if (ActDetectorBase.detectorsContainer == null)
						{
							ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
						}
						ActDetectorBase.detectorsContainer.AddComponent<InjectionDetector>();
					}
				}
				return InjectionDetector.Instance;
			}
		}

		public static void StartDetection(Action callback)
		{
			InjectionDetector.GetOrCreateInstance.StartDetectionInternal(callback);
		}

		public static void StopDetection()
		{
			if (InjectionDetector.Instance != null)
			{
				InjectionDetector.Instance.StopDetectionInternal();
			}
		}

		public static void Dispose()
		{
			if (InjectionDetector.Instance != null)
			{
				InjectionDetector.Instance.DisposeInternal();
			}
		}

		private void Awake()
		{
			if (this.Init(InjectionDetector.Instance, "Injection Detector"))
			{
				InjectionDetector.Instance = this;
			}
		}

		private void StartDetectionInternal(Action callback)
		{
			if (InjectionDetector.isRunning)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Injection Detector already running!");
				return;
			}
			if (!base.enabled)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Injection Detector disabled but StartDetection still called from somewhere!");
				return;
			}
			this.onDetection = callback;
			if (this.allowedAssemblies == null)
			{
				this.LoadAndParseAllowedAssemblies();
			}
			if (this.signaturesAreNotGenuine)
			{
				this.OnInjectionDetected();
				return;
			}
			if (!this.FindInjectionInCurrentAssemblies())
			{
				AppDomain.CurrentDomain.AssemblyLoad += this.OnNewAssemblyLoaded;
				InjectionDetector.isRunning = true;
			}
			else
			{
				this.OnInjectionDetected();
			}
		}

		protected override void StopDetectionInternal()
		{
			if (InjectionDetector.isRunning)
			{
				AppDomain.CurrentDomain.AssemblyLoad -= this.OnNewAssemblyLoaded;
				this.onDetection = null;
				InjectionDetector.isRunning = false;
			}
		}

		protected override void PauseDetector()
		{
			InjectionDetector.isRunning = false;
			AppDomain.CurrentDomain.AssemblyLoad -= this.OnNewAssemblyLoaded;
		}

		protected override void ResumeDetector()
		{
			InjectionDetector.isRunning = true;
			AppDomain.CurrentDomain.AssemblyLoad += this.OnNewAssemblyLoaded;
		}

		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (InjectionDetector.Instance == this)
			{
				InjectionDetector.Instance = null;
			}
		}

		private void OnInjectionDetected()
		{
			if (this.onDetection != null)
			{
				this.onDetection();
			}
			if (this.autoDispose)
			{
				InjectionDetector.Dispose();
			}
			else
			{
				this.StopDetectionInternal();
			}
		}

		private void OnNewAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
		{
			if (!this.AssemblyAllowed(args.LoadedAssembly))
			{
				this.OnInjectionDetected();
			}
		}

		private bool FindInjectionInCurrentAssemblies()
		{
			bool result = false;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			if (assemblies.Length == 0)
			{
				result = true;
			}
			else
			{
				foreach (Assembly ass in assemblies)
				{
					if (!this.AssemblyAllowed(ass))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private bool AssemblyAllowed(Assembly ass)
		{
			string fullName = ass.FullName;
			string b = fullName.Substring(0, fullName.IndexOf(", ", StringComparison.Ordinal));
			int assemblyHash = this.GetAssemblyHash(ass);
			bool result = false;
			for (int i = 0; i < this.allowedAssemblies.Length; i++)
			{
				InjectionDetector.AllowedAssembly allowedAssembly = this.allowedAssemblies[i];
				if (allowedAssembly.name == b && Array.IndexOf<int>(allowedAssembly.hashes, assemblyHash) != -1)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private void LoadAndParseAllowedAssemblies()
		{
			TextAsset textAsset = (TextAsset)Resources.Load("fndid", typeof(TextAsset));
			if (textAsset == null)
			{
				this.signaturesAreNotGenuine = true;
				return;
			}
			string[] separator = new string[]
			{
				":"
			};
			MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			int num = binaryReader.ReadInt32();
			this.allowedAssemblies = new InjectionDetector.AllowedAssembly[num];
			for (int i = 0; i < num; i++)
			{
				string text = binaryReader.ReadString();
				text = ObscuredString.EncryptDecrypt(text, "Elina");
				string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				int num2 = array.Length;
				if (num2 <= 1)
				{
					this.signaturesAreNotGenuine = true;
					binaryReader.Close();
					memoryStream.Close();
					return;
				}
				string name = array[0];
				int[] array2 = new int[num2 - 1];
				for (int j = 1; j < num2; j++)
				{
					array2[j - 1] = int.Parse(array[j]);
				}
				this.allowedAssemblies[i] = new InjectionDetector.AllowedAssembly(name, array2);
			}
			binaryReader.Close();
			memoryStream.Close();
			Resources.UnloadAsset(textAsset);
			this.hexTable = new string[256];
			for (int k = 0; k < 256; k++)
			{
				this.hexTable[k] = k.ToString("x2");
			}
		}

		private int GetAssemblyHash(Assembly ass)
		{
			string fullName = ass.FullName;
			string str = fullName.Substring(0, fullName.IndexOf(", ", StringComparison.Ordinal));
			int num = fullName.IndexOf("PublicKeyToken=", StringComparison.Ordinal) + 15;
			string text = fullName.Substring(num, fullName.Length - num);
			if (text == "null")
			{
				text = string.Empty;
			}
			string text2 = str + text;
			int num2 = 0;
			int length = text2.Length;
			for (int i = 0; i < length; i++)
			{
				num2 += (int)text2[i];
				num2 += num2 << 10;
				num2 ^= num2 >> 6;
			}
			num2 += num2 << 3;
			num2 ^= num2 >> 11;
			return num2 + (num2 << 15);
		}

		private const string COMPONENT_NAME = "Injection Detector";

		internal static bool isRunning;

		private bool signaturesAreNotGenuine;

		private InjectionDetector.AllowedAssembly[] allowedAssemblies;

		private string[] hexTable;

		private class AllowedAssembly
		{
			public AllowedAssembly(string name, int[] hashes)
			{
				this.name = name;
				this.hashes = hashes;
			}

			public readonly string name;

			public readonly int[] hashes;
		}
	}
}
