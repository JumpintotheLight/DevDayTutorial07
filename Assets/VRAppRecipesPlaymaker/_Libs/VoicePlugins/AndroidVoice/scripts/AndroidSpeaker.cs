using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using HutongGames.PlayMaker;

// Speaking text with human voice
namespace ZefirVR {
	
	public class AndroidSpeaker : MonoBehaviour {

		public PlayMakerFSM fsm;
		public float pitch = 1f;
		public float speechRate = 1f;

		private bool initError = false;
		private int speechId = 0;
		private int locale = 0;
		private string[] localeStrings;

		private Action OnComplete = null;
		private float waitAtPhraseEnd = 0f;

		void Start()
		{
			AndroidTextToSpeech.Initialize(transform.name, "OnTTSInit");
		}

		// Helper method for playmaker
		public void SpeakPM(string text, float silence, string completeEventName)
		{
			Speak(text,silence, () => {
				if (fsm!=null) fsm.SendEvent (completeEventName);
				else PlayMakerFSM.BroadcastEvent (completeEventName);
			});
		}

		// Speak out one phrase text, ending with silence and trigger OnCompleted
		public void Speak(string text, float silence = 0f, Action onCompleted = null)
		{
			OnComplete = onCompleted;
			waitAtPhraseEnd = silence; 

			if (AndroidTextToSpeech.IsInitialized ()) {
				AndroidTextToSpeech.Speak (text, false, AndroidTextToSpeech.STREAM.Music, 1f, 0f, transform.name, "OnSpeechCompleted", "speech_" + (++speechId));
			}
		}

		// wait for a while (to give silence)
		public void Silence(float seconds, Action OnCompleteSilence) 
		{
			StartCoroutine (wait(seconds, () => {
				OnCompleteSilence.Invoke ();
			}));
		}

		IEnumerator wait(float seconds, Action OnCompleteWait)
		{
			yield return new WaitForSeconds (seconds);
			OnCompleteWait.Invoke ();
		}

		public void Stop()
		{
			AndroidTextToSpeech.Stop();
		}

		void OnDestroy()
		{
			AndroidTextToSpeech.Shutdown();
		}

		// called when TTS initialized
		void OnTTSInit(string message)
		{
			int response = int.Parse(message);
			switch (response)
			{
			case AndroidTextToSpeech.SUCCESS:
				List<AndroidTextToSpeech.Locale> l = AndroidTextToSpeech.AvailableLanguages();
				localeStrings = new string[l.Count];
				for (int i = 0; i < localeStrings.Length; ++i) localeStrings [i] = l [i].Name;

				AndroidTextToSpeech.SetPitch (pitch);
				AndroidTextToSpeech.SetSpeechRate (speechRate);

				print("AndroidSpeaker is ready...");
				break;
			case AndroidTextToSpeech.ERROR:
				initError = true;
				break;
			}
		}

		// called when speech is completed
		void OnSpeechCompleted(string id)
		{
			if (OnComplete!=null) {
				if (waitAtPhraseEnd>0) Silence (waitAtPhraseEnd, OnComplete);
				else OnComplete.Invoke ();
				waitAtPhraseEnd = 0;
			}
			Debug.Log("Speech '" + id + "' is complete.");
		}

		void OnSynthesizeCompleted(string id)
		{
			Debug.Log("Synthesize of speech '" + id + "' is complete.");
		}
	}
}