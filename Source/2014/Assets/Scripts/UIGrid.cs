using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Grid")]
public class UIGrid : UIWidgetContainer
{
	public bool repositionNow
	{
		set
		{
			if (value)
			{
				this.mReposition = true;
				base.enabled = true;
			}
		}
	}

	public BetterList<Transform> GetChildList()
	{
		Transform transform = base.transform;
		BetterList<Transform> betterList = new BetterList<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (!this.hideInactive || (child && NGUITools.GetActive(child.gameObject)))
			{
				betterList.Add(child);
			}
		}
		if (this.sorting != UIGrid.Sorting.None)
		{
			if (this.sorting == UIGrid.Sorting.Alphabetic)
			{
				betterList.Sort(new BetterList<Transform>.CompareFunc(UIGrid.SortByName));
			}
			else if (this.sorting == UIGrid.Sorting.Horizontal)
			{
				betterList.Sort(new BetterList<Transform>.CompareFunc(UIGrid.SortHorizontal));
			}
			else if (this.sorting == UIGrid.Sorting.Vertical)
			{
				betterList.Sort(new BetterList<Transform>.CompareFunc(UIGrid.SortVertical));
			}
			else if (this.onCustomSort != null)
			{
				betterList.Sort(this.onCustomSort);
			}
			else
			{
				this.Sort(betterList);
			}
		}
		return betterList;
	}

	public Transform GetChild(int index)
	{
		BetterList<Transform> childList = this.GetChildList();
		return (index >= childList.size) ? null : childList[index];
	}

	public int GetIndex(Transform trans)
	{
		return this.GetChildList().IndexOf(trans);
	}

	public void AddChild(Transform trans)
	{
		this.AddChild(trans, true);
	}

	public void AddChild(Transform trans, bool sort)
	{
		if (trans != null)
		{
			BetterList<Transform> childList = this.GetChildList();
			childList.Add(trans);
			this.ResetPosition(childList);
		}
	}

	public void AddChild(Transform trans, int index)
	{
		if (trans != null)
		{
			if (this.sorting != UIGrid.Sorting.None)
			{
				UnityEngine.Debug.LogWarning("The Grid has sorting enabled, so AddChild at index may not work as expected.", this);
			}
			BetterList<Transform> childList = this.GetChildList();
			childList.Insert(index, trans);
			this.ResetPosition(childList);
		}
	}

	public Transform RemoveChild(int index)
	{
		BetterList<Transform> childList = this.GetChildList();
		if (index < childList.size)
		{
			Transform result = childList[index];
			childList.RemoveAt(index);
			this.ResetPosition(childList);
			return result;
		}
		return null;
	}

	public bool RemoveChild(Transform t)
	{
		BetterList<Transform> childList = this.GetChildList();
		if (childList.Remove(t))
		{
			this.ResetPosition(childList);
			return true;
		}
		return false;
	}

	protected virtual void Init()
	{
		this.mInitDone = true;
		this.mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
	}

	protected virtual void Start()
	{
		if (!this.mInitDone)
		{
			this.Init();
		}
		bool flag = this.animateSmoothly;
		this.animateSmoothly = false;
		this.Reposition();
		this.animateSmoothly = flag;
		base.enabled = false;
	}

	protected virtual void Update()
	{
		if (this.mReposition)
		{
			this.Reposition();
		}
		base.enabled = false;
	}

	public static int SortByName(Transform a, Transform b)
	{
		return string.Compare(a.name, b.name);
	}

	public static int SortHorizontal(Transform a, Transform b)
	{
		return a.localPosition.x.CompareTo(b.localPosition.x);
	}

	public static int SortVertical(Transform a, Transform b)
	{
		return b.localPosition.y.CompareTo(a.localPosition.y);
	}

	protected virtual void Sort(BetterList<Transform> list)
	{
	}

	[ContextMenu("Execute")]
	public virtual void Reposition()
	{
		if (Application.isPlaying && !this.mInitDone && NGUITools.GetActive(this))
		{
			this.mReposition = true;
			return;
		}
		if (this.sorted)
		{
			this.sorted = false;
			if (this.sorting == UIGrid.Sorting.None)
			{
				this.sorting = UIGrid.Sorting.Alphabetic;
			}
			NGUITools.SetDirty(this);
		}
		if (!this.mInitDone)
		{
			this.Init();
		}
		BetterList<Transform> childList = this.GetChildList();
		this.ResetPosition(childList);
		if (this.keepWithinPanel)
		{
			this.ConstrainWithinPanel();
		}
		if (this.onReposition != null)
		{
			this.onReposition();
		}
	}

	public void ConstrainWithinPanel()
	{
		if (this.mPanel != null)
		{
			this.mPanel.ConstrainTargetToBounds(base.transform, true);
		}
	}

	protected void ResetPosition(BetterList<Transform> list)
	{
		this.mReposition = false;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		Transform transform = base.transform;
		int i = 0;
		int size = list.size;
		while (i < size)
		{
			Transform transform2 = list[i];
			float z = transform2.localPosition.z;
			Vector3 vector = (this.arrangement != UIGrid.Arrangement.Horizontal) ? new Vector3(this.cellWidth * (float)num2, -this.cellHeight * (float)num, z) : new Vector3(this.cellWidth * (float)num, -this.cellHeight * (float)num2, z);
			if (this.animateSmoothly && Application.isPlaying)
			{
				SpringPosition.Begin(transform2.gameObject, vector, 15f).updateScrollView = true;
			}
			else
			{
				transform2.localPosition = vector;
			}
			num3 = Mathf.Max(num3, num);
			num4 = Mathf.Max(num4, num2);
			if (++num >= this.maxPerLine && this.maxPerLine > 0)
			{
				num = 0;
				num2++;
			}
			i++;
		}
		if (this.pivot != UIWidget.Pivot.TopLeft)
		{
			Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.pivot);
			float num5;
			float num6;
			if (this.arrangement == UIGrid.Arrangement.Horizontal)
			{
				num5 = Mathf.Lerp(0f, (float)num3 * this.cellWidth, pivotOffset.x);
				num6 = Mathf.Lerp((float)(-(float)num4) * this.cellHeight, 0f, pivotOffset.y);
			}
			else
			{
				num5 = Mathf.Lerp(0f, (float)num4 * this.cellWidth, pivotOffset.x);
				num6 = Mathf.Lerp((float)(-(float)num3) * this.cellHeight, 0f, pivotOffset.y);
			}
			for (int j = 0; j < transform.childCount; j++)
			{
				Transform child = transform.GetChild(j);
				SpringPosition component = child.GetComponent<SpringPosition>();
				if (component != null)
				{
					SpringPosition springPosition = component;
					springPosition.target.x = springPosition.target.x - num5;
					SpringPosition springPosition2 = component;
					springPosition2.target.y = springPosition2.target.y - num6;
				}
				else
				{
					Vector3 localPosition = child.localPosition;
					localPosition.x -= num5;
					localPosition.y -= num6;
					child.localPosition = localPosition;
				}
			}
		}
	}

	public UIGrid.Arrangement arrangement;

	public UIGrid.Sorting sorting;

	public UIWidget.Pivot pivot;

	public int maxPerLine;

	public float cellWidth = 200f;

	public float cellHeight = 200f;

	public bool animateSmoothly;

	public bool hideInactive = true;

	public bool keepWithinPanel;

	public UIGrid.OnReposition onReposition;

	public BetterList<Transform>.CompareFunc onCustomSort;

	[SerializeField]
	[HideInInspector]
	private bool sorted;

	protected bool mReposition;

	protected UIPanel mPanel;

	protected bool mInitDone;

	public enum Arrangement
	{
		Horizontal,
		Vertical
	}

	public enum Sorting
	{
		None,
		Alphabetic,
		Horizontal,
		Vertical,
		Custom
	}

	public delegate void OnReposition();
}
