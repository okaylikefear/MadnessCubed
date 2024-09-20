using System;

public class TabRowCreating : TabRow
{
	private void Start()
	{
		this.allow.GetComponent<ToggleTextButton>().states = new string[]
		{
			Localize.BCS_allowBuild,
			Localize.BCS_forbidBuild
		};
	}

	public void OnBan()
	{
		base.transform.parent.parent.GetComponent<CreatingTab>().BanPlayer(this.id);
	}

	public void OnAllow()
	{
		ToggleTextButton current = ToggleTextButton.current;
		base.transform.parent.parent.GetComponent<CreatingTab>().ChangeCanBuildStatus(this.id, current.value);
	}

	public UIButton ban;

	public UIButton allow;
}
