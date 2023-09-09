using System;
using UnityEngine;

public class Geom6Lab : GeomLab
{
	public override void Start()
	{
		this.g1_indicies = Geom6Lab.indicies;
		this.g1_points = this.points;
		this.g1_uv = Geom6Lab.uv;
		this.g3_normals = Geom6Lab.normals;
	}

	public static Vector2[] side_uv = new Vector2[]
	{
		new Vector3(0f, -0.125f),
		new Vector2(0f, 0f),
		new Vector3(0.125f, 0f),
		new Vector3(0.125f, -0.125f)
	};

	public Vector3[][] points = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(-0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, 0.5f, 0f),
			new Vector3(0.5f, 0.5f, 0f),
			new Vector3(0.5f, 0.5f, -0.5f)
		},
		new Vector3[]
		{
			new Vector3(0.5f, -0.5f, -0.5f),
			new Vector3(0.5f, -0.5f, 0f),
			new Vector3(-0.5f, -0.5f, 0f),
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
			new Vector3(0.5f, -0.5f, -0.5f),
			new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(0.5f, 0.5f, 0f),
			new Vector3(0.5f, -0.5f, 0f)
		},
		new Vector3[]
		{
			new Vector3(-0.5f, -0.5f, 0f),
			new Vector3(-0.5f, 0.5f, 0f),
			new Vector3(-0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, -0.5f)
		},
		new Vector3[]
		{
			new Vector3(0.5f, -0.5f, 0f),
			new Vector3(0.5f, 0.5f, 0f),
			new Vector3(-0.5f, 0.5f, 0f),
			new Vector3(-0.5f, -0.5f, 0f)
		}
	};

	public static Vector2[] side2_uv = new Vector2[]
	{
		new Vector2(0f, -0.125f),
		new Vector2(0f, -0.062f),
		new Vector2(0.125f, -0.062f),
		new Vector2(0.125f, -0.125f)
	};

	public static Vector2[] side4_uv = new Vector2[]
	{
		new Vector3(0f, -0.125f),
		new Vector2(0f, 0f),
		new Vector3(0.062f, 0f),
		new Vector3(0.062f, -0.125f)
	};

	public static Vector2[][] uv = new Vector2[][]
	{
		Geom6Lab.side2_uv,
		Geom6Lab.side2_uv,
		new Vector2[0],
		Geom6Lab.side_uv,
		Geom6Lab.side4_uv,
		Geom6Lab.side4_uv,
		Geom6Lab.side_uv
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
		}
	};

	public static Vector3[][] normals = new Vector3[][]
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
}
