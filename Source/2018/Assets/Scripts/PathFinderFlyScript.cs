using System;
using kube;
using UnityEngine;

public class PathFinderFlyScript : PathFinderMoveScript
{
	public override bool CanPathTo(Vector3 targetPos)
	{
		if (this.pathLength > 0)
		{
			return true;
		}
		this.FindPathToVector3Fly(targetPos);
		if (this.pathLength > 0)
		{
			return true;
		}
		base.FindPathToVector3Walking(targetPos);
		return this.pathLength != 0;
	}

	public new void SetPathFinderParams(float _speed, float _jumpSpeed, int _charSizeY)
	{
		base.SetPathFinderParams(_speed, _jumpSpeed, _charSizeY);
	}

	private void FastFly(Vector3 pos)
	{
		int x = Mathf.RoundToInt(this.lastPathPoint.x);
		int y = Mathf.RoundToInt(this.lastPathPoint.y);
		int z = Mathf.RoundToInt(this.lastPathPoint.z);
		int num = Mathf.RoundToInt(pos.x);
		int num2 = Mathf.RoundToInt(pos.y);
		int num3 = Mathf.RoundToInt(pos.z);
		this.PFAS.ClearArray();
		this.PFAS.openedArrayNum = 1;
		this.PFAS.openedArray[0].x = x;
		this.PFAS.openedArray[0].y = y;
		this.PFAS.openedArray[0].z = z;
		this.PFAS.openedArray[0].parent = -1;
		this.PFAS.openedArray[0].stepNum = 0;
		this.PFAS.openedArray[0].isClosed = false;
		this.PFAS.openedArray[0].distFromSource = 0;
		this.PFAS.openedArray[0].distToTarget = base.GetDistToTarget(this.PFAS.openedArray[0].x, this.PFAS.openedArray[0].y, this.PFAS.openedArray[0].z, num, num2, num3);
		int num4 = -1;
		int num5 = 0;
		for (;;)
		{
			num5++;
			if (num5 > this.maxIterations)
			{
				break;
			}
			float num6 = 9999999f;
			num4 = -1;
			for (int i = 0; i < this.PFAS.openedArrayNum; i++)
			{
				if (!this.PFAS.openedArray[i].isClosed)
				{
					float num7 = (float)base.GetElementValue(this.PFAS.openedArray[i].distFromSource, this.PFAS.openedArray[i].distToTarget);
					if (num7 < num6)
					{
						num4 = i;
						num6 = num7;
					}
				}
			}
			if (num4 == -1)
			{
				goto Block_5;
			}
			this.PFAS.openedArray[num4].cannotStop = false;
			if (this.PFAS.openedArray[num4].x == num && this.PFAS.openedArray[num4].y == num2 && this.PFAS.openedArray[num4].z == num3)
			{
				goto Block_8;
			}
			this.PFAS.openedArray[num4].isClosed = true;
			this.PFAS.closedArray[this.PFAS.closedArrayNum] = this.PFAS.openedArray[num4];
			this.PFAS.closedArrayNum++;
			if (this.PFAS.openedArray[num4].stepNum <= this.maxPath)
			{
				CubePhys cubePhysType = Kube.WHS.GetCubePhysType(this.PFAS.openedArray[num4].x, this.PFAS.openedArray[num4].y, this.PFAS.openedArray[num4].z);
				for (int j = 0; j < PathFinderMoveScript.nX1.Length; j++)
				{
					int num8 = this.PFAS.openedArray[num4].x + PathFinderMoveScript.nX1[j];
					int num9 = this.PFAS.openedArray[num4].y + PathFinderMoveScript.nY1[j];
					int num10 = this.PFAS.openedArray[num4].z + PathFinderMoveScript.nZ1[j];
					if (Kube.WHS.IsInWorld(num8, num9, num10))
					{
						CubePhys cubePhysType2 = Kube.WHS.GetCubePhysType(num8, num9 - 1, num10);
						if (!Kube.WHS.isOccupied[num8, num9, num10])
						{
							bool flag = false;
							for (int k = 0; k < this.PFAS.openedArrayNum; k++)
							{
								if (num8 == this.PFAS.openedArray[k].x && num9 == this.PFAS.openedArray[k].y && num10 == this.PFAS.openedArray[k].z)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								bool flag2 = false;
								for (int l = 0; l < this.PFAS.closedArrayNum; l++)
								{
									if (num8 == this.PFAS.closedArray[l].x && num9 == this.PFAS.closedArray[l].y && num10 == this.PFAS.closedArray[l].z)
									{
										flag2 = true;
										break;
									}
								}
								if (!flag2)
								{
									bool flag3 = true;
									for (int m = 0; m < this.charSizeY; m++)
									{
										CubePhys cubePhysType3 = Kube.WHS.GetCubePhysType(num8, num9 + m, num10);
										if (cubePhysType3 == CubePhys.solid || cubePhysType3 == CubePhys.ice || Kube.WHS.isOccupied[num8, num9 + m, num10])
										{
											flag3 = false;
											break;
										}
									}
									if (flag3)
									{
										CubePhys cubePhysType4 = Kube.WHS.GetCubePhysType(num8, num9, num10);
										this.PFAS.openedArray[this.PFAS.openedArrayNum].distFromSource = this.PFAS.openedArray[num4].distFromSource + PathFinderMoveScript.wt1[j];
										this.PFAS.openedArray[this.PFAS.openedArrayNum].distToTarget = base.GetDistToTarget(num8, num9, num10, num, num2, num3);
										this.PFAS.openedArray[this.PFAS.openedArrayNum].isClosed = false;
										this.PFAS.openedArray[this.PFAS.openedArrayNum].parent = num4;
										this.PFAS.openedArray[this.PFAS.openedArrayNum].stepNum = this.PFAS.openedArray[num4].stepNum + 1;
										this.PFAS.openedArray[this.PFAS.openedArrayNum].x = num8;
										this.PFAS.openedArray[this.PFAS.openedArrayNum].y = num9;
										this.PFAS.openedArray[this.PFAS.openedArrayNum].z = num10;
										this.PFAS.openedArrayNum++;
									}
								}
							}
						}
					}
				}
				for (int n = 0; n < PathFinderMoveScript.nX2.Length; n++)
				{
					int num11 = this.PFAS.openedArray[num4].x + PathFinderMoveScript.nX2[n];
					int num12 = this.PFAS.openedArray[num4].y + PathFinderMoveScript.nY2[n];
					int num13 = this.PFAS.openedArray[num4].z + PathFinderMoveScript.nZ2[n];
					if (Kube.WHS.IsInWorld(num11, num12, num13))
					{
						CubePhys cubePhysType5 = Kube.WHS.GetCubePhysType(num11, num12 - 1, num13);
						if (!Kube.WHS.isOccupied[num11, num12, num13])
						{
							bool flag4 = false;
							for (int num14 = 0; num14 < this.PFAS.openedArrayNum; num14++)
							{
								if (num11 == this.PFAS.openedArray[num14].x && num12 == this.PFAS.openedArray[num14].y && num13 == this.PFAS.openedArray[num14].z)
								{
									flag4 = true;
									break;
								}
							}
							if (!flag4)
							{
								bool flag5 = false;
								for (int num15 = 0; num15 < this.PFAS.closedArrayNum; num15++)
								{
									if (num11 == this.PFAS.closedArray[num15].x && num12 == this.PFAS.closedArray[num15].y && num13 == this.PFAS.closedArray[num15].z)
									{
										flag5 = true;
										break;
									}
								}
								if (!flag5)
								{
									bool flag6 = true;
									for (int num16 = 0; num16 < this.charSizeY; num16++)
									{
										CubePhys cubePhysType6 = Kube.WHS.GetCubePhysType(num11, num12 + num16, num13);
										if (cubePhysType6 == CubePhys.solid || cubePhysType6 == CubePhys.ice || Kube.WHS.isOccupied[num11, num12 + num16, num13])
										{
											flag6 = false;
											break;
										}
									}
									for (int num17 = 0; num17 < this.charSizeY; num17++)
									{
										CubePhys cubePhysType7 = Kube.WHS.GetCubePhysType(num11 - PathFinderMoveScript.nX2[n], num12 + num17, num13);
										if (cubePhysType7 == CubePhys.solid || cubePhysType7 == CubePhys.ice || Kube.WHS.isOccupied[num11 - PathFinderMoveScript.nX2[n], num12 + num17, num13])
										{
											flag6 = false;
											break;
										}
									}
									for (int num18 = 0; num18 < this.charSizeY; num18++)
									{
										CubePhys cubePhysType8 = Kube.WHS.GetCubePhysType(num11, num12 + num18, num13 - PathFinderMoveScript.nZ2[n]);
										if (cubePhysType8 == CubePhys.solid || cubePhysType8 == CubePhys.ice || Kube.WHS.isOccupied[num11, num12 + num18, num13 - PathFinderMoveScript.nZ2[n]])
										{
											flag6 = false;
											break;
										}
									}
									if (flag6)
									{
										this.PFAS.openedArray[this.PFAS.openedArrayNum].distFromSource = this.PFAS.openedArray[num4].distFromSource + PathFinderMoveScript.wt2[n];
										this.PFAS.openedArray[this.PFAS.openedArrayNum].distToTarget = base.GetDistToTarget(num11, num12, num13, num, num2, num3);
										this.PFAS.openedArray[this.PFAS.openedArrayNum].isClosed = false;
										this.PFAS.openedArray[this.PFAS.openedArrayNum].parent = num4;
										this.PFAS.openedArray[this.PFAS.openedArrayNum].stepNum = this.PFAS.openedArray[num4].stepNum + 1;
										this.PFAS.openedArray[this.PFAS.openedArrayNum].x = num11;
										this.PFAS.openedArray[this.PFAS.openedArrayNum].y = num12;
										this.PFAS.openedArray[this.PFAS.openedArrayNum].z = num13;
										this.PFAS.openedArrayNum++;
									}
								}
							}
						}
					}
				}
				int num19;
				for (num19 = 0; num19 < this.jumpCubes + this.charSizeY; num19++)
				{
					CubePhys cubePhysType9 = Kube.WHS.GetCubePhysType(this.PFAS.openedArray[num4].x, this.PFAS.openedArray[num4].y + num19, this.PFAS.openedArray[num4].z);
					if (cubePhysType9 == CubePhys.solid || cubePhysType9 == CubePhys.ice)
					{
						break;
					}
				}
				num19 -= this.charSizeY;
			}
		}
		int num20 = 1;
		goto IL_CC2;
		Block_5:
		num20 = 1;
		goto IL_CC2;
		Block_8:
		num20 = 2;
		IL_CC2:
		if (num20 == 2)
		{
			int num21 = num4;
			this.pathLength = this.PFAS.openedArray[num21].stepNum + 1;
			while (num21 != -1)
			{
				if (this.PFAS.openedArray[num21].parent != -1)
				{
					this.path[this.pathLength - this.PFAS.openedArray[num21].stepNum - 1] = new Vector3((float)this.PFAS.openedArray[num21].x, (float)this.PFAS.openedArray[num21].y, (float)this.PFAS.openedArray[num21].z);
				}
				num21 = this.PFAS.openedArray[num21].parent;
			}
			this.pathLength--;
		}
		else
		{
			float num22 = 9999999f;
			num4 = -1;
			for (int num23 = 0; num23 < this.PFAS.openedArrayNum; num23++)
			{
				if (!this.PFAS.openedArray[num23].cannotStop)
				{
					float num24 = (float)this.PFAS.openedArray[num23].distToTarget;
					if (num24 < num22)
					{
						num4 = num23;
						num22 = num24;
					}
				}
			}
			int num25 = num4;
			if (num25 >= 0)
			{
				this.pathLength = this.PFAS.openedArray[num25].stepNum + 1;
				while (num25 != -1)
				{
					if (this.PFAS.openedArray[num25].parent != -1)
					{
						this.path[this.pathLength - this.PFAS.openedArray[num25].stepNum - 1] = new Vector3((float)this.PFAS.openedArray[num25].x, (float)this.PFAS.openedArray[num25].y, (float)this.PFAS.openedArray[num25].z);
					}
					num25 = this.PFAS.openedArray[num25].parent;
				}
				this.pathLength--;
			}
		}
	}

	private void FindPathToVector3Fly(Vector3 targetPos)
	{
		int num = Mathf.RoundToInt(this.lastPathPoint.x);
		int num2 = Mathf.RoundToInt(this.lastPathPoint.y);
		int num3 = Mathf.RoundToInt(this.lastPathPoint.z);
		this.flyPath = false;
		this.FastFly(targetPos);
		if (this.pathLength == 0)
		{
			return;
		}
		this.flyPath = true;
	}

	public override void WalkingFollowTarget(Vector3 targetPos)
	{
		if (Time.time - this.lastRefindPath > this.refindPathPeriod)
		{
			int x = Mathf.RoundToInt(this.lastOccupiedPoint.x);
			int y = Mathf.RoundToInt(this.lastOccupiedPoint.y);
			int z = Mathf.RoundToInt(this.lastOccupiedPoint.z);
			Kube.WHS.isOccupied[x, y, z] = false;
			this.FindPathToVector3Fly(targetPos);
			if (this.pathLength == 0)
			{
				x = Mathf.RoundToInt(this.lastOccupiedPoint.x);
				y = Mathf.RoundToInt(this.lastOccupiedPoint.y);
				z = Mathf.RoundToInt(this.lastOccupiedPoint.z);
				Kube.WHS.isOccupied[x, y, z] = true;
			}
			this.flagJumpUp = (this.flagJumpAcross = (this.flagFall = false));
			this.lastRefindPath = Time.time - UnityEngine.Random.Range(0f, 0.5f);
		}
		if (this.flyPath && this.pathLength > 0)
		{
			int x = Mathf.RoundToInt(this.lastOccupiedPoint.x);
			int y = Mathf.RoundToInt(this.lastOccupiedPoint.y);
			int z = Mathf.RoundToInt(this.lastOccupiedPoint.z);
			Kube.WHS.isOccupied[x, y, z] = false;
			this.lastOccupiedPoint = this.path[this.pathLength - 1];
			x = Mathf.RoundToInt(this.lastOccupiedPoint.x);
			y = Mathf.RoundToInt(this.lastOccupiedPoint.y);
			z = Mathf.RoundToInt(this.lastOccupiedPoint.z);
			Kube.WHS.isOccupied[x, y, z] = true;
			Vector3 vector = this.path[this.pathLength - 1] + Vector3.up * this.deltaHeightTransform;
			base.transform.LookAt(new Vector3(vector.x, base.transform.position.y, vector.z));
			int num = Kube.WHS.cubeTypes[Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z)];
			base.transform.Translate((vector - base.transform.position).normalized * this.flySpeed * Time.deltaTime, Space.World);
			if (Vector3.Distance(base.transform.position, vector) < this.flySpeed * Time.deltaTime * 2f)
			{
				this.lastPathPoint = this.path[this.pathLength - 1];
				this.pathLength--;
				this.flagJumpUp = (this.flagJumpAcross = (this.flagFall = false));
				if (this.pathLength > 0 && Kube.WHS.isOccupied[Mathf.RoundToInt(this.path[this.pathLength - 1].x), Mathf.RoundToInt(this.path[this.pathLength - 1].y), Mathf.RoundToInt(this.path[this.pathLength - 1].z)])
				{
					this.pathLength = 0;
					this.lastRefindPath = 0f;
					return;
				}
			}
		}
		else
		{
			base.WalkingFollowTarget(targetPos);
		}
	}

	private new void Start()
	{
		this.isFly = true;
		base.Start();
	}

	private new void Update()
	{
		CubePhys cubePhysType = Kube.WHS.GetCubePhysType(base.transform.position - Vector3.up * 0.5f);
		bool flag = cubePhysType == CubePhys.air;
		if (this.jetPack)
		{
			this.jetPack.SendMessage("PlayStop", flag, SendMessageOptions.DontRequireReceiver);
		}
		base.Update();
	}

	public GameObject jetPack;

	public float flySpeed = 8f;

	protected bool flyPath;
}
