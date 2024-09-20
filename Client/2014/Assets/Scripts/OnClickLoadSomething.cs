using System;
using UnityEngine;

public class OnClickLoadSomething : MonoBehaviour
{
	public void OnClick()
	{
		OnClickLoadSomething.ResourceTypeOption resourceTypeToLoad = this.ResourceTypeToLoad;
		if (resourceTypeToLoad != OnClickLoadSomething.ResourceTypeOption.Scene)
		{
			if (resourceTypeToLoad == OnClickLoadSomething.ResourceTypeOption.Web)
			{
				Application.OpenURL(this.ResourceToLoad);
			}
		}
		else
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(this.ResourceToLoad);
		}
	}

	public OnClickLoadSomething.ResourceTypeOption ResourceTypeToLoad;

	public string ResourceToLoad;

	public enum ResourceTypeOption : byte
	{
		Scene,
		Web
	}
}
