using System;
using kube;
using UnityEngine;

public class KubeMenu : MonoBehaviour
{
	private void Start()
	{
		if (this.fi == null)
		{
			this.fi = base.transform.parent.GetComponentInChildren<FastInventarPanel>();
		}
	}

	private void onChangeFilter()
	{
		this.fi.stop();
		if (!UIToggle.current.value)
		{
			return;
		}
		this.inventoryPageType = Array.IndexOf<UIToggle>(this.filters, UIToggle.current);
		if (this.inventoryPageType == -1)
		{
			return;
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
			array = Kube.IS.cubesDecorNums;
		}
		KGUITools.removeAllChildren(this.container.gameObject, true);
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = NGUITools.AddChild(this.container.gameObject, this.itemPrefab);
			KubeItem component = gameObject.GetComponent<KubeItem>();
			int kubeId = array[i];
			component.kubeId = kubeId;
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
		if (Kube.GPS.cubesTimeOfEnd[this.inventoryPageType] >= Time.time)
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
				this.btn[j].text.text = array[j] + " - " + Kube.GPS.inventarCubesPrice[this.inventoryPageType, j];
				this.btn[j].isGold = false;
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
		if (Kube.GPS.playerMoney1 >= Kube.GPS.inventarCubesPrice[this.inventoryPageType, 1])
		{
			Kube.SS.BuyCubes(this.inventoryPageType, 1, Kube.IS.gameObject, "BuyCubesDone");
		}
	}

	private void Update()
	{
	}

	private void CubesUpdate()
	{
		this.Invalidate();
	}

	private void OnEnable()
	{
		for (int i = 0; i < this.filters.Length; i++)
		{
			this.filters[i].GetComponentInChildren<UILabel>().text = Localize.CubesTypes[i];
			this.filters[i].onChange.Add(new EventDelegate(new EventDelegate.Callback(this.onChangeFilter)));
		}
		this.filters[0].value = true;
	}

	public void onSelectKube(int kubeId)
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

	public UIPanel container;

	public UIToggle[] filters;

	public GameObject itemPrefab;

	public PriceButton[] btn;

	public FastInventarPanel fi;

	private int inventoryPageType;
}
