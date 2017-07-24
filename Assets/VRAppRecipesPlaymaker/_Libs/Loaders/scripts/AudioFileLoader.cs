// Load audio helpers with Playmaker support

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine.UI;

// Load and play audio file from streaming assets, folder or http server
public class AudioFileLoader : MonoBehaviour
{
	[UnityEngine.Tooltip("Use this url to autoload audio on start")]
	public string url = null;

	[UnityEngine.Tooltip("This game object will be active while loading")]
	public GameObject loadingIndicator;

	[UnityEngine.Tooltip("Assign this if you have a progress indicator for loading")]
	public Slider progressSlider = null;

	[UnityEngine.Tooltip("Enable this to load specific audio, assign it to audiosource and play it")]
	public bool loadOnStart = false;

	[UnityEngine.Tooltip("Enable this to play audio once it's loaded")]
	public bool playOnStart = false;

	[UnityEngine.Tooltip("Target Audio source")]
	public AudioSource targetAudioSource;

	[HideInInspector] public AudioClip audioClip;

	bool loading = false;
	WWW www;

	void Start()
	{
		if (loadOnStart) {
			LoadAudioFromUrl (url, () => {
				if (playOnStart) {
					if (targetAudioSource==null) {
						targetAudioSource = gameObject.AddComponent <AudioSource>();
						targetAudioSource.playOnAwake = false;
					}
					targetAudioSource.clip = audioClip;
					targetAudioSource.Play ();
				} else {
					if (targetAudioSource!=null) targetAudioSource.clip = audioClip;
				}
			});
		}
	}

	// General method to be used from other scripts
	public void LoadAudioFromUrl(string url, Action OnComplete)
	{
		StartCoroutine(LoadAudio(url, OnComplete));
	}

	// Helper method for Playmaker
	public void LoadAudioPM(string url, PlayMakerFSM fsm, string eventName)
	{
		StartCoroutine(LoadAudio(url, () => {
			if (fsm!=null) fsm.SendEvent (eventName);
		}));
	}

	// Load audio file
	IEnumerator LoadAudio(string url, Action OnComplete)
	{		
		Debug.Log("Loading image: " + url);
		if (!url.StartsWith ("http") && !url.StartsWith ("file") && !url.StartsWith ("jar:")) url = "file:///" + url; // add 'file' prefix for local file system

		ShowLoading (true);
		www = new WWW(url);
		www.threadPriority = ThreadPriority.Low;
		yield return www;

		if (www.error==null) {
			audioClip = www.GetAudioClip (true);

			Debug.Log ("finished loading audio file: " + url);
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
		audioClip = null;
		www = null;
		Resources.UnloadUnusedAssets ();
	}

	// Show / Hide loading
	void ShowLoading(bool show) {
		loading = show;
		if (loadingIndicator != null) loadingIndicator.SetActive (show);
	}
}
