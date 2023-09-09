using System;
using kube;
using kube.map;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
	public BlockScript()
	{
		int[] array = new int[6];
		array[0] = 1;
		array[1] = -1;
		this.neighborY = array;
		int[] array2 = new int[6];
		array2[2] = 1;
		array2[3] = -1;
		this.neighborZ = array2;
		this.normalsSide = new Vector3[6];
		
	}

	private void Awake()
	{
		BlockScript._geometryGenerator.Init();
	}

	private void RecreateSolidMeshObjIfNeed(int size, int trisize)
	{
		if (size == 0)
		{
			return;
		}
		if (this.solidMeshObj != null)
		{
			if (this.meshFilterSolid.sharedMesh.vertices.Length <= size)
			{
				this.meshFilterSolid.sharedMesh.Clear();
				if (size != 0)
				{
				}
			}
		}
		else if (size != 0)
		{
			this.solidMeshObj = (UnityEngine.Object.Instantiate(this.solidMeshObjPrefab, Vector3.zero, Quaternion.identity) as GameObject);
			this.solidMeshObj.renderer.material = Kube.ASS3.cubesMat;
			this.solidMeshObj.transform.parent = base.transform;
			this.solidMeshObj.transform.localPosition = Vector3.zero;
			this.solidMeshObj.transform.localRotation = Quaternion.identity;
			this.meshFilterSolid = this.solidMeshObj.GetComponent<MeshFilter>();
			this.meshFilterSolid.mesh = new Mesh();
			this.meshFilterSolid.sharedMesh.MarkDynamic();
		}
	}

	private void RecreateTransMeshObjIfNeed(int size, int trisize)
	{
		if (size == 0)
		{
			return;
		}
		if (this.transSolidMeshObj != null)
		{
			if (this.meshFilterTrans.sharedMesh.vertices.Length <= size)
			{
				this.meshFilterTrans.sharedMesh.Clear();
				if (size != 0)
				{
				}
			}
		}
		else
		{
			this.transSolidMeshObj = (UnityEngine.Object.Instantiate(this.transSolidMeshObjPrefab, Vector3.zero, Quaternion.identity) as GameObject);
			this.transSolidMeshObj.renderer.material = Kube.ASS3.cubesTransMat;
			this.transSolidMeshObj.transform.parent = base.transform;
			this.transSolidMeshObj.transform.localPosition = Vector3.zero;
			this.transSolidMeshObj.transform.localRotation = Quaternion.identity;
			this.meshFilterTrans = this.transSolidMeshObj.GetComponent<MeshFilter>();
			this.meshFilterTrans.mesh = new Mesh();
			this.meshFilterTrans.sharedMesh.MarkDynamic();
		}
	}

	private void RecreateWaterMeshObjIfNeed(int size)
	{
		if (this.waterMeshObj != null)
		{
			if (this.meshFilterWater.mesh.vertices.Length <= size)
			{
				this.meshFilterWater.mesh.Clear();
				if (size != 0)
				{
					this.meshFilterWater.mesh.vertices = new Vector3[size + 300];
					this.meshFilterWater.mesh.normals = new Vector3[size + 300];
					this.meshFilterWater.mesh.colors32 = new Color32[size + 300];
					this.meshFilterWater.mesh.uv = new Vector2[size + 300];
					this.meshFilterWater.mesh.triangles = new int[2 * (size + 300)];
				}
			}
		}
		else if (size != 0)
		{
			this.waterMeshObj = (UnityEngine.Object.Instantiate(this.waterMeshObjPrefab, Vector3.zero, Quaternion.identity) as GameObject);
			this.waterMeshObj.renderer.material = Kube.ASS3.waterMat;
			this.waterMeshObj.transform.parent = base.transform;
			this.waterMeshObj.transform.localPosition = Vector3.zero;
			this.waterMeshObj.transform.localRotation = Quaternion.identity;
			this.meshFilterWater = this.waterMeshObj.GetComponent<MeshFilter>();
			this.meshFilterWater.mesh = new Mesh();
			this.meshFilterWater.mesh.MarkDynamic();
			this.meshFilterWater.mesh.vertices = new Vector3[size + 300];
			this.meshFilterWater.mesh.normals = new Vector3[size + 300];
			this.meshFilterWater.mesh.colors32 = new Color32[size + 300];
			this.meshFilterWater.mesh.uv = new Vector2[size + 300];
			this.meshFilterWater.mesh.triangles = new int[2 * (size + 300)];
		}
	}

	public void SetBlock(Vector3 _startCube, Vector3 _endCube)
	{
		this.startCube.x = Mathf.Min(_startCube.x, _endCube.x);
		this.startCube.y = Mathf.Min(_startCube.y, _endCube.y);
		this.startCube.z = Mathf.Min(_startCube.z, _endCube.z);
		this.endCube.x = Mathf.Max(_startCube.x, _endCube.x);
		this.endCube.y = Mathf.Max(_startCube.y, _endCube.y);
		this.endCube.z = Mathf.Max(_startCube.z, _endCube.z);
		this.Init();
	}

	private void Start()
	{
		this.Init();
	}

	private void Init()
	{
		if (this.isInitialized)
		{
			return;
		}
		this.WHS = GameObject.FindGameObjectWithTag("WorldHolder").GetComponent<WorldHolderScript>();
		this.sides3 = new int[6];
		this.sides3[0] = 0;
		this.sides3[1] = 2;
		this.sides3[2] = 1;
		this.sides3[3] = 1;
		this.sides3[4] = 1;
		this.sides3[5] = 1;
		this.normalsSide[0].x = 0f;
		this.normalsSide[0].y = 1f;
		this.normalsSide[0].z = 0f;
		this.normalsSide[1].x = 0f;
		this.normalsSide[1].y = -1f;
		this.normalsSide[1].z = 0f;
		this.normalsSide[2].x = 0f;
		this.normalsSide[2].y = 0f;
		this.normalsSide[2].z = 1f;
		this.normalsSide[3].x = 0f;
		this.normalsSide[3].y = 0f;
		this.normalsSide[3].z = -1f;
		this.normalsSide[4].x = 1f;
		this.normalsSide[4].y = 0f;
		this.normalsSide[4].z = 0f;
		this.normalsSide[5].x = -1f;
		this.normalsSide[5].y = 0f;
		this.normalsSide[5].z = 0f;
		this.vertCoords = new Vector3[6, 4];
		this.vertCoords[0, 0].x = -0.5f;
		this.vertCoords[0, 0].y = 0.5f;
		this.vertCoords[0, 0].z = -0.5f;
		this.vertCoords[0, 1].x = -0.5f;
		this.vertCoords[0, 1].y = 0.5f;
		this.vertCoords[0, 1].z = 0.5f;
		this.vertCoords[0, 2].x = 0.5f;
		this.vertCoords[0, 2].y = 0.5f;
		this.vertCoords[0, 2].z = 0.5f;
		this.vertCoords[0, 3].x = 0.5f;
		this.vertCoords[0, 3].y = 0.5f;
		this.vertCoords[0, 3].z = -0.5f;
		this.vertCoords[1, 0].x = 0.5f;
		this.vertCoords[1, 0].y = -0.5f;
		this.vertCoords[1, 0].z = -0.5f;
		this.vertCoords[1, 1].x = 0.5f;
		this.vertCoords[1, 1].y = -0.5f;
		this.vertCoords[1, 1].z = 0.5f;
		this.vertCoords[1, 2].x = -0.5f;
		this.vertCoords[1, 2].y = -0.5f;
		this.vertCoords[1, 2].z = 0.5f;
		this.vertCoords[1, 3].x = -0.5f;
		this.vertCoords[1, 3].y = -0.5f;
		this.vertCoords[1, 3].z = -0.5f;
		this.vertCoords[2, 0].x = 0.5f;
		this.vertCoords[2, 0].y = -0.5f;
		this.vertCoords[2, 0].z = 0.5f;
		this.vertCoords[2, 1].x = 0.5f;
		this.vertCoords[2, 1].y = 0.5f;
		this.vertCoords[2, 1].z = 0.5f;
		this.vertCoords[2, 2].x = -0.5f;
		this.vertCoords[2, 2].y = 0.5f;
		this.vertCoords[2, 2].z = 0.5f;
		this.vertCoords[2, 3].x = -0.5f;
		this.vertCoords[2, 3].y = -0.5f;
		this.vertCoords[2, 3].z = 0.5f;
		this.vertCoords[3, 0].x = -0.5f;
		this.vertCoords[3, 0].y = -0.5f;
		this.vertCoords[3, 0].z = -0.5f;
		this.vertCoords[3, 1].x = -0.5f;
		this.vertCoords[3, 1].y = 0.5f;
		this.vertCoords[3, 1].z = -0.5f;
		this.vertCoords[3, 2].x = 0.5f;
		this.vertCoords[3, 2].y = 0.5f;
		this.vertCoords[3, 2].z = -0.5f;
		this.vertCoords[3, 3].x = 0.5f;
		this.vertCoords[3, 3].y = -0.5f;
		this.vertCoords[3, 3].z = -0.5f;
		this.vertCoords[4, 0].x = 0.5f;
		this.vertCoords[4, 0].y = -0.5f;
		this.vertCoords[4, 0].z = -0.5f;
		this.vertCoords[4, 1].x = 0.5f;
		this.vertCoords[4, 1].y = 0.5f;
		this.vertCoords[4, 1].z = -0.5f;
		this.vertCoords[4, 2].x = 0.5f;
		this.vertCoords[4, 2].y = 0.5f;
		this.vertCoords[4, 2].z = 0.5f;
		this.vertCoords[4, 3].x = 0.5f;
		this.vertCoords[4, 3].y = -0.5f;
		this.vertCoords[4, 3].z = 0.5f;
		this.vertCoords[5, 0].x = -0.5f;
		this.vertCoords[5, 0].y = -0.5f;
		this.vertCoords[5, 0].z = 0.5f;
		this.vertCoords[5, 1].x = -0.5f;
		this.vertCoords[5, 1].y = 0.5f;
		this.vertCoords[5, 1].z = 0.5f;
		this.vertCoords[5, 2].x = -0.5f;
		this.vertCoords[5, 2].y = 0.5f;
		this.vertCoords[5, 2].z = -0.5f;
		this.vertCoords[5, 3].x = -0.5f;
		this.vertCoords[5, 3].y = -0.5f;
		this.vertCoords[5, 3].z = -0.5f;
		this.vertCoordsWaterAngle = new int[6, 4];
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				if (this.vertCoords[i, j].y > 0f && this.vertCoords[i, j].x > 0f && this.vertCoords[i, j].z > 0f)
				{
					this.vertCoordsWaterAngle[i, j] = 0;
				}
				else if (this.vertCoords[i, j].y > 0f && this.vertCoords[i, j].x < 0f && this.vertCoords[i, j].z > 0f)
				{
					this.vertCoordsWaterAngle[i, j] = 1;
				}
				else if (this.vertCoords[i, j].y > 0f && this.vertCoords[i, j].x < 0f && this.vertCoords[i, j].z < 0f)
				{
					this.vertCoordsWaterAngle[i, j] = 2;
				}
				else if (this.vertCoords[i, j].y > 0f && this.vertCoords[i, j].x > 0f && this.vertCoords[i, j].z < 0f)
				{
					this.vertCoordsWaterAngle[i, j] = 3;
				}
				else
				{
					this.vertCoordsWaterAngle[i, j] = -1;
				}
			}
		}
		this.isInitialized = true;
	}

	public void RefreshSolidMesh()
	{
		BlockScript._geometryGenerator.SetBlock(base.transform.position, this.startCube, this.endCube);
		BlockScript._geometryGenerator.collect(CubeDrawTypes.solid);
		this.RecreateSolidMeshObjIfNeed(BlockScript._geometryGenerator.numVerts, BlockScript._geometryGenerator.numTriangles);
		if (this.meshFilterSolid == null)
		{
			return;
		}
		BlockScript._geometryGenerator.RefreshMesh(this.meshFilterSolid.mesh);
		this.numSolidVerts = BlockScript._geometryGenerator.numVerts;
		this.solidMeshObj.GetComponent<MeshCollider>().sharedMesh = null;
		this.solidMeshObj.GetComponent<MeshCollider>().sharedMesh = this.meshFilterSolid.mesh;
	}

	public void RefreshTransparentSolidMesh()
	{
		BlockScript._geometryGenerator.SetBlock(base.transform.position, this.startCube, this.endCube);
		BlockScript._geometryGenerator.collect(CubeDrawTypes.transparent);
		this.RecreateTransMeshObjIfNeed(BlockScript._geometryGenerator.numVerts, BlockScript._geometryGenerator.numTriangles);
		if (this.meshFilterTrans == null)
		{
			return;
		}
		BlockScript._geometryGenerator.RefreshMesh(this.meshFilterTrans.mesh);
		this.numTransVerts = BlockScript._geometryGenerator.numVerts;
		this.transSolidMeshObj.GetComponent<MeshCollider>().sharedMesh = null;
		this.transSolidMeshObj.GetComponent<MeshCollider>().sharedMesh = this.meshFilterTrans.mesh;
	}

	public void _RefreshTransparentSolidMesh()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		this.Init();
		for (int i = (int)this.startCube.x; i < (int)this.endCube.x; i++)
		{
			for (int j = (int)this.startCube.y; j < (int)this.endCube.y; j++)
			{
				for (int k = (int)this.startCube.z; k < (int)this.endCube.z; k++)
				{
					if (this.WHS.cubesDrawTypes[(int)this.WHS.cubes[i, j, k].type] == 1 && this.WHS.cubes[i, j, k].type < 155)
					{
						if (this.WHS.cubes[i, j, k].type != 128)
						{
							for (int l = 0; l < 6; l++)
							{
								bool flag = true;
								if (i + this.neighborX[l] > this.WHS.sizeX - 1 || i + this.neighborX[l] < 0 || j + this.neighborY[l] > this.WHS.sizeY - 1 || j + this.neighborY[l] < 0 || k + this.neighborZ[l] > this.WHS.sizeZ - 1 || k + this.neighborZ[l] < 0)
								{
									flag = false;
								}
								else if (this.WHS.cubes[i, j, k].type == 0)
								{
									flag = false;
								}
								else if (this.WHS.cubesDrawTypes[(int)this.WHS.cubes[i + this.neighborX[l], j + this.neighborY[l], k + this.neighborZ[l]].type] == 0)
								{
									flag = false;
								}
								else if (this.WHS.cubes[i + this.neighborX[l], j + this.neighborY[l], k + this.neighborZ[l]].type == this.WHS.cubes[i, j, k].type)
								{
									flag = false;
								}
								if (flag)
								{
									num += 6;
								}
							}
						}
					}
				}
			}
		}
		if (num == 0 && this.meshFilterTrans == null)
		{
			return;
		}
		num = 0;
		Vector3[] vertices = this.meshFilterTrans.mesh.vertices;
		Vector3[] normals = this.meshFilterTrans.mesh.normals;
		Vector2[] uv = this.meshFilterTrans.mesh.uv;
		int[] triangles = this.meshFilterTrans.mesh.triangles;
		for (int m = (int)this.startCube.x; m < (int)this.endCube.x; m++)
		{
			for (int n = (int)this.startCube.y; n < (int)this.endCube.y; n++)
			{
				for (int num5 = (int)this.startCube.z; num5 < (int)this.endCube.z; num5++)
				{
					if (this.WHS.cubesDrawTypes[(int)this.WHS.cubes[m, n, num5].type] == 1 && this.WHS.cubes[m, n, num5].type < 155)
					{
						if (this.WHS.cubes[m, n, num5].type != 128)
						{
							Vector3 a = new Vector3((float)m, (float)n, (float)num5);
							for (int num6 = 0; num6 < 6; num6++)
							{
								bool flag = true;
								if (m + this.neighborX[num6] > this.WHS.sizeX - 1 || m + this.neighborX[num6] < 0 || n + this.neighborY[num6] > this.WHS.sizeY - 1 || n + this.neighborY[num6] < 0 || num5 + this.neighborZ[num6] > this.WHS.sizeZ - 1 || num5 + this.neighborZ[num6] < 0)
								{
									flag = false;
								}
								else if (this.WHS.cubes[m, n, num5].type == 0)
								{
									flag = false;
								}
								else if (this.WHS.cubesDrawTypes[(int)this.WHS.cubes[m + this.neighborX[num6], n + this.neighborY[num6], num5 + this.neighborZ[num6]].type] == 0)
								{
									flag = false;
								}
								else if (this.WHS.cubes[m + this.neighborX[num6], n + this.neighborY[num6], num5 + this.neighborZ[num6]].type == this.WHS.cubes[m, n, num5].type)
								{
									flag = false;
								}
								if (flag)
								{
									int data = (int)this.WHS.cubes[m, n, num5].data;
									Vector3[,] array = this.vertCoords;
									vertices[num] = a + array[num6, 0] - base.transform.position;
									vertices[num + 1] = a + array[num6, 1] - base.transform.position;
									vertices[num + 2] = a + array[num6, 2] - base.transform.position;
									vertices[num + 3] = a + array[num6, 2] - base.transform.position;
									vertices[num + 4] = a + array[num6, 3] - base.transform.position;
									vertices[num + 5] = a + array[num6, 0] - base.transform.position;
									uv[num2] = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[(int)this.WHS.cubes[m, n, num5].type, this.sides3[num6]], 0];
									uv[num2 + 1] = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[(int)this.WHS.cubes[m, n, num5].type, this.sides3[num6]], 1];
									uv[num2 + 2] = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[(int)this.WHS.cubes[m, n, num5].type, this.sides3[num6]], 2];
									uv[num2 + 3] = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[(int)this.WHS.cubes[m, n, num5].type, this.sides3[num6]], 2];
									uv[num2 + 4] = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[(int)this.WHS.cubes[m, n, num5].type, this.sides3[num6]], 3];
									uv[num2 + 5] = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[(int)this.WHS.cubes[m, n, num5].type, this.sides3[num6]], 0];
									normals[num3] = this.normalsSide[num6];
									normals[num3 + 1] = this.normalsSide[num6];
									normals[num3 + 2] = this.normalsSide[num6];
									normals[num3 + 3] = this.normalsSide[num6];
									normals[num3 + 4] = this.normalsSide[num6];
									normals[num3 + 5] = this.normalsSide[num6];
									triangles[num4] = num;
									triangles[num4 + 1] = num + 1;
									triangles[num4 + 2] = num + 2;
									triangles[num4 + 3] = num + 3;
									triangles[num4 + 4] = num + 4;
									triangles[num4 + 5] = num + 5;
									num += 6;
									num2 += 6;
									num3 += 6;
									num4 += 6;
								}
							}
						}
					}
				}
			}
		}
		for (int num7 = num; num7 < vertices.Length; num7++)
		{
			vertices[num7].x = 0f;
			vertices[num7].y = 0f;
			vertices[num7].z = 0f;
		}
		for (int num8 = num; num8 < normals.Length; num8++)
		{
			normals[num8].x = 0f;
			normals[num8].y = 0f;
			normals[num8].z = 0f;
		}
		for (int num9 = num; num9 < uv.Length; num9++)
		{
			uv[num9].x = 0f;
			uv[num9].y = 0f;
		}
		for (int num10 = num; num10 < triangles.Length; num10++)
		{
			triangles[num10] = 0;
		}
		this.numTransVerts = num;
		this.meshFilterTrans.mesh.vertices = vertices;
		this.meshFilterTrans.mesh.normals = normals;
		this.meshFilterTrans.mesh.uv = uv;
		this.meshFilterTrans.mesh.triangles = triangles;
		this.meshFilterTrans.mesh.RecalculateBounds();
		this.meshFilterTrans.mesh.Optimize();
		this.transSolidMeshObj.GetComponent<MeshCollider>().sharedMesh = null;
		this.transSolidMeshObj.GetComponent<MeshCollider>().sharedMesh = this.meshFilterTrans.mesh;
	}

	public void RefreshWaterMesh()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		this.Init();
		for (int i = (int)this.startCube.x; i < (int)this.endCube.x; i++)
		{
			for (int j = (int)this.startCube.y; j < (int)this.endCube.y; j++)
			{
				for (int k = (int)this.startCube.z; k < (int)this.endCube.z; k++)
				{
					if (this.WHS.cubesDrawTypes[(int)this.WHS.cubes[i, j, k].type] == 2 && this.WHS.cubes[i, j, k].type < 155)
					{
						for (int l = 0; l < 6; l++)
						{
							bool flag = true;
							if (i + this.neighborX[l] > this.WHS.sizeX - 1 || i + this.neighborX[l] < 0 || j + this.neighborY[l] > this.WHS.sizeY - 1 || j + this.neighborY[l] < 0 || k + this.neighborZ[l] > this.WHS.sizeZ - 1 || k + this.neighborZ[l] < 0)
							{
								flag = false;
							}
							else if (this.WHS.cubes[i, j, k].type == 0)
							{
								flag = false;
							}
							else if (this.WHS.cubes[i + this.neighborX[l], j + this.neighborY[l], k + this.neighborZ[l]].data == 0 && this.WHS.cubesDrawTypes[(int)this.WHS.cubes[i + this.neighborX[l], j + this.neighborY[l], k + this.neighborZ[l]].type] == 0)
							{
								flag = false;
							}
							else if (this.WHS.cubes[i + this.neighborX[l], j + this.neighborY[l], k + this.neighborZ[l]].type == this.WHS.cubes[i, j, k].type)
							{
								flag = false;
							}
							if (flag)
							{
								num += 6;
							}
						}
					}
				}
			}
		}
		this.RecreateWaterMeshObjIfNeed(num);
		if (num == 0 && this.meshFilterWater == null)
		{
			return;
		}
		num = 0;
		Vector3[] vertices = this.meshFilterWater.mesh.vertices;
		Vector3[] normals = this.meshFilterWater.mesh.normals;
		Vector2[] uv = this.meshFilterWater.mesh.uv;
		int[] triangles = this.meshFilterWater.mesh.triangles;
		float[] array = new float[4];
		for (int m = (int)this.startCube.x; m < (int)this.endCube.x; m++)
		{
			for (int n = (int)this.startCube.y; n < (int)this.endCube.y; n++)
			{
				for (int num5 = (int)this.startCube.z; num5 < (int)this.endCube.z; num5++)
				{
					if (this.WHS.cubesDrawTypes[(int)this.WHS.cubes[m, n, num5].type] == 2 && this.WHS.cubes[m, n, num5].type < 155)
					{
						Vector3 a = new Vector3((float)m, (float)n, (float)num5);
						for (int num6 = 0; num6 < 4; num6++)
						{
							array[num6] = (float)this.WHS.cubes[m, n, num5].waterLevel;
						}
						int num7 = 1;
						int num8 = 1;
						int num9 = 1;
						int num10 = 1;
						for (int num11 = -1; num11 <= 1; num11++)
						{
							for (int num12 = -1; num12 <= 1; num12++)
							{
								if (num11 != 0 || num12 != 0)
								{
									if (this.WHS.IsInWorld(m + num11, n, num5 + num12) && this.WHS.cubes[m + num11, n, num5 + num12].type == 128)
									{
										if (num11 >= 0 && num12 >= 0 && array[0] / (float)num7 != (float)this.WHS.maxWaterLevel)
										{
											if (this.WHS.cubes[m + num11, n, num5 + num12].waterLevel != this.WHS.maxWaterLevel)
											{
												array[0] += (float)this.WHS.cubes[m + num11, n, num5 + num12].waterLevel;
												num7++;
											}
											else
											{
												num7++;
												array[0] = (float)this.WHS.maxWaterLevel * (float)num7;
											}
										}
										if (num11 <= 0 && num12 >= 0 && array[1] / (float)num8 != (float)this.WHS.maxWaterLevel)
										{
											if (this.WHS.cubes[m + num11, n, num5 + num12].waterLevel != this.WHS.maxWaterLevel)
											{
												array[1] += (float)this.WHS.cubes[m + num11, n, num5 + num12].waterLevel;
												num8++;
											}
											else
											{
												num8++;
												array[1] = (float)this.WHS.maxWaterLevel * (float)num8;
											}
										}
										if (num11 <= 0 && num12 <= 0 && array[2] / (float)num9 != (float)this.WHS.maxWaterLevel)
										{
											if (this.WHS.cubes[m + num11, n, num5 + num12].waterLevel != this.WHS.maxWaterLevel)
											{
												array[2] += (float)this.WHS.cubes[m + num11, n, num5 + num12].waterLevel;
												num9++;
											}
											else
											{
												num9++;
												array[2] = (float)this.WHS.maxWaterLevel * (float)num9;
											}
										}
										if (num11 >= 0 && num12 <= 0 && array[3] / (float)num10 != (float)this.WHS.maxWaterLevel)
										{
											if (this.WHS.cubes[m + num11, n, num5 + num12].waterLevel != this.WHS.maxWaterLevel)
											{
												array[3] += (float)this.WHS.cubes[m + num11, n, num5 + num12].waterLevel;
												num10++;
											}
											else
											{
												num10++;
												array[3] = (float)this.WHS.maxWaterLevel * (float)num10;
											}
										}
									}
								}
							}
						}
						array[0] = 1f - array[0] / ((float)num7 * (float)this.WHS.maxWaterLevel);
						array[1] = 1f - array[1] / ((float)num8 * (float)this.WHS.maxWaterLevel);
						array[2] = 1f - array[2] / ((float)num9 * (float)this.WHS.maxWaterLevel);
						array[3] = 1f - array[3] / ((float)num10 * (float)this.WHS.maxWaterLevel);
						if (this.WHS.IsInWorld(m, n - 1, num5) && this.WHS.IsInWorld(m, n + 1, num5) && this.WHS.cubes[m, n - 1, num5].type == 128 && this.WHS.cubes[m, n + 1, num5].type == 128)
						{
							if (num7 == 1)
							{
								array[0] = 0f;
							}
							if (num8 == 1)
							{
								array[1] = 0f;
							}
							if (num9 == 1)
							{
								array[2] = 0f;
							}
							if (num10 == 1)
							{
								array[3] = 0f;
							}
						}
						for (int num13 = 0; num13 < 6; num13++)
						{
							bool flag = true;
							if (m + this.neighborX[num13] > this.WHS.sizeX - 1 || m + this.neighborX[num13] < 0 || n + this.neighborY[num13] > this.WHS.sizeY - 1 || n + this.neighborY[num13] < 0 || num5 + this.neighborZ[num13] > this.WHS.sizeZ - 1 || num5 + this.neighborZ[num13] < 0)
							{
								flag = false;
							}
							else if (this.WHS.cubes[m, n, num5].type == 0)
							{
								flag = false;
							}
							else if (this.WHS.cubes[m + this.neighborX[num13], n + this.neighborY[num13], num5 + this.neighborZ[num13]].data == 0 && this.WHS.cubesDrawTypes[(int)this.WHS.cubes[m + this.neighborX[num13], n + this.neighborY[num13], num5 + this.neighborZ[num13]].type] == 0)
							{
								flag = false;
							}
							else if (this.WHS.cubes[m + this.neighborX[num13], n + this.neighborY[num13], num5 + this.neighborZ[num13]].type == this.WHS.cubes[m, n, num5].type)
							{
								flag = false;
							}
							if (flag)
							{
								vertices[num] = a + this.vertCoords[num13, 0] - base.transform.position;
								if (this.vertCoordsWaterAngle[num13, 0] != -1)
								{
									Vector3[] array2 = vertices;
									int num14 = num;
									array2[num14].y = array2[num14].y - array[this.vertCoordsWaterAngle[num13, 0]];
								}
								vertices[num + 1] = a + this.vertCoords[num13, 1] - base.transform.position;
								if (this.vertCoordsWaterAngle[num13, 1] != -1)
								{
									Vector3[] array3 = vertices;
									int num15 = num + 1;
									array3[num15].y = array3[num15].y - array[this.vertCoordsWaterAngle[num13, 1]];
								}
								vertices[num + 2] = a + this.vertCoords[num13, 2] - base.transform.position;
								if (this.vertCoordsWaterAngle[num13, 2] != -1)
								{
									Vector3[] array4 = vertices;
									int num16 = num + 2;
									array4[num16].y = array4[num16].y - array[this.vertCoordsWaterAngle[num13, 2]];
								}
								vertices[num + 3] = a + this.vertCoords[num13, 2] - base.transform.position;
								if (this.vertCoordsWaterAngle[num13, 2] != -1)
								{
									Vector3[] array5 = vertices;
									int num17 = num + 3;
									array5[num17].y = array5[num17].y - array[this.vertCoordsWaterAngle[num13, 2]];
								}
								vertices[num + 4] = a + this.vertCoords[num13, 3] - base.transform.position;
								if (this.vertCoordsWaterAngle[num13, 3] != -1)
								{
									Vector3[] array6 = vertices;
									int num18 = num + 4;
									array6[num18].y = array6[num18].y - array[this.vertCoordsWaterAngle[num13, 3]];
								}
								vertices[num + 5] = a + this.vertCoords[num13, 0] - base.transform.position;
								if (this.vertCoordsWaterAngle[num13, 0] != -1)
								{
									Vector3[] array7 = vertices;
									int num19 = num + 5;
									array7[num19].y = array7[num19].y - array[this.vertCoordsWaterAngle[num13, 0]];
								}
								uv[num2] = Vector2.zero;
								uv[num2 + 1] = Vector2.up;
								uv[num2 + 2] = Vector2.one;
								uv[num2 + 3] = Vector2.one;
								uv[num2 + 4] = Vector2.right;
								uv[num2 + 5] = Vector2.zero;
								normals[num3] = this.normalsSide[num13];
								normals[num3 + 1] = this.normalsSide[num13];
								normals[num3 + 2] = this.normalsSide[num13];
								normals[num3 + 3] = this.normalsSide[num13];
								normals[num3 + 4] = this.normalsSide[num13];
								normals[num3 + 5] = this.normalsSide[num13];
								triangles[num4] = num;
								triangles[num4 + 1] = num + 1;
								triangles[num4 + 2] = num + 2;
								triangles[num4 + 3] = num + 3;
								triangles[num4 + 4] = num + 4;
								triangles[num4 + 5] = num + 5;
								triangles[num4 + 6] = num;
								triangles[num4 + 7] = num + 2;
								triangles[num4 + 8] = num + 1;
								triangles[num4 + 9] = num + 3;
								triangles[num4 + 10] = num + 5;
								triangles[num4 + 11] = num + 4;
								num += 6;
								num2 += 6;
								num3 += 6;
								num4 += 12;
							}
						}
					}
				}
			}
		}
		for (int num20 = num; num20 < vertices.Length; num20++)
		{
			vertices[num20].x = 0f;
			vertices[num20].y = 0f;
			vertices[num20].z = 0f;
		}
		for (int num21 = num2; num21 < normals.Length; num21++)
		{
			normals[num21].x = 0f;
			normals[num21].y = 0f;
			normals[num21].z = 0f;
		}
		for (int num22 = num3; num22 < uv.Length; num22++)
		{
			uv[num22].x = 0f;
			uv[num22].y = 0f;
		}
		for (int num23 = num4; num23 < triangles.Length; num23++)
		{
			triangles[num23] = 0;
		}
		this.numWaterVerts = num;
		this.meshFilterWater.mesh.vertices = vertices;
		this.meshFilterWater.mesh.uv = uv;
		this.meshFilterWater.mesh.triangles = triangles;
		this.meshFilterWater.mesh.normals = normals;
		this.meshFilterWater.mesh.RecalculateBounds();
		this.meshFilterWater.mesh.Optimize();
		this.waterMeshObj.GetComponent<MeshCollider>().sharedMesh = null;
		this.waterMeshObj.GetComponent<MeshCollider>().sharedMesh = this.meshFilterWater.mesh;
	}

	public void DestroyBlock()
	{
		UnityEngine.Object.Destroy(this.meshFilterSolid.mesh);
		UnityEngine.Object.Destroy(this.solidMeshObj);
		UnityEngine.Object.Destroy(this.meshFilterTrans.mesh);
		UnityEngine.Object.Destroy(this.transSolidMeshObj);
		UnityEngine.Object.Destroy(this.meshFilterWater.mesh);
		UnityEngine.Object.Destroy(this.waterMeshObj);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void RefreshMeshes()
	{
		this.RefreshSolidMesh();
		this.RefreshTransparentSolidMesh();
		this.RefreshWaterMesh();
		this.RecountLight();
	}

	public void RecountLight()
	{
		int tickCount = Environment.TickCount;
		if (this.numSolidVerts != 0)
		{
			Vector3[] vertices = this.meshFilterSolid.sharedMesh.vertices;
			Vector3[] normals = this.meshFilterSolid.sharedMesh.normals;
			Color32[] array = this.meshFilterSolid.sharedMesh.colors32;
			if (array == null || array.Length == 0)
			{
				array = new Color32[vertices.Length];
			}
			int[] triangles = this.meshFilterSolid.sharedMesh.triangles;
			int num = triangles.Length;
			Vector3 b = new Vector3(0.5f, 0.5f, 0.5f);
			for (int i = 0; i < this.numSolidVerts; i++)
			{
				Vector3 vector = vertices[i] + b + normals[i] + base.transform.position;
				int num2 = (int)vector.x;
				int num3 = (int)Mathf.Ceil(vector.y);
				int num4 = (int)vector.z;
				if (num2 < this.WHS.sizeX && num2 >= 0 && num3 < this.WHS.sizeY && num3 >= 0 && num4 < this.WHS.sizeZ && num4 >= 0)
				{
					float num5 = (float)this.WHS.cubes[num2, num3, num4].sunLight;
					byte r = (byte)Mathf.Min(255f, num5 * this.WHS.sunInt * this.WHS.sunR + num5 * (1f - this.WHS.sunInt) * this.WHS.moonR + (float)this.WHS.cubes[num2, num3, num4].lightR);
					byte g = (byte)Mathf.Min(255f, num5 * this.WHS.sunInt * this.WHS.sunG + num5 * (1f - this.WHS.sunInt) * this.WHS.moonG + (float)this.WHS.cubes[num2, num3, num4].lightG);
					byte b2 = (byte)Mathf.Min(255f, num5 * this.WHS.sunInt * this.WHS.sunB + num5 * (1f - this.WHS.sunInt) * this.WHS.moonB + (float)this.WHS.cubes[num2, num3, num4].lightB);
					array[i] = new Color32(r, g, b2, byte.MaxValue);
				}
			}
			this.meshFilterSolid.sharedMesh.colors32 = array;
		}
		if (this.numTransVerts != 0)
		{
			Vector3[] vertices = this.meshFilterTrans.sharedMesh.vertices;
			Vector3[] normals = this.meshFilterTrans.sharedMesh.normals;
			Color32[] array = this.meshFilterTrans.sharedMesh.colors32;
			if (array == null || array.Length == 0)
			{
				array = new Color32[vertices.Length];
			}
			int[] triangles2 = this.meshFilterSolid.mesh.triangles;
			int num6 = triangles2.Length;
			Vector3 b3 = new Vector3(0.5f, 0.5f, 0.5f);
			for (int j = 0; j < this.numTransVerts; j++)
			{
				Vector3 vector2 = vertices[j] + b3 + normals[j] + base.transform.position;
				int num7 = (int)vector2.x;
				int num8 = (int)Mathf.Ceil(vector2.y);
				int num9 = (int)vector2.z;
				if (num7 < this.WHS.sizeX && num7 >= 0 && num8 < this.WHS.sizeY && num8 >= 0 && num9 < this.WHS.sizeZ && num9 >= 0)
				{
					float num10 = (float)this.WHS.cubes[num7, num8, num9].sunLight;
					byte r2 = (byte)Mathf.Min(255f, num10 * this.WHS.sunInt * this.WHS.sunR + num10 * (1f - this.WHS.sunInt) * this.WHS.moonR + (float)this.WHS.cubes[num7, num8, num9].lightR);
					byte g2 = (byte)Mathf.Min(255f, num10 * this.WHS.sunInt * this.WHS.sunG + num10 * (1f - this.WHS.sunInt) * this.WHS.moonG + (float)this.WHS.cubes[num7, num8, num9].lightG);
					byte b4 = (byte)Mathf.Min(255f, num10 * this.WHS.sunInt * this.WHS.sunB + num10 * (1f - this.WHS.sunInt) * this.WHS.moonB + (float)this.WHS.cubes[num7, num8, num9].lightB);
					array[j] = new Color32(r2, g2, b4, byte.MaxValue);
				}
			}
			this.meshFilterTrans.mesh.colors32 = array;
		}
		if (this.numWaterVerts != 0)
		{
			Vector3[] vertices = this.meshFilterWater.mesh.vertices;
			Vector3[] normals = this.meshFilterWater.mesh.normals;
			Color32[] array = this.meshFilterWater.mesh.colors32;
			for (int k = 0; k < this.numWaterVerts; k += 6)
			{
				Vector3 b5 = normals[k];
				Vector3 vector3 = (vertices[k] + vertices[k + 2] + b5) / 2f + base.transform.position;
				byte r3 = (byte)Mathf.Min(255f, (float)this.WHS.cubes[Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z)].sunLight * this.WHS.sunInt * this.WHS.sunR + (float)this.WHS.cubes[Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z)].sunLight * (1f - this.WHS.sunInt) * this.WHS.moonR + (float)this.WHS.cubes[(int)vector3.x, (int)vector3.y, (int)vector3.z].lightR);
				byte g3 = (byte)Mathf.Min(255f, (float)this.WHS.cubes[Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z)].sunLight * this.WHS.sunInt * this.WHS.sunG + (float)this.WHS.cubes[Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z)].sunLight * (1f - this.WHS.sunInt) * this.WHS.moonG + (float)this.WHS.cubes[(int)vector3.x, (int)vector3.y, (int)vector3.z].lightG);
				byte b6 = (byte)Mathf.Min(255f, (float)this.WHS.cubes[Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z)].sunLight * this.WHS.sunInt * this.WHS.sunB + (float)this.WHS.cubes[Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z)].sunLight * (1f - this.WHS.sunInt) * this.WHS.moonB + (float)this.WHS.cubes[(int)vector3.x, (int)vector3.y, (int)vector3.z].lightB);
				array[k] = new Color32(r3, g3, b6, byte.MaxValue);
				array[k + 1] = new Color32(r3, g3, b6, byte.MaxValue);
				array[k + 2] = new Color32(r3, g3, b6, byte.MaxValue);
				array[k + 3] = new Color32(r3, g3, b6, byte.MaxValue);
				array[k + 4] = new Color32(r3, g3, b6, byte.MaxValue);
				array[k + 5] = new Color32(r3, g3, b6, byte.MaxValue);
			}
			this.meshFilterWater.sharedMesh.colors32 = array;
		}
		int tickCount2 = Environment.TickCount;
	}

	protected static GeometryGenerator _geometryGenerator = new GeometryGenerator();

	private WorldHolderScript WHS;

	private bool isInitialized;

	private Vector3 startCube;

	private Vector3 endCube;

	public GameObject solidMeshObjPrefab;

	public GameObject transSolidMeshObjPrefab;

	public GameObject waterMeshObjPrefab;

	public GameObject solidMeshObj;

	public GameObject transSolidMeshObj;

	public GameObject waterMeshObj;

	private ObjectsHolderScript OH;

	private int numSolidVerts;

	private int numTransVerts;

	private int numWaterVerts;

	private int[] neighborX = new int[]
	{
		0,
		0,
		0,
		0,
		1,
		-1
	};

	private int[] neighborY;

	private int[] neighborZ;

	private Vector3[] normalsSide;

	private int[] sides3;

	private Vector3[,] vertCoords;

	private int[,] vertCoordsWaterAngle;

	private MeshFilter meshFilterSolid;

	private MeshFilter meshFilterTrans;

	private MeshFilter meshFilterWater;

	private MeshFilter meshFilter;
}
