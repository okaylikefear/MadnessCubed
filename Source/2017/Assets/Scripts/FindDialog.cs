using System;
using UnityEngine;

public class FindDialog : MonoBehaviour
{
	private void Start()
	{
		this.onShow();
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		this.onShow();
	}

	private void onShow()
	{
		this.input.value = string.Empty;
	}

	public void Open(string title, global::AsyncCallback cb, UIInput.Validation val = UIInput.Validation.Integer)
	{
		this.title.text = title;
		this.input.validation = val;
		this.onFind = cb;
		base.gameObject.SetActive(true);
	}

	public void OnClick()
	{
		FindDialog.instance = this;
		this.onFind();
		base.gameObject.SetActive(false);
	}

	public UILabel title;

	public UILabel label;

	public UIInput input;

	private global::AsyncCallback onFind;

	public static FindDialog instance;
}
