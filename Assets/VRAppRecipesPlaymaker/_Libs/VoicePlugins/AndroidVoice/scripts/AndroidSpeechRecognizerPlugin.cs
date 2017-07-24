using UnityEngine;
using System.Collections.Generic;

// Android native speech recognition
namespace ZefirVR  {

	public class AndroidSpeechRecognizerPlugin
	{
		public const string RESULT_SEPARATOR = "/::/";
		public const int	EVENT_SPEECH_READY = -1;
		public const int	EVENT_SPEECH_STARTING = -2;
		public const int	EVENT_SPEECH_FINISHED = -3;
		public const int	ERROR_NOT_INITIALIZED = -1;
		public const int	ERROR_AUDIO = 3;
		public const int	ERROR_CLIENT = 5;
		public const int	ERROR_INSUFFICIENT_PERMISSIONS = 9;
		public const int	ERROR_NETWORK = 2;
		public const int	ERROR_NETWORK_TIMEOUT = 1;
		public const int	ERROR_NO_MATCH = 7;
		public const int	ERROR_RECOGNIZER_BUSY = 8;
		public const int	ERROR_SERVER = 4;
		public const int	ERROR_SPEECH_TIMEOUT = 6;

		private static AndroidJavaObject srManager = null;

		private const string JAVA_CLASS_NAME = "com.zefirvr.unity.plugins.AndroidSpeechRecognizer";

		public AndroidSpeechRecognizerPlugin (string gameObjectName)
		{
			if (Application.platform != RuntimePlatform.Android) return;

			srManager = new AndroidJavaObject (JAVA_CLASS_NAME, new object[] {
				gameObjectName
			});
		}

		public static bool IsAvailable ()
		{
			if (Application.platform != RuntimePlatform.Android) return false;
			AndroidJavaClass jc = new AndroidJavaClass (JAVA_CLASS_NAME);
			return jc.CallStatic<bool> ("isAvailable");
		}

		public void StartListening(int maxResults=5, string language=null)
		{
			if (Application.platform != RuntimePlatform.Android) return;
			srManager.Call ("startListening", language, maxResults);
		}

		public void StopListening ()
		{
			if (Application.platform != RuntimePlatform.Android) return;
			srManager.Call ("stopListening");
		}

		public void CancelListening ()
		{
			if (Application.platform != RuntimePlatform.Android) return;
			srManager.Call ("cancel");
		}

		public void Release ()
		{
			if (Application.platform != RuntimePlatform.Android) return;

			srManager.Call ("release");
		}
	}

}
