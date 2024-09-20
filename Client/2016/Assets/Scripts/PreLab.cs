using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PreLab : MonoBehaviour
{
	private void Start()
	{
	}

	[ContextMenu("Clone")]
	private void Clone()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		Mesh mesh = UnityEngine.Object.Instantiate<Mesh>(component.sharedMesh);
		component.mesh = mesh;
	}

	[ContextMenu("SubTex")]
	private void SubTex()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		Mesh sharedMesh = component.sharedMesh;
		List<Vector2> list = new List<Vector2>(sharedMesh.uv);
		for (int i = 0; i < list.Count; i++)
		{
			Vector2 value = list[i];
			value.x *= 0.125f;
			value.y *= -0.125f;
			list[i] = value;
		}
		sharedMesh.uv = list.ToArray();
	}

	[ContextMenu("Rotate")]
	private void Rotate()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		Mesh sharedMesh = component.sharedMesh;
		List<Vector3> list = new List<Vector3>(sharedMesh.vertices);
		List<Vector3> list2 = new List<Vector3>(sharedMesh.normals);
		Quaternion rotation;
		if (this.rotAxis == PreLab.Axis.up)
		{
			rotation = Quaternion.AngleAxis(this.rotAngle, Vector3.up);
		}
		else if (this.rotAxis == PreLab.Axis.left)
		{
			rotation = Quaternion.AngleAxis(this.rotAngle, Vector3.left);
		}
		else
		{
			rotation = Quaternion.AngleAxis(this.rotAngle, Vector3.forward);
		}
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = rotation * list[i];
			list2[i] = rotation * list2[i];
		}
		sharedMesh.vertices = list.ToArray();
		sharedMesh.normals = list2.ToArray();
	}

	[ContextMenu("Offset")]
	private void Offset()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		Mesh sharedMesh = component.sharedMesh;
		List<Vector3> list = new List<Vector3>(sharedMesh.vertices);
		for (int i = 0; i < list.Count; i++)
		{
			list[i] += this.offset;
		}
		sharedMesh.vertices = list.ToArray();
	}

	[ContextMenu("Scale")]
	private void DoScale()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		Mesh sharedMesh = component.sharedMesh;
		List<Vector3> list = new List<Vector3>(sharedMesh.vertices);
		for (int i = 0; i < list.Count; i++)
		{
			Vector3 value = list[i];
			value.x *= this.offset.x;
			value.y *= this.offset.y;
			value.z *= this.offset.z;
			list[i] = value;
		}
		sharedMesh.vertices = list.ToArray();
	}

	[ContextMenu("Outside")]
	private void Outside()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		Mesh sharedMesh = component.sharedMesh;
		List<int> list = new List<int>(sharedMesh.triangles);
		list.Reverse();
		sharedMesh.triangles = list.ToArray();
	}

	[ContextMenu("UV Atlas")]
	private void UVAtlas()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		Mesh sharedMesh = component.sharedMesh;
		List<Vector2> list = new List<Vector2>(sharedMesh.uv);
		for (int i = 0; i < list.Count; i++)
		{
			Vector2 value = list[i];
			value.x *= this.uv.x;
			value.y *= this.uv.y;
			list[i] = value;
		}
		sharedMesh.uv = list.ToArray();
	}

	[ContextMenu("Mesh Clone")]
	private void MeshClone()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		Mesh mesh = component.mesh;
		component.sharedMesh = mesh;
	}

	private Mesh _mesh;

	public float rotAngle;

	public PreLab.Axis rotAxis;

	public Vector3 offset;

	public Vector2 uv;

	public enum Axis
	{
		up,
		forward,
		left
	}
}
