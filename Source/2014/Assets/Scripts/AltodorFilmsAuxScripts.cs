using System;
using UnityEngine;

public class AltodorFilmsAuxScripts : MonoBehaviour
{
	public void ReplaceWithRagDoll(string numGO)
	{
		int num = Convert.ToInt32(numGO);
		if (this.go[num] != null)
		{
			Transform transform = (UnityEngine.Object.Instantiate(this.go[num], base.transform.position, base.transform.rotation) as GameObject).transform;
			AltodorFilmsAuxScripts.CopyTransformsRecurse(base.transform, transform);
			UnityEngine.Object.Destroy(base.transform.gameObject);
		}
	}

	private static void CopyTransformsRecurse(Transform src, Transform dst)
	{
		dst.position = src.position;
		dst.rotation = src.rotation;
		foreach (object obj in dst)
		{
			Transform transform = (Transform)obj;
			Transform transform2 = src.Find(transform.name);
			if (transform2)
			{
				AltodorFilmsAuxScripts.CopyTransformsRecurse(transform2, transform);
			}
		}
	}

	public void LoadNewLevel(string levelName)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public GameObject[] go;
}
