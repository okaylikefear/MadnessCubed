using System;
using kube;
using UnityEngine;

public class KubeMenu : ShopMenu
{
	private new void Start()
	{
		if (this.fi == null)
		{
			this.fi = base.transform.parent.GetComponentInChildren<FastInventarPanel>();
		}
	}

	public void onPage()
	{
		this.Invalidate();
	}

	private new void onChangeFilter()
	{
		this.fi.stop();
		if (!UIToggle.current.value)
		{
			return;
		}
		this.inventoryPageType = Array.IndexOf<UIToggle>(this.filters, UIToggle.current);
		this.RedrawView();
	}

	private void RedrawView()
	{
		if (this.inventoryPageType == -1)
		{
			return;
		}
		bool flag = true;
		if (this.inventoryPageType < 6)
		{
			flag = (Kube.GPS.cubesTimeOfEnd[this.inventoryPageType] >= Time.time);
		}
		int[] array = Kube.IS.cubesNatureNums;
		if (this.inventoryPageType == 0)
		{
			array = Kube.IS.cubesNatureNums;
		}
		if (this.inventoryPageType == 1)
		{
			array = Kube.IS.cubesBuilderNums;
		}
		if (this.inventoryPageType == 2)
		{
			array = Kube.IS.cubesDecorNums;
		}
		if (this.inventoryPageType == 3)
		{
			array = Kube.IS.cubesGlassNums;
		}
		if (this.inventoryPageType == 4)
		{
			array = Kube.IS.cubesWaterNums;
		}
		if (this.inventoryPageType == 5)
		{
			array = Kube.IS.cubesDifferentNums;
		}
		if (this.inventoryPageType == 6)
		{
			array = Kube.IS.getSpecListNums(InventoryScript.ItemPage.Forms);
		}
		KGUITools.removeAllChildren(this.container.gameObject, true);
		for (int i = 0; i < array.Length; i++)
		{
			if (this.inventoryPageType == 6)
			{
				GameObject gameObject = this.container.gameObject.AddChild(this.specItemPrefab);
				DecorItem component = gameObject.GetComponent<DecorItem>();
				int n = array[i];
				int t = 7;
				component.fi = new FastInventar(t, n);
			}
			else
			{
				GameObject gameObject = this.container.gameObject.AddChild(this.itemPrefab);
				KubeItem component2 = gameObject.GetComponent<KubeItem>();
				int kubeId = array[i];
				component2.kubeId = kubeId;
				UISprite componentInChildren = component2.GetComponentInChildren<UISprite>();
				if (flag)
				{
					componentInChildren.spriteName = "frame_open";
				}
				else
				{
					componentInChildren.spriteName = "frame_closed";
				}
			}
		}
		this.container.GetComponent<PagePanel>().Reposition();
		this.Invalidate();
	}

	private void Invalidate()
	{
		string[] array = new string[]
		{
			Localize.is_one_day,
			Localize.is_one_week,
			Localize.is_unlimit
		};
		bool flag = true;
		if (this.inventoryPageType == -1)
		{
			return;
		}
		if (this.inventoryPageType < Kube.GPS.cubesTimeOfEnd.Length)
		{
			flag = (Kube.GPS.cubesTimeOfEnd[this.inventoryPageType] >= Time.time);
		}
		if (flag)
		{
			for (int i = 0; i < 3; i++)
			{
				this.btn[i].gameObject.SetActive(false);
			}
		}
		else
		{
			for (int j = 0; j < 3; j++)
			{
				this.btn[j].gameObject.SetActive(true);
				if (Kube.GPS.inventarCubesPrice2[this.inventoryPageType, j] == 0)
				{
					this.btn[j].text.text = array[j] + " - " + Kube.GPS.inventarCubesPrice1[this.inventoryPageType, j];
					this.btn[j].isGold = false;
				}
				else
				{
					this.btn[j].text.text = array[j] + " - " + Kube.GPS.inventarCubesPrice2[this.inventoryPageType, j];
					this.btn[j].isGold = true;
				}
			}
		}
	}

	public void onBuyClick()
	{
		PriceButton component = UIButton.current.GetComponent<PriceButton>();
		int num = Array.IndexOf<PriceButton>(this.btn, component);
		if (num == -1)
		{
			return;
		}
		if (Kube.GPS.playerMoney1 < Kube.GPS.inventarCubesPrice1[this.inventoryPageType, num])
		{
			MainMenu.ShowBank();
		}
		else if (Kube.GPS.playerMoney2 < Kube.GPS.inventarCubesPrice2[this.inventoryPageType, num])
		{
			MainMenu.ShowBank();
		}
		else
		{
			Kube.SS.BuyCubes(this.inventoryPageType, num, Kube.IS.gameObject, "BuyCubesDone");
		}
	}

	private void Update()
	{
	}

	private void CubesUpdate()
	{
		this.RedrawView();
	}

	private new void OnEnable()
	{
		for (int i = 0; i < this.filters.Length; i++)
		{
			this.filters[i].GetComponentInChildren<UILabel>().text = Localize.CubesTypes[i];
			this.filters[i].onChange.Add(new EventDelegate(new EventDelegate.Callback(this.onChangeFilter)));
		}
		this.filters[0].value = true;
	}

	public new void onSelectKube(int kubeId)
	{
		UnityEngine.Debug.Log("select" + kubeId.ToString());
		if (Kube.GPS.cubesTimeOfEnd[this.inventoryPageType] >= Time.time)
		{
			this.fi.SelectSlot(new FastInventar(0, kubeId));
		}
		else
		{
			this.fi.stop();
		}
	}

	public override void onBuyKube(FastInventar fi)
	{
		RentItemDialog rentItemDialog = Cub2Menu.Find<RentItemDialog>("dialog_rent_item");
		if (fi.Type == 7 && Kube.GPS.inventarSpecItems[fi.Num] <= 0)
		{
			rentItemDialog.itemId = fi.Num;
			rentItemDialog.gameObject.SetActive(true);
		}
	}

	public GameObject specItemPrefab;

	public PriceButton[] btn;
}
