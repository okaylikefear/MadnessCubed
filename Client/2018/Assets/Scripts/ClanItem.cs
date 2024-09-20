using System;
using kube.data;
using UnityEngine;

public class ClanItem : MonoBehaviour
{
	[NonSerialized]
	public int id;

	[NonSerialized]
	public ClanInfo info;

	public UILabel title;

	public UILabel nnplayers;

	public UILabel nnfrags;

	public UILabel nnkills;

	public UISprite mode;
}
