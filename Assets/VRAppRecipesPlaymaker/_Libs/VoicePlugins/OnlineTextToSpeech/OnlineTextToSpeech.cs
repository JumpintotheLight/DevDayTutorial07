using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Provdes online text to speech by positiongames.com
namespace ZefirVR {
	
	public class OnlineTextToSpeech : MonoBehaviour {

		public String text2Speech= "Hello, how are you";
		public bool speakOnStart = false;

		string API_URL = "http://positiongames.com/speech/speak?text=";

		void Start()
		{
			if (speakOnStart) Speak(text2Speech);
		}

		// Speak 
		public void Speak(String text, float silence = 0f, Action OnComplete = null)
		{
			String e = WWW.EscapeURL(text);
			StartCoroutine(DownloadAndPlay(API_URL + e, silence, OnComplete));
		}

		IEnumerator DownloadAndPlay(string url, float silence, Action OnComplete)
		{
			WWW www = new WWW(url);
			yield return www;
			AudioSource audio = GetComponent<AudioSource>();
			audio.clip = www.GetAudioClip(false, true, AudioType.WAV);
			audio.Play();

			// wait until audio is playing
			while(audio.isPlaying) yield return null;

			Silence (silence, OnComplete);
		}

		// wait for a while (to give silence)
		public void Silence(float seconds, Action OnCompleteSilence) 
		{
			StartCoroutine (wait(seconds, () => {
				if (OnCompleteSilence!=null) OnCompleteSilence.Invoke ();
			}));
		}

		IEnumerator wait(float seconds, Action OnCompleteWait)
		{
			yield return new WaitForSeconds (seconds);
			OnCompleteWait.Invoke ();
		}
	}
}