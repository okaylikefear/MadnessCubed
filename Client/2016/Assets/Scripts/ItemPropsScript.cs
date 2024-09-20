using System;
using kube;
using UnityEngine;

public class ItemPropsScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void MoveItem(Vector3 newPos)
	{
		ActionAreaScript component = base.GetComponent<ActionAreaScript>();
		TriggerScript component2 = base.GetComponent<TriggerScript>();
		WireScript component3 = base.GetComponent<WireScript>();
		NetworkObjectScript no = Kube.BCS.NO;
		if (!(component != null))
		{
			if (component2 != null)
			{
				no.MoveItem(this.id, newPos);
			}
			else if (!(component3 != null))
			{
				no.MoveItem(this.id, newPos);
			}
		}
	}

	public bool magic;

	public bool buildMagic;

	public bool canTake;

	public bool canMove;

	public bool canRotate;

	public bool canActivate;

	public bool canSetup;

	public bool isTrigger;

	public bool automaticTakeIfNear;

	public ItemPlaceType placeType;

	public CubePhys physType;

	public SoundMaterialType soundMaterialType;

	public byte health;

	public Color32 lightColor;

	public int id;

	public int state;

	public int type;

	public Vector2 doorSize;

	public CubeProps mapProps;
}
