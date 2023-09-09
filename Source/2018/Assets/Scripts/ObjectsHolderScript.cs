using System;
using System.Collections;
using System.Collections.Generic;
using kube;
using kube.data;
using kube.game;
using kube.ui;
using UnityEngine;

public class ObjectsHolderScript : MonoBehaviour
{
	[ContextMenu("DoCubeGroups")]
	private void DoCubeGroups()
	{
		InventoryScript component = base.GetComponent<InventoryScript>();
		int[] array = component.cubesNatureNums;
		ObjectsHolderScript.CubeGroup group = ObjectsHolderScript.CubeGroup.cubesNature;
		foreach (int num in array)
		{
			this.blockTypes[num].group = group;
		}
		array = component.cubesBuilderNums;
		group = ObjectsHolderScript.CubeGroup.cubesBuilder;
		foreach (int num2 in array)
		{
			this.blockTypes[num2].group = group;
		}
		array = component.cubesGlassNums;
		group = ObjectsHolderScript.CubeGroup.cubesGlass;
		foreach (int num3 in array)
		{
			this.blockTypes[num3].group = group;
		}
		array = component.cubesDecorNums;
		group = ObjectsHolderScript.CubeGroup.cubesDecor;
		foreach (int num4 in array)
		{
			this.blockTypes[num4].group = group;
		}
		array = component.cubesWaterNums;
		group = ObjectsHolderScript.CubeGroup.cubesWater;
		foreach (int num5 in array)
		{
			this.blockTypes[num5].group = group;
		}
		array = component.cubesDifferentNums;
		group = ObjectsHolderScript.CubeGroup.cubesDifferent;
		foreach (int num6 in array)
		{
			this.blockTypes[num6].group = group;
		}
	}

	[ContextMenu("DoAtlas")]
	private void DoAtlas()
	{
		int num = 0;
		int[] array = new int[]
		{
			-3,
			-4
		};
		for (int i = 0; i < this.blockTypes.Length; i++)
		{
			if (this.blockTypes[i].type == 0)
			{
				int atlas = i / 64;
				if (i % 64 == 0)
				{
					num = 0;
				}
				if (this.blockTypes[i].itemId < 0)
				{
					if (Array.IndexOf<int>(array, this.blockTypes[i].itemId) != -1)
					{
						num++;
					}
					else
					{
						num += 2;
					}
				}
				else if (this.blockTypes[i].atlas < 0)
				{
					if (i == 0)
					{
						num++;
					}
				}
				else
				{
					this.blockTypes[i].itemId = num % 64;
					this.blockTypes[i].atlas = atlas;
					num++;
				}
			}
		}
	}

	public ObjectsHolderScript.BuiltInMap findMapInfo(long id)
	{
		for (int i = 0; i < this.builtInMaps.Length; i++)
		{
			if ((long)this.builtInMaps[i].Id == id)
			{
				return this.builtInMaps[i];
			}
		}
		return null;
	}

	public ObjectsHolderScript.BuiltInMap[] findMaps(GameType gameType)
	{
		List<ObjectsHolderScript.BuiltInMap> list = new List<ObjectsHolderScript.BuiltInMap>();
		for (int i = 0; i < this.builtInMaps.Length; i++)
		{
			if (gameType < (GameType)this.builtInMaps[i].gameTypes.Length && this.builtInMaps[i].gameTypes[(int)gameType])
			{
				list.Add(this.builtInMaps[i]);
			}
		}
		return list.ToArray();
	}

	public ObjectsHolderScript.BuiltInMap[] findCreatingMaps(bool showBig)
	{
		GameType gameType = GameType.creating;
		List<ObjectsHolderScript.BuiltInMap> list = new List<ObjectsHolderScript.BuiltInMap>();
		for (int i = 0; i < this.builtInMaps.Length; i++)
		{
			if (gameType < (GameType)this.builtInMaps[i].gameTypes.Length && this.builtInMaps[i].gameTypes[(int)gameType] && (this.builtInMaps[i].size == 0 || Kube.GPS.isVIP || (this.builtInMaps[i].size > 0 && showBig)))
			{
				list.Add(this.builtInMaps[i]);
			}
		}
		return list.ToArray();
	}

	private void networkErrorGUI()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		GUI.skin = Kube.ASS1.mainSkinSmall;
		GUI.Box(new Rect(0.5f * num - 150f, 0.5f * num2 - 100f, 300f, 60f), Localize.server_error);
	}

	public void ServerError()
	{
		if (this.isError)
		{
			return;
		}
		this.isError = true;
		this.closeMenuAll();
		UnityEngine.SceneManagement.SceneManager.LoadScene("Empty");
	}

	public void PlayerSparks(SoundMaterialType smt, SoundHitType sht, Vector3 pos, Vector3 normal)
	{
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
		GameObject original = Kube.ASS3.sparksStoneBullet;
		if (smt == SoundMaterialType.glass)
		{
			original = Kube.ASS3.sparksGlassBullet;
		}
		if (smt == SoundMaterialType.ground)
		{
			original = Kube.ASS3.sparksGroundBullet;
		}
		if (smt == SoundMaterialType.metal)
		{
			original = Kube.ASS3.sparksMetalBullet;
		}
		if (smt == SoundMaterialType.water)
		{
			original = Kube.ASS3.sparksWaterBullet;
		}
		if (smt == SoundMaterialType.wood)
		{
			original = Kube.ASS3.sparksWoodBullet;
		}
		CachedObject.Instantiate(original, pos, rotation);
	}

	public void PlayerBlood(Vector3 pos, Vector3 normal, Pawn pawn)
	{
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
		UnityEngine.Object.Instantiate(Kube.ASS3.bloodSplash, pos, rotation);
		GameObject gameObject = pawn.gameObject;
		if (pos.y - pawn.transform.position.y > 1.1f && pos.y - pawn.transform.position.y < 1.8f)
		{
			UnityEngine.Object.Instantiate(Kube.ASS3.bloodSplash, pos, rotation);
		}
	}

	public void PlayMaterialSound(SoundMaterialType smt, SoundHitType sht, Vector3 pos, float strength)
	{
		if (smt == SoundMaterialType.ground)
		{
			if (sht == SoundHitType.bullet)
			{
				if (Kube.ASS4.soundGroundBullet.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundGroundBullet[UnityEngine.Random.Range(0, Kube.ASS4.soundGroundBullet.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.axe)
			{
				if (Kube.ASS4.soundGroundAxe.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundGroundAxe[UnityEngine.Random.Range(0, Kube.ASS4.soundGroundAxe.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.footSteps)
			{
				if (Kube.ASS4.soundGroundFootsteps.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundGroundFootsteps[UnityEngine.Random.Range(0, Kube.ASS4.soundGroundFootsteps.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.breaking && Kube.ASS4.soundGroundBreak.Length != 0)
			{
				UnityEngine.Object.Instantiate(Kube.ASS4.soundGroundBreak[UnityEngine.Random.Range(0, Kube.ASS4.soundGroundBreak.Length)], pos, Quaternion.identity);
			}
		}
		else if (smt == SoundMaterialType.metal)
		{
			if (sht == SoundHitType.bullet)
			{
				if (Kube.ASS4.soundMetalBullet.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundMetalBullet[UnityEngine.Random.Range(0, Kube.ASS4.soundMetalBullet.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.axe)
			{
				if (Kube.ASS4.soundMetalAxe.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundMetalAxe[UnityEngine.Random.Range(0, Kube.ASS4.soundMetalAxe.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.footSteps)
			{
				if (Kube.ASS4.soundMetalFootsteps.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundMetalFootsteps[UnityEngine.Random.Range(0, Kube.ASS4.soundMetalFootsteps.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.breaking && Kube.ASS4.soundMetalBreak.Length != 0)
			{
				UnityEngine.Object.Instantiate(Kube.ASS4.soundMetalBreak[UnityEngine.Random.Range(0, Kube.ASS4.soundMetalBreak.Length)], pos, Quaternion.identity);
			}
		}
		else if (smt == SoundMaterialType.wood)
		{
			if (sht == SoundHitType.bullet)
			{
				if (Kube.ASS4.soundWoodBullet.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundWoodBullet[UnityEngine.Random.Range(0, Kube.ASS4.soundWoodBullet.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.axe)
			{
				if (Kube.ASS4.soundWoodAxe.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundWoodAxe[UnityEngine.Random.Range(0, Kube.ASS4.soundWoodAxe.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.footSteps)
			{
				if (Kube.ASS4.soundWoodFootsteps.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundWoodFootsteps[UnityEngine.Random.Range(0, Kube.ASS4.soundWoodFootsteps.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.breaking && Kube.ASS4.soundWoodBreak.Length != 0)
			{
				UnityEngine.Object.Instantiate(Kube.ASS4.soundWoodBreak[UnityEngine.Random.Range(0, Kube.ASS4.soundWoodBreak.Length)], pos, Quaternion.identity);
			}
		}
		else if (smt == SoundMaterialType.stone)
		{
			if (sht == SoundHitType.bullet)
			{
				if (Kube.ASS4.soundStoneBullet.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundStoneBullet[UnityEngine.Random.Range(0, Kube.ASS4.soundStoneBullet.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.axe)
			{
				if (Kube.ASS4.soundStoneAxe.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundStoneAxe[UnityEngine.Random.Range(0, Kube.ASS4.soundStoneAxe.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.footSteps)
			{
				if (Kube.ASS4.soundStoneFootsteps.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundStoneFootsteps[UnityEngine.Random.Range(0, Kube.ASS4.soundStoneFootsteps.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.breaking && Kube.ASS4.soundStoneBreak.Length != 0)
			{
				UnityEngine.Object.Instantiate(Kube.ASS4.soundStoneBreak[UnityEngine.Random.Range(0, Kube.ASS4.soundStoneBreak.Length)], pos, Quaternion.identity);
			}
		}
		else if (smt == SoundMaterialType.glass)
		{
			if (sht == SoundHitType.bullet)
			{
				if (Kube.ASS4.soundGlassBullet.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundGlassBullet[UnityEngine.Random.Range(0, Kube.ASS4.soundGlassBullet.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.axe)
			{
				if (Kube.ASS4.soundGlassAxe.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundGlassAxe[UnityEngine.Random.Range(0, Kube.ASS4.soundGlassAxe.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.footSteps)
			{
				if (Kube.ASS4.soundGlassFootsteps.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundGlassFootsteps[UnityEngine.Random.Range(0, Kube.ASS4.soundGlassFootsteps.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.breaking && Kube.ASS4.soundGlassBreak.Length != 0)
			{
				UnityEngine.Object.Instantiate(Kube.ASS4.soundGlassBreak[UnityEngine.Random.Range(0, Kube.ASS4.soundGlassBreak.Length)], pos, Quaternion.identity);
			}
		}
		else if (smt == SoundMaterialType.water)
		{
			if (sht == SoundHitType.bullet)
			{
				if (Kube.ASS4.soundWaterBullet.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundWaterBullet[UnityEngine.Random.Range(0, Kube.ASS4.soundWaterBullet.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.axe)
			{
				if (Kube.ASS4.soundWaterAxe.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundWaterAxe[UnityEngine.Random.Range(0, Kube.ASS4.soundWaterAxe.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.footSteps)
			{
				if (Kube.ASS4.soundWaterFootsteps.Length != 0)
				{
					UnityEngine.Object.Instantiate(Kube.ASS4.soundWaterFootsteps[UnityEngine.Random.Range(0, Kube.ASS4.soundWaterFootsteps.Length)], pos, Quaternion.identity);
				}
			}
			else if (sht == SoundHitType.breaking && Kube.ASS4.soundWaterBreak.Length != 0)
			{
				UnityEngine.Object.Instantiate(Kube.ASS4.soundWaterBreak[UnityEngine.Random.Range(0, Kube.ASS4.soundWaterBreak.Length)], pos, Quaternion.identity);
			}
		}
	}

	public void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
	}

	private void OnDestroy()
	{
		Kube.OH = null;
	}

	private void Awake()
	{
		Kube.OH = this;
	}

	private void Start()
	{
		this.Init();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Application.runInBackground = true;
		this.gravity = -15f;
		this.friends = new ObjectsHolderScript.FriendInfo[1];
		this.friends[0].Id = 1185293;
		this.friends[0].uid = "1185293";
		this.friends[0].Name = "Павел Логинов";
		for (int i = 0; i < 10; i++)
		{
		}
		base.Invoke("ImHereSEC30", 30f);
		base.Invoke("ImHereMIN1", 60f);
		base.Invoke("ImHereMIN2", 120f);
		base.Invoke("ImHereMIN5", 300f);
		base.Invoke("ImHereMIN10", 600f);
		base.Invoke("ImHereMIN20", 1200f);
		base.Invoke("ImHereMIN60", 3600f);
		base.Invoke("ImHereMIN120", 7200f);
		PhotonNetwork.sendRate = 10;
		PhotonNetwork.sendRateOnSerialize = 4;
	}

	public void BeginLoading()
	{
		this._isLoading = true;
	}

	public void EndLoading()
	{
		this._isLoading = false;
	}

	private void ImHereSEC30()
	{
		Kube.SS.SendStat("play30sec");
	}

	private void ImHereMIN1()
	{
		Kube.SS.SendStat("play1min");
	}

	private void ImHereMIN2()
	{
		Kube.SS.SendStat("play2min");
	}

	private void ImHereMIN5()
	{
		Kube.SS.SendStat("play5min");
	}

	private void ImHereMIN10()
	{
		Kube.SS.SendStat("play10min");
	}

	private void ImHereMIN20()
	{
		Kube.SS.SendStat("play20min");
	}

	private void ImHereMIN60()
	{
		Kube.SS.SendStat("play60min");
	}

	private void ImHereMIN120()
	{
		Kube.SS.SendStat("play120min");
	}

	public bool fullScreen
	{
		get
		{
			return this._fullscreen;
		}
		set
		{
			UnityEngine.Debug.Log("Change fullscreen " + value);
			if (value)
			{
				Screen.fullScreen = value;
				Screen.SetResolution(this.screenResolution.width, this.screenResolution.height, true);
			}
			else
			{
				Screen.fullScreen = false;
			}
			this._fullscreen = value;
		}
	}

	private void Update()
	{
		this.timeleft -= Time.deltaTime;
		this.accum += Time.timeScale / Time.deltaTime;
		this.frames++;
		if ((double)this.timeleft <= 0.0)
		{
			this.fps = this.accum / (float)this.frames;
			this.timeleft = this.updateFPSInterval;
			this.accum = 0f;
			this.frames = 0;
		}
		KubeInput.ProcessInput();
		if (UnityEngine.Input.GetKeyUp(KeyCode.F12))
		{
			this.fullScreen = !this.fullScreen;
		}
		if (Kube.ASS2 && Kube.ASS3 && Kube.OH.waterAnimMat != null && Time.time - this.lastWaterTexChange > this.waterAnimDeltaTime)
		{
			Kube.OH.waterAnimMat.mainTexture = Kube.ASS3.waterAnimTex[this.numWaterTex];
			Kube.OH.AAselectMat.mainTexture = Kube.ASS2.AAselectTex[this.numWaterTex];
			this.numWaterTex++;
			if (this.numWaterTex >= Kube.ASS3.waterAnimTex.Length)
			{
				this.numWaterTex = 0;
			}
			this.lastWaterTexChange = Time.time;
		}
		if (UnityEngine.Input.GetKeyUp(KeyCode.F11))
		{
			Kube.SN.TakeScreenshot();
		}
	}

	public void OnLevelWasLoaded(int level)
	{
		this.closeMenuAll();
		if (this.isError)
		{
			this.openMenu(new DrawCall(this.networkErrorGUI), false, false);
		}
		if (this.usedCheat)
		{
			Cub2UI.MessageBox(Localize.hello_chiter, null);
		}
		this.lastTempMap = this.tempMap;
	}

	public BaseUI openMenu(DrawCall menu, bool canClose = true, bool isPopup = false)
	{
		return this.openMenu(new DelegateUI(menu)
		{
			canClose = canClose,
			popup = isPopup
		});
	}

	public BaseUI openMenu(BaseUI ui)
	{
		ui.show();
		this._menuStack.Add(ui);
		this._menuDraw = ui;
		this._menuBottom = 0;
		for (int i = this._menuStack.Count - 1; i >= 0; i--)
		{
			if (!this._menuStack[i].popup)
			{
				this._menuBottom = i;
				break;
			}
		}
		KubeScreen.lockCursor = false;
		return ui;
	}

	public void closeMenu(DrawCall menu)
	{
		BaseUI baseUI = null;
		foreach (BaseUI baseUI2 in this._menuStack)
		{
			if (baseUI2 is DelegateUI && menu == ((DelegateUI)baseUI2).drawCall)
			{
				baseUI = baseUI2;
			}
		}
		if (baseUI != null)
		{
			this.closeMenu(baseUI);
		}
	}

	public void closeMenu(BaseUI menu = null)
	{
		if (menu == null)
		{
			menu = this._menuDraw;
		}
		if (menu != null)
		{
			menu.hide();
		}
		this._menuStack.Remove(menu);
		if (this._menuStack.Count > 0)
		{
			this._menuDraw = this._menuStack[this._menuStack.Count - 1];
		}
		else
		{
			this._menuDraw = null;
		}
	}

	public void closeMenuAll()
	{
		foreach (BaseUI baseUI in this._menuStack)
		{
			baseUI.hide();
		}
		this._menuStack.Clear();
		this._menuDraw = null;
	}

	public bool topMenu(BaseUI ui)
	{
		return this._menuDraw == ui;
	}

	public bool isMenu
	{
		get
		{
			return this._isMenu || this._isNewMenu;
		}
		set
		{
			this._isNewMenu = value;
		}
	}

	public bool hasMenu(DrawCall drawCall)
	{
		foreach (BaseUI baseUI in this._menuStack)
		{
			if (baseUI is DelegateUI && ((DelegateUI)baseUI).drawCall == drawCall)
			{
				return true;
			}
		}
		return false;
	}

	public bool hasMenu(string name)
	{
		return false;
	}

	public bool hasMenu(BaseUI ui)
	{
		return this._menuStack.Contains(ui);
	}

	private void OnGUI()
	{
		KUI.DownScale();
		float width = (float)KUI.width;
		float height = (float)KUI.height;
		GUI.depth = 0;
		if (this._isLoading)
		{
			GUI.DrawTexture(new Rect(0f, 0f, (float)KUI.width, (float)KUI.height), this.loadTex);
			Kube.RM.DrawLoading();
			return;
		}
		GUI.depth = -1;
		if (this._menuDraw != null)
		{
			GUI.DrawTexture(new Rect(0f, 0f, width, height), Kube.ASS1.menuFrame);
			GUI.enabled = false;
			if (this._menuDraw.popup)
			{
				for (int i = this._menuBottom; i < this._menuStack.Count - 1; i++)
				{
					GUI.color = new Color(1f, 1f, 1f, 2f);
					this._menuStack[i].draw();
				}
			}
			GUI.enabled = true;
			GUI.tooltip = string.Empty;
			if (this._menuStack.Count > 1)
			{
				GUI.DrawTexture(new Rect(0f, 0f, (float)KUI.width, (float)KUI.height), KUI.BlackTx);
			}
			this._menuDraw.draw();
			if (GUI.tooltip != null && GUI.tooltip.Length != 0)
			{
				Vector2 vector = GUIUtility.ScreenToGUIPoint(Event.current.mousePosition);
				Vector2 vector2 = GUI.skin.GetStyle("Label").CalcSize(new GUIContent(GUI.tooltip));
				GUI.Box(new Rect(Mathf.Min(vector.x, (float)Screen.width - vector2.x - 50f), vector.y - vector2.y, vector2.x, vector2.y), string.Empty);
				GUI.Label(new Rect(Mathf.Min(vector.x, (float)Screen.width - vector2.x - 50f), vector.y - vector2.y, vector2.x, vector2.y), GUI.tooltip);
			}
		}
		this._isMenu = (this._menuStack.Count > 0);
		if (this.showFPS)
		{
			string text = string.Format("{0:F2} FPS", this.fps);
			GUI.Label(new Rect((float)(Screen.width - 100), 0f, 100f, 20f), text);
		}
	}

	public string GetServerCode(int num, int needRazm = 0)
	{
		string text = string.Empty;
		int num2 = 1;
		for (int i = 1; i < 10; i++)
		{
			if (num < (int)Mathf.Pow(64f, (float)i))
			{
				num2 = i;
				break;
			}
		}
		if (needRazm != 0 && num2 > needRazm)
		{
			num2 = needRazm;
		}
		for (int j = 1; j <= num2; j++)
		{
			int num3 = num % (int)Mathf.Pow(64f, (float)j) / (int)Mathf.Pow(64f, (float)(j - 1));
			text = this.serverCodes[num3] + text;
			num -= num3 * (int)Mathf.Pow(64f, (float)(j - 1));
		}
		if (needRazm != 0)
		{
			if (needRazm < text.Length)
			{
				throw new Exception("GetServerCode bad needRazm ");
			}
			while (text.Length < needRazm)
			{
				text = this.serverCodes[0] + text;
			}
		}
		return text;
	}

	public int DecodeServerCode(string code)
	{
		int length = code.Length;
		int num = 0;
		for (int i = length - 1; i >= 0; i--)
		{
			num += this.serverCodes.IndexOf(code[length - i - 1]) * (int)Mathf.Pow(64f, (float)i);
		}
		return num;
	}

	public string Lang(string str1, string str2)
	{
		if (this.lang == 0)
		{
			return str1;
		}
		return str2;
	}

	private void GetPlayerMoneyDone(string[] strs)
	{
		Kube.GPS.playerMoney1 = Convert.ToInt32(strs[2]);
		Kube.GPS.playerMoney2 = Convert.ToInt32(strs[3]);
	}

	public int GetLevel(uint exp)
	{
		int num = 0;
		if (this.GetExp(Kube.GPS.playerLevel) < exp)
		{
			num = Kube.GPS.playerLevel;
		}
		if (num >= Localize.RankName.Length)
		{
			num = Localize.RankName.Length - 1;
		}
		for (int i = 0; i < Localize.RankName.Length; i++)
		{
			if (this.GetExp(num) <= exp && this.GetExp(num + 1) > exp)
			{
				break;
			}
			num++;
		}
		return num;
	}

	public uint GetExp(int level)
	{
		if (level < 0)
		{
			level = 0;
		}
		double num = (double)(200 * level);
		if (level >= 40)
		{
			double num2 = (double)(level - 40);
			num += num2 * num2 * 4000.0;
		}
		if (level >= 20)
		{
			double num2 = (double)(level - 20);
			num += num2 * num2 * 2000.0;
		}
		if (level >= 2)
		{
			double num2 = (double)(level - 2);
			num += num2 * num2 * 300.0;
		}
		return (uint)Math.Round(num);
	}

	public uint GetExpToLevelUp(int currentLevel)
	{
		return this.GetExp(currentLevel + 1) - this.GetExp(currentLevel);
	}

	public uint GetExpFromLevelUp(uint exp, int currentLevel = -1)
	{
		if (currentLevel == -1)
		{
			currentLevel = this.GetLevel(exp);
		}
		return exp - this.GetExp(currentLevel);
	}

	public float GetExpToLevelUpAlpha(uint exp, int currentLevel = -1)
	{
		if (currentLevel == -1)
		{
			currentLevel = this.GetLevel(exp);
		}
		return this.GetExpFromLevelUp(exp, currentLevel) / this.GetExpToLevelUp(currentLevel);
	}

	private void SendLevelDoneDone(string[] strs)
	{
		if (Convert.ToInt32(strs[0]) == 0)
		{
			long num = Convert.ToInt64(strs[2]);
			if (num >= (long)((ulong)-1))
			{
				num = (long)((ulong)-1);
			}
			Kube.GPS.playerExp = (uint)num;
			Kube.GPS.playerFrags = Convert.ToInt32(strs[3]);
			Kube.GPS.playerMoney1 = Convert.ToInt32(strs[4]);
			Kube.GPS.playerLevel = Convert.ToInt32(strs[5]);
			Kube.GPS.playerMoney2 = Convert.ToInt32(strs[6]);
		}
	}

	private void BuyNewMapDone(string str)
	{
		char[] separator = new char[]
		{
			'^'
		};
		string[] array = str.Split(separator);
		if (Convert.ToInt32(array[0]) == 0)
		{
			Kube.GPS.playerNumMaps = Convert.ToInt32(array[2]);
			Kube.GPS.playerMoney2 = Convert.ToInt32(array[3]);
		}
		else
		{
			Kube.GPS.printLog(str);
		}
	}

	private void GotFriends(string str)
	{
		if (str == string.Empty)
		{
			return;
		}
		char[] separator = new char[]
		{
			'^'
		};
		string[] array = str.Split(separator);
		this.friends = new ObjectsHolderScript.FriendInfo[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			this.friends[i].uid = array[i];
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["ids"] = str;
		Kube.SS.Request(900, dictionary, new ServerCallback(this.onFriendsIds));
	}

	public string FriendUID(int id)
	{
		for (int i = 0; i < this.friends.Length; i++)
		{
			if (this.friends[i].Id == id)
			{
				return this.friends[i].uid;
			}
		}
		return null;
	}

	private void onFriendsIds(string str)
	{
		if (str == string.Empty)
		{
			if (Kube.SN.sn == socialNetType.vk)
			{
				for (int i = 0; i < this.friends.Length; i++)
				{
					this.friends[i].Id = Convert.ToInt32(this.friends[i].uid);
				}
			}
			return;
		}
		char[] separator = new char[]
		{
			'^'
		};
		string[] array = str.Split(separator);
		for (int j = 0; j < array.Length; j += 2)
		{
			for (int k = 0; k < this.friends.Length; k++)
			{
				if (!(this.friends[k].uid != array[j]))
				{
					this.friends[k].Id = Convert.ToInt32(array[j + 1]);
				}
			}
		}
		for (int l = 0; l < this.friends.Length; l++)
		{
		}
	}

	private void GotFriends(Texture[] texs)
	{
		for (int i = 0; i < texs.Length; i++)
		{
			this.friends[i].Tex = texs[i];
		}
	}

	private void GotFriends(string[] names)
	{
		for (int i = 0; i < names.Length; i++)
		{
			this.friends[i].Name = names[i];
		}
	}

	private void OnJoinedRoom()
	{
		Kube.GPS.printLog("OnJoinedRoom");
		PhotonNetwork.isMessageQueueRunning = false;
		this.LoadGameLevel();
	}

	private void OnCreatedRoom()
	{
		Kube.GPS.printLog("OnCreatedRoom");
	}

	private void LoadGameLevel()
	{
		if (PhotonNetwork.insideLobby)
		{
			return;
		}
		this.closeMenuAll();
		if (!PhotonNetwork.offlineMode)
		{
			this.tempMap.Id = (long)PhotonNetwork.room.customProperties["m"];
		}
		Kube.GPS.printLog("Start game at MAP:" + this.tempMap.Id);
		base.StopCoroutine("_LoadGameLevel");
		this.BeginLoading();
		base.StartCoroutine(this._LoadGameLevel());
	}

	private IEnumerator _LoadGameLevel()
	{
		Kube.RM.ClearCache();
		Kube.RM.DownloadGameData();
		Kube.RM.require("Assets2", null);
		Kube.RM.require("Assets5", null);
		Kube.RM.require("Assets6", null);
		Kube.RM.require("Assets7", null);
		for (;;)
		{
			if (Kube.ASS3 == null || Kube.ASS4 == null)
			{
				yield return new WaitForSeconds(0.2f);
			}
			else if (Kube.ASS2 == null || Kube.ASS5 == null || Kube.ASS6 == null)
			{
				yield return new WaitForSeconds(0.2f);
			}
			else
			{
				if (Kube.RM.downloadReady)
				{
					break;
				}
				yield return new WaitForSeconds(0.2f);
			}
		}
		UnityEngine.Debug.Log("Start game level");
		UnityEngine.SceneManagement.SceneManager.LoadScene("TestNew");
		Application.LoadLevelAdditive("InGameMenu");
		yield break;
	}

	private void OnPhotonJoinRoomFailed()
	{
		Kube.GPS.printMessage(Localize.connect_failed, Color.red);
	}

	private void OnPhotonCreateRoomFailed()
	{
		Kube.GPS.printMessage(Localize.create_room_failed, Color.red);
	}

	private void OnConnectedToPhoton()
	{
		Kube.GPS.printLog("Connected To Photon");
		PhotonNetwork.networkingPeer.WarningSize = 500;
		PhotonNetwork.networkingPeer.LimitOfUnreliableCommands = 500;
	}

	private void OnDisconnectedFromPhoton()
	{
		Kube.GPS.printLog("Disconnected From Photon");
	}

	private void OnConnectionFail(DisconnectCause cs)
	{
		Kube.GPS.printLog("Disconnected From Photon" + cs);
	}

	public long randomMap(GameType t)
	{
		ObjectsHolderScript.BuiltInMap[] array = this.findMaps(t);
		int num = UnityEngine.Random.Range(0, array.Length - 1);
		return (long)array[num].Id;
	}

	[NonSerialized]
	public Dictionary<int, Texture> gameItemsTex = new Dictionary<int, Texture>(200);

	[NonSerialized]
	public Dictionary<int, Texture> inventarSkinsTex = new Dictionary<int, Texture>(200);

	[NonSerialized]
	public Dictionary<int, Texture> inventarClothesTex = new Dictionary<int, Texture>(200);

	[NonSerialized]
	public Dictionary<int, GameObject> clothesGO = new Dictionary<int, GameObject>(200);

	[NonSerialized]
	public Dictionary<int, Material> skinMats = new Dictionary<int, Material>(200);

	[NonSerialized]
	public List<GameObject> photonObjects = new List<GameObject>(200);

	[NonSerialized]
	public Dictionary<int, GameObject> gameItemsGO = new Dictionary<int, GameObject>(250);

	[NonSerialized]
	public Dictionary<int, GameObject> charWeaponsGO = new Dictionary<int, GameObject>(200);

	[NonSerialized]
	public Dictionary<int, GameObject> weaponsBulletPrefab = new Dictionary<int, GameObject>(200);

	[NonSerialized]
	public Dictionary<int, Material> weaponsSkin = new Dictionary<int, Material>(20);

	public int build;

	public Texture2D loadTex;

	public int lang;

	public float gravity;

	public GameObject testObj;

	public string[] monsterPrefabName;

	public int[] monstrePoints;

	[NonSerialized]
	public TaskDesc task;

	[NonSerialized]
	public TaskDesc[] tasks;

	public string[] transportPrefabName;

	public SoundMaterialType[] cubesSound;

	public GameObject miniCube;

	public float[] cubesStrength;

	public int[] AAnumInShop;

	public GameObject[] AAsounds;

	public string[] AAsoundsNames;

	public int wireItemNum;

	public float waterAnimDeltaTime;

	private int numWaterTex;

	private float lastWaterTexChange;

	public GameObject boundsPlane;

	public GameObject testLightCube;

	public string api_url;

	public int api_id;

	public string access_token;

	public string phpSecret;

	public float pointsToMoney = 1f;

	public GameObject pointsText;

	public ObjectsHolderScript.GameItemInfo[] gameItemInfo;

	[HideInInspector]
	public ObjectsHolderScript.TempMap tempMap;

	[HideInInspector]
	public ObjectsHolderScript.TempMap lastTempMap;

	public GameObject tutorialGO;

	public string[] gameTypeStrRoom;

	public int[] gameMaxTime;

	public Color[] teamColor;

	public ObjectsHolderScript.FriendInfo[] friends;

	public ObjectsHolderScript.BuiltInMap[] builtInMaps;

	public ObjectsHolderScript.BlockType[] blockTypes;

	public ObjectsHolderScript.EpisodeDesc[] episodeDesc;

	public Texture2D mainCubesTex;

	public Vector3[] GameItemRotationVector;

	public bool usedCheat;

	public Material dieEffectMaterial;

	private bool isError;

	private bool initialized;

	private bool _isLoading;

	private float fps;

	public float updateFPSInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private bool tempLockCursor;

	private bool _fullscreen;

	[NonSerialized]
	public Material waterAnimMat;

	[NonSerialized]
	public Material AAselectMat;

	public bool emptyScreen;

	[NonSerialized]
	public Resolution screenResolution;

	private BaseUI _menuDraw;

	private List<BaseUI> _menuStack = new List<BaseUI>();

	private int _menuBottom;

	protected bool _isMenu;

	protected bool _isNewMenu;

	public bool showFPS;

	private string serverCodes = "0123456789qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM-_";

	public bool autoaim = true;

	[Serializable]
	public class GameItemInfo
	{
		public string iconName;

		public string goName;

		[NonSerialized]
		public Texture icon;

		[NonSerialized]
		public GameObject go;
	}

	public enum MissionType
	{
		reachTheExit = 1,
		holdNSeconds,
		killNMonsters,
		findNitems,
		findNitemsInMSeconds,
		killNMonstersInMSeconds,
		reachTheExitInTime
	}

	public struct TempMap
	{
		public long Id;

		public GameType GameType;

		public int CanBreak;

		public bool CreatedGame;

		public int DayLight;

		public int missionId;

		public ObjectsHolderScript.MissionType missionType;

		public object[] missionConfig;
	}

	public struct FriendInfo
	{
		public int Id;

		public Texture Tex;

		public string Name;

		public string uid;

		public string nickName;
	}

	[Serializable]
	public class BuiltInMap
	{
		public int Id;

		public string name;

		public int playersMax = 12;

		public bool[] gameTypes;

		public int size;
	}

	[Serializable]
	public enum CubeGroup
	{
		cubesNone,
		cubesNature,
		cubesBuilder,
		cubesDecor,
		cubesGlass,
		cubesWater,
		cubesDifferent
	}

	[Serializable]
	public struct BlockType
	{
		public int type;

		public int itemId;

		public int atlas;

		public ObjectsHolderScript.CubeGroup group;
	}

	[Serializable]
	public struct EpisodeDesc
	{
		public int minlevel;

		public bool vip;
	}
}
