using System;
using UnityEngine;

public class CarBase : TransportScript
{
	[ContextMenu("Editor Setup")]
	public void EditorSetup()
	{
		this.GearRatio = new float[]
		{
			0.93f,
			1.13f,
			1.4f,
			1.8f,
			2.7f,
			4.3f
		};
	}

	protected void ShiftGears()
	{
		int num = this.CurrentGear;
		if (this.shiftTime > Time.time)
		{
			return;
		}
		if (this.DrivenWheel == null)
		{
			for (int i = 0; i < this.wheelsPhys.Length; i++)
			{
				if (!this.isWheelDriven[i])
				{
					this.DrivenWheel = this.wheelsPhys[i];
					break;
				}
			}
			if (this.DrivenWheel == null)
			{
				this.DrivenWheel = this.wheelsPhys[0];
			}
		}
		this.realRpm = this.DrivenWheel.rpm / this.GearRatio[this.CurrentGear];
		if (this.realRpm >= this.maxRPM)
		{
			for (int j = 0; j < this.GearRatio.Length; j++)
			{
				if (this.DrivenWheel.rpm / this.GearRatio[j] < this.maxRPM)
				{
					num = j;
					break;
				}
			}
			if (num > this.CurrentGear)
			{
				this.CurrentGear = num;
				this.shiftTime = Time.time + 1f;
			}
		}
		if (this.realRpm <= this.minRPM)
		{
			num = 0;
			for (int k = this.GearRatio.Length - 1; k >= 0; k--)
			{
				if (this.DrivenWheel.rpm / this.GearRatio[k] > this.minRPM)
				{
					num = k;
					break;
				}
			}
			if (num < this.CurrentGear)
			{
				this.CurrentGear = num;
				this.shiftTime = Time.time + 1f;
			}
		}
	}

	public float maxRPM = 3000f;

	public float minRPM = 1000f;

	protected float meanRPM;

	public bool[] isWheelDriven;

	protected WheelCollider[] wheelsPhys;

	private WheelCollider DrivenWheel;

	public float[] GearRatio;

	protected int CurrentGear;

	private float shiftTime;

	public float realRpm;
}
