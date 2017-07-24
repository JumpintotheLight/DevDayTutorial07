using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.IO;

public class PlayVideo : MonoBehaviour {
	//Raw Image to Show Video Images [Assign from the Editor]
	//public RawImage image;
	public Renderer target;

	public RenderTexture renderTex;
	//Video To Play [Assign from the Editor]
	public string videoFileName;
	public bool loop = false;

	private VideoPlayer videoPlayer;
	private VideoSource videoSource;

	//Audio
	private AudioSource audioSource;

	// Use this for initialization
	void Start()
	{
		Application.runInBackground = true;
		StartCoroutine(PlayFromStreamingAssets(videoFileName));
	}

	IEnumerator PlayFromStreamingAssets(string mediaFileName)
	{
		string streamingMediaPath = Application.streamingAssetsPath + "/" + mediaFileName;
		string persistentPath =  Application.persistentDataPath + "/" + Path.GetFileName(mediaFileName);

		#if !UNITY_EDITOR && !UNITY_STANDALONE
			Debug.Log(" streaming Media Path: " + streamingMediaPath);
			if (!File.Exists(persistentPath))
			{
				WWW wwwReader = new WWW(streamingMediaPath);
				yield return wwwReader;

				if (wwwReader.error != null) Debug.LogError("wwwReader error: " + wwwReader.error);

				System.IO.File.WriteAllBytes(persistentPath, wwwReader.bytes);
				Debug.Log("saved video file from streaming assets: " + persistentPath);
			}
		#else
		persistentPath = streamingMediaPath;
		#endif

		// Video must start only after mediaFullPath is filled in
		StartCoroutine(playVideo("file://" + persistentPath));
		yield return null;
	}

	IEnumerator playVideo(string url)
	{
		//Add VideoPlayer to the GameObject
		videoPlayer = gameObject.AddComponent<VideoPlayer>();

		//Add AudioSource
		audioSource = gameObject.AddComponent<AudioSource>();

		videoPlayer.playOnAwake = false;
		videoPlayer.isLooping = loop;
		audioSource.playOnAwake = false;

		videoPlayer.errorReceived += VideoPlayer_errorReceived;

		videoPlayer.source = VideoSource.Url;
		videoPlayer.url = url;
		//videoPlayer.targetCamera = Camera.main;
		//videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
		videoPlayer.renderMode = VideoRenderMode.RenderTexture;
		videoPlayer.targetTexture = renderTex;

		//videoPlayer.targetMaterialRenderer = target;

		//videoPlayer.targetMaterialProperty = "_MainTex";

		//videoPlayer.frame = 100;

		videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
		videoPlayer.EnableAudioTrack(0, true);
		videoPlayer.SetTargetAudioSource(0, audioSource);

		//Assign the Texture from Video to RawImage to be displayed
		//image.texture = videoPlayer.texture;

		//Set video To Play then prepare Audio to prevent Buffering
		videoPlayer.Prepare();

		//Wait until video is prepared
		WaitForSeconds waitTime = new WaitForSeconds(1);
//		while (!videoPlayer.isPrepared)
//		{
//			Debug.Log("Preparing Video");
//			yield return waitTime;
//		}

		yield return waitTime;

		Debug.Log("Done Preparing Video");

		videoPlayer.aspectRatio = VideoAspectRatio.Stretch;

		//Play Video
		videoPlayer.Play();

		//Play Sound
		audioSource.Play();

		Debug.Log("Playing Video");
		while (videoPlayer.isPlaying)
		{
			Debug.Log("Video Time: " + videoPlayer.time);
			yield return null;
		}

		Debug.Log("Done Playing Video");
	}

	void VideoPlayer_errorReceived (VideoPlayer source, string message)
	{
		Debug.Log ("error: " + message);
	}
}
