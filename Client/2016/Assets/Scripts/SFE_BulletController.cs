using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class SFE_BulletController : MonoBehaviour
{
	public SFE_BulletController()
	{
		this.impulseForce = (float)10;
	}

	public virtual void Start()
	{
		if (this.muzzleFire)
		{
			UnityEngine.Object.Instantiate(this.muzzleFire, this.transform.position, this.transform.rotation);
		}
		this.GetComponent<Rigidbody>().AddForce(this.transform.up * this.impulseForce, ForceMode.Impulse);
	}

	public virtual void Update()
	{
	}

	public virtual void OnCollisionEnter(Collision collision)
	{
		UnityEngine.Object.Instantiate(this.explosion, this.transform.position, this.transform.rotation);
		if (this.detachOnDeath != null)
		{
			for (int i = 0; i < Extensions.get_length(this.detachOnDeath); i++)
			{
				this.detachOnDeath[i].transform.parent = null;
				ParticleSystem particleSystem = (ParticleSystem)this.detachOnDeath[i].GetComponent(typeof(ParticleSystem));
				particleSystem.enableEmission = false;
				UnityEngine.Object.Destroy(this.detachOnDeath[i], (float)5);
			}
		}
		UnityEngine.Object.Destroy(this.gameObject);
	}

	public virtual void Main()
	{
	}

	public float impulseForce;

	public GameObject muzzleFire;

	public GameObject explosion;

	public float damage;

	public GameObject[] detachOnDeath;
}
