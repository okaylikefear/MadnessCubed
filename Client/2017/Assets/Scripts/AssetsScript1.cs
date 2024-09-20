using System;
using kube;
using UnityEngine;

public class AssetsScript1 : AssetBase
{
	private void Awake()
	{
		Kube.ASS1 = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		Kube.ASS1 = null;
	}

	public GUISkin mainSkin;

	public GUISkin mainSkinSmall;

	public GUISkin emptySkin;

	public GUISkin yellowButton;

	public GUISkin bigWhiteLabel;

	public GUISkin bigBlackLabel;

	public GUISkin smallWhiteSkin;

	public GUISkin smallBlackCenterSkin;

	public GUISkin triggerSkin;

	public GUISkin triggerSkinArrowLeft;

	public GUISkin triggerSkinArrowRight;

	public Texture menuBack;

	public Texture menuFrame;

	public Texture tabTex;

	public Texture levelLine;

	public Texture levelProgress;
}
