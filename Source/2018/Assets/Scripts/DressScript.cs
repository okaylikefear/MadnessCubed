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
		for (int i = 0; i < this.clothesTypeGO.GetLength(0); i++)
		{
			for (int j = 0; j < this.clothesTypeGO.GetLength(1); j++)
			{
				if (this.clothesTypeGO[i, j] != null)
				{
					UnityEngine.Object.Destroy(this.clothesTypeGO[i, j]);
				}
				this.clothesTypeGO[i, j] = null;
			}
		}
		string[] clothesType = Localize.ClothesType;
		for (int k = 0; k < clothesType.Length; k++)
		{
			if (array.Length <= k + 1)
			{
				break;
			}
			if (array[k + 1].Length != 0)
			{
				array2[k] = Convert.ToInt32(array[k + 1]);
				if (array2[k] >= 0 && Kube.OH.clothesGO.ContainsKey(array2[k]))
				{
					DressItemsScript component = Kube.OH.clothesGO[array2[k]].GetComponent<DressItemsScript>();
					for (int l = 0; l < component.dressItemsPrefabs.Length; l++)
					{
						if (this.clothesTypeGO[k, (int)component.transformToBind[l]] != null)
						{
							UnityEngine.Object.Destroy(this.clothesTypeGO[k, (int)component.transformToBind[l]]);
						}
						this.FindTransformToBind(base.transform, k, component.transformToBind[l], component.dressItemsPrefabs[l]);
					}
				}
			}
		}
	}

	private bool FindTransformToBind(Transform tr, int clothesType, ClothesPlace clothesPlace, GameObject clothesGO)
	{
		foreach (object obj in tr)
		{
			Transform transform = (Transform)obj;
			if (transform.gameObject.name == Kube.IS.clothesTransforms[(int)clothesPlace])
			{
				this.clothesTypeGO[clothesType, (int)clothesPlace] = (UnityEngine.Object.Instantiate(clothesGO, Vector3.zero, Quaternion.identity) as GameObject);
				this.clothesTypeGO[clothesType, (int)clothesPlace].transform.parent = transform;
				this.clothesTypeGO[clothesType, (int)clothesPlace].transform.localPosition = Vector3.zero;
				this.clothesTypeGO[clothesType, (int)clothesPlace].transform.localRotation = Quaternion.identity;
				this.clothesTypeGO[clothesType, (int)clothesPlace].transform.localScale = Vector3.one;
				return true;
			}
			if (this.FindTransformToBind(transform, clothesType, clothesPlace, clothesGO))
			{
				return true;
			}
		}
		return false;
	}

	private GameObject[,] clothesTypeGO = new GameObject[32, 12];

	protected string _clothesString;
}
