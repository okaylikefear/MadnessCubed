using System;
using UnityEngine;

[Serializable]
public class SFE_fragmentController : MonoBehaviour
{
	public SFE_fragmentController()
	{
		this.impulseForce = (float)10;
		this.impulseRot = (float)10;
		this.shrinkTime = (float)5;
	}

	public virtual void Start()
	{
		int num = UnityEngine.Random.Range(-180, 180);
		Quaternion rotation = this.transform.rotation;
		float num2 = rotation.x = (float)num;
		Quaternion quaternion = this.transform.rotation = rotation;
		int num3 = UnityEngine.Random.Range(-180, 180);
		Quaternion rotation2 = this.transform.rotation;
		float num4 = rotation2.y = (float)num3;
		Quaternion quaternion2 = this.transform.rotation = rotation2;
		int num5 = UnityEngine.Random.Range(-180, 180);
		Quaternion rotation3 = this.transform.rotation;
		float num6 = rotation3.z = (float)num5;
		Quaternion quaternion3 = this.transform.rotation = rotation3;
		this.rnd = UnityEngine.Random.Range(-this.impulseRot, this.impulseRot);
		this.GetComponent<Rigidbody>().AddTorque(this.transform.up * this.rnd, ForceMode.Impulse);
		this.rnd = UnityEngine.Random.Range(-this.impulseRot, this.impulseRot);
		this.GetComponent<Rigidbody>().AddTorque(this.transform.right * this.rnd, ForceMode.Impulse);
		this.rnd = UnityEngine.Random.Range(-this.impulseRot, this.impulseRot);
		this.GetComponent<Rigidbody>().AddTorque(this.transform.forward * this.rnd, ForceMode.Impulse);
		this.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(UnityEngine.Random.Range(-this.impulseForce, this.impulseForce), UnityEngine.Random.Range(-this.impulseForce, this.impulseForce), UnityEngine.Random.Range(-this.impulseForce, this.impulseForce)), ForceMode.Impulse);
	}

	public virtual void Update()
	{
		this.lifetime += Time.deltaTime;
		float x = (float)1 - this.lifetime / this.shrinkTime;
		Vector3 localScale = this.transform.localScale;
		float num = localScale.x = x;
		Vector3 vector = this.transform.localScale = localScale;
		float y = (float)1 - this.lifetime / this.shrinkTime;
		Vector3 localScale2 = this.transform.localScale;
		float num2 = localScale2.y = y;
		Vector3 vector2 = this.transform.localScale = localScale2;
		float z = (float)1 - this.lifetime / this.shrinkTime;
		Vector3 localScale3 = this.transform.localScale;
		float num3 = localScale3.z = z;
		Vector3 vector3 = this.transform.localScale = localScale3;
		if (this.shrinkTime < this.lifetime)
		{
			UnityEngine.Object.Destroy(this.gameObject);
		}
	}

	public virtual void Main()
	{
	}

	public float impulseForce;

	public float impulseRot;

	public float shrinkTime;

	private float lifetime;

	private float rnd;
}
