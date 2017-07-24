using UnityEngine;
using System.Collections.Generic;

/// Android text to speech generator
namespace ZefirVR {

	public static class AndroidTextToSpeech
	{
		public enum STREAM
		{
			Alarm = 4,
			DTMF = 8,
			Music = 3,
			Notification = 5,
			Ring = 2 
		}

		public class Locale
		{
			public string Name { get; private set; }
			public string Language { get; private set; }
			public Locale(string name, string language)
			{
				Name = name;
				Language = language;
			}
		}

		public static readonly Locale ENGLISH = new Locale("English", "en");
		public static readonly Locale RUSSIAN = new Locale("Russian", "ru");
		public static readonly Locale GERMAN = new Locale("German", "de");
		public static readonly Locale ITALIAN = new Locale("Italian", "it");
		public static readonly Locale CHINESE = new Locale("Chinese", "zh");
		public static readonly Locale FRENCH = new Locale("French", "fr");
		public static readonly Locale JAPANESE = new Locale("Japanese", "ja");
		public static readonly Locale KOREAN = new Locale("Korean", "ko");
		public static readonly Locale[] Locales;

		public const int SUCCESS = 0;
		public const int ERROR = -1;

		private static AndroidJavaObject ttsPlugin = null;

		private const string JAVA_CLASS_NAME = "com.zefirvr.unity.plugins.AndroidTTSManager";

		static AndroidTextToSpeech()
		{
			Locales = new Locale[] {ENGLISH, RUSSIAN, GERMAN, ITALIAN, CHINESE, FRENCH, JAPANESE, KOREAN};
		}

		/// Initialize the TextToSpeech Generator
		public static void Initialize(string gameObjectName, string callbackMethodName)
		{
			if (Application.platform != RuntimePlatform.Android) return;
			ttsPlugin = new AndroidJavaObject(JAVA_CLASS_NAME, new object[] {gameObjectName, callbackMethodName});
		}

		// Generate speech
		public static void Speak(string text, bool addToQueue, STREAM stream = STREAM.Music,
			float volume = 1f, float pan = 0f, string gameObjectName = null,
			string callbackMethodName = null, string id = null)
		{
			if (Application.platform != RuntimePlatform.Android) return;
			if (!IsInitialized()) throw new System.InvalidOperationException("Android Text To Speech Plugin is not initialized");

			ttsPlugin.Call("speak", text, addToQueue, (int)stream, volume, pan, gameObjectName, callbackMethodName, id);
		}

		public static void SaveSpeechToFile(string text, string filename, string gameObjectName = null, string callbackMethodName = null, string id = null)
		{
			if (Application.platform != RuntimePlatform.Android) return;
			if (!IsInitialized()) throw new System.InvalidOperationException("Android Text To Speech Plugin is not initialized");

			ttsPlugin.Call("speakToFile", text, filename, gameObjectName, callbackMethodName, id);
		}

		public static void Stop()
		{
			if (Application.platform != RuntimePlatform.Android) return;

			if (!IsInitialized()) throw new System.InvalidOperationException("Android Text To Speech Plugin is not initialized");

			ttsPlugin.Call("stop");
		}

		public static void Shutdown()
		{
			if (Application.platform != RuntimePlatform.Android) return;

			if (!IsInitialized()) throw new System.InvalidOperationException("Android Text To Speech Plugin is not initialized");

			ttsPlugin.Call("shutdown");
			ttsPlugin = null;
		}

		public static bool IsInitialized()
		{
			return Application.platform == RuntimePlatform.Android && ttsPlugin != null && ttsPlugin.Call<bool>("isInitialized");
		}

		public static bool IsSpeaking()
		{
			if (Application.platform != RuntimePlatform.Android) return false;
			if (!IsInitialized()) throw new System.InvalidOperationException("Android Text To Speech Plugin is not initialized");

			return ttsPlugin.Call<bool>("isSpeaking");
		}

		public static bool IsLanguageAvailable(Locale locale)
		{
			if (Application.platform != RuntimePlatform.Android) return false;
			if (!IsInitialized()) throw new System.InvalidOperationException("Android Text To Speech Plugin is not initialized");

			return ttsPlugin.Call<bool>("isLanguageAvailable", locale.Language);
		}

		public static List<Locale> AvailableLanguages()
		{
			List<Locale> availableLanguages = new List<Locale>();
			if (Application.platform != RuntimePlatform.Android) return availableLanguages;

			if (!IsInitialized()) throw new System.InvalidOperationException("Android Text To Speech Plugin is not initialized");

			foreach (Locale locale in Locales)
			{
				if (IsLanguageAvailable(locale)) availableLanguages.Add(locale);
			}
			return availableLanguages;
		}

		public static bool SetLanguage(Locale locale)
		{
			if (Application.platform != RuntimePlatform.Android) return false;
			if (!IsInitialized()) throw new System.InvalidOperationException("Android Text To Speech Plugin is not initialized");

			return ttsPlugin.Call<bool>("setLanguage", locale.Language);
		}

		public static void SetPitch(float pitch)
		{
			if (Application.platform != RuntimePlatform.Android) return;
			if (!IsInitialized()) throw new System.InvalidOperationException("Android Text To Speech Plugin is not initialized");

			ttsPlugin.Call("setPitch", pitch);
		}

		public static void SetSpeechRate(float rate)
		{
			if (Application.platform != RuntimePlatform.Android) return;
			if (!IsInitialized()) throw new System.InvalidOperationException("Android Text To Speech Plugin is not initialized");

			ttsPlugin.Call("setSpeechRate", rate);
		}
	}

}
