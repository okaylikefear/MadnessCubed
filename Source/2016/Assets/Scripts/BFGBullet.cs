using System;
using UnityEngine;

public class BFGBullet : MonoBehaviour
{
	private void SetDamageParam(DamageMessage _dm)
	{
		this.dm = new DamageMessage();
		this.dm.damage = _dm.damage;
		this.dm.id_killer = _dm.id_killer;
		this.dm.weaponType = _dm.weaponType;
		DamageMessage damageMessage = this.dm;
		damageMessage.damage /= 3;
		this.dm.team = _dm.team;
	}

	private void Start()
	{
		this.startTime = Time.time;
		this.monstersToCheck = GameObject.FindGameObjectsWithTag("Monster");
		this.monsterDamaged = new bool[this.monstersToCheck.Length];
		this.playersToCheck = GameObject.FindGameObjectsWithTag("Player");
		this.playerDamaged = new bool[this.playersToCheck.Length];
	}

	private void Update()
	{
		if (Time.time - this.lastCheck > this.checkTargetsDeltaTime && Time.time - this.startTime > this.prewarmDamageTime)
		{
			for (int i = 0; i < this.monstersToCheck.Length; i++)
			{
				if (!this.monsterDamaged[i])
				{
					if (!(this.monstersToCheck[i] == null))
					{
						float num = Vector3.Distance(base.transform.position, this.monstersToCheck[i].transform.position);
						if (num < this.damageDistance)
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(this.lightningPrefab, base.transform.position, base.transform.rotation) as GameObject;
							gameObject.SendMessage("SetSource", base.transform);
							gameObject.SendMessage("SetDestination", this.monstersToCheck[i].transform);
							this.monsterDamaged[i] = true;
							this.monstersToCheck[i].SendMessage("ApplyDamage", this.dm);
						}
					}
				}
			}
			for (int j = 0; j < this.playersToCheck.Length; j++)
			{
				if (!this.playerDamaged[j])
				{
					float num2 = Vector3.Distance(base.transform.position, this.playersToCheck[j].transform.position);
					if (num2 < this.damageDistance)
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate(this.lightningPrefab, base.transform.position, base.transform.rotation) as GameObject;
						gameObject2.SendMessage("SetSource", base.transform);
						gameObject2.SendMessage("SetDestination", this.playersToCheck[j].transform);
						this.playerDamaged[j] = true;
						this.playersToCheck[j].SendMessage("ApplyDamage", this.dm);
					}
				}
			}
			this.lastCheck = Time.time;
		}
	}

	public GameObject lightningPrefab;

	private GameObject[] monstersToCheck;

	private bool[] monsterDamaged;

	private GameObject[] playersToCheck;

	private bool[] playerDamaged;

	public float damageDistance;

	private DamageMessage dm;

	public float prewarmDamageTime = 1f;

	private float startTime;

	private float checkTargetsDeltaTime = 0.3f;

	private float lastCheck;
}
