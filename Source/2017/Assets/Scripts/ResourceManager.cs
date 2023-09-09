using System;
using kube;
using kube.data;
using UnityEngine;

public class ResourceManager : ResourceManagerBase, IBaseResource
{
	private void Awake()
	{
		Kube.RM = this;
	}

	[ContextMenu("sort")]
	private void SortAssets()
	{
		Array.Sort<DownloadInfo>(this.downloadInfo, (DownloadInfo keyfirst, DownloadInfo keylast) => DataUtils.IntParseFast(keyfirst.name).CompareTo(DataUtils.IntParseFast(keylast.name)));
	}

	[ContextMenu("list revisions")]
	private void ListAssets()
	{
		DownloadInfo[] array = (DownloadInfo[])this.downloadInfo.Clone();
		Array.Sort<DownloadInfo>(array, (DownloadInfo keyfirst, DownloadInfo keylast) => keyfirst.assetRevision.CompareTo(keylast.assetRevision));
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Debug.Log(array[i].assetRevision + array[i].name);
		}
	}

	[ContextMenu("reset revisions")]
	private void ResetAssets()
	{
		DownloadInfo[] array = (DownloadInfo[])this.downloadInfo.Clone();
		DateTime now = DateTime.Now;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].assetRevision = int.Parse(now.Year.ToString().Substring(2, 2) + now.Month.ToString("00") + now.Day.ToString("00"));
		}
	}

	virtual void Init(string assetPath, string bundlesPath)
	{
		base.Init(assetPath, bundlesPath);
	}

	virtual void downloadMap(long id)
	{
		base.downloadMap(id);
	}

	virtual WWW WWWLoad(string str)
	{
		return base.WWWLoad(str);
	}

	virtual void requireResource(string path, global::AsyncCallback onLoaded)
	{
		base.requireResource(path, onLoaded);
	}

	virtual void require(string name, global::AsyncCallback cb)
	{
		base.require(name, cb);
	}

	virtual void requireByTag(string tag)
	{
		base.requireByTag(tag);
	}

	virtual GameObject FindItemAsset(int index)
	{
		return base.FindItemAsset(index);
	}

	virtual void ClearCache()
	{
		base.ClearCache();
	}

	virtual UnityEngine.Object loadResource(string path, Type type)
	{
		return base.loadResource(path, type);
	}

	virtual void DownloadGameData()
	{
		base.DownloadGameData();
	}

	virtual void DrawLoading()
	{
		base.DrawLoading();
	}

	virtual bool get_downloadReady()
	{
		return base.downloadReady;
	}
}
