using System;
using UnityEngine;

namespace kube.map
{
	public class GeometryGenerator
	{
		public GeometryGenerator()
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

		public void Init()
		{
			this.WHS = Kube.WHS;
			if (this.isInitialized)
			{
				return;
			}
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

		private void Update()
		{
		}

		public void SetBlock(Vector3 pos, Vector3 _startCube, Vector3 _endCube)
		{
			this.startCube.x = Mathf.Min(_startCube.x, _endCube.x);
			this.startCube.y = Mathf.Min(_startCube.y, _endCube.y);
			this.startCube.z = Mathf.Min(_startCube.z, _endCube.z);
			this.endCube.x = Mathf.Max(_startCube.x, _endCube.x);
			this.endCube.y = Mathf.Max(_startCube.y, _endCube.y);
			this.endCube.z = Mathf.Max(_startCube.z, _endCube.z);
			this.transformPosition = pos;
		}

		public void collect(CubeDrawTypes cdttype)
		{
			this.cdttype = cdttype;
			this.numVerts = 0;
			this.numTriangles = 0;
			this.Init();
			for (int i = (int)this.startCube.x; i < (int)this.endCube.x; i++)
			{
				for (int j = (int)this.startCube.y; j < (int)this.endCube.y; j++)
				{
					for (int k = (int)this.startCube.z; k < (int)this.endCube.z; k++)
					{
						int type = (int)this.WHS.cubes[i, j, k].type;
						if (type < 155 && this.WHS.cubesDrawTypes[type] == (int)cdttype)
						{
							int num = 0;
							int data = (int)this.WHS.cubes[i, j, k].data;
							for (int l = 0; l < 6; l++)
							{
								bool flag = true;
								int num2 = i + this.neighborX[l];
								int num3 = j + this.neighborY[l];
								int num4 = k + this.neighborZ[l];
								if (num2 <= this.WHS.sizeX - 1 && num2 >= 0 && num3 <= this.WHS.sizeY - 1 && num3 >= 0 && num4 <= this.WHS.sizeZ - 1 && num4 >= 0)
								{
									if (type != 0)
									{
										int data2 = (int)this.WHS.cubes[num2, num3, num4].data;
										int type2 = (int)this.WHS.cubes[num2, num3, num4].type;
										if (type2 < 155 && GeometryGenerator.neighbor_has_side[data2, l] && data2 == data)
										{
											if (this.WHS.cubesDrawTypes[type2] == 0)
											{
												flag = false;
											}
											else if (type2 == type)
											{
												flag = false;
											}
										}
										if (flag)
										{
											num |= 1 << l;
											if (data == 0)
											{
												this.numVerts += 6;
												this.numTriangles += 6;
											}
											else if (data != 0)
											{
												this.numVerts += GeometryGenerator.geomNumVerts[data, l];
												this.numTriangles += GeometryGenerator.geomNumTris[data, l];
											}
										}
									}
								}
							}
							if (num != 0)
							{
								this.numVerts += GeometryGenerator.geomNumVerts[data, 6];
								this.numTriangles += GeometryGenerator.geomNumTris[data, 6];
							}
						}
					}
				}
			}
		}

		private void AllocBuffers()
		{
			if (this.numVerts <= this._size && this.numTriangles <= this._trisize && this._trisize - this.numTriangles <= 300)
			{
				return;
			}
			int num = this.numTriangles + 300;
			int num2 = this.numVerts + 300;
			this._trisize = num;
			this._size = num2;
			this.MeshVertices = new Vector3[num2];
			this.MeshNormals = new Vector3[num2];
			this.MeshUv = new Vector2[num2];
			this.MeshTriangles = new int[num];
		}

		private void AddGeometry(Mesh mesh, int x, int y, int z, GeometryGenerator.GeomAddInfo ai)
		{
			Vector3 a = new Vector3((float)x, (float)y, (float)z) - this.transformPosition;
			Vector3[] meshVertices = this.MeshVertices;
			Vector3[] meshNormals = this.MeshNormals;
			Vector2[] meshUv = this.MeshUv;
			int[] meshTriangles = this.MeshTriangles;
			int num = 0;
			int data = (int)this.WHS.cubes[x, y, z].data;
			int[][] indexes = GeometryGenerator.g4_indicies_3;
			Vector3[][] points = GeometryGenerator.g4_points_3;
			Vector3[][] array = null;
			Vector2[][] array2 = null;
			if (data == 1)
			{
				indexes = GeometryGenerator.g1_indicies;
				points = GeometryGenerator.g1_points;
				array2 = GeometryGenerator.g1_uv;
			}
			else if (data == 2)
			{
				indexes = GeometryGenerator.g2_indicies;
				points = GeometryGenerator.g2_points;
				array2 = GeometryGenerator.g2_uv;
			}
			else if (data == 3)
			{
				indexes = GeometryGenerator.g3_indicies;
				points = GeometryGenerator.g3_points;
				array = GeometryGenerator.g3_normals;
				array2 = GeometryGenerator.g3_uv;
			}
			else if ((data & 4) == 4)
			{
				int num2 = data & 3;
				indexes = GeometryGenerator.geomInfo[num2].indexes;
				points = GeometryGenerator.geomInfo[num2].points;
				array2 = GeometryGenerator.geomInfo[num2].uv;
			}
			int type = (int)this.WHS.cubes[x, y, z].type;
			int num6;
			int num7;
			for (int i = 0; i < 6; i++)
			{
				bool flag = true;
				int num3 = x + this.neighborX[i];
				int num4 = y + this.neighborY[i];
				int num5 = z + this.neighborZ[i];
				if (num3 <= this.WHS.sizeX - 1 && num3 >= 0 && num4 <= this.WHS.sizeY - 1 && num4 >= 0 && num5 <= this.WHS.sizeZ - 1 && num5 >= 0)
				{
					if (type != 0)
					{
						int data2 = (int)this.WHS.cubes[num3, num4, num5].data;
						int type2 = (int)this.WHS.cubes[num3, num4, num5].type;
						if (type2 < 155 && GeometryGenerator.neighbor_has_side[data2, i] && data2 == data)
						{
							if (this.WHS.cubesDrawTypes[type2] == 0)
							{
								flag = false;
							}
							else if (type2 == type)
							{
								flag = false;
							}
						}
						if (flag)
						{
							num |= 1 << i;
							num6 = GeometryGenerator.geomNumVerts[data, i];
							if (num6 != 0)
							{
								int verts = ai.verts;
								for (int j = 0; j < num6; j++)
								{
									meshVertices[ai.verts] = a + points[i][j];
									if (array != null && array[i] != null)
									{
										meshNormals[ai.verts] = array[i][j];
									}
									else
									{
										meshNormals[ai.verts] = this.normalsSide[i];
									}
									if (array2 != null)
									{
										Vector2 a2 = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[(int)this.WHS.cubes[x, y, z].type, this.sides3[i]], 1];
										meshUv[ai.verts] = a2 + array2[i][j];
									}
									else
									{
										meshUv[ai.verts] = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[(int)this.WHS.cubes[x, y, z].type, this.sides3[i]], j];
									}
									ai.verts++;
								}
								num7 = indexes[i].Length;
								for (int k = 0; k < num7; k++)
								{
									meshTriangles[ai.indexes++] = verts + indexes[i][k];
								}
							}
						}
					}
				}
			}
			if (num == 0)
			{
				return;
			}
			int num8 = 6;
			num6 = GeometryGenerator.geomNumVerts[data, num8];
			if (num6 == 0)
			{
				return;
			}
			int verts2 = ai.verts;
			for (int l = 0; l < num6; l++)
			{
				meshVertices[ai.verts] = a + points[num8][l];
				meshNormals[ai.verts] = this.normalsSide[0];
				if (array2 != null)
				{
					Vector2 a3 = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[(int)this.WHS.cubes[x, y, z].type, 0], 1];
					meshUv[ai.verts] = a3 + array2[num8][l];
				}
				else
				{
					meshUv[ai.verts] = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[(int)this.WHS.cubes[x, y, z].type, 0], l % 4];
				}
				ai.verts++;
			}
			num7 = indexes[num8].Length;
			for (int m = 0; m < num7; m++)
			{
				meshTriangles[ai.indexes++] = verts2 + indexes[num8][m];
			}
		}

		public void RefreshMesh(Mesh mesh)
		{
			this._mesh = mesh;
			this.AllocBuffers();
			this.numTriangles = 0;
			this.numUVs = 0;
			this.numNormals = 0;
			if (this.numVerts == 0 && this._mesh == null)
			{
				return;
			}
			this.numVerts = 0;
			Vector3[] meshVertices = this.MeshVertices;
			Vector3[] meshNormals = this.MeshNormals;
			Vector2[] meshUv = this.MeshUv;
			int[] meshTriangles = this.MeshTriangles;
			GeometryGenerator.GeomAddInfo geomAddInfo = new GeometryGenerator.GeomAddInfo();
			for (int i = (int)this.startCube.x; i < (int)this.endCube.x; i++)
			{
				for (int j = (int)this.startCube.y; j < (int)this.endCube.y; j++)
				{
					for (int k = (int)this.startCube.z; k < (int)this.endCube.z; k++)
					{
						int type = (int)this.WHS.cubes[i, j, k].type;
						if (type < 155 && this.WHS.cubesDrawTypes[type] == (int)this.cdttype)
						{
							int data = (int)this.WHS.cubes[i, j, k].data;
							if (data != 0)
							{
								geomAddInfo.indexes = this.numTriangles;
								geomAddInfo.verts = this.numVerts;
								this.AddGeometry(this._mesh, i, j, k, geomAddInfo);
								this.numTriangles = geomAddInfo.indexes;
								this.numVerts = geomAddInfo.verts;
								this.numUVs = this.numVerts;
								this.numNormals = this.numVerts;
							}
							else
							{
								Vector3 a = new Vector3((float)i, (float)j, (float)k) - this.transformPosition;
								for (int l = 0; l < 6; l++)
								{
									bool flag = true;
									int num = i + this.neighborX[l];
									int num2 = j + this.neighborY[l];
									int num3 = k + this.neighborZ[l];
									if (num <= this.WHS.sizeX - 1 && num >= 0 && num2 <= this.WHS.sizeY - 1 && num2 >= 0 && num3 <= this.WHS.sizeZ - 1 && num3 >= 0)
									{
										if (type != 0)
										{
											int data2 = (int)this.WHS.cubes[num, num2, num3].data;
											int type2 = (int)this.WHS.cubes[num, num2, num3].type;
											if (type2 < 155 && GeometryGenerator.neighbor_has_side[data2, l] && data2 == data)
											{
												if (this.WHS.cubesDrawTypes[type2] == 0)
												{
													flag = false;
												}
												else if (type2 == type)
												{
													flag = false;
												}
											}
											if (flag)
											{
												Vector3[,] array = this.vertCoords;
												meshVertices[this.numVerts] = a + array[l, 0];
												meshVertices[this.numVerts + 1] = a + array[l, 1];
												meshVertices[this.numVerts + 2] = a + array[l, 2];
												meshVertices[this.numVerts + 3] = a + array[l, 2];
												meshVertices[this.numVerts + 4] = a + array[l, 3];
												meshVertices[this.numVerts + 5] = a + array[l, 0];
												meshUv[this.numUVs] = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[type, this.sides3[l]], 0];
												meshUv[this.numUVs + 1] = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[type, this.sides3[l]], 1];
												meshUv[this.numUVs + 2] = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[type, this.sides3[l]], 2];
												meshUv[this.numUVs + 3] = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[type, this.sides3[l]], 2];
												meshUv[this.numUVs + 4] = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[type, this.sides3[l]], 3];
												meshUv[this.numUVs + 5] = this.WHS.cubesTexUV[this.WHS.cubesSidesTex[type, this.sides3[l]], 0];
												meshNormals[this.numNormals] = this.normalsSide[l];
												meshNormals[this.numNormals + 1] = this.normalsSide[l];
												meshNormals[this.numNormals + 2] = this.normalsSide[l];
												meshNormals[this.numNormals + 3] = this.normalsSide[l];
												meshNormals[this.numNormals + 4] = this.normalsSide[l];
												meshNormals[this.numNormals + 5] = this.normalsSide[l];
												meshTriangles[this.numTriangles] = this.numVerts;
												meshTriangles[this.numTriangles + 1] = this.numVerts + 1;
												meshTriangles[this.numTriangles + 2] = this.numVerts + 2;
												meshTriangles[this.numTriangles + 3] = this.numVerts + 3;
												meshTriangles[this.numTriangles + 4] = this.numVerts + 4;
												meshTriangles[this.numTriangles + 5] = this.numVerts + 5;
												this.numVerts += 6;
												this.numUVs += 6;
												this.numNormals += 6;
												this.numTriangles += 6;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			for (int m = this.numVerts; m < meshVertices.Length; m++)
			{
				meshVertices[m].x = 0f;
				meshVertices[m].y = 0f;
				meshVertices[m].z = 0f;
			}
			for (int n = this.numVerts; n < meshNormals.Length; n++)
			{
				meshNormals[n].x = 0f;
				meshNormals[n].y = 0f;
				meshNormals[n].z = 0f;
			}
			for (int num4 = this.numVerts; num4 < meshUv.Length; num4++)
			{
				meshUv[num4].x = 0f;
				meshUv[num4].y = 0f;
			}
			for (int num5 = this.numTriangles; num5 < meshTriangles.Length; num5++)
			{
				meshTriangles[num5] = 0;
			}
			this.numSolidVerts = this.numVerts;
			this._mesh.Clear();
			this._mesh.vertices = this.MeshVertices;
			this._mesh.normals = this.MeshNormals;
			this._mesh.uv = this.MeshUv;
			this._mesh.triangles = this.MeshTriangles;
			this._mesh.RecalculateBounds();
			this._mesh.Optimize();
		}

		private Vector3 transformPosition;

		private WorldHolderScript WHS;

		private bool isInitialized;

		private Vector3 startCube;

		private Vector3 endCube;

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

		protected Vector3[] MeshVertices;

		protected Vector3[] MeshNormals;

		protected Color32[] MeshColors32;

		protected Vector2[] MeshUv;

		protected int[] MeshTriangles;

		private static Vector2[] side_uv = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector2(0f, 0f),
			new Vector3(0.125f, 0f),
			new Vector3(0.125f, -0.125f)
		};

		private static Vector2[][] g1_uv = new Vector2[][]
		{
			new Vector2[0],
			GeometryGenerator.side_uv,
			new Vector2[]
			{
				new Vector3(0f, -0.125f),
				new Vector2(0f, -0.093f),
				new Vector3(0.125f, -0.093f),
				new Vector3(0.125f, -0.125f)
			},
			new Vector2[]
			{
				new Vector3(0f, -0.125f),
				new Vector2(0f, -0.093f),
				new Vector3(0.125f, -0.093f),
				new Vector3(0.125f, -0.125f)
			},
			new Vector2[]
			{
				new Vector3(0f, -0.125f),
				new Vector2(0f, -0.093f),
				new Vector3(0.125f, -0.093f),
				new Vector3(0.125f, -0.125f)
			},
			new Vector2[]
			{
				new Vector3(0f, -0.125f),
				new Vector2(0f, -0.093f),
				new Vector3(0.125f, -0.093f),
				new Vector3(0.125f, -0.125f)
			},
			GeometryGenerator.side_uv
		};

		private static Vector3[][] g1_points = new Vector3[][]
		{
			new Vector3[0],
			new Vector3[]
			{
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f)
			},
			new Vector3[]
			{
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(0.5f, -0.25f, 0.5f),
				new Vector3(-0.5f, -0.25f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f)
			},
			new Vector3[]
			{
				new Vector3(-0.5f, -0.5f, -0.5f),
				new Vector3(-0.5f, -0.25f, -0.5f),
				new Vector3(0.5f, -0.25f, -0.5f),
				new Vector3(0.5f, -0.5f, -0.5f)
			},
			new Vector3[]
			{
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, -0.25f, -0.5f),
				new Vector3(0.5f, -0.25f, 0.5f),
				new Vector3(0.5f, -0.5f, 0.5f)
			},
			new Vector3[]
			{
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.25f, 0.5f),
				new Vector3(-0.5f, -0.25f, -0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f)
			},
			new Vector3[]
			{
				new Vector3(-0.5f, -0.25f, -0.5f),
				new Vector3(-0.5f, -0.25f, 0.5f),
				new Vector3(0.5f, -0.25f, 0.5f),
				new Vector3(0.5f, -0.25f, -0.5f)
			}
		};

		private static int[][] g1_indicies = new int[][]
		{
			new int[0],
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			}
		};

		private static Vector2[][] g2_uv = new Vector2[][]
		{
			new Vector2[0],
			GeometryGenerator.side_uv,
			new Vector2[]
			{
				new Vector3(0f, -0.125f),
				new Vector2(0f, -0.093f),
				new Vector3(0.125f, -0.093f),
				new Vector3(0.125f, -0.125f)
			},
			new Vector2[]
			{
				new Vector3(0f, -0.125f),
				new Vector2(0f, -0.093f),
				new Vector3(0.125f, -0.093f),
				new Vector3(0.125f, -0.125f)
			},
			new Vector2[]
			{
				new Vector3(0f, -0.125f),
				new Vector2(0f, -0.093f),
				new Vector3(0.125f, -0.093f),
				new Vector3(0.125f, -0.125f)
			},
			new Vector2[]
			{
				new Vector3(0f, -0.125f),
				new Vector2(0f, -0.093f),
				new Vector3(0.125f, -0.093f),
				new Vector3(0.125f, -0.125f)
			},
			GeometryGenerator.side_uv
		};

		private static Vector3[][] g2_points = new Vector3[][]
		{
			new Vector3[0],
			new Vector3[]
			{
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f)
			},
			new Vector3[]
			{
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(0.5f, 0f, 0.5f),
				new Vector3(-0.5f, 0f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f)
			},
			new Vector3[]
			{
				new Vector3(-0.5f, -0.5f, -0.5f),
				new Vector3(-0.5f, 0f, -0.5f),
				new Vector3(0.5f, 0f, -0.5f),
				new Vector3(0.5f, -0.5f, -0.5f)
			},
			new Vector3[]
			{
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, 0f, -0.5f),
				new Vector3(0.5f, 0f, 0.5f),
				new Vector3(0.5f, -0.5f, 0.5f)
			},
			new Vector3[]
			{
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, 0f, 0.5f),
				new Vector3(-0.5f, 0f, -0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f)
			},
			new Vector3[]
			{
				new Vector3(-0.5f, 0f, -0.5f),
				new Vector3(-0.5f, 0f, 0.5f),
				new Vector3(0.5f, 0f, 0.5f),
				new Vector3(0.5f, 0f, -0.5f)
			}
		};

		private static int[][] g2_indicies = new int[][]
		{
			new int[0],
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			}
		};

		private static Vector2[][] g3_uv = new Vector2[][]
		{
			new Vector2[]
			{
				new Vector3(0.032f, -0.093f),
				new Vector2(0.032f, -0.032f),
				new Vector3(0.093f, -0.032f),
				new Vector3(0.093f, -0.093f)
			},
			new Vector2[]
			{
				new Vector3(0.032f, -0.093f),
				new Vector2(0.032f, -0.032f),
				new Vector3(0.093f, -0.032f),
				new Vector3(0.093f, -0.093f)
			},
			new Vector2[0],
			new Vector2[0],
			new Vector2[0],
			new Vector2[0],
			new Vector2[]
			{
				new Vector3(0.032f, -0.125f),
				new Vector2(0.032f, 0f),
				new Vector3(0.093f, 0f),
				new Vector3(0.093f, -0.125f),
				new Vector3(0.032f, -0.125f),
				new Vector2(0.032f, 0f),
				new Vector3(0.093f, 0f),
				new Vector3(0.093f, -0.125f),
				new Vector3(0.032f, -0.125f),
				new Vector2(0.032f, 0f),
				new Vector3(0.093f, 0f),
				new Vector3(0.093f, -0.125f),
				new Vector3(0.032f, -0.125f),
				new Vector2(0.032f, 0f),
				new Vector3(0.093f, 0f),
				new Vector3(0.093f, -0.125f)
			}
		};

		private static Vector3[][] g3_points = new Vector3[][]
		{
			new Vector3[]
			{
				new Vector3(-0.25f, 0.5f, -0.25f),
				new Vector3(-0.25f, 0.5f, 0.25f),
				new Vector3(0.25f, 0.5f, 0.25f),
				new Vector3(0.25f, 0.5f, -0.25f)
			},
			new Vector3[]
			{
				new Vector3(0.25f, -0.5f, -0.25f),
				new Vector3(0.25f, -0.5f, 0.25f),
				new Vector3(-0.25f, -0.5f, 0.25f),
				new Vector3(-0.25f, -0.5f, -0.25f)
			},
			new Vector3[0],
			new Vector3[0],
			new Vector3[0],
			new Vector3[0],
			new Vector3[]
			{
				new Vector3(0.25f, -0.5f, 0.25f),
				new Vector3(0.25f, 0.5f, 0.25f),
				new Vector3(-0.25f, 0.5f, 0.25f),
				new Vector3(-0.25f, -0.5f, 0.25f),
				new Vector3(-0.25f, -0.5f, -0.25f),
				new Vector3(-0.25f, 0.5f, -0.25f),
				new Vector3(0.25f, 0.5f, -0.25f),
				new Vector3(0.25f, -0.5f, -0.25f),
				new Vector3(0.25f, -0.5f, -0.25f),
				new Vector3(0.25f, 0.5f, -0.25f),
				new Vector3(0.25f, 0.5f, 0.25f),
				new Vector3(0.25f, -0.5f, 0.25f),
				new Vector3(-0.25f, -0.5f, 0.25f),
				new Vector3(-0.25f, 0.5f, 0.25f),
				new Vector3(-0.25f, 0.5f, -0.25f),
				new Vector3(-0.25f, -0.5f, -0.25f)
			}
		};

		private static Vector3[][] g3_normals = new Vector3[][]
		{
			default(Vector3[]),
			default(Vector3[]),
			default(Vector3[]),
			default(Vector3[]),
			default(Vector3[]),
			default(Vector3[]),
			new Vector3[]
			{
				new Vector3(0f, 0f, 1f),
				new Vector3(0f, 0f, 1f),
				new Vector3(0f, 0f, 1f),
				new Vector3(0f, 0f, 1f),
				new Vector3(0f, 0f, -1f),
				new Vector3(0f, 0f, -1f),
				new Vector3(0f, 0f, -1f),
				new Vector3(0f, 0f, -1f),
				new Vector3(1f, 0f, 0f),
				new Vector3(1f, 0f, 0f),
				new Vector3(1f, 0f, 0f),
				new Vector3(1f, 0f, 0f),
				new Vector3(-1f, 0f, 0f),
				new Vector3(-1f, 0f, 0f),
				new Vector3(-1f, 0f, 0f),
				new Vector3(-1f, 0f, 0f)
			}
		};

		private static int[][] g3_indicies = new int[][]
		{
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[0],
			new int[0],
			new int[0],
			new int[0],
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0,
				4,
				5,
				6,
				6,
				7,
				4,
				8,
				9,
				10,
				10,
				11,
				8,
				12,
				13,
				14,
				14,
				15,
				12
			}
		};

		public static int[][] g4_indicies_3 = new int[][]
		{
			new int[0],
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[0],
			new int[]
			{
				0,
				1,
				2
			},
			new int[]
			{
				0,
				2,
				1
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			}
		};

		public static Vector3[][] g4_points_3 = new Vector3[][]
		{
			new Vector3[0],
			new Vector3[]
			{
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f)
			},
			new Vector3[]
			{
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(0.5f, 0.5f, 0.5f),
				new Vector3(-0.5f, 0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f)
			},
			new Vector3[0],
			new Vector3[]
			{
				new Vector3(0.5f, 0.5f, 0.5f),
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(0.5f, -0.5f, -0.5f)
			},
			new Vector3[]
			{
				new Vector3(-0.5f, 0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f)
			},
			new Vector3[]
			{
				new Vector3(-0.5f, -0.5f, -0.5f),
				new Vector3(-0.5f, 0.5f, 0.5f),
				new Vector3(0.5f, 0.5f, 0.5f),
				new Vector3(0.5f, -0.5f, -0.5f)
			}
		};

		private static Vector2[] g4_side_uv = new Vector2[]
		{
			new Vector3(0f, 0f),
			new Vector2(0f, -0.125f),
			new Vector3(0.125f, -0.125f)
		};

		private static Vector2[][] g4_uv_3 = new Vector2[][]
		{
			GeometryGenerator.side_uv,
			GeometryGenerator.side_uv,
			GeometryGenerator.side_uv,
			GeometryGenerator.side_uv,
			GeometryGenerator.g4_side_uv,
			GeometryGenerator.g4_side_uv,
			GeometryGenerator.side_uv
		};

		public static int[][] g4_indicies_1 = new int[][]
		{
			new int[0],
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				1,
				2
			},
			new int[]
			{
				0,
				2,
				1
			},
			new int[0],
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			}
		};

		public static Vector3[][] g4_points_1 = new Vector3[][]
		{
			new Vector3[0],
			new Vector3[]
			{
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f)
			},
			new Vector3[]
			{
				new Vector3(-0.5f, 0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(0.5f, -0.5f, 0.5f)
			},
			new Vector3[]
			{
				new Vector3(-0.5f, 0.5f, -0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, -0.5f)
			},
			new Vector3[0],
			new Vector3[]
			{
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, 0.5f, 0.5f),
				new Vector3(-0.5f, 0.5f, -0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f)
			},
			new Vector3[]
			{
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(-0.5f, 0.5f, -0.5f),
				new Vector3(-0.5f, 0.5f, 0.5f),
				new Vector3(0.5f, -0.5f, 0.5f)
			}
		};

		private static Vector2[][] g4_uv_1 = new Vector2[][]
		{
			GeometryGenerator.side_uv,
			GeometryGenerator.side_uv,
			GeometryGenerator.g4_side_uv,
			GeometryGenerator.g4_side_uv,
			GeometryGenerator.side_uv,
			GeometryGenerator.side_uv,
			GeometryGenerator.side_uv
		};

		public static int[][] g4_indicies_0 = new int[][]
		{
			new int[0],
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[0],
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				2,
				1
			},
			new int[]
			{
				0,
				1,
				2
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			}
		};

		public static Vector3[][] g4_points_0 = new Vector3[][]
		{
			new Vector3[0],
			new Vector3[]
			{
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f)
			},
			new Vector3[0],
			new Vector3[]
			{
				new Vector3(-0.5f, -0.5f, -0.5f),
				new Vector3(-0.5f, 0.5f, -0.5f),
				new Vector3(0.5f, 0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, -0.5f)
			},
			new Vector3[]
			{
				new Vector3(0.5f, 0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, 0.5f)
			},
			new Vector3[]
			{
				new Vector3(-0.5f, 0.5f, -0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f)
			},
			new Vector3[]
			{
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(0.5f, 0.5f, -0.5f),
				new Vector3(-0.5f, 0.5f, -0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f)
			}
		};

		private static Vector2[][] g4_uv_0 = new Vector2[][]
		{
			GeometryGenerator.side_uv,
			GeometryGenerator.side_uv,
			GeometryGenerator.side_uv,
			GeometryGenerator.side_uv,
			GeometryGenerator.g4_side_uv,
			GeometryGenerator.g4_side_uv,
			GeometryGenerator.side_uv
		};

		public static int[][] g4_indicies_2 = new int[][]
		{
			new int[0],
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[]
			{
				0,
				2,
				1
			},
			new int[]
			{
				0,
				1,
				2
			},
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			},
			new int[0],
			new int[]
			{
				0,
				1,
				2,
				2,
				3,
				0
			}
		};

		public static Vector3[][] g4_points_2 = new Vector3[][]
		{
			new Vector3[0],
			new Vector3[]
			{
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f)
			},
			new Vector3[]
			{
				new Vector3(0.5f, 0.5f, 0.5f),
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f)
			},
			new Vector3[]
			{
				new Vector3(0.5f, 0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f)
			},
			new Vector3[]
			{
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, 0.5f, -0.5f),
				new Vector3(0.5f, 0.5f, 0.5f),
				new Vector3(0.5f, -0.5f, 0.5f)
			},
			new Vector3[0],
			new Vector3[]
			{
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(0.5f, 0.5f, 0.5f),
				new Vector3(0.5f, 0.5f, -0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f)
			}
		};

		private static Vector2[][] g4_uv_2 = new Vector2[][]
		{
			GeometryGenerator.side_uv,
			GeometryGenerator.side_uv,
			GeometryGenerator.g4_side_uv,
			GeometryGenerator.g4_side_uv,
			GeometryGenerator.side_uv,
			GeometryGenerator.side_uv,
			GeometryGenerator.side_uv
		};

		public static int[,] geomNumVerts = new int[,]
		{
			{
				6,
				6,
				6,
				6,
				6,
				6,
				0
			},
			{
				0,
				4,
				4,
				4,
				4,
				4,
				4
			},
			{
				0,
				4,
				4,
				4,
				4,
				4,
				4
			},
			{
				4,
				4,
				0,
				0,
				0,
				0,
				16
			},
			{
				0,
				4,
				0,
				4,
				3,
				3,
				4
			},
			{
				0,
				4,
				3,
				3,
				0,
				4,
				4
			},
			{
				0,
				4,
				3,
				3,
				4,
				0,
				4
			},
			{
				0,
				4,
				4,
				0,
				3,
				3,
				4
			}
		};

		private static int[,] geomNumTris = new int[,]
		{
			{
				6,
				6,
				6,
				6,
				6,
				6,
				0
			},
			{
				0,
				6,
				6,
				6,
				6,
				6,
				6
			},
			{
				0,
				6,
				6,
				6,
				6,
				6,
				6
			},
			{
				6,
				6,
				0,
				0,
				0,
				0,
				24
			},
			{
				0,
				6,
				0,
				6,
				3,
				3,
				6
			},
			{
				0,
				6,
				3,
				3,
				0,
				6,
				6
			},
			{
				0,
				6,
				3,
				3,
				6,
				0,
				6
			},
			{
				0,
				6,
				6,
				0,
				6,
				6,
				6
			}
		};

		private static bool[,] neighbor_has_side = new bool[,]
		{
			{
				true,
				true,
				true,
				true,
				true,
				true,
				true
			},
			{
				true,
				false,
				true,
				true,
				true,
				true,
				true
			},
			{
				true,
				false,
				true,
				true,
				true,
				true,
				true
			},
			{
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				true,
				false,
				true,
				false,
				true,
				true,
				false
			},
			{
				true,
				false,
				true,
				true,
				true,
				false,
				false
			},
			{
				true,
				false,
				true,
				true,
				false,
				true,
				false
			},
			{
				true,
				false,
				false,
				true,
				true,
				true,
				false
			}
		};

		private static GeometryGenerator.GeomInfo[] geomInfo = new GeometryGenerator.GeomInfo[]
		{
			new GeometryGenerator.GeomInfo(GeometryGenerator.g4_indicies_0, GeometryGenerator.g4_points_0, GeometryGenerator.g4_uv_0, null),
			new GeometryGenerator.GeomInfo(GeometryGenerator.g4_indicies_1, GeometryGenerator.g4_points_1, GeometryGenerator.g4_uv_1, null),
			new GeometryGenerator.GeomInfo(GeometryGenerator.g4_indicies_2, GeometryGenerator.g4_points_2, GeometryGenerator.g4_uv_2, null),
			new GeometryGenerator.GeomInfo(GeometryGenerator.g4_indicies_3, GeometryGenerator.g4_points_3, GeometryGenerator.g4_uv_3, null)
		};

		public int numVerts;

		private int numUVs;

		private int numNormals;

		public int numTriangles;

		private Mesh _mesh;

		private CubeDrawTypes cdttype;

		protected int _trisize;

		protected int _size;

		private class GeomInfo
		{
			public GeomInfo(int[][] i, Vector3[][] p, Vector2[][] u = null, Vector3[][] n = null)
			{
				this.indexes = i;
				this.points = p;
				this.uv = u;
				this.normals = n;
			}

			public int[][] indexes;

			public Vector3[][] points;

			public Vector3[][] normals;

			public Vector2[][] uv;
		}

		private class GeomAddInfo
		{
			public int verts;

			public int indexes;
		}
	}
}
