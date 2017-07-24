// Simple Video Player
// Helper method to do all things video with new Unity built-in VideoPlayer

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Video;
using HutongGames.PlayMaker;

namespace ZefirVR {
	
	public class SimpleVideoPlayer : MonoBehaviour {

		[UnityEngine.Tooltip("Use this url to autoload and play video on start")]
		public string videoUrl = null;

		[UnityEngine.Tooltip("Enable this to start playing specific video and assign it to target materials")]
		public bool playOnStart = false;
		public bool loop = true;
		public bool loadFromStreamingAssets = false;

		[UnityEngine.Tooltip("Assign material that you want to accept the downloaded texture")]
		public Material[] targetMaterials;

		[HideInInspector] public Texture videoTexture;

		[HideInInspector] public VideoPlayer videoPlayer;
		[HideInInspector] public AudioSource audioSource;

		private Action onVideoStarted;

		private int currentDisplayIndex = -1;
		private string videoStartedEvent = "OnVideoStarted";

		void Awake () {
			audioSource = gameObject.AddComponent <AudioSource> ();
			videoPlayer = gameObject.AddComponent <VideoPlayer> ();
		}

		void Start()
		{
			// setup default values
			audioSource.playOnAwake = false;
			videoPlayer.playOnAwake = false;
			videoPlayer.isLooping = loop;

			// setup callbacks
			videoPlayer.prepareCompleted += StartPlayback;
			videoPlayer.started += VideoStarted;
			videoPlayer.errorReceived += VideoPlayer_errorReceived;

			// On Start play, if setup
			if (playOnStart) {
				PlayVideo (videoUrl, () => {
					if (targetMaterials!=null) {
						for(int i=0;i<targetMaterials.Length;i++) targetMaterials[i].mainTexture = videoTexture;
					}
				}, loadFromStreamingAssets);
			}
		}

		// Play video method for Playmaker
		public void PlayVideoPM(string url, PlayMakerFSM fsm, bool fromStreamingAssets) {
			PlayVideo (url, () => {
				if (targetMaterials!=null) {
					for(int i=0;i<targetMaterials.Length;i++) targetMaterials[i].mainTexture = videoTexture;
				}
				if (fsm!=null) fsm.SendEvent (videoStartedEvent);
				else PlayMakerFSM.BroadcastEvent (videoStartedEvent);
			}, fromStreamingAssets);
		}

		// Play video from videoUrl
		public void PlayVideo(string url, Action OnStartedPlaying = null, bool fromStreamingAssets = false)
		{
			onVideoStarted = OnStartedPlaying;

			if (string.IsNullOrEmpty (url.Trim ())) url = videoUrl;

			// form url
			if (fromStreamingAssets) url = Application.streamingAssetsPath + "/" + url;

			if (!url.StartsWith ("http") && !url.StartsWith ("file") && !url.StartsWith ("jar:")) url = "file://" + url; // add 'file' prefix for local file system
			Debug.Log ("Play video: " + url);

			if (url.Contains ("jar:")) StartCoroutine (PlayFromStreamingAssets (url)); // this video has to be downloaded to persistant storage first
			else PrepareAndPlay (url);
		}

		// Download video into persistant storage for playback
		IEnumerator PlayFromStreamingAssets(string url)
		{
			string persistentPath =  Application.persistentDataPath + "/" + Path.GetFileName(url);

			#if !UNITY_EDITOR && !UNITY_STANDALONE
			Debug.Log("Downloading video from StreamingAssets:" + url);
			if (!File.Exists(persistentPath))
			{
			WWW wwwReader = new WWW(url);
			yield return wwwReader;

			if (wwwReader.error != null) Debug.LogError("wwwReader error: " + wwwReader.error);

			System.IO.File.WriteAllBytes(persistentPath, wwwReader.bytes);
			Debug.Log("saved video file from streaming assets to: " + persistentPath);
			}
			#else
			persistentPath = url;
			#endif

			// Video must start only after mediaFullPath is filled in
			PrepareAndPlay("file://" + persistentPath);
			yield return null;
		}

		void PrepareAndPlay(string url)
		{
			videoPlayer.enabled = true;
			videoPlayer.source = VideoSource.Url;
			videoPlayer.url = url;
			videoPlayer.renderMode = VideoRenderMode.APIOnly;

			videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
//			videoPlayer.SetDirectAudioVolume (0, 0.1f);
//			videoPlayer.controlledAudioTrackCount = 1;
//			videoPlayer.EnableAudioTrack (0, true);
			videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
			videoPlayer.EnableAudioTrack(0, true);
			videoPlayer.SetTargetAudioSource(0, audioSource);

			videoPlayer.Prepare();
		}

		// this is called when video player is ready
		void StartPlayback(VideoPlayer source)
		{
			Debug.Log("Video player prepared");

			string fileName = videoPlayer.url;

			//Play Video and Audio
			audioSource.Play();
			videoPlayer.Play();
			videoTexture = videoPlayer.texture;

			Debug.Log("Started Playing Video");
			if (onVideoStarted!=null) onVideoStarted();			
		}

		// Stop playing video
		public void StopVideo()
		{
			videoPlayer.Pause ();
			videoPlayer.enabled = false;
			print ("Video Stopped");
			audioSource.Stop ();
			Clear ();
		}

		// TODO  make sure memory is freed after video is stopped
		// stop playing video, clear memory and get it ready to be despawned
		public void Clear () {
			// not sure how to do that yet...
		}

		/// <summary>
		///  Callback methods
		/// </summary>

		// this is called when video started playing
		void VideoStarted(VideoPlayer player)
		{
			
			if (onVideoStarted!=null) onVideoStarted.Invoke ();
		}

		// this is called when ther is a playback error
		void VideoPlayer_errorReceived (VideoPlayer source, string message)
		{
			Debug.Log ("Simple Video Player Error: " + message);
		}

		// Return current video time in seconds
		public float GetSeek()
		{
			return (float)videoPlayer.time;
		}

		public void SeekTo(float timeInSeconds)
		{
			videoPlayer.time = timeInSeconds;
		}

	}
}