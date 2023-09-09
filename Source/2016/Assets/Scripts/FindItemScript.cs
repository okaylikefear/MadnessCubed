using System;
using kube;
using kube.ui;
using UnityEngine;

public class FindItemScript : MonoBehaviour
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
		this.initialized = true;
		this.IPS = base.transform.root.gameObject.GetComponent<ItemPropsScript>();
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
	}

	private void Update()
	{
		this.itemHolder.localPosition = new Vector3(0f, 1f + 0.5f * Mathf.Sin(Time.time), 0f);
		this.itemHolder.RotateAround(Vector3.up, 1f * Time.deltaTime);
	}

	private void ChangeItemState(int state)
	{
		this.Init();
		this.IPS.state = state;
		if (this.itemGO != null)
		{
			UnityEngine.Object.Destroy(this.itemGO);
		}
		if (state != 0)
		{
			this.itemGO = (UnityEngine.Object.Instantiate(Kube.ASS3.findItemsPrefabs[state - 1], Vector3.zero, Quaternion.identity) as GameObject);
			this.itemGO.transform.parent = this.itemHolder;
			this.itemGO.transform.localPosition = Vector3.zero;
			this.itemGO.transform.localRotation = Quaternion.identity;
			this.itemType = state - 1;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.IPS.state != 0 && Kube.BCS.gameType != GameType.creating && other.gameObject.transform.root.gameObject.layer == 9)
		{
			Kube.BCS.SendMessage("FoundItem", this.IPS.state);
			UnityEngine.Object.Instantiate(Kube.ASS4.soundGetItem, base.transform.position, base.transform.rotation);
			this.ChangeItemState(0);
		}
	}

	private void setupGUI()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		float num3 = 0.5f * num - 350f;
		float num4 = num2 - 320f;
		GUI.skin = Kube.ASS1.mainSkin;
		GUI.DrawTexture(new Rect(num3, num4, 700f, 240f), Kube.ASS3.setupItemTex);
		GUI.skin = Kube.ASS1.bigWhiteLabel;
		GUI.Label(new Rect(num3 + 20f, num4 + 10f, 300f, 40f), Localize.find_item_choose_type);
		GUI.skin = Kube.ASS1.triggerSkin;
		GUI.Label(new Rect(num3 + 10f, num4 + 50f, 150f, 30f), Localize.find_item_type);
		GUI.skin = Kube.ASS1.triggerSkinArrowLeft;
		if (GUI.Button(new Rect(num3 + 10f, num4 + 85f, 50f, 30f), string.Empty))
		{
			this._visibleItemType--;
			if (this._visibleItemType < 0)
			{
				this._visibleItemType = Kube.ASS3.findItemsPrefabs.Length - 1;
			}
		}
		GUI.skin = Kube.ASS1.triggerSkinArrowRight;
		if (GUI.Button(new Rect(num3 + 310f, num4 + 85f, 50f, 30f), string.Empty))
		{
			this._visibleItemType++;
			if (this._visibleItemType >= Kube.ASS3.findItemsPrefabs.Length)
			{
				this._visibleItemType = 0;
			}
		}
		GUI.skin = Kube.ASS1.mainSkin;
		GUI.Label(new Rect(num3 + 60f, num4 + 85f, 250f, 30f), Localize.findPrefabsNames[this._visibleItemType]);
		GUI.skin = Kube.ASS1.triggerSkin;
		if (GUI.Button(new Rect(num3 + 500f, num4 + 140f, 180f, 50f), Localize.apply))
		{
			this.NO.ChangeItemState(this.IPS.id, this._visibleItemType + 1);
			Kube.OH.closeMenu(null);
		}
	}

	private void SetupItem()
	{
		this._visibleItemType = this.itemType;
		if (Kube.BCS.gameType == GameType.creating)
		{
			Kube.OH.openMenu(new DrawCall(this.setupGUI), true, false);
		}
	}

	private ItemPropsScript IPS;

	private NetworkObjectScript NO;

	private GameObject itemGO;

	private int itemType;

	public Transform itemHolder;

	private bool initialized;

	private int _visibleItemType;
}
