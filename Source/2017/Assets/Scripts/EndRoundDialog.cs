using System;
using UnityEngine;

public class EndRoundDialog : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Open(EndGameStats endGameStats, int endGameTime)
	{
		this.xp.text = endGameStats.deltaExp.ToString();
		this.frags.text = endGameStats.playerFrags.ToString();
		this.time.text = endGameTime.ToString();
		this.frps.text = Mathf.CeilToInt((float)endGameStats.playerFrags / (float)endGameTime).ToString();
		this.money1.text = endGameStats.deltaMoney.ToString();
		base.gameObject.SetActive(true);
	}

	public void exitDialog()
	{
		PhotonNetwork.LeaveRoom();
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
	}

	public UILabel title;

	public UILabel xp;

	public UILabel frags;

	public UILabel time;

	public UILabel frps;

	public UILabel money1;
}
