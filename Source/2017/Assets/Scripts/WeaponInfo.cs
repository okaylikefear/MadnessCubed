using System;
using kube;
using kube.data;
using UnityEngine;

public class WeaponInfo : MonoBehaviour
{
	private void Start()
	{
		if (this.upgrade_dialog == null)
		{
			this.upgrade_dialog = Cub2Menu.Find<UpgradeWeaponDialog>("dialog_upgrade_weapon");
		}
	}

	private void OnEnable()
	{
	}

	public void onSkinClick()
	{
		WeaponSkinDialog weaponSkinDialog = Cub2Menu.Find<WeaponSkinDialog>("dialog_weapon_skin");
		weaponSkinDialog.weaponId = this.weaponId;
		weaponSkinDialog.gameObject.SetActive(true);
	}

	public void ShowWeapon(int weaponId)
	{
		this.weaponId = weaponId;
		this.ammotype.spriteName = "p_" + Kube.IS.weaponParams[weaponId].BulletsType.ToString();
		this.ammotype.alpha = ((Kube.IS.weaponParams[weaponId].UsingBullets <= 0) ? 0f : 255f);
		WeaponParamsObj weaponParamsObj = Kube.IS.weaponParams[weaponId];
		WeaponUpgradeData upgradeData = WeaponUpgrade.getUpgradeData(weaponId);
		this.prms[0].value = Mathf.Round(upgradeData.upgradeValue[0]);
		this.prms[1].value = Mathf.Round(77.7f * Mathf.Pow(1f / (upgradeData.upgradeValue[1] * 100f), 0.25f));
		this.prms[2].value = Mathf.Round(10f / upgradeData.upgradeValue[2]) / 10f;
		this.prms[3].value = Mathf.Round(upgradeData.upgradeValue[3]);
		this.prms[4].value = Mathf.Round(upgradeData.upgradeValue[4]);
		float num = 2592000f;
		bool flag = Kube.GPS.inventarWeapons[weaponId] == 1 || (float)Kube.GPS.inventarWeapons[weaponId] > Time.time;
		bool flag2 = (float)Kube.GPS.inventarWeapons[weaponId] > Time.time + num;
		this.price.gameObject.SetActive(!flag);
		this.timer.gameObject.SetActive(flag);
		this.timer.value = Kube.GPS.inventarWeapons[weaponId];
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < 3; i++)
		{
			num2 = Kube.GPS.weaponsPrice1[weaponId, i];
			num3 = Kube.GPS.weaponsPrice2[weaponId, i];
			if (num2 > 0 || num3 > 0)
			{
				break;
			}
		}
		int num4 = num2;
		if (num4 == 0)
		{
			this.price.isGold = true;
			num4 = num3;
		}
		else
		{
			this.price.isGold = false;
		}
		if (!flag2)
		{
			this.price.value = num4;
		}
		for (int j = 0; j < upgradeData.upgradeAvail.Length; j++)
		{
			bool active = upgradeData.upgradeAvail[j] > upgradeData.upgradeIndex[j] + 1;
			this.prms[j].button.gameObject.SetActive(active);
		}
		this.prms[1].gameObject.SetActive(upgradeData.upgradeValue[1] > 0f && Kube.IS.weaponParams[weaponId].UsingBullets > 0);
		this.prms[3].gameObject.SetActive(Kube.IS.weaponParams[weaponId].UsingBullets > 0);
		this.prms[4].gameObject.SetActive(Kube.IS.weaponParams[weaponId].UsingBullets > 0);
		for (int k = 0; k < this.prms.Length; k++)
		{
			EventDelegate.Add(this.prms[k].button.onClick, new EventDelegate(new EventDelegate.Callback(this.onUpgrade)));
		}
	}

	private void WeaponUpgradeEvent()
	{
		this.ShowWeapon(this.weaponId);
	}

	private void WeaponsUpdate()
	{
		this.ShowWeapon(this.weaponId);
	}

	private void onUpgrade()
	{
		int num = -1;
		for (int i = 0; i < this.prms.Length; i++)
		{
			if (this.prms[i].button == UIButton.current)
			{
				num = i;
				break;
			}
		}
		float num2 = 2592000f;
		if (num != -1)
		{
			bool flag = (float)Kube.GPS.inventarWeapons[this.weaponId] > Time.time + num2;
			if (flag)
			{
				this.upgrade_dialog.Show(this.weaponId, num);
			}
			else
			{
				Cub2UI.MessageBox(Localize.no_weapon_upgrade, null);
			}
		}
	}

	[ContextMenu("collect")]
	private void CollectParams()
	{
		UISlider[] componentsInChildren = base.GetComponentsInChildren<UISlider>();
		this.prms = new WeaponParams[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			WeaponParams weaponParams;
			if ((weaponParams = componentsInChildren[i].GetComponent<WeaponParams>()) == null)
			{
				weaponParams = componentsInChildren[i].gameObject.AddComponent<WeaponParams>();
			}
			weaponParams.slider = componentsInChildren[i];
			weaponParams.button = componentsInChildren[i].GetComponentInChildren<UIButton>();
			weaponParams.label = componentsInChildren[i].GetComponentsInChildren<UILabel>()[1];
			this.prms[i] = weaponParams;
		}
	}

	public UILabel title;

	public UITexture tx;

	public UISprite ammotype;

	public WeaponParams[] prms;

	public UpgradeWeaponDialog upgrade_dialog;

	public PriceButton price;

	public ExpireTimer timer;

	protected int weaponId;
}
