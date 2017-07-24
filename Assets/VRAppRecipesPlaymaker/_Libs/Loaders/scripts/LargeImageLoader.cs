// Load large images from streaming assets of web with Playmaker support

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine.UI;

public class LargeImageLoader : MonoBehaviour
{
	[UnityEngine.Tooltip("This game object will be active while loading")]
	public GameObject loadingIndicator;

	[UnityEngine.Tooltip("Assign this if you have a progress indicator for loading")]
	public Slider progressSlider = null;

	[UnityEngine.Tooltip("Use this url to autoload image on start")]
	public string url = null;

	[UnityEngine.Tooltip("Enable this to load a specific image and assign it to a material")]
	public bool loadOnStart = false;

	[UnityEngine.Tooltip("Assign material that you want to accept the downloaded texture")]
	public Material[] targetMaterials;

	[HideInInspector] public Texture2D texture;

	bool loading = false;
	WWW www;

	float maxWidth = 4096f;
	float maxHeight = 4096f;

	void Start()
	{
		if (loadOnStart) {
			LoadImageFromUrl (url, () => {
				if (targetMaterials!=null) {
					for(int i=0;i<targetMaterials.Length;i++) targetMaterials[i].mainTexture = texture;
				}
			});
		}
	}

	public void LoadImageFromUrl(string url, Action OnComplete)
	{
		StartCoroutine(LoadImage(url, OnComplete));
	}

	public void LoadImagePM(string url, PlayMakerFSM fsm, string eventName)
	{
		StartCoroutine(LoadImage(url, () => {
			if (fsm!=null) fsm.SendEvent (eventName);
		}));
	}

	IEnumerator LoadImage(string url, Action OnComplete)
	{		
		Debug.Log("Loading image: " + url);
		if (!url.StartsWith ("http") && !url.StartsWith ("file") && !url.StartsWith ("jar:")) url = "file:///" + url; // add 'file' prefix for local file system

		ShowLoading (true);
		www = new WWW(url);
		www.threadPriority = ThreadPriority.Low;
		yield return www;

		if (www.error==null) {
			texture = www.texture;
			texture.wrapMode = TextureWrapMode.Clamp;

			// Resize image to max size
			int widthSize = texture.width;
			int heightSize = texture.height;
			if (widthSize>4096 || heightSize>4096) {
				print ("large texture : " + widthSize + " / " + heightSize);
				if (widthSize > heightSize) {
					heightSize = Mathf.CeilToInt((float)heightSize * maxWidth / (float)widthSize);
					widthSize = (int)maxWidth;
				} else {
					widthSize = Mathf.CeilToInt((float)widthSize * maxHeight / (float)heightSize);
					heightSize = (int)maxHeight;
				}
				print ("... resizing it to: " + widthSize + " / " + heightSize);
				#if UNITY_ANDROID || UNITY_IOS
				TextureScale.Bilinear (texture, widthSize, heightSize);
				#else
				TextureScaler.scale(texture, widthSize, heightSize);
				#endif
			}

			// assign image name to url
			texture.name = url;
			Debug.Log ("finished loading file: " + url);
			ShowLoading (false);
			if (OnComplete!=null) OnComplete();
		} else print("Error loading: " + url + " - " + www.error);
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
		texture = null;
		www = null;
		Resources.UnloadUnusedAssets ();
	}

	void ShowLoading(bool show) {
		loading = show;
		if (loadingIndicator != null) loadingIndicator.SetActive (show);
	}

}
