using System;
using kube.data;
using UnityEngine;

public class MyMaptopItem : MonoBehaviour
{
	private void Start()
	{
	}

	public void OnClickLoad()
	{
		base.transform.parent.parent.GetComponent<MaptopMyTab>().onSelectSlot(this);
	}

	public void OnClickReset()
	{
		base.transform.parent.parent.GetComponent<MaptopMyTab>().onResetSlot(this);
	}

	public UILabel title;

	public UILabel id;

	public int mapId;

	public int oid;

	public UISprite mode;

	public TopInfo info;
}
