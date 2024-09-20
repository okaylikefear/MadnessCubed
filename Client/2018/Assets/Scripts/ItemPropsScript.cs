using System;
using kube;
using UnityEngine;

public class ItemPropsScript : MonoBehaviour
{
	private void Awake()
	{
	}

	private void PreBake()
	{
		bool flag = !Kube.BCS.mapCanBreak && !Kube.BCS.canChangeWorld;
		ActionAreaScript component = base.GetComponent<ActionAreaScript>();
		TriggerScript component2 = base.GetComponent<TriggerScript>();
		WireScript component3 = base.GetComponent<WireScript>();
		MonsterRespawnScript component4 = base.GetComponent<MonsterRespawnScript>();
		WeaponsRespawnScript component5 = base.GetComponent<WeaponsRespawnScript>();
		BulletsRespawnScript component6 = base.GetComponent<BulletsRespawnScript>();
		bool flag2 = component3 || component2 || component || component4 || component5 || component6;
		if (flag && !this.canActivate && !flag2)
		{
			MeshRenderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>(true);
			int num = 0;
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				if (meshRenderer.gameObject.layer != 14)
				{
					num++;
				}
			}
			if (num <= 0)
			{
				return;
			}
			int num2 = Mathf.RoundToInt(base.transform.position.x / (float)Kube.WHS.blockSizeX);
			int num3 = Mathf.RoundToInt(base.transform.position.y / (float)Kube.WHS.blockSizeY);
			int num4 = Mathf.RoundToInt(base.transform.position.z / (float)Kube.WHS.blockSizeZ);
			string text = string.Format("sbc{0}", base.gameObject.name, num2, num4);
			GameObject gameObject = GameObject.Find(text);
			if (gameObject == null)
			{
				gameObject = new GameObject(text);
			}
			base.transform.parent = gameObject.transform;
			GetWorldLightColorScript component7 = base.GetComponent<GetWorldLightColorScript>();
			UnityEngine.Object.Destroy(component7);
			Kube.WHS.StaticBatchCombine[text] = gameObject;
		}
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
