using System;
using kube;
using UnityEngine;

public class HeadPanel : MonoBehaviour
{
	private void Start()
	{
		if (!this.isInit)
		{
			this.Init();
		}
	}

	private void OnEnable()
	{
		if (!this.isInit)
		{
			this.Init();
		}
	}

	private void OnDisable()
	{
		this.select(null);
	}

	private void Init()
	{
		for (int i = 0; i < this.btn.Length; i++)
		{
			EventDelegate.Add(this.btn[i].onChange, new EventDelegate(new EventDelegate.Callback(this.onMenu)));
			this.btn[i].optionCanBeNone = true;
		}
		this.select(null);
		this.isInit = true;
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

	public void onMenu()
	{
		UIToggle uitoggle = UIToggle.current;
		if (!uitoggle.value)
		{
			return;
		}
		this.select(uitoggle);
	}

	public void onMenuNum(int numMenu)
	{
		this.btn[numMenu].value = true;
		UIToggle uitoggle = this.btn[numMenu];
		if (!uitoggle.value)
		{
			return;
		}
		this.select(uitoggle);
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

	public void onOptions()
	{
		this.options_dialog.SetActive(true);
	}

	private void select(UIToggle next)
	{
		int num = Array.IndexOf<UIToggle>(this.btn, next);
		if (next != this.current)
		{
			for (int i = 0; i < this.btn.Length; i++)
			{
				this.tab[i].gameObject.SetActive(num == i);
				this.btn[i].optionCanBeNone = true;
				this.btn[i].value = (num == i);
				this.btn[i].optionCanBeNone = false;
			}
		}
		this.current = next;
		bool flag = num == -1;
		this.home.SetActive(flag);
		bool flag2 = !flag;
		HeadTab headTab = null;
		if (!flag)
		{
			headTab = this.tab[num].GetComponent<HeadTab>();
		}
		if (headTab)
		{
			flag2 = (flag2 && !headTab.hideCloseButton);
		}
		this.close.gameObject.SetActive(flag2);
		this.toolbar.SetActive(flag);
	}

	public void CloseAll()
	{
		this.select(null);
	}

	public UIToggle[] btn;

	public GameObject[] tab;

	public GameObject home;

	public UIButton close;

	public GameObject toolbar;

	public GameObject options_dialog;

	public UILabel money1;

	public UILabel money2;

	public UIToggle fullscreenToggle;

	protected bool isInit;

	private UIToggle current;

	private int _playerMoney1;

	private int _playerMoney2;

	private bool isUpdate;
}
