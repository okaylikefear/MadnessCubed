using System;
using UnityEngine;

[Serializable]
public class SFE_AsteroidController : MonoBehaviour
{
	public SFE_AsteroidController()
	{
		this.impulseForce = (float)10;
		this.HPmin = (float)3;
		this.HPmax = (float)6;
	}

	public virtual void Start()
	{
		this.HP = UnityEngine.Random.Range(this.HPmin, this.HPmax);
	}

	public virtual void Update()
	{
	}

	public virtual void OnCollisionEnter(Collision collision)
	{
		if ((SFE_BulletController)collision.gameObject.GetComponent(typeof(SFE_BulletController)))
		{
			this.HP -= ((SFE_BulletController)collision.gameObject.GetComponent(typeof(SFE_BulletController))).damage;
		}
		if ((SFE_LaserController)collision.gameObject.GetComponent(typeof(SFE_LaserController)))
		{
			this.HP -= ((SFE_LaserController)collision.gameObject.GetComponent(typeof(SFE_LaserController))).damage;
		}
		if (this.HP <= (float)0)
		{
			UnityEngine.Object.Instantiate(this.explosion, this.transform.position, this.transform.rotation);
			UnityEngine.Object.Destroy(this.gameObject);
		}
	}

	public virtual void Main()
	{
	}

	public float impulseForce;

	public float HPmin;

	public float HPmax;

	private float HP;

	public GameObject explosion;
}
