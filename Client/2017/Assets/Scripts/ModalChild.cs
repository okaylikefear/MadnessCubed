using System;
using UnityEngine;

public class ModalChild : MonoBehaviour
{
	public void getBlack(bool b)
	{
		if (this.black == null)
		{
			this.black = base.transform.Find("black").gameObject;
		}
		this.black.SetActive(b);
		if (b)
		{
			UISprite component = this.black.GetComponent<UISprite>();
			component.width = Mathf.FloorToInt((float)Cub2UI.activeWidth);
			component.height = Mathf.FloorToInt((float)Cub2UI.activeHeight);
		}
	}

	private void Start()
	{
		this.getBlack(true);
		if (base.enabled)
		{
			ModalPanel.open(this);
		}
	}

	private void Update()
	{
	}

	public void OnEnable()
	{
		ModalPanel.open(this);
	}

	public void OnDisable()
	{
		ModalPanel.close(this);
	}

	public void CloseOk()
	{
		this.Close(1);
	}

	public void Close(int result)
	{
		ModalChild.current = this;
		this.modalResult = result;
		if (this.onClose != null)
		{
			this.onClose.Execute();
		}
		base.gameObject.SetActive(false);
	}

	public static ModalChild current;

	public GameObject black;

	public EventDelegate onClose;

	[NonSerialized]
	public int modalResult;
}
