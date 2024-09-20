using System;
using UnityEngine;

namespace CodeStage.AntiCheat.Detectors
{
	[DisallowMultipleComponent]
	public class SpeedHackDetector : ActDetectorBase
	{
		private SpeedHackDetector()
		{
		}

		public static SpeedHackDetector Instance { get; private set; }

		private static SpeedHackDetector GetOrCreateInstance
		{
			get
			{
				if (SpeedHackDetector.Instance == null)
				{
					SpeedHackDetector speedHackDetector = UnityEngine.Object.FindObjectOfType<SpeedHackDetector>();
					if (speedHackDetector != null)
					{
						SpeedHackDetector.Instance = speedHackDetector;
					}
					else
					{
						if (ActDetectorBase.detectorsContainer == null)
						{
							ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
						}
						ActDetectorBase.detectorsContainer.AddComponent<SpeedHackDetector>();
					}
				}
				return SpeedHackDetector.Instance;
			}
		}

		public static void StartDetection(Action callback)
		{
			SpeedHackDetector.StartDetection(callback, SpeedHackDetector.GetOrCreateInstance.interval);
		}

		public static void StartDetection(Action callback, float checkInterval)
		{
			SpeedHackDetector.StartDetection(callback, checkInterval, SpeedHackDetector.GetOrCreateInstance.maxFalsePositives);
		}

		public static void StartDetection(Action callback, float checkInterval, byte falsePositives)
		{
			SpeedHackDetector.StartDetection(callback, checkInterval, falsePositives, SpeedHackDetector.GetOrCreateInstance.coolDown);
		}

		public static void StartDetection(Action callback, float checkInterval, byte falsePositives, int shotsTillCooldown)
		{
			SpeedHackDetector.GetOrCreateInstance.StartDetectionInternal(callback, checkInterval, falsePositives, shotsTillCooldown);
		}

		public static void StopDetection()
		{
			if (SpeedHackDetector.Instance != null)
			{
				SpeedHackDetector.Instance.StopDetectionInternal();
			}
		}

		public static void Dispose()
		{
			if (SpeedHackDetector.Instance != null)
			{
				SpeedHackDetector.Instance.DisposeInternal();
			}
		}

		private void Awake()
		{
			if (this.Init(SpeedHackDetector.Instance, "Speed Hack Detector"))
			{
				SpeedHackDetector.Instance = this;
			}
		}

		private void StartDetectionInternal(Action callback, float checkInterval, byte falsePositives, int shotsTillCooldown)
		{
			if (SpeedHackDetector.isRunning)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Speed Hack Detector already running!");
				return;
			}
			if (!base.enabled)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Speed Hack Detector disabled but StartDetection still called from somewhere!");
				return;
			}
			this.onDetection = callback;
			this.interval = checkInterval;
			this.maxFalsePositives = falsePositives;
			this.coolDown = shotsTillCooldown;
			this.ResetStartTicks();
			this.currentFalsePositives = 0;
			this.currentCooldownShots = 0;
			SpeedHackDetector.isRunning = true;
		}

		protected override void StopDetectionInternal()
		{
			if (SpeedHackDetector.isRunning)
			{
				this.onDetection = null;
				SpeedHackDetector.isRunning = false;
			}
		}

		protected override void PauseDetector()
		{
			SpeedHackDetector.isRunning = false;
		}

		protected override void ResumeDetector()
		{
			SpeedHackDetector.isRunning = true;
		}

		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (SpeedHackDetector.Instance == this)
			{
				SpeedHackDetector.Instance = null;
			}
		}

		private void ResetStartTicks()
		{
			this.ticksOnStart = DateTime.UtcNow.Ticks;
			this.vulnerableTicksOnStart = (long)Environment.TickCount * 10000L;
			this.prevTicks = this.ticksOnStart;
			this.prevIntervalTicks = this.ticksOnStart;
		}

		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				this.ResetStartTicks();
			}
		}

		private void Update()
		{
			if (!SpeedHackDetector.isRunning)
			{
				return;
			}
			long ticks = DateTime.UtcNow.Ticks;
			long num = ticks - this.prevTicks;
			if (num < 0L || num > 10000000L)
			{
				if (Debug.isDebugBuild)
				{
					UnityEngine.Debug.LogWarning("[ACTk] SpeedHackDetector: System DateTime change or > 1 second game freeze detected!");
				}
				this.ResetStartTicks();
				return;
			}
			this.prevTicks = ticks;
			long num2 = (long)(this.interval * 1E+07f);
			if (ticks - this.prevIntervalTicks >= num2)
			{
				long num3 = (long)Environment.TickCount * 10000L;
				if (Mathf.Abs((float)(num3 - this.vulnerableTicksOnStart - (ticks - this.ticksOnStart))) > 5000000f)
				{
					this.currentFalsePositives += 1;
					if (this.currentFalsePositives > this.maxFalsePositives)
					{
						if (Debug.isDebugBuild)
						{
							UnityEngine.Debug.LogWarning("[ACTk] SpeedHackDetector: final detection!");
						}
						if (this.onDetection != null)
						{
							this.onDetection();
						}
						if (this.autoDispose)
						{
							SpeedHackDetector.Dispose();
						}
						else
						{
							SpeedHackDetector.StopDetection();
						}
					}
					else
					{
						if (Debug.isDebugBuild)
						{
							UnityEngine.Debug.LogWarning("[ACTk] SpeedHackDetector: detection! Allowed false positives left: " + (int)(this.maxFalsePositives - this.currentFalsePositives));
						}
						this.currentCooldownShots = 0;
						this.ResetStartTicks();
					}
				}
				else if (this.currentFalsePositives > 0 && this.coolDown > 0)
				{
					if (Debug.isDebugBuild)
					{
						UnityEngine.Debug.LogWarning("[ACTk] SpeedHackDetector: success shot! Shots till Cooldown: " + (this.coolDown - this.currentCooldownShots));
					}
					this.currentCooldownShots++;
					if (this.currentCooldownShots >= this.coolDown)
					{
						if (Debug.isDebugBuild)
						{
							UnityEngine.Debug.LogWarning("[ACTk] SpeedHackDetector: Cooldown!");
						}
						this.currentFalsePositives = 0;
					}
				}
				this.prevIntervalTicks = ticks;
			}
		}

		private const string COMPONENT_NAME = "Speed Hack Detector";

		private const long TICKS_PER_SECOND = 10000000L;

		private const int THRESHOLD = 5000000;

		internal static bool isRunning;

		[Tooltip("Time (in seconds) between detector checks.")]
		public float interval = 1f;

		[Tooltip("Maximum false positives count allowed before registering speed hack.")]
		public byte maxFalsePositives = 3;

		[Tooltip("Amount of sequential successful checks before clearing internal false positives counter.\nSet 0 to disable Cool Down feature.")]
		public int coolDown = 30;

		private byte currentFalsePositives;

		private int currentCooldownShots;

		private long ticksOnStart;

		private long vulnerableTicksOnStart;

		private long prevTicks;

		private long prevIntervalTicks;
	}
}
