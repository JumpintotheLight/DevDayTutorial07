using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using System;

// Universal wrapper for Text to speech services with playmaker support
namespace ZefirVR {
	
	public class TTSManager : MonoBehaviour 
	{
		[UnityEngine.Tooltip("Specify fsm component or leave empty to broadcast event to all")]
		public PlayMakerFSM fsm;

		public string text2speech = "Hello!";

		[UnityEngine.Tooltip("Enable to automatically start speaking on activation")]
		public bool autoStart = false;

		public enum TTSmodule {
			AndroidTTS,
			OnlineTTS,
			WatsonTTS
		}

		[UnityEngine.Tooltip("Choose which module to use for TTS")]
		public TTSmodule TTSSourceModule;

		private OnlineTextToSpeech onlineTTS;
		private AndroidSpeaker androidSpeaker;
		private WatsonTTS watsonTTS;

		void Awake()
		{
			androidSpeaker = GetComponent<AndroidSpeaker> ();
			onlineTTS = GetComponent<OnlineTextToSpeech> ();
			watsonTTS = GetComponent<WatsonTTS> ();
		}

		void Start()
		{
			if (autoStart) Speak(text2speech, 0f);
		}

		// Playmaker helper method, trigger event when completes speaking
		public void SpeakPM(string text, float silence, string completeEventName) {
			if (string.IsNullOrEmpty (text)) text = text2speech;
			Speak (text, silence, () => {
				if (fsm!=null) fsm.SendEvent (completeEventName);
				else PlayMakerFSM.BroadcastEvent (completeEventName);
			});
		}

		// Choose a random phrase, insert a param and say it
		public void SpeakRandomWithParam(string[] phrases, string paramText, string completeEventName) {

			if (phrases.Length>0) {
				int index = UnityEngine.Random.Range (0,phrases.Length);
				string phrase = phrases [index].Replace ("$", paramText);

				Speak (phrase, 0f, () => {
					if (fsm!=null) fsm.SendEvent (completeEventName);
					else PlayMakerFSM.BroadcastEvent (completeEventName);
				});

			}
		}

		// Speak
		public void Speak (string text, float silence, Action OnComplete = null) {
			switch(TTSSourceModule) {
			case TTSmodule.AndroidTTS:
				androidSpeaker.Speak (text, silence, OnComplete);
				break;
			case TTSmodule.OnlineTTS:
				onlineTTS.Speak (text, silence, OnComplete);
				break;
			case TTSmodule.WatsonTTS:
				watsonTTS.Speak (text, silence, OnComplete);
				break;
			}
		}
	
	}
}