using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using SimpleJSON;

using HutongGames.PlayMaker;
using UnityEngine.UI;

// Recognizing and parsing speech
namespace ZefirVR {

	public class SpeechRecognizerManager : MonoBehaviour {

		[UnityEngine.Tooltip("Use this to specify target Fsm, if not set, will broadcast to All")]
		public PlayMakerFSM targetFsm;

		[UnityEngine.Tooltip("if set will start listening automatically on scene start")]
		public bool autoStart = false;

		public bool matchCompletePhrase = false;

		public enum Recognizer {
			GoogleSpeechRecognizer,
			AndroidSpeechRecozniger
		}

		[UnityEngine.Tooltip("Choose the recognizer you want to be used")]
		public Recognizer m_recognizer;

		[Serializable]
		public class Phrase {
			public string playmakerEvent;
			public string words;
		}

		[UnityEngine.Tooltip("Prive a list of words and phrases to recognize and event to fire")]
		public Phrase[] recognizePhrases;

		[Serializable]
		public class ResultPhrase {
			public string phrase;
			public float confidence;
		}

		public Image recordIndicator;

		public Color idleColor = Color.clear;
		public Color recordColor = Color.green;
		public Color recognizeColor = Color.red;

		public string matchedPhrase;
		[HideInInspector] public string api_result;

		private List<ResultPhrase> results;
		GoogleSpeechRecognizer googleRecognizer;
		AndroidSpeechRecognizer androidRecognizer;
		AndroidSpeaker speaker;

		private bool isListening = false;

		void Awake()
		{
			googleRecognizer = GetComponent<GoogleSpeechRecognizer> ();
			androidRecognizer = GetComponent<AndroidSpeechRecognizer> ();
			speaker = GetComponent<AndroidSpeaker> ();
			isListening = false;
		}

		void Start()
		{
			if (m_recognizer == Recognizer.GoogleSpeechRecognizer) {
				googleRecognizer.OnListeningStarted += Listening;
				googleRecognizer.OnRecognitionStarted += Recognizing;
			}

			if (androidRecognizer!=null) {
				androidRecognizer.OnListeningStarted += Listening;
			}

			if (autoStart) StartRecognizing ();
		}

		// Playmaker helper method, will fire completeEvent when finished listening and recognizing
		public void StartRecognizingPM(string completeEvent) {
			StartRecognizing (()=>{
				if (!string.IsNullOrEmpty (completeEvent)) SendEvent (completeEvent);
			});
		}

		// Start listening and recognizing
		public void StartRecognizing (Action onComplete = null) {
			matchedPhrase = "";
			isListening = true;
			Listening ();
			if (m_recognizer == Recognizer.GoogleSpeechRecognizer) {
				googleRecognizer.ListenAndRecognize ((result) => {
					isListening = false;
					Idle ();
					print("result: " + result);
					api_result = result;
					ParseGoogleResponse(result);
					ExecuteResults();
					if (onComplete!=null) onComplete();
				});
			} else {
				androidRecognizer.ListenAndRecognize ((result) => {
					isListening = false;
					Idle ();
					api_result = result;
					print("result: " + result);
					ParseAndroidResponse (result);
					ExecuteResults();
					if (onComplete!=null) onComplete();
				});
			}
		}

		// Send playmaker event
		void SendEvent(string eventName)
		{
			if (targetFsm!=null) targetFsm.SendEvent (eventName);
			else PlayMakerFSM.BroadcastEvent (eventName);
		}

		// Search for phrases and fire events
		public void ExecuteResults()
		{
			print("Analyze Results...");
			for(int i=0; i< results.Count; i++) {
				for(int j=0; j < recognizePhrases.Length; j++) {
					var words = recognizePhrases [j].words.Split (',').ToArray ();
					foreach(var word in words) {
						if ((matchCompletePhrase && word == results[i].phrase) || (!matchCompletePhrase && results[i].phrase.Contains (word))) {
							print ("found match: " + word + " firing playmaker event: " + recognizePhrases [j].playmakerEvent);
							matchedPhrase = recognizePhrases [j].playmakerEvent;
							SendEvent (recognizePhrases [j].playmakerEvent);
							return;
						}
					}
				}
			}
			matchedPhrase = "";
			print ("no match found... try again");
		}

		// Parse json result from google recognizer
		void ParseGoogleResponse(string result) 
		{
			results = new List<ResultPhrase> ();
			if (string.IsNullOrEmpty (result)) {
				print ("Empty result from Google...");
				return;
			}

			JSONNode jsonData = JSONNode.Parse (result);

			if (jsonData["results"]!=null) {
				var jsonResults = jsonData["results"];
				for (int i = 0; i < jsonResults.Count; i++) {
					for (int j = 0; j < jsonResults[i]["alternatives"].Count; j++) {
						ResultPhrase data = new ResultPhrase ();
						data.phrase = jsonResults[i]["alternatives"][j]["transcript"];
						data.confidence = jsonResults[i]["alternatives"][j]["confidence"].AsFloat;
						results.Add (data);
					}
				}
			}

			// sort results by confidence
			results.Sort((r1,r2)  => r1.confidence.CompareTo (r2.confidence));
			print ("recognized " + results.Count + " phrases");
		}

		// Parse json result from android recognizer
		void ParseAndroidResponse(string result) 
		{
			results = new List<ResultPhrase> ();
			if (string.IsNullOrEmpty (result)) {
				print ("Empty result from Android...");
				return;
			}

			var parts = result.Replace ("/::/",",").Split (',');

			for (int i = 0; i < parts.Length; i++) {
				ResultPhrase data = new ResultPhrase ();
				data.phrase = parts[i].Trim ();
				data.confidence = 1.0f;
				results.Add (data);
			}

			// sort results by confidence
			print ("recognized " + results.Count + " phrases");
		}

		void Listening()
		{
			if (recordIndicator!=null) recordIndicator.color = recordColor;
		}

		void Recognizing()
		{
			if (recordIndicator!=null) recordIndicator.color = recognizeColor;
		}

		void Idle()
		{
			if (recordIndicator!=null) recordIndicator.color = idleColor;
		}

	}
}
