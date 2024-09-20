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

	public GUISkin mainSkinWhite;

	public GUISkin mainSkinYellow;

	public GUISkin emptySkin;

	public GUISkin mainSkinSmallLeft;

	public GUISkin mainSkinSmallRight;

	public GUISkin buttonArrowSkin;

	public GUISkin yellowButton;

	public GUISkin blueButtonSkin;

	public GUISkin bigWhiteLabel;

	public GUISkin bigWhiteLabelLeft;

	public GUISkin bigBlackLabel;

	public GUISkin smallWhiteSkin;

	public GUISkin smallBlackCenterSkin;

	public GUISkin triggerSkin;

	public GUISkin triggerSkinArrowLeft;

	public GUISkin triggerSkinArrowRight;

	public GUISkin missionSkin;

	public Texture[] menuLogo;

	public Texture menuBack;

	public Texture menuFrame;

	public Texture tabTex;

	public Texture tabLightTex;

	public Texture buyForMoney;

	public Texture buyForGold;

	public Texture playerMoneyTex;

	public Texture playerGoldTex;

	public Texture levelLine;

	public Texture levelProgress;

	public Texture levelStar;

	public Texture[] dayLightTex;

	public Texture inventoryTex;

	public Texture anticheatPict;

	public Texture netBattleWindow;

	public Texture[] newMapTypeTex;

	public Texture lockGameTex;

	public Texture inventorySideTex;
}
