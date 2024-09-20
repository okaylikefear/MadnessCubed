using System;
using UnityEngine;

public class BulletTraceScript : MonoBehaviour
{
	private void SetBulletTrace(Vector3 secondPos)
	{
		LineRenderer component = base.gameObject.GetComponent<LineRenderer>();
		component.SetPosition(0, base.transform.position + Vector3.zero);
		component.SetPosition(1, base.transform.position + Vector3.zero);
		this._velPos = secondPos - base.transform.position;
		this._dist = this._velPos.magnitude;
		this.lifeTime = this._dist / this.speed;
		this._velPos.Normalize();
		UnityEngine.Object.Destroy(base.gameObject, this.lifeTime);
	}

	private void Start()
	{
		this._lr = base.gameObject.GetComponent<LineRenderer>();
		this.startTime = Time.time;
	}

	private void Update()
	{
		float num = (Time.time - this.startTime) / this.lifeTime;
		float num2 = this._dist * num;
		this._lr.SetPosition(0, base.transform.position + this._velPos * num2);
		this._lr.SetPosition(1, base.transform.position + this._velPos * (num2 + this.traceLength));
	}

	private float lifeTime = 0.5f;

	private float startTime;

	public float traceLength = 3f;

	public float speed = 200f;

	protected Vector3 _velPos;

	protected float _dist;

	protected LineRenderer _lr;
}
