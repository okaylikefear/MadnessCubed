using System;
using kube;
using UnityEngine;

public class WeaponsMenu : MonoBehaviour
{
	private void Start()
	{
		if (this.popup == null)
		{
			this.popup = Cub2Menu.Find<WeaponDialog>("dialog_gun");
		}
		KGUITools.removeAllChildren(this.container.gameObject, true);
		this.items = new WeaponItem[Kube.IS.inventarWeaponsNums.Length];
		for (int i = 0; i < Kube.IS.inventarWeaponsNums.Length; i++)
		{
			int weaponId = Kube.IS.inventarWeaponsNums[i];
			GameObject gameObject = NGUITools.AddChild(this.container.gameObject, this.itemPrefab);
			WeaponItem component = gameObject.GetComponent<WeaponItem>();
			component.weaponId = weaponId;
			this.items[i] = component;
		}
		for (int j = 0; j < this.filters.Length; j++)
		{
			this.filters[j].onChange.Add(new EventDelegate(new EventDelegate.Callback(this.onFilter)));
		}
		if (base.enabled)
		{
			this.OnEnable();
		}
		this.SelectItems(0);
	}

	private void onFilter()
	{
		if (!UIToggle.current.value)
		{
			return;
		}
		int index = Array.IndexOf<UIToggle>(this.filters, UIToggle.current);
		this.selectedSlot = index;
		this.SelectItems(index);
	}

	private void OnEnable()
	{
		for (int i = 0; i < this.filters.Length; i++)
		{
			if (Kube.GPS.fastInventarWeapon[i].Type == 4)
			{
				this.filters[i].GetComponent<WeaponSlotBtn>().weaponId = Kube.GPS.fastInventarWeapon[i].Num;
			}
		}
		this.info.gameObject.SetActive(false);
	}

	private void SelectItems(int index)
	{
		this.onSelectWeapon(null);
		for (int i = 0; i < this.items.Length; i++)
		{
			WeaponItem weaponItem = this.items[i];
			bool flag = false;
			if (index == (int)Kube.IS.weaponParams[weaponItem.weaponId].weaponGroup)
			{
				flag = true;
			}
			if (Kube.GPS.inventarWeapons[weaponItem.weaponId] <= 0)
			{
				flag = (flag && !Kube.IS.weaponParams[weaponItem.weaponId].hidden);
			}
			weaponItem.gameObject.SetActive(flag);
			if (flag)
			{
				weaponItem.current = (Kube.GPS.fastInventarWeapon[index].Type == 4 && weaponItem.weaponId == Kube.GPS.fastInventarWeapon[index].Num);
			}
		}
		this.container.GetComponent<PagePanel>().Reposition();
	}

	private void WeaponsUpdate()
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			WeaponItem weaponItem = this.items[i];
			weaponItem.Invalidate();
			weaponItem.current = (weaponItem.weaponId == Kube.GPS.fastInventarWeapon[this.selectedSlot].Num);
			if (this.selecteditem != null)
			{
				this.selecteditem.value = true;
			}
		}
	}

	private void Update()
	{
	}

	public void onBuyWeapon(int weaponId)
	{
		this.popup.weaponId = weaponId;
		this.popup.gameObject.SetActive(true);
	}

	public void onUseWeapon(WeaponItem item)
	{
		int weaponId = item.weaponId;
		if (Kube.GPS.inventarWeapons[weaponId] > 0)
		{
			this.filters[this.selectedSlot].GetComponent<WeaponSlotBtn>().weaponId = weaponId;
			Kube.GPS.fastInventarWeapon[this.selectedSlot] = new FastInventar(4, weaponId);
			Kube.SS.SaveFastInventory(1, Kube.GPS.fastInventarWeapon, null);
			if (Kube.BCS && Kube.BCS.ps)
			{
				Kube.BCS.ps.ChangeWeapon(weaponId);
			}
		}
		this.WeaponsUpdate();
	}

	public void onSelectWeapon(WeaponItem item)
	{
		if (this.selecteditem != null)
		{
			this.selecteditem.value = false;
		}
		this.selecteditem = item;
		if (this.selecteditem == null)
		{
			this.info.gameObject.SetActive(false);
			return;
		}
		this.info.gameObject.SetActive(true);
		this.info.title.text = item.title.text;
		this.info.tx.mainTexture = item.tx.mainTexture;
		this.info.ShowWeapon(item.weaponId);
		this.selecteditem = item;
		item.value = true;
	}

	public UIPanel container;

	public GameObject itemPrefab;

	public WeaponDialog popup;

	private WeaponItem[] items;

	public WeaponInfo info;

	protected WeaponItem selectedItem;

	public UIToggle[] filters;

	private WeaponItem[] activeItems;

	protected int selectedSlot;

	protected WeaponItem selecteditem;
}
