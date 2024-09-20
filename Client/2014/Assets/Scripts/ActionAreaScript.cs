using System;
using kube;
using kube.ui;
using UnityEngine;

public class ActionAreaScript : GameMapItem
{
	private void Start()
	{
	}

	public override void SaveMap(KubeStream bw)
	{
		bw.WriteByte((byte)this.x1);
		bw.WriteByte((byte)this.y1);
		bw.WriteByte((byte)this.z1);
		bw.WriteByte((byte)this.x2);
		bw.WriteByte((byte)this.y2);
		bw.WriteByte((byte)this.z2);
		bw.WriteByte((byte)this.type);
		bw.WriteByte((byte)this.materialType);
		bw.WriteByte((byte)this.status);
		bw.WriteByte((byte)this.soundType);
		bw.WriteByte((byte)this.prop1);
		bw.WriteByte((byte)this.prop2);
		bw.WriteByte((byte)this.prop3);
		bw.WriteByte((byte)this.id);
	}

	public override void LoadMap(KubeStream br)
	{
		int num = (int)br.ReadByte();
		int num2 = (int)br.ReadByte();
		int num3 = (int)br.ReadByte();
		int num4 = (int)br.ReadByte();
		int num5 = (int)br.ReadByte();
		int num6 = (int)br.ReadByte();
		AAType aatype = (AAType)br.ReadByte();
		int num7 = (int)br.ReadByte();
		int num8 = (int)br.ReadByte();
		int num9 = (int)br.ReadByte();
		this.prop1 = (int)br.ReadByte();
		this.prop2 = (int)br.ReadByte();
		this.prop3 = (int)br.ReadByte();
		this.id = (int)br.ReadByte();
		Kube.WHS.AAid(base.gameObject, this.id);
		this.SetParameters(num, num2, num3, num4, num5, num6, (int)aatype, num7, num8, (int)this.coordState, num9, this.prop1, this.prop2, this.prop3, this.id);
	}

	private void UpdateVerticalDoor()
	{
		for (int i = 0; i < this.AAsamples.Length; i++)
		{
			this.AAsamples[i].transform.position = new Vector3(this.AAsamples[i].transform.position.x, Mathf.Max(this.AAsamples[0].transform.position.y - this.maxHeight * (1f - this.coordState), this.AAsamples[0].transform.position.y - (float)i), this.AAsamples[i].transform.position.z);
		}
		for (int j = Mathf.Min(this.x1, this.x2); j <= Mathf.Max(this.x1, this.x2); j++)
		{
			for (int k = Mathf.Min(this.z1, this.z2); k <= Mathf.Max(this.z1, this.z2); k++)
			{
				int l;
				for (l = Mathf.Max(this.y1, this.y2); l >= (int)((float)Mathf.Max(this.y1, this.y2) - (1f - this.coordState) * this.maxHeight); l--)
				{
					if (Kube.WHS.IsInWorld(j, l, k))
					{
						Kube.WHS.cubes[j, l, k].prop = CubeProps.closedDoor;
					}
				}
				while (l >= Mathf.Min(this.y1, this.y2))
				{
					if (Kube.WHS.IsInWorld(j, l, k))
					{
						Kube.WHS.cubes[j, l, k].prop = CubeProps.no;
					}
					l--;
				}
			}
		}
	}

	private void UpdateHorizontalDoor()
	{
		bool[,] array = new bool[Mathf.Abs(this.x2 - this.x1) + 1, Mathf.Abs(this.z2 - this.z1) + 1];
		if (this.prop2 == 0)
		{
			for (int i = 0; i < this.AAsamples.Length; i++)
			{
				this.AAsamples[i].transform.position = new Vector3(Mathf.Max(this.AAsamples[0].transform.position.x - this.maxHeight * (1f - this.coordState), this.AAsamples[0].transform.position.x - (float)i), this.AAsamples[i].transform.position.y, this.AAsamples[i].transform.position.z);
				array[Mathf.RoundToInt(this.AAsamples[i].transform.position.x) - Mathf.Min(this.x1, this.x2), Mathf.RoundToInt(this.AAsamples[i].transform.position.z) - Mathf.Min(this.z1, this.z2)] = true;
			}
		}
		else if (this.prop2 == 2)
		{
			for (int j = 0; j < this.AAsamples.Length; j++)
			{
				this.AAsamples[j].transform.position = new Vector3(Mathf.Min(this.AAsamples[0].transform.position.x - this.maxHeight * (1f - this.coordState), this.AAsamples[0].transform.position.x + (float)j), this.AAsamples[j].transform.position.y, this.AAsamples[j].transform.position.z);
				array[Mathf.RoundToInt(this.AAsamples[j].transform.position.x) - Mathf.Min(this.x1, this.x2), Mathf.RoundToInt(this.AAsamples[j].transform.position.z) - Mathf.Min(this.z1, this.z2)] = true;
			}
		}
		else if (this.prop2 == 1)
		{
			for (int k = 0; k < this.AAsamples.Length; k++)
			{
				this.AAsamples[k].transform.position = new Vector3(this.AAsamples[k].transform.position.x, this.AAsamples[k].transform.position.y, Mathf.Max(this.AAsamples[0].transform.position.z - this.maxHeight * (1f - this.coordState), this.AAsamples[0].transform.position.z - (float)k));
				array[Mathf.RoundToInt(this.AAsamples[k].transform.position.x) - Mathf.Min(this.x1, this.x2), Mathf.RoundToInt(this.AAsamples[k].transform.position.z) - Mathf.Min(this.z1, this.z2)] = true;
			}
		}
		else if (this.prop2 == 3)
		{
			for (int l = 0; l < this.AAsamples.Length; l++)
			{
				this.AAsamples[l].transform.position = new Vector3(this.AAsamples[l].transform.position.x, this.AAsamples[l].transform.position.y, Mathf.Min(this.AAsamples[0].transform.position.z - this.maxHeight * (1f - this.coordState), this.AAsamples[0].transform.position.z + (float)l));
				array[Mathf.RoundToInt(this.AAsamples[l].transform.position.x) - Mathf.Min(this.x1, this.x2), Mathf.RoundToInt(this.AAsamples[l].transform.position.z) - Mathf.Min(this.z1, this.z2)] = true;
			}
		}
		for (int m = 0; m < Mathf.Abs(this.x2 - this.x1) + 1; m++)
		{
			for (int n = 0; n < Mathf.Abs(this.z2 - this.z1) + 1; n++)
			{
				for (int num = Mathf.Min(this.y1, this.y2); num <= Mathf.Max(this.y1, this.y2); num++)
				{
					if (m + this.x1 <= Kube.WHS.cubes.GetLength(0))
					{
						if (n + this.z1 <= Kube.WHS.cubes.GetLength(2))
						{
							if (array[m, n])
							{
								Kube.WHS.cubes[m + this.x1, num, n + this.z1].prop = CubeProps.closedDoor;
							}
							else
							{
								Kube.WHS.cubes[m + this.x1, num, n + this.z1].prop = CubeProps.no;
							}
						}
					}
				}
			}
		}
	}

	private void Update()
	{
		float num = -1f;
		if (this.type == AAType.doorVertical)
		{
			if (this.status == 1)
			{
				num = Mathf.Min(1f, this.coordState + Time.deltaTime * (10f / ((float)this.prop1 + 1f)));
			}
			else if (this.status == 0)
			{
				num = Mathf.Max(0f, this.coordState - Time.deltaTime * (10f / ((float)this.prop1 + 1f)));
			}
			if (num == this.coordState)
			{
				return;
			}
			this.coordState = num;
			this.UpdateVerticalDoor();
		}
		else if (this.type == AAType.doorHorizontal)
		{
			if (this.status == 1)
			{
				num = Mathf.Min(1f, this.coordState + Time.deltaTime * (10f / ((float)this.prop1 + 1f)));
			}
			else if (this.status == 0)
			{
				num = Mathf.Max(0f, this.coordState - Time.deltaTime * (10f / ((float)this.prop1 + 1f)));
			}
			if (num == this.coordState)
			{
				return;
			}
			this.coordState = num;
			this.UpdateHorizontalDoor();
		}
		if (Time.time - this.lastSaveCoordState > this.saveCoordStatePeriod)
		{
			Kube.WHS.SaveAA(this.x1, this.y1, this.z1, this.x2, this.y2, this.z2, (int)this.type, this.materialType, this.status, (int)(this.coordState * 255f), this.soundType, this.prop1, this.prop2, this.prop3, this.id);
			this.lastSaveCoordState = Time.time;
		}
	}

	private void GenerateCubeMesh(Mesh mesh, Vector3 pos1, Vector3 pos2, float uvScaleOffset, Vector3 parentPos)
	{
		Vector3[] array = new Vector3[6];
		array[0].x = 0f;
		array[0].y = 1f;
		array[0].z = 0f;
		array[1].x = 0f;
		array[1].y = -1f;
		array[1].z = 0f;
		array[2].x = 0f;
		array[2].y = 0f;
		array[2].z = 1f;
		array[3].x = 0f;
		array[3].y = 0f;
		array[3].z = -1f;
		array[4].x = 1f;
		array[4].y = 0f;
		array[4].z = 0f;
		array[5].x = -1f;
		array[5].y = 0f;
		array[5].z = 0f;
		Vector3[,] array2 = new Vector3[6, 4];
		array2[0, 0].x = pos1.x;
		array2[0, 0].y = pos2.y;
		array2[0, 0].z = pos1.z;
		array2[0, 1].x = pos1.x;
		array2[0, 1].y = pos2.y;
		array2[0, 1].z = pos2.z;
		array2[0, 2].x = pos2.x;
		array2[0, 2].y = pos2.y;
		array2[0, 2].z = pos2.z;
		array2[0, 3].x = pos2.x;
		array2[0, 3].y = pos2.y;
		array2[0, 3].z = pos1.z;
		array2[1, 0].x = pos2.x;
		array2[1, 0].y = pos1.y;
		array2[1, 0].z = pos1.z;
		array2[1, 1].x = pos2.x;
		array2[1, 1].y = pos1.y;
		array2[1, 1].z = pos2.z;
		array2[1, 2].x = pos1.x;
		array2[1, 2].y = pos1.y;
		array2[1, 2].z = pos2.z;
		array2[1, 3].x = pos1.x;
		array2[1, 3].y = pos1.y;
		array2[1, 3].z = pos1.z;
		array2[2, 0].x = pos2.x;
		array2[2, 0].y = pos1.y;
		array2[2, 0].z = pos2.z;
		array2[2, 1].x = pos2.x;
		array2[2, 1].y = pos2.y;
		array2[2, 1].z = pos2.z;
		array2[2, 2].x = pos1.x;
		array2[2, 2].y = pos2.y;
		array2[2, 2].z = pos2.z;
		array2[2, 3].x = pos1.x;
		array2[2, 3].y = pos1.y;
		array2[2, 3].z = pos2.z;
		array2[3, 0].x = pos1.x;
		array2[3, 0].y = pos1.y;
		array2[3, 0].z = pos1.z;
		array2[3, 1].x = pos1.x;
		array2[3, 1].y = pos2.y;
		array2[3, 1].z = pos1.z;
		array2[3, 2].x = pos2.x;
		array2[3, 2].y = pos2.y;
		array2[3, 2].z = pos1.z;
		array2[3, 3].x = pos2.x;
		array2[3, 3].y = pos1.y;
		array2[3, 3].z = pos1.z;
		array2[4, 0].x = pos2.x;
		array2[4, 0].y = pos1.y;
		array2[4, 0].z = pos1.z;
		array2[4, 1].x = pos2.x;
		array2[4, 1].y = pos2.y;
		array2[4, 1].z = pos1.z;
		array2[4, 2].x = pos2.x;
		array2[4, 2].y = pos2.y;
		array2[4, 2].z = pos2.z;
		array2[4, 3].x = pos2.x;
		array2[4, 3].y = pos1.y;
		array2[4, 3].z = pos2.z;
		array2[5, 0].x = pos1.x;
		array2[5, 0].y = pos1.y;
		array2[5, 0].z = pos2.z;
		array2[5, 1].x = pos1.x;
		array2[5, 1].y = pos2.y;
		array2[5, 1].z = pos2.z;
		array2[5, 2].x = pos1.x;
		array2[5, 2].y = pos2.y;
		array2[5, 2].z = pos1.z;
		array2[5, 3].x = pos1.x;
		array2[5, 3].y = pos1.y;
		array2[5, 3].z = pos1.z;
		Vector3[] array3 = new Vector3[36];
		Vector3[] array4 = new Vector3[36];
		Vector2[] array5 = new Vector2[36];
		int[] array6 = new int[36];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < 6; i++)
		{
			array3[num] = array2[i, 0] - parentPos;
			array3[num + 1] = array2[i, 1] - parentPos;
			array3[num + 2] = array2[i, 2] - parentPos;
			array3[num + 3] = array2[i, 2] - parentPos;
			array3[num + 4] = array2[i, 3] - parentPos;
			array3[num + 5] = array2[i, 0] - parentPos;
			float d = 1f;
			float d2 = 1f;
			if (i == 0 || i == 1)
			{
				d = pos2.x - pos1.x + 1f - 2f * uvScaleOffset;
				d2 = pos2.z - pos1.z + 1f - 2f * uvScaleOffset;
			}
			else if (i == 2 || i == 3)
			{
				d = pos2.x - pos1.x + 1f - 2f * uvScaleOffset;
				d2 = pos2.y - pos1.y + 1f - 2f * uvScaleOffset;
			}
			else if (i == 4 || i == 5)
			{
				d = pos2.z - pos1.z + 1f - 2f * uvScaleOffset;
				d2 = pos2.y - pos1.y + 1f - 2f * uvScaleOffset;
			}
			array5[num2] = Vector2.zero;
			array5[num2 + 1] = Vector2.up * d2;
			array5[num2 + 2] = Vector2.up * d2 + Vector2.right * d;
			array5[num2 + 3] = Vector2.up * d2 + Vector2.right * d;
			array5[num2 + 4] = Vector2.right * d;
			array5[num2 + 5] = Vector2.zero;
			array4[num3] = array[i];
			array4[num3 + 1] = array[i];
			array4[num3 + 2] = array[i];
			array4[num3 + 3] = array[i];
			array4[num3 + 4] = array[i];
			array4[num3 + 5] = array[i];
			array6[num4] = num;
			array6[num4 + 1] = num + 1;
			array6[num4 + 2] = num + 2;
			array6[num4 + 3] = num + 3;
			array6[num4 + 4] = num + 4;
			array6[num4 + 5] = num + 5;
			num += 6;
			num2 += 6;
			num3 += 6;
			num4 += 6;
		}
		mesh.vertices = array3;
		mesh.normals = array4;
		mesh.uv = array5;
		mesh.triangles = array6;
		mesh.RecalculateBounds();
		mesh.Optimize();
	}

	private void RecreateSamples()
	{
		Vector3 vector = new Vector3((float)Mathf.Min(this.x1, this.x2), (float)Mathf.Min(this.y1, this.y2), (float)Mathf.Min(this.z1, this.z2));
		Vector3 vector2 = new Vector3((float)Mathf.Max(this.x1, this.x2), (float)Mathf.Max(this.y1, this.y2), (float)Mathf.Max(this.z1, this.z2));
		string text = string.Empty;
		int num = 0;
		int num2 = 0;
		for (int i = (int)vector.x; i <= (int)vector2.x; i++)
		{
			for (int j = (int)vector.y; j <= (int)vector2.y; j++)
			{
				for (int k = (int)vector.z; k <= (int)vector2.z; k++)
				{
					if (num2 == 0)
					{
						text += Kube.OH.GetServerCode(Mathf.Min((int)(vector2.x - vector.x + 1f) * (int)(vector2.y - vector.y + 1f) * (int)(vector2.z - vector.z + 1f) - num, 4096), 2);
					}
					text += Kube.OH.GetServerCode(i, 2);
					text += Kube.OH.GetServerCode(j, 2);
					text += Kube.OH.GetServerCode(k, 2);
					text += Kube.OH.GetServerCode(0, 2);
					num++;
					num2++;
					if (num2 == 4096)
					{
						Kube.WHS.ChangeCubes(text, false, true);
						num2 = 0;
						text = string.Empty;
					}
				}
			}
		}
		if (num2 != 0)
		{
			Kube.WHS.ChangeCubes(text, false, true);
		}
		for (int l = 0; l < this.AAsamples.Length; l++)
		{
			if (!(this.AAsamples[l] == null))
			{
				this.AAsamples[l].GetComponent<MeshFilter>().mesh.Clear();
				UnityEngine.Object.Destroy(this.AAsamples[l]);
			}
		}
		if (this.type == AAType.doorVertical)
		{
			this.AAsamples = new GameObject[(int)vector2.y - (int)vector.y + 1];
			int num3 = 0;
			for (int m = (int)vector2.y; m >= (int)vector.y; m--)
			{
				this.AAsamples[num3] = (UnityEngine.Object.Instantiate(this.AAsimplePrefab, new Vector3(vector.x, (float)m, vector.z), Quaternion.identity) as GameObject);
				this.AAsamples[num3].renderer.material = Kube.ASS2.miniCubesMat[this.materialType];
				this.GenerateCubeMesh(this.AAsamples[num3].GetComponent<MeshFilter>().mesh, new Vector3(vector.x - 0.5f, (float)m - 0.5f, vector.z - 0.5f), new Vector3(vector2.x + 0.5f, (float)m + 0.5f, vector2.z + 0.5f), 0.5f, this.AAsamples[num3].transform.position);
				this.AAsamples[num3].GetComponent<MeshCollider>().sharedMesh = null;
				this.AAsamples[num3].GetComponent<MeshCollider>().sharedMesh = this.AAsamples[num3].GetComponent<MeshFilter>().mesh;
				num3++;
			}
		}
		if (this.type == AAType.doorHorizontal)
		{
			if (this.prop2 == 0)
			{
				this.AAsamples = new GameObject[(int)vector2.x - (int)vector.x + 1];
				this.maxHeight = vector2.x - vector.x;
				int num4 = 0;
				for (int n = (int)vector2.x; n >= (int)vector.x; n--)
				{
					this.AAsamples[num4] = (UnityEngine.Object.Instantiate(this.AAsimplePrefab, new Vector3((float)n, vector.y, vector.z), Quaternion.identity) as GameObject);
					this.AAsamples[num4].renderer.material = Kube.ASS2.miniCubesMat[this.materialType];
					this.GenerateCubeMesh(this.AAsamples[num4].GetComponent<MeshFilter>().mesh, new Vector3((float)n - 0.5f, vector.y - 0.5f, vector.z - 0.5f), new Vector3((float)n + 0.5f, vector2.y + 0.5f, vector2.z + 0.5f), 0.5f, this.AAsamples[num4].transform.position);
					this.AAsamples[num4].GetComponent<MeshCollider>().sharedMesh = null;
					this.AAsamples[num4].GetComponent<MeshCollider>().sharedMesh = this.AAsamples[num4].GetComponent<MeshFilter>().mesh;
					num4++;
				}
			}
			else if (this.prop2 == 1)
			{
				this.AAsamples = new GameObject[(int)vector2.z - (int)vector.z + 1];
				this.maxHeight = vector2.z - vector.z;
				int num5 = 0;
				for (int num6 = (int)vector2.z; num6 >= (int)vector.z; num6--)
				{
					this.AAsamples[num5] = (UnityEngine.Object.Instantiate(this.AAsimplePrefab, new Vector3(vector.x, vector.y, (float)num6), Quaternion.identity) as GameObject);
					this.AAsamples[num5].renderer.material = Kube.ASS2.miniCubesMat[this.materialType];
					this.GenerateCubeMesh(this.AAsamples[num5].GetComponent<MeshFilter>().mesh, new Vector3(vector.x - 0.5f, vector.y - 0.5f, (float)num6 - 0.5f), new Vector3(vector2.x + 0.5f, vector2.y + 0.5f, (float)num6 + 0.5f), 0.5f, this.AAsamples[num5].transform.position);
					this.AAsamples[num5].GetComponent<MeshCollider>().sharedMesh = null;
					this.AAsamples[num5].GetComponent<MeshCollider>().sharedMesh = this.AAsamples[num5].GetComponent<MeshFilter>().mesh;
					num5++;
				}
			}
			else if (this.prop2 == 2)
			{
				this.AAsamples = new GameObject[(int)vector2.x - (int)vector.x + 1];
				this.maxHeight = vector.x - vector2.x;
				int num7 = 0;
				for (int num8 = (int)vector.x; num8 <= (int)vector2.x; num8++)
				{
					this.AAsamples[num7] = (UnityEngine.Object.Instantiate(this.AAsimplePrefab, new Vector3((float)num8, vector.y, vector.z), Quaternion.identity) as GameObject);
					this.AAsamples[num7].renderer.material = Kube.ASS2.miniCubesMat[this.materialType];
					this.GenerateCubeMesh(this.AAsamples[num7].GetComponent<MeshFilter>().mesh, new Vector3((float)num8 - 0.5f, vector.y - 0.5f, vector.z - 0.5f), new Vector3((float)num8 + 0.5f, vector2.y + 0.5f, vector2.z + 0.5f), 0.5f, this.AAsamples[num7].transform.position);
					this.AAsamples[num7].GetComponent<MeshCollider>().sharedMesh = null;
					this.AAsamples[num7].GetComponent<MeshCollider>().sharedMesh = this.AAsamples[num7].GetComponent<MeshFilter>().mesh;
					num7++;
				}
			}
			else if (this.prop2 == 3)
			{
				this.AAsamples = new GameObject[(int)vector2.z - (int)vector.z + 1];
				this.maxHeight = vector.x - vector2.x;
				int num9 = 0;
				for (int num10 = (int)vector.z; num10 <= (int)vector2.z; num10++)
				{
					this.AAsamples[num9] = (UnityEngine.Object.Instantiate(this.AAsimplePrefab, new Vector3(vector.x, vector.y, (float)num10), Quaternion.identity) as GameObject);
					this.AAsamples[num9].renderer.material = Kube.ASS2.miniCubesMat[this.materialType];
					this.GenerateCubeMesh(this.AAsamples[num9].GetComponent<MeshFilter>().mesh, new Vector3(vector.x - 0.5f, vector.y - 0.5f, (float)num10 - 0.5f), new Vector3(vector2.x + 0.5f, vector2.y + 0.5f, (float)num10 + 0.5f), 0.5f, this.AAsamples[num9].transform.position);
					this.AAsamples[num9].GetComponent<MeshCollider>().sharedMesh = null;
					this.AAsamples[num9].GetComponent<MeshCollider>().sharedMesh = this.AAsamples[num9].GetComponent<MeshFilter>().mesh;
					num9++;
				}
			}
		}
		if (this.type == AAType.lift)
		{
			ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
			for (int num11 = 0; num11 < componentsInChildren.Length; num11++)
			{
				componentsInChildren[num11].gameObject.transform.position = Vector3.Lerp(vector, vector2, 0.5f);
				componentsInChildren[num11].gameObject.transform.localScale = vector2 - vector + Vector3.one;
				componentsInChildren[num11].emissionRate = (float)((int)(((vector2 - vector).x + 1f) * ((vector2 - vector).y + 1f) * ((vector2 - vector).z + 1f)));
				if (componentsInChildren[num11].gameObject.name == "on")
				{
					componentsInChildren[num11].emissionRate *= 3f;
				}
			}
		}
		if (this.type == AAType.forceField)
		{
			this.AAsamples = new GameObject[1];
			this.AAsamples[0] = (UnityEngine.Object.Instantiate(this.AAsimplePrefab, new Vector3(vector.x, vector.y, vector.z), Quaternion.identity) as GameObject);
			this.AAsamples[0].renderer.sharedMaterial = Kube.ASS2.AAselectMat;
			this.GenerateCubeMesh(this.AAsamples[0].GetComponent<MeshFilter>().mesh, new Vector3(vector.x - 0.5f, vector.y - 0.5f, vector.z - 0.5f), new Vector3(vector2.x + 0.5f, vector2.y + 0.5f, vector2.z + 0.5f), 0.5f, this.AAsamples[0].transform.position);
			this.AAsamples[0].GetComponent<MeshCollider>().sharedMesh = null;
			this.AAsamples[0].GetComponent<MeshCollider>().sharedMesh = this.AAsamples[0].GetComponent<MeshFilter>().mesh;
		}
	}

	private void SetParameters(int playerId)
	{
		Vector3 vector = Vector3.zero;
		if (this.type == AAType.doorVertical || this.type == AAType.doorHorizontal)
		{
			vector = base.transform.position + base.transform.TransformDirection(-Vector3.forward);
		}
		else if (this.type == AAType.lift || this.type == AAType.forceField)
		{
			vector = base.transform.position;
		}
		this.x1 = (this.x2 = Mathf.RoundToInt(vector.x));
		this.y1 = (this.y2 = Mathf.RoundToInt(vector.y));
		this.z1 = (this.z2 = Mathf.RoundToInt(vector.z));
		if (this.type == AAType.doorVertical)
		{
			this.maxHeight = (float)(this.y2 - this.y1);
		}
		base.transform.rotation = Quaternion.identity;
		this.materialType = (int)Kube.WHS.cubes[this.x1, this.y1, this.z1].type;
		if (this.id == -1)
		{
			this.id = Kube.WHS.GetNewAAid(base.gameObject);
		}
		this.status = 0;
		this.soundType = 0;
		this.prop1 = 20;
		this.prop2 = (this.prop3 = 0);
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
		this.NO.CreateNewAA(this.x1, this.y1, this.z1, this.x2, this.y2, this.z2, (int)this.type, this.materialType, this.status, (int)(this.coordState * 255f), this.soundType, this.prop1, this.prop2, this.prop3, this.id, Kube.GPS.playerId);
		this.RecreateBoundMesh();
		this.RecreateSamples();
		this.SetupItem();
	}

	public void SetParameters(int _x1, int _y1, int _z1, int _x2, int _y2, int _z2, int _type, int _materialType, int _status, int _coordState, int _soundType, int _prop1, int _prop2, int _prop3, int _id)
	{
		bool flag = false;
		if (_x1 > Kube.WHS.cubes.GetLength(0))
		{
			_x1 = 0;
		}
		if (_z1 > Kube.WHS.cubes.GetLength(2))
		{
			_z1 = 0;
		}
		if (_x2 > Kube.WHS.cubes.GetLength(0))
		{
			_x2 = 0;
		}
		if (_z2 > Kube.WHS.cubes.GetLength(2))
		{
			_z2 = 0;
		}
		if (this.x1 != _x1)
		{
			this.x1 = _x1;
			flag = true;
		}
		if (this.y1 != _y1)
		{
			this.y1 = _y1;
			flag = true;
		}
		if (this.z1 != _z1)
		{
			this.z1 = _z1;
			flag = true;
		}
		if (this.x2 != _x2)
		{
			this.x2 = _x2;
			flag = true;
		}
		if (this.y2 != _y2)
		{
			this.y2 = _y2;
			flag = true;
		}
		if (this.z2 != _z2)
		{
			this.z2 = _z2;
			flag = true;
		}
		if (this.type == AAType.doorVertical)
		{
			this.maxHeight = (float)(this.y2 - this.y1);
		}
		this.type = (AAType)_type;
		if (this.materialType != _materialType)
		{
			this.materialType = _materialType;
			flag = true;
		}
		if (!flag && this.status != _status && this.soundType != 0 && Kube.OH.AAsounds[(this.soundType - 1) * 2 + _status] != null)
		{
			UnityEngine.Object.Instantiate(Kube.OH.AAsounds[(this.soundType - 1) * 2 + _status], new Vector3((float)(this.x1 + this.x2) * 0.5f, (float)(this.y1 + this.y2) * 0.5f, (float)(this.z1 + this.z2) * 0.5f), Quaternion.identity);
		}
		if (this.type == AAType.lift)
		{
			if (_status == 0)
			{
				base.transform.Find("off").gameObject.particleSystem.enableEmission = true;
				base.transform.Find("on").gameObject.particleSystem.enableEmission = false;
				Kube.WHS.RecalculatePhysForAA(this.x1, this.y1, this.z1, this.x2, this.y2, this.z2);
			}
			else
			{
				base.transform.Find("off").gameObject.particleSystem.Clear();
				base.transform.Find("off").gameObject.particleSystem.enableEmission = false;
				base.transform.Find("on").gameObject.particleSystem.enableEmission = true;
				Kube.WHS.RecalculatePhysForAA(this.x1, this.y1, this.z1, this.x2, this.y2, this.z2);
			}
		}
		this.status = _status;
		this.coordState = (float)_coordState / 255f;
		this.soundType = _soundType;
		this.prop1 = _prop1;
		if (this.type == AAType.doorHorizontal && this.prop2 != _prop2)
		{
			this.prop2 = _prop2;
			flag = true;
		}
		this.prop3 = _prop3;
		this.id = _id;
		this.RecreateBoundMesh();
		if (flag)
		{
			this.RecreateSamples();
		}
		if (this.type == AAType.forceField)
		{
			if (this.status == 0)
			{
				if (this.AAsamples != null && this.AAsamples.Length > 0)
				{
					this.AAsamples[0].SetActive(false);
				}
			}
			else if (this.status == 1 && this.AAsamples != null && this.AAsamples.Length > 0)
			{
				this.AAsamples[0].SetActive(true);
			}
		}
		if (Kube.BCS != null && Kube.BCS.gameType != GameType.creating)
		{
			base.gameObject.layer = 14;
		}
		if (this.type == AAType.doorVertical)
		{
			this.UpdateVerticalDoor();
		}
		if (this.type == AAType.doorHorizontal)
		{
			this.UpdateHorizontalDoor();
		}
	}

	private void RecreateBoundMesh()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		component.mesh.Clear();
		Vector3 pos = new Vector3((float)Mathf.Min(this.x1, this.x2) - 0.65f, (float)Mathf.Min(this.y1, this.y2) - 0.65f, (float)Mathf.Min(this.z1, this.z2) - 0.65f);
		Vector3 pos2 = new Vector3((float)Mathf.Max(this.x1, this.x2) + 0.65f, (float)Mathf.Max(this.y1, this.y2) + 0.65f, (float)Mathf.Max(this.z1, this.z2) + 0.65f);
		this.GenerateCubeMesh(component.mesh, pos, pos2, 0.65f, base.transform.position);
		base.GetComponent<MeshCollider>().sharedMesh = null;
		base.GetComponent<MeshCollider>().sharedMesh = component.mesh;
	}

	private void SetupItem()
	{
		Kube.OH.openMenu(new DrawCall(this.setupGUI), true, false);
	}

	private void SaveAA(bool redraw)
	{
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
		int num = this.x1;
		int num2 = this.y1;
		int num3 = this.z1;
		if (redraw)
		{
			this.x1 = (this.y1 = (this.z1 = 0));
		}
		this.NO.SetAAParameters(num, num2, num3, this.x2, this.y2, this.z2, (int)this.type, this.materialType, this.status, (int)(this.coordState * 255f), this.soundType, this.prop1, this.prop2, this.prop3, this.id, Kube.GPS.playerId);
	}

	private void DeleteItem()
	{
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
		this.NO.DeleteAA(this.id);
		if (Kube.OH.hasMenu(new DrawCall(this.setupGUI)))
		{
			Kube.OH.closeMenu(null);
		}
	}

	private void OnDestroy()
	{
		if (Kube.OH && Kube.OH.hasMenu(new DrawCall(this.setupGUI)))
		{
			Kube.OH.closeMenu(null);
		}
	}

	private void Command_On()
	{
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
		this.NO.SetAAParameters(this.x1, this.y1, this.z1, this.x2, this.y2, this.z2, (int)this.type, this.materialType, 1, (int)(this.coordState * 255f), this.soundType, this.prop1, this.prop2, this.prop3, this.id, Kube.GPS.playerId);
	}

	private void Command_Off()
	{
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
		this.NO.SetAAParameters(this.x1, this.y1, this.z1, this.x2, this.y2, this.z2, (int)this.type, this.materialType, 0, (int)(this.coordState * 255f), this.soundType, this.prop1, this.prop2, this.prop3, this.id, Kube.GPS.playerId);
	}

	private void Command_Toggle()
	{
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
		this.NO.SetAAParameters(this.x1, this.y1, this.z1, this.x2, this.y2, this.z2, (int)this.type, this.materialType, 1 - this.status, (int)(this.coordState * 255f), this.soundType, this.prop1, this.prop2, this.prop3, this.id, Kube.GPS.playerId);
	}

	private void setupGUI()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		float num3 = 0.5f * num - 350f;
		float num4 = num2 - 300f;
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
		GUI.skin = Kube.ASS1.mainSkin;
		GUI.DrawTexture(new Rect(num3, num4, 700f, 230f), Kube.ASS3.setupItemTex);
		GUI.skin = Kube.ASS1.bigWhiteLabel;
		GUI.Label(new Rect(num3 + 100f, num4 + 2f, 350f, 40f), Localize.AAnames[(int)this.type]);
		GUI.skin = Kube.ASS1.triggerSkin;
		GUI.Label(new Rect(num3 + 50f, num4 + 40f, 150f, 30f), Localize.AAS_Position);
		if (GUI.Button(new Rect(num3 + 10f, num4 + 85f, 60f, 30f), Localize.AAS_Upper) && Mathf.Max(this.y1, this.y2) < Kube.WHS.sizeY - 1)
		{
			this.y1++;
			this.y2++;
			this.SaveAA(true);
		}
		if (GUI.Button(new Rect(num3 + 10f, num4 + 125f, 60f, 30f), Localize.AAS_Lower) && Mathf.Min(this.y1, this.y2) > 0)
		{
			this.y1--;
			this.y2--;
			this.SaveAA(true);
		}
		if (GUI.Button(new Rect(num3 + 110f, num4 + 65f, 90f, 30f), Localize.AAS_Far))
		{
			Vector3 vector = Kube.IS.ps.cameraComp.gameObject.transform.TransformDirection(Vector3.forward);
			vector.y = 0f;
			if (Mathf.Abs(vector.x) >= Mathf.Abs(vector.z))
			{
				vector.z = 0f;
				vector.Normalize();
			}
			else
			{
				vector.x = 0f;
				vector.Normalize();
			}
			if (vector.x > 0f)
			{
				if (Mathf.Max(this.x1, this.x2) < Kube.WHS.sizeX - 1)
				{
					this.x1++;
					this.x2++;
					this.SaveAA(true);
				}
			}
			else if (vector.z > 0f)
			{
				if (Mathf.Max(this.z1, this.z2) < Kube.WHS.sizeZ - 1)
				{
					this.z1++;
					this.z2++;
					this.SaveAA(true);
				}
			}
			else if (vector.x < 0f)
			{
				if (Mathf.Min(this.x1, this.x2) < Kube.WHS.sizeX - 1)
				{
					this.x1--;
					this.x2--;
					this.SaveAA(true);
				}
			}
			else if (vector.z < 0f && Mathf.Min(this.z1, this.z2) < Kube.WHS.sizeX - 1)
			{
				this.z1--;
				this.z2--;
				this.SaveAA(true);
			}
		}
		if (GUI.Button(new Rect(num3 + 110f, num4 + 140f, 90f, 30f), Localize.AAS_Near))
		{
			Vector3 vector2 = Kube.IS.ps.cameraComp.gameObject.transform.TransformDirection(-Vector3.forward);
			vector2.y = 0f;
			if (Mathf.Abs(vector2.x) >= Mathf.Abs(vector2.z))
			{
				vector2.z = 0f;
				vector2.Normalize();
			}
			else
			{
				vector2.x = 0f;
				vector2.Normalize();
			}
			if (vector2.x > 0f)
			{
				if (Mathf.Max(this.x1, this.x2) < Kube.WHS.sizeX - 1)
				{
					this.x1++;
					this.x2++;
					this.SaveAA(true);
				}
			}
			else if (vector2.z > 0f)
			{
				if (Mathf.Max(this.z1, this.z2) < Kube.WHS.sizeZ - 1)
				{
					this.z1++;
					this.z2++;
					this.SaveAA(true);
				}
			}
			else if (vector2.x < 0f)
			{
				if (Mathf.Min(this.x1, this.x2) < Kube.WHS.sizeX - 1)
				{
					this.x1--;
					this.x2--;
					this.SaveAA(true);
				}
			}
			else if (vector2.z < 0f && Mathf.Min(this.z1, this.z2) < Kube.WHS.sizeX - 1)
			{
				this.z1--;
				this.z2--;
				this.SaveAA(true);
			}
		}
		if (GUI.Button(new Rect(num3 + 165f, num4 + 100f, 80f, 30f), Localize.AAS_Right))
		{
			Vector3 vector3 = Kube.IS.ps.cameraComp.gameObject.transform.TransformDirection(Vector3.right);
			vector3.y = 0f;
			if (Mathf.Abs(vector3.x) >= Mathf.Abs(vector3.z))
			{
				vector3.z = 0f;
				vector3.Normalize();
			}
			else
			{
				vector3.x = 0f;
				vector3.Normalize();
			}
			if (vector3.x > 0f)
			{
				if (Mathf.Max(this.x1, this.x2) < Kube.WHS.sizeX - 1)
				{
					this.x1++;
					this.x2++;
					this.SaveAA(true);
				}
			}
			else if (vector3.z > 0f)
			{
				if (Mathf.Max(this.z1, this.z2) < Kube.WHS.sizeZ - 1)
				{
					this.z1++;
					this.z2++;
					this.SaveAA(true);
				}
			}
			else if (vector3.x < 0f)
			{
				if (Mathf.Min(this.x1, this.x2) < Kube.WHS.sizeX - 1)
				{
					this.x1--;
					this.x2--;
					this.SaveAA(true);
				}
			}
			else if (vector3.z < 0f && Mathf.Min(this.z1, this.z2) < Kube.WHS.sizeX - 1)
			{
				this.z1--;
				this.z2--;
				this.SaveAA(true);
			}
		}
		if (GUI.Button(new Rect(num3 + 75f, num4 + 100f, 80f, 30f), Localize.AAS_Left))
		{
			Vector3 vector4 = Kube.IS.ps.cameraComp.gameObject.transform.TransformDirection(-Vector3.right);
			vector4.y = 0f;
			if (Mathf.Abs(vector4.x) >= Mathf.Abs(vector4.z))
			{
				vector4.z = 0f;
				vector4.Normalize();
			}
			else
			{
				vector4.x = 0f;
				vector4.Normalize();
			}
			if (vector4.x > 0f)
			{
				if (Mathf.Max(this.x1, this.x2) < Kube.WHS.sizeX - 1)
				{
					this.x1++;
					this.x2++;
					this.SaveAA(true);
				}
			}
			else if (vector4.z > 0f)
			{
				if (Mathf.Max(this.z1, this.z2) < Kube.WHS.sizeZ - 1)
				{
					this.z1++;
					this.z2++;
					this.SaveAA(true);
				}
			}
			else if (vector4.x < 0f)
			{
				if (Mathf.Min(this.x1, this.x2) < Kube.WHS.sizeX - 1)
				{
					this.x1--;
					this.x2--;
					this.SaveAA(true);
				}
			}
			else if (vector4.z < 0f && Mathf.Min(this.z1, this.z2) < Kube.WHS.sizeX - 1)
			{
				this.z1--;
				this.z2--;
				this.SaveAA(true);
			}
		}
		GUI.Label(new Rect(num3 + 340f, num4 + 40f, 150f, 30f), Localize.AAS_Size);
		if (GUI.Button(new Rect(num3 + 310f, num4 + 85f, 60f, 30f), Localize.AAS_Upper) && Mathf.Max(this.y1, this.y2) < Kube.WHS.sizeY - 1)
		{
			this.y2++;
			this.SaveAA(true);
		}
		if (GUI.Button(new Rect(num3 + 310f, num4 + 125f, 60f, 30f), Localize.AAS_Lower) && Mathf.Max(this.y1, this.y2) > Mathf.Min(this.y1, this.y2))
		{
			this.y2--;
			this.SaveAA(true);
		}
		if (GUI.Button(new Rect(num3 + 410f, num4 + 65f, 90f, 30f), Localize.AAS_Longer))
		{
			Vector3 vector5 = Kube.IS.ps.cameraComp.gameObject.transform.TransformDirection(Vector3.forward);
			vector5.y = 0f;
			if (Mathf.Abs(vector5.x) >= Mathf.Abs(vector5.z))
			{
				vector5.z = 0f;
				vector5.Normalize();
			}
			else
			{
				vector5.x = 0f;
				vector5.Normalize();
			}
			if (vector5.x > 0f)
			{
				if (Mathf.Max(this.x1, this.x2) < Kube.WHS.sizeX - 1)
				{
					this.x2++;
					this.SaveAA(true);
				}
			}
			else if (vector5.z > 0f)
			{
				if (Mathf.Max(this.z1, this.z2) < Kube.WHS.sizeZ - 1)
				{
					this.z2++;
					this.SaveAA(true);
				}
			}
			else if (vector5.x < 0f)
			{
				if (Mathf.Min(this.x1, this.x2) < Mathf.Max(this.x1, this.x2))
				{
					this.x2--;
					this.SaveAA(true);
				}
			}
			else if (vector5.z < 0f && Mathf.Min(this.z1, this.z2) < Mathf.Max(this.z1, this.z2))
			{
				this.z2--;
				this.SaveAA(true);
			}
		}
		if (GUI.Button(new Rect(num3 + 410f, num4 + 140f, 90f, 30f), Localize.AAS_Shorter))
		{
			Vector3 vector6 = Kube.IS.ps.cameraComp.gameObject.transform.TransformDirection(-Vector3.forward);
			vector6.y = 0f;
			if (Mathf.Abs(vector6.x) >= Mathf.Abs(vector6.z))
			{
				vector6.z = 0f;
				vector6.Normalize();
			}
			else
			{
				vector6.x = 0f;
				vector6.Normalize();
			}
			if (vector6.x > 0f)
			{
				if (Mathf.Max(this.x1, this.x2) < Kube.WHS.sizeX - 1)
				{
					this.x2++;
					this.SaveAA(true);
				}
			}
			else if (vector6.z > 0f)
			{
				if (Mathf.Max(this.z1, this.z2) < Kube.WHS.sizeZ - 1)
				{
					this.z2++;
					this.SaveAA(true);
				}
			}
			else if (vector6.x < 0f)
			{
				if (Mathf.Min(this.x1, this.x2) < Mathf.Max(this.x1, this.x2))
				{
					this.x2--;
					this.SaveAA(true);
				}
			}
			else if (vector6.z < 0f && Mathf.Min(this.z1, this.z2) < Mathf.Max(this.z1, this.z2))
			{
				this.z2--;
				this.SaveAA(true);
			}
		}
		if (GUI.Button(new Rect(num3 + 465f, num4 + 100f, 80f, 30f), Localize.AAS_Wider))
		{
			Vector3 vector7 = Kube.IS.ps.cameraComp.gameObject.transform.TransformDirection(Vector3.right);
			vector7.y = 0f;
			if (Mathf.Abs(vector7.x) >= Mathf.Abs(vector7.z))
			{
				vector7.z = 0f;
				vector7.Normalize();
			}
			else
			{
				vector7.x = 0f;
				vector7.Normalize();
			}
			if (vector7.x > 0f)
			{
				if (Mathf.Max(this.x1, this.x2) < Kube.WHS.sizeX - 1)
				{
					this.x2++;
					this.SaveAA(true);
				}
			}
			else if (vector7.z > 0f)
			{
				if (Mathf.Max(this.z1, this.z2) < Kube.WHS.sizeZ - 1)
				{
					this.z2++;
					this.SaveAA(true);
				}
			}
			else if (vector7.x < 0f)
			{
				if (Mathf.Min(this.x1, this.x2) < Mathf.Max(this.x1, this.x2))
				{
					this.x2--;
					this.SaveAA(true);
				}
			}
			else if (vector7.z < 0f && Mathf.Min(this.z1, this.z2) < Mathf.Max(this.z1, this.z2))
			{
				this.z2--;
				this.SaveAA(true);
			}
		}
		if (GUI.Button(new Rect(num3 + 375f, num4 + 100f, 80f, 30f), Localize.AAS_Uje))
		{
			Vector3 vector8 = Kube.IS.ps.cameraComp.gameObject.transform.TransformDirection(-Vector3.right);
			vector8.y = 0f;
			if (Mathf.Abs(vector8.x) >= Mathf.Abs(vector8.z))
			{
				vector8.z = 0f;
				vector8.Normalize();
			}
			else
			{
				vector8.x = 0f;
				vector8.Normalize();
			}
			if (vector8.x > 0f)
			{
				if (Mathf.Max(this.x1, this.x2) < Kube.WHS.sizeX - 1)
				{
					this.x2++;
					this.SaveAA(true);
				}
			}
			else if (vector8.z > 0f)
			{
				if (Mathf.Max(this.z1, this.z2) < Kube.WHS.sizeZ - 1)
				{
					this.z2++;
					this.SaveAA(true);
				}
			}
			else if (vector8.x < 0f)
			{
				if (Mathf.Min(this.x1, this.x2) < Mathf.Max(this.x1, this.x2))
				{
					this.x2--;
					this.SaveAA(true);
				}
			}
			else if (vector8.z < 0f && Mathf.Min(this.z1, this.z2) < Mathf.Max(this.z1, this.z2))
			{
				this.z2--;
				this.SaveAA(true);
			}
		}
		if (this.type == AAType.doorHorizontal || this.type == AAType.doorVertical)
		{
			GUI.Label(new Rect(num3 + 10f, num4 + 180f, 250f, 30f), string.Concat(new object[]
			{
				Localize.AAS_Opening_for_N_sec,
				" ",
				(float)(this.prop1 + 1) / 10f,
				" ",
				Localize.sec
			}));
			this.prop1 = (int)GUI.HorizontalScrollbar(new Rect(num3 + 260f, num4 + 190f, 256f, 20f), (float)this.prop1, 1f, 0f, 255f);
		}
		if (this.type == AAType.doorHorizontal)
		{
			GUI.Label(new Rect(num3 + 580f, num4 + 10f, 150f, 30f), Localize.AAS_rotation);
			int num5 = GUI.SelectionGrid(new Rect(num3 + 600f, num4 + 40f, 50f, 130f), this.prop2, this.doorRotation, 1);
			if (num5 != this.prop2)
			{
				this.prop2 = num5;
				this.SaveAA(true);
			}
		}
		if (GUI.Button(new Rect(num3 + 550f, num4 + 190f, 150f, 40f), Localize.save))
		{
			Kube.OH.closeMenu(null);
		}
	}

	public int x1;

	public int y1;

	public int z1;

	public int x2;

	public int y2;

	public int z2;

	public AAType type;

	private int materialType;

	public float coordState = 1f;

	public int status;

	private int soundType;

	public int id = -1;

	public int prop1;

	public int prop2;

	public int prop3;

	public GameObject AAsimplePrefab;

	public GameObject[] AAsamples = new GameObject[0];

	private float maxHeight;

	private float lastSaveCoordState;

	private float saveCoordStatePeriod = 1f;

	private NetworkObjectScript NO;

	private GameObject[] meshes;

	private Camera mainCamera;

	private string[] doorRotation = new string[]
	{
		"0°",
		"90°",
		"180°",
		"270°"
	};
}
