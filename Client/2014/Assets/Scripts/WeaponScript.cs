using System;
using System.Collections;
using kube;
using UnityEngine;

public class WeaponScript : WeaponBase
{
	private void SetAudioLoopFalse()
	{
		base.audio.loop = false;
	}

	public void WeaponShot(GameObject bulletGO, Vector3 shotPoint, DamageMessage dm)
	{
		if (base.audio != null)
		{
			if (!this.isLoopSound || !base.audio.isPlaying)
			{
				base.audio.Play((ulong)(this.delaySound * (float)base.audio.clip.frequency));
			}
			if (this.isLoopSound)
			{
				base.audio.loop = true;
				base.CancelInvoke();
				base.Invoke("SetAudioLoopFalse", 0.25f);
			}
		}
		base.StartCoroutine(this.CreateBullet(bulletGO, shotPoint, dm));
		if (this.animGO != null && this.fireAnimName.Length != 0)
		{
			this.animGO.animation.Rewind(this.fireAnimName);
			this.animGO.animation.Play(this.fireAnimName);
		}
		if (this.muzzleFlash)
		{
			this.muzzleFlash.enableEmission = true;
			this._muzzleFlashTime = 0.2;
			this.muzzleFlash.Emit(1);
		}
		if (this.muzzleGO != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.muzzleGO, base.transform.Find("ShootPoint").position, base.transform.rotation) as GameObject;
			gameObject.transform.parent = base.transform;
		}
		if (this.lightObj)
		{
			this.lightObj.enabled = true;
			this._lightTime = 0.05;
		}
		if (this.shellGO)
		{
			GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(this.shellGO, base.transform.Find("ShootPoint").position, base.transform.Find("ShootPoint").rotation);
			gameObject2.SetActive(true);
			gameObject2.layer = LayerMask.NameToLayer("TransparentFX");
			if (this.owner != null)
			{
				gameObject2.rigidbody.AddForce(this.owner.GetComponent<CharacterController>().velocity * 20f + base.transform.Find("ShootPoint").TransformDirection(Vector3.left * 30f));
			}
		}
	}

	public void WeaponEmptyClip()
	{
		if (this.emptyClipSound != null)
		{
			UnityEngine.Object.Instantiate(this.emptyClipSound, base.transform.position, base.transform.rotation);
		}
	}

	public void WeaponReloadSound()
	{
		if (this.rechargeSound != null)
		{
			UnityEngine.Object.Instantiate(this.rechargeSound, base.transform.position, base.transform.rotation);
		}
	}

	private IEnumerator CreateBullet(GameObject bulletGO, Vector3 shotPoint, DamageMessage dm)
	{
		Vector3 pos = base.transform.Find("ShootPoint").position;
		Vector3 cubePos = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
		CubePhys cbp = Kube.WHS.GetCubePhysType(cubePos);
		if (cbp != CubePhys.air && cbp != CubePhys.water)
		{
			Ray backRay = new Ray(pos, shotPoint);
			pos -= backRay.direction;
		}
		yield return new WaitForSeconds(this.delayBullet);
		GameObject bullet = UnityEngine.Object.Instantiate(bulletGO, Vector3.zero, Quaternion.identity) as GameObject;
		bullet.transform.position = pos;
		bullet.transform.LookAt(shotPoint);
		BulletScript bs = bullet.GetComponent<BulletScript>();
		if (bs != null)
		{
			bs.accuarcy = this.accuarcy;
			bs.fatalDistance = this.fatalDistance;
		}
		bullet.SendMessage("SetDamageParam", dm);
		yield break;
	}

	private void Start()
	{
		if (this.lightObj)
		{
			this.lightObj.enabled = false;
		}
		if (this.muzzleFlash)
		{
			this.muzzleFlash.enableEmission = false;
		}
		Renderer componentInChildren = base.gameObject.GetComponentInChildren<Renderer>();
		if (componentInChildren)
		{
			this.renderGO = componentInChildren.gameObject;
		}
	}

	private void Update()
	{
		if (this.muzzleFlash)
		{
			if (this._muzzleFlashTime <= 0.0)
			{
				this.muzzleFlash.enableEmission = false;
			}
			else
			{
				this._muzzleFlashTime -= (double)Time.deltaTime;
			}
		}
		if (this.lightObj)
		{
			if (this._lightTime <= 0.0)
			{
				this.lightObj.enabled = false;
			}
			else
			{
				this._lightTime -= (double)Time.deltaTime;
			}
		}
	}

	public void HideWeapon(bool b)
	{
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		bool enabled = !b;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = enabled;
		}
	}

	public float delaySound;

	public float delayBullet;

	public GameObject animGO;

	public string fireAnimName;

	public bool isLoopSound;

	public ParticleSystem muzzleFlash;

	public GameObject muzzleGO;

	protected double _muzzleFlashTime;

	protected double _lightTime;

	public GameObject shellGO;

	public PlayerScript owner;

	public Light lightObj;

	public GameObject renderGO;

	public GameObject emptyClipSound;

	public GameObject rechargeSound;

	[NonSerialized]
	public float fatalDistance;

	[NonSerialized]
	public float accuarcy;
}
