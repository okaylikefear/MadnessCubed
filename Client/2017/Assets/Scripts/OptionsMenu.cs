using System;
using kube;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
	private void Start()
	{
		if (!this._init)
		{
			this.Init();
		}
	}

	private void Init()
	{
		Resolution[] resolutions = Screen.resolutions;
		this.resolutionNames = new string[resolutions.Length];
		for (int i = 0; i < resolutions.Length; i++)
		{
			this.resolutionNames[i] = resolutions[i].width + "x" + resolutions[i].height;
		}
		this.resolution.states = this.resolutionNames;
		platformType platform = Kube.SN.platform;
		for (int j = 0; j < this.specific.Length; j++)
		{
			for (int k = 0; k < this.specific[j].objects.Length; k++)
			{
				this.specific[j].objects[k].SetActive(Array.IndexOf<platformType>(this.specific[j].pt, platform) != -1);
			}
		}
		if (base.gameObject.activeSelf)
		{
			this.OnShow();
		}
		this._init = true;
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		if (!this._init)
		{
			this.Init();
		}
		this.OnShow();
	}

	public void onVolumeChange()
	{
		AudioListener.volume = this.sounds.value;
	}

	public void onMusicChange()
	{
		MusicManagerScript component = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManagerScript>();
		component.GetComponent<AudioSource>().volume = this.music.value;
	}

	private void OnShow()
	{
		float @float = PlayerPrefs.GetFloat("mouseSens", 1f);
		MusicManagerScript component = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManagerScript>();
		float float2 = PlayerPrefs.GetFloat("soundVol", AudioListener.volume);
		float float3 = PlayerPrefs.GetFloat("musicVol", component.GetComponent<AudioSource>().volume);
		string[] array = new string[QualitySettings.names.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Localize.graphStrs[i];
		}
		this.quality.states = array;
		this.quality.index = QualitySettings.GetQualityLevel();
		int index = PlayerPrefs.GetInt("screen", 0);
		Resolution[] resolutions = Screen.resolutions;
		for (int j = 0; j < resolutions.Length; j++)
		{
			if (Screen.width == resolutions[j].width && Screen.height == resolutions[j].height)
			{
				index = j;
			}
		}
		this.resolution.states = this.resolutionNames;
		this.resolution.index = index;
		this.sounds.value = float2;
		this.music.value = float3;
		this.mouse.value = (@float - 1f) / 15f;
	}

	public void OnDisable()
	{
		this.OnApply();
	}

	public void OnApply()
	{
		if (QualitySettings.GetQualityLevel() != this.quality.index)
		{
			QualitySettings.SetQualityLevel(this.quality.index, true);
		}
		if (Screen.resolutions != null && this.resolution.index > 0 && this.resolution.index < Screen.resolutions.Length)
		{
			Kube.OH.screenResolution = Screen.resolutions[this.resolution.index];
		}
		float num = this.mouse.value * 15f + 1f;
		PlayerPrefs.SetFloat("mouseSens", num);
		PlayerPrefs.SetFloat("soundVol", this.sounds.value);
		PlayerPrefs.SetFloat("musicVol", this.music.value);
		PlayerPrefs.SetInt("dynstick", (!this.dynstick.value) ? 0 : 1);
		PlayerPrefs.SetInt("autoaim", (!this.autoaim.value) ? 0 : 1);
		PlayerPrefs.SetInt("screen", this.resolution.index);
		if (Screen.fullScreen)
		{
			Screen.SetResolution(Kube.OH.screenResolution.width, Kube.OH.screenResolution.height, true);
		}
		Kube.GPS.mouseSens = num;
		Kube.OH.autoaim = this.autoaim.value;
		KubeInput.Reset();
		Kube.SendMonoMessage("ApplyOptions", new object[0]);
	}

	public void onLogout()
	{
		Kube.SN.Logout();
	}

	public void onEmpty()
	{
		Kube.OH.emptyScreen = UIToggle.current.value;
	}

	public UISlider sounds;

	public UISlider music;

	public UISlider mouse;

	public UIToggle autoaim;

	public UIToggle dynstick;

	public LRButton quality;

	public LRButton resolution;

	protected string[] resolutionNames;

	public OptionsMenu.PlatformSpecific[] specific;

	private bool _init;

	[Serializable]
	public class PlatformSpecific
	{
		public platformType[] pt;

		public GameObject[] objects;
	}
}
