using System;
using kube;
using UnityEngine;

public class WeaponsRespawnScript : MonoBehaviour
{
	private void Start()
	{
		this.Init();
	}

	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.IPS = base.transform.root.gameObject.GetComponent<ItemPropsScript>();
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
		this.respawnSumWeight = 0f;
		this.respawnRandomRange = new float[this.respawnRandomWeight.Length + 1];
		this.respawnRandomRange[0] = 0f;
		this.nextRespawnTime = 0f;
		for (int i = 0; i < this.respawnRandomWeight.Length; i++)
		{
			this.respawnSumWeight += this.respawnRandomWeight[i];
			this.respawnRandomRange[i + 1] = this.respawnSumWeight;
		}
		this.initialized = true;
	}

	private void Update()
	{
		if (Time.time > this.nextRespawnTime && this.IPS.state == 0)
		{
			float num = UnityEngine.Random.Range(0f, this.respawnSumWeight);
			for (int i = 0; i < this.respawnRandomWeight.Length; i++)
			{
				if (num >= this.respawnRandomRange[i] && num <= this.respawnRandomRange[i + 1])
				{
					this.numRespawn = i;
					break;
				}
			}
			if (Kube.BCS == null)
			{
				Kube.BCS = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControllerScript>();
			}
			if (this.NO == null)
			{
				this.NO = Kube.BCS.NO;
			}
			this.NO.ChangeItemState(this.IPS.id, 1 + this.numRespawn);
			this.nextRespawnTime = Time.time + this.respawnPeriod;
		}
	}

	private void ChangeItemState(int newState)
	{
		this.Init();
		if (this.IPS.state == newState)
		{
			return;
		}
		this.IPS.state = newState;
		if (this.currentRespawnGO != null)
		{
			UnityEngine.Object.Destroy(this.currentRespawnGO);
		}
		if (newState != 0)
		{
			this.currentRespawnGO = (UnityEngine.Object.Instantiate(this.respawnGO[newState - 1], Vector3.zero, Quaternion.identity) as GameObject);
			this.currentRespawnGO.transform.parent = base.transform;
			this.currentRespawnGO.transform.localPosition = Vector3.zero;
		}
		else
		{
			this.nextRespawnTime = Time.time + this.respawnPeriod;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.IPS.state != 0 && other.gameObject.transform.root.gameObject.layer == 9)
		{
			PlayerScript component = other.gameObject.transform.root.gameObject.GetComponent<PlayerScript>();
			component.GetNewWeapon(this.respawnNumWeapons[this.numRespawn], this.respawnAmountOfBullets[this.numRespawn]);
			this.NO.ChangeItemState(this.IPS.id, 0);
			this.IPS.state = 0;
			this.nextRespawnTime = Time.time + this.respawnPeriod;
		}
	}

	private float nextRespawnTime;

	public float prerespawnDelay = 10f;

	public float respawnPeriod;

	public GameObject[] respawnGO;

	public float[] respawnRandomWeight;

	private float respawnSumWeight;

	private float[] respawnRandomRange;

	public int[] respawnNumWeapons;

	public int[] respawnAmountOfBullets;

	private GameObject currentRespawnGO;

	private NetworkObjectScript NO;

	private ItemPropsScript IPS;

	private int numRespawn;

	private bool initialized;
}
