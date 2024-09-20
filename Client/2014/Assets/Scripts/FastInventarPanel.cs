using System;
using kube;
using UnityEngine;

public class FastInventarPanel : MonoBehaviour
{
	protected virtual FastInventar[] fastInventar
	{
		get
		{
			return Kube.GPS.fastInventar;
		}
	}

	public void CurrentSlot(int index)
	{
		UISprite uisprite = null;
		if (index < this.slots.Length)
		{
			uisprite = this.slots[index].GetComponent<UISprite>();
		}
		if (uisprite == this._current)
		{
			return;
		}
		if (this._current)
		{
			this._current.spriteName = "rama_1";
		}
		if (uisprite)
		{
			uisprite.spriteName = "rama_2";
		}
		this._current = uisprite;
		this.Invalidate();
	}

	private void Start()
	{
		this.currentItem = FastInventarPanel.empty;
		this.Invalidate();
		for (int i = 0; i < this.slots.Length; i++)
		{
			this.slots[i].onClick = new EventDelegate(new EventDelegate.Callback(this.onSlotClick));
			if (this.slots[i].id)
			{
				this.slots[i].id.text = (this.slotOffset + i + 1).ToString();
			}
		}
	}

	protected virtual void Invalidate()
	{
		for (int i = 0; i < this.slots.Length; i++)
		{
			SlotItem component = this.slots[i].GetComponent<SlotItem>();
			component.invItem = this.fastInventar[this.slotOffset + i];
		}
	}

	private void Update()
	{
		this.Invalidate();
	}

	private void OnEnable()
	{
		if (this.arrows)
		{
			this.arrows.SetActive(false);
		}
	}

	public void SelectSlot(FastInventar item)
	{
		this._SelectSlot(item);
	}

	public void stop()
	{
		this._Stop();
	}

	private void _SelectSlot(FastInventar item)
	{
		this.arrows.SetActive(true);
		this.arrows.animation.Play();
		this.currentItem = item;
	}

	private void _Stop()
	{
		this.currentItem = FastInventarPanel.empty;
		this.arrows.SetActive(false);
	}

	public bool checkDublicate(FastInventar[] fastInventar)
	{
		bool result = true;
		for (int i = 0; i < 10; i++)
		{
			if (fastInventar[i].Type == this.currentItem.Type && fastInventar[i].Num == this.currentItem.Num)
			{
				fastInventar[i].Num = 0;
				fastInventar[i].Type = -1;
				result = false;
			}
		}
		return result;
	}

	public void onSlotClick()
	{
		SlotItem current = SlotItem.current;
		int num = Array.IndexOf<SlotItem>(this.slots, current);
		if (this.currentItem.Type == -1)
		{
			this.currentItem = FastInventarPanel.empty;
			this.fastInventar[this.slotOffset + num] = this.currentItem;
			current.invItem = this.currentItem;
			Kube.SS.SaveFastInventory(0, this.fastInventar, null);
			return;
		}
		bool flag = this.checkDublicate(this.fastInventar);
		this.fastInventar[this.slotOffset + num] = this.currentItem;
		current.invItem = this.currentItem;
		this.arrows.SetActive(false);
		this.currentItem.Type = -1;
		this.currentItem = FastInventarPanel.empty;
		int type = (this.fastInventar != Kube.GPS.fastInventar) ? 1 : 0;
		Kube.SS.SaveFastInventory(type, this.fastInventar, null);
		if (!flag)
		{
			this.Invalidate();
		}
	}

	public static int SortByName(SlotItem a, SlotItem b)
	{
		int num = int.Parse(a.transform.name);
		int num2 = int.Parse(b.transform.name);
		return num - num2;
	}

	[ContextMenu("collect")]
	public virtual void collect()
	{
		foreach (object obj in base.transform.GetChild(0))
		{
			Transform transform = (Transform)obj;
			GameObject gameObject = transform.gameObject;
			SlotItem component = gameObject.GetComponent<SlotItem>();
			component.id = gameObject.transform.FindChild("Label").GetComponent<UILabel>();
		}
	}

	public GameObject arrows;

	protected static FastInventar empty = new FastInventar(-1, 0);

	public int slotOffset;

	protected UISprite _current;

	private FastInventar currentItem;

	public SlotItem[] slots;
}
