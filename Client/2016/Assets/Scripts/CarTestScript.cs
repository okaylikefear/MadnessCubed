using System;
using UnityEngine;

public class CarTestScript : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.GetComponent<Rigidbody>().centerOfMass = new Vector3(0f, -0.5f, 0f);
		this.wheelsPhys = new WheelCollider[this.wheelsPhysGO.Length];
		this.wheelsRotateAngle = new float[this.wheelsPhysGO.Length];
		for (int i = 0; i < this.wheelsPhys.Length; i++)
		{
			this.wheelsPhys[i] = this.wheelsPhysGO[i].GetComponent<WheelCollider>();
		}
	}

	private void FixedUpdate()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.W))
		{
			for (int i = 0; i < this.wheelsPhys.Length; i++)
			{
				if (this.isWheelDriven[i])
				{
					this.wheelsPhys[i].motorTorque = this.motorTorque;
				}
			}
		}
		if (UnityEngine.Input.GetKeyUp(KeyCode.W))
		{
			for (int j = 0; j < this.wheelsPhys.Length; j++)
			{
				if (this.isWheelDriven[j])
				{
					this.wheelsPhys[j].motorTorque = 0f;
				}
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.S))
		{
			for (int k = 0; k < this.wheelsPhys.Length; k++)
			{
				if (this.isWheelDriven[k])
				{
					this.wheelsPhys[k].motorTorque = -this.motorTorque;
				}
			}
		}
		if (UnityEngine.Input.GetKeyUp(KeyCode.S))
		{
			for (int l = 0; l < this.wheelsPhys.Length; l++)
			{
				if (this.isWheelDriven[l])
				{
					this.wheelsPhys[l].motorTorque = 0f;
				}
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.A))
		{
			for (int m = 0; m < this.wheelsPhys.Length; m++)
			{
				this.wheelsPhys[m].transform.localRotation = Quaternion.Euler(0f, -this.wheelMaxRotateAngle[m], 0f);
			}
		}
		if (UnityEngine.Input.GetKeyUp(KeyCode.A))
		{
			for (int n = 0; n < this.wheelsPhys.Length; n++)
			{
				this.wheelsPhys[n].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.D))
		{
			for (int num = 0; num < this.wheelsPhys.Length; num++)
			{
				this.wheelsPhys[num].transform.localRotation = Quaternion.Euler(0f, this.wheelMaxRotateAngle[num], 0f);
			}
		}
		if (UnityEngine.Input.GetKeyUp(KeyCode.D))
		{
			for (int num2 = 0; num2 < this.wheelsPhys.Length; num2++)
			{
				this.wheelsPhys[num2].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
		{
			for (int num3 = 0; num3 < this.wheelsPhys.Length; num3++)
			{
				this.wheelsPhys[num3].brakeTorque = this.wheelBrakeTorque[num3];
			}
		}
		if (UnityEngine.Input.GetKeyUp(KeyCode.Space))
		{
			for (int num4 = 0; num4 < this.wheelsPhys.Length; num4++)
			{
				this.wheelsPhys[num4].brakeTorque = 0f;
			}
		}
	}

	private void Update()
	{
		for (int i = 0; i < this.wheelsPhys.Length; i++)
		{
			this.wheelsRotateAngle[i] += this.wheelsPhys[i].rpm * Time.deltaTime * 6f;
			while (this.wheelsRotateAngle[i] > 180f)
			{
				this.wheelsRotateAngle[i] -= 360f;
			}
			while (this.wheelsRotateAngle[i] < -180f)
			{
				this.wheelsRotateAngle[i] += 360f;
			}
			this.wheelsModels[i].transform.localPosition = new Vector3(this.wheelsModels[i].transform.localPosition.x, this.wheelsPhys[i].transform.localPosition.y, this.wheelsModels[i].transform.localPosition.z);
			this.wheelsModels[i].transform.localRotation = Quaternion.Euler(this.wheelsRotateAngle[i], this.wheelsPhys[i].transform.localRotation.eulerAngles.y, 0f);
		}
		MonoBehaviour.print(this.wheelsPhys[0].rpm * Time.deltaTime * 6f + "   " + this.wheelsModels[0].transform.localRotation.eulerAngles.x);
	}

	public GameObject[] wheelsPhysGO;

	private WheelCollider[] wheelsPhys;

	public GameObject[] wheelsModels;

	public bool[] isWheelDriven;

	public float[] wheelMaxRotateAngle;

	public float[] wheelBrakeTorque;

	public float motorTorque = 10f;

	private float[] wheelsRotateAngle;
}
