using System;
using UnityEngine;

public class g6_wall
{
	// Note: this type is marked as 'beforefieldinit'.
	static g6_wall()
	{
		bool[] array = new bool[7];
		array[2] = true;
		g6_wall.neighbor_has_side_0 = array;
		g6_wall.indexes_90_0 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_90_0 = new Vector3[]
		{
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(0f, 0.5f, 0.5f),
			new Vector3(0f, 0.5f, -0.5f),
			new Vector3(-0.5f, 0.5f, -0.5f)
		};
		g6_wall.normals_90_0 = new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0f)
		};
		g6_wall.uv_90_0 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, -0.062f),
			new Vector3(0.125f, -0.062f),
			new Vector3(0.125f, -0.125f)
		};
		g6_wall.indexes_90_1 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_90_1 = new Vector3[]
		{
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(0f, -0.5f, -0.5f),
			new Vector3(0f, -0.5f, 0.5f),
			new Vector3(-0.5f, -0.5f, 0.5f)
		};
		g6_wall.normals_90_1 = new Vector3[]
		{
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, -1f, 0f)
		};
		g6_wall.uv_90_1 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, -0.062f),
			new Vector3(0.125f, -0.062f),
			new Vector3(0.125f, -0.125f)
		};
		g6_wall.indexes_90_2 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_90_2 = new Vector3[]
		{
			new Vector3(0f, -0.5f, 0.5f),
			new Vector3(0f, 0.5f, 0.5f),
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, -0.5f, 0.5f)
		};
		g6_wall.normals_90_2 = new Vector3[]
		{
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f)
		};
		g6_wall.uv_90_2 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, 0f),
			new Vector3(0.062f, 0f),
			new Vector3(0.062f, -0.125f)
		};
		g6_wall.indexes_90_3 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_90_3 = new Vector3[]
		{
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f, 0.5f, -0.5f),
			new Vector3(0f, 0.5f, -0.5f),
			new Vector3(0f, -0.5f, -0.5f)
		};
		g6_wall.normals_90_3 = new Vector3[]
		{
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f)
		};
		g6_wall.uv_90_3 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, 0f),
			new Vector3(0.062f, 0f),
			new Vector3(0.062f, -0.125f)
		};
		g6_wall.indexes_90_4 = new int[0];
		g6_wall.vertices_90_4 = new Vector3[0];
		g6_wall.normals_90_4 = new Vector3[0];
		g6_wall.uv_90_4 = new Vector2[0];
		g6_wall.indexes_90_5 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_90_5 = new Vector3[]
		{
			new Vector3(-0.5f, -0.5f, 0.5f),
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, -0.5f)
		};
		g6_wall.normals_90_5 = new Vector3[]
		{
			new Vector3(-1f, 0f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(-1f, 0f, 0f)
		};
		g6_wall.uv_90_5 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, 0f),
			new Vector3(0.125f, 0f),
			new Vector3(0.125f, -0.125f)
		};
		g6_wall.indexes_90_6 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_90_6 = new Vector3[]
		{
			new Vector3(0f, -0.5f, -0.5f),
			new Vector3(0f, 0.5f, -0.5f),
			new Vector3(0f, 0.5f, 0.5f),
			new Vector3(0f, -0.5f, 0.5f)
		};
		g6_wall.normals_90_6 = new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0f, 0f)
		};
		g6_wall.uv_90_6 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, 0f),
			new Vector3(0.125f, 0f),
			new Vector3(0.125f, -0.125f)
		};
		g6_wall.indexes_90 = new int[][]
		{
			g6_wall.indexes_90_0,
			g6_wall.indexes_90_1,
			g6_wall.indexes_90_2,
			g6_wall.indexes_90_3,
			g6_wall.indexes_90_4,
			g6_wall.indexes_90_5,
			g6_wall.indexes_90_6
		};
		g6_wall.points_90 = new Vector3[][]
		{
			g6_wall.vertices_90_0,
			g6_wall.vertices_90_1,
			g6_wall.vertices_90_2,
			g6_wall.vertices_90_3,
			g6_wall.vertices_90_4,
			g6_wall.vertices_90_5,
			g6_wall.vertices_90_6
		};
		g6_wall.normals_90 = new Vector3[][]
		{
			g6_wall.normals_90_0,
			g6_wall.normals_90_1,
			g6_wall.normals_90_2,
			g6_wall.normals_90_3,
			g6_wall.normals_90_4,
			g6_wall.normals_90_5,
			g6_wall.normals_90_6
		};
		g6_wall.uv_90 = new Vector2[][]
		{
			g6_wall.uv_90_0,
			g6_wall.uv_90_1,
			g6_wall.uv_90_2,
			g6_wall.uv_90_3,
			g6_wall.uv_90_4,
			g6_wall.uv_90_5,
			g6_wall.uv_90_6
		};
		g6_wall.geomNumVerts_90 = new int[]
		{
			4,
			4,
			4,
			4,
			0,
			4,
			4
		};
		g6_wall.geomNumTris_90 = new int[]
		{
			6,
			6,
			6,
			6,
			0,
			6,
			6
		};
		bool[] array2 = new bool[7];
		array2[4] = true;
		g6_wall.neighbor_has_side_90 = array2;
		g6_wall.indexes_180_0 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_180_0 = new Vector3[]
		{
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, 0f),
			new Vector3(-0.5f, 0.5f, 0f),
			new Vector3(-0.5f, 0.5f, 0.5f)
		};
		g6_wall.normals_180_0 = new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0f)
		};
		g6_wall.uv_180_0 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, -0.062f),
			new Vector3(0.125f, -0.062f),
			new Vector3(0.125f, -0.125f)
		};
		g6_wall.indexes_180_1 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_180_1 = new Vector3[]
		{
			new Vector3(-0.5f, -0.5f, 0.5f),
			new Vector3(-0.5f, -0.5f, 0f),
			new Vector3(0.5f, -0.5f, 0f),
			new Vector3(0.5f, -0.5f, 0.5f)
		};
		g6_wall.normals_180_1 = new Vector3[]
		{
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, -1f, 0f)
		};
		g6_wall.uv_180_1 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, -0.062f),
			new Vector3(0.125f, -0.062f),
			new Vector3(0.125f, -0.125f)
		};
		g6_wall.indexes_180_2 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_180_2 = new Vector3[]
		{
			new Vector3(0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, -0.5f, 0.5f)
		};
		g6_wall.normals_180_2 = new Vector3[]
		{
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f)
		};
		g6_wall.uv_180_2 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, 0f),
			new Vector3(0.125f, 0f),
			new Vector3(0.125f, -0.125f)
		};
		g6_wall.indexes_180_3 = new int[0];
		g6_wall.vertices_180_3 = new Vector3[0];
		g6_wall.normals_180_3 = new Vector3[0];
		g6_wall.uv_180_3 = new Vector2[0];
		g6_wall.indexes_180_4 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_180_4 = new Vector3[]
		{
			new Vector3(0.5f, -0.5f, 0f),
			new Vector3(0.5f, 0.5f, 0f),
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(0.5f, -0.5f, 0.5f)
		};
		g6_wall.normals_180_4 = new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0f, 0f)
		};
		g6_wall.uv_180_4 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, 0f),
			new Vector3(0.062f, 0f),
			new Vector3(0.062f, -0.125f)
		};
		g6_wall.indexes_180_5 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_180_5 = new Vector3[]
		{
			new Vector3(-0.5f, -0.5f, 0.5f),
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, 0.5f, 0f),
			new Vector3(-0.5f, -0.5f, 0f)
		};
		g6_wall.normals_180_5 = new Vector3[]
		{
			new Vector3(-1f, 0f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(-1f, 0f, 0f)
		};
		g6_wall.uv_180_5 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, 0f),
			new Vector3(0.062f, 0f),
			new Vector3(0.062f, -0.125f)
		};
		g6_wall.indexes_180_6 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_180_6 = new Vector3[]
		{
			new Vector3(-0.5f, -0.5f, 0f),
			new Vector3(-0.5f, 0.5f, 0f),
			new Vector3(0.5f, 0.5f, 0f),
			new Vector3(0.5f, -0.5f, 0f)
		};
		g6_wall.normals_180_6 = new Vector3[]
		{
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f)
		};
		g6_wall.uv_180_6 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, 0f),
			new Vector3(0.125f, 0f),
			new Vector3(0.125f, -0.125f)
		};
		g6_wall.indexes_180 = new int[][]
		{
			g6_wall.indexes_180_0,
			g6_wall.indexes_180_1,
			g6_wall.indexes_180_2,
			g6_wall.indexes_180_3,
			g6_wall.indexes_180_4,
			g6_wall.indexes_180_5,
			g6_wall.indexes_180_6
		};
		g6_wall.points_180 = new Vector3[][]
		{
			g6_wall.vertices_180_0,
			g6_wall.vertices_180_1,
			g6_wall.vertices_180_2,
			g6_wall.vertices_180_3,
			g6_wall.vertices_180_4,
			g6_wall.vertices_180_5,
			g6_wall.vertices_180_6
		};
		g6_wall.normals_180 = new Vector3[][]
		{
			g6_wall.normals_180_0,
			g6_wall.normals_180_1,
			g6_wall.normals_180_2,
			g6_wall.normals_180_3,
			g6_wall.normals_180_4,
			g6_wall.normals_180_5,
			g6_wall.normals_180_6
		};
		g6_wall.uv_180 = new Vector2[][]
		{
			g6_wall.uv_180_0,
			g6_wall.uv_180_1,
			g6_wall.uv_180_2,
			g6_wall.uv_180_3,
			g6_wall.uv_180_4,
			g6_wall.uv_180_5,
			g6_wall.uv_180_6
		};
		g6_wall.geomNumVerts_180 = new int[]
		{
			4,
			4,
			4,
			0,
			4,
			4,
			4
		};
		g6_wall.geomNumTris_180 = new int[]
		{
			6,
			6,
			6,
			0,
			6,
			6,
			6
		};
		bool[] array3 = new bool[7];
		array3[3] = true;
		g6_wall.neighbor_has_side_180 = array3;
		g6_wall.indexes_270_0 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_270_0 = new Vector3[]
		{
			new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(0f, 0.5f, -0.5f),
			new Vector3(0f, 0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, 0.5f)
		};
		g6_wall.normals_270_0 = new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0f)
		};
		g6_wall.uv_270_0 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, -0.062f),
			new Vector3(0.125f, -0.062f),
			new Vector3(0.125f, -0.125f)
		};
		g6_wall.indexes_270_1 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_270_1 = new Vector3[]
		{
			new Vector3(0.5f, -0.5f, 0.5f),
			new Vector3(0f, -0.5f, 0.5f),
			new Vector3(0f, -0.5f, -0.5f),
			new Vector3(0.5f, -0.5f, -0.5f)
		};
		g6_wall.normals_270_1 = new Vector3[]
		{
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, -1f, 0f)
		};
		g6_wall.uv_270_1 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, -0.062f),
			new Vector3(0.125f, -0.062f),
			new Vector3(0.125f, -0.125f)
		};
		g6_wall.indexes_270_2 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_270_2 = new Vector3[]
		{
			new Vector3(0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(0f, 0.5f, 0.5f),
			new Vector3(0f, -0.5f, 0.5f)
		};
		g6_wall.normals_270_2 = new Vector3[]
		{
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f)
		};
		g6_wall.uv_270_2 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, 0f),
			new Vector3(0.062f, 0f),
			new Vector3(0.062f, -0.125f)
		};
		g6_wall.indexes_270_3 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_270_3 = new Vector3[]
		{
			new Vector3(0f, -0.5f, -0.5f),
			new Vector3(0f, 0.5f, -0.5f),
			new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(0.5f, -0.5f, -0.5f)
		};
		g6_wall.normals_270_3 = new Vector3[]
		{
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f)
		};
		g6_wall.uv_270_3 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, 0f),
			new Vector3(0.062f, 0f),
			new Vector3(0.062f, -0.125f)
		};
		g6_wall.indexes_270_4 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_270_4 = new Vector3[]
		{
			new Vector3(0.5f, -0.5f, -0.5f),
			new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(0.5f, -0.5f, 0.5f)
		};
		g6_wall.normals_270_4 = new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0f, 0f)
		};
		g6_wall.uv_270_4 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, 0f),
			new Vector3(0.125f, 0f),
			new Vector3(0.125f, -0.125f)
		};
		g6_wall.indexes_270_5 = new int[0];
		g6_wall.vertices_270_5 = new Vector3[0];
		g6_wall.normals_270_5 = new Vector3[0];
		g6_wall.uv_270_5 = new Vector2[0];
		g6_wall.indexes_270_6 = new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		};
		g6_wall.vertices_270_6 = new Vector3[]
		{
			new Vector3(0f, -0.5f, 0.5f),
			new Vector3(0f, 0.5f, 0.5f),
			new Vector3(0f, 0.5f, -0.5f),
			new Vector3(0f, -0.5f, -0.5f)
		};
		g6_wall.normals_270_6 = new Vector3[]
		{
			new Vector3(-1f, 0f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(-1f, 0f, 0f)
		};
		g6_wall.uv_270_6 = new Vector2[]
		{
			new Vector3(0f, -0.125f),
			new Vector3(0f, 0f),
			new Vector3(0.125f, 0f),
			new Vector3(0.125f, -0.125f)
		};
		g6_wall.indexes_270 = new int[][]
		{
			g6_wall.indexes_270_0,
			g6_wall.indexes_270_1,
			g6_wall.indexes_270_2,
			g6_wall.indexes_270_3,
			g6_wall.indexes_270_4,
			g6_wall.indexes_270_5,
			g6_wall.indexes_270_6
		};
		g6_wall.points_270 = new Vector3[][]
		{
			g6_wall.vertices_270_0,
			g6_wall.vertices_270_1,
			g6_wall.vertices_270_2,
			g6_wall.vertices_270_3,
			g6_wall.vertices_270_4,
			g6_wall.vertices_270_5,
			g6_wall.vertices_270_6
		};
		g6_wall.normals_270 = new Vector3[][]
		{
			g6_wall.normals_270_0,
			g6_wall.normals_270_1,
			g6_wall.normals_270_2,
			g6_wall.normals_270_3,
			g6_wall.normals_270_4,
			g6_wall.normals_270_5,
			g6_wall.normals_270_6
		};
		g6_wall.uv_270 = new Vector2[][]
		{
			g6_wall.uv_270_0,
			g6_wall.uv_270_1,
			g6_wall.uv_270_2,
			g6_wall.uv_270_3,
			g6_wall.uv_270_4,
			g6_wall.uv_270_5,
			g6_wall.uv_270_6
		};
		g6_wall.geomNumVerts_270 = new int[]
		{
			4,
			4,
			4,
			4,
			4,
			0,
			4
		};
		g6_wall.geomNumTris_270 = new int[]
		{
			6,
			6,
			6,
			6,
			6,
			0,
			6
		};
		bool[] array4 = new bool[7];
		array4[5] = true;
		g6_wall.neighbor_has_side_270 = array4;
	}

	public static int[] indexes_0_0 = new int[]
	{
		0,
		1,
		2,
		2,
		3,
		0
	};

	public static Vector3[] vertices_0_0 = new Vector3[]
	{
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, 0f),
		new Vector3(0.5f, 0.5f, 0f),
		new Vector3(0.5f, 0.5f, -0.5f)
	};

	public static Vector3[] normals_0_0 = new Vector3[]
	{
		new Vector3(0f, 1f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(0f, 1f, 0f)
	};

	public static Vector2[] uv_0_0 = new Vector2[]
	{
		new Vector3(0f, -0.125f),
		new Vector3(0f, -0.062f),
		new Vector3(0.125f, -0.062f),
		new Vector3(0.125f, -0.125f)
	};

	public static int[] indexes_0_1 = new int[]
	{
		0,
		1,
		2,
		2,
		3,
		0
	};

	public static Vector3[] vertices_0_1 = new Vector3[]
	{
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, 0f),
		new Vector3(-0.5f, -0.5f, 0f),
		new Vector3(-0.5f, -0.5f, -0.5f)
	};

	public static Vector3[] normals_0_1 = new Vector3[]
	{
		new Vector3(0f, -1f, 0f),
		new Vector3(0f, -1f, 0f),
		new Vector3(0f, -1f, 0f),
		new Vector3(0f, -1f, 0f)
	};

	public static Vector2[] uv_0_1 = new Vector2[]
	{
		new Vector3(0f, -0.125f),
		new Vector3(0f, -0.062f),
		new Vector3(0.125f, -0.062f),
		new Vector3(0.125f, -0.125f)
	};

	public static int[] indexes_0_2 = new int[0];

	public static Vector3[] vertices_0_2 = new Vector3[0];

	public static Vector3[] normals_0_2 = new Vector3[0];

	public static Vector2[] uv_0_2 = new Vector2[0];

	public static int[] indexes_0_3 = new int[]
	{
		0,
		1,
		2,
		2,
		3,
		0
	};

	public static Vector3[] vertices_0_3 = new Vector3[]
	{
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, -0.5f)
	};

	public static Vector3[] normals_0_3 = new Vector3[]
	{
		new Vector3(0f, 0f, -1f),
		new Vector3(0f, 0f, -1f),
		new Vector3(0f, 0f, -1f),
		new Vector3(0f, 0f, -1f)
	};

	public static Vector2[] uv_0_3 = new Vector2[]
	{
		new Vector3(0f, -0.125f),
		new Vector3(0f, 0f),
		new Vector3(0.125f, 0f),
		new Vector3(0.125f, -0.125f)
	};

	public static int[] indexes_0_4 = new int[]
	{
		0,
		1,
		2,
		2,
		3,
		0
	};

	public static Vector3[] vertices_0_4 = new Vector3[]
	{
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, 0f),
		new Vector3(0.5f, -0.5f, 0f)
	};

	public static Vector3[] normals_0_4 = new Vector3[]
	{
		new Vector3(1f, 0f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(1f, 0f, 0f)
	};

	public static Vector2[] uv_0_4 = new Vector2[]
	{
		new Vector3(0f, -0.125f),
		new Vector3(0f, 0f),
		new Vector3(0.062f, 0f),
		new Vector3(0.062f, -0.125f)
	};

	public static int[] indexes_0_5 = new int[]
	{
		0,
		1,
		2,
		2,
		3,
		0
	};

	public static Vector3[] vertices_0_5 = new Vector3[]
	{
		new Vector3(-0.5f, -0.5f, 0f),
		new Vector3(-0.5f, 0.5f, 0f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, -0.5f)
	};

	public static Vector3[] normals_0_5 = new Vector3[]
	{
		new Vector3(-1f, 0f, 0f),
		new Vector3(-1f, 0f, 0f),
		new Vector3(-1f, 0f, 0f),
		new Vector3(-1f, 0f, 0f)
	};

	public static Vector2[] uv_0_5 = new Vector2[]
	{
		new Vector3(0f, -0.125f),
		new Vector3(0f, 0f),
		new Vector3(0.062f, 0f),
		new Vector3(0.062f, -0.125f)
	};

	public static int[] indexes_0_6 = new int[]
	{
		0,
		1,
		2,
		2,
		3,
		0
	};

	public static Vector3[] vertices_0_6 = new Vector3[]
	{
		new Vector3(0.5f, -0.5f, 0f),
		new Vector3(0.5f, 0.5f, 0f),
		new Vector3(-0.5f, 0.5f, 0f),
		new Vector3(-0.5f, -0.5f, 0f)
	};

	public static Vector3[] normals_0_6 = new Vector3[]
	{
		new Vector3(0f, 0f, 1f),
		new Vector3(0f, 0f, 1f),
		new Vector3(0f, 0f, 1f),
		new Vector3(0f, 0f, 1f)
	};

	public static Vector2[] uv_0_6 = new Vector2[]
	{
		new Vector3(0f, -0.125f),
		new Vector3(0f, 0f),
		new Vector3(0.125f, 0f),
		new Vector3(0.125f, -0.125f)
	};

	public static int[][] indexes_0 = new int[][]
	{
		g6_wall.indexes_0_0,
		g6_wall.indexes_0_1,
		g6_wall.indexes_0_2,
		g6_wall.indexes_0_3,
		g6_wall.indexes_0_4,
		g6_wall.indexes_0_5,
		g6_wall.indexes_0_6
	};

	public static Vector3[][] points_0 = new Vector3[][]
	{
		g6_wall.vertices_0_0,
		g6_wall.vertices_0_1,
		g6_wall.vertices_0_2,
		g6_wall.vertices_0_3,
		g6_wall.vertices_0_4,
		g6_wall.vertices_0_5,
		g6_wall.vertices_0_6
	};

	public static Vector3[][] normals_0 = new Vector3[][]
	{
		g6_wall.normals_0_0,
		g6_wall.normals_0_1,
		g6_wall.normals_0_2,
		g6_wall.normals_0_3,
		g6_wall.normals_0_4,
		g6_wall.normals_0_5,
		g6_wall.normals_0_6
	};

	public static Vector2[][] uv_0 = new Vector2[][]
	{
		g6_wall.uv_0_0,
		g6_wall.uv_0_1,
		g6_wall.uv_0_2,
		g6_wall.uv_0_3,
		g6_wall.uv_0_4,
		g6_wall.uv_0_5,
		g6_wall.uv_0_6
	};

	public static int[] geomNumVerts_0 = new int[]
	{
		4,
		4,
		0,
		4,
		4,
		4,
		4
	};

	public static int[] geomNumTris_0 = new int[]
	{
		6,
		6,
		0,
		6,
		6,
		6,
		6
	};

	public static bool[] neighbor_has_side_0;

	public static int[] indexes_90_0;

	public static Vector3[] vertices_90_0;

	public static Vector3[] normals_90_0;

	public static Vector2[] uv_90_0;

	public static int[] indexes_90_1;

	public static Vector3[] vertices_90_1;

	public static Vector3[] normals_90_1;

	public static Vector2[] uv_90_1;

	public static int[] indexes_90_2;

	public static Vector3[] vertices_90_2;

	public static Vector3[] normals_90_2;

	public static Vector2[] uv_90_2;

	public static int[] indexes_90_3;

	public static Vector3[] vertices_90_3;

	public static Vector3[] normals_90_3;

	public static Vector2[] uv_90_3;

	public static int[] indexes_90_4;

	public static Vector3[] vertices_90_4;

	public static Vector3[] normals_90_4;

	public static Vector2[] uv_90_4;

	public static int[] indexes_90_5;

	public static Vector3[] vertices_90_5;

	public static Vector3[] normals_90_5;

	public static Vector2[] uv_90_5;

	public static int[] indexes_90_6;

	public static Vector3[] vertices_90_6;

	public static Vector3[] normals_90_6;

	public static Vector2[] uv_90_6;

	public static int[][] indexes_90;

	public static Vector3[][] points_90;

	public static Vector3[][] normals_90;

	public static Vector2[][] uv_90;

	public static int[] geomNumVerts_90;

	public static int[] geomNumTris_90;

	public static bool[] neighbor_has_side_90;

	public static int[] indexes_180_0;

	public static Vector3[] vertices_180_0;

	public static Vector3[] normals_180_0;

	public static Vector2[] uv_180_0;

	public static int[] indexes_180_1;

	public static Vector3[] vertices_180_1;

	public static Vector3[] normals_180_1;

	public static Vector2[] uv_180_1;

	public static int[] indexes_180_2;

	public static Vector3[] vertices_180_2;

	public static Vector3[] normals_180_2;

	public static Vector2[] uv_180_2;

	public static int[] indexes_180_3;

	public static Vector3[] vertices_180_3;

	public static Vector3[] normals_180_3;

	public static Vector2[] uv_180_3;

	public static int[] indexes_180_4;

	public static Vector3[] vertices_180_4;

	public static Vector3[] normals_180_4;

	public static Vector2[] uv_180_4;

	public static int[] indexes_180_5;

	public static Vector3[] vertices_180_5;

	public static Vector3[] normals_180_5;

	public static Vector2[] uv_180_5;

	public static int[] indexes_180_6;

	public static Vector3[] vertices_180_6;

	public static Vector3[] normals_180_6;

	public static Vector2[] uv_180_6;

	public static int[][] indexes_180;

	public static Vector3[][] points_180;

	public static Vector3[][] normals_180;

	public static Vector2[][] uv_180;

	public static int[] geomNumVerts_180;

	public static int[] geomNumTris_180;

	public static bool[] neighbor_has_side_180;

	public static int[] indexes_270_0;

	public static Vector3[] vertices_270_0;

	public static Vector3[] normals_270_0;

	public static Vector2[] uv_270_0;

	public static int[] indexes_270_1;

	public static Vector3[] vertices_270_1;

	public static Vector3[] normals_270_1;

	public static Vector2[] uv_270_1;

	public static int[] indexes_270_2;

	public static Vector3[] vertices_270_2;

	public static Vector3[] normals_270_2;

	public static Vector2[] uv_270_2;

	public static int[] indexes_270_3;

	public static Vector3[] vertices_270_3;

	public static Vector3[] normals_270_3;

	public static Vector2[] uv_270_3;

	public static int[] indexes_270_4;

	public static Vector3[] vertices_270_4;

	public static Vector3[] normals_270_4;

	public static Vector2[] uv_270_4;

	public static int[] indexes_270_5;

	public static Vector3[] vertices_270_5;

	public static Vector3[] normals_270_5;

	public static Vector2[] uv_270_5;

	public static int[] indexes_270_6;

	public static Vector3[] vertices_270_6;

	public static Vector3[] normals_270_6;

	public static Vector2[] uv_270_6;

	public static int[][] indexes_270;

	public static Vector3[][] points_270;

	public static Vector3[][] normals_270;

	public static Vector2[][] uv_270;

	public static int[] geomNumVerts_270;

	public static int[] geomNumTris_270;

	public static bool[] neighbor_has_side_270;
}
