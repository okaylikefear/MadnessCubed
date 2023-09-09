using System;
using kube;
using UnityEngine;

public class DressItemsScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void DestroyMe()
	{
		for (int i = 0; i < this.dressItems.Length; i++)
		{
			if (this.dressItems[i] != null)
			{
				UnityEngine.Object.Destroy(this.dressItems[i]);
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void DressMe()
	{
		for (int i = 0; i < this.dressItems.Length; i++)
		{
			if (this.dressItems[i] != null)
			{
				UnityEngine.Object.Destroy(this.dressItems[i]);
			}
		}
		for (int j = 0; j < this.dressItemsPrefabs.Length; j++)
		{
			this.FindTransformToBind(base.transform.parent, j);
		}
	}

	private bool FindTransformToBind(Transform tr, int numDressItems)
	{
		foreach (object obj in tr)
		{
			Transform transform = (Transform)obj;
			if (transform.gameObject.name == Kube.IS.clothesTransforms[(int)this.transformToBind[numDressItems]])
			{
				this.dressItems[numDressItems] = (UnityEngine.Object.Instantiate(this.dressItemsPrefabs[numDressItems], Vector3.zero, Quaternion.identity) as GameObject);
				this.dressItems[numDressItems].transform.parent = transform;
				this.dressItems[numDressItems].transform.localPosition = Vector3.zero;
				this.dressItems[numDressItems].transform.localRotation = Quaternion.identity;
				return true;
			}
			if (this.FindTransformToBind(transform, numDressItems))
			{
				return true;
			}
		}
		return false;
	}

	public GameObject[] dressItemsPrefabs;

	public GameObject[] dressItems;

	public ClothesPlace[] transformToBind;
}
