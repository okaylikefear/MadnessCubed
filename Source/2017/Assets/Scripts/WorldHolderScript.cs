using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ionic.Zlib;
using kube;
using kube.map;
using UnityEngine;

public class WorldHolderScript : MonoBehaviour
{
	public bool cubeContainsVoxelsDestructive(int x, int y, int z, int halfSize)
	{
		int num = this._cubegrid.chunks.GetLength(1);
		if (halfSize > 16)
		{
			int num2 = x >> 4;
			int num3 = y >> 4;
			int num4 = z >> 4;
			int num5 = halfSize >> 4;
			bool flag = false;
			for (int i = 0; i < num5; i++)
			{
				for (int j = 0; j < num5; j++)
				{
					for (int k = 0; k < num5; k++)
					{
						if (num3 + j < num)
						{
							Chunk chunk = this._cubegrid.chunks[num2 + i, num3 + j, num4 + k];
							if (chunk != null && chunk.type != null)
							{
								flag = true;
							}
						}
					}
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		num *= 16;
		for (int l = 0; l < halfSize; l++)
		{
			for (int m = 0; m < halfSize; m++)
			{
				for (int n = 0; n < halfSize; n++)
				{
					if (y + m < num)
					{
						if (this.cubesDrawTypes[this.cubeTypes[x + l, y + m, z + n]] == 0)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	private void Awake()
	{
		Kube.WHS = this;
		this.itemToCube = new int[Kube.IS.gameItemsGO.Length];
		this.cubeToItem = new int[Kube.OH.blockTypes.Length];
		for (int i = 0; i < this.itemToCube.Length; i++)
		{
			this.itemToCube[i] = 0;
		}
		for (int j = 0; j < this.cubeToItem.Length; j++)
		{
			this.cubeToItem[j] = ((Kube.OH.blockTypes[j].type != 1) ? -1 : Kube.OH.blockTypes[j].itemId);
		}
		for (int k = 0; k < Kube.OH.blockTypes.Length; k++)
		{
			int itemId = Kube.OH.blockTypes[k].itemId;
			if (Kube.OH.blockTypes[k].type == 1)
			{
				this.itemToCube[itemId] = k;
			}
		}
		this.cubesHealth = new int[Kube.OH.cubesStrength.Length];
		for (int l = 0; l < this.cubesHealth.Length; l++)
		{
			this.cubesHealth[l] = (int)(64f * Kube.OH.cubesStrength[l]);
		}
	}

	private void CMD_skybox(object[] argv)
	{
		this.skybox = int.Parse(argv[1].ToString());
	}

	private void CMD_kube_fill(object[] argv)
	{
		int x = int.Parse(argv[1].ToString());
		int y = int.Parse(argv[2].ToString());
		int z = int.Parse(argv[3].ToString());
		int wx = int.Parse(argv[4].ToString());
		int wy = int.Parse(argv[5].ToString());
		int wz = int.Parse(argv[6].ToString());
		int type = int.Parse(argv[7].ToString());
		this.kube_fill(x, y, z, wx, wy, wz, type);
	}

	private void kube_fill(int x, int y, int z, int wx, int wy, int wz, int type)
	{
		string text = string.Empty;
		int num = 0;
		for (int i = x; i < x + wx; i++)
		{
			for (int j = y; j < y + wy; j++)
			{
				for (int k = z; k < z + wz; k++)
				{
					string text2 = text;
					text = string.Concat(new string[]
					{
						text2,
						Kube.OH.GetServerCode(Mathf.RoundToInt((float)i), 2),
						string.Empty,
						Kube.OH.GetServerCode(Mathf.RoundToInt((float)j), 2),
						string.Empty,
						Kube.OH.GetServerCode(Mathf.RoundToInt((float)k), 2),
						string.Empty,
						Kube.OH.GetServerCode(type, 2)
					});
					num++;
				}
			}
		}
		int num2 = 128;
		if (num > num2)
		{
			UnityEngine.Debug.Log("Limit " + num2);
			return;
		}
		text = Kube.OH.GetServerCode(num, 2) + text;
		Kube.BCS.NO.ChangeCubes(text);
	}

	private void BuildMiniCubes()
	{
		this.miniCubesMat = new Material[Kube.OH.blockTypes.Length];
		for (int i = 0; i < this.miniCubesMat.Length; i++)
		{
			Material material = null;
			if (Kube.OH.blockTypes[i].type == 0)
			{
				int atlas = Kube.OH.blockTypes[i].atlas;
				if (atlas < 0)
				{
					int num = -atlas;
					if (num != 1)
					{
						if (num == 2)
						{
							material = Kube.ASS3.lavaMat;
						}
					}
					else
					{
						material = Kube.OH.waterAnimMat;
					}
				}
				else
				{
					material = UnityEngine.Object.Instantiate<Material>(Kube.ASS3.cubesMat[atlas]);
				}
				if (!(material == null))
				{
					if (atlas >= 0)
					{
						int num2 = Kube.OH.blockTypes[i].itemId;
						if (num2 < 0)
						{
							num2 = this.cubesSidesTex[-num2, 0];
						}
						Vector2 offset = this.cubesTexUV[num2, 0];
						material.SetTextureOffset("_MainTex", offset);
						material.SetTextureScale("_MainTex", new Vector2(0.125f, 0.125f));
					}
					this.miniCubesMat[i] = material;
				}
			}
		}
	}

	private void Start()
	{
		Kube.WHS = this;
		this.blendedSkybox = UnityEngine.Object.Instantiate<Material>(Kube.ASS3.blendedSkybox);
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

	public void ChangeWorldBytesCube(int x, int y, int z, ushort type, byte prop)
	{
		this._cubegrid.set(x, y, z, (int)type, (int)prop);
		this.needSaveMap = true;
	}

	public void ChangeWorldBytesItem(int x, int y, int z, int type, byte prop)
	{
		int num = this.itemToCube[type];
		if (num != 0)
		{
			this._cubegrid.set(x, y, z, num, (int)prop);
		}
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
		if ((x != 0 || y != 0 || z != 0) && this.triggerS[id])
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

	private void LightWaveSource(int x, int y, int z)
	{
		byte b = (byte)Mathf.Max(0, (int)(this.sunLight[x, y, z] - 16));
		if (b == 0)
		{
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			int num = x + this.lightNeibours[i, 0];
			int num2 = y + this.lightNeibours[i, 1];
			int num3 = z + this.lightNeibours[i, 2];
			if (this.IsInWorld(num, num2, num3))
			{
				if (this.sunLight[num, num2, num3] < b)
				{
					int num4 = (this.cubeData[num, num2, num3] != 0) ? 1 : this.cubesDrawTypes[this.cubeTypes[num, num2, num3]];
					if (num4 != 0)
					{
						this.blocksToChange[Mathf.FloorToInt((float)num / (float)this.blockSizeX), Mathf.FloorToInt((float)num2 / (float)this.blockSizeY), Mathf.FloorToInt((float)num3 / (float)this.blockSizeZ)] = true;
						this.sunLight[num, num2, num3] = b;
						this.LightWaveSource(num, num2, num3);
					}
				}
			}
		}
	}

	private void LightItemWaveSources(int x, int y, int z, int Y)
	{
		byte b = (byte)Mathf.Max(0, (int)(this.lightR[x, y, z] - 16));
		byte b2 = (byte)Mathf.Max(0, (int)(this.lightG[x, y, z] - 16));
		byte b3 = (byte)Mathf.Max(0, (int)(this.lightB[x, y, z] - 16));
		byte b4 = (byte)Mathf.Max(new int[]
		{
			(int)b,
			(int)b2,
			(int)b3
		});
		if (Y <= 0)
		{
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			int num = x + this.lightNeibours[i, 0];
			int num2 = y + this.lightNeibours[i, 1];
			int num3 = z + this.lightNeibours[i, 2];
			if (this.IsInWorld(num, num2, num3))
			{
				if (this.lightR[num, num2, num3] < b || this.lightG[num, num2, num3] < b2 || this.lightB[num, num2, num3] < b3)
				{
					int num4 = (this.cubeData[num, num2, num3] != 0) ? 1 : this.cubesDrawTypes[this.cubeTypes[num, num2, num3]];
					if (num4 != 0)
					{
						this.blocksToChange[Mathf.FloorToInt((float)num / (float)this.blockSizeX), Mathf.FloorToInt((float)num2 / (float)this.blockSizeY), Mathf.FloorToInt((float)num3 / (float)this.blockSizeZ)] = true;
						this.lightR[num, num2, num3] = (byte)Mathf.Max((int)this.lightR[num, num2, num3], (int)b);
						this.lightG[num, num2, num3] = (byte)Mathf.Max((int)this.lightG[num, num2, num3], (int)b2);
						this.lightB[num, num2, num3] = (byte)Mathf.Max((int)this.lightB[num, num2, num3], (int)b3);
						this.LightItemWaveSources(num, num2, num3, Y - 16);
					}
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
		WorldHolderScript.Vector3Int[] array;
		int num;
		if (flag)
		{
			array = this.updateWaterBuffer1;
			num = this.numUpdateWaterBuffer1;
		}
		else
		{
			array = this.updateWaterBuffer2;
			num = this.numUpdateWaterBuffer2;
		}
		if (num >= array.Length)
		{
			UnityEngine.Debug.LogError("SetWaterToCheck FULL");
			return;
		}
		int i;
		for (i = 0; i < num; i++)
		{
			if ((int)array[i].x == x && (int)array[i].y == y && (int)array[i].z == z)
			{
				break;
			}
		}
		if (i == num)
		{
			array[num].x = (byte)x;
			array[num].y = (byte)y;
			array[num].z = (byte)z;
			if (flag)
			{
				this.numUpdateWaterBuffer1++;
			}
			else
			{
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
		WorldHolderScript.Vector3Int[] array;
		int num;
		if (this.isFirstUpdateWaterBuffer)
		{
			array = this.updateWaterBuffer1;
			num = this.numUpdateWaterBuffer1;
		}
		else
		{
			array = this.updateWaterBuffer2;
			num = this.numUpdateWaterBuffer2;
		}
		for (int l = 0; l < num; l++)
		{
			int x = (int)array[l].x;
			int y = (int)array[l].y;
			int z = (int)array[l].z;
			if (this.IsInWorld(x, y, z))
			{
				if (this.waterLevel[x, y, z] != 0)
				{
					if (this.IsInWorld(x, y - 1, z) && (this.cubeTypes[x, y - 1, z] == 0 || (this.cubeTypes[x, y - 1, z] == 128 && this.waterLevel[x, y - 1, z] != this.maxWaterLevel)))
					{
						this.cubeTypes[x, y - 1, z] = 128;
						this.phys[x, y - 1, z] = (byte)this.cubePhys[this.cubeTypes[x, y - 1, z]];
						this.waterLevel[x, y - 1, z] = this.maxWaterLevel;
						this.SetWaterToCheck(x, y - 1, z, false);
						this.CheckWaterBlocksToChange(x, y - 1, z);
						this.ChangeWorldBytesCube(x, y - 1, z, (ushort)this.cubeTypes[x, y - 1, z], this.waterLevel[x, y - 1, z]);
					}
					else
					{
						int x2 = x;
						int y2 = y + 1;
						int z2 = z;
						if (this.IsInWorld(x2, y2, z2) && this.cubeTypes[x2, y2, z2] != 128 && this.waterLevel[x, y, z] != this.maxWaterLevel)
						{
							byte b = 0;
							int num2 = 0;
							if (this.IsInWorld(x + 1, y, z))
							{
								if (this.waterLevel[x + 1, y, z] > b)
								{
									b = this.waterLevel[x + 1, y, z];
								}
								if (this.waterLevel[x + 1, y, z] == this.maxWaterLevel)
								{
									num2++;
								}
							}
							if (this.IsInWorld(x - 1, y, z))
							{
								if (this.waterLevel[x - 1, y, z] > b)
								{
									b = this.waterLevel[x - 1, y, z];
								}
								if (this.waterLevel[x - 1, y, z] == this.maxWaterLevel)
								{
									num2++;
								}
							}
							if (this.IsInWorld(x, y, z + 1))
							{
								if (this.waterLevel[x, y, z + 1] > b)
								{
									b = this.waterLevel[x, y, z + 1];
								}
								if (this.waterLevel[x, y, z + 1] == this.maxWaterLevel)
								{
									num2++;
								}
							}
							if (this.IsInWorld(x, y, z - 1))
							{
								if (this.waterLevel[x, y, z - 1] > b)
								{
									b = this.waterLevel[x, y, z - 1];
								}
								if (this.waterLevel[x, y, z - 1] == this.maxWaterLevel)
								{
									num2++;
								}
							}
							if (num2 >= 3)
							{
								this.waterLevel[x, y, z] = this.maxWaterLevel;
								this.ChangeWorldBytesCube(x, y, z, (ushort)this.cubeTypes[x, y, z], this.waterLevel[x, y, z]);
								this.SetWaterToCheck(x, y, z, false);
								this.CheckWaterBlocksToChange(x, y, z);
								goto IL_D61;
							}
							if (b <= this.waterLevel[x, y, z])
							{
								CubeWaterGrid cubeWaterGrid2;
								CubeWaterGrid cubeWaterGrid = cubeWaterGrid2 = this.waterLevel;
								int x4;
								int x3 = x4 = x;
								int y4;
								int y3 = y4 = y;
								int z4;
								int z3 = z4 = z;
								byte b2 = cubeWaterGrid2[x4, y4, z4];
								cubeWaterGrid[x3, y3, z3] = b2 - 1;
								this.CheckWaterBlocksToChange(x, y, z);
								this.SetWaterToCheck(x + 1, y, z, false);
								this.SetWaterToCheck(x - 1, y, z, false);
								this.SetWaterToCheck(x, y, z + 1, false);
								this.SetWaterToCheck(x, y, z - 1, false);
								this.SetWaterToCheck(x, y, z, false);
								if (this.waterLevel[x, y, z] == 0)
								{
									this.cubeTypes[x, y, z] = 0;
									this.phys[x, y, z] = (byte)this.cubePhys[this.cubeTypes[x, y, z]];
									this.ChangeWorldBytesCube(x, y, z, (ushort)this.cubeTypes[x, y, z], this.waterLevel[x, y, z]);
									if (this.IsInWorld(x, y - 1, z) && this.cubeTypes[x, y - 1, z] == 128)
									{
										this.waterLevel[x, y - 1, z] = this.maxWaterLevel - 1;
										this.SetWaterToCheck(x, y - 1, z, false);
										this.CheckWaterBlocksToChange(x, y - 1, z);
										this.ChangeWorldBytesCube(x, y - 1, z, (ushort)this.cubeTypes[x, y - 1, z], this.waterLevel[x, y - 1, z]);
									}
								}
								goto IL_D61;
							}
						}
						if (!this.IsInWorld(x, y - 1, z) || this.cubeTypes[x, y - 1, z] != 128)
						{
							int m = 0;
							if (this.waterLevel[x, y, z] > 1)
							{
								for (m = 1; m < (int)this.maxWaterLevel; m++)
								{
									bool flag = false;
									bool flag2 = false;
									for (int n = x - m; n <= x + m; n++)
									{
										for (int num3 = z - m; num3 <= z + m; num3++)
										{
											if (Mathf.Abs(x - n) + Mathf.Abs(z - num3) == m)
											{
												if (this.IsInWorld(n, y, num3) && this.IsInWorld(n, y - 1, num3) && (this.cubeTypes[n, y, num3] == 0 || this.cubeTypes[n, y - 1, num3] == 128))
												{
													flag = true;
													if (this.IsInWorld(n, y - 1, num3) && (this.cubeTypes[n, y - 1, num3] == 0 || this.cubeTypes[n, y - 1, num3] == 128))
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
								for (int num4 = x - m; num4 <= x + m; num4++)
								{
									for (int num5 = z - m; num5 <= z + m; num5++)
									{
										if (Mathf.Abs(x - num4) + Mathf.Abs(z - num5) == m)
										{
											if (this.IsInWorld(num4, y, num5) && (this.cubeTypes[num4, y, num5] == 0 || this.cubeTypes[num4, y - 1, num5] == 128) && this.IsInWorld(num4, y - 1, num5) && (this.cubeTypes[num4, y - 1, num5] == 0 || this.cubeTypes[num4, y - 1, num5] == 128))
											{
												flag7 = true;
												if (num5 > z)
												{
													flag3 = true;
												}
												if (num5 < z)
												{
													flag4 = true;
												}
												if (num4 > x)
												{
													flag5 = true;
												}
												if (num4 < x)
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
								x2 = x;
								y2 = y;
								z2 = z + 1;
								if (flag3 && this.IsInWorld(x2, y2, z2) && (this.cubeTypes[x2, y2, z2] == 0 || (this.cubeTypes[x2, y2, z2] == 128 && this.waterLevel[x2, y2, z2] < this.waterLevel[x, y, z])))
								{
									this.cubeTypes[x2, y2, z2] = 128;
									this.phys[x2, y2, z2] = (byte)this.cubePhys[this.cubeTypes[x2, y2, z2]];
									this.waterLevel[x2, y2, z2] = this.waterLevel[x, y, z] - 1;
									this.SetWaterToCheck(x2, y2, z2, false);
									this.CheckWaterBlocksToChange(x2, y2, z2);
									this.ChangeWorldBytesCube(x2, y2, z2, (ushort)this.cubeTypes[x2, y2, z2], this.waterLevel[x2, y2, z2]);
								}
								x2 = x;
								y2 = y;
								z2 = z - 1;
								if (flag4 && this.IsInWorld(x2, y2, z2) && (this.cubeTypes[x2, y2, z2] == 0 || (this.cubeTypes[x2, y2, z2] == 128 && this.waterLevel[x2, y2, z2] < this.waterLevel[x, y, z])))
								{
									this.cubeTypes[x2, y2, z2] = 128;
									this.phys[x2, y2, z2] = (byte)this.cubePhys[this.cubeTypes[x2, y2, z2]];
									this.waterLevel[x2, y2, z2] = this.waterLevel[x, y, z] - 1;
									this.SetWaterToCheck(x2, y2, z2, false);
									this.CheckWaterBlocksToChange(x2, y2, z2);
									this.ChangeWorldBytesCube(x2, y2, z2, (ushort)this.cubeTypes[x2, y2, z2], this.waterLevel[x2, y2, z2]);
								}
								x2 = x + 1;
								y2 = y;
								z2 = z;
								if (flag5 && this.IsInWorld(x2, y2, z2) && (this.cubeTypes[x2, y2, z2] == 0 || (this.cubeTypes[x2, y2, z2] == 128 && this.waterLevel[x2, y2, z2] < this.waterLevel[x, y, z])))
								{
									this.cubeTypes[x2, y2, z2] = 128;
									this.phys[x2, y2, z2] = (byte)this.cubePhys[this.cubeTypes[x2, y2, z2]];
									this.waterLevel[x2, y2, z2] = this.waterLevel[x, y, z] - 1;
									this.SetWaterToCheck(x2, y2, z2, false);
									this.CheckWaterBlocksToChange(x2, y2, z2);
									this.ChangeWorldBytesCube(x2, y2, z2, (ushort)this.cubeTypes[x2, y2, z2], this.waterLevel[x2, y2, z2]);
								}
								x2 = x - 1;
								y2 = y;
								z2 = z;
								if (flag6 && this.IsInWorld(x2, y2, z2) && (this.cubeTypes[x2, y2, z2] == 0 || (this.cubeTypes[x2, y2, z2] == 128 && this.waterLevel[x2, y2, z2] < this.waterLevel[x, y, z])))
								{
									this.cubeTypes[x2, y2, z2] = 128;
									this.phys[x2, y2, z2] = (byte)this.cubePhys[this.cubeTypes[x2, y2, z2]];
									this.waterLevel[x2, y2, z2] = this.waterLevel[x, y, z] - 1;
									this.SetWaterToCheck(x2, y2, z2, false);
									this.CheckWaterBlocksToChange(x2, y2, z2);
									this.ChangeWorldBytesCube(x2, y2, z2, (ushort)this.cubeTypes[x2, y2, z2], this.waterLevel[x2, y2, z2]);
								}
							}
						}
					}
				}
			}
			IL_D61:;
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
		for (int num6 = 0; num6 < this.nBlocksX; num6++)
		{
			for (int num7 = 0; num7 < this.nBlocksY; num7++)
			{
				for (int num8 = 0; num8 < this.nBlocksZ; num8++)
				{
					if (this.waterBlocksToChange[num6, num7, num8])
					{
						this.blocks[num6, num7, num8].RefreshWaterMesh();
						this.blocks[num6, num7, num8].RecountLight();
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
		this.blocks = new BlockScript[this.nBlocksX, this.nBlocksY, this.nBlocksZ];
		this.blocksToChange = new bool[this.nBlocksX, this.nBlocksY, this.nBlocksZ];
		this.waterBlocksToChange = new bool[this.nBlocksX, this.nBlocksY, this.nBlocksZ];
		this.isOccupied = new BitData(this.sizeX, this.sizeY, this.sizeZ);
		this.lightSurface = new int[this.sizeX, this.sizeZ];
		this.updateWaterBuffer1 = new WorldHolderScript.Vector3Int[this.sizeX * this.sizeZ * 4];
		this.updateWaterBuffer2 = new WorldHolderScript.Vector3Int[this.sizeX * this.sizeZ * 4];
		this.numUpdateWaterBuffer1 = (this.numUpdateWaterBuffer2 = 0);
		int num = Kube.OH.blockTypes.Length;
		this.cubesLightChange = new int[4096, 3];
		this.cubesDrawTypes = new int[num];
		for (int i = 0; i < Kube.OH.blockTypes.Length; i++)
		{
			this.cubesDrawTypes[i] = 4;
		}
		this.cubePhys = new CubePhys[num];
		this._cubegrid = new CubeGrid(this.sizeX, this.sizeY, this.sizeZ);
		this.cubeTypes = new CubeTypesGrid(this._cubegrid);
		this.cubeData = new CubeDataGrid(this._cubegrid);
		this.waterLevel = new CubeWaterGrid(this._cubegrid);
		this.sunLight = new LightData(this.sizeX, this.sizeY, this.sizeZ);
		this.lightR = new LightData(this.sizeX, this.sizeY, this.sizeZ);
		this.lightG = new LightData(this.sizeX, this.sizeY, this.sizeZ);
		this.lightB = new LightData(this.sizeX, this.sizeY, this.sizeZ);
		this.phys = new QuadData(this.sizeX, this.sizeY, this.sizeZ);
		this.prop = new QuadData(this.sizeX, this.sizeY, this.sizeZ);
		this.cubesDrawTypes[0] = 4;
		this.cubePhys[0] = CubePhys.air;
		for (int j = 1; j < 64; j++)
		{
			this.cubesDrawTypes[j] = 0;
			this.cubePhys[j] = CubePhys.solid;
		}
		for (int k = 80; k < 96; k++)
		{
			this.cubesDrawTypes[k] = 0;
			this.cubePhys[k] = CubePhys.solid;
		}
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
		for (int l = 130; l < 154; l++)
		{
			this.cubesDrawTypes[l] = 0;
			this.cubePhys[l] = CubePhys.solid;
		}
		this.cubesDrawTypes[128] = 2;
		this.cubePhys[128] = CubePhys.water;
		this.cubesDrawTypes[129] = 3;
		this.cubePhys[129] = CubePhys.lava;
		this.cubesDrawTypes[256] = 0;
		this.cubePhys[256] = CubePhys.lava;
		this.cubesSidesTex = new int[128, 3];
		this.cubesSidesTex[0, 0] = 0;
		this.cubesSidesTex[0, 1] = 0;
		this.cubesSidesTex[0, 2] = 0;
		this.cubesSidesTex[1, 0] = 1;
		this.cubesSidesTex[1, 1] = 2;
		this.cubesSidesTex[1, 2] = 3;
		this.cubesSidesTex[2, 0] = 15;
		this.cubesSidesTex[2, 1] = 16;
		this.cubesSidesTex[2, 2] = 15;
		this.cubesSidesTex[3, 0] = 36;
		this.cubesSidesTex[3, 1] = 37;
		this.cubesSidesTex[3, 2] = 21;
		this.cubesSidesTex[4, 0] = 36;
		this.cubesSidesTex[4, 1] = 38;
		this.cubesSidesTex[4, 2] = 3;
		this.cubesTexUV = new Vector2[64, 4];
		for (int m = 0; m < 64; m++)
		{
			float num2 = (float)(m % 8) / 8f;
			float num3 = Mathf.Floor((float)m / 8f) / 8f;
			this.cubesTexUV[m, 1].x = num2;
			this.cubesTexUV[m, 1].y = 1f - num3;
			this.cubesTexUV[m, 2].x = num2 + 0.125f;
			this.cubesTexUV[m, 2].y = 1f - num3;
			this.cubesTexUV[m, 3].x = num2 + 0.125f;
			this.cubesTexUV[m, 3].y = 1f - (num3 + 0.125f);
			this.cubesTexUV[m, 0].x = num2;
			this.cubesTexUV[m, 0].y = 1f - (num3 + 0.125f);
		}
		this.BuildMiniCubes();
		this.transportRespawnS = new TransportRespawnScript[1024];
		this.transportLastDieTime = new float[1024];
		this.triggerS = new TriggerScript[1024];
		this.monsterRespawnS = new MonsterRespawnScript[1024];
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
		foreach (GameObject go in this.StaticBatchCombine.Values)
		{
			this.StaticBatchingUtility_Light(go);
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
									if (this.cubeTypes[i * this.blockSizeX + l, j * this.blockSizeY + n, k * this.blockSizeZ + m] != 0)
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
							goto IL_27E;
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
					IL_27E:;
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
		gameObject.GetComponentInChildren<BoxCollider>().size = new Vector3(6f, (float)this.sizeY, (float)this.sizeZ);
		gameObject = (UnityEngine.Object.Instantiate(Kube.OH.boundsPlane, new Vector3((float)this.sizeX - 0.5f, (float)this.sizeY / 2f, (float)this.sizeZ / 2f - 0.5f), Quaternion.Euler(0f, 180f, 0f)) as GameObject);
		gameObject.GetComponentInChildren<BoxCollider>().size = new Vector3(6f, (float)this.sizeY, (float)this.sizeZ);
		gameObject = (UnityEngine.Object.Instantiate(Kube.OH.boundsPlane, new Vector3((float)this.sizeX / 2f - 0.5f, (float)this.sizeY / 2f, -0.5f), Quaternion.Euler(0f, -90f, 0f)) as GameObject);
		gameObject.GetComponentInChildren<BoxCollider>().size = new Vector3(6f, (float)this.sizeY, (float)this.sizeZ);
		gameObject = (UnityEngine.Object.Instantiate(Kube.OH.boundsPlane, new Vector3((float)this.sizeX / 2f - 0.5f, (float)this.sizeY / 2f, (float)this.sizeZ - 0.5f), Quaternion.Euler(0f, 90f, 0f)) as GameObject);
		gameObject.GetComponentInChildren<BoxCollider>().size = new Vector3(6f, (float)this.sizeY, (float)this.sizeZ);
		gameObject = (UnityEngine.Object.Instantiate(Kube.OH.boundsPlane, new Vector3((float)this.sizeX / 2f - 0.5f, (float)this.sizeY, (float)this.sizeZ / 2f - 0.5f), Quaternion.Euler(0f, 0f, -90f)) as GameObject);
		gameObject.GetComponentInChildren<BoxCollider>().size = new Vector3(6f, (float)this.sizeX, (float)this.sizeZ);
		foreach (GameObject go in this.StaticBatchCombine.Values)
		{
			this.StaticBatchingUtility_Combine(go);
		}
	}

	private void StaticBatchingUtility_Combine(GameObject go)
	{
		MeshFilter[] componentsInChildren = go.GetComponentsInChildren<MeshFilter>();
		CombineInstance[] array = new CombineInstance[componentsInChildren.Length];
		Material material = null;
		List<Color> list = new List<Color>();
		for (int i = 0; i < array.Length; i++)
		{
			MeshRenderer component = componentsInChildren[i].GetComponent<MeshRenderer>();
			if (component)
			{
				array[i].mesh = componentsInChildren[i].sharedMesh;
				array[i].transform = componentsInChildren[i].gameObject.transform.localToWorldMatrix;
				int vertexCount = array[i].mesh.vertexCount;
				for (int j = 0; j < vertexCount; j++)
				{
					Color item = Kube.WHS.GetWorldLightAtPoint(componentsInChildren[i].gameObject.transform.position);
					list.Add(item);
				}
				if (material == null)
				{
					material = component.material;
				}
				UnityEngine.Object.Destroy(componentsInChildren[i]);
				UnityEngine.Object.Destroy(component);
			}
		}
		go.AddComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		go.GetComponent<MeshFilter>().mesh = mesh;
		go.GetComponent<MeshFilter>().mesh.CombineMeshes(array, true, true);
		mesh.colors = list.ToArray();
		MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
		if (material.shader.name.Contains("Alpha"))
		{
			material.shader = Kube.ASS3.cubesMat[1].shader;
		}
		else
		{
			material.shader = Kube.ASS3.cubesMat[0].shader;
		}
		meshRenderer.sharedMaterial = material;
	}

	private void StaticBatchingUtility_Light(GameObject go)
	{
		MeshFilter componentInChildren = go.GetComponentInChildren<MeshFilter>();
		Mesh sharedMesh = componentInChildren.sharedMesh;
		List<Color> list = new List<Color>();
		int vertexCount = sharedMesh.vertexCount;
		Vector3[] vertices = sharedMesh.vertices;
		for (int i = 0; i < vertexCount; i++)
		{
			Color item = Kube.WHS.GetWorldLightAtPoint(vertices[i]);
			list.Add(item);
		}
		sharedMesh.colors = list.ToArray();
		componentInChildren.sharedMesh = sharedMesh;
	}

	private void _GenerateWorld(Texture2D defaultMaps)
	{
		int num = 0;
		MonoBehaviour.print("Selected new map: " + num);
		if (defaultMaps.width == 128 && defaultMaps.height == 128)
		{
			this.containerSize = 1048576;
			this.Init(128, 96, 128, true);
		}
		else
		{
			if (defaultMaps.width != 224 || defaultMaps.height != 224)
			{
				MonoBehaviour.print("Bad map size");
				return;
			}
			this.containerSize = 4194304;
			this.Init(224, 125, 224, true);
		}
		int num2 = (int)((float)this.sizeY * 0.3f);
		for (int i = 0; i < this.sizeX; i++)
		{
			for (int j = 0; j < this.sizeZ; j++)
			{
				int num3 = Mathf.RoundToInt(defaultMaps.GetPixel(i, j).grayscale * (float)(this.sizeY - 1) * 0.3f + (float)this.sizeY * 0.25f);
				for (int k = 0; k < this.sizeY; k++)
				{
					if (k > num3 && k > num2)
					{
						this.cubeTypes[i, k, j] = 0;
						this.phys[i, k, j] = (byte)this.cubePhys[this.cubeTypes[i, k, j]];
						this.ChangeWorldBytesCube(i, k, j, (ushort)this.cubeTypes[i, k, j], 0);
					}
					else if (k > num3 && k <= num2)
					{
						this.cubeTypes[i, k, j] = 128;
						this.waterLevel[i, k, j] = this.maxWaterLevel;
						this.phys[i, k, j] = (byte)this.cubePhys[this.cubeTypes[i, k, j]];
						this.ChangeWorldBytesCube(i, k, j, (ushort)this.cubeTypes[i, k, j], this.waterLevel[i, k, j]);
					}
					else if (k > num2 && k == num3)
					{
						this.cubeTypes[i, k, j] = 1;
						this.phys[i, k, j] = (byte)this.cubePhys[this.cubeTypes[i, k, j]];
						this.ChangeWorldBytesCube(i, k, j, (ushort)this.cubeTypes[i, k, j], 0);
					}
					else if (k <= num2 && k <= num3 && k >= num3 - 2)
					{
						this.cubeTypes[i, k, j] = 13;
						this.phys[i, k, j] = (byte)this.cubePhys[this.cubeTypes[i, k, j]];
						this.ChangeWorldBytesCube(i, k, j, (ushort)this.cubeTypes[i, k, j], 0);
					}
					else if (k <= num3 && k >= num3 - 2)
					{
						this.cubeTypes[i, k, j] = 2;
						this.phys[i, k, j] = (byte)this.cubePhys[this.cubeTypes[i, k, j]];
						this.ChangeWorldBytesCube(i, k, j, (ushort)this.cubeTypes[i, k, j], 0);
					}
					else
					{
						this.cubeTypes[i, k, j] = 18;
						this.phys[i, k, j] = (byte)this.cubePhys[this.cubeTypes[i, k, j]];
						this.ChangeWorldBytesCube(i, k, j, (ushort)this.cubeTypes[i, k, j], 0);
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
				for (int j = num4; j < num2; j++)
				{
					bool flag = false;
					for (int k = this.sizeY - 1; k >= 0; k--)
					{
						this.lightR[i, k, j] = 0;
						this.lightG[i, k, j] = 0;
						this.lightB[i, k, j] = 0;
						if (!flag)
						{
							int num5 = this.cubesDrawTypes[this.cubeTypes[i, k, j]];
							if (this.cubeData[i, k, j] != 0)
							{
								num5 = 1;
							}
							if (num5 == 0)
							{
								flag = true;
								this.lightSurface[i, j] = k + 1;
								this.sunLight[i, k, j] = 0;
							}
							else
							{
								byte maxValue = byte.MaxValue;
								if (this.sunLight[i, k, j] != maxValue)
								{
									this.blocksToChange[Mathf.FloorToInt((float)i / (float)this.blockSizeX), Mathf.FloorToInt((float)k / (float)this.blockSizeY), Mathf.FloorToInt((float)j / (float)this.blockSizeZ)] = true;
								}
								this.sunLight[i, k, j] = maxValue;
							}
						}
						else
						{
							this.sunLight[i, k, j] = 0;
						}
					}
				}
			}
			for (int l = num3; l < num; l++)
			{
				for (int m = num4; m < num2; m++)
				{
					int num6 = Mathf.Max(new int[]
					{
						this.lightSurface[l - 1, m],
						this.lightSurface[l, m - 1],
						this.lightSurface[l + 1, m],
						this.lightSurface[l, m + 1]
					});
					for (int n = this.lightSurface[l, m]; n < num6; n++)
					{
						this.LightWaveSource(l, n, m);
						if (!fullCheck)
						{
						}
					}
				}
			}
			for (int num7 = 0; num7 < this.gameItems.Count; num7++)
			{
				GameItemStruct gameItemStruct = this.gameItems[num7];
				if (gameItemStruct.lightColor.grayscale > 0f)
				{
					this.lightR[gameItemStruct.x, gameItemStruct.y, gameItemStruct.z] = (byte)(255f * gameItemStruct.lightColor.r);
					this.lightG[gameItemStruct.x, gameItemStruct.y, gameItemStruct.z] = (byte)(255f * gameItemStruct.lightColor.g);
					this.lightB[gameItemStruct.x, gameItemStruct.y, gameItemStruct.z] = (byte)(255f * gameItemStruct.lightColor.b);
					this.LightItemWaveSources((int)((byte)gameItemStruct.x), (int)((byte)gameItemStruct.y), (int)((byte)gameItemStruct.z), 256);
				}
			}
		}
		else
		{
			int num3 = this.sizeX - 1;
			int num4 = this.sizeZ - 1;
			for (int num8 = this.numCubesLightChange - 1; num8 >= 0; num8--)
			{
				if (this.sunLight[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]] == 0)
				{
					byte b = 0;
					int num9 = 0;
					for (int num10 = 0; num10 < 6; num10++)
					{
						int x = this.cubesLightChange[num8, 0] + this.lightNeibours[num10, 0];
						int y = this.cubesLightChange[num8, 1] + this.lightNeibours[num10, 1];
						int z = this.cubesLightChange[num8, 2] + this.lightNeibours[num10, 2];
						if (this.IsInWorld(x, y, z) && this.sunLight[x, y, z] > b)
						{
							b = this.sunLight[x, y, z];
							num9 = num10;
						}
					}
					if (b == 255 && num9 == 1)
					{
						this.sunLight[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]] = byte.MaxValue;
						for (int num11 = this.cubesLightChange[num8, 1]; num11 >= 0; num11--)
						{
							int num12 = this.cubesLightChange[num8, 0];
							int num13 = num11;
							int num14 = this.cubesLightChange[num8, 2];
							int num15 = (this.cubeData[num12, num13, num14] != 0) ? 1 : this.cubesDrawTypes[this.cubeTypes[num12, num13, num14]];
							if (num15 != 0)
							{
								this.sunLight[num12, num13, num14] = byte.MaxValue;
								this.LightWaveSource(num12, num13, num14);
								this.blocksToChange[Mathf.FloorToInt((float)num12 / (float)this.blockSizeX), Mathf.FloorToInt((float)num13 / (float)this.blockSizeY), Mathf.FloorToInt((float)num14 / (float)this.blockSizeZ)] = true;
							}
						}
					}
					else if (b != 0)
					{
						int x2 = (int)((byte)(this.cubesLightChange[num8, 0] + this.lightNeibours[num9, 0]));
						int y2 = (int)((byte)(this.cubesLightChange[num8, 1] + this.lightNeibours[num9, 1]));
						int z2 = (int)((byte)(this.cubesLightChange[num8, 2] + this.lightNeibours[num9, 2]));
						this.LightWaveSource(x2, y2, z2);
					}
				}
				else
				{
					if (this.sunLight[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]] == 255)
					{
						if (this.cubeData[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]] == 0)
						{
							this.sunLight[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]] = 0;
						}
						this.numAntiLightWaveSources = 0;
						this.initAntiLight = byte.MaxValue;
						for (int num16 = this.cubesLightChange[num8, 1] - 1; num16 >= 0; num16--)
						{
							int num17 = this.cubesDrawTypes[this.cubeTypes[this.cubesLightChange[num8, 0], num16, this.cubesLightChange[num8, 2]]];
							if (this.cubeData[this.cubesLightChange[num8, 0], num16, this.cubesLightChange[num8, 2]] != 0)
							{
								num17 = 1;
							}
							if (num17 == 0)
							{
								break;
							}
							this.antiLightWaveSources[this.numAntiLightWaveSources].x = (byte)this.cubesLightChange[num8, 0];
							this.antiLightWaveSources[this.numAntiLightWaveSources].y = (byte)num16;
							this.antiLightWaveSources[this.numAntiLightWaveSources].z = (byte)this.cubesLightChange[num8, 2];
							this.numAntiLightWaveSources++;
							this.sunLight[this.cubesLightChange[num8, 0], num16, this.cubesLightChange[num8, 2]] = 0;
						}
					}
					else
					{
						this.numAntiLightWaveSources = 0;
						this.initAntiLight = this.sunLight[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]];
						this.antiLightWaveSources[this.numAntiLightWaveSources].x = (byte)this.cubesLightChange[num8, 0];
						this.antiLightWaveSources[this.numAntiLightWaveSources].y = (byte)this.cubesLightChange[num8, 1];
						this.antiLightWaveSources[this.numAntiLightWaveSources].z = (byte)this.cubesLightChange[num8, 2];
						this.numAntiLightWaveSources++;
						this.sunLight[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]] = 0;
					}
					WorldHolderScript.Vector3Int[] array = this.antiLightWaveSources;
					WorldHolderScript.Vector3Int[] array2 = this.tmpLight;
					int num18 = 0;
					while (this.numAntiLightWaveSources > 0)
					{
						num18++;
						this.numAntiLightWaveSourcesNew = 0;
						byte b2 = (byte)Mathf.Max(0, (int)this.initAntiLight - 16 * num18);
						if (num18 > 16)
						{
							break;
						}
						for (int num19 = 0; num19 < this.numAntiLightWaveSources; num19++)
						{
							if (b2 == 0)
							{
								break;
							}
							for (int num20 = 0; num20 < 6; num20++)
							{
								WorldHolderScript.Vector3Int vector3Int = array[num19];
								int num21 = (int)vector3Int.x + this.lightNeibours[num20, 0];
								int num22 = (int)vector3Int.y + this.lightNeibours[num20, 1];
								int num23 = (int)vector3Int.z + this.lightNeibours[num20, 2];
								if (this.IsInWorld(num21, num22, num23))
								{
									if (this.sunLight[num21, num22, num23] != 0)
									{
										if (this.sunLight[num21, num22, num23] > b2)
										{
											this.LightWaveSource(num21, num22, num23);
										}
										else if (this.cubeData[num21, num22, num23] != 0 || this.cubesDrawTypes[this.cubeTypes[num21, num22, num23]] != 0)
										{
											this.sunLight[num21, num22, num23] = 0;
											array2[this.numAntiLightWaveSourcesNew].x = (byte)num21;
											array2[this.numAntiLightWaveSourcesNew].y = (byte)num22;
											array2[this.numAntiLightWaveSourcesNew].z = (byte)num23;
											this.numAntiLightWaveSourcesNew++;
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
				if (this.lightR[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]] == 0 && this.lightG[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]] == 0 && this.lightB[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]] == 0)
				{
					byte b3 = 0;
					byte b4 = 0;
					byte b5 = 0;
					int num24 = 0;
					int num25 = 0;
					int num26 = 0;
					for (int num27 = 0; num27 < 6; num27++)
					{
						int x3 = this.cubesLightChange[num8, 0] + this.lightNeibours[num27, 0];
						int y3 = this.cubesLightChange[num8, 1] + this.lightNeibours[num27, 1];
						int z3 = this.cubesLightChange[num8, 2] + this.lightNeibours[num27, 2];
						if (this.IsInWorld(x3, y3, z3))
						{
							if (this.lightR[x3, y3, z3] > b3)
							{
								b3 = this.lightR[x3, y3, z3];
								num24 = num27;
							}
							if (this.lightG[x3, y3, z3] > b4)
							{
								b4 = this.lightG[x3, y3, z3];
								num25 = num27;
							}
							if (this.lightB[x3, y3, z3] > b5)
							{
								b5 = this.lightB[x3, y3, z3];
								num26 = num27;
							}
						}
					}
					if (b3 != 0)
					{
						int x4 = (int)((byte)(this.cubesLightChange[num8, 0] + this.lightNeibours[num24, 0]));
						int y4 = (int)((byte)(this.cubesLightChange[num8, 1] + this.lightNeibours[num24, 1]));
						int z4 = (int)((byte)(this.cubesLightChange[num8, 2] + this.lightNeibours[num24, 2]));
						this.LightItemWaveSources(x4, y4, z4, 255);
					}
					if (b4 != 0)
					{
						int x4 = (int)((byte)(this.cubesLightChange[num8, 0] + this.lightNeibours[num25, 0]));
						int y4 = (int)((byte)(this.cubesLightChange[num8, 1] + this.lightNeibours[num25, 1]));
						int z4 = (int)((byte)(this.cubesLightChange[num8, 2] + this.lightNeibours[num25, 2]));
						this.LightItemWaveSources(x4, y4, z4, 255);
					}
					if (b5 != 0)
					{
						int x4 = (int)((byte)(this.cubesLightChange[num8, 0] + this.lightNeibours[num26, 0]));
						int y4 = (int)((byte)(this.cubesLightChange[num8, 1] + this.lightNeibours[num26, 1]));
						int z4 = (int)((byte)(this.cubesLightChange[num8, 2] + this.lightNeibours[num26, 2]));
						this.LightItemWaveSources(x4, y4, z4, 255);
					}
				}
				else
				{
					this.numAntiLightItemWaveSources = 0;
					this.initAntiLightItem = (byte)Mathf.Max(new int[]
					{
						(int)this.lightR[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]],
						(int)this.lightG[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]],
						(int)this.lightB[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]]
					});
					this.antiLightItemWaveSources[this.numAntiLightItemWaveSources].x = (byte)this.cubesLightChange[num8, 0];
					this.antiLightItemWaveSources[this.numAntiLightItemWaveSources].y = (byte)this.cubesLightChange[num8, 1];
					this.antiLightItemWaveSources[this.numAntiLightItemWaveSources].z = (byte)this.cubesLightChange[num8, 2];
					this.numAntiLightItemWaveSources++;
					this.lightR[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]] = 0;
					this.lightG[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]] = 0;
					this.lightB[this.cubesLightChange[num8, 0], this.cubesLightChange[num8, 1], this.cubesLightChange[num8, 2]] = 0;
					int num28 = 0;
					WorldHolderScript.Vector3Int[] array4 = this.antiLightItemWaveSources;
					WorldHolderScript.Vector3Int[] array5 = this.tmpLight;
					while (this.numAntiLightItemWaveSources > 0)
					{
						num28++;
						this.numAntiLightItemWaveSourcesNew = 0;
						byte b6 = (byte)Mathf.Max(0, (int)this.initAntiLightItem - 16 * num28);
						if (num28 > 16)
						{
							break;
						}
						for (int num29 = 0; num29 < this.numAntiLightItemWaveSources; num29++)
						{
							if (b6 == 0)
							{
								break;
							}
							for (int num30 = 0; num30 < 6; num30++)
							{
								WorldHolderScript.Vector3Int vector3Int2 = array4[num29];
								int num31 = (int)vector3Int2.x + this.lightNeibours[num30, 0];
								int num32 = (int)vector3Int2.y + this.lightNeibours[num30, 1];
								int num33 = (int)vector3Int2.z + this.lightNeibours[num30, 2];
								if (this.IsInWorld(num31, num32, num33))
								{
									if (this.lightR[num31, num32, num33] != 0 || this.lightG[num31, num32, num33] != 0 || this.lightB[num31, num32, num33] != 0)
									{
										if (this.lightR[num31, num32, num33] >= b6 || this.lightG[num31, num32, num33] >= b6 || this.lightB[num31, num32, num33] >= b6)
										{
											this.LightItemWaveSources(num31, num32, num33, (int)b6);
										}
										else if (this.isLight(num31, num32, num33))
										{
											this.LightItemWaveSources(num31, num32, num33, 256);
											array5[this.numAntiLightItemWaveSourcesNew].x = (byte)num31;
											array5[this.numAntiLightItemWaveSourcesNew].y = (byte)num32;
											array5[this.numAntiLightItemWaveSourcesNew].z = (byte)num33;
											this.numAntiLightItemWaveSourcesNew++;
										}
										else if (this.cubeData[num31, num32, num33] != 0 || this.cubesDrawTypes[this.cubeTypes[num31, num32, num33]] != 0)
										{
											this.lightR[num31, num32, num33] = 0;
											this.lightG[num31, num32, num33] = 0;
											this.lightB[num31, num32, num33] = 0;
											array5[this.numAntiLightItemWaveSourcesNew].x = (byte)num31;
											array5[this.numAntiLightItemWaveSourcesNew].y = (byte)num32;
											array5[this.numAntiLightItemWaveSourcesNew].z = (byte)num33;
											this.numAntiLightItemWaveSourcesNew++;
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
	}

	public bool isLight(int nx, int ny, int nz)
	{
		return this.sunLight[nx, ny, nz] >= byte.MaxValue || this.lightR[nx, ny, nz] >= byte.MaxValue || this.lightG[nx, ny, nz] >= byte.MaxValue || this.lightB[nx, ny, nz] >= byte.MaxValue;
	}

	private void PlaceItemLight(int x, int y, int z)
	{
		this.LightItemWaveSources(x, y, z, 256);
	}

	private void ReplaceItemLight(int x, int y, int z)
	{
		this.numAntiLightItemWaveSources = 0;
		this.initAntiLightItem = (byte)Mathf.Max(new int[]
		{
			(int)this.lightR[x, y, z],
			(int)this.lightG[x, y, z],
			(int)this.lightB[x, y, z]
		});
		this.antiLightItemWaveSources[this.numAntiLightItemWaveSources].x = (byte)x;
		this.antiLightItemWaveSources[this.numAntiLightItemWaveSources].y = (byte)y;
		this.antiLightItemWaveSources[this.numAntiLightItemWaveSources].z = (byte)z;
		this.numAntiLightItemWaveSources++;
		this.lightR[x, y, z] = 0;
		this.lightG[x, y, z] = 0;
		this.lightB[x, y, z] = 0;
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
						if (this.lightR[num2, num3, num4] != 0 || this.lightG[num2, num3, num4] != 0 || this.lightB[num2, num3, num4] != 0)
						{
							if (this.lightR[num2, num3, num4] >= b && this.lightG[num2, num3, num4] >= b && this.lightB[num2, num3, num4] >= b)
							{
								this.LightItemWaveSources(num2, num3, num4, 255);
							}
							else if (this.isLight(num2, num3, num4))
							{
								this.LightItemWaveSources(num2, num3, num4, 255);
								array2[this.numAntiLightItemWaveSourcesNew].x = (byte)num2;
								array2[this.numAntiLightItemWaveSourcesNew].y = (byte)num3;
								array2[this.numAntiLightItemWaveSourcesNew].z = (byte)num4;
								this.numAntiLightItemWaveSourcesNew++;
							}
							else if (this.cubeData[num2, num3, num4] != 0 || this.cubesDrawTypes[this.cubeTypes[num2, num3, num4]] != 0)
							{
								this.blocksToChange[Mathf.FloorToInt((float)num2 / (float)this.blockSizeX), Mathf.FloorToInt((float)num3 / (float)this.blockSizeY), Mathf.FloorToInt((float)num4 / (float)this.blockSizeZ)] = true;
								this.lightR[num2, num3, num4] = 0;
								this.lightG[num2, num3, num4] = 0;
								this.lightB[num2, num3, num4] = 0;
								array2[this.numAntiLightItemWaveSourcesNew].x = (byte)num2;
								array2[this.numAntiLightItemWaveSourcesNew].y = (byte)num3;
								array2[this.numAntiLightItemWaveSourcesNew].z = (byte)num4;
								this.numAntiLightItemWaveSourcesNew++;
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
	}

	public CubePhys GetCubePhysType(Vector3 pos)
	{
		int num = Mathf.RoundToInt(pos.x);
		int num2 = Mathf.RoundToInt(pos.y);
		int num3 = Mathf.RoundToInt(pos.z);
		if (num < 0 || num2 < 0 || num3 < 0 || num >= this.sizeX || num2 >= this.sizeY || num3 >= this.sizeZ)
		{
			return CubePhys.air;
		}
		return (CubePhys)this.phys[num, num2, num3];
	}

	public CubePhys GetCubePhysType(int x, int y, int z)
	{
		if (x < 0 || y < 0 || z < 0 || x >= this.sizeX || y >= this.sizeY || z >= this.sizeZ)
		{
			return CubePhys.air;
		}
		return (CubePhys)this.phys[x, y, z];
	}

	public void RestoreBlockPhys(int bX, int bY, int bZ)
	{
		for (int i = bX * this.blockSizeX; i < (bX + 1) * this.blockSizeX; i++)
		{
			for (int j = bY * this.blockSizeY; j < (bY + 1) * this.blockSizeY; j++)
			{
				for (int k = bZ * this.blockSizeZ; k < (bZ + 1) * this.blockSizeZ; k++)
				{
					this.phys[i, j, k] = (byte)this.cubePhys[this.cubeTypes[i, j, k]];
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
			this.phys[gameItemStruct.x, gameItemStruct.y, gameItemStruct.z] = (byte)gameItemStruct.phys;
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
									this.phys[k, l, m] = 5;
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
		if (num2 == 0 && cubesToChange.Length > 2)
		{
			num2 = 4096;
		}
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
										if (this.cubeTypes[num8 * this.blockSizeX + num11, num9 * this.blockSizeY + num12, num10 * this.blockSizeZ + num13] != 0)
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
			foreach (GameObject go in this.StaticBatchCombine.Values)
			{
				this.StaticBatchingUtility_Light(go);
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
									if (this.cubeTypes[l * this.blockSizeX + num5, m * this.blockSizeY + num6, n * this.blockSizeZ + num7] != 0)
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
		if (this.cubeTypes[cubeX, cubeY, cubeZ] != 0 && typeCube == 0)
		{
			this.VisualizeDestroyCube(cubeX, cubeY, cubeZ, (ushort)this.cubeTypes[cubeX, cubeY, cubeZ], 10f / (float)numBlocksToChange);
		}
		if (typeCube == 128)
		{
			this.waterLevel[cubeX, cubeY, cubeZ] = this.maxWaterLevel;
			this.SetWaterToCheck(cubeX, cubeY, cubeZ, true);
		}
		if (this.cubeTypes[cubeX, cubeY, cubeZ] == 128 && typeCube == 0)
		{
			this.waterLevel[cubeX, cubeY, cubeZ] = 0;
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
			if (this.IsInWorld(cubeX, cubeY - 1, cubeZ - 1) && this.cubeTypes[cubeX, cubeY - 1, cubeZ] == 128)
			{
				CubeWaterGrid cubeWaterGrid2;
				CubeWaterGrid cubeWaterGrid = cubeWaterGrid2 = this.waterLevel;
				int y2;
				int y = y2 = cubeY - 1;
				byte b = cubeWaterGrid2[cubeX, y2, cubeZ];
				cubeWaterGrid[cubeX, y, cubeZ] = b - 1;
				this.SetWaterToCheck(cubeX, cubeY - 1, cubeZ, true);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			this.SetWaterToCheck(cubeX + this.lightNeibours[i, 0], cubeY + this.lightNeibours[i, 1], cubeZ + this.lightNeibours[i, 2], true);
		}
		this.cubeTypes[cubeX, cubeY, cubeZ] = (int)((ushort)typeCube);
		this.cubeData[cubeX, cubeY, cubeZ] = (byte)geom;
		this.cubesDamage[cubeX, cubeY, cubeZ] = this.cubesHealth[this.cubeTypes[cubeX, cubeY, cubeZ]];
		this.phys[cubeX, cubeY, cubeZ] = (byte)this.cubePhys[this.cubeTypes[cubeX, cubeY, cubeZ]];
		this.SetNewCubesLightChange(cubeX, cubeY, cubeZ);
		if (typeCube == 128)
		{
			this.ChangeWorldBytesCube(cubeX, cubeY, cubeZ, (ushort)this.cubeTypes[cubeX, cubeY, cubeZ], this.maxWaterLevel);
		}
		else
		{
			this.ChangeWorldBytesCube(cubeX, cubeY, cubeZ, (ushort)this.cubeTypes[cubeX, cubeY, cubeZ], (byte)geom);
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
			if (this.cubePhys[this.cubeTypes[num4, num5, num6]] != CubePhys.air)
			{
				this.cubesDamage[num4, num5, num6] = (int)((byte)num7);
				if (this.cubesDamage[num4, num5, num6] <= 0)
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
		if (this.cubePhys[(int)type] == CubePhys.air)
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
			if ((int)type < Kube.WHS.miniCubesMat.Length)
			{
				gameObject.GetComponent<Renderer>().sharedMaterial = Kube.WHS.miniCubesMat[(int)type];
			}
		}
	}

	public GameObject CreateGameItem(int numItem, byte rotation, int x, int y, int z, int state, int id, bool redraw = true)
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
			return null;
		}
		ItemPropsScript component = gameObject.GetComponent<ItemPropsScript>();
		if (component.buildMagic || component.magic)
		{
			UnityEngine.Debug.LogError("Create magic from file " + numItem);
			return null;
		}
		item.gameObject = (UnityEngine.Object.Instantiate(gameObject, new Vector3((float)x, (float)y, (float)z), rotation2) as GameObject);
		item.rotation = rotation;
		item.x = x;
		item.y = y;
		item.z = z;
		item.numItem = (int)((byte)numItem);
		if (this.itemToCube[numItem] != 0)
		{
			this.cubeTypes[item.x, item.y, item.z] = (int)((ushort)this.itemToCube[numItem]);
		}
		item.phys = item.gameObject.GetComponent<ItemPropsScript>().physType;
		item.id = id;
		item.gameObject.GetComponent<ItemPropsScript>().id = id;
		item.gameObject.GetComponent<ItemPropsScript>().type = numItem;
		item.lightColor = item.gameObject.GetComponent<ItemPropsScript>().lightColor;
		this.gameItems.Add(item);
		item.gameObject.SendMessage("ChangeItemState", state, SendMessageOptions.DontRequireReceiver);
		this.ChangeWorldBytesItem(x, y, z, (int)((byte)numItem), (byte)((int)rotation + state * 6));
		int bX = Mathf.FloorToInt((float)x / (float)this.blockSizeX);
		int bY = Mathf.FloorToInt((float)y / (float)this.blockSizeY);
		int bZ = Mathf.FloorToInt((float)z / (float)this.blockSizeZ);
		this.RestoreBlockPhys(bX, bY, bZ);
		this.RecalculatePhys();
		if (item.lightColor.grayscale > 0f && redraw)
		{
			this.lightR[x, y, z] = (byte)(255f * item.lightColor.r);
			this.lightG[x, y, z] = (byte)(255f * item.lightColor.g);
			this.lightB[x, y, z] = (byte)(255f * item.lightColor.b);
			this.PlaceItemLight(x, y, z);
			this.RedrawWorld(false, true, false);
		}
		return item.gameObject;
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
				this.cubeTypes[gameItemStruct.x, gameItemStruct.y, gameItemStruct.z] = 0;
				if (gameItemStruct.lightColor.grayscale > 0f)
				{
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
				if (this.itemToCube[value.numItem] != 0)
				{
					this.cubeTypes[value.x, value.y, value.z] = 0;
				}
				if (value.lightColor.grayscale > 0f)
				{
					this.ReplaceItemLight(value.x, value.y, value.z);
					this.RedrawWorld(false, true, false);
				}
				this.ChangeWorldBytesItem(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), Mathf.RoundToInt(newPos.z), value.numItem, value.rotation + value.state * 6);
				value.x = Mathf.RoundToInt(newPos.x);
				value.y = Mathf.RoundToInt(newPos.y);
				value.z = Mathf.RoundToInt(newPos.z);
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
					this.lightR[Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), Mathf.RoundToInt(newPos.z)] = (byte)(255f * value.lightColor.r);
					this.lightG[Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), Mathf.RoundToInt(newPos.z)] = (byte)(255f * value.lightColor.g);
					this.lightB[Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), Mathf.RoundToInt(newPos.z)] = (byte)(255f * value.lightColor.b);
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

	public int FindGameItemId(GameObject go)
	{
		if (this.gameItems == null)
		{
			return -1;
		}
		for (int i = 0; i < this.gameItems.Count; i++)
		{
			if (this.gameItems[i].gameObject == go)
			{
				return this.gameItems[i].id;
			}
		}
		return -1;
	}

	public int FindGameItemType(GameObject go)
	{
		if (this.gameItems == null)
		{
			return -1;
		}
		for (int i = 0; i < this.gameItems.Count; i++)
		{
			if (this.gameItems[i].gameObject == go)
			{
				return this.gameItems[i].numItem;
			}
		}
		return -1;
	}

	public GameObject FindGameItem(int id)
	{
		if (this.gameItems == null)
		{
			return null;
		}
		for (int i = 0; i < this.gameItems.Count; i++)
		{
			if (this.gameItems[i].id == id)
			{
				return this.gameItems[i].gameObject;
			}
		}
		return null;
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
				GameItemStruct value = this.gameItems[i];
				if (value.id == id)
				{
					value.gameObject.BroadcastMessage("ChangeItemState", newState, SendMessageOptions.DontRequireReceiver);
					value.state = (byte)newState;
					this.ChangeWorldBytesItem(value.x, value.y, value.z, value.numItem, value.rotation + value.state * 6);
					this.gameItems[i] = value;
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
		binaryWriter.Write(2564);
		memoryStream.WriteByte((byte)this.sizeX);
		memoryStream.WriteByte((byte)this.sizeY);
		memoryStream.WriteByte((byte)this.sizeZ);
		memoryStream.WriteByte((byte)this.skybox);
		this._cubegrid.save(memoryStream);
		List<GameItemStruct> list = new List<GameItemStruct>();
		for (int i = 0; i < this.gameItems.Count; i++)
		{
			if (!(this.gameItems[i].gameObject == null))
			{
				GameMapItem component = this.gameItems[i].gameObject.GetComponent<GameMapItem>();
				if (!(component == null))
				{
					list.Add(this.gameItems[i]);
				}
			}
		}
		binaryWriter.Write(list.Count);
		for (int j = 0; j < list.Count; j++)
		{
			GameMapItem component2 = list[j].gameObject.GetComponent<GameMapItem>();
			binaryWriter.Write((ushort)list[j].numItem);
			binaryWriter.Write((byte)list[j].x);
			binaryWriter.Write((byte)list[j].y);
			binaryWriter.Write((byte)list[j].z);
			binaryWriter.Write(list[j].rotation + list[j].state * 6);
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

	public bool ready { get; set; }

	public IEnumerator LoadWorld(byte[] newWorldData)
	{
		this.LoadWorldResult = 1;
		MonoBehaviour.print("LoadWorld length: " + newWorldData.Length);
		if (newWorldData[0] == 137 && newWorldData[1] == 80)
		{
			yield return base.StartCoroutine(this._LoadWorldOld(newWorldData));
			yield break;
		}
		newWorldData = ZlibStream.UncompressBuffer(newWorldData);
		yield return 1;
		MemoryStream ms = new MemoryStream(newWorldData);
		BinaryReader br = new BinaryReader(ms);
		ms.Position = 0L;
		int sig = (int)br.ReadUInt16();
		int version = 0;
		switch (sig)
		{
		case 2561:
			version = 1;
			break;
		case 2562:
			version = 2;
			break;
		case 2563:
			version = 3;
			break;
		case 2564:
			version = 4;
			break;
		default:
			yield return base.StartCoroutine(this._LoadWorldOld(newWorldData));
			yield break;
		}
		this.sizeX = ms.ReadByte();
		this.sizeY = ms.ReadByte();
		this.sizeZ = ms.ReadByte();
		this.Init(this.sizeX, this.sizeY, this.sizeZ, true);
		if (version > 3)
		{
			this.skybox = ms.ReadByte();
		}
		this._cubegrid.load(ms);
		for (int x = 0; x < this.sizeX; x++)
		{
			for (int y = 0; y < this.sizeY; y++)
			{
				for (int z = 0; z < this.sizeZ; z++)
				{
					int type = this._cubegrid.get(x, y, z);
					byte prop = this._cubegrid.getdata(x, y, z);
					if (version == 1 && (type & 3840) != 0 && (type < 275 || type > 281))
					{
						type = 0;
						this._cubegrid.set(x, y, z, 0, 0);
					}
					this.cubeTypes[x, y, z] = (int)((ushort)type);
					this.cubeData[x, y, z] = prop;
					int gameitemtype = -1;
					if (version < 3 && type >= 155)
					{
						gameitemtype = type - 155;
						if (type < this.cubeToItem.Length)
						{
							if (this.cubeToItem[type] == -1)
							{
								this.cubeTypes[x, y, z] = 0;
								this.cubeData[x, y, z] = 0;
								this._cubegrid.set(x, y, z, 0, 0);
							}
							else
							{
								this.cubeTypes[x, y, z] = (int)((ushort)this.itemToCube[gameitemtype]);
							}
						}
						else
						{
							this.cubeTypes[x, y, z] = 0;
							this.cubeData[x, y, z] = 0;
							this._cubegrid.set(x, y, z, 0, 0);
						}
					}
					else
					{
						gameitemtype = ((Kube.OH.blockTypes[type].type != 1) ? -1 : Kube.OH.blockTypes[type].itemId);
					}
					if (Kube.OH.blockTypes[type].type == 2)
					{
						gameitemtype = Kube.OH.blockTypes[type].itemId;
					}
					if (gameitemtype == -1)
					{
						this.cubesDamage[x, y, z] = this.cubesHealth[this.cubeTypes[x, y, z]];
						this.phys[x, y, z] = (byte)this.cubePhys[this.cubeTypes[x, y, z]];
						if (this.cubeTypes[x, y, z] == 128)
						{
							this.waterLevel[x, y, z] = prop;
						}
					}
					else
					{
						byte rotation = prop % 6;
						byte state = (prop - rotation) / 6;
						this.CreateGameItem(gameitemtype, rotation, x, y, z, (int)state, x + z * 256 + y * 256 * 256, false);
					}
				}
			}
		}
		int nItems = br.ReadInt32();
		for (int i = 0; i < nItems; i++)
		{
			int q = 0;
			int id;
			if (version >= 3)
			{
				int type2 = (int)br.ReadUInt16();
				int x2 = (int)br.ReadByte();
				int y2 = (int)br.ReadByte();
				int z2 = (int)br.ReadByte();
				int prop2 = (int)br.ReadByte();
				byte rotation2 = (byte)(prop2 % 6);
				byte state2 = (byte)((prop2 - (int)rotation2) / 6);
				id = x2 + z2 * 256 + y2 * 256 * 256;
				this.CreateGameItem(type2, rotation2, x2, y2, z2, (int)state2, id, false);
				yield return 1;
				q = this.gameItems.Count - 1;
			}
			else
			{
				id = br.ReadInt32();
			}
			int len = (int)br.ReadByte();
			KubeStream ks = new KubeStream(br.ReadBytes(len));
			while (q < this.gameItems.Count)
			{
				if (this.gameItems[q].id == id)
				{
					GameMapItem gmi = this.gameItems[q].gameObject.GetComponent<GameMapItem>();
					gmi.LoadMap(ks);
					yield return 1;
					break;
				}
				q++;
			}
		}
		nItems = br.ReadInt32();
		for (int j = 0; j < nItems; j++)
		{
			int numItem = (int)br.ReadUInt16();
			GameMapItem gmi2 = this._CreateNewMagic(numItem);
			int len2 = (int)br.ReadByte();
			KubeStream ks2 = new KubeStream(br.ReadBytes(len2));
			gmi2.LoadMap(ks2);
			yield return 1;
		}
		for (int k = 0; k < this.monsterRespawnS.Length; k++)
		{
			if (this.monsterRespawnS[k])
			{
				this.monsterRespawnS[k].monsterLastDieTime = -999999f;
			}
		}
		for (int l = 0; l < this.transportRespawnS.Length; l++)
		{
			this.transportLastDieTime[l] = -999999f;
		}
		this.GenerateBounds();
		this.needSaveMap = false;
		if (this.numQueuedChanges != 0)
		{
			for (int m = 0; m < this.numQueuedChanges; m++)
			{
				if (this.queuedChanges[m, 0] == "ChangeCubes")
				{
					this.ChangeCubes(this.queuedChanges[m, 1], false, false);
				}
				else if (this.queuedChanges[m, 0] == "ChangeCubesHealth")
				{
					this.ChangeCubesHealth(this.queuedChanges[m, 1]);
				}
			}
		}
		this.LoadWorldResult = 0;
		yield break;
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

	public IEnumerator _LoadWorldOld(byte[] newWorldData)
	{
		Color32[] worldBytes = null;
		if (newWorldData[0] != 137 || newWorldData[1] != 80)
		{
			worldBytes = this.DecodeFromBytes(newWorldData);
			this.containerSize = worldBytes.Length;
			if (worldBytes.Length == 1048576)
			{
				MonoBehaviour.print("small map");
				this.Init(128, 96, 128, false);
			}
			else
			{
				if (worldBytes.Length != 4194304)
				{
					MonoBehaviour.print("Bad map size");
					this.LoadWorldResult = 1;
					yield break;
				}
				MonoBehaviour.print("big map");
				this.Init(224, 125, 224, false);
			}
		}
		else
		{
			Texture2D worldPackTex = new Texture2D(4, 4);
			worldPackTex.LoadImage(newWorldData);
			this.containerSize = worldPackTex.width * worldPackTex.height;
			if (worldPackTex.width == 1024 && worldPackTex.height == 1024)
			{
				MonoBehaviour.print("small map");
				this.Init(128, 96, 128, false);
			}
			else
			{
				if (worldPackTex.width != 2048 || worldPackTex.height != 2048)
				{
					this._GenerateWorld(worldPackTex);
					this.LoadWorldResult = 0;
					yield break;
				}
				MonoBehaviour.print("big map");
				this.Init(224, 125, 224, false);
			}
			worldBytes = worldPackTex.GetPixels32();
			UnityEngine.Object.Destroy(worldPackTex);
		}
		this.RemoveAllGameItems();
		int numNotAir = 0;
		int numGameItems = 0;
		for (int x = 0; x < this.sizeX; x++)
		{
			for (int y = 0; y < this.sizeY; y++)
			{
				for (int z = 0; z < this.sizeZ; z++)
				{
					int place = Mathf.FloorToInt((float)(x + z * this.sizeX + y * this.sizeX * this.sizeZ) / 4f);
					int placeInColor = (x + z * this.sizeX + y * this.sizeX * this.sizeZ) % 4;
					byte type = 0;
					byte prop = 0;
					if (placeInColor == 0)
					{
						type = worldBytes[place].r;
						prop = worldBytes[place + this.containerSize / 2].r;
					}
					else if (placeInColor == 1)
					{
						type = worldBytes[place].g;
						prop = worldBytes[place + this.containerSize / 2].g;
					}
					else if (placeInColor == 2)
					{
						type = worldBytes[place].b;
						prop = worldBytes[place + this.containerSize / 2].b;
					}
					else if (placeInColor == 3)
					{
						type = worldBytes[place].a;
						prop = worldBytes[place + this.containerSize / 2].a;
					}
					this.cubeTypes[x, y, z] = (int)type;
					if (prop != 0 && type < 155 && type != 128)
					{
						prop = 0;
					}
					this._cubegrid.set(x, y, z, (int)type, (int)prop);
					this.cubeData[x, y, z] = prop;
					if (type < 155)
					{
						this.cubesDamage[x, y, z] = this.cubesHealth[this.cubeTypes[x, y, z]];
						this.phys[x, y, z] = (byte)this.cubePhys[this.cubeTypes[x, y, z]];
						if (this.cubeTypes[x, y, z] == 128)
						{
							this.waterLevel[x, y, z] = prop;
						}
						if (type != 0)
						{
							numNotAir++;
						}
					}
					else
					{
						numGameItems++;
						byte rotation = prop % 6;
						byte state = (prop - rotation) / 6;
						this.CreateGameItem((int)(type - 155), prop % 6, x, y, z, (int)state, x + z * 256 + y * 256 * 256, false);
						yield return 1;
						if (this.itemToCube[(int)(type - 155)] != 0)
						{
							type = (byte)this.itemToCube[(int)(type - 155)];
						}
						else
						{
							type = 0;
							prop = 0;
						}
						this._cubegrid.set(x, y, z, (int)type, (int)prop);
						this.cubeTypes[x, y, z] = (int)type;
					}
				}
			}
		}
		MonoBehaviour.print("NumGameItems: " + numGameItems);
		int numAAs = 0;
		for (int i = 0; i < this.AAgo.Length; i++)
		{
			byte[] AAbytes = new byte[30];
			for (int j = 0; j < 30; j++)
			{
				int place2 = Mathf.FloorToInt((float)(12 * this.containerSize / 8 + i * 30 + j) / 4f);
				int placeInColor2 = (12 * this.containerSize / 8 + i * 30 + j) % 4;
				if (placeInColor2 == 0)
				{
					AAbytes[j] = worldBytes[place2].r;
				}
				else if (placeInColor2 == 1)
				{
					AAbytes[j] = worldBytes[place2].g;
				}
				else if (placeInColor2 == 2)
				{
					AAbytes[j] = worldBytes[place2].b;
				}
				else if (placeInColor2 == 3)
				{
					AAbytes[j] = worldBytes[place2].a;
				}
			}
			if (AAbytes[0] != 0 || AAbytes[1] != 0 || AAbytes[2] != 0 || AAbytes[3] != 0 || AAbytes[4] != 0 || AAbytes[5] != 0)
			{
				this.AAgo[i] = null;
				numAAs++;
				this.CreateNewAA((int)AAbytes[0], (int)AAbytes[1], (int)AAbytes[2], (int)AAbytes[3], (int)AAbytes[4], (int)AAbytes[5], (int)AAbytes[6], (int)AAbytes[7], (int)AAbytes[8], (int)AAbytes[9], (int)AAbytes[10], (int)AAbytes[11], (int)AAbytes[12], (int)AAbytes[13], i);
				yield return 1;
			}
		}
		int numTriggers = 0;
		for (int k = 0; k < this.triggerS.Length; k++)
		{
			byte[] triggerBytes = new byte[15];
			this.triggerS[k] = null;
			for (int l = 0; l < triggerBytes.Length; l++)
			{
				int place3 = Mathf.FloorToInt((float)(12 * this.containerSize / 8 + k * 15 + l + 30720) / 4f);
				int placeInColor3 = (12 * this.containerSize / 8 + k * 15 + l + 30720) % 4;
				if (placeInColor3 == 0)
				{
					triggerBytes[l] = worldBytes[place3].r;
				}
				else if (placeInColor3 == 1)
				{
					triggerBytes[l] = worldBytes[place3].g;
				}
				else if (placeInColor3 == 2)
				{
					triggerBytes[l] = worldBytes[place3].b;
				}
				else if (placeInColor3 == 3)
				{
					triggerBytes[l] = worldBytes[place3].a;
				}
			}
			if (triggerBytes[0] != 0 || triggerBytes[1] != 0 || triggerBytes[2] != 0)
			{
				for (int q = 0; q < this.gameItems.Count; q++)
				{
					GameItemStruct gis = this.gameItems[q];
					if (gis.x == (int)triggerBytes[0] && gis.y == (int)triggerBytes[1] && gis.z == (int)triggerBytes[2])
					{
						this.triggerS[k] = gis.gameObject.GetComponent<TriggerScript>();
						break;
					}
				}
				if (this.triggerS[k])
				{
					this.SaveTrigger((int)triggerBytes[0], (int)triggerBytes[1], (int)triggerBytes[2], (int)triggerBytes[3], (int)triggerBytes[4], (int)triggerBytes[5], (int)triggerBytes[6], (int)triggerBytes[7], k);
					numTriggers++;
				}
			}
		}
		int numWires = 0;
		for (int m = 0; m < this.wireS.Length; m++)
		{
			byte[] wireBytes = new byte[10];
			for (int n = 0; n < 10; n++)
			{
				int place4 = Mathf.FloorToInt((float)(12 * this.containerSize / 8 + 30720 + 15360 + m * 10 + n) / 4f);
				int placeInColor4 = (12 * this.containerSize / 8 + 30720 + 15360 + m * 10 + n) % 4;
				if (placeInColor4 == 0)
				{
					wireBytes[n] = worldBytes[place4].r;
				}
				else if (placeInColor4 == 1)
				{
					wireBytes[n] = worldBytes[place4].g;
				}
				else if (placeInColor4 == 2)
				{
					wireBytes[n] = worldBytes[place4].b;
				}
				else if (placeInColor4 == 3)
				{
					wireBytes[n] = worldBytes[place4].a;
				}
			}
			if (wireBytes[3] != 0)
			{
				this.wireS[m] = null;
				numWires++;
				this.CreateNewWire((int)wireBytes[0], (int)wireBytes[1], (int)wireBytes[2], (int)(wireBytes[3] - 1), (int)wireBytes[4], (int)wireBytes[5], (int)wireBytes[6], m);
			}
		}
		int numMonsterRespawn = 0;
		for (int k2 = 0; k2 < this.monsterRespawnS.Length; k2++)
		{
			byte[] monsterRespawnBytes = new byte[10];
			this.monsterRespawnS[k2] = null;
			for (int i2 = 0; i2 < monsterRespawnBytes.Length; i2++)
			{
				int place5 = Mathf.FloorToInt((float)(12 * this.containerSize / 8 + k2 * 10 + i2 + 30720 + 15360 + 20480) / 4f);
				int placeInColor5 = (12 * this.containerSize / 8 + k2 * 10 + i2 + 30720 + 15360 + 20480) % 4;
				if (placeInColor5 == 0)
				{
					monsterRespawnBytes[i2] = worldBytes[place5].r;
				}
				else if (placeInColor5 == 1)
				{
					monsterRespawnBytes[i2] = worldBytes[place5].g;
				}
				else if (placeInColor5 == 2)
				{
					monsterRespawnBytes[i2] = worldBytes[place5].b;
				}
				else if (placeInColor5 == 3)
				{
					monsterRespawnBytes[i2] = worldBytes[place5].a;
				}
			}
			if (monsterRespawnBytes[0] != 0 || monsterRespawnBytes[1] != 0 || monsterRespawnBytes[2] != 0)
			{
				for (int q2 = 0; q2 < this.gameItems.Count; q2++)
				{
					GameItemStruct gis2 = this.gameItems[q2];
					if (!(gis2.gameObject.GetComponent<MonsterRespawnScript>() == null))
					{
						bool flagNoSuchMonsterRespawn = true;
						for (int i3 = 0; i3 < this.monsterRespawnS.Length; i3++)
						{
							if (this.monsterRespawnS[k2])
							{
								if (this.monsterRespawnS[k2].x == (int)monsterRespawnBytes[0] && this.monsterRespawnS[k2].y == (int)monsterRespawnBytes[1] && this.monsterRespawnS[k2].z == (int)monsterRespawnBytes[2])
								{
									flagNoSuchMonsterRespawn = false;
									break;
								}
							}
						}
						if (flagNoSuchMonsterRespawn && gis2.x == (int)monsterRespawnBytes[0] && gis2.y == (int)monsterRespawnBytes[1] && gis2.z == (int)monsterRespawnBytes[2])
						{
							this.monsterRespawnS[k2] = gis2.gameObject.GetComponent<MonsterRespawnScript>();
							break;
						}
					}
				}
				if (this.monsterRespawnS[k2])
				{
					this.SaveMonsterRespawn((int)monsterRespawnBytes[0], (int)monsterRespawnBytes[1], (int)monsterRespawnBytes[2], (int)monsterRespawnBytes[3], (int)monsterRespawnBytes[4], (int)monsterRespawnBytes[5], (int)monsterRespawnBytes[6], (int)monsterRespawnBytes[7], k2);
					numMonsterRespawn++;
				}
			}
		}
		int numTransportRespawn = 0;
		for (int k3 = 0; k3 < this.transportRespawnS.Length; k3++)
		{
			byte[] transportRespawnBytes = new byte[10];
			this.transportRespawnS[k3] = null;
			for (int i4 = 0; i4 < transportRespawnBytes.Length; i4++)
			{
				int place6 = Mathf.FloorToInt((float)(12 * this.containerSize / 8 + k3 * 10 + i4 + 30720 + 15360 + 20480 + 10240) / 4f);
				int placeInColor6 = (12 * this.containerSize / 8 + k3 * 10 + i4 + 30720 + 15360 + 20480 + 10240) % 4;
				if (placeInColor6 == 0)
				{
					transportRespawnBytes[i4] = worldBytes[place6].r;
				}
				else if (placeInColor6 == 1)
				{
					transportRespawnBytes[i4] = worldBytes[place6].g;
				}
				else if (placeInColor6 == 2)
				{
					transportRespawnBytes[i4] = worldBytes[place6].b;
				}
				else if (placeInColor6 == 3)
				{
					transportRespawnBytes[i4] = worldBytes[place6].a;
				}
			}
			if (transportRespawnBytes[0] != 0 || transportRespawnBytes[1] != 0 || transportRespawnBytes[2] != 0)
			{
				for (int q3 = 0; q3 < this.gameItems.Count; q3++)
				{
					GameItemStruct gis3 = this.gameItems[q3];
					bool flagNoSuchTransportRespawn = true;
					for (int i5 = 0; i5 < this.transportRespawnS.Length; i5++)
					{
						if (this.transportRespawnS[k3])
						{
							if (this.transportRespawnS[k3].x == (int)transportRespawnBytes[0] && this.transportRespawnS[k3].y == (int)transportRespawnBytes[1] && this.transportRespawnS[k3].z == (int)transportRespawnBytes[2])
							{
								flagNoSuchTransportRespawn = false;
								break;
							}
						}
					}
					if (flagNoSuchTransportRespawn && gis3.x == (int)transportRespawnBytes[0] && gis3.y == (int)transportRespawnBytes[1] && gis3.z == (int)transportRespawnBytes[2])
					{
						this.transportRespawnS[k3] = gis3.gameObject.GetComponent<TransportRespawnScript>();
						break;
					}
				}
				if (this.transportRespawnS[k3])
				{
					this.SaveTransportRespawn((int)transportRespawnBytes[0], (int)transportRespawnBytes[1], (int)transportRespawnBytes[2], (int)transportRespawnBytes[3], (int)transportRespawnBytes[4], (int)transportRespawnBytes[5], (int)transportRespawnBytes[6], (int)transportRespawnBytes[7], k3);
					numTransportRespawn++;
				}
				else
				{
					this.SaveTransportRespawn(0, 0, 0, 0, 0, 0, 0, 0, k3);
				}
			}
		}
		for (int k4 = 0; k4 < this.monsterRespawnS.Length; k4++)
		{
			if (this.monsterRespawnS[k4])
			{
				this.monsterRespawnS[k4].monsterLastDieTime = -999999f;
			}
		}
		for (int k5 = 0; k5 < this.transportRespawnS.Length; k5++)
		{
			this.transportLastDieTime[k5] = -999999f;
		}
		this.GenerateBounds();
		if (numNotAir < 50)
		{
			this.LoadWorldResult = 1;
			yield break;
		}
		this.needSaveMap = false;
		if (this.numQueuedChanges != 0)
		{
			for (int k6 = 0; k6 < this.numQueuedChanges; k6++)
			{
				if (this.queuedChanges[k6, 0] == "ChangeCubes")
				{
					this.ChangeCubes(this.queuedChanges[k6, 1], false, false);
				}
				else if (this.queuedChanges[k6, 0] == "ChangeCubesHealth")
				{
					this.ChangeCubesHealth(this.queuedChanges[k6, 1]);
				}
			}
		}
		this.LoadWorldResult = 0;
		yield break;
	}

	public CubeDrawTypes GetCubeDraw(int x, int y, int z)
	{
		if (x < 0 || x >= this.sizeX || y < 0 || y >= this.sizeY || z < 0 || z >= this.sizeZ)
		{
			return CubeDrawTypes.transparent;
		}
		return (CubeDrawTypes)this.cubesDrawTypes[this.cubeTypes[x, y, z]];
	}

	public ushort GetCubeFill(int x, int y, int z)
	{
		if (x < 0 || x >= this.sizeX || y < 0 || y >= this.sizeY || z < 0 || z >= this.sizeZ)
		{
			return 55;
		}
		return (ushort)this.cubeTypes[x, y, z];
	}

	public byte GetCubeData(int x, int y, int z)
	{
		if (x < 0 || x >= this.sizeX || y < 0 || y >= this.sizeY || z < 0 || z >= this.sizeZ)
		{
			return 0;
		}
		return this.cubeData[x, y, z];
	}

	public void PlayCubeHit(Vector3 pos, SoundHitType sht)
	{
		int num = this.cubeTypes[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z)];
		int num2 = this.cubeToItem[num];
		if (num2 == -1)
		{
			if (Kube.OH.cubesSound.Length > num)
			{
				Kube.OH.PlayMaterialSound(Kube.OH.cubesSound[num], sht, pos, 1f);
			}
		}
		else
		{
			Kube.OH.PlayMaterialSound(Kube.IS.gameItemsGO[num2].GetComponent<ItemPropsScript>().soundMaterialType, sht, pos, 1f);
		}
	}

	public void PlayCubeSparks(Vector3 posCube, Vector3 pos, Vector3 normal, SoundHitType sht)
	{
		int num = this.cubeTypes[Mathf.RoundToInt(posCube.x), Mathf.RoundToInt(posCube.y), Mathf.RoundToInt(posCube.z)];
		if (num >= Kube.OH.cubesSound.Length)
		{
			num = 0;
		}
		Kube.OH.PlayerSparks(Kube.OH.cubesSound[num], sht, pos, normal);
	}

	public int DayLight
	{
		get
		{
			return this.currentSunLight;
		}
	}

	public void SetDayLight(float tLight, bool redraw = true)
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
			this.blendedSkybox.SetFloat("_Blend", 1f - tLight);
			this.currentSunLight = num;
			if (redraw)
			{
				this.RedrawWorld(true, true, false);
			}
		}
	}

	private Color32 GetWorldLightInCube(int x, int y, int z)
	{
		Color32 result = default(Color32);
		if (!this.IsInWorld(x, y, z))
		{
			return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
		}
		result.r = (byte)Mathf.Min(255f, (float)this.sunLight[x, y, z] * this.sunInt * this.sunR + (float)this.sunLight[x, y, z] * (1f - this.sunInt) * this.moonR + (float)this.lightR[x, y, z]);
		result.g = (byte)Mathf.Min(255f, (float)this.sunLight[x, y, z] * this.sunInt * this.sunG + (float)this.sunLight[x, y, z] * (1f - this.sunInt) * this.moonG + (float)this.lightG[x, y, z]);
		result.b = (byte)Mathf.Min(255f, (float)this.sunLight[x, y, z] * this.sunInt * this.sunB + (float)this.sunLight[x, y, z] * (1f - this.sunInt) * this.moonB + (float)this.lightB[x, y, z]);
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

	public bool KubeTrace(Vector3 from, Vector3 dist)
	{
		float magnitude = dist.magnitude;
		return this.KubeTrace(from, dist, magnitude) == magnitude;
	}

	public float KubeTrace(Vector3 from, Vector3 dist, float magnitude)
	{
		Vector3 vector = from;
		int num = (int)magnitude * 2;
		int num2 = (int)vector.x;
		int num3 = (int)vector.y;
		int num4 = (int)vector.z;
		CubePhys cubePhys = CubePhys.air;
		byte b = 0;
		for (int i = 0; i < num; i++)
		{
			int num5 = (int)vector.x;
			int num6 = (int)vector.y;
			int num7 = (int)vector.z;
			if (num5 < 0 || num6 < 0 || num7 < 0 || num5 >= this.sizeX || num6 >= this.sizeY || num7 >= this.sizeZ)
			{
				return (float)(i >> 1);
			}
			if (num5 != num2 || num5 != num3 || num7 != num4)
			{
				cubePhys = this.GetCubePhysType(vector);
				b = this.prop[num5, num6, num7];
				num2 = num5;
				num3 = num6;
				num4 = num7;
			}
			if (cubePhys != CubePhys.air && cubePhys != CubePhys.water)
			{
				return (float)(i << 1);
			}
			if (b == 1)
			{
				return (float)(i << 1);
			}
			vector += dist;
		}
		return magnitude;
	}

	public float KubeTraceScreen(Vector3 from, Vector3 dist, float magnitude)
	{
		Vector3 a = from;
		Vector3 b = dist * 0.5f;
		int num = (int)magnitude * 2;
		int num2 = -1;
		int num3 = -1;
		int num4 = -1;
		int num5 = 0;
		int i = 0;
		while (i < num)
		{
			int num6 = Mathf.RoundToInt(a.x);
			int num7 = Mathf.RoundToInt(a.y);
			int num8 = Mathf.RoundToInt(a.z);
			if (num6 < 0 || num7 < 0 || num8 < 0 || num6 >= this.sizeX || num7 >= this.sizeY || num8 >= this.sizeZ)
			{
				return (float)(i >> 1);
			}
			if (num6 == num2 && num7 == num3 && num8 == num4)
			{
				goto IL_194;
			}
			num2 = num6;
			num3 = num7;
			num4 = num8;
			int num9 = num6 >> 4;
			int num10 = num7 >> 4;
			int num11 = num8 >> 4;
			Chunk chunk = this._cubegrid.chunks[num9, num10, num11];
			if (chunk != null)
			{
				if (chunk.type != null)
				{
					int num12 = num6 & 15;
					int num13 = num7 & 15;
					int num14 = num8 & 15;
					int num15 = num12 | num14 << 4 | num13 << 8;
					if (chunk.xtype != null)
					{
						int num16 = num15 & 1;
						int num17 = num15 >> 1;
						if (num16 == 0)
						{
							num5 = ((int)(chunk.xtype[num17] & 15) << 8 | (int)chunk.type[num15]);
						}
						else
						{
							num5 = ((int)(chunk.xtype[num17] & 240) << 4 | (int)chunk.type[num15]);
						}
						goto IL_194;
					}
					num5 = (int)chunk.type[num15];
					goto IL_194;
				}
			}
			IL_1A8:
			a += b;
			i++;
			continue;
			IL_194:
			if (this.cubesDrawTypes[num5] == 0)
			{
				return (float)(i >> 1);
			}
			goto IL_1A8;
		}
		return magnitude;
	}

	public GameObject blockPrefab;

	public CubeTypesGrid cubeTypes;

	public CubeDataGrid cubeData;

	public CubeWaterGrid waterLevel;

	public QuadData phys;

	public QuadData prop;

	public LightData sunLight;

	public LightData lightR;

	public LightData lightG;

	public LightData lightB;

	[NonSerialized]
	public int skybox;

	protected CubeGrid _cubegrid;

	protected CubeTypes _cubeTypes;

	public List<GameItemStruct> gameItems;

	public List<MagicItemStruct> magicItems;

	public BlockScript[,,] blocks;

	public Dictionary<string, GameObject> StaticBatchCombine = new Dictionary<string, GameObject>();

	public BitData isOccupied;

	private int containerSize;

	private bool needSaveMap;

	private int numCubesLightChange;

	private int[,] cubesLightChange;

	protected int[] itemToCube;

	protected int[] cubeToItem;

	[NonSerialized]
	public int[] cubesHealth;

	[NonSerialized]
	public Material[] miniCubesMat;

	public WorldHolderScript.CubesHealth cubesDamage = new WorldHolderScript.CubesHealth();

	private WireScript[] wireS;

	private GameObject[] AAgo;

	[NonSerialized]
	public TriggerScript[] triggerS;

	public MonsterRespawnScript[] monsterRespawnS;

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

	[NonSerialized]
	public int nBlocksX;

	[NonSerialized]
	public int nBlocksY;

	[NonSerialized]
	public int nBlocksZ;

	public int[] cubesDrawTypes;

	public CubePhys[] cubePhys;

	public int[,] cubesSidesTex;

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

	private int numAntiLightWaveSources;

	private int numAntiLightWaveSourcesNew;

	private byte initAntiLight;

	private WorldHolderScript.Vector3Int[] antiLightWaveSources;

	private int numAntiLightItemWaveSources;

	private int numAntiLightItemWaveSourcesNew;

	private byte initAntiLightItem;

	private WorldHolderScript.Vector3Int[] antiLightItemWaveSources;

	private bool isFirstUpdateWaterBuffer = true;

	private WorldHolderScript.Vector3Int[] updateWaterBuffer1;

	private WorldHolderScript.Vector3Int[] updateWaterBuffer2;

	private int numUpdateWaterBuffer1;

	private int numUpdateWaterBuffer2;

	private bool[,,] blocksToChange;

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

	public Material blendedSkybox;

	[NonSerialized]
	public int LoadWorldResult;

	public struct CubeDamage
	{
		public int cube;

		public int health;
	}

	public class CubesHealth
	{
		public int this[int x, int y, int z]
		{
			get
			{
				int num = Kube.WHS.cubeTypes[x, y, z];
				int num2 = x | y << 8 | z << 16;
				if (num2 == this.lastCube)
				{
					return this.lastHealth;
				}
				for (int i = 0; i < 8; i++)
				{
					if (this.stack[i].cube == num2)
					{
						return this.stack[i].health;
					}
				}
				return Kube.WHS.cubesHealth[num];
			}
			set
			{
				int num = x | y << 8 | z << 16;
				this.lastCube = num;
				this.lastHealth = value;
				for (int i = 0; i < 8; i++)
				{
					if (this.stack[i].cube == num)
					{
						this.stack[i].health = value;
						return;
					}
				}
				this.stack[this.index].cube = num;
				this.stack[this.index].health = value;
				this.index = (this.index + 1) % 8;
			}
		}

		public int lastCube;

		public int lastHealth = 100;

		protected WorldHolderScript.CubeDamage[] stack = new WorldHolderScript.CubeDamage[8];

		protected int index;
	}

	private struct Vector3Int
	{
		public byte x;

		public byte y;

		public byte z;
	}
}
