using System;
using System.Collections;
using System.Collections.Generic;
using MoPhoGames.USpeak.Core;
using MoPhoGames.USpeak.Interface;
using UnityEngine;

[AddComponentMenu("USpeak/USpeaker")]
public class USpeaker : MonoBehaviour
{
	public bool IsTalking
	{
		get
		{
			return this.talkTimer > 0f;
		}
	}

	private int audioFrequency
	{
		get
		{
			if (this.recFreq == 0)
			{
				BandMode bandwidthMode = this.BandwidthMode;
				if (bandwidthMode != BandMode.Narrow)
				{
					if (bandwidthMode != BandMode.Wide)
					{
						this.recFreq = 8000;
					}
					else
					{
						this.recFreq = 16000;
					}
				}
				else
				{
					this.recFreq = 8000;
				}
			}
			return this.recFreq;
		}
	}

	public void SetInputDevice(int deviceID)
	{
		USpeaker.InputDeviceID = deviceID;
	}

	public static USpeaker Get(UnityEngine.Object source)
	{
		if (source is GameObject)
		{
			return (source as GameObject).GetComponent<USpeaker>();
		}
		if (source is Transform)
		{
			return (source as Transform).GetComponent<USpeaker>();
		}
		if (source is Component)
		{
			return (source as Component).GetComponent<USpeaker>();
		}
		return null;
	}

	public void GetInputHandler()
	{
		this.talkController = (IUSpeakTalkController)this.FindInputHandler();
	}

	public void DrawTalkControllerUI()
	{
		if (this.talkController != null)
		{
			this.talkController.OnInspectorGUI();
		}
		else
		{
			GUILayout.Label("No component available which implements IUSpeakTalkController\nReverting to default behavior - data is always sent", new GUILayoutOption[0]);
		}
	}

	public void ReceiveAudio(byte[] data)
	{
		if (this.settings == null)
		{
			UnityEngine.Debug.LogWarning("Trying to receive remote audio data without calling InitializeSettings!\nIncoming packet will be ignored");
		}
		if (this.MuteAll || this.Mute || (this.SpeakerMode == SpeakerMode.Local && !this.DebugPlayback))
		{
			return;
		}
		if (this.SpeakerMode == SpeakerMode.Remote)
		{
			this.talkTimer = 1f;
		}
		byte[] array;
		for (int i = 0; i < data.Length; i += array.Length)
		{
			int num = BitConverter.ToInt32(data, i);
			array = new byte[num + 6];
			Array.Copy(data, i, array, 0, array.Length);
			USpeakFrameContainer uspeakFrameContainer = default(USpeakFrameContainer);
			uspeakFrameContainer.LoadFrom(array);
			this.playBuffer.Add(USpeakAudioClipCompressor.DecompressAudioClip(uspeakFrameContainer.encodedData, (int)uspeakFrameContainer.Samples, 1, false, this.settings.bandMode, this.RemoteGain));
		}
	}

	public void InitializeSettings(int data)
	{
		MonoBehaviour.print("Settings changed");
		this.settings = new USpeakSettingsData((byte)data);
	}

	private void Awake()
	{
		USpeaker.USpeakerList.Add(this);
	}

	private void OnDestroy()
	{
		USpeaker.USpeakerList.Remove(this);
	}

	private IEnumerator Start()
	{
		yield return null;
		this.audioHandler = (ISpeechDataHandler)this.FindSpeechHandler();
		this.talkController = (IUSpeakTalkController)this.FindInputHandler();
		if (this.audioHandler == null)
		{
			UnityEngine.Debug.LogError("USpeaker requires a component which implements the ISpeechDataHandler interface");
			yield break;
		}
		if (this.SpeakerMode == SpeakerMode.Remote)
		{
			yield break;
		}
		if (this.AskPermission && !Application.HasUserAuthorization(UserAuthorization.Microphone))
		{
			yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
		}
		if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
		{
			UnityEngine.Debug.LogError("Failed to start recording - user has denied microphone access");
			yield break;
		}
		if (Microphone.devices.Length == 0)
		{
			UnityEngine.Debug.LogWarning("Failed to find a recording device");
			yield break;
		}
		this.UpdateSettings();
		this.sendt = 1f / this.SendRate;
		this.recording = Microphone.Start(this.currentDeviceName, false, 1000, this.audioFrequency);
		MonoBehaviour.print(Microphone.devices[USpeaker.InputDeviceID]);
		this.currentDeviceName = Microphone.devices[USpeaker.InputDeviceID];
		yield break;
	}

	private void Update()
	{
		this.talkTimer -= Time.deltaTime;
		this.playbuffTimer += Time.deltaTime;
		if (this.playbuffTimer >= this.PlayBufferSize)
		{
			this.playbuffTimer = 0f;
			uint num = 0u;
			foreach (AudioClip audioClip in this.playBuffer)
			{
				USpeakAudioManager.PlayClipAtPoint(audioClip, base.transform.position, (ulong)num, this.settings.Is3D);
				num += (uint)(44100f / (float)this.audioFrequency * audioClip.samples);
			}
			this.playBuffer.Clear();
		}
		if (this.SpeakerMode == SpeakerMode.Remote)
		{
			return;
		}
		if (this.audioHandler == null)
		{
			return;
		}
		if (Microphone.devices.Length == 0)
		{
			return;
		}
		if (Microphone.devices[Mathf.Min(USpeaker.InputDeviceID, Microphone.devices.Length - 1)] != this.currentDeviceName)
		{
			this.currentDeviceName = Microphone.devices[Mathf.Min(USpeaker.InputDeviceID, Microphone.devices.Length - 1)];
			MonoBehaviour.print("Using input device: " + this.currentDeviceName);
			this.recording = Microphone.Start(this.currentDeviceName, false, 1000, this.audioFrequency);
			this.lastReadPos = 0;
		}
		int num2 = Microphone.GetPosition(null);
		if (num2 >= this.audioFrequency * 1000)
		{
			num2 = 0;
			this.lastReadPos = 0;
			Microphone.End(null);
			this.recording = Microphone.Start(this.currentDeviceName, false, 1000, this.audioFrequency);
		}
		if (num2 <= this.overlap)
		{
			return;
		}
		try
		{
			int num3 = num2 - this.lastReadPos;
			if (num3 > 1)
			{
				float[] array = new float[num3 - 1];
				this.recording.GetData(array, this.lastReadPos);
				if (this.talkController == null || this.talkController.ShouldSend())
				{
					this.talkTimer = 1f;
					this.OnAudioAvailable(array);
				}
			}
			this.lastReadPos = num2;
		}
		catch (Exception)
		{
		}
		bool flag = true;
		if (this.SendingMode == SendBehavior.RecordThenSend && this.talkController != null)
		{
			flag = !this.talkController.ShouldSend();
		}
		this.sendTimer += Time.deltaTime;
		if (this.sendTimer >= this.sendt && flag)
		{
			this.sendTimer = 0f;
			this.tempSendBytes.Clear();
			foreach (USpeakFrameContainer uspeakFrameContainer in this.sendBuffer)
			{
				this.tempSendBytes.AddRange(uspeakFrameContainer.ToByteArray());
			}
			this.sendBuffer.Clear();
			if (this.tempSendBytes.Count > 0)
			{
				this.audioHandler.USpeakOnSerializeAudio(this.tempSendBytes.ToArray());
			}
		}
	}

	private void UpdateSettings()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.settings = new USpeakSettingsData();
		this.settings.bandMode = this.BandwidthMode;
		this.settings.Is3D = this.Is3D;
		this.audioHandler.USpeakInitializeSettings((int)this.settings.ToByte());
	}

	private Component FindSpeechHandler()
	{
		Component[] components = base.GetComponents<Component>();
		foreach (Component component in components)
		{
			if (component is ISpeechDataHandler)
			{
				return component;
			}
		}
		return null;
	}

	private Component FindInputHandler()
	{
		Component[] components = base.GetComponents<Component>();
		foreach (Component component in components)
		{
			if (component is IUSpeakTalkController)
			{
				return component;
			}
		}
		return null;
	}

	private void OnAudioAvailable(float[] pcmData)
	{
		if (this.UseVAD && !this.CheckVAD(pcmData))
		{
			return;
		}
		AudioClip audioClip = AudioClip.Create("temp", pcmData.Length, 1, this.audioFrequency, false, false);
		audioClip.SetData(pcmData, 0);
		int num;
		byte[] encodedData = USpeakAudioClipCompressor.CompressAudioClip(audioClip, out num, this.BandwidthMode, this.LocalGain);
		USpeakFrameContainer item = default(USpeakFrameContainer);
		item.Samples = (ushort)num;
		item.encodedData = encodedData;
		this.sendBuffer.Add(item);
	}

	private int CalculateSamplesRead(int readPos)
	{
		if (readPos >= this.lastReadPos)
		{
			return readPos - this.lastReadPos;
		}
		return this.audioFrequency * 10 - this.lastReadPos + readPos;
	}

	private bool CheckVAD(float[] samples)
	{
		if (Time.realtimeSinceStartup < this.lastVTime + this.vadHangover)
		{
			return true;
		}
		float num = 0f;
		foreach (float f in samples)
		{
			num = Mathf.Max(num, Mathf.Abs(f));
		}
		bool flag = num >= 0.005f;
		if (flag)
		{
			this.lastVTime = Time.realtimeSinceStartup;
		}
		return flag;
	}

	public float RemoteGain = 1f;

	public float LocalGain = 1f;

	public bool MuteAll;

	public static List<USpeaker> USpeakerList = new List<USpeaker>();

	private static int InputDeviceID = 0;

	public SpeakerMode SpeakerMode;

	public BandMode BandwidthMode;

	public float SendRate = 16f;

	public SendBehavior SendingMode;

	public bool UseVAD;

	public bool Is3D;

	public bool DebugPlayback;

	public bool AskPermission = true;

	public bool Mute;

	public float PlayBufferSize = 1f;

	private AudioClip recording;

	private int recFreq;

	private int lastReadPos;

	private List<AudioClip> playBuffer = new List<AudioClip>();

	private float playbuffTimer;

	private float sendTimer;

	private float sendt = 1f;

	private List<USpeakFrameContainer> sendBuffer = new List<USpeakFrameContainer>();

	private List<byte> tempSendBytes = new List<byte>();

	private ISpeechDataHandler audioHandler;

	private IUSpeakTalkController talkController;

	private int overlap;

	private USpeakSettingsData settings;

	private string currentDeviceName = string.Empty;

	private float talkTimer;

	private float vadHangover = 0.5f;

	private float lastVTime;
}
