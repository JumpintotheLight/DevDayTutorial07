using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace ZefirVR {

	public class AndroidSpeechRecognizer : MonoBehaviour
	{
		public string languageCode = "en-US";
		public int maxResults = 5; // max number of recognized results (there will be options)

		[Tooltip("If enabled, continue listening until there is a recognized result")]
		public bool continuousListening = true; // continue listening until there is a recognized result

		public Action<string> OnRecognized; 
		public Action OnListeningStarted;
		public Action OnRecognitionStarted;

		private AndroidSpeechRecognizerPlugin recognizer = null;
		private bool listening = false;
		private string message = "";
		public string[] phrases;

		void Start ()
		{
			if (Application.platform != RuntimePlatform.Android) {
				Debug.Log ("Speech recognition is only available on Android");
				return;
			}

			if (!AndroidSpeechRecognizerPlugin.IsAvailable ()) {
				Debug.Log ("Speech recognition is not available...");
				return;
			}

			recognizer = new AndroidSpeechRecognizerPlugin (gameObject.name);
		}

		// Start listening
		public void ListenAndRecognize (Action<string> OnComplete=null)
		{
			if (AndroidSpeechRecognizerPlugin.IsAvailable ()) {
				listening = true;
				if (OnComplete!=null) OnRecognized = OnComplete;
				recognizer.StartListening (maxResults, languageCode);
			}
		}

		// Stop listening
		public void StopListening()
		{
			if (listening) {
				recognizer.StopListening ();

				listening = false;
			}
		}

		// Cancel listening
		public void CancelListening()
		{
			if (listening) {
				recognizer.CancelListening ();
				listening = false;
			}
		}

		void OnDestroy ()
		{
			if (recognizer != null) recognizer.Release ();
		}

		void OnSpeechEvent (string e)
		{
			switch (int.Parse (e)) {
			case AndroidSpeechRecognizerPlugin.EVENT_SPEECH_READY:
				print ("Started listening");
				if (OnListeningStarted!=null) OnListeningStarted();
				break;
			case AndroidSpeechRecognizerPlugin.EVENT_SPEECH_STARTING:
				print ("Speech started");
				break;
			case AndroidSpeechRecognizerPlugin.EVENT_SPEECH_FINISHED:
				print ("Speech stopped");
				break;
			}
		}
			
		void OnSpeechResults (string results)
		{
			listening = false;
			phrases = results.Split (new string[] { AndroidSpeechRecognizerPlugin.RESULT_SEPARATOR }, System.StringSplitOptions.None);
			var result = string.Join (",", phrases);
			print ("Recognition results: " + result);
			if (OnRecognized!=null) OnRecognized(result);
		}

		void OnSpeechError (string error)
		{
			switch (int.Parse (error)) {
			case AndroidSpeechRecognizerPlugin.ERROR_INSUFFICIENT_PERMISSIONS:
				print ("Insufficient permissions. Make sure RECORD_AUDIO and INTERNET permissions are in the manifest");
				break;
			case AndroidSpeechRecognizerPlugin.ERROR_NETWORK:
				print ("Network error");
				break;
			case AndroidSpeechRecognizerPlugin.ERROR_NETWORK_TIMEOUT:
				print ("Network timeout");
				break;
			case AndroidSpeechRecognizerPlugin.ERROR_NO_MATCH:
				print ("No recognized results");
				if (continuousListening) {
					StartCoroutine (wait(2, () => {ListenAndRecognize();}));
				}
				break;
			case AndroidSpeechRecognizerPlugin.ERROR_NOT_INITIALIZED:
				print ("Not initialized");
				break;
			case AndroidSpeechRecognizerPlugin.ERROR_RECOGNIZER_BUSY:
				print ("Recognizer is busy");
				break;
			case AndroidSpeechRecognizerPlugin.ERROR_SERVER:
				print ("Server Error");
				break;
			case AndroidSpeechRecognizerPlugin.ERROR_SPEECH_TIMEOUT:
				print ("Speech was not detected");
				break;
			case AndroidSpeechRecognizerPlugin.ERROR_AUDIO:
				print ("Audio recording Error");
				break;
			case AndroidSpeechRecognizerPlugin.ERROR_CLIENT:
				print ("Client error");
				break;
			default:
				break;
			}

			listening = false;
		}

		IEnumerator wait(float seconds, Action OnCompleteWait)
		{
			yield return new WaitForSeconds (seconds);
			OnCompleteWait.Invoke ();
		}
	}

}
