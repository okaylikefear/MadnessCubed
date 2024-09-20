using System;
using UnityEngine;

public class UIToggleIcon : MonoBehaviour
{
	private void Start()
	{
		UIButton component = base.GetComponent<UIButton>();
		this.enabledSprite.SetActive(component.isEnabled);
		this.disabledSprite.SetActive(!component.isEnabled);
	}

	private void Update()
	{
	}

	public GameObject enabledSprite;

	public GameObject disabledSprite;
}
