// Helper script to show console messages

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZefirVR {
	public class SimpleDebugConsole : MonoBehaviour {

		public bool showStackTrace = false;
		public int stackTraceMaxLength = 100;

		public Text messageText;
		public Image stopButtonImage;
		public Text counterText;
		public Sprite playButtonImage;
		public Sprite pauseButtonImage;
		public Canvas myCanvas;

		private List<string> messages;
		private bool freeze = false;
		private int lastMessage = 0;

		void Awake()
		{
			// assign event camera
			myCanvas.worldCamera = Camera.main;
		}

		void Start () {
			messages = new List<string> ();
		}

		void OnEnable ()
		{
			Application.logMessageReceivedThreaded += HandleLog;
		}

		void OnDisable ()
		{
			Application.logMessageReceivedThreaded -= HandleLog;
		}

		private System.Object mutex = new System.Object ();

		void HandleLog (string text, string stackTrace, LogType type)
		{
			lock(mutex) {
				string message = text;
				switch(type) {
				case LogType.Error:
					message = "<color=#FF7070FF><b>Error:</b> " + message + "</color>";
					break;
				case LogType.Warning:
					message = "<color=#D7DA02FF><b>Warning:</b> " + message + "</color>";
					break;
				case LogType.Exception:
					message = "<b>Exception:</b> " + message;
					break;
				}

				// Add new message to the list
				if (showStackTrace) {
					string stack = stackTrace.Substring (0, stackTraceMaxLength < stackTrace.Length ? stackTraceMaxLength : stackTrace.Length);
					message = message + "\n<i>" + stack + "</i>...";
				}
				messages.Add(message);
			}

			// if not stopped, jump to the last message
			if (!freeze) lastMessage = messages.Count - 1;
			UpdateMessageLog ();
		}

		// Show current message in the log display
		void UpdateMessageLog()
		{
			if (messages.Count>lastMessage) {
				messageText.text = messages[lastMessage];
				counterText.text = (lastMessage + 1).ToString () + "/" + messages.Count;
			}
		}

		// Freeze log and jump to the previous message
		public void PrevMessage()
		{
			if (!freeze) Stop();
			if (lastMessage>0) lastMessage--;
			UpdateMessageLog ();
		}

		// Freeze log and jump to the next message
		public void NextMessage()
		{
			if (!freeze) Stop();
			if (lastMessage<messages.Count-1) lastMessage++;
			UpdateMessageLog ();
		}

		// freeze and unfreeze message log display
		public void Stop()
		{
			if (freeze) {
				// message log was frozen, 
				freeze = false;
				stopButtonImage.sprite = pauseButtonImage;
				lastMessage = messages.Count - 1;
				UpdateMessageLog ();
			} else {
				freeze = true;
				stopButtonImage.sprite = playButtonImage;
			}
		}
	}
}