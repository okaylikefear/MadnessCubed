using System;
using UnityEngine;

[Serializable]
public class MovementClass
{
	public MovementClass()
	{
		this.maxSpeed = 2f;
		this.jumpPower = 8f;
		this.gravity = 9.8f;
		this.currentDirection = Vector3.right;
	}

	public float maxSpeed;

	public float jumpPower;

	public float gravity;

	[NonSerialized]
	public float currentSpeed;

	[NonSerialized]
	public float currentVerticalSpeed;

	[NonSerialized]
	public float targetSpeed;

	[NonSerialized]
	public Vector3 currentDirection;

	[NonSerialized]
	public bool moving;

	[NonSerialized]
	public bool crouching;

	[NonSerialized]
	public bool jumping;

	[NonSerialized]
	public bool falling;

	[NonSerialized]
	public float timeLastGrounded;

	[NonSerialized]
	public CollisionFlags collisionFlags;
}
