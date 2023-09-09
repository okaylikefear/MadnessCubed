using System;
using UnityEngine;

public class FilmsTestGameController : MonoBehaviour
{
	private void Start()
	{
		GameObject.FindGameObjectWithTag("FilmManager").SendMessage("PlayScene", "1");
	}

	private void Update()
	{
		if (!this.started)
		{
			GameObject.FindGameObjectWithTag("FilmManager").SendMessage("PlayScene", "1");
			this.started = true;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("LoadData");
		}
	}

	private void OnGUI()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		GUI.Box(new Rect(0.4f * num, 0.85f * num2, 0.2f * num, 30f), "Пробел - пропустить");
	}

	private bool started;
}
