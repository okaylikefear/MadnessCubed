using System;
using kube;
using UnityEngine;

public class HeadPanel : MonoBehaviour
{
	private void Awake()
	{
		this.isUpdate = true;
		this.select(null, true, false);
	}

	private void Start()
	{
		this.isUpdate = true;
		this.fullscreenToggle.value = Screen.fullScreen;
		if (!this.isInit)
		{
			this.Init();
		}
	}

	public void onQuit()
	{
		MessageBox yesno = Cub2UI.FindAndOpenDialog<MessageBox>("dialog_yesno");
		yesno.title.text = Localize.quit_yesno;
		EventDelegate.Callback call = delegate()
		{
			if (yesno.modalResult == 1)
			{
				UnityEngine.Debug.Log("Quit");
				Application.Quit();
			}
		};
		yesno.handler = new EventDelegate(call);
	}

	private void OnEnable()
	{
		if (!this.isInit)
		{
			this.Init();
		}
		PlatformType platformType = KubeApp.detectPlatform();
		if (this.quit)
		{
			this.quit.SetActive(platformType == PlatformType.Mobile || platformType == PlatformType.PC);
		}
		if (this.fullscreenToggle)
		{
			this.fullscreenToggle.gameObject.SetActive(platformType == PlatformType.Web || platformType == PlatformType.PC);
		}
		if (Kube.SN.isVIPPlatform && this.vip)
		{
			this.vip.SetActive(false);
		}
		UIGrid component = base.GetComponent<UIGrid>();
		if (component)
		{
			component.Reposition();
		}
	}

	private void OnDisable()
	{
	}

	private void Init()
	{
		this.isInit = true;
	}

	public void select(GameObject go, bool isHome, bool showClose)
	{
		if (this.current)
		{
			this.current.SetActive(false);
		}
		this.bg.gameObject.SetActive(!isHome);
		PlatformType platformType = KubeApp.detectPlatform();
		Cub2Menu.instance.home.gameObject.SetActive(isHome);
		this.close.gameObject.SetActive(showClose);
		if (this.quit)
		{
			this.quit.SetActive((platformType == PlatformType.Mobile || platformType == PlatformType.PC) && !showClose);
		}
		this.toolbar.SetActive(isHome);
		this.current = go;
		if (go)
		{
			go.SetActive(true);
		}
	}

	public void Update()
	{
		if (Kube.GPS == null)
		{
			return;
		}
		this.isUpdate = true;
		if (this._playerMoney1 != Kube.GPS.playerMoney1)
		{
			this.money1.text = Kube.GPS.playerMoney1.ToString();
			this._playerMoney1 = Kube.GPS.playerMoney1;
		}
		if (this._playerMoney2 != Kube.GPS.playerMoney2)
		{
			this.money2.text = Kube.GPS.playerMoney2.ToString();
			this._playerMoney2 = Kube.GPS.playerMoney2;
		}
		if (this.fullscreenToggle)
		{
			this.fullscreenToggle.value = Screen.fullScreen;
		}
		this.isUpdate = false;
	}

	public void onFullscreen()
	{
		if (this.isUpdate)
		{
			return;
		}
		Kube.OH.fullScreen = UIToggle.current.value;
	}

	public void onBank()
	{
		MainMenu.ShowBank();
	}

	public void onVIP()
	{
		Cub2UI.FindAndOpenDialog("dialog_vip");
	}

	public void onOptions()
	{
		MonoBehaviour monoBehaviour = Cub2Menu.Find<MonoBehaviour>("Options_menu");
		Cub2Menu.instance.head.select(monoBehaviour.gameObject, false, true);
	}

	public void CloseAll()
	{
		this.select(Cub2Menu.instance.home, true, false);
	}

	public UISprite bg;

	public GameObject quit;

	public GameObject vip;

	public UIButton close;

	public GameObject toolbar;

	public UILabel money1;

	public UILabel money2;

	public UIToggle fullscreenToggle;

	protected bool isInit;

	private GameObject current;

	private int _playerMoney1;

	private int _playerMoney2;

	private bool isUpdate;
}
