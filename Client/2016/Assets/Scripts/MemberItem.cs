using System;
using kube.data;
using UnityEngine;

public class MemberItem : MonoBehaviour
{
	private void Start()
	{
	}

	public void OnClickItem()
	{
		base.transform.root.GetComponentInChildren<ClansMyTab>().onMember(this);
	}

	public void OnClickYes()
	{
		base.transform.root.GetComponentInChildren<ClansMyTab>().onYesMember(this);
	}

	public void OnClickNo()
	{
		base.transform.root.GetComponentInChildren<ClansMyTab>().onNoMember(this);
	}

	public UIButton yes;

	public UIButton no;

	public UILabel title;

	public UILabel id;

	public ClanMember info;
}
