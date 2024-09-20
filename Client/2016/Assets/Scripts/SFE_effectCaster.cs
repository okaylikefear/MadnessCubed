using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class SFE_effectCaster : MonoBehaviour
{
	public SFE_effectCaster()
	{
		this.camCurrent = 1;
	}

	public virtual void Start()
	{
		this.selected = Extensions.get_length(this.createThis) - 1;
		this.writeThis.text = this.selected.ToString() + " " + this.createThis[this.selected].name;
	}

	public virtual void Update()
	{
		if (this.cooldown > (float)0)
		{
			this.cooldown -= Time.deltaTime;
		}
		if (this.changeCooldown > (float)0)
		{
			this.changeCooldown -= Time.deltaTime;
		}
		Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
		if (Physics.Raycast(ray, out this.hit, (float)1000, this.layermask))
		{
			this.moveThis.transform.position = this.hit.point;
			if (Input.GetMouseButton(0) && this.cooldown <= (float)0)
			{
				this.effect = (GameObject)UnityEngine.Object.Instantiate(this.createThis[this.selected], this.spaceShip.transform.position, this.spaceShip.transform.rotation);
				this.effect.transform.parent = this.spaceShip.transform;
				this.cooldown = 0.5f;
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) && this.changeCooldown <= (float)0)
		{
			this.selected++;
			if (this.selected > Extensions.get_length(this.createThis) - 1)
			{
				this.selected = 0;
			}
			this.writeThis.text = this.selected.ToString() + " " + this.createThis[this.selected].name;
			this.changeCooldown = 0.1f;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) && this.changeCooldown <= (float)0)
		{
			this.selected--;
			if (this.selected < 0)
			{
				this.selected = Extensions.get_length(this.createThis) - 1;
			}
			this.writeThis.text = this.selected.ToString() + " " + this.createThis[this.selected].name;
			this.changeCooldown = 0.1f;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Space) && this.changeCooldown <= (float)0)
		{
			if (this.camCurrent == 1)
			{
				this.camCurrent = 2;
				this.camRoot.transform.position = this.camPoint2.transform.position;
				this.camRoot.transform.rotation = this.camPoint2.transform.rotation;
			}
			else
			{
				this.camCurrent = 1;
				this.camRoot.transform.position = this.camPoint1.transform.position;
				this.camRoot.transform.rotation = this.camPoint1.transform.rotation;
			}
			this.changeCooldown = 0.1f;
		}
	}

	public virtual void Main()
	{
	}

	public GameObject moveThis;

	public GameObject camRoot;

	public GameObject camPoint1;

	public GameObject camPoint2;

	private int camCurrent;

	public GameObject spaceShip;

	private RaycastHit hit;

	public GameObject[] createThis;

	private float cooldown;

	private float changeCooldown;

	private int selected;

	public GUIText writeThis;

	private float rndNr;

	private GameObject effect;

	public LayerMask layermask;
}
