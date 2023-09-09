using System;
using UnityEngine;

public class Geom1Lab : GeomLab
{
	public override void Start()
	{
		this.g1_indicies = Geom1Lab.indicies;
		this.g1_points = this.points;
		this.g1_uv = Geom1Lab.uv;
		this.g3_normals = Geom1Lab.normals;
	}

	public static Vector2[] side_uv = new Vector2[]
	{
		new Vector3(0f, -0.125f),
		new Vector2(0f, 0f),
		new Vector3(0.125f, 0f),
		new Vector3(0.125f, -0.125f)
	};

	public static Vector2[][] uv = new Vector2[][]
	{
		Geom1Lab.side_uv,
		Geom1Lab.side_uv,
		Geom1Lab.side_uv,
		Geom1Lab.side_uv,
		Geom1Lab.side_uv,
		Geom1Lab.side_uv,
		new Vector2[0]
	};

	public Vector3[][] points = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(-0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, -0.5f)
		},
		new Vector3[]
		{
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, -0.5f, -0.5f)
		},
		new Vector3[]
		{
			new Vector3(0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, -0.5f, 0.5f)
		},
		new Vector3[]
		{
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f, 0.5f, -0.5f),
			new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(0.5f, -0.5f, -0.5f)
		},
		new Vector3[]
		{
			new Vector3(0.5f, -0.5f, -0.5f),
			new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(0.5f, -0.5f, 0.5f)
		},
		new Vector3[]
		{
			new Vector3(-0.5f, -0.5f, 0.5f),
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, -0.5f)
		},
		new Vector3[0]
	};

	public static int[][] indicies = new int[][]
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
		new int[0]
	};

	public static Vector3[][] normals = new Vector3[][]
	{
		default(Vector3[]),
		default(Vector3[]),
		default(Vector3[]),
		default(Vector3[]),
		default(Vector3[]),
		default(Vector3[]),
		new Vector3[0]
	};
}
