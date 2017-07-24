using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using LitJson;

namespace ZefirVR {

	public enum Status {
		NOT_READY,
		READY,
		LISTENING,
		RECOGNIZING,
		ERROR
	}

	public class GoogleSpeechRecognizer : MonoBehaviour {

		public string API_KEY = "Your Google Cloud Speech API key";        
		public string languageCode = "en-US";   

		[Tooltip("Minimal voice level to activate recording")]
		public float minVoiceLevel = 0.35f;

		[Tooltip("Minimal time to listen for a phrase in seconds")]
		public float minPhraseTime = 3f;

		[Tooltip("Max recodring time in seconds")]
		public int maxListeningTime = 10;

		// current recognizer status
		[HideInInspector] public Status recognizerStatus = Status.NOT_READY;

		// current voice level
		[HideInInspector] public float voiceLevel;

		// recorded audio
		[HideInInspector] public AudioClip audioClip;

		[HideInInspector] public string resultJson;

		// Subscribe to these actions if you want to track progress
		public Action OnListeningStarted;
		public Action OnListeningStopped;
		public Action OnRecognitionStarted;
		public Action<string> OnRecognitionCompleted;

		private string microphoneDeviceName;

		private bool isListening;
		private bool voiceDetected;
		private float silenceThreshold = 0.046f;
		private int SampleFrequency = 16000;

		public int SAMPLE_BLOCK = 32;
		const string GOOGLE_API_URL = "https://speech.googleapis.com/v1beta1/speech:syncrecognize/?key=";

		void Start() {
			ChangeStatus(Status.NOT_READY);
			FindMicrophoneDevices ();
		}

		// Start listening
		public void ListenAndRecognize(Action<string> OnComplete = null) {
			print ("Google Speech Recognizer - start listening");
			if (OnComplete!=null) OnRecognitionCompleted = OnComplete;

			StartCoroutine (waitForInit ());
		}

		// wait until recognizer is ready and start recognizing
		IEnumerator waitForInit()
		{
			// wait until recognizer is not ready... anything but NOT ready
			if (recognizerStatus==Status.NOT_READY) print("wait for recognizer to get ready...");
			while (recognizerStatus==Status.NOT_READY) yield return null;

			if (recognizerStatus!=Status.ERROR && !string.IsNullOrEmpty(microphoneDeviceName)) {                
				isListening = true;

				// start recording audio from microphone
				audioClip = Microphone.Start(microphoneDeviceName, false, maxListeningTime, SampleFrequency);
				ChangeStatus(Status.LISTENING);

				// make sure we listen for a phrase
				ListenOnePhrase();

				// make sure it's not longer than max time
				Invoke("CompleteListening", maxListeningTime);

				// let other scripts know that we started listening
				if (OnListeningStarted != null) OnListeningStarted();
			} else {
				print("Microphone is not available"); 
			}
		}

		// Stop recording audio
		public void StopListening() {            
			if (isListening) {
				// stop recording audio from microphone
				Microphone.End(microphoneDeviceName);
				isListening = false;
			}
			ChangeStatus(Status.READY);

			print ("Google Speech Recognizer - stop listening");
			if (OnListeningStopped!= null) OnListeningStopped();
		}

		void Update() {
			if (isListening) {
				// make sure we don't miss when user starts speaking
				bool isSpeaking = voiceDetected;
				voiceLevel = GetVoiceInputLevel();

				// have not detected user speaking? try again...
				if (!voiceDetected) voiceDetected = (voiceLevel >= minVoiceLevel);

				// if new voice peak detected make sure we listen for another phrase
				if (isSpeaking != voiceDetected) ListenOnePhrase();
			}
		}

		// make sure user gets minPhraseTime to speak
		private void ListenOnePhrase() {
			//print ("Listen One Phrase");
			string invokeName = "CompleteListening";
			if (IsInvoking(invokeName)) CancelInvoke(invokeName);
			Invoke(invokeName, minPhraseTime);
			voiceDetected = false;
		}

		// finish with listening and start recognizing
		private void CompleteListening() {
			CancelInvoke();        
			StopListening();
			Recognize();                            
		}

		// voice detector, analyzing wav data for volume
		private float GetVoiceInputLevel() {
			float inputLevel = 0;
			float[] wavData = new float[SAMPLE_BLOCK];
			int micPosition = Microphone.GetPosition(null) - (SAMPLE_BLOCK + 1);
			if (micPosition < 0) return 0;
			audioClip.GetData(wavData, micPosition);
			for (int i = 0; i < SAMPLE_BLOCK; i++) {
				float wavePeak = wavData[i] * wavData[i];
				if (inputLevel < wavePeak) {
					inputLevel = wavePeak;
				}
			}
			return Mathf.Sqrt(inputLevel);
		}
			
		// save recorded audio and get it recognized by Google
		private void Recognize() {            
			if (audioClip != null && audioClip.length > 0) {
				audioClip = SavWav.TrimSilence(audioClip, silenceThreshold);
				if (audioClip!=null && audioClip.length >0) {
					byte[] buffer = SavWav.Save(audioClip);
					if (buffer != null) {
						StartCoroutine(AskGoogle(buffer));
					}
				} else if (OnRecognitionCompleted!= null) OnRecognitionCompleted("");
			} else {
				// hmm, there is nothing to recognize...
				if (OnRecognitionCompleted!= null) OnRecognitionCompleted("");
			}
		}

		// calling google api
		private IEnumerator AskGoogle(byte[] buffer) 
		{

			JsonData requestJSON = new JsonData();
			JsonData requestConfig = new JsonData();
			JsonData requestAudio = new JsonData();

			requestConfig["encoding"] = "LINEAR16";
			requestConfig["sampleRate"] = SampleFrequency.ToString ();
			requestConfig["languageCode"] = languageCode;

			requestAudio["content"] = Convert.ToBase64String(buffer);

			requestJSON["config"] = requestConfig;
			requestJSON["audio"] = requestAudio;

			string url = GOOGLE_API_URL + API_KEY;
			byte[] bytes = Encoding.UTF8.GetBytes(requestJSON.ToJson());
			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Content-Type", "application/json");

			if (OnRecognitionStarted != null) OnRecognitionStarted();

			ChangeStatus(Status.RECOGNIZING);
			print ("Google Recognizer - start recognizing");

			// calling Google...
			WWW www = new WWW(url, bytes, headers);
			while (!www.isDone) {
				yield return null;
			}

			if (!string.IsNullOrEmpty(www.error)) {
				Debug.LogError("Recognition error from google: " + www.error);
			}

			// we got results, pass them on
			resultJson = www.text;
			if (OnRecognitionCompleted!= null) OnRecognitionCompleted(resultJson);

			//print ("Google Recognizer - completed recognizing: " + resultJson);

			// all done, waiting for command
			ChangeStatus(Status.READY);
		}

		private void ChangeStatus(Status status) {
			if (this.recognizerStatus != status) this.recognizerStatus = status;
		}

		// find first available microphone on this device
		void FindMicrophoneDevices()
		{
			string[] devices = Microphone.devices;
			if (devices.Length > 0) {
				microphoneDeviceName = devices[0];
				print ("Found microphone: " + microphoneDeviceName);
				ChangeStatus(Status.READY);
			}
			else {
				ChangeStatus(Status.ERROR);
				Debug.LogError("Microphone not found");
			}
		}

		// stop microphone and all coroutines
		void OnDestroy() {
			switch (recognizerStatus) {
			case Status.LISTENING:
				if (isListening) Microphone.End(microphoneDeviceName);
				break;
			case Status.RECOGNIZING:
				StopAllCoroutines();
				break;
			}            
		}
	}

}
