using System;
using kube;
using UnityEngine;

public class WeaponSlotBtn : MonoBehaviour
{
	public int weaponId
	{
		set
		{
			this._weaponId = value;
			if (this._weaponId == -1)
			{
				this.tx.mainTexture = null;
				return;
			}
			if (Kube.ASS2 != null)
			{
				Texture mainTexture = Kube.ASS2.inventarWeaponsTex[this._weaponId];
				this.tx.mainTexture = mainTexture;
			}
		}
	}

	private void Start()
	{
	}

	public UITexture tx;

	protected int _weaponId;
}
