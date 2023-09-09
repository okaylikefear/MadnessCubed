using System;
using kube;
using UnityEngine;

public class DoorDoubleScript : MonoBehaviour
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
		if (Kube.BCS == null)
		{
			Kube.BCS = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControllerScript>();
		}
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
		this.initialized = true;
	}

	private void Update()
	{
	}

	private void Activate(PlayerScript ps)
	{
		this.Init();
		if (this.IPS.state == 0)
		{
			if (base.transform.InverseTransformDirection(ps.transform.position - base.transform.position).x > 0f)
			{
				this.NO.ChangeItemState(this.IPS.id, 1);
			}
			else
			{
				this.NO.ChangeItemState(this.IPS.id, 2);
			}
		}
		else
		{
			this.NO.ChangeItemState(this.IPS.id, 0);
		}
	}

	private void ChangeItemState(int newState)
	{
		this.Init();
		this.IPS.state = newState;
		if (this.doorLeft != null && this.doorRight != null)
		{
			if (newState == 0)
			{
				this.doorLeft.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				this.doorRight.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
				for (int i = 0; i < (int)this.IPS.doorSize.x; i++)
				{
					for (int j = 0; j < (int)this.IPS.doorSize.y; j++)
					{
						Vector3 point = new Vector3(0f, (float)j, (float)i);
						point = base.transform.rotation * point;
						Kube.WHS.cubes[Mathf.RoundToInt(base.transform.position.x + point.x), Mathf.RoundToInt(base.transform.position.y + point.y), Mathf.RoundToInt(base.transform.position.z + point.z)].prop = CubeProps.closedDoor;
					}
				}
				if (Time.timeSinceLevelLoad > 5f)
				{
					UnityEngine.Object.Instantiate(this.soundClose, base.transform.position, Quaternion.identity);
				}
			}
			else if (newState == 1)
			{
				this.doorLeft.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
				this.doorRight.transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
				this.ClearWorldProps();
				if (Time.timeSinceLevelLoad > 5f)
				{
					UnityEngine.Object.Instantiate(this.soundOpen, base.transform.position, Quaternion.identity);
				}
			}
			else if (newState == 2)
			{
				this.doorLeft.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
				this.doorRight.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
				this.ClearWorldProps();
				if (Time.timeSinceLevelLoad > 5f)
				{
					UnityEngine.Object.Instantiate(this.soundOpen, base.transform.position, Quaternion.identity);
				}
			}
		}
	}

	private void ClearWorldProps()
	{
		for (int i = 0; i < (int)this.IPS.doorSize.x; i++)
		{
			for (int j = 0; j < (int)this.IPS.doorSize.y; j++)
			{
				Vector3 point = new Vector3(0f, (float)j, (float)i);
				point = base.transform.rotation * point;
				Kube.WHS.cubes[Mathf.RoundToInt(base.transform.position.x + point.x), Mathf.RoundToInt(base.transform.position.y + point.y), Mathf.RoundToInt(base.transform.position.z + point.z)].prop = CubeProps.no;
			}
		}
	}

	private void MonsterHit(Vector3 monsterPos)
	{
		this.Init();
		int num = UnityEngine.Random.Range(0, this.doorStrength);
		if (num == 1 && this.IPS.state == 0)
		{
			if (base.transform.InverseTransformDirection(monsterPos - base.transform.position).x > 0f)
			{
				this.NO.ChangeItemState(this.IPS.id, 1);
			}
			else
			{
				this.NO.ChangeItemState(this.IPS.id, 2);
			}
		}
		else if (num == 1 && this.IPS.state != 0)
		{
			this.NO.ChangeItemState(this.IPS.id, 0);
		}
	}

	private void OnDestroy()
	{
		if (Kube.WHS)
		{
			this.ClearWorldProps();
		}
	}

	private ItemPropsScript IPS;

	private NetworkObjectScript NO;

	private BattleControllerScript BCS;

	public GameObject doorLeft;

	public GameObject doorRight;

	public int doorStrength = 15;

	public GameObject soundOpen;

	public GameObject soundClose;

	private bool initialized;
}
