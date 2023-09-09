using System;
using kube;
using UnityEngine;

public class UIHUD : MonoBehaviour
{
	private void Start()
	{
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
			null,
			this.shooterStats,
			this.survivalStats,
			this.teamsStats,
			gameObject,
			this.ctfStats,
			this.dominatingStats
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
		this.modes.gameObject.SetActive(Kube.GPS.isVIP && this.isCreating);
		this.specItems.SetActive(!this.isCreating);
		this.cubes.SetActive(this.isCreating);
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
			int currentWeapon = Kube.BCS.ps.currentWeapon;
			if (currentWeapon == -1 || currentWeapon >= Kube.IS.weaponParams.Length)
			{
				this.ammo.gameObject.SetActive(false);
			}
			else if (Kube.IS.weaponParams[currentWeapon].UsingBullets <= 0)
			{
				this.ammo.gameObject.SetActive(false);
			}
			else
			{
				this.ammo.gameObject.SetActive(!this.isCreating);
				int bulletsType = Kube.IS.weaponParams[currentWeapon].BulletsType;
				if (Kube.IS.weaponParams[currentWeapon].UsingBullets > 0)
				{
					int num = ps.bullets[bulletsType];
					this.ammo.label.text = ps.clips[currentWeapon] + "/" + num;
					this.ammo.sprite.spriteName = this.ammo.names[bulletsType];
				}
			}
			this.frags.text = ps.kills.ToString();
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

	public HUDAmmo ammo;

	public GameObject score;

	public HUDTeams teams;

	public UILabel frags;

	public HUDTimer timer;

	public HUDTimer SurvTimer;

	public HUDValue jetpack;

	public HUDCreatingMode modes;

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
