using System;
using System.Collections;
using kube;
using kube.data;
using UnityEngine;

public class WeaponSkinDialog : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		this.index = 0;
		string[] array = new string[]
		{
			Localize.is_one_day,
			Localize.is_one_week,
			Localize.is_unlimit
		};
		this.weaponSkins = WeaponSkins.select(this.weaponId);
		for (int i = 0; i < this.weaponSkins.Length; i++)
		{
			if (Kube.GPS.weaponsCurrentSkin[this.weaponId] == this.weaponSkins[i].id)
			{
				this.index = i;
				break;
			}
		}
		this.title.text = Localize.weaponNames[this.weaponId];
		this.ShowSkin();
	}

	private void ShowSkin()
	{
		base.StartCoroutine(this._loadTx());
		bool flag = this.index == 0;
		int id = this.weaponSkins[this.index].id;
		if (id >= 0 && Kube.GPS.weaponsSkin[id] != 0)
		{
			flag = true;
		}
		if (!flag)
		{
			this.buttonBuy.GetComponent<PriceButton>().value = Kube.GPS.weaponsSkinPrice1[id];
		}
		this.buttonBuy.SetActive(!flag);
		this.buttonUse.SetActive(flag && Kube.GPS.weaponsCurrentSkin[this.weaponId] != id);
		if (!flag)
		{
			PriceButton component = this.buttonBuy.GetComponent<PriceButton>();
			component.isGold = (Kube.GPS.weaponsSkinPrice2[id] != 0);
			component.value = ((!component.isGold) ? Kube.GPS.weaponsSkinPrice1[id] : Kube.GPS.weaponsSkinPrice2[id]);
		}
	}

	public void onBuyClick()
	{
		GameObject gameObject = UIButton.current.gameObject;
		int id = this.weaponSkins[this.index].id;
		int num = Kube.GPS.weaponsSkinPrice2[id];
		bool flag = true;
		if (num == 0)
		{
			num = Kube.GPS.weaponsSkinPrice1[id];
			flag = false;
		}
		int num2;
		if (flag)
		{
			num2 = Kube.GPS.playerMoney2;
		}
		else
		{
			num2 = Kube.GPS.playerMoney1;
		}
		if (num > num2)
		{
			MainMenu.ShowBank();
			return;
		}
		Kube.SS.BuyWeaponSkin(this.weaponId, id, Kube.IS.gameObject, "BuyWeaponSkinDone");
		Kube.SN.PostWeaponSkinOnWall(id);
	}

	private void onUnlockClick()
	{
	}

	private void WeaponsUpdate()
	{
		this.ShowSkin();
	}

	public void onUseClick()
	{
		base.gameObject.SetActive(false);
		int id = this.weaponSkins[this.index].id;
		Kube.SS.UseWeaponSkin(this.weaponId, id, Kube.IS.gameObject, "UseWeaponSkinDone");
	}

	public void onLRClick()
	{
		int num = 1;
		if (UIButton.current == this.left)
		{
			num = -1;
		}
		this.index += num;
		if (this.index < 0)
		{
			this.index = this.weaponSkins.Length + this.index;
		}
		this.index %= this.weaponSkins.Length;
		this.ShowSkin();
	}

	private IEnumerator _loadTx()
	{
		while (Kube.ASS2 == null)
		{
			yield return new WaitForSeconds(2f);
		}
		Texture texture = null;
		int id = this.weaponSkins[this.index].id;
		if (id == -1)
		{
			texture = Kube.ASS2.inventarWeaponsTex[this.weaponId];
		}
		else
		{
			texture = Kube.ASS2.inventarWeaponsSkinTex[id];
		}
		this.tx.mainTexture = texture;
		yield break;
	}

	public int weaponId;

	public UITexture tx;

	public GameObject buttonBuy;

	public GameObject buttonUse;

	public UILabel title;

	public UILabel desc;

	public UIButton left;

	public UIButton right;

	private int index;

	private WSkinDescObj[] weaponSkins;
}
