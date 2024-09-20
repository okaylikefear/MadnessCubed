using System;
using System.Collections;
using System.Collections.Generic;
using kube;
using UnityEngine;

public class ResourceManagerBase : MonoBehaviour
{
	public bool savingMap
	{
		get
		{
			return this._savingMap;
		}
	}

	public bool loadingMap
	{
		get
		{
			return this._loadingMap;
		}
	}

	public void Init(string assetPath)
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
		this.assetPath = assetPath;
	}

	private void OnApplicationQuit()
	{
		this.ReleaseAssets();
	}

	public WWW WWWLoad(string url)
	{
		return new WWW(this.assetPath + url);
	}

	public void requireResource(string path, global::AsyncCallback onLoaded)
	{
		int num = path.IndexOf("/");
		string name = path;
		if (num != -1)
		{
			name = path.Substring(0, num);
			path = path.Substring(num + 1);
		}
		this.require(name, onLoaded);
	}

	public void require(string name, global::AsyncCallback cb = null)
	{
		for (int i = 0; i < this.downloadInfo.Length; i++)
		{
			if (this.downloadInfo[i].name == name || this.downloadInfo[i].name.StartsWith(name))
			{
				if (this._pending.IndexOf(this.downloadInfo[i]) == -1)
				{
					this._pending.Add(this.downloadInfo[i]);
					if (cb != null)
					{
						this.downloadInfo[i].cb.Add(cb);
					}
					this.DownloadAsset(this.downloadInfo[i], false);
				}
				else if (cb != null)
				{
					cb();
				}
				return;
			}
		}
	}

	public void requireByTag(string tag)
	{
		for (int i = 0; i < this.downloadInfo.Length; i++)
		{
			if (this.downloadInfo[i].tag == tag && this._pending.IndexOf(this.downloadInfo[i]) == -1)
			{
				this._pending.Add(this.downloadInfo[i]);
				this.DownloadAsset(this.downloadInfo[i], false);
			}
		}
	}

	public bool downloadReady
	{
		get
		{
			return this._downloadReady;
		}
	}

	public GameObject FindItemAsset(int index)
	{
		return this.FindAsset("ItemGO", index);
	}

	public virtual GameObject FindAsset(string prefix, int index)
	{
		string name = prefix + index;
		GameObject gameObject = null;
		DownloadInfo[] array = this.downloadInfo;
		for (int i = 0; i < array.Length; i++)
		{
			string name2 = array[i].name;
			if (array[i].ready && array[i].isPackage)
			{
				if (this.debugDownloadWWW || !Application.isEditor)
				{
					gameObject = (GameObject)this.downloadInfo[i].ab.LoadAsset(name, typeof(GameObject));
				}
				if (gameObject != null)
				{
					break;
				}
			}
		}
		return gameObject;
	}

	public void ClearCache()
	{
		this._cache.Clear();
	}

	public UnityEngine.Object loadResource(string path, Type type)
	{
		int num = path.IndexOf("/");
		string value = path;
		if (num != -1)
		{
			value = path.Substring(0, num);
			path = path.Substring(num + 1);
		}
		if (this._cache.ContainsKey(path))
		{
			return this._cache[path];
		}
		UnityEngine.Object @object = null;
		for (int i = 0; i < this.downloadInfo.Length; i++)
		{
			if (this.downloadInfo[i].name.Contains(value))
			{
				if (this.downloadInfo[i].ready && this.downloadInfo[i].isPackage)
				{
					if (!this.debugDownloadWWW && Application.isEditor)
					{
						if (type == typeof(Material))
						{
						}
					}
					else
					{
						@object = this.downloadInfo[i].ab.LoadAsset(path, type);
					}
					break;
				}
			}
		}
		if (@object)
		{
			this._cache[path] = @object;
		}
		return @object;
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		Kube.RM = null;
		this.ReleaseAssets();
	}

	private IEnumerator DownloadOH()
	{
		for (int i = 0; i < this.downloadInfo.Length; i++)
		{
			if (!(this.downloadInfo[i].ab != null))
			{
				if (this.downloadInfo[i].www == null)
				{
					if (!this.downloadInfo[i].isAsyncDownload)
					{
						yield return base.StartCoroutine(this._DownloadAsset(this.downloadInfo[i], true));
					}
				}
			}
		}
		yield return new WaitForSeconds(2f);
		this.OHReady();
		this.isDownload = false;
		this._downloadReady = true;
		yield break;
	}

	private T[] LoadAll<T>(string path) where T : UnityEngine.Object
	{
		return null;
	}

	protected virtual void DownloadAsset(DownloadInfo downloadInfo, bool showProgress = false)
	{
		base.StartCoroutine(this._DownloadAsset(downloadInfo, showProgress));
	}

	protected IEnumerator _DownloadAsset(DownloadInfo downloadInfo, bool showProgress = false)
	{
		if (GameObject.Find(downloadInfo.name))
		{
			UnityEngine.Debug.Log("skip " + downloadInfo.name);
			yield break;
		}
		UnityEngine.Debug.Log("load " + downloadInfo.name + " from " + downloadInfo.path);
		if (!this.debugDownloadWWW && Application.isEditor)
		{
			if (downloadInfo.isPackage)
			{
				UnityEngine.Debug.Log("package: " + downloadInfo.name);
				UnityEngine.Object[] late = this.LoadAll<LateBindResource>("Assets/bundles/" + downloadInfo.name);
				if (late != null)
				{
					this.initLateBind(late);
				}
				downloadInfo.ready = true;
				yield break;
			}
			GameObject pf = null;
			yield return 0;
			if (pf != null)
			{
				GameObject obj = UnityEngine.Object.Instantiate<GameObject>(pf);
				UnityEngine.Object.DontDestroyOnLoad(obj);
			}
			yield return 0;
		}
		else
		{
			int rev = downloadInfo.assetRevision;
			string url = string.Concat(new string[]
			{
				this.assetPath,
				"v",
				rev.ToString(),
				"/",
				downloadInfo.path
			});
			WWW www = WWW.LoadFromCacheOrDownload(url, rev);
			downloadInfo.www = www;
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				UnityEngine.Debug.LogError("error " + www.error + " " + www.url);
				yield break;
			}
			yield return 0;
			string[] names = www.assetBundle.AllAssetNames();
			if (!downloadInfo.isPackage)
			{
				UnityEngine.Object prefab = www.assetBundle.LoadAsset(downloadInfo.name);
				GameObject obj2 = (GameObject)UnityEngine.Object.Instantiate(prefab);
				UnityEngine.Object.DontDestroyOnLoad(obj2);
			}
			UnityEngine.Object[] late2 = www.assetBundle.LoadAllAssets(typeof(LateBindResource));
			if (late2 != null)
			{
				this.initLateBind(late2);
			}
			yield return 0;
			downloadInfo.www = null;
			downloadInfo.ab = www.assetBundle;
		}
		UnityEngine.Debug.Log("end load " + downloadInfo.name + " from " + downloadInfo.path);
		downloadInfo.ready = true;
		for (int i = 0; i < downloadInfo.cb.Count; i++)
		{
			downloadInfo.cb[i]();
		}
		downloadInfo.cb.Clear();
		yield return new WaitForSeconds(0.2f);
		Kube.SendMonoMessage("onAssetsLoaded", new object[]
		{
			0
		});
		yield break;
	}

	protected void initLateBind(UnityEngine.Object[] late)
	{
		for (int i = 0; i < late.Length; i++)
		{
			LateBindResource lateBindResource = late[i] as LateBindResource;
			if (lateBindResource)
			{
				if (lateBindResource.t == LateBindResource.ResourceType.Item)
				{
					if (lateBindResource.icon)
					{
						Kube.OH.gameItemsTex[lateBindResource.id] = lateBindResource.icon;
					}
					if (lateBindResource.go)
					{
						PhotonView component = lateBindResource.go.GetComponent<PhotonView>();
						if (component)
						{
							Kube.OH.photonObjects.Add(lateBindResource.go);
						}
					}
				}
				else if (lateBindResource.t == LateBindResource.ResourceType.Clothes)
				{
					if (lateBindResource.icon)
					{
						Kube.OH.inventarClothesTex[lateBindResource.id] = lateBindResource.icon;
					}
					Kube.OH.clothesGO[lateBindResource.id] = lateBindResource.go;
				}
				else if (lateBindResource.t == LateBindResource.ResourceType.Skin)
				{
					if (lateBindResource.icon)
					{
						Kube.OH.inventarSkinsTex[lateBindResource.id] = lateBindResource.icon;
					}
					Kube.OH.skinMats[lateBindResource.id] = lateBindResource.go.GetComponent<DresSkinItem>().mat;
				}
				else if (lateBindResource.t == LateBindResource.ResourceType.Weapon)
				{
					Kube.OH.charWeaponsGO[lateBindResource.id] = lateBindResource.go;
				}
				else if (lateBindResource.t == LateBindResource.ResourceType.Bullet)
				{
					Kube.OH.weaponsBulletPrefab[lateBindResource.id] = lateBindResource.go;
					PhotonView componentInChildren = lateBindResource.go.GetComponentInChildren<PhotonView>();
					if (componentInChildren)
					{
						Kube.OH.photonObjects.Add(lateBindResource.go);
					}
				}
				else if (lateBindResource.t == LateBindResource.ResourceType.WeaponSkin)
				{
					Kube.OH.weaponsSkin[lateBindResource.id] = lateBindResource.mat;
				}
			}
		}
	}

	private void OHReady()
	{
		this.isDownloadReady = true;
	}

	public void downloadMap(long id)
	{
		this._loadingMap = true;
		base.StartCoroutine(this._downloadMap(-id));
	}

	public virtual IEnumerator _downloadMap(long id)
	{
		int mapid = (int)id;
		bool loadFromAsset = false;
		if (Kube.ASS3 && mapid < 100)
		{
			yield return new WaitForSeconds(0.2f);
			if (Kube.WHS != null)
			{
				if (mapid < Kube.ASS3.buildinMaps.Length && Kube.ASS3.buildinMaps[mapid] != null)
				{
					Kube.BCS.OnMapLoaded(Kube.ASS3.buildinMaps[mapid].bytes);
					loadFromAsset = true;
				}
				if (loadFromAsset)
				{
					this._loadingMap = false;
					yield break;
				}
			}
		}
		WWW newWWW = new WWW(this.assetPath + "m" + id.ToString() + ".bytes");
		yield return newWWW;
		if (!string.IsNullOrEmpty(newWWW.error))
		{
			Kube.OH.SendMessage("ServerError", SendMessageOptions.DontRequireReceiver);
			yield break;
		}
		if (Kube.WHS != null)
		{
			Kube.BCS.OnMapLoaded(newWWW.bytes);
		}
		this._loadingMap = false;
		yield break;
	}

	public void DownloadGameData()
	{
		if (this.isDownload)
		{
			return;
		}
		if (this.isDownloadReady)
		{
			return;
		}
		this.isDownload = true;
		base.StartCoroutine(this.DownloadOH());
	}

	public void DrawLoading()
	{
		if (!this.isDownload)
		{
			return;
		}
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		int num3 = this.downloadInfo.Length;
		float num4 = 0f;
		for (int i = 0; i < this.downloadInfo.Length; i++)
		{
			if (this.downloadInfo[i].ready)
			{
				num4 += 1f;
			}
			else if (this.downloadInfo[i].www != null)
			{
				num4 += this.downloadInfo[i].www.progress;
			}
		}
		num4 /= (float)num3;
		float num5 = Mathf.Floor(num4 * 100f);
		GUI.Label(new Rect(0.5f * num - 150f, num2 - 100f, 300f, 60f), string.Concat(new object[]
		{
			Localize.ss_loading,
			" ",
			num5,
			"%"
		}));
		GUI.DrawTexture(new Rect(0.5f * (num - 318f), num2 - 100f, 318f, 25f), this.pb_bgTex);
		GUI.DrawTextureWithTexCoords(new Rect(0.5f * (num - 318f), num2 - 100f, num4 * 318f, 25f), this.pb_fillTex, new Rect(0f, 0f, num4, 1f));
		GUI.DrawTexture(new Rect(0.5f * (num - 318f), num2 - 100f, 318f, 25f), this.pb_borderTex);
	}

	public void ReleaseAssets()
	{
		Kube.ASS1 = null;
		Kube.ASS2 = null;
		Kube.ASS3 = null;
		Kube.ASS4 = null;
		Kube.ASS5 = null;
		for (int i = 0; i < this.downloadInfo.Length; i++)
		{
			if (this.downloadInfo[i].ab != null)
			{
				this.downloadInfo[i].ab.Unload(false);
			}
		}
		for (int j = 0; j < this.downloadInfo.Length; j++)
		{
			if (this.downloadInfo[j].ab != null)
			{
				this.downloadInfo[j].ab.Unload(false);
			}
		}
	}

	public Texture pb_bgTex;

	public Texture pb_fillTex;

	public Texture pb_borderTex;

	public bool debugDownloadWWW;

	[NonSerialized]
	public string assetPath;

	private bool _loadingMap;

	private bool _savingMap;

	private float _serverTime;

	private bool initialized;

	public DownloadInfo[] downloadInfo;

	private WWW[] _www;

	private List<DownloadInfo> _pending = new List<DownloadInfo>();

	protected bool _downloadReady;

	protected Dictionary<string, UnityEngine.Object> _cache = new Dictionary<string, UnityEngine.Object>();

	private bool isDownloadReady;

	protected bool isDownload;
}
