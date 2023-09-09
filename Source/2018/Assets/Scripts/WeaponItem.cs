using System;
using System.Collections;
using kube;
using kube.data;
using UnityEngine;

public class WeaponItem : MonoBehaviour
{
	private void Start()
	{
		this.Invalidate();
		this.buy.onClick.Add(new EventDelegate(new EventDelegate.Callback(this.onBuy)));
		this.use.onClick.Add(new EventDelegate(new EventDelegate.Callback(this.onUse)));
	}

	public bool value
	{
		get
		{
			return this._value;
		}
		set
		{
			this.checkmark.alpha = ((!value) ? 0f : 255f);
			this._value = value;
		}
	}

	public bool current
	{
		get
		{
			return this._current;
		}
		set
		{
			this.currentmark.alpha = ((!value) ? 0f : 255f);
			this._current = value;
			if (Kube.GPS.inventarWeapons[this.weaponId] > 0)
			{
				this.use.isEnabled = !this._current;
			}
		}
	}

	public void Invalidate()
	{
		this.value = false;
		this.title.text = Localize.weaponNames[this.weaponId];
		if (Kube.ASS2 == null)
		{
			Kube.RM.require("Assets2", null);
		}
		if (this.loading == null)
		{
			this.loading = this.tx.gameObject.AddChild(Cub2Menu.instance.loadingPrefab);
		}
		this.loading.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		Cub2Menu.instance.StartCoroutine(this._loadTx());
		if (Kube.BCS && Kube.BCS.gameMode != GameMode.cooperative && Kube.BCS.ps && !Kube.BCS.ps.dead)
		{
			this.buy.gameObject.SetActive(false);
			this.use.gameObject.SetActive(false);
		}
		else if (Kube.GPS.inventarWeapons[this.weaponId] > 0)
		{
			this.buy.gameObject.SetActive(false);
			this.use.gameObject.SetActive(true);
		}
		else
		{
			this.use.gameObject.SetActive(false);
			this.buy.gameObject.SetActive(true);
		}
		bool flag = WeaponRang.needParamsUpgrade(this.weaponId);
		this.locked.SetActive(flag);
		if (flag)
		{
			this.locked.GetComponentsInChildren<UILabel>(true)[0].text = string.Format(Localize.need_level, Kube.IS.weaponParams[this.weaponId].needLevel);
		}
		this.use.isEnabled = !this._current;
		float[] upgradeValue = WeaponUpgrade.getUpgradeValue(this.weaponId);
		this.damageLabel.text = upgradeValue[0].ToString("0");
		this.fireRateLabel.text = upgradeValue[1].ToString("0.0");
		int bulletsType = Kube.IS.weaponParams[this.weaponId].BulletsType;
		if (Kube.IS.weaponParams[this.weaponId].UsingBullets > 0)
		{
			this.ammoType.spriteName = "p_" + bulletsType;
		}
		else
		{
			this.ammoType.spriteName = string.Empty;
		}
	}

	private IEnumerator _loadTx()
	{
		while (Kube.ASS2 == null)
		{
			yield return new WaitForSeconds(2f);
		}
		Texture texture = null;
		if (Kube.GPS.weaponsCurrentSkin[this.weaponId] == -1)
		{
			texture = Kube.ASS2.inventarWeaponsTex[this.weaponId];
		}
		else
		{
			texture = Kube.ASS2.inventarWeaponsSkinTex[Kube.GPS.weaponsCurrentSkin[this.weaponId]];
		}
		this.tx.mainTexture = texture;
		if (texture)
		{
			this.tx.width = texture.width;
			this.tx.height = texture.height;
		}
		if (this.loading)
		{
			this.loading.SetActive(false);
		}
		yield break;
	}

	private void Update()
	{
		if (this.tutorS == null)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("SystemGO");
			if (gameObject)
			{
				this.tutorS = gameObject.GetComponent<TutorialScript>();
			}
		}
		if (this.tutorS == null)
		{
			return;
		}
		if (this.tutorS.currentNumOfTutor == this.activateTutorStep && this.weaponId == 0)
		{
			if (this.use.state != UIButtonColor.State.Disabled)
			{
				this.tutorHighlight.SetActive(true);
			}
			else
			{
				this.tutorHighlight.SetActive(false);
			}
		}
	}

	private void onBuy()
	{
		base.transform.parent.parent.GetComponent<WeaponsMenu>().onBuyWeapon(this.weaponId);
	}

	private void onUse()
	{
		base.transform.parent.parent.GetComponent<WeaponsMenu>().onUseWeapon(this);
	}

	private void OnClick()
	{
		base.transform.parent.parent.GetComponent<WeaponsMenu>().onSelectWeapon(this);
	}

	public UILabel title;

	public int weaponId;

	public UITexture tx;

	public UIButton buy;

	public UIButton use;

	public UILabel damageLabel;

	public UILabel fireRateLabel;

	public UISprite ammoType;

	public UISprite checkmark;

	public UISprite currentmark;

	public GameObject tutorHighlight;

	public int activateTutorStep;

	private TutorialScript tutorS;

	public GameObject locked;

	public bool _value;

	public bool _current;

	private GameObject loading;
}
