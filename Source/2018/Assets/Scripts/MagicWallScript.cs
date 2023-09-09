using System;
using kube;
using UnityEngine;

public class MagicWallScript : MonoBehaviour
{
	private void SetParameters(int _playerId)
	{
		this.playerId = _playerId;
	}

	private void Start()
	{
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
		if (this.playerId == Kube.BCS.onlineId)
		{
			Vector3 vector = base.transform.position + base.transform.TransformDirection(-Vector3.forward);
			int num = Mathf.RoundToInt(vector.x);
			int num2 = Mathf.RoundToInt(vector.z);
			int num3 = Mathf.RoundToInt(vector.y);
			int num4 = Kube.WHS.cubeTypes[num, num3, num2];
			int num5 = 0;
			string text = string.Empty;
			for (int i = 0; i < this.wallLength; i++)
			{
				Vector3 vector2 = base.transform.position + base.transform.TransformDirection(Vector3.forward) * (float)i;
				num = Mathf.RoundToInt(vector2.x);
				num2 = Mathf.RoundToInt(vector2.z);
				num3 = Mathf.RoundToInt(vector2.y);
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					Kube.OH.GetServerCode(num, 2),
					string.Empty,
					Kube.OH.GetServerCode(num3, 2),
					string.Empty,
					Kube.OH.GetServerCode(num2, 2),
					string.Empty,
					Kube.OH.GetServerCode(num4, 2)
				});
				num5++;
			}
			text = Kube.OH.GetServerCode(num5, 2) + text;
			this.NO.ChangeCubes(text);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
	}

	public int wallLength = 4;

	private int playerId;

	private NetworkObjectScript NO;
}
