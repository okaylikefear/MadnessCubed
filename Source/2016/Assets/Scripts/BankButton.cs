using System;
using System.Collections.Generic;
using UnityEngine;

public class BankButton : MonoBehaviour
{
	private void Awake()
	{
		this.moneyName.overflowMethod = UILabel.Overflow.ResizeFreely;
	}

	private void Start()
	{
	}

	private void Init()
	{
		List<Transform> list = new List<Transform>();
		foreach (object obj in base.transform)
		{
			Transform item = (Transform)obj;
			list.Add(item);
		}
		list.Sort((Transform a, Transform b) => (int)(a.localPosition.x - b.localPosition.x));
		this._order = list.ToArray();
	}

	[ContextMenu("Reposition")]
	public void Reposition()
	{
		float num = 0f;
		this.Init();
		float[] array = new float[this._order.Length];
		for (int i = 0; i < this._order.Length; i++)
		{
			if (i > 0)
			{
				num += this.spacing;
			}
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(this._order[i], false);
			array[i] = bounds.max.x - bounds.min.x;
			num += array[i];
		}
		float num2 = -num / 2f;
		for (int j = 0; j < this._order.Length; j++)
		{
			Vector3 localPosition = this._order[j].localPosition;
			localPosition.x = num2 + array[j] * 0.5f;
			this._order[j].localPosition = localPosition;
			num2 += array[j] + this.spacing;
		}
	}

	public UILabel money1;

	public UILabel money2;

	public UITexture tx;

	public UILabel moneyName;

	public float spacing = 5f;

	private Transform[] _order;
}
