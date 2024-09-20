using System;
using UnityEngine;

[Serializable]
public class SFE_LaserEffect : MonoBehaviour
{
	public SFE_LaserEffect()
	{
		this.laserSize = 0.1f;
		this.fadeSpeed = (float)1;
		this.beginTintAlpha = 0.5f;
		this.normalizeUvLength = (float)1;
		this.maxRange = (float)300;
	}

	public virtual void Start()
	{
		this.direction = this.transform.TransformDirection(Vector3.forward);
		RaycastHit raycastHit = default(RaycastHit);
		if (Physics.Raycast(this.transform.position, this.direction, out raycastHit))
		{
			this.laser.SetPosition(0, this.transform.position);
			this.laser.SetPosition(1, raycastHit.point);
			this.lasBegin = this.transform.position;
			this.lasEnd = raycastHit.point;
		}
		else
		{
			this.laser.SetPosition(0, this.transform.position);
			Vector3 position = this.transform.position + this.direction * this.maxRange;
			this.laser.SetPosition(1, position);
			this.lasBegin = this.transform.position;
			this.lasEnd = position;
		}
		if (this.normalizeUV)
		{
			float num = Vector3.Distance(this.lasBegin, this.lasEnd);
			float x = num / this.normalizeUvLength;
			Vector2 mainTextureScale = this.GetComponent<Renderer>().materials[0].mainTextureScale;
			float num2 = mainTextureScale.x = x;
			Vector2 vector = this.GetComponent<Renderer>().materials[0].mainTextureScale = mainTextureScale;
		}
	}

	public virtual void Update()
	{
		this.time += Time.deltaTime;
		this.alpha = this.beginTintAlpha - this.fadeSpeed * this.time;
		if (this.alpha < (float)0)
		{
			this.alpha = (float)0;
		}
		this.laserSize = this.enlargeSpeed * Time.deltaTime + this.laserSize;
		this.laser.SetWidth(this.laserSize / 3f, this.laserSize);
		this.laser.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(this.myColor.r, this.myColor.g, this.myColor.b, this.alpha));
	}

	public virtual void Main()
	{
	}

	public LineRenderer laser;

	public float laserSize;

	public float fadeSpeed;

	public float enlargeSpeed;

	public float beginTintAlpha;

	public Color myColor;

	private float time;

	private float alpha;

	public bool normalizeUV;

	public float normalizeUvLength;

	public float maxRange;

	private Vector3 lasBegin;

	private Vector3 lasEnd;

	public Vector3 direction;
}
