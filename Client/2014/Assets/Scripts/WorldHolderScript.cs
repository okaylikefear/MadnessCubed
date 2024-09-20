using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zlib;
using kube;
using kube.map;
using UnityEngine;

public class WorldHolderScript : MonoBehaviour
{
	private void Awake()
	{
		Kube.WHS = this;
		Kube.OH.crackCube = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("CrackCube") as GameObject);
	}

	private void Start()
	{
		Kube.WHS = this;
	}

	private void OnDestroy()
	{
		Kube.WHS = null;
	}

	private void SetNewCubesLightChange(int x, int y, int z)
	{
		this.cubesLightChange[this.numCubesLightChange, 0] = x;
		this.cubesLightChange[this.numCubesLightChange, 1] = y;
		this.cubesLightChange[this.numCubesLightChange, 2] = z;
		this.numCubesLightChange++;
	}

	public void ChangeWorldBytesCube(int x, int y, int z, byte type, byte prop)
	{
		this._cubegrid.set(x, y, z, (int)type, (int)prop);
		this.needSaveMap = true;
	}

	public void ChangeWorldBytesCube(int x, int y, int z, ushort type, byte prop)
	{
		this._cubegrid.set(x, y, z, (int)type, (int)prop);
		this.needSaveMap = true;
	}

	public void ChangeWorldBytesItem(int x, int y, int z, byte type, byte prop)
	{
		this._cubegrid.set(x, y, z, (int)(155 + type), (int)prop);
		this.needSaveMap = true;
	}

	public int GetNewWireId(GameObject go)
	{
		int result = -1;
		for (int i = 0; i < this.wireS.Length; i++)
		{
			if (this.wireS[i] == null)
			{
				result = i;
				this.wireS[i] = go.GetComponent<WireScript>();
				break;
			}
		}
		return result;
	}

	public void WireId(GameObject go, int id)
	{
		this.wireS[id] = go.GetComponent<WireScript>();
	}

	public void SaveWire(int triggerId_1, int triggerId_2, int delay, int targetType, int xt, int yt, int zt, int id)
	{
		if (this.wireS[id] != null)
		{
			this.wireS[id].SetParameters(triggerId_1, triggerId_2, delay, targetType, xt, yt, zt, id);
		}
		this.needSaveMap = true;
	}

	public void CreateNewWire(int triggerId_1, int triggerId_2, int delay, int targetType, int xt, int yt, int zt, int id)
	{
		if (this.wireS[id] != null)
		{
			this.SaveWire(triggerId_1, triggerId_2, delay, targetType, xt, yt, zt, id);
		}
		else
		{
			this.wireS[id] = (UnityEngine.Object.Instantiate(Kube.IS.gameItemsGO[Kube.OH.wireItemNum], Vector3.zero, Quaternion.identity) as GameObject).GetComponent<WireScript>();
			if (this.wireS[id] != null)
			{
				this.wireS[id].SetParameters(triggerId_1, triggerId_2, delay, targetType, xt, yt, zt, id);
			}
			this.CreateMagic(this.wireS[id].gameObject, Kube.OH.wireItemNum);
		}
	}

	public void DeleteWire(int id)
	{
		this.SaveWire(0, 0, 0, -1, 0, 0, 0, id);
		this.RemoveMagic(this.wireS[id].gameObject);
		if (this.wireS[id])
		{
			UnityEngine.Object.Destroy(this.wireS[id].gameObject);
		}
		this.wireS[id] = null;
	}

	public void ActivateWiresOfTrigger(int id)
	{
		for (int i = 0; i < this.wireS.Length; i++)
		{
			if (!(this.wireS[i] == null))
			{
				if (this.wireS[i].triggerId == id)
				{
					this.wireS[i].Activate();
				}
			}
		}
	}

	public void PlayTrigger(int id, int targetType, int targetX, int targetY, int targetZ)
	{
		this.triggerS[id].PlayTrigger(targetType, targetX, targetY, targetZ);
	}

	public GameObject GetAAGO(int id)
	{
		return this.AAgo[id];
	}

	public Vector3 GetAAPos(int id)
	{
		if (this.AAgo[id] != null)
		{
			ActionAreaScript component = this.AAgo[id].GetComponent<ActionAreaScript>();
			return new Vector3((float)component.x1, (float)component.y1, (float)component.z1);
		}
		return Vector3.zero;
	}

	public int GetNewAAid(GameObject go)
	{
		int num = -1;
		for (int i = 0; i < this.AAgo.Length; i++)
		{
			if (this.AAgo[i] == null)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			this.AAgo[num] = go;
		}
		return num;
	}

	public void AAid(GameObject go, int id)
	{
		this.AAgo[id] = go;
	}

	public void SaveAA(int _x1, int _y1, int _z1, int _x2, int _y2, int _z2, int _type, int _materialType, int _status, int _coordState, int _soundType, int _prop1, int _prop2, int _prop3, int _id)
	{
		this.AAgo[_id].GetComponent<ActionAreaScript>().SetParameters(_x1, _y1, _z1, _x2, _y2, _z2, _type, _materialType, _status, _coordState, _soundType, _prop1, _prop2, _prop3, _id);
		this.needSaveMap = true;
	}

	public void CreateNewAA(int _x1, int _y1, int _z1, int _x2, int _y2, int _z2, int _type, int _materialType, int _status, int _coordState, int _soundType, int _prop1, int _prop2, int _prop3, int _id)
	{
		if (this.AAgo[_id] != null)
		{
			this.SaveAA(_x1, _y1, _z1, _x2, _y2, _z2, _type, _materialType, _status, _coordState, _soundType, _prop1, _prop2, _prop3, _id);
		}
		else if (_type < Kube.OH.AAnumInShop.Length)
		{
			this.AAgo[_id] = (UnityEngine.Object.Instantiate(Kube.IS.gameItemsGO[Kube.OH.AAnumInShop[_type]], new Vector3((float)_x1, (float)_y1, (float)_z1), Quaternion.identity) as GameObject);
			ActionAreaScript component = this.AAgo[_id].GetComponent<ActionAreaScript>();
			if (component != null)
			{
				component.SetParameters(_x1, _y1, _z1, _x2, _y2, _z2, _type, _materialType, _status, _coordState, _soundType, _prop1, _prop2, _prop3, _id);
			}
			this.CreateMagic(this.AAgo[_id], Kube.OH.AAnumInShop[_type]);
		}
	}

	public void DeleteAA(int id)
	{
		this.SaveAA(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, id);
		this.RemoveMagic(this.AAgo[id]);
		UnityEngine.Object.Destroy(this.AAgo[id]);
		this.AAgo[id] = null;
	}

	public GameObject GetTriggerGO(int id)
	{
		return this.triggerS[id].gameObject;
	}

	public int GetTriggerId(int x, int y, int z)
	{
		for (int i = 0; i < this.triggerS.Length; i++)
		{
			if (this.triggerS[i])
			{
				if (this.triggerS[i].x == x && this.triggerS[i].y == y && this.triggerS[i].z == z)
				{
					return i;
				}
			}
		}
		return -1;
	}

	public Vector3 GetTriggerPos(int id)
	{
		if (this.triggerS[id] != null)
		{
			return new Vector3((float)this.triggerS[id].x, (float)this.triggerS[id].y, (float)this.triggerS[id].z);
		}
		return Vector3.zero;
	}

	public int GetNewTriggerId(GameObject go)
	{
		int result = -1;
		for (int i = 0; i < this.triggerS.Length; i++)
		{
			if (!this.triggerS[i])
			{
				result = i;
				this.triggerS[i] = go.GetComponent<TriggerScript>();
				break;
			}
		}
		return result;
	}

	public void SaveTrigger(int x, int y, int z, int type, int state, int delayTime, int condActivate, int condKey, int id)
	{
		if (x != 0 || y != 0 || z != 0)
		{
			this.triggerS[id].SetParameters(x, y, z, type, state, delayTime, condActivate, condKey, id);
		}
		this.needSaveMap = true;
	}

	public void DeleteTrigger(int x, int y, int z)
	{
		for (int i = 0; i < this.triggerS.Length; i++)
		{
			if (this.triggerS[i])
			{
				if (this.triggerS[i].x == x && this.triggerS[i].y == y && this.triggerS[i].z == z)
				{
					this.triggerS[i] = null;
					this.SaveTrigger(0, 0, 0, 0, 0, 0, 0, 0, i);
					break;
				}
			}
		}
	}

	public void MoveTrigger(int x, int y, int z, int newX, int newY, int newZ)
	{
		MonoBehaviour.print(string.Concat(new object[]
		{
			"MoveTrigger: ",
			x,
			" ",
			y,
			" ",
			z,
			" --- ",
			newX,
			" ",
			newY,
			" ",
			newZ
		}));
		for (int i = 0; i < this.triggerS.Length; i++)
		{
			if (this.triggerS[i])
			{
				if (this.triggerS[i].x == x && this.triggerS[i].y == y && this.triggerS[i].z == z)
				{
					MonoBehaviour.print(string.Concat(new object[]
					{
						"Found trigger: ",
						i,
						"  ",
						UnityEngine.Random.value
					}));
					this.triggerS[i].x = newX;
					this.triggerS[i].y = newY;
					this.triggerS[i].z = newZ;
					this.triggerS[i].SaveTrigger();
					for (int j = 0; j < this.wireS.Length; j++)
					{
						if (!(this.wireS[j] == null))
						{
							if (this.wireS[j].triggerId == i)
							{
								this.wireS[j].SaveWire();
							}
						}
					}
					break;
				}
			}
		}
	}

	public GameObject GetMonsterRespawnGO(int id)
	{
		return this.monsterRespawnS[id].gameObject;
	}

	public int GetMonsterRespawnId(int x, int y, int z)
	{
		for (int i = 0; i < this.monsterRespawnS.Length; i++)
		{
			if (this.monsterRespawnS[i])
			{
				if (this.monsterRespawnS[i].x == x && this.monsterRespawnS[i].y == y && this.monsterRespawnS[i].z == z)
				{
					return i;
				}
			}
		}
		return -1;
	}

	public Vector3 GetMonsterRespawnPos(int id)
	{
		return new Vector3((float)this.monsterRespawnS[id].x, (float)this.monsterRespawnS[id].y, (float)this.monsterRespawnS[id].z);
	}

	public int GetNewMonsterRespawnId(GameObject go)
	{
		int result = -1;
		for (int i = 0; i < this.monsterRespawnS.Length; i++)
		{
			if (!this.monsterRespawnS[i])
			{
				result = i;
				this.monsterRespawnS[i] = go.GetComponent<MonsterRespawnScript>();
				break;
			}
		}
		return result;
	}

	public void SaveMonsterRespawn(int x, int y, int z, int type, int state, int respawnTime, int healthMultiplier, int damageMultiplier, int id)
	{
		if (x != 0 || y != 0 || z != 0)
		{
			this.monsterRespawnS[id].SetParameters(x, y, z, type, state, respawnTime, healthMultiplier, damageMultiplier, id);
		}
		this.needSaveMap = true;
	}

	public void DeleteMonsterRespawn(int x, int y, int z)
	{
		for (int i = 0; i < this.monsterRespawnS.Length; i++)
		{
			if (this.monsterRespawnS[i])
			{
				if (this.monsterRespawnS[i].x == x && this.monsterRespawnS[i].y == y && this.monsterRespawnS[i].z == z)
				{
					this.monsterRespawnS[i] = null;
					this.SaveMonsterRespawn(0, 0, 0, 0, 0, 0, 0, 0, i);
					break;
				}
			}
		}
	}

	public void MoveMonsterRespawn(int x, int y, int z, int newX, int newY, int newZ)
	{
		MonoBehaviour.print(string.Concat(new object[]
		{
			"MoveMonsterRespawn: ",
			x,
			" ",
			y,
			" ",
			z,
			" --- ",
			newX,
			" ",
			newY,
			" ",
			newZ
		}));
		for (int i = 0; i < this.monsterRespawnS.Length; i++)
		{
			if (this.monsterRespawnS[i])
			{
				if (this.monsterRespawnS[i].x == x && this.monsterRespawnS[i].y == y && this.monsterRespawnS[i].z == z)
				{
					MonoBehaviour.print(string.Concat(new object[]
					{
						"Found trigger: ",
						i,
						"  ",
						UnityEngine.Random.value
					}));
					this.monsterRespawnS[i].x = newX;
					this.monsterRespawnS[i].y = newY;
					this.monsterRespawnS[i].z = newZ;
					this.monsterRespawnS[i].SaveMonsterRespawn();
					break;
				}
			}
		}
	}

	public GameObject GetTransportRespawnGO(int id)
	{
		return this.transportRespawnS[id].gameObject;
	}

	public int GetTransportRespawnId(int x, int y, int z)
	{
		for (int i = 0; i < this.transportRespawnS.Length; i++)
		{
			if (this.transportRespawnS[i])
			{
				if (this.transportRespawnS[i].x == x && this.transportRespawnS[i].y == y && this.transportRespawnS[i].z == z)
				{
					return i;
				}
			}
		}
		return -1;
	}

	public Vector3 GetTransportRespawnPos(int id)
	{
		return new Vector3((float)this.transportRespawnS[id].x, (float)this.transportRespawnS[id].y, (float)this.transportRespawnS[id].z);
	}

	public int GetNewTransportRespawnId(GameObject go)
	{
		int result = -1;
		for (int i = 0; i < this.transportRespawnS.Length; i++)
		{
			if (!this.transportRespawnS[i])
			{
				result = i;
				this.transportRespawnS[i] = go.GetComponent<TransportRespawnScript>();
				break;
			}
		}
		return result;
	}

	public void SaveTransportRespawn(int x, int y, int z, int type, int state, int respawnTime, int healthMultiplier, int damageMultiplier, int id)
	{
		if (x != 0 || y != 0 || z != 0)
		{
			this.transportRespawnS[id].SetParameters(x, y, z, type, state, respawnTime, healthMultiplier, damageMultiplier, id);
		}
		this.needSaveMap = true;
	}

	public void DeleteTransportRespawn(int x, int y, int z)
	{
		for (int i = 0; i < this.transportRespawnS.Length; i++)
		{
			if (this.transportRespawnS[i])
			{
				if (this.transportRespawnS[i].x == x && this.transportRespawnS[i].y == y && this.transportRespawnS[i].z == z)
				{
					this.transportRespawnS[i] = null;
					this.SaveTransportRespawn(0, 0, 0, 0, 0, 0, 0, 0, i);
					break;
				}
			}
		}
	}

	public void MoveTransportRespawn(int x, int y, int z, int newX, int newY, int newZ)
	{
		MonoBehaviour.print(string.Concat(new object[]
		{
			"MoveTransportRespawn: ",
			x,
			" ",
			y,
			" ",
			z,
			" --- ",
			newX,
			" ",
			newY,
			" ",
			newZ
		}));
		for (int i = 0; i < this.transportRespawnS.Length; i++)
		{
			if (this.transportRespawnS[i])
			{
				if (this.transportRespawnS[i].x == x && this.transportRespawnS[i].y == y && this.transportRespawnS[i].z == z)
				{
					MonoBehaviour.print(string.Concat(new object[]
					{
						"Found trigger: ",
						i,
						"  ",
						UnityEngine.Random.value
					}));
					this.transportRespawnS[i].x = newX;
					this.transportRespawnS[i].y = newY;
					this.transportRespawnS[i].z = newZ;
					this.transportRespawnS[i].SaveTransportRespawn();
					break;
				}
			}
		}
	}

	public bool IsInWorld(int x, int y, int z)
	{
		return x >= 0 && y >= 0 && z >= 0 && x < this.sizeX && y < this.sizeY && z < this.sizeZ;
	}

	private void Update()
	{
		if (Time.time - this.waterCalculateLastTime > this.waterCalculateDeltaTime)
		{
			this.CheckWater();
			this.waterCalculateLastTime = Time.time;
		}
	}

	private void SetWaterToCheck(int x, int y, int z, bool thisBuffer = true)
	{
		bool flag = this.isFirstUpdateWaterBuffer;
		if (!thisBuffer)
		{
			flag = !flag;
		}
		int i;
		for (i = 0; i < ((!flag) ? this.numUpdateWaterBuffer2 : this.numUpdateWaterBuffer1); i++)
		{
			if (flag && (int)this.updateWaterBuffer1[i].x == x && (int)this.updateWaterBuffer1[i].y == y && (int)this.updateWaterBuffer1[i].z == z)
			{
				break;
			}
			if (!flag && (int)this.updateWaterBuffer2[i].x == x && (int)this.updateWaterBuffer2[i].y == y && (int)this.updateWaterBuffer2[i].z == z)
			{
				break;
			}
		}
		if (i == ((!flag) ? this.numUpdateWaterBuffer2 : this.numUpdateWaterBuffer1))
		{
			if (flag)
			{
				this.updateWaterBuffer1[this.numUpdateWaterBuffer1].x = (byte)x;
				this.updateWaterBuffer1[this.numUpdateWaterBuffer1].y = (byte)y;
				this.updateWaterBuffer1[this.numUpdateWaterBuffer1].z = (byte)z;
				this.numUpdateWaterBuffer1++;
			}
			else
			{
				this.updateWaterBuffer2[this.numUpdateWaterBuffer2].x = (byte)x;
				this.updateWaterBuffer2[this.numUpdateWaterBuffer2].y = (byte)y;
				this.updateWaterBuffer2[this.numUpdateWaterBuffer2].z = (byte)z;
				this.numUpdateWaterBuffer2++;
			}
		}
	}

	private void CheckWaterBlocksToChange(int x, int y, int z)
	{
		this.waterBlocksToChange[Mathf.FloorToInt((float)x / (float)this.blockSizeX), Mathf.FloorToInt((float)y / (float)this.blockSizeY), Mathf.FloorToInt((float)z / (float)this.blockSizeZ)] = true;
		if (x % this.blockSizeX == 0 && x > 0)
		{
			this.waterBlocksToChange[Mathf.FloorToInt((float)x / (float)this.blockSizeX) - 1, Mathf.FloorToInt((float)y / (float)this.blockSizeY), Mathf.FloorToInt((float)z / (float)this.blockSizeZ)] = true;
		}
		if (y % this.blockSizeY == 0 && y > 0)
		{
			this.waterBlocksToChange[Mathf.FloorToInt((float)x / (float)this.blockSizeX), Mathf.FloorToInt((float)y / (float)this.blockSizeY) - 1, Mathf.FloorToInt((float)z / (float)this.blockSizeZ)] = true;
		}
		if (z % this.blockSizeZ == 0 && z > 0)
		{
			this.waterBlocksToChange[Mathf.FloorToInt((float)x / (float)this.blockSizeX), Mathf.FloorToInt((float)y / (float)this.blockSizeY), Mathf.FloorToInt((float)z / (float)this.blockSizeZ) - 1] = true;
		}
		if (x % this.blockSizeX == this.blockSizeX - 1 && x < this.sizeX - 1)
		{
			this.waterBlocksToChange[Mathf.FloorToInt((float)x / (float)this.blockSizeX) + 1, Mathf.FloorToInt((float)y / (float)this.blockSizeY), Mathf.FloorToInt((float)z / (float)this.blockSizeZ)] = true;
		}
		if (y % this.blockSizeY == this.blockSizeY - 1 && y < this.sizeY - 1)
		{
			this.waterBlocksToChange[Mathf.FloorToInt((float)x / (float)this.blockSizeX), Mathf.FloorToInt((float)y / (float)this.blockSizeY) + 1, Mathf.FloorToInt((float)z / (float)this.blockSizeZ)] = true;
		}
		if (z % this.blockSizeZ == this.blockSizeZ - 1 && z < this.sizeZ - 1)
		{
			this.waterBlocksToChange[Mathf.FloorToInt((float)x / (float)this.blockSizeX), Mathf.FloorToInt((float)y / (float)this.blockSizeY), Mathf.FloorToInt((float)z / (float)this.blockSizeZ + 1f)] = true;
		}
	}

	private void CheckWater()
	{
		for (int i = 0; i < this.nBlocksX; i++)
		{
			for (int j = 0; j < this.nBlocksY; j++)
			{
				for (int k = 0; k < this.nBlocksZ; k++)
				{
					this.waterBlocksToChange[i, j, k] = false;
				}
			}
		}
		for (int l = 0; l < ((!this.isFirstUpdateWaterBuffer) ? this.numUpdateWaterBuffer2 : this.numUpdateWaterBuffer1); l++)
		{
			int x;
			int y;
			int z;
			if (this.isFirstUpdateWaterBuffer)
			{
				x = (int)this.updateWaterBuffer1[l].x;
				y = (int)this.updateWaterBuffer1[l].y;
				z = (int)this.updateWaterBuffer1[l].z;
			}
			else
			{
				x = (int)this.updateWaterBuffer2[l].x;
				y = (int)this.updateWaterBuffer2[l].y;
				z = (int)this.updateWaterBuffer2[l].z;
			}
			if (this.IsInWorld(x, y, z))
			{
				if (this.cubes[x, y, z].waterLevel != 0)
				{
					if (this.IsInWorld(x, y - 1, z) && (this.cubes[x, y - 1, z].type == 0 || (this.cubes[x, y - 1, z].type == 128 && this.cubes[x, y - 1, z].waterLevel != this.maxWaterLevel)))
					{
						this.cubes[x, y - 1, z].type = 128;
						this.cubes[x, y - 1, z].phys = this.cubePhys[(int)this.cubes[x, y - 1, z].type];
						this.cubes[x, y - 1, z].waterLevel = this.maxWaterLevel;
						this.SetWaterToCheck(x, y - 1, z, false);
						this.CheckWaterBlocksToChange(x, y - 1, z);
						this.ChangeWorldBytesCube(x, y - 1, z, this.cubes[x, y - 1, z].type, this.cubes[x, y - 1, z].waterLevel);
					}
					else
					{
						int num = x;
						int num2 = y + 1;
						int num3 = z;
						if (this.IsInWorld(num, num2, num3) && this.cubes[num, num2, num3].type != 128 && this.cubes[x, y, z].waterLevel != this.maxWaterLevel)
						{
							byte b = 0;
							int num4 = 0;
							if (this.IsInWorld(x + 1, y, z))
							{
								if (this.cubes[x + 1, y, z].waterLevel > b)
								{
									b = this.cubes[x + 1, y, z].waterLevel;
								}
								if (this.cubes[x + 1, y, z].waterLevel == this.maxWaterLevel)
								{
									num4++;
								}
							}
							if (this.IsInWorld(x - 1, y, z))
							{
								if (this.cubes[x - 1, y, z].waterLevel > b)
								{
									b = this.cubes[x - 1, y, z].waterLevel;
								}
								if (this.cubes[x - 1, y, z].waterLevel == this.maxWaterLevel)
								{
									num4++;
								}
							}
							if (this.IsInWorld(x, y, z + 1))
							{
								if (this.cubes[x, y, z + 1].waterLevel > b)
								{
									b = this.cubes[x, y, z + 1].waterLevel;
								}
								if (this.cubes[x, y, z + 1].waterLevel == this.maxWaterLevel)
								{
									num4++;
								}
							}
							if (this.IsInWorld(x, y, z - 1))
							{
								if (this.cubes[x, y, z - 1].waterLevel > b)
								{
									b = this.cubes[x, y, z - 1].waterLevel;
								}
								if (this.cubes[x, y, z - 1].waterLevel == this.maxWaterLevel)
								{
									num4++;
								}
							}
							if (num4 >= 3)
							{
								this.cubes[x, y, z].waterLevel = this.maxWaterLevel;
								this.ChangeWorldBytesCube(x, y, z, this.cubes[x, y, z].type, this.cubes[x, y, z].waterLevel);
								this.SetWaterToCheck(x, y, z, false);
								this.CheckWaterBlocksToChange(x, y, z);
								goto IL_F3A;
							}
							if (b <= this.cubes[x, y, z].waterLevel)
							{
								CubeStruct ptr = this.cubes[x, y, z];
								ptr.waterLevel -= 1;
								this.CheckWaterBlocksToChange(x, y, z);
								this.SetWaterToCheck(x + 1, y, z, false);
								this.SetWaterToCheck(x - 1, y, z, false);
								this.SetWaterToCheck(x, y, z + 1, false);
								this.SetWaterToCheck(x, y, z - 1, false);
								this.SetWaterToCheck(x, y, z, false);
								if (this.cubes[x, y, z].waterLevel == 0)
								{
									this.cubes[x, y, z].type = 0;
									this.cubes[x, y, z].phys = this.cubePhys[(int)this.cubes[x, y, z].type];
									this.ChangeWorldBytesCube(x, y, z, this.cubes[x, y, z].type, this.cubes[x, y, z].waterLevel);
									if (this.IsInWorld(x, y - 1, z) && this.cubes[x, y - 1, z].type == 128)
									{
										this.cubes[x, y - 1, z].waterLevel = this.maxWaterLevel - 1;
										this.SetWaterToCheck(x, y - 1, z, false);
										this.CheckWaterBlocksToChange(x, y - 1, z);
										this.ChangeWorldBytesCube(x, y - 1, z, this.cubes[x, y - 1, z].type, this.cubes[x, y - 1, z].waterLevel);
									}
								}
								goto IL_F3A;
							}
						}
						if (!this.IsInWorld(x, y - 1, z) || this.cubes[x, y - 1, z].type != 128)
						{
							int m = 0;
							if (this.cubes[x, y, z].waterLevel > 1)
							{
								for (m = 1; m < (int)this.maxWaterLevel; m++)
								{
									bool flag = false;
									bool flag2 = false;
									for (int n = x - m; n <= x + m; n++)
									{
										for (int num5 = z - m; num5 <= z + m; num5++)
										{
											if (Mathf.Abs(x - n) + Mathf.Abs(z - num5) == m)
											{
												if (this.IsInWorld(n, y, num5) && this.IsInWorld(n, y - 1, num5) && (this.cubes[n, y, num5].type == 0 || this.cubes[n, y - 1, num5].type == 128))
												{
													flag = true;
													if (this.IsInWorld(n, y - 1, num5) && (this.cubes[n, y - 1, num5].type == 0 || this.cubes[n, y - 1, num5].type == 128))
													{
														flag2 = true;
														break;
													}
												}
											}
										}
										if (flag2)
										{
											break;
										}
									}
									if (!flag)
									{
										m--;
										break;
									}
									if (flag2)
									{
										break;
									}
								}
							}
							if (m != 0)
							{
								bool flag3 = false;
								bool flag4 = false;
								bool flag5 = false;
								bool flag6 = false;
								bool flag7 = false;
								for (int num6 = x - m; num6 <= x + m; num6++)
								{
									for (int num7 = z - m; num7 <= z + m; num7++)
									{
										if (Mathf.Abs(x - num6) + Mathf.Abs(z - num7) == m)
										{
											if (this.IsInWorld(num6, y, num7) && (this.cubes[num6, y, num7].type == 0 || this.cubes[num6, y - 1, num7].type == 128) && this.IsInWorld(num6, y - 1, num7) && (this.cubes[num6, y - 1, num7].type == 0 || this.cubes[num6, y - 1, num7].type == 128))
											{
												flag7 = true;
												if (num7 > z)
												{
													flag3 = true;
												}
												if (num7 < z)
												{
													flag4 = true;
												}
												if (num6 > x)
												{
													flag5 = true;
												}
												if (num6 < x)
												{
													flag6 = true;
												}
											}
										}
									}
								}
								if (!flag7)
								{
									flag3 = true;
									flag4 = true;
									flag5 = true;
									flag6 = true;
								}
								num = x;
								num2 = y;
								num3 = z + 1;
								if (flag3 && this.IsInWorld(num, num2, num3) && (this.cubes[num, num2, num3].type == 0 || (this.cubes[num, num2, num3].type == 128 && this.cubes[num, num2, num3].waterLevel < this.cubes[x, y, z].waterLevel)))
								{
									this.cubes[num, num2, num3].type = 128;
									this.cubes[num, num2, num3].phys = this.cubePhys[(int)this.cubes[num, num2, num3].type];
									this.cubes[num, num2, num3].waterLevel = this.cubes[x, y, z].waterLevel - 1;
									this.SetWaterToCheck(num, num2, num3, false);
									this.CheckWaterBlocksToChange(num, num2, num3);
									this.ChangeWorldBytesCube(num, num2, num3, this.cubes[num, num2, num3].type, this.cubes[num, num2, num3].waterLevel);
								}
								num = x;
								num2 = y;
								num3 = z - 1;
								if (flag4 && this.IsInWorld(num, num2, num3) && (this.cubes[num, num2, num3].type == 0 || (this.cubes[num, num2, num3].type == 128 && this.cubes[num, num2, num3].waterLevel < this.cubes[x, y, z].waterLevel)))
								{
									this.cubes[num, num2, num3].type = 128;
									this.cubes[num, num2, num3].phys = this.cubePhys[(int)this.cubes[num, num2, num3].type];
									this.cubes[num, num2, num3].waterLevel = this.cubes[x, y, z].waterLevel - 1;
									this.SetWaterToCheck(num, num2, num3, false);
									this.CheckWaterBlocksToChange(num, num2, num3);
									this.ChangeWorldBytesCube(num, num2, num3, this.cubes[num, num2, num3].type, this.cubes[num, num2, num3].waterLevel);
								}
								num = x + 1;
								num2 = y;
								num3 = z;
								if (flag5 && this.IsInWorld(num, num2, num3) && (this.cubes[num, num2, num3].type == 0 || (this.cubes[num, num2, num3].type == 128 && this.cubes[num, num2, num3].waterLevel < this.cubes[x, y, z].waterLevel)))
								{
									this.cubes[num, num2, num3].type = 128;
									this.cubes[num, num2, num3].phys = this.cubePhys[(int)this.cubes[num, num2, num3].type];
									this.cubes[num, num2, num3].waterLevel = this.cubes[x, y, z].waterLevel - 1;
									this.SetWaterToCheck(num, num2, num3, false);
									this.CheckWaterBlocksToChange(num, num2, num3);
									this.ChangeWorldBytesCube(num, num2, num3, this.cubes[num, num2, num3].type, this.cubes[num, num2, num3].waterLevel);
								}
								num = x - 1;
								num2 = y;
								num3 = z;
								if (flag6 && this.IsInWorld(num, num2, num3) && (this.cubes[num, num2, num3].type == 0 || (this.cubes[num, num2, num3].type == 128 && this.cubes[num, num2, num3].waterLevel < this.cubes[x, y, z].waterLevel)))
								{
									this.cubes[num, num2, num3].type = 128;
									this.cubes[num, num2, num3].phys = this.cubePhys[(int)this.cubes[num, num2, num3].type];
									this.cubes[num, num2, num3].waterLevel = this.cubes[x, y, z].waterLevel - 1;
									this.SetWaterToCheck(num, num2, num3, false);
									this.CheckWaterBlocksToChange(num, num2, num3);
									this.ChangeWorldBytesCube(num, num2, num3, this.cubes[num, num2, num3].type, this.cubes[num, num2, num3].waterLevel);
								}
							}
						}
					}
				}
			}
			IL_F3A:;
		}
		if (this.isFirstUpdateWaterBuffer)
		{
			this.numUpdateWaterBuffer1 = 0;
		}
		else
		{
			this.numUpdateWaterBuffer2 = 0;
		}
		this.isFirstUpdateWaterBuffer = !this.isFirstUpdateWaterBuffer;
		for (int num8 = 0; num8 < this.nBlocksX; num8++)
		{
			for (int num9 = 0; num9 < this.nBlocksY; num9++)
			{
				for (int num10 = 0; num10 < this.nBlocksZ; num10++)
				{
					if (this.waterBlocksToChange[num8, num9, num10])
					{
						this.blocks[num8, num9, num10].RefreshWaterMesh();
						this.blocks[num8, num9, num10].RecountLight();
					}
				}
			}
		}
	}

	protected void Init(int _sizeX, int _sizeY, int _sizeZ, bool needCreateTex = false)
	{
		if (this.initialized)
		{
			return;
		}
		PhotonNetwork.isMessageQueueRunning = true;
		this.gameItems = new List<GameItemStruct>();
		this.magicItems = new List<MagicItemStruct>();
		if (needCreateTex)
		{
		}
		Resources.UnloadUnusedAssets();
		GC.Collect();
		MonoBehaviour.print(string.Format("Init: {0} {1} {2}", _sizeX, _sizeY, _sizeZ));
		this.sizeX = _sizeX;
		this.sizeY = _sizeY;
		this.sizeZ = _sizeZ;
		this.blockSizeX = 16;
		this.blockSizeY = this.sizeY;
		this.blockSizeZ = 16;
		this.nBlocksX = Mathf.FloorToInt((float)(this.sizeX / this.blockSizeX));
		this.nBlocksY = Mathf.FloorToInt((float)(this.sizeY / this.blockSizeY));
		this.nBlocksZ = Mathf.FloorToInt((float)(this.sizeZ / this.blockSizeZ));
		this.cubes = new CubeStruct[this.sizeX, this.sizeY, this.sizeZ];
		this.blocks = new BlockScript[this.nBlocksX, this.nBlocksY, this.nBlocksZ];
		this.blocksToChange = new bool[this.nBlocksX, this.nBlocksY, this.nBlocksZ];
		this.waterBlocksToChange = new bool[this.nBlocksX, this.nBlocksY, this.nBlocksZ];
		this.isOccupied = new bool[this.sizeX, this.sizeY, this.sizeZ];
		this.checkLight = new bool[this.sizeX, this.sizeY, this.sizeZ];
		this.lightSurface = new int[this.sizeX, this.sizeZ];
		this.updateWaterBuffer1 = new WorldHolderScript.Vector3Int[this.sizeX * this.sizeZ * this.sizeY];
		this.updateWaterBuffer2 = new WorldHolderScript.Vector3Int[this.sizeX * this.sizeZ * this.sizeY];
		this.numUpdateWaterBuffer1 = (this.numUpdateWaterBuffer2 = 0);
		this.cubesLightChange = new int[4096, 3];
		this.cubesDrawTypes = new int[Kube.IS.gameItemsGO.Length + 155];
		for (int i = 0; i < Kube.IS.gameItemsGO.Length; i++)
		{
			this.cubesDrawTypes[i + 155] = 1;
		}
		this.cubePhys = new CubePhys[155];
		this._cubegrid = new CubeGrid(this.sizeX, this.sizeY, this.sizeZ);
		this.cubesDrawTypes[0] = 1;
		this.cubePhys[0] = CubePhys.air;
		this.cubesDrawTypes[1] = 0;
		this.cubePhys[1] = CubePhys.solid;
		this.cubesDrawTypes[2] = 0;
		this.cubePhys[2] = CubePhys.solid;
		this.cubesDrawTypes[4] = 0;
		this.cubePhys[4] = CubePhys.solid;
		this.cubesDrawTypes[5] = 0;
		this.cubePhys[5] = CubePhys.solid;
		this.cubesDrawTypes[3] = 0;
		this.cubePhys[3] = CubePhys.solid;
		this.cubesDrawTypes[16] = 0;
		this.cubePhys[16] = CubePhys.solid;
		this.cubesDrawTypes[11] = 0;
		this.cubePhys[11] = CubePhys.solid;
		this.cubesDrawTypes[12] = 0;
		this.cubePhys[12] = CubePhys.solid;
		this.cubesDrawTypes[8] = 0;
		this.cubePhys[8] = CubePhys.solid;
		this.cubesDrawTypes[17] = 0;
		this.cubePhys[17] = CubePhys.solid;
		this.cubesDrawTypes[13] = 0;
		this.cubePhys[13] = CubePhys.solid;
		this.cubesDrawTypes[10] = 0;
		this.cubePhys[10] = CubePhys.solid;
		this.cubesDrawTypes[6] = 0;
		this.cubePhys[6] = CubePhys.solid;
		this.cubesDrawTypes[14] = 0;
		this.cubePhys[14] = CubePhys.solid;
		this.cubesDrawTypes[7] = 0;
		this.cubePhys[7] = CubePhys.solid;
		this.cubesDrawTypes[15] = 0;
		this.cubePhys[15] = CubePhys.solid;
		this.cubesDrawTypes[18] = 0;
		this.cubePhys[18] = CubePhys.solid;
		this.cubesDrawTypes[19] = 0;
		this.cubePhys[19] = CubePhys.solid;
		this.cubesDrawTypes[20] = 0;
		this.cubePhys[20] = CubePhys.solid;
		this.cubesDrawTypes[21] = 0;
		this.cubePhys[21] = CubePhys.solid;
		this.cubesDrawTypes[22] = 0;
		this.cubePhys[22] = CubePhys.solid;
		this.cubesDrawTypes[23] = 0;
		this.cubePhys[23] = CubePhys.solid;
		this.cubesDrawTypes[24] = 0;
		this.cubePhys[24] = CubePhys.solid;
		this.cubesDrawTypes[25] = 0;
		this.cubePhys[25] = CubePhys.solid;
		this.cubesDrawTypes[26] = 0;
		this.cubePhys[26] = CubePhys.solid;
		this.cubesDrawTypes[27] = 0;
		this.cubePhys[27] = CubePhys.solid;
		this.cubesDrawTypes[28] = 0;
		this.cubePhys[28] = CubePhys.solid;
		this.cubesDrawTypes[29] = 0;
		this.cubePhys[29] = CubePhys.solid;
		this.cubesDrawTypes[30] = 0;
		this.cubePhys[30] = CubePhys.solid;
		this.cubesDrawTypes[31] = 0;
		this.cubePhys[31] = CubePhys.solid;
		this.cubesDrawTypes[32] = 0;
		this.cubePhys[32] = CubePhys.solid;
		this.cubesDrawTypes[33] = 0;
		this.cubePhys[33] = CubePhys.solid;
		this.cubesDrawTypes[34] = 0;
		this.cubePhys[34] = CubePhys.solid;
		this.cubesDrawTypes[35] = 0;
		this.cubePhys[35] = CubePhys.solid;
		this.cubesDrawTypes[36] = 0;
		this.cubePhys[36] = CubePhys.solid;
		this.cubesDrawTypes[39] = 0;
		this.cubePhys[39] = CubePhys.solid;
		this.cubesDrawTypes[40] = 0;
		this.cubePhys[40] = CubePhys.solid;
		this.cubesDrawTypes[41] = 0;
		this.cubePhys[41] = CubePhys.solid;
		this.cubesDrawTypes[42] = 0;
		this.cubePhys[42] = CubePhys.solid;
		this.cubesDrawTypes[43] = 0;
		this.cubePhys[43] = CubePhys.solid;
		this.cubesDrawTypes[44] = 0;
		this.cubePhys[44] = CubePhys.solid;
		this.cubesDrawTypes[45] = 0;
		this.cubePhys[45] = CubePhys.solid;
		this.cubesDrawTypes[46] = 0;
		this.cubePhys[46] = CubePhys.solid;
		this.cubesDrawTypes[47] = 0;
		this.cubePhys[47] = CubePhys.solid;
		this.cubesDrawTypes[48] = 0;
		this.cubePhys[48] = CubePhys.solid;
		this.cubesDrawTypes[50] = 0;
		this.cubePhys[50] = CubePhys.solid;
		this.cubesDrawTypes[49] = 0;
		this.cubePhys[49] = CubePhys.solid;
		this.cubesDrawTypes[51] = 0;
		this.cubePhys[51] = CubePhys.solid;
		this.cubesDrawTypes[52] = 0;
		this.cubePhys[52] = CubePhys.solid;
		this.cubesDrawTypes[53] = 0;
		this.cubePhys[53] = CubePhys.solid;
		this.cubesDrawTypes[54] = 0;
		this.cubePhys[54] = CubePhys.solid;
		this.cubesDrawTypes[55] = 0;
		this.cubePhys[55] = CubePhys.solid;
		this.cubesDrawTypes[56] = 0;
		this.cubePhys[56] = CubePhys.solid;
		this.cubesDrawTypes[57] = 0;
		this.cubePhys[57] = CubePhys.solid;
		this.cubesDrawTypes[58] = 0;
		this.cubePhys[58] = CubePhys.solid;
		this.cubesDrawTypes[59] = 0;
		this.cubePhys[59] = CubePhys.solid;
		this.cubesDrawTypes[60] = 0;
		this.cubePhys[60] = CubePhys.solid;
		this.cubesDrawTypes[61] = 0;
		this.cubePhys[61] = CubePhys.solid;
		this.cubesDrawTypes[64] = 1;
		this.cubePhys[64] = CubePhys.solid;
		this.cubesDrawTypes[65] = 1;
		this.cubePhys[65] = CubePhys.solid;
		this.cubesDrawTypes[66] = 1;
		this.cubePhys[66] = CubePhys.solid;
		this.cubesDrawTypes[67] = 1;
		this.cubePhys[67] = CubePhys.solid;
		this.cubesDrawTypes[68] = 1;
		this.cubePhys[68] = CubePhys.solid;
		this.cubesDrawTypes[69] = 1;
		this.cubePhys[69] = CubePhys.solid;
		this.cubesDrawTypes[70] = 1;
		this.cubePhys[70] = CubePhys.solid;
		this.cubesDrawTypes[71] = 1;
		this.cubePhys[71] = CubePhys.solid;
		this.cubesDrawTypes[72] = 1;
		this.cubePhys[72] = CubePhys.solid;
		this.cubesDrawTypes[73] = 1;
		this.cubePhys[73] = CubePhys.solid;
		this.cubesDrawTypes[74] = 1;
		this.cubePhys[74] = CubePhys.solid;
		this.cubesDrawTypes[75] = 1;
		this.cubePhys[75] = CubePhys.solid;
		this.cubesDrawTypes[76] = 1;
		this.cubePhys[76] = CubePhys.solid;
		this.cubesDrawTypes[77] = 1;
		this.cubePhys[77] = CubePhys.solid;
		this.cubesDrawTypes[78] = 1;
		this.cubePhys[78] = CubePhys.solid;
		this.cubesDrawTypes[79] = 1;
		this.cubePhys[79] = CubePhys.solid;
		this.cubesDrawTypes[128] = 2;
		this.cubePhys[128] = CubePhys.water;
		this.cubesSidesTex = new int[128, 3];
		this.cubesSidesTex[0, 0] = 0;
		this.cubesSidesTex[0, 1] = 0;
		this.cubesSidesTex[0, 2] = 0;
		this.cubesSidesTex[1, 0] = 1;
		this.cubesSidesTex[1, 1] = 2;
		this.cubesSidesTex[1, 2] = 3;
		this.cubesSidesTex[2, 0] = 3;
		this.cubesSidesTex[2, 1] = 3;
		this.cubesSidesTex[2, 2] = 3;
		this.cubesSidesTex[3, 0] = 4;
		this.cubesSidesTex[3, 1] = 4;
		this.cubesSidesTex[3, 2] = 4;
		this.cubesSidesTex[4, 0] = 5;
		this.cubesSidesTex[4, 1] = 5;
		this.cubesSidesTex[4, 2] = 5;
		this.cubesSidesTex[5, 0] = 6;
		this.cubesSidesTex[5, 1] = 6;
		this.cubesSidesTex[5, 2] = 6;
		this.cubesSidesTex[6, 0] = 7;
		this.cubesSidesTex[6, 1] = 7;
		this.cubesSidesTex[6, 2] = 7;
		this.cubesSidesTex[7, 0] = 8;
		this.cubesSidesTex[7, 1] = 8;
		this.cubesSidesTex[7, 2] = 8;
		this.cubesSidesTex[8, 0] = 9;
		this.cubesSidesTex[8, 1] = 9;
		this.cubesSidesTex[8, 2] = 9;
		this.cubesSidesTex[9, 0] = 10;
		this.cubesSidesTex[9, 1] = 10;
		this.cubesSidesTex[9, 2] = 10;
		this.cubesSidesTex[10, 0] = 11;
		this.cubesSidesTex[10, 1] = 11;
		this.cubesSidesTex[10, 2] = 11;
		this.cubesSidesTex[11, 0] = 12;
		this.cubesSidesTex[11, 1] = 12;
		this.cubesSidesTex[11, 2] = 12;
		this.cubesSidesTex[12, 0] = 13;
		this.cubesSidesTex[12, 1] = 13;
		this.cubesSidesTex[12, 2] = 13;
		this.cubesSidesTex[13, 0] = 14;
		this.cubesSidesTex[13, 1] = 14;
		this.cubesSidesTex[13, 2] = 14;
		this.cubesSidesTex[14, 0] = 15;
		this.cubesSidesTex[14, 1] = 16;
		this.cubesSidesTex[14, 2] = 15;
		this.cubesSidesTex[15, 0] = 17;
		this.cubesSidesTex[15, 1] = 17;
		this.cubesSidesTex[15, 2] = 17;
		this.cubesSidesTex[16, 0] = 18;
		this.cubesSidesTex[16, 1] = 18;
		this.cubesSidesTex[16, 2] = 18;
		this.cubesSidesTex[17, 0] = 19;
		this.cubesSidesTex[17, 1] = 19;
		this.cubesSidesTex[17, 2] = 19;
		this.cubesSidesTex[18, 0] = 20;
		this.cubesSidesTex[18, 1] = 20;
		this.cubesSidesTex[18, 2] = 20;
		this.cubesSidesTex[19, 0] = 21;
		this.cubesSidesTex[19, 1] = 21;
		this.cubesSidesTex[19, 2] = 21;
		this.cubesSidesTex[20, 0] = 22;
		this.cubesSidesTex[20, 1] = 22;
		this.cubesSidesTex[20, 2] = 22;
		this.cubesSidesTex[21, 0] = 23;
		this.cubesSidesTex[21, 1] = 23;
		this.cubesSidesTex[21, 2] = 23;
		this.cubesSidesTex[22, 0] = 24;
		this.cubesSidesTex[22, 1] = 24;
		this.cubesSidesTex[22, 2] = 24;
		this.cubesSidesTex[23, 0] = 25;
		this.cubesSidesTex[23, 1] = 25;
		this.cubesSidesTex[23, 2] = 25;
		this.cubesSidesTex[24, 0] = 26;
		this.cubesSidesTex[24, 1] = 26;
		this.cubesSidesTex[24, 2] = 26;
		this.cubesSidesTex[25, 0] = 27;
		this.cubesSidesTex[25, 1] = 27;
		this.cubesSidesTex[25, 2] = 27;
		this.cubesSidesTex[26, 0] = 28;
		this.cubesSidesTex[26, 1] = 28;
		this.cubesSidesTex[26, 2] = 28;
		this.cubesSidesTex[27, 0] = 29;
		this.cubesSidesTex[27, 1] = 29;
		this.cubesSidesTex[27, 2] = 29;
		this.cubesSidesTex[28, 0] = 30;
		this.cubesSidesTex[28, 1] = 30;
		this.cubesSidesTex[28, 2] = 30;
		this.cubesSidesTex[29, 0] = 31;
		this.cubesSidesTex[29, 1] = 31;
		this.cubesSidesTex[29, 2] = 31;
		this.cubesSidesTex[30, 0] = 32;
		this.cubesSidesTex[30, 1] = 32;
		this.cubesSidesTex[30, 2] = 32;
		this.cubesSidesTex[31, 0] = 33;
		this.cubesSidesTex[31, 1] = 33;
		this.cubesSidesTex[31, 2] = 33;
		this.cubesSidesTex[32, 0] = 34;
		this.cubesSidesTex[32, 1] = 34;
		this.cubesSidesTex[32, 2] = 34;
		this.cubesSidesTex[33, 0] = 35;
		this.cubesSidesTex[33, 1] = 35;
		this.cubesSidesTex[33, 2] = 35;
		this.cubesSidesTex[34, 0] = 36;
		this.cubesSidesTex[34, 1] = 36;
		this.cubesSidesTex[34, 2] = 36;
		this.cubesSidesTex[35, 0] = 36;
		this.cubesSidesTex[35, 1] = 37;
		this.cubesSidesTex[35, 2] = 21;
		this.cubesSidesTex[36, 0] = 36;
		this.cubesSidesTex[36, 1] = 38;
		this.cubesSidesTex[36, 2] = 3;
		this.cubesSidesTex[37, 0] = 39;
		this.cubesSidesTex[37, 1] = 39;
		this.cubesSidesTex[37, 2] = 39;
		this.cubesSidesTex[38, 0] = 40;
		this.cubesSidesTex[38, 1] = 40;
		this.cubesSidesTex[38, 2] = 40;
		this.cubesSidesTex[39, 0] = 41;
		this.cubesSidesTex[39, 1] = 41;
		this.cubesSidesTex[39, 2] = 41;
		this.cubesSidesTex[40, 0] = 42;
		this.cubesSidesTex[40, 1] = 42;
		this.cubesSidesTex[40, 2] = 42;
		this.cubesSidesTex[41, 0] = 43;
		this.cubesSidesTex[41, 1] = 43;
		this.cubesSidesTex[41, 2] = 43;
		this.cubesSidesTex[42, 0] = 44;
		this.cubesSidesTex[42, 1] = 44;
		this.cubesSidesTex[42, 2] = 44;
		this.cubesSidesTex[43, 0] = 45;
		this.cubesSidesTex[43, 1] = 45;
		this.cubesSidesTex[43, 2] = 45;
		this.cubesSidesTex[44, 0] = 46;
		this.cubesSidesTex[44, 1] = 46;
		this.cubesSidesTex[44, 2] = 46;
		this.cubesSidesTex[45, 0] = 47;
		this.cubesSidesTex[45, 1] = 47;
		this.cubesSidesTex[45, 2] = 47;
		this.cubesSidesTex[46, 0] = 48;
		this.cubesSidesTex[46, 1] = 48;
		this.cubesSidesTex[46, 2] = 48;
		this.cubesSidesTex[47, 0] = 49;
		this.cubesSidesTex[47, 1] = 49;
		this.cubesSidesTex[47, 2] = 49;
		this.cubesSidesTex[48, 0] = 50;
		this.cubesSidesTex[48, 1] = 50;
		this.cubesSidesTex[48, 2] = 50;
		this.cubesSidesTex[49, 0] = 51;
		this.cubesSidesTex[49, 1] = 51;
		this.cubesSidesTex[49, 2] = 52;
		this.cubesSidesTex[50, 0] = 52;
		this.cubesSidesTex[50, 1] = 52;
		this.cubesSidesTex[50, 2] = 52;
		this.cubesSidesTex[51, 0] = 53;
		this.cubesSidesTex[51, 1] = 53;
		this.cubesSidesTex[51, 2] = 53;
		this.cubesSidesTex[52, 0] = 54;
		this.cubesSidesTex[52, 1] = 54;
		this.cubesSidesTex[52, 2] = 54;
		this.cubesSidesTex[53, 0] = 55;
		this.cubesSidesTex[53, 1] = 55;
		this.cubesSidesTex[53, 2] = 55;
		this.cubesSidesTex[54, 0] = 56;
		this.cubesSidesTex[54, 1] = 56;
		this.cubesSidesTex[54, 2] = 56;
		this.cubesSidesTex[55, 0] = 57;
		this.cubesSidesTex[55, 1] = 57;
		this.cubesSidesTex[55, 2] = 57;
		this.cubesSidesTex[56, 0] = 58;
		this.cubesSidesTex[56, 1] = 58;
		this.cubesSidesTex[56, 2] = 58;
		this.cubesSidesTex[57, 0] = 59;
		this.cubesSidesTex[57, 1] = 59;
		this.cubesSidesTex[57, 2] = 59;
		this.cubesSidesTex[58, 0] = 60;
		this.cubesSidesTex[58, 1] = 60;
		this.cubesSidesTex[58, 2] = 60;
		this.cubesSidesTex[59, 0] = 61;
		this.cubesSidesTex[59, 1] = 61;
		this.cubesSidesTex[59, 2] = 61;
		this.cubesSidesTex[60, 0] = 62;
		this.cubesSidesTex[60, 1] = 62;
		this.cubesSidesTex[60, 2] = 62;
		this.cubesSidesTex[61, 0] = 63;
		this.cubesSidesTex[61, 1] = 63;
		this.cubesSidesTex[61, 2] = 63;
		this.cubesSidesTex[64, 0] = 0;
		this.cubesSidesTex[64, 1] = 0;
		this.cubesSidesTex[64, 2] = 0;
		this.cubesSidesTex[65, 0] = 1;
		this.cubesSidesTex[65, 1] = 1;
		this.cubesSidesTex[65, 2] = 1;
		this.cubesSidesTex[66, 0] = 2;
		this.cubesSidesTex[66, 1] = 2;
		this.cubesSidesTex[66, 2] = 2;
		this.cubesSidesTex[67, 0] = 3;
		this.cubesSidesTex[67, 1] = 3;
		this.cubesSidesTex[67, 2] = 3;
		this.cubesSidesTex[68, 0] = 4;
		this.cubesSidesTex[68, 1] = 4;
		this.cubesSidesTex[68, 2] = 4;
		this.cubesSidesTex[69, 0] = 5;
		this.cubesSidesTex[69, 1] = 5;
		this.cubesSidesTex[69, 2] = 5;
		this.cubesSidesTex[70, 0] = 6;
		this.cubesSidesTex[70, 1] = 6;
		this.cubesSidesTex[70, 2] = 6;
		this.cubesSidesTex[71, 0] = 7;
		this.cubesSidesTex[71, 1] = 7;
		this.cubesSidesTex[71, 2] = 7;
		this.cubesSidesTex[72, 0] = 8;
		this.cubesSidesTex[72, 1] = 8;
		this.cubesSidesTex[72, 2] = 8;
		this.cubesSidesTex[73, 0] = 9;
		this.cubesSidesTex[73, 1] = 9;
		this.cubesSidesTex[73, 2] = 9;
		this.cubesSidesTex[74, 0] = 10;
		this.cubesSidesTex[74, 1] = 10;
		this.cubesSidesTex[74, 2] = 10;
		this.cubesSidesTex[75, 0] = 11;
		this.cubesSidesTex[75, 1] = 11;
		this.cubesSidesTex[75, 2] = 11;
		this.cubesSidesTex[76, 0] = 12;
		this.cubesSidesTex[76, 1] = 12;
		this.cubesSidesTex[76, 2] = 12;
		this.cubesSidesTex[77, 0] = 13;
		this.cubesSidesTex[77, 1] = 13;
		this.cubesSidesTex[77, 2] = 13;
		this.cubesSidesTex[78, 0] = 14;
		this.cubesSidesTex[78, 1] = 14;
		this.cubesSidesTex[78, 2] = 14;
		this.cubesSidesTex[77, 0] = 15;
		this.cubesSidesTex[77, 1] = 15;
		this.cubesSidesTex[77, 2] = 15;
		this.cubesTexUV = new Vector2[64, 4];
		for (int j = 0; j < 64; j++)
		{
			float num = (float)(j % 8) / 8f;
			float num2 = Mathf.Floor((float)j / 8f) / 8f;
			this.cubesTexUV[j, 1].x = num;
			this.cubesTexUV[j, 1].y = 1f - num2;
			this.cubesTexUV[j, 2].x = num + 0.125f;
			this.cubesTexUV[j, 2].y = 1f - num2;
			this.cubesTexUV[j, 3].x = num + 0.125f;
			this.cubesTexUV[j, 3].y = 1f - (num2 + 0.125f);
			this.cubesTexUV[j, 0].x = num;
			this.cubesTexUV[j, 0].y = 1f - (num2 + 0.125f);
		}
		this.transportRespawnS = new TransportRespawnScript[1024];
		this.transportLastDieTime = new float[1024];
		this.triggerS = new TriggerScript[1024];
		this.monsterRespawnS = new MonsterRespawnScript[1024];
		this.monsterLastDieTime = new float[1024];
		this.AAgo = new GameObject[1024];
		this.wireS = new WireScript[2048];
		this.initialized = true;
	}

	public void RedrawWorld(bool drawAll = true, bool onlyRelight = false, bool calculateLight = false)
	{
		int tickCount = Environment.TickCount;
		if (calculateLight)
		{
			this.CalculateLight(0, this.nBlocksX - 1, 0, this.nBlocksZ - 1, true);
		}
		for (int i = 0; i < this.nBlocksX; i++)
		{
			for (int j = 0; j < this.nBlocksY; j++)
			{
				for (int k = 0; k < this.nBlocksZ; k++)
				{
					if (this.blocksToChange[i, j, k] || drawAll)
					{
						bool flag = false;
						for (int l = 0; l < this.blockSizeX; l++)
						{
							for (int m = 0; m < this.blockSizeZ; m++)
							{
								for (int n = 0; n < this.blockSizeY; n++)
								{
									if (this.cubes[i * this.blockSizeX + l, j * this.blockSizeY + n, k * this.blockSizeZ + m].type != 0)
									{
										flag = true;
										break;
									}
								}
							}
						}
						if (flag && this.blocks[i, j, k] == null)
						{
							BlockScript component = (UnityEngine.Object.Instantiate(this.blockPrefab, new Vector3((float)(i * this.blockSizeX), (float)(j * this.blockSizeY), (float)(k * this.blockSizeZ)), Quaternion.identity) as GameObject).GetComponent<BlockScript>();
							component.SetBlock(new Vector3((float)i * (float)this.blockSizeX, (float)j * (float)this.blockSizeY, (float)k * (float)this.blockSizeZ), new Vector3((float)i * (float)this.blockSizeX + (float)this.blockSizeX, (float)j * (float)this.blockSizeY + (float)this.blockSizeY, (float)k * (float)this.blockSizeZ + (float)this.blockSizeZ));
							this.blocks[i, j, k] = component;
						}
						else if (!flag)
						{
							if (this.blocks[i, j, k] != null)
							{
								this.blocks[i, j, k].DestroyBlock();
							}
							goto IL_229;
						}
						if (!onlyRelight)
						{
							this.blocks[i, j, k].RefreshMeshes();
						}
						else
						{
							this.blocks[i, j, k].RecountLight();
						}
					}
					IL_229:;
				}
			}
		}
		int tickCount2 = Environment.TickCount;
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"Delta time ( onlyRelight - ",
			onlyRelight,
			"): ",
			tickCount2 - tickCount
		}));
	}

	private void GenerateBounds()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Kube.OH.boundsPlane, new Vector3(-0.5f, (float)this.sizeY / 2f, (float)this.sizeZ / 2f - 0.5f), Quaternion.identity) as GameObject;
		gameObject.transform.localScale = new Vector3(1f, (float)this.sizeY / 10f, (float)this.sizeZ / 10f);
		gameObject = (UnityEngine.Object.Instantiate(Kube.OH.boundsPlane, new Vector3((float)this.sizeX - 0.5f, (float)this.sizeY / 2f, (float)this.sizeZ / 2f - 0.5f), Quaternion.Euler(0f, 180f, 0f)) as GameObject);
		gameObject.transform.localScale = new Vector3(1f, (float)this.sizeY / 10f, (float)this.sizeZ / 10f);
		gameObject = (UnityEngine.Object.Instantiate(Kube.OH.boundsPlane, new Vector3((float)this.sizeX / 2f - 0.5f, (float)this.sizeY / 2f, -0.5f), Quaternion.Euler(0f, -90f, 0f)) as GameObject);
		gameObject.transform.localScale = new Vector3(1f, (float)this.sizeY / 10f, (float)this.sizeZ / 10f);
		gameObject = (UnityEngine.Object.Instantiate(Kube.OH.boundsPlane, new Vector3((float)this.sizeX / 2f - 0.5f, (float)this.sizeY / 2f, (float)this.sizeZ - 0.5f), Quaternion.Euler(0f, 90f, 0f)) as GameObject);
		gameObject.transform.localScale = new Vector3(1f, (float)this.sizeY / 10f, (float)this.sizeZ / 10f);
		gameObject = (UnityEngine.Object.Instantiate(Kube.OH.boundsPlane, new Vector3((float)this.sizeX / 2f - 0.5f, (float)this.sizeY, (float)this.sizeZ / 2f - 0.5f), Quaternion.Euler(0f, 0f, -90f)) as GameObject);
		gameObject.transform.localScale = new Vector3(1f, (float)this.sizeX / 10f, (float)this.sizeZ / 10f);
	}

	private void _GenerateWorld()
	{
		int newMapType = Kube.OH.tempMap.NewMapType;
		MonoBehaviour.print("Selected new map: " + newMapType);
		if (this.defaultMaps[newMapType].width == 128 && this.defaultMaps[newMapType].height == 128)
		{
			this.containerSize = 1048576;
			this.Init(128, 96, 128, true);
		}
		else
		{
			if (this.defaultMaps[newMapType].width != 224 || this.defaultMaps[newMapType].height != 224)
			{
				MonoBehaviour.print("Bad map size");
				return;
			}
			this.containerSize = 4194304;
			this.Init(224, 125, 224, true);
		}
		int num = (int)((float)this.sizeY * 0.3f);
		for (int i = 0; i < this.sizeX; i++)
		{
			for (int j = 0; j < this.sizeZ; j++)
			{
				int num2 = Mathf.RoundToInt(this.defaultMaps[newMapType].GetPixel(i, j).grayscale * (float)(this.sizeY - 1) * 0.3f + (float)this.sizeY * 0.25f);
				for (int k = 0; k < this.sizeY; k++)
				{
					if (k > num2 && k > num)
					{
						this.cubes[i, k, j].type = 0;
						this.cubes[i, k, j].health = (byte)Mathf.Min(255f, 64f * Kube.OH.cubesStrength[(int)this.cubes[i, k, j].type]);
						this.cubes[i, k, j].phys = this.cubePhys[(int)this.cubes[i, k, j].type];
						this.ChangeWorldBytesCube(i, k, j, this.cubes[i, k, j].type, 0);
					}
					else if (k > num2 && k <= num)
					{
						this.cubes[i, k, j].type = 128;
						this.cubes[i, k, j].waterLevel = this.maxWaterLevel;
						this.cubes[i, k, j].health = (byte)Mathf.Min(255f, 64f * Kube.OH.cubesStrength[(int)this.cubes[i, k, j].type]);
						this.cubes[i, k, j].phys = this.cubePhys[(int)this.cubes[i, k, j].type];
						this.ChangeWorldBytesCube(i, k, j, this.cubes[i, k, j].type, this.cubes[i, k, j].waterLevel);
					}
					else if (k > num && k == num2)
					{
						this.cubes[i, k, j].type = 1;
						this.cubes[i, k, j].health = (byte)Mathf.Min(255f, 64f * Kube.OH.cubesStrength[(int)this.cubes[i, k, j].type]);
						this.cubes[i, k, j].phys = this.cubePhys[(int)this.cubes[i, k, j].type];
						this.ChangeWorldBytesCube(i, k, j, this.cubes[i, k, j].type, 0);
					}
					else if (k <= num && k <= num2 && k >= num2 - 2)
					{
						this.cubes[i, k, j].type = 13;
						this.cubes[i, k, j].health = (byte)Mathf.Min(255f, 64f * Kube.OH.cubesStrength[(int)this.cubes[i, k, j].type]);
						this.cubes[i, k, j].phys = this.cubePhys[(int)this.cubes[i, k, j].type];
						this.ChangeWorldBytesCube(i, k, j, this.cubes[i, k, j].type, 0);
					}
					else if (k <= num2 && k >= num2 - 2)
					{
						this.cubes[i, k, j].type = 2;
						this.cubes[i, k, j].health = (byte)Mathf.Min(255f, 32f * Kube.OH.cubesStrength[(int)this.cubes[i, k, j].type]);
						this.cubes[i, k, j].phys = this.cubePhys[(int)this.cubes[i, k, j].type];
						this.ChangeWorldBytesCube(i, k, j, this.cubes[i, k, j].type, 0);
					}
					else
					{
						this.cubes[i, k, j].type = 18;
						this.cubes[i, k, j].health = (byte)Mathf.Min(255f, 64f * Kube.OH.cubesStrength[(int)this.cubes[i, k, j].type]);
						this.cubes[i, k, j].phys = this.cubePhys[(int)this.cubes[i, k, j].type];
						this.ChangeWorldBytesCube(i, k, j, this.cubes[i, k, j].type, 0);
					}
				}
			}
		}
		this.GenerateBounds();
	}

	public void CalculateLight(int blockXBegin, int blockXEnd, int blockZBegin, int blockZEnd, bool fullCheck = true)
	{
		if (blockXBegin < 0)
		{
			blockXBegin = 0;
		}
		if (blockXBegin >= this.nBlocksX - 1)
		{
			blockXBegin = this.nBlocksX - 1;
		}
		if (blockZBegin < 0)
		{
			blockZBegin = 0;
		}
		if (blockZBegin >= this.nBlocksZ - 1)
		{
			blockZBegin = this.nBlocksZ - 1;
		}
		if (blockXBegin * this.blockSizeX == 0)
		{
		}
		int num = (blockXEnd + 1) * this.blockSizeX;
		if (num >= this.sizeX)
		{
			num = this.sizeX - 1;
		}
		if (blockZBegin * this.blockSizeZ == 0)
		{
		}
		int num2 = (blockZEnd + 1) * this.blockSizeZ;
		if (num2 >= this.sizeZ)
		{
			num2 = this.sizeZ - 1;
		}
		if (this.lightWaveSources == null)
		{
			this.lightWaveSources = new WorldHolderScript.Vector3Int[this.sizeX * this.sizeY * this.sizeZ];
		}
		if (this.lightItemWaveSources == null)
		{
			this.lightItemWaveSources = new WorldHolderScript.Vector3Int[this.sizeX * this.sizeY * this.sizeZ];
		}
		if (this.antiLightWaveSources == null)
		{
			this.antiLightWaveSources = new WorldHolderScript.Vector3Int[this.sizeX * this.sizeY * this.sizeZ];
		}
		if (this.antiLightItemWaveSources == null)
		{
			this.antiLightItemWaveSources = new WorldHolderScript.Vector3Int[this.sizeX * this.sizeY * this.sizeZ];
		}
		if (this.tmpLight == null)
		{
			this.tmpLight = new WorldHolderScript.Vector3Int[this.sizeX * this.sizeY * this.sizeZ];
		}
		if (fullCheck)
		{
			int num3 = blockXBegin * this.blockSizeX;
			if (num3 == 0)
			{
				num3 = 1;
			}
			num = (blockXEnd + 1) * this.blockSizeX;
			if (num >= this.sizeX)
			{
				num = this.sizeX - 1;
			}
			int num4 = blockZBegin * this.blockSizeZ;
			if (num4 == 0)
			{
				num4 = 1;
			}
			num2 = (blockZEnd + 1) * this.blockSizeZ;
			if (num2 >= this.sizeZ)
			{
				num2 = this.sizeZ - 1;
			}
			for (int i = num3; i < num; i++)
			{
				for (int j = 0; j < this.sizeY; j++)
				{
					for (int k = num4; k < num2; k++)
					{
						this.checkLight[i, j, k] = false;
					}
				}
			}
			this.numLightWaveSources = 0;
			for (int l = num3; l < num; l++)
			{
				for (int m = num4; m < num2; m++)
				{
					bool flag = false;
					for (int n = this.sizeY - 1; n >= 0; n--)
					{
						this.cubes[l, n, m].lightR = 0;
						this.cubes[l, n, m].lightG = 0;
						this.cubes[l, n, m].lightB = 0;
						if (!flag)
						{
							int num5 = (this.cubes[l, n, m].type >= 155) ? 1 : this.cubesDrawTypes[(int)this.cubes[l, n, m].type];
							if (this.cubes[l, n, m].data != 0)
							{
								num5 = 1;
							}
							if (num5 != 1 && num5 != 2)
							{
								flag = true;
								this.lightSurface[l, m] = n + 1;
								this.cubes[l, n, m].sunLight = 0;
							}
							else
							{
								byte maxValue = byte.MaxValue;
								if (this.cubes[l, n, m].sunLight != maxValue)
								{
									this.blocksToChange[Mathf.FloorToInt((float)l / (float)this.blockSizeX), Mathf.FloorToInt((float)n / (float)this.blockSizeY), Mathf.FloorToInt((float)m / (float)this.blockSizeZ)] = true;
								}
								this.checkLight[l, n, m] = true;
								this.cubes[l, n, m].sunLight = maxValue;
							}
						}
						else
						{
							this.cubes[l, n, m].sunLight = 0;
						}
					}
				}
			}
			for (int num6 = num3; num6 < num; num6++)
			{
				for (int num7 = num4; num7 < num2; num7++)
				{
					int num8 = Mathf.Max(new int[]
					{
						this.lightSurface[num6 - 1, num7],
						this.lightSurface[num6, num7 - 1],
						this.lightSurface[num6 + 1, num7],
						this.lightSurface[num6, num7 + 1]
					});
					for (int num9 = this.lightSurface[num6, num7]; num9 < num8; num9++)
					{
						this.lightWaveSources[this.numLightWaveSources].x = (byte)num6;
						this.lightWaveSources[this.numLightWaveSources].y = (byte)num9;
						this.lightWaveSources[this.numLightWaveSources].z = (byte)num7;
						this.numLightWaveSources++;
						if (!fullCheck)
						{
						}
					}
				}
			}
			this.numLightItemWaveSources = 0;
			for (int num10 = 0; num10 < this.gameItems.Count; num10++)
			{
				GameItemStruct gameItemStruct = this.gameItems[num10];
				if (gameItemStruct.lightColor.grayscale > 0f)
				{
					this.cubes[gameItemStruct.x, gameItemStruct.y, gameItemStruct.z].lightR = (byte)(255f * gameItemStruct.lightColor.r);
					this.cubes[gameItemStruct.x, gameItemStruct.y, gameItemStruct.z].lightG = (byte)(255f * gameItemStruct.lightColor.g);
					this.cubes[gameItemStruct.x, gameItemStruct.y, gameItemStruct.z].lightB = (byte)(255f * gameItemStruct.lightColor.b);
					this.lightItemWaveSources[this.numLightItemWaveSources].x = (byte)gameItemStruct.x;
					this.lightItemWaveSources[this.numLightItemWaveSources].y = (byte)gameItemStruct.y;
					this.lightItemWaveSources[this.numLightItemWaveSources].z = (byte)gameItemStruct.z;
					this.numLightItemWaveSources++;
				}
			}
		}
		else
		{
			int num3 = this.sizeX - 1;
			int num4 = this.sizeZ - 1;
			for (int num11 = this.numCubesLightChange - 1; num11 >= 0; num11--)
			{
				if (this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].sunLight == 0)
				{
					byte b = 0;
					int num12 = 0;
					for (int num13 = 0; num13 < 6; num13++)
					{
						int num14 = this.cubesLightChange[num11, 0] + this.lightNeibours[num13, 0];
						int num15 = this.cubesLightChange[num11, 1] + this.lightNeibours[num13, 1];
						int num16 = this.cubesLightChange[num11, 2] + this.lightNeibours[num13, 2];
						if (this.IsInWorld(num14, num15, num16) && this.cubes[num14, num15, num16].sunLight > b)
						{
							b = this.cubes[num14, num15, num16].sunLight;
							num12 = num13;
						}
					}
					if (b == 255 && num12 == 1)
					{
						this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].sunLight = byte.MaxValue;
						for (int num17 = this.cubesLightChange[num11, 1]; num17 >= 0; num17--)
						{
							int num18 = this.cubesLightChange[num11, 0];
							int num19 = num17;
							int num20 = this.cubesLightChange[num11, 2];
							int num21 = (this.cubes[num18, num19, num20].data != 0) ? 1 : this.cubesDrawTypes[(int)this.cubes[num18, num19, num20].type];
							if (num21 == 1 || num21 == 2)
							{
								this.cubes[num18, num19, num20].sunLight = byte.MaxValue;
								this.lightWaveSources[this.numLightWaveSources].x = (byte)num18;
								this.lightWaveSources[this.numLightWaveSources].y = (byte)num19;
								this.lightWaveSources[this.numLightWaveSources].z = (byte)num20;
								this.numLightWaveSources++;
								this.blocksToChange[Mathf.FloorToInt((float)num18 / (float)this.blockSizeX), Mathf.FloorToInt((float)num19 / (float)this.blockSizeY), Mathf.FloorToInt((float)num20 / (float)this.blockSizeZ)] = true;
							}
						}
					}
					else if (b != 0)
					{
						this.lightWaveSources[this.numLightWaveSources].x = (byte)(this.cubesLightChange[num11, 0] + this.lightNeibours[num12, 0]);
						this.lightWaveSources[this.numLightWaveSources].y = (byte)(this.cubesLightChange[num11, 1] + this.lightNeibours[num12, 1]);
						this.lightWaveSources[this.numLightWaveSources].z = (byte)(this.cubesLightChange[num11, 2] + this.lightNeibours[num12, 2]);
						this.numLightWaveSources++;
					}
				}
				else
				{
					if (this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].sunLight == 255)
					{
						this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].sunLight = 0;
						this.numAntiLightWaveSources = 0;
						this.initAntiLight = byte.MaxValue;
						for (int num22 = this.cubesLightChange[num11, 1] - 1; num22 >= 0; num22--)
						{
							if (this.cubesDrawTypes[(int)this.cubes[this.cubesLightChange[num11, 0], num22, this.cubesLightChange[num11, 2]].type] == 0)
							{
								break;
							}
							this.antiLightWaveSources[this.numAntiLightWaveSources].x = (byte)this.cubesLightChange[num11, 0];
							this.antiLightWaveSources[this.numAntiLightWaveSources].y = (byte)num22;
							this.antiLightWaveSources[this.numAntiLightWaveSources].z = (byte)this.cubesLightChange[num11, 2];
							this.numAntiLightWaveSources++;
							this.cubes[this.cubesLightChange[num11, 0], num22, this.cubesLightChange[num11, 2]].sunLight = 0;
						}
					}
					else
					{
						this.numAntiLightWaveSources = 0;
						this.initAntiLight = this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].sunLight;
						this.antiLightWaveSources[this.numAntiLightWaveSources].x = (byte)this.cubesLightChange[num11, 0];
						this.antiLightWaveSources[this.numAntiLightWaveSources].y = (byte)this.cubesLightChange[num11, 1];
						this.antiLightWaveSources[this.numAntiLightWaveSources].z = (byte)this.cubesLightChange[num11, 2];
						this.numAntiLightWaveSources++;
						this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].sunLight = 0;
					}
					WorldHolderScript.Vector3Int[] array = this.antiLightWaveSources;
					WorldHolderScript.Vector3Int[] array2 = this.tmpLight;
					int num23 = 0;
					while (this.numAntiLightWaveSources > 0)
					{
						num23++;
						this.numAntiLightWaveSourcesNew = 0;
						byte b2 = (byte)Mathf.Max(0, (int)this.initAntiLight - 16 * num23);
						if (num23 > 16)
						{
							break;
						}
						for (int num24 = 0; num24 < this.numAntiLightWaveSources; num24++)
						{
							if (b2 == 0)
							{
								break;
							}
							for (int num25 = 0; num25 < 6; num25++)
							{
								WorldHolderScript.Vector3Int vector3Int = array[num24];
								int num26 = (int)vector3Int.x + this.lightNeibours[num25, 0];
								int num27 = (int)vector3Int.y + this.lightNeibours[num25, 1];
								int num28 = (int)vector3Int.z + this.lightNeibours[num25, 2];
								if (this.IsInWorld(num26, num27, num28))
								{
									if (this.cubes[num26, num27, num28].sunLight != 0)
									{
										if (this.cubes[num26, num27, num28].sunLight > b2)
										{
											this.lightWaveSources[this.numLightWaveSources].x = (byte)num26;
											this.lightWaveSources[this.numLightWaveSources].y = (byte)num27;
											this.lightWaveSources[this.numLightWaveSources].z = (byte)num28;
											this.numLightWaveSources++;
										}
										else
										{
											int num29 = (this.cubes[num26, num27, num28].data != 0) ? 1 : this.cubesDrawTypes[(int)this.cubes[num26, num27, num28].type];
											if (num29 == 1 || num29 == 2)
											{
												this.cubes[num26, num27, num28].sunLight = 0;
												array2[this.numAntiLightWaveSourcesNew].x = (byte)num26;
												array2[this.numAntiLightWaveSourcesNew].y = (byte)num27;
												array2[this.numAntiLightWaveSourcesNew].z = (byte)num28;
												this.numAntiLightWaveSourcesNew++;
											}
										}
									}
								}
							}
						}
						this.numAntiLightWaveSources = this.numAntiLightWaveSourcesNew;
						WorldHolderScript.Vector3Int[] array3 = array2;
						array2 = array;
						array = array3;
					}
				}
				if (this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].lightR == 0 && this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].lightG == 0 && this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].lightB == 0)
				{
					byte b3 = 0;
					byte b4 = 0;
					byte b5 = 0;
					int num30 = 0;
					int num31 = 0;
					int num32 = 0;
					for (int num33 = 0; num33 < 6; num33++)
					{
						int num34 = this.cubesLightChange[num11, 0] + this.lightNeibours[num33, 0];
						int num35 = this.cubesLightChange[num11, 1] + this.lightNeibours[num33, 1];
						int num36 = this.cubesLightChange[num11, 2] + this.lightNeibours[num33, 2];
						if (this.IsInWorld(num34, num35, num36))
						{
							if (this.cubes[num34, num35, num36].lightR > b3)
							{
								b3 = this.cubes[num34, num35, num36].lightR;
								num30 = num33;
							}
							if (this.cubes[num34, num35, num36].lightG > b4)
							{
								b4 = this.cubes[num34, num35, num36].lightG;
								num31 = num33;
							}
							if (this.cubes[num34, num35, num36].lightB > b5)
							{
								b5 = this.cubes[num34, num35, num36].lightB;
								num32 = num33;
							}
						}
					}
					if (b3 != 0)
					{
						this.lightItemWaveSources[this.numLightItemWaveSources].x = (byte)(this.cubesLightChange[num11, 0] + this.lightNeibours[num30, 0]);
						this.lightItemWaveSources[this.numLightItemWaveSources].y = (byte)(this.cubesLightChange[num11, 1] + this.lightNeibours[num30, 1]);
						this.lightItemWaveSources[this.numLightItemWaveSources].z = (byte)(this.cubesLightChange[num11, 2] + this.lightNeibours[num30, 2]);
						this.numLightItemWaveSources++;
					}
					if (b4 != 0)
					{
						this.lightItemWaveSources[this.numLightItemWaveSources].x = (byte)(this.cubesLightChange[num11, 0] + this.lightNeibours[num31, 0]);
						this.lightItemWaveSources[this.numLightItemWaveSources].y = (byte)(this.cubesLightChange[num11, 1] + this.lightNeibours[num31, 1]);
						this.lightItemWaveSources[this.numLightItemWaveSources].z = (byte)(this.cubesLightChange[num11, 2] + this.lightNeibours[num31, 2]);
						this.numLightItemWaveSources++;
					}
					if (b5 != 0)
					{
						this.lightItemWaveSources[this.numLightItemWaveSources].x = (byte)(this.cubesLightChange[num11, 0] + this.lightNeibours[num32, 0]);
						this.lightItemWaveSources[this.numLightItemWaveSources].y = (byte)(this.cubesLightChange[num11, 1] + this.lightNeibours[num32, 1]);
						this.lightItemWaveSources[this.numLightItemWaveSources].z = (byte)(this.cubesLightChange[num11, 2] + this.lightNeibours[num32, 2]);
						this.numLightItemWaveSources++;
					}
				}
				else
				{
					this.numAntiLightItemWaveSources = 0;
					this.initAntiLightItem = (byte)Mathf.Max(new int[]
					{
						(int)this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].lightR,
						(int)this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].lightG,
						(int)this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].lightB
					});
					this.antiLightItemWaveSources[this.numAntiLightItemWaveSources].x = (byte)this.cubesLightChange[num11, 0];
					this.antiLightItemWaveSources[this.numAntiLightItemWaveSources].y = (byte)this.cubesLightChange[num11, 1];
					this.antiLightItemWaveSources[this.numAntiLightItemWaveSources].z = (byte)this.cubesLightChange[num11, 2];
					this.numAntiLightItemWaveSources++;
					this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].lightR = 0;
					this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].lightG = 0;
					this.cubes[this.cubesLightChange[num11, 0], this.cubesLightChange[num11, 1], this.cubesLightChange[num11, 2]].lightB = 0;
					int num37 = 0;
					WorldHolderScript.Vector3Int[] array4 = this.antiLightItemWaveSources;
					WorldHolderScript.Vector3Int[] array5 = this.tmpLight;
					while (this.numAntiLightItemWaveSources > 0)
					{
						num37++;
						this.numAntiLightItemWaveSourcesNew = 0;
						byte b6 = (byte)Mathf.Max(0, (int)this.initAntiLightItem - 16 * num37);
						if (num37 > 16)
						{
							break;
						}
						for (int num38 = 0; num38 < this.numAntiLightItemWaveSources; num38++)
						{
							if (b6 == 0)
							{
								break;
							}
							for (int num39 = 0; num39 < 6; num39++)
							{
								WorldHolderScript.Vector3Int vector3Int2 = array4[num38];
								int num40 = (int)vector3Int2.x + this.lightNeibours[num39, 0];
								int num41 = (int)vector3Int2.y + this.lightNeibours[num39, 1];
								int num42 = (int)vector3Int2.z + this.lightNeibours[num39, 2];
								if (this.IsInWorld(num40, num41, num42))
								{
									if (this.cubes[num40, num41, num42].lightR != 0 || this.cubes[num40, num41, num42].lightG != 0 || this.cubes[num40, num41, num42].lightB != 0)
									{
										if (this.cubes[num40, num41, num42].lightR >= b6 && this.cubes[num40, num41, num42].lightG >= b6 && this.cubes[num40, num41, num42].lightB >= b6)
										{
											this.lightItemWaveSources[this.numLightItemWaveSources].x = (byte)num40;
											this.lightItemWaveSources[this.numLightItemWaveSources].y = (byte)num41;
											this.lightItemWaveSources[this.numLightItemWaveSources].z = (byte)num42;
											this.numLightItemWaveSources++;
										}
										else if (this.cubes[num40, num41, num42].isLight)
										{
											this.lightItemWaveSources[this.numLightItemWaveSources].x = (byte)num40;
											this.lightItemWaveSources[this.numLightItemWaveSources].y = (byte)num41;
											this.lightItemWaveSources[this.numLightItemWaveSources].z = (byte)num42;
											this.numLightItemWaveSources++;
											array5[this.numAntiLightItemWaveSourcesNew].x = (byte)num40;
											array5[this.numAntiLightItemWaveSourcesNew].y = (byte)num41;
											array5[this.numAntiLightItemWaveSourcesNew].z = (byte)num42;
											this.numAntiLightItemWaveSourcesNew++;
										}
										else
										{
											int num43 = (this.cubes[num40, num41, num42].data != 0) ? 1 : this.cubesDrawTypes[(int)this.cubes[num40, num41, num42].type];
											if (num43 == 1 || num43 == 2)
											{
												this.cubes[num40, num41, num42].lightR = 0;
												this.cubes[num40, num41, num42].lightG = 0;
												this.cubes[num40, num41, num42].lightB = 0;
												array5[this.numAntiLightItemWaveSourcesNew].x = (byte)num40;
												array5[this.numAntiLightItemWaveSourcesNew].y = (byte)num41;
												array5[this.numAntiLightItemWaveSourcesNew].z = (byte)num42;
												this.numAntiLightItemWaveSourcesNew++;
											}
										}
									}
								}
							}
						}
						this.numAntiLightItemWaveSources = this.numAntiLightItemWaveSourcesNew;
						WorldHolderScript.Vector3Int[] array6 = array5;
						array5 = array4;
						array4 = array6;
					}
				}
			}
			this.numCubesLightChange = 0;
		}
		int num44 = 0;
		WorldHolderScript.Vector3Int[] array7 = this.lightWaveSources;
		WorldHolderScript.Vector3Int[] array8 = this.tmpLight;
		while (this.numLightWaveSources > 0)
		{
			this.numLightWaveSourcesNew = 0;
			for (int num45 = 0; num45 < this.numLightWaveSources; num45++)
			{
				WorldHolderScript.Vector3Int vector3Int3 = array7[num45];
				byte b7 = (byte)Mathf.Max(0, (int)(this.cubes[(int)vector3Int3.x, (int)vector3Int3.y, (int)vector3Int3.z].sunLight - 16));
				if (b7 != 0)
				{
					for (int num46 = 0; num46 < 6; num46++)
					{
						num44++;
						int num47 = (int)vector3Int3.x + this.lightNeibours[num46, 0];
						int num48 = (int)vector3Int3.y + this.lightNeibours[num46, 1];
						int num49 = (int)vector3Int3.z + this.lightNeibours[num46, 2];
						if (this.IsInWorld(num47, num48, num49))
						{
							if (this.cubes[num47, num48, num49].sunLight < b7)
							{
								int num50 = (this.cubes[num47, num48, num49].data != 0) ? 1 : this.cubesDrawTypes[(int)this.cubes[num47, num48, num49].type];
								if (num50 == 1 || num50 == 2)
								{
									this.blocksToChange[Mathf.FloorToInt((float)num47 / (float)this.blockSizeX), Mathf.FloorToInt((float)num48 / (float)this.blockSizeY), Mathf.FloorToInt((float)num49 / (float)this.blockSizeZ)] = true;
									this.cubes[num47, num48, num49].sunLight = b7;
									array8[this.numLightWaveSourcesNew].x = (byte)num47;
									array8[this.numLightWaveSourcesNew].y = (byte)num48;
									array8[this.numLightWaveSourcesNew].z = (byte)num49;
									this.numLightWaveSourcesNew++;
								}
							}
						}
					}
				}
			}
			this.numLightWaveSources = this.numLightWaveSourcesNew;
			WorldHolderScript.Vector3Int[] array9 = array8;
			array8 = array7;
			array7 = array9;
		}
		WorldHolderScript.Vector3Int[] array10 = this.lightItemWaveSources;
		WorldHolderScript.Vector3Int[] array11 = this.tmpLight;
		int num51 = 0;
		while (this.numLightItemWaveSources > 0)
		{
			this.numLightItemWaveSourcesNew = 0;
			for (int num52 = 0; num52 < this.numLightItemWaveSources; num52++)
			{
				WorldHolderScript.Vector3Int vector3Int4 = array10[num52];
				byte b8 = (byte)Mathf.Max(0, (int)(this.cubes[(int)vector3Int4.x, (int)vector3Int4.y, (int)vector3Int4.z].lightR - 16));
				byte b9 = (byte)Mathf.Max(0, (int)(this.cubes[(int)vector3Int4.x, (int)vector3Int4.y, (int)vector3Int4.z].lightG - 16));
				byte b10 = (byte)Mathf.Max(0, (int)(this.cubes[(int)vector3Int4.x, (int)vector3Int4.y, (int)vector3Int4.z].lightB - 16));
				if ((byte)Mathf.Max(new int[]
				{
					(int)b8,
					(int)b9,
					(int)b10
				}) != 0)
				{
					for (int num53 = 0; num53 < 6; num53++)
					{
						num51++;
						int num54 = (int)vector3Int4.x + this.lightNeibours[num53, 0];
						int num55 = (int)vector3Int4.y + this.lightNeibours[num53, 1];
						int num56 = (int)vector3Int4.z + this.lightNeibours[num53, 2];
						if (this.IsInWorld(num54, num55, num56))
						{
							if (this.cubes[num54, num55, num56].lightR < b8 || this.cubes[num54, num55, num56].lightG < b9 || this.cubes[num54, num55, num56].lightB < b10)
							{
								int num57 = (this.cubes[num54, num55, num56].data != 0) ? 1 : this.cubesDrawTypes[(int)this.cubes[num54, num55, num56].type];
								if (num57 == 1 || num57 == 2)
								{
									this.blocksToChange[Mathf.FloorToInt((float)num54 / (float)this.blockSizeX), Mathf.FloorToInt((float)num55 / (float)this.blockSizeY), Mathf.FloorToInt((float)num56 / (float)this.blockSizeZ)] = true;
									this.cubes[num54, num55, num56].lightR = (byte)Mathf.Max((int)this.cubes[num54, num55, num56].lightR, (int)b8);
									this.cubes[num54, num55, num56].lightG = (byte)Mathf.Max((int)this.cubes[num54, num55, num56].lightG, (int)b9);
									this.cubes[num54, num55, num56].lightB = (byte)Mathf.Max((int)this.cubes[num54, num55, num56].lightB, (int)b10);
									array11[this.numLightItemWaveSourcesNew].x = (byte)num54;
									array11[this.numLightItemWaveSourcesNew].y = (byte)num55;
									array11[this.numLightItemWaveSourcesNew].z = (byte)num56;
									this.numLightItemWaveSourcesNew++;
								}
							}
						}
					}
				}
			}
			this.numLightItemWaveSources = this.numLightItemWaveSourcesNew;
			WorldHolderScript.Vector3Int[] array12 = array11;
			array11 = array10;
			array10 = array12;
		}
	}

	private void PlaceItemLight(int x, int y, int z)
	{
		this.lightItemWaveSources[this.numLightItemWaveSources].x = (byte)x;
		this.lightItemWaveSources[this.numLightItemWaveSources].y = (byte)y;
		this.lightItemWaveSources[this.numLightItemWaveSources].z = (byte)z;
		this.numLightItemWaveSources++;
		WorldHolderScript.Vector3Int[] array = this.lightItemWaveSources;
		WorldHolderScript.Vector3Int[] array2 = this.tmpLight;
		int num = 0;
		while (this.numLightItemWaveSources > 0)
		{
			this.numLightItemWaveSourcesNew = 0;
			for (int i = 0; i < this.numLightItemWaveSources; i++)
			{
				WorldHolderScript.Vector3Int vector3Int = array[i];
				byte b = (byte)Mathf.Max(0, (int)(this.cubes[(int)vector3Int.x, (int)vector3Int.y, (int)vector3Int.z].lightR - 16));
				byte b2 = (byte)Mathf.Max(0, (int)(this.cubes[(int)vector3Int.x, (int)vector3Int.y, (int)vector3Int.z].lightG - 16));
				byte b3 = (byte)Mathf.Max(0, (int)(this.cubes[(int)vector3Int.x, (int)vector3Int.y, (int)vector3Int.z].lightB - 16));
				if ((byte)Mathf.Max(new int[]
				{
					(int)b,
					(int)b2,
					(int)b3
				}) != 0)
				{
					for (int j = 0; j < 6; j++)
					{
						num++;
						int num2 = (int)vector3Int.x + this.lightNeibours[j, 0];
						int num3 = (int)vector3Int.y + this.lightNeibours[j, 1];
						int num4 = (int)vector3Int.z + this.lightNeibours[j, 2];
						if (this.IsInWorld(num2, num3, num4))
						{
							if (this.cubes[num2, num3, num4].lightR < b || this.cubes[num2, num3, num4].lightG < b2 || this.cubes[num2, num3, num4].lightB < b3)
							{
								int num5 = (this.cubes[num2, num3, num4].data != 0) ? 1 : this.cubesDrawTypes[(int)this.cubes[num2, num3, num4].type];
								if (num5 == 1 || num5 == 2)
								{
									this.blocksToChange[Mathf.FloorToInt((float)num2 / (float)this.blockSizeX), Mathf.FloorToInt((float)num3 / (float)this.blockSizeY), Mathf.FloorToInt((float)num4 / (float)this.blockSizeZ)] = true;
									this.cubes[num2, num3, num4].lightR = (byte)Mathf.Max((int)this.cubes[num2, num3, num4].lightR, (int)b);
									this.cubes[num2, num3, num4].lightG = (byte)Mathf.Max((int)this.cubes[num2, num3, num4].lightG, (int)b2);
									this.cubes[num2, num3, num4].lightB = (byte)Mathf.Max((int)this.cubes[num2, num3, num4].lightB, (int)b3);
									array2[this.numLightItemWaveSourcesNew].x = (byte)num2;
									array2[this.numLightItemWaveSourcesNew].y = (byte)num3;
									array2[this.numLightItemWaveSourcesNew].z = (byte)num4;
									this.numLightItemWaveSourcesNew++;
								}
							}
						}
					}
				}
			}
			this.numLightItemWaveSources = this.numLightItemWaveSourcesNew;
			WorldHolderScript.Vector3Int[] array3 = array2;
			array2 = array;
			array = array3;
		}
	}

	private void ReplaceItemLight(int x, int y, int z)
	{
		this.numAntiLightItemWaveSources = 0;
		this.initAntiLightItem = (byte)Mathf.Max(new int[]
		{
			(int)this.cubes[x, y, z].lightR,
			(int)this.cubes[x, y, z].lightG,
			(int)this.cubes[x, y, z].lightB
		});
		this.antiLightItemWaveSources[this.numAntiLightItemWaveSources].x = (byte)x;
		this.antiLightItemWaveSources[this.numAntiLightItemWaveSources].y = (byte)y;
		this.antiLightItemWaveSources[this.numAntiLightItemWaveSources].z = (byte)z;
		this.numAntiLightItemWaveSources++;
		this.cubes[x, y, z].lightR = 0;
		this.cubes[x, y, z].lightG = 0;
		this.cubes[x, y, z].lightB = 0;
		int num = 0;
		WorldHolderScript.Vector3Int[] array = this.antiLightItemWaveSources;
		WorldHolderScript.Vector3Int[] array2 = this.tmpLight;
		while (this.numAntiLightItemWaveSources > 0)
		{
			num++;
			this.numAntiLightItemWaveSourcesNew = 0;
			byte b = (byte)Mathf.Max(0, (int)this.initAntiLightItem - 16 * num);
			if (num > 16)
			{
				break;
			}
			for (int i = 0; i < this.numAntiLightItemWaveSources; i++)
			{
				for (int j = 0; j < 6; j++)
				{
					int num2 = (int)array[i].x + this.lightNeibours[j, 0];
					int num3 = (int)array[i].y + this.lightNeibours[j, 1];
					int num4 = (int)array[i].z + this.lightNeibours[j, 2];
					if (this.IsInWorld(num2, num3, num4))
					{
						if (this.cubes[num2, num3, num4].lightR != 0 || this.cubes[num2, num3, num4].lightG != 0 || this.cubes[num2, num3, num4].lightB != 0)
						{
							if (this.cubes[num2, num3, num4].lightR >= b && this.cubes[num2, num3, num4].lightG >= b && this.cubes[num2, num3, num4].lightB >= b)
							{
								this.lightItemWaveSources[this.numLightItemWaveSources].x = (byte)num2;
								this.lightItemWaveSources[this.numLightItemWaveSources].y = (byte)num3;
								this.lightItemWaveSources[this.numLightItemWaveSources].z = (byte)num4;
								this.numLightItemWaveSources++;
							}
							else if (this.cubes[num2, num3, num4].isLight)
							{
								this.lightItemWaveSources[this.numLightItemWaveSources].x = (byte)num2;
								this.lightItemWaveSources[this.numLightItemWaveSources].y = (byte)num3;
								this.lightItemWaveSources[this.numLightItemWaveSources].z = (byte)num4;
								this.numLightItemWaveSources++;
								array2[this.numAntiLightItemWaveSourcesNew].x = (byte)num2;
								array2[this.numAntiLightItemWaveSourcesNew].y = (byte)num3;
								array2[this.numAntiLightItemWaveSourcesNew].z = (byte)num4;
								this.numAntiLightItemWaveSourcesNew++;
							}
							else
							{
								int num5 = (this.cubes[num2, num3, num4].data != 0) ? 1 : this.cubesDrawTypes[(int)this.cubes[num2, num3, num4].type];
								if (num5 == 1 || num5 == 2)
								{
									this.blocksToChange[Mathf.FloorToInt((float)num2 / (float)this.blockSizeX), Mathf.FloorToInt((float)num3 / (float)this.blockSizeY), Mathf.FloorToInt((float)num4 / (float)this.blockSizeZ)] = true;
									this.cubes[num2, num3, num4].lightR = 0;
									this.cubes[num2, num3, num4].lightG = 0;
									this.cubes[num2, num3, num4].lightB = 0;
									array2[this.numAntiLightItemWaveSourcesNew].x = (byte)num2;
									array2[this.numAntiLightItemWaveSourcesNew].y = (byte)num3;
									array2[this.numAntiLightItemWaveSourcesNew].z = (byte)num4;
									this.numAntiLightItemWaveSourcesNew++;
								}
							}
						}
					}
				}
			}
			this.numAntiLightItemWaveSources = this.numAntiLightItemWaveSourcesNew;
			WorldHolderScript.Vector3Int[] array3 = array2;
			array2 = array;
			array = array3;
		}
		WorldHolderScript.Vector3Int[] array4 = this.lightItemWaveSources;
		WorldHolderScript.Vector3Int[] array5 = this.tmpLight;
		int num6 = 0;
		while (this.numLightItemWaveSources > 0)
		{
			this.numLightItemWaveSourcesNew = 0;
			for (int k = 0; k < this.numLightItemWaveSources; k++)
			{
				WorldHolderScript.Vector3Int vector3Int = array4[k];
				byte b2 = (byte)Mathf.Max(0, (int)(this.cubes[(int)vector3Int.x, (int)vector3Int.y, (int)vector3Int.z].lightR - 16));
				byte b3 = (byte)Mathf.Max(0, (int)(this.cubes[(int)vector3Int.x, (int)vector3Int.y, (int)vector3Int.z].lightG - 16));
				byte b4 = (byte)Mathf.Max(0, (int)(this.cubes[(int)vector3Int.x, (int)vector3Int.y, (int)vector3Int.z].lightB - 16));
				if ((byte)Mathf.Max(new int[]
				{
					(int)b2,
					(int)b3,
					(int)b4
				}) != 0)
				{
					for (int l = 0; l < 6; l++)
					{
						num6++;
						int num7 = (int)array4[k].x + this.lightNeibours[l, 0];
						int num8 = (int)array4[k].y + this.lightNeibours[l, 1];
						int num9 = (int)array4[k].z + this.lightNeibours[l, 2];
						if (this.IsInWorld(num7, num8, num9))
						{
							if (this.cubes[num7, num8, num9].lightR < b2 || this.cubes[num7, num8, num9].lightG < b3 || this.cubes[num7, num8, num9].lightB < b4)
							{
								int num10 = (this.cubes[num7, num8, num9].data != 0) ? 1 : this.cubesDrawTypes[(int)this.cubes[num7, num8, num9].type];
								if (num10 == 1 || num10 == 2)
								{
									this.blocksToChange[Mathf.FloorToInt((float)num7 / (float)this.blockSizeX), Mathf.FloorToInt((float)num8 / (float)this.blockSizeY), Mathf.FloorToInt((float)num9 / (float)this.blockSizeZ)] = true;
									this.cubes[num7, num8, num9].lightR = (byte)Mathf.Max((int)this.cubes[num7, num8, num9].lightR, (int)b2);
									this.cubes[num7, num8, num9].lightG = (byte)Mathf.Max((int)this.cubes[num7, num8, num9].lightG, (int)b3);
									this.cubes[num7, num8, num9].lightB = (byte)Mathf.Max((int)this.cubes[num7, num8, num9].lightB, (int)b4);
									array5[this.numLightItemWaveSourcesNew].x = (byte)num7;
									array5[this.numLightItemWaveSourcesNew].y = (byte)num8;
									array5[this.numLightItemWaveSourcesNew].z = (byte)num9;
									this.numLightItemWaveSourcesNew++;
								}
							}
						}
					}
				}
			}
			this.numLightItemWaveSources = this.numLightItemWaveSourcesNew;
			WorldHolderScript.Vector3Int[] array6 = array5;
			array5 = array4;
			array4 = array6;
		}
	}

	public CubePhys GetCubePhysType(Vector3 pos)
	{
		if (this.cubes == null)
		{
			return CubePhys.air;
		}
		int num = Mathf.RoundToInt(pos.x);
		int num2 = Mathf.RoundToInt(pos.y);
		int num3 = Mathf.RoundToInt(pos.z);
		if (num < 0 || num2 < 0 || num3 < 0 || num >= this.sizeX || num2 >= this.sizeY || num3 >= this.sizeZ)
		{
			return CubePhys.air;
		}
		return this.cubes[num, num2, num3].phys;
	}

	public CubePhys GetCubePhysType(int x, int y, int z)
	{
		if (x < 0 || y < 0 || z < 0 || x >= this.sizeX || y >= this.sizeY || z >= this.sizeZ)
		{
			return CubePhys.air;
		}
		return this.cubes[x, y, z].phys;
	}

	public void RestoreBlockPhys(int bX, int bY, int bZ)
	{
		for (int i = bX * this.blockSizeX; i < (bX + 1) * this.blockSizeX; i++)
		{
			for (int j = bY * this.blockSizeY; j < (bY + 1) * this.blockSizeY; j++)
			{
				for (int k = bZ * this.blockSizeZ; k < (bZ + 1) * this.blockSizeZ; k++)
				{
					if (this.cubes[i, j, k].type < 155)
					{
						this.cubes[i, j, k].phys = this.cubePhys[(int)this.cubes[i, j, k].type];
					}
					else
					{
						this.cubes[i, j, k].phys = CubePhys.air;
					}
				}
			}
		}
	}

	public void RecalculatePhysForAA(int x1, int y1, int z1, int x2, int y2, int z2)
	{
		for (int i = 0; i < this.nBlocksX; i++)
		{
			for (int j = 0; j < this.nBlocksY; j++)
			{
				for (int k = 0; k < this.nBlocksZ; k++)
				{
					this.blocksToChange[i, j, k] = false;
				}
			}
		}
		for (int l = Mathf.Min(x1, x2); l <= Mathf.Max(x1, x2); l++)
		{
			for (int m = Mathf.Min(y1, y2); m <= Mathf.Max(y1, y2); m++)
			{
				for (int n = Mathf.Min(z1, z2); n <= Mathf.Max(z1, z2); n++)
				{
					int num = Mathf.FloorToInt((float)l / (float)this.blockSizeX);
					int num2 = Mathf.FloorToInt((float)m / (float)this.blockSizeY);
					int num3 = Mathf.FloorToInt((float)n / (float)this.blockSizeZ);
					this.blocksToChange[num, num2, num3] = true;
				}
			}
		}
		for (int num4 = 0; num4 < this.nBlocksX; num4++)
		{
			for (int num5 = 0; num5 < this.nBlocksY; num5++)
			{
				for (int num6 = 0; num6 < this.nBlocksZ; num6++)
				{
					if (this.blocksToChange[num4, num5, num6])
					{
						this.RestoreBlockPhys(num4, num5, num6);
					}
				}
			}
		}
		this.RecalculatePhys();
	}

	public void RecalculatePhys()
	{
		for (int i = 0; i < this.gameItems.Count; i++)
		{
			GameItemStruct gameItemStruct = this.gameItems[i];
			this.cubes[gameItemStruct.x, gameItemStruct.y, gameItemStruct.z].phys = gameItemStruct.phys;
		}
		for (int j = 0; j < this.AAgo.Length; j++)
		{
			if (this.AAgo != null)
			{
				if (!(this.AAgo[j] == null))
				{
					ActionAreaScript component = this.AAgo[j].GetComponent<ActionAreaScript>();
					if (component.type == AAType.lift && component.status == 1)
					{
						for (int k = Mathf.Min(component.x1, component.x2); k <= Mathf.Max(component.x1, component.x2); k++)
						{
							for (int l = Mathf.Min(component.y1, component.y2); l <= Mathf.Max(component.y1, component.y2); l++)
							{
								for (int m = Mathf.Min(component.z1, component.z2); m <= Mathf.Max(component.z1, component.z2); m++)
								{
									this.cubes[k, l, m].phys = CubePhys.liftOn;
								}
							}
						}
					}
				}
			}
		}
	}

	public void ChangeCubes(string cubesToChange, bool logChange = true, bool redrawWorld = true)
	{
		if (!this.initialized)
		{
			this.queuedChanges[this.numQueuedChanges, 0] = "ChangeCubes";
			this.queuedChanges[this.numQueuedChanges, 1] = cubesToChange;
			this.numQueuedChanges++;
			return;
		}
		if (Kube.BCS == null)
		{
			Kube.BCS = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControllerScript>();
		}
		for (int i = 0; i < this.nBlocksX; i++)
		{
			for (int j = 0; j < this.nBlocksY; j++)
			{
				for (int k = 0; k < this.nBlocksZ; k++)
				{
					this.blocksToChange[i, j, k] = false;
				}
			}
		}
		int num = 0;
		string code = cubesToChange.Substring(num, 2);
		num += 2;
		int num2 = Kube.OH.DecodeServerCode(code);
		for (int l = 0; l < num2; l++)
		{
			code = cubesToChange.Substring(num, 2);
			num += 2;
			int cubeX = Kube.OH.DecodeServerCode(code);
			code = cubesToChange.Substring(num, 2);
			num += 2;
			int cubeY = Kube.OH.DecodeServerCode(code);
			code = cubesToChange.Substring(num, 2);
			num += 2;
			int cubeZ = Kube.OH.DecodeServerCode(code);
			code = cubesToChange.Substring(num, 2);
			num += 2;
			int typeCube = Kube.OH.DecodeServerCode(code);
			this.ChangeCube(cubeX, cubeY, cubeZ, typeCube, num2, 0);
		}
		if (redrawWorld)
		{
			int num3 = this.nBlocksX - 1;
			int num4 = 0;
			int num5 = this.nBlocksZ - 1;
			int num6 = 0;
			for (int m = 0; m < this.nBlocksX; m++)
			{
				for (int n = 0; n < this.nBlocksY; n++)
				{
					for (int num7 = 0; num7 < this.nBlocksZ; num7++)
					{
						if (this.blocksToChange[m, n, num7])
						{
							if (m < num3)
							{
								num3 = m;
							}
							if (m > num4)
							{
								num4 = m;
							}
							if (num7 < num5)
							{
								num5 = num7;
							}
							if (num7 > num6)
							{
								num6 = num7;
							}
						}
					}
				}
			}
			this.CalculateLight(num3 - 1, num4 + 1, num5 - 1, num6 + 1, false);
			for (int num8 = 0; num8 < this.nBlocksX; num8++)
			{
				for (int num9 = 0; num9 < this.nBlocksY; num9++)
				{
					for (int num10 = 0; num10 < this.nBlocksZ; num10++)
					{
						if (this.blocksToChange[num8, num9, num10])
						{
							bool flag = false;
							for (int num11 = 0; num11 < this.blockSizeX; num11++)
							{
								for (int num12 = 0; num12 < this.blockSizeY; num12++)
								{
									for (int num13 = 0; num13 < this.blockSizeZ; num13++)
									{
										if (this.cubes[num8 * this.blockSizeX + num11, num9 * this.blockSizeY + num12, num10 * this.blockSizeZ + num13].type != 0)
										{
											flag = true;
											break;
										}
									}
								}
							}
							if (flag && this.blocks[num8, num9, num10] == null)
							{
								BlockScript component = (UnityEngine.Object.Instantiate(this.blockPrefab, new Vector3((float)(num8 * this.blockSizeX), (float)(num9 * this.blockSizeY), (float)(num10 * this.blockSizeZ)), Quaternion.identity) as GameObject).GetComponent<BlockScript>();
								component.SetBlock(new Vector3((float)num8 * (float)this.blockSizeX, (float)num9 * (float)this.blockSizeY, (float)num10 * (float)this.blockSizeZ), new Vector3((float)num8 * (float)this.blockSizeX + (float)this.blockSizeX, (float)num9 * (float)this.blockSizeY + (float)this.blockSizeY, (float)num10 * (float)this.blockSizeZ + (float)this.blockSizeZ));
								this.blocks[num8, num9, num10] = component;
								this.blocks[num8, num9, num10].RefreshMeshes();
							}
							else if (flag)
							{
								this.blocks[num8, num9, num10].RefreshMeshes();
							}
							else if (!flag && this.blocks[num8, num9, num10] != null)
							{
								this.blocks[num8, num9, num10].DestroyBlock();
							}
							this.RestoreBlockPhys(num8, num9, num10);
						}
					}
				}
			}
		}
		this.RecalculatePhys();
	}

	public void ChangeOneCube(int cubeX, int cubeY, int cubeZ, int typeCube, int geom = 0)
	{
		if (!Kube.BCS.mapCanBreak && typeCube == 0)
		{
			return;
		}
		for (int i = 0; i < this.nBlocksX; i++)
		{
			for (int j = 0; j < this.nBlocksY; j++)
			{
				for (int k = 0; k < this.nBlocksZ; k++)
				{
					this.blocksToChange[i, j, k] = false;
				}
			}
		}
		this.ChangeCube(cubeX, cubeY, cubeZ, typeCube, 1, geom);
		this.UpdateChangedBlocks();
	}

	private void UpdateChangedBlocks()
	{
		int num = this.nBlocksX - 1;
		int num2 = 0;
		int num3 = this.nBlocksZ - 1;
		int num4 = 0;
		for (int i = 0; i < this.nBlocksX; i++)
		{
			for (int j = 0; j < this.nBlocksY; j++)
			{
				for (int k = 0; k < this.nBlocksZ; k++)
				{
					if (this.blocksToChange[i, j, k])
					{
						if (i < num)
						{
							num = i;
						}
						if (i > num2)
						{
							num2 = i;
						}
						if (k < num3)
						{
							num3 = k;
						}
						if (k > num4)
						{
							num4 = k;
						}
					}
				}
			}
		}
		this.CalculateLight(num - 1, num2 + 1, num3 - 1, num4 + 1, false);
		for (int l = 0; l < this.nBlocksX; l++)
		{
			for (int m = 0; m < this.nBlocksY; m++)
			{
				for (int n = 0; n < this.nBlocksZ; n++)
				{
					if (this.blocksToChange[l, m, n])
					{
						bool flag = false;
						for (int num5 = 0; num5 < this.blockSizeX; num5++)
						{
							for (int num6 = 0; num6 < this.blockSizeY; num6++)
							{
								for (int num7 = 0; num7 < this.blockSizeZ; num7++)
								{
									if (this.cubes[l * this.blockSizeX + num5, m * this.blockSizeY + num6, n * this.blockSizeZ + num7].type != 0)
									{
										flag = true;
										break;
									}
								}
							}
						}
						if (flag && this.blocks[l, m, n] == null)
						{
							BlockScript component = (UnityEngine.Object.Instantiate(this.blockPrefab, new Vector3((float)(l * this.blockSizeX), (float)(m * this.blockSizeY), (float)(n * this.blockSizeZ)), Quaternion.identity) as GameObject).GetComponent<BlockScript>();
							component.SetBlock(new Vector3((float)l * (float)this.blockSizeX, (float)m * (float)this.blockSizeY, (float)n * (float)this.blockSizeZ), new Vector3((float)l * (float)this.blockSizeX + (float)this.blockSizeX, (float)m * (float)this.blockSizeY + (float)this.blockSizeY, (float)n * (float)this.blockSizeZ + (float)this.blockSizeZ));
							this.blocks[l, m, n] = component;
							this.blocks[l, m, n].RefreshMeshes();
						}
						else if (flag)
						{
							this.blocks[l, m, n].RefreshMeshes();
						}
						else if (!flag && this.blocks[l, m, n] != null)
						{
							this.blocks[l, m, n].DestroyBlock();
						}
						this.RestoreBlockPhys(l, m, n);
					}
				}
			}
		}
		this.RecalculatePhys();
	}

	protected void ChangeCube(int cubeX, int cubeY, int cubeZ, int typeCube, int numBlocksToChange, int geom = 0)
	{
		if (!Kube.BCS.mapCanBreak && typeCube == 0)
		{
			return;
		}
		if (cubeX >= this.sizeX || cubeY >= this.sizeY || cubeZ >= this.sizeZ)
		{
			return;
		}
		if (cubeY >= this.sizeY - 1 && typeCube != 0)
		{
			return;
		}
		int num = Mathf.FloorToInt((float)cubeX / (float)this.blockSizeX);
		int num2 = Mathf.FloorToInt((float)cubeY / (float)this.blockSizeY);
		int num3 = Mathf.FloorToInt((float)cubeZ / (float)this.blockSizeZ);
		try
		{
			this.blocksToChange[num, num2, num3] = true;
		}
		catch
		{
			MonoBehaviour.print(string.Concat(new object[]
			{
				"errorCubes ",
				cubeX,
				" ",
				cubeY,
				" ",
				cubeZ,
				" ",
				typeCube,
				" block=",
				num,
				" ",
				num2,
				" ",
				num3
			}));
		}
		if (this.cubes[cubeX, cubeY, cubeZ].type != 0 && typeCube == 0)
		{
			this.VisualizeDestroyCube(cubeX, cubeY, cubeZ, this.cubes[cubeX, cubeY, cubeZ].type, 10f / (float)numBlocksToChange);
		}
		if (typeCube == 128)
		{
			this.cubes[cubeX, cubeY, cubeZ].waterLevel = this.maxWaterLevel;
			this.SetWaterToCheck(cubeX, cubeY, cubeZ, true);
		}
		if (this.cubes[cubeX, cubeY, cubeZ].type == 128 && typeCube == 0)
		{
			this.cubes[cubeX, cubeY, cubeZ].waterLevel = 0;
			if (this.IsInWorld(cubeX - 1, cubeY, cubeZ))
			{
				this.SetWaterToCheck(cubeX - 1, cubeY, cubeZ, true);
			}
			if (this.IsInWorld(cubeX + 1, cubeY, cubeZ))
			{
				this.SetWaterToCheck(cubeX + 1, cubeY, cubeZ, true);
			}
			if (this.IsInWorld(cubeX, cubeY, cubeZ + 1))
			{
				this.SetWaterToCheck(cubeX, cubeY, cubeZ + 1, true);
			}
			if (this.IsInWorld(cubeX, cubeY, cubeZ - 1))
			{
				this.SetWaterToCheck(cubeX, cubeY, cubeZ - 1, true);
			}
			if (this.IsInWorld(cubeX, cubeY - 1, cubeZ - 1) && this.cubes[cubeX, cubeY - 1, cubeZ].type == 128)
			{
				CubeStruct ptr = this.cubes[cubeX, cubeY - 1, cubeZ];
				ptr.waterLevel -= 1;
				this.SetWaterToCheck(cubeX, cubeY - 1, cubeZ, true);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			this.SetWaterToCheck(cubeX + this.lightNeibours[i, 0], cubeY + this.lightNeibours[i, 1], cubeZ + this.lightNeibours[i, 2], true);
		}
		this.cubes[cubeX, cubeY, cubeZ].type = (ushort)typeCube;
		this.cubes[cubeX, cubeY, cubeZ].data = (byte)geom;
		this.cubes[cubeX, cubeY, cubeZ].health = (byte)Mathf.Min(255f, 64f * Kube.OH.cubesStrength[(int)this.cubes[cubeX, cubeY, cubeZ].type]);
		this.cubes[cubeX, cubeY, cubeZ].phys = this.cubePhys[(int)this.cubes[cubeX, cubeY, cubeZ].type];
		this.SetNewCubesLightChange(cubeX, cubeY, cubeZ);
		if (typeCube == 128)
		{
			this.ChangeWorldBytesCube(cubeX, cubeY, cubeZ, this.cubes[cubeX, cubeY, cubeZ].type, this.maxWaterLevel);
		}
		else
		{
			this.ChangeWorldBytesCube(cubeX, cubeY, cubeZ, this.cubes[cubeX, cubeY, cubeZ].type, (byte)geom);
		}
		if (cubeX % this.blockSizeX == 0 && num > 0)
		{
			this.blocksToChange[num - 1, num2, num3] = true;
		}
		if (cubeY % this.blockSizeY == 0 && num2 > 0)
		{
			this.blocksToChange[num, num2 - 1, num3] = true;
		}
		if (cubeZ % this.blockSizeZ == 0 && num3 > 0)
		{
			this.blocksToChange[num, num2, num3 - 1] = true;
		}
		if (cubeX % this.blockSizeX == this.blockSizeX - 1 && num < this.nBlocksX - 1)
		{
			this.blocksToChange[num + 1, num2, num3] = true;
		}
		if (cubeY % this.blockSizeY == this.blockSizeY - 1 && num2 < this.nBlocksY - 1)
		{
			this.blocksToChange[num, num2 + 1, num3] = true;
		}
		if (cubeZ % this.blockSizeZ == this.blockSizeZ - 1 && num3 < this.nBlocksZ - 1)
		{
			this.blocksToChange[num, num2, num3 + 1] = true;
		}
	}

	public void ChangeCubesHealth(string cubesToChange)
	{
		if (!this.initialized)
		{
			this.queuedChanges[this.numQueuedChanges, 0] = "ChangeCubesHealth";
			this.queuedChanges[this.numQueuedChanges, 1] = cubesToChange;
			this.numQueuedChanges++;
			return;
		}
		if (Kube.BCS == null)
		{
			Kube.BCS = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControllerScript>();
		}
		if (!Kube.BCS.mapCanBreak || !Kube.BCS.canUseWeapons)
		{
			return;
		}
		for (int i = 0; i < this.nBlocksX; i++)
		{
			for (int j = 0; j < this.nBlocksY; j++)
			{
				for (int k = 0; k < this.nBlocksZ; k++)
				{
					this.blocksToChange[i, j, k] = false;
				}
			}
		}
		int num = 0;
		string code = cubesToChange.Substring(num, 2);
		num += 2;
		int num2 = Kube.OH.DecodeServerCode(code);
		string text = string.Empty;
		int num3 = 0;
		for (int l = 0; l < num2; l++)
		{
			code = cubesToChange.Substring(num, 2);
			num += 2;
			int num4 = Kube.OH.DecodeServerCode(code);
			code = cubesToChange.Substring(num, 2);
			num += 2;
			int num5 = Kube.OH.DecodeServerCode(code);
			code = cubesToChange.Substring(num, 2);
			num += 2;
			int num6 = Kube.OH.DecodeServerCode(code);
			code = cubesToChange.Substring(num, 2);
			num += 2;
			int num7 = Kube.OH.DecodeServerCode(code);
			if (this.cubes[num4, num5, num6].type <= 155)
			{
				this.cubes[num4, num5, num6].health = (byte)num7;
				if (this.cubes[num4, num5, num6].health <= 0)
				{
					num3++;
					string text2 = text;
					text = string.Concat(new string[]
					{
						text2,
						Kube.OH.GetServerCode(num4, 2),
						Kube.OH.GetServerCode(num5, 2),
						Kube.OH.GetServerCode(num6, 2),
						Kube.OH.GetServerCode(0, 2)
					});
					if (l < 10)
					{
						this.PlayCubeHit(new Vector3((float)num4, (float)num5, (float)num6), SoundHitType.breaking);
					}
					this.ChangeCube(num4, num5, num6, 0, num2, 0);
				}
			}
		}
		text = Kube.OH.GetServerCode(num3, 2) + text;
		this.UpdateChangedBlocks();
	}

	private void VisualizeDestroyCube(int cubeX, int cubeY, int cubeZ, ushort type, float strength)
	{
		if (type < 0 || type >= 155)
		{
			return;
		}
		if (strength > 1f)
		{
			strength = 1f;
		}
		if (strength < 0f)
		{
			strength = 0f;
		}
		int num = (int)Mathf.Max(1f, 5f * strength);
		for (int i = 0; i < num; i++)
		{
			Vector3 position = new Vector3(UnityEngine.Random.Range((float)cubeX - 0.4f, (float)cubeX + 0.4f), UnityEngine.Random.Range((float)cubeY - 0.4f, (float)cubeY + 0.4f), UnityEngine.Random.Range((float)cubeZ - 0.4f, (float)cubeZ + 0.4f));
			Quaternion rotation = UnityEngine.Random.rotation;
			GameObject gameObject = UnityEngine.Object.Instantiate(Kube.OH.miniCube, position, rotation) as GameObject;
			gameObject.renderer.sharedMaterial = Kube.ASS2.miniCubesMat[(int)type];
		}
	}

	public void CreateGameItem(int numItem, byte rotation, int x, int y, int z, int state, int id, bool redraw = true)
	{
		GameItemStruct item = default(GameItemStruct);
		Quaternion rotation2 = Quaternion.identity;
		if (rotation == 0)
		{
			rotation2 = Quaternion.LookRotation(Vector3.forward);
		}
		else if (rotation == 3)
		{
			rotation2 = Quaternion.LookRotation(-Vector3.forward);
		}
		else if (rotation == 1)
		{
			rotation2 = Quaternion.LookRotation(Vector3.right);
		}
		else if (rotation == 2)
		{
			rotation2 = Quaternion.LookRotation(-Vector3.right);
		}
		else if (rotation == 4)
		{
			rotation2 = Quaternion.LookRotation(Vector3.up);
		}
		else if (rotation == 5)
		{
			rotation2 = Quaternion.LookRotation(-Vector3.up);
		}
		GameObject gameObject = Kube.IS.gameItemsGO[numItem];
		if (gameObject == null)
		{
			return;
		}
		item.gameObject = (UnityEngine.Object.Instantiate(gameObject, new Vector3((float)x, (float)y, (float)z), rotation2) as GameObject);
		item.rotation = rotation;
		item.x = x;
		item.y = y;
		item.z = z;
		item.numItem = (byte)numItem;
		this.cubes[item.x, item.y, item.z].type = (ushort)(numItem + 155);
		item.phys = item.gameObject.GetComponent<ItemPropsScript>().physType;
		item.id = id;
		item.gameObject.GetComponent<ItemPropsScript>().id = id;
		item.gameObject.GetComponent<ItemPropsScript>().type = numItem;
		item.lightColor = item.gameObject.GetComponent<ItemPropsScript>().lightColor;
		this.gameItems.Add(item);
		item.gameObject.SendMessage("ChangeItemState", state, SendMessageOptions.DontRequireReceiver);
		this.ChangeWorldBytesItem(x, y, z, (byte)numItem, (byte)((int)rotation + state * 6));
		int bX = Mathf.FloorToInt((float)x / (float)this.blockSizeX);
		int bY = Mathf.FloorToInt((float)y / (float)this.blockSizeY);
		int bZ = Mathf.FloorToInt((float)z / (float)this.blockSizeZ);
		this.RestoreBlockPhys(bX, bY, bZ);
		this.RecalculatePhys();
		if (item.lightColor.grayscale > 0f)
		{
			this.cubes[x, y, z].isLight = true;
		}
		if (item.lightColor.grayscale > 0f && redraw)
		{
			this.cubes[x, y, z].lightR = (byte)(255f * item.lightColor.r);
			this.cubes[x, y, z].lightG = (byte)(255f * item.lightColor.g);
			this.cubes[x, y, z].lightB = (byte)(255f * item.lightColor.b);
			this.PlaceItemLight(x, y, z);
			this.RedrawWorld(false, true, false);
		}
	}

	public void RemoveGameItem(int id)
	{
		for (int i = 0; i < this.gameItems.Count; i++)
		{
			GameItemStruct gameItemStruct = this.gameItems[i];
			if (gameItemStruct.id == id)
			{
				int bX = Mathf.FloorToInt((float)gameItemStruct.x / (float)this.blockSizeX);
				int bY = Mathf.FloorToInt((float)gameItemStruct.y / (float)this.blockSizeY);
				int bZ = Mathf.FloorToInt((float)gameItemStruct.z / (float)this.blockSizeZ);
				this.DeleteTrigger(gameItemStruct.x, gameItemStruct.y, gameItemStruct.z);
				this.DeleteMonsterRespawn(gameItemStruct.x, gameItemStruct.y, gameItemStruct.z);
				this.DeleteTransportRespawn(gameItemStruct.x, gameItemStruct.y, gameItemStruct.z);
				UnityEngine.Object.Destroy(gameItemStruct.gameObject);
				this.gameItems.RemoveAt(i);
				this.RestoreBlockPhys(bX, bY, bZ);
				this.RecalculatePhys();
				this.ChangeWorldBytesCube(gameItemStruct.x, gameItemStruct.y, gameItemStruct.z, 0, 0);
				this.cubes[gameItemStruct.x, gameItemStruct.y, gameItemStruct.z].type = 0;
				if (gameItemStruct.lightColor.grayscale > 0f)
				{
					this.cubes[gameItemStruct.x, gameItemStruct.y, gameItemStruct.z].isLight = false;
					this.ReplaceItemLight(gameItemStruct.x, gameItemStruct.y, gameItemStruct.z);
					this.RedrawWorld(false, true, false);
				}
				break;
			}
		}
	}

	public GameMapItem _CreateNewMagic(int numItem)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Kube.IS.gameItemsGO[numItem], Vector3.zero, Quaternion.identity) as GameObject;
		this.CreateMagic(gameObject, numItem);
		return gameObject.GetComponent<GameMapItem>();
	}

	public void CreateMagic(GameObject magic, int numItem)
	{
		if (!magic.GetComponent<GameMapItem>())
		{
			return;
		}
		MagicItemStruct item = default(MagicItemStruct);
		item.gameObject = magic;
		item.id = 0;
		item.numItem = numItem;
		this.magicItems.Add(item);
	}

	public void RemoveMagic(GameObject magic)
	{
		for (int i = 0; i < this.magicItems.Count; i++)
		{
			if (this.magicItems[i].gameObject == magic)
			{
				this.magicItems.RemoveAt(i);
			}
		}
	}

	public void RemoveAllMagic()
	{
		this.magicItems.Clear();
	}

	public void MoveItem(int id, Vector3 newPos)
	{
		if ((int)newPos.x < 0 || (int)newPos.x >= this.sizeX || (int)newPos.y < 0 || (int)newPos.y >= this.sizeY || (int)newPos.z < 0 || (int)newPos.z >= this.sizeZ)
		{
			return;
		}
		for (int i = 0; i < this.gameItems.Count; i++)
		{
			GameItemStruct value = this.gameItems[i];
			if (value.id == id)
			{
				value.gameObject.SendMessage("ClearWorldProps", SendMessageOptions.DontRequireReceiver);
				int bX = Mathf.FloorToInt((float)value.x / (float)this.blockSizeX);
				int bY = Mathf.FloorToInt((float)value.y / (float)this.blockSizeY);
				int bZ = Mathf.FloorToInt((float)value.z / (float)this.blockSizeZ);
				value.gameObject.transform.position = newPos;
				this.MoveTrigger(value.x, value.y, value.z, Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), Mathf.RoundToInt(newPos.z));
				this.MoveMonsterRespawn(value.x, value.y, value.z, Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), Mathf.RoundToInt(newPos.z));
				this.RecalculatePhys();
				this.ChangeWorldBytesCube(value.x, value.y, value.z, 0, 0);
				this.cubes[value.x, value.y, value.z].type = 0;
				if (value.lightColor.grayscale > 0f)
				{
					this.cubes[value.x, value.y, value.z].isLight = false;
					this.ReplaceItemLight(value.x, value.y, value.z);
					this.RedrawWorld(false, true, false);
				}
				this.ChangeWorldBytesItem(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), Mathf.RoundToInt(newPos.z), value.numItem, value.rotation + value.state * 6);
				value.x = Mathf.RoundToInt(newPos.x);
				value.y = Mathf.RoundToInt(newPos.y);
				value.z = Mathf.RoundToInt(newPos.z);
				this.cubes[value.x, value.y, value.z].type = (ushort)(value.numItem + 155);
				value.gameObject.GetComponent<ItemPropsScript>().id = value.x + value.z * 256 + value.y * 256 * 256;
				value.id = value.x + value.z * 256 + value.y * 256 * 256;
				bX = Mathf.FloorToInt((float)Mathf.RoundToInt(newPos.x) / (float)this.blockSizeX);
				bY = Mathf.FloorToInt((float)Mathf.RoundToInt(newPos.y) / (float)this.blockSizeY);
				bZ = Mathf.FloorToInt((float)Mathf.RoundToInt(newPos.z) / (float)this.blockSizeZ);
				value.gameObject.SendMessage("ChangeItemState", (int)value.state, SendMessageOptions.DontRequireReceiver);
				this.RestoreBlockPhys(bX, bY, bZ);
				this.RecalculatePhys();
				if (value.lightColor.grayscale > 0f)
				{
					this.cubes[Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), Mathf.RoundToInt(newPos.z)].isLight = true;
				}
				if (value.lightColor.grayscale > 0f)
				{
					this.cubes[Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), Mathf.RoundToInt(newPos.z)].lightR = (byte)(255f * value.lightColor.r);
					this.cubes[Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), Mathf.RoundToInt(newPos.z)].lightG = (byte)(255f * value.lightColor.g);
					this.cubes[Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), Mathf.RoundToInt(newPos.z)].lightB = (byte)(255f * value.lightColor.b);
					this.PlaceItemLight(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), Mathf.RoundToInt(newPos.z));
					this.RedrawWorld(false, true, false);
				}
				MonoBehaviour.print(string.Concat(new object[]
				{
					"Move item, newCoords = ",
					value.x,
					" ",
					value.y,
					" ",
					value.z
				}));
				this.gameItems[i] = value;
				break;
			}
		}
	}

	public void RotateGameItem(int id)
	{
		for (int i = 0; i < this.gameItems.Count; i++)
		{
			GameItemStruct value = this.gameItems[i];
			if (value.id == id)
			{
				value.gameObject.transform.RotateAround(Vector3.up, 1.57079637f);
				if (value.rotation == 0)
				{
					value.rotation = 1;
				}
				else if (value.rotation == 1)
				{
					value.rotation = 3;
				}
				else if (value.rotation == 3)
				{
					value.rotation = 2;
				}
				else if (value.rotation == 2)
				{
					value.rotation = 0;
				}
				this.gameItems[i] = value;
				this.ChangeWorldBytesItem(value.x, value.y, value.z, value.numItem, value.rotation + value.state * 6);
				break;
			}
		}
	}

	private void RemoveAllGameItems()
	{
		for (int i = 0; i < this.gameItems.Count; i++)
		{
			UnityEngine.Object.Destroy(this.gameItems[i].gameObject);
		}
		this.gameItems.Clear();
	}

	public void ChangeItemState(int id, int newState)
	{
		if (this.gameItems != null)
		{
			for (int i = 0; i < this.gameItems.Count; i++)
			{
				GameItemStruct gameItemStruct = this.gameItems[i];
				if (gameItemStruct.id == id)
				{
					gameItemStruct.gameObject.BroadcastMessage("ChangeItemState", newState, SendMessageOptions.RequireReceiver);
					gameItemStruct.state = (byte)newState;
					this.ChangeWorldBytesItem(gameItemStruct.x, gameItemStruct.y, gameItemStruct.z, gameItemStruct.numItem, gameItemStruct.rotation + gameItemStruct.state * 6);
					break;
				}
			}
		}
	}

	private byte[] EncodeToZLIB(Color32[] worldRGBA)
	{
		byte[] array = new byte[worldRGBA.Length * 4];
		int num = 0;
		for (int i = 0; i < worldRGBA.Length; i++)
		{
			array[num++] = worldRGBA[i].r;
			array[num++] = worldRGBA[i].g;
			array[num++] = worldRGBA[i].b;
			array[num++] = worldRGBA[i].a;
		}
		return ZlibStream.CompressBuffer(array);
	}

	private Color32[] DecodeFromZLIB(byte[] worldBytes)
	{
		worldBytes = ZlibStream.UncompressBuffer(worldBytes);
		Color32[] array = new Color32[worldBytes.Length / 4];
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].r = worldBytes[num++];
			array[i].g = worldBytes[num++];
			array[i].b = worldBytes[num++];
			array[i].a = worldBytes[num++];
		}
		return array;
	}

	public byte[] SaveWorld()
	{
		int tickCount = Environment.TickCount;
		this.needSaveMap = false;
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(2561);
		memoryStream.WriteByte((byte)this.sizeX);
		memoryStream.WriteByte((byte)this.sizeY);
		memoryStream.WriteByte((byte)this.sizeZ);
		this._cubegrid.save(memoryStream);
		List<GameItemStruct> list = new List<GameItemStruct>();
		for (int i = 0; i < this.gameItems.Count; i++)
		{
			GameMapItem component = this.gameItems[i].gameObject.GetComponent<GameMapItem>();
			if (!(component == null))
			{
				list.Add(this.gameItems[i]);
			}
		}
		binaryWriter.Write(list.Count);
		for (int j = 0; j < list.Count; j++)
		{
			GameMapItem component2 = list[j].gameObject.GetComponent<GameMapItem>();
			binaryWriter.Write(list[j].id);
			KubeStream kubeStream = new KubeStream(null);
			component2.SaveMap(kubeStream);
			binaryWriter.Write((byte)kubeStream.Length);
			memoryStream.Write(kubeStream.data, 0, kubeStream.Length);
		}
		binaryWriter.Write(this.magicItems.Count);
		for (int k = 0; k < this.magicItems.Count; k++)
		{
			GameMapItem component3 = this.magicItems[k].gameObject.GetComponent<GameMapItem>();
			binaryWriter.Write((ushort)this.magicItems[k].numItem);
			KubeStream kubeStream2 = new KubeStream(null);
			component3.SaveMap(kubeStream2);
			binaryWriter.Write((byte)kubeStream2.Length);
			memoryStream.Write(kubeStream2.data, 0, kubeStream2.Length);
		}
		byte[] array = memoryStream.ToArray();
		array = ZlibStream.CompressBuffer(array);
		int tickCount2 = Environment.TickCount;
		MonoBehaviour.print(string.Concat(new object[]
		{
			"SaveWorld: worldSize=",
			array.Length,
			" time: ",
			(tickCount2 - tickCount).ToString()
		}));
		return array;
	}

	public int LoadWorld(byte[] newWorldData)
	{
		MonoBehaviour.print("LoadWorld length: " + newWorldData.Length);
		if (newWorldData.Length <= 2)
		{
			this._GenerateWorld();
			return 0;
		}
		if (newWorldData[0] == 137 && newWorldData[1] == 80)
		{
			return this.LoadWorldOld(newWorldData);
		}
		newWorldData = ZlibStream.UncompressBuffer(newWorldData);
		MemoryStream memoryStream = new MemoryStream(newWorldData);
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		memoryStream.Position = 0L;
		int num = (int)binaryReader.ReadUInt16();
		if (num != 2561)
		{
			return this.LoadWorldOld(newWorldData);
		}
		this.sizeX = memoryStream.ReadByte();
		this.sizeY = memoryStream.ReadByte();
		this.sizeZ = memoryStream.ReadByte();
		this.Init(this.sizeX, this.sizeY, this.sizeZ, true);
		this._cubegrid.load(memoryStream);
		for (int i = 0; i < this.sizeX; i++)
		{
			for (int j = 0; j < this.sizeY; j++)
			{
				for (int k = 0; k < this.sizeZ; k++)
				{
					int num2 = this._cubegrid.get(i, j, k);
					byte b = this._cubegrid.getdata(i, j, k);
					this.cubes[i, j, k].type = (ushort)num2;
					this.cubes[i, j, k].data = b;
					if (num2 > 255)
					{
						UnityEngine.Debug.Log("Big type - " + num2);
					}
					if (num2 < 155)
					{
						this.cubes[i, j, k].health = (byte)Mathf.Min(255f, 64f * Kube.OH.cubesStrength[(int)this.cubes[i, j, k].type]);
						this.cubes[i, j, k].phys = this.cubePhys[(int)this.cubes[i, j, k].type];
						if (this.cubes[i, j, k].type == 128)
						{
							this.cubes[i, j, k].waterLevel = b;
						}
					}
					else
					{
						byte b2 = b % 6;
						byte state = (b - b2) / 6;
						this.CreateGameItem(num2 - 155, b2, i, j, k, (int)state, i + k * 256 + j * 256 * 256, false);
					}
				}
			}
		}
		int num3 = binaryReader.ReadInt32();
		for (int l = 0; l < num3; l++)
		{
			int num4 = binaryReader.ReadInt32();
			int count = (int)binaryReader.ReadByte();
			KubeStream br = new KubeStream(binaryReader.ReadBytes(count));
			for (int m = 0; m < this.gameItems.Count; m++)
			{
				if (this.gameItems[m].id == num4)
				{
					GameMapItem component = this.gameItems[m].gameObject.GetComponent<GameMapItem>();
					component.LoadMap(br);
					break;
				}
			}
		}
		num3 = binaryReader.ReadInt32();
		for (int n = 0; n < num3; n++)
		{
			int numItem = (int)binaryReader.ReadUInt16();
			GameMapItem gameMapItem = this._CreateNewMagic(numItem);
			int count2 = (int)binaryReader.ReadByte();
			KubeStream br2 = new KubeStream(binaryReader.ReadBytes(count2));
			gameMapItem.LoadMap(br2);
		}
		for (int num5 = 0; num5 < this.monsterRespawnS.Length; num5++)
		{
			this.monsterLastDieTime[num5] = -999999f;
		}
		for (int num6 = 0; num6 < this.transportRespawnS.Length; num6++)
		{
			this.transportLastDieTime[num6] = -999999f;
		}
		this.GenerateBounds();
		this.needSaveMap = false;
		if (this.numQueuedChanges != 0)
		{
			for (int num7 = 0; num7 < this.numQueuedChanges; num7++)
			{
				if (this.queuedChanges[num7, 0] == "ChangeCubes")
				{
					this.ChangeCubes(this.queuedChanges[num7, 1], false, false);
				}
				else if (this.queuedChanges[num7, 0] == "ChangeCubesHealth")
				{
					this.ChangeCubesHealth(this.queuedChanges[num7, 1]);
				}
			}
		}
		return 0;
	}

	private Color32[] DecodeFromBytes(byte[] worldBytes)
	{
		Color32[] array = new Color32[worldBytes.Length / 4];
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].r = worldBytes[num++];
			array[i].g = worldBytes[num++];
			array[i].b = worldBytes[num++];
			array[i].a = worldBytes[num++];
		}
		return array;
	}

	public int LoadWorldOld(byte[] newWorldData)
	{
		Kube.GPS.needTrainingBuild = false;
		Color32[] array;
		if (newWorldData[0] != 137 || newWorldData[1] != 80)
		{
			array = this.DecodeFromBytes(newWorldData);
			this.containerSize = array.Length;
			if (array.Length == 1048576)
			{
				MonoBehaviour.print("small map");
				this.Init(128, 96, 128, false);
			}
			else
			{
				if (array.Length != 4194304)
				{
					MonoBehaviour.print("Bad map size");
					return 0;
				}
				MonoBehaviour.print("big map");
				this.Init(224, 125, 224, false);
			}
		}
		else
		{
			Texture2D texture2D = new Texture2D(4, 4);
			texture2D.LoadImage(newWorldData);
			this.containerSize = texture2D.width * texture2D.height;
			if (texture2D.width == 1024 && texture2D.height == 1024)
			{
				MonoBehaviour.print("small map");
				this.Init(128, 96, 128, false);
			}
			else
			{
				if (texture2D.width != 2048 || texture2D.height != 2048)
				{
					MonoBehaviour.print("Bad map size");
					return 0;
				}
				MonoBehaviour.print("big map");
				this.Init(224, 125, 224, false);
			}
			array = texture2D.GetPixels32();
			UnityEngine.Object.Destroy(texture2D);
		}
		this.RemoveAllGameItems();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < this.sizeX; i++)
		{
			for (int j = 0; j < this.sizeY; j++)
			{
				for (int k = 0; k < this.sizeZ; k++)
				{
					int num3 = Mathf.FloorToInt((float)(i + k * this.sizeX + j * this.sizeX * this.sizeZ) / 4f);
					int num4 = (i + k * this.sizeX + j * this.sizeX * this.sizeZ) % 4;
					byte b = 0;
					byte b2 = 0;
					if (num4 == 0)
					{
						b = array[num3].r;
						b2 = array[num3 + this.containerSize / 2].r;
					}
					else if (num4 == 1)
					{
						b = array[num3].g;
						b2 = array[num3 + this.containerSize / 2].g;
					}
					else if (num4 == 2)
					{
						b = array[num3].b;
						b2 = array[num3 + this.containerSize / 2].b;
					}
					else if (num4 == 3)
					{
						b = array[num3].a;
						b2 = array[num3 + this.containerSize / 2].a;
					}
					this.cubes[i, j, k].type = (ushort)b;
					this.cubes[i, j, k].type = (ushort)b;
					if (b2 != 0 && b < 155 && b != 128)
					{
						b2 = 0;
					}
					this._cubegrid.set(i, j, k, (int)b, (int)b2);
					this.cubes[i, j, k].data = b2;
					if (b < 155)
					{
						this.cubes[i, j, k].health = (byte)Mathf.Min(255f, 64f * Kube.OH.cubesStrength[(int)this.cubes[i, j, k].type]);
						this.cubes[i, j, k].phys = this.cubePhys[(int)this.cubes[i, j, k].type];
						if (this.cubes[i, j, k].type == 128)
						{
							this.cubes[i, j, k].waterLevel = b2;
						}
						if (b != 0)
						{
							num++;
						}
					}
					else
					{
						num2++;
						byte b3 = b2 % 6;
						byte state = (b2 - b3) / 6;
						this.CreateGameItem((int)(b - 155), b2 % 6, i, j, k, (int)state, i + k * 256 + j * 256 * 256, false);
					}
				}
			}
		}
		MonoBehaviour.print("NumGameItems: " + num2);
		int num5 = 0;
		for (int l = 0; l < this.AAgo.Length; l++)
		{
			byte[] array2 = new byte[30];
			for (int m = 0; m < 30; m++)
			{
				int num6 = Mathf.FloorToInt((float)(12 * this.containerSize / 8 + l * 30 + m) / 4f);
				int num7 = (12 * this.containerSize / 8 + l * 30 + m) % 4;
				if (num7 == 0)
				{
					array2[m] = array[num6].r;
				}
				else if (num7 == 1)
				{
					array2[m] = array[num6].g;
				}
				else if (num7 == 2)
				{
					array2[m] = array[num6].b;
				}
				else if (num7 == 3)
				{
					array2[m] = array[num6].a;
				}
			}
			if (array2[0] != 0 || array2[1] != 0 || array2[2] != 0 || array2[3] != 0 || array2[4] != 0 || array2[5] != 0)
			{
				this.AAgo[l] = null;
				num5++;
				this.CreateNewAA((int)array2[0], (int)array2[1], (int)array2[2], (int)array2[3], (int)array2[4], (int)array2[5], (int)array2[6], (int)array2[7], (int)array2[8], (int)array2[9], (int)array2[10], (int)array2[11], (int)array2[12], (int)array2[13], l);
			}
		}
		int num8 = 0;
		for (int n = 0; n < this.triggerS.Length; n++)
		{
			byte[] array3 = new byte[15];
			this.triggerS[n] = null;
			for (int num9 = 0; num9 < array3.Length; num9++)
			{
				int num10 = Mathf.FloorToInt((float)(12 * this.containerSize / 8 + n * 15 + num9 + 30720) / 4f);
				int num11 = (12 * this.containerSize / 8 + n * 15 + num9 + 30720) % 4;
				if (num11 == 0)
				{
					array3[num9] = array[num10].r;
				}
				else if (num11 == 1)
				{
					array3[num9] = array[num10].g;
				}
				else if (num11 == 2)
				{
					array3[num9] = array[num10].b;
				}
				else if (num11 == 3)
				{
					array3[num9] = array[num10].a;
				}
			}
			if (array3[0] != 0 || array3[1] != 0 || array3[2] != 0)
			{
				for (int num12 = 0; num12 < this.gameItems.Count; num12++)
				{
					GameItemStruct gameItemStruct = this.gameItems[num12];
					if (gameItemStruct.x == (int)array3[0] && gameItemStruct.y == (int)array3[1] && gameItemStruct.z == (int)array3[2])
					{
						this.triggerS[n] = gameItemStruct.gameObject.GetComponent<TriggerScript>();
						break;
					}
				}
				if (this.triggerS[n])
				{
					this.SaveTrigger((int)array3[0], (int)array3[1], (int)array3[2], (int)array3[3], (int)array3[4], (int)array3[5], (int)array3[6], (int)array3[7], n);
					num8++;
				}
			}
		}
		int num13 = 0;
		for (int num14 = 0; num14 < this.wireS.Length; num14++)
		{
			byte[] array4 = new byte[10];
			for (int num15 = 0; num15 < 10; num15++)
			{
				int num16 = Mathf.FloorToInt((float)(12 * this.containerSize / 8 + 30720 + 15360 + num14 * 10 + num15) / 4f);
				int num17 = (12 * this.containerSize / 8 + 30720 + 15360 + num14 * 10 + num15) % 4;
				if (num17 == 0)
				{
					array4[num15] = array[num16].r;
				}
				else if (num17 == 1)
				{
					array4[num15] = array[num16].g;
				}
				else if (num17 == 2)
				{
					array4[num15] = array[num16].b;
				}
				else if (num17 == 3)
				{
					array4[num15] = array[num16].a;
				}
			}
			if (array4[3] != 0)
			{
				this.wireS[num14] = null;
				num13++;
				this.CreateNewWire((int)array4[0], (int)array4[1], (int)array4[2], (int)(array4[3] - 1), (int)array4[4], (int)array4[5], (int)array4[6], num14);
			}
		}
		int num18 = 0;
		for (int num19 = 0; num19 < this.monsterRespawnS.Length; num19++)
		{
			byte[] array5 = new byte[10];
			this.monsterRespawnS[num19] = null;
			for (int num20 = 0; num20 < array5.Length; num20++)
			{
				int num21 = Mathf.FloorToInt((float)(12 * this.containerSize / 8 + num19 * 10 + num20 + 30720 + 15360 + 20480) / 4f);
				int num22 = (12 * this.containerSize / 8 + num19 * 10 + num20 + 30720 + 15360 + 20480) % 4;
				if (num22 == 0)
				{
					array5[num20] = array[num21].r;
				}
				else if (num22 == 1)
				{
					array5[num20] = array[num21].g;
				}
				else if (num22 == 2)
				{
					array5[num20] = array[num21].b;
				}
				else if (num22 == 3)
				{
					array5[num20] = array[num21].a;
				}
			}
			if (array5[0] != 0 || array5[1] != 0 || array5[2] != 0)
			{
				for (int num23 = 0; num23 < this.gameItems.Count; num23++)
				{
					GameItemStruct gameItemStruct2 = this.gameItems[num23];
					if (!(gameItemStruct2.gameObject.GetComponent<MonsterRespawnScript>() == null))
					{
						bool flag = true;
						for (int num24 = 0; num24 < this.monsterRespawnS.Length; num24++)
						{
							if (this.monsterRespawnS[num19])
							{
								if (this.monsterRespawnS[num19].x == (int)array5[0] && this.monsterRespawnS[num19].y == (int)array5[1] && this.monsterRespawnS[num19].z == (int)array5[2])
								{
									flag = false;
									break;
								}
							}
						}
						if (flag && gameItemStruct2.x == (int)array5[0] && gameItemStruct2.y == (int)array5[1] && gameItemStruct2.z == (int)array5[2])
						{
							this.monsterRespawnS[num19] = gameItemStruct2.gameObject.GetComponent<MonsterRespawnScript>();
							break;
						}
					}
				}
				if (this.monsterRespawnS[num19])
				{
					this.SaveMonsterRespawn((int)array5[0], (int)array5[1], (int)array5[2], (int)array5[3], (int)array5[4], (int)array5[5], (int)array5[6], (int)array5[7], num19);
					num18++;
				}
			}
		}
		int num25 = 0;
		for (int num26 = 0; num26 < this.transportRespawnS.Length; num26++)
		{
			byte[] array6 = new byte[10];
			this.transportRespawnS[num26] = null;
			for (int num27 = 0; num27 < array6.Length; num27++)
			{
				int num28 = Mathf.FloorToInt((float)(12 * this.containerSize / 8 + num26 * 10 + num27 + 30720 + 15360 + 20480 + 10240) / 4f);
				int num29 = (12 * this.containerSize / 8 + num26 * 10 + num27 + 30720 + 15360 + 20480 + 10240) % 4;
				if (num29 == 0)
				{
					array6[num27] = array[num28].r;
				}
				else if (num29 == 1)
				{
					array6[num27] = array[num28].g;
				}
				else if (num29 == 2)
				{
					array6[num27] = array[num28].b;
				}
				else if (num29 == 3)
				{
					array6[num27] = array[num28].a;
				}
			}
			if (array6[0] != 0 || array6[1] != 0 || array6[2] != 0)
			{
				for (int num30 = 0; num30 < this.gameItems.Count; num30++)
				{
					GameItemStruct gameItemStruct3 = this.gameItems[num30];
					bool flag2 = true;
					for (int num31 = 0; num31 < this.transportRespawnS.Length; num31++)
					{
						if (this.transportRespawnS[num26])
						{
							if (this.transportRespawnS[num26].x == (int)array6[0] && this.transportRespawnS[num26].y == (int)array6[1] && this.transportRespawnS[num26].z == (int)array6[2])
							{
								flag2 = false;
								break;
							}
						}
					}
					if (flag2 && gameItemStruct3.x == (int)array6[0] && gameItemStruct3.y == (int)array6[1] && gameItemStruct3.z == (int)array6[2])
					{
						this.transportRespawnS[num26] = gameItemStruct3.gameObject.GetComponent<TransportRespawnScript>();
						break;
					}
				}
				if (this.transportRespawnS[num26])
				{
					this.SaveTransportRespawn((int)array6[0], (int)array6[1], (int)array6[2], (int)array6[3], (int)array6[4], (int)array6[5], (int)array6[6], (int)array6[7], num26);
					num25++;
				}
				else
				{
					this.SaveTransportRespawn(0, 0, 0, 0, 0, 0, 0, 0, num26);
				}
			}
		}
		for (int num32 = 0; num32 < this.monsterRespawnS.Length; num32++)
		{
			this.monsterLastDieTime[num32] = -999999f;
		}
		for (int num33 = 0; num33 < this.transportRespawnS.Length; num33++)
		{
			this.transportLastDieTime[num33] = -999999f;
		}
		this.GenerateBounds();
		if (num < 50)
		{
			return 1;
		}
		this.needSaveMap = false;
		if (this.numQueuedChanges != 0)
		{
			for (int num34 = 0; num34 < this.numQueuedChanges; num34++)
			{
				if (this.queuedChanges[num34, 0] == "ChangeCubes")
				{
					this.ChangeCubes(this.queuedChanges[num34, 1], false, false);
				}
				else if (this.queuedChanges[num34, 0] == "ChangeCubesHealth")
				{
					this.ChangeCubesHealth(this.queuedChanges[num34, 1]);
				}
			}
		}
		return 0;
	}

	public ushort GetCubeFill(int x, int y, int z)
	{
		if (x < 0 || x >= this.sizeX || y < 0 || y >= this.sizeY || z < 0 || z >= this.sizeZ)
		{
			return 55;
		}
		return this.cubes[x, y, z].type;
	}

	public byte GetCubeData(int x, int y, int z)
	{
		if (x < 0 || x >= this.sizeX || y < 0 || y >= this.sizeY || z < 0 || z >= this.sizeZ)
		{
			return 0;
		}
		return this.cubes[x, y, z].data;
	}

	public void PlayCubeHit(Vector3 pos, SoundHitType sht)
	{
		int type = (int)this.cubes[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z)].type;
		if (type < 155)
		{
			Kube.OH.PlayMaterialSound(Kube.OH.cubesSound[type], sht, pos, 1f);
		}
		else
		{
			Kube.OH.PlayMaterialSound(Kube.IS.gameItemsGO[type - 155].GetComponent<ItemPropsScript>().soundMaterialType, sht, pos, 1f);
		}
	}

	public void PlayCubeSparks(Vector3 posCube, Vector3 pos, Vector3 normal, SoundHitType sht)
	{
		Kube.OH.PlayerSparks(Kube.OH.cubesSound[(int)this.cubes[Mathf.RoundToInt(posCube.x), Mathf.RoundToInt(posCube.y), Mathf.RoundToInt(posCube.z)].type], sht, pos, normal);
	}

	public void SetDayLight(float tLight)
	{
		this.sunLightQuants = 16;
		if (tLight < 0f)
		{
			tLight = 0f;
		}
		if (tLight > 1f)
		{
			tLight = 1f;
		}
		int num = Mathf.RoundToInt(tLight * (float)this.sunLightQuants);
		if (this.currentSunLight != num)
		{
			this.sunInt = tLight;
			Kube.ASS3.blendedSkybox.SetFloat("_Blend", 1f - tLight);
			this.currentSunLight = num;
			this.RedrawWorld(true, true, false);
		}
	}

	private Color32 GetWorldLightInCube(int x, int y, int z)
	{
		Color32 result = default(Color32);
		if (!this.IsInWorld(x, y, z) || this.cubes == null)
		{
			return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
		}
		result.r = (byte)Mathf.Min(255f, (float)this.cubes[x, y, z].sunLight * this.sunInt * this.sunR + (float)this.cubes[x, y, z].sunLight * (1f - this.sunInt) * this.moonR + (float)this.cubes[x, y, z].lightR);
		result.g = (byte)Mathf.Min(255f, (float)this.cubes[x, y, z].sunLight * this.sunInt * this.sunG + (float)this.cubes[x, y, z].sunLight * (1f - this.sunInt) * this.moonG + (float)this.cubes[x, y, z].lightG);
		result.b = (byte)Mathf.Min(255f, (float)this.cubes[x, y, z].sunLight * this.sunInt * this.sunB + (float)this.cubes[x, y, z].sunLight * (1f - this.sunInt) * this.moonB + (float)this.cubes[x, y, z].lightB);
		return result;
	}

	public Color32 GetWorldLightAtPoint(Vector3 pos)
	{
		float num = (float)Mathf.FloorToInt(pos.x);
		float num2 = (float)Mathf.CeilToInt(pos.x);
		float num3 = (float)Mathf.FloorToInt(pos.y);
		float num4 = (float)Mathf.CeilToInt(pos.y);
		float num5 = (float)Mathf.FloorToInt(pos.z);
		float num6 = (float)Mathf.CeilToInt(pos.z);
		float x = pos.x;
		float y = pos.y;
		float z = pos.z;
		if (num == num2 && num3 == num4 && num5 == num6)
		{
			return this.GetWorldLightInCube((int)x, (int)y, (int)z);
		}
		Color32 worldLightInCube = this.GetWorldLightInCube((int)num, (int)num3, (int)num5);
		Color32 worldLightInCube2 = this.GetWorldLightInCube((int)num, (int)num3, (int)num6);
		Color32 worldLightInCube3 = this.GetWorldLightInCube((int)num, (int)num4, (int)num5);
		Color32 worldLightInCube4 = this.GetWorldLightInCube((int)num, (int)num4, (int)num6);
		Color32 worldLightInCube5 = this.GetWorldLightInCube((int)num2, (int)num3, (int)num5);
		Color32 worldLightInCube6 = this.GetWorldLightInCube((int)num2, (int)num3, (int)num6);
		Color32 worldLightInCube7 = this.GetWorldLightInCube((int)num2, (int)num4, (int)num5);
		Color32 worldLightInCube8 = this.GetWorldLightInCube((int)num2, (int)num4, (int)num6);
		Color32 result = default(Color32);
		float num7 = (float)worldLightInCube.r * (num2 - x) * (num4 - y) * (num6 - z) + (float)worldLightInCube2.r * (num2 - x) * (num4 - y) * (z - num5) + (float)worldLightInCube3.r * (num2 - x) * (y - num3) * (num6 - z) + (float)worldLightInCube4.r * (num2 - x) * (y - num3) * (z - num5) + (float)worldLightInCube5.r * (x - num) * (num4 - y) * (num6 - z) + (float)worldLightInCube6.r * (x - num) * (num4 - y) * (z - num5) + (float)worldLightInCube7.r * (x - num) * (y - num3) * (num6 - z) + (float)worldLightInCube8.r * (x - num) * (y - num3) * (z - num5);
		float num8 = (float)worldLightInCube.g * (num2 - x) * (num4 - y) * (num6 - z) + (float)worldLightInCube2.g * (num2 - x) * (num4 - y) * (z - num5) + (float)worldLightInCube3.g * (num2 - x) * (y - num3) * (num6 - z) + (float)worldLightInCube4.g * (num2 - x) * (y - num3) * (z - num5) + (float)worldLightInCube5.g * (x - num) * (num4 - y) * (num6 - z) + (float)worldLightInCube6.g * (x - num) * (num4 - y) * (z - num5) + (float)worldLightInCube7.g * (x - num) * (y - num3) * (num6 - z) + (float)worldLightInCube8.g * (x - num) * (y - num3) * (z - num5);
		float num9 = (float)worldLightInCube.b * (num2 - x) * (num4 - y) * (num6 - z) + (float)worldLightInCube2.b * (num2 - x) * (num4 - y) * (z - num5) + (float)worldLightInCube3.b * (num2 - x) * (y - num3) * (num6 - z) + (float)worldLightInCube4.b * (num2 - x) * (y - num3) * (z - num5) + (float)worldLightInCube5.b * (x - num) * (num4 - y) * (num6 - z) + (float)worldLightInCube6.b * (x - num) * (num4 - y) * (z - num5) + (float)worldLightInCube7.b * (x - num) * (y - num3) * (num6 - z) + (float)worldLightInCube8.b * (x - num) * (y - num3) * (z - num5);
		if (num7 == 0f && num8 == 0f && num9 == 0f)
		{
			num7 = (float)Mathf.Max(new int[]
			{
				(int)worldLightInCube.r,
				(int)worldLightInCube2.r,
				(int)worldLightInCube3.r,
				(int)worldLightInCube4.r,
				(int)worldLightInCube5.r,
				(int)worldLightInCube6.r,
				(int)worldLightInCube7.r,
				(int)worldLightInCube8.r
			});
			num8 = (float)Mathf.Max(new int[]
			{
				(int)worldLightInCube.g,
				(int)worldLightInCube2.g,
				(int)worldLightInCube3.g,
				(int)worldLightInCube4.g,
				(int)worldLightInCube5.g,
				(int)worldLightInCube6.g,
				(int)worldLightInCube7.g,
				(int)worldLightInCube8.g
			});
			num9 = (float)Mathf.Max(new int[]
			{
				(int)worldLightInCube.b,
				(int)worldLightInCube2.b,
				(int)worldLightInCube3.b,
				(int)worldLightInCube4.b,
				(int)worldLightInCube5.b,
				(int)worldLightInCube6.b,
				(int)worldLightInCube7.b,
				(int)worldLightInCube8.b
			});
		}
		result.r = (byte)num7;
		result.g = (byte)num8;
		result.b = (byte)num9;
		return result;
	}

	public GameObject blockPrefab;

	public CubeStruct[,,] cubes;

	protected CubeGrid _cubegrid;

	public List<GameItemStruct> gameItems;

	public List<MagicItemStruct> magicItems;

	public BlockScript[,,] blocks;

	private bool[,,] blocksToChange;

	public bool[,,] isOccupied;

	public Texture2D[] defaultMaps;

	private int containerSize;

	private bool needSaveMap;

	private int numCubesLightChange;

	private int[,] cubesLightChange;

	private WireScript[] wireS;

	private GameObject[] AAgo;

	[NonSerialized]
	public TriggerScript[] triggerS;

	public MonsterRespawnScript[] monsterRespawnS;

	public float[] monsterLastDieTime;

	public TransportRespawnScript[] transportRespawnS;

	public float[] transportLastDieTime;

	[NonSerialized]
	public Vector2[,] cubesTexUV;

	public int sizeX;

	public int sizeY;

	public int sizeZ;

	public int blockSizeX;

	public int blockSizeY;

	public int blockSizeZ;

	private int nBlocksX;

	private int nBlocksY;

	private int nBlocksZ;

	public int[] cubesDrawTypes;

	public CubePhys[] cubePhys;

	public int[,] cubesSidesTex;

	private bool[,,] checkLight;

	private int[,] lightSurface;

	public float sunR = 1f;

	public float sunG = 1f;

	public float sunB = 1f;

	public float sunInt = 1f;

	public float moonR = 0.05f;

	public float moonG = 0.05f;

	public float moonB = 0.15f;

	public byte maxWaterLevel = 7;

	public float waterCalculateDeltaTime = 0.2f;

	public float waterCalculateLastTime;

	private int numLightWaveSources;

	private int numLightWaveSourcesNew;

	private WorldHolderScript.Vector3Int[] lightWaveSources;

	private int numAntiLightWaveSources;

	private int numAntiLightWaveSourcesNew;

	private byte initAntiLight;

	private WorldHolderScript.Vector3Int[] antiLightWaveSources;

	private int numLightItemWaveSources;

	private int numLightItemWaveSourcesNew;

	private WorldHolderScript.Vector3Int[] lightItemWaveSources;

	private int numAntiLightItemWaveSources;

	private int numAntiLightItemWaveSourcesNew;

	private byte initAntiLightItem;

	private WorldHolderScript.Vector3Int[] antiLightItemWaveSources;

	private bool isFirstUpdateWaterBuffer = true;

	private WorldHolderScript.Vector3Int[] updateWaterBuffer1;

	private WorldHolderScript.Vector3Int[] updateWaterBuffer2;

	private int numUpdateWaterBuffer1;

	private int numUpdateWaterBuffer2;

	private bool[,,] waterBlocksToChange;

	private bool initialized;

	private int[,] lightNeibours = new int[,]
	{
		{
			0,
			-1,
			0
		},
		{
			0,
			1,
			0
		},
		{
			1,
			0,
			0
		},
		{
			-1,
			0,
			0
		},
		{
			0,
			0,
			1
		},
		{
			0,
			0,
			-1
		}
	};

	private WorldHolderScript.Vector3Int[] tmpLight;

	private int numQueuedChanges;

	private string[,] queuedChanges = new string[1024, 2];

	private byte[] packedWorld;

	private int currentSunLight = -1;

	private int sunLightQuants = 16;

	private struct Vector3Int
	{
		public byte x;

		public byte y;

		public byte z;
	}
}
