using System;
using UnityEngine;

[Serializable]
public class SFE_prefabCloner : MonoBehaviour
{
	public SFE_prefabCloner()
	{
		this.detachToWorld = true;
	}

	public virtual void Start()
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.createThis, this.transform.position, this.transform.rotation);
		if (!this.detachToWorld)
		{
			gameObject.transform.parent = this.transform;
		}
	}

	public virtual void Update()
	{
	}

	public virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color((float)1, (float)1, (float)1, 0.5f);
		Gizmos.DrawSphere(this.transform.position, 0.3f);
	}

	public virtual void OnDrawGizmos()
	{
		Gizmos.color = new Color((float)1, (float)1, (float)1, 0.1f);
		Gizmos.DrawSphere(this.transform.position, 0.3f);
	}

	public virtual void Main()
	{
	}

	public GameObject createThis;

	public bool detachToWorld;
}
