using System;
using kube;
using UnityEngine;

public class TestBulletsScript : MonoBehaviour
{
	private void Start()
	{
		this.weapon = (UnityEngine.Object.Instantiate(Kube.ASS6.charWeaponsGO[this.numWeapon], base.transform.position, base.transform.rotation) as GameObject);
		this.shootDeltaTime = 1f;
		this.cameras = GameObject.FindGameObjectsWithTag("MainCamera");
		for (int i = 0; i < this.cameras.Length; i++)
		{
			if (i == this.activeCamera)
			{
				this.cameras[i].SetActive(true);
			}
			else
			{
				this.cameras[i].SetActive(false);
			}
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetAxis("Fire1") > 0f && Time.time - this.lastShootTime > this.shootDeltaTime)
		{
			DamageMessage damageMessage = new DamageMessage();
			damageMessage.damage = 0;
			damageMessage.id_killer = 0;
			damageMessage.team = 0;
			damageMessage.weaponType = (short)this.numWeapon;
			this.weapon.GetComponent<WeaponScript>().WeaponShot(Kube.ASS6.weaponsBulletPrefab[this.numWeapon], new Vector3(0f, 5f, 5f), damageMessage);
			this.lastShootTime = Time.time;
		}
	}

	private void OnGUI()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		GUI.Box(new Rect(0.1f * num, 0.9f * num2, 0.2f * num, 30f), string.Concat(new object[]
		{
			"Патрон(",
			this.numWeapon,
			"): ",
			Kube.ASS6.weaponsBulletPrefab[this.numWeapon].name
		}));
		if (GUI.Button(new Rect(0.1f * num, 0.9f * num2 + 30f, 0.1f * num, 30f), "Prew"))
		{
			this.numWeapon--;
			if (this.numWeapon < 0)
			{
				this.numWeapon = Kube.ASS6.weaponsBulletPrefab.Length - 1;
			}
			if (this.weapon != null)
			{
				UnityEngine.Object.Destroy(this.weapon);
			}
			this.weapon = (UnityEngine.Object.Instantiate(Kube.ASS6.charWeaponsGO[this.numWeapon], base.transform.position, base.transform.rotation) as GameObject);
		}
		if (GUI.Button(new Rect(0.2f * num, 0.9f * num2 + 30f, 0.1f * num, 30f), "Next"))
		{
			this.numWeapon++;
			if (this.numWeapon >= Kube.ASS6.weaponsBulletPrefab.Length)
			{
				this.numWeapon = 0;
			}
			if (this.weapon != null)
			{
				UnityEngine.Object.Destroy(this.weapon);
			}
			this.weapon = (UnityEngine.Object.Instantiate(Kube.ASS6.charWeaponsGO[this.numWeapon], base.transform.position, base.transform.rotation) as GameObject);
		}
		this.shootDeltaTime = GUI.HorizontalScrollbar(new Rect(0.3f * num, 0.9f * num2, 0.3f * num, 30f), this.shootDeltaTime, 0.1f, 0.1f, 2f);
		if (GUI.Button(new Rect(0.6f * num, 0.9f * num2, 0.2f * num, 30f), "Камера"))
		{
			this.activeCamera++;
			if (this.activeCamera >= this.cameras.Length)
			{
				this.activeCamera = 0;
			}
			for (int i = 0; i < this.cameras.Length; i++)
			{
				if (i == this.activeCamera)
				{
					this.cameras[i].SetActive(true);
				}
				else
				{
					this.cameras[i].SetActive(false);
				}
			}
		}
	}

	private int numWeapon;

	private float speed = 1f;

	private GameObject weapon;

	private int activeCamera;

	public GameObject[] cameras;

	private float shootDeltaTime = 0.3f;

	private float lastShootTime;
}
