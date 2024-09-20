using System;
using kube;
using UnityEngine;

public class CaseGiftDialog : MonoBehaviour
{
	public void Open(bool isReplacementPack = true)
	{
		if (this.fi.Type == 5)
		{
			this.tx.mainTexture = Kube.ASS2.inventarWeaponsSkinTex[this.fi.Num];
			this.title.text = Localize.T("ui_giveskin", null);
		}
		else if (this.fi.Type == 4)
		{
			this.tx.mainTexture = Kube.ASS2.inventarWeaponsTex[this.fi.Num];
			this.title.text = Localize.T("ui_giveweapon", null);
		}
		else if (this.fi.Type == 7)
		{
			this.tx.mainTexture = Kube.ASS2.specItemsInvTex[this.fi.Num];
			this.title.text = Localize.T("ui_giveitem", null);
		}
		else if (this.fi.Type == 8)
		{
			this.tx.mainTexture = Kube.OH.inventarSkinsTex[this.fi.Num];
			this.title.text = Localize.T("ui_giveitem", null);
		}
		else if (this.fi.Type == 9)
		{
			this.tx.mainTexture = Kube.OH.inventarClothesTex[this.fi.Num];
			this.title.text = Localize.T("ui_giveitem", null);
		}
		else
		{
			this.tx.mainTexture = Kube.OH.gameItemsTex[this.fi.Num];
			if (isReplacementPack)
			{
				this.title.text = Localize.T("ui_haveitem", null);
			}
			else
			{
				this.title.text = Localize.T("ui_giveitem", null);
			}
		}
		base.gameObject.SetActive(true);
		Kube.SendMonoMessage("ItemsCubesUpdate", new object[0]);
	}

	private void Update()
	{
	}

	[NonSerialized]
	public FastInventar fi;

	public UITexture tx;

	public UILabel title;
}
