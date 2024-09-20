using System;
using System.Collections.Generic;
using UnityEngine;

public class ModalPanel : MonoBehaviour
{
	public static void close(ModalChild modalChild)
	{
		ModalPanel.childs.Remove(modalChild);
		ModalPanel.InvalidateList();
	}

	public static void open(ModalChild modalChild)
	{
		if (ModalPanel.childs.Contains(modalChild))
		{
			return;
		}
		ModalPanel.childs.Add(modalChild);
		ModalPanel.InvalidateList();
	}

	private static void RefreshChilds(ModalChild modalChild, int depth)
	{
		UIPanel[] componentsInChildren = modalChild.GetComponentsInChildren<UIPanel>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].depth = depth + i;
		}
	}

	private static void InvalidateList()
	{
		if (ModalPanel.childs.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < ModalPanel.childs.Count; i++)
		{
			ModalPanel.childs[i].getBlack(ModalPanel.childs.Count - 1 == i);
			ModalPanel.childs[i].GetComponent<UIPanel>().depth = 1000 + 10 * i;
			ModalPanel.RefreshChilds(ModalPanel.childs[i], 1000 + 10 * i);
		}
	}

	private void Start()
	{
		this.instance = this;
	}

	private void Update()
	{
	}

	private static List<ModalChild> childs = new List<ModalChild>();

	protected ModalPanel instance;
}
