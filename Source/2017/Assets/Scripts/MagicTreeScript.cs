using System;
using kube;
using UnityEngine;

public class MagicTreeScript : MonoBehaviour
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
		int num = 0;
		if (this.typeTree == 0)
		{
			num = 12;
		}
		if (this.typeTree == 1)
		{
			num = 37;
		}
		if (this.typeTree == 2)
		{
			num = 38;
		}
		if (this.playerId == Kube.BCS.onlineId)
		{
			int num2 = 0;
			string text = string.Empty;
			int num3 = UnityEngine.Random.Range(2, 6);
			for (int i = 0; i < num3; i++)
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					Kube.OH.GetServerCode(Mathf.RoundToInt(base.transform.position.x), 2),
					string.Empty,
					Kube.OH.GetServerCode(Mathf.RoundToInt(base.transform.position.y) + i, 2),
					string.Empty,
					Kube.OH.GetServerCode(Mathf.RoundToInt(base.transform.position.z), 2),
					string.Empty,
					Kube.OH.GetServerCode(14, 2)
				});
				num2++;
			}
			int num4 = 2;
			if (num3 >= 4)
			{
				num4 = 3;
			}
			for (int j = 0; j <= num4 - 1; j++)
			{
				int num5 = Mathf.RoundToInt(base.transform.position.x);
				int num6 = Mathf.RoundToInt(base.transform.position.z);
				int num7 = Mathf.RoundToInt(base.transform.position.y) + num3 + num4 - j - 1;
				for (int k = -j; k <= j; k++)
				{
					for (int l = -j; l <= j; l++)
					{
						if (Mathf.Abs(k) + Mathf.Abs(l) <= j)
						{
							string text2 = text;
							text = string.Concat(new string[]
							{
								text2,
								Kube.OH.GetServerCode(num5 + k, 2),
								string.Empty,
								Kube.OH.GetServerCode(num7, 2),
								string.Empty,
								Kube.OH.GetServerCode(num6 + l, 2),
								string.Empty,
								Kube.OH.GetServerCode(num, 2)
							});
							num2++;
						}
					}
				}
			}
			text = Kube.OH.GetServerCode(num2, 2) + text;
			this.NO.ChangeCubes(text);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
	}

	public int typeTree;

	private int playerId;

	private NetworkObjectScript NO;
}
