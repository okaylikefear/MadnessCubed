using System;
using System.Collections;
using kube;
using UnityEngine;

public class WeaponHoming : WeaponScript
{
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
		GameObject bullet = PhotonNetwork.Instantiate(bulletGO.name, Vector3.zero, Quaternion.identity, 0);
		bullet.transform.position = pos;
		bullet.transform.LookAt(shotPoint);
		BulletScript bs = bullet.GetComponent<BulletScript>();
		if (bs != null)
		{
			bs.accuarcy = this.accuarcy;
			bs.fatalDistance = this.fatalDistance;
		}
		bullet.SendMessage("SetDamageParam", dm);
		TransportScript ts = bullet.GetComponent<TransportScript>();
		ts.objectId = -1;
		ts.GetInTransport(this.owner, 0);
		yield break;
	}

	public override void WeaponShot(GameObject bulletGO, Vector3 shotPoint, DamageMessage dm)
	{
		if (base.GetComponent<AudioSource>() != null)
		{
			if (!this.isLoopSound || !base.GetComponent<AudioSource>().isPlaying)
			{
				base.GetComponent<AudioSource>().Play((ulong)(this.delaySound * (float)base.GetComponent<AudioSource>().clip.frequency));
			}
			if (this.isLoopSound)
			{
				base.GetComponent<AudioSource>().loop = true;
				base.CancelInvoke();
				base.Invoke("SetAudioLoopFalse", 0.25f);
			}
		}
		if (this.owner.type == 0)
		{
			base.StartCoroutine(this.CreateBullet(bulletGO, shotPoint, dm));
		}
		if (this.animGO != null && this.fireAnimName.Length != 0)
		{
			this.animGO.GetComponent<Animation>().Rewind(this.fireAnimName);
			this.animGO.GetComponent<Animation>().Play(this.fireAnimName);
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
				gameObject2.GetComponent<Rigidbody>().AddForce(this.owner.GetComponent<CharacterController>().velocity * 20f + base.transform.Find("ShootPoint").TransformDirection(Vector3.left * 30f));
			}
		}
	}
}
