using System;
using UnityEngine;

[Serializable]
public class PlayerConfig : ScriptableObject
{
	public float autoaimAngle = 10f;

	public float autoaimForce = 1f;

	public float accuracyOffsetTime = 0.1f;

	public Vector3 thirdPersonOffset = new Vector3(0.5f, 0f, -1.5f);

	public float folowSpeed = 10f;

	public float sniperNoAimAccuracy = 20f;

	public float sensitivityY = 1f;

	public float spreadSize = 0.2f;

	public int maxHealth = 100;

	public float height = 2f;

	public float heightCrounch = 1.5f;

	public float autoFireDist;
}
