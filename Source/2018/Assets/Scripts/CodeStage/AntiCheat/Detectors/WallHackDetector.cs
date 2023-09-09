using System;
using UnityEngine;

namespace CodeStage.AntiCheat.Detectors
{
	[DisallowMultipleComponent]
	public class WallHackDetector : ActDetectorBase
	{
		private WallHackDetector()
		{
		}

		public static WallHackDetector Instance { get; private set; }

		private static WallHackDetector GetOrCreateInstance
		{
			get
			{
				if (WallHackDetector.Instance == null)
				{
					WallHackDetector wallHackDetector = UnityEngine.Object.FindObjectOfType<WallHackDetector>();
					if (wallHackDetector != null)
					{
						WallHackDetector.Instance = wallHackDetector;
					}
					else
					{
						if (ActDetectorBase.detectorsContainer == null)
						{
							ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
						}
						ActDetectorBase.detectorsContainer.AddComponent<WallHackDetector>();
					}
				}
				return WallHackDetector.Instance;
			}
		}

		public static void StartDetection(Action callback)
		{
			WallHackDetector.StartDetection(callback, WallHackDetector.GetOrCreateInstance.spawnPosition);
		}

		public static void StartDetection(Action callback, Vector3 servicePosition)
		{
			WallHackDetector.GetOrCreateInstance.StartDetectionInternal(callback, servicePosition);
		}

		public static void StopDetection()
		{
			if (WallHackDetector.Instance != null)
			{
				WallHackDetector.Instance.StopDetectionInternal();
			}
		}

		public static void Dispose()
		{
			if (WallHackDetector.Instance != null)
			{
				WallHackDetector.Instance.DisposeInternal();
			}
		}

		private void Awake()
		{
			if (this.Init(WallHackDetector.Instance, "WallHack Detector"))
			{
				WallHackDetector.Instance = this;
			}
		}

		private void StartDetectionInternal(Action callback, Vector3 servicePosition)
		{
			if (WallHackDetector.isRunning)
			{
				UnityEngine.Debug.LogWarning("[ACTk] WallHack Detector already running!");
				return;
			}
			if (!base.enabled)
			{
				UnityEngine.Debug.LogWarning("[ACTk] WallHack Detector disabled but StartDetection still called from somewhere!");
				return;
			}
			this.onDetection = callback;
			this.spawnPosition = servicePosition;
			this.InitDetector();
			WallHackDetector.isRunning = true;
		}

		protected override void StopDetectionInternal()
		{
			if (WallHackDetector.isRunning)
			{
				this.UninitDetector();
				this.onDetection = null;
				WallHackDetector.isRunning = false;
			}
		}

		protected override void PauseDetector()
		{
			if (!WallHackDetector.isRunning)
			{
				return;
			}
			WallHackDetector.isRunning = false;
			this.StopRigidModule();
			this.StopControllerModule();
		}

		protected override void ResumeDetector()
		{
			WallHackDetector.isRunning = true;
			this.StartRigidModule();
			this.StartControllerModule();
		}

		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (WallHackDetector.Instance == this)
			{
				WallHackDetector.Instance = null;
			}
		}

		private void InitDetector()
		{
			this.InitCommon();
			this.InitRigidModule();
			this.InitControllerModule();
			this.StartRigidModule();
			this.StartControllerModule();
		}

		private void UninitDetector()
		{
			WallHackDetector.isRunning = false;
			this.StopRigidModule();
			this.StopControllerModule();
			UnityEngine.Object.Destroy(this.serviceContainer);
		}

		private void InitCommon()
		{
			if (this.whLayer == -1)
			{
				this.whLayer = LayerMask.NameToLayer("Ignore Raycast");
			}
			this.serviceContainer = new GameObject("[WH Detector Service]");
			this.serviceContainer.layer = this.whLayer;
			this.serviceContainer.transform.position = this.spawnPosition;
			UnityEngine.Object.DontDestroyOnLoad(this.serviceContainer);
			GameObject gameObject = new GameObject("Wall");
			gameObject.AddComponent<BoxCollider>();
			gameObject.layer = this.whLayer;
			gameObject.transform.parent = this.serviceContainer.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = new Vector3(3f, 3f, 0.5f);
		}

		private void InitRigidModule()
		{
			GameObject gameObject = new GameObject("RigidPlayer");
			gameObject.AddComponent<CapsuleCollider>().height = 2f;
			gameObject.layer = this.whLayer;
			gameObject.transform.parent = this.serviceContainer.transform;
			gameObject.transform.localPosition = new Vector3(0.75f, 0f, -1f);
			this.rigidPlayer = gameObject.AddComponent<Rigidbody>();
			this.rigidPlayer.useGravity = false;
		}

		private void InitControllerModule()
		{
			GameObject gameObject = new GameObject("ControlledPlayer");
			gameObject.AddComponent<CapsuleCollider>().height = 2f;
			gameObject.layer = this.whLayer;
			gameObject.transform.parent = this.serviceContainer.transform;
			gameObject.transform.localPosition = new Vector3(-0.75f, 0f, -1f);
			this.charControllerPlayer = gameObject.AddComponent<CharacterController>();
		}

		private void StartRigidModule()
		{
			this.rigidPlayer.rotation = Quaternion.identity;
			this.rigidPlayer.angularVelocity = Vector3.zero;
			this.rigidPlayer.transform.localPosition = new Vector3(0.75f, 0f, -1f);
			this.rigidPlayer.velocity = this.rigidPlayerVelocity;
			base.Invoke("StartRigidModule", 4f);
		}

		private void StopRigidModule()
		{
			this.rigidPlayer.velocity = Vector3.zero;
			base.CancelInvoke("StartRigidModule");
		}

		private void StartControllerModule()
		{
			this.charControllerPlayer.transform.localPosition = new Vector3(-0.75f, 0f, -1f);
			this.charControllerVelocity = 0.01f;
			base.Invoke("StartControllerModule", 4f);
		}

		private void StopControllerModule()
		{
			this.charControllerVelocity = 0f;
			base.CancelInvoke("StartControllerModule");
		}

		private void FixedUpdate()
		{
			if (!WallHackDetector.isRunning)
			{
				return;
			}
			if (this.rigidPlayer.transform.localPosition.z > 1f)
			{
				this.StopRigidModule();
				this.Detect();
			}
		}

		private void Update()
		{
			if (!WallHackDetector.isRunning)
			{
				return;
			}
			if (this.charControllerVelocity > 0f)
			{
				this.charControllerPlayer.Move(new Vector3(UnityEngine.Random.Range(-0.002f, 0.002f), 0f, this.charControllerVelocity));
				if (this.charControllerPlayer.transform.localPosition.z > 1f)
				{
					this.StopControllerModule();
					this.Detect();
				}
			}
		}

		private void Detect()
		{
			if (this.onDetection != null)
			{
				this.onDetection();
			}
			if (this.autoDispose)
			{
				WallHackDetector.Dispose();
			}
			else
			{
				WallHackDetector.StopDetection();
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(this.spawnPosition, new Vector3(3f, 3f, 3f));
		}

		private const string COMPONENT_NAME = "WallHack Detector";

		private const string SERVICE_CONTAINER_NAME = "[WH Detector Service]";

		private readonly Vector3 rigidPlayerVelocity = new Vector3(0f, 0f, 1f);

		internal static bool isRunning;

		[Tooltip("World position of the container for service objects within 3x3x3 cube (drawn as red wireframe cube in scene).")]
		public Vector3 spawnPosition;

		private int whLayer = -1;

		private GameObject serviceContainer;

		private Rigidbody rigidPlayer;

		private CharacterController charControllerPlayer;

		private float charControllerVelocity;
	}
}
