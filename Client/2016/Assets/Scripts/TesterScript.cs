using System;
using UnityEngine;

public class TesterScript : MonoBehaviour
{
	private void Start()
	{
		this.bigVerticesArray = new Vector3[10000];
		this.bigNormalsArray = new Vector3[10000];
		this.bigUVArray = new Vector2[10000];
		this.bigTrianglesArray = new int[10002];
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.P))
		{
			this.GenerateBlock();
		}
	}

	private void GenerateBlock()
	{
		MeshFilter component = base.gameObject.GetComponent<MeshFilter>();
		Mesh mesh;
		if (component.mesh != null)
		{
			component.mesh.Clear();
			mesh = component.mesh;
		}
		else
		{
			mesh = new Mesh();
		}
		mesh.MarkDynamic();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < 11; i++)
		{
			for (int j = 0; j < 11; j++)
			{
				for (int k = 0; k < 11; k++)
				{
					this.bigVerticesArray[num] = UnityEngine.Random.insideUnitSphere * 10f;
					this.bigVerticesArray[num + 1] = UnityEngine.Random.insideUnitSphere * 10f;
					this.bigVerticesArray[num + 2] = UnityEngine.Random.insideUnitSphere * 10f;
					this.bigVerticesArray[num + 3] = UnityEngine.Random.insideUnitSphere * 10f;
					this.bigVerticesArray[num + 4] = UnityEngine.Random.insideUnitSphere * 10f;
					this.bigVerticesArray[num + 5] = UnityEngine.Random.insideUnitSphere * 10f;
					this.bigUVArray[num2] = Vector2.zero;
					this.bigUVArray[num2 + 1] = Vector2.right;
					this.bigUVArray[num2 + 2] = Vector2.one;
					this.bigUVArray[num2 + 3] = Vector2.one;
					this.bigUVArray[num2 + 4] = Vector2.up;
					this.bigUVArray[num2 + 5] = Vector2.zero;
					this.bigNormalsArray[num3] = Vector3.one;
					this.bigNormalsArray[num3 + 1] = Vector3.one;
					this.bigNormalsArray[num3 + 2] = Vector3.one;
					this.bigNormalsArray[num3 + 3] = Vector3.one;
					this.bigNormalsArray[num3 + 4] = Vector3.one;
					this.bigNormalsArray[num3 + 5] = Vector3.one;
					this.bigTrianglesArray[num4] = num;
					this.bigTrianglesArray[num4 + 1] = num + 1;
					this.bigTrianglesArray[num4 + 2] = num + 2;
					this.bigTrianglesArray[num4 + 3] = num + 3;
					this.bigTrianglesArray[num4 + 4] = num + 4;
					this.bigTrianglesArray[num4 + 5] = num + 5;
					num += 6;
					num2 += 6;
					num3 += 6;
					num4 += 6;
				}
			}
		}
		for (int l = num; l < this.bigVerticesArray.Length; l++)
		{
			this.bigVerticesArray[l].x = 0f;
			this.bigVerticesArray[l].y = 0f;
			this.bigVerticesArray[l].z = 0f;
		}
		for (int m = num; m < this.bigNormalsArray.Length; m++)
		{
			this.bigNormalsArray[m].x = 0f;
			this.bigNormalsArray[m].y = 0f;
			this.bigNormalsArray[m].z = 0f;
		}
		for (int n = num; n < this.bigUVArray.Length; n++)
		{
			this.bigUVArray[n].x = 0f;
			this.bigUVArray[n].y = 0f;
		}
		for (int num5 = num; num5 < this.bigTrianglesArray.Length; num5++)
		{
			this.bigTrianglesArray[num5] = 0;
		}
		mesh.vertices = this.bigVerticesArray;
		mesh.normals = this.bigNormalsArray;
		mesh.uv = this.bigUVArray;
		mesh.triangles = this.bigTrianglesArray;
		component.mesh = mesh;
		mesh.Optimize();
		base.gameObject.GetComponent<MeshCollider>().sharedMesh = null;
		base.gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
	}

	private Vector3[] bigVerticesArray;

	private Vector3[] bigNormalsArray;

	private Vector2[] bigUVArray;

	private int[] bigTrianglesArray;
}
