using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Logging;
using System.IO;

#pragma warning disable 0414

// IBM Watson Text To Speech Service
namespace ZefirVR {

	public class WatsonTTS : MonoBehaviour {

		public string text2Speech = "Hello, how are you?";
		public bool speakOnStart = false;
		public VoiceType voice;

		public bool saveToFile = false;
		private string fileNameBase = "voice";
		private int fileIndex = 1;

		TextToSpeech m_TextToSpeech = new TextToSpeech();
		Action OnComplete;
		float silenceTime = 0f;
		int saveBps = 16;

		void Start()
		{
			LogSystem.InstallDefaultReactors();
			if (speakOnStart) Speak(text2Speech);
		}

		// Speak 
		public void Speak(String text, float silence = 0f, Action OnCompleteAction = null)
		{
			if (OnCompleteAction!=null) OnComplete = OnCompleteAction;
			silenceTime = silence;

			m_TextToSpeech.Voice = voice; //VoiceType.en_US_Allison;
			m_TextToSpeech.ToSpeech(text, HandleToSpeechCallback, true);
		}

		void HandleToSpeechCallback(AudioClip clip)
		{
			StartCoroutine(PlayClip(clip));
		}

		IEnumerator PlayClip(AudioClip clip)
		{
			if (Application.isPlaying && clip != null)
			{
				GameObject audioObject = new GameObject("AudioObject");
				AudioSource source = audioObject.AddComponent<AudioSource>();
				source.spatialBlend = 0.0f;
				source.loop = false;
				source.clip = clip;

				if (saveToFile) {
					// save result to a wav file
					byte[] wavData = IBM.Watson.DeveloperCloud.Utilities.WaveFile.CreateWAV (source.clip, saveBps);
					string fileName = Application.dataPath + "/" + fileNameBase + fileIndex + ".wav";
					fileIndex++;
					File.WriteAllBytes (fileName, wavData);
					print ("Saved wav file to " + fileName);
				}

				source.Play();

				// wait until audio is playing
				while(source.isPlaying) yield return null;

				Silence (silenceTime, OnComplete);

				GameObject.Destroy(audioObject);
			}
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