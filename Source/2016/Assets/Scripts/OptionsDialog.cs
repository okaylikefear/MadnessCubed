using System;
using kube;
using UnityEngine;

public class OptionsDialog : MonoBehaviour
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
		this.resolution.states = this.resolutionNames;
		this.resolution.index = PlayerPrefs.GetInt("screen", 1);
		this.screen.value = Kube.OH.emptyScreen;
		this.smooth.value = Kube.OH.smoothMove;
		this.sounds.value = float2;
		this.music.value = float3;
		this.mouse.value = (@float - 1f) / 15f;
	}

	public void OnApply()
	{
		if (QualitySettings.GetQualityLevel() != this.quality.index)
		{
			QualitySettings.SetQualityLevel(this.quality.index, true);
		}
		Kube.OH.emptyScreen = this.screen.value;
		Kube.OH.smoothMove = this.smooth.value;
		Kube.OH.screenResolution = Screen.resolutions[this.resolution.index];
		float num = this.mouse.value * 15f + 1f;
		PlayerPrefs.SetFloat("mouseSens", num);
		PlayerPrefs.SetFloat("soundVol", this.sounds.value);
		PlayerPrefs.SetFloat("musicVol", this.music.value);
		PlayerPrefs.SetInt("screen", this.resolution.index);
		if (Screen.fullScreen)
		{
			Screen.SetResolution(Kube.OH.screenResolution.width, Kube.OH.screenResolution.height, true);
		}
		Kube.GPS.mouseSens = num;
		base.gameObject.SetActive(false);
	}

	public UISlider sounds;

	public UISlider music;

	public UISlider mouse;

	public UIToggle screen;

	public UIToggle smooth;

	public LRButton quality;

	public LRButton resolution;

	protected string[] resolutionNames;

	private bool _init;
}
