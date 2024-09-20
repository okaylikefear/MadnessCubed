using System;
using UnityEngine;

public class LightningScript : MonoBehaviour
{
	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.line = base.GetComponent<LineRenderer>();
		this.initialized = true;
	}

	private void SetMaterialTile()
	{
		Vector3 position = this.startPosVector;
		if (this.startPosType == LightningScript.PositionType.TargetTransform)
		{
			position = this.startPosTransform.position;
		}
		Vector3 position2 = this.endPosVector;
		if (this.endPosType == LightningScript.PositionType.TargetTransform)
		{
			position2 = this.endPosTransform.position;
		}
		float x = Vector3.Distance(position, position2);
		base.GetComponent<Renderer>().material.mainTextureScale = new Vector2(x, 1f);
	}

	private void SetSource(Transform _source)
	{
		this.startPosType = LightningScript.PositionType.TargetTransform;
		this.startPosTransform = _source;
		this.startPosVector = _source.position;
		this.SetMaterialTile();
	}

	private void SetDestination(Transform _destination)
	{
		this.endPosType = LightningScript.PositionType.TargetTransform;
		this.endPosTransform = _destination;
		this.endPosVector = _destination.position;
		this.SetMaterialTile();
	}

	private void SetSource(Vector3 _source)
	{
		this.startPosType = LightningScript.PositionType.TargetVector;
		this.startPosVector = _source;
		this.Init();
		this.line.SetPosition(0, this.startPosVector);
		this.SetMaterialTile();
	}

	private void SetDestination(Vector3 _destination)
	{
		this.endPosType = LightningScript.PositionType.TargetVector;
		this.endPosVector = _destination;
		this.Init();
		this.line.SetPosition(1, this.endPosVector);
		this.SetMaterialTile();
	}

	private void Start()
	{
		this.Init();
	}

	private void Update()
	{
		if (Time.time - this.lastChange > this.deltaTime)
		{
			base.GetComponent<Renderer>().material.mainTexture = this.lightningTex[UnityEngine.Random.Range(0, this.lightningTex.Length)];
			this.lastChange = Time.time;
		}
		if (this.startPosType == LightningScript.PositionType.TargetTransform)
		{
			if (this.startPosTransform == null)
			{
				this.startPosType = LightningScript.PositionType.TargetVector;
				this.line.SetPosition(0, this.startPosVector);
				this.SetMaterialTile();
			}
			else
			{
				this.line.SetPosition(0, this.startPosTransform.position);
				this.startPosVector = this.startPosTransform.position;
			}
		}
		if (this.endPosType == LightningScript.PositionType.TargetTransform)
		{
			if (this.endPosTransform == null)
			{
				this.endPosType = LightningScript.PositionType.TargetVector;
				this.line.SetPosition(1, this.endPosVector);
				this.SetMaterialTile();
			}
			else
			{
				this.line.SetPosition(1, this.endPosTransform.position);
				this.endPosVector = this.endPosTransform.position;
			}
		}
		if (this.startPosType == LightningScript.PositionType.TargetTransform || this.endPosType == LightningScript.PositionType.TargetTransform)
		{
			this.SetMaterialTile();
		}
	}

	public Texture[] lightningTex;

	public float deltaTime = 0.1f;

	private float lastChange;

	private LightningScript.PositionType startPosType;

	private LightningScript.PositionType endPosType;

	private Transform startPosTransform;

	private Transform endPosTransform;

	private Vector3 startPosVector;

	private Vector3 endPosVector;

	private LineRenderer line;

	private bool initialized;

	private enum PositionType
	{
		TargetVector,
		TargetTransform
	}
}
