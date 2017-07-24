// Unity Asset Bundle Loader

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HutongGames.PlayMaker;
using System.Collections;

public class UnityAssetLoader : MonoBehaviour {

	[UnityEngine.Tooltip("This game object will be active while loading")]
	public GameObject loadingIndicator;

	[UnityEngine.Tooltip("Assign this if you have a progress indicator for loading")]
	public Slider progressSlider = null;

	[UnityEngine.Tooltip("Use this url to autoload bundle on start")]
	public string BundleURL;
	public string AssetName;
	public int version;

	public bool loadOnStart = false;
	public bool instantiateOnStart = false;

	[HideInInspector] public AssetBundle bundle;

	WWW www;
	bool loading = false;

	void Start() {
		if (loadOnStart) StartCoroutine (DownloadAndCache(BundleURL, ()=>{
			if (instantiateOnStart) {
				if (AssetName == "")
					Instantiate(bundle.mainAsset);
				else
					Instantiate(bundle.LoadAsset(AssetName));
				// Unload the AssetBundles compressed contents to conserve memory
				bundle.Unload(false);
			}
		}));
	}

	public void LoadBundle(string url, Action onComplete)
	{
		StartCoroutine (DownloadAndCache (url, onComplete));
	}

	public void LoadBundlePM(string url, PlayMakerFSM fsm, string eventName)
	{
		StartCoroutine(DownloadAndCache(url, () => {
			if (fsm!=null) fsm.SendEvent (eventName);
		}));
	}

	IEnumerator DownloadAndCache (string url, Action OnComplete) {

		if (!url.StartsWith ("http") && !url.StartsWith ("file") && !url.StartsWith ("jar:")) url = "file:///" + url; // add 'file' prefix for local file system
		Debug.Log("Loading unity asset bundle: " + url);

		ShowLoading (true);
		// Wait for the Caching system to be ready
		while (!Caching.ready)
			yield return null;

		// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
		using(www = WWW.LoadFromCacheOrDownload (url, version)){
			www.threadPriority = ThreadPriority.Low;
			yield return www;
			if (www.error != null)
				throw new Exception("WWW download had an error:" + www.error);
			bundle = www.assetBundle;
			ShowLoading (false);
			if (OnComplete!=null) OnComplete();

		} 
	}

	void Update()
	{
		if (loading) {
			if (progressSlider!=null) {
				progressSlider.value = www.progress;
			}
		}
	}

	public void Clear()
	{
		// clear texture and unload from memory
		bundle.Unload (false);
		//www.Dispose ();
		//Resources.UnloadUnusedAssets ();
	}

	void ShowLoading(bool show) {
		loading = show;
		if (loadingIndicator != null) loadingIndicator.SetActive (show);
	}
}
