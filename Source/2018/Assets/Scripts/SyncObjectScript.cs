using System;
using Photon;

public class SyncObjectScript : MonoBehaviour
{
	public void SetHealthMultiplier(int value)
	{
	}

	public void SetDamageMultiplier(int value)
	{
	}

	public void SetRespawnNum(int _id)
	{
		this.objectId = _id;
	}

	public void SaveCodeVars()
	{
	}

	public void LoadCodeVars()
	{
	}

	[NonSerialized]
	public int objectId = -1;
}
