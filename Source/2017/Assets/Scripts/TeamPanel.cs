using System;
using UnityEngine;

public class TeamPanel : TriggerScript
{
	public override void SetParameters(int _x, int _y, int _z, int _type, int _state, int _delayTime, int _condActivate, int _condKey, int _id)
	{
		base.SetParameters(_x, _y, _z, _type, _state, _delayTime, _condActivate, _condKey, _id);
		MeshRenderer componentInChildren = base.GetComponentInChildren<MeshRenderer>();
		if (this.cond2_red)
		{
			componentInChildren.material = this.colors[0];
		}
		if (this.cond2_green)
		{
			componentInChildren.material = this.colors[1];
		}
		if (this.cond2_blue)
		{
			componentInChildren.material = this.colors[2];
		}
		if (this.cond2_gold)
		{
			componentInChildren.material = this.colors[3];
		}
	}

	public Material[] colors;
}
