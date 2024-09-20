using System;
using kube;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
	private void PlaySoundTutor(int numOfTutor)
	{
		if (this.soundTutorGO != null)
		{
			UnityEngine.Object.Destroy(this.soundTutorGO);
		}
		if (numOfTutor < this.soundTutor.Length && this.soundTutor[this.currentNumOfTutor - 1] != null)
		{
			this.soundTutorGO = (UnityEngine.Object.Instantiate(this.soundTutor[this.currentNumOfTutor - 1], Vector3.zero, Quaternion.identity) as GameObject);
			if (this.MMS != null)
			{
				this.MMS.Mute(true);
			}
			base.Invoke("MusicMuteOff", this.soundTutorGO.audio.clip.length);
		}
	}

	private void ChangeState(int numOfTutor)
	{
		if (this.currentNumOfTutor < numOfTutor)
		{
			base.CancelInvoke("MusicMuteOff");
			this.tutorTimeBegin = Time.time;
			this.currentNumOfTutor = numOfTutor;
			this.currentStepOfTutor = 0;
			GameObject tutorialMessage = Kube.BCS.hud.tutorialMessage;
			tutorialMessage.SetActive(true);
			tutorialMessage.GetComponentInChildren<UILabel>().text = Localize.strTutor[this.currentNumOfTutor - 1];
			tutorialMessage.GetComponent<UITweener>().enabled = true;
			tutorialMessage.GetComponent<UITweener>().ResetToBeginning();
			tutorialMessage.GetComponent<UITweener>().PlayForward();
			Kube.GPS.needTraining = false;
			this.PlaySoundTutor(numOfTutor);
			Kube.SS.SendStat("Tutor" + numOfTutor);
			if (numOfTutor < 2)
			{
				for (int i = 0; i < 10; i++)
				{
					Kube.GPS.fastInventarWeapon[i].Type = -1;
					Kube.GPS.fastInventarWeapon[i].Num = 0;
					Kube.IS.ChoseFastInventar(0);
				}
			}
			if (numOfTutor == 8)
			{
				int index = 5;
				Kube.BCS.ps.clips[index] = 0;
			}
			if (numOfTutor == 21)
			{
				base.Invoke("ChangeToTutor22", 7f);
			}
			if (numOfTutor == 24)
			{
				this.timeOfNightBeginning = Time.time;
				base.Invoke("ChangeToTutor25", 7f);
			}
			if (numOfTutor == 27)
			{
				for (int j = 0; j < 10; j++)
				{
					Kube.GPS.fastInventar[j].Type = 0;
					Kube.GPS.fastInventar[j].Num = Kube.IS.cubesNatureNums[j];
				}
				this.timeOfNightBeginning = Time.time;
				base.Invoke("EndBuildingTutor", 7f);
			}
			Kube.SS.SendStatIoTrack("tutor" + numOfTutor, 1);
		}
	}

	private void ChangeToTutor21()
	{
		this.ChangeState(21);
	}

	private void ChangeToTutor22()
	{
		this.ChangeState(22);
	}

	private void ChangeToTutor25()
	{
		this.ChangeState(25);
	}

	private void EndBuildingTutor()
	{
		this.currentNumOfTutor = -1;
	}

	private void Awake()
	{
		Kube.TS = this;
	}

	private void OnDestroy()
	{
		Kube.TS = null;
	}

	private void MusicMuteOff()
	{
		if (this.MMS != null)
		{
			this.MMS.Mute(false);
		}
	}

	private void Start()
	{
		this.MMS = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManagerScript>();
		for (int i = 0; i < this.tutorialSounds.Length; i++)
		{
			if (this.tutorialSounds[i].locale == Kube.GPS.locale)
			{
				this.soundTutor = this.tutorialSounds[i].soundTutor;
			}
		}
	}

	private void StartMissionTutor()
	{
		Kube.BCS.gameTypeController.canRespawn = true;
	}

	private void StartCreatingTutor()
	{
		if (Kube.GPS.needTrainingBuild && Kube.BCS.gameType == GameType.creating)
		{
			for (int i = 0; i < 10; i++)
			{
				Kube.GPS.fastInventar[i].Type = -1;
				Kube.GPS.fastInventar[i].Num = 0;
			}
			base.Invoke("ChangeToTutor21", 2f);
		}
	}

	private void Update()
	{
		if (this.currentNumOfTutor == 24)
		{
			Kube.WHS.SetDayLight(1f - (Time.time - this.timeOfNightBeginning) * 0.2f);
		}
		if (this.currentNumOfTutor == 27)
		{
			Kube.WHS.SetDayLight((Time.time - this.timeOfNightBeginning) * 0.2f);
		}
	}

	private void totorStep5()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		if (this.currentStepOfTutor == 0)
		{
			if (!Kube.OH.hasMenu("Decor_menu"))
			{
				GUI.DrawTexture(new Rect(num * 0.5f - 150f, num2 - 120f, 300f, 50f), this.tutorMiniBackground);
				GUI.Label(new Rect(num * 0.5f - 145f, num2 - 120f, 290f, 50f), Localize.tutor_open_inventory);
				Color color = new Color(1f, 1f, 1f, 0.5f * Mathf.Sin(Time.time * 6f) + 0.5f);
				GUI.color = color;
				GUI.DrawTexture(new Rect(-20f, num2 - 100f, (float)Kube.ASS3.inventarCaseTex.width, (float)Kube.ASS3.inventarCaseTex.height), Kube.ASS3.inventarCaseLightTex);
				GUI.skin = Kube.ASS1.bigBlackLabel;
				GUI.Label(new Rect(57f, num2 - 37f, 25f, 25f), "C");
				GUI.skin = Kube.ASS1.bigWhiteLabel;
				GUI.Label(new Rect(52f, num2 - 39f, 25f, 25f), "C");
				GUI.DrawTexture(new Rect(70f, num2 - 270f, (float)this.arrowTex.width, (float)this.arrowTex.height), this.arrowTex);
			}
			else
			{
				this.currentStepOfTutor++;
			}
			return;
		}
		if (Kube.GPS.fastInventarWeapon[0].Type == 4)
		{
			if (Kube.OH.hasMenu("Decor_menu"))
			{
				GUI.DrawTexture(new Rect(num * 0.5f - 150f, num2 - 120f, 300f, 50f), this.tutorMiniBackground);
				GUI.Label(new Rect(num * 0.5f - 145f, num2 - 120f, 290f, 50f), Localize.tutor_close_inventory);
				Color color2 = new Color(1f, 1f, 1f, 0.5f * Mathf.Sin(Time.time * 6f) + 0.5f);
				GUI.color = color2;
				GUI.DrawTexture(new Rect(num * 0.5f + 380f, 249f, (float)(-(float)this.arrowTex.width), (float)(-(float)this.arrowTex.height)), this.arrowTex);
				GUI.DrawTexture(new Rect(-20f, num2 - 100f, (float)Kube.ASS3.inventarCaseTex.width, (float)Kube.ASS3.inventarCaseTex.height), Kube.ASS3.inventarCaseLightTex);
				GUI.skin = Kube.ASS1.bigBlackLabel;
				GUI.Label(new Rect(57f, num2 - 37f, 25f, 25f), "C");
				GUI.skin = Kube.ASS1.bigWhiteLabel;
				GUI.Label(new Rect(52f, num2 - 39f, 25f, 25f), "C");
			}
			else
			{
				GUI.DrawTexture(new Rect(num * 0.5f - 150f, num2 - 120f, 300f, 50f), this.tutorMiniBackground);
				GUI.Label(new Rect(num * 0.5f - 145f, num2 - 120f, 290f, 50f), Localize.tutor_kill_zombie_and_go);
			}
			return;
		}
		if (Kube.OH.hasMenu("Decor_menu"))
		{
			if (Kube.IS.inventaryType != InventarMenu.weapons)
			{
				GUI.DrawTexture(new Rect(num * 0.5f - 150f, num2 - 120f, 300f, 50f), this.tutorMiniBackground);
				GUI.Label(new Rect(num * 0.5f - 145f, num2 - 120f, 290f, 50f), Localize.tutor_choose_weapons);
				Color color3 = new Color(1f, 1f, 1f, 0.5f * Mathf.Sin(Time.time * 6f) + 0.5f);
				GUI.color = color3;
				GUI.DrawTexture(new Rect(num * 0.5f - 290f, 140f, (float)this.arrowTex.width, (float)this.arrowTex.height), this.arrowTex);
			}
			else if (Kube.IS.inventaryType == InventarMenu.weapons && Kube.IS.inventoryPageType != 0)
			{
				GUI.DrawTexture(new Rect(num * 0.5f - 150f, num2 - 120f, 300f, 50f), this.tutorMiniBackground);
				GUI.Label(new Rect(num * 0.5f - 145f, num2 - 120f, 290f, 50f), Localize.tutor_choose_weapons);
				Color color4 = new Color(1f, 1f, 1f, 0.5f * Mathf.Sin(Time.time * 6f) + 0.5f);
				GUI.color = color4;
				GUI.DrawTexture(new Rect(num * 0.5f + 40f, -50f, (float)this.arrowTex.width, (float)this.arrowTex.height), this.arrowTex);
			}
			else if (Kube.IS.inventaryType == InventarMenu.weapons && Kube.IS.inventoryPageType == 0 && Kube.IS.chosenInventarItem.Num != 0)
			{
				GUI.DrawTexture(new Rect(num * 0.5f - 150f, num2 - 120f, 300f, 50f), this.tutorMiniBackground);
				GUI.Label(new Rect(num * 0.5f - 145f, num2 - 120f, 290f, 50f), Localize.tutor_choose_axe);
				Color color5 = new Color(1f, 1f, 1f, 0.5f * Mathf.Sin(Time.time * 6f) + 0.5f);
				GUI.color = color5;
				GUI.DrawTexture(new Rect(num * 0.5f - 160f, 10f, (float)this.arrowTex.width, (float)this.arrowTex.height), this.arrowTex);
			}
			else if (Kube.IS.inventaryType == InventarMenu.weapons && Kube.IS.inventoryPageType == 0 && Kube.IS.chosenInventarItem.Num == 0 && Kube.GPS.inventarWeapons[0] <= (int)Time.time)
			{
				GUI.DrawTexture(new Rect(num * 0.5f - 150f, num2 - 120f, 300f, 50f), this.tutorMiniBackground);
				GUI.Label(new Rect(num * 0.5f - 145f, num2 - 120f, 290f, 50f), Localize.tutor_buy_axe_forever);
				Color color6 = new Color(1f, 1f, 1f, 0.5f * Mathf.Sin(Time.time * 6f) + 0.5f);
				GUI.color = color6;
				GUI.DrawTexture(new Rect(num * 0.5f - 160f + 405f, 283f, (float)this.arrowTex.width, (float)this.arrowTex.height), this.arrowTex);
			}
			else if (Kube.IS.inventaryType == InventarMenu.weapons && Kube.IS.inventoryPageType == 0 && Kube.IS.chosenInventarItem.Num == 0 && Kube.GPS.inventarWeapons[0] > (int)Time.time && (Kube.GPS.fastInventarWeapon[0].Type != 4 || Kube.GPS.fastInventarWeapon[0].Num != 0))
			{
				GUI.DrawTexture(new Rect(num * 0.5f - 150f, num2 - 120f, 300f, 50f), this.tutorMiniBackground);
				GUI.Label(new Rect(num * 0.5f - 145f, num2 - 120f, 290f, 50f), Localize.tutor_place_axe_inventar);
				Color color7 = new Color(1f, 1f, 1f, 0.5f * Mathf.Sin(Time.time * 6f) + 0.5f);
				GUI.color = color7;
				GUI.DrawTexture(new Rect(num * 0.5f - 291f, num2 - 224f, (float)this.arrowTex.width, (float)this.arrowTex.height), this.arrowTex);
			}
		}
	}

	private void DestroyedCube()
	{
		if (this.currentNumOfTutor == 23)
		{
			this.cubesToDestroy--;
			if (this.cubesToDestroy == 0)
			{
				this.ChangeState(24);
			}
		}
	}

	private void PlacedCube()
	{
		this.cubesToBuild--;
		if (this.cubesToBuild == 0)
		{
			this.ChangeState(23);
		}
	}

	private void PlacedCubelikeItem()
	{
		if (this.currentNumOfTutor == 25)
		{
			this.ChangeState(26);
		}
	}

	private void MapSaved()
	{
		if (this.currentNumOfTutor == 26)
		{
			this.ChangeState(27);
			base.Invoke("ClearTutor", 10f);
		}
	}

	private void ClearTutor()
	{
		GameObject tutorialMessage = Kube.BCS.hud.tutorialMessage;
		tutorialMessage.SetActive(false);
	}

	private void ReloadedGun()
	{
		this.reloadedGun = true;
	}

	public int currentNumOfTutor;

	public int currentStepOfTutor;

	public TutorialSounds[] tutorialSounds;

	public GameObject[] soundTutor;

	private GameObject soundTutorGO;

	public Texture tutorBackground;

	public Texture tutorMiniBackground;

	public Texture arrowTex;

	private bool reloadedGun;

	private MusicManagerScript MMS;

	private int cubesToBuild = 5;

	private int maxCubesToBuild = 5;

	private int cubesToDestroy = 5;

	private int maxCubesToDestroy = 5;

	private float tutorTimeBegin;

	private float timeOfNightBeginning;

	private float fullMessageTime = 10f;

	public GUISkin skin;
}
