using System;
using kube;
using UnityEngine;

public class SummonMonsterScript : MonoBehaviour
{
	private void Start()
	{
		if (PhotonNetwork.isMasterClient)
		{
			int num = UnityEngine.Random.Range(0, this.monstersName.Length);
			for (int i = 0; i < 100; i++)
			{
				Vector3 pos = UnityEngine.Random.insideUnitSphere * this.radiusToSummon + base.transform.position;
				if (Kube.WHS.IsInWorld((int)pos.x, (int)pos.y, (int)pos.z) && !Kube.WHS.isOccupied[(int)pos.x, (int)pos.y, (int)pos.z] && !Kube.WHS.isOccupied[(int)pos.x, (int)pos.y + 1, (int)pos.z] && Kube.WHS.cubeTypes[(int)pos.x, (int)pos.y, (int)pos.z] == 0 && Kube.WHS.cubeTypes[(int)pos.x, (int)pos.y + 1, (int)pos.z] == 0)
				{
					Kube.BCS.NO.SummonMonster(pos, this.monstersName[num]);
					break;
				}
			}
		}
	}

	private void Update()
	{
	}

	public string[] monstersName;

	public GameObject summonEffectGO;

	public float radiusToSummon;
}
