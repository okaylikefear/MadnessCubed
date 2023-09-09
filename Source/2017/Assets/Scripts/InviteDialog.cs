using System;
using kube;
using kube.data;
using UnityEngine;

public class InviteDialog : MonoBehaviour
{
	private void OnEnable()
	{
		this.Invalidate();
	}

	public void onScroll()
	{
	}

	private void onItemClick()
	{
		InviteDialog.current = UIButton.current.GetComponent<Top10Item>();
		this.handler();
		base.gameObject.SetActive(false);
	}

	private void Invalidate()
	{
		KGUITools.removeAllChildren(this.container.gameObject, true);
		int num = Kube.OH.friends.Length;
		ObjectsHolderScript.FriendInfo[] friends = Kube.OH.friends;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = this.container.gameObject.AddChild(this.itemPrefab);
			Top10Item component = gameObject.GetComponent<Top10Item>();
			component.title.text = (i + 1).ToString() + ". " + NGUIText.StripSymbols(friends[i].nickName);
			component.nnplayers.text = string.Empty;
			component.uid = friends[i].uid;
			EventDelegate.Add(gameObject.GetComponent<UIButton>().onClick, new EventDelegate(new EventDelegate.Callback(this.onItemClick)));
		}
		this.container.GetComponent<UIGrid>().Reposition();
		this.container.ResetPosition();
	}

	public UIScrollView container;

	public GameObject itemPrefab;

	public VoidCallback handler;

	public static Top10Item current;
}
