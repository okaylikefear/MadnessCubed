using System;
using kube;
using UnityEngine;

public class NameDialog : MonoBehaviour
{
	private void OnEnable()
	{
		this.applyBtn.isEnabled = false;
		UIInput.current = this.nickname;
	}

	private void Update()
	{
		this.applyBtn.isEnabled = !string.IsNullOrEmpty(this.nickname.value);
	}

	private void UpdateName(string ans)
	{
		string[] array = ans.Split(new char[]
		{
			';'
		});
		if (array[0] != "1")
		{
			Cub2UI.MessageBox(Localize.nick_taken, null);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
		this.nickname.value = Kube.GPS.playerName;
	}

	public void onApply()
	{
		string value = this.nickname.value;
		if (value.Length >= 3)
		{
			Kube.SS.SaveNewName(Kube.SS.serverId, value);
		}
		else
		{
			Cub2UI.MessageBox("Имя должно быть длиннее 3х символов", null);
		}
	}

	public UIInput nickname;

	public UIButton applyBtn;
}
