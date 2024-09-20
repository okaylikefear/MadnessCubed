using System;
using kube;
using UnityEngine;

public class UIHUD : MonoBehaviour
{
	private void Awake()
	{
		Kube.AddListener(base.gameObject);
	}

	private void OnEnable()
	{
		KubeInput.Reset();
	}

	private void OnDisable()
	{
		KubeInput.Reset();
	}

	private void OnDestroy()
	{
		Kube.RemoveListener(base.gameObject);
	}

	public bool isVisible
	{
		get
		{
			return this._isVisible;
		}
		set
		{
			this._isVisible = value;
			base.gameObject.SetActive(this._isVisible);
		}
	}

	public void Init()
	{
		GameObject gameObject = this.mission0Stats;
		GameObject[] array = new GameObject[]
		{
			null,
			this.creatingStats,
			this.shooterStats,
			this.survivalStats,
			this.teamsStats,
			gameObject,
			this.ctfStats,
			this.dominatingStats,
			this.teamsStats,
			this.shooterStats,
			this.teamsStats
		};
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i])
			{
				array[i].SetActive(false);
			}
		}
		this.timer.gameObject.SetActive(false);
		if (array[(int)Kube.BCS.gameType])
		{
			this.curstat = array[(int)Kube.BCS.gameType].GetComponent<HUDStatus>();
		}
		if (this.curstat)
		{
			this.curstat.gameObject.SetActive(true);
		}
		this._weaponsPanel = this.weapons.GetComponent<UIPanel>();
		this._weaponsPanel.alpha = 0f;
	}

	private void EventBuyVIPDone()
	{
		this.isCreating = (Kube.BCS.gameType == GameType.creating);
		this.modes.gameObject.SetActive(Kube.GPS.isVIP && this.isCreating);
	}

	public void BeginGame()
	{
		if (Kube.BCS.gameTypeController is TeamControllerBase)
		{
			this.teams.gameObject.SetActive(true);
			this.teams.BeginGame();
		}
		else
		{
			this.teams.gameObject.SetActive(false);
		}
		this.isCreating = (Kube.BCS.gameType == GameType.creating);
		bool flag = Kube.BCS.gameMode == GameMode.cooperative;
		this.specItems.SetActive(!this.isCreating);
		this.cubes.SetActive(this.isCreating);
		this.frags.transform.parent.gameObject.SetActive(!flag && !this.isCreating);
		this.ammo.gameObject.SetActive(!this.isCreating);
	}

	private void Update()
	{
		PlayerScript ps = Kube.BCS.ps;
		if (ps == null)
		{
			return;
		}
		this.hp.value = ps.health;
		this.armor.value = ps.armor;
		if (ps)
		{
			WeaponParamsObj currentWeapon = Kube.BCS.ps.currentWeapon;
			if (currentWeapon == null)
			{
				this.ammo.gameObject.SetActive(false);
			}
			else if (currentWeapon.UsingBullets <= 0)
			{
				this.ammo.gameObject.SetActive(false);
			}
			else
			{
				this.ammo.gameObject.SetActive(!this.isCreating);
				int bulletsType = currentWeapon.BulletsType;
				if (currentWeapon.UsingBullets > 0)
				{
					int num = ps.bullets[bulletsType];
					this.ammo.label.text = ps.clips[currentWeapon.id] + "/" + num;
					this.ammo.sprite.spriteName = this.ammo.names[bulletsType];
				}
			}
			if (Kube.BCS.gameType == GameType.survival)
			{
				this.frags.text = ps.kills.ToString();
			}
			else
			{
				this.frags.text = ps.frags.ToString();
			}
		}
		if (this._hideWeapon < Time.time)
		{
			if (this._weaponsPanel.alpha > 0f)
			{
				this._weaponsPanel.alpha -= 0.05f;
			}
			if (this._weaponsPanel.alpha < 0f)
			{
				this._weaponsPanel.alpha = 0f;
			}
		}
	}

	public void ChoseWeapon(int num)
	{
		this._weaponsPanel.alpha = 1f;
		this._hideWeapon = Time.time + 5f;
		this.weapons.GetComponent<FastInventarPanel>().CurrentSlot(num);
	}

	public void ChoseCube(int num)
	{
		this.cubes.GetComponent<FastInventarPanel>().CurrentSlot(num);
	}

	public HUDBar hp;

	public HUDBar armor;

	public HUDTasks tasks;

	public HUDAmmo ammo;

	public GameObject score;

	public HUDTeams teams;

	public UILabel frags;

	public HUDTimer timer;

	public HUDTimer SurvTimer;

	public HUDValue jetpack;

	public HUDCreatingModeBase modes;

	public GameObject creatingStats;

	public GameObject shooterStats;

	public GameObject survivalStats;

	public GameObject teamsStats;

	public GameObject ctfStats;

	public GameObject dominatingStats;

	public GameObject mission0Stats;

	public GameObject specItems;

	public GameObject cubes;

	public GameObject weapons;

	protected UIPanel _weaponsPanel;

	public UITexture aim;

	public GameObject tutorialMessage;

	protected bool isCreating;

	[HideInInspector]
	public HUDStatus curstat;

	protected bool _isVisible;

	protected float _hideWeapon;
}
