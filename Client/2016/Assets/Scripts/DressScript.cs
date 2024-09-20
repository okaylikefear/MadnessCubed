using System;
using kube;
using UnityEngine;

public class DressScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void onAssetsLoaded(int id)
	{
		if (Kube.ASS5 == null)
		{
			return;
		}
		if (this._clothesString != null)
		{
			this.DressSkin(this._clothesString);
		}
		this._clothesString = null;
	}

	private void DressSkin(string clothesString)
	{
		if (Kube.ASS5 == null)
		{
			this._clothesString = clothesString;
			if (Kube.RM != null)
			{
				Kube.RM.require("Assets5", null);
			}
			return;
		}
		char[] separator = new char[]
		{
			';'
		};
		string[] array = clothesString.Split(separator);
		int[] array2 = new int[array.Length - 1];
		int num = Convert.ToInt32(array[0]);
		if (num >= 0 && Kube.OH.skinMats.ContainsKey(num))
		{
			base.transform.Find("PM3D_1234").gameObject.GetComponent<Renderer>().material = Kube.OH.skinMats[num];
		}
		for (int i = 0; i < this.clothesTypeGO.Length; i++)
		{
			if (this.clothesTypeGO[i] != null)
			{
				UnityEngine.Object.Destroy(this.clothesTypeGO[i]);
			}
			this.clothesTypeGO[i] = null;
		}
		string[] clothesType = Localize.ClothesType;
		for (int j = 0; j < clothesType.Length; j++)
		{
			if (array.Length <= j + 1)
			{
				break;
			}
			if (array[j + 1].Length != 0)
			{
				array2[j] = Convert.ToInt32(array[j + 1]);
				if (array2[j] >= 0 && Kube.OH.clothesGO.ContainsKey(array2[j]))
				{
					DressItemsScript component = Kube.OH.clothesGO[array2[j]].GetComponent<DressItemsScript>();
					for (int k = 0; k < component.dressItemsPrefabs.Length; k++)
					{
						if (this.clothesTypeGO[(int)component.transformToBind[k]] != null)
						{
							UnityEngine.Object.Destroy(this.clothesTypeGO[(int)component.transformToBind[k]]);
						}
						this.FindTransformToBind(base.transform, component.transformToBind[k], component.dressItemsPrefabs[k]);
					}
				}
			}
		}
	}

	private bool FindTransformToBind(Transform tr, ClothesPlace clothesPlace, GameObject clothesGO)
	{
		foreach (object obj in tr)
		{
			Transform transform = (Transform)obj;
			if (transform.gameObject.name == Kube.IS.clothesTransforms[(int)clothesPlace])
			{
				this.clothesTypeGO[(int)clothesPlace] = (UnityEngine.Object.Instantiate(clothesGO, Vector3.zero, Quaternion.identity) as GameObject);
				this.clothesTypeGO[(int)clothesPlace].transform.parent = transform;
				this.clothesTypeGO[(int)clothesPlace].transform.localPosition = Vector3.zero;
				this.clothesTypeGO[(int)clothesPlace].transform.localRotation = Quaternion.identity;
				this.clothesTypeGO[(int)clothesPlace].transform.localScale = Vector3.one;
				return true;
			}
			if (this.FindTransformToBind(transform, clothesPlace, clothesGO))
			{
				return true;
			}
		}
		return false;
	}

	private GameObject[] clothesTypeGO = new GameObject[32];

	protected string _clothesString;
}
