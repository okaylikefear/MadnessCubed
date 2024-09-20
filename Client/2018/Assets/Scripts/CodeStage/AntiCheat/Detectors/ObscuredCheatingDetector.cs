using System;
using UnityEngine;

namespace CodeStage.AntiCheat.Detectors
{
	[DisallowMultipleComponent]
	public class ObscuredCheatingDetector : ActDetectorBase
	{
		private ObscuredCheatingDetector()
		{
		}

		public static ObscuredCheatingDetector Instance { get; private set; }

		private static ObscuredCheatingDetector GetOrCreateInstance
		{
			get
			{
				if (ObscuredCheatingDetector.Instance == null)
				{
					ObscuredCheatingDetector obscuredCheatingDetector = UnityEngine.Object.FindObjectOfType<ObscuredCheatingDetector>();
					if (obscuredCheatingDetector != null)
					{
						ObscuredCheatingDetector.Instance = obscuredCheatingDetector;
					}
					else
					{
						if (ActDetectorBase.detectorsContainer == null)
						{
							ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
						}
						ActDetectorBase.detectorsContainer.AddComponent<ObscuredCheatingDetector>();
					}
				}
				return ObscuredCheatingDetector.Instance;
			}
		}

		public static void StartDetection(Action callback)
		{
			ObscuredCheatingDetector.GetOrCreateInstance.StartDetectionInternal(callback);
		}

		public static void StopDetection()
		{
			if (ObscuredCheatingDetector.Instance != null)
			{
				ObscuredCheatingDetector.Instance.StopDetectionInternal();
			}
		}

		public static void Dispose()
		{
			if (ObscuredCheatingDetector.Instance != null)
			{
				ObscuredCheatingDetector.Instance.DisposeInternal();
			}
		}

		private void Awake()
		{
			if (this.Init(ObscuredCheatingDetector.Instance, "Obscured Cheating Detector"))
			{
				ObscuredCheatingDetector.Instance = this;
			}
		}

		private void StartDetectionInternal(Action callback)
		{
			if (ObscuredCheatingDetector.isRunning)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Obscured Cheating Detector already running!");
				return;
			}
			if (!base.enabled)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Obscured Cheating Detector disabled but StartDetection still called from somewhere!");
				return;
			}
			this.onDetection = callback;
			ObscuredCheatingDetector.isRunning = true;
		}

		protected override void StopDetectionInternal()
		{
			if (ObscuredCheatingDetector.isRunning)
			{
				this.onDetection = null;
				ObscuredCheatingDetector.isRunning = false;
			}
		}

		protected override void PauseDetector()
		{
			ObscuredCheatingDetector.isRunning = false;
		}

		protected override void ResumeDetector()
		{
			ObscuredCheatingDetector.isRunning = true;
		}

		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (ObscuredCheatingDetector.Instance == this)
			{
				ObscuredCheatingDetector.Instance = null;
			}
		}

		internal void OnCheatingDetected()
		{
			if (this.onDetection != null)
			{
				this.onDetection();
				if (this.autoDispose)
				{
					ObscuredCheatingDetector.Dispose();
				}
				else
				{
					ObscuredCheatingDetector.StopDetection();
				}
			}
		}

		private const string COMPONENT_NAME = "Obscured Cheating Detector";

		internal static bool isRunning;

		[HideInInspector]
		public float floatEpsilon = 0.0001f;

		[HideInInspector]
		public float vector2Epsilon = 0.1f;

		[HideInInspector]
		public float vector3Epsilon = 0.1f;

		[HideInInspector]
		public float quaternionEpsilon = 0.1f;
	}
}
