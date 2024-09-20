using System;
using System.Collections;
using UnityEngine;

public interface IBaseResource
{
	void Init(string assetPath, string bundlesPath);

	void downloadMap(long id);

	WWW WWWLoad(string str);

	void requireResource(string path, global::AsyncCallback onLoaded);

	void require(string name, global::AsyncCallback cb = null);

	void requireByTag(string tag);

	GameObject FindItemAsset(int index);

	GameObject FindAsset(string prefix, int index);

	void ClearCache();

	UnityEngine.Object loadResource(string path, Type type);

	void DownloadGameData();

	void DrawLoading();

	IEnumerator _downloadMap(long id);

	bool downloadReady { get; }
}
