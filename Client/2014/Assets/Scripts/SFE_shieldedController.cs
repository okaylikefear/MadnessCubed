using System;
using UnityEngine;

[Serializable]
public class SFE_shieldedController : MonoBehaviour
{
	public SFE_shieldedController()
	{
		this.impulseForce = (float)10;
		this.HPmin = (float)3;
		this.HPmax = (float)6;
		this.shieldMin = (float)5;
		this.shieldMax = (float)5;
		this.shieldGraphicsStuff = "---------------------------------";
		this.normalShieldGfxPower = (float)1;
		this.onHitShieldGfxPower = (float)10;
		this.onHitShieldGfxCooldownSpeed = (float)1;
	}

	public virtual void Start()
	{
		this.HP = UnityEngine.Random.Range(this.HPmin, this.HPmax);
		this.shield = UnityEngine.Random.Range(this.shieldMin, this.shieldMax);
		this.power = this.normalShieldGfxPower;
		this.shieldObject.renderer.material.SetFloat("_AllPower", this.normalShieldGfxPower);
	}

	public virtual void Update()
	{
		if (this.shieldObject)
		{
			this.shieldObject.renderer.material.SetFloat("_AllPower", this.power);
			if (this.power > this.normalShieldGfxPower)
			{
				this.power -= Time.deltaTime * this.onHitShieldGfxCooldownSpeed;
			}
			if (this.power < this.normalShieldGfxPower)
			{
				this.power = this.normalShieldGfxPower;
			}
		}
	}

	public virtual void OnCollisionEnter(Collision collision)
	{
		if (this.shieldObject)
		{
			this.power = this.onHitShieldGfxPower;
		}
		if (this.shield <= (float)0)
		{
			if ((SFE_BulletController)collision.gameObject.GetComponent(typeof(SFE_BulletController)))
			{
				this.HP -= ((SFE_BulletController)collision.gameObject.GetComponent(typeof(SFE_BulletController))).damage;
			}
			if ((SFE_LaserController)collision.gameObject.GetComponent(typeof(SFE_LaserController)))
			{
				this.HP -= ((SFE_LaserController)collision.gameObject.GetComponent(typeof(SFE_LaserController))).damage;
			}
		}
		if (this.shield > (float)0)
		{
			if (this.shield > (float)0 && this.onHitShieldGenerate)
			{
				ContactPoint contactPoint = collision.contacts[0];
				Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contactPoint.normal);
				Vector3 point = contactPoint.point;
				UnityEngine.Object.Instantiate(this.onHitShieldGenerate, point, rotation);
			}
			if ((SFE_BulletController)collision.gameObject.GetComponent(typeof(SFE_BulletController)))
			{
				this.shield -= ((SFE_BulletController)collision.gameObject.GetComponent(typeof(SFE_BulletController))).damage;
			}
			if ((SFE_LaserController)collision.gameObject.GetComponent(typeof(SFE_LaserController)))
			{
				this.shield -= ((SFE_LaserController)collision.gameObject.GetComponent(typeof(SFE_LaserController))).damage;
			}
			if (this.shield <= (float)0)
			{
				UnityEngine.Object.Destroy(this.shieldObject);
				if (this.onDestroyShieldGenerate)
				{
					UnityEngine.Object.Instantiate(this.onDestroyShieldGenerate, this.transform.position, this.transform.rotation);
				}
			}
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

	public float shieldMin;

	public float shieldMax;

	public GameObject shieldObject;

	public string shieldGraphicsStuff;

	public float normalShieldGfxPower;

	public float onHitShieldGfxPower;

	public float onHitShieldGfxCooldownSpeed;

	private float power;

	public GameObject onHitShieldGenerate;

	public GameObject onDestroyShieldGenerate;

	private float HP;

	private float shield;

	public GameObject explosion;
}
