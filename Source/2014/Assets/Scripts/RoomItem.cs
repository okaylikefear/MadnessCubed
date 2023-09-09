using System;
using kube;
using UnityEngine;

public class RoomItem : MonoBehaviour
{
	private void Start()
	{
		for (int i = 0; i < Kube.OH.friends.Length; i++)
		{
			if (Kube.OH.friends[i].Tex)
			{
				Texture tex = Kube.OH.friends[i].Tex;
				break;
			}
		}
		this.locked.alpha = ((this.room.roomPassword != null && !(this.room.roomPassword == string.Empty)) ? 1f : 0f);
		if (this.room.friendsIds != null)
		{
			KGUITools.removeAllChildren(this.friendsCont, true);
			for (int j = 0; j < this.room.friendsIds.Length; j++)
			{
				for (int k = 0; k < Kube.OH.friends.Length; k++)
				{
					if (Kube.OH.friends[k].Id == this.room.friendsIds[j])
					{
						Texture tex = Kube.OH.friends[k].Tex;
						GameObject gameObject = NGUITools.AddChild(this.friendsCont, OnlineManager.instance.friendPrefab);
						gameObject.GetComponent<UITexture>().mainTexture = tex;
						break;
					}
				}
			}
			this.friendsCont.GetComponent<UIGrid>().Reposition();
		}
	}

	private void Update()
	{
	}

	public UILabel title;

	public UILabel nnplayers;

	public UISprite mode;

	public UISprite locked;

	public GameObject friendsCont;

	public OnlineManager.RoomsInfo room;
}
