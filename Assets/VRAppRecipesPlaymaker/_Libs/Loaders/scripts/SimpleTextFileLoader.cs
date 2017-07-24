// Load text file with Playmaker support

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine.UI;

// Load text file from streaming assets, folder or http server
public class SimpleTextFileLoader : MonoBehaviour
{
	[UnityEngine.Tooltip("Use this url to autoload text on start")]
	public string url = null;

	[UnityEngine.Tooltip("This game object will be active while loading")]
	public GameObject loadingIndicator;

	[UnityEngine.Tooltip("Assign this if you have a progress indicator for loading")]
	public Slider progressSlider = null;

	[UnityEngine.Tooltip("Enable this to load specific text file, assign it to Text element")]
	public bool loadOnStart = false;

	[UnityEngine.Tooltip("Target Text component")]
	public Text targetTextComponent;

	[HideInInspector] public string text;

	bool loading = false;
	WWW www;

	void Start()
	{
		if (loadOnStart) {
			LoadTextFromUrl (url, () => {
				if (targetTextComponent!=null) targetTextComponent.text = text;
			});
		}
	}

	// General method to be used from other scripts
	public void LoadTextFromUrl(string url, Action OnComplete)
	{
		StartCoroutine(LoadText(url, OnComplete));
	}

	// Helper method for Playmaker
	public void LoadTextPM(string url, PlayMakerFSM fsm, string eventName)
	{
		StartCoroutine(LoadText(url, () => {
			if (fsm!=null) fsm.SendEvent (eventName);
		}));
	}

	// Load audio file
	IEnumerator LoadText(string url, Action OnComplete)
	{		
		Debug.Log("Loading text file: " + url);
		if (!url.StartsWith ("http") && !url.StartsWith ("file") && !url.StartsWith ("jar:")) url = "file:///" + url; // add 'file' prefix for local file system

		ShowLoading (true);
		www = new WWW(url);
		www.threadPriority = ThreadPriority.Low;
		yield return www;

		if (www.error==null) {
			text = www.text;

			Debug.Log ("finished loading text file: " + url);
			ShowLoading (false);
			if (OnComplete!=null) OnComplete();
		} else print("Error loading: " + url + " - " + www.error);
	}

	// Update loading progress
	void Update()
	{
		if (loading) {
			if (progressSlider!=null) {
				progressSlider.value = www.progress;
			}
		}
	}

	// Clean up memory (not sure if this is needed at all, but...)
	public void Clear()
	{
		// clear texture and unload from memory
		text = null;
		www = null;
		Resources.UnloadUnusedAssets ();
	}

	// Show / Hide loading
	void ShowLoading(bool show) {
		loading = show;
		if (loadingIndicator != null) loadingIndicator.SetActive (show);
	}
}